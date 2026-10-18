using System;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Classes
{
    public partial class Delete : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
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

            if (!IsPostBack)
            {
                string idStr = Request.QueryString["id"];
                int id;
                if (!string.IsNullOrEmpty(idStr) && int.TryParse(idStr, out id))
                {
                    LoadClassRoom(id);
                }
                else
                {
                    pnlConfirm.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadClassRoom(int id)
        {
            var classRoom = _context.ClassRooms.Find(id);
            if (classRoom == null)
            {
                pnlConfirm.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            hfClassId.Value = classRoom.Id.ToString();

            // =====================================================
            // ✅ السنة الدراسية
            // =====================================================
            lblAcademicYear.Text = classRoom.AcademicYear != null
                ? classRoom.AcademicYear.Name
                : "—";

            // =====================================================
            // ✅ الصف الدراسي
            // =====================================================
            lblGradeLevel.Text = classRoom.GradeLevel != null
                ? classRoom.GradeLevel.Name
                : "—";

            // =====================================================
            // ✅ القسم (من ClassRoom.SectionName)
            // =====================================================
            string section = (classRoom.SectionName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(section))
            {
                lblSection.Text = "—";
            }
            else if (section == "علمي")
            {
                lblSection.Text = "القسم العلمي";
            }
            else if (section == "أدبي")
            {
                lblSection.Text = "القسم الأدبي";
            }
            else
            {
                lblSection.Text = section;
            }

            // =====================================================
            // ✅ الشعبة (من ClassRoom.DivisionName)
            // =====================================================
            string division = (classRoom.DivisionName ?? "").Trim();

            lblDivision.Text = string.IsNullOrWhiteSpace(division)
                ? "—"
                : "شعبة " + division;

            // =====================================================
            // ✅ عدد الطلاب النشطين + التحقق من الارتباطات
            // =====================================================
            int studentsCount = _context.Students
                .Count(s => s.ClassRoomId == id && s.IsActive);

            int inactiveStudentsCount = _context.Students
                .Count(s => s.ClassRoomId == id && !s.IsActive);

            int feesCount = _context.Fees
                .Count(f => f.ClassRoomId == id);

            int timetableCount = _context.TimetableEntries
                .Count(t => t.ClassRoomId == id);

            int dailyLessonsCount = _context.DailyLessons
                .Count(d => d.ClassRoomId == id);

            int substitutionsCount = _context.Substitutions
                .Count(s => s.ClassRoomId == id);

            lblStudentsCount.Text = studentsCount.ToString();

            // =====================================================
            // ✅ منع الحذف عند وجود ارتباطات
            // =====================================================
            if (studentsCount > 0 || inactiveStudentsCount > 0
                || feesCount > 0 || timetableCount > 0
                || dailyLessonsCount > 0 || substitutionsCount > 0)
            {
                btnConfirmDelete.Enabled = false;
                btnConfirmDelete.CssClass = "cls-btn-outline";
                btnConfirmDelete.Style["pointer-events"] = "none";
                btnConfirmDelete.Style["opacity"] = "0.5";
                btnConfirmDelete.Style["cursor"] = "not-allowed";

                lblMessage.CssClass = "cls-message warning";
                lblMessage.Text = "لا يمكن حذف هذا الفصل لأنه مرتبط بسجلات تشغيلية أو تاريخية (طلاب / رسوم / جدول / حصص / تعويضيات). يجب الاحتفاظ به حفاظاً على سلامة السجل.";
            }
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(hfClassId.Value);
                var classRoom = _context.ClassRooms.Find(id);

                if (classRoom == null)
                {
                    lblMessage.CssClass = "cls-message danger";
                    lblMessage.Text = "الفصل غير موجود";
                    return;
                }

                bool hasDependencies =
                    _context.Students.Any(s => s.ClassRoomId == id)
                    || _context.Fees.Any(f => f.ClassRoomId == id)
                    || _context.TimetableEntries.Any(t => t.ClassRoomId == id)
                    || _context.DailyLessons.Any(d => d.ClassRoomId == id)
                    || _context.Substitutions.Any(s => s.ClassRoomId == id);

                if (hasDependencies)
                {
                    lblMessage.CssClass = "cls-message warning";
                    lblMessage.Text = "لا يمكن حذف الفصل لوجود سجلات مرتبطة به. احتفظ بالفصل للحفاظ على السجل التاريخي.";
                    return;
                }

                _context.ClassRooms.Remove(classRoom);
                _context.SaveChanges();

                string stage = hfStage.Value;
                Response.Redirect("Default.aspx?stage=" + stage);
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "cls-message danger";
                lblMessage.Text = ErrorMessageHelper.GetUserMessage(ex, GetType().FullName);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string stage = hfStage.Value;
            Response.Redirect("Default.aspx?stage=" + stage);
        }

        protected void btnBackToList_Click(object sender, EventArgs e)
        {
            string stage = hfStage.Value;
            Response.Redirect("Default.aspx?stage=" + stage);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_context != null) _context.Dispose();
            base.OnUnload(e);
        }
    }
}