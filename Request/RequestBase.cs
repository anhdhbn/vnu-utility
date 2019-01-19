using Chilkat;
using System;
using System.Collections.Generic;

namespace Request
{
    public static class LinkVnu
    {
        public static string LinkLogin = "/dang-nhap";
        public static string LinkListCredit = "/danh-sach-mon-hoc/{0}/2";
        public static string LinkListCreditRegistered = "/danh-sach-mon-hoc-da-dang-ky/{0}";
        public static string LinkConfirmCredit = "/xac-nhan-dang-ky/{0}";
        public static string ChooseCredit = "/chon-mon-hoc/{0}/1/2";
        public static string GetLink(string format, int typeLogin = -1)
        {
            if (typeLogin == -1) return string.Format(format);
            return string.Format(format, typeLogin);
        }

        public static string GetChooseCredit(string dataRowIndex)
        {
            return string.Format(ChooseCredit, dataRowIndex);
        }
    }
    public class RequestBase : IDisposable
    {
        private int Timeout = 5 * 60;
        protected Http http;
        protected string Host { get; set; }
        protected int port { get; set; }
        protected bool ssl { get; set; }
        public RequestBase()
        {
            http = new Http();
            http.UnlockComponent("DkMEz0.CB_1199CvlR1BXM");
            http.CookieDir = "memory";
            http.SaveCookies = true;
            http.SendCookies = true;
            http.ConnectTimeout = Timeout;
            http.ReadTimeout = Timeout;
            http.HeartbeatMs = 1000;
        }

        protected HttpResponse ExcuteRequest(HttpRequest request)
        {
            HttpResponse response = http.SynchronousRequest(Host, 80, ssl, request);
            return response;
        }

        protected HttpRequest CreateRequest(string path, Method method = Method.GET, PostType postType = PostType.json, Dictionary<string, string> parameters = null)
        {
            HttpRequest request = new HttpRequest();
            request.Path = path;
            if (method == Method.POST)
            {
                request.HttpVerb = "POST";
                request.AddHeader("Content-Type", GetTypePost(postType));
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        request.AddParam(param.Key, param.Value);
                    }
                }
            }
            else request.HttpVerb = "GET";
            return request;
        }

        protected string GetTypePost(PostType postType)
        {
            if (postType == PostType.form) return "application/x-www-form-urlencoded";
            if (postType == PostType.json) return "application/json";
            return string.Empty;
        }

        public void Dispose()
        {
            http.Dispose();
        }

        public enum Method
        {
            GET,
            POST
        }

        public enum PostType
        {
            form = 0,
            json = 1
        }

        protected void Empty() { }
    }
}
