using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using ASPXUtils;
using ASPXUtils.Controls;
using ASPXUtils.Data;

namespace ASPXUtils
{
    public enum LogTypes
    {
        INFO,
        WARNING,
        ERROR
    }


    //[ASPXUtils.EntityAttribute("Log", TableName = "wutil_Log")]
    [ASPXUtils.DataEntityAttribute(TableName = "wutil_Log" )]
    public class Log  
    {
        public int ID { get; set; }
        public int LogTypeID { get; set; }
        public int Severity { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string IPAddress { get; set; }
        public int UserID { get; set; }
        public string PageURL { get; set; }
        public string ReferrerURL { get; set; }
        public DateTime CreatedOn { get; set; } 

        private List<Log> _Collection;
        public List<Log> Collection
        {
            get { return _Collection; }
            set { _Collection = value; }
        }
        public static void Insert(string Message)
        { 
            Log log = new Log() { Message = Message };
            log.IPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            log.PageURL = HttpContext.Current.Request.ServerVariables["PATH_INFO"];
            log.ReferrerURL = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
            log.Insert(log); 
        }
        public static void Insert(string Message, Exception ex)
        {
            Log log = new Log() { Message = Message };
            log.IPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            log.PageURL = HttpContext.Current.Request.ServerVariables["PATH_INFO"];
            log.ReferrerURL = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
            log.Exception = string.Format("{0} {1} {2}", ex.Message, ex.Source, ex.InnerException);// ex.Message; 
            log.Insert(log);
        }
        public int Insert(Log obj)
        {
            return LogDAL.Add(obj);
        }  
        public Log Select(int ID)
        {
            List<Log> oResult = ((from o in SelectAll() where o.ID == ID select o).Take(1)).ToList();
            return (oResult.Count > 0) ? oResult[0] : null;
        } 
        public List<Log> SelectAll()
        {
            if (this.Collection == null)
                this.Collection  = new LogDAL().GetAll();
            return this.Collection;
        }
    }

}
