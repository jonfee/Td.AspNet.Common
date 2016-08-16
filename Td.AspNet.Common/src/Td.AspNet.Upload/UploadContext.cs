using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Td.AspNet.Upload
{
    /// <summary>
    /// 上传的文件内容
    /// </summary>
    public class UploadContext
    {
        #region 单个文件上传实例

        /// <summary>
        /// 实始化文件上传内容实例（自动随机生成文件名称）
        /// </summary
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="file">待上传的文件</param>
        /// <param name="filter">允许上传的文件格式（多个用"|"分隔），如：jpg|gif|doc</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        public UploadContext(string apiAddress, IFormFile file, string filter, long maxSize, string uploadFolder, bool isFixedPath)
            : this(apiAddress, file, filter, maxSize, uploadFolder, isFixedPath, null) { }

        /// <summary>
        /// 实始化文件上传内容实例（存在同名文件时不覆盖）
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="file">待上传的文件</param>
        /// <param name="filter">允许上传的文件格式（多个用"|"分隔），如：jpg|gif|doc</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        public UploadContext(string apiAddress, IFormFile file, string filter, long maxSize, string uploadFolder, bool isFixedPath, string uploadName)
            : this(apiAddress, file, filter, maxSize, uploadFolder, isFixedPath, uploadName, null) { }

        /// <summary>
        /// 实始化文件上传内容实例
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="file">待上传的文件</param>
        /// <param name="filter">允许上传的文件格式（多个用"|"分隔），如：jpg|gif|doc</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        /// <param name="extensionName">扩展名（如：jpg）</param>
        public UploadContext(string apiAddress, IFormFile file, string filter, long maxSize, string uploadFolder, bool isFixedPath, string uploadName, string extensionName)
            : this(apiAddress, file, filter, maxSize, uploadFolder, isFixedPath, uploadName, extensionName, false) { }

        /// <summary>
        /// 实始化文件上传内容实例
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="file">待上传的文件</param>
        /// <param name="filter">允许上传的文件格式（多个用"|"分隔），如：jpg|gif|doc</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        /// <param name="extensionName">扩展名（如：jpg）</param>
        /// <param name="beOverride">存在同名文件时是否覆盖</param>
        /// <param name="compressIfGreaterSize">超过该大小时进行质量压缩（单位：KB）,该参数只对图片上传有效</param>
        /// <param name="maxWidth">图片最大宽（超出时裁剪/压缩）</param>
        /// <param name="maxHeight">图片最大高（超出时裁剪/压缩）</param>
        /// <param name="cutIfOut">超出图片限制的宽/高时是否裁剪（为false时不足部分留白）</param>
        public UploadContext(string apiAddress, IFormFile file, string filter, long maxSize, string uploadFolder, bool isFixedPath, string uploadName = null, string extensionName = null, bool beOverride = false, long compressIfGreaterSize = 0, int maxWidth = 0, int maxHeight = 0, bool cutIfOut = false)
            : this(apiAddress, new IFormFile[] { file }, filter, maxSize, uploadFolder, isFixedPath, uploadName, extensionName, beOverride, compressIfGreaterSize, maxWidth, maxHeight, cutIfOut) { }

        #endregion

        #region 多文件上传实例

        /// <summary>
        /// 实始化文件上传内容实例（自动随机生成文件名称）
        /// </summary
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="files">待上传的文件</param>
        /// <param name="filter">允许上传的文件格式（多个用"|"分隔），如：jpg|gif|doc</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        public UploadContext(string apiAddress, IEnumerable<IFormFile> files, string filter, long maxSize, string uploadFolder, bool isFixedPath)
            : this(apiAddress, files, filter, maxSize, uploadFolder, isFixedPath, null) { }

        /// <summary>
        /// 实始化文件上传内容实例（存在同名文件时不覆盖）
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="files">待上传的文件</param>
        /// <param name="filter">允许上传的文件格式（多个用"|"分隔），如：jpg|gif|doc</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        public UploadContext(string apiAddress, IEnumerable<IFormFile> files, string filter, long maxSize, string uploadFolder, bool isFixedPath, string uploadName)
            : this(apiAddress, files, filter, maxSize, uploadFolder, isFixedPath, uploadName, null) { }

        /// <summary>
        /// 实始化文件上传内容实例
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="files">待上传的文件</param>
        /// <param name="filter">允许上传的文件格式（多个用"|"分隔），如：jpg|gif|doc</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        /// <param name="extensionName">扩展名（如：jpg）</param>
        public UploadContext(string apiAddress, IEnumerable<IFormFile> files, string filter, long maxSize, string uploadFolder, bool isFixedPath, string uploadName, string extensionName)
            : this(apiAddress, files, filter, maxSize, uploadFolder, isFixedPath, uploadName, extensionName, false, 0, 0, 0, false) { }

        /// <summary>
        /// 实始化文件上传内容实例
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="files">待上传的文件</param>
        /// <param name="filter">允许上传的文件格式（多个用"|"分隔），如：jpg|gif|doc</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        /// <param name="extensionName">扩展名（如：jpg）</param>
        /// <param name="beOverride">存在同名文件时是否覆盖</param>
        /// <param name="compressIfGreaterSize">超过该大小时进行质量压缩（单位：KB）,该参数只对图片上传有效</param>
        /// <param name="maxWidth">图片最大宽（超出时裁剪/压缩）</param>
        /// <param name="maxHeight">图片最大高（超出时裁剪/压缩）</param>
        /// <param name="cutIfOut">超出图片限制的宽/高时是否裁剪（为false时不足部分留白）</param>
        public UploadContext(string apiAddress, IEnumerable<IFormFile> files, string filter, long maxSize, string uploadFolder, bool isFixedPath, string uploadName = null, string extensionName = null, bool beOverride = false, long compressIfGreaterSize = 0, int maxWidth = 0, int maxHeight = 0, bool cutIfOut = false)
        {
            this.ApiAddress = apiAddress ?? string.Empty;
            this.MaxSize = maxSize;
            this.UploadFolder = uploadFolder ?? string.Empty;
            this.IsFixedPath = isFixedPath;
            this.UploadName = uploadName ?? string.Empty;
            this.SaveExtension = extensionName ?? string.Empty;
            this.BeOverride = beOverride;
            this.CompressIfGreaterSize = compressIfGreaterSize;
            this.MaxWidth = maxWidth;
            this.MaxHeight = maxHeight;
            this.CutIfOut = cutIfOut;

            //文件
            this.FormFileList = files.Where(file => file != null && file.Length > 0)
                .Select(file => new FormFileContent
                {
                    FormFile = file
                }).ToList();

            //格式限制
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var arr = filter.ToLower().Split(new[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct();

                this.Filter = new string[arr.Count()];

                for (var i = 0; i < arr.Count(); i++)
                {
                    var temp = arr.ElementAt(i).Trim();
                    if (!temp.StartsWith(".")) temp = "." + temp;
                    this.Filter[i] = temp;
                }
            }
        }

        #endregion

        /// <summary>
        /// 允许上传的文件格式
        /// </summary>
        public string[] Filter { get; private set; }

        /// <summary>
        /// 最大允许上传的文件大小（单位：KB）
        /// </summary>
        public long MaxSize { get; private set; }

        /// <summary>
        /// 超过该大小时进行质量压缩（单位：KB）,该参数只对图片上传有效
        /// </summary>
        public long CompressIfGreaterSize { get; private set; }

        /// <summary>
        /// 图片最大宽（超出时裁剪/压缩）
        /// </summary>
        public int MaxWidth { get; private set; }

        /// <summary>
        /// 图片最大高（超出时裁剪/压缩）
        /// </summary>
        public int MaxHeight { get; private set; }

        /// <summary>
        /// 超出图片限制的宽/高时是否裁剪（为false时不足部分留白）
        /// </summary>
        public bool CutIfOut { get; private set; }

        /// <summary>
        /// 保存的文件扩展名（如：jpg）
        /// </summary>
        public string SaveExtension { get; private set; }

        /// <summary>
        /// 文件对象集合
        /// </summary>
        public List<FormFileContent> FormFileList { get; private set; }

        /// <summary>
        /// 是否为批量上传
        /// </summary>
        public bool IsBatchUpload { get { return FormFileList != null && FormFileList.Count > 0; } }

        /// <summary>
        /// 文件保存的文件夹路径
        /// </summary>
        public string UploadFolder { get; private set; }

        /// <summary>
        /// 是否固定存储在指定的UploadFolder目录下，为false时在UploadFolder目录后添加日期目录
        /// </summary>
        public bool IsFixedPath { get; private set; }

        /// <summary>
        /// 存在同名文件时覆盖
        /// </summary>
        public bool BeOverride { get; private set; }

        /// <summary>
        /// 上传API地址
        /// </summary>
        public string ApiAddress { get; private set; }

        /// <summary>
        /// 上传文件名
        /// </summary>
        public string UploadName { get; private set; }
    }

    /// <summary>
    /// 上传文件内容
    /// </summary>
    public class FormFileContent
    {
        /// <summary>
        /// 文件
        /// </summary>
        public IFormFile FormFile { get; set; }

        /// <summary>
        /// 文件标志名
        /// </summary>
        public string FieldName
        {
            get
            {
                return null != HeaderValue ? HeaderValue.Name : string.Empty;
            }
        }

        private string _contentType;
        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_contentType))
                {
                    //文件内容信息
                    if (null != HeaderValue && null != HeaderValue.Parameters)
                    {
                        foreach (var p in HeaderValue.Parameters)
                        {
                            if (p.Name.Equals("ContentType", StringComparison.OrdinalIgnoreCase))
                            {
                                _contentType = p.Value.Replace("-", "/");
                                break;
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(_contentType))
                    {
                        string extension = Path.GetExtension(FormFile.FileName);
                        _contentType = FileContentType.GetMimeType(extension);
                    }

                    if (string.IsNullOrWhiteSpace(_contentType))
                    {
                        _contentType = FormFile.ContentType;
                    }
                }

                return _contentType;
            }
        }

        /// <summary>
        /// 是否为图片文件
        /// </summary>
        public bool IsImage
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ContentType))
                {
                    return ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
                }

                return false;
            }
        }

        private ContentDispositionHeaderValue _headerValue;
        private ContentDispositionHeaderValue HeaderValue
        {
            get
            {
                if (_headerValue == null)
                {
                    _headerValue = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition);
                }

                return _headerValue;
            }
        }
    }
}
