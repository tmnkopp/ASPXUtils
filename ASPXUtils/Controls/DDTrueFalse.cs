using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace ASPXUtils.Controls
{ 
    public class DDTrueFalse : DropDownList{
        protected override void OnInit(System.EventArgs e)
        {
            Items.Clear();
            Items.Add(new ListItem("True", "True"));
            Items.Add(new ListItem("False", "False"));
            base.OnInit(e);
        } 
    } 
}

