using System;
using System.Globalization;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.TeacherAttendance
{
    public partial class Record : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();
        private const string FlashKey = "TeacherAttendance_FlashMessage";

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                ApplyDateQueryString();
                LoadTeachers();
                ApplyTeacherQueryString();

                DateTime date;
                if (DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    LoadLessons(date);
            }
        }

        #endregion

        #region Query String

        private void ApplyDateQueryString()
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
        }

        private void ApplyTeacherQueryString()
        {
            int teacherId;
            if (!int.TryParse(Request.QueryString["teacherId"], out teacherId) || teacherId <= 0)
                return;

            if (ddlTeacher.Items.FindByValue(teacherId.ToString()) != null)
                ddlTeacher.SelectedValue = teacherId.ToString();
        }

        #endregion

        #region Load Data

        private void LoadTeachers()
        {
            // ✅ المرحلة الحالية فقط
            var stage = EnumHelper.GetCurrentStage();

            ddlTeacher.Items.Clear();
            ddlTeacher.Items.Add(new System.Web.UI.WebControls.ListItem("جميع المعلمين", ""));

            var teachers = db.Teachers
                .Where(t => t.Stage == stage && t.IsActive)
                .OrderBy(t => t.Name)
                .Select(t => new { t.Id, t.Name })
                .ToList();

            foreach (var t in teachers)
            {
                ddlTeacher.Items.Add(new System.Web.UI.WebControls.ListItem(t.Name, t.Id.ToString()));
            }
        }

        private void LoadLessons(DateTime date)
        {
            // ✅ فلترة حسب المرحلة الحالية
            var stage = EnumHelper.GetCurrentStage();

            var stageTeacherIds = db.Teachers
                .Where(t => t.Stage == stage)
                .Select(t => t.Id)
                .ToList();

            if (stageTeacherIds.Count == 0)
            {
                gvLessons.DataSource = null;
                gvLessons.DataBind();
                pnlEmpty.Visible = true;
                return;
            }

            var query = db.DailyLessons
                .Where(d => d.Date == date && stageTeacherIds.Contains(d.TeacherId));

            int teacherId;
            if (int.TryParse(ddlTeacher.SelectedValue, out teacherId) && teacherId > 0)
                query = query.Where(d => d.TeacherId == teacherId);

            var lessons = query
                .Select(d => new
                {
                    d.Id,
                    d.PeriodNumber,
                    TeacherName = d.Teacher.Name,
                    SubjectName = d.Subject.Name,
                    ClassName = d.ClassRoom.Name,
                    d.StartTime,
                    d.EndTime,
                    d.DurationMinutes,
                    d.TeacherId,
                    d.Status
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
                    d.DurationMinutes,
                    d.TeacherId,
                    d.Status
                })
                .OrderBy(d => d.PeriodNumber)
                .ToList();

            gvLessons.DataSource = lessons;
            gvLessons.DataBind();

            // ✅ إظهار Empty State عند عدم وجود بيانات
            pnlEmpty.Visible = lessons.Count == 0;
        }

        #endregion

        #region GridView Row Binding

        protected void gvLessons_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType != System.Web.UI.WebControls.DataControlRowType.DataRow) return;

            int lessonId = Convert.ToInt32(gvLessons.DataKeys[e.Row.RowIndex].Value);
            var lesson = db.DailyLessons.Find(lessonId);
            if (lesson == null) return;

            var ddlStatus = (System.Web.UI.WebControls.DropDownList)e.Row.FindControl("ddlStatus");
            var txtMinutes = (System.Web.UI.WebControls.TextBox)e.Row.FindControl("txtMinutes");
            var txtNotes = (System.Web.UI.WebControls.TextBox)e.Row.FindControl("txtNotes");

            if (ddlStatus != null)
            {
                ddlStatus.CssClass = "form-select form-select-sm status-dropdown";
                ddlStatus.Attributes["onchange"] = "syncMinutesWithStatus(this);";
            }

            if (txtMinutes != null)
            {
                txtMinutes.CssClass = "form-control form-control-sm text-center minutes-field";
                txtMinutes.Attributes["data-duration"] = lesson.DurationMinutes.ToString();
                txtMinutes.Attributes["min"] = "0";
                txtMinutes.Attributes["max"] = lesson.DurationMinutes.ToString();
            }

            var existing = db.AttendanceRecords
                .FirstOrDefault(r => r.DailyLessonId == lessonId && r.TeacherId == lesson.TeacherId);

            if (existing == null)
            {
                if (ddlStatus != null)
                    ddlStatus.SelectedValue = AttendanceStatus.PresentFull.ToString();
                if (txtMinutes != null)
                    txtMinutes.Text = lesson.DurationMinutes.ToString();
                return;
            }

            if (ddlStatus != null)
                ddlStatus.SelectedValue = existing.Status.ToString();
            if (txtMinutes != null)
                txtMinutes.Text = existing.ActualMinutes.ToString();
            if (txtNotes != null)
                txtNotes.Text = existing.Notes ?? "";

            if (existing.IsApproved)
            {
                if (ddlStatus != null) ddlStatus.Enabled = false;
                if (txtMinutes != null) txtMinutes.Enabled = false;
                if (txtNotes != null) txtNotes.Enabled = false;
                e.Row.CssClass = "table-row-approved";
            }
        }

        #endregion

        #region Event Handlers

        protected void ddlTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime date;
            if (DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                LoadLessons(date);
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            DateTime date;
            if (DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                LoadLessons(date);
            else
                ShowMessage("التاريخ غير صحيح", false);
        }

        protected void btnGenerateDay_Click(object sender, EventArgs e)
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

            ShowMessage(message + ".", true);
            LoadLessons(date);
        }

        #endregion

        #region Reset

        protected void btnReset_Click(object sender, EventArgs e)
        {
            int resetCount = 0;
            int skippedApproved = 0;

            for (int i = 0; i < gvLessons.Rows.Count; i++)
            {
                var row = gvLessons.Rows[i];
                if (i >= gvLessons.DataKeys.Count) continue;

                int lessonId;
                if (!int.TryParse(Convert.ToString(gvLessons.DataKeys[i].Value), out lessonId))
                    continue;

                var lesson = db.DailyLessons.Find(lessonId);
                if (lesson == null) continue;

                var existing = db.AttendanceRecords
                    .FirstOrDefault(r => r.DailyLessonId == lessonId && r.TeacherId == lesson.TeacherId);

                if (existing != null && existing.IsApproved)
                {
                    skippedApproved++;
                    continue;
                }

                var ddlStatus = (System.Web.UI.WebControls.DropDownList)row.FindControl("ddlStatus");
                var txtMinutes = (System.Web.UI.WebControls.TextBox)row.FindControl("txtMinutes");
                var txtNotes = (System.Web.UI.WebControls.TextBox)row.FindControl("txtNotes");

                if (ddlStatus != null)
                {
                    ddlStatus.SelectedValue = AttendanceStatus.PresentFull.ToString();
                    ddlStatus.Enabled = true;
                }

                if (txtMinutes != null)
                {
                    txtMinutes.Text = lesson.DurationMinutes.ToString();
                    txtMinutes.Enabled = true;
                    txtMinutes.ReadOnly = false;
                    txtMinutes.CssClass = "form-control form-control-sm text-center minutes-field";
                }

                if (txtNotes != null)
                {
                    txtNotes.Text = "";
                    txtNotes.Enabled = true;
                }

                if (!string.IsNullOrEmpty(row.CssClass))
                    row.CssClass = row.CssClass.Replace("table-row-approved", "").Trim();

                resetCount++;
            }

            ClientScript.RegisterStartupScript(
                GetType(),
                "resetSync_" + Guid.NewGuid().ToString("N"),
                "setTimeout(function(){" +
                "  var ddlList = document.querySelectorAll('.status-dropdown');" +
                "  for (var i = 0; i < ddlList.length; i++) {" +
                "    syncMinutesWithStatus(ddlList[i]);" +
                "  }" +
                "}, 100);",
                true);

            string msg;
            if (resetCount > 0 && skippedApproved > 0)
                msg = "تمت إعادة تعيين " + resetCount + " صف — تم تخطّي " + skippedApproved + " سجل معتمد.";
            else if (resetCount > 0)
                msg = "تمت إعادة تعيين " + resetCount + " صف إلى الحالة الافتراضية (حضور كامل).";
            else
                msg = "لا توجد صفوف قابلة لإعادة التعيين (كلها معتمدة مالياً).";

            ShowMessage(msg, resetCount > 0);
        }

        #endregion

        #region Save

        protected void btnSave_Click(object sender, EventArgs e)
        {
            DateTime date;
            if (!DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                ShowMessage("التاريخ غير صحيح", false);
                return;
            }

            string lockMessage;
            if (new TeacherAttendanceFinancialService(db).IsDateLocked(date, out lockMessage))
            {
                ShowMessage(lockMessage, false);
                return;
            }

            int saved = 0;
            int blocked = 0;

            for (int i = 0; i < gvLessons.Rows.Count; i++)
            {
                var row = gvLessons.Rows[i];
                int lessonId = Convert.ToInt32(gvLessons.DataKeys[i].Value);
                var lesson = db.DailyLessons.Find(lessonId);
                if (lesson == null) continue;

                var ddlStatus = (System.Web.UI.WebControls.DropDownList)row.FindControl("ddlStatus");
                var txtMinutes = (System.Web.UI.WebControls.TextBox)row.FindControl("txtMinutes");
                var txtNotes = (System.Web.UI.WebControls.TextBox)row.FindControl("txtNotes");

                var existing = db.AttendanceRecords
                    .FirstOrDefault(r => r.DailyLessonId == lessonId && r.TeacherId == lesson.TeacherId);

                if (existing != null && existing.IsApproved)
                {
                    blocked++;
                    continue;
                }

                AttendanceStatus status;
                if (!Enum.TryParse(ddlStatus.SelectedValue, true, out status))
                {
                    ShowMessage("حالة الحضور غير صالحة.", false);
                    return;
                }

                if (status == AttendanceStatus.SubstitutionFull || status == AttendanceStatus.SubstitutionPartial)
                {
                    ShowMessage("لا يمكن تسجيل التعويضية من شاشة الحضور العادي.", false);
                    return;
                }

                int duration = lesson.DurationMinutes;

                int minutes = 0;
                if (txtMinutes != null)
                {
                    int m;
                    if (int.TryParse(txtMinutes.Text, out m)) minutes = m;
                }

                if (minutes < 0) minutes = 0;
                if (minutes > duration) minutes = duration;

                int absent = 0;

                switch (status)
                {
                    case AttendanceStatus.PresentFull:
                        // «حضور كامل» يعني الحضور لا اكتمال المدة: نقبل أن يسجّل
                        // المسجِّل دقائق أقل، ويستحق المعلم أجرها بنسبة وقتها
                        // الفعلي في PayrollCalculationService.
                        if (minutes <= 0)
                        {
                            ShowMessage("الحضور الكامل للمعلم " + lesson.Teacher.Name +
                                " يجب أن يكون أكبر من 0 دقيقة. راجع الحصة رقم " + lesson.PeriodNumber + ".", false);
                            return;
                        }
                        absent = duration - minutes;
                        break;

                    case AttendanceStatus.PresentPartial:
                        if (minutes <= 0)
                        {
                            ShowMessage("الحضور الجزئي للمعلم " + lesson.Teacher.Name +
                                " يجب أن يكون أكبر من 0. راجع الحصة رقم " + lesson.PeriodNumber + ".", false);
                            return;
                        }
                        if (minutes >= duration)
                        {
                            ShowMessage("الحضور الجزئي للمعلم " + lesson.Teacher.Name +
                                " يجب أن يكون أقل من " + duration + " دقيقة. راجع الحصة رقم " + lesson.PeriodNumber + ".", false);
                            return;
                        }
                        absent = duration - minutes;
                        break;

                    case AttendanceStatus.AbsentExcused:
                        absent = duration - minutes;
                        break;

                    case AttendanceStatus.AbsentUnexcused:
                        minutes = 0;
                        absent = 0;
                        break;

                    case AttendanceStatus.Cancelled:
                        absent = 0;
                        break;
                }

                if (existing == null)
                {
                    existing = new AttendanceRecord
                    {
                        DailyLessonId = lessonId,
                        Date = date,
                        TeacherId = lesson.TeacherId,
                        PeriodNumber = lesson.PeriodNumber,
                        LessonRateApplied = ResolveLessonRate(lesson),
                        HourlyRateApplied = ResolveHourlyRate(lesson),
                        RecordedBy = EnumHelper.CurrentUserName
                    };
                    db.AttendanceRecords.Add(existing);
                }
                else
                {
                    existing.IsApproved = false;
                    existing.ApprovedBy = null;
                    existing.ApprovedAt = null;
                }

                existing.Status = status;
                existing.ActualMinutes = minutes;
                existing.AbsentMinutes = absent;
                existing.Notes = txtNotes != null ? txtNotes.Text.Trim() : "";
                existing.RecordedAt = DateTime.Now;
                saved++;
            }

            db.SaveChanges();

            if (saved == 0)
            {
                ShowMessage(
                    blocked > 0
                        ? "لم يتم تعديل أي سجل — السجلات معتمدة مالياً. يجب التراجع عن الاعتماد أولاً."
                        : "لا توجد سجلات قابلة للحفظ.",
                    false);
                return;
            }

            string successMsg = "تم حفظ " + saved + " سجل ساعات حضور";
            if (blocked > 0)
                successMsg += " — تخطّي " + blocked + " سجل معتمد";

            Session[FlashKey] = successMsg + ".";
            Response.Redirect("Home.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        #endregion

        #region Financial Rate Helpers

        private decimal ResolveLessonRate(DailyLesson lesson)
        {
            if (lesson == null) return 0m;

            var rate = db.TeacherSubjectAssignments
                .Where(a => a.TeacherId == lesson.TeacherId &&
                            a.SubjectId == lesson.SubjectId &&
                            a.IsActive &&
                            a.StartDate <= lesson.Date &&
                            (!a.EndDate.HasValue || a.EndDate.Value >= lesson.Date))
                .OrderByDescending(a => a.StartDate)
                .Select(a => (decimal?)a.LessonRate)
                .FirstOrDefault();

            if (rate.HasValue && rate.Value > 0m) return rate.Value;

            return lesson.Teacher != null ? lesson.Teacher.LessonRate :
                db.Teachers.Where(t => t.Id == lesson.TeacherId).Select(t => t.LessonRate).FirstOrDefault();
        }

        private decimal ResolveHourlyRate(DailyLesson lesson)
        {
            if (lesson == null) return 0m;

            var rate = db.TeacherSubjectAssignments
                .Where(a => a.TeacherId == lesson.TeacherId &&
                            a.SubjectId == lesson.SubjectId &&
                            a.IsActive &&
                            a.StartDate <= lesson.Date &&
                            (!a.EndDate.HasValue || a.EndDate.Value >= lesson.Date))
                .OrderByDescending(a => a.StartDate)
                .Select(a => (decimal?)a.HourlyRate)
                .FirstOrDefault();

            if (rate.HasValue && rate.Value > 0m) return rate.Value;

            return lesson.Teacher != null ? lesson.Teacher.HourlyRate :
                db.Teachers.Where(t => t.Id == lesson.TeacherId).Select(t => t.HourlyRate).FirstOrDefault();
        }

        #endregion

        #region Messages

        private void ShowMessage(string text, bool success)
        {
            if (lblMessage == null) return;

            lblMessage.CssClass = success ? "ta-alert success" : "ta-alert danger";
            lblMessage.Text =
                "<i class='fas " + (success ? "fa-check-circle" : "fa-exclamation-triangle") + "'></i>" +
                "<span>" + Server.HtmlEncode(text) + "</span>";
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