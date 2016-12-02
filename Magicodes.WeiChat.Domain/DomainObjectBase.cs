using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Interface;
using Magicodes.WeiChat.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Domain
{
    public abstract class DomainObjectBase : IDisposable
    {
        /// <summary>
        /// 获取当前用户Id
        /// </summary>
        public string UserId
        {
            get
            {
                return WeiChatApplicationContext.Current.UserId;
            }
        }

        public int TenantId
        {
            get
            {
                return WeiChatApplicationContext.Current.TenantId;
            }
        }

        protected AppDbContext Db;
        protected bool isToDisposeDb = false;
        public DomainObjectBase(AppDbContext context = null)
        {
            if (context == null)
            {
                Db = new AppDbContext();
                isToDisposeDb = true;
            }
            else
                Db = context;
        }

        /// <summary>
        /// 设置模型
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="model"></param>
        /// <param name="key"></param>
        /// <returns>是否为添加</returns>
        protected bool SetModel<TModel, Tkey>(TModel model, Tkey key)
          where TModel : class, new()
        {
            //判断是否为默认值
            if (EqualityComparer<Tkey>.Default.Equals(key, default(Tkey)))
            {
                var tmpCreate = model as IAdminCreate<string>;
                if (tmpCreate != null)
                {
                    tmpCreate.CreateBy = UserId;
                    tmpCreate.CreateTime = DateTime.Now;
                }

                var tmpTenantId = model as ITenantId;
                if (tmpTenantId != null)
                {
                    tmpTenantId.TenantId = TenantId;
                }
                return true;
            }
            else
            {
                Db.Set<TModel>().Attach(model);
                //取数据库值
                var databaseValues = Db.Entry(model).GetDatabaseValues();
                var tmpCreate = model as IAdminCreate<string>;
                if (tmpCreate != null)
                {
                    tmpCreate.CreateBy = databaseValues.GetValue<string>("CreateBy");
                    tmpCreate.CreateTime = databaseValues.GetValue<DateTime>("CreateTime");
                }

                var tmpTenantId = model as ITenantId;
                if (tmpTenantId != null)
                {
                    tmpTenantId.TenantId = databaseValues.GetValue<int>("TenantId"); ;
                }

                var tmpUpdate = model as IAdminUpdate<string>;
                if (tmpUpdate != null)
                {
                    tmpUpdate.UpdateTime = DateTime.Now;
                    tmpUpdate.UpdateBy = UserId;
                }
                return false;
            }
        }

        public void Dispose()
        {
            if (isToDisposeDb)
            {
                Db.Dispose();
            }
        }
    }
}
