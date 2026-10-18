
using System;
using System.Linq;
using System.Data.Entity;
using System.Web;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Services
{
    public static class SecurityHelper
    {
        // =========================================================
        // Session-cache keys
        // =========================================================
        // مفاتيح الـ Session المستخدمة لتخزين دور المستخدم مؤقتاً
        // لتقليل عدد استعلامات قاعدة البيانات في كل طلب.
        private const string SessionKeyRole           = "Role";
        private const string SessionKeyCacheUsername  = "_CachedUsername";
        private const string SessionKeyCacheExpiry    = "_RoleCacheExpiry";

        /// <summary>
        /// المدة التي يظل فيها الكاش صالحاً قبل التحقق من DB من جديد.
        /// تُختار 5 دقائق: ترضي الأداء وتضمن اكتشاف تعطيل الحساب
        /// أو تغيير الدور خلال وقت معقول.
        /// </summary>
        private static readonly TimeSpan RoleCacheDuration = TimeSpan.FromMinutes(5);

        /// <summary>
        /// دور المستخدم الحالي.
        /// يُقرأ من Session الكاش إن كان سارياً، وإلا يُحدَّث من قاعدة البيانات.
        /// </summary>
        public static UserRole? CurrentRole
        {
            get
            {
                return ResolveRole();
            }
        }

        /// <summary>
        /// يُعيد دور المستخدم الحالي مستخدماً طبقة كاش في Session
        /// لتجنب استعلام DB في كل طلب.
        ///
        /// منطق الكاش:
        ///  1. إن لم يكن هناك مستخدم مصادق عليه → null فوراً.
        ///  2. إن كان الكاش سارياً وينتمي لنفس المستخدم → يُرجع الدور منه.
        ///  3. في كل الحالات الأخرى (انتهى الكاش / تغيّر المستخدم) → يستعلم DB
        ///     ويُجدّد الكاش لـ RoleCacheDuration قادمة.
        ///
        /// الإبطال الفوري:
        ///  - تسجيل الخروج يستدعي InvalidateCurrentAuthentication() التي تحذف Session.
        ///  - تغيير الدور من لوحة الأدمن يستدعي InvalidateRoleCache() لمسح مفاتيح الكاش.
        ///  - في أسوأ حال، ينتهي الكاش بعد RoleCacheDuration تلقائياً.
        /// </summary>
        private static UserRole? ResolveRole()
        {
            var context = HttpContext.Current;

            if (context == null || context.User == null)
                return null;

            var identity = context.User.Identity;

            if (identity == null || !identity.IsAuthenticated)
                return null;

            string username = identity.Name;

            if (string.IsNullOrWhiteSpace(username))
            {
                InvalidateCurrentAuthentication(context);
                return null;
            }

            // --- محاولة قراءة الكاش من Session ---
            var session = context.Session;
            if (session != null)
            {
                string cachedUsername = session[SessionKeyCacheUsername] as string;
                object cachedRole    = session[SessionKeyRole];
                object cachedExpiry  = session[SessionKeyCacheExpiry];

                bool cacheHit =
                    string.Equals(cachedUsername, username, StringComparison.Ordinal)
                    && cachedRole is UserRole
                    && cachedExpiry is DateTime
                    && (DateTime)cachedExpiry > DateTime.UtcNow;

                if (cacheHit)
                    return (UserRole)cachedRole;
            }

            // --- الكاش منتهٍ أو غير موجود: استعلام DB ---
            return ResolveRoleFromDatabase(context, username);
        }

        /// <summary>
        /// يُحمّل بيانات الحساب من قاعدة البيانات، يتحقق من صحته،
        /// ثم يُحدّث الـ Session بالدور الجديد وتاريخ انتهاء الكاش.
        /// يُغلق في حالة الخطأ (Fail-Closed).
        /// </summary>
        private static UserRole? ResolveRoleFromDatabase(HttpContext context, string username)
        {
            try
            {
                using (var db = new NoorAlMalaikaSchool.Data.SchoolDbContext())
                {
                    var user = db.Users.AsNoTracking().FirstOrDefault(
                        u => u.Username == username && u.IsActive);

                    if (user == null)
                    {
                        InvalidateCurrentAuthentication(context);
                        return null;
                    }

                    if (!Enum.IsDefined(typeof(UserRole), user.Role))
                    {
                        InvalidateCurrentAuthentication(context);
                        return null;
                    }

                    // تحديث Session بالبيانات الجديدة وتجديد الكاش
                    var session = context.Session;
                    if (session != null)
                    {
                        session[SessionKeyRole]          = user.Role;
                        session[SessionKeyCacheUsername] = user.Username;
                        session[SessionKeyCacheExpiry]   = DateTime.UtcNow.Add(RoleCacheDuration);
                        session["Username"]              = user.Username;
                        session["DisplayName"]           = user.DisplayName;
                        session["EmployeeNo"]            = user.EmployeeNo;
                        session["MustChangePassword"]    = user.MustChangePassword;
                    }

                    return user.Role;
                }
            }
            catch (Exception ex)
            {
                if (context.Session != null)
                    context.Session.Remove(SessionKeyRole);

                System.Diagnostics.Trace.TraceError(
                    "SecurityHelper.ResolveRoleFromDatabase failed: " + ex);

                InvalidateCurrentAuthentication(context);
                return null;
            }
        }

        /// <summary>
        /// يُبطل كاش الدور في Session الحالية دون تسجيل الخروج.
        /// استخدمه بعد تعديل دور المستخدم من لوحة الأدمن، حتى تُحمَّل التغييرات
        /// في أول طلب تالٍ بدلاً من انتظار انتهاء RoleCacheDuration.
        /// </summary>
        public static void InvalidateRoleCache()
        {
            var session = HttpContext.Current?.Session;
            if (session == null) return;
            session.Remove(SessionKeyRole);
            session.Remove(SessionKeyCacheUsername);
            session.Remove(SessionKeyCacheExpiry);
        }

        /// <summary>
        /// Signs out the current user and clears the session.
        /// </summary>
        private static void InvalidateCurrentAuthentication(HttpContext context)
        {
            try
            {
                System.Web.Security.FormsAuthentication.SignOut();
            }
            catch
            {
            }

            try
            {
                if (context.Session != null)
                {
                    context.Session.Clear();
                    context.Session.Abandon();
                }
            }
            catch
            {
            }

            try
            {
                var cookieName =
                    System.Web.Security.FormsAuthentication.FormsCookieName;

                var expired = new HttpCookie(cookieName, string.Empty)
                {
                    Expires = DateTime.UtcNow.AddDays(-1),
                    HttpOnly = true,
                    Secure = context.Request != null &&
                             context.Request.IsSecureConnection,
                    Path = System.Web.Security.FormsAuthentication.FormsCookiePath
                };

                context.Response.Cookies.Add(expired);
            }
            catch
            {
            }

            // Mark this request anonymous as well.
            try
            {
                var anonymous = new System.Security.Principal.GenericPrincipal(
                    new System.Security.Principal.GenericIdentity(string.Empty),
                    new string[0]);

                context.User = anonymous;
                System.Threading.Thread.CurrentPrincipal = anonymous;
            }
            catch
            {
            }
        }

        public static string CurrentRoleName
        {
            get
            {
                var role = CurrentRole;
                return role == null ? "" : role.Value.GetDisplayName();
            }
        }

        public static bool IsAdmin
        {
            get { return CurrentRole == UserRole.Admin; }
        }

        /// <summary>
        /// Per-user ViewStateUserKey so the ViewState MAC is bound to the
        /// signed-in account instead of being shared by every user of the site.
        ///
        /// Binding the key to the username stops one-click/CSRF-style ViewState
        /// replay: a ViewState signed for one account cannot be replayed from
        /// another, because the user key differs and the MAC fails. The key is
        /// derived with HMAC over the pinned machineKey validation key and the
        /// user name, so it stays identical for the same user across postbacks
        /// and does not change when a session expires or the app pool recycles.
        ///
        /// Anonymous requests (for example the login form) get no user key; the
        /// move to an authenticated key happens on the redirect that follows a
        /// successful login, never in the middle of a postback.
        /// </summary>
        public static string ViewStateUserKey
        {
            get
            {
                var context = HttpContext.Current;

                if (context == null ||
                    context.User == null ||
                    context.User.Identity == null ||
                    !context.User.Identity.IsAuthenticated ||
                    string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    return null;
                }

                var section =
                    System.Web.Configuration.WebConfigurationManager
                        .GetSection("system.web/machineKey")
                    as System.Web.Configuration.MachineKeySection;

                string secret = section == null
                    ? null
                    : section.ValidationKey;

                if (string.IsNullOrEmpty(secret) ||
                    secret.IndexOf(
                        "AutoGenerate",
                        StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    secret = "NoorAlMalaikaSchool.ViewStateUserKey";
                }

                byte[] secretBytes =
                    System.Text.Encoding.UTF8.GetBytes(secret);

                byte[] nameBytes =
                    System.Text.Encoding.UTF8.GetBytes(
                        context.User.Identity.Name);

                using (var hmac =
                    new System.Security.Cryptography.HMACSHA256(
                        secretBytes))
                {
                    return BitConverter
                        .ToString(hmac.ComputeHash(nameBytes))
                        .Replace("-", string.Empty);
                }
            }
        }

        public static string CurrentEmployeeNo
        {
            get
            {
                if (HttpContext.Current == null ||
                    HttpContext.Current.Session == null)
                    return null;

                var role = CurrentRole;

                if (role == null)
                    return null;

                return HttpContext.Current.Session["EmployeeNo"] as string;
            }
        }

        public static bool CanAccess(params UserRole[] roles)
        {
            var current = CurrentRole;

            if (current == null)
                return false;

            if (current == UserRole.Admin)
                return true;

            return roles.Contains(current.Value);
        }

        /// <summary>
        /// Central authorization check for content pages.
        /// </summary>
        public static void RequireRolesForPath(string pagePath)
        {
            if (HttpContext.Current == null ||
                HttpContext.Current.Response == null)
                return;

            if (HttpContext.Current.User == null ||
                HttpContext.Current.User.Identity == null ||
                !HttpContext.Current.User.Identity.IsAuthenticated)
                return;

            var role = CurrentRole;

            if (role == null)
            {
                RedirectAndStop("~/Login.aspx");
                return;
            }

            // Admin can access all pages.
            if (role == UserRole.Admin)
                return;

            string lower = (pagePath ?? "")
                .Trim()
                .ToLowerInvariant()
                .Replace("\\", "/");

            lower = lower.Replace("~/", "").TrimStart('/');

            if (lower.StartsWith("pages/"))
            {
                lower = lower.Substring("pages/".Length).TrimStart('/');
            }

            // =====================================================
            // VIEW ROLE: STRICT READ-ONLY ALLOW-LIST
            // =====================================================
            if (role == UserRole.View)
            {
                bool viewPageAllowed =
                       string.IsNullOrEmpty(lower)
                    || lower.Equals(
                        "default.aspx",
                        StringComparison.OrdinalIgnoreCase)
                    || lower.Equals(
                        "changepassword.aspx",
                        StringComparison.OrdinalIgnoreCase)
                    || lower.Equals(
                        "accessdenied.aspx",
                        StringComparison.OrdinalIgnoreCase)
                    || lower.Equals(
                        "error.aspx",
                        StringComparison.OrdinalIgnoreCase)

                    // Student reports
                    || lower.Equals(
                        "reports/studentlist.aspx",
                        StringComparison.OrdinalIgnoreCase)
                    || lower.Equals(
                        "reports/attendancereport.aspx",
                        StringComparison.OrdinalIgnoreCase)

                    // Attendance and teacher reports
                    || lower.Equals(
                        "attendance/report.aspx",
                        StringComparison.OrdinalIgnoreCase)
                    || lower.Equals(
                        "teachers/teacherreport.aspx",
                        StringComparison.OrdinalIgnoreCase)

                    // Financial reports already allowed for View
                    || lower.Equals(
                        "reports/financialreport.aspx",
                        StringComparison.OrdinalIgnoreCase)
                    || lower.Equals(
                        "reports/studentfinancialtotals.aspx",
                        StringComparison.OrdinalIgnoreCase)
                    || lower.Equals(
                        "reports/studentfinancialstatement.aspx",
                        StringComparison.OrdinalIgnoreCase)
                    || lower.Equals(
                        "payments/studentreport.aspx",
                        StringComparison.OrdinalIgnoreCase)

                    // NEW: Read-only school timetable page
                    // Read-only payroll details for View role
                    || lower.Equals(
                        "payroll/detailsview.aspx",
                        StringComparison.OrdinalIgnoreCase)
                    || lower.Equals(
                        "timetable/schooltimetable.aspx",
                        StringComparison.OrdinalIgnoreCase);

                if (!viewPageAllowed)
                {
                    if (HttpContext.Current.Session != null)
                    {
                        HttpContext.Current.Session["DeniedPath"] = lower;
                    }

                    Deny();
                }

                return;
            }

            // =====================================================
            // COMMON PAGES FOR AUTHENTICATED NON-ADMIN USERS
            // =====================================================
            if (string.IsNullOrEmpty(lower)
                || lower.Equals(
                    "login.aspx",
                    StringComparison.OrdinalIgnoreCase)
                || lower.Equals(
                    "default.aspx",
                    StringComparison.OrdinalIgnoreCase)
                || lower.Equals(
                    "changepassword.aspx",
                    StringComparison.OrdinalIgnoreCase)
                || lower.Equals(
                    "accessdenied.aspx",
                    StringComparison.OrdinalIgnoreCase)
                || lower.Equals(
                    "error.aspx",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            bool allowed;

            // Teacher attendance readiness
            if (lower.Equals(
                "teacherattendance/readiness.aspx",
                StringComparison.OrdinalIgnoreCase))
            {
                allowed =
                       role == UserRole.Accountant
                    || role == UserRole.Supervisor
                    || role == UserRole.DataEntry;
            }
            // Teacher finances
            else if (lower.Equals(
                "teacherattendance/teachersfinances.aspx",
                StringComparison.OrdinalIgnoreCase))
            {
                allowed =
                       role == UserRole.Accountant
                    || role == UserRole.Supervisor
                    || role == UserRole.Teacher;
            }
            else if (lower.StartsWith("whatsapp/"))
            {
                // WhatsApp messaging contains family contact and student information.
                allowed = role == UserRole.Admin
                    || role == UserRole.Accountant
                    || role == UserRole.Supervisor
                    || role == UserRole.DataEntry;
            }
            else
            {
                bool isFinance =
                       lower.StartsWith("payroll/")
                    || lower.StartsWith("invoices/")
                    || lower.StartsWith("payments/")
                    || lower.StartsWith("fees/")
                    || lower.StartsWith("discountpolicies/")
                    || lower.Equals(
                        "students/financialbenefits.aspx",
                        StringComparison.OrdinalIgnoreCase)
                    || lower.Contains("financialreport")
                    || lower.Equals(
                        "reports/studentfinancialstatement.aspx",
                        StringComparison.OrdinalIgnoreCase)
                    || lower.Equals(
                        "payments/studentreport.aspx",
                        StringComparison.OrdinalIgnoreCase);

                if (role == UserRole.Teacher)
                {
                    // Teachers can access their own records and payslip.
                    allowed =
                           lower.Equals(
                               "teacherattendance/myrecords.aspx",
                               StringComparison.OrdinalIgnoreCase)
                        || lower.Equals(
                               "payroll/statement.aspx",
                               StringComparison.OrdinalIgnoreCase);
                }
                else if (lower.StartsWith("security/") ||
                         lower.StartsWith("database/"))
                {
                    // Security and database maintenance are Admin-only.
                    allowed = false;
                }
                else if (isFinance)
                {
                    allowed = role == UserRole.Accountant;
                }
                else if (lower.StartsWith("reports/"))
                {
                    // The general financial report is Accountant-only.
                    // Other report pages are shared between the accountant
                    // and the operational roles (supervisor / data entry).
                    if (lower.Equals(
                        "reports/financialreport.aspx",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        allowed = role == UserRole.Accountant;
                    }
                    else
                    {
                        allowed =
                               role == UserRole.Accountant
                            || role == UserRole.Supervisor
                            || role == UserRole.DataEntry;
                    }
                }
                else
                {
                    // Default deny: a page that is not classified above is
                    // reachable only through the explicit operational list.
                    // New pages must therefore be classified deliberately
                    // instead of falling through open to Supervisor/DataEntry.
                    bool operational =
                           lower.StartsWith("students/")
                        || lower.StartsWith("classes/")
                        || lower.StartsWith("subjects/")
                        || lower.StartsWith("attendance/")
                        || lower.StartsWith("teachers/")
                        || lower.StartsWith("timetable/")
                        || lower.StartsWith("monthlysheets/")
                        || lower.StartsWith("teacherattendance/");

                    allowed =
                        operational &&
                        (role == UserRole.Supervisor ||
                         role == UserRole.DataEntry);
                }
            }

            if (!allowed)
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session["DeniedPath"] = lower;
                }

                Deny();
            }
        }

        /// <summary>
        /// Redirects and stops the current request.
        /// </summary>
        public static void RedirectAndStop(string url)
        {
            if (HttpContext.Current == null ||
                HttpContext.Current.Response == null)
                return;

            HttpContext.Current.Response.Redirect(url, true);
        }

        /// <summary>
        /// Redirects after login without aborting the current thread.
        /// </summary>
        public static void RedirectAndSaveSession(string url)
        {
            if (HttpContext.Current == null ||
                HttpContext.Current.Response == null)
                return;

            HttpContext.Current.Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Sends the current user to AccessDenied.aspx or Login.aspx.
        /// </summary>
        public static void Deny()
        {
            if (HttpContext.Current == null)
                return;

            bool authenticated =
                HttpContext.Current.User != null
                && HttpContext.Current.User.Identity != null
                && HttpContext.Current.User.Identity.IsAuthenticated;

            RedirectAndStop(
                authenticated
                    ? "~/AccessDenied.aspx"
                    : "~/Login.aspx");
        }

        /// <summary>
        /// Local authorization guard for selected roles.
        /// </summary>
        public static void DenyUnless(params UserRole[] roles)
        {
            if (CanAccess(roles))
                return;

            Deny();
        }

        /// <summary>
        /// Re-applies the central path authorization rules.
        /// </summary>
        public static void DenyUnlessPath()
        {
            if (HttpContext.Current == null)
                return;

            RequireRolesForPath(
                HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath);
        }

        /// <summary>
        /// Blocks server-side export/print handlers for View users.
        /// </summary>
        public static void DenyViewExport()
        {
            if (CurrentRole == UserRole.View)
                Deny();
        }

        /// <summary>
        /// Hides export and print buttons for View users.
        /// </summary>
        public static void HideViewExportControls(
            System.Web.UI.Control root)
        {
            if (root == null || CurrentRole != UserRole.View)
                return;

            foreach (System.Web.UI.Control child in root.Controls)
            {
                var button =
                    child as System.Web.UI.WebControls.Button;

                var linkButton =
                    child as System.Web.UI.WebControls.LinkButton;

                string controlText = button != null
                    ? button.Text
                    : (linkButton != null
                        ? linkButton.Text
                        : string.Empty);

                string id = child.ID ?? string.Empty;

                bool exportOrPrint =
                       id.IndexOf(
                           "export",
                           StringComparison.OrdinalIgnoreCase) >= 0
                    || id.IndexOf(
                           "print",
                           StringComparison.OrdinalIgnoreCase) >= 0
                    || controlText.IndexOf(
                           "تصدير",
                           StringComparison.OrdinalIgnoreCase) >= 0
                    || controlText.IndexOf(
                           "طباعة",
                           StringComparison.OrdinalIgnoreCase) >= 0
                    || controlText.IndexOf(
                           "export",
                           StringComparison.OrdinalIgnoreCase) >= 0
                    || controlText.IndexOf(
                           "print",
                           StringComparison.OrdinalIgnoreCase) >= 0;

                if (exportOrPrint &&
                    (button != null || linkButton != null))
                {
                    child.Visible = false;
                }

                if (child.HasControls())
                {
                    HideViewExportControls(child);
                }
            }
        }
    }
}

