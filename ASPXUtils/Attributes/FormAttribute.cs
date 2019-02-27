using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using ASPXUtils;

namespace ASPXUtils
{
 
    public class FormAttribute : Attribute
    {
        public FormAttribute(string s)
        {
            this.Caption = s;
        } 
        protected string caption;
        public string Caption
        {
            get { return this.caption; }
            set { this.caption = value; }
        }

    }
    public class FormfieldAttribute : Attribute
    {
        public FormfieldAttribute(string s)
        {
            this.Caption = s; 
        }
        protected string caption;
        public string Caption
        {
            get { return this.caption; }
            set { this.caption = value; }
        }   
        protected Type _controlType = typeof(ASPXUtils.Controls.ExtendedTextBox);
        public Type ControlType
        {
            get { return this._controlType ; }
            set { this._controlType  = value; }
        }
        protected string _controlID = "";
        public string ControlID  
        {
            get { return this._controlID; }
            set { this._controlID = value; }
        }   
    }
}
