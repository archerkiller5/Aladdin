using System.Linq;
using System.Web.Mvc;
using System.Text;
using System;
using System.IO;

namespace Magicodes.Shop.Controllers.Areas
{
    public class MareasController : BaseController
    {
        // GET: Mareas
        public ActionResult Index()
        {
            var areslis = db.Logistics_Areas.ToList();
            StringBuilder sb = new StringBuilder("var enType=typeof(Logistics_AreaLevel);");
            sb.Append("<br/>");
            foreach (var item in areslis)
            {
                sb.AppendFormat("db.Logistics_Areas.Add(new Logistics_Area(){{Id=\"{0}\",AreaName=\"{1}\",AreaLevel=(Logistics_AreaLevel)Enum.Parse(enType, \"{2}\"),ParentId=\"{3}\",Pinyinma=\"{4}\",PostCode=\"{5}\",SortNumber={6},CreateBy=\"{7}\",CreateTime=DateTime.Now}});", 
                    item.Id, item.AreaName,item.AreaLevel,item.ParentId,item.Pinyinma,item.PostCode,item.SortNumber,item.CreateBy,item.CreateTime);
                sb.Append("<br/>");
                sb.AppendLine();
            }
            return Content(sb.ToString());
        }
     
    }
}