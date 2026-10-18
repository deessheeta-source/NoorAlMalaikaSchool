using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Security
{
    public partial class Users : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnless(UserRole.Admin);
            if (lblMessage != null) lblMessage.EnableViewState = false;

            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        // =========================================================
        // تحميل المستخدمين
        // =========================================================
        private void LoadUsers()
        {
            try
            {
                var userRows = db.Users
                    .AsNoTracking()
                    .Where(u => u.Username != "admin")
                    .OrderBy(u => u.Username)
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        u.DisplayName,
                        u.IsActive,
                        u.Role,
                        u.EmployeeNo
                    })
                    .ToList();

                var employeeNos = userRows
                    .Where(u => !string.IsNullOrEmpty(u.EmployeeNo))
                    .Select(u => u.EmployeeNo)
                    .Distinct()
                    .ToList();

                var teacherNames = db.Teachers
                    .AsNoTracking()
                    .Where(t => employeeNos.Contains(t.EmployeeNo))
                    .Select(t => new { t.EmployeeNo, t.Name })
                    .ToList()
                    .ToDictionary(t => t.EmployeeNo, t => t.Name, StringComparer.OrdinalIgnoreCase);

                var users = userRows.Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.DisplayName,
                    u.IsActive,
                    u.Role,
                    RoleName = u.Role.GetDisplayName(),
                    EmployeeNo = u.EmployeeNo,
                    TeacherLink = !string.IsNullOrEmpty(u.EmployeeNo)
                        ? (teacherNames.ContainsKey(u.EmployeeNo) ? teacherNames[u.EmployeeNo] : u.EmployeeNo) + " (" + u.EmployeeNo + ")"
                        : ""
                }).ToList();

                gvUsers.DataSource = users;
                gvUsers.DataBind();

                if (lblCount != null)
                    lblCount.Text = users.Count.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("LoadUsers error: " + ex);
                ShowMessage("تعذر تحميل قائمة المستخدمين بسبب خطأ داخلي. راجع سجل النظام.", false);
            }
        }

        // =========================================================
        // RowDataBound — تعيين الدور الحالي في القائمة المنسدلة
        // =========================================================
        protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.CssClass = (e.Row.CssClass + " us-table-header-row").Trim();
                return;
            }

            if (e.Row.RowType != DataControlRowType.DataRow) return;

            // تُستخدم هذه العناوين لعرض خلايا GridView كبطاقة مقروءة على الهاتف.
            string[] mobileLabels = { "المستخدم", "الدور", "المعلم المرتبط", "الحالة", "تغيير الدور", "إجراءات" };
            for (int i = 0; i < e.Row.Cells.Count && i < mobileLabels.Length; i++)
                e.Row.Cells[i].Attributes["data-label"] = mobileLabels[i];

            try
            {
                var ddl = e.Row.FindControl("ddlRoleGrid") as DropDownList;
                if (ddl == null) return;

                var roleValue = DataBinder.Eval(e.Row.DataItem, "Role");
                if (roleValue != null)
                {
                    string roleStr = roleValue.ToString();
                    ListItem item = ddl.Items.FindByValue(roleStr);
                    if (item != null)
                    {
                        ddl.ClearSelection();
                        item.Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("gvUsers_RowDataBound error: " + ex);
            }
        }

        // =========================================================
        // RowCommand — تنفيذ الأوامر
        // =========================================================
        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                SecurityHelper.DenyUnless(UserRole.Admin);
                int id;
                if (!int.TryParse(e.CommandArgument as string, out id)) return;

                var user = db.Users.Find(id);
                if (user == null)
                {
                    ShowMessage("المستخدم غير موجود.", false);
                    LoadUsers();
                    return;
                }

                string currentUsername = Context.User != null && Context.User.Identity != null
                    ? Context.User.Identity.Name : null;

                switch (e.CommandName)
                {
                    case "SetRole":
                        HandleSetRole(e, user, currentUsername);
                        break;

                    case "ToggleActive":
                        HandleToggleActive(user, currentUsername);
                        break;

                    case "ResetPass":
                        HandleResetPassword(user);
                        break;

                    case "EditUser":
                        // ✅ فتح لوحة التعديل — لا نستدعي LoadUsers هنا
                        PopulateEditPanel(user);
                        return;

                    case "DeleteUser":
                        // ✅ حذف المستخدم
                        HandleDeleteUser(user, currentUsername);
                        LoadUsers();
                        return;

                    default:
                        LoadUsers();
                        break;
                }

                LoadUsers();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("gvUsers_RowCommand error: " + ex);
                ShowMessage("تعذر تنفيذ العملية بسبب خطأ داخلي. راجع سجل النظام.", false);
                LoadUsers();
            }
        }

        // =========================================================
        // SetRole
        // =========================================================
        private void HandleSetRole(GridViewCommandEventArgs e, User user, string currentUsername)
        {
            if (IsProtectedAdmin(user))
            {
                ShowMessage("لا يمكن تغيير حساب admin الرئيسي من هذه الشاشة.", false);
                return;
            }

            int rowIndex = GetRowIndexByCommandArgument(e.CommandArgument as string);
            if (rowIndex < 0 || rowIndex >= gvUsers.Rows.Count) return;

            var row = gvUsers.Rows[rowIndex];
            var ddl = row.FindControl("ddlRoleGrid") as DropDownList;
            if (ddl == null) return;

            UserRole role;
            if (!Enum.TryParse<UserRole>(ddl.SelectedValue, true, out role) ||
                !Enum.IsDefined(typeof(UserRole), role))
            {
                ShowMessage("دور المستخدم غير صحيح.", false);
                return;
            }

            if (string.Equals(user.Username, currentUsername, StringComparison.OrdinalIgnoreCase) && role != UserRole.Admin)
            {
                ShowMessage("لا يمكن تغيير دور حسابك الحالي من مدير النظام أثناء تسجيل الدخول.", false);
                return;
            }

            if (user.Role == UserRole.Admin && role != UserRole.Admin &&
                db.Users.Count(u => u.IsActive && u.Role == UserRole.Admin) <= 1)
            {
                ShowMessage("لا يمكن إزالة صلاحية آخر مدير نظام نشط.", false);
                return;
            }

            if (role == UserRole.Teacher)
            {
                ShowMessage("ربط حساب المعلم يتم من شاشة التعديل باختيار معلم محدد.", false);
                return;
            }

            user.Role = role;
            user.EmployeeNo = null;
            db.SaveChanges();
            // إبطال كاش الدور في Session حتى تُطبَّق التغييرات فوراً
            SecurityHelper.InvalidateRoleCache();
            ShowMessage("تم تحديث دور المستخدم \"" + user.Username + "\"", true);
        }

        // =========================================================
        // ToggleActive
        // =========================================================
        private void HandleToggleActive(User user, string currentUsername)
        {
            if (IsProtectedAdmin(user))
            {
                ShowMessage("لا يمكن تعطيل أو تفعيل حساب admin الرئيسي من هذه الشاشة.", false);
                return;
            }

            if (string.Equals(user.Username, currentUsername, StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage("لا يمكن تعطيل أو تفعيل حسابك الحالي من هذه الشاشة.", false);
                return;
            }

            if (user.IsActive && user.Role == UserRole.Admin &&
                db.Users.Count(u => u.IsActive && u.Role == UserRole.Admin) <= 1)
            {
                ShowMessage("لا يمكن تعطيل آخر مدير نظام نشط.", false);
                return;
            }

            user.IsActive = !user.IsActive;
            db.SaveChanges();
            // إبطال كاش الدور حتى يُعاد التحقق من الحساب في أول طلب تالٍ
            SecurityHelper.InvalidateRoleCache();
            ShowMessage(user.IsActive ? "تم تفعيل المستخدم" : "تم تعطيل المستخدم", true);
        }

        // =========================================================
        // ResetPass
        // =========================================================
        private void HandleResetPassword(User user)
        {
            if (IsProtectedAdmin(user))
            {
                ShowMessage("لا يمكن إعادة تعيين كلمة مرور حساب admin الرئيسي من هذه الشاشة.", false);
                return;
            }

            string temporaryPassword = GenerateTemporaryPassword();
            user.PasswordHash = PasswordHasher.Hash(temporaryPassword);
            user.MustChangePassword = true;
            user.FailedLoginCount = 0;
            user.LockoutUntil = null;
            user.LastFailedLoginAt = null;
            db.SaveChanges();
            // The generated password is shown once in this response only; never log it.
            ShowMessage("تم إنشاء كلمة مرور مؤقتة. سلّمها للمستخدم عبر قناة آمنة؛ لن تُعرض مرة أخرى.", true);
            lblMessage.Text = HttpUtility.HtmlEncode("كلمة المرور المؤقتة (تُعرض مرة واحدة): " + temporaryPassword);
            lblMessage.CssClass = "us-message success";
        }

        // =========================================================
        // ✅ حذف المستخدم
        // =========================================================
        private void HandleDeleteUser(User user, string currentUsername)
        {
            // 1) لا يمكن حذف المستخدم الحالي
            if (string.Equals(user.Username, currentUsername, StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage("لا يمكن حذف حسابك الحالي أثناء تسجيل الدخول.", false);
                return;
            }

            // 2) لا يمكن حذف حساب admin الرئيسي
            if (IsProtectedAdmin(user))
            {
                ShowMessage("لا يمكن حذف حساب admin الرئيسي.", false);
                return;
            }

            // 3) لا يمكن حذف آخر مدير نظام نشط
            if (user.Role == UserRole.Admin &&
                user.IsActive &&
                db.Users.Count(u => u.IsActive && u.Role == UserRole.Admin) <= 1)
            {
                ShowMessage("لا يمكن حذف آخر مدير نظام نشط. يجب إضافة مدير آخر أولاً.", false);
                return;
            }

            // 4) التحقق من الارتباطات (AuditLogs)
            // إذا كان هناك سجل تدقيق يشير إلى هذا المستخدم، نُبقيه ونُعطّل الحساب فقط
            bool hasAuditLogs;
            try
            {
                string targetId = user.Id.ToString();
                hasAuditLogs = db.AuditLogs.Any(a => a.UserName == user.Username ||
                    (a.EntityType == "User" && a.EntityId == targetId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("HandleDeleteUser audit lookup failed: " + ex);
                ShowMessage("تعذر التحقق من سجلات المستخدم؛ أُلغيت عملية الحذف حفاظًا على سلامة البيانات.", false);
                return;
            }

            if (hasAuditLogs)
            {
                user.IsActive = false;
                db.SaveChanges();

                ShowMessage(
                    "لا يمكن حذف المستخدم لوجود سجلات مرتبطة به في سجل النظام (Audit Logs). " +
                    "تم تعطيل الحساب بدلاً من ذلك للحفاظ على سلامة السجل.",
                    false);
                return;
            }

            // 5) الحذف الفعلي
            try
            {
                string username = user.Username;
                db.Users.Remove(user);
                db.SaveChanges();

                ShowMessage("تم حذف المستخدم \"" + username + "\" بنجاح.", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("HandleDeleteUser error: " + ex);
                ShowMessage(
                    "تعذر حذف المستخدم لوجود بيانات مرتبطة به. " +
                    "يرجى تعطيل الحساب بدلاً من حذفه.",
                    false);
            }
        }

        // =========================================================
        // ملء لوحة التعديل — نسخة آمنة (بدون استثناءات)
        // =========================================================
        private void PopulateEditPanel(User user)
        {
            try
            {
                hfEditId.Value = user.Id.ToString();
                txtEditName.Text = user.DisplayName ?? "";

                // تعيين الدور بأمان
                string roleValue = user.Role.ToString();
                ListItem roleItem = ddlEditRole.Items.FindByValue(roleValue);
                if (roleItem != null)
                {
                    ddlEditRole.ClearSelection();
                    roleItem.Selected = true;
                }
                else
                {
                    ddlEditRole.SelectedIndex = 0;
                }

                chkEditActive.Checked = user.IsActive;

                // تحميل قائمة المعلمين
                LoadEditTeachers();

                // تعيين المعلم بأمان
                if (!string.IsNullOrEmpty(user.EmployeeNo))
                {
                    ListItem teacherItem = ddlEditTeacher.Items.FindByValue(user.EmployeeNo);
                    if (teacherItem != null)
                    {
                        ddlEditTeacher.ClearSelection();
                        teacherItem.Selected = true;
                    }
                    else
                    {
                        // المعلم المرتبط لم يعد موجوداً — أضفه كعنصر مؤقت
                        ddlEditTeacher.Items.Add(new ListItem(
                            "(محذوف) " + user.EmployeeNo,
                            user.EmployeeNo));
                        ddlEditTeacher.SelectedValue = user.EmployeeNo;
                    }
                }
                else
                {
                    if (ddlEditTeacher.Items.Count > 0)
                        ddlEditTeacher.SelectedIndex = 0;
                }

                pnlEdit.Visible = true;
                lblEditMessage.Text = "";
                lblEditMessage.CssClass = "us-message";

                // تحديث ظهور صف المعلم
                string script = "toggleEditTeacherRow();";
                ScriptManager.RegisterStartupScript(this, GetType(), "toggleTeacherRow", script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("PopulateEditPanel error: " + ex);
                ShowMessage("تعذر تحميل بيانات المستخدم للتعديل بسبب خطأ داخلي.", false);
                pnlEdit.Visible = false;
            }
        }

        // =========================================================
        // تحميل قائمة المعلمين
        // =========================================================
        private void LoadEditTeachers()
        {
            ddlEditTeacher.Items.Clear();
            ddlEditTeacher.Items.Add(new ListItem("— اختر المعلم —", ""));

            var teachers = db.Teachers
                .AsNoTracking()
                .OrderBy(t => t.Name)
                .Select(t => new { t.Name, t.EmployeeNo })
                .ToList();

            foreach (var t in teachers)
            {
                string label = t.Name + (string.IsNullOrEmpty(t.EmployeeNo) ? "" : " (" + t.EmployeeNo + ")");
                ddlEditTeacher.Items.Add(new ListItem(label, t.EmployeeNo ?? ""));
            }
        }

        // =========================================================
        // حفظ التعديلات
        // =========================================================
        protected void btnEditSave_Click(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnless(UserRole.Admin);
            try
            {
                int id;
                if (!int.TryParse(hfEditId.Value, out id))
                {
                    lblEditMessage.CssClass = "us-message danger";
                    lblEditMessage.Text = "رقم المستخدم غير صالح.";
                    return;
                }

                var user = db.Users.Find(id);
                if (user == null)
                {
                    lblEditMessage.CssClass = "us-message danger";
                    lblEditMessage.Text = "المستخدم غير موجود.";
                    return;
                }

                if (IsProtectedAdmin(user))
                {
                    lblEditMessage.CssClass = "us-message danger";
                    lblEditMessage.Text = "لا يمكن تعديل حساب admin الرئيسي من هذه الشاشة.";
                    return;
                }

                UserRole role;
                if (!Enum.TryParse<UserRole>(ddlEditRole.SelectedValue, true, out role) ||
                    !Enum.IsDefined(typeof(UserRole), role))
                {
                    lblEditMessage.CssClass = "us-message danger";
                    lblEditMessage.Text = "الدور المحدد غير صالح.";
                    return;
                }

                string displayName = (txtEditName.Text ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(displayName) || displayName.Length > 100)
                {
                    lblEditMessage.CssClass = "us-message danger";
                    lblEditMessage.Text = "اسم العرض مطلوب ويجب ألا يتجاوز 100 حرف.";
                    return;
                }

                string employeeNo = null;

                if (role == UserRole.Teacher)
                {
                    employeeNo = ddlEditTeacher.SelectedValue;

                    if (string.IsNullOrEmpty(employeeNo) ||
                        !db.Teachers.Any(t => t.EmployeeNo == employeeNo))
                    {
                        lblEditMessage.CssClass = "us-message danger";
                        lblEditMessage.Text = "يرجى اختيار معلم صحيح مرتبط بالحساب";
                        return;
                    }

                    if (db.Users.Any(u => u.EmployeeNo == employeeNo && u.Id != user.Id))
                    {
                        lblEditMessage.CssClass = "us-message danger";
                        lblEditMessage.Text = "لهذا المعلم حساب مسبقاً";
                        return;
                    }
                }

                if (string.Equals(user.Username, Session["Username"] as string, StringComparison.OrdinalIgnoreCase))
                {
                    if (role != UserRole.Admin)
                    {
                        lblEditMessage.CssClass = "us-message danger";
                        lblEditMessage.Text = "لا يمكن إزالة دور مدير النظام من حسابك الحالي.";
                        return;
                    }

                    if (!chkEditActive.Checked)
                    {
                        lblEditMessage.CssClass = "us-message danger";
                        lblEditMessage.Text = "لا يمكن تعطيل حسابك الحالي.";
                        return;
                    }
                }

                if (user.Role == UserRole.Admin && role != UserRole.Admin &&
                    db.Users.Count(u => u.IsActive && u.Role == UserRole.Admin) <= 1)
                {
                    lblEditMessage.CssClass = "us-message danger";
                    lblEditMessage.Text = "لا يمكن إزالة صلاحية آخر مدير نظام نشط.";
                    return;
                }

                if (user.IsActive && !chkEditActive.Checked && user.Role == UserRole.Admin &&
                    db.Users.Count(u => u.IsActive && u.Role == UserRole.Admin) <= 1)
                {
                    lblEditMessage.CssClass = "us-message danger";
                    lblEditMessage.Text = "لا يمكن تعطيل آخر مدير نظام نشط.";
                    return;
                }

                user.DisplayName = displayName;
                user.Role = role;
                user.EmployeeNo = employeeNo;
                user.IsActive = chkEditActive.Checked;
                db.SaveChanges();
                // إبطال كاش الدور حتى تُطبَّق أي تغييرات في الدور أو الحالة فوراً
                SecurityHelper.InvalidateRoleCache();

                ShowMessage("تم حفظ تعديلات المستخدم \"" + user.Username + "\"", true);
                pnlEdit.Visible = false;
                LoadUsers();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("btnEditSave_Click error: " + ex);
                lblEditMessage.CssClass = "us-message danger";
                lblEditMessage.Text = "تعذر حفظ التعديلات بسبب خطأ داخلي. راجع سجل النظام.";
            }
        }

        protected void btnEditCancel_Click(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnless(UserRole.Admin);
            pnlEdit.Visible = false;
        }

        // =========================================================
        // إيجاد رقم الصف من CommandArgument
        // =========================================================
        private int GetRowIndexByCommandArgument(string arg)
        {
            for (int i = 0; i < gvUsers.Rows.Count; i++)
            {
                if (gvUsers.DataKeys[i].Value.ToString() == arg)
                    return i;
            }
            return -1;
        }

        // =========================================================
        // دوال العرض (Badges)
        // =========================================================
        protected string FormatRoleBadge(object roleName, object roleValue)
        {
            string name = Convert.ToString(roleName).Trim();
            string role = Convert.ToString(roleValue).Trim();

            string cssClass = "us-role-badge";

            switch (role)
            {
                case "Admin": cssClass += " admin"; break;
                case "Accountant": cssClass += " accountant"; break;
                case "Supervisor": cssClass += " supervisor"; break;
                case "DataEntry": cssClass += " dataentry"; break;
                case "Teacher": cssClass += " teacher"; break;
            }

            return "<span class='" + cssClass + "'>" +
                   "<i class='fas fa-user-tag' style='font-size:10px;'></i> " +
                   HttpUtility.HtmlEncode(name) +
                   "</span>";
        }

        protected string FormatStatusBadge(object isActive)
        {
            bool active = false;
            try { active = Convert.ToBoolean(isActive); } catch { }

            if (active)
                return "<span class='us-status-badge active'><i class='fas fa-check-circle' style='font-size:10px;'></i> نشط</span>";

            return "<span class='us-status-badge inactive'><i class='fas fa-pause-circle' style='font-size:10px;'></i> معطّل</span>";
        }

        protected string FormatTeacherLink(object teacherLink)
        {
            string value = teacherLink == null ? "" : Convert.ToString(teacherLink).Trim();

            if (string.IsNullOrWhiteSpace(value))
                return "<span class='text-muted'>—</span>";

            return "<span class='us-teacher-link'>" + HttpUtility.HtmlEncode(value) + "</span>";
        }

        private static bool IsProtectedAdmin(User user)
        {
            return user != null && string.Equals(user.Username, "admin", StringComparison.OrdinalIgnoreCase);
        }

        // =========================================================
        // توليد كلمة مرور مؤقتة
        // =========================================================
        private static string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
            var bytes = new byte[12];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create()) rng.GetBytes(bytes);
            var charsOut = new char[12];
            for (int i = 0; i < charsOut.Length; i++) charsOut[i] = chars[bytes[i] % chars.Length];
            return new string(charsOut);
        }

        // =========================================================
        // رسالة عامة
        // =========================================================
        private void ShowMessage(string text, bool success)
        {
            lblMessage.CssClass = success ? "us-message success" : "us-message danger";
            lblMessage.Text = HttpUtility.HtmlEncode(text ?? string.Empty);
        }

        // =========================================================
        // التخلص من DbContext
        // =========================================================
        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}
