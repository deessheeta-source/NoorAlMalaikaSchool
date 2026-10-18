using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Teachers
{
    public partial class Subjects : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();
        private int TeacherId { get { int id; return int.TryParse(Request.QueryString["id"], out id) ? id : 0; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadTeacher();
                LoadSubjects();
                LoadAssignments();
            }
        }

        private void LoadTeacher()
        {
            var teacher = db.Teachers.Find(TeacherId);
            if (teacher == null)
            {
                lblMessage.Text = "المعلم غير موجود.";
                btnSave.Enabled = false;
                return;
            }
            lblTeacherName.Text = teacher.Name;
        }

        private void LoadSubjects()
        {
            var teacher = db.Teachers.Find(TeacherId);
            if (teacher == null) return;

            var subjects = db.Subjects
                .Where(s => s.Stage == teacher.Stage)
                .OrderBy(s => s.Name)
                .ToList();

            ddlSubject.Items.Clear();
            ddlSubject.Items.Add(new ListItem("-- اختر المادة --", ""));
            foreach (var subject in subjects)
                ddlSubject.Items.Add(new ListItem(subject.Name, subject.Id.ToString(CultureInfo.InvariantCulture)));
        }

        private void LoadAssignments()
        {
            var rows = (from a in db.TeacherSubjectAssignments
                        join s in db.Subjects on a.SubjectId equals s.Id
                        where a.TeacherId == TeacherId
                        orderby a.IsActive descending, s.Name
                        select new
                        {
                            a.Id,
                            SubjectName = s.Name,
                            a.LessonRate,
                            a.HourlyRate,
                            a.StartDate,
                            a.EndDate,
                            a.IsActive
                        }).ToList();
            gvAssignments.DataSource = rows;
            gvAssignments.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int subjectId;
                if (!int.TryParse(ddlSubject.SelectedValue, out subjectId) || subjectId <= 0)
                {
                    ShowMessage("اختر المادة أولاً.", false);
                    return;
                }

                decimal lessonRate = ParseMoney(txtLessonRate.Text);
                decimal hourlyRate = ParseMoney(txtHourlyRate.Text);
                if (lessonRate <= 0m)
                {
                    ShowMessage("قيمة الحصة مطلوبة ويجب أن تكون أكبر من صفر.", false);
                    return;
                }

                var teacher = db.Teachers.Find(TeacherId);
                var subject = db.Subjects.Find(subjectId);
                if (teacher == null || subject == null)
                {
                    ShowMessage("المعلم أو المادة غير موجود.", false);
                    return;
                }

                if (teacher.Stage != subject.Stage)
                {
                    ShowMessage("لا يمكن ربط المعلم بمادة من مرحلة دراسية مختلفة.", false);
                    return;
                }

                var assignment = db.TeacherSubjectAssignments
                    .FirstOrDefault(a => a.TeacherId == TeacherId && a.SubjectId == subjectId && a.IsActive);

                if (assignment == null)
                {
                    assignment = new TeacherSubjectAssignment
                    {
                        TeacherId = TeacherId,
                        SubjectId = subjectId,
                        StartDate = DateTime.Today,
                        IsActive = true
                    };
                    db.TeacherSubjectAssignments.Add(assignment);
                }

                assignment.LessonRate = lessonRate;
                assignment.HourlyRate = hourlyRate;
                assignment.IsActive = true;
                db.SaveChanges();

                ShowMessage("تم حفظ تكليف المادة وسعرها بنجاح.", true);
                txtLessonRate.Text = "";
                txtHourlyRate.Text = "";
                LoadAssignments();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ErrorMessageHelper.GetUserMessage(ex, "Teachers.Subjects.Save");
                lblMessage.CssClass = "text-danger d-block mb-3";
            }
        }

        protected void gvAssignments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Toggle") return;
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            var assignment = db.TeacherSubjectAssignments.Find(id);
            if (assignment == null) return;

            assignment.IsActive = !assignment.IsActive;
            if (!assignment.IsActive)
                assignment.EndDate = DateTime.Today;
            else
                assignment.EndDate = null;

            db.SaveChanges();
            LoadAssignments();
        }

        private decimal ParseMoney(string value)
        {
            decimal result;
            return decimal.TryParse(value, out result) && result > 0m ? result : 0m;
        }

        private void ShowMessage(string message, bool success)
        {
            lblMessage.CssClass = success ? "text-success d-block mb-3" : "text-danger d-block mb-3";
            lblMessage.Text = Server.HtmlEncode(message);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}
