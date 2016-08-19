using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Td.AspNet.Upload
{
    /// <summary>
    /// 上传的文件内容
    /// </summary>
    public class UploadContext
    {
        #region 单个文件FormFile上传实例

        /// <summary>
        /// 实始化文件上传内容实例（自动随机生成文件名称）
        /// </summary>
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

        #region 多文件FormFile上传实例

        /// <summary>
        /// 实始化文件上传内容实例（自动随机生成文件名称）
        /// </summary>
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
            this.FileList = files.Where(file => file != null && file.Length > 0)
                .Select(file => new FormFileContent
                {
                    FormFile = file
                }).ToList<FileContent>();

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

        #region 单个文件Base64上传实例

        /// <summary>
        /// 实始化文件上传内容实例（自动随机生成文件名称）
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="fieldName">上传的文件标志字段名称</param>
        /// <param name="base64String">上传文件的Base64内容</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        public UploadContext(string apiAddress, string fieldName, string base64String, long maxSize, string uploadFolder, bool isFixedPath)
            : this(apiAddress, new KeyValuePair<string, string>(fieldName, base64String), maxSize, uploadFolder, isFixedPath, null) { }

        /// <summary>
        /// 实始化文件上传内容实例（自动随机生成文件名称）
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="fieldAndBase64String">待上传的文件标志字段及Base64字符串</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        public UploadContext(string apiAddress, KeyValuePair<string, string> fieldAndBase64String, long maxSize, string uploadFolder, bool isFixedPath)
            : this(apiAddress, fieldAndBase64String, maxSize, uploadFolder, isFixedPath, null) { }

        /// <summary>
        /// 实始化文件上传内容实例（存在同名文件时不覆盖）
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="fieldAndBase64String">待上传的文件标志字段及Base64字符串</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        public UploadContext(string apiAddress, KeyValuePair<string, string> fieldAndBase64String, long maxSize, string uploadFolder, bool isFixedPath, string uploadName)
            : this(apiAddress, fieldAndBase64String, maxSize, uploadFolder, isFixedPath, uploadName, null) { }

        /// <summary>
        /// 实始化文件上传内容实例
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="fieldAndBase64String">待上传的文件标志字段及Base64字符串</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        /// <param name="extensionName">扩展名（如：jpg）</param>
        public UploadContext(string apiAddress, KeyValuePair<string, string> fieldAndBase64String, long maxSize, string uploadFolder, bool isFixedPath, string uploadName, string extensionName)
            : this(apiAddress, fieldAndBase64String, maxSize, uploadFolder, isFixedPath, uploadName, extensionName, false) { }

        /// <summary>
        /// 实始化文件上传内容实例
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="fieldAndBase64String">待上传的文件标志字段及Base64字符串</param>
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
        public UploadContext(string apiAddress, KeyValuePair<string, string> fieldAndBase64String, long maxSize, string uploadFolder, bool isFixedPath, string uploadName = null, string extensionName = null, bool beOverride = false, long compressIfGreaterSize = 0, int maxWidth = 0, int maxHeight = 0, bool cutIfOut = false)
            : this(apiAddress, new Dictionary<string, string>() { { fieldAndBase64String.Key, fieldAndBase64String.Value } }, maxSize, uploadFolder, isFixedPath, uploadName, extensionName, beOverride, compressIfGreaterSize, maxWidth, maxHeight, cutIfOut) { }

        #endregion

        #region 多文件Base64上传实例

        /// <summary>
        /// 实始化文件上传内容实例（自动随机生成文件名称）
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="fieldAndBase64Strings">待上传的文件标志字段名称及Base64字符串集合</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        public UploadContext(string apiAddress, IDictionary<string, string> fieldAndBase64Strings, long maxSize, string uploadFolder, bool isFixedPath)
                    : this(apiAddress, fieldAndBase64Strings, maxSize, uploadFolder, isFixedPath, null) { }

        /// <summary>
        /// 实始化文件上传内容实例（存在同名文件时不覆盖）
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="fieldAndBase64Strings">待上传的文件标志字段名称及Base64字符串集合</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        public UploadContext(string apiAddress, IDictionary<string, string> fieldAndBase64Strings, long maxSize, string uploadFolder, bool isFixedPath, string uploadName)
                    : this(apiAddress, fieldAndBase64Strings, maxSize, uploadFolder, isFixedPath, uploadName, null) { }

        /// <summary>
        /// 实始化文件上传内容实例
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="fieldAndBase64Strings">待上传的文件标志字段名称及Base64字符串集合</param>
        /// <param name="maxSize">最大允许上传的文件大小（单位：KB）</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="isFixedPath">是否固定存储在指定的<paramref name="uploadFolder"/>目录下，为false时在<paramref name="uploadFolder"/>目录后添加日期目录</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        /// <param name="extensionName">扩展名（如：jpg）</param>
        public UploadContext(string apiAddress, IDictionary<string, string> fieldAndBase64Strings, long maxSize, string uploadFolder, bool isFixedPath, string uploadName, string extensionName)
                    : this(apiAddress, fieldAndBase64Strings, maxSize, uploadFolder, isFixedPath, uploadName, extensionName, false, 0, 0, 0, false) { }

        /// <summary>
        /// 实始化文件上传内容实例
        /// </summary>
        /// <param name="apiAddress">上传API地址</param>
        /// <param name="fieldAndBase64Strings">待上传的文件标志字段名称及Base64字符串集合</param>
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
        public UploadContext(string apiAddress, IDictionary<string, string> fieldAndBase64Strings, long maxSize, string uploadFolder, bool isFixedPath, string uploadName = null, string extensionName = null, bool beOverride = false, long compressIfGreaterSize = 0, int maxWidth = 0, int maxHeight = 0, bool cutIfOut = false)
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
            this.FileList = fieldAndBase64Strings.Select(p => new Base64FileContent
            {
                FieldName = p.Key,
                Data = p.Value,
            }).ToList<FileContent>();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 添加FormFile文件内容
        /// </summary>
        /// <param name="file"></param>
        public void AddFormFile(IFormFile file)
        {
            if (file != null)
            {
                this.FileList.Add(new FormFileContent
                {
                    FormFile = file
                });
            }
        }

        /// <summary>
        /// 添加FormFile文件内容
        /// </summary>
        /// <param name="files"></param>
        public void AddFormFile(IEnumerable<IFormFile> files)
        {
            if (files != null && files.Count() > 0)
            {
                var newFiles = files.Where(file => file != null && file.Length > 0)
                 .Select(file => new FormFileContent
                 {
                     FormFile = file
                 }).ToList<FileContent>();

                this.FileList.AddRange(newFiles);
            }
        }

        #region 添加Base64文件数据

        /// <summary>
        /// 添加Base64图片文件内容
        /// </summary>
        /// <param name="fieldName">标志名称</param>
        /// <param name="base64String">Base64文件内容</param>
        public void AddBase64File(string fieldName, string base64String)
        {
            if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(base64String))
            {
                this.FileList.Add(new Base64FileContent
                {
                    FieldName = fieldName,
                    Data = base64String,
                });
            }
        }

        /// <summary>
        /// 添加Base64图片文件内容
        /// </summary>
        /// <param name="fieldAndBase64String">标志名称 及 Base64文件内容</param>
        public void AddBase64File(KeyValuePair<string, string> fieldAndBase64String)
        {
            if (!fieldAndBase64String.Equals(default(KeyValuePair<string, string>)))
            {
                this.FileList.Add(new Base64FileContent
                {
                    FieldName = fieldAndBase64String.Key,
                    Data = fieldAndBase64String.Value,
                });
            }
        }

        /// <summary>
        /// 添加Base64图片文件内容
        /// </summary>
        /// <param name="fieldAndBase64Strings">标志名称 及 Base64文件内容 集合</param>
        public void AddBase64File(IDictionary<string, string> fieldAndBase64Strings)
        {
            if (fieldAndBase64Strings != null && fieldAndBase64Strings.Count > 0)
            {
                var files = fieldAndBase64Strings.Select(p => new Base64FileContent
                {
                    FieldName = p.Key,
                    Data = p.Value,
                }).ToList<FileContent>();

                this.FileList.AddRange(files);
            }
        }

        #endregion

        #endregion

        #region 属性

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
        public List<FileContent> FileList { get; private set; }

        /// <summary>
        /// 是否为批量上传
        /// </summary>
        public bool IsBatchUpload { get { return FileList != null && FileList.Count > 0; } }

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

        #endregion
    }

    #region 文件信息类

    /// <summary>
    /// 文件内容
    /// </summary>
    public abstract class FileContent
    {
        /// <summary>
        /// 文件标志名
        /// </summary>
        public virtual string FieldName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public virtual string ContentType { get; }

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

        /// <summary>
        /// 文件大小
        /// </summary>
        public abstract long FileSize { get; }
    }

    /// <summary>
    /// FormFileContent
    /// </summary>
    public class FormFileContent : FileContent
    {
        /// <summary>
        /// 文件
        /// </summary>
        public IFormFile FormFile { get; set; }

        private string _fieldName;
        /// <summary>
        /// 文件标志名
        /// </summary>
        public override string FieldName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_fieldName) && FormFile != null)
                {
                    var _headerValue = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition);
                    _fieldName = _headerValue?.Name ?? string.Empty;
                }

                return _fieldName;
            }
            set
            {
                _fieldName = value;
            }
        }

        private string _contentType;
        /// <summary>
        /// 文件类型
        /// </summary>
        public override string ContentType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_contentType) && FormFile != null)
                {
                    var _headerValue = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition);
                    //文件内容信息
                    if (null != _headerValue && null != _headerValue.Parameters)
                    {
                        foreach (var p in _headerValue.Parameters)
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
        /// 文件大小
        /// </summary>
        public override long FileSize
        {
            get
            {
                return FormFile != null ? FormFile.Length : 0;
            }
        }
    }

    /// <summary>
    /// Base64FileContent
    /// </summary>
    public class Base64FileContent : FileContent
    {
        /// <summary>
        /// 标准的Base64文件字符串表示,如：“data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAATgAAAE4==”
        /// </summary>
        public string Data { get; set; }

        private string _fieldName;
        /// <summary>
        /// 文件标志名
        /// </summary>
        public override string FieldName
        {
            get
            {
                return _fieldName ?? string.Empty;
            }
            set { _fieldName = value; }
        }

        private string _contentType;
        /// <summary>
        /// 文件类型
        /// </summary>
        public override string ContentType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_contentType) && !string.IsNullOrWhiteSpace(Data))
                {
                    Regex reg = new Regex(@"^data:(?<contenttype>[^;]+);base64,.+=+$", RegexOptions.IgnoreCase);

                    Match m = reg.Match(Data);

                    if (m.Success)
                    {
                        return m.Groups["contenttype"].Value;
                    }
                }

                return _contentType;
            }
        }

        private long _fileSize;
        /// <summary>
        /// 文件大小
        /// </summary>
        public override long FileSize
        {
            get
            {
                if (_fileSize == 0)
                {
                    Regex reg = new Regex(@"^data:(?<contenttype>[^;]+);base64,(?<content>.+=+)$", RegexOptions.IgnoreCase);

                    Match m = reg.Match(Data);

                    if (m.Success)
                    {
                        string content = m.Groups["content"].Value;

                        byte[] arr = Convert.FromBase64String(content);

                        _fileSize = arr.Length;
                    }
                }
                return _fileSize;
            }
        }
    }

    #endregion
}
