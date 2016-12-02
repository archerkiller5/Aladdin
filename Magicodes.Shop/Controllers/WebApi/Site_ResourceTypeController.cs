// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Site_ResourceTypeController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.Logging;
using Magicodes.WeiChat.Infrastructure.Tenant;

namespace Magicodes.Shop.Controllers.WebApi
{
    [System.Web.Http.RoutePrefix("api/Site_ResourceType")]
    public class Site_ResourceTypeController : TenantBaseApiController<Site_ResourceType>
    {
        // GET: api/Site_ResourceType/{resourceType}
        [System.Web.Http.Route("{resourceType}")]
        public IQueryable<Site_ResourceType> GetSite_ResourceTypes(SiteResourceTypes resourceType)
        {
            return db.Site_ResourceTypes.Where(p => p.ResourceType == resourceType).OrderByDescending(p => p.CreateTime);
        }

        // GET: api/Site_ResourceType/{resourceType}/5
        [ResponseType(typeof(Site_ResourceType))]
        [System.Web.Http.Route("{resourceType}/{id:guid}")]
        public async Task<IHttpActionResult> GetSite_ResourceType(Guid id)
        {
            var site_ResourceType = await db.Site_ResourceTypes.FindAsync(id);
            if (site_ResourceType == null)
                return NotFound();

            return Ok(site_ResourceType);
        }

        // PUT: api/Site_ResourceType/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSite_ResourceType(Guid id, Site_ResourceType site_ResourceType)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != site_ResourceType.Id)
                return BadRequest();

            db.Entry(site_ResourceType).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Site_ResourceTypeExists(id))
                    return NotFound();
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Site_ResourceType
        [ResponseType(typeof(Site_ResourceType))]
        public async Task<IHttpActionResult> PostSite_ResourceType(
            [Bind(Include = "Title,ResourceType")] Site_ResourceType site_ResourceType)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (
                db.Site_ResourceTypes.Any(
                    p => (p.Title == site_ResourceType.Title) && (p.ResourceType == site_ResourceType.ResourceType)))
                return BadRequest("该标签已存在，请不要添加重复的标签！");
            SetModelWithChangeStates(site_ResourceType, default(Guid));
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Site_ResourceTypeExists(site_ResourceType.Id))
                    return Conflict();
                throw;
            }

            return CreatedAtRoute("DefaultApi", new {id = site_ResourceType.Id}, site_ResourceType);
        }

        // DELETE: api/Site_ResourceType/5
        [System.Web.Http.Route("{id:guid}")]
        [ResponseType(typeof(Site_ResourceType))]
        public async Task<IHttpActionResult> DeleteSite_ResourceType(Guid id)
        {
            try
            {
                TenantManager.Current.DisableTenantFilter(db);
                var site_ResourceType = await db.Site_ResourceTypes.FindAsync(id);
                if (site_ResourceType == null)
                    return NotFound();
                if (site_ResourceType.IsSystemResource)
                    return BadRequest("系统类型无法删除！");
                //批量操作性能优化
                switch (site_ResourceType.ResourceType)
                {
                    case SiteResourceTypes.Gallery:
                    {
                        var defaultType =
                            db.Site_ResourceTypes.First(
                                p => (p.ResourceType == SiteResourceTypes.Gallery) && p.IsSystemResource);

                        db.Database.ExecuteSqlCommand(
                            "Update [MWC].[Site_Image] set ResourcesTypeId={0} where ResourcesTypeId={1}",
                            defaultType.Id, site_ResourceType.Id);
                        //db.BathUpdateBy(db.Site_Images, x => x.ResourcesTypeId == site_ResourceType.Id, x => x.ResourcesTypeId, x => defaultType.Id);
                    }
                        break;
                    case SiteResourceTypes.Voice:
                    {
                        var defaultType =
                            db.Site_ResourceTypes.First(
                                p => (p.ResourceType == SiteResourceTypes.Voice) && p.IsSystemResource);

                        db.Database.ExecuteSqlCommand(
                            "Update [MWC].[Site_Voice] set ResourcesTypeId={0} where ResourcesTypeId={1}",
                            defaultType.Id, site_ResourceType.Id);

                        //db.BathUpdateBy(db.Site_Voices, x => x.ResourcesTypeId == site_ResourceType.Id, x => x.ResourcesTypeId, x => defaultType.Id);
                    }
                        break;
                    case SiteResourceTypes.Video:
                    {
                        var defaultType =
                            db.Site_ResourceTypes.First(
                                p => (p.ResourceType == SiteResourceTypes.Video) && p.IsSystemResource);

                        db.Database.ExecuteSqlCommand(
                            "Update [MWC].[Site_Video] set ResourcesTypeId={0} where ResourcesTypeId={1}",
                            defaultType.Id, site_ResourceType.Id);

                        //db.BathUpdateBy(db.Site_Videos, x => x.ResourcesTypeId == site_ResourceType.Id, x => x.ResourcesTypeId, x => defaultType.Id);
                    }
                        break;
                    case SiteResourceTypes.Article:
                    {
                        var defaultType =
                            db.Site_ResourceTypes.First(
                                p => (p.ResourceType == SiteResourceTypes.Article) && p.IsSystemResource);

                        db.Database.ExecuteSqlCommand(
                            "Update [MWC].[Site_Article] set ResourcesTypeId={0} where ResourcesTypeId={1}",
                            defaultType.Id, site_ResourceType.Id);
                        //db.BathUpdateBy(db.Site_Articles, x => x.ResourcesTypeId == site_ResourceType.Id, x => x.ResourcesTypeId, x => defaultType.Id);
                    }
                        break;
                    case SiteResourceTypes.News:
                    {
                        var defaultType =
                            db.Site_ResourceTypes.First(
                                p => (p.ResourceType == SiteResourceTypes.News) && p.IsSystemResource);

                        db.Database.ExecuteSqlCommand(
                            "Update [MWC].[Site_News] set ResourcesTypeId={0} where ResourcesTypeId={1}", defaultType.Id,
                            site_ResourceType.Id);
                        //db.BathUpdateBy(db.Site_News, x => x.ResourcesTypeId == site_ResourceType.Id, x => x.ResourcesTypeId, x => defaultType.Id);
                    }
                        break;
                    default:
                        break;
                }
                db.Site_ResourceTypes.Remove(site_ResourceType);
                await db.SaveChangesAsync();

                return Ok(site_ResourceType);
            }
            catch (Exception ex)
            {
                //LogManager.GetCurrentClassLogger().LogException(ex);
                Loggers.Current.DefaultLogger.LogException(ex);
                return BadRequest("删除出错，请联系管理员！");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        private bool Site_ResourceTypeExists(Guid id)
        {
            return db.Site_ResourceTypes.Count(e => e.Id == id) > 0;
        }
    }
}