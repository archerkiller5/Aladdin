using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Magicodes.WeiChat.ComponentModel;

namespace Magicodes.WeiChat.Autofac
{
    public class ContainerConfig
    {
        /// <summary>
        /// 注册容器
        /// </summary>
        /// <param name="registerAction">注册函数</param>
        public static void RegisterAll(Action<ContainerBuilder> registerAction)
        {
            var builder = new ContainerBuilder();
            

            // 获取所有相关类库的程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p=>p.FullName.StartsWith("Magicodes")).ToArray();
            var baseType = typeof(IDependency);

            builder.RegisterAssemblyTypes(assemblies)
           .Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract)
           .AsImplementedInterfaces().InstancePerLifetimeScope();//InstancePerLifetimeScope 保证对象生命周期基于请求

            //此处会覆盖上面注册的内容
            registerAction.Invoke(builder);

            ContainerManager.Current.Container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(ContainerManager.Current.Container));
        }
    }
}
