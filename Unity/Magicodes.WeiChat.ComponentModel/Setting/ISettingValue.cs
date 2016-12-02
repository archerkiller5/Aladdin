using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.ComponentModel.Setting
{
    public interface ISettingValue
    {
        string Name { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        SettingScopes Scopes { get; set; }
        string Value { get; set; }
        bool IsVisibleToClients { get; set; }
        string CustomData { get; set; }
    }
}
