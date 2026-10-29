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
        private void SaveCampaign(string status, DateTime? scheduledAtUtc, bool createMessages)
        {
            try
            {
                EnforceFinancialPermission();
                string audience = ddlAudienceType.SelectedValue;
                int? targetClass = null;
                if (audience != "School")
                {
                    int classId;
                    if (int.TryParse(ddlClass.SelectedValue, out classId))
                    {
                        targetClass = classId;
                    }
                    else if (ddlComposeGrade.SelectedValue == "")
                    {
                        ShowNotice("اختر الفصل أو الشعبة أولاً.", true);
                        return;
                    }
                }
                string body = txtMessage.Text == null ? string.Empty : txtMessage.Text.Trim();
                if (chkIncludeAnnouncement.Checked)
                {
                    string announcement = txtAnnouncement.Text == null ? string.Empty : txtAnnouncement.Text.Trim();
                    if (announcement.Length == 0)
                    {
                        ShowNotice("اكتب نص الإعلان قبل حفظ الرسالة أو إرسالها.", true);
                        return;
                    }
                    body = body.Length == 0 ? announcement : body + Environment.NewLine + announcement;
                }
                if (body.Length == 0)
                {
                    ShowNotice("اكتب نص الرسالة أو اختر قالباً.", true);
                    return;
                }
                if (WhatsAppMessageFormatter.EnsureSchoolHeader(body).Length > 4000)
                {
                    ShowNotice("نص الرسالة أطول من الحد المسموح.", true);
                    return;
                }
                if ((body.IndexOf("{balance}", StringComparison.OrdinalIgnoreCase) >= 0 && !chkIncludeBalance.Checked) ||
                    (body.IndexOf("{attendanceSummary}", StringComparison.OrdinalIgnoreCase) >= 0 && !chkIncludeAttendance.Checked) ||
                    (body.IndexOf("{latestGrade}", StringComparison.OrdinalIgnoreCase) >= 0 && !chkIncludeGrades.Checked) ||
                    (ContainsMessageToken(body, "dailyLessons") && !chkAppendDailyLessons.Checked))
                {
                    ShowNotice("فعّل خيارات البيانات التي تظهر متغيراتها في نص الرسالة قبل الحفظ أو الإرسال.", true);
                    return;
                }
                string campaignTitle = body.Length > 80 ? body.Substring(0, 80) : body;
                WhatsAppTemplate selectedTemplate = GetSelectedTemplate();
                bool addsCustomText = chkIncludeAnnouncement.Checked ||
                    (chkAppendDailyLessons.Checked && !ContainsMessageToken(body, "dailyLessons"));
                string kind = ddlMessageKind.SelectedValue;
                int targetStudentId;
                int? targetStudent = audience == "Individual" &&
                    int.TryParse(ddlStudent.SelectedValue, out targetStudentId) ? (int?)targetStudentId : null;
                if (audience == "Individual" && !targetStudent.HasValue)
                {
                    ShowNotice("اختر الطالب الذي تريد مراسلة ولي أمره.", true);
                    return;
                }
                List<Recipient> recipients = createMessages ? LoadEligibleRecipients(targetClass, targetStudent) : new List<Recipient>();
                if (createMessages && recipients.Count == 0)
                {
                    ShowNotice("لا يوجد ولي أمر لديه رقم وموافقة واتساب فعّالة ضمن النطاق المحدد. راجع تبويب الموافقات.", true);
                    return;
                }

                int editingId;
                WhatsAppCampaign campaign = int.TryParse(Convert.ToString(Session[EditingDraftKey]), out editingId)
                    ? db.WhatsAppCampaigns.FirstOrDefault(x => x.Id == editingId && x.Status == "Draft")
                    : null;
                if (campaign == null)
                {
                    campaign = new WhatsAppCampaign
                    {
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = CurrentUserName()
                    };
                    db.WhatsAppCampaigns.Add(campaign);
                }
                campaign.Title = campaignTitle;
                campaign.MessageBody = body;
                campaign.MessageKind = kind;
                campaign.Status = status;
                campaign.AudienceType = audience;
                campaign.ClassRoomId = targetClass;
                campaign.TargetStudentId = targetStudent;
                campaign.WhatsAppTemplateId = selectedTemplate == null ? (int?)null : selectedTemplate.Id;
                campaign.IncludeBalance = chkIncludeBalance.Checked;
                campaign.IncludeAttendance = chkIncludeAttendance.Checked;
                campaign.IncludeGrades = chkIncludeGrades.Checked;
                campaign.AppendDailyLessons = chkAppendDailyLessons.Checked;
                campaign.ScheduledAt = scheduledAtUtc;
                db.SaveChanges();
                Session.Remove(EditingDraftKey);

                if (createMessages)
                {
                    foreach (Recipient recipient in recipients)
                    {
                        List<string> parameters;
                        string rendered = RenderMessage(body, recipient, selectedTemplate, out parameters);
                        db.WhatsAppMessages.Add(new WhatsAppMessage
                        {
                            CampaignId = campaign.Id,
                            ParentId = recipient.Parent.Id,
                            StudentId = recipient.Students.Count == 1 ? recipient.Students[0].Id : (int?)null,
                            RecipientName = recipient.Parent.Name,
                            PhoneNumber = WhatsAppCloudApi.NormalizeRecipientNumber(recipient.Parent.Phone),
                            MessageBody = rendered,
                            Direction = "Outbound",
                            Status = status,
                            ProviderTemplateName = !addsCustomText && selectedTemplate != null && selectedTemplate.IsMetaApproved
                                ? selectedTemplate.ProviderTemplateName : null,
                            TemplateLanguage = selectedTemplate == null || addsCustomText ? null : selectedTemplate.LanguageCode,
                            TemplateParametersJson = new JavaScriptSerializer().Serialize(parameters),
                            ScheduledAt = scheduledAtUtc,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = CurrentUserName()
                        });
                    }
                    db.SaveChanges();
                }
                bool manual = WhatsAppApiSettings.Load().ManualLinkMode;
                string message;
                if (status == "Draft")
                    message = "حُفظت المسودة. لم يُنشأ طابور إرسال.";
                else if (manual)
                    message = "جُهّزت " + recipients.Count + " رسالة في صندوق الإرسال اليدوي. افتح كل رابط، أرسل من واتساب، ثم علّمها كمُرسلة.";
                else if (status == "Scheduled")
                    message = "جُدولت الحملة لعدد " + recipients.Count + " وليّ أمر. سيُعاد التحقق من الموافقة عند موعد الإرسال.";
                else
                    message = "أُضيفت " + recipients.Count + " رسالة إلى طابور الإرسال. سيعالجها المجدول على الخادم.";
                ShowNotice(message, false);
                if (status == "Draft")
                {
                    FlashNotice(message, false);
                    Response.Redirect(ViewUrl("drafts"), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else if (manual && createMessages)
                {
                    FlashNotice(message, false);
                    Response.Redirect(ViewUrl("outbox"), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowNotice(ex.Message, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("WhatsApp campaign save failed: " + ex);
                ShowNotice("تعذر حفظ الحملة. راجع سجل أخطاء التطبيق.", true);
            }
        }

        private void EnforceFinancialPermission()
        {
            if (ddlMessageKind.SelectedValue == "Financial" &&
                SecurityHelper.CurrentRole != UserRole.Admin &&
                SecurityHelper.CurrentRole != UserRole.Accountant)
            {
                ddlMessageKind.SelectedValue = "Announcement";
                chkIncludeBalance.Checked = false;
                throw new UnauthorizedAccessException("إرسال البيانات المالية متاح لمدير النظام أو المحاسب فقط.");
            }
            if (chkIncludeBalance.Checked &&
                SecurityHelper.CurrentRole != UserRole.Admin &&
                SecurityHelper.CurrentRole != UserRole.Accountant)
            {
                chkIncludeBalance.Checked = false;
                throw new UnauthorizedAccessException("إظهار الرصيد المالي متاح لمدير النظام أو المحاسب فقط.");
            }
        }

        private Dictionary<int, WhatsAppConsent> LoadConsentMap()
        {
            var map = new Dictionary<int, WhatsAppConsent>();
            foreach (WhatsAppConsent consent in db.WhatsAppConsents.AsNoTracking().ToList())
                map[consent.ParentId] = consent;
            return map;
        }

        private static bool IsConsentValid(Dictionary<int, WhatsAppConsent> consents, int parentId, string phone)
        {
            WhatsAppConsent consent;
            if (consents == null || !consents.TryGetValue(parentId, out consent) || !consent.IsOptedIn) return false;
            string currentPhone = WhatsAppCloudApi.NormalizeRecipientNumber(phone);
            if (currentPhone.Length < 10) return false;
            return currentPhone == WhatsAppCloudApi.NormalizeRecipientNumber(consent.PhoneNumber);
        }

        private List<Recipient> LoadEligibleRecipients(int? classId, int? targetStudentId = null)
        {
            var query = db.Students.AsNoTracking().Include(x => x.Parent)
                .Include(x => x.ClassRoom).Include(x => x.ClassRoom.GradeLevel)
                .Where(x => x.IsActive && x.ParentId.HasValue);

            if (classId.HasValue)
            {
                query = query.Where(x => x.ClassRoomId == classId.Value);
            }
            else
            {
                // Check if specific Grade or Division is selected
                int gradeId;
                if (int.TryParse(ddlComposeGrade.SelectedValue, out gradeId))
                {
                    query = query.Where(x => x.ClassRoom.GradeLevelId == gradeId);
                    if (!string.IsNullOrEmpty(ddlComposeDivision.SelectedValue))
                    {
                        query = query.Where(x => x.ClassRoom.DivisionName == ddlComposeDivision.SelectedValue);
                    }
                }
                else
                {
                    EducationStage? stage = ResolveStage();
                    if (stage.HasValue) query = query.Where(x => x.ClassRoom.GradeLevel.Stage == stage.Value);
                }
            }

            // عند الإرسال الفردي تُستخدم بيانات الطالب المحدد فقط، حتى لو كان لولي الأمر أبناء آخرون.
            if (targetStudentId.HasValue)
            {
                query = query.Where(x => x.Id == targetStudentId.Value);
            }

            var students = query.ToList()
                .Where(x => x.Parent != null && !string.IsNullOrWhiteSpace(x.Parent.Phone)).ToList();
            var consents = LoadConsentMap();
            var result = new List<Recipient>();
            foreach (var group in students.GroupBy(x => x.ParentId.Value))
            {
                Parent parent = group.First().Parent;
                if (!IsConsentValid(consents, group.Key, parent.Phone)) continue;
                result.Add(new Recipient { Parent = parent, Students = group.OrderBy(x => x.Name).ToList() });
            }
            return result;
        }

        private string RenderMessage(string body, Recipient recipient, WhatsAppTemplate selectedTemplate, out List<string> orderedParameters)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "guardianName", recipient.Parent == null || string.IsNullOrWhiteSpace(recipient.Parent.Name) ? "ولي الأمر" : recipient.Parent.Name },
                { "studentName", JoinNames(recipient.Students) },
                { "class", ClassNames(recipient.Students) },
                { "date", DateTime.Today.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) },
                { "time", string.Empty },
                { "extra", string.Empty },
                { "dailyLessons", string.Empty },
                { "balance", chkIncludeBalance.Checked ? BuildBalanceSummary(recipient.Students) : "يرجى التواصل مع الإدارة" },
                { "attendanceSummary", chkIncludeAttendance.Checked ? BuildAttendanceSummary(recipient.Students) : "يرجى التواصل مع الإدارة" },
                { "latestGrade", chkIncludeGrades.Checked ? BuildLatestGradeSummary(recipient.Students) : "يرجى التواصل مع الإدارة" }
            };

            string renderedBody = body ?? string.Empty;
            if (chkAppendDailyLessons.Checked)
            {
                string dailyLessons = BuildDailyLessonSummary(recipient.Students);
                values["dailyLessons"] = dailyLessons;
                if (!ContainsMessageToken(renderedBody, "dailyLessons") && dailyLessons.Length > 0)
                    renderedBody = renderedBody.Length == 0
                        ? dailyLessons
                        : renderedBody + Environment.NewLine + dailyLessons;
            }

            return WhatsAppMessageFormatter.EnsureSchoolHeader(
                WhatsAppTemplateRenderer.Render(renderedBody, values, out orderedParameters));
        }

        private string BuildDailyLessonSummary(List<Student> students)
        {
            List<IGrouping<int, Student>> classGroups = (students ?? new List<Student>())
                .Where(x => x.ClassRoom != null)
                .GroupBy(x => x.ClassRoomId)
                .ToList();
            if (classGroups.Count == 0) return string.Empty;

            return string.Join(Environment.NewLine, classGroups.Select(group =>
                (classGroups.Count > 1 ? GetClassName(group.First().ClassRoom) + ": " : string.Empty) +
                BuildDailyLessonSummary(group.Key)));
        }

        private static bool ContainsMessageToken(string body, string token)
        {
            return !string.IsNullOrEmpty(body) &&
                body.IndexOf("{" + token + "}", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string JoinNames(List<Student> students)
        {
            var names = students.Select(x => x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            return names.Count == 0 ? string.Empty : string.Join(" و ", names);
        }

        private static string ClassNames(List<Student> students)
        {
            var names = students.Select(x => x.ClassRoom == null ? string.Empty : GetClassName(x.ClassRoom))
                .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            return string.Join("، ", names);
        }

        private decimal StudentBalance(int studentId)
        {
            return db.Invoices.AsNoTracking().Where(x => x.StudentId == studentId &&
                    x.Status != InvoiceStatus.Draft && x.Status != InvoiceStatus.Cancelled && x.AcademicYear.IsCurrent)
                .Select(x => new { x.NetAmount, x.PaidAmount }).ToList()
                .Sum(x => Math.Max(0m, x.NetAmount - x.PaidAmount));
        }

        private string BuildBalanceSummary(List<Student> students)
        {
            if (students.Count <= 1)
                return StudentBalance(students.Count == 0 ? 0 : students[0].Id).ToString("N2", CultureInfo.CurrentCulture) + " د.ل";
            decimal total = 0m;
            var parts = new List<string>();
            foreach (Student student in students)
            {
                decimal balance = StudentBalance(student.Id);
                total += balance;
                parts.Add(student.Name + ": " + balance.ToString("N2", CultureInfo.CurrentCulture) + " د.ل");
            }
            return "الإجمالي " + total.ToString("N2", CultureInfo.CurrentCulture) + " د.ل (" + string.Join("؛ ", parts) + ")";
        }

        private string BuildAttendanceSummary(List<Student> students)
        {
            DateTime from = DateTime.Today.AddDays(-6);
            DateTime to = DateTime.Today.AddDays(1);
            if (students.Count <= 1)
            {
                int single = students.Count == 0 ? 0 : AbsenceCount(students[0].Id, from, to);
                return "الغياب خلال آخر 7 أيام: " + single + " يوم";
            }
            var parts = new List<string>();
            foreach (Student student in students)
                parts.Add(student.Name + ": " + AbsenceCount(student.Id, from, to) + " يوم");
            return "الغياب خلال آخر 7 أيام (" + string.Join("؛ ", parts) + ")";
        }

        private int AbsenceCount(int studentId, DateTime from, DateTime to)
        {
            return db.Attendances.AsNoTracking()
                .Count(x => x.StudentId == studentId && !x.IsPresent && x.Date >= from && x.Date < to);
        }

        private string BuildLatestGradeSummary(List<Student> students)
        {
            if (students.Count <= 1)
                return BuildLatestGrade(students.Count == 0 ? 0 : students[0].Id);
            var parts = new List<string>();
            foreach (Student student in students)
                parts.Add(student.Name + " — " + BuildLatestGrade(student.Id));
            return string.Join("؛ ", parts);
        }

        private string BuildLatestGrade(int studentId)
        {
            var grade = db.MonthlySheetGrades.AsNoTracking().Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.MonthNumber).ThenByDescending(x => x.Id)
                .Select(x => new { x.Subject, x.Total, x.MonthNumber })
                .FirstOrDefault();
            return grade == null ? "لا توجد درجة حديثة مسجلة" :
                grade.Subject + ": " + (grade.Total ?? "—") + " (الشهر " + grade.MonthNumber + ")";
        }

        private string BuildDailyLessonSummary(int classId)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            var lessons = db.DailyLessons.AsNoTracking().Include(x => x.Subject).Include(x => x.Teacher)
                .Where(x => x.ClassRoomId == classId && x.Date >= today && x.Date < tomorrow && x.Status == LessonStatus.Approved)
                .OrderBy(x => x.PeriodNumber).ToList();
            return lessons.Count == 0 ? "لا يوجد جدول مسجل لهذا الفصل اليوم" :
                "دروس اليوم: " + string.Join("، ", lessons.Select(x =>
                    x.PeriodNumber + "-" + (x.Subject == null ? "حصة" : x.Subject.Name) +
                    (x.Teacher == null ? string.Empty : " مع " + x.Teacher.Name)));
        }

        private WhatsAppTemplate GetSelectedTemplate()
        {
            int id;
            return int.TryParse(ddlTemplate.SelectedValue, out id)
                ? db.WhatsAppTemplates.AsNoTracking().FirstOrDefault(x => x.Id == id && x.IsActive)
                : null;
        }

        private static bool TryParseLocalSchedule(string input, out DateTime local)
        {
            string[] formats = { "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss" };
            return DateTime.TryParseExact((input ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out local);
        }

        private void ShowNotice(string message, bool isError)
        {
            pnlNotice.Visible = true;
            pnlNotice.CssClass = isError ? "wa-notice wa-notice-warning" : "wa-notice";
            litNotice.Text = message;
        }

    }
}