using System;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Subjects
{
    public partial class Delete : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                string idStr = Request.QueryString["id"];
                int id;
                if (!string.IsNullOrEmpty(idStr) && int.TryParse(idStr, out id))
                {
                    LoadSubject(id);
                }
                else
                {
                    pnlConfirm.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadSubject(int id)
        {
            var subject = _context.Subjects.Find(id);
            if (subject == null)
            {
                pnlConfirm.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            hfSubjectId.Value = subject.Id.ToString();
            lblName.Text = subject.Name;
            lblCode.Text = !string.IsNullOrEmpty(subject.Code) ? subject.Code : "غير محدد";
            lblDescription.Text = !string.IsNullOrEmpty(subject.Description) ? subject.Description : "غير محدد";
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(hfSubjectId.Value);
                var subject = _context.Subjects.Find(id);
                if (subject == null)
                {
                    lblMessage.CssClass = "alert alert-danger d-block";
                    lblMessage.Text = "المادة غير موجودة";
                    return;
                }

                bool hasDependencies = _context.TimetableEntries.Any(t => t.SubjectId == id)
                    || _context.DailyLessons.Any(d => d.SubjectId == id)
                    || _context.Substitutions.Any(s => s.SubjectId == id);
                if (hasDependencies)
                {
                    lblMessage.CssClass = "alert alert-warning d-block";
                    lblMessage.Text = "لا يمكن حذف المادة لوجود جدول أو حصص أو تعويضيات مرتبطة بها. احتفظ بها للحفاظ على السجل التاريخي.";
                    return;
                }

                _context.Subjects.Remove(subject);
                _context.SaveChanges();
                Response.Redirect("Default.aspx");
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "alert alert-danger d-block";
                lblMessage.Text = ErrorMessageHelper.GetUserMessage(ex, GetType().FullName);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_context != null) _context.Dispose();
            base.OnUnload(e);
        }
    }
}
