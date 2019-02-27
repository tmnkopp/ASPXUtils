using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASPXUtils
{
    public static class ImgUtils
    {


        public static string ImagePathToThumb(string _arg)
        {
            string _ret = "";
            if (_arg != "" && _arg != null)
            {
                if (_arg.IndexOf("\\") > 1)
                {
                    _arg = ToVirtual(_arg);
                }
                string _fname = _arg.Split('/')[_arg.Split('/').Length - 1];
                string _path = _arg.Replace(_fname, "");
                _ret = _path + "th/" + _fname;
            }
            return _ret;
        }
        public static string GetThumb(string _path)
        {

            string _fname = _path.Split('/')[_path.Split('/').Length - 1];
            return _path.Replace(_fname, "tn-" + _fname);
        }
        public static string ToVirtual(string _arg)
        {
            int _pos = 0;
            _pos = _arg.IndexOf("\\uploads");
            _arg = _arg.Remove(0, _pos);
            _arg = _arg.Replace("\\", "/");
            return _arg;
        }
    }
}
