// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : EncoderHelper.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Text;
using Encoder = Microsoft.Security.Application.Encoder;

namespace Magicodes.WeiChat.Infrastructure.MvcExtension.Filters.Helper
{
    public class EncoderHelper
    {
        public static string CssEncode(string str)
        {
            return Encoder.CssEncode(str);
        }

        public static string HtmlAttributeEncode(string str)
        {
            return Encoder.HtmlAttributeEncode(str);
        }

        public static string HtmlEncode(string str, bool useNamedEntities)
        {
            return Encoder.HtmlEncode(str, useNamedEntities);
        }

        public static string HtmlEncode(string str)
        {
            return Encoder.HtmlEncode(str);
        }

        public static string HtmlFormUrlEncode(string str, Encoding inputEncoding)
        {
            return Encoder.HtmlFormUrlEncode(str, inputEncoding);
        }

        public static string HtmlFormUrlEncode(string str, int codePage)
        {
            return Encoder.HtmlFormUrlEncode(str, codePage);
        }

        public static string HtmlFormUrlEncode(string str)
        {
            return Encoder.HtmlFormUrlEncode(str);
        }

        public static string JavascriptEncode(string str, bool emitQuotes = false)
        {
            return Encoder.JavaScriptEncode(str, emitQuotes);
        }

        public static string JavascriptEncode(string str)
        {
            return Encoder.JavaScriptEncode(str);
        }

        public static string LdapDistinguishedNameEncode(string input, bool useInitialCharacterRules,
            bool useFinalCharacterRule)
        {
            return Encoder.LdapDistinguishedNameEncode(input, useInitialCharacterRules, useFinalCharacterRule);
        }

        public static string LdapDistinguishedNameEncode(string input)
        {
            return Encoder.LdapDistinguishedNameEncode(input);
        }

        public static string LdapEncode(string input)
        {
            return Encoder.LdapFilterEncode(input);
        }

        public static string LdapFilterEncode(string input)
        {
            return Encoder.LdapFilterEncode(input);
        }

        public static string UrlEncode(string input, Encoding inputEncoding)
        {
            return Encoder.UrlEncode(input, inputEncoding);
        }

        public static string UrlEncode(string input, int codePage)
        {
            return Encoder.UrlEncode(input, codePage);
        }

        public static string UrlEncode(string input)
        {
            return Encoder.UrlEncode(input);
        }

        public static string UrlPathEncode(string input)
        {
            return Encoder.UrlPathEncode(input);
        }

        public static string VisualBasicScriptEncode(string input)
        {
            return Encoder.VisualBasicScriptEncode(input);
        }

        public static string XmlAttributeEncode(string input)
        {
            return Encoder.XmlAttributeEncode(input);
        }

        public static string XmlEncode(string input)
        {
            return Encoder.XmlEncode(input);
        }
    }
}