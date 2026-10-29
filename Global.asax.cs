using System;
using System.Configuration;
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
            // Housekeeping first: it is best-effort and must never block startup.
            LogMaintenanceService.Run();

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
                // Startup failures are logged loudly. By default they no longer take the
                // whole site down: the app starts and each page surfaces its own database
                // error, which is far easier to diagnose than a blank 500 at startup.
                // Set FailFastOnStartup=true if you prefer the old fail-fast behaviour.
                try { AppErrorLogger.Log(ex, "Application_Start"); } catch { }
                WriteSimpleLog(ex, "Application_Start");

                if (IsAppSettingTrue("FailFastOnStartup"))
                {
                    throw;
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // HTTPS enforcement and HSTS are opt-in (see Web.config appSettings).
            // Both are no-ops on an HTTP-only installation.
            if (EnforceHttps())
            {
                return;
            }

            // Keep links created by older versions working after the page was renamed.
            string appRelativePath = Request.AppRelativeCurrentExecutionFilePath ?? string.Empty;
            if (appRelativePath.Equals(
                "~/Pages/Payroll/DetailsVies.aspx", // link-check-ignore (legacy alias)
                StringComparison.OrdinalIgnoreCase))
            {
                string destination =
                    VirtualPathUtility.ToAbsolute("~/Pages/Payroll/DetailsView.aspx") +
                    Request.Url.Query;

                Response.Redirect(destination, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        /// <summary>
        /// Redirects insecure requests to HTTPS when EnableHttpsRedirect=true and adds
        /// Strict-Transport-Security when EnableHsts=true. Returns true when the current
        /// request was redirected (the caller must then stop processing the request).
        /// </summary>
        private bool EnforceHttps()
        {
            bool enableRedirect = IsAppSettingTrue("EnableHttpsRedirect");
            bool enableHsts = IsAppSettingTrue("EnableHsts");
            if (!enableRedirect && !enableHsts)
            {
                return false;
            }

            bool isSecure = Request.IsSecureConnection ||
                            (IsAppSettingTrue("TrustForwardedProto") &&
                             string.Equals(Request.Headers["X-Forwarded-Proto"], "https",
                                 StringComparison.OrdinalIgnoreCase));

            if (enableHsts && isSecure)
            {
                // HSTS is only meaningful (and only honoured by browsers) over HTTPS.
                Response.Headers["Strict-Transport-Security"] =
                    "max-age=31536000; includeSubDomains";
            }

            if (!enableRedirect || isSecure)
            {
                return false;
            }

            UriBuilder builder = new UriBuilder(Request.Url)
            {
                Scheme = Uri.UriSchemeHttps,
                Port = -1 // use the default HTTPS port unless a custom one is configured
            };

            string httpsPort = ConfigurationManager.AppSettings["HttpsPort"];
            int configuredPort;
            if (!string.IsNullOrWhiteSpace(httpsPort) &&
                int.TryParse(httpsPort, out configuredPort) && configuredPort > 0)
            {
                builder.Port = configuredPort;
            }

            // 301 preserves GET semantics; 308 makes clients repeat the same method and body,
            // which is what ASP.NET postbacks need if they ever arrive over HTTP.
            int statusCode = (Request.HttpMethod == "GET" || Request.HttpMethod == "HEAD") ? 301 : 308;

            Response.StatusCode = statusCode;
            Response.StatusDescription = statusCode == 301 ? "Moved Permanently" : "Permanent Redirect";
            Response.RedirectLocation = builder.Uri.AbsoluteUri;
            Response.End();
            return true;
        }

        private static bool IsAppSettingTrue(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            bool parsed;
            return !string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out parsed) && parsed;
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
