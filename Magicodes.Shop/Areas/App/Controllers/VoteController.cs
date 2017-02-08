using Magicodes.Logger;
using Magicodes.Shop.Areas.App.Models;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Activity;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Areas.App.Controllers
{
    [RouteArea("App")]
    public class VoteController : AppBaseController
    {
        private readonly LoggerBase log = Loggers.Current.DefaultLogger;
        // GET: App/Vote
        ///// <summary>
        ///// 根据活动ID展示活动信息
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[WeChatOAuth]
        //[Route("App/Vote/Index/{id}")]
        //public ActionResult GetUpTopics(int? id)
        //{
        //    if (id == null)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    var activity_Survey = db.Activity_Surveys.Find(id);
        //    if (activity_Survey == null)
        //        return HttpNotFound();
        //    return View(activity_Survey);
        //}
        /// <summary>
        /// 根据活动ID展示投票选项
        /// </summary>
        /// <param name="surveyId">活动ID</param>
        /// <returns></returns>
        [HttpGet]
        [WeChatOAuth]
        public ActionResult Index(int? surveyId = null)
        {
            ViewBag.surveyId = surveyId;
            if (surveyId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //活动信息
            var activitySurvey = db.Activity_Surveys.Find(surveyId);
            if (activitySurvey == null)
                return HttpNotFound();
            var Activity_SurveyChoiceTopic = db.Activity_SurveyChoiceTopics.Include("Options").FirstOrDefault(p => p.SurveyId == surveyId);
            VoteViewModel vote = new VoteViewModel();
            //foreach (var option in Activity_SurveyChoiceTopic.Options)
            //{
            //    vote.VoteOption.Add(option as Activity_SurveyOption);
            //}
            //取出投票活动的所有选项
            vote.VoteOption = Activity_SurveyChoiceTopic.Options;
            vote.AllowNumberPerDay = activitySurvey.AllowNumberPerDay;
            vote.AllowNumberPerPerson = activitySurvey.AllowNumberPerPerson;
            vote.Title = activitySurvey.Title;
            vote.StartTime = activitySurvey.StartTime;
            vote.EndTime = activitySurvey.EndTime;
            vote.Voter = WeiChatApplicationContext.Current.WeiChatUser.NickName;
            return View(vote);
        }
        ///// <summary>
        ///// 获取投票选项
        ///// </summary>
        ///// <param name="surveyId"></param>
        ///// <param name="pageIndex"></param>x
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[WeChatOAuth]
        //public ActionResult GetVoteOptions(int? surveyId)
        //{
        //    if (surveyId == null)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    //活动信息
        //    var activitySurvey = db.Activity_Surveys.Find(surveyId);
        //    if (activitySurvey == null)
        //        return HttpNotFound();
        //    var Activity_SurveyChoiceTopic = db.Activity_SurveyChoiceTopics.FirstOrDefault(p => p.SurveyId == surveyId);
        //    var queryable = Activity_SurveyChoiceTopic.Options;
        //    return PartialView(queryable);
        //}
        /// <summary>
        /// 投票提交后保存投票结果
        /// </summary>
        /// <param name="surveyId">投票标题ID</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(VoteViewModel model,string[] OptionTexts=null, int? surveyId = null)
        {
            if (ModelState.IsValid)
            {
                //UpdateModel(model);
                var activitySurvey = db.Activity_Surveys.Find(surveyId);
                var activity_votes = new Activity_Vote();
                activity_votes.Title = activitySurvey.Title;
                activity_votes.Id = activitySurvey.Id;
                activity_votes.User = WeiChatApplicationContext.Current.WeiChatUser.NickName;
                activity_votes.VoteTime = DateTime.Now;
                if (OptionTexts!=null)
                {
                    for (int i = 0; i < OptionTexts.Length; i++)
                    {
                        var pt = new Activity_VoteOption();
                        pt.VoteId = surveyId;
                        pt.OptionText = OptionTexts[i];
                        db.Activity_VoteOptions.Add(pt);
                    }
                }
                db.Activity_Votes.Add(activity_votes);
                db.SaveChanges();
                var ajaxRes = new AjaxResponse() { Success = true, Message = "投票成功！" };
                return Json(ajaxRes);
            }
            var ajaxfalse = new AjaxResponse() { Success = false, Message = "投票失败！" };
            return Json(ajaxfalse);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}