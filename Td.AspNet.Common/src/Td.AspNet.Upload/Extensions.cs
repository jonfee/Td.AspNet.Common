using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Td.AspNet.Upload
{
    /// <summary>
    /// 文件上传扩展类
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="context">上传文件上下文</param>
        /// <returns></returns>
        public static async Task<UploadResult> Save(this UploadContext context)
        {
            UploadResult result = null;

            if (null != context && context.FormFile.Length > 0 && !string.IsNullOrWhiteSpace(context.WebRootPath))
            {
                var file = context.FormFile;

                //文件上传的绝对文件夹目录，如：
                var uploadFullFolder = context.WebRootPath;

                //文件相对站点的路径，默认为文件名，如：aa.jpg
                string relFilePath = context.UploadName;

                if (!string.IsNullOrWhiteSpace(context.UploadFolder))
                {
                    //更新最终上传文件的目录路径，如：D:\\WebSite\\wwwroot\\upload\\photo
                    uploadFullFolder = Path.Combine(context.WebRootPath, context.UploadFolder);

                    //更新文件相对站点的路径，如：\\upload\\photo\\aa.jpg
                    relFilePath = Path.Combine(context.UploadFolder, context.UploadName);
                }

                //文件夹不存在,则创建
                if (!Directory.Exists(context.UploadFolder))
                {
                    Directory.CreateDirectory(context.UploadFolder);
                }

                //文件在磁盘中的路径，如：D:\\WebSite\\wwwroot\\upload\\photo\\aa.jpg
                string absFileName = Path.Combine(context.WebRootPath, relFilePath);

                //存在同名文件且需要覆盖时删除原文件
                if (File.Exists(absFileName) && context.BeOverride)
                {
                    File.Delete(absFileName);
                }

                using (FileStream fs = File.Create(absFileName))
                {
                    await context.FormFile.CopyToAsync(fs);
                    fs.Flush();
                }
                //上传
                //await context.FormFile.CopyToAsync(new FileStream(absFileName, FileMode.Create));

                result = new UploadResult
                {
                    FieldName = context.FiledName,
                    FileName = context.UploadName,
                    FilePath = relFilePath.Replace(@"\", @"/"),
                    FileSize = file.Length,
                    ContentType = context.ContentType,
                    IsImage = context.IsImage,
                    OriginalFileName = context.OriginalName
                };
            }

            return result;
        }

        /// <summary>
        /// 保存文件队列
        /// </summary>
        /// <param name="queueContext">上传文件队列</param>
        /// <returns></returns>
        public static async Task<List<UploadResult>> Save(this IEnumerator<UploadContext> queueContext)
        {
            List<UploadResult> list = new List<UploadResult>();

            while (queueContext.MoveNext())
            {
                var result = await queueContext.Current.Save();

                if (null != result) list.Add(result);
            }

            return list;
        }

        /// <summary>
        /// 保存文件队列
        /// </summary>
        /// <param name="queueContext">上传文件队列</param>
        /// <returns></returns>
        public static async Task<List<UploadResult>> Save(this IEnumerable<UploadContext> queueContext)
        {
            List<UploadResult> list = new List<UploadResult>();

            if (null != queueContext && queueContext.Count() > 0)
            {
                foreach (var context in queueContext)
                {
                    var result = await context.Save();

                    if (null != result) list.Add(result);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取上传文件的路径集合，用指定分隔符分隔
        /// </summary>
        /// <param name="result"></param>
        /// <param name="splitChar">分隔符</param>
        /// <returns></returns>
        public static string SplitToString(this IEnumerable<UploadResult> result, char splitChar = ',')
        {
            string paths = string.Empty;

            if (null != result && result.Count() > 0)
            {
                foreach (var file in result)
                {
                    paths += splitChar + file.FilePath;
                }

                if (paths.StartsWith(splitChar.ToString())) paths = paths.TrimStart(splitChar);
            }

            return paths;
        }
    }
}
