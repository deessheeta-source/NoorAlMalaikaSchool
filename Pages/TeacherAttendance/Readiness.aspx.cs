using System;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.TeacherAttendance
{
    public partial class Readiness : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadPeriods();

                int periodId;
                if (int.TryParse(Request.QueryString["periodId"], out periodId))
                {
                    if (ddlPeriod.Items.FindByValue(periodId.ToString()) != null)
                        ddlPeriod.SelectedValue = periodId.ToString();
                }

                if (!string.IsNullOrEmpty(ddlPeriod.SelectedValue))
                    Check();
            }
        }

        #endregion

        #region Load

        private void LoadPeriods()
        {
            var periods = db.PayrollPeriods
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ToList();

            ddlPeriod.Items.Clear();
            foreach (var period in periods)
            {
                ddlPeriod.Items.Add(new System.Web.UI.WebControls.ListItem(
                    period.Year + "/" + period.Month + " — " + GetStatusName(period.Status),
                    period.Id.ToString()));
            }
        }

        #endregion

        #region Check

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            Check();
        }

        private void Check()
        {
            int periodId;
            if (!int.TryParse(ddlPeriod.SelectedValue, out periodId) || periodId <= 0)
            {
                ShowSummary("اختر فترة مالية أولاً.", false);
                return;
            }

            // الفحص المالي على مستوى المدرسة، والحضور المطلوب للحساب يخص معلمي الحصة فقط.
            var result = new TeacherAttendanceFinancialService(db)
                .ValidatePeriod(periodId, true, null);

            lblScheduled.Text = result.ScheduledLessons.ToString();
            lblRecorded.Text = result.RecordedLessons.ToString();
            lblApproved.Text = result.ApprovedLessons.ToString();
            lblMissing.Text = result.MissingLessons.ToString();
            lblUnapproved.Text = result.UnapprovedLessons.ToString();
            lblConflicts.Text = result.ConflictCount.ToString();

            var rows = result.Issues.Select(i => new
            {
                Severity = i.IsError ? "خطأ" : "تنبيه",
                DateText = i.Date.HasValue ? i.Date.Value.ToString("yyyy-MM-dd") : "—",
                i.TeacherName,
                PeriodNumber = i.PeriodNumber.HasValue ? i.PeriodNumber.Value.ToString() : "—",
                i.Message
            }).ToList();

            gvIssues.DataSource = rows;
            gvIssues.DataBind();

            // ✅ Empty State
            if (pnlEmpty != null)
                pnlEmpty.Visible = rows.Count == 0;

            ShowSummary("[كل المراحل] " + result.Summary, result.IsReady);
        }

        #endregion

        #region Messages

        private void ShowSummary(string text, bool success)
        {
            if (lblSummary == null) return;

            lblSummary.CssClass = success ? "ta-alert success" : "ta-alert warning";
            lblSummary.Text =
                "<i class='fas " + (success ? "fa-check-circle" : "fa-exclamation-triangle") + "'></i>" +
                "<span>" + Server.HtmlEncode(text) + "</span>";
        }

        #endregion

        #region Helpers

        private string GetStatusName(PayrollPeriodStatus status)
        {
            switch (status)
            {
                case PayrollPeriodStatus.Open: return "مفتوح";
                case PayrollPeriodStatus.UnderReview: return "قيد المراجعة";
                case PayrollPeriodStatus.Approved: return "معتمد";
                case PayrollPeriodStatus.Closed: return "مغلق";
                default: return "";
            }
        }

        #endregion

        #region Dispose

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }

        #endregion
    }
}