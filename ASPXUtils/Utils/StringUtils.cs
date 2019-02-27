using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ASPXUtils
{

    public static class StringUtils
    {
        public static bool StringToBool(string arg)
        {
            if (arg.ToLower() == "false")
                return false;
            else
                return true;
        }
        public static string Pcase(string stringInput)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool fEmptyBefore = true;
            foreach (char ch in stringInput)
            {
                char chThis = ch;
                if (Char.IsWhiteSpace(chThis))
                    fEmptyBefore = true;
                else
                {
                    if (Char.IsLetter(chThis) && fEmptyBefore)
                        chThis = Char.ToUpper(chThis);
                    else
                        chThis = Char.ToLower(chThis);
                    fEmptyBefore = false;
                }
                sb.Append(chThis);
            }
            return sb.ToString();
        }

        public static bool IsAlphaNumeric(string _arg )
        { 
             Regex regexAlphaNum = new Regex("[^a-zA-Z0-9]");
             return !regexAlphaNum.IsMatch(_arg);  
        }

        public static string ParseCamel(string _arg)
        {
            string _temp = "";
            foreach (char c in _arg)
            {
                if (IsUpper(c))
                {
                    _temp += " " + c;
                }
                else
                {
                    _temp += c;
                }
            }
            return _temp;
        }
        public static bool IsUpper(char ch)
        {
            if (Convert.ToInt32(ch) >= 65 && Convert.ToInt32(ch) <= 90)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsInteger(string theValue)
        {
            if (theValue == "" || theValue == null)
            {
                return false;
            }
            try
            {
                Convert.ToInt32(theValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDecimal(string theValue)
        {
            try
            {
                Convert.ToDouble(theValue);
                return true;
            }
            catch
            {
                return false;
            }
        } //IsDecimal
        public static bool StrContains(string Source, string Find)
        {
            int index = System.Globalization.CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                Source, Find, System.Globalization.CompareOptions.IgnoreCase
            );
            return (index >= 0);
        } 
        
        const string HTML_TAG_PATTERN = @"<(.|\n)*?>";
 
        public static string StripHTML (string inputString)
        {
            if (inputString == null)
                inputString = "";
           return Regex.Replace 
             (inputString, HTML_TAG_PATTERN, string.Empty);
        }

    } 
}
 