// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : JsonModelBinder.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Magicodes.WeiChat.Infrastructure.MvcExtension.ModelBinder
{
    public class JsonModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            object model = null;
            try
            {
                var strJson = controllerContext.HttpContext.Request.Params[bindingContext.ModelName];
                if (string.IsNullOrEmpty(strJson))
                {
                    return null;
                }
                model = JsonConvert.DeserializeObject(strJson, bindingContext.ModelType);
                var modelMetaData = ModelMetadataProviders.Current
                    .GetMetadataForType(() => model, bindingContext.ModelType);
                var validator = ModelValidator
                    .GetModelValidator(modelMetaData, controllerContext);
                var validationResult = validator.Validate(null);
                foreach (var item in validationResult)
                    bindingContext.ModelState
                        .AddModelError(item.MemberName, item.Message);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelType.Name, ex.Message);
            }
            return model;
        }
    }
}