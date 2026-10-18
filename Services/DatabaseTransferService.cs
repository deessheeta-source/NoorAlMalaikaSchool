using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.SqlServer.Dac;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>
    /// SQL Server backup/restore and BACPAC transfer operations.
    /// Backup files are kept outside the public content tree by default.
    /// </summary>
    public static class DatabaseTransferService
    {
        private const int SqlCommandTimeoutSeconds = 0;
        private static readonly Regex SafeDatabaseName =
            new Regex(@"^[A-Za-z][A-Za-z0-9_]{0,127}$", RegexOptions.Compiled);

        public static string DatabaseName
        {
            get { return new SqlConnectionStringBuilder(ConnectionString).InitialCatalog; }
        }

        public static string BackupDirectory
        {
            get
            {
                string configured = ConfigurationManager.AppSettings["DatabaseBackupPath"];
                if (string.IsNullOrWhiteSpace(configured))
                    configured = "~/App_Data/DatabaseBackups";

                string path;
                if (configured.StartsWith("~", StringComparison.Ordinal))
                {
                    string relative = configured.TrimStart('~', '/', '\\')
                        .Replace('/', Path.DirectorySeparatorChar);
                    path = Path.Combine(HttpRuntime.AppDomainAppPath, relative);
                }
                else if (Path.IsPathRooted(configured))
                {
                    path = configured;
                }
                else
                {
                    path = Path.Combine(HttpRuntime.AppDomainAppPath, configured);
                }

                return Path.GetFullPath(path);
            }
        }

        public static IList<string> ListFiles(string extension)
        {
            if (extension != ".bak" && extension != ".bacpac")
                throw new ArgumentException("امتداد ملف غير مدعوم.", "extension");

            Directory.CreateDirectory(BackupDirectory);
            return Directory.GetFiles(BackupDirectory, "*" + extension, SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .OrderByDescending(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static string CreateBackup()
        {
            Directory.CreateDirectory(BackupDirectory);
            string fileName = SafeFileName(DatabaseName) + "_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "_" +
                Guid.NewGuid().ToString("N").Substring(0, 6) + ".bak";
            string path = GetManagedOutputPath(fileName, ".bak");
            string database = QuoteIdentifier(DatabaseName);

            try
            {
                using (var connection = new SqlConnection(ServerConnectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = SqlCommandTimeoutSeconds;
                    command.CommandText = "BACKUP DATABASE " + database +
                        " TO DISK = @backupPath WITH COPY_ONLY, CHECKSUM, STATS = 5;" +
                        " RESTORE VERIFYONLY FROM DISK = @backupPath;";
                    command.Parameters.Add("@backupPath", SqlDbType.NVarChar, 4000).Value = path;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                TryDelete(path);
                throw;
            }

            return fileName;
        }

        public static string RestoreBackup(string fileName)
        {
            string path = GetManagedFilePath(fileName, ".bak");
            ValidateFullBackup(path);
            string database = QuoteIdentifier(DatabaseName);

            using (var connection = new SqlConnection(ServerConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = SqlCommandTimeoutSeconds;
                    command.CommandText = "ALTER DATABASE " + database +
                        " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                    command.ExecuteNonQuery();
                }

                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = SqlCommandTimeoutSeconds;
                        command.CommandText = "RESTORE DATABASE " + database +
                            " FROM DISK = @backupPath WITH REPLACE, RECOVERY;";
                        command.Parameters.Add("@backupPath", SqlDbType.NVarChar, 4000).Value = path;
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = SqlCommandTimeoutSeconds;
                        command.CommandText = "ALTER DATABASE " + database + " SET MULTI_USER;";
                        command.ExecuteNonQuery();
                    }
                }
            }

            return path;
        }

        public static string ExportBacpac()
        {
            Directory.CreateDirectory(BackupDirectory);
            string fileName = SafeFileName(DatabaseName) + "_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "_" +
                Guid.NewGuid().ToString("N").Substring(0, 6) + ".bacpac";
            string path = GetManagedOutputPath(fileName, ".bacpac");

            try
            {
                var services = new DacServices(ServerConnectionString);
                services.ExportBacpac(path, DatabaseName, new DacExportOptions(), null);
            }
            catch
            {
                TryDelete(path);
                throw;
            }
            return fileName;
        }

        public static string ImportBacpac(string fileName, string targetDatabaseName)
        {
            string path = GetManagedFilePath(fileName, ".bacpac");
            string target = (targetDatabaseName ?? string.Empty).Trim();
            if (!SafeDatabaseName.IsMatch(target))
                throw new InvalidOperationException("اسم قاعدة البيانات الجديدة غير صالح. استخدم أحرفاً إنجليزية وأرقاماً وشرطة سفلية، وابدأ بحرف.");

            if (string.Equals(target, DatabaseName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("استيراد BACPAC ينشئ قاعدة جديدة ولا يستبدل قاعدة التطبيق الحالية.");

            using (var connection = new SqlConnection(ServerConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT DB_ID(@databaseName);";
                command.Parameters.Add("@databaseName", SqlDbType.NVarChar, 128).Value = target;
                connection.Open();
                if (command.ExecuteScalar() != DBNull.Value)
                    throw new InvalidOperationException("قاعدة البيانات " + target + " موجودة بالفعل. اختر اسماً جديداً حتى لا تُستبدل أي قاعدة.");
            }

            var services = new DacServices(ServerConnectionString);
            using (var stream = File.OpenRead(path))
            using (var package = BacPackage.Load(stream))
            {
                services.ImportBacpac(package, target, new DacImportOptions());
            }

            return target;
        }

        public static string GetManagedFilePath(string fileName, string expectedExtension)
        {
            string path = ResolveManagedPath(fileName, expectedExtension);
            if (!File.Exists(path))
                throw new FileNotFoundException("الملف المحدد غير موجود في مجلد النسخ الاحتياطية.");

            return path;
        }

        private static string GetManagedOutputPath(string fileName, string expectedExtension)
        {
            string path = ResolveManagedPath(fileName, expectedExtension);
            if (File.Exists(path))
                throw new IOException("يوجد ملف بالاسم نفسه في مجلد النسخ الاحتياطية.");

            return path;
        }

        private static string ResolveManagedPath(string fileName, string expectedExtension)
        {
            if (string.IsNullOrWhiteSpace(fileName) ||
                !string.Equals(Path.GetFileName(fileName), fileName, StringComparison.Ordinal) ||
                !string.Equals(Path.GetExtension(fileName), expectedExtension, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("اسم الملف المحدد غير صالح.");

            string root = BackupDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            string path = Path.GetFullPath(Path.Combine(root, fileName));
            if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("مسار الملف خارج مجلد النسخ الاحتياطية.");

            return path;
        }

        private static void ValidateFullBackup(string path)
        {
            using (var connection = new SqlConnection(ServerConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = SqlCommandTimeoutSeconds;
                    command.CommandText = "RESTORE HEADERONLY FROM DISK = @backupPath;";
                    command.Parameters.Add("@backupPath", SqlDbType.NVarChar, 4000).Value = path;
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new InvalidOperationException("تعذر قراءة بيانات النسخة الاحتياطية.");

                        int databaseOrdinal = reader.GetOrdinal("DatabaseName");
                        int typeOrdinal = reader.GetOrdinal("BackupType");
                        string sourceDatabase = reader.IsDBNull(databaseOrdinal)
                            ? string.Empty
                            : reader.GetString(databaseOrdinal);
                        int backupType = Convert.ToInt32(reader.GetValue(typeOrdinal));

                        if (!string.Equals(sourceDatabase, DatabaseName, StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException("هذه النسخة تخص قاعدة " + sourceDatabase + " وليست قاعدة التطبيق الحالية " + DatabaseName + ". استخدم BACPAC لنقل القاعدة إلى بيئة أو اسم مختلف.");
                        if (backupType != 1)
                            throw new InvalidOperationException("الاستعادة تقبل النسخ الكاملة فقط.");
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = SqlCommandTimeoutSeconds;
                    command.CommandText = "RESTORE VERIFYONLY FROM DISK = @backupPath;";
                    command.Parameters.Add("@backupPath", SqlDbType.NVarChar, 4000).Value = path;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static string ConnectionString
        {
            get
            {
                var setting = ConfigurationManager.ConnectionStrings["DefaultConnection"];
                if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                    throw new ConfigurationErrorsException("إعداد اتصال قاعدة البيانات DefaultConnection غير موجود.");
                return setting.ConnectionString;
            }
        }

        private static string ServerConnectionString
        {
            get
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString)
                {
                    InitialCatalog = "master",
                    ApplicationName = "NoorAlMalaikaSchool Database Maintenance"
                };
                return builder.ConnectionString;
            }
        }

        private static string QuoteIdentifier(string value)
        {
            return "[" + (value ?? string.Empty).Replace("]", "]]") + "]";
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
                // A failed cleanup must not hide the database operation error.
            }
        }

        private static string SafeFileName(string value)
        {
            foreach (char invalid in Path.GetInvalidFileNameChars())
                value = value.Replace(invalid, '_');
            return value.Replace("\\", "_").Replace("/", "_");
        }
    }
}
