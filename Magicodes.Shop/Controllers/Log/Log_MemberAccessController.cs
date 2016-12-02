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
    public class Log_MemberAccessController : TenantBaseController<Log_MemberAccess>
    {
        // GET: Log_MemberAccess
        [RoleMenuFilter(title: "访问日志", id: "{A05A0EC0-99DB-42ED-AEDB-1C29296F9677}", roleNames: "Admin,TenantManager,ShopManager", url: "/Log_MemberAccess", parentId: "F348A29F-9F4C-476A-9932-23C607A4F9FB")]
        public async Task<ActionResult> Index(string q, string openId, DateTime? sDatePicker, DateTime? eDatePicker, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Log_MemberAccess.AsQueryable();
            this.TempData["query"] = q;
            this.TempData["openId"] = openId;
            if (!string.IsNullOrWhiteSpace(q))
            {
                //请替换为相应的搜索逻辑
                queryable = queryable.Where(p => p.TenantId.ToString().Contains(q) || p.RequestUrl.Contains(q));
            }

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

            var pagedList = new PagedList<Log_MemberAccess>(
                             await queryable.OrderBy(p => p.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                             pageIndex,
                             pageSize,
                             queryable.Count());
            return View(pagedList);
        }

        // GET: Log_MemberAccess/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Log_MemberAccess log_MemberAccess = await db.Log_MemberAccess.FindAsync(id);
            if (log_MemberAccess == null)
            {
                return HttpNotFound();
            }
            return View(log_MemberAccess);
        }

        [HttpGet]
        public JavaScriptJsonResult UserAreaChart()
        {
            string openId = this.TempData["openId"] != null ? this.TempData["openId"] as string : "";
            DateTime sDate = (DateTime)this.TempData["StartDate"];
            DateTime eDate = (DateTime)this.TempData["EndDate"];
            var yQuery = db.Log_MemberAccess.AsNoTracking();
            List<object> accessList = new List<object>();
            string q = this.TempData["query"] as string;
            if (!String.IsNullOrEmpty(q))
            {
                if (String.IsNullOrEmpty(openId))
                {
                    accessList = yQuery.Where(p => p.RequestUrl.Contains(q) && p.CreateTime >= sDate && p.CreateTime <= eDate).Select(p => p.RequestUrl).Distinct().ToList().Select(p => (object)p).ToList();
                }
                else
                {
                    accessList = yQuery.Where(p => p.RequestUrl.Contains(q) && p.OpenId.Contains(openId) && p.CreateTime >= sDate && p.CreateTime <= eDate).Select(p => p.RequestUrl).Distinct().ToList().Select(p => (object)p).ToList();
                }
            }
            else
            {
                if (String.IsNullOrEmpty(openId))
                {
                    accessList = yQuery.Where(p => p.CreateTime >= sDate && p.CreateTime <= eDate).Select(p => p.RequestUrl).Distinct().ToList().Select(p => (object)p).ToList();
                }
                else
                {
                    accessList = yQuery.Where(p => p.OpenId.Contains(openId) && p.CreateTime >= sDate && p.CreateTime <= eDate).Select(p => p.RequestUrl).Distinct().ToList().Select(p => (object)p).ToList();
                }
            }

            var yPoint = new List<object>();
            foreach (var item in accessList)
            {
                if (String.IsNullOrEmpty(openId))
                {
                    yPoint.Add(db.Log_MemberAccess.Where(p => p.CreateTime >= sDate && p.CreateTime <= eDate && p.RequestUrl == item).Count().ToString());
                }
                else
                {
                    yPoint.Add(db.Log_MemberAccess.Where(p => p.OpenId.Contains(openId) && p.CreateTime >= sDate && p.CreateTime <= eDate && p.RequestUrl == item).Count().ToString());
                }
            }

            var chartOptions = new EChartsOption()
            {
                Title = new Title("URL访问排名分布统计图") { Left = new AlignValue(Align.center) },
                Series = new Series[]
                {
                    new BarSeries()
                    {
                        Name = "访问排名分布",
                        Data = yPoint,
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
                XAxis = new XAxis[1] { new XAxis { Type = AxisTypes.category, Data = accessList } },
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
