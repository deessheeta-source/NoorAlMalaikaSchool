using System;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.TeacherAttendance
{
    public partial class MyRecords : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!SecurityHelper.CanAccess(UserRole.Teacher, UserRole.Admin, UserRole.Supervisor))
            {
                Response.Redirect("~/AccessDenied.aspx");
                return;
            }

            // ✅ اسم المرحلة
            var stage = EnumHelper.GetCurrentStage();
            lblStage.Text = stage == EducationStage.Basic
                ? "المرحلة الأساسية"
                : "المرحلة الثانوية";

            var role = SecurityHelper.CurrentRole;
            string employeeNo = SecurityHelper.CurrentEmployeeNo;

            if (role == UserRole.Teacher)
            {
                // ============ معلم: يعرض سجله الشخصي فقط ============
                if (string.IsNullOrEmpty(employeeNo))
                {
                    Response.Redirect("~/AccessDenied.aspx");
                    return;
                }

                var teacher = db.Teachers.FirstOrDefault(t => t.EmployeeNo == employeeNo);
                if (teacher == null)
                {
                    Response.Redirect("~/AccessDenied.aspx");
                    return;
                }

                LoadAttendance(teacher.Id);
                LoadSubstitutions(teacher.Id);

                lblInfo.Text = "أنت تشاهد سجل ساعات حضورك وغيابك والتعويضيات الخاصة بك فقط.";
            }
            else
            {
                // ============ مشرف/مدير: يعرض معلمي المرحلة الحالية فقط ============
                LoadAttendance(null);
                LoadSubstitutions(null);

                lblInfo.Text = "عرض سجل الساعات والتعويضيات لمعلمي " +
                    (stage == EducationStage.Basic ? "المرحلة الأساسية" : "المرحلة الثانوية") +
                    " فقط.";
            }
        }

        #endregion

        #region Attendance

        private void LoadAttendance(int? teacherId)
        {
            var query = db.AttendanceRecords
                .AsNoTracking()
                .Include(r => r.Teacher)
                .AsQueryable();

            if (teacherId.HasValue)
            {
                // ✅ معلم واحد فقط
                query = query.Where(r => r.TeacherId == teacherId.Value);
            }
            else
            {
                // ✅ فلترة حسب المرحلة الحالية (لمشرف/مدير)
                var stage = EnumHelper.GetCurrentStage();

                var stageTeacherIds = db.Teachers
                    .Where(t => t.Stage == stage)
                    .Select(t => t.Id)
                    .ToList();

                if (stageTeacherIds.Count == 0)
                {
                    gvAttendance.DataSource = null;
                    gvAttendance.DataBind();
                    lblAttendanceCount.Text = "0";
                    lblTotalPresentHours.Text = "0";
                    lblTotalAbsentHours.Text = "0";
                    lblLessonsCount.Text = "0";
                    pnlAttendanceEmpty.Visible = true;   // ✅ جديد
                    return;
                }

                query = query.Where(r => stageTeacherIds.Contains(r.TeacherId));
            }

            var records = query.ToList();

            var attendance = records
                .GroupBy(r => new { r.TeacherId, Date = r.Date.Date })
                .Select(g =>
                {
                    int presentMinutes = g.Where(IsPresentStatus).Sum(r => Math.Max(0, r.ActualMinutes));
                    int absentMinutes = g.Where(IsAbsenceStatus).Sum(r => Math.Max(0, r.AbsentMinutes));
                    bool hasPresent = presentMinutes > 0;
                    bool hasAbsence = absentMinutes > 0;
                    string teacherName = g.Select(r => r.Teacher != null ? r.Teacher.Name : "").FirstOrDefault() ?? "";

                    return new
                    {
                        g.Key.Date,
                        LessonsCount = g.Count(),
                        PresentMinutes = presentMinutes,
                        PresentHours = presentMinutes / 60m,
                        AbsentMinutes = absentMinutes,
                        AbsentHours = absentMinutes / 60m,
                        StatusText = hasPresent
                            ? (hasAbsence ? "حضور وغياب بالساعات" : "حاضر بالساعات")
                            : "غائب بالساعات",
                        StatusClass = hasPresent
                            ? (hasAbsence ? "badge-soft yellow" : "badge-soft green")
                            : "badge-soft red",
                        AllApproved = g.All(r => r.IsApproved),
                        TeacherName = teacherName
                    };
                })
                .OrderByDescending(x => x.Date)
                .ThenBy(x => x.TeacherName)
                .ToList();

            gvAttendance.DataSource = attendance;
            gvAttendance.DataBind();

            // ✅ إظهار/إخفاء Empty State
            pnlAttendanceEmpty.Visible = attendance.Count == 0;

            // ✅ KPI Attendance
            lblAttendanceCount.Text = attendance.Count.ToString();
            lblTotalPresentHours.Text = attendance.Sum(a => a.PresentHours).ToString("0.##");
            lblTotalAbsentHours.Text = attendance.Sum(a => a.AbsentHours).ToString("0.##");
            lblLessonsCount.Text = attendance.Sum(a => a.LessonsCount).ToString();
        }

        private static bool IsPresentStatus(AttendanceRecord record)
        {
            return record.Status == AttendanceStatus.PresentFull ||
                   record.Status == AttendanceStatus.PresentPartial ||
                   record.Status == AttendanceStatus.SubstitutionFull ||
                   record.Status == AttendanceStatus.SubstitutionPartial;
        }

        private static bool IsAbsenceStatus(AttendanceRecord record)
        {
            return record.Status == AttendanceStatus.AbsentExcused ||
                   record.Status == AttendanceStatus.AbsentUnexcused;
        }

        #endregion

        #region Substitutions

        private void LoadSubstitutions(int? teacherId)
        {
            var query = db.Substitutions
                .AsNoTracking()
                .AsQueryable();

            if (teacherId.HasValue)
            {
                // ✅ معلم واحد فقط (أصلي أو معوِّض)
                query = query.Where(s => s.OriginalTeacherId == teacherId.Value
                                      || s.SubstituteTeacherId == teacherId.Value);
            }
            else
            {
                // ✅ فلترة حسب المرحلة الحالية (لمشرف/مدير)
                var stage = EnumHelper.GetCurrentStage();

                var stageTeacherIds = db.Teachers
                    .Where(t => t.Stage == stage)
                    .Select(t => t.Id)
                    .ToList();

                if (stageTeacherIds.Count == 0)
                {
                    gvSubstitutions.DataSource = null;
                    gvSubstitutions.DataBind();
                    lblSubstitutionsCount.Text = "0";
                    lblSubstitutionsTotal.Text = "0";
                    pnlSubstitutionsEmpty.Visible = true;   // ✅ جديد
                    return;
                }

                query = query.Where(s =>
                    stageTeacherIds.Contains(s.OriginalTeacherId) ||
                    stageTeacherIds.Contains(s.SubstituteTeacherId));
            }

            var subs = query
                .OrderByDescending(s => s.Date)
                .Select(s => new
                {
                    s.Date,
                    s.PeriodNumber,
                    ClassName = s.ClassRoom != null ? s.ClassRoom.Name : "",
                    SubjectName = s.Subject != null ? s.Subject.Name : "",
                    OriginalTeacher = s.OriginalTeacher != null ? s.OriginalTeacher.Name : "",
                    Role = teacherId.HasValue
                        ? (s.OriginalTeacherId == teacherId.Value ? "أصلي" : "معوِّض")
                        : "",
                    s.IsApproved
                })
                .ToList();

            gvSubstitutions.DataSource = subs;
            gvSubstitutions.DataBind();

            // ✅ إظهار/إخفاء Empty State
            pnlSubstitutionsEmpty.Visible = subs.Count == 0;

            // ✅ KPI Substitutions
            lblSubstitutionsCount.Text = subs.Count.ToString();
            lblSubstitutionsTotal.Text = subs.Count.ToString();
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