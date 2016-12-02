// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : VoicesApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Magicodes.Shop.Controllers.WebApi.Material
{
    [RoutePrefix("api/voices")]
    public class VoicesApiController : WebApiControllerBase
    {
        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var result = MediaApi.DeleteForeverMedia(AccessToken, id);
            if (result.errcode != ReturnCode.请求成功)
                return BadRequest(result.errmsg);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}