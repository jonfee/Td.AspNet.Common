namespace Td.AspNet.Upload
{
    /// <summary>
    /// 文件保存后输出类型枚举
    /// </summary>
    public enum SaveAfterOutputType
    {
        /// <summary>
        /// 以文件路径输出，如：/upload/product/myproduct.jpg
        /// </summary>
        FilePath = 1,
        /// <summary>
        /// 以Base64编码输出，如：image/gif,5uiafasfda96s0afa21s5af9sdfasdg7a0sdabcxg==
        /// </summary>
        Base64
    }
}
