using System;
using System.Web;
using System.Web.UI;

namespace NoorAlMalaikaSchool
{
    public partial class AccessDenied : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = Session["DeniedPath"] as string;
            if (!string.IsNullOrEmpty(path))
            {
                litDeniedPath.Text = "الصفحة المطلوبة: " + HttpUtility.HtmlEncode(path);
            }
            else
            {
                litDeniedPath.Text = "";
            }
        }
    }
}