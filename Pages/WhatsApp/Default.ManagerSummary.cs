using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.WhatsApp
{
    public partial class Default : Page
    {
        private void BindManagerSummary()
        {
            string managerPhone = WhatsAppApiSettings.ReadSetting(
                "NOOR_WHATSAPP_MANAGER_PHONE", "WhatsAppManagerPhone");
            litManagerPhone.Text = string.IsNullOrWhiteSpace(managerPhone)
                ? "غير مهيأ — يُعدَّل من تبويب الإعدادات"
                : managerPhone;

            if (ddlManagerStage.Items.Count == 0)
            {
                BindManagerStages();
            }
            TryAutoPreviewManagerSummary();
        }

        private void BindManagerStages()
        {
            ddlManagerStage.Items.Clear();
            ddlManagerStage.Items.Add(new ListItem("اختر المستوى...", ""));

            foreach (EducationStage stage in Enum.GetValues(typeof(EducationStage)))
            {
                ddlManagerStage.Items.Add(new ListItem(GetStageName(stage), ((int)stage).ToString()));
            }

            if (ddlManagerStage.Items.Count > 1)
            {
                ddlManagerStage.SelectedIndex = 1;
            }
            BindManagerGrades();
        }

        private string GetStageName(EducationStage stage)
        {
            switch (stage.ToString())
            {
                case "Kindergarten": return "رياض الأطفال";
                case "Basic": return "المرحلة الأساسية";
                case "Primary": return "المرحلة الابتدائية";
                case "Middle":
                case "Preparatory": return "المرحلة الإعدادية";
                case "High":
                case "Secondary": return "المرحلة الثانوية";
                default: return stage.ToString();
            }
        }

        protected void ddlManagerStage_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindManagerGrades();
            TryAutoPreviewManagerSummary();
        }

        private void BindManagerGrades()
        {
            ddlManagerGrade.Items.Clear();
            ddlManagerGrade.Items.Add(new ListItem("كل الصفوف", ""));
            ddlManagerDivision.Items.Clear();
            ddlManagerDivision.Items.Add(new ListItem("كل الأقسام", ""));
            ddlManagerClass.Items.Clear();
            ddlManagerClass.Items.Add(new ListItem("كل الشعب", ""));

            pnlManagerPreview.Visible = false;

            int stageId;
            if (!int.TryParse(ddlManagerStage.SelectedValue, out stageId)) return;

            var grades = db.GradeLevels.AsNoTracking()
                .Where(x => (int)x.Stage == stageId)
                .OrderBy(x => x.SortOrder)
                .ToList();

            foreach (var grade in grades)
            {
                ddlManagerGrade.Items.Add(new ListItem(grade.Name, grade.Id.ToString(CultureInfo.InvariantCulture)));
            }

            TryAutoPreviewManagerSummary();
        }

        protected void ddlManagerGrade_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindManagerDivisions();
            TryAutoPreviewManagerSummary();
        }

        private void BindManagerDivisions()
        {
            ddlManagerDivision.Items.Clear();
            ddlManagerDivision.Items.Add(new ListItem("كل الأقسام", ""));
            ddlManagerClass.Items.Clear();
            ddlManagerClass.Items.Add(new ListItem("كل الشعب", ""));

            pnlManagerPreview.Visible = false;

            int gradeId;
            if (!int.TryParse(ddlManagerGrade.SelectedValue, out gradeId)) return;

            var sections = db.ClassRooms.AsNoTracking()
                .Where(x => x.GradeLevelId == gradeId && !string.IsNullOrEmpty(x.SectionName))
                .Select(x => x.SectionName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            foreach (var section in sections)
            {
                ddlManagerDivision.Items.Add(new ListItem(GetManagerSectionLabel(section), section));
            }

            TryAutoPreviewManagerSummary();
        }
        protected void ddlManagerDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindManagerClasses();
            TryAutoPreviewManagerSummary();
        }

        private void BindManagerClasses()
        {
            ddlManagerClass.Items.Clear();
            ddlManagerClass.Items.Add(new ListItem("كل الشعب", ""));
            pnlManagerPreview.Visible = false;

            int gradeId;
            if (!int.TryParse(ddlManagerGrade.SelectedValue, out gradeId)) return;

            string section = ddlManagerDivision.SelectedValue;
            var query = db.ClassRooms.AsNoTracking().Where(x => x.GradeLevelId == gradeId);
            if (!string.IsNullOrWhiteSpace(section))
            {
                query = query.Where(x => x.SectionName == section);
            }

            var rooms = query.OrderBy(x => x.DivisionName).ThenBy(x => x.SectionName).ToList();
            foreach (var room in rooms)
            {
                string division = string.IsNullOrWhiteSpace(room.DivisionName)
                    ? "بدون شعبة"
                    : room.DivisionName.Trim();
                string name = string.IsNullOrWhiteSpace(section)
                    ? GetManagerSectionLabel(room.SectionName) + " — " + division
                    : division;
                ddlManagerClass.Items.Add(new ListItem(name, room.Id.ToString(CultureInfo.InvariantCulture)));
            }

            TryAutoPreviewManagerSummary();
        }

        private static string GetManagerSectionLabel(string sectionName)
        {
            string value = (sectionName ?? string.Empty).Trim();
            if (string.Equals(value, "علمي", StringComparison.Ordinal) ||
                string.Equals(value, "القسم العلمي", StringComparison.Ordinal))
                return "القسم العلمي";
            if (string.Equals(value, "أدبي", StringComparison.Ordinal) ||
                string.Equals(value, "ادبي", StringComparison.Ordinal) ||
                string.Equals(value, "القسم الأدبي", StringComparison.Ordinal) ||
                string.Equals(value, "القسم الادبي", StringComparison.Ordinal))
                return "القسم الأدبي";
            return value;
        }
        protected void ddlManagerClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            TryAutoPreviewManagerSummary();
        }

        private void TryAutoPreviewManagerSummary()
        {
            if (ddlManagerStage.SelectedValue == "")
            {
                pnlManagerPreview.Visible = false;
                return;
            }

            try
            {
                bool includeDaily = chkManagerIncludeDaily != null && chkManagerIncludeDaily.Checked;
                bool includeFinancial = chkManagerIncludeFinancial != null && chkManagerIncludeFinancial.Checked;
                bool includePayroll = chkManagerIncludePayroll != null && chkManagerIncludePayroll.Checked;

                int stageId = 0, gradeId = 0, classId = 0;
                int.TryParse(ddlManagerStage.SelectedValue, out stageId);
                int.TryParse(ddlManagerGrade.SelectedValue, out gradeId);
                int.TryParse(ddlManagerClass.SelectedValue, out classId);
                string sectionName = ddlManagerDivision.SelectedValue;

                litManagerPreview.Text = BuildManagerDailySummary(stageId, gradeId > 0 ? (int?)gradeId : null, sectionName, classId > 0 ? (int?)classId : null, includeDaily, includeFinancial, includePayroll);
                pnlManagerPreview.Visible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning("Manager preview failed: " + ex.Message);
                pnlManagerPreview.Visible = false;
            }
        }

        protected void chkManagerIncludeDaily_CheckedChanged(object sender, EventArgs e)
        {
            TryAutoPreviewManagerSummary();
        }

        protected void chkManagerIncludeFinancial_CheckedChanged(object sender, EventArgs e)
        {
            TryAutoPreviewManagerSummary();
        }

        protected void chkManagerIncludePayroll_CheckedChanged(object sender, EventArgs e)
        {
            TryAutoPreviewManagerSummary();
        }

        protected void btnPreviewManagerSummary_Click(object sender, EventArgs e)
        {
            if (ddlManagerStage.SelectedValue == "")
            {
                ShowNotice("اختر المستوى الدراسي أولاً.", true);
                return;
            }
            TryAutoPreviewManagerSummary();
            ShowNotice("حُدِّثت المعاينة.", false);
        }

        protected void btnSendManagerSummary_Click(object sender, EventArgs e)
        {
            if (!IsAdmin)
            {
                ShowNotice("إرسال الملخص الداخلي متاح لمدير النظام فقط.", true);
                return;
            }
            if (!chkManagerConfirmed.Checked)
            {
                ShowNotice("أكد أن الرقم المهيأ يخص المدير وأنه وافق على استلام الملخص.", true);
                return;
            }
            WhatsAppApiSettings settings = WhatsAppApiSettings.Load();
            if (!settings.IsConfigured && !settings.ManualLinkMode)
            {
                ShowNotice("إعداد WhatsApp Cloud API غير مكتمل.", true);
                return;
            }
            string managerPhone = WhatsAppApiSettings.ReadSetting(
                "NOOR_WHATSAPP_MANAGER_PHONE", "WhatsAppManagerPhone");
            string normalizedPhone = WhatsAppCloudApi.NormalizeRecipientNumber(managerPhone);
            if (normalizedPhone.Length < 10)
            {
                ShowNotice("أضف رقم المدير بصيغة دولية أو اضبط رمز الدولة في إعدادات الخادم.", true);
                return;
            }
            if (ddlManagerStage.SelectedValue == "")
            {
                ShowNotice("اختر المستوى الدراسي أو الصف أولاً.", true);
                return;
            }

            try
            {
                bool includeDaily = chkManagerIncludeDaily != null && chkManagerIncludeDaily.Checked;
                bool includeFinancial = chkManagerIncludeFinancial != null && chkManagerIncludeFinancial.Checked;
                bool includePayroll = chkManagerIncludePayroll != null && chkManagerIncludePayroll.Checked;

                int stageId = 0, gradeId = 0, classId = 0;
                int.TryParse(ddlManagerStage.SelectedValue, out stageId);
                int.TryParse(ddlManagerGrade.SelectedValue, out gradeId);
                int.TryParse(ddlManagerClass.SelectedValue, out classId);
                string sectionName = ddlManagerDivision.SelectedValue;

                string summary = BuildManagerDailySummary(stageId, gradeId > 0 ? (int?)gradeId : null, sectionName, classId > 0 ? (int?)classId : null, includeDaily, includeFinancial, includePayroll);
                string rendered = WhatsAppMessageFormatter.EnsureSchoolHeader(summary);
                if (rendered.Length > 4000)
                {
                    ShowNotice("الملخص طويل جداً (" + rendered.Length + " حرف). ألغِ تضمين بعض الخيارات أو راجع بيانات اليوم.", true);
                    return;
                }

                var campaign = new WhatsAppCampaign
                {
                    Title = "ملخص اليوم للمدير · " +
                        DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    MessageBody = summary,
                    MessageKind = "ManagerSummary",
                    Status = "Queued",
                    AudienceType = "Class",
                    ClassRoomId = classId > 0 ? (int?)classId : null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = CurrentUserName()
                };
                db.WhatsAppCampaigns.Add(campaign);
                db.SaveChanges();

                db.WhatsAppMessages.Add(new WhatsAppMessage
                {
                    CampaignId = campaign.Id,
                    RecipientName = "مدير النظام",
                    PhoneNumber = normalizedPhone,
                    MessageBody = rendered,
                    Direction = "Outbound",
                    Status = "Queued",
                    IsInternalRecipient = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = CurrentUserName()
                });
                db.SaveChanges();

                chkManagerConfirmed.Checked = false;
                pnlManagerPreview.Visible = false;
                string successNotice = settings.ManualLinkMode
                    ? "جُهّز ملخص اليوم في صندوق الإرسال اليدوي. افتح الرابط وأرسله للمدير ثم علّمه كمُرسل."
                    : "أُضيف ملخص اليوم إلى طابور الإرسال للمدير.";
                FlashNotice(successNotice, false);
                Response.Redirect(ViewUrl("outbox"), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("WhatsApp manager summary failed: " + ex);
                ShowNotice("تعذر إنشاء ملخص المدير. راجع سجل أخطاء التطبيق.", true);
            }
        }

        private string BuildManagerDailySummary(int stageId, int? gradeId, string sectionName, int? classId, bool includeDaily = true, bool includeFinancial = true, bool includePayroll = true)
        {
            DateTime today = DateTime.Today;

            var studentQuery = db.Students.AsNoTracking().Include(x => x.ClassRoom).Include(x => x.ClassRoom.GradeLevel).Where(x => x.IsActive);
            ClassRoom selectedRoom = null;
            GradeLevel selectedGrade = null;
            if (classId.HasValue)
            {
                studentQuery = studentQuery.Where(x => x.ClassRoomId == classId.Value);
                selectedRoom = db.ClassRooms.AsNoTracking().Include(x => x.GradeLevel)
                    .FirstOrDefault(x => x.Id == classId.Value);
                if (selectedRoom != null)
                    selectedGrade = selectedRoom.GradeLevel;
            }
            else if (gradeId.HasValue)
            {
                studentQuery = studentQuery.Where(x => x.ClassRoom.GradeLevelId == gradeId.Value);
                if (!string.IsNullOrWhiteSpace(sectionName))
                    studentQuery = studentQuery.Where(x => x.ClassRoom.SectionName == sectionName);
                selectedGrade = db.GradeLevels.AsNoTracking().FirstOrDefault(x => x.Id == gradeId.Value);
            }
            else
            {
                studentQuery = studentQuery.Where(x => (int)x.ClassRoom.GradeLevel.Stage == stageId);
            }

            EducationStage selectedStage = selectedGrade != null
                ? selectedGrade.Stage
                : (EducationStage)stageId;
            string selectedSection = selectedRoom != null ? selectedRoom.SectionName : sectionName;
            string selectedDivision = selectedRoom == null ? null : selectedRoom.DivisionName;
            string scopeTitle = string.Join(Environment.NewLine, new[]
            {
                "المستوى الدراسي: " + GetStageName(selectedStage),
                "الصف: " + (selectedGrade == null ? "كل الصفوف" : selectedGrade.Name),
                "القسم: " + (string.IsNullOrWhiteSpace(selectedSection) ? "كل الأقسام" : GetManagerSectionLabel(selectedSection)),
                "الشعبة: " + (selectedRoom == null
                    ? "كل الشعب"
                    : (string.IsNullOrWhiteSpace(selectedDivision) ? "غير محددة" : selectedDivision.Trim()))
            });

            List<int> studentIds = studentQuery.Select(x => x.Id).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("ملخص اليوم — " + today.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));
            sb.AppendLine(scopeTitle);

            if (studentIds.Count == 0)
            {
                sb.AppendLine("لا يوجد طلاب نشطون في هذا النطاق المحدد.");
                return sb.ToString().TrimEnd();
            }

            if (includeDaily)
            {
                DateTime tomorrow = today.AddDays(1);
                int recordedAttendance = db.Attendances.AsNoTracking().Count(x => studentIds.Contains(x.StudentId) && x.Date >= today && x.Date < tomorrow);
                int absences = db.Attendances.AsNoTracking().Count(x => studentIds.Contains(x.StudentId) && !x.IsPresent && x.Date >= today && x.Date < tomorrow);

                var classRoomIds = studentQuery.Select(x => x.ClassRoomId).Distinct().ToList();
                var lessons = db.DailyLessons.AsNoTracking().Include(x => x.Subject).Include(x => x.Teacher).Include(x => x.ClassRoom)
                    .Where(x => classRoomIds.Contains(x.ClassRoomId) && x.Date >= today && x.Date < tomorrow && x.Status == LessonStatus.Approved)
                    .OrderBy(x => x.ClassRoom.DivisionName).ThenBy(x => x.PeriodNumber).ToList();

                string lessonSummary = lessons.Count == 0 ? "لا توجد دروس معتمدة مسجلة" :
                    string.Join("، ", lessons.Select(x =>
                        (x.ClassRoom != null && !string.IsNullOrWhiteSpace(x.ClassRoom.DivisionName) ? "[" + x.ClassRoom.DivisionName + "] " : "") +
                        x.PeriodNumber + "-" + (x.Subject == null ? "حصة" : x.Subject.Name) +
                        (x.Teacher == null ? string.Empty : " مع " + x.Teacher.Name)));

                sb.AppendLine("عدد الطلاب النشطين: " + studentIds.Count);
                sb.AppendLine("حضور اليوم: " + recordedAttendance);
                sb.AppendLine("غياب اليوم: " + absences);
            }

            if (includeFinancial)
            {
                if (includeDaily) sb.AppendLine();
                sb.AppendLine("── الملخص المالي للطلاب ──");
                sb.Append(BuildManagerFinancialSection(studentIds));
            }

            if (includePayroll)
            {
                if (includeDaily || includeFinancial) sb.AppendLine();
                sb.AppendLine("── التقرير المالي للمعلمين والمعلمات ──");
                sb.Append(BuildManagerPayrollSection());
            }

            if (!includeDaily && !includeFinancial && !includePayroll)
            {
                sb.AppendLine("لم يتم تحديد أي بيانات للتضمين في التقرير.");
            }

            return sb.ToString().TrimEnd();
        }

        private string BuildManagerFinancialSection(List<int> studentIds)
        {
            var sb = new StringBuilder();
            if (studentIds.Count == 0)
            {
                sb.AppendLine("لا يوجد طلاب نشطون في هذا النطاق.");
                return sb.ToString();
            }

            var invoiceRows = db.Invoices.AsNoTracking()
                .Where(x => studentIds.Contains(x.StudentId)
                    && x.AcademicYear.IsCurrent
                    && x.Status != InvoiceStatus.Draft
                    && x.Status != InvoiceStatus.Cancelled)
                .Select(x => new { x.StudentId, x.NetAmount, x.PaidAmount })
                .ToList();

            if (invoiceRows.Count == 0)
            {
                sb.AppendLine("لا توجد فواتير مسجلة في العام الدراسي الحالي.");
                return sb.ToString();
            }

            decimal totalDue = invoiceRows.Sum(x => x.NetAmount);
            decimal totalPaid = invoiceRows.Sum(x => x.PaidAmount);
            decimal outstanding = invoiceRows.Sum(x => Math.Max(0m, x.NetAmount - x.PaidAmount));
            decimal credit = invoiceRows.Sum(x => Math.Max(0m, x.PaidAmount - x.NetAmount));
            decimal collectionRate = totalDue > 0 ? (totalPaid / totalDue) * 100m : 0m;

            var perStudent = invoiceRows
                .GroupBy(x => x.StudentId)
                .Select(g => new
                {
                    Net = g.Sum(x => x.NetAmount),
                    Paid = g.Sum(x => x.PaidAmount)
                })
                .Select(x => new { x.Net, x.Paid, Balance = x.Net - x.Paid })
                .ToList();

            int fullyPaid = perStudent.Count(x => x.Balance <= 0.01m);
            int withDebt = perStudent.Count(x => x.Balance > 0.01m);
            int noInvoice = studentIds.Count - perStudent.Count;

            var ci = CultureInfo.CurrentCulture;
            sb.AppendLine("إجمالي المستحق: " + totalDue.ToString("N2", ci) + " د.ل");
            sb.AppendLine("إجمالي المسدد: " + totalPaid.ToString("N2", ci) + " د.ل");
            sb.AppendLine("نسبة التحصيل: " + collectionRate.ToString("N1", ci) + "%");
            sb.AppendLine("إجمالي المتأخرات: " + outstanding.ToString("N2", ci) + " د.ل");
            if (credit > 0.01m)
                sb.AppendLine("أرصدة دائنة (دفعات زائدة): " + credit.ToString("N2", ci) + " د.ل");
            sb.AppendLine("طلاب مسدّدون بالكامل: " + fullyPaid);
            sb.AppendLine("طلاب عليهم متأخرات: " + withDebt);
            if (noInvoice > 0)
                sb.AppendLine("طلاب بلا فواتير: " + noInvoice);

            return sb.ToString();
        }

        private string BuildManagerPayrollSection()
        {
            var sb = new StringBuilder();

            // جلب أحدث فترة مالية
            var latestPeriod = db.PayrollPeriods.AsNoTracking()
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .FirstOrDefault();

            if (latestPeriod == null)
            {
                sb.AppendLine("لا توجد فترات مالية مسجلة في النظام.");
                return sb.ToString();
            }

            var payrollsWithTeachers = (from p in db.TeacherPayrolls.AsNoTracking()
                                        join t in db.Teachers.AsNoTracking() on p.TeacherId equals t.Id
                                        where p.PayrollPeriodId == latestPeriod.Id
                                        select new
                                        {
                                            p.EarnedAmount,
                                            p.DeductionAmount,
                                            p.AdjustmentAmount,
                                            p.NetAmount,
                                            t.EmploymentType
                                        }).ToList();

            if (payrollsWithTeachers.Count == 0)
            {
                sb.AppendLine("لا توجد بيانات مرتبات محسوبة لأحدث فترة مالية.");
                return sb.ToString();
            }

            int teacherCount = payrollsWithTeachers.Count;
            decimal lessonsSalary = payrollsWithTeachers
                .Where(x => x.EmploymentType == EmploymentType.PerLesson)
                .Sum(x => (decimal?)x.EarnedAmount).GetValueOrDefault();

            decimal monthlySalaries = payrollsWithTeachers
                .Where(x => x.EmploymentType == EmploymentType.Permanent || x.EmploymentType == EmploymentType.PermanentSupervisor)
                .Sum(x => (decimal?)x.EarnedAmount).GetValueOrDefault();

            decimal totalAdjustments = payrollsWithTeachers.Sum(x => (decimal?)x.AdjustmentAmount).GetValueOrDefault();
            decimal totalDeductions = payrollsWithTeachers.Sum(x => (decimal?)x.DeductionAmount).GetValueOrDefault();
            decimal totalNet = payrollsWithTeachers.Sum(x => (decimal?)x.NetAmount).GetValueOrDefault();

            var ci = CultureInfo.CurrentCulture;
            string monthName = latestPeriod.Month >= 1 && latestPeriod.Month <= 12
                ? new[] { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" }[latestPeriod.Month - 1]
                : latestPeriod.Month.ToString();

            sb.AppendLine("فترة الاحتساب: " + monthName + " " + latestPeriod.Year);
            sb.AppendLine("عدد المعلمين المحسوبين: " + teacherCount + " معلم/ة");
            sb.AppendLine("إجمالي المرتبات الشهرية: " + monthlySalaries.ToString("N2", ci) + " د.ل");
            sb.AppendLine("إجمالي مرتب الحصص: " + lessonsSalary.ToString("N2", ci) + " د.ل");
            sb.AppendLine("إجمالي المكافآت والتسويات: " + totalAdjustments.ToString("N2", ci) + " د.ل");
            sb.AppendLine("إجمالي الخصومات: " + totalDeductions.ToString("N2", ci) + " د.ل");
            sb.AppendLine("إجمالي صافي المرتبات: " + totalNet.ToString("N2", ci) + " د.ل");

            return sb.ToString();
        }
        // ============================================================
        // END MANAGER SUMMARY TAB
        // ============================================================

    }
}