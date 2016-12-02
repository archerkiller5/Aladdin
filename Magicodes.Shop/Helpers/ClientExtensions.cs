using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace Magicodes.Shop.Helpers
{
    public static class ClientExtensions
    {
        /// <summary>
        ///     获取浏览器信息
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetBrowserInfo(this HttpContextBase httpContext)
        {
            return httpContext.Request.Browser.Browser + " / " +
                   httpContext.Request.Browser.Version + " / " +
                   httpContext.Request.Browser.Platform;
        }

        /// <summary>
        ///     获取客户端IP信息
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetClientIpAddress(this HttpContextBase httpContext)
        {
            var clientIp = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                           httpContext.Request.ServerVariables["REMOTE_ADDR"];

            try
            {
                foreach (var hostAddress in Dns.GetHostAddresses(clientIp))
                    if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                        return hostAddress.ToString();
                foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
                    if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                        return hostAddress.ToString();
            }
            catch (Exception)
            {
            }

            return clientIp;
        }

        /// <summary>
        ///     获取电脑名称
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetComputerName(this HttpContextBase httpContext)
        {
            if (!httpContext.Request.IsLocal)
                return null;

            try
            {
                var clientIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                               HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                return Dns.GetHostEntry(IPAddress.Parse(clientIp)).HostName;
            }
            catch
            {
                return null;
            }
        }
    }
}