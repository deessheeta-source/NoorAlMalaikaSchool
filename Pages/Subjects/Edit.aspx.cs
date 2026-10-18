using System;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Subjects
{
    public partial class Edit : Page
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
                    pnlForm.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private void LoadSubject(int id)
        {
            var subject = _context.Subjects.Find(id);
            if (subject == null)
            {
                pnlForm.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            hfSubjectId.Value = subject.Id.ToString();
            txtName.Text = subject.Name;
            txtCode.Text = subject.Code;
            txtDescription.Text = subject.Description;
            ddlStage.SelectedValue = subject.Stage.ToString();
            ddlCategory.SelectedValue = subject.Category.ToString();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(hfSubjectId.Value);
                var subject = _context.Subjects.Find(id);
                if (subject == null)
                {
                    lblMessage.CssClass = "alert alert-danger d-block";
                    lblMessage.Text = "المادة غير موجودة!";
                    return;
                }

                subject.Name = txtName.Text.Trim();
                subject.Code = string.IsNullOrWhiteSpace(txtCode.Text) ? null : txtCode.Text.Trim();
                subject.Description = txtDescription.Text.Trim();
                subject.Stage = (EducationStage)Enum.Parse(typeof(EducationStage), ddlStage.SelectedValue);
                subject.Category = (SubjectCategory)Enum.Parse(typeof(SubjectCategory), ddlCategory.SelectedValue);

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
