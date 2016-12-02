using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Unity
{
    public static class ObjectUtils
    {
        public static T GetAttribute<T>(this ICustomAttributeProvider provider, bool inherit = false)
where T : Attribute
        {
            return provider
                .GetCustomAttributes(typeof(T), inherit)
                .OfType<T>()
                .FirstOrDefault();
        }
    }
}
