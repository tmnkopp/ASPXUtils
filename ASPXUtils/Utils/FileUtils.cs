using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ASPXUtils
{
    public static class FileUtils
    {

        private static readonly string[] _imgExtensions = { ".jpg", ".jpeg", ".bmp", ".gif", ".png" }; //  etc
       
        public static bool IsImageExtension(string FileName)
        {
            foreach(string item in _imgExtensions){
                if (FileName.ToLower().EndsWith(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static string ToVirtual(string FullPath, string Base)
        {
            int _pos = 0;
            FullPath = FullPath.ToLower();
            Base = Base.ToLower();
            _pos = FullPath.IndexOf("\\" + Base);
            FullPath = FullPath.Remove(0, _pos);
            FullPath = FullPath.Replace("\\", "/");
            return FullPath;
        }
        public static string ToPhysical(string FullPath )
        {
            string _ret = FullPath;
            if (FullPath.IndexOf("/") > 0)
                FullPath = HttpContext.Current.Server.MapPath(FullPath);

            return FullPath;
        }
        public static bool isValidFolderName(string inputStr)
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(inputStr))
                isValid = false;
            if (!(char.IsLetter(inputStr[0])) && (!(char.IsNumber(inputStr[0]))))
                isValid = false;
            for (int i = 0; i < inputStr.Length; i++)
            {
                if (!(char.IsLetter(inputStr[i])) && (!(char.IsNumber(inputStr[i]))) && (inputStr[i] != '_' && (inputStr[i] != ' ')))
                    isValid = false;
            }
            return isValid;

        }
        public static bool isValidPageName(string inputStr)
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(inputStr))
                isValid = false;
            if (!(char.IsLetter(inputStr[0])) && (!(char.IsNumber(inputStr[0]))))
                isValid = false;
            for (int i = 0; i < inputStr.Length; i++)
            {
                if (!(char.IsLetter(inputStr[i])) && (!(char.IsNumber(inputStr[i]))) && (inputStr[i] != '-' && (inputStr[i] != '_')))
                    isValid = false;
            }
            return isValid;

        }

        public static string PathToSettingName(string _path)
        {
            _path = _path.ToLower().Replace("~/", "");
            _path = _path.Replace("/", "_");
            _path = _path.Split('.')[0];
            return _path;
        }
    }
}
