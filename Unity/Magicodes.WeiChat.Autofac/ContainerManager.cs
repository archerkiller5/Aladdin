using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Magicodes.WeiChat.Unity;

namespace Magicodes.WeiChat.Autofac
{
    /// <summary>
    /// 容器管理器
    /// </summary>
    public class ContainerManager : ThreadSafeLazyBaseSingleton<ContainerManager>
    {
        /// <summary>
        /// 容器
        /// </summary>
        public IContainer Container { get; set; }
        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>类型实例</returns>
        public T Resolve<T>(IEnumerable<Parameter> parameters = null)
        {
            return parameters == null ? Container.Resolve<T>() : Container.Resolve<T>(parameters);
        }
        public T Resolve<T>(params Parameter[] parameters)
        {
            return parameters == null ? Container.Resolve<T>() : Container.Resolve<T>(parameters);
        }
    }
}
