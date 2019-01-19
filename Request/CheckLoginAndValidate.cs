using Chilkat;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Request
{
    public class CheckLoginAndValidate : RequestBase
    {
        public string User { get; set; }
        public string Pass { get; set; }
        public bool IsSuccess { get; private set; }
        private int LoginType { get; set; }
        public CheckLoginAndValidate()
        {
            Host = "dangkyhoc.vnu.edu.vn";
            port = 80;
            ssl = false;
            LoginType = 1;
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
                return CheckValidate();
            }
            return Exit();
        }

        private Action CheckValidate()
        {
            string str = http.QuickGetStr($"http://{Host}/dang-ky-mon-hoc-nganh-1");
            str = WebUtility.HtmlDecode(str);
            if(str.Contains("Ghi nhận"))
            {
                return CheckNumberCredit();
            }
            return Exit();
        }

        private Action CheckNumberCredit()
        {
            HttpRequest request = CreateRequest("/danh-sach-mon-hoc-da-dang-ky/1", Method.POST, PostType.form, null);
            HttpResponse result = ExcuteRequest(request);
            if(result.BodyStr.Contains("Bạn chưa đăng ký môn học nào!"))
            {
                return Done();
            }
            return Exit();
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
