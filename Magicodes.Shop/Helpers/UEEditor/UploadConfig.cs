// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : UploadConfig.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

namespace Magicodes.Shop.Helpers.UEEditor
{
    public class UploadConfig
    {
        /// <summary>
        ///     文件命名规则
        /// </summary>
        public string PathFormat { get; set; }

        /// <summary>
        ///     上传表单域名称
        /// </summary>
        public string UploadFieldName { get; set; }

        /// <summary>
        ///     上传大小限制
        /// </summary>
        public int SizeLimit { get; set; }

        /// <summary>
        ///     上传允许的文件格式
        /// </summary>
        public string[] AllowExtensions { get; set; }

        /// <summary>
        ///     文件是否以 Base64 的形式上传
        /// </summary>
        public bool Base64 { get; set; }

        /// <summary>
        ///     Base64 字符串所表示的文件名
        /// </summary>
        public string Base64Filename { get; set; }
    }
}