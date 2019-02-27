using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.Collections;
using System.Drawing;
using System.IO;
namespace ASPXUtils
{
    public static class ImageHelper
    {

        public static void CreateThumbnail(string PathFrom, string PathTo, int Width, int Height)
        {
            DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath( PathTo ));
            if (!di.Exists)
                di.Create();
            string _thumb = String.Format("{0}/{1}", di.FullName, System.IO.Path.GetFileName(PathFrom));
            ResizeImage(HttpContext.Current.Server.MapPath(PathFrom), HttpContext.Current.Server.MapPath(_thumb), Width, Height, true);
        }

        public static void ResizeImage(string PathFrom, string PathTo, int Width, int Height, bool MaintainAspectRatio)
        {
            if (PathTo == "")
                PathTo = PathFrom;
            System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(PathFrom); 

            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            int NewHeight = FullsizeImage.Height * Width / FullsizeImage.Width;
            if (NewHeight > Height)
            {
                Width = FullsizeImage.Width * Height / FullsizeImage.Height;
                NewHeight = Height;
            }
            System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(Width, NewHeight, null, IntPtr.Zero); 
            FullsizeImage.Dispose();
            NewImage.Save(PathTo);
        } 
    }
}
