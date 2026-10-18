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
    public partial class DayDetail : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (IsPostBack) return;

            string dateParam = Request.QueryString["date"];
            if (!string.IsNullOrEmpty(dateParam))
            {
                DateTime d;
                if (DateTime.TryParse(dateParam, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                    txtDate.Text = d.ToString("yyyy-MM-dd");
            }
            else
            {
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }

            LoadTeachers();
            LoadDetail();
        }

        #endregion

        #region Load

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

        private void LoadDetail()
        {
            DateTime date;
            if (!DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                ShowMessage("التاريخ غير صحيح", false);
                return;
            }

            lblDateTitle.Text = date.ToString("yyyy-MM-dd");

            var stage = EnumHelper.GetCurrentStage();

            var stageTeacherIds = db.Teachers
                .Where(t => t.Stage == stage)
                .Select(t => t.Id)
                .ToList();

            if (stageTeacherIds.Count == 0)
            {
                gvRecords.DataSource = null;
                gvRecords.DataBind();
                lblInfo.Text = "لا يوجد معلمون في هذه المرحلة.";
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

            var rows = query
                .OrderBy(x => x.PeriodNumber)
                .ToList()
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
                })
                .ToList();

            gvRecords.DataSource = rows;
            gvRecords.DataBind();

            // ✅ إظهار Empty State عند عدم وجود بيانات
            pnlEmpty.Visible = rows.Count == 0;

            LoadApprovalState(date);
            lblInfo.Text = "عدد الحصص المسجلة: " + rows.Count;
        }

        private void LoadApprovalState(DateTime date)
        {
            var stage = EnumHelper.GetCurrentStage();

            var stageTeacherIds = db.Teachers
                .Where(t => t.Stage == stage)
                .Select(t => t.Id)
                .ToList();

            var records = stageTeacherIds.Count == 0
                ? new List<AttendanceRecord>()
                : db.AttendanceRecords
                    .Where(r => r.Date == date && stageTeacherIds.Contains(r.TeacherId))
                    .ToList();

            int total = records.Count;
            int approved = records.Count(r => r.IsApproved);

            if (total == 0)
            {
                lblApprovalState.Text = "<span class='badge-gray'><i class='fas fa-info-circle'></i> لا توجد سجلات لهذا اليوم</span>";
                btnApproveDay.Enabled = false;
                return;
            }

            if (approved == total)
            {
                lblApprovalState.Text =
                    "<span class='badge-green'>" +
                    "<i class='fas fa-check-circle'></i> تم اعتماد جميع السجلات (" + total + ")" +
                    "</span>";
                btnApproveDay.Enabled = true;
                btnApproveDay.Text = "<i class='fas fa-undo'></i> التراجع عن الاعتماد";
            }
            else if (approved > 0)
            {
                lblApprovalState.Text =
                    "<span class='badge-yellow'>" +
                    "<i class='fas fa-hourglass-half'></i> اعتماد جزئي: " + approved + " من " + total +
                    "</span>";
                btnApproveDay.Enabled = true;
                btnApproveDay.Text = "<i class='fas fa-check-circle'></i> اعتماد اليوم";
            }
            else
            {
                lblApprovalState.Text =
                    "<span class='badge-gray'>" +
                    "<i class='fas fa-hourglass-half'></i> غير معتمد (" + total + " سجل)" +
                    "</span>";
                btnApproveDay.Enabled = true;
                btnApproveDay.Text = "<i class='fas fa-check-circle'></i> اعتماد اليوم";
            }
        }

        #endregion

        #region Helpers

        protected string StatusName(int status)
        {
            return EnumHelper.GetDisplayName((AttendanceStatus)status);
        }

        protected string StatusClass(int status)
        {
            switch ((AttendanceStatus)status)
            {
                case AttendanceStatus.PresentFull:
                case AttendanceStatus.SubstitutionFull:
                    return "badge-green";

                case AttendanceStatus.PresentPartial:
                case AttendanceStatus.SubstitutionPartial:
                case AttendanceStatus.AbsentExcused:
                    return "badge-yellow";

                case AttendanceStatus.AbsentUnexcused:
                    return "badge-red";

                case AttendanceStatus.Cancelled:
                    return "badge-gray";

                default:
                    return "badge-gray";
            }
        }

        #endregion

        #region Handlers

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadDetail();
        }

        protected void ddlTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDetail();
        }

        protected void btnApproveDay_Click(object sender, EventArgs e)
        {
            if (!SecurityHelper.CanAccess(UserRole.Supervisor))
            {
                ShowMessage("الاعتماد يتطلب صلاحية مشرف أو مدير نظام", false);
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
                ? new List<AttendanceRecord>()
                : db.AttendanceRecords
                    .Where(r => r.Date == date && stageTeacherIds.Contains(r.TeacherId))
                    .ToList();

            if (records.Count == 0)
            {
                ShowMessage("لا توجد سجلات لهذا اليوم في هذه المرحلة", false);
                return;
            }

            bool allApproved = records.All(r => r.IsApproved);

            var validation = new TeacherAttendanceFinancialService(db)
                .ValidateDate(date, false, stage);

            if (!allApproved && !validation.IsReady)
            {
                ShowMessage(
                    validation.Summary +
                    " لا يمكن اعتماد اليوم قبل اكتمال جميع الحصص ومعالجة التعارضات المالية.",
                    false);
                return;
            }

            string user = EnumHelper.CurrentUserName;

            foreach (var r in records)
            {
                if (allApproved)
                {
                    r.IsApproved = false;
                    r.ApprovedBy = null;
                    r.ApprovedAt = null;
                }
                else
                {
                    r.IsApproved = true;
                    r.ApprovedBy = user;
                    r.ApprovedAt = DateTime.Now;
                }
            }

            db.SaveChanges();

            LoadDetail();

            ShowMessage(
                allApproved
                    ? "تم التراجع عن اعتماد اليوم."
                    : "تم اعتماد جميع سجلات اليوم بنجاح.",
                true);
        }

        protected void gvRecords_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvRecords.PageIndex = e.NewPageIndex;
            LoadDetail();
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
            if (db != null)
                db.Dispose();

            base.OnUnload(e);
        }

        #endregion
    }
}