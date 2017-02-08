// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : StringHelper.cs
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
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Magicodes.Shop.Controllers.Order
{
    /// <summary>
    ///     StringHelper
    /// </summary>
    public static class StringHelper
    {
        #region 转换类型

        #region 转换为Guid

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string str)
        {
            Guid guid;
            if (Guid.TryParse(str, out guid))
                return guid;
            return Guid.Empty;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid? ToGuidOrNull(this string str)
        {
            Guid guid;
            if (Guid.TryParse(str, out guid))
                return guid;
            return null;
        }

        #endregion

        #region 转换为int long decimal byte

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string str, int defaultValue = 0)
        {
            int num;
            if (int.TryParse(str, out num))
                return num;
            return defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int? ToIntOrNull(this string str, int? defaultValue = null)
        {
            int num;
            if (int.TryParse(str, out num))
                return num;
            return defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToLong(this string str, long defaultValue = 0L)
        {
            long num;
            if (long.TryParse(str, out num))
                return num;
            return defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long? ToLongOrNull(this string str, long? defaultValue = null)
        {
            long num;
            if (long.TryParse(str, out num))
                return num;
            return defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string str, decimal defaultValue = 0m)
        {
            decimal num;
            if (decimal.TryParse(str, out num))
                return num;
            return defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal? ToDecimalOrNull(this string str, decimal? defaultValue = null)
        {
            decimal num;
            if (decimal.TryParse(str, out num))
                return num;
            return defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static byte ToByte(this string str, byte defaultValue = 0)
        {
            byte num;
            if (byte.TryParse(str, out num))
                return num;
            return defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static byte? ToByteOrNull(this string str, byte? defaultValue = null)
        {
            byte num;
            if (byte.TryParse(str, out num))
                return num;
            return defaultValue;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ToBoolean(this string str, bool defaultValue = false)
        {
            str = string.IsNullOrWhiteSpace(str) ? string.Empty : str;
            if (("0" == str.Trim()) || ("1" == str.Trim())) return Convert.ToBoolean(int.Parse(str));

            bool result;
            if (bool.TryParse(str, out result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool? ToBooleanOrNull(this string str, bool? defaultValue = null)
        {
            str = string.IsNullOrWhiteSpace(str) ? string.Empty : str;
            if (("0" == str.Trim()) || ("1" == str.Trim())) return Convert.ToBoolean(int.Parse(str));

            bool result;
            if (bool.TryParse(str, out result))
                return result;
            return defaultValue;
        }

        #region 转换为DateTime

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string str)
        {
            DateTime time;
            var formats = new[]
            {
                "yyyy-MM-dd",
                "yyyy-M-dd",
                "yyyy-MM-d",
                "yyyy-M-d",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-M-dd HH:mm:ss",
                "yyyy-MM-d HH:mm:ss",
                "yyyy-M-d HH:mm:ss",
                "yyyy-MM-dd HH:mm",
                "yyyy-M-dd HH:mm",
                "yyyy-MM-d HH:mm",
                "yyyy-M-d HH:mm",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyyMMdd",
                "yyyyMMdd HH:mm:ss",
                "yyyy/MM/dd",
                "yyyy/M/dd",
                "yyyy/MM/d",
                "yyyy/M/d",
                "yyyy/MM/dd HH:mm:ss",
                "MM/dd/yyyy",
                "M/dd/yyyy",
                "MM/d/yyyy",
                "M/d/yyyy",
                "MM/dd/yyyy HH:mm:ss"
            };
            return DateTime.TryParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
                out time);
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str, DateTime defaultValue)
        {
            DateTime time;
            var formats = new[]
            {
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-d HH:mm:ss",
                "yyyy-M-dd HH:mm:ss",
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-d HH:mm",
                "yyyy-M-dd HH:mm",
                "yyyy-MM-dd",
                "yyyy-MM-d",
                "yyyy-M-dd",
                "yyyy-M-d",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyyMMdd",
                "yyyyMMdd HH:mm:ss",
                "yyyy/MM/dd",
                "yyyy/M/dd",
                "yyyy/MM/d",
                "yyyy/M/d",
                "yyyy/MM/dd HH:mm:ss",
                "MM/dd/yyyy",
                "M/dd/yyyy",
                "MM/d/yyyy",
                "M/d/yyyy",
                "MM/dd/yyyy HH:mm:ss"
            };
            if (DateTime.TryParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
                out time))
                return time;
            return defaultValue;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(this string str, TimeSpan defaultValue)
        {
            TimeSpan result;
            if (TimeSpan.TryParse(str, out result))
                return result;
            return defaultValue;
        }

        #region 转换为枚举Enum

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value) where T : struct
        {
            return value.ToEnum(default(T));
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, bool ignoreCase) where T : struct
        {
            return value.ToEnum(default(T), ignoreCase);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            return value.ToEnum(defaultValue, true);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, T defaultValue, bool ignoreCase) where T : struct
        {
            T local;
            if (Enum.TryParse(value, ignoreCase, out local) && Enum.IsDefined(typeof(T), local))
                return local;
            return defaultValue;
        }

        #endregion

        #region 获取枚举Enum值

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetEnumValue<T>(this string value, int defaultValue = 0) where T : struct
        {
            return value.GetEnumValue<T>(defaultValue, true);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetEnumValue<T>(this T value, int defaultValue) where T : struct
        {
            if (!typeof(T).IsEnum)
                return defaultValue;
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static int GetEnumValue<T>(this string value, int defaultValue, bool ignoreCase) where T : struct
        {
            T local;
            if (Enum.TryParse(value, ignoreCase, out local))
                return Convert.ToInt32(local);
            return defaultValue;
        }

        #endregion

        /// <summary>
        ///     转换为IP地址
        /// </summary>
        /// <param name="ipstring"></param>
        /// <returns>如果转换失败，返回 IPAddress.None</returns>
        public static IPAddress ToIPAddress(this string ipstring)
        {
            IPAddress address;
            if (IPAddress.TryParse(ipstring, out address))
                return address;
            return IPAddress.None;
        }

        #endregion

        #region 验证格式

        /// <summary>
        ///     验证邮箱
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmail(this string s)
        {
            return Regex.IsMatch(s,
                @"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$",
                RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///     验证手机号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsMobile(this string s)
        {
            return Regex.IsMatch(s, @"^1[3458]\d{9}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///     验证是否是固定电话
        ///     <para>局限：中国</para>
        ///     <para>特殊热线电话除外(如:88889999)</para>
        ///     <para>格式010-85849685</para>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsTelephone(this string s)
        {
            return Regex.IsMatch(s, @"^\d{3,4}-?\d{6,8}$", RegexOptions.IgnoreCase);
        }
        #region 身份证号码验证
        /// <summary>
        ///     验证身份证是否有效
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIDCard(this string s)
        {
            if (s.Length == 18)
                return IsIDCard18(s);
            if (s.Length == 15)
                return IsIDCard15(s);
            return false;
        }

        /// <summary>
        ///     验证是否为18位有效身份证号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIDCard18(this string s)
        {
            //数字验证
            long n = 0;
            if (!long.TryParse(s.Remove(17), out n) || (n < Math.Pow(10, 16)) ||
                !long.TryParse(s.Replace('x', '0').Replace('X', '0'), out n))
                return false;
            //省份验证
            var address =
                "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(s.Remove(2)) < 0)
                return false;
            //生日验证
            var birth = s.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            var time = new DateTime();
            if (!DateTime.TryParse(birth, out time))
                return false;
            //校验码验证
            var arrVarifyCode = "1,0,x,9,8,7,6,5,4,3,2".Split(',');
            var Wi = "7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2".Split(',');
            var Ai = s.Remove(17).ToCharArray();
            var sum = 0;
            for (var i = 0; i < 17; i++)
                sum += int.Parse(Wi[i])*int.Parse(Ai[i].ToString());
            var y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != s.Substring(17, 1).ToLower())
                return false;
            //符合GB11643-1999标准
            return true;
        }

        /// <summary>
        ///     验证是否为15位有效身份证号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIDCard15(this string s)
        {
            //数字验证
            long n = 0;
            if (!long.TryParse(s, out n) || (n < Math.Pow(10, 14)))
                return false;
            //省份验证
            var address =
                "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(s.Remove(2)) == -1)
                return false;
            //生日验证
            var birth = s.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            var time = new DateTime();
            if (!DateTime.TryParse(birth, out time))
                return false;
            //符合15位身份证标准
            return true;
        }
        #endregion
        /// <summary>
        ///     邮政编码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsPostCode(this string s)
        {
            return Regex.IsMatch(s, @"^\d{6}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///     验证IP
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsIP(string source)
        {
            return Regex.IsMatch(source,
                @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$",
                RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///     验证是否是正确的Url
        /// </summary>
        /// <param name="url">要验证的Url</param>
        /// <returns></returns>
        public static bool IsURL(this string url)
        {
            //var regex = @"^(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&amp;%_\./-~-]*)?$";
            return Regex.IsMatch(url,
                @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }

        /// <summary>
        ///     验证是否是正常字符，字母，数字，下划线的组合
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsAlphanum(this string s)
        {
            return Regex.IsMatch(s, @"[\w\d_]+", RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///     验证是否为中文
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsChinese(this string s)
        {
            return Regex.IsMatch(s, @"^[\u4e00-\u9fa5]+$", RegexOptions.IgnoreCase);
        }

        #endregion

        #region 其他扩展方法

        /// <summary>
        ///     修正URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="defaultPrefix"></param>
        /// <returns></returns>
        public static string UrlFix(this string url, string defaultPrefix = "")
        {
            var tmp = url.Trim();
            if (!Regex.Match(tmp, "^(http|https):").Success)
                tmp = string.Format("{0}/{1}", defaultPrefix, tmp);
            tmp = Regex.Replace(tmp, @"(?<!(http|https):)[\\/]+", "/").Trim();
            return tmp;
        }

        /// <summary>
        ///     判断URl是否有效
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer">防盗链时启用</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public static bool UrlValid(this string url, string referer = "", int timeout = 3000)
        {
            url = url.UrlFix();

            var sw = new Stopwatch();
            sw.Restart();
            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    var myRequest = (HttpWebRequest) WebRequest.Create(url);
                    myRequest.Method = "HEAD";
                    if (!string.IsNullOrWhiteSpace(referer))
                    {
                        myRequest.Referer = referer;
                        myRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Maxthon;)";
                    }
                    myRequest.Timeout = timeout;
                    myRequest.ReadWriteTimeout = timeout;
                    var res = myRequest.GetResponse() as HttpWebResponse;

                    return res.StatusCode == HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    return false;
                }
            });
            while (true)
            {
                if (task.IsCompleted) return task.Result;
                if (sw.ElapsedMilliseconds >= timeout)
                    return false;
            }
        }

        /// <summary>
        ///     从URL中取 Key / Value
        /// </summary>
        /// <param name="s"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseString(this string s, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(s))
                return new Dictionary<string, string>();

            if (s.IndexOf('?') != -1)
                s = s.Remove(0, s.IndexOf('?'));

            var kvs = new Dictionary<string, string>();
            var reg = new Regex(@"[\?&]?(?<key>[^=]+)=(?<value>[^\&]*)", RegexOptions.Compiled | RegexOptions.Multiline);
            var ms = reg.Matches(s);
            string key;
            foreach (Match ma in ms)
            {
                key = ignoreCase ? ma.Groups["key"].Value.ToLower() : ma.Groups["key"].Value;
                if (kvs.ContainsKey(key))
                    kvs[key] += "," + ma.Groups["value"].Value;
                else
                    kvs[key] = ma.Groups["value"].Value;
            }
            return kvs;
        }

        /// <summary>
        ///     从 URL 中取出键的值, 如果不存在,返回空
        /// </summary>
        /// <param name="s"></param>
        /// <param name="key"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static string ParseString(this string s, string key, bool ignoreCase)
        {
            var dictionary = s.ParseString(ignoreCase);
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return string.Empty;
        }

        /// <summary>
        ///     设置 URL中的 key
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string SetUrlKeyValue(this string url, string key, string value, Encoding encode)
        {
            if (string.IsNullOrWhiteSpace(url))
                url = string.Empty;

            if (!string.IsNullOrEmpty(url.ParseString(key, true).Trim()))
            {
                var reg = new Regex(@"([\?\&])(" + key + @"=)([^\&]*)(\&?)");
                return reg.Replace(url, "$1$2" + HttpUtility.UrlEncode(value, encode) + "$4");
            }
            return url + (url.IndexOf('?') > -1 ? "&" : "?") + key + "=" + HttpUtility.UrlEncode(value, encode);
        }

        /// <summary>
        ///     虚拟路径转化为绝对路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToAbsolute(this string url)
        {
            return VirtualPathUtility.ToAbsolute(url);
        }

        /// <summary>
        ///     字符串实际长度
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static int ActualLength(this string s)
        {
            return Encoding.UTF8.GetBytes(s).Length;
        }

        /// <summary>
        ///     字符串中的中文字符
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static List<char> ChineseChar(this string s)
        {
            var list = new List<char>();
            foreach (var c in s.ToCharArray())
                if (Encoding.UTF8.GetBytes(c.ToString()).Length > 1)
                    list.Add(c);
            return list;
        }

        /// <summary>
        ///     过滤Html标记
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FilterHtml(this string s)
        {
            //定义script的正则表达式{或<script[^>]*?>[\\s\\S]*?<\\/script> } 
            var regScript = "<[\\s]*?script[^>]*?>[\\s\\S]*?<[\\s]*?\\/[\\s]*?script[\\s]*?>";
            //定义style的正则表达式{或<style[^>]*?>[\\s\\S]*?<\\/style> }
            var regStyle = "<[\\s]*?style[^>]*?>[\\s\\S]*?<[\\s]*?\\/[\\s]*?style[\\s]*?>";
            //定义HTML标签的正则表达式
            var regHtml = "<[^>]+>";

            var regex = new Regex(regScript, RegexOptions.IgnoreCase);
            s = regex.Replace(s, string.Empty);

            regex = new Regex(regStyle, RegexOptions.IgnoreCase);
            s = regex.Replace(s, string.Empty);

            regex = new Regex(regHtml, RegexOptions.IgnoreCase);
            return regex.Replace(s, string.Empty);
        }

        /// <summary>
        ///     过滤Sql关键字
        ///     <para>防Sql注入</para>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FilterSqlKey(this string s)
        {
            var sqlKey =
                @"and|or|exec|execute|insert|select|delete|update|alter|create|drop|count|\*|chr|char|asc|mid|substring|master|truncate|declare|xp_cmdshell|restore|backup|net +user|net +localgroup +administrators";
            if (!string.IsNullOrWhiteSpace(s))
            {
                var strRegex = @"\b(" + sqlKey + @")\b";

                var regex = new Regex(strRegex, RegexOptions.IgnoreCase);
                var matches = regex.Matches(s);
                for (var i = 0; i < matches.Count; i++)
                    s = s.Replace(matches[i].Value, string.Empty);
            }
            return s;
        }

        /// <summary>
        ///     格式化成多行
        /// </summary>
        /// <param name="s"></param>
        /// <param name="rowLength"></param>
        /// <returns></returns>
        public static string ToMultiline(this string s, int rowLength)
        {
            var length = Encoding.Default.GetBytes(s).Length;
            if (length <= rowLength)
                return s;

            var sb = new StringBuilder();
            var rowCount = 0;
            for (var i = 0; i < s.Length; i++)
            {
                sb.Append(s.Substring(i, 1));
                var bytes = Encoding.Default.GetBytes(s.Substring(i, 1));
                if (bytes.Length > 1)
                    rowCount += 2;
                else
                    rowCount++;

                if (rowCount >= rowLength)
                {
                    rowCount = 0;
                    sb.Append("\r\n");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///     把秒转成时分秒
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FormatSecond(this string str)
        {
            var second = str.ToInt(0);
            var h = second/3600;
            var m = second%3600/60;
            var s = second%60;
            return h > 0 ? h + "时:" + m + "分:" + s + "秒" : m > 0 ? m + "分" + s + "秒" : (s > 0 ? s + "秒" : "空缺");
        }

        #endregion
    }
}