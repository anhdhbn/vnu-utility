using Chilkat;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Utility;

namespace Request
{
    public class HuyMonHoc : RequestBase
    {
        public string User { get; set; }
        public string Pass { get; set; }
        public bool IsSuccess { get; private set; }
        private int LoginType { get; set; }
        private List<string> ListDataDelete { get; set; }
        public HuyMonHoc()
        {
            Host = "dangkyhoc.vnu.edu.vn";
            port = 80;
            ssl = false;
            LoginType = 1;
            ListDataDelete = new List<string>();
        }

        public Action Begin()
        {
            return Login();
        }

        private Action Login()
        {
            string html = QuickGetRequest(LinkVnu.GetLink(LinkVnu.LinkLogin));
            if (html == null)
            {
                return Login();
            }
            string token = Regex.Match(html, @"__RequestVerificationToken.*ue=""(.*?)""").Groups[1].Value;
            if (string.IsNullOrEmpty(token))
            {
                return Login();
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("__RequestVerificationToken", token);
            dic.Add("LoginName", User);
            dic.Add("Password", Pass);

            HttpRequest request = CreateRequest(LinkVnu.GetLink(LinkVnu.LinkLogin), Method.POST, PostType.form, dic);
            HttpResponse result = ExcuteRequest(request);
            request.Dispose();
            if (result == null) return Login();
            string htmlResult = WebUtility.HtmlDecode(result.BodyStr);
            request.Dispose();
            dic.Clear();
            //dic.TrimExcess();
            if (htmlResult.Contains("Chào mừng"))
            {
                return DanhSachMonHoc();
            }
            return Exit();
        }

        private Action DanhSachMonHoc()
        {
            HttpRequest request = CreateRequest("/danh-sach-mon-hoc/1/1", Method.POST, PostType.form);
            HttpResponse result = ExcuteRequest(request);
            return DanhSachMonHocDaDangKy();
        }

        private Action DanhSachMonHocDaDangKy()
        {
            HttpRequest request = CreateRequest("/danh-sach-mon-hoc-da-dang-ky/1", Method.POST, PostType.form);
            HttpResponse result = ExcuteRequest(request);
            List<EntityCredit> entityCredits = Utility.Utility.ParseDataFromString(result.BodyStr);
            foreach (var entity in entityCredits)
            {
                ListDataDelete.Add(entity.DataRemove);
            }
            return HuyMon();
        }

        private Action HuyMon()
        {
            if (ListDataDelete.Count == 0) return Done();
            foreach (var delete in ListDataDelete)
            {
                HttpRequest request = CreateRequest($"/huy-mon-hoc/{delete}/1/1", Method.POST, PostType.form);
                HttpResponse result = ExcuteRequest(request);
            }
            return Confirm();
        }

        private Action Confirm()
        {
            HttpRequest request = CreateRequest($"/xac-nhan-dang-ky/1", Method.POST, PostType.form);
            HttpResponse result = ExcuteRequest(request);
            return Done();
        }

        private Action Done()
        {
            IsSuccess = true;
            Console.WriteLine($"{DateTime.Now} {User}");
            return Empty;
        }

        private Action Exit()
        {
            return Empty;
        }

        private string QuickGetRequest(string sublink)
        {
            var link = $"http://{Host}{sublink}";
            return http.QuickGetStr(link);
        }
    }
}
