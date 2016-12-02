// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Activity_SurveyController.cs
//          description :
//  
//          created by 李文强 at  2016/10/03 11:54
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub：https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data.Models.Activity;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Magicodes.WeiChat.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Activity
{
    [RoleMenuFilter("推广管理", "{8A337252-14B3-47A3-85BF-48FE964F88D2}", "Admin,TenantManager,ShopManager",
         iconCls: "fa fa-share-alt")]
    public class Activity_SurveyController : TenantBaseController<Activity_Survey>
    {
        // GET: Activity_Survey
        [RoleMenuFilter("调查管理", "{6D263A46-D5CC-4916-A59F-0F423057C424}", "Admin,TenantManager,ShopManager",
        parentId: "{8A337252-14B3-47A3-85BF-48FE964F88D2}",url: "/Activity_Survey")]
        [AuditFilter("调查查询", "d72d8933-f2b1-42bf-a6dc-1bf889ae8cd5")]
        public ActionResult Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Activity_Surveys.Include(a => a.CreateUser).Include(a => a.UpdateUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Title.Contains(q) || p.Description.Contains(q));
            var pagedList = new PagedList<Activity_Survey>(
                queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }
        // GET: Activity_Survey/Details/5
        [AuditFilter("设置问题", "{D657FEE4-B84D-4902-BA9D-F85FCF2C3237}")]
        public ActionResult SetUpTopics(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var activity_Survey = db.Activity_Surveys.Find(id);
            if (activity_Survey == null)
                return HttpNotFound();
            return View(activity_Survey);
        }

        // GET: Activity_Survey/Details/5
        [AuditFilter("调查详细", "f0a7c2bd-2275-4b40-88d7-d9b93e3e1a32")]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var activity_Survey = db.Activity_Surveys.Find(id);
            if (activity_Survey == null)
                return HttpNotFound();
            return View(activity_Survey);
        }

        [HttpGet]
        public ActionResult GetSurveyTopicTypes()
        {
            var list =
                Enum.GetNames(typeof(SurveyTopicTypes))
                    .Select(
                        p =>
                            new
                            {
                                value = (int)(SurveyTopicTypes)Enum.Parse(typeof(SurveyTopicTypes), p),
                                name = p,
                                displayName =
                                ((SurveyTopicTypes)Enum.Parse(typeof(SurveyTopicTypes), p)).GetEnumMemberDisplayName()
                            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Activity_Survey/Topics/{surveyId}")]
        public ActionResult GetTopics(int surveyId)
        {
            var topics = new List<Activity_SurveyTopicBase>();
            topics.AddRange(db.Activity_EssayQuestionTopics.Where(p => p.SurveyId == surveyId).ToList());
            topics.AddRange(
                db.Activity_SurveyChoiceTopics.Include(p => p.Options).Where(p => p.SurveyId == surveyId).ToList());
            return Json(topics.OrderBy(p => p.Order), JsonRequestBehavior.AllowGet);
        }

        // GET: Activity_Survey/Create
        [AuditFilter("调查待创建", "70867841-2198-40c3-bee8-37edd697643e")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Activity_Survey/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("调查创建”", "ab5b7f19-ecde-4c7b-9eaf-18c730603e76")]
        public ActionResult Create(Activity_Survey activity_Survey)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(activity_Survey, default(int?));
                db.Activity_Surveys.Add(activity_Survey);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activity_Survey);
        }

        // GET: Activity_Survey/Edit/5
        [AuditFilter("调查待编辑", "63a838d3-bd27-4ae5-be11-bcb4bf22b6ac")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var activity_Survey = db.Activity_Surveys.Find(id);
            if (activity_Survey == null)
                return HttpNotFound();
            return View(activity_Survey);
        }

        // POST: Activity_Survey/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("调查编辑", "221b6ce0-4e81-4161-b6a5-42f51257a628")]
        public ActionResult Edit(Activity_Survey activitySurvey)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(activitySurvey, activitySurvey.Id);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activitySurvey);
        }

        // GET: Activity_Survey/Delete/5
        [AuditFilter("调查待删除", "d35d94f1-0ff5-4dba-81a8-3efd6cef91d9")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var activitySurvey = db.Activity_Surveys.Find(id);
            if (activitySurvey == null)
                return HttpNotFound();
            return View(activitySurvey);
        }

        // POST: Activity_Survey/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("调查删除", "34a93d70-5087-4322-8a84-52dc4895ef17")]
        public ActionResult DeleteConfirmed(int? id)
        {
            var activitySurvey = db.Activity_Surveys.Find(id);
            db.Activity_Surveys.Remove(activitySurvey);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("Activity_Survey/Topics/{surveyId}")]
        [AuditFilter("保存问卷题目", "74D2E2C6-2264-4F7E-8D04-0511B7ACC168")]
        public ActionResult AddTopics(int surveyId)
        {
            var ajaxRes = new AjaxResponse() { Success = true, Message = "操作成功！" };
            var topics = JsonConvert.DeserializeObject<List<Activity_SurveyTopicBase>>(Request.Params["topics"], new SurveyTopicCustomConverter());
            var index = 0;
            foreach (var topic in topics)
            {
                topic.SurveyId = surveyId;
                topic.Order = index;
                index++;
                if (topic.Id == default(int))
                {
                    if (topic is Activity_EssayQuestionTopic)
                    {
                        db.Activity_EssayQuestionTopics.Add(topic as Activity_EssayQuestionTopic);
                    }
                    else if (topic is Activity_SurveyChoiceTopic)
                    {
                        db.Activity_SurveyChoiceTopics.Add(topic as Activity_SurveyChoiceTopic);
                    }
                    db.SaveChanges();
                }
                else
                {
                    if (topic is Activity_EssayQuestionTopic)
                    {
                        var model = topic as Activity_EssayQuestionTopic;
                        db.Activity_EssayQuestionTopics.Attach(model);
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (topic is Activity_SurveyChoiceTopic)
                    {
                        var model = topic as Activity_SurveyChoiceTopic;
                        var editOptions = model.Options.Where(p => p.Id != default(int));
                        var addOptions = model.Options.Where(p => p.Id == default(int));
                        foreach (var item in model.Options)
                        {
                            if (item.Id == default(int))
                            {
                                db.Entry(item).State = System.Data.Entity.EntityState.Added;
                            }
                            else
                            {
                                db.Activity_SurveyOptions.Attach(item);
                                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                        db.Activity_SurveyChoiceTopics.Attach(model);
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }
            return Json(ajaxRes);
        }
        // POST: Activity_Survey/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Activity_Survey/BatchOperation/{operation}")]
        [AuditFilter("调查批量操作", "e978a84b-9f2f-402f-8642-3c739d3e2210")]
        public ActionResult BatchOperation(string operation, params int?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = db.Activity_Surveys.Where(p => ids.Contains(p.Id)).ToList();
                    if (models.Count == 0)
                    {
                        ajaxResponse.Success = false;
                        ajaxResponse.Message = "没有找到匹配的项，项已被删除或不存在！";
                        return Json(ajaxResponse);
                    }
                    switch (operation.ToUpper())
                    {
                        case "DELETE":

                            #region 删除

                            {
                                db.Activity_Surveys.RemoveRange(models);
                                db.SaveChanges();
                                ajaxResponse.Success = true;
                                ajaxResponse.Message = string.Format("已成功操作{0}项！", models.Count);
                                break;
                            }

                        #endregion

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ajaxResponse.Success = false;
                    ajaxResponse.Message = ex.Message;
                }
            }
            else
            {
                ajaxResponse.Success = false;
                ajaxResponse.Message = "请至少选择一项！";
            }
            return Json(ajaxResponse);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class SurveyTopicCustomConverter : CustomCreationConverter<Activity_SurveyTopicBase>
    {
        /// <summary>
        /// 读取目标对象的JSON表示
        /// </summary>
        /// <param name="reader">JsonReader</param>
        /// <param name="objectType">对象类型</param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns>对象</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var jObject = JObject.Load(reader);
            Activity_SurveyTopicBase target = default(Activity_SurveyTopicBase);
            //获取type属性
            var type = jObject.Property("TopicType");
            if (type != null && type.Count > 0)
            {
                var typeValue = type.Value.ToString();
                var surveyTopicType = (SurveyTopicTypes)Enum.Parse(typeof(SurveyTopicTypes), typeValue);
                #region 根据类型返回相应题目类型
                switch (surveyTopicType)
                {
                    case SurveyTopicTypes.EssayQuestion:
                        target = new Activity_EssayQuestionTopic();
                        break;
                    case SurveyTopicTypes.MultipleChoice:
                        target = new Activity_SurveyChoiceTopic() { TopicType = SurveyTopicTypes.MultipleChoice };
                        break;
                    case SurveyTopicTypes.SingleChoice:
                        target = new Activity_SurveyChoiceTopic() { TopicType = SurveyTopicTypes.SingleChoice };
                        break;
                    default:
                        throw new NotSupportedException("不支持此类型：" + surveyTopicType);
                }
                #endregion
            }
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }
        /// <summary>
        /// 创建对象（会被填充）
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>对象</returns>
        public override Activity_SurveyTopicBase Create(Type objectType)
        {
            return new Activity_SurveyTopicBase();
        }
    }

}