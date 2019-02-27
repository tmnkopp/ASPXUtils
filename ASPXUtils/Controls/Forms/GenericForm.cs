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
using System.ComponentModel;
namespace ASPXUtils.Controls
{
    public class GenericForm : UserControl
    {
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
            get   { return _EntityType; }
            set { _EntityType = value; }
        }
        private int  _ID = 0;
        public int EntityID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public object MyObject { get; set; }

        public Button cmdSave = new Button() { Text = "Save" };
        public Button cmdDelete = new Button() { Text = "Delete", OnClientClick = "  if( !confirm(\"Are you sure you want to delete this entry?\") ){return false;} ;   " };
        public Button cmdNew = new Button() { Text = "New" };

        public Panel ControlHolder = new Panel() { ID = "UpdatePanel", CssClass = "ClassForm panes" };
        ValidationSummary oValidationSummary = new ValidationSummary() { ID = "oValidationSummary", CssClass = "  " };

        BasicTemplateContainer item;
        private ITemplate itemTemplate;
        [Browsable(false), Description("Item template"),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(BasicTemplateContainer))]
        public ITemplate ItemTemplate
        {
            get { return itemTemplate; }
            set { itemTemplate = value; }
        }
        public class BasicTemplateContainer : WebControl, INamingContainer
        {
            public BasicTemplateContainer()
                : base(HtmlTextWriterTag.Span)    {   }
        } 
        public Control GetItemTemplateControl(string name)
        {
            if (this.ControlHolder != null)
                return this.ControlHolder.Controls[0].FindControl(name);
            else
                return null;
        }

        protected virtual void cmdSave_Click(object source, EventArgs e)
        {
            SaveForm();

            string url = HttpContext.Current.Request.RawUrl;
            if (url.IndexOf("id=") < 1)
            {
                if (url.IndexOf("?") < 1)
                    url = url + "?id=" + this._ID.ToString();
                else
                    url = url + "&id=" + this._ID.ToString();
            }
            HttpContext.Current.Response.Redirect(url, true);

        }
        protected virtual void cmdDelete_Click(object source, EventArgs e)
        {
            DeleteForm();
        }
        protected override void OnLoad(EventArgs e)
        {
            cmdSave.Click += new EventHandler(cmdSave_Click);
            cmdDelete.Click += new EventHandler(cmdDelete_Click);
            cmdNew.OnClientClick = "window.location.href='" + HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"] + "?a=nw';  return false;";
   
            Controls.Add(oValidationSummary);
            Controls.Add(ControlHolder);
            ControlHolder.Controls.Add(cmdNew);
            ControlHolder.Controls.Add(cmdSave);  
            if (this._ID > 0) 
                ControlHolder.Controls.Add(cmdDelete);
            
            LoadControls();

            base.OnLoad(e);
        }
        protected void LoadControls()
        {
            if (ItemTemplate != null)
            {
                item = new BasicTemplateContainer();
                ItemTemplate.InstantiateIn(item);
                ControlHolder.Controls.Add(item); 
            }  
        }

        public virtual void SaveForm()
        { 
        }
        public virtual void DeleteForm()
        {  
        }
        public virtual void LoadForm()
        { 
        }
    }
}
