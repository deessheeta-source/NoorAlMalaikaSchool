using System;
using System.Web;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Data.Helpers
{
    /// <summary>
    /// Generates stage-aware application URLs for WebForms markup and code-behind.
    /// Does not access the database.
    /// </summary>
    public static class StageUrlHelper
    {
        public static string GetStageUrl(string page)
        {
            if (string.IsNullOrWhiteSpace(page))
                return VirtualPathUtility.ToAbsolute("~/");

            string cleanPage = page.Trim().TrimStart('~', '/');
            string appPath = HttpRuntime.AppDomainAppVirtualPath;
            if (string.IsNullOrEmpty(appPath)) appPath = "/";
            appPath = appPath.TrimEnd('/');

            string url = cleanPage.Equals("Default.aspx", StringComparison.OrdinalIgnoreCase)
                ? appPath + "/Default.aspx"
                : appPath + "/Pages/" + cleanPage;

            EducationStage stage = EnumHelper.GetCurrentStage();
            string separator = url.IndexOf('?') >= 0 ? "&" : "?";
            return url + separator + "stage=" + HttpUtility.UrlEncode(stage.ToString());
        }
    }
}
