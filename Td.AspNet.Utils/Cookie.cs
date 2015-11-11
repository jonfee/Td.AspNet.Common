﻿using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Td.AspNet.Utils
{
    public class Cookie
    {
        public static IDictionary<string, string> Get(HttpContext context, string name)
        {
            var cookies = (RequestCookiesCollection)context.Request.Cookies;
            var content = cookies.Get(name);
            return CookieDic(content);
        }

        public static void Save(HttpContext context, string name, IDictionary<string, string> values)
        {
            context.Response.Cookies.Append(name, BuildCookieValue(values));
        }

        public static void Save(HttpContext context, string name, IDictionary<string, string> values, CookieOptions options)
        {
            context.Response.Cookies.Append(name, BuildCookieValue(values), options);
        }

        public static void Remove(HttpContext context, string name)
        {
            var cookies = (ResponseCookies)context.Response.Cookies;
            CookieOptions options = new CookieOptions { Expires = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
            cookies.Delete(name, options);
        }

        public static string BuildCookieValue(IDictionary<string, string> parameters)
        {

            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");
                    postData.Append(value);//在没有找到asp.net 5中如何URL编码之前直接输出
                    //postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

        private static IDictionary<string, string> CookieDic(string cookievalue)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(cookievalue))
            {
                string[] pairs = cookievalue.Split(new char[] { '&' });
                if (pairs != null && pairs.Length > 0)
                {
                    foreach (string pair in pairs)
                    {
                        string[] oneParam = pair.Split(new char[] { '=' }, 2);
                        if (oneParam != null && oneParam.Length == 2)
                        {
                            result.Add(oneParam[0], oneParam[1]);
                        }
                    }
                }
            }
            return result;
        }
    }
}
