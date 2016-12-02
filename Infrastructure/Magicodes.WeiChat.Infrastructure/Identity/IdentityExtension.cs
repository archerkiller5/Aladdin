// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : IdentityExtension.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Security.Principal;
using Magicodes.WeiChat.Data.Models.Interface;

namespace Magicodes.WeiChat.Infrastructure.Identity
{
    public static class IdentityExtension
    {
        public static ITenant<int> GetTenantInfo(this IIdentity identity)
        {
            return WeiChatApplicationContext.Current.TenantInfo;
        }
    }
}