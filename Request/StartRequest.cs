using Chilkat;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Request
{
    public class StartRequest :  RequestBase
    {

        public string User { get; set; }
        public string Pass { get; set; }
        public int LoginType { get; set; }
        public List<string> ListDataRowIndex { get; set; }
        public bool IsSuccess { get; private set; }
        private bool IsRunning { get; set; }
        private Semaphore semaphore = new Semaphore(1, 1);

        public StartRequest()
        {
            Host = "dangkyhoc.vnu.edu.vn";
            port = 80;
            ssl = false;
        }


        public Action Login()
        {
            try
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
                if (htmlResult.Contains("Chào mừng"))
                {
                    return DanhSachMonHoc2();
                    //string str = http.QuickGetStr($"http://{Host}/dang-ky-mon-hoc-nganh-1");
                    //str = WebUtility.HtmlDecode(str);
                    //if (str.Contains("Ghi nhận"))
                    //    return Done();
                    //else return Exit();
                }
                return Exit();
            }
            catch
            {
                return Login();
            }
        }

        private Action DanhSachMonHoc2()
        {
            string link = LinkVnu.GetLink(LinkVnu.LinkListCredit, LoginType);
            HttpRequest request = CreateRequest(link, Method.POST, PostType.form, null);
            HttpResponse result = ExcuteRequest(request);
            request.Dispose();
            if (result == null)
                return Login();
            return DangkyMonHoc();
        }

        private Action DangkyMonHoc()
        {
            if (ListDataRowIndex.Count == 0)
            {
                return Done();
            }
            List<Thread> threads = new List<Thread>();
            foreach (var DataRowIndex in ListDataRowIndex)
            {
                Thread thread = new Thread(() =>
                {
                    var temp = DataRowIndex;
                    bool flag = Dangky(temp);
                    if (flag == false)
                        ReCall();
                });
                thread.Start();
            }

            while (true)
            {
                bool flag = true;
                foreach (var thread in threads)
                {
                    if (thread.IsAlive) flag = false;
                }
                Thread.Sleep(1000);
                if (flag) break;
            }
            //if (!XacNhanDangKi())
            //    return Login();
            return DangkyMonHoc();
        }

        private void ReCall()
        {
            semaphore.WaitOne();
            if (this.IsRunning == false)
            {
                IsRunning = true;
                Login();

                IsRunning = false;
            }
            semaphore.Release();
        }

        private bool Dangky(string DataRowIndex)
        {
            HttpRequest request = CreateRequest(LinkVnu.GetChooseCredit(DataRowIndex), Method.POST, PostType.form);
            HttpResponse result = ExcuteRequest(request);
            request.Dispose();
            if (result == null)
                return false;
            if (result.BodyStr.Contains("true"))
            {
                request.Dispose();
                result.Dispose();
                XacNhanDangKi();
                return true;
            }
            request.Dispose();
            result.Dispose();
            return false;
        }

        private bool XacNhanDangKi()
        {
            HttpRequest request2 = CreateRequest(LinkVnu.GetLink(LinkVnu.LinkConfirmCredit, LoginType), Method.POST, PostType.form);
            HttpResponse result2 = ExcuteRequest(request2);
            if (result2 == null)
                return false;
            if (result2.BodyStr.Contains("Đăng ký thành công"))
                return true;
            return false;
        }

        private string QuickGetRequest(string sublink)
        {
            var link = $"http://{Host}{sublink}";
            return http.QuickGetStr(link);
        }

        private Action Done()
        {
            Console.WriteLine($"{DateTime.Now} {User}");
            return Empty;
        }

        private Action Exit()
        {
            return Empty;
        }
    }
}
