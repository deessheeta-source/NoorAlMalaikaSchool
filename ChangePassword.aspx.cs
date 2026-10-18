using System;
using System.Linq;
using System.Web.Security;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            // See SecurityHelper.ViewStateUserKey for why this must not be Session.SessionID.
            Page.ViewStateUserKey = SecurityHelper.ViewStateUserKey;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string username = User.Identity.Name;
            string current = txtCurrentPassword.Text;
            string next = txtNewPassword.Text;
            string confirm = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(current) || !PasswordPolicy.IsValid(next))
            {
                ShowMessage("بيانات كلمة المرور غير صالحة.", false);
                return;
            }
            if (!string.Equals(next, confirm, StringComparison.Ordinal))
            {
                ShowMessage("تأكيد كلمة المرور غير مطابق.", false);
                return;
            }

            using (var db = new SchoolDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user == null || !user.IsActive)
                {
                    FormsAuthentication.SignOut();
                    Session.Clear();
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                bool validCurrent = PasswordHasher.Verify(current, user.PasswordHash)
                    || PasswordHasher.VerifyLegacySha256(current, user.PasswordHash);
                if (!validCurrent)
                {
                    ShowMessage("كلمة المرور الحالية غير صحيحة.", false);
                    return;
                }

                user.PasswordHash = PasswordHasher.Hash(next);
                user.MustChangePassword = false;
                user.FailedLoginCount = 0;
                user.LockoutUntil = null;
                user.LastFailedLoginAt = null;

                db.AuditLogs.Add(new NoorAlMalaikaSchool.Models.AuditLog
                {
                    Timestamp = DateTime.Now,
                    UserName = user.DisplayName,
                    Action = "تغيير كلمة المرور",
                    EntityType = "User",
                    EntityId = user.Id.ToString(),
                    Details = "تم تغيير كلمة المرور بنجاح",
                    IpAddress = Request.UserHostAddress ?? ""
                });
                db.SaveChanges();
                Session["MustChangePassword"] = false;
            }

            Response.Redirect("~/Default.aspx");
        }

        private void ShowMessage(string text, bool success)
        {
            lblMessage.CssClass = success ? "alert alert-success d-block" : "alert alert-danger d-block";
            lblMessage.Text = text;
        }
    }
}
