using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models.Activity;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Filters;
using NLog;

namespace Magicodes.Shop.Areas.App.Controllers
{
    [RouteArea("App")]
    public class ActivityController : AppBaseController
    {
        // GET: App/Activity
        [WeChatOAuth]
        public ActionResult Lottery(int id)
        {
            var model = db.Activity_Lotteries.Include(p => p.PrizeOptions).First(p => p.Id == id);
            if (!model.IsEnable)
            {
                return ErrorTip("温馨提示", "该活动不存在或已关闭！");
            }
            if (DateTime.Now < model.StartTime)
            {
                return WarnTip("温馨提示", "活动暂未开始，请耐心等待！");
            }
            if (model.EndTime != null && DateTime.Now > model.EndTime.Value)
            {
                return WarnTip("温馨提示", "很抱歉，该活动已经结束了！");
            }
            //每天最多抽奖次数
            if (model.AllowNumberPerDay > 0)
            {
                var date = DateTime.Now.Date.AddDays(-1);
                var count = db.Personal_Lotteries.Count(p => p.OpenId == WeiChatUser.OpenId && p.LotteryId == id && p.CreateTime >= date);
                if (count >= model.AllowNumberPerDay)
                {
                    return WarnTip("温馨提示", "您今天抽奖的机会已经用完啦，明天再来吧！");
                }
            }
            var openId = WeiChatUser.OpenId;
            ViewBag.OpenId = openId;
            //每人总共最多抽奖次数
            if (model.AllowNumberPerPerson > 0)
            {
                var count = db.Personal_Lotteries.Count(p => p.OpenId == openId && p.LotteryId == id);
                if (count >= model.AllowNumberPerPerson)
                {
                    return WarnTip("温馨提示", "您抽奖的机会已经用完啦，敬请关注我们后续的活动哦！");
                }
            }
            var rd = new Random();
            var num = rd.Next(1, model.ExpectedNumber);
            ViewBag.IsWinning = false;
            ViewBag.Result = "谢谢参与";
            ViewBag.Postion = -1;
            var logger = LogManager.GetCurrentClassLogger();
            var current = 0;
            for (int index = 0; index < model.PrizeOptions.Count; index++)
            {
                var option = model.PrizeOptions[index];
                if (option.PrizeCount <= 0)
                    continue;
                current += option.OverCount;
                if (option.OverCount <= 0 || num > current) continue;
                logger.Debug("抽奖结果：RandNo:" + num + "JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(option));
                ViewBag.Postion = index + 1;
                ViewBag.IsWinning = true;
                ViewBag.Result = string.Format("{1}（{0}）", option.Prize, option.Title);
                Session["Prize_Lv"] = option;
                break;
            }
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                model.Description = model.Description.Replace("\n\r", "<br/>").Replace("\n", "<br/>");
            }
            Session["Lottery"] = model.Id;
            switch (model.LotteryType)
            {
                case LotteryTypes.BigWheel:
                    {
                        if (model.PrizeOptions.Count < 5)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                model.PrizeOptions.Add(new Activity_LotteryPrizeOption()
                                {
                                    Title = "谢谢参与",
                                    Prize = "谢谢参与"
                                });
                            }
                        }
                        model.PrizeOptions.Add(new Activity_LotteryPrizeOption()
                        {
                            Title = "谢谢参与",
                            Prize = "谢谢参与"
                        });
                        if (ViewBag.Postion == -1)
                            ViewBag.Postion = model.PrizeOptions.Count;
                        return View("BigWheel", model);
                    }
                case LotteryTypes.HitGoldenEggs:
                    return View("HitGoldenEggs", model);
                case LotteryTypes.Scratch:
                    return View("Scratch", model);
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        [WeChatOAuth]
        public ActionResult Survey(int id)
        {
            var model = db.Activity_Surveys.First(p => p.Id == id);
            if (!model.IsEnable)
            {
                return ErrorTip("温馨提示", "该活动不存在或已关闭！");
            }
            if (DateTime.Now < model.StartTime)
            {
                return WarnTip("温馨提示", "活动暂未开始，请耐心等待！");
            }
            if (model.EndTime != null && DateTime.Now > model.EndTime.Value)
            {
                return WarnTip("温馨提示", "很抱歉，该活动已经结束了！");
            }
            //每天最多抽奖次数
            if (model.AllowNumberPerDay > 0)
            {
                var date = DateTime.Now.Date.AddDays(-1);
                var count = db.Personal_Surveys.Count(p => p.OpenId == WeiChatUser.OpenId && p.SurveyId == id && p.CreateTime >= date);
                if (count >= model.AllowNumberPerDay)
                {
                    return WarnTip("温馨提示", model.AllowNumberPerPerson == 1 ? "您已提交成功，只能提交一次哦！" : "您今天的机会已经用完啦，明天再来吧！");
                }
            }
            var openId = WeiChatUser.OpenId;
            ViewBag.OpenId = openId;
            //每人总共最多抽奖次数
            if (model.AllowNumberPerPerson > 0)
            {
                var count = db.Personal_Surveys.Count(p => p.OpenId == openId && p.SurveyId == id);
                if (count >= model.AllowNumberPerPerson)
                {
                    return WarnTip("温馨提示", "您的机会已经用完啦，敬请关注我们后续的活动哦！");
                }
            }
            return View(model);
        }
    }
}