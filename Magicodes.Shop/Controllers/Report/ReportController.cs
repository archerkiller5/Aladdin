// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ReportController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:11
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
using Magicodes.ECharts.Components.Legend;
using Magicodes.ECharts.Components.Title;
using Magicodes.ECharts.Components.ToolBox;
using Magicodes.ECharts.Components.ToolTip;
using Magicodes.ECharts.Mvc;
using Magicodes.ECharts.Mvc.Results;
using Magicodes.ECharts.Series;
using Magicodes.ECharts.Series.Mark;
using Magicodes.ECharts.ValueTypes;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Order;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;

namespace Magicodes.Shop.Controllers.Report
{
    [RoleMenuFilter("业务统计", "331BF9AD-EC05-4DE3-9655-FBE5B8104AA9", "Admin,TenantManager,ShopManager",
         iconCls: "fa fa-pie-chart")]
    public class ReportController : TenantBaseController<WeiChat_App>
    {
        [RoleMenuFilter("统计图表", "9576EBE2-BF9C-45EC-9034-756804F1D994", "Admin,TenantManager,ShopManager",
             url: "/Report", parentId: "331BF9AD-EC05-4DE3-9655-FBE5B8104AA9")]
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///     订单30天统计图表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JavaScriptJsonResult OrderMouthReport()
        {
            var seriesData_number = new List<object>();
            var seriesData_money = new List<object>();
            var xAxisData_rq = new List<object>();
            var Lst_Order = db.Order_Infos.AsNoTracking().ToList();
            for (var i = 0; i < 30; i++)
            {
                var currentTime = DateTime.Now.AddDays(-i);
                xAxisData_rq.Add(currentTime.ToString("dd号"));
                var Lst = Lst_Order.Where(p => (p.CreateTime.ToString("yyyy-MM-dd") ==
                                                currentTime.ToString("yyyy-MM-dd")) &&
                                               ((p.State != EnumOrderStatus.Obligation) ||
                                                (p.State != EnumOrderStatus.ReturnedGoods) ||
                                                (p.State != EnumOrderStatus.UnpaidDelete))).ToList();

                seriesData_number.Add(Lst.Count().ToString());
                seriesData_money.Add(Lst.Sum(p => p.TotalPrice).ToString());
            }

            var x_rq = new XAxis();
            x_rq.Type = AxisTypes.category;
            x_rq.Data = xAxisData_rq;

            var y_number = new YAxis();
            y_number.Type = AxisTypes.value;
            y_number.AxisLabel = new Label { Formatter = "{value} 单" };
            y_number.Name = "订单笔数";

            var y_money = new YAxis();
            y_money.Type = AxisTypes.value;
            y_money.AxisLabel = new Label { Formatter = "{value} 元" };
            y_money.Name = "订单金额";
            y_money.Position = YAxisPosition.right;

            var order_legend = new Legend();
            order_legend.Data = new[] { new LegendData("订单笔数"), new LegendData("订单金额") };
            var chart_tooltip = new Tooltip();
            chart_tooltip.Trigger = TooltipTriggerTypes.axis;
            var chartOptions = new EChartsOption
            {
                Title = new Title("订单交易图表") { Left = new AlignValue(Align.left), Subtext = "最近30天" },
                Calculable = true,
                Toolbox = new ToolBox(),
                Tooltip = chart_tooltip,
                Series = new Series[]
                {
                    new LineSeries
                    {
                        Name = "订单笔数",
                        Data = seriesData_number,
                        Smooth = true,
                        YAxisIndex = 0,
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
                    },
                    new LineSeries
                    {
                        Name = "订单金额",
                        Data = seriesData_money,
                        Smooth = true,
                        YAxisIndex = 1,
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
                Legend = order_legend,
                XAxis = new XAxis[1] { x_rq },
                YAxis = new YAxis[2] { y_number, y_money }
            };
            return this.ToEChartResult(chartOptions);
        }

        /// <summary>
        ///     用户30天新增图表(30天)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JavaScriptJsonResult UserMouthAddReport()
        {
            var seriesData_number = new List<object>();
            var seriesData_logincount = new List<object>();
            var xAxisData_rq = new List<object>();
            var Lst_User = db.User_Infos.AsNoTracking().ToList();

            for (var i = 0; i < 30; i++)
            {
                var currentTime = DateTime.Now.AddDays(-i);
                xAxisData_rq.Add(currentTime.ToString("dd号"));
                var Lst = Lst_User.Where(p => p.CreateTime.ToString("yyyy-MM-dd") ==
                                              currentTime.ToString("yyyy-MM-dd")).ToList();

                seriesData_number.Add(Lst.Count().ToString());
                seriesData_logincount.Add(Lst.Sum(p => p.LoginCount).ToString());
            }

            var x_rq = new XAxis();
            x_rq.Type = AxisTypes.category;
            x_rq.Data = xAxisData_rq;

            var y_number = new YAxis();
            y_number.Type = AxisTypes.value;
            y_number.AxisLabel = new Label { Formatter = "{value} 人" };
            y_number.Name = "会员人数";

            var y_logincount = new YAxis();
            y_logincount.Type = AxisTypes.value;
            y_logincount.AxisLabel = new Label { Formatter = "{value} 次" };
            y_logincount.Name = "会员登录次数";
            y_logincount.Position = YAxisPosition.right;

            var login_legend = new Legend();
            login_legend.Data = new[] { new LegendData("会员人数"), new LegendData("会员登录次数") };
            var chart_tooltip = new Tooltip();
            chart_tooltip.Trigger = TooltipTriggerTypes.axis;
            var chartOptions = new EChartsOption
            {
                Title = new Title("会员新增图表") { Left = new AlignValue(Align.left), Subtext = "最近30天" },
                Toolbox = new ToolBox(),
                Tooltip = chart_tooltip,
                Series = new Series[]
                {
                    new LineSeries
                    {
                        Name = "会员人数",
                        Data = seriesData_number,
                        Smooth = true,
                        YAxisIndex = 0,
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
                    },
                    new LineSeries
                    {
                        Name = "会员登录次数",
                        Data = seriesData_logincount,
                        Smooth = true,
                        YAxisIndex = 1,
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
                Legend = login_legend,
                XAxis = new XAxis[1] { x_rq },
                YAxis = new YAxis[2] { y_number, y_logincount }
            };
            return this.ToEChartResult(chartOptions);
        }

        /// <summary>
        ///     用户订单状况图表(30天)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JavaScriptJsonResult OrderStateReport()
        {
            var seriesData_number = new List<object>(); //所有数量
            var seriesData_Closed = new List<object>(); //已经关闭订单
            var seriesData_Obligation = new List<object>(); //待付款订单
            var seriesData_Overhang = new List<object>(); //待发货订单
            var seriesData_PaidDelete = new List<object>(); //已付款未删除
            var seriesData_ReturnedGoods = new List<object>(); //退货/退款
            var seriesData_Success = new List<object>(); //交易完成
            var seriesData_UnpaidDelete = new List<object>(); //未付款删除
            var seriesData_WaitReceiving = new List<object>(); //待发货
            var xAxisData_rq = new List<object>();
            var Lst_Order = db.Order_Infos.AsNoTracking().ToList();

            for (var i = 0; i < 30; i++)
            {
                var currentTime = DateTime.Now.AddDays(-i);
                xAxisData_rq.Add(currentTime.ToString("dd号"));
                var Lst = Lst_Order.Where(p => p.CreateTime.ToString("yyyy-MM-dd") ==
                                               currentTime.ToString("yyyy-MM-dd")).ToList();
                seriesData_number.Add(Lst.Count().ToString());
                seriesData_Closed.Add(Lst.Where(p => p.State == EnumOrderStatus.Closed).ToList().Count().ToString());
                seriesData_Obligation.Add(
                    Lst.Where(p => p.State == EnumOrderStatus.Obligation).ToList().Count().ToString());
                seriesData_Overhang.Add(Lst.Where(p => p.State == EnumOrderStatus.Overhang).ToList().Count().ToString());
                seriesData_PaidDelete.Add(
                    Lst.Where(p => p.State == EnumOrderStatus.PaidDelete).ToList().Count().ToString());
                seriesData_ReturnedGoods.Add(
                    Lst.Where(p => p.State == EnumOrderStatus.ReturnedGoods).ToList().Count().ToString());
                seriesData_Success.Add(Lst.Where(p => p.State == EnumOrderStatus.Success).ToList().Count().ToString());
                seriesData_UnpaidDelete.Add(
                    Lst.Where(p => p.State == EnumOrderStatus.UnpaidDelete).ToList().Count().ToString());
                seriesData_WaitReceiving.Add(
                    Lst.Where(p => p.State == EnumOrderStatus.WaitReceiving).ToList().Count().ToString());
            }
            //多个(大于3个)series的data元素，一个日期x坐标一个数量y坐标
            var x_rq = new XAxis();
            x_rq.Type = AxisTypes.category;
            x_rq.Data = xAxisData_rq;

            var y_number = new YAxis();
            y_number.Type = AxisTypes.value;
            y_number.AxisLabel = new Label { Formatter = "{value} 单" };
            y_number.Name = "订单数";

            var login_legend = new Legend();
            login_legend.Data = new[]
            {
                new LegendData("总订单"),
                new LegendData("已关闭"),
                new LegendData("待付款"),
                new LegendData("待发货"),
                new LegendData("已付款未删除"),
                new LegendData("退货/退款"),
                new LegendData("交易完成"),
                new LegendData("未付款删除"),
                new LegendData("待发货")
            };
            login_legend.Padding = 2;
            var chart_tooltip = new Tooltip();
            chart_tooltip.Trigger = TooltipTriggerTypes.axis;
            var chartOptions = new EChartsOption
            {
                Title = new Title("订单状态图表") { Left = new AlignValue(Align.left), Subtext = "最近30天" },
                Calculable = true,
                Toolbox = new ToolBox(),
                Tooltip = chart_tooltip,
                Series = new Series[]
                {
                    #region Series
                    new LineSeries
                    {
                        Name = "总订单",
                        Data = seriesData_number,
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
                    },
                    new LineSeries
                    {
                        Name = "已关闭订单数",
                        Data = seriesData_Closed,
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
                    },
                    new LineSeries
                    {
                        Name = "待付款",
                        Data = seriesData_Obligation,
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
                    },
                    new LineSeries
                    {
                        Name = "待发货",
                        Data = seriesData_Overhang,
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
                    },
                    new LineSeries
                    {
                        Name = "已付款未删除",
                        Data = seriesData_PaidDelete,
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
                    },
                    new LineSeries
                    {
                        Name = "退货/退款",
                        Data = seriesData_ReturnedGoods,
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
                    },
                    new LineSeries
                    {
                        Name = "交易完成",
                        Data = seriesData_Success,
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
                    },
                    new LineSeries
                    {
                        Name = "未付款删除",
                        Data = seriesData_UnpaidDelete,
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
                    },
                    new LineSeries
                    {
                        Name = "待发货",
                        Data = seriesData_WaitReceiving,
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
                    #endregion
                },
                Legend = login_legend,
                XAxis = new XAxis[1] { x_rq },
                YAxis = new YAxis[1] { y_number }
            };
            return this.ToEChartResult(chartOptions);
        }

        /// <summary>
        ///     用户访问图表(30天)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JavaScriptJsonResult UserMouthTransmissionReport()
        {
            var yQuery = db.Log_MemberAccess.AsNoTracking().ToList();
            var seriesData_number = new List<object>();
            var xData_rq = new List<object>();
            for (var i = 0; i < 30; i++)
            {
                var currentTime = DateTime.Now.AddDays(-i);
                xData_rq.Add(currentTime.ToString("dd号"));

                var Lst = yQuery.Where(p => p.CreateTime.ToString("yyyy-MM-dd") ==
                                            currentTime.ToString("yyyy-MM-dd")).ToList();
                seriesData_number.Add(Lst.Count().ToString());
            }
            Legend login_legend = new Legend();
            login_legend.Data = new LegendData[] { new LegendData("访问次数") };
            Tooltip chart_tooltip = new Tooltip();
            chart_tooltip.Trigger = TooltipTriggerTypes.axis;
            var chartOptions = new EChartsOption
            {
                Title = new Title("最近30天会员访问情况") { Left = new AlignValue(Align.left) },
                Toolbox = new ToolBox(),
                Tooltip = chart_tooltip,
                Legend = login_legend,
                Series = new Series[]
                {
                    new BarSeries
                    {
                        Name = "访问次数",
                        Data = seriesData_number,
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
                XAxis = new XAxis[1] { new XAxis { Type = AxisTypes.category, Data = xData_rq } },
                YAxis =
                    new YAxis[1]
                    {
                        new YAxis
                        {
                            Name = "访问次数",
                            Type = AxisTypes.value,
                            AxisLabel = new Label {Formatter = "{value} 次"}
                        }
                    }
            };
            return this.ToEChartResult(chartOptions);
        }
    }
}