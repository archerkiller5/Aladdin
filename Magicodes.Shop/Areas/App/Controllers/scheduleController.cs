using Magicodes.WeiChat.Data.Models.WeChatStore;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Magicodes.WeiChat.Data.Models.User;
using Magicodes.Shop.Controllers.Order;
using Magicodes.WeiChat.Domain.PhotoGallery;
using System.IO;
using Magicodes.WeiChat.Data.Models.Site;

namespace Magicodes.Shop.Areas.App.Controllers
{

    public class scheduleController : AppBaseController
    {
        //待办事项
        // GET: App/schedule
        //[WeChatOAuth]
        public ActionResult Index()
        {
            return View();
        }
        #region 签到
        /// <summary>
        /// 签到
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WeChatOAuth]
        public async Task<ActionResult> Sign_Info()
        {
            var user = WeiChatApplicationContext.Current.WeiChatUser;
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag._nickName = user.NickName;
            List<int> frequency = new List<int>();
            int minFrequency = -1;
            StringBuilder sb = new StringBuilder("将会获得:");
            //拿到用户记录
            var userRecord = db.Sign_Records.FirstOrDefault(p => p.Member_ID == user.OpenId);
            //如果为空，创建一个用户签到记录表
            if (userRecord == null)
            {
                userRecord = new Sign_Record()
                {
                    Member_ID = user.OpenId,
                    Sign_num = 1,
                    LatestTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                };
                //最近的奖励目标天数
                minFrequency = MinFrequency(userRecord.Sign_num);
                userRecord.NeedDay = minFrequency - 1;
                foreach (var reward in db.Sign_Setups.Include(p => p.Sign_Rewards).FirstOrDefault(p => p.Frequency == minFrequency).Sign_Rewards)
                {
                    sb.AppendFormat("{0}{1}个", reward.Reward_type, reward.Reward_num);
                }
                userRecord.Reword = sb.ToString();
                db.Sign_Records.Add(userRecord);
            }
            else
            {
                //正常续签到
                if (userRecord.LatestTime.Day + 1 == DateTime.Now.Day)
                {
                    userRecord.LatestTime = DateTime.Now;
                    userRecord.Sign_num++;
                    //最近的奖励目标天数
                    minFrequency = MinFrequency(userRecord.Sign_num);
                    userRecord.NeedDay = minFrequency - userRecord.Sign_num;
                    foreach (var reward in db.Sign_Setups.Include(p => p.Sign_Rewards).FirstOrDefault(p => p.Frequency == minFrequency).Sign_Rewards)
                    {
                        sb.AppendFormat("{0}{1}个", reward.Reward_type, reward.Reward_num);
                    }
                    userRecord.Reword = sb.ToString();
                }
                //断掉签到
                else
                {
                    userRecord.LatestTime = DateTime.Now;
                    userRecord.Sign_num = 1;
                    //foreach (var item in db.Sign_Setups.Where(p => p.Frequency > userRecord.Sign_num))
                    //{
                    //    frequency.Add(item.Frequency);
                    //}
                    ////最近的奖励目标天数
                    //minFrequency = frequency.Min();
                    minFrequency = MinFrequency(userRecord.Sign_num);
                    userRecord.NeedDay = minFrequency - 1;
                    foreach (var reward in db.Sign_Setups.Include(p => p.Sign_Rewards).FirstOrDefault(p => p.Frequency == minFrequency).Sign_Rewards)
                    {
                        sb.AppendFormat("{0}{1}个", reward.Reward_type, reward.Reward_num);
                    }
                    userRecord.Reword = sb.ToString();
                }
            }
            await db.SaveChangesAsync();
            return View(userRecord);
        }
        public int MinFrequency(int sign_num)
        {
            List<int> frequency = new List<int>();
            foreach (var item in db.Sign_Setups.Where(p => p.Frequency > sign_num))
            {
                frequency.Add(item.Frequency);
            }
            return frequency.Min();
        }
        #endregion
        public ActionResult Daiban_task()
        {
            return View();
        }
        public ActionResult MasterPage()
        {
            return View();
        }
        [WeChatOAuth]
        public ActionResult Edit_Info(string Id)
        {
            AppUserInfo member = db.AppUserInfos.FirstOrDefault(p => p.OpenId == Id);
            if (member == null)
            {
                member = new AppUserInfo();
            }
            ViewBag.school = GetSchoolList(null);
            return View(member);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit_Info([Bind
            (Include = "School,Campus,Nick_name,Real_Name,Member_img,Sex,IDNumber,Birth_Date,Email，Adress")]
        AppUserInfo member)
        {
            ViewBag.school = GetSchoolList(null);
            if (ModelState.IsValid)
            {
                if (!StringHelper.IsIDCard(member.IDNumber))
                {
                    ModelState.AddModelError("IDNumber", "请输入身份证号码！！");
                    return View(member);
                }
                if (!StringHelper.IsEmail(member.Email))
                {
                    ModelState.AddModelError("Email", "请输入邮箱号！！");
                    return View(member);
                }
                if (!StringHelper.IsChinese(member.Real_Name))
                {
                    ModelState.AddModelError("Real_Name", "请输入真实姓名！！");
                }
                var memberinfo = await db.AppUserInfos.FirstOrDefaultAsync(p => p.OpenId == member.OpenId);
                if (memberinfo == null)
                {
                    db.AppUserInfos.Add(member);
                }
                else
                {
                    memberinfo.School_ID = member.School_ID;
                    memberinfo.Campus_ID = member.Campus_ID;
                    memberinfo.Nick_name = member.Nick_name;
                    memberinfo.Real_Name = member.Real_Name;
                    memberinfo.Sex = member.Sex;
                    memberinfo.IDNumber = member.IDNumber;
                    memberinfo.Adress = member.Adress;
                    memberinfo.Birth_Date = member.Birth_Date;
                    memberinfo.Email = member.Email;
                }
                await db.SaveChangesAsync();
                ViewBag.school = GetSchoolList(member.School_ID);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upImg)
        {
            //定义允许上传的文件扩展名
            const string fileTypes = "gif,jpg,png,bmp";
            string fileExt = Path.GetExtension(upImg.FileName);
            if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(fileTypes.Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                return Json(new { error = "上传文件不支持文件格式！" }, "text/html");
            }
            //设置最大文件大小（2M）
            const int maxSize = 2050000;
            if (upImg.ContentLength > maxSize)
            {
                return Json(new { error = "上传文件中的图片大小超出2M!!" }, "text/html");
            }
            //存储文件名
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmssff") + CreateRandomCode(8) + "_" + System.IO.Path.GetFileName(upImg.FileName);
            //租户id
            var dirName = TenantId.ToString();
            string virtualPath = Path.Combine(HttpContext.Server.MapPath("../Uploads"), fileName);
            string imgUrl = "", error = "", msg = "";
            imgUrl = Path.Combine(HttpContext.Server.MapPath("..\\Uploads"), fileName);
            try
            {
                upImg.SaveAs(virtualPath);
                msg = "上传成功！";
                //var pic = new Site_Image
                //{
                //    Name = fileName,
                //    SiteUrl = imgUrl,
                //};
                //db.Site_Images.Add(pic);
                //db.SaveChanges();
            }
            catch (Exception ex)
            {
                error = "上传失败！";
                throw new Exception("上传失败！", ex);
            }
            return Json(new { msg = msg, imgUrl = imgUrl, error = error }, "text/html");
            //return Json(new { msg = "ok", imgUrl = "url", error = "" }, "text/html");
        }
        /// <summary>
        /// 生成指定长度的随机码。
        /// </summary>
        private string CreateRandomCode(int length)
        {
            string[] codes = new string[36] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            StringBuilder randomCode = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                randomCode.Append(codes[rand.Next(codes.Length)]);
            }
            return randomCode.ToString();
        }
        /// <summary>
        /// ajax获取学院
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public ActionResult GetCampusList(int schoolId)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(db.User_Schools.Where(p => p.ParentID == schoolId).ToList(), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public SelectList GetSchoolList(int? schoolId)
        {
            return new SelectList(db.User_Schools.Where(p => p.ParentID == 0), dataTextField: "School_Name", dataValueField: "ID", selectedValue: schoolId);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}