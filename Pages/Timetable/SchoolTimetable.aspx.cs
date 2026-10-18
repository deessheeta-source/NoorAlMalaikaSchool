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


namespace NoorAlMalaikaSchool.Pages.Timetable
{
    public partial class SchoolTimetable : Page
    {
        public const int PeriodCount = 7;

        private readonly SchoolDbContext db = new SchoolDbContext();

        // =========================================================
        // تحميل الصفحة
        // =========================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            EducationStage stage = EnumHelper.GetCurrentStage();

            lblStage.Text = EnumHelper.GetStageArabic(stage.ToString());

            if (!IsPostBack)
            {
                // تعبئة القوائم عند أول تحميل فقط.
                PopulateGradeLevels(stage);
                PopulateSectionAndDivisionLists();

                // إذا وصلت الصفحة من رابط فصل محدد.
                SelectClassFromQueryString();

                // ضبط ظهور قائمة القسم حسب الصف المحدد.
                ConfigureSectionFilter();

                // عرض الجدول.
                LoadTimetable();
            }
            else
            {
                // تحديث ظهور القسم فقط دون إعادة تعبئة القوائم،
                // حتى لا نفقد الاختيارات الحالية.
                ConfigureSectionFilter();
            }
        }

        // =========================================================
        // تعبئة الصفوف الدراسية حسب المرحلة الحالية
        // =========================================================
        private void PopulateGradeLevels(EducationStage stage)
        {
            var gradeLevels = db.GradeLevels
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlGradeLevel.Items.Clear();

            ddlGradeLevel.Items.Add(
                new ListItem("-- اختر الصف الدراسي --", "")
            );

            foreach (var grade in gradeLevels)
            {
                ddlGradeLevel.Items.Add(
                    new ListItem(grade.Name, grade.Id.ToString())
                );
            }
        }

        // =========================================================
        // تعبئة قائمة الأقسام والشعب
        // =========================================================
        private void PopulateSectionAndDivisionLists()
        {
            // الأقسام الدراسية.
            ddlSection.Items.Clear();

            ddlSection.Items.Add(
                new ListItem("-- بدون قسم --", "")
            );

            ddlSection.Items.Add(
                new ListItem("القسم العلمي", "علمي")
            );

            ddlSection.Items.Add(
                new ListItem("القسم الأدبي", "أدبي")
            );

            // الشعب الدراسية.
            ddlDivision.Items.Clear();

            ddlDivision.Items.Add(
                new ListItem("-- بدون شعبة --", "")
            );

            ddlDivision.Items.Add(
                new ListItem("الشعبة أ", "أ")
            );

            ddlDivision.Items.Add(
                new ListItem("الشعبة ب", "ب")
            );

            ddlDivision.Items.Add(
                new ListItem("الشعبة ج", "ج")
            );

            ddlDivision.Items.Add(
                new ListItem("الشعبة د", "د")
            );
        }

        // =========================================================
        // اختيار الفصل من معامل الرابط ?class=ID
        // =========================================================
        private void SelectClassFromQueryString()
        {
            string classValue = Request.QueryString["class"];

            if (string.IsNullOrWhiteSpace(classValue))
                return;

            int classId;

            if (!int.TryParse(classValue, out classId) || classId <= 0)
                return;

            var classroom = db.ClassRooms.Find(classId);

            if (classroom == null)
                return;

            // اختيار الصف.
            ListItem gradeItem =
                ddlGradeLevel.Items.FindByValue(
                    classroom.GradeLevelId.ToString()
                );

            if (gradeItem == null)
                return;

            ddlGradeLevel.ClearSelection();
            gradeItem.Selected = true;

            // تحديد ما إذا كان القسم مطلوبًا لهذا الصف.
            ConfigureSectionFilter();

            // اختيار القسم إن كان موجودًا ضمن القائمة.
            string sectionName = classroom.SectionName ?? "";

            ListItem sectionItem =
                ddlSection.Items.FindByValue(sectionName);

            if (sectionItem != null)
            {
                ddlSection.ClearSelection();
                sectionItem.Selected = true;
            }

            // اختيار الشعبة إن كانت موجودة ضمن القائمة.
            string divisionName = classroom.DivisionName ?? "";

            ListItem divisionItem =
                ddlDivision.Items.FindByValue(divisionName);

            if (divisionItem != null)
            {
                ddlDivision.ClearSelection();
                divisionItem.Selected = true;
            }
        }

        // =========================================================
        // تغيير الصف الدراسي
        // =========================================================
        protected void ddlGradeLevel_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            ConfigureSectionFilter();
            LoadTimetable();
        }

        // =========================================================
        // تغيير القسم
        // =========================================================
        protected void ddlSection_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            LoadTimetable();
        }

        // =========================================================
        // تغيير الشعبة
        // =========================================================
        protected void ddlDivision_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            LoadTimetable();
        }

        // =========================================================
        // تحديد هل يجب إظهار قائمة القسم
        // تظهر للصف الثاني والثالث الثانوي فقط
        // =========================================================
        private void ConfigureSectionFilter()
        {
            int gradeLevelId;

            if (!int.TryParse(
                    ddlGradeLevel.SelectedValue,
                    out gradeLevelId)
                || gradeLevelId <= 0)
            {
                pnlSectionFilter.Visible = false;
                return;
            }

            string gradeName = db.GradeLevels
                .Where(g => g.Id == gradeLevelId)
                .Select(g => g.Name)
                .FirstOrDefault();

            bool showSection =
                !string.IsNullOrWhiteSpace(gradeName)
                &&
                (
                    gradeName.Trim() == "ثاني ثانوي"
                    ||
                    gradeName.Trim() == "ثالث ثانوي"
                );

            pnlSectionFilter.Visible = showSection;

            // عند اختيار صف لا يحتاج إلى قسم،
            // نعتمد قيمة القسم الفارغة.
            if (!showSection)
            {
                ListItem emptySection =
                    ddlSection.Items.FindByValue("");

                if (emptySection != null)
                {
                    ddlSection.ClearSelection();
                    emptySection.Selected = true;
                }
            }
        }

        // =========================================================
        // البحث عن الفصل المطابق للاختيارات
        // =========================================================
        private ClassRoom FindClassRoom()
        {
            int gradeLevelId;

            if (!int.TryParse(
                    ddlGradeLevel.SelectedValue,
                    out gradeLevelId)
                || gradeLevelId <= 0)
            {
                return null;
            }

            string section = "";

            if (pnlSectionFilter.Visible)
                section = ddlSection.SelectedValue ?? "";

            string division = ddlDivision.SelectedValue ?? "";

            // السنة الدراسية الحالية.
            var currentYear = db.AcademicYears
                .FirstOrDefault(y => y.IsCurrent);

            int yearId = currentYear != null
                ? currentYear.Id
                : 0;

            var query = db.ClassRooms
                .Where(c => c.GradeLevelId == gradeLevelId);

            // عند وجود سنة حالية، نبحث ضمنها فقط.
            if (yearId > 0)
            {
                query = query.Where(
                    c => c.AcademicYearId == yearId
                );
            }

            // مطابقة القسم.
            if (!string.IsNullOrWhiteSpace(section))
            {
                query = query.Where(
                    c => c.SectionName == section
                );
            }
            else
            {
                query = query.Where(
                    c => c.SectionName == null
                         || c.SectionName == ""
                );
            }

            // مطابقة الشعبة.
            if (!string.IsNullOrWhiteSpace(division))
            {
                query = query.Where(
                    c => c.DivisionName == division
                );
            }
            else
            {
                query = query.Where(
                    c => c.DivisionName == null
                         || c.DivisionName == ""
                );
            }

            return query
                .OrderBy(c => c.Id)
                .FirstOrDefault();
        }

        // =========================================================
        // تحميل وعرض الجدول الدراسي
        // =========================================================
        protected void LoadTimetable()
        {
            ClassRoom classroom = FindClassRoom();

            if (classroom == null)
            {
                ShowEmptyTimetable();
                return;
            }

            var entries = db.TimetableEntries
                .Include(e => e.Subject)
                .Include(e => e.Teacher)
                .Where(e => e.ClassRoomId == classroom.Id)
                .ToList();

            // تحويل الحصص إلى قاموس لتسهيل الوصول إليها.
            var byCell = entries
                .GroupBy(e =>
                    ((int)e.DayOfWeek)
                    + "-"
                    + e.PeriodNumber)
                .ToDictionary(
                    g => g.Key,
                    g => g.First()
                );

            var days = new[]
            {
                WeekDay.Sunday,
                WeekDay.Monday,
                WeekDay.Tuesday,
                WeekDay.Wednesday,
                WeekDay.Thursday
            };

            StringBuilder sb = new StringBuilder();

            sb.Append(
                "<div class='table-responsive timetable-responsive'>"
            );

            sb.Append("<table class='table-timetable'>");

            // رأس الجدول.
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th>اليوم</th>");

            for (int period = 1; period <= PeriodCount; period++)
            {
                sb.Append("<th>")
                  .Append("الحصة ")
                  .Append(period)
                  .Append("</th>");
            }

            sb.Append("</tr>");
            sb.Append("</thead>");

            // محتوى الجدول.
            sb.Append("<tbody>");

            foreach (WeekDay day in days)
            {
                sb.Append("<tr>");

                sb.Append("<th>")
                  .Append(HttpUtility.HtmlEncode(GetDayName(day)))
                  .Append("</th>");

                for (int period = 1; period <= PeriodCount; period++)
                {
                    string cellKey =
                        ((int)day) + "-" + period;

                    TimetableEntry entry;

                    if (!byCell.TryGetValue(cellKey, out entry))
                    {
                        sb.Append(
                            "<td class='timetable-cell'></td>"
                        );

                        continue;
                    }

                    string subjectName =
                        entry.Subject != null
                            ? entry.Subject.Name
                            : "—";

                    string teacherName =
                        entry.Teacher != null
                            ? entry.Teacher.Name
                            : "";

                    sb.Append(
                        "<td class='timetable-cell has-entry'>"
                    );

                    sb.Append("<div class='tt-subject'>")
                      .Append(
                          HttpUtility.HtmlEncode(subjectName)
                      )
                      .Append("</div>");

                    if (!string.IsNullOrWhiteSpace(teacherName))
                    {
                        sb.Append("<div class='tt-teacher'>")
                          .Append(
                              HttpUtility.HtmlEncode(teacherName)
                          )
                          .Append("</div>");
                    }

                    sb.Append("</td>");
                }

                sb.Append("</tr>");
            }

            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append("</div>");

            litTimetable.Text = sb.ToString();

            // تفاصيل الفصل المستخدمة في الطباعة.
            UpdatePrintDetails(classroom);
        }

        // =========================================================
        // عرض الرسائل عند عدم وجود فصل مطابق
        // =========================================================
        private void ShowEmptyTimetable()
        {
            int gradeLevelId;

            bool hasGrade =
                int.TryParse(
                    ddlGradeLevel.SelectedValue,
                    out gradeLevelId
                )
                && gradeLevelId > 0;

            if (!hasGrade)
            {
                litTimetable.Text =
                    "<div class='tt-empty'>"
                    + "<div class='tt-empty-icon'>"
                    + "<i class='fas fa-calendar-alt'></i>"
                    + "</div>"
                    + "<h4>اختر الصف الدراسي</h4>"
                    + "<p>يرجى اختيار الصف الدراسي لعرض الجدول الأسبوعي.</p>"
                    + "</div>";
            }
            else
            {
                litTimetable.Text =
                    "<div class='tt-empty'>"
                    + "<div class='tt-empty-icon'>"
                    + "<i class='fas fa-exclamation-circle'></i>"
                    + "</div>"
                    + "<h4>لا يوجد فصل مطابق</h4>"
                    + "<p>لم يتم العثور على فصل بهذه المواصفات "
                    + "في السنة الدراسية الحالية.</p>"
                    + "</div>";
            }

            // مسح تفاصيل الطباعة القديمة.
            litPrintDetails.Text = "";
        }

        // =========================================================
        // تجهيز بيانات الفصل للطباعة
        // =========================================================
        private void UpdatePrintDetails(ClassRoom classroom)
        {
            if (classroom == null)
            {
                litPrintDetails.Text = "";
                return;
            }

            string gradeName = db.GradeLevels
                .Where(g => g.Id == classroom.GradeLevelId)
                .Select(g => g.Name)
                .FirstOrDefault();

            string sectionName =
                string.IsNullOrWhiteSpace(classroom.SectionName)
                    ? ""
                    : classroom.SectionName;

            string divisionName =
                string.IsNullOrWhiteSpace(classroom.DivisionName)
                    ? ""
                    : classroom.DivisionName;

            StringBuilder details = new StringBuilder();

            details.Append("<div class='print-class-details'>");

            if (!string.IsNullOrWhiteSpace(gradeName))
            {
                details.Append("<span><strong>الصف:</strong> ")
                       .Append(HttpUtility.HtmlEncode(gradeName))
                       .Append("</span>");
            }

            if (!string.IsNullOrWhiteSpace(sectionName))
            {
                details.Append("<span><strong>القسم:</strong> ")
                       .Append(HttpUtility.HtmlEncode(sectionName))
                       .Append("</span>");
            }

            if (!string.IsNullOrWhiteSpace(divisionName))
            {
                details.Append("<span><strong>الشعبة:</strong> ")
                       .Append(HttpUtility.HtmlEncode(divisionName))
                       .Append("</span>");
            }

            details.Append("</div>");

            litPrintDetails.Text = details.ToString();
        }

        // =========================================================
        // أسماء أيام الأسبوع
        // =========================================================
        private string GetDayName(WeekDay day)
        {
            switch (day)
            {
                case WeekDay.Sunday:
                    return "الأحد";

                case WeekDay.Monday:
                    return "الاثنين";

                case WeekDay.Tuesday:
                    return "الثلاثاء";

                case WeekDay.Wednesday:
                    return "الأربعاء";

                case WeekDay.Thursday:
                    return "الخميس";

                default:
                    return day.ToString();
            }
        }

        // =========================================================
        // تحرير موارد قاعدة البيانات
        // =========================================================
        protected override void OnUnload(EventArgs e)
        {
            if (db != null)
                db.Dispose();

            base.OnUnload(e);
        }
    }
}