// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : BatchOperationExtend.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using EntityFramework.Utilities;

namespace Magicodes.WeiChat.Data.BatchOperation
{
    public static class BatchOperationExtend
    {
        /// <summary>
        ///     根据条件批量删除
        /// </summary>
        /// <typeparam name="TContext"><see cref="DbContext" />DbContext</typeparam>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="db"><see cref="DbContext" /> EF DbContext</param>
        /// <param name="dbSet"><see cref="IDbSet{TEntity}" />EF实体集合</param>
        /// <param name="whereExp">筛选表达式</param>
        /// <returns>删除的行数</returns>
        public static int BathRemoveBy<TContext, TEntity>(this TContext db, IDbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> whereExp)
            where TContext : DbContext
            where TEntity : class
        {
            var bt = EFBatchOperation.For(db, dbSet);
            return bt.Where(whereExp).Delete();
        }

        /// <summary>
        ///     批量插入
        /// </summary>
        /// <typeparam name="TContext"><see cref="DbContext" />DbContext</typeparam>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="db"><see cref="DbContext" /> EF DbContext</param>
        /// <param name="dbSet"><see cref="IDbSet{TEntity}" />EF实体集合</param>
        /// <param name="items">实体集合</param>
        public static void BathInsert<TContext, TEntity>(this TContext db, IDbSet<TEntity> dbSet,
            IEnumerable<TEntity> items)
            where TContext : DbContext
            where TEntity : class
        {
            var bt = EFBatchOperation.For(db, dbSet);
            bt.InsertAll(items);
        }

        /// <summary>
        ///     根据条件批量更新
        /// </summary>
        /// <typeparam name="TP"></typeparam>
        /// <typeparam name="TContext"><see cref="DbContext" />DbContext</typeparam>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="db"><see cref="DbContext" /> EF DbContext</param>
        /// <param name="dbSet"><see cref="IDbSet{TEntity}" />EF实体集合</param>
        /// <param name="whereExp">筛选表达式</param>
        /// <param name="propExp">原字段表达式</param>
        /// <param name="modifierExp">目标字段表达式</param>
        /// <returns>修改的行数</returns>
        public static int BathUpdateBy<TContext, TEntity, TP>(this TContext db, IDbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> whereExp, Expression<Func<TEntity, TP>> propExp,
            Expression<Func<TEntity, TP>> modifierExp)
            where TContext : DbContext
            where TEntity : class
        {
            var bt = EFBatchOperation.For(db, dbSet);
            return bt.Where(whereExp).Update(propExp, modifierExp);
        }
    }
}