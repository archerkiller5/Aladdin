// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Menu.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

namespace Magicodes.WeiChat.Data.Models.WeiChat
{
    /// <summary>
    ///     自定义菜单版本历史
    /// </summary>
    public class WeiChat_Menu : WeiChat_TenantBase<int>
    {
        public string MenuData { get; set; }
        public string Remark { get; set; }

        /// <summary>
        ///     是否为当前菜单
        /// </summary>
        public bool IsCurrent { get; set; }

        public string Result { get; set; }
    }
}