using System;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Security
{
    public partial class CreateUser : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnless(UserRole.Admin);

            if (!IsPostBack)
            {
                LoadTeachers();
            }
        }

        private void LoadTeachers()
        {
            ddlTeacher.Items.Clear();
            ddlTeacher.Items.Add(new System.Web.UI.WebControls.ListItem("— اختر المعلم —", ""));
            var teachers = db.Teachers
                .OrderBy(t => t.Name)
                .Select(t => new { t.Id, t.EmployeeNo, t.Name })
                .ToList();

            foreach (var t in teachers)
            {
                string label = t.Name + (string.IsNullOrEmpty(t.EmployeeNo) ? "" : " (" + t.EmployeeNo + ")");
                ddlTeacher.Items.Add(new System.Web.UI.WebControls.ListItem(label, t.EmployeeNo ?? ""));
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnless(UserRole.Admin);

            string username = (txtUsername.Text ?? string.Empty).Trim();
            string displayName = (txtDisplayName.Text ?? string.Empty).Trim();
            string password = txtPassword.Text;
            string employeeNo = null;

            UserRole role;
            if (!Enum.TryParse<UserRole>(ddlRole.SelectedValue, true, out role) ||
                !Enum.IsDefined(typeof(UserRole), role))
            {
                ShowMessage("الدور المحدد غير صالح.", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(username) || username.Length > 100 || username.Any(char.IsControl) ||
                string.IsNullOrEmpty(password))
            {
                ShowMessage("اسم المستخدم (بحد أقصى 100 حرف) وكلمة المرور مطلوبان، ولا يجوز أن يحتوي الاسم على محارف تحكم.", false);
                return;
            }

            if (displayName.Length > 100)
            {
                ShowMessage("اسم العرض يجب ألا يتجاوز 100 حرف.", false);
                return;
            }

            if (!PasswordPolicy.IsValid(password))
            {
                ShowMessage("كلمة المرور يجب أن تكون 8 أحرف على الأقل وتحتوي على حرف كبير وحرف صغير ورقم ورمز خاص", false);
                return;
            }

            if (db.Users.Any(u => u.Username == username))
            {
                ShowMessage("اسم المستخدم موجود مسبقاً", false);
                return;
            }

            if (role == UserRole.Teacher)
            {
                employeeNo = ddlTeacher.SelectedValue;
                if (string.IsNullOrEmpty(employeeNo))
                {
                    ShowMessage("يرجى اختيار المعلم المرتبط بالحساب", false);
                    return;
                }

                if (!db.Teachers.Any(t => t.EmployeeNo == employeeNo))
                {
                    ShowMessage("المعلم المحدد غير موجود", false);
                    return;
                }

                if (db.Users.Any(u => u.EmployeeNo == employeeNo))
                {
                    ShowMessage("لهذا المعلم حساب مسبقاً", false);
                    return;
                }
            }

            if (role != UserRole.Teacher)
                employeeNo = null;

            db.Users.Add(new User
            {
                Username = username,
                DisplayName = string.IsNullOrEmpty(displayName) ? username : displayName,
                PasswordHash = PasswordHasher.Hash(password),
                MustChangePassword = true,
                IsActive = chkActive.Checked,
                Role = role,
                EmployeeNo = employeeNo
            });
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                System.Diagnostics.Trace.TraceError("CreateUser duplicate/constraint failure: " + ex);
                ShowMessage("تعذر إنشاء الحساب. تحقق من عدم تكرار اسم المستخدم أو ربط المعلم ثم أعد المحاولة.", false);
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("CreateUser save failed: " + ex);
                ShowMessage("تعذر إنشاء الحساب بسبب خطأ داخلي. راجع سجل النظام.", false);
                return;
            }

            ShowMessage("تم إنشاء الحساب بنجاح", true);
            txtUsername.Text = "";
            txtDisplayName.Text = "";
            txtPassword.Text = "";
        }

        private void ShowMessage(string text, bool success)
        {
            lblMessage.CssClass = success ? "cu-message success" : "cu-message danger";
            lblMessage.Text = System.Web.HttpUtility.HtmlEncode(text ?? string.Empty);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}