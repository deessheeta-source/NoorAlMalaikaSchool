using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool
{
    public class Global : HttpApplication
    {
        // =========================================================
        // بدء التطبيق
        // =========================================================
        protected void Application_Start(object sender, EventArgs applicationStartEventArgs)
        {
            try
            {
                using (var db = new SchoolDbContext())
                {
                    db.Database.Initialize(force: false);
                    SeedData.Initialize(db);
                }
            }
            catch (Exception ex)
            {
                WriteSimpleLog(ex, "Application_Start");
                throw; // لا تبتلع الخطأ في بدء التطبيق
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Keep links created by older versions working after the page was renamed.
            string appRelativePath = Request.AppRelativeCurrentExecutionFilePath ?? string.Empty;
            if (appRelativePath.Equals(
                "~/Pages/Payroll/DetailsVies.aspx",
                StringComparison.OrdinalIgnoreCase))
            {
                string destination =
                    VirtualPathUtility.ToAbsolute("~/Pages/Payroll/DetailsView.aspx") +
                    Request.Url.Query;

                Response.Redirect(destination, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }


        // =========================================================
        // معالجة الأخطاء العامة
        // =========================================================
        protected void Application_Error(object sender, EventArgs applicationErrorEventArgs)
        {
            Exception exception = Server.GetLastError();

            // 1) التسجيل عبر الـ logger الموجود (كما كان)
            try
            {
                AppErrorLogger.Log(exception, "Application_Error");
            }
            catch { /* لا نسمح لفشل التسجيل بإسقاط الصفحة */ }

            // 2) تسجيل مبسّط في ملف نصي سهل القراءة
            WriteSimpleLog(exception, "Application_Error");

            // 3) الانتقال إلى صفحة الخطأ
            try
            {
                Server.ClearError();

                string currentPath = Request.AppRelativeCurrentExecutionFilePath ?? string.Empty;

                if (!currentPath.Equals("~/Error.aspx", StringComparison.OrdinalIgnoreCase))
                {
                    Server.Transfer("~/Error.aspx", true);
                }
            }
            catch (Exception transferException)
            {
                try
                {
                    AppErrorLogger.Log(transferException, "Application_Error.Transfer");
                }
                catch { }

                WriteSimpleLog(transferException, "Application_Error.Transfer");
            }
        }


        // =========================================================
        // تسجيل مبسّط في ~/App_Data/app-errors.log
        // =========================================================
        private void WriteSimpleLog(Exception ex, string source)
        {
            if (ex == null) return;

            try
            {
                string folder = HostingEnvironment.MapPath("~/App_Data");
                if (folder == null) return;

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string path = Path.Combine(folder, "app-errors.log");

                string line =
                    "=========================================================" + Environment.NewLine +
                    "Time   : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine +
                    "Source : " + source + Environment.NewLine +
                    "URL    : " + (Request != null ? Request.Url.ToString() : "N/A") + Environment.NewLine +
                    "Type   : " + ex.GetType().FullName + Environment.NewLine +
                    "Message: " + ex.Message + Environment.NewLine +
                    "--- StackTrace ---" + Environment.NewLine +
                    ex.ToString() + Environment.NewLine +
                    Environment.NewLine;

                File.AppendAllText(path, line, System.Text.Encoding.UTF8);
            }
            catch
            {
                // تجاهل أي فشل في التسجيل نفسه
            }
        }
    }
}
