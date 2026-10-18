using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Students
{
    public partial class Delete : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();
        private bool IsHardMode;

        private const string FlashMessageKey = "Students_FlashMessage";

        /// <summary>مدة عرض الرسالة قبل إعادة التوجيه (بالمللي ثانية).</summary>
        private const int RedirectDelayMs = 1800;

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();

            string stage = Request.QueryString["stage"];
            if (!string.IsNullOrEmpty(stage))
            {
                hfStage.Value = stage;
            }
            else if (Session["Stage"] != null)
            {
                hfStage.Value = Session["Stage"].ToString();
            }

            IsHardMode = string.Equals(Request.QueryString["mode"], "hard", StringComparison.OrdinalIgnoreCase);

            // ⭐ عناوين الوضع الحالي
            lblTitle.Text = IsHardMode ? "حذف الطالب نهائياً" : "تعطيل حساب الطالب";
            lblWarningTitle.Text = IsHardMode
                ? "هل أنت متأكد من حذف هذا الطالب نهائياً؟"
                : "هل أنت متأكد من تعطيل حساب هذا الطالب؟";
            lblWarning.Text = IsHardMode
                ? "لا يمكن التراجع عن هذا الإجراء — سيُحذف الطالب وجميع بياناته المرتبطة."
                : "لن يتم حذفه نهائياً — يمكن إعادة تفعيله لاحقاً من قائمة الطلاب.";
            btnConfirmDelete.Text = IsHardMode ? "حذف نهائي" : "تعطيل الحساب";

            // ⭐ Chip في الـ Hero
            string stageText = "";
            try
            {
                if (!string.IsNullOrEmpty(hfStage.Value))
                    stageText = "المستوى: " + EnumHelper.GetStageArabic(hfStage.Value);
            }
            catch { stageText = hfStage.Value ?? ""; }

            if (lblStageChip != null)
                lblStageChip.Text = stageText;

            if (!IsPostBack)
            {
                int id;
                if (int.TryParse(Request.QueryString["id"], out id) && id > 0)
                {
                    LoadStudent(id);
                }
                else
                {
                    pnlConfirm.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadStudent(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null)
            {
                pnlConfirm.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            EducationStage stageEnum;
            if (!string.IsNullOrEmpty(hfStage.Value)
                && Enum.TryParse(hfStage.Value, true, out stageEnum)
                && student.Stage != stageEnum)
            {
                pnlConfirm.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            hfStudentId.Value = student.Id.ToString();
            lblName.Text = Server.HtmlEncode(student.Name ?? string.Empty);
            lblNationalId.Text = Server.HtmlEncode(student.NationalId ?? string.Empty);
            lblPhone.Text = Server.HtmlEncode(string.IsNullOrWhiteSpace(student.Phone) ? "غير محدد" : student.Phone);

            if (student.ClassRoom != null && !string.IsNullOrWhiteSpace(student.ClassRoom.Name))
            {
                lblClassRoom.Text = Server.HtmlEncode(student.ClassRoom.Name);
            }
            else
            {
                lblClassRoom.Text = "غير محدد";
            }

            if (Enum.IsDefined(typeof(EducationStage), student.Stage))
            {
                lblStage.Text = EnumHelper.GetDisplayName(student.Stage);
            }
            else
            {
                lblStage.Text = "غير محدد";
            }
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (!int.TryParse(hfStudentId.Value, out id) || id <= 0)
                {
                    ShowError("معرّف الطالب غير صالح.");
                    return;
                }

                var student = _context.Students.Find(id);
                if (student == null)
                {
                    ShowError("الطالب غير موجود!");
                    return;
                }

                if (IsHardMode)
                {
                    HandleHardDelete(student);
                }
                else
                {
                    HandleSoftDelete(student);
                }
            }
            catch (DbUpdateException)
            {
                ShowError("لا يمكن الحذف النهائي لوجود سجلات مرتبطة بالطالب. استخدم التعطيل بدلاً من ذلك.");
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowError(ErrorMessageHelper.GetUserMessage(ex, GetType().FullName));
            }
        }

        private void HandleSoftDelete(Student student)
        {
            student.IsActive = false;
            _context.SaveChanges();

            SetFlashMessage("تم تعطيل حساب الطالب: " + (student.Name ?? ""), "success");

            lblMessage.CssClass = "del-alert success";
            lblMessage.Text = "<i class='fas fa-check-circle'></i><span>✅ تم تعطيل حساب الطالب بنجاح! جاري العودة للقائمة...</span>";
            lblMessage.Visible = true;
            pnlConfirm.Visible = false;

            ScheduleRedirect();
        }

        private void HandleHardDelete(Student student)
        {
            int studentId = student.Id;

            bool hasRelated =
                _context.Attendances.Any(a => a.StudentId == studentId) ||
                _context.Payments.Any(p => p.StudentId == studentId);

            if (hasRelated)
            {
                ShowError("لا يمكن حذف هذا الطالب نهائياً لأنه مرتبط بسجلات (حضور/مدفوعات). يُرجى الاكتفاء بالتعطيل أو مراجعة السجلات المرتبطة.");
                pnlConfirm.Visible = false;
                return;
            }

            string studentName = student.Name;
            string photoPath = student.PhotoPath;

            _context.Students.Remove(student);
            _context.SaveChanges();

            DeletePhotoFile(photoPath);

            SetFlashMessage("تم حذف الطالب نهائياً: " + (studentName ?? ""), "success");

            lblMessage.CssClass = "del-alert success";
            lblMessage.Text = "<i class='fas fa-check-circle'></i><span>✅ تم حذف الطالب نهائياً بنجاح! جاري العودة للقائمة...</span>";
            lblMessage.Visible = true;
            pnlConfirm.Visible = false;

            ScheduleRedirect();
        }

        private void ScheduleRedirect()
        {
            string stage = hfStage.Value ?? string.Empty;
            string url = "Default.aspx?stage=" + HttpUtility.UrlEncode(stage);

            string script =
                "setTimeout(function(){ " +
                "  window.location.href = '" + url.Replace("'", "\\'") + "'; " +
                "}, " + RedirectDelayMs.ToString() + ");";

            try
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "redirectAfterDelay",
                    script,
                    true);
            }
            catch
            {
                Response.Redirect(url, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private void DeletePhotoFile(string photoPath)
        {
            if (string.IsNullOrEmpty(photoPath)) return;
            try
            {
                string studentFolder = HostingEnvironment.MapPath("~/Content/Uploads/Students");
                FileUploadService.DeleteStudentPhoto(photoPath, studentFolder);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void ShowError(string message)
        {
            lblMessage.CssClass = "del-alert danger";
            lblMessage.Text = "<i class='fas fa-exclamation-triangle'></i><span>" +
                              Server.HtmlEncode(message) + "</span>";
            lblMessage.Visible = true;
        }

        private void SetFlashMessage(string text, string type)
        {
            try
            {
                Session[FlashMessageKey] = new FlashMessage
                {
                    Text = text,
                    Type = type
                };
            }
            catch { }
        }

        private void RedirectToList()
        {
            string stage = hfStage.Value ?? string.Empty;
            Response.Redirect("Default.aspx?stage=" + HttpUtility.UrlEncode(stage), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void LogError(Exception ex)
        {
            try
            {
                string path = HostingEnvironment.MapPath("~/App_Data/errors.log");
                if (path == null) return;

                string line = string.Format(
                    "[{0:yyyy-MM-dd HH:mm:ss}] Delete.aspx.cs{1}{2}{1}---{1}",
                    DateTime.Now, Environment.NewLine, ex);

                System.IO.File.AppendAllText(path, line);
            }
            catch { }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectToList();
        }

        protected void btnBackToList_Click(object sender, EventArgs e)
        {
            RedirectToList();
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_context != null)
            {
                try { _context.Dispose(); }
                catch { }
            }
            base.OnUnload(e);
        }

        /// <summary>كيان الرسالة المؤقتة.</summary>
        public class FlashMessage
        {
            public string Text { get; set; }
            public string Type { get; set; }
        }
    }
}