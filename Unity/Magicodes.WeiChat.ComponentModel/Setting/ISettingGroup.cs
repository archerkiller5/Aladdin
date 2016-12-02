using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.ComponentModel.Setting
{
    public interface ISettingGroup
    {
        string Name { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        SettingScopes Scopes { get; set; }
        bool IsVisibleToClients { get; set; }
        ISettingGroup ParentGroup { get; set; }
    }
}
