// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MyAddressApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Magicodes.Logger;
using Magicodes.Shop.Areas.App.Models;
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeiChat.Data.Models.Logistics;
using Magicodes.WeiChat.Data.Models.User;
using Magicodes.WeiChat.Infrastructure;
using Newtonsoft.Json;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/MyAddress")]
    public class MyAddressApiController : TenantBaseApiController<User_Address>
    {
        private readonly LoggerBase log = Loggers.Current.DefaultLogger;

        [HttpPost]
        [Route("DelAddress")]
        public IHttpActionResult DelAddress([FromBody] User_Address model)
        {
            log.Log(LoggerLevels.Debug, "进入API[DelAddress]");
            var address = db.User_Addresses.Find(model.Id);
            if (address == null)
                log.Log(LoggerLevels.Debug, "id为:" + model.Id + "数据库查询不到记录");
            else
                log.Log(LoggerLevels.Debug, JsonConvert.SerializeObject(address));
            db.User_Addresses.Remove(address);
            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        ///     新增收货地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddAddress")]
        public IHttpActionResult AddAddress([FromBody] User_Address model)
        {
            log.Log(LoggerLevels.Debug, "进入API[AddAddress]");
            //log.Log(LoggerLevels.Debug, "model：" + JsonConvert.SerializeObject(model));
            if ((model.Id == null) || (model.Id == Guid.Empty))
            {
                var address = new User_Address();
                model.Id = Guid.NewGuid();
                model.CreateTime = DateTime.Now;
                model.OpenId = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
                address = model;
                address.TenantId = WeiChatApplicationContext.Current.WeiChatUser.TenantId;
                db.User_Addresses.Add(address);
            }
            else
            {
                var address = db.User_Addresses.Find(model.Id);
                address.Name = model.Name; //: v_name,
                address.PhoneNumber = model.PhoneNumber; //: v_mobile,
                address.Province = model.PhoneNumber; //: Province,
                address.City = model.City; // : City,
                address.District = model.District; //: District,
                address.Street = model.Street; //: v_address,
                address.IsDefault = model.IsDefault;
            }
            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        ///     获取地区信息 赋值给前台下来集合
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCity")]
        public IHttpActionResult GetCity()
        {
            log.Log(LoggerLevels.Trace, "进入API[GetCity]");
            var List = db.Logistics_Areas.Where(p => p.AreaLevel != 0).ToList();
            var Lst_areas = new List<LogisArea>();

            foreach (var a in List)
            {
                if (a.AreaLevel != Logistics_AreaLevel.Province) continue;
                var m = new LogisArea();
                m.name = a.AreaName;
                m.type = 1;
                m.sub = GetBaseArea(Logistics_AreaLevel.City, a.Id, List);
                Lst_areas.Add(m);
            }
            //log.Log(LoggerLevels.Trace, "返回[GetCity]出参:" + JsonConvert.SerializeObject(Lst_areas));
            return Ok(Lst_areas);
        }


        public List<LogisArea> GetBaseArea(Logistics_AreaLevel level, string parentId, List<Logistics_Area> lst_parent)
        {
            var lst = new List<LogisArea>();
            foreach (var a in lst_parent)
            {
                if (a.ParentId != parentId)
                    continue;
                var m = new LogisArea();
                if (level == Logistics_AreaLevel.City)
                {
                    m.type = 0;
                    m.sub = GetBaseArea(Logistics_AreaLevel.Zone, a.Id, lst_parent);
                }
                m.name = a.AreaName;
                lst.Add(m);
            }
            return lst;
        }

        [HttpGet]
        [Route("Address")]
        public IHttpActionResult Get(Guid? Id = null)
        {
            User_Address model = null;
            var openId = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
            if (Id == null)
                model = db.User_Addresses.FirstOrDefault(p => p.OpenId == openId);
            else
                model = db.User_Addresses.Find(Id);
            return Ok(model);
        }
    }
}