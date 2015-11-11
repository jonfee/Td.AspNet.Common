namespace Td.AspNet.WebApi
{
    public class ErrorMessage
    {
        /// <summary>
        /// 消息代码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 提示
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 详细内容
        /// </summary>
        public string Content { get; set; }
    }
}
