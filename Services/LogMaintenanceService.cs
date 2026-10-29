using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>
    /// Best-effort housekeeping for the text logs under ~/App_Data.
    /// Runs once per application start and never throws: logging problems must
    /// not prevent the site from serving requests.
    /// </summary>
    public static class LogMaintenanceService
    {
        private const long MaxRootLogBytes = 5L * 1024L * 1024L;
        private const int RetentionDays = 90;
        private const int MaxArchivedFilesPerLog = 10;

        public static void Run()
        {
            try
            {
                string appData = HostingEnvironment.MapPath("~/App_Data");
                if (string.IsNullOrEmpty(appData) || !Directory.Exists(appData))
                {
                    return;
                }

                string archive = Path.Combine(appData, "Logs", "archive");
                Directory.CreateDirectory(archive);

                RotateIfTooLarge(Path.Combine(appData, "app-errors.log"), archive);
                RotateIfTooLarge(Path.Combine(appData, "delete-error.log"), archive);
                TrimArchives(archive);
                PurgeExpired(Path.Combine(appData, "Logs"));
            }
            catch
            {
                // Housekeeping must never break Application_Start.
            }
        }

        private static void RotateIfTooLarge(string path, string archiveFolder)
        {
            if (!File.Exists(path))
            {
                return;
            }

            FileInfo info = new FileInfo(path);
            if (info.Length < MaxRootLogBytes)
            {
                return;
            }

            string archived = Path.Combine(
                archiveFolder,
                Path.GetFileNameWithoutExtension(path) + "-" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".log");

            // Move may fail if the file is locked; try copy+truncate as a fallback.
            try
            {
                File.Move(path, archived);
            }
            catch (IOException)
            {
                try
                {
                    File.Copy(path, archived, true);
                    using (var stream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite))
                    {
                        stream.SetLength(0);
                    }
                }
                catch
                {
                    // Give up quietly; the log keeps growing until the next attempt.
                }
            }
        }

        private static void TrimArchives(string archiveFolder)
        {
            try
            {
                var groups = Directory.GetFiles(archiveFolder, "*.log")
                    .GroupBy(f => f.Split('-')[0]);

                foreach (var group in groups)
                {
                    var stale = group
                        .OrderByDescending(File.GetLastWriteTimeUtc)
                        .Skip(MaxArchivedFilesPerLog);

                    foreach (string file in stale)
                    {
                        TryDelete(file);
                    }
                }
            }
            catch
            {
            }
        }

        private static void PurgeExpired(string logsFolder)
        {
            if (!Directory.Exists(logsFolder))
            {
                return;
            }

            DateTime cutoff = DateTime.UtcNow.AddDays(-RetentionDays);
            foreach (string file in Directory.EnumerateFiles(logsFolder, "*", SearchOption.AllDirectories))
            {
                try
                {
                    if (File.GetLastWriteTimeUtc(file) < cutoff)
                    {
                        TryDelete(file);
                    }
                }
                catch
                {
                }
            }
        }

        private static void TryDelete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
            }
        }
    }
}
