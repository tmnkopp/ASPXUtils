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


    [DefaultProperty("Text")]
    [ToolboxData("<{0}:CKEditor runat=server></{0}:CKEditor>")]
    public class CKEditor : TextBox
    {
        #region Properties
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public override string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }
            set { ViewState["Text"] = value; }
        }

        public bool UseSource { get; set; }
        public bool BlockEditorCss { get; set; }
        

        private string filebrowserBrowseUrl = "/Admin/Browse.aspx";
        public string FileBrowserBrowseUrl
        {
            get { return filebrowserBrowseUrl; }
            set { filebrowserBrowseUrl = value; }
        }

        public string BackgroundStyle { get; set; }
        private string contentsCss = "/css/StyleSheet.css";
        public string ContentsCss
        {
            get { return contentsCss; }
            set { contentsCss = value; }
        }
         
        private string toolbar = "";
        public string Toolbar
        {
            get { return toolbar; }
            set { toolbar = value; }
        }
        private string plugins = "";
        public string Plugins
        {
            get { return plugins; }
            set { plugins = value; }
        }
        private string _SourceFolder = "ckeditor";
        public string SourceFolder
        {
            get { return _SourceFolder; }
            set { _SourceFolder = value; }
        } 
        public bool UseInlineEditor {get;set;}

        public string InlineID { get; set; }
      
        
        #endregion
        //
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.FileBrowserBrowseUrl += "?t=" + this.ClientID; 
            this.CssClass = "ckeditor";
            this.TextMode = TextBoxMode.MultiLine;
            if (InlineID == null)
                InlineID = "ckeditor_" + this.ClientID;

        }    
        protected override void OnPreRender(EventArgs e)
        {
             
            string ckeditor = "<script type='text/javascript' src='/js/" + SourceFolder  + "/ckeditor.js'></script>";

            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "ckeditor",
                ckeditor
                );
        }
        protected override void Render(HtmlTextWriter writer)
        {
            //writer.WriteLine(ckeditor);
            //ToDo: Client Script Not registering
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ckeditor-ckeditor", ckeditor); 


            RenderConfigScript();
            RenderBrowseComplete();


            if (UseInlineEditor)
            {
                writer.WriteLine("<div id='" + InlineID + "' class=' contenteditable ' contenteditable='true'>");
                writer.WriteLine(this.Text);
                writer.WriteLine("</div>");

            }   else {
                writer.WriteLine("<div class='ckedit-wrapper'>");
                base.Render(writer);
                writer.WriteLine("</div>");
            }
           
 

        }

        public virtual void RenderBrowseComplete()
        {
            StringBuilder SB = new StringBuilder();
            SB.AppendFormat(@"
            function CTRL{0}_browseComplete(obj) {{
                 var txtLinkUrl = CKEDITOR.dialog.getCurrent().getContentElement('info', 'url');
                 if (txtLinkUrl) {{
                     CKEDITOR.dialog.getCurrent().setValueOf('info', 'url', obj);
                 }}
                 var txtImgUrl = CKEDITOR.dialog.getCurrent().getContentElement('info', 'txtUrl');
                 if (txtImgUrl) {{
                     CKEDITOR.dialog.getCurrent().setValueOf('info', 'txtUrl', obj);
                 }} 
             }}
            ", this.ClientID);

            Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID + "ckeditor-BrowseComplete", SB.ToString(), true);

        }
        public virtual void RenderConfigScript()
        {

            if (this.Height == 0 || this.Height == Unit.Empty || this.Height == null)
                this.Height = 700;
            if (this.Width == 0 || this.Width == Unit.Empty || this.Width == null)
                this.Width = 700;

            StringBuilder SB = new StringBuilder();

            if(UseInlineEditor){
                SB.AppendFormat(@"
                  
                     CKEDITOR.on('instanceCreated', function (event) {{
                        var editor = event.editor;
                        editor.config.filebrowserBrowseUrl = '{0}';   
                     }});
                 
                    ", this.FileBrowserBrowseUrl);
            }else{
                    
           

                SB.Append("  $(document).ready(function() {  \n");
                SB.Append("     var _id = '" + this.ClientID + "';\n");  

                        SB.Append("     CKEDITOR.replace( _id,  {  \n");
                        SB.Append("         filebrowserBrowseUrl : '" + this.FileBrowserBrowseUrl + "'  \n");
                        SB.Append("         , height: '" + this.Height.ToString() + "'  \n");
                        SB.Append("         , width: '" + this.Width.ToString() + "'    \n");
                        if(!BlockEditorCss){
                            SB.Append("         , contentsCss : '" + this.ContentsCss + "'  \n"); 
                        } 
                        if (plugins != "") 
                            SB.Append("         , extraPlugins : " + plugins + "   \n"); 
                        if (this.Toolbar != "") 
                            SB.Append("         , toolbar : '" + this.Toolbar + "'   \n");
             
                        SB.Append("     }); \n");
                 
                SB.Append("  }); \n");


            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID + "ckeditor-replace", SB.ToString(), true);

            if (this.BackgroundStyle != null)
                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    this.ClientID + "ckeditor-style",
                    "<style> .ckedit{" + this.BackgroundStyle + "} \n .cke_contents iframe{" + this.BackgroundStyle + "}  </style>  ", false);

            

        }
    }

 
}
