using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Filter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class CorsAttribute : ActionFilterAttribute, IActionFilter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="AllowOriginsPattern"></param>
        public CorsAttribute(string allowOriginsPattern = "")
        {
            if (string.IsNullOrEmpty(allowOriginsPattern))
            {
                this.AllowOriginsPattern = ConfigurationManager.AppSettings["AllowOriginsPattern"].ToString();
            }
            else
            {
                this.AllowOriginsPattern = allowOriginsPattern;
            }
        }

        /// <summary>
        /// 访问完成，追加head
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            try
            {
                base.OnResultExecuted(filterContext);
                GetResponse();
            }
            catch (Exception exception)
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Write(exception.Message);
                HttpContext.Current.Response.End();
            }
        }


        /// <summary>
        /// 允许的正则表达式
        /// </summary>
        public string AllowOriginsPattern { get; set; }

        /// <summary>
        /// 获取response
        /// </summary>
        /// <returns></returns>
        public HttpResponse GetResponse()
        {
            HttpRequest request = HttpContext.Current.Request;
            IDictionary<string, string> headers;
            bool IsEvaluate = TryEvaluate(HttpContext.Current.Request, out headers);
            if (IsEvaluate)
            {
                foreach (var item in headers)
                {
                    HttpContext.Current.Response.Headers.Add(item.Key, item.Value);
                }
            }
            return HttpContext.Current.Response;
        }

        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool TryEvaluate(HttpRequest request, out IDictionary<string, string> headers)
        {
            headers = null;
            if (request.Headers.GetValues("Origin") != null)
            {
                string origin = request.Headers.GetValues("Origin").First();
                Regex regex = new Regex(AllowOriginsPattern, RegexOptions.IgnoreCase);
                if (regex.IsMatch(origin))//匹配正则
                {
                    headers = this.GenerateResponseHeaders(request);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 生成head
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private IDictionary<string, string> GenerateResponseHeaders(HttpRequest request)
        {
            string origin = request.Headers.GetValues("Origin").First();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Access-Control-Allow-Origin", origin);
            headers.Add("Access-Control-Allow-Headers", "x-requested-with,content-type，requesttype,Token");
            headers.Add("Access-Control-Allow-Methods", "POST,GET");
            return headers;
        }

    }
}