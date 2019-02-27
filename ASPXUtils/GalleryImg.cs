using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASPXUtils
{
    public class GalleryImg
    {

        public GalleryImg()
        {
        }
        public string ImageSRC { get; set; }
        public string Caption { get; set; }
        public string ThumbSRC { get; set; }
        public string Header { get; set; }
        private string _NavigateURL = "";
        public string NavigateURL
        {
            get
            {
                if (_NavigateURL == "")
                {
                    _NavigateURL = "#";
                }
                return _NavigateURL;

            }
            set { _NavigateURL = value; }
        }


    }
}
