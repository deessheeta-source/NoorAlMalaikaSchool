using System;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Timetable
{
    public partial class Edit : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                PopulateDays();
                PopulatePeriods();
                PopulateGradeLevels();

                int entryId;
                if (int.TryParse(Request.QueryString["id"], out entryId))
                {
                    LoadEntry(entryId);
                    return;
                }

                // Prefill from classId query string
                if (!string.IsNullOrEmpty(Request.QueryString["classId"]))
                {
                    int classId;
                    if (int.TryParse(Request.QueryString["classId"], out classId))
                    {
                        var cr = db.ClassRooms.Find(classId);
                        if (cr != null)
                        {
                            if (ddlGradeLevel.Items.FindByValue(cr.GradeLevelId.ToString()) != null)
                                ddlGradeLevel.SelectedValue = cr.GradeLevelId.ToString();

                            ConfigureSectionFilter();

                            if (!string.IsNullOrEmpty(cr.SectionName)
                                && ddlSection.Items.FindByValue(cr.SectionName) != null)
                                ddlSection.SelectedValue = cr.SectionName;

                            if (!string.IsNullOrEmpty(cr.DivisionName)
                                && ddlDivision.Items.FindByValue(cr.DivisionName) != null)
                                ddlDivision.SelectedValue = cr.DivisionName;
                        }
                    }
                }

                // day / period from query string
                string day = Request.QueryString["day"];
                string period = Request.QueryString["period"];

                if (!string.IsNullOrEmpty(day))
                {
                    int d;
                    if (int.TryParse(day, out d) && Enum.IsDefined(typeof(WeekDay), d))
                        ddlDay.SelectedValue = d.ToString();
                }

                if (!string.IsNullOrEmpty(period))
                {
                    int p;
                    if (int.TryParse(period, out p) && p >= 1 && p <= Default.PeriodCount)
                        ddlPeriod.SelectedValue = p.ToString();
                }

                if (!string.IsNullOrEmpty(ddlGradeLevel.SelectedValue))
                {
                    PopulateSubjects();
                    PopulateTeachers();
                }
            }
        }

        // =========================================================
        // الأحداث
        // =========================================================
        protected void ddlGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
            PopulateSubjects();
            PopulateTeachers();
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            // القسم يؤثر فقط على تحديد الفصل — لا يوجد إجراء إضافي
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            // الشعبة تؤثر فقط على تحديد الفصل — لا يوجد إجراء إضافي
        }

        protected void ddlSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateTeachers();
        }

        // =========================================================
        // القوائم
        // =========================================================
        private void PopulateDays()
        {
            foreach (WeekDay day in Enum.GetValues(typeof(WeekDay)))
            {
                if (day == WeekDay.Saturday) continue;
                ddlDay.Items.Add(new ListItem(GetDayName(day), ((int)day).ToString()));
            }
        }

        private void PopulatePeriods()
        {
            for (int p = 1; p <= Default.PeriodCount; p++)
            {
                ddlPeriod.Items.Add(new ListItem("الحصة " + p, p.ToString()));
            }
        }

        private void PopulateGradeLevels()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();

            var gradeLevels = db.GradeLevels
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlGradeLevel.Items.Clear();
            ddlGradeLevel.Items.Add(new ListItem("-- اختر الصف --", ""));

            foreach (var gl in gradeLevels)
                ddlGradeLevel.Items.Add(new ListItem(gl.Name, gl.Id.ToString()));
        }

        private void ConfigureSectionFilter()
        {
            int gradeLevelId;
            if (!int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
            {
                pnlSectionFilter.Visible = false;
                return;
            }

            string gradeName = db.GradeLevels
                .Where(g => g.Id == gradeLevelId)
                .Select(g => g.Name)
                .FirstOrDefault();

            bool showSection = !string.IsNullOrWhiteSpace(gradeName)
                && (gradeName.Trim() == "ثاني ثانوي" || gradeName.Trim() == "ثالث ثانوي");

            pnlSectionFilter.Visible = showSection;

            if (!showSection && ddlSection.Items.Count > 0)
            {
                var empty = ddlSection.Items.FindByValue("");
                if (empty != null)
                {
                    ddlSection.ClearSelection();
                    empty.Selected = true;
                }
            }
        }

        private void PopulateSubjects()
        {
            ddlSubject.Items.Clear();

            int gradeLevelId;
            if (!int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
                return;

            var gradeLevel = db.GradeLevels.Find(gradeLevelId);
            if (gradeLevel == null) return;

            var stage = gradeLevel.Stage;

            var subjects = db.Subjects
                .Where(s => s.Stage == stage)
                .OrderBy(s => s.Name)
                .ToList();

            foreach (var s in subjects)
                ddlSubject.Items.Add(new ListItem(s.Name, s.Id.ToString()));
        }

        private void PopulateTeachers()
        {
            ddlTeacher.Items.Clear();

            int subjectId;
            if (!string.IsNullOrEmpty(ddlSubject.SelectedValue)
                && int.TryParse(ddlSubject.SelectedValue, out subjectId))
            {
                var subject = db.Subjects.Find(subjectId);
                var gradeLevelId = 0;
                int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId);
                var gradeLevel = gradeLevelId > 0 ? db.GradeLevels.Find(gradeLevelId) : null;

                if (subject != null && gradeLevel != null)
                {
                    string gradeName = (gradeLevel.Name ?? "").Trim();
                    bool isNinthGrade = gradeName == "التاسع" || gradeName.Contains("التاسع");

                    IQueryable<Teacher> query;

                    if (isNinthGrade)
                    {
                        // قاعدة خاصة بالصف التاسع: معلمو المرحلة الثانوية فقط،
                        // ويظهر كل معلم ثانوي مرة واحدة دون ربطه بالمادة المختارة.
                        query = db.Teachers
                            .Where(t => t.IsActive && t.Stage == EducationStage.Secondary);
                    }
                    else
                    {
                        // بقية الصفوف: معلمو مرحلة الصف فقط، وتُحترم المادة.
                        query = db.Teachers
                            .Where(t => t.IsActive && t.Stage == gradeLevel.Stage &&
                                (t.Specialty == subject.Name ||
                                 t.SubjectAssignments.Any(a => a.IsActive && a.SubjectId == subject.Id)));
                    }

                    var matches = query
                        .OrderBy(t => t.Name)
                        .ThenBy(t => t.Id)
                        .ToList();

                    ddlTeacher.DataSource = matches;
                    ddlTeacher.DataTextField = "Name";
                    ddlTeacher.DataValueField = "Id";
                    ddlTeacher.DataBind();
                }
            }
        }

        // =========================================================
        // إيجاد الفصل من (الصف + القسم + الشعبة)
        // =========================================================
        private ClassRoom FindClassRoom()
        {
            int gradeLevelId;
            if (!int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
                return null;

            string section = (pnlSectionFilter.Visible && ddlSection != null)
                ? (ddlSection.SelectedValue ?? "")
                : "";

            string division = (ddlDivision != null)
                ? (ddlDivision.SelectedValue ?? "")
                : "";

            var currentYear = db.AcademicYears.FirstOrDefault(y => y.IsCurrent);
            int yearId = currentYear != null ? currentYear.Id : 0;

            var query = db.ClassRooms.Where(c => c.GradeLevelId == gradeLevelId);

            if (yearId > 0)
                query = query.Where(c => c.AcademicYearId == yearId);

            // القسم
            if (!string.IsNullOrEmpty(section))
                query = query.Where(c => c.SectionName == section);
            else
                query = query.Where(c => c.SectionName == null || c.SectionName == "");

            // الشعبة
            if (!string.IsNullOrEmpty(division))
                query = query.Where(c => c.DivisionName == division);
            else
                query = query.Where(c => c.DivisionName == null || c.DivisionName == "");

            return query.FirstOrDefault();
        }

        // =========================================================
        // تحميل حصة موجودة
        // =========================================================
        private void LoadEntry(int id)
        {
            var entry = db.TimetableEntries.Find(id);
            if (entry == null)
            {
                litTitle.Text = "الحصة غير موجودة";
                pnlFormDisabled();
                return;
            }

            litTitle.Text = "تعديل الحصة";
            hfEntryId.Value = entry.Id.ToString();

            // استخراج الصف/القسم/الشعبة من الفصل المرتبط بالحصة
            var cr = db.ClassRooms.Find(entry.ClassRoomId);
            if (cr != null)
            {
                if (ddlGradeLevel.Items.FindByValue(cr.GradeLevelId.ToString()) != null)
                    ddlGradeLevel.SelectedValue = cr.GradeLevelId.ToString();

                ConfigureSectionFilter();

                if (!string.IsNullOrEmpty(cr.SectionName)
                    && ddlSection.Items.FindByValue(cr.SectionName) != null)
                    ddlSection.SelectedValue = cr.SectionName;

                if (!string.IsNullOrEmpty(cr.DivisionName)
                    && ddlDivision.Items.FindByValue(cr.DivisionName) != null)
                    ddlDivision.SelectedValue = cr.DivisionName;
            }

            ddlDay.SelectedValue = ((int)entry.DayOfWeek).ToString();
            ddlPeriod.SelectedValue = entry.PeriodNumber.ToString();

            PopulateSubjects();
            if (ddlSubject.Items.FindByValue(entry.SubjectId.ToString()) != null)
                ddlSubject.SelectedValue = entry.SubjectId.ToString();

            PopulateTeachers();
            if (ddlTeacher.Items.FindByValue(entry.TeacherId.ToString()) != null)
                ddlTeacher.SelectedValue = entry.TeacherId.ToString();

            // ===== ضبط القائمة المخصصة للحالة =====
            int statusValue = (int)entry.Status;
            hfStatus.Value = statusValue.ToString();

            string statusText;
            switch (statusValue)
            {
                case 0: statusText = "مسودة"; break;
                case 1: statusText = "معتمد"; break;
                case 2: statusText = "مؤرشف"; break;
                default: statusText = "مسودة"; break;
            }

            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "setStatus_" + entry.Id,
                string.Format(
                    "document.addEventListener('DOMContentLoaded',function(){{" +
                    "var d=document.getElementById('statusDisplay');if(d)d.textContent='{0}';" +
                    "var opts=document.querySelectorAll('#statusOptions .custom-select-option');" +
                    "opts.forEach(function(el){{el.classList.remove('selected');" +
                    "if(el.dataset.value==='{1}')el.classList.add('selected');}});}});",
                    statusText,
                    statusValue),
                true);
        }

        private void pnlFormDisabled()
        {
            ddlGradeLevel.Enabled = false;
            ddlSection.Enabled = false;
            ddlDivision.Enabled = false;
            ddlDay.Enabled = false;
            ddlPeriod.Enabled = false;
            ddlSubject.Enabled = false;
            ddlTeacher.Enabled = false;
            btnSave.Visible = false;

            // تعطيل القائمة المخصصة
            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "disableStatus",
                "document.addEventListener('DOMContentLoaded',function(){" +
                "var t=document.querySelector('#statusDropdown .custom-select-trigger');" +
                "if(t){t.style.pointerEvents='none';t.style.opacity='0.6';}});",
                true);
        }

        // =========================================================
        // الحفظ
        // =========================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int day, period, subjectId, teacherId, statusValue;
            if (!int.TryParse(ddlDay.SelectedValue, out day) ||
                !int.TryParse(ddlPeriod.SelectedValue, out period) ||
                !int.TryParse(ddlSubject.SelectedValue, out subjectId) ||
                !int.TryParse(ddlTeacher.SelectedValue, out teacherId) ||
                !int.TryParse(hfStatus.Value, out statusValue) ||
                !Enum.IsDefined(typeof(WeekDay), day) || period < 1 || period > Default.PeriodCount ||
                !Enum.IsDefined(typeof(LessonStatus), statusValue))
            {
                ShowMessage("بيانات الحصة غير صحيحة.", false);
                return;
            }

            // تحديد الفصل من (الصف + القسم + الشعبة)
            var classRoom = FindClassRoom();
            if (classRoom == null)
            {
                ShowMessage("لا يوجد فصل مطابق للصف/القسم/الشعبة المختارة في السنة الدراسية الحالية. تأكد من إنشاء الفصل أولاً.", false);
                return;
            }

            int classId = classRoom.Id;
            LessonStatus status = (LessonStatus)statusValue;

            var subject = db.Subjects.FirstOrDefault(s => s.Id == subjectId);
            var teacher = db.Teachers.FirstOrDefault(t => t.Id == teacherId && t.IsActive);

            // تحميل GradeLevel للتحقق من المرحلة
            classRoom = db.ClassRooms.Include("GradeLevel").FirstOrDefault(c => c.Id == classId);

            if (classRoom == null || classRoom.GradeLevel == null || subject == null || teacher == null ||
                EnumHelper.GetStageForGradeLevel(classRoom.GradeLevel) != subject.Stage)
            {
                ShowMessage("لا يمكن حفظ الحصة: الصف أو المادة لا يتوافقان مع المرحلة التعليمية.", false);
                return;
            }

            // قاعدة الصف التاسع: معلمو المرحلة الثانوية فقط،
            // ومواد التاسع تبقى مواد مرحلة أساسية — لا نشترط تطابق مرحلة المعلم مع مرحلة المادة.
            string savedGradeName = (classRoom.GradeLevel.Name ?? "").Trim();
            bool savedIsNinthGrade = savedGradeName == "التاسع" || savedGradeName.Contains("التاسع");

            if (savedIsNinthGrade)
            {
                if (teacher.Stage != EducationStage.Secondary)
                {
                    ShowMessage("لا يمكن حفظ الحصة: حسب قاعدة الصف التاسع يُدرّس معلمو المرحلة الثانوية فقط.", false);
                    return;
                }
            }
            else if (teacher.Stage != subject.Stage)
            {
                ShowMessage("لا يمكن حفظ الحصة: الصف أو المادة أو المعلم لا يتوافق مع المرحلة التعليمية.", false);
                return;
            }

            try
            {
                int entryId;
                if (!string.IsNullOrEmpty(hfEntryId.Value) && int.TryParse(hfEntryId.Value, out entryId))
                {
                    // ===== تعديل حصة موجودة =====
                    var teacherClash = db.TimetableEntries
                        .FirstOrDefault(t => t.Id != entryId
                            && t.TeacherId == teacherId
                            && t.DayOfWeek == (WeekDay)day
                            && t.PeriodNumber == period);

                    if (teacherClash != null)
                    {
                        string otherClass = db.ClassRooms.Find(teacherClash.ClassRoomId)?.Name ?? "";
                        ShowMessage("تعارض: هذا المعلم مشغول في نفس اليوم والحصة في صف آخر (" + otherClass + ")", false);
                        return;
                    }

                    var entry = db.TimetableEntries.Find(entryId);
                    var clash = db.TimetableEntries
                        .FirstOrDefault(t => t.Id != entryId
                            && t.ClassRoomId == classId
                            && t.DayOfWeek == (WeekDay)day
                            && t.PeriodNumber == period);

                    if (clash != null)
                    {
                        ShowMessage("توجد حصة أخرى في نفس اليوم والحصة لهذا الفصل", false);
                        return;
                    }

                    if (entry != null)
                    {
                        entry.ClassRoomId = classId;
                        entry.DayOfWeek = (WeekDay)day;
                        entry.PeriodNumber = period;
                        entry.SubjectId = subjectId;
                        entry.TeacherId = teacherId;
                        entry.Status = status;
                        db.SaveChanges();
                    }
                }
                else
                {
                    // ===== إضافة حصة جديدة =====
                    var teacherClash = db.TimetableEntries
                        .FirstOrDefault(t => t.TeacherId == teacherId
                            && t.DayOfWeek == (WeekDay)day
                            && t.PeriodNumber == period);

                    if (teacherClash != null)
                    {
                        string otherClass = db.ClassRooms.Find(teacherClash.ClassRoomId)?.Name ?? "";
                        ShowMessage("تعارض: هذا المعلم مشغول في نفس اليوم والحصة في صف آخر (" + otherClass + ")", false);
                        return;
                    }

                    var clash = db.TimetableEntries
                        .FirstOrDefault(t => t.ClassRoomId == classId
                            && t.DayOfWeek == (WeekDay)day
                            && t.PeriodNumber == period);

                    if (clash != null)
                    {
                        ShowMessage("توجد حصة أخرى في نفس اليوم والحصة لهذا الفصل", false);
                        return;
                    }

                    var entry = new TimetableEntry
                    {
                        ClassRoomId = classId,
                        DayOfWeek = (WeekDay)day,
                        PeriodNumber = period,
                        SubjectId = subjectId,
                        TeacherId = teacherId,
                        Status = status
                    };

                    db.TimetableEntries.Add(entry);
                    db.SaveChanges();
                }

                Response.Redirect("Default.aspx?class=" + classId);
            }
            catch (Exception ex)
            {
                ShowMessage(ErrorMessageHelper.GetUserMessage(ex, GetType().FullName), false);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        private void ShowMessage(string text, bool success)
        {
            lblMessage.CssClass = success
                ? "alert alert-success d-block"
                : "alert alert-danger d-block";
            lblMessage.Text = text;
        }

        private string GetDayName(WeekDay day)
        {
            switch (day)
            {
                case WeekDay.Saturday: return "السبت";
                case WeekDay.Sunday: return "الأحد";
                case WeekDay.Monday: return "الاثنين";
                case WeekDay.Tuesday: return "الثلاثاء";
                case WeekDay.Wednesday: return "الأربعاء";
                case WeekDay.Thursday: return "الخميس";
                default: return day.ToString();
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}