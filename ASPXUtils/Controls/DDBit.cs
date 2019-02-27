using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace ASPXUtils.Controls
{ 
    public class DDBit : DropDownList{
        protected override void OnInit(System.EventArgs e)
        {
            Items.Clear();
            Items.Add (new ListItem("True" , "1"));
            Items.Add(new ListItem("False", "0"));
            base.OnInit(e);
        } 
    } 
}

