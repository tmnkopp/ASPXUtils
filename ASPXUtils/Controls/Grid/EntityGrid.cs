using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASPXUtils;
using System.Web.Compilation;
using System.Reflection;
using System.Collections;
using System.Web;

namespace ASPXUtils.Controls
{
    public  class EntityGrid : Control
    {
        private Type _type;
        public Type type
        {
            get
            {
                if (_type == null)
                    _type = BuildManager.GetType("" + this.EntityType, true);
                return _type;
            }
            set { _type = value; }
        }
        private string _EntityType = "";
        public string EntityType
        {
            get { return _EntityType; }
            set { _EntityType = value; }
        }
        private string _EditString = "<a href='{0}?id={1}' class='grid-edit'>{2}</a>";
        public string EditString
        {
            get { return _EditString; }
            set { _EditString = value; }
        }
        private string _EditCaption= "[EDIT]";
        public string EditCaption
        {
            get { return _EditCaption; }
            set { _EditCaption = value; }
        }

        public string EditUrl { get; set; }
        public bool HideEdit { get; set; }
        public string DataKeyName { get; set; }
        public string CssClass { get; set; }


        private string _SelectMethod = "SelectAll";
        public string SelectMethod
        {
            get { return _SelectMethod; }
            set { _SelectMethod = value; }
        }

        private object[] _parameters = { };
        public object[] SelectParameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public StringBuilder HTML = new StringBuilder(); 

                    
        protected override void OnInit(EventArgs e)
        { 
            base.OnInit(e);
            
        }
        protected override void OnLoad(EventArgs e)
        {

            if(EntityType==""){
                EntityType = this.type.ToString(); 
            } 
            base.OnLoad(e);
            
        }
        public override void DataBind()
        {
            base.DataBind();
            if(this.type==null){
                throw new System.ArgumentException("EntityType cannot be null", "class EntityGrid LoadControl()");
            }
            
            object[] parameters = this.SelectParameters;
            var Instance = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod(SelectMethod);
            IEnumerable items = methodInfo.Invoke(Instance, parameters) as IEnumerable;
            
   

            int cnt = 0;
            int attrcounted = 0;
            PropertyInfo[] fields = type.GetProperties();
            Dictionary<string, IEnumerable<Attribute>> DataFieldAttCache = new Dictionary<string, IEnumerable<Attribute>>();

            HTML.AppendFormat("<table id='{0}Grid-{2}' class='EntityGrid {0}Grid {1} '>", this.type, this.CssClass, this.ClientID);
            HTML.Append("<thead>");
            
            HTML.AppendFormat("\n\t<tr {0}>", "");
            foreach (PropertyInfo field in fields)
            {
                IEnumerable<Attribute> EntityGridAttributes;
                if (!DataFieldAttCache.ContainsKey(field.Name))
                {
                    var attributes = (IEnumerable<Attribute>)field.GetCustomAttributes(typeof(EntityGridAttribute), false);
                    DataFieldAttCache.Add(field.Name, attributes);
                    EntityGridAttributes = DataFieldAttCache[field.Name];
                }
                EntityGridAttributes = (IEnumerable<Attribute>)DataFieldAttCache[field.Name];
                foreach (EntityGridAttribute attr in EntityGridAttributes)
                {
                    if (!attr.IsID)
                    {
                        HTML.AppendFormat("\n\t\t<th class='type-string'>{0}</th>", attr.Caption);
                    }
                    else {
                        HTML.AppendFormat("\n\t\t<th   style='width:1px;' >{0}</th>", "");
                    } 
                } 
            }
            HTML.AppendFormat("\n\t</tr>", "");
            HTML.Append("</thead>");
            foreach (object obj in items)
            {
                HTML.AppendFormat("\n\t<tr {0}>", "");
                foreach (PropertyInfo field in fields)
                {

                    foreach (ASPXUtils.EntityGridAttribute attr in field.GetCustomAttributes(typeof(ASPXUtils.EntityGridAttribute), true))
                    {
                        attrcounted++;
                        HTML.AppendFormat("\n\t\t<td {0}>", "");
                        PropertyInfo propertyInfo = obj.GetType().GetProperty(field.Name);
                        if (attr.IsID)
                        {
                            if(EditUrl==null){
                                EditUrl= HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"];
                            }
                            string id = propertyInfo.GetValue(obj, null).ToString();

                            string editstring = string.Format(EditString, EditUrl, id, EditCaption);
                            if (!HideEdit)
                            {
                                HTML.Append(editstring);
                            }
                            
                        }
                        else
                        {
                            var value = propertyInfo.GetValue(obj, null);
                            if (value != null)
                            {
                                if (DateUtils.IsDate(value))
                                {
                                    try
                                    {
                                        value = value.ToString().Replace("12:00:00 AM", "");
                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                }
                            } 
                            HTML.AppendFormat("{0}", value);
                        }
                        cnt++;
                        HTML.AppendFormat("</td>", "");
                    }
                    
                }
                HTML.AppendFormat("\n\t</tr>", "");
            }
            HTML.AppendFormat("</table>", "");

            if (attrcounted < 1)
            {
                //throw new System.ArgumentException("No EntityGridAttribute Defined " + type, "class EntityGrid LoadControl()");
            }

        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteLine(HTML.ToString());
            base.Render(writer);
        }
    }
}


