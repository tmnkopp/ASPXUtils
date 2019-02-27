using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.Caching;
using System.Web;
namespace ASPXUtils.Controls
{
    public static class XmlFormUtils {

        public static string GetNode(string _path, string _settingName) 
        { 
            string ret = "";
            XmlDocument xmlDoc = new XmlDocument(); 
            try
            {
                if (HttpContext.Current.Cache["siteSettings"] == null)
                { 
                    xmlDoc.Load(HttpContext.Current.Server.MapPath(_path));
                    HttpContext.Current.Cache.Insert("siteSettings", xmlDoc, new CacheDependency(HttpContext.Current.Server.MapPath(_path)));
                }
                xmlDoc = (XmlDocument)HttpContext.Current.Cache["siteSettings"];
            } 
            catch (Exception )   {  }

            try
            { 
                 ret = xmlDoc.SelectSingleNode("//item/" + _settingName + "").InnerText;
            } 
            catch (Exception  )   {  } 
            return ret;
        
            }
        }
    [ToolboxData("<{0}:XmlForm runat=server></{0}:XmlForm>")]
    public class XmlForm : CompositeControl
    {
        public string SavePath
        {
            get
            {
                String s = (String)ViewState["SavePath"];
                return ((s == null) ? s : Convert.ToString(s));
            }
            set { ViewState["SavePath"] = Convert.ToString(value); }
        }
        //public bool Multiple { get; set; }
        private bool _Multiple;
        public bool Multiple
        {
            get
            {
                String s = (String)ViewState["Multiple"];
                if (s == null || s == "")
                {
                    return _Multiple;
                }
                else
                {
                    if (s == "True")
                        return true;
                    else
                        return false;
                }
            }
            set
            {
                _Multiple = value;
                if (value)
                    ViewState["Multiple"] = "True";
                else
                    ViewState["Multiple"] = "False";
            }
        }

      
        public string Orient
        {
            get
            {
                String s = (String)ViewState["Orient"];
                return ((s == null) ? s : Convert.ToString(s));
            }
            set { ViewState["Orient"] = Convert.ToString(value); }
        }

        public bool ShowSubmit { get; set; }

        private PlaceHolder pForm = new PlaceHolder();
        private Button cmdSubmit = new Button() { Text = "Save" };


        protected void LoadControls()
        {

            LoadForms();
            cmdSubmit.Click += new EventHandler(cmdSubmit_Click);
            Controls.Add(pForm);
            Controls.Add(cmdSubmit);
            SetStyles();
        }

        protected void cmdSubmit_Click(object source, EventArgs e)
        {
            UpdateForm();
        }
        public void UpdateForm()
        {
            StringBuilder sbXML = new StringBuilder();

            XmlDocument oDoc = new XmlDocument();
            oDoc.Load(SavePath);
            XmlNode nFormDef = oDoc.SelectNodes("//formdef")[0];
            int rows = Convert.ToInt32(ViewState["rows"]);
            bool AddRow = true;
            for (int row = 0; row < rows; row++)
            {
                AddRow = true;
                CheckBox checkbox = (CheckBox)pForm.FindControl("DEL" + Convert.ToString(row));
                if (checkbox != null)
                    AddRow = !checkbox.Checked;

                if (row == rows - 1 && this.Multiple)
                {
                    CheckBox chkAdd = (CheckBox)pForm.FindControl("ADD");
                    AddRow = chkAdd.Checked;
                }
                if (AddRow)
                {
                    sbXML.Append("<item id='" + Guid.NewGuid().ToString() + "'>");
                    foreach (XmlNode nControl in nFormDef.ChildNodes)
                    {
                        Control control = pForm.FindControl(nControl.Name + Convert.ToString(row));
                        sbXML.Append("<" + nControl.Name + "><![CDATA[" + GetControlValue(nControl, control) + "]]></" + nControl.Name + ">");
                    }
                    sbXML.Append("</item>");
                }
            }

            oDoc.InnerXml = "<items><formdef>" + nFormDef.InnerXml + "</formdef>" + sbXML.ToString() + "</items>";
            oDoc.Save(SavePath);
            LoadForms();
        }

        private string GetControlValue(XmlNode nControlDef, Control control)
        {
            string value;
            switch (ASPXUtils.XmlHelper.GetAttributeValue(nControlDef, "type"))
            {
                case "DropDownList":
                    value = ((DropDownList)control).SelectedValue;
                    break;
                case "CheckBox":
                    value = ((CheckBox)control).Checked ? "True" : "False";
                    break; 
                default:
                    value = ((TextBox)control).Text;
                    break;
            }
            return value;
        }
        private void LoadForms()
        {
            pForm.Controls.Clear();
            XmlDocument oDoc = new XmlDocument();
            oDoc.Load(SavePath);
            string value = "";
            XmlNodeList nItems = oDoc.SelectNodes("//item");
            XmlNodeList nFormDef = oDoc.SelectSingleNode("//formdef").ChildNodes;
            int row = 0;

            Control ctrl;


            foreach (XmlNode nItem in nItems)
            {
                pForm.Controls.Add(new Literal() { Text = "<div class='xmlform_row'>" });
                pForm.Controls.Add(new Literal() { Text = "<div class='xmlform_col'>" });
                if(this.Multiple)
                    pForm.Controls.Add(new CheckBox() { ID = "DEL" + Convert.ToString(row) });
                pForm.Controls.Add(new Literal() { Text = "</div>" });
                foreach (XmlNode nControl in nFormDef)
                {
                    value = "";
                    XmlNode nField = nItem.SelectSingleNode(nControl.Name);
                    if (nField != null)
                        value = nField.InnerText.Trim();

                    pForm.Controls.Add(new Literal() { Text = "<div class='xmlform_col'>" });
                    pForm.Controls.Add(new Label() { Text = ASPXUtils.XmlHelper.GetAttributeValue(nControl, "label"), CssClass = "xmlform_label" });
                   
                    ctrl = GetControl(nControl, value, row);

                    pForm.Controls.Add(ctrl);

                    GetValidator(nControl, ctrl.ID);

                    pForm.Controls.Add(new Literal() { Text = "</div>" });
                }

                pForm.Controls.Add(new Literal() { Text = "<div style='clear:both;'></div>" });
                pForm.Controls.Add(new Literal() { Text = "</div>" });
                row++;
            }
            if (this.Multiple)
            {
                pForm.Controls.Add(new Label() { Text = "<br style='clear:both;'>Add<br style='clear:both;'>" });
                pForm.Controls.Add(new CheckBox() { ID = "ADD" });
                foreach (XmlNode nControl in nFormDef)
                {
                    pForm.Controls.Add(new Label() { Text = ASPXUtils.XmlHelper.GetAttributeValue(nControl, "label"), CssClass = "xmlform_label" });
                    pForm.Controls.Add(GetControl(nControl, "", row));
                }
                row++;
            }
            ViewState["rows"] = Convert.ToString(row);
        }


        private void GetValidator(XmlNode nControlDef, string CtrlToValidate)
        {
            string req = ASPXUtils.XmlHelper.GetAttributeValue(nControlDef, "req");
            string validate = ASPXUtils.XmlHelper.GetAttributeValue(nControlDef, "validate");
            string regex = ASPXUtils.XmlHelper.GetAttributeValue(nControlDef, "regex");
            string errmess = ASPXUtils.XmlHelper.GetAttributeValue(nControlDef, "errmess");
            bool add = false;
            if (req != "")
            {
                pForm.Controls.Add(new Literal() { Text = "*" });
                RequiredFieldValidator oRFV = new RequiredFieldValidator();
                oRFV.ControlToValidate = CtrlToValidate;
                oRFV.ErrorMessage = "This is a required field";
                oRFV.ID = "rfv" + CtrlToValidate;
                pForm.Controls.Add(oRFV);
                add = true;
            }
            if (validate.IndexOf("email") >= 0)
            {
                RegularExpressionValidator oREV_Email = new RegularExpressionValidator();
                oREV_Email.ControlToValidate = CtrlToValidate;
                oREV_Email.ID = "rfv_email" + CtrlToValidate;
                oREV_Email.ValidationExpression = @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$";
                oREV_Email.ErrorMessage = "Please enter a valid email address (Example: email@domain.com)";
                if (add)
                    pForm.Controls.Add(new Literal() { Text = "<br>" });
                pForm.Controls.Add(oREV_Email);
            }
            if (regex != "")
            {
                RegularExpressionValidator oREV_Email = new RegularExpressionValidator();
                oREV_Email.ControlToValidate = CtrlToValidate;
                oREV_Email.ID = "rfv_email" + CtrlToValidate;
                oREV_Email.ValidationExpression = regex;
                oREV_Email.ErrorMessage = errmess;
                if (add)
                    pForm.Controls.Add(new Literal() { Text = "<br>" });
                pForm.Controls.Add(oREV_Email);
            }
        }


        private Control GetControl(XmlNode nControlDef, string value, int row)
        {
            string type = ASPXUtils.XmlHelper.GetAttributeValue(nControlDef, "type");
            string width = ASPXUtils.XmlHelper.GetAttributeValue(nControlDef, "width");

            Control control;
            switch (type)
            {
                case "DropDownList":
                    control = new DropDownList();
                    ((DropDownList)control).DataSource = (ASPXUtils.XmlHelper.GetAttributeValue(nControlDef, "listitems")).Split('|');
                    ((DropDownList)control).DataBind();
                    ((DropDownList)control).SelectedValue = value;
                    break;
                case "CheckBox":
                    control = new CheckBox();
                    ((CheckBox)control).Checked = (value == "True" ? true : false);
                    break;
                case "Multiline":
                    control = new TextBox();
                    ((TextBox)control).Text = value;
                    ((TextBox)control).TextMode = TextBoxMode.MultiLine;
                    ((TextBox)control).Height = 50;
                    if (ASPXUtils.StringUtils.IsInteger(width)) 
                        ((TextBox)control).Width = Unit.Pixel(Convert.ToInt32(width));
                    
                    break;

                default:
                    control = new TextBox();
                    ((TextBox)control).Text = value;
                    if (ASPXUtils.StringUtils.IsInteger(width)) 
                        ((TextBox)control).Width = Unit.Pixel(Convert.ToInt32(width));
                
                    break;
            }
            control.ID = nControlDef.Name + Convert.ToString(row);
            return control;
        }

        private void SetStyles()
        {
            string style = "";
            if (this.Orient == "V")
            {
                style = @" 
                <style>
                    .xmlform_col{  padding:0px 4px;width:100%;  }
                    .xmlform_row{  border-bottom:1px dotted #000;  width:100%;  }
                </style> 
            ";
            }
            else
            {
                style = @"   <style>  .xmlform_col{ padding:0px 4px; float:left;   } </style>  ";
            }
            Controls.Add(new Literal() { Text = style });

        }

        override protected void CreateChildControls()
        {
            Controls.Clear();
            LoadControls();
        }
        public override ControlCollection Controls
        {
            get { EnsureChildControls(); return base.Controls; }
        }
        public override void DataBind()
        {
            CreateChildControls();
            ChildControlsCreated = true;
            base.DataBind();
        }
    }
}
