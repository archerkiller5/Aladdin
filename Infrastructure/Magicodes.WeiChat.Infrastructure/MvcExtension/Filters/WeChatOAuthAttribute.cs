// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeChatOAuthAttribute.cs
//          description :
//  
//          created by 李文强 at  2016/10/06 10:21
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub：https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Magicodes.Logger;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Apis.Token;
using Magicodes.WeChat.SDK.Apis.User;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.User;
using Newtonsoft.Json;

namespace Magicodes.WeiChat.Infrastructure.MvcExtension.Filters
{
    /// <summary>
    ///     网页授权获取用户基本信息特性
    ///     关于公众号如何获取用户信息，请参考此文档：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class WeChatOAuthAttribute : FilterAttribute, IAuthorizationFilter
    {
        private const string RedirectUrlCookieName = "Magicodes.Weichat_RedirectUrlCookie";
        private readonly string _state = "magicodes.weichat";
        private readonly LoggerBase _logger = Loggers.Current.DefaultLogger;

        public WeChatOAuthAttribute()
        {
        }

        public WeChatOAuthAttribute(string state)
        {
            _state = state;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var httpContextBase = filterContext.HttpContext;
            var request = httpContextBase.Request;
            _logger.Log(LoggerLevels.Trace, string.Format("正在获取授权...{0}", request.Url));
            //如果用户已经验证或者已经获取微信信息，则不再验证（提高效率）
            //if (WeiChatApplicationContext.Current.WeiChatUser != null)
            //{
            //    _logger.Log(LoggerLevels.Trace, string.Format("用户已经验证或者已经获取微信信息，不再验证...{0}", request.Url));
            //    return;
            //}
            var tenantId = WeiChatApplicationContext.Current.TenantId;
            //为了防止遇到用户关注多个租户的多个公众号的问题，因此此处添加租户Id参数
            var cookieName = string.Format("{0}_{1}", WeiChatApplicationContext.OpenIdCookieName, tenantId);
            var userSesstionName = string.Format("{0}_{1}", WeiChatApplicationContext.UserSessionName, tenantId);
            //用户的OPENID
            var openIdCookie = request.Cookies[cookieName];
            openIdCookie = new HttpCookie(cookieName) { Value = "o3Q4xv9pXYCBk-pdpvgqWTbs8aLY" };
            var code = request.QueryString["code"];
            var state = request.QueryString["state"];

            #region 如果是从微信验证页面跳转回来

            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(state))
            {
                _logger.Log(LoggerLevels.Trace,
                    string.Format("从微信验证页面跳转回来...{2}\ncode:{0}\tstate:{1}", code, state, request.Url));
                //TODO:判断是否来自open.weixin.qq.com/connect/oauth2/authorize
                //if (request.)
                //{

                //}
                //通过code换取access_token,Code只能用一次
                //网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同
                var result = WeChatApisContext.Current.OAuthApi.Get(code);
                if (!result.IsSuccess())
                {
                    _logger.Log(LoggerLevels.Error, string.Format("授权出错，获取access_token失败...Url:{0};code:{1};DetailResult:{2}", request.Url, code, result.DetailResult));
                    throw new Exception("授权出错，获取access_token失败！");
                }
                openIdCookie = new HttpCookie(cookieName) { Value = result.OpenId };
                
                httpContextBase.Response.Cookies.Add(openIdCookie);

                #region 获取微信用户信息

                var userInfo = WeChatApisContext.Current.OAuthApi.GetUserInfo(result.AccessToken, result.OpenId);
                if (!userInfo.IsSuccess())
                {
                    _logger.Log(LoggerLevels.Error, string.Format("授权出错，获取粉丝信息失败...Url:{0};DetailResult:{1}", request.Url, userInfo.DetailResult));
                    throw new Exception("授权出错，获取粉丝信息失败！");
                }
                WeiChat_User user;
                try
                {
                    using (var db = new AppDbContext())
                    {
                        user = db.WeiChat_Users.FirstOrDefault(p => p.OpenId == userInfo.OpenId);
                        if (user == null)
                        {
                            var currentUserInfo = db.User_Infos.FirstOrDefault(p => p.OpenId == userInfo.OpenId);
                            if (currentUserInfo == null)
                            {
                                currentUserInfo = new User_Info
                                {
                                    OpenId = userInfo.OpenId,
                                    Balance = 0,
                                    Integral = 0,
                                    State = EnumUserState.Normal,
                                    TenantId = tenantId,
                                    CreateTime = DateTime.Now,
                                    LastLoginOn = DateTime.Now,
                                    LoginCount = 0
                                };
                                _logger.Log(LoggerLevels.Trace, "新增userinfo对象");
                                db.User_Infos.Add(currentUserInfo);
                            }
                            user = new WeiChat_User
                            {
                                City = userInfo.City,
                                Country = userInfo.Country,
                                //GroupId = userInfo.groupid,
                                HeadImgUrl = userInfo.Headimgurl,
                                //Language = userInfo.language,
                                NickName = userInfo.NickName,
                                OpenId = userInfo.OpenId,
                                Province = userInfo.Province,
                                //Remark = userInfo.remark,
                                Sex = userInfo.Sex,
                                Subscribe = false,
                                SubscribeTime = DateTime.Now,
                                UnionId = userInfo.Unionid,
                                TenantId = tenantId
                            };
                            db.WeiChat_Users.Add(user);
                            db.SaveChanges();
                        }
                        else
                        {
                            _logger.Log(LoggerLevels.Trace,
                                string.Format("用户信息已存在...\n{0}\n{1}", request.Url, JsonConvert.SerializeObject(user)));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(LoggerLevels.Error, string.Format("{0}\n{1}\n{2}", request.Url, ex.Message, ex));
                    //filterContext.Result = new ContentResult { Content = "授权出错：" + code + "     具体错误：" + ex.Message };
                    throw new Exception("授权出错：" + code + "     具体错误：" + ex.Message);
                }
                if (!HttpContext.Current.Items.Contains(userSesstionName))
                    HttpContext.Current.Items.Add(userSesstionName, user);
                HttpContext.Current.Session[userSesstionName] = user;
                _logger.Log(LoggerLevels.Trace, string.Format("已成功赋值...\n{0}", request.Url));
                var redirectUrlCookie = request.Cookies[RedirectUrlCookieName];
                if (redirectUrlCookie != null)
                {
                    var redirectUrl = redirectUrlCookie.Value;
                    if (httpContextBase.Response.Cookies[RedirectUrlCookieName] != null)
                        httpContextBase.Response.Cookies.Remove(RedirectUrlCookieName);
                    filterContext.Result = new RedirectResult(redirectUrl);
                }

                #endregion
            }
            #endregion
            #region 如果没有验证，则进行验证

            //else if (string.IsNullOrEmpty(openIdCookie?.Value))
            //{
            //    var redirectUrl = request.Url.ToString();

            //    var cookie = new HttpCookie(RedirectUrlCookieName) { Value = redirectUrl };
            //    httpContextBase.Response.Cookies.Add(cookie);
            //    //获取授权Url
            //    var url = WeChatApisContext.Current.OAuthApi.GetAuthorizeUrl(redirectUrl, _state,WeChat.SDK.Apis.OAuth.OAuthScopes.snsapi_base);
            //    _logger.Log(LoggerLevels.Trace, string.Format("跳转至微信服务器获取授权...\n{0}\n{1}", redirectUrl, url));
            //    filterContext.Result = new RedirectResult(url);
            //}
            #endregion
            #region 如果已验证

            else if ((openIdCookie != null) && !string.IsNullOrEmpty(openIdCookie.Value))
            {
                var openId = openIdCookie.Value;
                _logger.Log(LoggerLevels.Trace, string.Format("从openIdCookie获取openId...{0}", openId));
                using (var db = new AppDbContext())
                {
                    var user = db.WeiChat_Users.FirstOrDefault(p => p.OpenId == openId);
                    if (user != null)
                    {
                        _logger.Log(LoggerLevels.Trace, string.Format("已从数据库获取到粉丝信息...{0}", openId));
                        HttpContext.Current.Items.Add(userSesstionName, user);
                        HttpContext.Current.Session[userSesstionName] = user;
                    }
                    else
                    {
                        try
                        {
                            var redirectUrl = request.Url.ToString();
                            _logger.Log(LoggerLevels.Trace, string.Format("准备跳转至微信服务器获取授权...\n{0}", redirectUrl));
                            var cookie = new HttpCookie(RedirectUrlCookieName);
                            cookie.Value = redirectUrl;
                            httpContextBase.Response.Cookies.Add(cookie);
                            //获取授权Url
                            var url = WeChatApisContext.Current.OAuthApi.GetAuthorizeUrl(redirectUrl, _state);
                            _logger.Log(LoggerLevels.Trace, string.Format("跳转至微信服务器获取授权...\n{0}\n{1}", redirectUrl, url));
                            //注意修改授权回调页面域名
                            filterContext.Result = new RedirectResult(url);
                        }
                        catch (Exception ex)
                        {
                            _logger.Log(LoggerLevels.Error, ex);
                            throw ex;
                        }
                    }
                }
            }
            #endregion

            else
            {
                _logger.Log(LoggerLevels.Error, string.Format("授权出错，请检查...{0}", request.Url));
                filterContext.Result = new ContentResult { Content = "授权出错，请检查！" };
            }
        }
    }
}