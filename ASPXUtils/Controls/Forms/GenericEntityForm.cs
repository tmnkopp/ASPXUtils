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
namespace ASPXUtils.Controls
{
    public class GenericEntityForm : EntityForm
    {
        protected override void LoadControls()
        {
            foreach (PropertyInfo field in this.typeProperties)
            {
                foreach (FormfieldAttribute Attr in field.GetCustomAttributes(typeof(FormfieldAttribute), false))
                {
                    if (Attr != null)
                    {
                        string cssClasses = "";
                        if (Attr.Caption == "")
                            Attr.Caption = field.Name;

                        try   {
                            ControlHolder.Controls.Add(new Literal()
                            {
                                Text = string.Format("<label class=' {1} {2}'>{0}</label>"
                                , Attr.Caption, Attr.CssClass, " field-" + field.Name.ToLower())
                            });
                        }   catch (Exception) { throw; }

                        Control c = LoadControlByType(Attr, cssClasses);
                        c.ID = field.Name;
                        try   {
                            ControlHolder.Controls.Add(c);
                        }   catch (Exception) { throw; }
                    } 
                }
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
                        Control parent = null;
                        if (Attr.Tab != "" && Attr.Tab != null)
                            parent = ControlHolder.FindControl(Attr.Tab);

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
                                //Log.Insert(string.Format("{0} {1}={2}", "field.SetValue GenericEntityForm 76 ", field.Name, oValue.ToString()));
                                field.SetValue(_Instance, oValue, null);
                            }
                            catch (Exception ex)
                            { 
                                //HttpContext.Current.Response.Write(ex.Message);
                                throw;
                            }
                        }
                        else 
                        {
                            //Log.Insert(string.Format("{0} {1}={2}", "oValue null line GenericEntityForm 82 ", field.Name, oValue.ToString()));
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
                            base.SetControlValues(field.Name, oValue, null);
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
                        base.SetControlValue(field.Name, oValue, null);
                    }
                    catch (Exception) { throw; }
                }
            } 
        }
    }
}
