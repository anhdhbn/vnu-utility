using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Utility
{
    public static class Utility
    {
        public static List<string> GenerateAccount(string CodeVnu, int skip = 0)
        {
            List<string> ListAccount = new List<string>();
            int index = DateTime.Now.Year;
            if (DateTime.Now.Month < 10) index--;
            for (int i = 0; i < 5 - skip; i++)
            {
                int head = index - i;
                for (int j = 1; j < 2000; j++)
                {
                    string headStr = head.ToString().Remove(0, 2) + CodeVnu;
                    string acc = GetMssv(j, headStr);
                    ListAccount.Add(acc);
                }
            }
            return ListAccount;
        }

        private static string GetMssv(int i, string head)
        {
            if (i < 10) return head + "000" + i.ToString();
            else if (i < 100) return head + "00" + i.ToString();
            else if (i < 1000) return head + "0" + i.ToString();
            else return head + i.ToString();
        }

        public static List<EntityCredit> ParseDataFromString(string html)
        {
            List<EntityCredit> entities = new List<EntityCredit>();
            MatchCollection mc2 = Regex.Matches(html, "tr class=(.*?)>.*?<td.*?>(.*?)</td>.*?<td.*?>(.*?)</td>.*?<td.*?>(.*?)</td>.*?<td.*?>(.*?)</td>.*?<td.*?>(.*?)</td>.*?<td.*?>(.*?)</td>.*?<td.*?>(.*?)</td>.*?<td.*?>(.*?)</td>.*?<td.*?>(.*?)</td>.*?<td.*?>(.*?)</td>.*?(.*?)/tr", RegexOptions.Singleline);
            foreach (Match item in mc2)
            {
                string Title = item.Groups[1].Value.Replace("\"", "");
                string Data = item.Groups[2].Value.Trim();

                string TenMonHoc = item.Groups[3].Value.Trim();
                TenMonHoc = RemoveSpace(TenMonHoc);
                //string SoTinChi = item.Groups[4].Value.Trim();
                string MaHocPhan = item.Groups[5].Value.Trim();
                MaHocPhan = RemoveSpace(MaHocPhan);
                //string MaxSv = item.Groups[7].Value.Trim();
                //string CurrentSv = item.Groups[8].Value.Trim();
                //string GiangVien = item.Groups[9].Value.Trim();
                //string HocPhi = item.Groups[10].Value.Trim().Replace("<span>", "").Replace("</span>", "");
                string lichhoc = item.Groups[7].Value.Trim();
                string time = Regex.Match(lichhoc, @"T\d-.*").Value;
                string data = item.Groups[11].Value.Trim();
                string dataRemove = Regex.Match(data, @"data-rowindex=""(\d{1,2})""").Groups[1].Value;
                //LichHoc = LichHoc.Replace(Regex.Match(LichHoc, "<input.*?>").Value, "").Trim();
                if (Data != "")
                {
                    Match m = Regex.Match(Data, @"data-rowindex=""(.*?)"" data-crdid=""(.*?)""");
                    string DataRowIndex = m.Groups[1].Value;
                    string DataCrdid = m.Groups[2].Value;
                    entities.Add(new EntityCredit
                    {
                        MaHocPhan = MaHocPhan.Trim(),
                        TenMonHoc = TenMonHoc.Trim(),
                        Title = Title,
                        DataCrdid = DataCrdid,
                        DataRowIndex = DataRowIndex,
                        DataRemove = dataRemove
                    });
                }
                else
                {
                    entities.Add(new EntityCredit
                    {
                        MaHocPhan = MaHocPhan.Trim(),
                        TenMonHoc = TenMonHoc.Trim(),
                        Title = Title,
                        DataCrdid = "",
                        DataRowIndex = "",
                        DataRemove = dataRemove
                    });
                }
            }
            return entities;
        }

        public static string RemoveSpace(string data)
        {
            data = data.Replace("  ", "").Replace("(<b>TH</b>)", "").Trim().Replace("<span>(<b>LT</b>)</span>", "");
            return data;
        }
    }

    public class EntityCredit
    {
        public string DataRowIndex { get; set; }
        public string DataCrdid { get; set; }
        public string TenMonHoc { get; set; }
        public string MaHocPhan { get; set; }
        public string Title { get; set; }
        public string DataRemove { get; set; }
    }
}
