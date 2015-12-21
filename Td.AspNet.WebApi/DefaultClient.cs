using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Td.AspNet.Utils;

namespace Td.AspNet.WebApi
{
    public class DefaultClient
    {
        /// <summary>
        /// 通过GET方式调用API
        /// </summary>
        /// <param name="url">api url</param>
        /// <param name="parameters">参数字典</param>
        /// <param name="partnerId">合作者或模块ID</param>
        /// <param name="secretKey">加密参数</param>
        /// <returns></returns>
        public static async Task<string> DoGet(string url, IDictionary<string, string> parameters, string partnerId, string secretKey)
        {
            IDictionary<string, string> txtParams;
            if (parameters != null)
            {
                txtParams = new Dictionary<string, string>(parameters);
            }
            else
            {
                txtParams = new Dictionary<string, string>();
            }

            txtParams.Add("PartnerId", partnerId);
            txtParams.Add("Timestamp", DateTime.Now.ToUniversalTime().Ticks.ToString());//时间戳以时间周期数表示
            txtParams.Add("Sign", Strings.SignRequest(txtParams, secretKey));

            url = Strings.BuildGetUrl(url, txtParams);
            var txt = string.Empty;
            HttpClient client = new HttpClient();
            using (client = new HttpClient())
            {
                try
                {
                    //await异步等待回应
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        txt = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        txt = ErrorInfo(response.StatusCode.ToString(), response.RequestMessage.ToString());
                    }
                }
                catch (Exception ex)
                {
                    txt = ErrorInfo(ex.Message, ex.InnerException.Message);
                }
            }
            return txt;
        }

        /// <summary>
        /// 通过Post方式调用WebApi,不包含文件上传
        /// </summary>
        /// <param name="url">api url</param>
        /// <param name="parameters">表单数据字典</param>
        /// <param name="partnerid">合作者或模块ID</param>
        /// <param name="secretKey">加密参数</param>
        /// <returns></returns>
        public static async Task<string> DoPost(string url, IDictionary<string, string> parameters, string partnerid, string secretKey)
        {
            if (parameters == null)
            {
                return ErrorInfo("提交数据异常", "POST表单数据值不存在");
            }

            IDictionary<string, string> txtParams = new Dictionary<string, string>(parameters);

            IDictionary<string, string> urlParams = new Dictionary<string, string>();
            urlParams.Add("PartnerId", partnerid);
            urlParams.Add("Timestamp", DateTime.Now.ToUniversalTime().Ticks.ToString());//时间戳以时间周期数表示

            foreach (var item in urlParams)
            {
                txtParams.Add(item.Key, item.Value);
            }

            var sign = Strings.SignRequest(txtParams, secretKey);
            urlParams.Add("Sign", sign);

            url = Strings.BuildGetUrl(url, urlParams);

            HttpContent content = new FormUrlEncodedContent(parameters);

            HttpClient client = new HttpClient();
            //指定返回XML格式
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            //指定返回JSON
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json")); 
            var txt = string.Empty;
            using (client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        txt = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        txt = ErrorInfo(response.StatusCode.ToString(), response.RequestMessage.ToString());
                    }
                }
                catch (Exception ex)
                {
                    txt = ErrorInfo(ex.Message, ex.InnerException.Message);
                }
            }
            return txt;
        }

        /// <summary>
        /// 通过Post方式调用WebApi,包含文件上传
        /// </summary>
        /// <param name="url">api url</param>
        /// <param name="parameters">表单数据字典</param>
        /// <param name="files">表单中要上传的文件</param>
        /// <param name="partnerid">合作者或模块ID</param>
        /// <param name="secretKey">加密参数</param>
        /// <returns></returns>
        public static async Task<string> DoPost(string url, IDictionary<string, string> parameters, List<FileContent> files, string partnerid, string secretKey)
        {
            if (parameters == null && files.Count == 0)
            {
                return ErrorInfo("提交数据异常", "POST表单数据值或上传的文件对象同时不存在");

            }
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }
            IDictionary<string, string> txtParams = new Dictionary<string, string>(parameters);

            TimeSpan timeSpan = new TimeSpan(DateTime.UtcNow.Ticks);
            IDictionary<string, string> urlParams = new Dictionary<string, string>();
            urlParams.Add("PartnerId", partnerid);
            urlParams.Add("Timestamp", DateTime.Now.ToUniversalTime().Ticks.ToString());//时间戳以时间周期数表示

            foreach (var item in urlParams)
            {
                txtParams.Add(item.Key, item.Value);
            }

            var sign = Strings.SignRequest(txtParams, secretKey);
            urlParams.Add("Sign", sign);

            url = Strings.BuildGetUrl(url, urlParams);

            var content = new MultipartFormDataContent();
            foreach (var item in parameters)
            {
                content.Add(new StringContent(item.Value), item.Key);
            }

            foreach (var file in files)
            {
                StreamContent fileContent = new StreamContent(file.FileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                fileContent.Headers.ContentDisposition.FileName = file.FileName;
                fileContent.Headers.ContentDisposition.Name = file.FieldName;
                fileContent.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("ContentType", file.ContentType.Replace("/", "-")));

                content.Add(fileContent);
            }

            HttpClient client = new HttpClient();
            //指定返回XML格式
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            //指定返回JSON
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json")); 
            var txt = string.Empty;
            using (client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        txt = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        txt = ErrorInfo(response.StatusCode.ToString(), response.RequestMessage.ToString());
                    }
                }
                catch (Exception ex)
                {
                    txt = ErrorInfo(ex.Message, ex.InnerException.Message);
                }
            }
            return txt;
        }

        public static string ErrorInfo(string message, string content)
        {
            ErrorMessage error = new ErrorMessage();
            error.Code = 100;
            error.Message = string.Format("错误信息：{0}", message);
            error.Content = string.Format("具体错误：{0}", content);
            return Newtonsoft.Json.JsonConvert.SerializeObject(error);
        }
    }
}
