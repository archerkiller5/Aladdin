// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChatUsersController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:14
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Linq;
using System.Web.Mvc;
using Magicodes.Shop.Models;
using Magicodes.WeiChat.Data.Models;
using Senparc.Weixin.MP.AdvancedAPIs;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.WeChat.UsersAndGroups
{
    public class WeiChatUsersController : TenantBaseController<WeiChat_User>
    {
        //
        // GET: /WeiChatUsers/
        public ActionResult Index(int pageIndex = 1, int pageSize = 12, int? groupId = null)
        {
            var q = db.WeiChat_Users.AsQueryable();
            if (groupId != null)
                q = q.Where(p => p.GroupId == groupId);
            var pagedList = new PagedList<WeiChat_User>(
                q.OrderByDescending(p => p.SubscribeTime)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToList(),
                pageIndex, pageSize, q.Count());
            return View(pagedList);
        }

        [HttpPost]
        public ActionResult Remark(RemarkViewModel model)
        {
            var message = new MessageInfo
            {
                Message = "操作成功！",
                MessageType = MessageTypes.Success
            };
            var user = db.WeiChat_Users.FirstOrDefault(p => p.OpenId == model.OpenId);
            if (user != null)
            {
                UserApi.UpdateRemark(AccessToken, model.OpenId, model.Remark);
                //TODO:判断是否成功
                user.Remark = model.Remark;
                db.SaveChanges();
            }
            else
            {
                message.Message = "账号不存在！";
                message.MessageType = MessageTypes.Danger;
            }
            return Json(message);
        }
    }
}