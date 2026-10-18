using System;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>Simple application error logger that never writes request data or secrets.</summary>
    public static class AppErrorLogger
    {
        private static readonly object Sync = new object();
        private const long MaxLogBytes = 5L * 1024L * 1024L;

        public static void Log(Exception exception, string source)
        {
            try
            {
                string folder = HostingEnvironment.MapPath("~/App_Data/Logs");
                if (string.IsNullOrEmpty(folder)) return;
                Directory.CreateDirectory(folder);

                string path = Path.Combine(folder, "application-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                lock (Sync)
                {
                    if (File.Exists(path) && new FileInfo(path).Length >= MaxLogBytes)
                    {
                        string rotated = path + ".1";
                        if (File.Exists(rotated)) File.Delete(rotated);
                        File.Move(path, rotated);
                    }

                    var sb = new StringBuilder();
                    sb.AppendLine("============================================================");
                    sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    sb.AppendLine("Source: " + (source ?? "Application"));
                    if (exception != null)
                    {
                        sb.AppendLine("Exception: " + exception.GetType().FullName);
                        sb.AppendLine("Message: " + exception.Message);
                        sb.AppendLine("StackTrace: " + exception.StackTrace);
                        if (exception.InnerException != null)
                        {
                            sb.AppendLine("InnerException: " + exception.InnerException.GetType().FullName);
                            sb.AppendLine("InnerMessage: " + exception.InnerException.Message);
                            sb.AppendLine("InnerStackTrace: " + exception.InnerException.StackTrace);
                        }
                    }
                    File.AppendAllText(path, sb.ToString(), Encoding.UTF8);
                }
            }
            catch
            {
                // Logging must never cause a second application failure.
            }
        }
    }
}
