using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Configuration;

namespace ASPXUtils.Controls
{
    public class FileBrowser : UserControl
    {
        public StringBuilder HTML = new StringBuilder();

        public Panel pnlMasterPane = new Panel() { ID = "MasterPane", CssClass = "master-pane" };
        public Panel pnlPaneLeft = new Panel() { ID = "PaneLeft", CssClass = "master-pane-left" };
        public Panel pnlPaneRight = new Panel() { ID = "PaneRight", CssClass = "master-pane-right" };
        public Panel pnlDisplayFile = new Panel() { ID = "DisplayFile", CssClass = "displaypane" };

        public HiddenField hidLoadPath = new HiddenField() { ID = "hidLoadPath" };
        public HiddenField hidCurrentPath = new HiddenField() { ID = "hidCurrentPath" };
        public HiddenField hidCurrentFile = new HiddenField() { ID = "hidCurrentFile" };

        public TextBox txtCurrentPath = new TextBox() { ID = "txtCurrentPath", CssClass = "txtCurrentPath" };
        public Label lblFolders = new Label() { ID = "lblFolders", CssClass = "folderpane" };
        public Label litFiles = new Label() { ID = "litFiles", CssClass = "filepane" };
        public ASPXUtils.Controls.FileUploader oUploader = new ASPXUtils.Controls.FileUploader() { ID = "oUploader" };
        public Button cmdUpload = new Button() { ID = "cmdUpload", CssClass = "ctrlcmdUpload", Text = "Upload" };
        public Button cmdSelect = new Button() { ID = "cmdSelect", CssClass = "ctrlcmdSelect", OnClientClick = "Select_Click(this); return false;", Text = "Select" };
        public Button cmdDelete = new Button() { ID = "cmdDelete", CssClass = "ctrlcmdDelete", OnClientClick = " if(!confirm('Are you sure you want delete?')) return false; ", Text = "Delete" };
        public TextBox txtSelectedFileName = new TextBox() { ID = "txtSelectedFileName", CssClass = "txtSelectedFileName" };
        public Image imgFile = new Image() { ID = "imgFile", CssClass = "imgFile" };

        public string targetID = ASPXUtils.HTTPUtils.QString("t");
        public PlaceHolder ControlHolder = new PlaceHolder();
        public string BaseFolderPath { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            ControlHolder.Controls.Add(hidLoadPath);
            ControlHolder.Controls.Add(hidCurrentPath);
            ControlHolder.Controls.Add(hidCurrentFile);

            ControlHolder.Controls.Add(pnlMasterPane);
            pnlMasterPane.Controls.Add(pnlPaneLeft);
            pnlMasterPane.Controls.Add(pnlPaneRight);
            pnlPaneRight.Controls.Add(pnlDisplayFile);

            pnlPaneLeft.Controls.Add(txtCurrentPath);
            pnlPaneLeft.Controls.Add(new Literal() { Text = "<br class='clearfix'/>" });

            pnlPaneLeft.Controls.Add(lblFolders);

            pnlPaneLeft.Controls.Add(litFiles);
            pnlPaneLeft.Controls.Add(new Literal() { Text = "<br class='clearfix'/><br/><br/><br/><br/>" });

            pnlPaneLeft.Controls.Add(oUploader);
            cmdUpload.Click += new EventHandler(Upload_Click);
            pnlPaneLeft.Controls.Add(cmdUpload);

            pnlDisplayFile.Controls.Add(cmdSelect);

            cmdDelete.Click += new EventHandler(Delete_Click);
            pnlDisplayFile.Controls.Add(cmdDelete);
            pnlDisplayFile.Controls.Add(new Literal() { Text = "<br class='clearfix'/>" });

            pnlDisplayFile.Controls.Add(txtSelectedFileName);
            pnlDisplayFile.Controls.Add(new Literal() { Text = "<br class='clearfix'/>" });

            pnlDisplayFile.Controls.Add(imgFile);

            

            ControlHolder.Controls.Add(new Literal() { Text = "<script src='/js/browse.js'></script>" });
            ControlHolder.Controls.Add(new Literal() { Text = "<link href='/css/browse.css' rel='stylesheet' type='text/css' />" });

            ControlHolder.Controls.Add(new Literal() { Text = "<script src='/js/splitter.js'></script>" }); 
            ControlHolder.Controls.Add(new Literal() { Text = "<script> $().ready(function () { $('.master-pane').splitter(); });</script>" });

            this.Controls.Add(ControlHolder);

            hidLoadPath.Value = ASPXUtils.HTTPUtils.QString("p");

            //if (!IsPostBack)
            //{
            //    LoadTree();   
            //}
            if (oUploader.SavePath == null || oUploader.SavePath == "")
            {
                oUploader.SavePath = ConfigurationManager.AppSettings["UploadPath"];
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            RenderScriptVars();
        }
        protected void Upload_Click(object sender, EventArgs e)
        {
            //string path = hidCurrentPath.Value;
            oUploader.SavePath = hidCurrentPath.Value;
            oUploader.UploadFiles();

            //HttpContext.Current.Response.Write(oUploader.SavePath);
            HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl + "&p=" + hidCurrentPath.Value + "&esp=mp");
        }
        protected void Delete_Click(object sender, EventArgs e)
        {
            string sPath = HttpContext.Current.Server.MapPath(hidCurrentPath.Value + hidCurrentFile.Value);
            if (System.IO.File.Exists(sPath))
                System.IO.File.Delete(sPath);
            
            HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Path + "?p=" + hidCurrentPath.Value);
        }
        public void LoadTree()
        {
            DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["UploadPath"]));
            List<DirectoryInfo> folders = new List<DirectoryInfo>(); 
            int level = 0; 
            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                HTML.AppendFormat("<ul data-path='{0}'  >\n", VirtualPath(di.FullName));
                HTML.AppendFormat("\t<li><a href='#'  id='{0}'  class='dir folder' data-path='{1}'>{0}</a>\n", di.Name, VirtualPath(di.FullName));
                 
                foreach (DirectoryInfo d in di.GetDirectories())
                {
                    HTML.AppendFormat("\t\t<ul data-path='{0}' class='subfolder {1}-folders'  data-dir='{1}'>\n", VirtualPath(di.FullName), di.Name);
                    HTML.AppendFormat("\t\t\t<li><a href='#'  id='{0}'  class='dir folder' data-path='{1}'>{0}</a>\n", d.Name, VirtualPath(d.FullName));
                    GetFiles(d, level);
                    HTML.AppendFormat("\t</li>\n");
                    HTML.AppendFormat("</ul>\n");
                } 
                GetFiles(di, level);
                 
                HTML.AppendFormat("\t</li>\n");
                HTML.AppendFormat("</ul>\n");
            }
             
            lblFolders.Text += HTML.ToString(); 
        }
        public string VirtualPath(string PhysicalPath)
        {
            return ("\\" + PhysicalPath.Replace(HttpContext.Current.Server.MapPath("~/"), "")).Replace("\\", "/") + "/";
        }
        private void GetFiles(DirectoryInfo d, int level)
        {
            HTML.AppendFormat("\t<ul data-path='{0}' class='files {1}-files'  data-dir='{1}'>\n", VirtualPath(d.FullName), d.Name);
            foreach (FileInfo f in d.GetFiles("*"))
            {
                HTML.AppendFormat("\t\t<li class='fileitem'><a href='#' class='file' onclick='file_click(this)' data-name='{0}' data-path='{1}'>{0}</a></li>\n", f.Name, VirtualPath(d.FullName));
                level++;
            }
            HTML.AppendFormat("\t</ul>\n");
        }
        private void RenderScriptVars()
        {
            string scripts = @"
             <script>
             var cntrl_hidLoadPath = $('#{0}');
             var cntrl_hidCurrentPath = $('#{1}');
             var cntrl_hidCurrentFile = $('#{2}');  
             var cntrl_func; 
            if(window.opener){{
                cntrl_func = window.opener.CTRL{3}_browseComplete
            }}
             </script>
        ";
            scripts = string.Format(
                scripts,
                hidLoadPath.ClientID,
                hidCurrentPath.ClientID,
                hidCurrentFile.ClientID,
                targetID
                );
            ControlHolder.Controls.Add(new Literal() { Text = scripts });
        } 
    }
}
