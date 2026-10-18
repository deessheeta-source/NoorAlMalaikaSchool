using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;
using System;
using System.Linq;
using System.Web.UI;

namespace NoorAlMalaikaSchool.Pages.Teachers
{
    public partial class Delete : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();
        private int _teacherId;

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            try
            {
                if (!int.TryParse(Request.QueryString["id"], out _teacherId) || _teacherId <= 0)
                {
                    Response.Redirect("Default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                if (!IsPostBack)
                    LoadTeacherInfo();
            }
            catch (Exception ex)
            {
                ShowMessage(ErrorMessageHelper.GetUserMessage(ex, GetType().FullName), false);
            }
        }

        #endregion

        #region Load

        private void LoadTeacherInfo()
        {
            var teacher = _context.Teachers
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == _teacherId);

            if (teacher == null)
            {
                pnlNotFound.Visible = true;
                pnlConfirm.Visible = false;
                pnlBlocked.Visible = false;
                return;
            }

            pnlNotFound.Visible = false;

            lblName.Text = teacher.Name ?? "—";
            lblEmployeeNo.Text = string.IsNullOrWhiteSpace(teacher.EmployeeNo) ? "—" : teacher.EmployeeNo;
            lblSpecialty.Text = string.IsNullOrWhiteSpace(teacher.Specialty) ? "—" : teacher.Specialty;
            lblStatus.Text = teacher.IsActive
                ? "<span class='badge-green'>نشط</span>"
                : "<span class='badge-gray'>غير نشط</span>";

            string blockReason;
            if (HasRelatedData(_teacherId, out blockReason))
            {
                pnlConfirm.Visible = false;
                pnlBlocked.Visible = true;
                lblBlockReason.Text = blockReason;

                // بيانات إضافية للعرض في القسم المحظور (إن كانت موجودة في .aspx)
                if (lblName2 != null) lblName2.Text = teacher.Name ?? "—";
                if (lblSpecialty2 != null) lblSpecialty2.Text = string.IsNullOrWhiteSpace(teacher.Specialty) ? "—" : teacher.Specialty;
                if (lblStatus2 != null) lblStatus2.Text = lblStatus.Text;
            }
            else
            {
                pnlConfirm.Visible = true;
                pnlBlocked.Visible = false;
            }
        }

        #endregion

        #region Related Data Check

        private bool HasRelatedData(int teacherId, out string reason)
        {
            reason = null;

            // 1) حصص يومية
            int lessonsCount = _context.DailyLessons.Count(d => d.TeacherId == teacherId);
            if (lessonsCount > 0)
            {
                reason = "يوجد " + lessonsCount + " حصة يومية مرتبطة بهذا المعلم.";
                return true;
            }

            // 2) سجلات حضور
            int attendanceCount = _context.AttendanceRecords.Count(r => r.TeacherId == teacherId);
            if (attendanceCount > 0)
            {
                reason = "يوجد " + attendanceCount + " سجل حضور مرتبط بهذا المعلم.";
                return true;
            }

            // 3) تعويضيات
            int subsCount = _context.Substitutions.Count(s =>
                s.OriginalTeacherId == teacherId || s.SubstituteTeacherId == teacherId);
            if (subsCount > 0)
            {
                reason = "يوجد " + subsCount + " تعويضية مرتبطة بهذا المعلم.";
                return true;
            }

            // 4) استحقاق مالي
            int payrollCount = _context.TeacherPayrolls.Count(p => p.TeacherId == teacherId);
            if (payrollCount > 0)
            {
                reason = "يوجد " + payrollCount + " سجل استحقاق مالي مرتبط بهذا المعلم.";
                return true;
            }

            // 5) جدول دراسي
            int timetableCount = _context.TimetableEntries.Count(t => t.TeacherId == teacherId);
            if (timetableCount > 0)
            {
                reason = "يوجد " + timetableCount + " إدخال في الجدول الدراسي مرتبط بهذا المعلم.";
                return true;
            }

            return false;
        }

        #endregion

        #region Actions

        /// <summary>
        /// زر "تأكيد الحذف النهائي" — يتحقق من عدم وجود ارتباطات ثم يحذف.
        /// </summary>
        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var teacher = _context.Teachers.Find(_teacherId);
                if (teacher == null)
                {
                    ShowMessage("المعلم غير موجود.", false);
                    return;
                }

                string blockReason;
                if (HasRelatedData(_teacherId, out blockReason))
                {
                    ShowMessage("لا يمكن الحذف — " + blockReason, false);
                    LoadTeacherInfo();
                    return;
                }

                _context.Teachers.Remove(teacher);
                _context.SaveChanges();

                Response.Redirect("Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                ShowMessage(ErrorMessageHelper.GetUserMessage(ex, GetType().FullName), false);
            }
        }

        /// <summary>
        /// زر "إلغاء التنشيط".
        /// </summary>
        protected void btnDeactivate_Click(object sender, EventArgs e)
        {
            try
            {
                var teacher = _context.Teachers.Find(_teacherId);
                if (teacher == null)
                {
                    ShowMessage("المعلم غير موجود.", false);
                    return;
                }

                teacher.IsActive = false;
                _context.SaveChanges();

                Response.Redirect("Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                ShowMessage(ErrorMessageHelper.GetUserMessage(ex, GetType().FullName), false);
            }
        }

        /// <summary>
        /// زر "إلغاء" و"رجوع للقائمة" و"العودة للقائمة".
        /// </summary>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        #endregion

        #region Messages / Dispose

        private void ShowMessage(string text, bool success)
        {
            if (lblMessage == null) return;

            lblMessage.CssClass = success
                ? "alert alert-success d-block"
                : "alert alert-danger d-block";
            lblMessage.Text = text ?? "";
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_context != null) _context.Dispose();
            base.OnUnload(e);
        }

        #endregion
    }
}