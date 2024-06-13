using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace XMLFile
{
    public class XMLFileOperation
    {
        #region 读取XML到DataSet
        /// <summary>
        /// 功能:读取XML到DataSet中
        /// </summary>
        /// <param name="xmlFilePath">xml路径</param>
        /// <returns>DataSet</returns>
        public static DataSet ConvertXMLFileToDataSet(string xmlFilePath)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(xmlFilePath);
            return ds;
        }
        #endregion

        #region 读取DataSet到XML文件里并保存
        /// <summary>
        /// 功能:读取DataSet到XML文件里并保存
        /// </summary>
        /// <param name="xmlDS">DataSet</param>
        /// <param name="xmlFilePath">xml路径</param>
        /// <returns></returns>
        public static void ConvertDataSetToXMLFile(DataSet xmlDS, string xmlFilePath)
        {
            xmlDS.WriteXml(xmlFilePath);
        }
        #endregion

        #region 查找数据,返回当前节点的所有下级节点,填充到一个DataSet中
        /// <summary>
        /// 查找数据,返回当前节点的所有下级节点,填充到一个DataSet中
        /// </summary>
        /// <param name="xmlFilePath">xml文档路径</param>
        /// <param name="XmlNodePath">节点的路径:根节点/父节点/当前节点</param>
        /// <returns>DataSet</returns>
        public static DataSet GetXmlData(string xmlFilePath, string XmlNodePath)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(xmlFilePath);
            DataSet ds = new DataSet();
            StringReader read = new StringReader(xmldoc.SelectSingleNode(XmlNodePath).OuterXml);
            ds.ReadXml(read);
            return ds;
        }
        #endregion

        #region 读取xml并返回一个节点
        /// <summary>
        /// 读取xml并返回一个节点:适用于一级节点
        /// </summary>
        /// <param name="xmlFilePath">xml路径</param>
        /// <param name="nodeName">节点</param>
        /// <returns>string</returns>
        public static string ReadXmlReturnNode(string xmlFilePath, string nodeName)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(xmlFilePath);
            XmlNodeList nodeList = xmldoc.GetElementsByTagName(nodeName);
            return nodeList.Item(0).InnerText.ToString();
        }
        #endregion

        #region 修改xml声明
        /// <summary>
        /// 修改xml声明
        /// </summary>
        /// <param name="xmlFilePath">xml路径</param>
        /// <param name="xmldecl_Encoding">声明的Encoding</param>
        /// <param name="xmldecl_Standalone">声明的Standalone</param>
        /// <returns></returns>
        public static void ModifyXMLDecl(string xmlFilePath, string xmldecl_Encoding, string xmldecl_Standalone)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(xmlFilePath);
            XmlDeclaration xmldecl = (XmlDeclaration)xmldoc.FirstChild;
            xmldecl.Encoding = xmldecl_Encoding;
            xmldecl.Standalone = xmldecl_Standalone;
            xmldoc.Save(xmlFilePath);
        }
        #endregion

        #region 读取节点的值或者属性
        /// <summary>
        /// 读取节点的值或者属性
        /// </summary>
        /// <param name="xmlFilePath">xml路径</param>
        /// <param name="nodePath">节点路径</param>
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
        /// <returns>string</returns>
        /**************************************************
         * 使用示列:
         * XMLFileOperation.readNodeValueOrAttribute(XmlFilePath, "/NodeName", "")
         * XMLFileOperation.readNodeValueOrAttribute(XmlFilePath, "/NodeName/Element[@Attribute='Name']", "Attribute")
         ************************************************/
        public static string readNodeValueOrAttribute(string xmlFilePath, string nodePath, string attribute)
        {
            string value = "";
            //try
            //{
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNode node = xmlDoc.SelectSingleNode(nodePath);
            value = (attribute.Equals("") ? node.InnerText : node.Attributes[attribute].Value);
            //}
            //catch { }
            return value;
        }
        #endregion

        #region 修改节点的值或者属性(例:XMLFileOperation.updateNodeValueOrAttribute(XmlFilePath, "/NodeName[@Attribute='Name']", "", "Value)//,修改指定属性的节点值)
        /// <summary>
        /// 修改节点的值或者属性
        /// </summary>
        /// <param name="xmlFilePath">xml路径</param>
        /// <param name="nodePath">节点路径</param>
        /// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XMLFileOperation.updateNodeValueOrAttribute(XmlFilePath, "/NodeName", "", "Value")
         * XMLFileOperation.updateNodeValueOrAttribute(XmlFilePath, "/NodeName[@Attribute='Name']", "", "Value)//根据节点属性,修改节点值
         * XMLFileOperation.updateNodeValueOrAttribute(XmlFilePath, "/NodeName", "Attribute", "Value")
         ************************************************/
        public static void updateNodeValueOrAttribute(string xmlFilePath, string nodePath, string attribute, string value)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);
                XmlNode node = xmlDoc.SelectSingleNode(nodePath);
                XmlElement xmlEle = (XmlElement)node;
                if (attribute.Equals(""))
                    xmlEle.InnerText = value;
                else
                    xmlEle.SetAttribute(attribute, value);
                xmlDoc.Save(xmlFilePath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 插入新节点或者元素
        /// <summary>
        /// 插入新节点或者元素
        /// </summary>
        /// <param name="xmlFilePath">路径</param>
        /// <param name="nodeName">节点</param>
        /// <param name="element">节点或者元素名，非空时插入新节点或者元素，否则在该节点或者元素中插入属性</param>
        /// <param name="attribute">属性名，非空时插入该节点或者元素属性值，否则插入节点或者元素值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XMLFileOperation.Insert(XmlFilePath, "/NodeName", "Element", "", "Value")
         * XMLFileOperation.Insert(XmlFilePath, "/NodeName", "Element", "Attribute", "Value")
         * XMLFileOperation.Insert(XmlFilePath, "/NodeName", "", "Attribute", "Value")
         ************************************************/
        public static void Insert(string xmlFilePath, string nodeName, string element, string attribute, string value)
        {
            //try
            //{
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNode node = xmlDoc.SelectSingleNode(nodeName);
            if (element.Equals(""))
            {
                if (!attribute.Equals(""))
                {
                    XmlElement xmlEle = (XmlElement)node;
                    xmlEle.SetAttribute(attribute, value);
                }
            }
            else
            {
                XmlElement xmlEle = xmlDoc.CreateElement(element);
                if (attribute.Equals(""))
                    xmlEle.InnerText = value;
                else
                    xmlEle.SetAttribute(attribute, value);
                node.AppendChild(xmlEle);
            }
            xmlDoc.Save(xmlFilePath);
            //}
            //catch { }
        }
        #endregion

        #region 删除节点或者属性
        /// <summary>
        /// 删除节点或者属性
        /// </summary>
        /// <param name="xmlFilePath">路径</param>
        /// <param name="nodeName">节点</param>
        /// <param name="attribute">属性名,非空时删除该节点属性值,否则删除节点</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XMLFileOperation.Delete(XmlFilePath, "/NodeName", "")
         * XMLFileOperation.Delete(XmlFilePath, "/NodeName", "Attribute")
         ************************************************/
        public static void Delete(string xmlFilePath, string nodeName, string attribute)
        {
            //try
            //{
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNode node = xmlDoc.SelectSingleNode(nodeName);
            XmlElement xmlEle = (XmlElement)node;
            if (attribute.Equals(""))
                node.ParentNode.RemoveChild(node);
            else
                xmlEle.RemoveAttribute(attribute);
            xmlDoc.Save(xmlFilePath);
            //}
            //catch { }
        }
        #endregion

        #region 更新Xml节点内容
        /// <summary>
        /// 更新Xml节点内容
        /// </summary>
        /// <param name="xmlFilePath">xml路径</param>
        /// <param name="nodeName">要更换内容的节点:节点路径 根节点/父节点/当前节点</param>
        /// <param name="Content">新的内容</param>
        public static void XmlNodeReplace(string xmlFilePath, string nodeName, string Content)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            xmlDoc.SelectSingleNode(nodeName).InnerText = Content;
            xmlDoc.Save(xmlFilePath);
        }
        #endregion

        #region 更新指定Index节点内容
        /// <summary>
        /// 更新指定Index节点内容
        /// </summary>
        /// <param name="xmlFilePath">xml路径</param>
        /// <param name="nodePath">要更换内容的节点:节点路径 根节点/父节点/当前节点</param>
        /// <param name="index">要更换内容的节点的index</param>
        /// <param name="Content">新的内容</param>
        /// <returns></returns>
        public static void XmlNodeReplace(string xmlFilePath, string nodePath, int index, string Content)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);

            XmlNodeList fnodes = xmlDoc.DocumentElement.SelectNodes(nodePath);
            for (int i = 0; i < fnodes.Count; i++)
            {
                if (i == index)
                {
                    fnodes[i].InnerText = Content;
                    break;
                }
            }
            xmlDoc.Save(xmlFilePath);
        }
        #endregion

        #region 插入一个节点和此节点的子节点
        /// <summary>
        /// 插入一个节点和此节点的子节点
        /// </summary>
        /// <param name="xmlFilePath">xml路径</param>
        /// <param name="mainNodeName">当前节点路径</param>
        /// <param name="childNodeName">新插入节点</param>
        /// <param name="element">插入节点的子节点</param>
        /// <param name="content">子节点的内容,已经应用CDATA</param>
        public static void XmlInsertNode(string xmlFilePath, string mainNodeName, string childNodeName, string element, string content)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNode objRootNode = xmlDoc.SelectSingleNode(mainNodeName);//或者当前节点
            XmlElement objChildNode = xmlDoc.CreateElement(childNodeName);//新建新插入节点
            objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
            XmlElement objElement = xmlDoc.CreateElement(element);//新建新插入节点的子节点

            //XmlCDataSection xcds = xmlDoc.CreateCDataSection(content);//新建新插入节点的子节点的内容
            //objElement.AppendChild(xcds);//内容APPEND到此子节点
            objElement.InnerText = content;

            objChildNode.AppendChild(objElement);//子节点Append到新插入节点
            xmlDoc.Save(xmlFilePath);
        }
        #endregion

        #region 插入一个节点和此节点的多个子节点
        /// <summary>
        /// 插入一个节点和此节点的多个子节点
        /// </summary>
        /// <param name="xmlFilePath">xml路径</param>
        /// <param name="mainNodeName">当前节点路径</param>
        /// <param name="childNodeName">新插入节点</param>
        /// <param name="elementsArray">插入节点的多个子节点</param>
        /// <param name="contentsArray">多个子节点的内容,已经应用CDATA</param>
        public static void XmlInsertNodes(string xmlFilePath, string mainNodeName, string childNodeName, string[] elementsArray, string[] contentsArray)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNode objRootNode = xmlDoc.SelectSingleNode(mainNodeName);//或者当前节点
            XmlElement objChildNode = xmlDoc.CreateElement(childNodeName);//新建新插入节点
            objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
            for (int i = 0; i < elementsArray.Length; i++)
            {
                XmlElement objElement = xmlDoc.CreateElement(elementsArray[i]);//新建新插入节点的子节点

                //XmlCDataSection xcds = xmlDoc.CreateCDataSection(content);//新建新插入节点的子节点的内容
                //objElement.AppendChild(xcds);//内容APPEND到此子节点
                objElement.InnerText = contentsArray[i];

                objChildNode.AppendChild(objElement);//子节点Append到新插入节点
            }
            xmlDoc.Save(xmlFilePath);
        }
        #endregion

        #region 插入一节点,带一属性
        /// <summary>
        /// 插入一节点,带一属性
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="fatherNodeName">当前节点路径</param>
        /// <param name="element">新节点名称</param>
        /// <param name="attrib">新节点属性名称</param>
        /// <param name="attribContent">新节点属性值</param>
        /// <param name="content">新节点值</param>
        public static void XmlInsertElement(string xmlFilePath, string fatherNodeName, string element, string attrib, string attribContent, string content)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNode fNode = xmlDoc.SelectSingleNode(fatherNodeName);
            XmlElement newNode = xmlDoc.CreateElement(element);
            newNode.SetAttribute(attrib, attribContent);
            newNode.InnerText = content;
            fNode.AppendChild(newNode);
            xmlDoc.Save(xmlFilePath);
        }
        #endregion

        #region 插入一节点不带属性
        /// <summary>
        /// 插入一节点不带属性
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="fatherNodeName">当前节点路径</param>
        /// <param name="element">新节点</param>
        /// <param name="content">新节点值</param>
        public static void XmlInsertElement(string xmlFilePath, string fatherNodeName, string element, string content)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNode fnode = xmlDoc.SelectSingleNode(fatherNodeName);
            XmlElement newNode = xmlDoc.CreateElement(element);
            newNode.InnerText = content;
            fnode.AppendChild(newNode);
            xmlDoc.Save(xmlFilePath);
        }
        #endregion

        #region 所有相同路径的节点下都插入一节点不带属性
        /// <summary>
        /// 所有相同路径的节点下都插入一节点不带属性
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="fatherNodePath">当前节点路径</param>
        /// <param name="element">新节点</param>
        /// <param name="contentAL">新节点值的ArrayList</param>
        public static void XmlInsertElementForEachOne(string xmlFilePath, string fatherNodePath, string element, System.Collections.ArrayList contentAL)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNodeList fnodes = xmlDoc.DocumentElement.SelectNodes(fatherNodePath);
            XmlElement newNode = xmlDoc.CreateElement(element);

            for (int i = 0; i < fnodes.Count; i++)
            {
                newNode.InnerText = contentAL[i].ToString();
                fnodes[i].AppendChild(newNode);
                xmlDoc.Save(xmlFilePath);
                xmlDoc.Load(xmlFilePath);
                fnodes = xmlDoc.DocumentElement.SelectNodes(fatherNodePath);
            }
        }
        #endregion

        #region 寻找指定值的节点然后修改此节点的值
        /// <summary>
        /// 寻找指定值的节点然后修改此节点的值
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="nodePath">指定节点的路径</param>
        /// <param name="nodeOldValue">节点原来的值</param>
        /// <param name="nodeNewValue">节点的修改值</param>
        /// <returns></returns>
        public static void modifyElementValue(string xmlFilePath, string nodePath, string nodeOldValue, string nodeNewValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(nodePath);
            foreach (XmlNode node in nodes)
            {
                if (node.InnerText == nodeOldValue)
                {
                    node.InnerText = nodeNewValue;
                    xmlDoc.Save(xmlFilePath);
                }
            }
        }
        #endregion

        #region 寻找指定序号的节点然后修改此节点的值
        /// <summary>
        /// 寻找指定序号的节点然后修改此节点的值
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="nodePath">指定节点的路径</param>
        /// <param name="nodeNumber">指定节点的序号</param>
        /// <param name="nodeNewValue">指定节点的修改值</param>
        /// <returns></returns>
        public static void modifyElementValue(string xmlFilePath, string nodePath, int nodeNumber, string nodeNewValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(nodePath);
            int i = 0;
            foreach (XmlNode node in nodes)
            {
                if (i + 1 == nodeNumber)
                {
                    node.InnerText = nodeNewValue;
                    xmlDoc.Save(xmlFilePath);
                }
                i++;
            }
        }
        #endregion

        #region 获取指定节点(ArrayList)的值
        /// <summary>
        /// 获取指定节点(ArrayList)的值
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="nodePath">指定节点的路径</param>
        /// <param name="nodeName">指定节点的名字</param>
        /// <returns>ArrayList</returns>
        public static System.Collections.ArrayList getElementValue(string xmlFilePath, string nodePath, string nodeName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(nodePath);
            foreach (XmlNode node in nodes)
            {
                if (node.Name == nodeName)
                {
                    al.Add(node.InnerText);
                }
            }
            return al;
        }
        #endregion

        #region 获取指定序号的节点的值
        /// <summary>
        /// 获取指定序号的节点的值
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="nodePath">指定节点的路径</param>
        /// <param name="nodeNumber">指定节点的序号</param>
        /// <returns>string</returns>
        public static string getElementValue(string xmlFilePath, string nodePath, int nodeNumber)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            string s = "";
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(nodePath);
            int i = 0;
            foreach (XmlNode node in nodes)
            {
                if (i + 1 == nodeNumber)
                {
                    s = node.InnerText;
                }
                i++;
            }
            return s;
        }
        #endregion

        #region 读取节点属性
        /// <summary>
        /// 读取节点属性
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="nodeName">节点名</param>
        /// <param name="nodeAttributeName">节点属性名</param>
        /// <returns>string</returns>
        public static string readNodeAttribute(string xmlFilePath, string nodeName, string nodeAttributeName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNode node = xmlDoc.SelectSingleNode(nodeName);
            XmlElement xmlEle = (XmlElement)node;
            return xmlEle.GetAttribute(nodeAttributeName);
        }
        #endregion

        #region 修改或新建节点属性
        /// <summary>
        /// 读取节点属性
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="nodePath">节点路径</param>
        /// <param name="nodeAttributeName">节点属性名</param>
        /// <param name="nodeAttribute">节点属性</param>
        public static void modifyOrCreateNodeAttribute(string xmlFilePath, string nodePath, string nodeAttributeName, string nodeAttribute)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlElement xmlEle = (XmlElement)xmlDoc.SelectSingleNode(nodePath);
            xmlEle.SetAttribute(nodeAttributeName, nodeAttribute);
            xmlDoc.Save(xmlFilePath);
        }
        #endregion

        #region 根据父节点属性值读取子节点值
        /// <summary>
        /// 根据父节点属性值读取子节点值
        /// </summary>
        /// <param name="xmlFilePath">xml路径</param>
        /// <param name="FatherElementName">父节点名</param>
        /// <param name="AttributeValue">属性值</param>
        /// <param name="AttributeIndex">属性索引:<0时不验证索引</param>
        /// <param name="ArrayLength">要返回的节点值数组长度:<=0时返回所有子节点值,>0时返回指定个数子节点值</param>
        /// <returns>ArrayList</returns>
        public static System.Collections.ArrayList GetSubElementByAttribute(string xmlFilePath, string FatherElementName, string AttributeValue, int AttributeIndex, int ArrayLength)
        {
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNodeList nodes = xmlDoc.DocumentElement.ChildNodes;
            //遍历第一层节点
            foreach (XmlElement element in nodes)
            {
                //判断父节点是否为指定节点
                if (element.Name == FatherElementName)
                {
                    if (AttributeIndex >= 0)
                    {
                        //判断父节点属性的索引是否大于指定索引
                        if (element.Attributes.Count - 1 < AttributeIndex)
                            return null;
                        //判断父节点的属性值是否等于指定属性
                        if (element.Attributes[AttributeIndex].Value == AttributeValue)
                        {
                            XmlNodeList cNodes = element.ChildNodes;
                            if (cNodes.Count > 0)
                            {
                                if (ArrayLength > 0)
                                {
                                    for (int i = 0; i < ArrayLength & i < cNodes.Count; i++)
                                    {
                                        al.Add(cNodes[i].InnerText);
                                    }
                                }
                                if (ArrayLength <= 0)
                                {
                                    for (int i = 0; i < cNodes.Count; i++)
                                    {
                                        al.Add(cNodes[i].InnerText);
                                    }
                                }

                            }
                        }
                    }
                    if (AttributeIndex < 0 && element.Attributes.Count > 0)
                    {
                        for (int i = 0; i < element.Attributes.Count; i++)
                        {
                            if (element.Attributes[i].Value == AttributeValue)
                            {
                                XmlNodeList cNodes = element.ChildNodes;
                                if (cNodes.Count > 0)
                                {
                                    if (ArrayLength > 0)
                                    {
                                        for (int j = 0; j < ArrayLength & j < cNodes.Count; i++)
                                        {
                                            al.Add(cNodes[j].InnerText);
                                        }
                                    }
                                    if (ArrayLength <= 0)
                                    {
                                        for (int j = 0; j < cNodes.Count; j++)
                                        {
                                            al.Add(cNodes[j].InnerText);
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return al;
        }
        #endregion

        #region 根据父节点属性名及属性值读取子节点值
        /// <summary>
        /// 根据父节点属性名及属性值读取子节点值
        /// </summary>
        /// <param name="xmlFilePath">xml路径</param>
        /// <param name="FatherElementName">父节点路径</param>
        /// <param name="AttributeName">属性名称</param>
        /// <param name="AttributeValue">属性值</param>
        /// <param name="ArrayLength">要返回的节点值数组长度:<=0时返回所有子节点值,>0时返回指定个数子节点值</param>
        /// <returns>ArrayList</returns>
        public static System.Collections.ArrayList GetSubElementByAttribute(string xmlFilePath, string FatherElementPath, string AttributeName, string AttributeValue, int ArrayLength)
        {
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNodeList fNodes;
            string path = FatherElementPath + "[" + "@" + AttributeName + "='" + AttributeValue + "']";
            fNodes = xmlDoc.DocumentElement.SelectNodes(path);
            if (fNodes.Count == 0)
                return null;
            XmlNodeList cNodes = fNodes.Item(0).ChildNodes;
            if (ArrayLength > 0)
            {
                for (int i = 0; i < ArrayLength & i < cNodes.Count; i++)
                {
                    al.Add(cNodes.Item(i).InnerText);
                }
            }
            if (ArrayLength <= 0)
            {
                for (int i = 0; i < cNodes.Count; i++)
                {
                    al.Add(cNodes.Item(i).InnerText);
                }
            }
            return al;
        }
        #endregion

        #region 寻找指定值的节点然后返回同级所有节点的值(ArrayList)
        /// <summary>
        /// 寻找指定值的节点然后返回同级所有节点的值(ArrayList)
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="nodePath">指定节点的路径</param>
        /// <param name="nodeValue">节点原来的值</param>
        /// <returns>ArrayList</returns>
        public static System.Collections.ArrayList findElementValue(string xmlFilePath, string nodePath, string nodeValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(nodePath);
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            foreach (XmlNode node in nodes)
            {
                if (node.InnerText == nodeValue)
                {
                    foreach (XmlNode nodeChild in node.ParentNode)
                    {
                        al.Add(nodeChild.InnerText);
                    }
                    break;
                }
            }
            return al;
        }
        #endregion

        #region 寻找指定值的节点然后返回节点的值(string)
        /// <summary>
        /// 寻找指定值的节点然后返回节点的值(string)
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="nodePath">指定节点的路径</param>
        /// <returns>ArrayList</returns>
        public static string findElementValue(string xmlFilePath, string nodePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNode node = xmlDoc.SelectSingleNode(nodePath);
            return node.InnerText;
        }
        #endregion

        #region 寻找指定值的节点然后更改同级节点的值
        /// <summary>
        /// 寻找指定值的节点然后更改同级节点的值
        /// </summary>
        /// <param name="xmlFilePath">Xml文档路径</param>
        /// <param name="nodePath">指定节点的路径</param>
        /// <param name="nodeValue">节点原来的值</param>
        /// <param name="updataNodeNamesArray">string[]:要更改的节点名</param>
        /// <param name="updataNodeContentsArray">string[]:要更改的节点值</param>
        /// <returns></returns>
        public static void updateElementValue(string xmlFilePath, string nodePath, string nodeValue, string[] updataNodeNamesArray, string[] updataNodeContentsArray)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(nodePath);
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            foreach (XmlNode node in nodes)
            {
                if (node.InnerText == nodeValue)
                {
                    foreach (XmlNode nodeChild in node.ParentNode)
                    {
                        for (int i = 0; i < updataNodeNamesArray.Length; i++)
                        {
                            if (nodeChild.Name == updataNodeNamesArray[i])
                            {
                                nodeChild.InnerText = updataNodeContentsArray[i];
                                xmlDoc.Save(xmlFilePath);
                                break;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 查找节点是否存在
        /// <summary>
        /// 通过读取指定节点数据查找对应节点数据组是否存在
        /// </summary>
        /// <param name="xmlFilePath">xml文件路径</param>
        /// <param name="nodePath">节点名称</param>
        /// <param name="nodeValue">节点值</param>
        /// <returns>true-存在，false-不存在</returns>
        public static bool checkNodeExistOrNot(string xmlFilePath, string nodePath, string nodeValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(nodePath);
            foreach (XmlNode nodeChild in nodes)
            {
                if (nodeChild.InnerText == nodeValue)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 查找指定名称的节点是否存在
        /// </summary>
        /// <param name="xmlFilePath">xml文件路径</param>
        /// <param name="nodePath">节点名称</param>
        /// <returns>true-存在，false-不存在</returns>
        public static bool checkNodeExistOrNot(string xmlFilePath, string nodePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFilePath);
                XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(nodePath);
                if (nodes.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }
        #endregion
        //必须创建对象才能使用的类
        private bool _alreadyDispose = false;
        private XmlDocument xmlDoc = new XmlDocument();
        private XmlNode xmlNode;
        private XmlElement xmlElem;
        #region 构造与释构
        public XMLFileOperation()
        {
        }
        ~XMLFileOperation()
        {
            Dispose();
        }
        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDispose) return;
            if (isDisposing)
            {
                //
            }
            _alreadyDispose = true;
        }
        #endregion
        #region IDisposable 成员
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region 创建xml文档
        /**/
        /**************************************************
     * 对象名称:XmlObject
     * 功能说明:创建xml文档
     * 使用示列:
     *     using XMLFile; //引用命名空间
     *     string xmlPath = Server.MapPath("test.xml");
     *     XMLFileOperation obj = new XMLFileOperation();
     *     创建根节点
     *     obj.CreateXmlRoot("root");
     *     // 创建空节点
     *     //obj.CreatXmlNode("root", "Node");
     *     //创建一个带值的节点
     *     //obj.CreatXmlNode("root", "Node", "带值的节点");
     *     //创建一个仅带属性的节点
     *     //obj.CreatXmlNode("root", "Node", "Attribe", "属性值");
     *     //创建一个仅带两个属性值的节点
     *     //obj.CreatXmlNode("root", "Node", "Attribe", "属性值", "Attribe2", "属性值2");
     *     //创建一个带属性值的节点值的节点
     *     // obj.CreatXmlNode("root", "Node", "Attribe", "属性值","节点值");
     *     //在当前节点插入带两个属性值的节点
     *     obj.CreatXmlNode("root", "Node", "Attribe", "属性值", "Attribe2", "属性值2","节点值");
     *     obj.XmlSave(xmlPath);
     *     obj.Dispose();
     ************************************************/
        #region 创建一个只有声明和根节点的XML文档
        /// <summary>
        /// 创建一个只有声明和根节点的XML文档
        /// </summary>
        /// <param name="root"></param>
        public void CreateXmlRoot(string root)
        {
            //加入XML的声明段落
            xmlNode = xmlDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            xmlDoc.AppendChild(xmlNode);
            //加入一个根元素
            xmlElem = xmlDoc.CreateElement("", root, "");
            xmlDoc.AppendChild(xmlElem);
        }
        #endregion
        #region 在当前节点下插入一个空节点节点
        /// <summary>
        /// 在当前节点下插入一个空节点节点
        /// </summary>
        /// <param name="mainNode">当前节点路径</param>
        /// <param name="node">节点名称</param>
        public void CreatXmlNode(string mainNode, string node)
        {
            XmlNode MainNode = xmlDoc.SelectSingleNode(mainNode);
            XmlElement objElem = xmlDoc.CreateElement(node);
            MainNode.AppendChild(objElem);
        }
        #endregion
        #region 在当前节点插入一个仅带值的节点
        /// <summary>
        /// 在当前节点插入一个仅带值的节点
        /// </summary>
        /// <param name="mainNode">当前节点</param>
        /// <param name="node">新节点</param>
        /// <param name="content">新节点值</param>
        public void CreatXmlNode(string mainNode, string node, string content)
        {
            XmlNode MainNode = xmlDoc.SelectSingleNode(mainNode);
            XmlElement objElem = xmlDoc.CreateElement(node);
            objElem.InnerText = content;
            MainNode.AppendChild(objElem);
        }
        #endregion
        #region 在当前节点的插入一个仅带属性值的节点
        /// <summary>
        /// 在当前节点的插入一个仅带属性值的节点
        /// </summary>
        /// <param name="MainNode">当前节点或路径</param>
        /// <param name="Node">新节点</param>
        /// <param name="Attrib">新节点属性名称</param>
        /// <param name="AttribValue">新节点属性值</param>
        public void CreatXmlNode(string MainNode, string Node, string Attrib, string AttribValue)
        {
            XmlNode mainNode = xmlDoc.SelectSingleNode(MainNode);
            XmlElement objElem = xmlDoc.CreateElement(Node);
            objElem.SetAttribute(Attrib, AttribValue);
            mainNode.AppendChild(objElem);
        }
        #endregion
        #region 创建一个带属性值的节点值的节点
        /// <summary>
        /// 创建一个带属性值的节点值的节点
        /// </summary>
        /// <param name="MainNode">当前节点或路径</param>
        /// <param name="Node">节点名称</param>
        /// <param name="Attrib">属性名称</param>
        /// <param name="AttribValue">属性值</param>
        /// <param name="Content">节点传情</param>
        public void CreatXmlNode(string MainNode, string Node, string Attrib, string AttribValue, string Content)
        {
            XmlNode mainNode = xmlDoc.SelectSingleNode(MainNode);
            XmlElement objElem = xmlDoc.CreateElement(Node);
            objElem.SetAttribute(Attrib, AttribValue);
            objElem.InnerText = Content;
            mainNode.AppendChild(objElem);
        }
        #endregion
        #region 在当前节点的插入一个仅带2个属性值的节点
        /// <summary>
        /// 在当前节点的插入一个仅带2个属性值的节点
        /// </summary>
        /// <param name="MainNode">当前节点或路径</param>
        /// <param name="Node">节点名称</param>
        /// <param name="Attrib">属性名称一</param>
        /// <param name="AttribValue">属性值一</param>
        /// <param name="Attrib2">属性名称二</param>
        /// <param name="AttribValue2">属性值二</param>
        public void CreatXmlNode(string MainNode, string Node, string Attrib, string AttribValue, string Attrib2, string AttribValue2)
        {
            XmlNode mainNode = xmlDoc.SelectSingleNode(MainNode);
            XmlElement objElem = xmlDoc.CreateElement(Node);
            objElem.SetAttribute(Attrib, AttribValue);
            objElem.SetAttribute(Attrib2, AttribValue2);
            mainNode.AppendChild(objElem);
        }
        #endregion
        #region 在当前节点插入带两个属性的节点
        /// <summary>
        /// 在当前节点插入带两个属性的节点
        /// </summary>
        /// <param name="MainNode">当前节点或路径</param>
        /// <param name="Node">节点名称</param>
        /// <param name="Attrib">属性名称一</param>
        /// <param name="AttribValue">属性值一</param>
        /// <param name="Attrib2">属性名称二</param>
        /// <param name="AttribValue2">属性值二</param>
        /// <param name="Content">节点值</param>
        public void CreatXmlNode(string MainNode, string Node, string Attrib, string AttribValue, string Attrib2, string AttribValue2, string Content)
        {
            XmlNode mainNode = xmlDoc.SelectSingleNode(MainNode);
            XmlElement objElem = xmlDoc.CreateElement(Node);
            objElem.SetAttribute(Attrib, AttribValue);
            objElem.SetAttribute(Attrib2, AttribValue2);
            objElem.InnerText = Content;
            mainNode.AppendChild(objElem);
        }
        #endregion
        #region 保存Xml
        /// <summary>
        /// 保存Xml
        /// </summary>
        /// <param name="path">保存的当前路径</param>
        public void XmlSave(string path)
        {
            try
            {
                xmlDoc.Save(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #endregion
    }
}
