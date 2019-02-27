using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ASPXUtils
{
    [ToolboxData("<{0}:ImageGallery runat=server></{0}:ImageGallery>")]
    public class ImageGallery : CompositeControl
    {
        public string ScriptTagID { get; set; }
        public string ScriptType { get; set; }

        private string _GalleryType = "";
        public string GalleryType
        {
            get { return _GalleryType; }
            set { _GalleryType = value; }
        }
        private string _ImageFolder = "";
        public string ImageFolder
        {
            get { return _ImageFolder; }
            set { _ImageFolder = value; }
        }
        private string _TWidth = "100";
        public string TWidth
        {
            get { return _TWidth; }
            set { _TWidth = value; }
        }
        private string _THeight = "100";
        public string THeight
        {
            get { return _THeight; }
            set { _THeight = value; }
        }
        private string _Width = "700";
        public string Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        private string _Height = "350";
        public string Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        private bool _LoadScripts = true;
        public bool LoadScripts
        {
            get { return _LoadScripts; }
            set { _LoadScripts = value; }
        }

        private List<GalleryImg> _oGalleryImages = new List<GalleryImg>();
        public List<GalleryImg> oGalleryImages 
        {
            get { 
                if(_oGalleryImages==null)
                    _oGalleryImages = new List<GalleryImg>();
                return _oGalleryImages; }
            set { _oGalleryImages = value; }
        }
        protected Literal litScriptResources;
        protected Literal litHTML;
 

        protected override void CreateChildControls()
        {
            Controls.Clear();
            if (this.ImageFolder != "")
            {
                LoadImagesFromFolder(this.ImageFolder);
            }
            LoadControls();
        }
        public override void DataBind()
        {
            CreateChildControls();
            ChildControlsCreated = true; 
            base.DataBind();
        }

        public override ControlCollection Controls
        {
            get { EnsureChildControls(); return base.Controls; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            RenderContents(writer);
        }

        public void LoadImage(string img, string thumb, string caption)
        {
            oGalleryImages.Add(new GalleryImg() { ImageSRC = img, ThumbSRC = thumb, Caption = caption, Header = "", NavigateURL = "" });
        }
        public void LoadImage(string img, string thumb, string caption, string header, string navigateURL)
        {
            oGalleryImages.Add(new GalleryImg() { ImageSRC = img, ThumbSRC = thumb, Caption = caption, Header = header, NavigateURL = navigateURL });
        }

        public void LoadControls()
        {

            litScriptResources = new Literal();
            Controls.Add(litScriptResources);

            litHTML = new Literal();
            Controls.Add(litHTML);

            litScriptResources.Text = "";
            litHTML.Text = "";

            if (this.LoadScripts && this.GalleryType != "")
            {
                LoadJS();
                LoadCSS();
            }

            if (this.GalleryType == "jquery-newsslider")
                LoadSlider();
            if (this.GalleryType == "jquery-slideshow")
                LoadSlideShow();
            if (this.GalleryType == "jquery-galleria")
                LoadGalleria();
            if (this.GalleryType == "jquery-galleryview")
                LoadGalleryView();
            if (this.GalleryType == "jquery-colorbox")
                LoadColorBox();

            if (this.GalleryType == "jquery-lightbox")
                LoadLightBox();
            if (this.GalleryType == "jquery-galleriffic")
                LoadGalleriffic();
            if (this.GalleryType == "jquery-accessible-slideshow")
                LoadAccessibleSlideshow();
        }

        public void LoadImagesFromFolder(string _path)
        {
            DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(_path));
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo f in files)
                oGalleryImages.Add(new GalleryImg() { ImageSRC = "/" + _path + f.Name, ThumbSRC = "/" + _path + "th/" + f.Name, Caption = f.Name });
        }


        private void LoadAccessibleSlideshow()
        {

            StringBuilder objSB = new StringBuilder();
            objSB.Append("<div id='pageContainer'> ");
            objSB.Append(" <div id='slideshow'> ");
            objSB.Append("   <div id='slidesContainer'> ");
            foreach (GalleryImg gi in oGalleryImages)
            {
                objSB.Append("     <div class='slide'> ");
                objSB.Append("       <h2>" + gi.Header + "</h2> ");
                objSB.Append("       <p>" + gi.Caption + "</p><img style='height:120px;width:220px;' src='" + gi.ImageSRC + "' /> ");
                objSB.Append("     </div> ");
            }
            objSB.Append("   </div> ");
            objSB.Append(" </div> ");
            objSB.Append("</div>");
            litHTML.Text = objSB.ToString();

        }

        private void LoadGalleriffic()
        {

            StringBuilder objSB = new StringBuilder();
            objSB.Append("<div style='width:" + Convert.ToString(Convert.ToInt32(this.Width) + 120) + "px; border:1px #ddd solid;margin: 0 auto;padding: 20px;'>");
            objSB.Append("<div id='gallery' class='content'> ");
            objSB.Append(" <div id='controls' class='controls'></div> ");
            objSB.Append(" <div class='slideshow-container'> ");
            objSB.Append("	<div id='loading' class='loader'></div> ");
            objSB.Append("	<div id='slideshow' class='slideshow'></div> ");
            objSB.Append(" </div> ");
            objSB.Append(" <div id='caption' class='caption-container'></div> ");
            objSB.Append("</div>");
            objSB.Append("<div id='thumbs' class='navigation'> ");
            objSB.Append(" <ul class='thumbs noscript'> ");

            foreach (GalleryImg gi in oGalleryImages)
            {

                objSB.Append("	<li> ");
                objSB.Append("		<a class='thumb' name='leaf' href='" + gi.ImageSRC + "' title='" + gi.Caption + "'> ");
                objSB.Append("			<img src='" + gi.ThumbSRC + "' width='60' alt='" + gi.Caption + "'  /> ");
                objSB.Append("		</a> ");
                objSB.Append("		<div class='caption'> ");
                objSB.Append("			<div class='download'> ");
                objSB.Append("				<a href='" + gi.ThumbSRC + "'>Download Original</a> ");
                objSB.Append("			</div>");
                objSB.Append("			<div class='image-title'>" + gi.Header + "</div> ");
                objSB.Append("			<div class='image-desc'>" + gi.Caption + "</div> ");
                objSB.Append("		</div> ");
                objSB.Append("	</li>  ");
            }

            objSB.Append(" </ul> ");
            objSB.Append("</div> ");
            objSB.Append("<div style='clear: both;'></div> ");
            objSB.Append("</div> ");


            objSB.Append("<style> div.content { display: none; float: right; width: " + this.Width + "px !important; }</style>");
            objSB.Append("<style> div.slideshow a.advance-link, span.image-caption , div.loader { width:" + this.Width + "px !important; }</style>");

            litHTML.Text = objSB.ToString();
        }

        private void LoadLightBox()
        {
            StringBuilder objSB = new StringBuilder();
            objSB.Append(" <div id='gallery'> ");
            objSB.Append("     <ul> ");
            foreach (GalleryImg gi in oGalleryImages)
            {
                objSB.Append("         <li> ");
                objSB.Append("             <a href='" + gi.ImageSRC + "' title='" + gi.Caption + "'> ");
                objSB.Append("                 <img src='" + gi.ThumbSRC + "' width='" + this.TWidth + "'  height='" + this.THeight + "'  alt='' /> ");
                objSB.Append("             </a> ");
                objSB.Append("         </li> ");
            }

            objSB.Append("     </ul> ");
            objSB.Append(" </div> ");
            objSB.Append("     <script> ");
            objSB.Append("     $(function() {   $('#gallery a').lightBox();   }); ");
            objSB.Append("     </script> ");
            objSB.Append("<style>");
            objSB.Append(" 	#gallery { padding: 10px; width: " + this.Width + "px; 	} ");
            objSB.Append(" 	#gallery ul { list-style: none; } ");
            objSB.Append(" 	#gallery ul li { display: inline; } ");
            objSB.Append(" 	#gallery ul img { border-width:0px; ");
            objSB.Append("</style> ");
            litHTML.Text = objSB.ToString();

        }

 

        private void LoadColorBox()
        {
            StringBuilder objSB = new StringBuilder();
            objSB.Append("");
            foreach (GalleryImg gi in oGalleryImages)
            {
                objSB.Append("<a href=\"" + gi.ImageSRC + "\" rel=\"img\"  title=\"" + gi.Caption + "\" ");
                objSB.Append(" class='cb_ahref' style='vertical-align:text-bottom;text-align:center;margin:12px; border-width:0px;display:block;width:" + AddDim(this.TWidth, 0) + "px;height:" + AddDim(this.THeight, 0) + "px; float:left; ' "); 
                objSB.Append(">");
                objSB.Append("<img  src='" + gi.ThumbSRC + "' style='width:" + this.TWidth + "px;height:" + this.THeight + "px;' border='0' />"); 
                objSB.Append("</a>"); 
            
            
            }
            objSB.Append(" <script> ");
            objSB.Append(" $(document).ready(function(){ ");
            objSB.Append("   $(\"a[rel='img']\").colorbox(); ");
            objSB.Append(" }); ");
            objSB.Append(" </script> ");
            litHTML.Text = objSB.ToString();
        }

        private void LoadGalleryView()
        {
            StringBuilder objSB = new StringBuilder();
            objSB.Append("");
            objSB.Append(" <div id='photos' class='galleryview'> ");

            foreach (GalleryImg gi in oGalleryImages)
            {
                objSB.Append("   <div class='panel'>  ");
                objSB.Append("     <img src='" + gi.ImageSRC + "' />  ");
                objSB.Append("     <div class='panel-overlay'> ");
                objSB.Append("       <h2>" + gi.Header + "</h2> ");
                objSB.Append("       <p>" + gi.Caption + "</p> ");
                objSB.Append("     </div> ");
                objSB.Append("   </div> ");
            }

            objSB.Append("<ul class='filmstrip'>");
            foreach (GalleryImg gi in oGalleryImages)
            {
                objSB.Append(" <li><img src='" + gi.ThumbSRC + "' alt='" + gi.Caption + "' width='" + this.TWidth + "' height='" + this.THeight + "'  title='" + gi.Caption + "' /></li>  ");
            }

            objSB.Append("</ul>");
            objSB.Append("</div>");

            if (ScriptType == "filmstrip")
            {

                objSB.Append("<script> ");
                objSB.Append(" $('#photos').galleryView({ ");
                objSB.Append(" 	   panel_width: " + this.Width + ", ");
                objSB.Append(" 	   panel_height: " + this.Height + ", ");
                objSB.Append(" 	   frame_width: " + this.TWidth + ", ");
                objSB.Append(" 	   frame_height: " + this.THeight + ", ");
                objSB.Append("     overlay_color: '#222', ");
                objSB.Append("     overlay_text_color: 'white', ");
                objSB.Append("     caption_text_color: '#222', ");
                objSB.Append("     background_color: 'transparent', ");
                objSB.Append("     border: 'none', ");
                objSB.Append("     nav_theme: 'light', ");
                objSB.Append("     easing: 'easeInOutQuad', ");
                objSB.Append("     pause_on_hover: true ");
                objSB.Append(" }); </script>");

            }
            else
            {
                objSB.Append("<script> $('#photos').galleryView({ ");
                objSB.Append(" 	panel_width: " + this.Width + ", ");
                objSB.Append(" 	panel_height: " + this.Height + ", ");
                objSB.Append(" 	frame_width: " + this.TWidth + ", ");
                objSB.Append(" 	frame_height: " + this.THeight + " ");
                objSB.Append(" }); </script>");
            }
            litHTML.Text = objSB.ToString();
        }

        private void LoadGalleria()
        {

            StringBuilder objSB = new StringBuilder();
            objSB.Append("     <div class='content'> ");
            objSB.Append("         <div id=\"galleria\"> ");
            foreach (GalleryImg gi in oGalleryImages)
            {
                objSB.Append(" <img title=\"" + gi.Caption + "\" alt=\"" + gi.Caption + "\" src='" + gi.ImageSRC + "'> ");
            }
            objSB.Append("         </div> ");
            objSB.Append("     </div>  ");
            objSB.Append("     <script>   ");
            objSB.Append("       Galleria.loadTheme('/js/jquery-galleria/themes/classic/galleria.classic.js'); $('#galleria').galleria();  ");
            objSB.Append("     </script> ");
            litHTML.Text = objSB.ToString();

        }

        private void LoadSlideShow()
        {

            StringBuilder objSB = new StringBuilder();
            objSB.Append(" <div style='width:" + this.Width + "px; '>\n ");

            objSB.Append(" <div id=\"gallery\">\n ");
            int imgCnt = 0;
            string clsID = "";
            foreach (GalleryImg gi in oGalleryImages)
            {
                clsID = "";
                if (imgCnt == 0)
                    clsID = "show";
                objSB.Append(" 	<a href=\"#\" class=\"" + clsID + "\">\n  ");
                objSB.Append(" 		<img src='" + gi.ImageSRC + "' width=\"" + this.Width + "\" height=\"" + this.Height + "\"   title=\"\" alt=\"\"  rel='" + gi.Caption + "' />\n  ");
                objSB.Append(" 	</a>\n ");
                imgCnt++;
            }
            objSB.Append(" 	<div class=\"caption\"><div class=\"content\"></div></div>\n  ");
            objSB.Append(" </div>\n  ");
            objSB.Append(" <div class=\"clear\"></div>\n<br/><br/> \n ");

            objSB.Append(" </div>\n  ");

            objSB.Append("<script> \n $(document).ready( function() { slideShow();  } );  \n  </script>");
            objSB.Append("<style> \n #gallery { position:relative; height:" + this.Height + "px; } \n  </style>");
            litHTML.Text = objSB.ToString();
        }

        private void LoadSlider()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<div id='slider'>\n");
            sb.Append("<div id='mask-gallery'>\n");
            sb.Append("<ul id='gallery'>\n");
            foreach (GalleryImg gi in oGalleryImages)
                sb.Append("<li><img src='" + gi.ImageSRC + "'  alt=''/></li>\n");

            sb.Append("</ul>\n");
            sb.Append("</div>\n");
            sb.Append("<div id='mask-excerpt'>\n");
            sb.Append("<ul id='excerpt'>\n");
            foreach (GalleryImg gi in oGalleryImages)
                sb.Append(" <li>" + gi.Caption + " </li>\n");

            sb.Append("</ul>\n");
            sb.Append("</div>\n");
            sb.Append("</div>\n");
            sb.Append("<div class='clear'></div>\n");

            litHTML.Text = sb.ToString();
        }

        protected void LoadJS()
        {
            StringBuilder sb = new StringBuilder();
            string _filePath = "";
            DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/js/" + this.GalleryType + "/js"));
            FileInfo[] files = di.GetFiles("*.js");
            foreach (FileInfo f in files)
            {
                _filePath = ASPXUtils.FileUtils.ToVirtual(f.FullName, "js");
                sb.Append("<script type='text/javascript' src='" + _filePath + "'></script>\n");
            }
            litScriptResources.Text += sb.ToString();
        }
        protected void LoadCSS()
        {
            StringBuilder sb = new StringBuilder();
            string _filePath = "";
            DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/js/" + this.GalleryType + "/css"));
            FileInfo[] files = di.GetFiles("*.css");
            foreach (FileInfo f in files)
            {
                _filePath = ASPXUtils.FileUtils.ToVirtual(f.FullName, "js");
                sb.Append("<link rel='stylesheet' href='" + _filePath + "' type='text/css' />\n");
            }
            litScriptResources.Text += sb.ToString();
        }
        private string AddDim(string dim, int _add)
        {
            return Convert.ToString(Convert.ToInt32(dim) + _add);
        }
    }
}


