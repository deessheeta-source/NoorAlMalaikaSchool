using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;
using AuditLogEntity = NoorAlMalaikaSchool.Models.AuditLog;

namespace NoorAlMalaikaSchool.Pages.Security
{
    public partial class AuditLog : Page
    {
        private const int MaxRows = 1000;
        private readonly SchoolDbContext db = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnless(UserRole.Admin);

            if (!IsPostBack)
            {
                LoadUsers();
                SetDefaultDateRange();
                BindLog();
            }
        }

        private void LoadUsers()
        {
            ddlUser.Items.Clear();
            ddlUser.Items.Add(new ListItem("كل المستخدمين", string.Empty));

            // Read names from the audit history so deleted users and failed-login
            // usernames remain searchable as well as current display names.
            List<string> users = db.AuditLogs.AsNoTracking()
                .Where(log => log.UserName != null && log.UserName != string.Empty)
                .Select(log => log.UserName)
                .Distinct()
                .OrderBy(name => name)
                .ToList();

            foreach (string user in users)
                ddlUser.Items.Add(new ListItem(user, user));
        }

        private void SetDefaultDateRange()
        {
            txtTo.Text = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            txtFrom.Text = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            gvLog.PageIndex = 0;
            BindLog();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            SetDefaultDateRange();
            txtEntity.Text = string.Empty;
            txtSearch.Text = string.Empty;
            if (ddlUser.Items.Count > 0)
                ddlUser.SelectedIndex = 0;
            if (ddlAction.Items.Count > 0)
                ddlAction.SelectedIndex = 0;

            gvLog.PageIndex = 0;
            BindLog();
        }

        private void BindLog()
        {
            IQueryable<AuditLogEntity> query;
            if (!TryBuildFilteredQuery(out query))
                return;

            int matchedCount = query.Count();
            List<AuditLogEntity> rows = query
                .OrderByDescending(log => log.Timestamp)
                .Take(MaxRows)
                .ToList();

            foreach (AuditLogEntity row in rows)
                row.TimestampText = row.Timestamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            UpdateSummary(rows, matchedCount, query);

            int pageCount = Math.Max(1, (int)Math.Ceiling(rows.Count / (double)gvLog.PageSize));
            if (gvLog.PageIndex >= pageCount)
                gvLog.PageIndex = pageCount - 1;

            gvLog.DataSource = rows;
            gvLog.DataBind();

            List<AuditLogEntity> currentPage = rows
                .Skip(gvLog.PageIndex * gvLog.PageSize)
                .Take(gvLog.PageSize)
                .ToList();
            rptLogMobile.DataSource = currentPage;
            rptLogMobile.DataBind();

            pnlMobileEmpty.Visible = rows.Count == 0;
            pnlMobilePager.Visible = pageCount > 1;
            pnlSelectAllResults.Visible = matchedCount > gvLog.PageSize;
            litFilteredSelectionCount.Text = matchedCount.ToString("N0", CultureInfo.CurrentCulture);
            hfFilteredCount.Value = matchedCount.ToString(CultureInfo.InvariantCulture);
            btnMobilePrevious.Enabled = gvLog.PageIndex > 0;
            btnMobileNext.Enabled = gvLog.PageIndex < pageCount - 1;
            litMobilePageInfo.Text = "صفحة " + (gvLog.PageIndex + 1).ToString(CultureInfo.InvariantCulture) +
                " من " + pageCount.ToString(CultureInfo.InvariantCulture);
        }

        private bool TryBuildFilteredQuery(out IQueryable<AuditLogEntity> query)
        {
            return TryBuildFilteredQuery(out query, true);
        }

        private bool TryBuildFilteredQuery(out IQueryable<AuditLogEntity> query, bool noTracking)
        {
            pnlFilterMessage.Visible = false;
            query = noTracking ? db.AuditLogs.AsNoTracking() : db.AuditLogs;

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MinValue;
            bool hasFrom = !string.IsNullOrWhiteSpace(txtFrom.Text);
            bool hasTo = !string.IsNullOrWhiteSpace(txtTo.Text);

            if (hasFrom && !TryParseDate(txtFrom.Text, out fromDate))
            {
                ShowFilterMessage("تاريخ البداية غير صالح.");
                return false;
            }

            if (hasTo && !TryParseDate(txtTo.Text, out toDate))
            {
                ShowFilterMessage("تاريخ النهاية غير صالح.");
                return false;
            }

            if (hasFrom && hasTo && fromDate.Date > toDate.Date)
            {
                ShowFilterMessage("تاريخ البداية يجب ألا يتجاوز تاريخ النهاية.");
                return false;
            }

            if (hasFrom)
                query = query.Where(log => log.Timestamp >= fromDate);

            if (hasTo)
            {
                DateTime endExclusive = toDate.Date.AddDays(1);
                query = query.Where(log => log.Timestamp < endExclusive);
            }

            string selectedUser = ddlUser.SelectedValue;
            if (!string.IsNullOrWhiteSpace(selectedUser))
                query = query.Where(log => log.UserName == selectedUser);

            string selectedAction = ddlAction.SelectedValue;
            if (!string.IsNullOrWhiteSpace(selectedAction))
                query = query.Where(log => log.Action == selectedAction);

            string entity = txtEntity.Text.Trim();
            if (!string.IsNullOrWhiteSpace(entity))
                query = query.Where(log => log.EntityType.Contains(entity));

            string search = txtSearch.Text.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(log =>
                    log.UserName.Contains(search) ||
                    log.EntityType.Contains(search) ||
                    log.EntityId.Contains(search) ||
                    log.Details.Contains(search) ||
                    log.ChangedFields.Contains(search) ||
                    log.IpAddress.Contains(search));
            }

            return true;
        }

        private bool TryGetSelectedLogs(out List<AuditLogEntity> selectedLogs, out bool selectedAllResults)
        {
            selectedLogs = new List<AuditLogEntity>();
            selectedAllResults = string.Equals(Request.Form["auditSelectAll"], "1", StringComparison.Ordinal);
            pnlActionMessage.Visible = false;

            if (selectedAllResults)
            {
                IQueryable<AuditLogEntity> allFilteredLogs;
                if (!TryBuildFilteredQuery(out allFilteredLogs, false))
                    return false;

                selectedLogs = allFilteredLogs
                    .OrderByDescending(log => log.Timestamp)
                    .ToList();

                if (selectedLogs.Count == 0)
                {
                    ShowActionMessage("لا توجد سجلات مطابقة للفلاتر الحالية.", false);
                    return false;
                }

                return true;
            }

            string[] postedValues = Request.Form.GetValues("auditSelect");
            if (postedValues == null || postedValues.Length == 0)
            {
                ShowActionMessage("حدد سجلًا واحدًا على الأقل من الصفحة الحالية.", false);
                return false;
            }

            List<int> selectedIds = new List<int>();
            foreach (string value in postedValues)
            {
                int id;
                if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out id) || id <= 0)
                {
                    ShowActionMessage("تعذر التحقق من الاختيار. حدّث الصفحة ثم أعد المحاولة.", false);
                    return false;
                }

                if (!selectedIds.Contains(id))
                    selectedIds.Add(id);
            }

            if (selectedIds.Count > gvLog.PageSize)
            {
                ShowActionMessage("يمكن تنفيذ العملية على سجلات الصفحة الحالية فقط.", false);
                return false;
            }

            IQueryable<AuditLogEntity> filteredQuery;
            if (!TryBuildFilteredQuery(out filteredQuery))
                return false;

            List<int> currentPageIds = filteredQuery
                .OrderByDescending(log => log.Timestamp)
                .Take(MaxRows)
                .Skip(gvLog.PageIndex * gvLog.PageSize)
                .Take(gvLog.PageSize)
                .Select(log => log.Id)
                .ToList();

            if (selectedIds.Any(id => !currentPageIds.Contains(id)))
            {
                ShowActionMessage("بعض السجلات المحددة لم تعد ضمن الصفحة الحالية. حدّث النتائج ثم أعد الاختيار.", false);
                return false;
            }

            selectedLogs = db.AuditLogs
                .Where(log => selectedIds.Contains(log.Id))
                .OrderByDescending(log => log.Timestamp)
                .ToList();

            if (selectedLogs.Count != selectedIds.Count)
            {
                ShowActionMessage("تعذر العثور على جميع السجلات المحددة. حدّث الصفحة ثم أعد المحاولة.", false);
                selectedLogs.Clear();
                return false;
            }

            return true;
        }

        protected void btnBackupSelected_Click(object sender, EventArgs e)
        {
            List<AuditLogEntity> selectedLogs;
            bool selectedAllResults;
            if (!TryGetSelectedLogs(out selectedLogs, out selectedAllResults))
                return;

            var backup = new
            {
                format = "NoorAlMalaikaSchool.AuditLogBackup",
                version = 1,
                exportedAt = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                exportedBy = GetCurrentAdminName(),
                selectionMode = selectedAllResults ? "allFilteredResults" : "manuallySelectedRecords",
                records = selectedLogs.Select(log => new
                {
                    log.Id,
                    timestamp = log.Timestamp.ToString("o", CultureInfo.InvariantCulture),
                    log.UserName,
                    log.Action,
                    log.EntityType,
                    log.EntityId,
                    log.Details,
                    log.ChangedFields,
                    log.OldValues,
                    log.NewValues,
                    log.IpAddress
                }).ToList()
            };

            try
            {
                var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
                string json = serializer.Serialize(backup);
                string fileName = "audit-log-backup-" + DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture) + ".json";

                Response.Clear();
                Response.ContentType = "application/json";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Charset = "utf-8";
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
                Response.AddHeader("X-Content-Type-Options", "nosniff");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(json);
                Response.End();
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ShowActionMessage("تعذر إنشاء النسخة: " + ex.Message, false);
            }
        }

        protected void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            List<AuditLogEntity> selectedLogs;
            bool selectedAllResults;
            if (!TryGetSelectedLogs(out selectedLogs, out selectedAllResults))
                return;

            List<int> deletedIds = selectedLogs.Select(log => log.Id).ToList();
            string deletedIdsText = selectedAllResults
                ? "جميع النتائج المطابقة للتصفية"
                : string.Join(",", deletedIds.Select(id => id.ToString(CultureInfo.InvariantCulture)));
            string adminName = GetCurrentAdminName();
            if (adminName.Length > 100)
                adminName = adminName.Substring(0, 100);

            db.AuditLogs.RemoveRange(selectedLogs);
            AuditLogEntity deletionSummary = new AuditLogEntity
            {
                Timestamp = DateTime.Now,
                UserName = adminName,
                Action = "حذف",
                EntityType = "AuditLog",
                EntityId = "Bulk",
                Details = "حذف مدير النظام " + deletedIds.Count.ToString(CultureInfo.InvariantCulture) + " سجلًا من سجل التدقيق" +
                    (selectedAllResults ? " (جميع النتائج المطابقة للتصفية)." : "."),
                ChangedFields = "عدد السجلات: " + deletedIds.Count.ToString(CultureInfo.InvariantCulture) + "; المعرفات: " + deletedIdsText,
                IpAddress = Request.UserHostAddress ?? string.Empty
            };
            db.AuditLogs.Add(deletionSummary);

            try
            {
                db.SaveChanges();
                ShowActionMessage("تم حذف " + deletedIds.Count.ToString("N0", CultureInfo.CurrentCulture) + " سجل. تم الاحتفاظ بسجل لعملية الحذف.", true);
                BindLog();
            }
            catch (Exception)
            {
                db.Entry(deletionSummary).State = System.Data.Entity.EntityState.Detached;
                foreach (AuditLogEntity log in selectedLogs)
                    db.Entry(log).State = System.Data.Entity.EntityState.Unchanged;
                ShowActionMessage("تعذر حذف السجلات. لم يتم إكمال العملية.", false);
            }
        }

        private string GetCurrentAdminName()
        {
            if (Session != null)
            {
                string displayName = Session["DisplayName"] as string;
                if (!string.IsNullOrWhiteSpace(displayName))
                    return displayName;

                string username = Session["Username"] as string;
                if (!string.IsNullOrWhiteSpace(username))
                    return username;
            }

            return User != null && User.Identity != null && User.Identity.IsAuthenticated
                ? User.Identity.Name
                : "مدير النظام";
        }

        private void ShowActionMessage(string message, bool success)
        {
            litActionMessage.Text = message;
            pnlActionMessage.CssClass = success
                ? "audit-action-message audit-action-message-success"
                : "audit-action-message audit-action-message-error";
            pnlActionMessage.Visible = true;
        }

        private void UpdateSummary(IList<AuditLogEntity> rows, int matchedCount, IQueryable<AuditLogEntity> filteredQuery)
        {
            int deletes = filteredQuery.Count(log => log.Action == "حذف");
            int failedLogins = filteredQuery.Count(log => log.Action == "محاولة دخول فاشلة");

            litResultCount.Text = matchedCount.ToString("N0", CultureInfo.CurrentCulture);
            litResultCountLabel.Text = matchedCount.ToString("N0", CultureInfo.CurrentCulture) + " سجل مطابق";
            litDeleteCount.Text = deletes.ToString("N0", CultureInfo.CurrentCulture);
            litFailedLoginCount.Text = failedLogins.ToString("N0", CultureInfo.CurrentCulture);
            litLatestActivity.Text = rows.Count == 0 ? "—" : rows[0].TimestampText;
        }

        private static bool TryParseDate(string value, out DateTime result)
        {
            return DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result);
        }

        private void ShowFilterMessage(string message)
        {
            litFilterMessage.Text = message;
            pnlFilterMessage.Visible = true;
        }

        protected string GetActionClass(object value)
        {
            string action = Convert.ToString(value);
            switch (action)
            {
                case "إنشاء": return "audit-action-create";
                case "تعديل": return "audit-action-edit";
                case "حذف": return "audit-action-delete";
                case "تسجيل دخول": return "audit-action-login";
                case "محاولة دخول فاشلة": return "audit-action-failed";
                case "تغيير كلمة المرور": return "audit-action-password";
                default: return "audit-action-default";
            }
        }

        protected bool HasChangedFields(object value)
        {
            return !string.IsNullOrWhiteSpace(Convert.ToString(value));
        }

        protected bool HasAuditValues(object oldValues, object newValues)
        {
            return !string.IsNullOrWhiteSpace(Convert.ToString(oldValues)) ||
                !string.IsNullOrWhiteSpace(Convert.ToString(newValues));
        }

        protected void gvLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLog.PageIndex = e.NewPageIndex;
            BindLog();
        }

        protected void btnMobilePrevious_Click(object sender, EventArgs e)
        {
            gvLog.PageIndex = Math.Max(0, gvLog.PageIndex - 1);
            BindLog();
        }

        protected void btnMobileNext_Click(object sender, EventArgs e)
        {
            gvLog.PageIndex += 1;
            BindLog();
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null)
                db.Dispose();
            base.OnUnload(e);
        }
    }
}
