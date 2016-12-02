// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChatUserApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Infrastructure;

namespace Magicodes.Shop.Controllers.WebApi
{
    [RoutePrefix("api/WeiChatUser")]
    public class WeiChatUserApiController : WebApiControllerBase
    {
        protected AppDbContext db = new AppDbContext();

        [Route("")]
        // GET: api/WeiChatUser
        public async Task<IHttpActionResult> Get(string key, int top = 100)
        {
            var queryable =
                db.WeiChat_Users.Where(p => p.Subscribe && (p.TenantId == WeiChatApplicationContext.Current.TenantId))
                    .AsQueryable();
            if (!string.IsNullOrWhiteSpace(key))
                queryable = queryable.Where(p => p.NickName.Contains(key));
            return Ok(queryable.Take(top).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}