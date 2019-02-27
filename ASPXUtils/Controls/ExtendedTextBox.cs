using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
namespace ASPXUtils.Controls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ExtendedTextBox runat=server></{0}:ExtendedTextBox>")]
    public class ExtendedTextBox : TextBox
    {

        private RequiredFieldValidator oRFV; 
        private RegularExpressionValidator oRegExVal;

       
         
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            } 
            set
            {  ViewState["Text"] = value;   }
        }

        public string Label { get; set; }
        private bool _required = false;
        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }

        private bool _validateEmail = false;
        public bool  ValidateEmail
        {
            get { return _validateEmail; }
            set { _validateEmail = value; }
        }
        private bool _validateAlphaNumeric = false;
        public bool ValidateAlphaNumeric
        {
            get { return _validateAlphaNumeric; }
            set { _validateAlphaNumeric = value; }
        }

        private bool _validateDate = false;
        public bool ValidateDate
        {
            get { return _validateDate; }
            set { _validateDate = value; }
        }

        private bool _validateDecimalNumber = false;
        public bool ValidateDecimalNumber
        {
            get { return _validateDecimalNumber; }
            set { _validateDecimalNumber = value; }
        }

        private bool _validateInteger = false;
        public bool ValidateInteger
        {
            get { return _validateInteger; }
            set { _validateInteger = value; }
        }

        private string _ValidateRegEx = "";
        public string ValidateRegEx
        {
            get { return _ValidateRegEx; }
            set { _ValidateRegEx = value; }
        }
 
        private bool _showCalendar = false;
        public bool ShowCalendar
        {
            get { return _showCalendar; }
            set { _showCalendar = value; }
        }
        private string _MessageDisplay = "";
        public string MessageDisplay
        {
            get { return _MessageDisplay; }
            set { _MessageDisplay = value; }
        }
        private string _ErrorMessage = "";
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }
        private string _FriendlyName = "";
        public string FriendlyName
        {
            get {    return _FriendlyName;   }
            set { _FriendlyName = value; }
        }
        private string _Title = "";
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        } 
        protected override void OnInit(EventArgs e)
        {
            if (this.Required)
            {
                string _name = FriendlyName;
                if (_name == "")
                    _name = "This";
                oRFV = new RequiredFieldValidator();
                oRFV.ControlToValidate = this.ID;
                oRFV.ErrorMessage = _name + " is a required field";
                oRFV.ID = "rfv" + this.ID;
                oRFV.CssClass = "errormsg";
                Controls.Add(oRFV); 
            }
            //if (MaxLength >0) {
            //    oRegExVal = new RegularExpressionValidator();
            //    oRegExVal.ControlToValidate = this.ID;
            //    oRegExVal.ID = "rfv_maxlen" + this.ID;
            //    oRegExVal.ValidationExpression = @"^[\s\S]{0," + MaxLength.ToString() + "}$";
            //    oRegExVal.ErrorMessage = "Character Limit " + MaxLength.ToString();
            //    if (ErrorMessage != "")
            //        oRegExVal.ErrorMessage = ErrorMessage;
            //    oRegExVal.CssClass = "errormsg";
            //    Controls.Add(oRegExVal);
            //}
            if (this.ValidateEmail)
            {
                oRegExVal = new RegularExpressionValidator();
                oRegExVal.ControlToValidate = this.ID;
                oRegExVal.ID = "rfv_email" + this.ID;
                oRegExVal.ValidationExpression = @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$";
                oRegExVal.ErrorMessage = "Please enter a valid email (Example: email@domain.com)";
                if (ErrorMessage != "")
                    oRegExVal.ErrorMessage = ErrorMessage;
                oRegExVal.CssClass = "errormsg";
                Controls.Add(oRegExVal);
            }


            if (this.ValidateDate)
            {
                oRegExVal = new RegularExpressionValidator();
                oRegExVal.ControlToValidate = this.ID;
                oRegExVal.ID = "rfv_Date" + this.ID;
                oRegExVal.ValidationExpression = @"((^(10|12|0?[13578])([/])(3[01]|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(11|0?[469])([/])(30|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(2[0-8]|1[0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(29)([/])([2468][048]00)$)|(^(0?2)([/])(29)([/])([3579][26]00)$)|(^(0?2)([/])(29)([/])([1][89][0][48])$)|(^(0?2)([/])(29)([/])([2-9][0-9][0][48])$)|(^(0?2)([/])(29)([/])([1][89][2468][048])$)|(^(0?2)([/])(29)([/])([2-9][0-9][2468][048])$)|(^(0?2)([/])(29)([/])([1][89][13579][26])$)|(^(0?2)([/])(29)([/])([2-9][0-9][13579][26])$))";
                oRegExVal.ErrorMessage = "Date Required (MM/DD/YYYY)";
                if (ErrorMessage != "")
                    oRegExVal.ErrorMessage = ErrorMessage;
                oRegExVal.CssClass = "errormsg";
                Controls.Add(oRegExVal);

            }
            if (this.ValidateDecimalNumber)
            {
                oRegExVal = new RegularExpressionValidator();
                oRegExVal.ControlToValidate = this.ID;
                oRegExVal.ID = "rfv_DecimalNumber" + this.ID;
                oRegExVal.CssClass = "errormsg";
                oRegExVal.ValidationExpression = @"^\d*[0-9](\.\d*[0-9])?$";
                oRegExVal.ErrorMessage = "Numeric Required (ex. 123.00)";
                if (ErrorMessage != "")
                    oRegExVal.ErrorMessage = ErrorMessage;
                Controls.Add(oRegExVal);

            }
            if (this.ValidateInteger)
            {
                oRegExVal = new RegularExpressionValidator();
                oRegExVal.ControlToValidate = this.ID;
                oRegExVal.ID = "rfv_int" + this.ID;
                oRegExVal.CssClass = "errormsg";
                oRegExVal.ValidationExpression = @"^0|[0-9][0-9]*$";
                oRegExVal.ErrorMessage = "Numeric Required";
                if (ErrorMessage != "")
                    oRegExVal.ErrorMessage = ErrorMessage;
                Controls.Add(oRegExVal);

            } 
            if (this.ValidateAlphaNumeric)
            {
                oRegExVal = new RegularExpressionValidator();
                oRegExVal.ControlToValidate = this.ID;
                oRegExVal.ID = "rfv_anum" + this.ID;
                oRegExVal.CssClass = "errormsg";
                oRegExVal.ValidationExpression = @"^\w+$";
                oRegExVal.ErrorMessage = "Alphanumeric Required (ex. Abc123)";
                if (ErrorMessage != "")
                    oRegExVal.ErrorMessage = ErrorMessage;
                Controls.Add(oRegExVal);

            }
            if (this.ValidateRegEx != "" && this.ValidateRegEx != null)
            { 
                oRegExVal = new RegularExpressionValidator();
                oRegExVal.ControlToValidate = this.ID;
                oRegExVal.ID = "rfv_custregex" + this.ID;
                oRegExVal.CssClass = "errormsg";
                oRegExVal.ValidationExpression = this.ValidateRegEx;
                oRegExVal.ErrorMessage = this.ErrorMessage;
                Controls.Add(oRegExVal);

            }
             
            if(this.ShowCalendar)
            {

                Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
                    "jquery.tools.min",
                    "<script type='text/javascript' src='http://cdn.jquerytools.org/1.2.5/full/jquery.tools.min.js'></script>",
                    false);
                string script = String.Format(@"
                                     $(function() {{  $( '#{0}' ).dateinput({{format: 'mm/dd/yyyy'}});
                	                 }});    ", this.ClientID);
                Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID + "datepicker", script, true); 


            }
            if (this.Title != "")
            {
                base.Attributes.Add("title", this.Title);
            }

        }
        protected override void OnLoad(EventArgs e)
        { 
            base.OnLoad(e); 
        }
        protected override void Render( HtmlTextWriter writer) 
        {
            if(this.ShowCalendar || this.ValidateDate)
            {
                try
                {
                    this.Text = Convert.ToDateTime(this.Text).ToShortDateString();
                    this.CssClass = " date-input ";
                }   catch (Exception )  {    }
            }
            if(Label!=null)
                writer.WriteLine(string.Format("<label class=' {1} '>{0}</label>", Label, Label.ToLower()));
            base.Render(writer);
            if (Label != null)
                writer.WriteLine("<br>");
            if (oRFV != null)
            { 
                if (this.ValidationGroup != "")
                {
                    oRFV.ValidationGroup = this.ValidationGroup;
                }
                writer.Write(" <span class=\"required \" style=\"float:top;\">*</span>");
                oRFV.RenderControl(writer);                
            }

            if (oRegExVal != null)
            {
                if (this.ValidationGroup != "")
                {
                    oRegExVal.ValidationGroup = this.ValidationGroup;
                }
                oRegExVal.RenderControl(writer);
          
            }    
        }  
    }
}
