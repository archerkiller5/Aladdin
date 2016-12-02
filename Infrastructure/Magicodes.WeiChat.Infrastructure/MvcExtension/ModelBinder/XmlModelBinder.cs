// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : XmlModelBinder.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Mvc;
using System.Xml.Serialization;

namespace Magicodes.WeiChat.Infrastructure.MvcExtension.ModelBinder
{
    /// <summary>
    ///     XML Model绑定
    ///     Demo:public ActionResult PostXmlContent([ModelBinder(typeof(XmlModelBinder))]UserViewModel user)
    /// </summary>
    public class XmlModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                var modelType = bindingContext.ModelType;
                var serializer = new XmlSerializer(modelType);
                var inputStream = controllerContext.HttpContext.Request.InputStream;
                return serializer.Deserialize(inputStream);
            }
            catch
            {
                bindingContext.ModelState.AddModelError("", "该项无法被序列化！");
                return null;
            }
        }
    }
}