
using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool
{
    public partial class SiteMaster : MasterPage
    {
        public EducationStage CurrentStage { get; private set; }

        public string StageName
        {
            get
            {
                return CurrentStage == EducationStage.Basic
                    ? "المرحلة الأساسية"
                    : "المرحلة الثانوية";
            }
        }

        // =========================================================
        // OnInit
        // =========================================================
        protected override void OnInit(EventArgs e)
        {
            Page.ViewStateUserKey = SecurityHelper.ViewStateUserKey;

            base.OnInit(e);

            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return;

            // التحقق من صلاحية الحساب والدور الحالي من قاعدة البيانات.
            var currentRole = SecurityHelper.CurrentRole;

            if (currentRole == null)
            {
                SecurityHelper.RedirectAndStop("~/Login.aspx");
                return;
            }

            // إجبار المستخدم على تغيير كلمة المرور عند الحاجة.
            bool mustChange =
                Session["MustChangePassword"] is bool
                && (bool)Session["MustChangePassword"];

            string currentPage =
                VirtualPathUtility.GetFileName(
                    Request.AppRelativeCurrentExecutionFilePath);

            if (mustChange &&
                !string.Equals(
                    currentPage,
                    "ChangePassword.aspx",
                    StringComparison.OrdinalIgnoreCase))
            {
                SecurityHelper.RedirectAndStop("~/ChangePassword.aspx");
                return;
            }

            // التحقق من صلاحية الوصول إلى الصفحة الحالية.
            SecurityHelper.RequireRolesForPath(
                Request.AppRelativeCurrentExecutionFilePath);
        }

        // =========================================================
        // OnPreRender
        // =========================================================
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ApplyNavVisibility();
        }

        // =========================================================
        // Page_Load
        // =========================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStageFromQueryString();
            }
            else
            {
                CurrentStage = GetCurrentStage();
            }

            SetStageDropdown();
            SetupHeader();
        }

        // =========================================================
        // إظهار وإخفاء أقسام القائمة حسب دور المستخدم
        // =========================================================
        private void ApplyNavVisibility()
        {
            var role = SecurityHelper.CurrentRole;

            bool isAdmin = role == UserRole.Admin;
            bool isFinance = role == UserRole.Accountant;

            bool isAcademic =
                role == UserRole.Supervisor ||
                role == UserRole.DataEntry;

            bool isTeacher = role == UserRole.Teacher;
            bool isView = role == UserRole.View;
            bool isSupervisor = role == UserRole.Supervisor;
            bool canUseWhatsApp = isAdmin || isFinance || isAcademic;

            // =====================================================
            // 1) لوحة التحكم
            // =====================================================
            if (navMain != null)
                navMain.Visible = true;

            if (navWhatsApp != null)
                navWhatsApp.Visible = canUseWhatsApp;

            // =====================================================
            // 2) الشؤون الإدارية للطلاب
            // =====================================================
            if (navStudents != null)
            {
                navStudents.Visible =
                    isAdmin || isAcademic || isView;
            }

            // إدارة بيانات الطلاب
            if (liStudentsManagement != null)
            {
                liStudentsManagement.Visible =
                    isAdmin || isAcademic;
            }

            // إدارة الحضور والغياب
            if (liAttendanceManagement != null)
            {
                liAttendanceManagement.Visible =
                    isAdmin || isAcademic;
            }

            // تقرير بيانات الطلاب
            if (liStudentListReport != null)
            {
                liStudentListReport.Visible =
                    isAdmin || isAcademic || isView;
            }

            // تقرير الحضور والغياب
            if (liAttendanceReport != null)
            {
                liAttendanceReport.Visible =
                    isAdmin || isAcademic || isView;
            }

            // =====================================================
            // 3) الشؤون المالية للطلاب
            // =====================================================
            if (navFinance != null)
            {
                navFinance.Visible =
                    isAdmin || isFinance;
            }

            // =====================================================
            // 4) الشؤون الأكاديمية
            // =====================================================
            if (navAcademic != null)
            {
                navAcademic.Visible =
                    isAdmin || isAcademic;
            }

            // =====================================================
            // 5) شؤون المعلمين والمعلمات
            // =====================================================
            if (navTeachers != null)
            {
                navTeachers.Visible =
                    isAdmin ||
                    isTeacher ||
                    isFinance ||
                    isAcademic ||
                    isView;

                navTeachers.Attributes["class"] =
                    "nav-item has-flyout " +
                    TeachersNavActiveClass;
            }

            // إعداد روابط شؤون المعلمين حسب الدور.
            SetupTeacherNavLinks(
                isAdmin,
                isTeacher,
                isFinance,
                isAcademic,
                isSupervisor,
                isView);

            // تقرير المعلمين والمعلمات
            if (liTeacherReport != null)
            {
                liTeacherReport.Visible =
                    isAdmin || isAcademic || isView;
            }

            // =====================================================
            // 6) التقارير العامة
            // =====================================================
            if (navReports != null)
            {
                // التقرير المالي والتحصيل
                bool hasFinancialReport =
                    isAdmin || isFinance || isView;

                // الكشف المالي للطلاب
                bool hasStatement =
                    isAdmin || isFinance || isAcademic || isView;

                // التقرير المالي الإجمالي للطلاب
                bool hasFinancialTotals =
                    isAdmin || isFinance || isView;

                navReports.Visible =
                    hasFinancialReport ||
                    hasStatement ||
                    hasFinancialTotals;

                // -------------------------------------------------
                // التقرير المالي والتحصيل
                // -------------------------------------------------
                if (lnkFinancialReport != null)
                {
                    lnkFinancialReport.HRef =
                        StageUrlHelper.GetStageUrl(
                            "Reports/FinancialReport.aspx");

                    lnkFinancialReport.Attributes["class"] =
                        GetActiveClass("FinancialReport");

                    lnkFinancialReport.Visible =
                        hasFinancialReport;
                }

                // -------------------------------------------------
                // التقرير المالي الإجمالي للطلاب
                // -------------------------------------------------
                if (liStudentFinancialTotals != null)
                {
                    liStudentFinancialTotals.Visible =
                        hasFinancialTotals;
                }

                SetFlyoutLink(
                    lnkStudentFinancialTotals,
                    "Reports/StudentFinancialTotals.aspx",
                    "StudentFinancialTotals",
                    "fa-chart-pie",
                    "التقرير المالي الإجمالي للطلاب");

                // -------------------------------------------------
                // الكشف المالي للطلاب
                // -------------------------------------------------
                if (liStudentFinancialStatement != null)
                {
                    liStudentFinancialStatement.Visible =
                        hasStatement;
                }
            }

            // =====================================================
            // 7) إدارة النظام
            // المدير فقط
            // =====================================================
            if (navSystem != null)
            {
                navSystem.Visible = isAdmin;
            }
        }

        // =========================================================
        // إعداد بيانات المستخدم في رأس الصفحة
        // =========================================================
        private void SetupHeader()
        {
            string displayName =
                Session["DisplayName"] as string;

            if (string.IsNullOrEmpty(displayName))
            {
                string username =
                    Session["Username"] as string;

                displayName =
                    string.IsNullOrEmpty(username)
                        ? HttpContext.Current.User.Identity.Name
                        : username;
            }

            litDisplayName.Text =
                HttpUtility.HtmlEncode(displayName);

            if (!string.IsNullOrEmpty(displayName))
            {
                litUserInitial.Text =
                    HttpUtility.HtmlEncode(
                        displayName[0].ToString());
            }
        }

        // =========================================================
        // تسجيل الخروج
        // =========================================================
        protected void btnLogout_Click(
            object sender,
            EventArgs e)
        {
            Session.Abandon();

            FormsAuthentication.SignOut();

            Response.Redirect("~/Login.aspx");
        }

        // =========================================================
        // تحديد المرحلة الدراسية من QueryString أو Session
        // =========================================================
        private void LoadStageFromQueryString()
        {
            string stageParam =
                Request.QueryString["stage"];

            EducationStage stage;

            if (!string.IsNullOrEmpty(stageParam) &&
                Enum.TryParse<EducationStage>(
                    stageParam,
                    true,
                    out stage))
            {
                CurrentStage = stage;
            }
            else
            {
                string sessionStage =
                    Session["Stage"] as string;

                EducationStage savedStage;

                if (!string.IsNullOrEmpty(sessionStage) &&
                    Enum.TryParse<EducationStage>(
                        sessionStage,
                        true,
                        out savedStage))
                {
                    CurrentStage = savedStage;
                }
                else
                {
                    CurrentStage = EducationStage.Basic;
                }
            }

            Session["Stage"] = CurrentStage.ToString();
        }

        private EducationStage GetCurrentStage()
        {
            string selected = ddlStage.SelectedValue;

            EducationStage stage;

            if (Enum.TryParse<EducationStage>(
                selected,
                true,
                out stage))
            {
                return stage;
            }

            return EducationStage.Basic;
        }

        private void SetStageDropdown()
        {
            if (ddlStage == null)
                return;

            string value = CurrentStage.ToString();

            if (ddlStage.Items.FindByValue(value) != null)
            {
                ddlStage.SelectedValue = value;
            }
        }

        // =========================================================
        // تغيير المرحلة الدراسية
        // =========================================================
        protected void ddlStage_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            CurrentStage = GetCurrentStage();

            Session["Stage"] = CurrentStage.ToString();

            string appPath =
                HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/');

            string absPath =
                Request.Url.AbsolutePath.TrimStart('/');

            string relativePath = absPath;

            if (!string.IsNullOrEmpty(appPath))
            {
                string appPathWithoutSlash =
                    appPath.TrimStart('/');

                if (relativePath.StartsWith(
                    appPathWithoutSlash + "/",
                    StringComparison.OrdinalIgnoreCase))
                {
                    relativePath =
                        relativePath.Substring(
                            appPathWithoutSlash.Length)
                        .TrimStart('/');
                }
            }

            string currentUrl = relativePath;

            if (!string.IsNullOrEmpty(Request.Url.Query))
            {
                currentUrl += Request.Url.Query;
            }

            if (currentUrl.Contains("stage="))
            {
                currentUrl =
                    System.Text.RegularExpressions.Regex.Replace(
                        currentUrl,
                        @"stage=[^&]*",
                        "stage=" + CurrentStage);
            }
            else if (currentUrl.Contains("?"))
            {
                currentUrl += "&stage=" + CurrentStage;
            }
            else
            {
                currentUrl += "?stage=" + CurrentStage;
            }

            Response.Redirect("~/" + currentUrl);
        }

        // =========================================================
        // بناء رابط الصفحة مع المرحلة الدراسية
        // =========================================================
        public string GetStageUrl(string page)
        {
            string cleanPage =
                page.TrimStart('~', '/');

            string appPath =
                HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/');

            string fullUrl;

            if (cleanPage.Equals(
                "Default.aspx",
                StringComparison.OrdinalIgnoreCase))
            {
                fullUrl = appPath + "/Default.aspx";
            }
            else
            {
                fullUrl =
                    appPath + "/Pages/" + cleanPage;
            }

            if (fullUrl.Contains("?"))
            {
                return fullUrl + "&stage=" + CurrentStage;
            }

            return fullUrl + "?stage=" + CurrentStage;
        }

        // =========================================================
        // تحديد الرابط النشط في القائمة الجانبية
        // =========================================================
        public string GetActiveClass(string section)
        {
            string currentPath =
                Request.Url.AbsolutePath;

            string fileName =
                System.IO.Path.GetFileName(currentPath);

            string fileNameNoEx =
                System.IO.Path.GetFileNameWithoutExtension(
                    currentPath);

            // الصفحة الرئيسية
            if (section == "Default.aspx")
            {
                if (currentPath.EndsWith(
                    "/Default.aspx",
                    StringComparison.OrdinalIgnoreCase) ||
                    currentPath.EndsWith(
                    "/",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return "active";
                }

                return "";
            }

            // صفحة الشؤون المالية للمعلمين
            if (section.Equals(
                "Details",
                StringComparison.OrdinalIgnoreCase) &&
                currentPath.IndexOf(
                    "/Payroll/Details.aspx",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "active";
            }

            // صفحة كشف استحقاقات المعلمين
            if (section.Equals(
                "DetailsView",
                StringComparison.OrdinalIgnoreCase) &&
                currentPath.IndexOf(
                    "/Payroll/DetailsView.aspx",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "active";
            }

            // صفحة كشف الرواتب الأخرى
            if (section.Equals(
                "Statement",
                StringComparison.OrdinalIgnoreCase) &&
                currentPath.IndexOf(
                    "/Payroll/Statement.aspx",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "active";
            }

            // مطابقة اسم الصفحة
            if (section.Equals(
                fileName,
                StringComparison.OrdinalIgnoreCase) ||
                section.Equals(
                    fileNameNoEx,
                    StringComparison.OrdinalIgnoreCase))
            {
                return "active";
            }

            // مطابقة المجلد في الصفحة الافتراضية
            if (fileNameNoEx.Equals(
                "Default",
                StringComparison.OrdinalIgnoreCase) &&
                currentPath.IndexOf(
                    "/" + section + "/",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "active";
            }

            return "";
        }

        // =========================================================
        // تحديد حالة القسم إذا كان أحد روابطه نشطاً
        // =========================================================
        public string GetSectionActiveClass(
            params string[] sections)
        {
            if (sections == null)
                return "";

            foreach (var section in sections)
            {
                if (string.IsNullOrWhiteSpace(section))
                    continue;

                if (!string.IsNullOrEmpty(
                    GetActiveClass(section)))
                {
                    return "section-active";
                }
            }

            return "";
        }

        // =========================================================
        // خصائص قسم شؤون المعلمين والمعلمات
        // =========================================================
        public string TeachersNavActiveClass
        {
            get
            {
                return GetSectionActiveClass(
                    "Teachers",
                    "Home",
                    "MyRecords",
                    "TeachersFinances",
                    "Details",
                    "DetailsView",
                    "Statement",
                    "PermanentStaff");
            }
        }

        public string TeachersFinancesUrl
        {
            get
            {
                return GetStageUrl("Payroll/Details.aspx");
            }
        }

        public string TeachersFinancesActiveClass
        {
            get
            {
                return GetActiveClass("Details");
            }
        }

        // =========================================================
        // إعداد روابط قسم شؤون المعلمين والمعلمات
        // =========================================================
        private void SetupTeacherNavLinks(
            bool isAdmin,
            bool isTeacher,
            bool isFinance,
            bool isAcademic,
            bool isSupervisor,
            bool isView)
        {
            // إدارة المعلمين والمعلمات
            if (liTeachersDefault != null)
            {
                liTeachersDefault.Visible =
                    isAdmin || isAcademic;
            }

            // الحضور والغياب والاستحقاق المالي
            if (liAttendanceHome != null)
            {
                liAttendanceHome.Visible =
                    isAdmin || isAcademic;
            }

            // سجل الغياب والتعويضات
            if (liMyRecords != null)
            {
                liMyRecords.Visible =
                    isAdmin || isAcademic || isTeacher;
            }

            // -----------------------------------------------------
            // الشؤون المالية للمعلمين
            // -----------------------------------------------------
            if (liTeachersFinances != null)
            {
                liTeachersFinances.Visible =
                    isAdmin ||
                    isTeacher ||
                    isFinance ||
                    isSupervisor;
            }

            // -----------------------------------------------------
            // كشف استحقاقات المعلمين
            // مخصص للمدير والمحاسب ودور View.
            // -----------------------------------------------------
            if (liTeachersFinancesView != null)
            {
                liTeachersFinancesView.Visible =
                    isAdmin ||
                    isFinance ||
                    isView;
            }

            // المعلمون الدائمون والمشرفون
            if (liPermanentStaff != null)
            {
                liPermanentStaff.Visible =
                    isAdmin || isFinance || isAcademic;
            }

            // تقرير المعلمين والمعلمات
            if (liTeacherReport != null)
            {
                liTeacherReport.Visible =
                    isAdmin || isAcademic || isView;
            }

            // -----------------------------------------------------
            // رابط إدارة المعلمين والمعلمات
            // -----------------------------------------------------
            SetFlyoutLink(
                lnkTeachersDefault,
                "Teachers/Default.aspx",
                "Teachers",
                "fa-chalkboard-teacher",
                "إدارة المعلمين والمعلمات");

            // -----------------------------------------------------
            // رابط الحضور والغياب والاستحقاق المالي
            // -----------------------------------------------------
            SetFlyoutLink(
                lnkAttendanceHome,
                "TeacherAttendance/Home.aspx",
                "Home",
                "fa-clock",
                "الحضور والغياب والاستحقاق المالي");

            // -----------------------------------------------------
            // رابط المعلمين الدائمين والمشرفين
            // -----------------------------------------------------
            SetFlyoutLink(
                lnkPermanentStaff,
                "TeacherAttendance/PermanentStaff.aspx",
                "PermanentStaff",
                "fa-user-tie",
                "المعلمون الدائمون والمشرفون");

            // -----------------------------------------------------
            // رابط سجل الغياب والتعويضات
            // -----------------------------------------------------
            SetFlyoutLink(
                lnkMyRecords,
                "TeacherAttendance/MyRecords.aspx",
                "MyRecords",
                "fa-user-clock",
                "سجل الغياب والتعويضات");

            // -----------------------------------------------------
            // الرابط الأول: الشؤون المالية للمعلمين
            // -----------------------------------------------------
            SetFlyoutLink(
                lnkTeachersFinances,
                "Payroll/Details.aspx",
                "Details",
                "fa-file-invoice-dollar",
                "الشؤون المالية للمعلمين");

            // -----------------------------------------------------
            // الرابط الثاني: كشف استحقاقات المعلمين
            // -----------------------------------------------------
            SetFlyoutLink(
                lnkTeachersFinancesView,
                "Payroll/DetailsView.aspx",
                "DetailsView",
                "fa-file-alt",
                "كشف استحقاقات المعلمين");

            // -----------------------------------------------------
            // رابط تقرير المعلمين والمعلمات
            // -----------------------------------------------------
            SetFlyoutLink(
                lnkTeacherReport,
                "Teachers/TeacherReport.aspx",
                "TeacherReport",
                "fa-file-alt",
                "تقرير المعلمين والمعلمات");
        }

        // =========================================================
        // ضبط رابط داخل القائمة المنسدلة
        // =========================================================
        private void SetFlyoutLink(
            HtmlAnchor anchor,
            string page,
            string section,
            string icon,
            string caption)
        {
            if (anchor == null)
                return;

            anchor.HRef = GetStageUrl(page);

            anchor.Attributes["class"] =
                GetActiveClass(section);

            anchor.InnerHtml =
                "<i class=\"fas " +
                icon +
                "\"></i> " +
                Server.HtmlEncode(caption);
        }
    }
}

