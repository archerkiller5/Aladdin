using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeiChat.Data.Models.Activity;
using NLog;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/Activity")]
    public class ActivityApiController : TenantBaseApiController<Activity_Base>
    {
        private static readonly object LockObj = new object();
        [HttpGet]
        [Route("Lottery/{openId}")]
        public IHttpActionResult Lottery(string openId)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var lotteryId = HttpContext.Current.Session["Lottery"] as int?;
            if (lotteryId == null)
            {
                return NotFound();
            }
            var model = db.Activity_Lotteries.Include(p => p.PrizeOptions).First(p => p.Id == lotteryId.Value);
            HttpContext.Current.Session["Lottery"] = null;
            if (!model.IsEnable)
            {
                return BadRequest("该活动不存在或已关闭！");
            }
            if (DateTime.Now < model.StartTime)
            {
                return BadRequest("活动暂未开始，请耐心等待！");
            }
            if (model.EndTime != null && DateTime.Now > model.EndTime.Value)
            {
                return BadRequest("很抱歉，该活动已经结束了！");
            }
            //每天最多抽奖次数
            if (model.AllowNumberPerDay > 0)
            {
                var date = DateTime.Now.Date.AddDays(-1);
                var count = db.Personal_Lotteries.Count(p => p.OpenId == openId && p.LotteryId == lotteryId.Value && p.CreateTime >= date);
                if (count >= model.AllowNumberPerDay)
                {
                    return BadRequest("您今天抽奖的机会已经用完啦，明天再来吧！");
                }
            }
            //每人总共最多抽奖次数
            if (model.AllowNumberPerPerson > 0)
            {
                var count = db.Personal_Lotteries.Count(p => p.OpenId == openId && p.LotteryId == lotteryId.Value);
                if (count >= model.AllowNumberPerPerson)
                {
                    return BadRequest("您抽奖的机会已经用完啦，敬请关注我们后续的活动哦！");
                }
            }
            var myLottery = new Personal_Lottery()
            {
                CreateTime = DateTime.Now,
                LotteryId = lotteryId.Value,
                OpenId = openId,
                TenantId = TenantId,
                IsWinning = false
            };
            if (HttpContext.Current.Session["Prize_Lv"] != null)
            {
                myLottery.IsWinning = true;
                var prize = HttpContext.Current.Session["Prize_Lv"] as Activity_LotteryPrizeOption;
                lock (LockObj)
                {
                    if (prize != null)
                    {
                        var prizeOption = db.Activity_LotteryPrizeOptions.Find(prize.Id);
                        prizeOption.OverCount--;
                        myLottery.Prize = prizeOption.Prize;
                        myLottery.LotteryPrizeOptionId = prizeOption.Id;
                        db.Personal_Lotteries.Add(myLottery);
                        db.SaveChanges();
                    }
                    HttpContext.Current.Session.Remove("Prize_Lv");
                    //需要完善信息
                    if (!db.User_Infos.Any(p => p.OpenId == openId))
                    {
                        return Ok(new { targetUrl = "/App/Personal/FillPersonalInfo?TenantId=" + TenantId });
                    }
                }
            }
            return Ok();
        }

        [HttpGet]
        [Route("Survey/{surveyId}/{openId}")]
        public IHttpActionResult Survey(int surveyId, string openId)
        {
            var model = db.Activity_Surveys.Find(surveyId);
            if (!model.IsEnable)
            {
                return BadRequest("该活动不存在或已关闭！");
            }
            if (DateTime.Now < model.StartTime)
            {
                return BadRequest("活动暂未开始，请耐心等待！");
            }
            if (model.EndTime != null && DateTime.Now > model.EndTime.Value)
            {
                return BadRequest("很抱歉，该活动已经结束了！");
            }
            //每天最多抽奖次数
            if (model.AllowNumberPerDay > 0)
            {
                var date = DateTime.Now.Date.AddDays(-1);
                var count = db.Personal_Surveys.Count(p => p.OpenId == openId && p.SurveyId == surveyId && p.CreateTime >= date);
                if (count >= model.AllowNumberPerDay)
                {
                    return BadRequest("您今天的机会已经用完啦，明天再来吧！");
                }
            }
            //每人总共最多抽奖次数
            if (model.AllowNumberPerPerson > 0)
            {
                var count = db.Personal_Surveys.Count(p => p.OpenId == openId && p.SurveyId == surveyId);
                if (count >= model.AllowNumberPerPerson)
                {
                    return BadRequest("您的机会已经用完啦，敬请关注我们后续的活动哦！");
                }
            }
            var topics = new List<Activity_SurveyTopicBase>();
            topics.AddRange(db.Activity_EssayQuestionTopics.Where(p => p.SurveyId == surveyId).ToList());
            topics.AddRange(db.Activity_SurveyChoiceTopics.Include(p => p.Options).Where(p => p.SurveyId == surveyId).ToList());
            return Ok(topics.OrderBy(p => p.Order));
        }

        [HttpPost]
        [Route("Survey/{surveyId}/{openId}")]
        public IHttpActionResult SubmitSurvey(int surveyId, string openId, List<dynamic> list)
        {
            var model = db.Activity_Surveys.Find(surveyId);
            if (!model.IsEnable)
            {
                return BadRequest("该活动不存在或已关闭！");
            }
            if (DateTime.Now < model.StartTime)
            {
                return BadRequest("活动暂未开始，请耐心等待！");
            }
            if (model.EndTime != null && DateTime.Now > model.EndTime.Value)
            {
                return BadRequest("很抱歉，该活动已经结束了！");
            }
            //每天最多抽奖次数
            if (model.AllowNumberPerDay > 0)
            {
                var date = DateTime.Now.Date.AddDays(-1);
                var count = db.Personal_Surveys.Count(p => p.OpenId == openId && p.SurveyId == surveyId && p.CreateTime >= date);
                if (count >= model.AllowNumberPerDay)
                {
                    return BadRequest("您今天的机会已经用完啦，明天再来吧！");
                }
            }
            //每人总共最多抽奖次数
            if (model.AllowNumberPerPerson > 0)
            {
                var count = db.Personal_Surveys.Count(p => p.OpenId == openId && p.SurveyId == surveyId);
                if (count >= model.AllowNumberPerPerson)
                {
                    return BadRequest("您的机会已经用完啦，敬请关注我们后续的活动哦！");
                }
            }
            var mySurvey = new Personal_Survey()
            {
                CreateTime = DateTime.Now,
                SurveyId = surveyId,
                OpenId = openId,
                TenantId = TenantId,
            };
            db.Personal_Surveys.Add(mySurvey);
            foreach (var item in list)
            {
                var answer = new Personal_SurveyAnswer()
                {
                    TopicType = (SurveyTopicTypes)item.type,
                    CreateTime = DateTime.Now,
                    OpenId = openId,
                    OptionId = item.optionId,
                    SurveyId = surveyId,
                    TenantId = TenantId,
                    TopicId = item.topicId,
                    Answer = item.answer
                };
                db.Personal_SurveyAnswers.Add(answer);
            }
            lock (LockObj)
            {
                db.SaveChanges();
            }
            return Ok();
        }
    }
}
