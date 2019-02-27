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
    [ToolboxData("<{0}:Textarea runat=server></{0}:Textarea>")]
    public class TextArea : ExtendedTextBox
    {        
        protected override void OnInit(EventArgs e)
        {
            this.TextMode = TextBoxMode.MultiLine;
            base.OnInit(e);
        }
    
    }
}
