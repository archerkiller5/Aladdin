using Magicodes.Shop.Helpers;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Site;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Magicodes.Shop.Controllers.WebApi
{
    [RoutePrefix("api/Site_Notifies")]
    public class Site_NotifiesApiController : WebApiControllerBase
    {
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Remove(int id)
        {
            var uid = UserId;
            if (!db.Site_ReadNotifies.Any(p => p.NotifyId == id && p.CreateBy == uid))
            {
                var read = new Site_ReadNotify()
                {
                    CreateBy = uid,
                    CreateTime = DateTime.Now,
                    NotifyId = id
                };
                db.Site_ReadNotifies.Add(read);
                db.SaveChanges();
            }
            return Ok();
        }
    }
}