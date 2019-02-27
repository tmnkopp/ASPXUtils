using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ASPXUtils
{
    public static class DateUtils
    {
        public static bool IsDate(Object obj)
        {
            string strDate;
            try
            {
                strDate = obj.ToString();
                if (strDate.Length < 6)
                {
                    return false;
                }
                DateTime dt = DateTime.Parse(strDate);
                if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                    return true;
                return false;
            }
            catch
            { 
                return false;
            }
        }
        public static string GetDOW(int _i)
        {
            switch (_i)
            {
                case 0:
                    return "Sunday";  
                case 1:
                    return "Monday";  
                case 2:
                    return "Tuesday";  
                case 3:
                    return "Wednesday";  
                case 4:
                    return "Thursday"; 
                case 5:
                    return "Friday";  
                case 6:
                    return "Saturday";  
                default:
                    return "";  
            }
        }
        public static string ParseHour(string _time)
        {
            if (_time != "")
            {
                try
                {
                    return _time.Split(':')[0];
                }
                catch (Exception)
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public static string ParseMinute(string _time)
        {
            if (_time != "")
            {
                try
                {
                    return _time.Split(':')[1];
                }
                catch (Exception)
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public static string ParseDate(string _longdate)
        {
            return DateParse(_longdate, 0);
        }
        public static string ParseTime(string _longdate)
        {
            string _ret = DateParse(_longdate, 1);
            return ParseHour(_ret) + ":" + ParseMinute(_ret);
        }
        public static string ParseMeridiem(string _longdate)
        {
            return DateParse(_longdate, 2);
        }

        private static string DateParse(string _longdate, int _which)
        {
            if (_longdate != "")
            {
                try  {
                    return _longdate.Split(' ')[_which];
                }
                catch (Exception)  { 
                    return "";
                }
            }
            else  {
                return "";
            }
        }
       
    }
}
