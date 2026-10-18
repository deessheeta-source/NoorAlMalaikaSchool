using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Database
{
    public partial class BackupRestore : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnless(UserRole.Admin);
            SecurityHelper.DenyUnlessPath();
            Server.ScriptTimeout = 3600;

            litDatabaseName.Text = HttpUtility.HtmlEncode(DatabaseTransferService.DatabaseName);
            litBackupPath.Text = HttpUtility.HtmlEncode(DatabaseTransferService.BackupDirectory);

            if (!IsPostBack)
            {
                txtTargetDatabaseName.Text = DatabaseTransferService.DatabaseName + "_Imported";
                BindFiles();
            }
        }

        protected void btnCreateBackup_Click(object sender, EventArgs e)
        {
            RunOperation(delegate
            {
                string fileName = DatabaseTransferService.CreateBackup();
                ShowMessage("تم إنشاء النسخة والتحقق منها: " + fileName, true);
            });
        }

        protected void btnExportBacpac_Click(object sender, EventArgs e)
        {
            RunOperation(delegate
            {
                string fileName = DatabaseTransferService.ExportBacpac();
                ShowMessage("تم تصدير قاعدة البيانات إلى: " + fileName, true);
            });
        }

        protected void btnRestoreBackup_Click(object sender, EventArgs e)
        {
            RunOperation(delegate
            {
                string databaseName = DatabaseTransferService.DatabaseName;
                if (!string.Equals(txtRestoreConfirm.Text.Trim(), databaseName, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("اكتب اسم قاعدة البيانات الحالية كما يظهر أعلى الصفحة للتأكيد.");
                if (string.IsNullOrWhiteSpace(ddlBackup.SelectedValue))
                    throw new InvalidOperationException("اختر ملف نسخة احتياطية أولاً.");

                string safetyCopy = DatabaseTransferService.CreateBackup();
                DatabaseTransferService.RestoreBackup(ddlBackup.SelectedValue);
                txtRestoreConfirm.Text = string.Empty;
                ShowMessage("اكتملت الاستعادة. تم إنشاء نسخة أمان قبلها باسم " + safetyCopy + ".", true);
            });
        }

        protected void btnImportBacpac_Click(object sender, EventArgs e)
        {
            RunOperation(delegate
            {
                if (string.IsNullOrWhiteSpace(ddlBacpac.SelectedValue))
                    throw new InvalidOperationException("اختر ملف BACPAC أولاً.");

                string databaseName = DatabaseTransferService.ImportBacpac(
                    ddlBacpac.SelectedValue,
                    txtTargetDatabaseName.Text);
                ShowMessage("تم استيراد الملف كقاعدة جديدة باسم " + databaseName + ". لم يتغير اتصال التطبيق الحالي.", true);
            });
        }

        protected void btnDownloadBackup_Click(object sender, EventArgs e)
        {
            DownloadSelected(ddlBackup.SelectedValue, ".bak");
        }

        protected void btnDownloadBacpac_Click(object sender, EventArgs e)
        {
            DownloadSelected(ddlBacpac.SelectedValue, ".bacpac");
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                BindFiles();
                ShowMessage("تم تحديث قائمة ملفات النسخ والنقل.", true);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, false);
            }
        }

        private void RunOperation(Action operation)
        {
            try
            {
                operation();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, false);
            }
            finally
            {
                try { BindFiles(); }
                catch (Exception ex) { ShowMessage(ex.Message, false); }
            }
        }

        private void BindFiles()
        {
            BindFileList(ddlBackup, ".bak", "-- اختر نسخة .bak --");
            BindFileList(ddlBacpac, ".bacpac", "-- اختر ملف .bacpac --");
        }

        private static void BindFileList(DropDownList list, string extension, string emptyText)
        {
            string selected = list.SelectedValue;
            list.Items.Clear();
            list.Items.Add(new ListItem(emptyText, string.Empty));
            foreach (string fileName in DatabaseTransferService.ListFiles(extension))
                list.Items.Add(new ListItem(fileName, fileName));

            if (!string.IsNullOrEmpty(selected) && list.Items.FindByValue(selected) != null)
                list.SelectedValue = selected;
        }

        private void DownloadSelected(string fileName, string extension)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new InvalidOperationException("اختر ملفاً أولاً.");

                string path = DatabaseTransferService.GetManagedFilePath(fileName, extension);
                Response.Clear();
                Response.BufferOutput = false;
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Length", new FileInfo(path).Length.ToString());
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" +
                    Path.GetFileName(path).Replace("\"", string.Empty) + "\"; filename*=UTF-8''" +
                    Uri.EscapeDataString(Path.GetFileName(path)));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.TransmitFile(path);
                Response.End();
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, false);
            }
        }

        private void ShowMessage(string message, bool success)
        {
            litMessage.CssClass = success
                ? "alert alert-success d-block mb-3"
                : "alert alert-danger d-block mb-3";
            litMessage.Text = HttpUtility.HtmlEncode(message ?? string.Empty);
        }
    }
}
