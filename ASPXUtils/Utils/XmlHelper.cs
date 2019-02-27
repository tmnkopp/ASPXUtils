using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ASPXUtils
{

    public static class XmlHelper
    {

        public static string GetAttributeValue(XmlNode oNode, string _attName)
        {
            string _return = "";

            try
            {
                if (oNode != null)
                {
                    if (oNode.Attributes != null)
                    {
                        XmlAttribute _att = oNode.Attributes[_attName];
                        if (_att != null)
                        {
                            _return = _att.Value;
                        }
                    }
                }
            }
            catch (Exception)
            {  }

            return _return;
        }//Get Attribute Value


        public static string GetAttributeValue(string Xml, string _attName)
        {
            string _return = ""; 
            try
            {
                if (Xml != "")
                { 
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.LoadXml(Xml);
                    XmlNode oNode = oXmlDocument.FirstChild;
                    if (oNode != null)
                    {
                        if (oNode.Attributes != null)
                        {
                            XmlAttribute _att = oNode.Attributes[_attName];
                            if (_att != null)
                            {
                                _return = _att.Value;
                            }
                        }
                    } 
                }
            }
            catch (Exception)    { } 
            return _return;
        } 

        public static string GetNodeValue(string _xml, string _node)
        {
            string _return = "";
            if (_xml != "")
            {
                try
                {
                    XmlDocument oDoc = new XmlDocument();
                    oDoc.LoadXml(_xml);
                    if (oDoc != null)
                    {
                        XmlNode oNode = oDoc.SelectSingleNode("//" + _node);
                        if (oNode != null)
                        {
                            _return = oNode.InnerText;
                        }
                    }
                }
                catch (Exception  )
                { }
            }
            return _return;
        }
        public static string GetNodeValue(XmlDocument oDoc, string _node)
        {
            string _return = "";
            if (oDoc != null)
            {
                try
                { 
                    XmlNode oNode = oDoc.SelectSingleNode("//" + _node);
                    if (oNode != null)
                    {
                        _return = oNode.InnerText;
                    }
                } 
                catch (Exception  )
                { }
            }
            return _return;
        }

    }

}