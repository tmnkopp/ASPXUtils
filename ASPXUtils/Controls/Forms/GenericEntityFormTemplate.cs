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
    public class GenericEntityFormTemplate : EntityForm
    {
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
                : base(HtmlTextWriterTag.Span)
            {
            }
        }
        override protected void CreateChildControls()
        {
            //Log.Insert(string.Format("{0} {1}={2}", "CreateChildControls " ,"",""));
        } 
        public Control GetItemTemplateControl(string name)
        {
            if (this.ControlHolder != null)
                return this.ControlHolder.Controls[0].FindControl(name);
            else
                return null;
        }
        protected override void LoadControls()
        {
            if (ItemTemplate != null)
            {
                item = new BasicTemplateContainer();
                ItemTemplate.InstantiateIn(item);
                ControlHolder.Controls.Add(item); 
            }  
        }
        protected override void SaveForm()
        { 
            foreach (PropertyInfo field in base.typeProperties)
            { 
                object oValue = null;
               
                foreach (FormfieldAttribute Attr in field.GetCustomAttributes(typeof(FormfieldAttribute), true))
                {
                   
                    if (Attr != null)
                    {
                        Control parent = item;

                        if (field.PropertyType.ToString() == "System.Int32")
                            oValue = ASPXUtils.CommonUtils.GetDataInt(base.GetValueFromControl(field.Name, parent));
                        if (field.PropertyType.ToString() == "System.DateTime")
                            oValue = ASPXUtils.CommonUtils.GetDataDateTime(base.GetValueFromControl(field.Name, parent));
                        if (field.PropertyType.ToString() == "System.String")
                            oValue = ASPXUtils.CommonUtils.GetDataString(base.GetValueFromControl(field.Name, parent));
                        if (field.PropertyType.ToString() == "System.Boolean")
                            oValue = ASPXUtils.CommonUtils.GetDataBoolian(base.GetValueFromControl(field.Name, parent));
                        if (field.PropertyType.ToString() == "System.Decimal")
                            oValue = ASPXUtils.CommonUtils.GetDataDeci(base.GetValueFromControl(field.Name, parent));

                       

                        if(field.PropertyType.IsGenericType){
                             
                            if(typeof(List<>) == field.PropertyType.GetGenericTypeDefinition()){

                                    var objList = Activator.CreateInstance(field.PropertyType);
                                    var basetype = objList.GetType().GetGenericArguments()[0].FullName;

                                    Type typeBase = BuildManager.GetType(basetype, true);
                                    var objInstance = Activator.CreateInstance(typeBase);
                                    var objListInstance = Activator.CreateInstance(field.PropertyType);

                                    List<string> values = base.GetValuesFromControl(field.Name);
                                    if (values != null)
                                    {
                                        foreach (string val in values)
                                        {
                                            objInstance = Activator.CreateInstance(typeBase);
                                            object[] param = { Convert.ToInt32(val) };
                                            MethodInfo _MethodInfo = objInstance.GetType().GetMethod("Select");
                                            objInstance = _MethodInfo.Invoke(objInstance, param);
                                            object[] param1 = { objInstance };
                                            objListInstance.GetType().GetMethod("Add").Invoke(objListInstance, param1);
                                        }
                                        field.SetValue(_Instance, objListInstance, null);

                                    }
                             }
                        }
                         
                        if (oValue != null)
                        {
                            try
                            { 
                                field.SetValue(_Instance, oValue, null);
                            }
                            catch (Exception ex)
                            {
                                Log.Insert(string.Format("{0} {1}={2}", "field.SetValue fail 114 ", field.Name, oValue.ToString()));
                                //HttpContext.Current.Response.Write(ex.Message);
                                throw;
                            }
                        }
                        else 
                        {
                            Log.Insert(string.Format("field{0} item{1} {2}", "oValue null GenericEntityForm   ", field.Name, parent, parent));
                        }
                    }
                }
            }  
        }
        protected override void DeleteForm()
        {
            object[] parameters = { base._ID };
            var Instance = Activator.CreateInstance(base.type);
            if (base._ID > 0)
            {
                MethodInfo _MethodInfo = base.type.GetMethod("Select");
                Instance = _MethodInfo.Invoke(Instance, parameters);
                ((ASPXUtils.IBusinessEntity)Instance).Delete();
            }
            HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, true);
        }

        protected override void LoadForm()
        {
             
            if (_Instance == null)
            {
                int LoadID = base._ID;
                if (base._CopyFrom > 0 && base._ID < 1)
                    LoadID = base._CopyFrom;

                object[] parameters = { LoadID };
                _Instance = Activator.CreateInstance(base.type);
                if (LoadID > 0)
                {
                    MethodInfo _MethodInfo = base.type.GetMethod("Select");
                    _Instance = _MethodInfo.Invoke(_Instance, parameters);
                }
            }
             

            if (_Instance == null)
                return;

            foreach (PropertyInfo field in base.typeProperties)
            {
                object oValue = field.GetValue(_Instance, null);
                if (oValue != null)
                {
                    if (field.PropertyType.IsGenericType)
                    {
                        if (typeof(List<>) == field.PropertyType.GetGenericTypeDefinition())
                        {
                            base.SetControlValues(field.Name, oValue, item);
                        } 
                    }
                    try
                    {
                        if (field.PropertyType.ToString() == "System.DateTime")
                        {
                            oValue = ((DateTime)oValue).ToShortDateString();
                            if (Convert.ToDateTime(oValue.ToString()) < Convert.ToDateTime("1/1/1001")) 
                                oValue = DateTime.Now.ToShortDateString(); 
                        }
                        //Log.Insert(string.Format("SetControlValue: {0}  {1}", field.Name, oValue));
                        base.SetControlValue(field.Name, oValue, item);
                    }
                    catch (Exception) { throw; }
                }
            } 
        }
    }
}
