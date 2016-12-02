// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MyPointsApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Linq;
using System.Web.Http;
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeiChat.Data.Models.Log;
using Magicodes.WeiChat.Infrastructure;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/MyPoints")]
    public class MyPointsApiController : TenantBaseApiController<Log_Point>
    {
        [HttpGet]
        [Route("GetMyPoints")]
        public IHttpActionResult GetMyPoints()
        {
            var Lst =
                db.Log_Points.Where(p => p.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId).ToList();
            return Ok(Lst);
        }
    }
}