using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Td.AspNet.Utils
{
    public class Strings
    {
        /// <summary>
        /// 密码加密，三次加密，加密顺序：MD5，SHA1，MD5
        /// </summary>
        /// <param name="text">需要加密的密码</param>
        /// <returns></returns>
        public static string PasswordEncrypt(string text)
        {
            return Cryptography.MD5Encrypt(Cryptography.SHA1Encrypt(Cryptography.MD5Encrypt(text)));
        }

        /// <summary>
        /// 将全角数字转换为数字
        /// </summary>
        /// <param name="SBCCase"></param>
        /// <returns></returns>
        public static string SBCCaseToNumberic(string SBCCase)
        {
            char[] c = SBCCase.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new string(c);
        }

        /// <summary>
        /// 建立密文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CreateToken(string text)
        {
            return Cryptography.MD5Encrypt("hibruin" + text + "@outlook" + "#lite");
        }

        /// <summary>
        /// 将字段key-value形式数据组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildUrlQuery(IDictionary<string, string> parameters)
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
                    //postData.Append(value);//在没有找到asp.net 5中如何URL编码之前直接输出
                    postData.Append(WebUtility.UrlEncode(value));
                    //postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

        /// <summary>
        /// 组装GET请求URL。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>带参数的GET请求URL</returns>
        public static string BuildGetUrl(string url, IDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildUrlQuery(parameters);
                }
                else
                {
                    url = url + "?" + BuildUrlQuery(parameters);
                }
            }
            return url;
        }

        /// <summary>
        /// 对URL参数进行切割，并拼装成字典
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IDictionary<string, string> SplitUrlQuery(string query)
        {
            query = query.Trim(new char[] { '?', ' ' });
            if (query.Length == 0)
            {
                return new Dictionary<string, string>();
            }
            IDictionary<string, string> result = new Dictionary<string, string>();

            string[] pairs = query.Split(new char[] { '&' });
            if (pairs != null && pairs.Length > 0)
            {
                foreach (string pair in pairs)
                {
                    string[] oneParam = pair.Split(new char[] { '=' }, 2);
                    if (oneParam != null && oneParam.Length == 2)
                    {
                        result.Add(oneParam[0], WebUtility.UrlDecode(oneParam[1]));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 使用MD5加密进行签名
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string SignRequest(IDictionary<string, string> parameters, string secret)
        {
            ///把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            var query = new StringBuilder(secret);
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                }
            }
            //MD5 md5 = MD5.Create();
            //byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));

            //// 第四步：把二进制转化为大写的十六进制
            //StringBuilder result = new StringBuilder();
            //for (int i = 0; i < bytes.Length; i++)
            //{
            //    result.Append(bytes[i].ToString("X2"));
            //}

            //return result.ToString();
            return Cryptography.MD5Encrypt(query.ToString());
        }
    }
}
