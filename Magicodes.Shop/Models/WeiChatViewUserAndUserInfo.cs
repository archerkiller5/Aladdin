using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magicodes.Shop.Models
{
    public class WeiChatViewUserAndUserInfo
    {
        public string openid;
        public WeiChat.Data.Models.User.User_Info Userinfo { get; set; }
        public WeiChat.Data.Models.WeiChat_User User { get; set; }
    }
}