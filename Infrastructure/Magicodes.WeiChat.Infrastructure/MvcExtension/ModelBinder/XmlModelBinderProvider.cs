// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : XmlModelBinderProvider.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Web;
using System.Web.Mvc;

namespace Magicodes.WeiChat.Infrastructure.MvcExtension.ModelBinder
{
    /// <summary>
    ///     明确告知MVC当客户的请求格式为text/xml时，应该使用XmlModelBinder
    ///     注册方式如下：
    ///     protected void Application_Start()
    ///     {
    ///     ModelBinderProviders.BinderProviders.Insert(0, new XmlModelBinderProvider());
    ///     }
    /// </summary>
    public class XmlModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(Type modelType)
        {
            var contentType = HttpContext.Current.Request.ContentType.ToLower();
            if (contentType != "text/xml")
                return null;
            return new XmlModelBinder();
        }
    }
}