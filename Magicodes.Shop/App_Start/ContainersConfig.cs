// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ContainersConfig.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using Magicodes.WeiChat.Autofac;

namespace Magicodes.Shop
{
    public class ContainersConfig
    {
        public static void RegisterAll()
        {
            ContainerConfig.RegisterAll(builder =>
            {
                #region Demo

                //builder.RegisterType<EmailService>().As<IEmailService>();
                //builder.RegisterType<SmsService>().As<ISmsService>(); 
                #endregion
            });
        }
    }
}