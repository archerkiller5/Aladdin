// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : HomeController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Magicodes.ECharts;
using Magicodes.ECharts.Axis;
using Magicodes.ECharts.CommonDefinitions;
using Magicodes.ECharts.Components.Title;
using Magicodes.ECharts.Mvc;
using Magicodes.ECharts.Mvc.Results;
using Magicodes.ECharts.Series;
using Magicodes.ECharts.Series.Mark;
using Magicodes.ECharts.ValueTypes;
using Magicodes.Shop.Models;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Unity;
using Senparc.Weixin.MP.AdvancedAPIs;
using Magicodes.Mvc.RoleMenuFilter;

namespace Magicodes.Shop.Controllers
{
    [Authorize]
    [RoleMenuFilter("仪表盘", "{99A31780-B8C5-4AF6-ACBB-4598C8A60661}", "Admin,TenantManager,ShopManager",
     iconCls: "fa fa-dashboard", orderNo: -100)]
    public class HomeController : TenantBaseController<WeiChat_App>
    {
        //
        // GET: /Home/
        [RoleMenuFilter("首页", "{79FD819B-976E-441C-AF57-DB9AB963BBDD}", "Admin,TenantManager,ShopManager",
                     url: "/", parentId: "{99A31780-B8C5-4AF6-ACBB-4598C8A60661}",iconCls: "fa fa-home")]
        public ActionResult Index()
        {
            //如果没有配置租户信息，则必须前去配置
            if (!HasConfigWeiChat && IsSystemIenant)
                return RedirectToAction("WeiChatAppConfig", "Account_Tenant",
                    new { area = "SystemAdmin", tenantId = TenantId });

            #region 获取最新关注的用户

            var lastUsers = GetLastUsers(8);
            ViewBag.LastUsers = lastUsers;

            #endregion

            var cache = WeiChat.Infrastructure.Cache.CacheManager.Current;

            #region 累积关注数

            {
                var value = cache.GetByTenant<int>("UserSummaryCount");
                if (value == default(int))
                    value = SafeReturnHelper.SafeReturn(() =>
                    {
                        //TODO:切换SDK
                        var userCumulateData = AnalysisApi.GetUserCumulate(WeChatConfigManager.Current.GetAccessToken(),
                            DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd"),
                            DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));

                        if ((userCumulateData.list != null) && (userCumulateData.list.Count > 0))
                            return Convert.ToInt32(userCumulateData.list.First().cumulate_user);
                        return default(int);
                    });
                cache.AddOrUpdateByTenant("UserSummaryCount", value, TimeSpan.FromHours(12));
                ViewBag.UserSummaryCount = value;
            }

            #endregion

            #region 取消关注数、新关注数、净增用户数

            {
                //取消关注数
                var cancelUserCount = cache.GetByTenant<int>("CancelUserCount");
                //新关注数
                var newUserCount = cache.GetByTenant<int>("NewUserCount");
                //净增用户数
                var growUserCount = cache.GetByTenant<int>("GrowUserCount");
                if (newUserCount == default(int))
                {
                    var value =
                        SafeReturnHelper.SafeReturn(
                            () =>
                            {
                                //TODO:切换SDK
                                return AnalysisApi.GetUserSummary(WeChatConfigManager.Current.GetAccessToken(),
                                    DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd"),
                                    DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                            });
                    if (value != null)
                    {
                        cancelUserCount = value.list.Select(p => Convert.ToInt32(p.cancel_user)).Sum();
                        newUserCount = value.list.Select(p => Convert.ToInt32(p.new_user)).Sum();
                        growUserCount = Convert.ToInt32(newUserCount) - Convert.ToInt32(cancelUserCount);
                        cache.AddOrUpdateByTenant("CancelUserCount", cancelUserCount, TimeSpan.FromHours(12));
                        cache.AddOrUpdateByTenant("NewUserCount", newUserCount, TimeSpan.FromHours(12));
                        cache.AddOrUpdateByTenant("GrowUserCount", growUserCount, TimeSpan.FromHours(12));
                    }
                }
                //取消关注数
                ViewBag.CancelUserCount = cancelUserCount;
                //新关注数
                ViewBag.NewUserCount = newUserCount;
                //净增用户数
                ViewBag.GrowUserCount = growUserCount;
            }

            #endregion

            ViewBag.ChartEnable = false;
            //如果粉丝量大于20万，禁用部分图表 
            if (cache.GetByTenant<int>("UserSummaryCount") < 200000)
                if (db.WeiChat_Users.Count() < 200000)
                    ViewBag.ChartEnable = true;
            return View();
        }

        [HttpGet]
        public JavaScriptJsonResult UserTimeChart()
        {
            var seriesData = new List<object>();
            var xAxisData = new List<object>();
            for (var i = 0; i < 6; i++)
            {
                var currentTime = DateTime.Now.AddMonths(-i);
                xAxisData.Add(currentTime.ToString("MM月"));
                seriesData.Add(
                    db.WeiChat_Users.AsNoTracking()
                        .Where(
                            p =>
                                (p.SubscribeTime.Year == currentTime.Year) &&
                                (p.SubscribeTime.Month == currentTime.Month))
                        .Count()
                        .ToString());
            }
            var chartOptions = new EChartsOption
            {
                Title = new Title("半年公众号关注变化") { Left = new AlignValue(Align.center) },
                Series = new Series[]
                {
                    new LineSeries
                    {
                        Name = "半年公众号关注变化",
                        Data = seriesData,
                        Smooth = true,
                        MarkPoint = new MarkPoint
                        {
                            Data = new List<MarkData>
                            {
                                new MarkData {Type = MarkPointDataTypes.max, Name = "最大值"},
                                new MarkData {Type = MarkPointDataTypes.min, Name = "最小值"}
                            }
                        },
                        MarkLine = new MarkLine
                        {
                            Data = new List<MarkData>
                            {
                                new MarkData {Type = MarkPointDataTypes.average, Name = "平均值"}
                            }
                        }
                    }
                },
                XAxis = new XAxis[1] { new XAxis { Type = AxisTypes.category, Data = xAxisData } },
                YAxis =
                    new YAxis[1] { new YAxis { Type = AxisTypes.value, AxisLabel = new Label { Formatter = "{value} 人" } } }
            };
            return this.ToEChartResult(chartOptions);
        }

        /// <summary>
        ///     获取粉丝分布统计数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JavaScriptJsonResult UserAreaChart()
        {
            var provinces =
                db.WeiChat_Users.AsNoTracking()
                    .Where(p => p.Subscribe)
                    .Select(p => p.Province)
                    .Distinct()
                    .ToList()
                    .Select(p => (object)p)
                    .ToList();
            var valueList = new List<object>();
            foreach (var item in provinces)
                valueList.Add(db.WeiChat_Users.Where(p => p.Subscribe && (p.Province == item)).Count().ToString());
            var chartOptions = new EChartsOption
            {
                Title = new Title("粉丝分布统计图") { Left = new AlignValue(Align.center) },
                Series = new Series[]
                {
                    new BarSeries
                    {
                        Name = "粉丝分布",
                        Data = valueList,
                        MarkPoint = new MarkPoint
                        {
                            Data = new List<MarkData>
                            {
                                new MarkData {Type = MarkPointDataTypes.max, Name = "最大值"},
                                new MarkData {Type = MarkPointDataTypes.min, Name = "最小值"}
                            }
                        },
                        MarkLine = new MarkLine
                        {
                            Data = new List<MarkData>
                            {
                                new MarkData {Type = MarkPointDataTypes.average, Name = "平均值"}
                            }
                        }
                    }
                },
                XAxis = new XAxis[1] { new XAxis { Type = AxisTypes.category, Data = provinces } },
                YAxis = new YAxis[1] { new YAxis { Type = AxisTypes.value } }
            };
            return this.ToEChartResult(chartOptions);
        }

        private List<HomeUserViewModel> GetLastUsers(int top)
        {
            var styles = new[] { "success", "info", "primary", "default", "success", "info", "primary", "default" };
            var last5Users =
                db.WeiChat_Users.Where(p => p.Subscribe)
                    .OrderByDescending(p => p.SubscribeTime)
                    .Take(top)
                    .ToList()
                    .Select(p => new HomeUserViewModel
                    {
                        NickName = p.NickName,
                        SubscribeTime = GetTimeStr(p.SubscribeTime)
                    }).ToList();
            for (var i = 0; i < last5Users.Count; i++)
                last5Users[i].Style = styles[i];
            return last5Users;
        }

        private string GetTimeStr(DateTime time)
        {
            if (DateTime.Now.AddHours(-3) < time)
            {
                return "刚刚";
            }
            if (DateTime.Now.Date == time.Date)
            {
                return "今天";
            }
            if (DateTime.Now.AddDays(-3) < time)
            {
                return "3天内";
            }
            if (DateTime.Now.AddDays(-7) < time)
            {
                return "7天内";
            }
            if (DateTime.Now.AddDays(-30) < time)
            {
                return "30天内";
            }
            return "最近";
        }

        /// <summary>
        ///     500
        /// </summary>
        /// <returns></returns>
        [Route("Error")]
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        ///     404
        /// </summary>
        /// <returns></returns>
        [Route("NotFoundError")]
        public ActionResult NotFoundError()
        {
            return View();
        }
    }
}