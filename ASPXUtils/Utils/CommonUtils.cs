using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace ASPXUtils
{
 
    /// <summary>
    /// Summary description for DBUtils
    /// </summary> 
        public static class CommonUtils
        { 
            public static string GetDataString(object arg)
            {
                string val = Convert.ToString(arg);
                if (val == null)
                    val = "";
                return val;
            }
            public static int GetDataInt(object arg)
            {
                if (arg.ToString() == "")
                    arg = 0;
                int val = Convert.ToInt32(arg);
                return val;
            }
            public static string GetDataBit(string arg)
            {
                arg = arg.ToUpper();
                if (arg == "" || arg == "FALSE")
                    arg = "0";
                if (arg == "TRUE")
                    arg = "1";
                return  arg;
            }
            public static bool GetDataBoolian(object arg)
            {
                arg = arg.ToString().ToUpper();
                if (arg.ToString() == "1")
                    arg = "TRUE";
                if (arg.ToString() == "0")
                    arg = "FALSE";
                try
                {
                    return Convert.ToBoolean(arg);
                }
                catch (FormatException)
                {
                    return false;
                }
            }
            public static decimal GetDataDeci(string arg)
            {
                if (arg == "")
                    arg = "0";
                decimal value;
                if (Decimal.TryParse(arg, out value)) 
                    value = Convert.ToDecimal( arg ); 
                else 
                    value=0; 
                return Convert.ToDecimal(value);
            }
            public static decimal GetDataDeci(object arg)
            {
                if (arg == null || arg == DBNull.Value)
                    arg = 0;
                decimal value;
                if (Decimal.TryParse(arg.ToString(), out value))
                    value = Convert.ToDecimal(arg);
                else
                    value = 0;
                return value;
            }
            public static DateTime GetDataDT(object arg)
            {
                if (arg == null || arg == DBNull.Value)
                    arg = System.DateTime.Now;
                if (!DateUtils.IsDate(arg))
                    arg = System.DateTime.Now;
                return Convert.ToDateTime(arg);
            }

            public static DateTime GetDataDateTime(object val)
            {
                DateTime arg = GetDataDT(val);
                DateTime rngMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
                DateTime rngMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
                if (arg < rngMin || arg > rngMax)
                    arg = System.DateTime.Now;
                 
                return Convert.ToDateTime(arg);
            } 
         

        

    }    
}
