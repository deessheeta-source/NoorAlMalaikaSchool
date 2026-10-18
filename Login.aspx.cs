using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            // See SecurityHelper.ViewStateUserKey for why this must not be Session.SessionID:
            // the login page stores nothing in Session, so no session cookie is ever sent and
            // Session.SessionID changes on every request, which invalidated the ViewState MAC
            // and made the login button impossible to submit.
            Page.ViewStateUserKey = SecurityHelper.ViewStateUserKey;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                SecurityHelper.RedirectAndStop("~/Default.aspx");
            }
        }

        private const int MaxAttempts = 5;
        private static readonly TimeSpan LockoutSpan = TimeSpan.FromMinutes(10);

        /// <summary>
        /// One message for every rejected login.
        ///
        /// It must not depend on WHY the attempt failed. A distinct message for a
        /// locked account tells an attacker that the username exists, which turns the
        /// login form into an account-enumeration oracle: they can list valid
        /// usernames from the response alone, and then attack only those with passwords.
        /// The lockout itself still applies, and the reason is still recorded in the
        /// audit log, so support can see what really happened.
        /// </summary>
        private const string InvalidCredentialsMessage = "اسم المستخدم أو كلمة المرور غير صحيحة";

        private bool IsLocked(User user)
        {
            return user != null
                && user.LockoutUntil.HasValue
                && user.LockoutUntil.Value > DateTime.Now;
        }

        private void RegisterFailure(User user)
        {
            if (user == null) return;

            if (user.LastFailedLoginAt.HasValue &&
                DateTime.Now.Subtract(user.LastFailedLoginAt.Value) > LockoutSpan)
            {
                user.FailedLoginCount = 0;
            }

            user.FailedLoginCount++;
            user.LastFailedLoginAt = DateTime.Now;

            if (user.FailedLoginCount >= MaxAttempts)
            {
                user.LockoutUntil = DateTime.Now.Add(LockoutSpan);
                user.FailedLoginCount = 0;
            }
        }

        private static bool IsSafeReturnUrl(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl)) return false;
            if (!returnUrl.StartsWith("/", StringComparison.Ordinal) ||
                returnUrl.StartsWith("//", StringComparison.Ordinal) ||
                returnUrl.StartsWith(@"/\\", StringComparison.Ordinal))
                return false;

            Uri absolute;
            if (Uri.TryCreate(returnUrl, UriKind.Absolute, out absolute))
                return false;

            return !returnUrl.Contains("\\");
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("الرجاء إدخال اسم المستخدم وكلمة المرور");
                return;
            }

            using (var db = new SchoolDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username);

                if (IsLocked(user))
                {
                    // Same message as a bad password: see InvalidCredentialsMessage.
                    // RegisterFailure is not called so a locked account is not extended
                    // by continued attempts, and the reason is not echoed to the caller.
                    db.AuditLogs.Add(new AuditLog
                    {
                        Timestamp = DateTime.Now,
                        UserName = username,
                        Action = "محاولة دخول أثناء الحظر",
                        EntityType = "User",
                        EntityId = user.Id.ToString(),
                        Details = "محاولة دخول لحساب محظور مؤقتاً",
                        IpAddress = Request.UserHostAddress ?? ""
                    });
                    db.SaveChanges();
                    ShowError(InvalidCredentialsMessage);
                    return;
                }

                bool valid = false;
                bool legacyPassword = false;
                if (user != null && user.IsActive)
                {
                    valid = PasswordHasher.Verify(password, user.PasswordHash);
                    if (!valid && PasswordHasher.IsLegacySha256(user.PasswordHash))
                    {
                        valid = PasswordHasher.VerifyLegacySha256(password, user.PasswordHash);
                        legacyPassword = valid;
                    }
                }

                if (!valid)
                {
                    RegisterFailure(user);
                    db.AuditLogs.Add(new AuditLog
                    {
                        Timestamp = DateTime.Now,
                        UserName = username,
                        Action = "محاولة دخول فاشلة",
                        EntityType = "User",
                        EntityId = user != null ? user.Id.ToString() : "",
                        Details = "فشل تسجيل الدخول",
                        IpAddress = Request.UserHostAddress ?? ""
                    });
                    db.SaveChanges();
                    ShowError(InvalidCredentialsMessage);
                    return;
                }

                Session.Remove("LoginFailures");
                Session.Remove("LoginLockUntil");
                user.FailedLoginCount = 0;
                user.LockoutUntil = null;
                user.LastFailedLoginAt = null;
                user.LastLoginAt = DateTime.Now;

                if (legacyPassword)
                    user.PasswordHash = PasswordHasher.Hash(password);

                FormsAuthentication.SetAuthCookie(username, false);

                // Session fixation defence: drop the pre-authentication session so a
                // session id planted before login cannot be reused afterwards.
                //
                // The redirect below MUST be RedirectAndSaveSession, not
                // RedirectAndStop. RedirectAndStop uses endResponse:true, which throws
                // ThreadAbortException and ends the request before ASP.NET commits the
                // session, so the values written after the abandon are thrown away:
                // Session["Role"] comes back null on the next request and the freshly
                // authenticated admin is bounced to AccessDenied.aspx from every
                // protected page. See SecurityHelper.RedirectAndSaveSession.
                Session.Abandon();

                Session["Username"] = username;
                Session["DisplayName"] = user.DisplayName;
                Session["Role"] = user.Role;
                Session["EmployeeNo"] = user.EmployeeNo;
                Session["MustChangePassword"] = user.MustChangePassword;

                db.AuditLogs.Add(new AuditLog
                {
                    Timestamp = DateTime.Now,
                    UserName = user.DisplayName,
                    Action = "تسجيل دخول",
                    EntityType = "User",
                    EntityId = user.Id.ToString(),
                    Details = "تسجيل دخول ناجح (" + user.Role.GetDisplayName() + ")",
                    IpAddress = Request.UserHostAddress ?? ""
                });
                db.SaveChanges();

                if (user.MustChangePassword)
                {
                    SecurityHelper.RedirectAndSaveSession("~/ChangePassword.aspx");
                    return;
                }

                string returnUrl = Request.QueryString["ReturnUrl"];
                if (IsSafeReturnUrl(returnUrl))
                {
                    SecurityHelper.RedirectAndSaveSession(returnUrl);
                }
                else
                {
                    SecurityHelper.RedirectAndSaveSession("~/Default.aspx");
                }
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.CssClass = "alert alert-danger d-block mb-3";
            lblError.Visible = true;
        }
    }
}
