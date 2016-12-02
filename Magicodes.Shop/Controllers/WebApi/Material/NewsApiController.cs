// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : NewsApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Magicodes.Shop.Models;
using Magicodes.WeiChat.Unity.WeChat;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.GroupMessage;

namespace Magicodes.Shop.Controllers.WebApi.Material
{
    [RoutePrefix("api/news")]
    public class NewsApiController : WebApiControllerBase
    {
        [Route("{pageIndex}/{pageSize}")]
        // GET: api/News
        public async Task<IHttpActionResult> Get(int pageIndex = 1, int pageSize = 6)
        {
            var data = MediaApi.GetNewsMediaList(AccessToken, (pageIndex - 1)*pageSize, pageSize);
            var dataList = data.item
                .Select(p =>
                    new MaterialNewsViewModel
                    {
                        Id = p.media_id,
                        Title = p.content.news_item.First().title,
                        UpdateTime = p.update_time.ConvertToDateTime(),
                        ThumbMediaId = p.content.news_item.First().thumb_media_id,
                        Digest = p.content.news_item.First().digest,
                        Url = p.content.news_item.First().url
                    });
            var path = HttpContext.Current.Server.MapPath("~/MediaFiles");
            path = Path.Combine(path, "thumb");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (var item in dataList)
            {
                var mediaPath = Path.Combine(path, item.ThumbMediaId + ".jpg");
                if (!File.Exists(mediaPath))
                    using (var fs = File.Create(mediaPath))
                    {
                        MediaApi.GetForeverMedia(AccessToken, item.ThumbMediaId, fs);
                        fs.Close();
                    }
            }
            var pagedList = new DataPageListViewModel<MaterialNewsViewModel>(dataList, pageIndex, pageSize,
                data.total_count);
            return Ok(pagedList);
        }

        // GET: api/News/5
        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            var data = MediaApi.GetForeverNews(AccessToken, id);
            var model = new MaterialNewsViewModel
            {
                Id = id,
                Title = data.news_item.First().title,
                ThumbMediaId = data.news_item.First().thumb_media_id,
                Digest = data.news_item.First().digest,
                Url = data.news_item.First().url
            };
            var path = HttpContext.Current.Server.MapPath("~/MediaFiles");
            path = Path.Combine(path, "thumb");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var mediaPath = Path.Combine(path, model.ThumbMediaId + ".jpg");
            if (!File.Exists(mediaPath))
                using (var stream = File.Create(mediaPath))
                {
                    MediaApi.GetForeverMedia(AccessToken, model.ThumbMediaId, stream);
                }
            return Ok(model);
        }

        [HttpPost]
        [Route("")]
        // POST: api/News
        public async Task<IHttpActionResult> Post([FromBody] List<PostNewsViewModel> news)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var newsList = new List<NewsModel>();
            foreach (var model in news)
            {
                var newsModel = new NewsModel
                {
                    author = model.Author,
                    content = model.Text,
                    content_source_url = model.OriginalLink,
                    digest = model.Summary,
                    show_cover_pic = model.IsShowInText ? "1" : "0",
                    title = model.Title,
                    thumb_media_id = model.ThumbMediaId
                };
                newsList.Add(newsModel);
            }
            var result = MediaApi.UploadNews(AccessToken, Config.TIME_OUT, newsList.ToArray());
            if (result.errcode != ReturnCode.请求成功)
                return BadRequest(result.errmsg);
            return Ok(news);
        }

        // PUT: api/Material/5
        public void Put(int id, [FromBody] string value)
        {
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var result = MediaApi.DeleteForeverMedia(AccessToken, id);
            if (result.errcode != ReturnCode.请求成功)
                return BadRequest(result.errmsg);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}