using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Timetable
{
    public partial class Default : Page
    {
        public const int PeriodCount = 7;

        private readonly SchoolDbContext db = new SchoolDbContext();

        // =========================================================
        // Page_Load
        // =========================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            var stage = EnumHelper.GetCurrentStage();
            lblStage.Text = EnumHelper.GetStageArabic(stage.ToString());

            if (!IsPostBack)
            {
                PopulateGradeLevels(stage);

                // Preselect from query string (?class=X)
                string preselectClass = Request.QueryString["class"];
                if (!string.IsNullOrEmpty(preselectClass))
                {
                    int classId;
                    if (int.TryParse(preselectClass, out classId))
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

                LoadTimetable();
            }
            else
            {
                // ✅ بعد كل PostBack، أعد تكوين فلتر القسم (لأن Panel.Visible لا يُحفظ في ViewState)
                ConfigureSectionFilter();
            }
        }

        // =========================================================
        // الأحداث
        // =========================================================
        protected void ddlGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
            LoadTimetable();
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTimetable();
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTimetable();
        }

        protected void btnAddEntry_Click(object sender, EventArgs e)
        {
            var classRoom = FindClassRoom();
            if (classRoom == null)
            {
                ShowMessage("لا يوجد فصل مطابق للصف/القسم/الشعبة المختارة. تأكد من إنشاء الفصل أولاً.", false);
                return;
            }
            Response.Redirect("Edit.aspx?classId=" + classRoom.Id);
        }

        protected void btnDeleteEntry_Click(object sender, EventArgs e)
        {
            try
            {
                // ===== التحقق من المدخلات =====
                int entryId;
                if (!int.TryParse(hfDeleteEntryId.Value, out entryId) || entryId <= 0)
                {
                    ShowMessage("الحصة المحددة غير صحيحة.", false);
                    return;
                }

                var entry = db.TimetableEntries
                    .Include(t => t.Subject)
                    .Include(t => t.Teacher)
                    .FirstOrDefault(t => t.Id == entryId);

                if (entry == null)
                {
                    ShowMessage("الحصة غير موجودة.", false);
                    return;
                }

                // ===== التحقق من الصلاحية =====
                var stage = EnumHelper.GetCurrentStage();
                var allowedClassIds = EnumHelper.GetClassRoomIdsForStage(db, stage) ?? new int[0];

                if (!allowedClassIds.Contains(entry.ClassRoomId))
                {
                    ShowMessage("لا تملك صلاحية حذف هذه الحصة.", false);
                    return;
                }

                // ===== حذف الدروس اليومية المرتبطة (مباشرةً دون طلب تأكيد إضافي) =====
                var relatedLessons = db.DailyLessons
                    .Where(d => d.ScheduleEntryId == entryId)
                    .ToList();

                int relatedCount = relatedLessons.Count;

                // ===== الحذف داخل Transaction لضمان التكامل =====
                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (relatedCount > 0)
                        {
                            db.DailyLessons.RemoveRange(relatedLessons);
                        }

                        db.TimetableEntries.Remove(entry);
                        db.SaveChanges();
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }

                // ===== تنظيف الحالة وإعادة التحميل =====
                hfDeleteEntryId.Value = "";
                hfForceDelete.Value = "false";
                LoadTimetable();

                ShowMessage(
                    relatedCount > 0
                        ? "تم حذف الحصة بنجاح مع " + relatedCount + " درس يومي مرتبط بها."
                        : "تم حذف الحصة بنجاح.",
                    true);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
            {
                System.IO.File.AppendAllText(
                    Server.MapPath("~/App_Data/delete-error.log"),
                    DateTime.Now + " :: DbUpdateException :: " + dbEx + Environment.NewLine +
                    new string('-', 80) + Environment.NewLine);

                ShowMessage("لا يمكن حذف الحصة لوجود بيانات مرتبطة بها. راجع مسؤول النظام.", false);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(
                    Server.MapPath("~/App_Data/delete-error.log"),
                    DateTime.Now + " :: " + ex + Environment.NewLine +
                    new string('-', 80) + Environment.NewLine);

                ShowMessage("حدث خطأ أثناء الحذف: " + ex.Message, false);
            }
        }

        // =========================================================
        // القوائم
        // =========================================================
        private void PopulateGradeLevels(EducationStage stage)
        {
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

            if (!string.IsNullOrEmpty(section))
                query = query.Where(c => c.SectionName == section);
            else
                query = query.Where(c => c.SectionName == null || c.SectionName == "");

            if (!string.IsNullOrEmpty(division))
                query = query.Where(c => c.DivisionName == division);
            else
                query = query.Where(c => c.DivisionName == null || c.DivisionName == "");

            return query.FirstOrDefault();
        }

        private void ShowMessage(string text, bool success)
        {
            lblMessage.CssClass = success
                ? "tt-message success"
                : "tt-message danger";
            lblMessage.Text = text;
        }

        // =========================================================
        // تحميل الجدول
        // =========================================================
        protected void LoadTimetable()
        {
            var classRoom = FindClassRoom();

            if (classRoom == null)
            {
                int gradeLevelId;
                bool hasGrade = int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) && gradeLevelId > 0;

                if (!hasGrade)
                {
                    litTimetable.Text =
                        "<div class='tt-empty'>" +
                        "<div class='tt-empty-icon'><i class='fas fa-calendar-alt'></i></div>" +
                        "<h4>اختر الصف الدراسي</h4>" +
                        "<p>يجب اختيار الصف الدراسي لعرض الجدول الأسبوعي</p>" +
                        "</div>";
                }
                else
                {
                    litTimetable.Text =
                        "<div class='tt-empty'>" +
                        "<div class='tt-empty-icon'><i class='fas fa-exclamation-circle'></i></div>" +
                        "<h4>لا يوجد فصل مطابق</h4>" +
                        "<p>لم يتم العثور على فصل بهذه المواصفات في السنة الدراسية الحالية. تأكد من إنشاء الفصل أولاً.</p>" +
                        "</div>";
                }
                return;
            }

            int classId = classRoom.Id;

            var entries = db.TimetableEntries
                .Include(e => e.Subject)
                .Include(e => e.Teacher)
                .Where(e => e.ClassRoomId == classId)
                .ToList();

            var byCell = entries.ToDictionary(
                e => ((int)e.DayOfWeek) + "-" + e.PeriodNumber,
                e => e);

            var dayKeys = new[]
            {
                WeekDay.Sunday, WeekDay.Monday, WeekDay.Tuesday,
                WeekDay.Wednesday, WeekDay.Thursday
            };

            var className = classRoom.Name;

            var sb = new StringBuilder();

            // ===== النسخة المعروضة على الشاشة =====
            var screenTableHtml = BuildScreenTableHtml(dayKeys, byCell, classId);
            sb.Append("<div class='screen-only'>")
              .Append(screenTableHtml)
              .Append("</div>");

            // ===== نسخة الطباعة (Flexbox Grid) =====
            var printGridHtml = BuildPrintGridHtml(dayKeys, byCell, classId);

            sb.Append("<div class='print-page'>");
            sb.Append("<div class='print-header'>");
            sb.Append("<h2>مدرسة نور الملائكة</h2>");
            sb.Append("<h3>الجدول الدراسي الأسبوعي</h3>");
            sb.Append("<p>الصف الدراسي: ")
              .Append(System.Web.HttpUtility.HtmlEncode(className))
              .Append("</p>");
            sb.Append("</div>");

            sb.Append("<div class='print-copy'>").Append(printGridHtml).Append("</div>");
            sb.Append("<div class='print-copy'>").Append(printGridHtml).Append("</div>");
            sb.Append("<div class='print-copy print-copy-last'>").Append(printGridHtml).Append("</div>");

            sb.Append("</div>");

            // =====================================================
            // CSS
            // =====================================================
            sb.Append("<style>");

            sb.Append("@page{size:A4 portrait;margin:0;}");
            sb.Append(".print-page{display:none;}");

            sb.Append("@media print{");

            sb.Append("html,body{");
            sb.Append("  background:#fff !important;");
            sb.Append("  margin:0 !important;");
            sb.Append("  padding:0 !important;");
            sb.Append("  width:210mm !important;");
            sb.Append("  font-family:'Droid Arabic Kufi',sans-serif !important;");
            sb.Append("}");

            sb.Append("*{");
            sb.Append("  -webkit-print-color-adjust:exact !important;");
            sb.Append("  print-color-adjust:exact !important;");
            sb.Append("  box-sizing:border-box !important;");
            sb.Append("}");

            sb.Append(".screen-only{display:none !important;}");
            sb.Append(".tt-actions{display:none !important;}");
            sb.Append(".add-cell{display:none !important;}");
            sb.Append(".no-print{display:none !important;}");

            sb.Append(".print-page{");
            sb.Append("  display:block !important;");
            sb.Append("  width:210mm !important;");
            sb.Append("  padding:3mm 4mm !important;");
            sb.Append("}");

            sb.Append(".print-header{");
            sb.Append("  display:block !important;");
            sb.Append("  text-align:center;");
            sb.Append("  margin:0 0 3mm 0;");
            sb.Append("  padding-bottom:1.5mm;");
            sb.Append("  border-bottom:0.8mm solid #00b14f;");
            sb.Append("}");
            sb.Append(".print-header h2{margin:0;font-size:11pt;font-weight:800;color:#111827;line-height:1;}");
            sb.Append(".print-header h3{margin:1mm 0 0;font-size:8pt;font-weight:700;color:#00b14f;line-height:1;}");
            sb.Append(".print-header p{margin:1mm 0 0;font-size:7pt;font-weight:600;color:#374151;line-height:1;}");

            sb.Append(".print-copy{");
            sb.Append("  display:block !important;");
            sb.Append("  margin:0 0 5mm 0 !important;");
            sb.Append("  page-break-inside:avoid !important;");
            sb.Append("  break-inside:avoid !important;");
            sb.Append("}");
            sb.Append(".print-copy-last{margin-bottom:0 !important;}");

            sb.Append(".print-grid{");
            sb.Append("  display:flex !important;");
            sb.Append("  flex-direction:column !important;");
            sb.Append("  width:100% !important;");
            sb.Append("  border:0.3mm solid #4b5563 !important;");
            sb.Append("  border-collapse:collapse !important;");
            sb.Append("}");

            sb.Append(".print-row{");
            sb.Append("  display:flex !important;");
            sb.Append("  flex-direction:row !important;");
            sb.Append("  width:100% !important;");
            sb.Append("  flex-wrap:nowrap !important;");
            sb.Append("}");

            sb.Append(".print-cell{");
            sb.Append("  flex:1 1 0 !important;");
            sb.Append("  min-width:0 !important;");
            sb.Append("  max-width:100% !important;");
            sb.Append("  border:0.2mm solid #6b7280 !important;");
            sb.Append("  padding:1mm 0.5mm !important;");
            sb.Append("  text-align:center !important;");
            sb.Append("  vertical-align:middle !important;");
            sb.Append("  display:flex !important;");
            sb.Append("  flex-direction:column !important;");
            sb.Append("  align-items:center !important;");
            sb.Append("  justify-content:center !important;");
            sb.Append("  overflow:hidden !important;");
            sb.Append("  word-wrap:break-word !important;");
            sb.Append("  word-break:break-word !important;");
            sb.Append("}");

            sb.Append(".print-cell.is-day{");
            sb.Append("  flex:0 0 12mm !important;");
            sb.Append("  min-width:12mm !important;");
            sb.Append("  max-width:12mm !important;");
            sb.Append("}");

            sb.Append(".print-row.is-header .print-cell{");
            sb.Append("  background:#00b14f !important;");
            sb.Append("  color:#fff !important;");
            sb.Append("  font-weight:800 !important;");
            sb.Append("  font-size:7pt !important;");
            sb.Append("  height:6mm !important;");
            sb.Append("  padding:1.5mm 0.5mm !important;");
            sb.Append("}");

            sb.Append(".print-row:not(.is-header) .print-cell.is-day{");
            sb.Append("  background:#e8f4ec !important;");
            sb.Append("  color:#007a33 !important;");
            sb.Append("  font-weight:800 !important;");
            sb.Append("  font-size:7pt !important;");
            sb.Append("}");

            sb.Append(".print-row:not(.is-header) .print-cell:not(.is-day){");
            sb.Append("  background:#fff !important;");
            sb.Append("  height:11mm !important;");
            sb.Append("  font-size:6pt !important;");
            sb.Append("}");

            sb.Append(".print-row:not(.is-header) .print-cell.has-entry{");
            sb.Append("  background:#f0f8f3 !important;");
            sb.Append("}");

            sb.Append(".print-subject{");
            sb.Append("  font-size:6pt !important;");
            sb.Append("  font-weight:800 !important;");
            sb.Append("  color:#065f46 !important;");
            sb.Append("  margin:0 !important;");
            sb.Append("  line-height:1.1 !important;");
            sb.Append("  word-break:break-word !important;");
            sb.Append("}");

            sb.Append(".print-teacher{");
            sb.Append("  font-size:5pt !important;");
            sb.Append("  color:#4b5563 !important;");
            sb.Append("  margin:0.5mm 0 0 0 !important;");
            sb.Append("  line-height:1.05 !important;");
            sb.Append("  word-break:break-word !important;");
            sb.Append("}");

            sb.Append("}");
            sb.Append("</style>");

            litTimetable.Text = sb.ToString();
        }

        // =========================================================
        // بناء نسخة الطباعة بـ Flexbox
        // =========================================================
        private string BuildPrintGridHtml(
            WeekDay[] dayKeys,
            IDictionary<string, Models.TimetableEntry> byCell,
            int classId)
        {
            var sb = new StringBuilder();

            sb.Append("<div class='print-grid'>");

            // Header Row
            sb.Append("<div class='print-row is-header'>");
            sb.Append("<div class='print-cell is-day'>اليوم</div>");

            for (int period = 1; period <= PeriodCount; period++)
            {
                sb.Append("<div class='print-cell'>الحصة ")
                  .Append(period)
                  .Append("</div>");
            }

            sb.Append("</div>");

            // Body Rows
            foreach (var day in dayKeys)
            {
                sb.Append("<div class='print-row'>");

                sb.Append("<div class='print-cell is-day'>")
                  .Append(GetDayName(day))
                  .Append("</div>");

                for (int period = 1; period <= PeriodCount; period++)
                {
                    Models.TimetableEntry entry;
                    byCell.TryGetValue(((int)day) + "-" + period, out entry);

                    if (entry != null)
                    {
                        string subject = entry.Subject != null ? entry.Subject.Name : "—";
                        string teacher = entry.Teacher != null ? entry.Teacher.Name : "";

                        sb.Append("<div class='print-cell has-entry'>");

                        sb.Append("<div class='print-subject'>")
                          .Append(System.Web.HttpUtility.HtmlEncode(subject))
                          .Append("</div>");

                        if (teacher.Length > 0)
                        {
                            sb.Append("<div class='print-teacher'>")
                              .Append(System.Web.HttpUtility.HtmlEncode(teacher))
                              .Append("</div>");
                        }

                        sb.Append("</div>");
                    }
                    else
                    {
                        sb.Append("<div class='print-cell'></div>");
                    }
                }

                sb.Append("</div>");
            }

            sb.Append("</div>");

            return sb.ToString();
        }

        // =========================================================
        // بناء جدول نسخة الشاشة
        // =========================================================
        private string BuildScreenTableHtml(
            WeekDay[] dayKeys,
            IDictionary<string, Models.TimetableEntry> byCell,
            int classId)
        {
            var sb = new StringBuilder();

            sb.Append("<div class='table-responsive' style='padding: 16px;'>");
            sb.Append("<table class='table-timetable'>");

            sb.Append("<thead><tr>");
            sb.Append("<th>اليوم</th>");
            for (int period = 1; period <= PeriodCount; period++)
            {
                sb.Append("<th>الحصة ").Append(period).Append("</th>");
            }
            sb.Append("</tr></thead>");

            sb.Append("<tbody>");

            foreach (var day in dayKeys)
            {
                sb.Append("<tr>");
                sb.Append("<th>").Append(GetDayName(day)).Append("</th>");

                for (int period = 1; period <= PeriodCount; period++)
                {
                    Models.TimetableEntry entry;
                    byCell.TryGetValue(((int)day) + "-" + period, out entry);

                    if (entry != null)
                    {
                        string subject = entry.Subject != null ? entry.Subject.Name : "—";
                        string teacher = entry.Teacher != null ? entry.Teacher.Name : "";

                        sb.Append("<td class='timetable-cell has-entry'>");

                        sb.Append("<div class='tt-subject'>")
                          .Append(System.Web.HttpUtility.HtmlEncode(subject))
                          .Append("</div>");

                        if (teacher.Length > 0)
                        {
                            sb.Append("<div class='tt-teacher'>")
                              .Append(System.Web.HttpUtility.HtmlEncode(teacher))
                              .Append("</div>");
                        }

                        sb.Append("<div class='tt-actions no-print'>");

                        sb.Append("<a class='btn btn-sm' ")
                          .Append("style='border:1px solid #00b14f;color:#00b14f;' ")
                          .Append("href='Edit.aspx?id=")
                          .Append(entry.Id)
                          .Append("' title='تعديل'>")
                          .Append("<i class='fas fa-pen'></i></a>");

                        sb.Append("<button type='button' class='btn btn-sm' ")
                          .Append("style='border:1px solid #dc3545;color:#dc3545;' ")
                          .Append("onclick=\"return openDeleteModal(")
                          .Append(entry.Id)
                          .Append(");\" title='حذف'>")
                          .Append("<i class='fas fa-trash'></i></button>");

                        sb.Append("</div>");
                        sb.Append("</td>");
                    }
                    else
                    {
                        sb.Append("<td class='timetable-cell'>");
                        sb.Append("<a class='add-cell no-print' ")
                          .Append("title='إضافة حصة' ")
                          .Append("href='Edit.aspx?classId=")
                          .Append(classId)
                          .Append("&day=").Append((int)day)
                          .Append("&period=").Append(period)
                          .Append("'><i class='fas fa-plus'></i></a>");
                        sb.Append("</td>");
                    }
                }

                sb.Append("</tr>");
            }

            sb.Append("</tbody>");
            sb.Append("</table></div>");

            return sb.ToString();
        }

        private string GetDayName(WeekDay day)
        {
            switch (day)
            {
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