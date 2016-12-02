// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ConfigBase.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;

namespace Magicodes.WeiChat.Infrastructure.Config
{
    public abstract class ConfigBase
    {
        public ConfigBase()
        {
            UpdateTime = DateTime.Now;
        }

        public DateTime UpdateTime { get; set; }
    }
}