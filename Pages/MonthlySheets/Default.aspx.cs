using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.MonthlySheets
{
    public partial class Default : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private const string GradesField = "ms";
        private const string EvaluationField = "ev";
        private const string MonthsField = "mnth";

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadAcademicYears();
                LoadGradeLevels();
                LoadDivisions();
                LoadStudents();
                RenderSheet();
            }
        }

        // =========================================================
        // السنة الدراسية
        // =========================================================
        private void LoadAcademicYears()
        {
            var years = db.AcademicYears
                .OrderByDescending(y => y.StartDate)
                .ToList();

            ddlAcademicYear.Items.Clear();
            ddlAcademicYear.Items.Add(new ListItem("— اختر السنة —", ""));

            foreach (var y in years)
                ddlAcademicYear.Items.Add(new ListItem(y.Name, y.Id.ToString()));

            var current = years.FirstOrDefault(y => y.IsCurrent);
            if (current != null)
                ddlAcademicYear.SelectedValue = current.Id.ToString();
        }

        // =========================================================
        // الصف الدراسي
        // =========================================================
        private void LoadGradeLevels()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();

            var gradeLevels = db.GradeLevels
                .AsNoTracking()
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlGradeLevel.Items.Clear();
            ddlGradeLevel.Items.Add(new ListItem("— اختر الصف —", ""));

            foreach (var gl in gradeLevels)
                ddlGradeLevel.Items.Add(new ListItem(gl.Name, gl.Id.ToString()));
        }

        private bool IsSecondaryGrade(int gradeLevelId)
        {
            if (gradeLevelId <= 0) return false;

            try
            {
                string gradeName = db.GradeLevels
                    .AsNoTracking()
                    .Where(g => g.Id == gradeLevelId)
                    .Select(g => g.Name)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(gradeName)) return false;

                gradeName = gradeName.Trim();
                return gradeName == "ثاني ثانوي" || gradeName == "ثالث ثانوي";
            }
            catch { return false; }
        }

        private void ConfigureSectionFilter()
        {
            int gradeLevelId;
            int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId);

            bool show = IsSecondaryGrade(gradeLevelId);
            pnlSectionFilter.Visible = show;

            if (!show)
            {
                if (ddlSection.Items.FindByValue("") != null)
                {
                    ddlSection.ClearSelection();
                    ddlSection.Items.FindByValue("").Selected = true;
                }
            }
        }

        private string GetSelectedSection()
        {
            if (ddlSection == null) return null;
            string value = ddlSection.SelectedValue;
            if (string.IsNullOrWhiteSpace(value)) return null;
            return value;
        }

        private string ExtractDivisionLetter(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            int idx = classRoomName.IndexOf("شعبة", StringComparison.Ordinal);
            if (idx < 0) return null;

            string after = classRoomName.Substring(idx + "شعبة".Length).Trim();
            if (string.IsNullOrWhiteSpace(after)) return null;

            return after[0].ToString();
        }

        private string ExtractSectionFromName(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            if (classRoomName.Contains("علمي")) return "علمي";
            if (classRoomName.Contains("أدبي")) return "أدبي";

            return null;
        }

        // =========================================================
        // الشعبة
        // =========================================================
        private void LoadDivisions()
        {
            if (ddlDivision == null) return;

            string previousValue = ddlDivision.SelectedValue;

            ddlDivision.Items.Clear();
            ddlDivision.Items.Add(new ListItem("جميع الشعب", ""));

            int gradeLevelId;
            if (!int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
                return;

            int yearId;
            int.TryParse(ddlAcademicYear.SelectedValue, out yearId);

            string selectedSection = GetSelectedSection();

            IQueryable<ClassRoom> query = db.ClassRooms.AsNoTracking()
                .Where(c => c.GradeLevelId == gradeLevelId);

            if (yearId > 0)
                query = query.Where(c => c.AcademicYearId == yearId);

            if (!string.IsNullOrWhiteSpace(selectedSection))
                query = query.Where(c => c.SectionName == selectedSection ||
                                          c.Name.Contains(selectedSection));

            var rooms = query
                .OrderBy(c => c.DivisionName)
                .ThenBy(c => c.Name)
                .Select(c => new { c.Id, c.Name, c.DivisionName, c.SectionName })
                .ToList();

            var roomInfos = rooms.Select(r => new
            {
                r.Id,
                r.Name,
                Letter = !string.IsNullOrWhiteSpace(r.DivisionName)
                            ? r.DivisionName.Trim()
                            : ExtractDivisionLetter(r.Name),
                Section = !string.IsNullOrWhiteSpace(r.SectionName)
                            ? r.SectionName.Trim()
                            : ExtractSectionFromName(r.Name)
            }).ToList();

            char nextLetter = 'أ';
            var finalRooms = roomInfos.Select(r =>
            {
                string letter = !string.IsNullOrWhiteSpace(r.Letter)
                    ? r.Letter
                    : (nextLetter++).ToString();

                return new { r.Id, Letter = letter, Section = r.Section };
            }).ToList();

            var duplicateLetters = finalRooms
                .GroupBy(r => r.Letter)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            var added = new HashSet<string>();

            foreach (var r in finalRooms)
            {
                string displayName = (duplicateLetters.Contains(r.Letter) && !string.IsNullOrWhiteSpace(r.Section))
                    ? "شعبة " + r.Letter + " - " + r.Section
                    : "شعبة " + r.Letter;

                if (added.Contains(displayName)) continue;

                added.Add(displayName);
                ddlDivision.Items.Add(new ListItem(displayName, r.Letter));
            }

            if (!string.IsNullOrWhiteSpace(previousValue))
            {
                ListItem prev = ddlDivision.Items.FindByValue(previousValue);
                if (prev != null)
                {
                    ddlDivision.ClearSelection();
                    prev.Selected = true;
                }
            }
        }

        // =========================================================
        // الطلاب
        // =========================================================
        private void LoadStudents()
        {
            ddlStudent.Items.Clear();
            ddlStudent.Items.Add(new ListItem("— اختر التلميذ —", ""));

            int gradeLevelId;
            if (!int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
                return;

            int yearId;
            int.TryParse(ddlAcademicYear.SelectedValue, out yearId);

            string section = GetSelectedSection();
            string division = string.IsNullOrWhiteSpace(ddlDivision.SelectedValue)
                                ? null
                                : ddlDivision.SelectedValue.Trim();

            EducationStage stage = EnumHelper.GetCurrentStage();

            IQueryable<Student> query = db.Students
                .Where(s => s.IsActive
                         && s.Stage == stage
                         && s.ClassRoom != null
                         && s.ClassRoom.GradeLevelId == gradeLevelId);

            if (yearId > 0)
                query = query.Where(s => s.ClassRoom.AcademicYearId == yearId);

            if (!string.IsNullOrWhiteSpace(section))
                query = query.Where(s => s.ClassRoom.SectionName == section);

            if (!string.IsNullOrWhiteSpace(division))
                query = query.Where(s => s.ClassRoom.DivisionName == division);

            var students = query
                .OrderBy(s => s.Name)
                .Select(s => new { s.Id, s.Name })
                .ToList();

            foreach (var st in students)
                ddlStudent.Items.Add(new ListItem(st.Name, st.Id.ToString()));
        }

        // =========================================================
        // الأحداث
        // =========================================================
        protected void ddlAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDivisions();
            LoadStudents();
            RenderSheet();
        }

        protected void ddlGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
            LoadDivisions();
            LoadStudents();
            RenderSheet();
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDivisions();
            LoadStudents();
            RenderSheet();
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStudents();
            RenderSheet();
        }

        protected void ddlStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderSheet();
        }

        // =========================================================
        // الحفظ
        // =========================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int studentId;
            if (!int.TryParse(GetSelectedStudentId(), out studentId) || studentId <= 0)
            {
                RenderSheet();
                return;
            }

            SaveGrades(studentId);
            SaveEvaluations(studentId);
            SaveMonths(studentId);
            db.SaveChanges();

            RenderSheet();
            ShowMessage("تم حفظ الدرجات بنجاح.");
        }

        private string GetSelectedStudentId()
        {
            return ddlStudent.SelectedValue;
        }

        private void ShowMessage(string text)
        {
            ClientScript.RegisterStartupScript(GetType(), "saveMsg",
                "alert('" + text.Replace("'", "\\'") + "');", true);
        }

        // =========================================================
        // العرض
        // =========================================================
        private void RenderSheet()
        {
            int studentId;
            if (!int.TryParse(ddlStudent.SelectedValue, out studentId) || studentId <= 0)
            {
                litSheet.Text =
                    "<div class='ms-empty'>" +
                    "<div class='ms-empty-icon'><i class='fas fa-file-alt'></i></div>" +
                    "<h4>اختر الصف والتلميذ</h4>" +
                    "<p>يجب اختيار الصف الدراسي ثم التلميذ لعرض الصحيفة الشهرية</p>" +
                    "</div>";
                return;
            }

            var student = db.Students
                .Include(s => s.ClassRoom)
                .Include(s => s.ClassRoom.GradeLevel)
                .FirstOrDefault(s => s.Id == studentId);

            if (student == null)
            {
                litSheet.Text =
                    "<div class='ms-empty'>" +
                    "<div class='ms-empty-icon'><i class='fas fa-user-slash'></i></div>" +
                    "<h4>التلميذ غير موجود</h4>" +
                    "<p>ربما تم حذفه أو أن الرابط غير صالح</p>" +
                    "</div>";
                return;
            }

            string className = student.ClassRoom != null ? student.ClassRoom.Name : "";
            string academicYear = GetAcademicYearRange();

            litSheet.Text = BuildSheetHtml(student.Name, className, academicYear, studentId, student.ClassRoom);
        }

        private string GetAcademicYearRange()
        {
            var current = db.AcademicYears.FirstOrDefault(a => a.IsCurrent);
            if (current != null && current.StartDate != DateTime.MinValue && current.EndDate != DateTime.MinValue)
            {
                return current.StartDate.Year + " – " + current.EndDate.Year;
            }
            return "2026 – 2027";
        }

        private void SaveGrades(int studentId)
        {
            var existing = db.MonthlySheetGrades.Where(g => g.StudentId == studentId).ToList();

            var student = db.Students
                .Include(s => s.ClassRoom)
                .FirstOrDefault(s => s.Id == studentId);

            foreach (var subject in GetSubjects(student != null ? student.ClassRoom : null))
            {
                for (int month = 1; month <= 3; month++)
                {
                    var grade = existing.FirstOrDefault(g => g.Subject == subject.Key && g.MonthNumber == month);
                    if (grade == null)
                    {
                        grade = new MonthlySheetGrade { StudentId = studentId, Subject = subject.Key, MonthNumber = month };
                        db.MonthlySheetGrades.Add(grade);
                        existing.Add(grade);
                    }

                    grade.DailyActivity = TrimOrNull(Request.Form[GradesField + "_" + subject.Key + "_" + month + "_0"]);
                    grade.ApplicationAverage = TrimOrNull(Request.Form[GradesField + "_" + subject.Key + "_" + month + "_1"]);
                    grade.Total = TrimOrNull(Request.Form[GradesField + "_" + subject.Key + "_" + month + "_2"]);
                }

                if (HasMidtermSection(student != null ? student.ClassRoom : null))
                {
                    var midtermGrade = existing.FirstOrDefault(g => g.Subject == subject.Key && g.MonthNumber == 1);
                    if (midtermGrade == null)
                    {
                        midtermGrade = new MonthlySheetGrade { StudentId = studentId, Subject = subject.Key, MonthNumber = 1 };
                        db.MonthlySheetGrades.Add(midtermGrade);
                        existing.Add(midtermGrade);
                    }
                    midtermGrade.MidtermStudent = TrimOrNull(Request.Form[GradesField + "_" + subject.Key + "_mt"]);
                }
            }
        }

        private void SaveEvaluations(int studentId)
        {
            var existing = db.MonthlySheetEvaluations.Where(e => e.StudentId == studentId).ToList();

            foreach (var cat in GetEvaluationCategories())
            {
                var eval = existing.FirstOrDefault(e => e.Category == cat.Key);
                if (eval == null)
                {
                    eval = new MonthlySheetEvaluation { StudentId = studentId, Category = cat.Key };
                    db.MonthlySheetEvaluations.Add(eval);
                }

                eval.Month1 = TrimOrNull(Request.Form[EvaluationField + "_" + cat.Key + "_1"]);
                eval.Month2 = TrimOrNull(Request.Form[EvaluationField + "_" + cat.Key + "_2"]);
                eval.Month3 = TrimOrNull(Request.Form[EvaluationField + "_" + cat.Key + "_3"]);
            }
        }

        private void SaveMonths(int studentId)
        {
            var months = db.MonthlySheetMonths.FirstOrDefault(m => m.StudentId == studentId);
            if (months == null)
            {
                months = new MonthlySheetMonth { StudentId = studentId };
                db.MonthlySheetMonths.Add(months);
            }

            // ✅ يأخذ القيمة من الحقل الرئيسي (master) — جميع الحقول الأخرى متزامنة معه
            months.Month1 = TrimOrNull(Request.Form[MonthsField + "_1"]);
            months.Month2 = TrimOrNull(Request.Form[MonthsField + "_2"]);
            months.Month3 = TrimOrNull(Request.Form[MonthsField + "_3"]);
        }

        private static string TrimOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return value.Trim();
        }

        // =========================================================
        // بناء الصحيفة
        // =========================================================
        private string BuildSheetHtml(string studentName, string className, string academicYear, int studentId, ClassRoom classroom)
        {
            var existingGrades = db.MonthlySheetGrades.Where(g => g.StudentId == studentId).ToList();
            var existingEvaluations = db.MonthlySheetEvaluations.Where(e => e.StudentId == studentId).ToList();

            bool isSixth = HasMidtermSection(classroom);

            var savedMonths = db.MonthlySheetMonths.FirstOrDefault(m => m.StudentId == studentId);
            string m1 = savedMonths != null ? savedMonths.Month1 : "";
            string m2 = savedMonths != null ? savedMonths.Month2 : "";
            string m3 = savedMonths != null ? savedMonths.Month3 : "";

            var sb = new StringBuilder();

            sb.Append("<style>");
            if (isSixth)
                sb.Append("@page{size:A4 landscape;margin:7mm;}");
            else
                sb.Append("@page{size:A4 portrait;margin:8mm;}");

            sb.Append(".report-container{direction:rtl;font-family:'Droid Arabic Kufi','Segoe UI',sans-serif;width:100%;box-sizing:border-box;max-width:100%;}*{box-sizing:border-box;}");
            sb.Append(".report-container table{width:100%;border-collapse:collapse;font-size:10px;font-weight: bold;table-layout:fixed;border:1px solid #000;}");
            sb.Append(".report-container th,.report-container td{border:1px solid #000;padding:4px 4px;text-align:center;vertical-align:middle;overflow:hidden;word-break:break-word;}");
            sb.Append(".report-container thead th{background:#e8e8e8;font-weight:bold;color:#111;}");
            sb.Append(".header-title{text-align:center;font-size:12px;font-weight:bold;margin-bottom:30px;line-height:1.6;}");
            sb.Append(".header-title .school-name{font-size:14px;}");

            // ✅ تنسيق حقل إدخال الشهر (يستخدم في كل الجداول)
            sb.Append(".report-container .month-input{width:60px;max-width:100%;border:1px solid #b3b2b2;background:#e8e8e8;text-align:center;font-size:11px;font-family:inherit;font-weight:bold;box-sizing:border-box;border-radius:20px;padding:1px 3px;margin:0 2px;}");
            sb.Append(".report-container .month-input:focus{outline:none;background:#fff8e1;border-color:#00b14f;}");
            sb.Append(".report-container .month-input::placeholder{color:#999;font-weight:normal;}");
            sb.Append(".mobile-sign-month-editor{display:none;}");
            sb.Append(".mobile-sign-month-field{display:flex;flex-direction:column;gap:5px;font-size:10px;font-weight:bold;color:#333;min-width:0;}");
            sb.Append("@media screen and (max-width:767.98px){.mobile-sign-month-editor{display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:8px;margin:0 0 10px;padding:10px;border:1px solid #d9e7de;border-radius:10px;background:#f8fcf9;}.mobile-sign-month-editor .month-input{width:100%;min-width:0;max-width:100%;height:36px;margin:0;padding:4px 5px;background:#fff;border-radius:8px;font-size:12px;}}");

            sb.Append(".report-container .grade-input,.report-container .eval-input{width:100%;max-width:100%;border:none;background:transparent;text-align:center;font-size:11px;font-family:inherit; font-weight: bold;box-sizing:border-box;padding:1px;margin:0;}");
            sb.Append(".report-container .grade-input:focus,.report-container .eval-input:focus{outline:none;background:#fff8e1;}");
            sb.Append(".section-title{font-size:12px;font-weight:bold;color:#111;margin:12px 0 4px;padding:5px 8px;border-radius:2px;margin-bottom: 20px;}");
            sb.Append(".eval-table tbody th,.sign-table tbody th{font-weight:normal;}");
            sb.Append(".sign-table td{height:34px;}");
            sb.Append(".footer-note{margin-top:10px;font-size:10px;text-align:center;border:1px solid #000;padding:5px;line-height:1.6;}");
            if (isSixth)
            {
                sb.Append(".report-container.sixth thead th{font-size:9px;}");
                sb.Append(".report-container.sixth table{font-size:9.5px;}");
            }
            sb.Append("@media print{");
            sb.Append("html,body{width:auto!important;overflow:visible!important;}");
            sb.Append(".sidebar,.sidebar *,.top-header,.footer,.nav-link,.nav-section-title,.stage-selector,.sidebar-brand{display:none!important;}");
            sb.Append(".main-content,.content-wrapper,.container-fluid{margin:0!important;padding:0!important;max-width:100%!important;}");
            sb.Append(".report-container{width:100%!important;max-width:100%!important;margin:0!important;}");
            sb.Append(".report-container table{table-layout:fixed!important;width:100%!important;}");
            sb.Append(".report-container .grade-input,.report-container .eval-input{border:none!important;background:none!important;}");
            sb.Append(".report-container thead th{background:#e8e8e8!important;-webkit-print-color-adjust:exact;print-color-adjust:exact;}");
            sb.Append(".section-title{-webkit-print-color-adjust:exact;print-color-adjust:exact;}");
            sb.Append(".report-container th,.report-container td{padding:3px 2px!important;}");
            sb.Append("}");
            sb.Append("</style>");

            // ====== الترويسة ======
            sb.Append("<div class='report-container").Append(isSixth ? " sixth" : "").Append("'>");
            sb.Append("<div class='header-title'><span class='school-name'>مدرسة نور الملائكة</span><br />الصحيفة الشهرية للتلميذ</div>");

            // ====== معلومات التلميذ ======
            sb.Append("<table style='margin-bottom:20px;'>");
            sb.Append("<tr><td style='text-align:right; padding: 8px 6px; font-weight: 700;'>اسم الطالب : &nbsp;").Append(HttpUtility.HtmlEncode(studentName)).Append("&nbsp;</td>");
            sb.Append("<td style='text-align:right; padding: 8px 6px; font-weight: 700; '>الصف الدراسي : &nbsp;").Append(HttpUtility.HtmlEncode(className)).Append("&nbsp;</td>");
            sb.Append("<td style='text-align:right; padding: 8px 6px; font-weight: 700;'>العام الدراسي : &nbsp;").Append(HttpUtility.HtmlEncode(academicYear)).Append("&nbsp;</td>");
            sb.Append("</tr></table>");

            // =====================================================
            // ✅ جدول المواد — الحقول الرئيسية (master) للشهور
            // =====================================================
            sb.Append("<table >");
            if (isSixth)
            {
                sb.Append("<colgroup>");
                sb.Append("<col style='width:9%' />");
                sb.Append("<col style='width:7%' />");
                sb.Append("<col style='width:6%' />");
                sb.Append("<col style='width:6%' />");
                for (int k = 0; k < 9; k++)
                    sb.Append("<col style='width:6%' />");
                sb.Append("<col style='width:9%' />");
                sb.Append("<col style='width:9%' />");
                sb.Append("</colgroup>");
            }
            sb.Append("<thead><tr>");
            sb.Append("<th rowspan='2'>المواد</th>");
            sb.Append("<th colspan='3'>الدرجة المخصصة للمادة</th>");

            // ✅ حقول الشهور (master) — هذه التي ستُحفظ في قاعدة البيانات
            sb.Append("<th colspan='3'>الشهر ( ")
              .Append(MakeMonthInput(MonthsField + "_1", m1, true, "الشهر"))
              .Append(" )</th>");
            sb.Append("<th colspan='3'>الشهر ( ")
              .Append(MakeMonthInput(MonthsField + "_2", m2, true, "الشهر"))
              .Append(" )</th>");
            sb.Append("<th colspan='3'>الشهر ( ")
              .Append(MakeMonthInput(MonthsField + "_3", m3, true, "الشهر"))
              .Append(" )</th>");

            if (isSixth)
                sb.Append("<th colspan='2'>النصفي</th>");
            sb.Append("</tr><tr>");
            int monthColGroups = isSixth ? 5 : 4;
            for (int i = 0; i < monthColGroups; i++)
            {
                if (isSixth && i == 4)
                {
                    sb.Append("<th>درجة الامتحان النصفي</th>");
                    sb.Append("<th>درجة التلميذ</th>");
                }
                else
                {
                    sb.Append("<th>الأنشطة اليومية</th>");
                    sb.Append("<th>متوسط التطبيقات</th>");
                    sb.Append("<th>المجموع</th>");
                }
            }
            sb.Append("</tr></thead><tbody>");

            var subjects = GetSubjects(classroom);
            foreach (var row in subjects)
            {
                sb.Append("<tr>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(row.Name)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(row.Activity)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(row.Average)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(row.Sum)).Append("</td>");

                for (int month = 1; month <= 3; month++)
                {
                    var grade = existingGrades.FirstOrDefault(g => g.Subject == row.Key && g.MonthNumber == month);
                    sb.Append("<td>").Append(GradeInput(GradesField + "_" + row.Key + "_" + month + "_0", grade != null ? grade.DailyActivity : null)).Append("</td>");
                    sb.Append("<td>").Append(GradeInput(GradesField + "_" + row.Key + "_" + month + "_1", grade != null ? grade.ApplicationAverage : null)).Append("</td>");
                    sb.Append("<td>").Append(GradeInput(GradesField + "_" + row.Key + "_" + month + "_2", grade != null ? grade.Total : null)).Append("</td>");
                }
                if (isSixth)
                {
                    var midtermGrade = existingGrades.FirstOrDefault(g => g.Subject == row.Key && g.MonthNumber == 1);
                    sb.Append("<td>").Append(HttpUtility.HtmlEncode(row.Sum)).Append("</td>");
                    sb.Append("<td>").Append(GradeInput(GradesField + "_" + row.Key + "_mt", midtermGrade != null ? midtermGrade.MidtermStudent : null)).Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");

            // =====================================================
            // ✅ المستوى العام للتلميذ — مع حقول الشهر القابلة للتحرير
            // =====================================================
            sb.Append("<h4 class='section-title' style='margin-top: 30px;' > المستوى العام للتلميذ</h4>");
            sb.Append("<table class='eval-table' style='width:60%; align: center;'>");
            sb.Append("<colgroup><col style='width:40%' /><col style='width:20%' /><col style='width:20%' /><col style='width:20%' /></colgroup>");
            sb.Append("<thead><tr>");
            sb.Append("<th style='text-align:right;background-color:#f2f2f2;'>البند</th>");

            // ✅ حقول الشهر القابلة للتحرير (متزامنة مع الحقول الرئيسية)
            sb.Append("<th style='background-color:#f2f2f2;'>الشهر ( ")
              .Append(MakeMonthInput(MonthsField + "_1", m1, false, "الشهر"))
              .Append(" )</th>");
            sb.Append("<th style='background-color:#f2f2f2;'>الشهر ( ")
              .Append(MakeMonthInput(MonthsField + "_2", m2, false, "الشهر"))
              .Append(" )</th>");
            sb.Append("<th style='background-color:#f2f2f2;'>الشهر ( ")
              .Append(MakeMonthInput(MonthsField + "_3", m3, false, "الشهر"))
              .Append(" )</th>");

            sb.Append("</tr></thead><tbody>");
            foreach (var cat in GetEvaluationCategories())
            {
                var eval = existingEvaluations.FirstOrDefault(e => e.Category == cat.Key);
                sb.Append("<tr>");
                sb.Append("<th style='text-align:right;background-color:#fafafa; font-weight: bold;'>").Append(cat.Label).Append("</th>");
                sb.Append("<td>").Append(EvalInput(EvaluationField + "_" + cat.Key + "_1", ConvertToInput(eval != null ? eval.Month1 : null))).Append("</td>");
                sb.Append("<td>").Append(EvalInput(EvaluationField + "_" + cat.Key + "_2", ConvertToInput(eval != null ? eval.Month2 : null))).Append("</td>");
                sb.Append("<td>").Append(EvalInput(EvaluationField + "_" + cat.Key + "_3", ConvertToInput(eval != null ? eval.Month3 : null))).Append("</td>");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");

            // =====================================================
            // ✅ التوقيعات — مع حقول الشهر القابلة للتحرير
            // =====================================================
            sb.Append("<h4 class='section-title' style='margin-top: 30px;'>التوقيعات</h4>");
            sb.Append("<div class='mobile-sign-month-editor no-print' aria-label='إدخال أسماء الأشهر'>");
            sb.Append("<label class='mobile-sign-month-field'>الشهر الأول")
              .Append(MakeMonthInput(MonthsField + "_1", m1, false, "اسم الشهر"))
              .Append("</label>");
            sb.Append("<label class='mobile-sign-month-field'>الشهر الثاني")
              .Append(MakeMonthInput(MonthsField + "_2", m2, false, "اسم الشهر"))
              .Append("</label>");
            sb.Append("<label class='mobile-sign-month-field'>الشهر الثالث")
              .Append(MakeMonthInput(MonthsField + "_3", m3, false, "اسم الشهر"))
              .Append("</label>");
            sb.Append("</div>");
            sb.Append("<table class='sign-table'>");
            sb.Append("<colgroup><col style='width:25%' /><col style='width:25%' /><col style='width:25%' /><col style='width:25%' /></colgroup>");
            sb.Append("<thead><tr>");
            sb.Append("<th style='background-color:#f2f2f2;text-align:center;'>الصفة</th>");

            // ✅ حقول الشهر القابلة للتحرير (متزامنة)
            sb.Append("<th style='background-color:#f2f2f2;'>الشهر ( ")
              .Append(MakeMonthInput(MonthsField + "_1", m1, false, "الشهر"))
              .Append(" )</th>");
            sb.Append("<th style='background-color:#f2f2f2;'>الشهر ( ")
              .Append(MakeMonthInput(MonthsField + "_2", m2, false, "الشهر"))
              .Append(" )</th>");
            sb.Append("<th style='background-color:#f2f2f2;'>الشهر ( ")
              .Append(MakeMonthInput(MonthsField + "_3", m3, false, "الشهر"))
              .Append(" )</th>");

            sb.Append("</tr></thead><tbody>");
            foreach (var role in new[] { "الدراسة والامتحانات", "مدير المدرسة", "ولي الأمر" })
            {
                sb.Append("<tr>");
                sb.Append("<td style='text-align:right;background-color:#fafafa;'>").Append(role).Append("</td>");
                sb.Append("<td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");

            // ملاحظة
            sb.Append("<div class='footer-note' style='margin-top: 30px;'>");
            sb.Append("ملاحظة: ولي أمر التلميذ هذه الصحيفة الشهرية هي توضيح لمستوى ابنكم أو ابنتكم شهرياً لغرض مساهمتكم مع المدرسة في الرفع من المستوى التعليمي لابنكم ،،،،، وليست وثيقة رسمية");
            sb.Append("</div>");

            // =====================================================
            // ✅ JavaScript — لمزامنة حقول الشهور بين جميع الجداول
            // =====================================================
            sb.Append("<script type='text/javascript'>");
            sb.Append("(function() {");
            sb.Append("  function syncMonthInputs(e) {");
            sb.Append("    var t = e.target;");
            sb.Append("    if (!t || !t.hasAttribute) return;");
            sb.Append("    if (!t.hasAttribute('data-sync')) return;");
            sb.Append("    var key = t.getAttribute('data-sync');");
            sb.Append("    var val = t.value;");
            sb.Append("    var all = document.querySelectorAll('[data-sync=\"' + key + '\"]');");
            sb.Append("    for (var i = 0; i < all.length; i++) {");
            sb.Append("      if (all[i] !== t) all[i].value = val;");
            sb.Append("    }");
            sb.Append("  }");
            sb.Append("  document.addEventListener('input', syncMonthInputs, true);");
            sb.Append("  document.addEventListener('change', syncMonthInputs, true);");
            sb.Append("})();");
            sb.Append("</script>");

            sb.Append("</div>"); // report-container
            return sb.ToString();
        }

        // =========================================================
        // ✅ إنشاء حقل إدخال الشهر
        // isMaster=true: هذا هو الحقل الرئيسي الذي يُرسل للـ Server (name)
        // isMaster=false: حقل مزامنة فقط (data-sync فقط)
        // =========================================================
        private static string MakeMonthInput(string fieldName, string value, bool isMaster, string placeholder)
        {
            string v = value == null ? "" : value;
            string ph = string.IsNullOrEmpty(placeholder) ? "" : " placeholder='" + HttpUtility.HtmlAttributeEncode(placeholder) + "'";

            var sb = new StringBuilder();
            sb.Append("<input type='text'");
            sb.Append(" class='month-input'");

            // الحقل الرئيسي: لديه name ليُرسل في POST
            if (isMaster)
                sb.Append(" name='").Append(fieldName).Append("'");

            // جميع الحقول (الرئيسية والفرعية) لديها data-sync للمزامنة
            sb.Append(" data-sync='").Append(fieldName).Append("'");
            sb.Append(" value='").Append(HttpUtility.HtmlAttributeEncode(v)).Append("'");
            sb.Append(ph);
            sb.Append(" />");

            return sb.ToString();
        }

        private static string GradeInput(string name, string value)
        {
            string v = value == null ? "" : value;
            return "<input type='text' class='grade-input' name='" + name + "' value='" + HttpUtility.HtmlEncode(v) + "' />";
        }

        private static string EvalInput(string name, string value)
        {
            return "<input type='text' class='eval-input' name='" + name + "' value='" + HttpUtility.HtmlEncode(value) + "' />";
        }

        private static string ConvertToInput(string value)
        {
            return value == null ? "" : value;
        }

        private static IEnumerable<EvaluationCategory> GetEvaluationCategories()
        {
            return new List<EvaluationCategory>
            {
                new EvaluationCategory { Key = "سلوك", Label = "السلوك" },
                new EvaluationCategory { Key = "واجب", Label = "الاهتمام بالواجب" },
                new EvaluationCategory { Key = "انتباه", Label = "الانتباه" },
                new EvaluationCategory { Key = "علاقة", Label = "علاقته مع زملائه" },
                new EvaluationCategory { Key = "غياب", Label = "الغياب" }
            };
        }

        private static IEnumerable<SubjectRow> GetSubjects(ClassRoom classroom)
        {
            if (IsFourthOrFifthGradeBasic(classroom) || HasMidtermSection(classroom))
            {
                if (IsNinthGradeBasic(classroom))
                {
                    return new List<SubjectRow>
                    {
                        new SubjectRow { Key = "تربية", Name = "التربية الإسلامية", Activity = "3", Average = "3", Sum = "6" },
                        new SubjectRow { Key = "عربية", Name = "اللغة العربية", Activity = "6", Average = "6", Sum = "12" },
                        new SubjectRow { Key = "كتابة", Name = "الكتابة", Activity = "3", Average = "3", Sum = "6" },
                        new SubjectRow { Key = "إنجليزية", Name = "اللغة الإنجليزية", Activity = "6", Average = "6", Sum = "12" },
                        new SubjectRow { Key = "حاسوب", Name = "الحاسوب", Activity = "3", Average = "3", Sum = "6" },
                        new SubjectRow { Key = "رياضيات", Name = "الرياضيات", Activity = "8", Average = "7", Sum = "15" },
                        new SubjectRow { Key = "علوم", Name = "العلوم", Activity = "8", Average = "7", Sum = "15" },
                        new SubjectRow { Key = "تاريخ", Name = "التاريخ", Activity = "3", Average = "3", Sum = "6" },
                        new SubjectRow { Key = "جغرافيا", Name = "الجغرافيا", Activity = "3", Average = "3", Sum = "6" }
                    };
                }

                if (IsSeventhOrEighthGradeBasic(classroom))
                {
                    return new List<SubjectRow>
                    {
                        new SubjectRow { Key = "تربية", Name = "التربية الإسلامية", Activity = "4", Average = "4", Sum = "8" },
                        new SubjectRow { Key = "عربية", Name = "اللغة العربية", Activity = "8", Average = "8", Sum = "16" },
                        new SubjectRow { Key = "كتابة", Name = "الكتابة", Activity = "4", Average = "4", Sum = "8" },
                        new SubjectRow { Key = "إنجليزية", Name = "اللغة الإنجليزية", Activity = "8", Average = "8", Sum = "16" },
                        new SubjectRow { Key = "حاسوب", Name = "الحاسوب", Activity = "4", Average = "4", Sum = "8" },
                        new SubjectRow { Key = "رياضيات", Name = "الرياضيات", Activity = "10", Average = "10", Sum = "20" },
                        new SubjectRow { Key = "علوم", Name = "العلوم", Activity = "8", Average = "8", Sum = "16" },
                        new SubjectRow { Key = "تاريخ", Name = "التاريخ", Activity = "4", Average = "4", Sum = "8" },
                        new SubjectRow { Key = "جغرافيا", Name = "الجغرافيا", Activity = "4", Average = "4", Sum = "8" }
                    };
                }

                if (IsSixthGradeBasic(classroom))
                {
                    return new List<SubjectRow>
                    {
                        new SubjectRow { Key = "تربية", Name = "التربية الإسلامية", Activity = "6", Average = "6", Sum = "12" },
                        new SubjectRow { Key = "عربية", Name = "اللغة العربية", Activity = "8", Average = "8", Sum = "16" },
                        new SubjectRow { Key = "كتابة", Name = "الكتابة", Activity = "4", Average = "4", Sum = "8" },
                        new SubjectRow { Key = "إنجليزية", Name = "اللغة الإنجليزية", Activity = "8", Average = "8", Sum = "16" },
                        new SubjectRow { Key = "حاسوب", Name = "الحاسوب", Activity = "4", Average = "4", Sum = "8" },
                        new SubjectRow { Key = "رياضيات", Name = "الرياضيات", Activity = "10", Average = "10", Sum = "20" },
                        new SubjectRow { Key = "علوم", Name = "العلوم", Activity = "6", Average = "6", Sum = "12" },
                        new SubjectRow { Key = "اجتماعيات", Name = "الاجتماعيات", Activity = "4", Average = "4", Sum = "8" }
                    };
                }

                return new List<SubjectRow>
                {
                    new SubjectRow { Key = "تربية", Name = "التربية الإسلامية", Activity = "18", Average = "18", Sum = "36" },
                    new SubjectRow { Key = "عربية", Name = "اللغة العربية", Activity = "24", Average = "24", Sum = "48" },
                    new SubjectRow { Key = "كتابة", Name = "الكتابة", Activity = "12", Average = "12", Sum = "24" },
                    new SubjectRow { Key = "إنجليزية", Name = "اللغة الإنجليزية", Activity = "18", Average = "18", Sum = "36" },
                    new SubjectRow { Key = "حاسوب", Name = "الحاسوب", Activity = "6", Average = "6", Sum = "12" },
                    new SubjectRow { Key = "رياضيات", Name = "الرياضيات", Activity = "30", Average = "30", Sum = "60" },
                    new SubjectRow { Key = "علوم", Name = "العلوم", Activity = "18", Average = "18", Sum = "36" },
                    new SubjectRow { Key = "اجتماعيات", Name = "الاجتماعيات", Activity = "12", Average = "12", Sum = "24" }
                };
            }

            var list = new List<SubjectRow>
            {
                new SubjectRow { Key = "تربية", Name = "التربية الإسلامية", Activity = "18", Average = "18", Sum = "36" },
                new SubjectRow { Key = "عربية", Name = "اللغة العربية", Activity = "36", Average = "36", Sum = "72" },
                new SubjectRow { Key = "إنجليزية", Name = "اللغة الإنجليزية", Activity = "20", Average = "16", Sum = "36" },
                new SubjectRow { Key = "حاسوب", Name = "الحاسوب", Activity = "6", Average = "6", Sum = "12" },
                new SubjectRow { Key = "رياضيات", Name = "الرياضيات", Activity = "30", Average = "30", Sum = "60" }
            };

            if (IsThirdGradeBasic(classroom))
                list.Add(new SubjectRow { Key = "علوم", Name = "العلوم", Activity = "24", Average = "12", Sum = "36" });

            return list;
        }

        private static bool IsThirdGradeBasic(ClassRoom classroom) =>
            classroom != null && !string.IsNullOrEmpty(classroom.Name) && classroom.Name.Contains("الصف الثالث");

        private static bool IsFourthOrFifthGradeBasic(ClassRoom classroom) =>
            classroom != null && !string.IsNullOrEmpty(classroom.Name) &&
            (classroom.Name.Contains("الصف الرابع") || classroom.Name.Contains("الصف الخامس"));

        private static bool IsSixthGradeBasic(ClassRoom classroom) =>
            classroom != null && !string.IsNullOrEmpty(classroom.Name) && classroom.Name.Contains("الصف السادس");

        private static bool IsSeventhOrEighthGradeBasic(ClassRoom classroom) =>
            classroom != null && !string.IsNullOrEmpty(classroom.Name) &&
            (classroom.Name.Contains("الصف السابع") || classroom.Name.Contains("الصف الثامن"));

        private static bool IsNinthGradeBasic(ClassRoom classroom) =>
            classroom != null && !string.IsNullOrEmpty(classroom.Name) && classroom.Name.Contains("الصف التاسع");

        private static bool HasMidtermSection(ClassRoom classroom) =>
            IsSixthGradeBasic(classroom) || IsSeventhOrEighthGradeBasic(classroom) || IsNinthGradeBasic(classroom);

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }

        private class SubjectRow
        {
            public string Key { get; set; }
            public string Name { get; set; }
            public string Activity { get; set; }
            public string Average { get; set; }
            public string Sum { get; set; }
        }

        private class EvaluationCategory
        {
            public string Key { get; set; }
            public string Label { get; set; }
        }
    }
}
