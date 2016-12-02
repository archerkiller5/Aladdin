// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : XmlSerializeHelper.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:23
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

//======================================================================
//
//        Copyright (C) 2014-2016 Magicodes团队    
//        All rights reserved
//
//        filename :XmlSerializeHelper
//        description :
//
//        created by 雪雁 at  2015/7/10 17:02:08
//        http://www.magicodes.net
//
//======================================================================

namespace Magicodes.WeiChat.Infrastructure.Serialize
{
    public class XmlSerializeHelper
    {
        #region XML序列化

        /// <summary>
        ///     文件化XML序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        public static void Save(object obj, string filename)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }

        /// <summary>
        ///     文件化XML反序列化
        /// </summary>
        /// <param name="filename">文件路径</param>
        public static T Load<T>(string filename)
        {
            return (T) Load(typeof(T), filename);
        }

        /// <summary>
        ///     文件化XML反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filename">文件路径</param>
        public static object Load(Type type, string filename)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var serializer = new XmlSerializer(type);
                return serializer.Deserialize(fs);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }

        /// <summary>
        ///     文件化XML反序列化
        /// </summary>
        /// <param name="strXml">xml内容</param>
        public static T LoadFromStr<T>(string strXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(strXml))
            {
                return (T) xmlSerializer.Deserialize(stringReader);
            }
        }

        /// <summary>
        ///     文本化XML序列化
        /// </summary>
        /// <param name="item">对象</param>
        public static string ToXml<T>(T item)
        {
            var serializer = new XmlSerializer(item.GetType());
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                serializer.Serialize(writer, item);
                return sb.ToString();
            }
        }

        /// <summary>
        ///     文本化XML序列化
        /// </summary>
        /// <param name="item">对象</param>
        public static string ToXmlAndFormat<T>(T item)
        {
            var serializer = new XmlSerializer(item.GetType());
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (var writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 1;
                writer.IndentChar = '\t';
                //xd.WriteTo(xtw);
                serializer.Serialize(writer, item);
                return sb.ToString();
            }
        }

        /// <summary>
        ///     文本化XML序列化
        /// </summary>
        /// <param name="item">对象</param>
        /// <param name="path">路径</param>
        public static void ToXmlAndFormat<T>(T item, string path)
        {
            File.WriteAllText(path, ToXmlAndFormat(item));
        }

        #endregion
    }
}