using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool
{
    public partial class Default : Page
    {
        private SchoolDbContext db = new SchoolDbContext();


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }


        private void LoadDashboard()
        {
            try
            {
                // The dashboard is open to every authenticated role, but the payments figure
                // and the recent-payments grid are financial data. A Teacher is denied
                // Pages/Payments/* and Pages/Fees/* by SecurityHelper, so those widgets must
                // be hidden here too, otherwise the dashboard leaks the same information.
                bool canSeeFinance = SecurityHelper.CanAccess(UserRole.Accountant);
                var stage = EnumHelper.GetCurrentStage();

                int totalStudents = db.Students.Count(s => s.Stage == stage && s.IsActive);
                var currentYear = db.AcademicYears.FirstOrDefault(y => y.IsCurrent);
                var classRoomsQuery = db.ClassRooms.AsQueryable().Where(c => c.GradeLevel != null && c.GradeLevel.Stage == stage);
                if (currentYear != null)
                    classRoomsQuery = classRoomsQuery.Where(c => c.AcademicYearId == currentYear.Id);
                int totalClasses = classRoomsQuery.Count();
                int totalTeachers = db.Teachers.Count(t => t.Stage == stage && t.IsActive);

                DateTime today = DateTime.Today;

                int totalBasicStudents = db.Students.Count(s => s.Stage == EducationStage.Basic && s.IsActive);
                int totalSecondaryStudents = db.Students.Count(s => s.Stage == EducationStage.Secondary && s.IsActive);

                int presentBasic = db.Attendances.Count(a => a.Date == today && a.Student.Stage == EducationStage.Basic && a.Student.IsActive && a.IsPresent);
                int absentBasic = db.Attendances.Count(a => a.Date == today && a.Student.Stage == EducationStage.Basic && a.Student.IsActive && !a.IsPresent);
                int presentSecondary = db.Attendances.Count(a => a.Date == today && a.Student.Stage == EducationStage.Secondary && a.Student.IsActive && a.IsPresent);
                int absentSecondary = db.Attendances.Count(a => a.Date == today && a.Student.Stage == EducationStage.Secondary && a.Student.IsActive && !a.IsPresent);

                int totalStudentsAll = totalBasicStudents + totalSecondaryStudents;
                int totalAbsent = absentBasic + absentSecondary;
                int totalPresent = presentBasic + presentSecondary;
                string attendanceRate = totalStudentsAll > 0
                    ? (totalPresent * 100 / totalStudentsAll).ToString("N0") + "%"
                    : "-";

                // The panels ship with visible="false" in the markup, so they are only ever
                // rendered when this role is actually allowed to see financial data.
                pnlPaymentsStat.Visible = canSeeFinance;
                pnlRecentPayments.Visible = canSeeFinance;

                if (canSeeFinance)
                {
                    decimal totalPaymentAmount = db.Payments
                        .Where(p => p.Student.Stage == stage)
                        .Select(p => (decimal?)p.Amount)
                        .Sum() ?? 0;
                    lblTotalPayments.InnerText = totalPaymentAmount.ToString("N0");
                }

                lblTotalStudents.InnerText = totalStudents.ToString("N0");
                lblTotalStudentsAll.InnerText = totalStudentsAll.ToString("N0");
                lblStageLabel.InnerText = stage == EducationStage.Basic ? "طلاب الاساسي" : "طلاب الثانوي";
                lblTotalTeachers.InnerText = totalTeachers.ToString("N0");
                lblTotalClasses.InnerText = totalClasses.ToString("N0");
                lblAbsentToday.InnerText = totalAbsent.ToString("N0");

                lblPresentBasic.InnerText = presentBasic.ToString("N0");
                lblAbsentBasic.InnerText = absentBasic.ToString("N0");
                lblPresentSecondary.InnerText = presentSecondary.ToString("N0");
                lblAbsentSecondary.InnerText = absentSecondary.ToString("N0");

                if (totalStudentsAll > 0)
                    attendanceBar.Style["width"] = (totalPresent * 100 / totalStudentsAll) + "%";

                lblAttendanceRateBig.InnerText = attendanceRate;

                // حضور المعلمين يُستخرج من سجل الحصص بالساعات/الدقائق فقط.
                // لا نعتمد على TeacherAttendances (التسجيل اليومي القديم).
                var todayLessons = db.DailyLessons
                    .AsNoTracking()
                    .Where(l => l.Date == today
                        && l.Teacher != null
                        && l.Teacher.Stage == stage
                        && l.Teacher.IsActive)
                    .Select(l => l.Id)
                    .ToList();

                var todayTeacherRecords = todayLessons.Count == 0
                    ? new List<AttendanceRecord>()
                    : db.AttendanceRecords
                        .AsNoTracking()
                        .Where(r => todayLessons.Contains(r.DailyLessonId))
                        .ToList();

                var teacherDailyGroups = todayTeacherRecords
                    .GroupBy(r => r.TeacherId)
                    .ToList();

                int presentTeachers = teacherDailyGroups.Count(g => g.Any(IsPresentAttendance));
                int absentTeachers = teacherDailyGroups.Count(g => g.All(IsAbsenceAttendance));
                lblPresentTeachers.InnerText = presentTeachers.ToString("N0");
                lblAbsentTeachers.InnerText = absentTeachers.ToString("N0");

                if (canSeeFinance)
                    LoadRecentPayments(stage);
                LoadClassChart(stage);
            }
            catch (Exception ex)
            {
                AppErrorLogger.Log(ex, "Dashboard.LoadDashboard");
                lblError.Text = "تعذر تحميل بعض بيانات لوحة التحكم حالياً. حاول تحديث الصفحة.";
                lblError.CssClass = "text-danger";
                lblError.Visible = true;
            }
        }


        private static bool IsPresentAttendance(AttendanceRecord record)
        {
            return (record.Status == AttendanceStatus.PresentFull ||
                    record.Status == AttendanceStatus.PresentPartial ||
                    record.Status == AttendanceStatus.SubstitutionFull ||
                    record.Status == AttendanceStatus.SubstitutionPartial) &&
                   record.ActualMinutes > 0;
        }

        private static bool IsAbsenceAttendance(AttendanceRecord record)
        {
            return record.Status == AttendanceStatus.AbsentExcused ||
                   record.Status == AttendanceStatus.AbsentUnexcused;
        }

        private void LoadRecentPayments(EducationStage stage)
        {
            var payments = db.Payments
                .Include(p => p.Student)
                .Where(p => p.Student.Stage == stage)
                .OrderByDescending(p => p.PaymentDate)
                .Take(6)
                .ToList()
                .Select(p => new
                {
                    StudentName = p.Student.Name,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    PaymentMethodName = EnumHelper.GetPaymentMethodName(p.Method)
                })
                .ToList();

            gvRecentPayments.DataSource = payments;
            gvRecentPayments.DataBind();
        }


        private void LoadClassChart(EducationStage currentStage)
        {
            var classData = db.Students
                .AsNoTracking()
                .Where(s => s.Stage == currentStage)
                .GroupBy(s => s.ClassRoomId)
                .Select(g => new
                {
                    ClassRoomId = g.Key,
                    Count = g.Count()
                })
                .ToList();

            bool isBasic = currentStage == EducationStage.Basic;

            var idsForStage = EnumHelper.GetClassRoomIdsForStage(db, currentStage);
            var sortedRooms = EnumHelper.SortClassRooms(db.ClassRooms.Where(c => idsForStage.Contains(c.Id)).ToList());
            var roomOrder = sortedRooms
                .Select((r, i) => new { r.Id, Order = i })
                .ToDictionary(x => x.Id, x => x.Order);

            var currentData = classData.ToList();

            int maxCount = currentData.Select(c => c.Count).DefaultIfEmpty(1).Max();

            var roomNames = sortedRooms.ToDictionary(r => r.Id, r => r.Name);
            var chartData = currentData
                .OrderBy(c => roomOrder.ContainsKey(c.ClassRoomId) ? roomOrder[c.ClassRoomId] : int.MaxValue)
                .Select(c =>
                {
                    string roomName;
                    roomNames.TryGetValue(c.ClassRoomId, out roomName);
                    int percent = maxCount > 0 ? (c.Count * 100 / maxCount) : 0;
                    return new
                    {
                        ClassName = GetShortClassName(roomName),
                        Count = c.Count,
                        Percent = percent,
                        Height = (percent * 160 / 100).ToString() + "px"
                    };
                }).ToList();

            if (isBasic)
            {
                rptBasicChart.DataSource = chartData;
                rptBasicChart.DataBind();
                lblBasicChartEmpty.Visible = chartData.Count == 0;
                pnlBasicChart.Visible = true;
                pnlSecondaryChart.Visible = false;
            }
            else
            {
                rptSecondaryChart.DataSource = chartData;
                rptSecondaryChart.DataBind();
                lblSecondaryChartEmpty.Visible = chartData.Count == 0;
                pnlSecondaryChart.Visible = true;
                pnlBasicChart.Visible = false;
            }
        }


        private string GetShortClassName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "—";
            string trimmed = name.Trim();
            if (trimmed.StartsWith("الصف ")) return trimmed.Substring("الصف ".Length).Trim();
            if (trimmed.StartsWith("الصف")) return trimmed.Substring("الصف".Length).Trim();
            return trimmed;
        }

        protected void rptBasicChart_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                System.Web.UI.HtmlControls.HtmlGenericControl fill = e.Item.FindControl("chartFill") as System.Web.UI.HtmlControls.HtmlGenericControl;
                if (fill != null)
                {
                    string height = DataBinder.Eval(e.Item.DataItem, "Height") as string;
                    if (string.IsNullOrEmpty(height)) height = "0px";
                    fill.Style["height"] = height;
                }
            }
        }

        protected void rptSecondaryChart_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                System.Web.UI.HtmlControls.HtmlGenericControl fill = e.Item.FindControl("chartFillSecondary") as System.Web.UI.HtmlControls.HtmlGenericControl;
                if (fill != null)
                {
                    string height = DataBinder.Eval(e.Item.DataItem, "Height") as string;
                    if (string.IsNullOrEmpty(height)) height = "0px";
                    fill.Style["height"] = height;
                }
            }
        }

        public string GetStageUrl(string page)
        {
            var master = Master as SiteMaster;
            if (master != null) return master.GetStageUrl(page);
            return page;
        }


        public string GetActiveClass(string section)
        {
            string currentSection = Request.Url.Segments.Length > 1
                ? Request.Url.Segments[1].TrimEnd('/')
                : "Default.aspx";
            if (section == "Default.aspx" && currentSection == "Default.aspx")
                return "active";
            if (section != "Default.aspx" && currentSection == section)
                return "active";
            return "";
        }


        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            if (db != null) db.Dispose();
        }
    }
}
