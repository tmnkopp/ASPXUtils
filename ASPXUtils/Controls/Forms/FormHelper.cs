using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls; 
using System.Reflection;
using System.Web.Compilation;
using System.Collections;
using System.IO;
using ASPXUtils;
using ASPXUtils.Controls;

namespace ASPXUtils
{
    public static class FormHelper
    {

        public static object LoadObjectFromFormValues( object MyObject, Control ControlContainer)
        { 
            PropertyInfo[] typeProperties = MyObject.GetType().GetProperties();

            foreach (PropertyInfo field in typeProperties)
            {
                object oValue = null; 
                foreach (FormfieldAttribute Attr in field.GetCustomAttributes(typeof(FormfieldAttribute), true))
                {

                    if (Attr != null)
                    { 

                        if (field.PropertyType.ToString() == "System.Int32")
                            oValue = ASPXUtils.CommonUtils.GetDataInt(FormHelper.GetValueFromControl(field.Name, ControlContainer));
                        if (field.PropertyType.ToString() == "System.DateTime")
                            oValue = ASPXUtils.CommonUtils.GetDataDateTime(FormHelper.GetValueFromControl(field.Name, ControlContainer));
                        if (field.PropertyType.ToString() == "System.String")
                            oValue = ASPXUtils.CommonUtils.GetDataString(FormHelper.GetValueFromControl(field.Name, ControlContainer));
                        if (field.PropertyType.ToString() == "System.Boolean")
                            oValue = ASPXUtils.CommonUtils.GetDataBoolian(FormHelper.GetValueFromControl(field.Name, ControlContainer));
                        if (field.PropertyType.ToString() == "System.Decimal")
                            oValue = ASPXUtils.CommonUtils.GetDataDeci(FormHelper.GetValueFromControl(field.Name, ControlContainer));
                         
                        //if (field.PropertyType.IsGenericType)
                        //{

                        //    if (typeof(List<>) == field.PropertyType.GetGenericTypeDefinition())
                        //    {

                        //        var objList = Activator.CreateInstance(field.PropertyType);
                        //        var basetype = objList.GetType().GetGenericArguments()[0].FullName;

                        //        Type typeBase = BuildManager.GetType(basetype, true);
                        //        var objInstance = Activator.CreateInstance(typeBase);
                        //        var objListInstance = Activator.CreateInstance(field.PropertyType);

                        //        List<string> values = FormHelper.GetCheckBoxListValues(field.Name, ControlContainer);
                        //        if (values != null)
                        //        {
                        //            foreach (string val in values)
                        //            {
                        //                objInstance = Activator.CreateInstance(typeBase);
                        //                object[] param = { Convert.ToInt32(val) };
                        //                MethodInfo _MethodInfo = objInstance.GetType().GetMethod("Select");
                        //                objInstance = _MethodInfo.Invoke(objInstance, param);
                        //                object[] param1 = { objInstance };
                        //                objListInstance.GetType().GetMethod("Add").Invoke(objListInstance, param1);
                        //            }
                        //            field.SetValue(MyObject, objListInstance, null);

                        //        }
                        //    }
                        //}

                        if (oValue != null)
                        {
                            try
                            {
                                field.SetValue(MyObject, oValue, null);
                            }
                            catch (Exception ex)
                            {
                                //Log.Insert(string.Format("{0} {1}={2}", "field.SetValue fail 114 ", field.Name, oValue.ToString()));
                                //HttpContext.Current.Response.Write(ex.Message);
                                throw;
                            }
                        }  
                    }
                }
            }
            return MyObject;
        }
        public static List<string> GetCheckBoxListValues(string controlName, Control ControlContainer)
        {
            Control c = ControlContainer.FindControl(controlName);
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


        public static object GetValueFromControl(string controlName, Control ParentControl)
        {
            Control c  = ParentControl.FindControl(controlName);
  
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
        public static void LoadFormValuesFromObject(object MyObject, Control ControlContainer)
        {
            if (MyObject == null)
                return;

            PropertyInfo[] typeProperties = MyObject.GetType().GetProperties();

            foreach (PropertyInfo field in typeProperties)
            {
                object oValue = field.GetValue(MyObject, null);
                if (oValue != null)
                {
                    try
                    {
                        if (field.PropertyType.IsGenericType)
                        {
                            if (typeof(List<>) == field.PropertyType.GetGenericTypeDefinition())
                            {
                                FormHelper.SetCheckBoxListValues(field.Name, oValue, ControlContainer);
                            }
                        }
                        else 
                        { 
                            if (field.PropertyType.ToString() == "System.DateTime")
                            {
                                oValue = ((DateTime)oValue).ToShortDateString();
                                if (Convert.ToDateTime(oValue.ToString()) < Convert.ToDateTime("1/1/1001"))
                                    oValue = DateTime.Now.ToShortDateString();
                            }
                            //Log.Insert(string.Format("SetControlValue: {0}  {1}", field.Name, oValue));
                            FormHelper.SetControlValue(field.Name, oValue, ControlContainer); 
                        }
                    }
                    catch (Exception) { 
                        throw; 
                    } 
                }
            }  
        }

        public static void SetCheckBoxListValues(string controlName, object Values, Control ContainerControl)
        {
            Control c = ContainerControl.FindControl(controlName);
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
        public static void SetControlValue(string controlName, object value, Control ContainerControl)
        {
 
            Control c = ContainerControl.FindControl(controlName); 
            if (c != null)
            {
                if (c is DropDownList)
                {
                    ((DropDownList)c).SelectedValue = Convert.ToString(value);
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
                    ((CheckBox)c).Checked = StringUtils.StringToBool(Convert.ToString(value));
                else if (c is ASPXUtils.Controls.ExtendedTextBox)
                    ((ASPXUtils.Controls.ExtendedTextBox)c).Text = Convert.ToString(value);
                else if (c is ASPXUtils.Controls.CKEditor)
                {
                    ((ASPXUtils.Controls.CKEditor)c).Text = Convert.ToString(value);
                }
            }
            else { }
        }


        public static Control FindControlRecursive(Control Root, string Id)
        {

            if (Root.ID == Id)
                return Root;
            foreach (Control Ctl in Root.Controls)
            {
                Control FoundCtl = FindControlRecursive(Ctl, Id);
                if (FoundCtl != null)
                    return FoundCtl;
            }
            return null;
        }



        public static string RequestForm(string sControlID)
        {
            string _return = "";
            string[] _keyParts;
            char _splitChar = '$';
            if (sControlID == "" || sControlID == null)
                return _return;

            foreach (string key in HttpContext.Current.Request.Form.Keys)
            {
                _splitChar = '$';
                if (key.StartsWith("_content_"))
                    _splitChar = '_';

                //_key = key.Split(_splitChar)[key.Split(_splitChar).Length - 1];
                _keyParts = key.Split(_splitChar);
                for (int i = _keyParts.Length; i >= 0; i--)
                {
                    if (_keyParts[i] == sControlID)
                    {
                        _return = HttpContext.Current.Request.Form[key];
                        break;
                    }
                }
            }
            return _return;
        } 
    }

}
