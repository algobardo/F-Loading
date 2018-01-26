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
using System.Drawing;
using Security;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WebInterface
{
    public partial class SaveLogo : System.Web.UI.Page
    {
        private const string SCRIPT_TEMPLATE = "<" + "script " + "type=\"text/javascript\">window.parent.stopUpload('{0}',{1});" + "<" + "/script" + ">";
        private const int MAX_HEIGHT = 100;
        private const int MAX_WIDTH = 190;

        static System.Drawing.Image FixedSize(System.Drawing.Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(0, 0, destWidth, destHeight),
                new Rectangle(0, 0, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["deleteImg"] != null)
            {
                Response.Expires = -1; //required to keep the page from being cached on the client's browser
                Response.Clear();
                Response.ContentType = "text/plain";
                Page.Session["TE_currentLogo"] = null;
                Response.Write("");
                Response.End();
            }
            else
            {

                string script = string.Empty;
                Bitmap img = null;
                

                // Taking the uploaded files
                HttpFileCollection uploads = Request.Files;



                if (uploads.Count > 0 && uploads[0] != null && uploads[0].ContentLength > 0)
                {
                    try
                    {
                        img = new Bitmap(uploads[0].InputStream);
                    }
                    catch (ArgumentException)
                    {
                        script = string.Format(SCRIPT_TEMPLATE, "The uploaded file is not a valid image file.", "false");
                    }
                }

                else
                {
                    script = string.Format(SCRIPT_TEMPLATE, "Please specify a valid file.", "false");
                }

                if (string.IsNullOrEmpty(script))
                {
                    //Uploaded file is valid, now we can do whatever we like to do, copying it file system,
                    //saving it in db etc.

                    //Your Logic goes here

                    int newHeight = img.Height;
                    int newWidth = img.Width;
                    bool resize = false;


                    img = (Bitmap)FixedSize(img, MAX_WIDTH, MAX_HEIGHT);

                 /*
                    if (img.Height > MAX_HEIGHT )
                    {
                        resize = true;
                        newHeight = MAX_HEIGHT;
                    }

                    if (img.Width > MAX_WIDTH)
                    {
                        resize = true;
                        newWidth = MAX_WIDTH;
                    }
                    */
                    if (resize)
                        img = (Bitmap)img.GetThumbnailImage( newWidth, newHeight, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), System.IntPtr.Zero);



                    Security.IToken tok = (Security.IToken)Session["Token"];
                    if (tok == null)
                    {
                        script = string.Format(SCRIPT_TEMPLATE, "You need to be authed to edit the Theme", "false");
                    }
                    else
                    {
                        try
                        {

                            Page.Session["TE_currentLogo"] = img;

                            script = string.Format(SCRIPT_TEMPLATE, "drawLogo.aspx", "true");

                        }
                        catch (Exception ex)
                        {
                            script = string.Format(SCRIPT_TEMPLATE, ex.Message.ToString(), "false");
                        }
                    }

                    if (string.IsNullOrEmpty(script))
                    {
                        Page.Session["TE_currentLogo"] = img;
                        script = string.Format(SCRIPT_TEMPLATE, "drawLogo.aspx", "true");
                    }
                }

                //Sleeping for 2 seconds, fake delay, You should not it try at home.
                //System.Threading.Thread.Sleep(2 * 1000);

                ClientScript.RegisterStartupScript(this.GetType(), "uploadNotify", script);

            }
        }
        public bool ThumbnailCallback()
        {
            return true;
        }
    }
}
