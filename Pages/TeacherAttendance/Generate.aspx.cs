using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.TeacherAttendance
{
    public partial class Generate : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();
        private const string FlashKey = "TeacherAttendance_FlashMessage";

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                string dateParam = Request.QueryString["date"];
                if (!string.IsNullOrEmpty(dateParam))
                {
                    DateTime d;
                    if (DateTime.TryParse(dateParam, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                        txtDate.Text = d.ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrEmpty(txtDate.Text))
                    txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

                DateTime date;
                if (DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    LoadLessons(date);
            }
        }

        #endregion

        #region Generate

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            DateTime date;
            if (!DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                ShowMessage("التاريخ غير صحيح", false);
                return;
            }

            var result = new DailyLessonService(db).GenerateForDate(date);

            if (result.IsWeekend)
            {
                ShowMessage("هذا اليوم عطلة (الجمعة/السبت) — لا يتم توليد حصص", false);
                LoadLessons(date);
                return;
            }

            if (result.ScheduledEntries == 0)
            {
                ShowMessage("لا توجد حصص مقررة في الجدول المعتمد لهذا اليوم", false);
                LoadLessons(date);
                return;
            }

            string message = "تمت مزامنة الحصص: أضيفت " + result.Added
                + "، عُدّلت " + result.Updated
                + "، دون تغيير " + result.SkippedUnchanged;

            if (result.SkippedWithAttendance > 0)
                message += "، وحُفظت " + result.SkippedWithAttendance
                    + " حصة لأنها تحتوي على سجل حضور سابق";

            Session[FlashKey] = message + ".";
            Response.Redirect("Home.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        #endregion

        #region Load

        private void LoadLessons(DateTime date)
        {
            // ✅ فلترة حسب المرحلة الحالية
            var stage = EnumHelper.GetCurrentStage();

            var stageTeacherIds = db.Teachers
                .Where(t => t.Stage == stage)
                .Select(t => t.Id)
                .ToList();

            // ✅ إذا لا يوجد معلمون → جدول فارغ مباشرة
            if (stageTeacherIds.Count == 0)
            {
                gvLessons.DataSource = new List<object>();
                gvLessons.DataBind();
                pnlEmpty.Visible = true;
                return;
            }

            // ✅ بناء القائمة كاملة
            var lessons = db.DailyLessons
                .Where(d =>
                    d.Date == date &&
                    stageTeacherIds.Contains(d.TeacherId))
                .Select(d => new
                {
                    d.Id,
                    d.PeriodNumber,
                    TeacherName = d.Teacher.Name,
                    SubjectName = d.Subject.Name,
                    ClassName = d.ClassRoom.Name,
                    d.StartTime,
                    d.EndTime,
                    d.DurationMinutes
                })
                .ToList()
                .Select(d => new
                {
                    d.Id,
                    d.PeriodNumber,
                    d.TeacherName,
                    d.SubjectName,
                    d.ClassName,
                    StartTimeText = d.StartTime.ToString(@"hh\:mm"),
                    EndTimeText = d.EndTime.ToString(@"hh\:mm"),
                    d.DurationMinutes
                })
                .OrderBy(d => d.PeriodNumber)
                .ToList();

            gvLessons.DataSource = lessons;
            gvLessons.DataBind();

            // ✅ إظهار Empty State فقط عند عدم وجود بيانات
            pnlEmpty.Visible = lessons.Count == 0;
        }

        #endregion

        #region Messages

        private void ShowMessage(string text, bool success)
        {
            lblMessage.CssClass = success ? "ta-alert success" : "ta-alert danger";
            lblMessage.Text = "<i class='fas " + (success ? "fa-check-circle" : "fa-exclamation-triangle") + "'></i>"
                + "<span>" + Server.HtmlEncode(text) + "</span>";
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