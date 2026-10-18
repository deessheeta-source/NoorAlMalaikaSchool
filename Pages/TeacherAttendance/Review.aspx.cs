using System;
using System.Globalization;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Text;
using System.Web;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.TeacherAttendance
{
    public partial class Review : Page
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
                DateTime d;
                if (!string.IsNullOrEmpty(dateParam) && DateTime.TryParse(dateParam, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                    txtDate.Text = d.ToString("yyyy-MM-dd");

                if (string.IsNullOrEmpty(txtDate.Text))
                    txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

                LoadTeachers();
                ApplyTeacherQueryString();

                DateTime date;
                if (DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    // ✅ إصلاح صامت للأسعار المفقودة قبل الفحص
                    FixMissingRates(date);

                    LoadAll(date);
                }
            }
        }

        #endregion

        #region Fix Missing Rates

        /// <summary>
        /// ✅ إصلاح الأسعار المفقودة في سجلات الحضور بأخذها من بطاقة المعلم.
        /// لا يلمس السجلات المعتمدة مالياً.
        /// </summary>
        private int FixMissingRates(DateTime date)
        {
            var stage = EnumHelper.GetCurrentStage();

            var stageTeacherIds = db.Teachers
                .Where(t => t.Stage == stage)
                .Select(t => t.Id)
                .ToList();

            if (stageTeacherIds.Count == 0) return 0;

            var records = db.AttendanceRecords
                .Include(r => r.Teacher)
                .Where(r => r.Date == date
                         && !r.IsApproved
                         && stageTeacherIds.Contains(r.TeacherId))
                .Where(r => r.LessonRateApplied == 0m || r.HourlyRateApplied == 0m)
                .ToList();

            if (records.Count == 0) return 0;

            int fixedCount = 0;

            foreach (var r in records)
            {
                var teacher = r.Teacher ?? db.Teachers.Find(r.TeacherId);
                if (teacher == null) continue;

                bool changed = false;

                // ✅ نسخ سعر الحصة من المعلم إذا كان مفقوداً
                if (r.LessonRateApplied == 0m && teacher.LessonRate > 0m)
                {
                    r.LessonRateApplied = teacher.LessonRate;
                    changed = true;
                }

                // ✅ نسخ سعر الساعة من المعلم إذا كان مفقوداً
                if (r.HourlyRateApplied == 0m && teacher.HourlyRate > 0m)
                {
                    r.HourlyRateApplied = teacher.HourlyRate;
                    changed = true;
                }

                if (changed) fixedCount++;
            }

            if (fixedCount > 0)
                db.SaveChanges();

            return fixedCount;
        }

        #endregion

        #region Query String / Teachers

        private void ApplyTeacherQueryString()
        {
            int teacherId;
            if (!int.TryParse(Request.QueryString["teacherId"], out teacherId) || teacherId <= 0)
                return;

            if (ddlTeacher.Items.FindByValue(teacherId.ToString()) != null)
                ddlTeacher.SelectedValue = teacherId.ToString();
        }

        private void LoadTeachers()
        {
            var stage = EnumHelper.GetCurrentStage();

            ddlTeacher.Items.Clear();
            ddlTeacher.Items.Add(new System.Web.UI.WebControls.ListItem("جميع المعلمين", ""));

            var teachers = db.Teachers
                .Where(t => t.Stage == stage && t.IsActive)
                .OrderBy(t => t.Name)
                .Select(t => new { t.Id, t.Name })
                .ToList();

            foreach (var t in teachers)
                ddlTeacher.Items.Add(new System.Web.UI.WebControls.ListItem(t.Name, t.Id.ToString()));
        }

        #endregion

        #region Load

        private void LoadAll(DateTime date)
        {
            LoadReviewGrid(date);
            LoadApprovalState(date);
        }

        private void LoadReviewGrid(DateTime date)
        {
            var stage = EnumHelper.GetCurrentStage();

            var stageTeacherIds = db.Teachers
                .Where(t => t.Stage == stage)
                .Select(t => t.Id)
                .ToList();

            if (stageTeacherIds.Count == 0)
            {
                gvReview.DataSource = null;
                gvReview.DataBind();
                lblReviewSummary.Text = "لا يوجد معلمون في هذه المرحلة.";
                pnlEmpty.Visible = true;
                return;
            }

            var query = from r in db.AttendanceRecords
                        where r.Date == date
                              && stageTeacherIds.Contains(r.TeacherId)
                        join d in db.DailyLessons on r.DailyLessonId equals d.Id into days
                        from d in days.DefaultIfEmpty()
                        select new
                        {
                            r.Id,
                            r.TeacherId,
                            r.PeriodNumber,
                            TeacherName = r.Teacher.Name,
                            SubjectName = d != null ? d.Subject.Name : "—",
                            ClassName = d != null ? d.ClassRoom.Name : "—",
                            StartTime = d != null ? d.StartTime : (TimeSpan?)null,
                            EndTime = d != null ? d.EndTime : (TimeSpan?)null,
                            DurationMinutes = d != null && d.DurationMinutes > 0 ? d.DurationMinutes : 45,
                            r.Status,
                            r.ActualMinutes,
                            r.IsApproved,
                            r.Notes
                        };

            int teacherId;
            if (int.TryParse(ddlTeacher.SelectedValue, out teacherId) && teacherId > 0)
                query = query.Where(x => x.TeacherId == teacherId);

            var rows = query.OrderBy(x => x.PeriodNumber).ToList()
                .Select(x => new
                {
                    x.Id,
                    x.PeriodNumber,
                    x.TeacherName,
                    x.SubjectName,
                    x.ClassName,
                    StartTimeText = x.StartTime.HasValue ? x.StartTime.Value.ToString(@"hh\:mm") : "—",
                    EndTimeText = x.EndTime.HasValue ? x.EndTime.Value.ToString(@"hh\:mm") : "—",
                    x.DurationMinutes,
                    x.Status,
                    x.ActualMinutes,
                    x.IsApproved,
                    x.Notes
                }).ToList();

            gvReview.DataSource = rows;
            gvReview.DataBind();

            pnlEmpty.Visible = rows.Count == 0;

            lblReviewSummary.Text = "عدد الحصص المسجلة: " + rows.Count;
        }

        protected string StatusName(int status) => EnumHelper.GetDisplayName((AttendanceStatus)status);

        protected string StatusClass(int status)
        {
            switch ((AttendanceStatus)status)
            {
                case AttendanceStatus.PresentFull:
                case AttendanceStatus.SubstitutionFull: return "badge-green";
                case AttendanceStatus.PresentPartial:
                case AttendanceStatus.SubstitutionPartial:
                case AttendanceStatus.AbsentExcused: return "badge-yellow";
                case AttendanceStatus.AbsentUnexcused: return "badge-red";
                case AttendanceStatus.Cancelled: return "badge-gray";
                default: return "badge-gray";
            }
        }

        private void LoadApprovalState(DateTime date)
        {
            var stage = EnumHelper.GetCurrentStage();

            var stageTeacherIds = db.Teachers
                .Where(t => t.Stage == stage)
                .Select(t => t.Id)
                .ToList();

            var service = new TeacherAttendanceFinancialService(db);
            var validation = service.ValidateDate(date, false);

            int existingForDate = stageTeacherIds.Count == 0
                ? 0
                : db.AttendanceRecords.Count(r =>
                    r.Date == date &&
                    stageTeacherIds.Contains(r.TeacherId));

            int approvedForDate = stageTeacherIds.Count == 0
                ? 0
                : db.AttendanceRecords.Count(r =>
                    r.Date == date &&
                    r.IsApproved &&
                    stageTeacherIds.Contains(r.TeacherId));

            lblWorkflowDetail.Text = string.Format(
                "<span style='color:#1f3d33;'>اليوم: <strong>{0}</strong></span>" +
                "<span class='stat-item'>مجدولة: <strong>{1}</strong></span>" +
                "<span class='stat-item green'>مسجلة: <strong>{2}</strong></span>" +
                "<span class='stat-item green'>معتمدة: <strong>{3}</strong></span>" +
                "<span class='stat-item {4}'>ناقصة: <strong>{5}</strong></span>" +
                "<span class='stat-item {6}'>غير معتمدة: <strong>{7}</strong></span>" +
                "<span class='stat-item {8}'>تعارض: <strong>{9}</strong></span>",
                date.ToString("yyyy-MM-dd"),
                validation.ScheduledLessons,
                validation.RecordedLessons,
                validation.ApprovedLessons,
                validation.MissingLessons > 0 ? "red" : "green",
                validation.MissingLessons,
                validation.UnapprovedLessons > 0 ? "orange" : "green",
                validation.UnapprovedLessons,
                validation.ConflictCount > 0 ? "red" : "green",
                validation.ConflictCount);

            if (existingForDate > 0 && approvedForDate == existingForDate)
            {
                lblApprovalState.Text =
                    "<span class='badge-green'>" +
                    "<i class='fas fa-check-circle'></i> اليوم معتمد مالياً (" + approvedForDate + " سجل)" +
                    "</span>";

                btnApprove.Text = "التراجع عن الاعتماد";
                btnApprove.OnClientClick = "return confirm('اليوم معتمد مالياً. هل تريد التراجع؟');";
            }
            else
            {
                bool ready = validation.ScheduledLessons > 0
                    && validation.RecordedLessons == validation.ScheduledLessons
                    && validation.MissingLessons == 0
                    && validation.ConflictCount == 0;

                if (ready)
                {
                    lblApprovalState.Text =
                        "<span class='badge-yellow'>" +
                        "<i class='fas fa-hourglass-half'></i> السجلات محفوظة وتنتظر الاعتماد" +
                        "</span>";

                    btnApprove.Text = "اعتماد ساعات اليوم مالياً";
                    btnApprove.OnClientClick = "return confirm('سيتم اعتماد جميع سجلات الحضور لهذا اليوم مالياً. هل راجعت؟');";
                }
                else
                {
                    lblApprovalState.Text = existingForDate > 0
                        ? "<span class='badge-red'><i class='fas fa-lock'></i> لا يمكن الاعتماد — عالج النواقص أولاً</span>"
                        : "<span class='badge-gray'>ابدأ بتسجيل وحفظ حضور الحصص</span>";

                    btnApprove.Text = "اعتماد ساعات اليوم مالياً";
                    btnApprove.OnClientClick = "return false;";
                }
            }

            RenderConflictDetails(validation, date);
        }

        private void RenderConflictDetails(TeacherAttendanceFinancialService.ValidationResult validation, DateTime date)
        {
            if (validation.ConflictCount == 0 && validation.WarningCount == 0)
            {
                conflictPanel.Visible = false;
                lblConflictDetails.Text = string.Empty;
                return;
            }

            var sb = new StringBuilder();

            sb.Append("<div class=\"conflict-title\">");
            sb.Append("<i class=\"fas fa-exclamation-triangle\"></i>");
            sb.Append("<span>تعارضات مالية (").Append(validation.ConflictCount).Append(")</span>");
            sb.Append("</div>");

            sb.Append("<div class=\"conflict-help\">");
            sb.Append("عالج العناصر التالية ثم أعد المحاولة لاعتماد اليوم مالياً. ");
            sb.Append("إذا كانت المشكلة متعلقة بالأسعار المفقودة، اضغط <strong>«إصلاح أسعار الحصص تلقائياً»</strong> بالأسفل.");
            sb.Append("</div>");

            sb.Append("<div class=\"conflict-list\">");

            int index = 0;
            foreach (var issue in validation.Errors.Where(i => !string.IsNullOrEmpty(i.Message)))
            {
                index++;
                sb.Append("<div class=\"conflict-item\">");
                sb.Append("<div class=\"conflict-index\">").Append(index).Append("</div>");
                sb.Append("<div class=\"conflict-body\">");

                if (!string.IsNullOrEmpty(issue.TeacherName))
                    sb.Append("<div><strong>المعلم/المعلمة:</strong> ")
                      .Append(HttpUtility.HtmlEncode(issue.TeacherName)).Append("</div>");

                if (issue.PeriodNumber.HasValue)
                    sb.Append("<div><strong>الحصة:</strong> ")
                      .Append(issue.PeriodNumber.Value).Append("</div>");

                if (issue.Date.HasValue)
                    sb.Append("<div><strong>التاريخ:</strong> ")
                      .Append(issue.Date.Value.ToString("yyyy-MM-dd")).Append("</div>");

                sb.Append("<div class=\"conflict-message\"><strong>المشكلة:</strong> ")
                  .Append(HttpUtility.HtmlEncode(issue.Message)).Append("</div>");

                sb.Append("</div></div>");
            }

            sb.Append("</div>");

            lblConflictDetails.Text = sb.ToString();
            conflictPanel.Visible = true;
        }

        #endregion

        #region Handlers

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            DateTime date;
            if (DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                LoadAll(date);
        }

        protected void ddlTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime date;
            if (DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                LoadAll(date);
        }

        // ✅ زر إصلاح الأسعار المفقودة (يدوي)
        protected void btnFixRates_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime date;
                if (!DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    ShowMessage("التاريخ غير صحيح", false);
                    return;
                }

                int fixedCount = FixMissingRates(date);

                if (fixedCount > 0)
                {
                    ShowMessage("تم إصلاح أسعار " + fixedCount + " سجل حضور. أعد الفحص الآن.", true);
                    LoadAll(date);
                }
                else
                {
                    ShowMessage("لا توجد سجلات تحتاج إصلاحاً. إما أن الأسعار محفوظة، أو أن أسعار المعلمين أنفسهم غير محددة في بطاقاتهم.", false);
                }
            }
            catch (Exception ex)
            {
                Exception error = ex;
                while (error.InnerException != null)
                    error = error.InnerException;

                ShowMessage("تعذر إصلاح الأسعار: " + Server.HtmlEncode(error.Message), false);
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (!SecurityHelper.CanAccess(UserRole.Supervisor))
            {
                ShowMessage("الاعتماد يتطلب صلاحية مشرف.", false);
                return;
            }

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

            var stage = EnumHelper.GetCurrentStage();

            var stageTeacherIds = db.Teachers
                .Where(t => t.Stage == stage)
                .Select(t => t.Id)
                .ToList();

            var records = stageTeacherIds.Count == 0
                ? new System.Collections.Generic.List<AttendanceRecord>()
                : db.AttendanceRecords
                    .Where(r => r.Date == date && stageTeacherIds.Contains(r.TeacherId))
                    .ToList();

            if (records.Count == 0)
            {
                ShowMessage("لا توجد سجلات لهذا اليوم في هذه المرحلة", false);
                return;
            }

            bool allApproved = records.All(r => r.IsApproved);

            if (!allApproved)
            {
                // ✅ إصلاح تلقائي للأسعار قبل الفحص
                FixMissingRates(date);

                var validation = new TeacherAttendanceFinancialService(db).ValidateDate(date, false);
                RenderConflictDetails(validation, date);

                if (!validation.IsReady)
                {
                    ShowMessage(validation.Summary + " لا يمكن الاعتماد قبل اكتمال الحضور. " +
                                "إذا كانت التعارضات متعلقة بالأسعار، اضغط «إصلاح أسعار الحصص تلقائياً».", false);
                    return;
                }
            }

            if (allApproved)
            {
                foreach (var r in records)
                {
                    r.IsApproved = false;
                    r.ApprovedBy = null;
                    r.ApprovedAt = null;
                }
                db.SaveChanges();

                Session[FlashKey] = "تم التراجع عن اعتماد ساعات يوم " +
                    date.ToString("yyyy-MM-dd") + " في " + StageName(stage) + ".";

                Response.Redirect("Home.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            else
            {
                foreach (var r in records)
                {
                    r.IsApproved = true;
                    r.ApprovedBy = EnumHelper.CurrentUserName;
                    r.ApprovedAt = DateTime.Now;
                }
                db.SaveChanges();

                Session[FlashKey] = "تم اعتماد ساعات يوم " +
                    date.ToString("yyyy-MM-dd") + " في " + StageName(stage) +
                    " — أصبحت محسوبة مالياً.";

                Response.Redirect("Home.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private string StageName(EducationStage stage)
        {
            return stage == EducationStage.Basic
                ? "المرحلة الأساسية"
                : "المرحلة الثانوية";
        }

        #endregion

        #region Messages / Dispose

        private void ShowMessage(string text, bool success)
        {
            if (lblMessage == null) return;

            lblMessage.CssClass = success ? "ta-alert success" : "ta-alert danger";
            lblMessage.Text =
                "<i class='fas " + (success ? "fa-check-circle" : "fa-exclamation-triangle") + "'></i>" +
                "<span>" + Server.HtmlEncode(text) + "</span>";
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }

        #endregion
    }
}