using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ASPXUtils
{
    public static class HTTPUtils
    {
        public static string GetPageName()
        {
            string _fname;
            _fname = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"].ToLower();
            _fname = _fname.Replace("/", "");
            _fname = _fname.Split('.')[0];
            return _fname;
        }
        public static string GetDomain() {
            return HttpContext.Current.Request.ServerVariables["HTTP_HOST"].ToLower();
        }
        public static string GetPageNameWithExtention()
        {
            string _fname;
            _fname = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"].ToLower(); 
             return  _fname.Split('/')[_fname.Split('/').Length-1]; 
        }
        public static string QString(string _key)
        {
            return QueryString( _key);
        }
        public static int QueryStringInt(string name, int defaultValue)
        {
            string resultStr = QueryString(name).ToUpperInvariant();
            if (resultStr.Length > 0)
            {
                return Int32.Parse(resultStr);
            }
            return defaultValue;
        }
        public static int QueryStringInt(string name)
        {
            string resultStr = QueryString(name).ToUpperInvariant();
            int result;
            Int32.TryParse(resultStr, out result);
            return result;
        }
        public static bool QueryStringBool(string name)
        {
            string resultStr = QueryString(name).ToUpperInvariant();
            return (resultStr == "YES" || resultStr == "TRUE" || resultStr == "1");
        }
        public static string QueryString(string name)
        {
            string result = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request.QueryString[name] != null)
                result = HttpContext.Current.Request.QueryString[name].ToString();
            return result;
        }
        public static string SCRIPT_NAME()
        { 
            return HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"].ToLower(); 
        }
        public static string GetCurrentFolder()
        {
            string _sn =  SCRIPT_NAME();
            _sn = _sn.Split('/')[_sn.Split('/').Length - 2];
            _sn = StringUtils.Pcase(_sn); 
            return _sn;
        }
        public static string GetCurrentPath(){
            string PATH_INFO = HttpContext.Current.Request.ServerVariables["PATH_INFO"].ToLower();
            if (PATH_INFO != "") 
                PATH_INFO = PATH_INFO.Substring(0, PATH_INFO.LastIndexOf(@"/")) + "/";
           
            if (PATH_INFO == "")
                PATH_INFO = "/";
            return PATH_INFO;
        }
        public static string GetRootFolder()
        {
            string _sn = SCRIPT_NAME();
            try {
                _sn = _sn.Split('/')[1];
            }catch(Exception ){}
            
            _sn = StringUtils.Pcase(_sn);
            return _sn;
        }

        public static string ToDirFriendly(string _arg)
        {
            string _ret = "";
            foreach (char c in ",()+&/?'")
            {
                _arg = _arg.Replace(" " + c + " ", " ");
                _arg = _arg.Replace(c + " ", " ");
                _arg = _arg.Replace(" " + c, " ");
            }
            _arg = _arg.Replace(" ", "-");
            foreach (char c in _arg)
            {
                if (StringUtils.IsAlphaNumeric(c.ToString()) || c == '-' || c == '_')
                {
                    _ret += c.ToString();
                }
            }
            return _ret;
        }
    }
}
