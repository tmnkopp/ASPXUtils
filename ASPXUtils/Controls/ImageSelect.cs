using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASPXUtils;
namespace ASPXUtils.Controls
{

    [ToolboxData("<{0}:ImageSelect runat=server></{0}:ImageSelect>")]
    public class ImageSelect : HiddenField
    {
        #region Properties  
        public string DisplayImgID
        {
            get
            {
                return String.Format("{0}-{1}", this.ClientID, "DisplayImg");
            }
        }
        #endregion   
 
  
        protected override void Render(HtmlTextWriter writer)
        {
 
            if (this.Value.Trim() == "" || this.Value == null || this.Value == String.Empty)
                this.Value = "/images/placeholder.png";

            writer.WriteLine(String.Format("<div class='imageselect {1}'><img src=\"{0}\" id=\"{1}\" />", this.Value, DisplayImgID));
            writer.WriteLine("<br><span class='imageselect-update'>Update</span>"); 
            writer.WriteLine("</div>");
            writer.WriteLine(String.Format("<span class='imageselect-remove' onclick=\"$('#{0}').val('');$('#{1}').attr( 'src', '');\">Remove</span>", this.ClientID, DisplayImgID));
            StartUp();
            base.Render(writer);
        }

        private void StartUp()
        {
            string js = String.Format(@"
            <script>  
                    $('.{0}').bind('click', function() {{ 
                         openwin('/Admin/Browse.aspx?p=site&t={1}');
                    }});  
                    function CTRL{1}_browseComplete( args ) {{   
                          $('#{1}').val(args);
                          $('#{0}').attr( 'src', args )   ;
                    }}; 
            </script>
            ", DisplayImgID, this.ClientID);
            Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID, js);
        }
    }
}
