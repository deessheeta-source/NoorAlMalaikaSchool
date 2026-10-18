using System;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Payroll
{
    public partial class Create : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                int currentYear = DateTime.Now.Year;
                for (int y = currentYear - 1; y <= currentYear + 1; y++)
                {
                    ddlYear.Items.Add(new System.Web.UI.WebControls.ListItem(y.ToString(), y.ToString()));
                }
                ddlYear.SelectedValue = currentYear.ToString();
                ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
            }
        }

        #endregion

        #region Create

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            int year;
            int month;
            if (!int.TryParse(ddlYear.SelectedValue, out year) ||
                !int.TryParse(ddlMonth.SelectedValue, out month) ||
                month < 1 || month > 12 ||
                year < 2000 || year > 2100)
            {
                ShowMessage("السنة أو الشهر غير صحيح.", false);
                return;
            }

            bool exists = db.PayrollPeriods.Any(p => p.Year == year && p.Month == month);
            if (exists)
            {
                ShowMessage("توجد فترة مالية لهذا الشهر مسبقاً.", false);
                return;
            }

            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var period = new PayrollPeriod
            {
                Year = year,
                Month = month,
                StartDate = start,
                EndDate = end,
                Status = PayrollPeriodStatus.Open,
                OpenedAt = DateTime.Now,
                OpenedBy = EnumHelper.CurrentUserName
            };

            db.PayrollPeriods.Add(period);
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                ShowMessage("تم إنشاء فترة الرواتب مسبقاً من جلسة أخرى.", false);
                return;
            }

            // عند فتح الفترة يتم تجهيز DailyLessons للشهر تلقائياً من الجدول المعتمد.
            // هذا يمنع ظهور الفترة وكأنها ناقصة فقط لأن الحصص اليومية لم تُولّد بعد.
            var generation = new DailyLessonService(db).GenerateForRange(start, end);

            ShowMessage(
                string.Format(
                    "تم فتح الفترة: <strong>{0} {1}</strong> ({2:yyyy-MM-dd} إلى {3:yyyy-MM-dd}). " +
                    "تم تجهيز الحصص اليومية تلقائياً: <strong>{4}</strong> مضافة، " +
                    "<strong>{5}</strong> محدثة، <strong>{6}</strong> دون تغيير.",
                    ArabicMonthName(month), year, start, end,
                    generation.Added, generation.Updated, generation.SkippedUnchanged),
                true);

            btnCreate.Enabled = false;
            btnCreate.Visible = false;
            ddlYear.Enabled = false;
            ddlMonth.Enabled = false;
        }

        #endregion

        #region Helpers

        private string ArabicMonthName(int month)
        {
            string[] months = { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
                                "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };
            return months[month - 1];
        }

        private void ShowMessage(string text, bool success)
        {
            if (lblInfo == null) return;

            lblInfo.CssClass = success ? "payroll-alert success" : "payroll-alert danger";
            lblInfo.Text =
                "<i class='fas " + (success ? "fa-check-circle" : "fa-exclamation-triangle") + "'></i>" +
                "<span>" + text + "</span>";   // نص HTML آمن — يتضمن <strong> فقط
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }

        #endregion
    }
}