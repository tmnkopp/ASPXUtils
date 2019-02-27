using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using ASPXUtils;
using System.Reflection;
using System.Web.Compilation;
using System.Collections;
using System.IO;
namespace ASPXUtils.Controls
{
 
    public abstract class EntityForm : UserControl
    {

        protected abstract void SaveForm();
        protected abstract void LoadForm();
        protected abstract void DeleteForm(); 

        public object _Instance { get; set; }
        public bool AlwaysOpen { get; set; }

        private bool _EnableCopyButton = false;
        public bool EnableCopyButton
        {
            get { return _EnableCopyButton; }
            set { _EnableCopyButton = value; }
        }
        private Type _type;
        public Type type
        {
            get
            {
                if (_type == null)
                    _type = BuildManager.GetType(this.EntityType, true);
                return _type;
            }
            set { _type = value; }
        }
        private string _EntityType = "";
        public string EntityType
        {
            get
            { 
                return _EntityType;
            }
            set { _EntityType = value; }
        }


        private bool _AutoDetectID = false;
        public bool AutoDetectID
        {
            get { return _AutoDetectID; }
            set { _AutoDetectID = value; }
        }

        private int _id = 0;
        public int _ID
        {
            get
            {
                if (AutoDetectID)
                {
                    string id = ASPXUtils.HTTPUtils.QString("id");
                    _id = StringUtils.IsInteger(id) ? Convert.ToInt32(id) : 0;
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        protected int _CID
        {
            get
            {
                string cid = ASPXUtils.HTTPUtils.QString("cid");
                return StringUtils.IsInteger(cid) ? Convert.ToInt32(cid) : 0;
            }
        }
        protected int _CopyFrom
        {
            get
            {
                string id = ASPXUtils.HTTPUtils.QString("cpyfm");
                return StringUtils.IsInteger(id) ? Convert.ToInt32(id) : 0;
            }
        }
        public PropertyInfo[] typeProperties
        {
            get
            {
                return this.type.GetProperties();
            }
        }

        private Button cmdSubmit = new Button() { Text = "Save" };
        private Button cmdCopy = new Button() { Text = "Copy" }; 
        private Button cmdDelete = new Button() { Text = "Delete", OnClientClick = "  if( !confirm(\"Are you sure you want to delete this entry?\") ){return false;} ;   " };
        private Button cmdNew = new Button() { Text = "New" };
        private Button cmdCancel = new Button() { Text = "Cancel" };

        public Panel ControlHolder = new Panel() { ID = "UpdatePanel", CssClass = "ClassForm panes" }; 
        ValidationSummary oValidationSummary = new ValidationSummary() { ID = "oValidationSummary", CssClass = "  " };

  
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
   
            cmdSubmit.Click += new EventHandler(cmdSubmit_Click);
            cmdDelete.Click += new EventHandler(cmdDelete_Click);
            cmdNew.OnClientClick = "window.location.href='" + HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"] + "?a=nw';  return false;";
            cmdCopy.OnClientClick = "window.location.href='" + HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"] + "?a=nw&cpyfm=" + this._ID + "';  return false;";
            cmdCancel.OnClientClick = cmdNew.OnClientClick;//"window.location.href='" + HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"] + "?a=cl&etype=" + this.EntityType + "';return false;";

            Controls.Add(oValidationSummary);
            Controls.Add(ControlHolder); 
            ControlHolder.Controls.Add(cmdNew);
            ControlHolder.Controls.Add(cmdSubmit);
            ControlHolder.Controls.Add(cmdCancel);

            if (this._ID > 0)
            {
                if (EnableCopyButton)
                    ControlHolder.Controls.Add(cmdCopy); 
                ControlHolder.Controls.Add(cmdDelete); 
            } 
            LoadControls();
        } 
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
     
        } 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadForm();
        }
        protected void cmdDelete_Click(object source, EventArgs e)
        {
            DeleteForm();
        }
        protected virtual void cmdSubmit_Click(object source, EventArgs e)
        { 
            object[] parameters = { this._ID };
            this._Instance = Activator.CreateInstance(this.type);

            if (this._ID > 0)
            {
                MethodInfo _MethodInfo = this.type.GetMethod("Select");
                _Instance = _MethodInfo.Invoke(_Instance, parameters);
            }

            SaveForm();
             
            int id = this._ID;
            if (id > 0)
                ((ASPXUtils.IBusinessEntity)_Instance).Update();
            else
                id = ((ASPXUtils.IBusinessEntity)_Instance).Insert();

            string url = HttpContext.Current.Request.RawUrl;
            if (url.IndexOf("id=") < 1)
            {
                if (url.IndexOf("?") < 1)
                    url = url + "?id=" + id.ToString();
                else
                    url = url + "&id=" + id.ToString();
            }
            HttpContext.Current.Response.Redirect(url, true);
        }
        protected virtual void LoadControls()
        { 
            foreach (PropertyInfo field in this.typeProperties)
            {
                foreach (FormfieldAttribute Attr in field.GetCustomAttributes(typeof(FormfieldAttribute), false))
                {
                    if (Attr != null)
                    {             
                        try
                        {
                            ControlHolder.Controls.Add(new Literal()
                            {
                                Text = string.Format("<label class=' {1} '>{0}</label>"
                                , Attr.Caption, " field-" + field.Name.ToLower())
                            });
                        }
                        catch (Exception) { throw; }

                        Control c = LoadControlByType(Attr.ControlType );
                        c.ID = field.Name;
                        try
                        {
                            ControlHolder.Controls.Add(c);
                        }
                        catch (Exception) { throw; }
                    }

                }
            }
            if (AddControls != null)
            {
                foreach (Control c in AddControls.Keys)
                {
                
                    ControlHolder.Controls.Add(c);

                    MethodInfo MO = c.GetType().GetMethod("DataBind");
                    if (MO.ToString() != "")
                    {
                        PropertyInfo PO = c.GetType().GetProperty("DataValueField");
                        PO.SetValue(c, Convert.ToString(this._ID), null); 
                        MO.Invoke(c, null);
                    } 
                }
            }
        }

        public Dictionary<Control, string> AddControls = new Dictionary<Control, string>();
        public void AddControl(Control c, string s)
        {
            AddControls.Add(c, s);
        }
        protected Control LoadControlByType(Type ControlType)
        { 
            var c = Activator.CreateInstance(ControlType) as Control;
            return c;
        }

        protected void SetControlValue(string controlName, object value, Control ContainerControl)
        {
             
            Control c;
            if (ContainerControl != null)
                c = ContainerControl.FindControl(controlName);
            else
                c = ControlHolder.FindControl(controlName);

            if (c != null)
            {
                if (c is DropDownList)
                { 
                    ((DropDownList)c).SelectedValue = Convert.ToString(value);
                    //Log.Insert(string.Format("SetControlValue: {0}  {1}", controlName, value));  
                }
                else if (c is TextBox)
                    ((TextBox)c).Text = Convert.ToString(value);
                else if (c is ExtendedTextBox)
                    ((ASPXUtils.Controls.ExtendedTextBox)c).Text = Convert.ToString(value);
                else if (c is Label)
                    ((Label)c).Text = Convert.ToString(value);
                else if (c is Literal)
                    ((Literal)c).Text = Convert.ToString(value);
                else if (c is HiddenField)
                    ((HiddenField)c).Value = Convert.ToString(value);
                else if (c is CheckBox)
                    ((CheckBox)c).Checked = StringToBool(Convert.ToString(value));
                else if (c is ASPXUtils.Controls.ExtendedTextBox)
                    ((ASPXUtils.Controls.ExtendedTextBox)c).Text = Convert.ToString(value);
                else if (c is ASPXUtils.Controls.CKEditor)
                {
                    ((ASPXUtils.Controls.CKEditor)c).Text = Convert.ToString(value);
                }
            }
            else
                Trace.Write("Exception:", "SetControlValue:Control null  " + controlName);
        }
        protected void SetControlValues(string controlName, object Values, Control ContainerControl)  
        {
            Control c;
            if (ContainerControl != null)
                c = ContainerControl.FindControl(controlName);
            else
                c = ControlHolder.FindControl(controlName);

            if (c != null)
            {
                if (c is CheckBoxList)
                {
                    foreach (object o in (Values as IEnumerable))
                    {
                        ((CheckBoxList)c).Items.FindByValue(((INameValue)o).ID.ToString()).Selected = true;
                    }
                }
            }
        }
        protected List<string> GetValuesFromControl(string controlName)
        {
            Control c = ControlHolder.FindControl(controlName);
            if (c != null)
            {
                if (c is CheckBoxList)
                {
                    return (from item in ((CheckBoxList)c).Items.Cast<ListItem>()
                            where item.Selected
                            select item.Value).ToList();
                }
            }
            return null;
        }
        protected object GetValueFromControl(string controlName)
        {
            return GetValueFromControl(controlName, null);
        }
        protected object GetValueFromControl(string controlName, Control ParentControl)
        {
            Control c;
            if (ParentControl != null)
                c = ParentControl.FindControl(controlName);
            else
                c = ControlHolder.FindControl(controlName);

            if (c != null)
            {
                if (c is ASPXUtils.Controls.DDTrueFalse)
                    return ((ASPXUtils.Controls.DDTrueFalse)c).SelectedValue;
                else if (c is System.Web.UI.WebControls.DropDownList)
                    return ((DropDownList)c).SelectedValue;
                else if (c is ASPXUtils.Controls.CKEditor)
                    return ((ASPXUtils.Controls.CKEditor)c).Text;
                else if (c is ASPXUtils.Controls.ExtendedTextBox)
                    return ((ASPXUtils.Controls.ExtendedTextBox)c).Text;
                else if (c is System.Web.UI.WebControls.TextBox)
                    return ((TextBox)c).Text;
                else if (c is System.Web.UI.WebControls.CheckBox)
                    return ((CheckBox)c).Checked.ToString();
                else if (c is System.Web.UI.WebControls.Label)
                {
                    //Log.Insert(string.Format("{0} {1}={2}", "EntityForm GetValueFromControl   ", controlName, ((Label)c).Text));
                    return ((Label)c).Text;
                }
                else if (c is System.Web.UI.WebControls.HiddenField)
                    return ((HiddenField)c).Value;
                else
                {
                    Log.Insert(string.Format("{0} {1}={2}", "EntityForm GetValueFromControl   ", "Control Not Found", controlName));
                    return "";//" GetValueFromControl:else control not found ";// (string)((TextBox)c).Text;
                }
            }
            else
                return "";//" GetValueFromControl:null";
        }
        protected bool StringToBool(string arg)
        {
            if (arg.ToLower() == "false")
                return false;
            else
                return true;
        }
    }

}
