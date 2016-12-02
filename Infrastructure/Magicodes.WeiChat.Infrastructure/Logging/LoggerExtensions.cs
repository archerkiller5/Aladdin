// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : LoggerExtensions.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using Magicodes.Logger;

namespace Magicodes.WeiChat.Infrastructure.Logging
{
    public static class LoggerExtensions
    {
        /// <summary>
        ///     记录异常日志
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="exception">异常</param>
        public static void LogException(this LoggerBase logger, Exception exception)
        {
            logger.Log(LoggerLevels.Error, exception);
            if (exception is AggregateException && (exception.InnerException != null))
            {
                var aggException = exception as AggregateException;
                if (aggException.InnerException is DbEntityValidationException)
                {
                    var sb = new StringBuilder();
                    var validationException = aggException.InnerException as DbEntityValidationException;
                    sb.AppendLine("实体验证错误。错误数：" + validationException.EntityValidationErrors.Count() + " ");
                    foreach (var validationResult in validationException.EntityValidationErrors)
                        foreach (var item in validationResult.ValidationErrors)
                            sb.AppendFormat("（{0}:{1}）", item.PropertyName, item.ErrorMessage);
                    logger.Log(LoggerLevels.Error, sb.ToString());
                }
            }
            else if (exception is DbEntityValidationException)
            {
                var sb = new StringBuilder();
                var validationException = exception as DbEntityValidationException;
                sb.AppendLine("实体验证错误。错误数：" + validationException.EntityValidationErrors.Count() + " ");
                foreach (var validationResult in validationException.EntityValidationErrors)
                    foreach (var item in validationResult.ValidationErrors)
                        sb.AppendFormat("（{0}:{1}）", item.PropertyName, item.ErrorMessage);
                logger.Log(LoggerLevels.Error, sb.ToString());
            }

            else if (exception is ReflectionTypeLoadException)
            {
                var refEx = exception as ReflectionTypeLoadException;
                if (refEx.LoaderExceptions.Length <= 0) return;
                foreach (var loaderEx in refEx.LoaderExceptions)
                    LogException(logger, loaderEx);
            }
        }
    }
}