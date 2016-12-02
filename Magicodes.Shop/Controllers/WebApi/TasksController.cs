using Magicodes.Shop.Helpers;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Magicodes.Shop.Controllers.WebApi
{
    [RoutePrefix("api/Tasks")]
    public class TasksController : WebApiControllerBase
    {
        [HttpGet]
        [Route("StartTenantTask/{type}")]
        public IHttpActionResult StartTenantTask(WeiChat_SyncTypes type)
        {
            var tenantId = TenantId;
            var userId = UserId;
            var paramStr = tenantId + ";" + userId;
            WeiChatApplicationContext.Current.TaskManager.Start(type.ToString(), paramStr, new Site_Notify()
            {
                CreateBy = UserId,
                CreateTime = DateTime.Now,
                IconCls = "fa fa-circle-o",
                UpdateTime = DateTime.Now,
                IsTaskFinish = false,
                Receiver = "Tenant_" + tenantId,
            });
            return Ok();
        }
    }
}