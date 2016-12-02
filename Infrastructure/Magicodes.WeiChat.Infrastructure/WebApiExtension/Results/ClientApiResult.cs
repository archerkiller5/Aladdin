// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ClientApiResult.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:23
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Magicodes.WeiChat.Infrastructure.WebApiExtension.Results
{
    /// <summary>
    ///     获取WebApi结果
    /// </summary>
    public class ClientApiResult : IHttpActionResult
    {
        private readonly ApiController _controller;

        public ClientApiResult(ApiController controller, string baseUrl, string apiUrl, ApiTypes apiType = ApiTypes.Get,
            object data = null, string contentType = "application/json")
        {
            if (baseUrl == null)
                throw new ArgumentNullException("baseUrl");

            if (apiUrl == null)
                throw new ArgumentNullException("apiUrl");

            BaseUrl = baseUrl;
            ApiUrl = apiUrl;
            ContentType = contentType;
            ApiType = apiType;
            Data = data;
            _controller = controller;
        }

        public string BaseUrl { get; set; }
        public string ApiUrl { get; set; }
        public string ContentType { get; set; }

        public ApiTypes ApiType { get; set; }
        public object Data { get; set; }

        public HttpRequestMessage Request
        {
            get { return _controller.Request; }
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                switch (ApiType)
                {
                    case ApiTypes.Get:
                        return await client.GetAsync(ApiUrl);
                    case ApiTypes.Post:
                        return await client.PostAsJsonAsync(ApiUrl, Data);
                    case ApiTypes.Put:
                        return await client.PutAsJsonAsync(ApiUrl, Data);
                    case ApiTypes.Delete:
                        return await client.DeleteAsync(ApiUrl);
                    default:
                        break;
                }
                return await client.GetAsync(ApiUrl);
                //return await response.Content.ReadAsHttpResponseMessageAsync();
            }
        }
    }
}