using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MatchModel
{
    /// <summary>
    /// XML工具函数
    /// </summary>
    public class XmlTool
    {

        public static void WriteXML(object obj, string fileName)
        {
            string DirectoryName = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(DirectoryName) && !string.IsNullOrEmpty(DirectoryName))
            {
                Directory.CreateDirectory(DirectoryName);
            }
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.OmitXmlDeclaration = true;
            ws.Indent = true;
            StreamWriter sw = new StreamWriter(fileName, false);
            try
            {
                if (null == obj)
                {
                    throw new ArgumentNullException("obj", "obj对象不能为null");
                }


                using (XmlWriter writer = XmlWriter.Create(sw, ws))
                {
                    //去除默认命名空间xmlns:xsd和xmlns:xsi
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    XmlSerializer formatter = new XmlSerializer(obj.GetType());
                    formatter.Serialize(writer, obj, ns);
                    writer.Close();
                }
                sw.Close();
            }
            catch (Exception e)
            {
                sw.Close();
                throw e;
            }

        }
        public static void AppendXML(object obj, string fileName)
        {
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.OmitXmlDeclaration = true;
            ws.Indent = true;
            StreamWriter sw = new StreamWriter(fileName, true);
            try
            {
                if (null == obj)
                {
                    throw new ArgumentNullException("obj", "obj对象不能为null");
                }


                using (XmlWriter writer = XmlWriter.Create(sw, ws))
                {
                    //去除默认命名空间xmlns:xsd和xmlns:xsi
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    XmlSerializer formatter = new XmlSerializer(obj.GetType());
                    formatter.Serialize(writer, obj, ns);
                    writer.Close();
                }
                sw.Close();
            }
            catch (Exception e)
            {
                sw.Close();
                throw e;
            }

        }
        public static object ReadXML(string fileName, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type", "type不能为null");
            }
            XmlSerializer serializer = new XmlSerializer(type);
            StreamReader sr = new StreamReader(fileName, Encoding.UTF8);
            try
            {
                object obj = serializer.Deserialize(sr);
                sr.Close();
                return obj;
            }
            catch (Exception e)
            {
                sr.Close();
                throw e;
            }

        }
        public static void Serializable(string filepath, object data)
        {
            BinaryFormatter binaryFomat = new BinaryFormatter();
            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                binaryFomat.Serialize(fs, data);
                fs.Dispose();
            }
        }

        public static object DeSerializable(string filepath, Type type)
        {
            BinaryFormatter binaryFomat = new BinaryFormatter();
            object obj;
            using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate))
            {
                obj = binaryFomat.Deserialize(fs);
                fs.Dispose();
            }
            return obj;
        }

        #region 枚举操作
        /// <summary>
        /// 通过字符串获取枚举对应描述值
        /// </summary>
        /// <param name="value"></param>
        public static string GetEnumDescription<T>(string value) where T : Enum
        {
            T enumValue = (T)Enum.Parse(typeof(T), value);

            FieldInfo fInfo = enumValue.GetType().GetField(value);
            object[] obj = fInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (obj.Length == 0)
            {
                return value;
            }
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)obj[0];
            return descriptionAttribute.Description;
        }

        /// <summary>
        /// 通过枚举描述获取枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T GetEnumByDescription<T>(string description) where T : Enum
        {
            FieldInfo[] fieldInfos = typeof(T).GetFields();
            foreach (FieldInfo fInfo in fieldInfos)
            {
                object[] obj = fInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (obj.Length > 0 && (obj[0] as DescriptionAttribute).Description == description)
                {
                    return (T)fInfo.GetValue(null);
                }
            }

            throw new Exception($"{description} 未能找到对应的枚举");
        }
        #endregion
    }
}
