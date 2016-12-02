using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Magicodes.WeiChat.Unity
{
    public class XmlHelper
    {
        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(T obj)
        {
            using (MemoryStream Stream = new MemoryStream())
            {
                XmlSerializer xmlSerial = new XmlSerializer(obj.GetType());
                //序列化对象
                xmlSerial.Serialize(Stream, obj);
                Stream.Position = 0;
                using (StreamReader sr = new StreamReader(Stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        ///// <summary>
        ///// 输出XML
        ///// </summary>
        ///// <returns></returns>
        //public static string ParseXML<T>(T obj)
        //{
        //    var dict = obj as Dictionary<string, string>;
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("<xml>");
        //    foreach (string k in dict.Keys)
        //    {

        //        if (!string.IsNullOrEmpty(dict[k]))
        //        {
        //            string v = (string)dict[k];
        //            if (Regex.IsMatch(v, @"^[0-9.]$"))
        //            {

        //                sb.Append("<" + k + ">" + v + "</" + k + ">");
        //            }
        //            else
        //            {
        //                sb.Append("<" + k + "><![CDATA[" + v + "]]></" + k + ">");
        //            }
        //        }

        //    }
        //    sb.Append("</xml>");
        //    return sb.ToString();
        //}


        //public static T FromXml<T>(string xml)
        //{
        //    PropertyInfo[] props = typeof(T).GetProperties();

        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    if (string.IsNullOrEmpty(xml))
        //    {
        //        throw new Exception("将空的xml串转换不合法!");
        //    }
        //    XmlDocument xmlDoc = new XmlDocument();
        //    xmlDoc.LoadXml(xml);
        //    XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
        //    XmlNodeList nodes = xmlNode.ChildNodes;
        //    foreach (XmlNode xn in nodes)
        //    {
        //        XmlElement xe = (XmlElement)xn;
        //        foreach (PropertyInfo prop in props)
        //        {
        //            object[] attrs = prop.GetCustomAttributes(true);
        //            foreach (object attr in attrs)
        //            {
        //                XmlAttributeAttribute authAttr = attr as XmlAttributeAttribute;
        //                if (authAttr != null)
        //                {
        //                    if (xe.Name == authAttr.AttributeName)
        //                    {
        //                        dict.Add(prop.Name, xe.InnerText);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    var _str = JsonConvert.SerializeObject(dict);

        //    return JsonConvert.DeserializeObject<T>(_str);
        //}


        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(Stream stream)
        {
            XmlSerializer xmlSerial = new XmlSerializer(typeof(T));
            return (T)xmlSerial.Deserialize(stream);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string input) where T : class
        {
            using (MemoryStream mem = new MemoryStream(Encoding.Default.GetBytes(input)))
            {
                using (XmlReader reader = XmlReader.Create(mem))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(T));
                    return formatter.Deserialize(reader) as T;
                }
            }
        }
    }
}
