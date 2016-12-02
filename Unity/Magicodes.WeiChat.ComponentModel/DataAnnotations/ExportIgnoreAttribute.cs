using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 忽略导出
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExportIgnoreAttribute : Attribute
    {
    }
}
