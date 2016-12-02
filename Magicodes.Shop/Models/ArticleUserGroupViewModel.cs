
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Site;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.Shop.Models
{
    public class ArticleUserGroupViewModel
    {
        public Site_Article Article { get; set; }
        public List<WeiChat_UserGroup> AllGroups { get; set; }
        public List<WeiChat_UserGroup> ArticleGroups { get; set; }
        public int[] SelectedGroupIds { get; set; }

    }
}