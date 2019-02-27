using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using ASPXUtils;

namespace ASPXUtils
{ 
    public class EntityGridAttribute : Attribute
    {
        public EntityGridAttribute(string s)
        {
            this.Caption = s;
        }
        protected string caption;
        public string Caption
        {
            get { return this.caption; }
            set { this.caption = value; }
        }
        private bool _isID = false;
        public bool IsID
        {
            get { return _isID; }
            set { _isID = value; }
        }
    }
     
}
