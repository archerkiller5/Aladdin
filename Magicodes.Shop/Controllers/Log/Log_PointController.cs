using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data.Models.Log;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Log
{
    public class Log_PointController : TenantBaseController<Log_Point>
    {
        // GET: Log_Point
        [RoleMenuFilter(title: "积分日志", id: "{45C64BD6-0446-46EF-BFDE-204B641527C0}", roleNames: "Admin,TenantManager,ShopManager", url: "/Log_Point", parentId: "{F348A29F-9F4C-476A-9932-23C607A4F9FB}")]
        public async Task<ActionResult> Index(string openId, DateTime? sDatePicker, DateTime? eDatePicker, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Log_Points.AsQueryable();
            this.TempData["openId"] = openId;//存储tempDate传递到UserTimeChart

            //按会员查询
            if (!String.IsNullOrEmpty(openId))
            {
                queryable = queryable.Where(p => p.OpenId.Contains(openId));
            }

            //按时间段查询
            DateTime now = DateTime.Now;
            DateTime startDate = new DateTime(now.Year, now.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            if (sDatePicker == null && eDatePicker == null)
            {
                queryable = queryable.Where(p => p.CreateTime >= startDate && p.CreateTime <= endDate);
                this.TempData["StartDate"] = startDate;
                this.TempData["EndDate"] = endDate;
            }
            else if (sDatePicker != null && eDatePicker == null)
            {
                var start = new DateTime(sDatePicker.Value.Year, sDatePicker.Value.Month, 1);
                DateTime end = start.AddMonths(1).AddDays(-1);
                queryable = queryable.Where(p => p.CreateTime >= sDatePicker && p.CreateTime <= end);
                this.TempData["StartDate"] = sDatePicker;
                this.TempData["EndDate"] = end;
            }
            else if (sDatePicker == null && eDatePicker != null)
            {
                DateTime start = new DateTime(eDatePicker.Value.Year, eDatePicker.Value.Month, 1);
                queryable = queryable.Where(p => p.CreateTime >= start && p.CreateTime <= eDatePicker);
                this.TempData["StartDate"] = start;
                this.TempData["EndDate"] = eDatePicker;
            }
            else
            {
                queryable = queryable.Where(p => p.CreateTime >= sDatePicker && p.CreateTime <= eDatePicker);
                this.TempData["StartDate"] = sDatePicker;
                this.TempData["EndDate"] = eDatePicker;
            }

            var maxPoint = queryable.Count() <= 0 ? 0 : queryable.Max(p => p.Point);
            this.TempData["MaxPoint"] = maxPoint.ToString();

            var pagedList = new PagedList<Log_Point>(
                             await queryable.OrderBy(p => p.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                             pageIndex,
                             pageSize,
                             await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Log_Point/Details/5 
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Log_Point log_Point = await db.Log_Points.FindAsync(id);
            if (log_Point == null)
            {
                return HttpNotFound();
            }
            return View(log_Point);
        }

        [HttpGet]
        public JavaScriptJsonResult UserTimeChart()
        {
            string openId = this.TempData["openId"] != null ? this.TempData["openId"] as string : "";
            var seriesData = new List<object>();
            var xAxisData = new List<object>();

            int firstDay = ((DateTime)this.TempData["StartDate"]).Day;
            int lastDay = ((DateTime)this.TempData["EndDate"]).Day;
            for (int i = firstDay; i <= lastDay; i++)
            {
                xAxisData.Add("第" + i.ToString() + "天");

                int currentDayPointCount;
                if (!String.IsNullOrEmpty(openId))
                {
                    currentDayPointCount = db.Log_Points.AsNoTracking().Where(p => p.OpenId == openId && p.CreateTime.Day == i && p.CreateTime.Month == DateTime.Now.Month).Count();
                }
                else
                {
                    currentDayPointCount = db.Log_Points.AsNoTracking().Where(p => p.CreateTime.Day == i && p.CreateTime.Month == DateTime.Now.Month).Count();
                }

                if (currentDayPointCount > 0)
                {
                    int currentdayMaxPoint;
                    if (!String.IsNullOrEmpty(openId))
                    {
                        currentdayMaxPoint = db.Log_Points.AsNoTracking().Where(p => p.OpenId == openId && p.CreateTime.Day == i && p.CreateTime.Month == DateTime.Now.Month).Max(m => m.Point);
                        seriesData.Add(currentdayMaxPoint.ToString());
                    }
                    else
                    {
                        currentdayMaxPoint = db.Log_Points.AsNoTracking().Where(p => p.CreateTime.Day == i && p.CreateTime.Month == DateTime.Now.Month).Max(m => m.Point);
                        seriesData.Add(currentdayMaxPoint.ToString());
                    }
                }
                else
                {
                    seriesData.Add("0");
                }
            }

            //获取最大积分用于y轴                               
            List<object> yPoint = new List<object>();
            yPoint.Add("0");
            yPoint.Add(this.TempData["MaxPoint"]);

            var chartOptions = new EChartsOption()
            {
                Title = new Title("当月会员积分变化") { Left = new AlignValue(Align.center) },
                Series = new Series[]
                {
                    new LineSeries()
                    {
                        Name = "当月会员积分变化",
                        Data = seriesData,
                        Smooth = true,
                        MarkPoint=new MarkPoint()
                        {
                            Data=new List<MarkData>()
                            {
                                new MarkData() {Type=MarkPointDataTypes.max,Name="最大值" },
                                new MarkData() {Type=MarkPointDataTypes.min,Name="最小值" },
                            }
                        },
                        MarkLine=new MarkLine()
                        {
                            Data=new List<MarkData>()
                            {
                                new MarkData() { Type=MarkPointDataTypes.average,Name="平均值"}
                            }
                        }
                    }
                },
                XAxis = new XAxis[1] { new XAxis { Type = AxisTypes.category, Data = xAxisData } },
                YAxis = new YAxis[1] { new YAxis { Type = AxisTypes.value, Data = yPoint } },
            };

            return this.ToEChartResult(chartOptions);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
