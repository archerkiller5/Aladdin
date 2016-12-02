// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AjaxResponse.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

namespace Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax
{
    /// <summary>
    ///     Ajax请求返回内容
    /// </summary>
    //[Serializable]:该特性会影响WebAPI JSON 序列化结果，见：http://stackoverflow.com/questions/12334382/net-webapi-serialization-k-backingfield-nastiness
    public class AjaxResponse
    {
        /// <summary>
        ///     创建<see cref="AjaxResponse" />
        ///     <see cref="Success" /> 默认设置为True
        /// </summary>
        public AjaxResponse()
        {
            Success = true;
        }


        public AjaxResponse(bool success)
        {
            Success = success;
        }

        /// <summary>
        ///     是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        ///     操作提示
        /// </summary>
        public string Message { get; set; }
    }

    //[Serializable]:该特性会影响WebAPI JSON 序列化结果，见：http://stackoverflow.com/questions/12334382/net-webapi-serialization-k-backingfield-nastiness
    public class AjaxResponse<TResult> : AjaxResponse
    {
        /// <summary>
        ///     创建<see cref="AjaxResponse" />
        ///     <see cref="Success" /> 默认设置为True
        /// </summary>
        public AjaxResponse()
        {
            Success = true;
        }

        /// <summary>
        ///     创建一个 <see cref="AjaxResponse" /> 含 <see cref="Result" /> 的对象
        ///     <see cref="Success" /> 为 True.
        /// </summary>
        /// <param name="result">返回结果</param>
        public AjaxResponse(TResult result)
        {
            Result = result;
            Success = true;
        }

        /// <summary>
        ///     返回结果对象.
        ///     只有在 <see cref="Success" /> 为True时赋值
        /// </summary>
        public TResult Result { get; set; }

        /// <summary>
        ///     操作提示
        /// </summary>
        public string Message { get; set; }
    }
}