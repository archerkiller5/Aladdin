using Magicodes.Logger;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Magicodes.WeiChat.Infrastructure.MvcExtension.Filters
{
    public class GroupFilter : FilterAttribute, IAuthorizationFilter
    {
        private const string RedirectUrlCookieName = "Magicodes.Weichat_RedirectUrlCookie";
        private readonly LoggerBase _logger = Loggers.Current.DefaultLogger;
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var httpContextBase = filterContext.HttpContext;
            var request = httpContextBase.Request;
            _logger.Log(LoggerLevels.Trace, string.Format("用户进入店铺信息管理...{0}", request.Url));
            //if (WeiChatApplicationContext.Current.WeiChatUser != null)
            //{
            //    _logger.Log(LoggerLevels.Trace, string.Format("用户已经验证或者已经获取微信信息，不再验证...{0}", request.Url));
            //    return;
            //}
            #region 拿到该用户在该模块的数据权限AccessIds
            HashSet<int> _groupIds = new HashSet<int>();
            foreach (var item in WeiChatApplicationContext.Current.WeiChatUser.GroupIds.Split(',').ToArray())
            {
                _groupIds.Add(Convert.ToInt32(item));
            };
            //拿到数据ID
            List<Guid> AccessIds = new List<Guid>();
            if (WeiChatApplicationContext.Current.WeiChatUser != null)
            {
                //拿到用户组
                //_groupIds = WeiChatApplicationContext.Current.WeiChatUser.UserGroup;

                //拿到模块UserGroup
                foreach (var listGroup in ListGroups())
                {
                    //字典对的键集合与用户组取交集,如果有交集的话，允许访问
                    if (listGroup.Key.Intersect(_groupIds).Count() > 0)
                    {
                        //用户所拥有的组包含模块中组的GroupId允许访问
                        AccessIds.Add(listGroup.Value);
                    }
                }
            }
            #endregion
            ////放在缓存里
            //long lTime = long.Parse(DateTime.Now.AddMinutes(20) + "0000000");
            //TimeSpan toNow = new TimeSpan(lTime);
            //Cache.CacheManager.AddOrUpdate(_groupIds, AccessIds.ToString(),toNow);
        }
        /// <summary>
        /// 读取该模块的组ID
        /// </summary>
        /// <returns></returns>
        public static Dictionary<ICollection<int>, Guid> ListGroups()
        {
            Dictionary<ICollection<int>, Guid> dictionaryShop = new Dictionary<ICollection<int>, Guid>();
            List<int> aa = new List<int>();
            //查数据库
            using (AppDbContext db = new AppDbContext())
            {
                //找到所有的shop_info,遍历shop_info存入字典对里
                foreach (var item in db.Shop_Infos)
                {
                    //解析usergroups
                    foreach (var c in item.UserGroups.Split(',').ToList())
                    {
                        aa.Add(Convert.ToInt32(c));
                    }
                    dictionaryShop.Add(aa, item.Id);
                }
                return dictionaryShop;
            }
        }
    }
    /// <summary>
    /// 负责创建EF数据操作上下文实例，必须保证线程唯一
    /// </summary>
    public class DBContextFactory
    {
        public static AppDbContext CreateDbContext()
        {
            AppDbContext dbContext = (AppDbContext)CallContext.GetData("dbContext");
            if (dbContext == null)
            {
                dbContext = new AppDbContext();
                CallContext.SetData("dbContext", dbContext);
            }
            return dbContext;
        }
    }
    /// <summary>
    /// 创建对APP类的泛型仓储方法
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repository<TEntity> where TEntity : WeiChat_TenantBase<Guid>
    {
        private AppDbContext context;
        private IDbSet<TEntity> entities;
        string errorMessage = string.Empty;
        public Repository(AppDbContext context)
        {
            this.context = context;
        }
        public TEntity GetById(Guid id)
        {
            return this.Entities.Find(id);
        }

        public void Insert(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                this.Entities.Add(entity);
                this.context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {

                //错误处理机制
                foreach (var validationErros in ex.EntityValidationErrors)
                {
                    foreach (var errorInfo in validationErros.ValidationErrors)
                    {
                        errorMessage += string.Format("属性:{0} 错误消息:{1}", errorInfo.PropertyName, errorInfo.ErrorMessage)
+ Environment.NewLine;

                    }
                }
                throw new Exception(errorMessage, ex);
            }


        }
        public void Update(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                this.context.SaveChanges();
            }

            catch (DbEntityValidationException ex)
            {
                foreach (var errorItems in ex.EntityValidationErrors)
                {
                    foreach (var errorinfo in errorItems.ValidationErrors)
                    {
                        errorMessage += string.Format("属性名:{0},错误消息:{1}", errorinfo.PropertyName, errorinfo.ErrorMessage)
+ Environment.NewLine;
                    }
                }
                throw new Exception(errorMessage, ex);
            }

        }

        public void Delete(TEntity entity)
        {
            try
            {

                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                this.Entities.Remove(entity);
                this.context.SaveChanges();

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var errorItems in ex.EntityValidationErrors)
                {
                    foreach (var errorinfo in errorItems.ValidationErrors)
                    {
                        errorMessage += string.Format("属性名:{0},错误消息:{1}", errorinfo.PropertyName, errorinfo.ErrorMessage)
+ Environment.NewLine;
                    }
                }
                throw new Exception(errorMessage, ex);
            }
        }
        private IDbSet<TEntity> Entities
        {
            get
            {
                if (entities == null)
                {
                    entities = context.Set<TEntity>();
                }
                return entities;
            }
        }
        public virtual IQueryable<TEntity> Table
        {
            get
            {
                return this.Entities;
            }
        }
        /// <summary>
        /// 根据实体取用户数据分组
        /// </summary>
        /// <param name="entity">APP类实体</param>
        /// <returns></returns>
        public List<Guid> GetGroups(TEntity entity)
        {
            List<Guid> AccessIds = new List<Guid>();
            List<int> groups = new List<int>();
            Dictionary<ICollection<int>, Guid> dictionary = new Dictionary<ICollection<int>, Guid>();
            //拿到用户组IDs
            HashSet<int> _groupIds = new HashSet<int>();
            foreach (var item in WeiChatApplicationContext.Current.WeiChatUser.GroupIds.Split(',').ToArray())
            {
                _groupIds.Add(Convert.ToInt32(item));
            };
            //取出实体的组IDs
            foreach (var item in this.entities)
            {
                foreach (var c in item.UserGroups.Split(',').ToList())
                {
                    groups.Add(Convert.ToInt32(c));
                }
                dictionary.Add(groups, item.Id);
            }
            foreach (var listGroup in dictionary)
            {
                //字典对的键集合与用户组取交集,如果有交集的话，允许访问
                if (listGroup.Key.Intersect(_groupIds).Count() > 0)
                {
                    //用户所拥有的组包含模块中组的GroupId允许访问
                    AccessIds.Add(listGroup.Value);
                }
            }
            return AccessIds;
        }
    }
    /// <summary>
    /// 创建对app类的工作单元
    /// </summary>
    public class UnitofWork : IDisposable
    {
        private readonly AppDbContext context;
        private bool disposed;
        private Dictionary<string, object> repositories;
        public UnitofWork(AppDbContext context)
        {
            this.context = context;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }
        public void Save()
        {
            context.SaveChanges();
        }
        /// <summary>
        /// 建立泛型仓储
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public Repository<TEntity> Repository<TEntity>() where TEntity : WeiChat_TenantBase<Guid>
        {
            if (repositories == null)
            {
                repositories = new Dictionary<string, object>();
            }
            var type = typeof(TEntity).Name;

            if (!repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), context);
                repositories.Add(type, repositoryInstance);
            }
            return (Repository<TEntity>)repositories[type];
        }
    }
}
