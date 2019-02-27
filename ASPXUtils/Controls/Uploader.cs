using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web;
using System.Configuration;

namespace ASPXUtils.Controls
{
    public class FileUploader : UserControl
    {
        
        public string SavePath { get; set; }
        private string _ValidExtentions = "";
        public string ValidExtentions
        {
            get { return _ValidExtentions; }
            set { _ValidExtentions = value; }
        } 
        private string _ValidType = "";
        public string ValidType
        {
            get { return _ValidType; }
            set
            {
                if (value == "IMAGE") 
                    this.ValidExtentions = ".jpg, .jpeg, .bmp, .gif, .png";
               
                _ValidType = value;

            }
        }

        private string _ThumbPath = "th/";
        public string ThumbPath
        {
            get { return _ThumbPath; }
            set { _ThumbPath = value; }
        } 
        private string _ForceFileName = "";
        public string ForceFileName
        {
            get { return _ForceFileName; }
            set { _ForceFileName = value; }
        } 
        private int _ImageMaxSize = 0;
        public int ImageMaxSize
        {
            get { return _ImageMaxSize; }
            set { _ImageMaxSize = value; }
        } 
        private int _ThumbSize = 100;
        public int ThumbSize
        {
            get { return _ThumbSize; }
            set { _ThumbSize = value; }
        }
        private bool _CreateThumb = false;
        public bool CreateThumb
        {
            get { return _CreateThumb; }
            set { _CreateThumb = value; }
        }
        private bool _ShowUpload = false;
        public bool ShowUpload
        {
            get { return _ShowUpload; }
            set { _ShowUpload = value; }
        }  
        private int _FileCount = 1;
        public int FileCount
        {
            get { return _FileCount; }
            set { _FileCount = value; }
        } 
        private string _FileName = "";
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        } 
        private string _ButtonValue = "Upload";
        public string ButtonValue
        {
            get { return _ButtonValue; }
            set { _ButtonValue = value; }
        } 
        private int _FilesUploaded = 0;
        public int FilesUploaded
        {
            get { return _FilesUploaded; }
            set { _FilesUploaded = value; }
        }

        //btnUpload
        public string _OriginalFullName = "";
        public List<string> _UploadedFileNameList = new List<string>();
        private Button cmdSubmit = new Button() { Text = "Upload" };
        public Panel ControlHolder = new Panel() { ID="ControlHolder" };
        private Label lblFeedBack = new Label() { ID="lblFeedBack" };

        protected void cmdSubmit_Click(object source, EventArgs e)
        {
            this.UploadFiles(); 
            UploadComplete();
        }
        protected virtual void UploadComplete()
        { 
            string url = HttpContext.Current.Request.RawUrl;
            HttpContext.Current.Response.Redirect(url, true);
        }
        protected override void OnInit(EventArgs e)
        { 
            base.OnInit(e); 
            cmdSubmit.Click += new EventHandler(cmdSubmit_Click);
  
        }
        protected override void OnLoad(EventArgs e)
        { 
            base.OnLoad(e);
 
            if (this.SavePath == null) {
                 this.SavePath = ConfigurationManager.AppSettings["UploadPath"] ;
            }
              
            DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(this.SavePath));
            if (!di.Exists)
                di.Create();

            this.Controls.Add(ControlHolder);
            ControlHolder.Controls.Add(lblFeedBack);
           
            for (int i = 0; i < this.FileCount; i++)
            {
                ControlHolder.Controls.Add(new FileUpload() { ID = "FU" + i });
                ControlHolder.Controls.Add(new Literal() { Text = "<br />" });
            }

            if (ShowUpload)
                ControlHolder.Controls.Add(cmdSubmit);
        } 
        public void UploadFiles()
        {
            HttpFileCollection uploadedFiles = Request.Files;
            int _i = 0;    
            bool err = false;
            for ( _i = 0; _i < uploadedFiles.Count; _i++)
            {
                if (uploadedFiles[_i].ContentLength > 0)
                {
                    if (this.ForceFileName != "") 
                        _FileName = this.ForceFileName; 
                    else 
                        _FileName = CleanFName(uploadedFiles[_i].FileName); 
                    

                    if (IsExtValid(Path.GetExtension(_FileName).ToString().ToLower()))
                    {
                        _OriginalFullName = @"" + this.SavePath + Path.GetFileName(_FileName);
                        _UploadedFileNameList.Add(_OriginalFullName);
                         

                        uploadedFiles[_i].SaveAs(HttpContext.Current.Server.MapPath(_OriginalFullName));
                        this.FilesUploaded += 1;

                        if (this.CreateThumb) {
                            CreateThumbnail(_OriginalFullName);
                        } 
                        if (this.ImageMaxSize > 0)
                        {
                            ResizeOriginal(); 
                        } 
                        FeedBackGood();
                    }
                    else
                    {
                        err = true;
                        FeedBackBad();
                    }
                }
            }
  
        }
     
        public string CleanFName(string _fname) {
            foreach (char c in " ~`!@#$%^&*()+={}|[]\\<>?,/:;'\"")
                _fname = _fname.Replace(Convert.ToString(c), "");
            return _fname;
        }

        private void CreateThumbnail(string _pathOfOriginal ) {
            DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(this.SavePath + this.ThumbPath));
            if (!di.Exists) 
                di.Create(); 
            string _thumb = @"" + this.SavePath + this.ThumbPath + System.IO.Path.GetFileName(_FileName); 
            ResizeImage(HttpContext.Current.Server.MapPath(_pathOfOriginal), HttpContext.Current.Server.MapPath(_thumb), this.ThumbSize, this.ThumbSize );
        }

        private void ResizeOriginal(){
            System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(_OriginalFullName));
            int _w = image.Width;
            int _h = image.Height;
            image.Dispose();
            if (_w > this.ImageMaxSize || _h > this.ImageMaxSize)
                ResizeImage(HttpContext.Current.Server.MapPath(_OriginalFullName), HttpContext.Current.Server.MapPath(_OriginalFullName), this.ImageMaxSize, this.ImageMaxSize );
            
        } 
        private void FeedBackGood(){
            lblFeedBack.Text = "Upload Complete<br>";
            lblFeedBack.CssClass = "ok";
            lblFeedBack.Visible = true;
        }
        private void FeedBackBad()
        {
            lblFeedBack.Text = "<b>The file type you are attempting to upload is invalid. Valid file types: " + this.ValidExtentions + "</b><br>";
            lblFeedBack.CssClass = "ko";
            lblFeedBack.Visible = true;
        }

        private bool IsExtValid(string _ext) {
            if (this.ValidExtentions == "")
                return true;

            return this.ValidExtentions.ToLower().IndexOf(_ext) >= 0; 
        }

        private void ResizeImage(string OriginalFile, string NewFile, int NewWidth, int MaxHeight )
        { 
            ImageHelper.ResizeImage(OriginalFile, NewFile, NewWidth, MaxHeight, true); 
        }
    }
}
