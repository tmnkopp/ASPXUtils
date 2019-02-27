using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Web;

namespace ASPXUtils
{

    public static class AppSetting
    { 
    
        public static string GetSetting(string sname)
        {
            sname = sname.ToLower();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(GetXml());
            XmlNodeList objNodes = xmlDoc.SelectNodes("//" + sname);
            return (objNodes.Count > 0) ? objNodes[0].InnerText.Replace("]]~", "]]>") : ""; 
        }

        public static void SaveSetting(string sname, string svalue)
        {
            sname = sname.ToLower();
            svalue = svalue.Replace("]]>", "]]~");
            string _ret = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(GetXml());
            XmlNodeList objNodes = xmlDoc.SelectNodes("//" + sname);
            if (objNodes.Count > 0)
            {
                _ret = objNodes[0].InnerXml = "<![CDATA[" + svalue + "]]>";
            }
            else
            {
                objNodes = xmlDoc.SelectNodes("//aspapp_custom");
                XmlElement ele = xmlDoc.CreateElement(sname);
                ele.InnerXml = "<![CDATA[" + svalue + "]]>";
                objNodes[0].AppendChild(ele);
            };
          
            System.Web.HttpContext.Current.Application["aspapp_settings"] = xmlDoc.InnerXml;
        }
        public static void Clear()
        {
            System.Web.HttpContext.Current.Application["aspapp_settings"] = "";
        }
        public static string GetXml()
        {
            string _xml = "";

            if (System.Web.HttpContext.Current.Application != null)
            {
                _xml = (string)System.Web.HttpContext.Current.Application["aspapp_settings"];
            }

            if (_xml == "" || _xml == null)
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement root = xmlDoc.CreateElement("aspapp_settings");
                XmlElement ele_system = xmlDoc.CreateElement("aspapp_system");
                XmlElement ele_custom = xmlDoc.CreateElement("aspapp_custom");

                if (ConfigurationManager.AppSettings["aspapp_settings"] != null)
                {
                    XmlElement ele_system_e;
                    string[] aSysSettings = ConfigurationManager.AppSettings["aspapp_settings"].Split(',');

                    for (int i = 0; i < aSysSettings.Length; i++)
                    {
                        ele_system_e = xmlDoc.CreateElement(aSysSettings[i].Trim());
                        ele_system_e.InnerXml = "<![CDATA[]]>";
                        ele_system.AppendChild(ele_system_e);
                    }
                }
                root.AppendChild(ele_system);
                root.AppendChild(ele_custom);
                xmlDoc.AppendChild(root);
                _xml = xmlDoc.InnerXml;
            }

            return _xml;
        }


    }
    
 


    public static class Settings
    {

        public static string GetSetting(string sname)
        {
            sname = sname.ToLower();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(GetXml());
            XmlNodeList objNodes = xmlDoc.SelectNodes("//" + sname);
            return (objNodes.Count > 0) ? objNodes[0].InnerText.Replace("]]~", "]]>") : ""; 
        }

        public static void SaveSetting(string sname, string svalue)
        {
            sname = sname.ToLower();
            svalue = svalue.Replace("]]>", "]]~");
            string _ret = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(GetXml());
            XmlNodeList objNodes = xmlDoc.SelectNodes("//" + sname);
            if (objNodes.Count > 0)
            {
                _ret = objNodes[0].InnerXml = "<![CDATA[" + svalue + "]]>";
            }
            else
            {
                objNodes = xmlDoc.SelectNodes("//aspses_custom");
                XmlElement ele = xmlDoc.CreateElement(sname);
                ele.InnerXml = "<![CDATA[" + svalue + "]]>";
                objNodes[0].AppendChild(ele);
            };

            System.Web.HttpContext.Current.Session["aspses_settings"] = xmlDoc.InnerXml;
        }
        public static void Clear()
        {
            System.Web.HttpContext.Current.Session["aspses_settings"] = "";
        }

        public static string GetXml()
        {
            string _xml = "";

            if (System.Web.HttpContext.Current.Session != null)
            {
                _xml = (string)System.Web.HttpContext.Current.Session["aspses_settings"];
            }

            if (_xml == "" || _xml == null)
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement root = xmlDoc.CreateElement("aspses_settings");
                XmlElement ele_system = xmlDoc.CreateElement("aspses_system");
                XmlElement ele_custom = xmlDoc.CreateElement("aspses_custom");

                XmlElement ele_system_e = xmlDoc.CreateElement("sessionid");
                XmlText ele_system_t = xmlDoc.CreateTextNode(HttpContext.Current.Session.SessionID);
                ele_system_e.AppendChild(ele_system_t);
                ele_system.AppendChild(ele_system_e);

                if (ConfigurationManager.AppSettings["aspsys_settings"] != null)
                {
                    string[] aSysSettings = ConfigurationManager.AppSettings["aspsys_settings"].Split(',');

                    for (int i = 0; i < aSysSettings.Length; i++)
                    {
                        ele_system_e = xmlDoc.CreateElement(aSysSettings[i].Trim());
                        ele_system_e.InnerXml = "<![CDATA[]]>";
                        ele_system.AppendChild(ele_system_e);
                    }
                }
                root.AppendChild(ele_system);
                root.AppendChild(ele_custom);
                xmlDoc.AppendChild(root);
                _xml = xmlDoc.InnerXml;
            }

            return _xml;
        }


    }
}

