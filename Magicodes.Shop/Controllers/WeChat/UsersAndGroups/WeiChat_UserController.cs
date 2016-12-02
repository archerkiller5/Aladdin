// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_UserController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:14
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.Shop.Helpers;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using NLog;
using Webdiyer.WebControls.Mvc;
using Magicodes.Shop.Models;
using Magicodes.WeChat.SDK.Apis.User;
using Magicodes.WeiChat.Data.Models.User;

namespace Magicodes.Shop.Controllers.WeChat.UsersAndGroups
{
    public class WeiChat_UserController : TenantBaseController<WeiChat_User>
    {
        // GET: WeiChat_User
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10,
            ExportTypes exportType = ExportTypes.None)
        {
          //  WeiChatViewUserAndUserInfo WeiChat = new WeiChatViewUserAndUserInfo();
            var queryable = db.WeiChat_Users.AsQueryable();
      
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(
                        p =>
                            p.NickName.Contains(q) || p.City.Contains(q) || p.Country.Contains(q) ||
                            p.Province.Contains(q) || p.Remark.Contains(q));
            queryable = queryable.OrderByDescending(p => p.SubscribeTime);
            switch (exportType)
            {
                case ExportTypes.Csv:
                    return Csv(queryable.ToList());
                    //case Helpers.ExportTypes.Excel:
                    //    return Excel(queryable.ToList());
            }
            var pagedList = new PagedList<WeiChat_User>(
                await queryable
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            var myGroups = db.WeiChat_UserGroups.Where(p => p.TenantId == TenantId).ToList();
            foreach (var item in pagedList)
                item.UserGroup = myGroups.FirstOrDefault(p => p.GroupId == item.GroupId);
            ViewBag.UserGroups = new SelectList(myGroups, "GroupId", "Name");
            return View(pagedList);
        }

        public ActionResult IndexView(string q, int pageIndex = 1, int pageSize = 12, int? groupId = null)
        {
            var queryable = db.WeiChat_Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(
                        p =>
                            p.NickName.Contains(q) || p.City.Contains(q) || p.Country.Contains(q) ||
                            p.Province.Contains(q) || p.Remark.Contains(q));
            if (groupId != null)
                queryable = queryable.Where(p => p.GroupId == groupId);
            var pagedList = new PagedList<WeiChat_User>(
                queryable
                    .OrderByDescending(p => p.SubscribeTime)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                pageIndex, pageSize, queryable.Count());
            var myGroups = db.WeiChat_UserGroups.Where(p => p.TenantId == TenantId).ToList();
            ViewBag.UserGroups = new SelectList(myGroups, "GroupId", "Name");
            return View(pagedList);
        }
        // GET: WeiChat_User/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_User = await db.WeiChat_Users.FindAsync(id);
            if (weiChat_User == null)
                return HttpNotFound();
            return View(weiChat_User);
        }

        [HttpPost]
        public ActionResult RemarkView(RemarkViewModel model)
        {
            var message = new MessageInfo
            {
                Message = "操作成功！",
                MessageType = MessageTypes.Success
            };
            var user = db.WeiChat_Users.FirstOrDefault(p => p.OpenId == model.OpenId);
            if (user != null)
            {
                var result = WeChatApisContext.Current.UserApi.SetRemark(model.OpenId, model.Remark);
                if (result.IsSuccess())
                {
                    user.Remark = model.Remark;
                    db.SaveChanges();
                }
                else
                {
                    message.Message = result.GetFriendlyMessage();
                    message.MessageType = MessageTypes.Danger;
                }
            }
            else
            {
                message.Message = "账号不存在！";
                message.MessageType = MessageTypes.Danger;
            }
            return Json(message);
        }


        // GET: WeiChat_User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WeiChat_User/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            WeiChatViewUserAndUserInfo model)
            
        {
            if (ModelState.IsValid)
            {
                var weichat =  new WeiChat_User();
                var info = new User_Info();
               
                //weichat .OpenId= model.User.OpenId;
             
                weichat.NickName = model.User.NickName;
                weichat.Sex = model.User.Sex;
                weichat.City = model.User.City;
                weichat.Language = model.User.Language;
                weichat.Country = model.User.Country;
                weichat.Province = model.User.Province;
                weichat.HeadImgUrl = model.User.HeadImgUrl;
                weichat.SubscribeTime = model.User.SubscribeTime;
                weichat.UnionId = model.User.UnionId;
                weichat.Remark = model.User.Remark;
                weichat.GroupId = model.User.GroupId;
               // info.UserNo = model.Userinfo.UserNo;
                info.Userid = model.Userinfo.Userid;
                info.Pwd = model.Userinfo.Pwd;
                info.Email = model.Userinfo.Email;
                info.Mobile = model.Userinfo.Mobile;
                info.NickName = weichat.NickName;
                info.TrueName = model.Userinfo.TrueName;
                info.IdCard = model.Userinfo.IdCard;
                info.Address = model.Userinfo.Address;
                info.WorkPlace_1 = model.Userinfo.WorkPlace_1;
                info.WorkPlace_2 = model.Userinfo.WorkPlace_2;
                info.WorkPlace_3 = model.Userinfo.WorkPlace_3;
                info.Business_scope_1 = model.Userinfo.Business_scope_1;
                info.Business_scope_2 = model.Userinfo.Business_scope_2;
                info.Business_scope_3 = model.Userinfo.Business_scope_3;
                info.Tel_1 = model.Userinfo.Tel_1;
                info.Tel_2 = model.Userinfo.Tel_2;
                info.Tel_3 = model.Userinfo.Tel_3;
                weichat.OpenId = info.Userid;
                info.CreateTime = DateTime.Now;
                info.State = 0;
                info.Integral = 0;
                info.Balance = 0;
                info.LastLoginOn = DateTime.Now;

                db.WeiChat_Users.Add(weichat);

                await db.SaveChangesAsync();

                db.User_Infos.Add(info);

                await db.SaveChangesAsync();

                

            }
            return RedirectToAction("Index");

        }
 
        // GET: WeiChat_User/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            WeiChatViewUserAndUserInfo weiChat_User=new WeiChatViewUserAndUserInfo();
            
            weiChat_User.User = await db.WeiChat_Users.FirstOrDefaultAsync(p => p.OpenId == id);
            weiChat_User.Userinfo = await db.User_Infos.FirstOrDefaultAsync(m => m.OpenId == id);
            if (weiChat_User == null)
                return HttpNotFound();
            weiChat_User.User.UserGroup = db.WeiChat_UserGroups.FirstOrDefault(p => p.GroupId == weiChat_User.User.GroupId);
            return View(weiChat_User);
        }

        // POST: WeiChat_User/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
              WeiChatViewUserAndUserInfo model)
        {
            if (ModelState.IsValid)
            {
                //var result = WeChatApisContext.Current.UserApi.SetRemark(model.User.OpenId, model.User.Remark);
                //if (result.IsSuccess())
                //{
                    var mo = await db.WeiChat_Users.FindAsync(model.User.OpenId);

                    var info = await db.User_Infos.FindAsync(model.User.OpenId);
                if (info == null) {
                    var infos = new User_Info();
                   infos.OpenId = model.User.OpenId;
                    infos.CreateTime = DateTime.Now;
                    infos.State = 0;
                    infos.Integral = 0;
                    infos.Balance = 0;
                    infos.LastLoginOn = DateTime.Now;
                    db.User_Infos.Add(infos);
                    await db.SaveChangesAsync();
                    info = await db.User_Infos.FindAsync(model.User.OpenId);
                }
               
                    mo.NickName = model.User.NickName;
                    mo.Sex = model.User.Sex;
                    mo.City = model.User.City;
                    mo.Language = model.User.Language;
                    mo.Country = model.User.Country;
                    mo.Province = model.User.Province;
                    mo.HeadImgUrl = model.User.HeadImgUrl;
                    mo.SubscribeTime = model.User.SubscribeTime;
                    mo.UnionId = model.User.UnionId;
                    mo.Remark = model.User.Remark;
                    mo.GroupId = model.User.GroupId;
                   // info.UserNo = model.Userinfo.UserNo;
                    info.Userid = model.Userinfo.Userid;
                    info.Pwd = model.Userinfo.Pwd;
                    info.Email = model.Userinfo.Email;
                    info.Mobile = model.Userinfo.Mobile;
                   
                    info.TrueName = model.Userinfo.TrueName;
                    info.IdCard = model.Userinfo.IdCard;
                    info.Address = model.Userinfo.Address;
                    info.WorkPlace_1 = model.Userinfo.WorkPlace_1;
                    info.WorkPlace_2 = model.Userinfo.WorkPlace_2;
                    info.WorkPlace_3 = model.Userinfo.WorkPlace_3;
                    info.Business_scope_1 = model.Userinfo.Business_scope_1;
                    info.Business_scope_2 = model.Userinfo.Business_scope_2;
                    info.Business_scope_3 = model.Userinfo.Business_scope_3;
                    info.Tel_1 = model.Userinfo.Tel_1;
                    info.Tel_2 = model.Userinfo.Tel_2;
                    info.Tel_3 = model.Userinfo.Tel_3;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                
                //ModelState.AddModelError(string.Empty, result.GetFriendlyMessage());
            }
            model.User.UserGroup = db.WeiChat_UserGroups.FirstOrDefault(p => p.GroupId == model.User.GroupId);
            return View();
        }

        // GET: WeiChat_User/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_User = await db.WeiChat_Users.FindAsync(id);
            if (weiChat_User == null)
                return HttpNotFound();
            return View(weiChat_User);
        }

        // POST: WeiChat_User/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var weiChat_User = await db.WeiChat_Users.FindAsync(id);
            db.WeiChat_Users.Remove(weiChat_User);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: WeiChat_User/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_User/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, int groupId, params string[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_Users.Where(p => ids.Contains(p.OpenId)).ToListAsync();
                    if (models.Count == 0)
                    {
                        ajaxResponse.Success = false;
                        ajaxResponse.Message = "没有找到匹配的项，项已被删除或不存在！";
                        return Json(ajaxResponse);
                    }
                    switch (operation.ToUpper())
                    {
                        case "MOVE":

                            #region 移动

                            {
                                var result = WeChatApisContext.Current.UserGroupApi.MemeberUpdate(ids, groupId);
                                if (result.IsSuccess())
                                {
                                    foreach (var item in models)
                                        item.GroupId = groupId;
                                    await db.SaveChangesAsync();
                                    ajaxResponse.Success = true;
                                    ajaxResponse.Message = string.Format("已成功操作{0}项！", models.Count);
                                }
                                else
                                {
                                    ajaxResponse.Success = false;
                                    ajaxResponse.Message = result.GetFriendlyMessage();
                                }
                                break;
                            }

                        #endregion

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ajaxResponse.Success = false;
                    ajaxResponse.Message = ex.Message;
                }
            }
            else
            {
                ajaxResponse.Success = false;
                ajaxResponse.Message = "请至少选择一项！";
            }
            return Json(ajaxResponse);
        }

        //[HttpPost]
        //public ActionResult SetRemark(string openId, string remark)
        //{
        //    var ajaxResponse = new AjaxResponse();
        //    var user = db.WeiChat_Users.FirstOrDefault(p => p.OpenId == model.OpenId);
        //    if (user != null)
        //    {
        //        var result = WeChatApisContext.Current.UserApi.SetRemark(openId, remark);
        //        if (result.IsSuccess())
        //        {
        //            user.Remark = remark;
        //            db.SaveChanges();
        //            ajaxResponse.Success = true;
        //            ajaxResponse.Message = "操作成功";
        //        }
        //        else
        //        {
        //            ajaxResponse.Success = false;
        //            ajaxResponse.Message = result.GetFriendlyMessage();
        //        }
        //    }
        //    else
        //    {
        //        ajaxResponse.Success = false;
        //        ajaxResponse.Message = "账号不存在！";
        //    }
        //    return Json(ajaxResponse);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}