using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Unity.WebRequests
{
    public class WeChatApiWebRequestHelper : WebRequestHelper
    {
        public WeChatApiWebRequestHelper()
        {
            this.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
            this.ContentType = "application/json";
            this.AcceptLanguage = "zh-cn";
        }
    }
}
