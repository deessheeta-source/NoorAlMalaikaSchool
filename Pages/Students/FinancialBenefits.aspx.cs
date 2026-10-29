using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Services;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Pages.Students
{
    public partial class FinancialBenefits : System.Web.UI.Page
    {
        private readonly SchoolDbContext db =
            new SchoolDbContext();

        private int StudentId
        {
            get
            {
                int id;

                return int.TryParse(
                    ddlStudent.SelectedValue,
                    out id)
                    ? id
                    : 0;
            }
        }

        private int YearId
        {
            get
            {
                int id;

                return int.TryParse(
                    ddlAcademicYear.SelectedValue,
                    out id)
                    ? id
                    : 0;
            }
        }

        private int ClassRoomId
        {
            get
            {
                int id;

                return int.TryParse(
                    ddlClassRoom.SelectedValue,
                    out id)
                    ? id
                    : 0;
            }
        }

        // =========================================================
        // Page Load
        // =========================================================

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            try
            {
                if (!IsPostBack)
                {
                    LoadClassRooms();

                    LoadYears();

                    LoadFeeTypes();

                    LoadPolicies();

                    LoadStudents();

                    SelectStudentFromQueryString();

                    BindAll();
                }
            }
            catch (Exception ex)
            {
                HandleException(
                    "تعذر تحميل صفحة الاستحقاقات المالية.",
                    ex);
            }
        }

        // =========================================================
        // الصفوف
        // =========================================================

        private void LoadClassRooms()
        {
            EducationStage stage =
                EnumHelper.GetCurrentStage();

            var ids =
                EnumHelper.GetClassRoomIdsForStage(
                    db,
                    stage);

            var classes =
                EnumHelper.SortClassRooms(
                    db.ClassRooms
                        .Where(x =>
                            ids.Contains(x.Id))
                        .ToList());

            ddlClassRoom.Items.Clear();

            ddlClassRoom.Items.Add(
                new ListItem(
                    "-- اختر الصف --",
                    ""));

            foreach (var classroom in classes)
            {
                ddlClassRoom.Items.Add(
                    new ListItem(
                        classroom.Name,
                        classroom.Id.ToString()));
            }
        }

        // =========================================================
        // الطلاب
        // =========================================================

        private void LoadStudents()
        {
            ddlStudent.Items.Clear();

            ddlStudent.Items.Add(
                new ListItem(
                    "-- اختر الطالب --",
                    ""));

            if (ClassRoomId <= 0)
            {
                return;
            }

            var students =
                db.Students
                    .Where(x =>
                        x.IsActive &&
                        x.ClassRoomId ==
                            ClassRoomId)
                    .OrderBy(x => x.Name)
                    .Select(x => new
                    {
                        x.Id,
                        x.Name
                    })
                    .ToList();

            foreach (var student in students)
            {
                ddlStudent.Items.Add(
                    new ListItem(
                        student.Name,
                        student.Id.ToString()));
            }
        }

        // =========================================================
        // السنوات الدراسية
        // =========================================================

        private void LoadYears()
        {
            ddlAcademicYear.Items.Clear();

            var years =
                db.AcademicYears
                    .OrderByDescending(
                        x => x.StartDate)
                    .ToList();

            foreach (var year in years)
            {
                ddlAcademicYear.Items.Add(
                    new ListItem(
                        year.Name,
                        year.Id.ToString()));
            }

            int currentYearId =
                db.AcademicYears
                    .Where(x => x.IsCurrent)
                    .Select(x => x.Id)
                    .FirstOrDefault();

            if (currentYearId > 0)
            {
                ListItem currentItem =
                    ddlAcademicYear.Items
                        .FindByValue(
                            currentYearId.ToString());

                if (currentItem != null)
                {
                    ddlAcademicYear.SelectedValue =
                        currentYearId.ToString();
                }
            }

            if (ddlAcademicYear.SelectedIndex < 0 &&
                ddlAcademicYear.Items.Count > 0)
            {
                ddlAcademicYear.SelectedIndex = 0;
            }
        }

        // =========================================================
        // أنواع الرسوم
        // =========================================================

        private void LoadFeeTypes()
        {
            DropDownList[] lists =
            {
                ddlDiscountFeeType,
                ddlExemptionFeeType
            };

            foreach (DropDownList ddl in lists)
            {
                ddl.Items.Clear();

                ddl.Items.Add(
                    new ListItem(
                        "كل الرسوم",
                        ""));

                foreach (
                    FeeType feeType
                    in Enum.GetValues(
                        typeof(FeeType)))
                {
                    ddl.Items.Add(
                        new ListItem(
                            feeType.GetDisplayName(),
                            feeType.ToString()));
                }
            }
        }

        // =========================================================
        // سياسات الخصم
        // =========================================================

        private void LoadPolicies()
        {
            ddlDiscountPolicy.Items.Clear();

            ddlDiscountPolicy.Items.Add(
                new ListItem(
                    "-- اختر السياسة --",
                    ""));

            var policies =
                db.DiscountPolicies
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.Name)
                    .Select(x => new
                    {
                        x.Id,
                        x.Name
                    })
                    .ToList();

            foreach (var policy in policies)
            {
                ddlDiscountPolicy.Items.Add(
                    new ListItem(
                        policy.Name,
                        policy.Id.ToString()));
            }
        }

        // =========================================================
        // اختيار طالب من QueryString
        // =========================================================

        private void SelectStudentFromQueryString()
        {
            int studentId;

            if (!int.TryParse(
                Request.QueryString["studentId"],
                out studentId))
            {
                return;
            }

            var student =
                db.Students
                    .FirstOrDefault(x =>
                        x.Id == studentId &&
                        x.IsActive);

            if (student == null)
            {
                return;
            }

            if (student.ClassRoomId > 0 &&
                ddlClassRoom.Items.FindByValue(
                    student.ClassRoomId.ToString()) != null)
            {
                ddlClassRoom.SelectedValue =
                    student.ClassRoomId.ToString();
            }

            LoadStudents();

            if (ddlStudent.Items.FindByValue(
                student.Id.ToString()) != null)
            {
                ddlStudent.SelectedValue =
                    student.Id.ToString();
            }
        }

        // =========================================================
        // تغيير الصف
        // =========================================================

        protected void ddlClassRoom_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            try
            {
                ClearMessage();

                LoadStudents();

                ClearGrids();

                lblEligibility.Text =
                    "اختر الطالب لعرض استحقاقاته المالية.";
            }
            catch (Exception ex)
            {
                HandleException(
                    "تعذر تحميل طلاب الصف.",
                    ex);
            }
        }

        // =========================================================
        // تغيير الطالب
        // =========================================================

        protected void ddlStudent_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            try
            {
                ClearMessage();

                BindAll();
            }
            catch (Exception ex)
            {
                HandleException(
                    "تعذر تحميل الاستحقاقات الخاصة بالطالب.",
                    ex);
            }
        }

        // =========================================================
        // تغيير العام الدراسي
        // =========================================================

        protected void ddlAcademicYear_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            try
            {
                ClearMessage();

                BindAll();
            }
            catch (Exception ex)
            {
                HandleException(
                    "تعذر تحميل استحقاقات الطالب للسنة المحددة.",
                    ex);
            }
        }

        // =========================================================
        // تحميل بيانات الطالب
        // =========================================================

        private void BindAll()
        {
            ClearMessage();

            ClearGrids();

            if (ClassRoomId <= 0)
            {
                lblEligibility.Text =
                    "يرجى اختيار الصف.";

                return;
            }

            if (StudentId <= 0)
            {
                lblEligibility.Text =
                    "يرجى اختيار الطالب.";

                return;
            }

            if (YearId <= 0)
            {
                lblEligibility.Text =
                    "يرجى اختيار العام الدراسي.";

                return;
            }

            var student =
                db.Students
                    .Include(x => x.ClassRoom)
                    .FirstOrDefault(x =>
                        x.Id == StudentId &&
                        x.IsActive);

            if (student == null)
            {
                ShowError(
                    "الطالب غير موجود أو غير نشط.");

                return;
            }

            if (student.ClassRoomId !=
                ClassRoomId)
            {
                ShowError(
                    "الطالب المحدد لا ينتمي إلى الصف المختار.");

                return;
            }

            EnsureStudentEnrollment(student);

            string yearName =
                ddlAcademicYear.SelectedItem != null
                    ? ddlAcademicYear.SelectedItem.Text
                    : "";

            lblEligibility.Text =
                "الطالب: " +
                HttpUtility.HtmlEncode(
                    student.Name) +
                " | العام الدراسي: " +
                HttpUtility.HtmlEncode(
                    yearName);

            LoadStudentDiscounts();

            LoadStudentExemptions();
        }

        // =========================================================
        // تسجيل الطالب في العام
        // =========================================================

        private void EnsureStudentEnrollment(
            Student student)
        {
            var enrollment =
                db.StudentAcademicYears
                    .FirstOrDefault(x =>
                        x.StudentId ==
                            student.Id &&
                        x.AcademicYearId ==
                            YearId);

            if (enrollment == null)
            {
                StudentFinancialService
                    .EnsureEnrollment(
                        db,
                        student,
                        YearId);

                db.SaveChanges();
            }
            else if (
                enrollment.ClassRoomId !=
                student.ClassRoomId)
            {
                enrollment.ClassRoomId =
                    student.ClassRoomId;

                enrollment.IsActive =
                    student.IsActive;

                db.SaveChanges();
            }
        }

        // =========================================================
        // تحميل الخصومات
        // =========================================================

        private void LoadStudentDiscounts()
        {
            var discounts =
                db.StudentDiscounts
                    .Include(x => x.DiscountPolicy)
                    .Where(x =>
                        x.StudentId ==
                            StudentId &&
                        x.AcademicYearId ==
                            YearId &&
                        x.IsActive)
                    .OrderByDescending(
                        x => x.StartDate)
                    .ThenByDescending(
                        x => x.Id)
                    .ToList();

            var rows =
                discounts
                    .Select(x => new
                    {
                        x.Id,

                        Name =
                            x.DiscountPolicy != null
                                ? x.DiscountPolicy.Name
                                : "خصم خاص",

                        ValueText =
                            FormatBenefitValue(
                                x.Kind,
                                x.Value),

                        FeeTypeName =
                            FormatFeeType(
                                x.FeeType),

                        Reason =
                            string.IsNullOrWhiteSpace(
                                x.Reason)
                                ? "-"
                                : x.Reason
                    })
                    .ToList();

            gvDiscounts.DataSource =
                rows;

            gvDiscounts.DataBind();
        }

        // =========================================================
        // تحميل الإعفاءات
        // =========================================================

        private void LoadStudentExemptions()
        {
            var exemptions =
                db.StudentExemptions
                    .Where(x =>
                        x.StudentId ==
                            StudentId &&
                        x.AcademicYearId ==
                            YearId &&
                        x.IsActive)
                    .OrderByDescending(
                        x => x.StartDate)
                    .ThenByDescending(
                        x => x.Id)
                    .ToList();

            var rows =
                exemptions
                    .Select(x => new
                    {
                        x.Id,

                        ValueText =
                            FormatBenefitValue(
                                x.Kind,
                                x.Value),

                        FeeTypeName =
                            FormatFeeType(
                                x.FeeType),

                        Reason =
                            string.IsNullOrWhiteSpace(
                                x.Reason)
                                ? "-"
                                : x.Reason
                    })
                    .ToList();

            gvExemptions.DataSource =
                rows;

            gvExemptions.DataBind();
        }

        // =========================================================
        // تنسيق القيمة
        // =========================================================

        private string FormatBenefitValue(
            DiscountKind kind,
            decimal value)
        {
            if (kind ==
                DiscountKind.Percentage)
            {
                return value.ToString("N2") +
                       "%";
            }

            return value.ToString("N2") +
                   " د.ل";
        }

        // =========================================================
        // تنسيق نوع الرسم
        // =========================================================

        private string FormatFeeType(
            FeeType? feeType)
        {
            if (!feeType.HasValue)
            {
                return "كل الرسوم";
            }

            return feeType.Value
                .GetDisplayName();
        }

        // =========================================================
        // إضافة خصم
        // =========================================================

        protected void btnAddDiscount_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                ClearMessage();

                if (StudentId <= 0 ||
                    ClassRoomId <= 0 ||
                    YearId <= 0)
                {
                    ShowError(
                        "اختر الصف والطالب والعام الدراسي.");

                    return;
                }

                int policyId;

                if (!int.TryParse(
                    ddlDiscountPolicy.SelectedValue,
                    out policyId) ||
                    policyId <= 0)
                {
                    ShowError(
                        "اختر سياسة الخصم.");

                    return;
                }

                decimal value;

                if (!decimal.TryParse(
                    txtDiscountValue.Text,
                    out value) ||
                    value <= 0)
                {
                    ShowError(
                        "قيمة الخصم غير صحيحة.");

                    return;
                }

                var policy =
                    db.DiscountPolicies
                        .FirstOrDefault(x =>
                            x.Id == policyId &&
                            x.IsActive);

                if (policy == null)
                {
                    ShowError(
                        "سياسة الخصم غير موجودة أو غير نشطة.");

                    return;
                }

                DiscountKind kind;

                if (!Enum.TryParse(
                    ddlDiscountKind.SelectedValue,
                    true,
                    out kind))
                {
                    ShowError(
                        "نوع الخصم غير صحيح.");

                    return;
                }

                if (kind ==
                    DiscountKind.Percentage &&
                    value > 100m)
                {
                    ShowError(
                        "نسبة الخصم لا يمكن أن تتجاوز 100%.");

                    return;
                }

                FeeType parsedFeeType;

                FeeType? feeType =
                    Enum.TryParse(
                        ddlDiscountFeeType.SelectedValue,
                        true,
                        out parsedFeeType)
                        ? (FeeType?)parsedFeeType
                        : null;

                string decisionNo =
                    txtDiscountDecisionNo.Text == null
                        ? ""
                        : txtDiscountDecisionNo.Text.Trim();

                string reason =
                    txtDiscountReason.Text == null
                        ? ""
                        : txtDiscountReason.Text.Trim();

                string approvedBy =
                    Session["DisplayName"] as string;

                var discount =
                    new StudentDiscount
                    {
                        StudentId =
                            StudentId,

                        AcademicYearId =
                            YearId,

                        DiscountPolicyId =
                            policyId,

                        Type =
                            policy.Type,

                        Kind =
                            kind,

                        Value =
                            value,

                        FeeType =
                            feeType,

                        StartDate =
                            DateTime.Today,

                        DecisionNo =
                            decisionNo,

                        DecisionDate =
                            DateTime.Today,

                        ApprovedBy =
                            approvedBy,

                        ApprovedAt =
                            DateTime.Now,

                        Reason =
                            reason,

                        IsActive =
                            true
                    };

                db.StudentDiscounts.Add(
                    discount);

                db.SaveChanges();

                ShowSuccess(
                    "تمت إضافة الخصم إلى استحقاق الطالب بنجاح.");

                ClearDiscountInputs();

                BindAll();
            }
            catch (Exception ex)
            {
                HandleException(
                    "تعذر إضافة الخصم.",
                    ex);
            }
        }

        // =========================================================
        // إضافة إعفاء
        // =========================================================

        protected void btnAddExemption_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                ClearMessage();

                if (StudentId <= 0 ||
                    ClassRoomId <= 0 ||
                    YearId <= 0)
                {
                    ShowError(
                        "اختر الصف والطالب والعام الدراسي.");

                    return;
                }

                decimal value;

                if (!decimal.TryParse(
                    txtExemptionValue.Text,
                    out value) ||
                    value <= 0)
                {
                    ShowError(
                        "قيمة الإعفاء غير صحيحة.");

                    return;
                }

                DiscountKind kind;

                if (!Enum.TryParse(
                    ddlExemptionKind.SelectedValue,
                    true,
                    out kind))
                {
                    ShowError(
                        "نوع الإعفاء غير صحيح.");

                    return;
                }

                if (kind ==
                    DiscountKind.Percentage &&
                    value > 100m)
                {
                    ShowError(
                        "نسبة الإعفاء لا يمكن أن تتجاوز 100%.");

                    return;
                }

                FeeType parsedFeeType;

                FeeType? feeType =
                    Enum.TryParse(
                        ddlExemptionFeeType.SelectedValue,
                        true,
                        out parsedFeeType)
                        ? (FeeType?)parsedFeeType
                        : null;

                string decisionNo =
                    txtExemptionDecisionNo.Text == null
                        ? ""
                        : txtExemptionDecisionNo.Text.Trim();

                string reason =
                    txtExemptionReason.Text == null
                        ? ""
                        : txtExemptionReason.Text.Trim();

                string approvedBy =
                    Session["DisplayName"] as string;

                var exemption =
                    new StudentExemption
                    {
                        StudentId =
                            StudentId,

                        AcademicYearId =
                            YearId,

                        FeeType =
                            feeType,

                        Kind =
                            kind,

                        Value =
                            value,

                        StartDate =
                            DateTime.Today,

                        DecisionNo =
                            decisionNo,

                        DecisionDate =
                            DateTime.Today,

                        ApprovedBy =
                            approvedBy,

                        ApprovedAt =
                            DateTime.Now,

                        Reason =
                            reason,

                        IsActive =
                            true
                    };

                db.StudentExemptions.Add(
                    exemption);

                db.SaveChanges();

                ShowSuccess(
                    "تمت إضافة الإعفاء إلى استحقاق الطالب بنجاح.");

                ClearExemptionInputs();

                BindAll();
            }
            catch (Exception ex)
            {
                HandleException(
                    "تعذر إضافة الإعفاء.",
                    ex);
            }
        }

        // =========================================================
        // إلغاء الخصم
        // =========================================================

        protected void gvDiscounts_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName !=
                    "Deactivate")
                {
                    return;
                }

                int index;

                if (!int.TryParse(
                    Convert.ToString(
                        e.CommandArgument),
                    out index))
                {
                    ShowError(
                        "تعذر تحديد الخصم.");

                    return;
                }

                var rows =
                    db.StudentDiscounts
                        .Where(x =>
                            x.StudentId ==
                                StudentId &&
                            x.AcademicYearId ==
                                YearId &&
                            x.IsActive)
                        .OrderByDescending(
                            x => x.StartDate)
                        .ThenByDescending(
                            x => x.Id)
                        .ToList();

                if (index < 0 ||
                    index >= rows.Count)
                {
                    ShowError(
                        "الخصم المطلوب غير موجود.");

                    return;
                }

                var discount =
                    rows[index];

                discount.IsActive =
                    false;

                discount.EndDate =
                    DateTime.Today;

                db.SaveChanges();

                ShowSuccess(
                    "تم إلغاء الخصم بنجاح.");

                BindAll();
            }
            catch (Exception ex)
            {
                HandleException(
                    "تعذر إلغاء الخصم.",
                    ex);
            }
        }

        // =========================================================
        // إلغاء الإعفاء
        // =========================================================

        protected void gvExemptions_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName !=
                    "Deactivate")
                {
                    return;
                }

                int index;

                if (!int.TryParse(
                    Convert.ToString(
                        e.CommandArgument),
                    out index))
                {
                    ShowError(
                        "تعذر تحديد الإعفاء.");

                    return;
                }

                var rows =
                    db.StudentExemptions
                        .Where(x =>
                            x.StudentId ==
                                StudentId &&
                            x.AcademicYearId ==
                                YearId &&
                            x.IsActive)
                        .OrderByDescending(
                            x => x.StartDate)
                        .ThenByDescending(
                            x => x.Id)
                        .ToList();

                if (index < 0 ||
                    index >= rows.Count)
                {
                    ShowError(
                        "الإعفاء المطلوب غير موجود.");

                    return;
                }

                var exemption =
                    rows[index];

                exemption.IsActive =
                    false;

                exemption.EndDate =
                    DateTime.Today;

                db.SaveChanges();

                ShowSuccess(
                    "تم إلغاء الإعفاء بنجاح.");

                BindAll();
            }
            catch (Exception ex)
            {
                HandleException(
                    "تعذر إلغاء الإعفاء.",
                    ex);
            }
        }

        // =========================================================
        // تنظيف الجداول
        // =========================================================

        private void ClearGrids()
        {
            gvDiscounts.DataSource =
                null;

            gvDiscounts.DataBind();

            gvExemptions.DataSource =
                null;

            gvExemptions.DataBind();
        }

        // =========================================================
        // تنظيف حقول الخصم
        // =========================================================

        private void ClearDiscountInputs()
        {
            txtDiscountValue.Text = "";

            txtDiscountDecisionNo.Text = "";

            txtDiscountReason.Text = "";

            if (ddlDiscountFeeType.Items.Count > 0)
            {
                ddlDiscountFeeType.SelectedIndex =
                    0;
            }

            if (ddlDiscountKind.Items.Count > 0)
            {
                ddlDiscountKind.SelectedIndex =
                    0;
            }

            if (ddlDiscountPolicy.Items.Count > 0)
            {
                ddlDiscountPolicy.SelectedIndex =
                    0;
            }
        }

        // =========================================================
        // تنظيف حقول الإعفاء
        // =========================================================

        private void ClearExemptionInputs()
        {
            txtExemptionValue.Text = "";

            txtExemptionDecisionNo.Text = "";

            txtExemptionReason.Text = "";

            if (ddlExemptionFeeType.Items.Count > 0)
            {
                ddlExemptionFeeType.SelectedIndex =
                    0;
            }

            if (ddlExemptionKind.Items.Count > 0)
            {
                ddlExemptionKind.SelectedIndex =
                    0;
            }
        }

        // =========================================================
        // رسالة نجاح
        // =========================================================

        private void ShowSuccess(
            string message)
        {
            lblMessage.Text =
                HttpUtility.HtmlEncode(
                    message);

            lblMessage.CssClass =
                "text-success d-block mt-3 fw-bold";
        }

        // =========================================================
        // رسالة خطأ
        // =========================================================

        private void ShowError(
            string message)
        {
            lblMessage.Text =
                HttpUtility.HtmlEncode(
                    message);

            lblMessage.CssClass =
                "text-danger d-block mt-3 fw-bold";
        }

        // =========================================================
        // مسح الرسالة
        // =========================================================

        private void ClearMessage()
        {
            lblMessage.Text = "";

            lblMessage.CssClass =
                "d-block mt-3";
        }

        // =========================================================
        // معالجة الأخطاء
        // =========================================================

        private void HandleException(
            string friendlyMessage,
            Exception ex)
        {
            System.Diagnostics.Trace.TraceError(
                "FinancialBenefits Error: " +
                ex);

            string databaseMessage =
                GetInnerExceptionMessage(ex);

            string finalMessage =
                friendlyMessage;

            if (!string.IsNullOrWhiteSpace(
                databaseMessage))
            {
                finalMessage +=
                    " التفاصيل: " +
                    databaseMessage;
            }

            ShowError(
                finalMessage);
        }

        // =========================================================
        // استخراج الخطأ الداخلي
        // =========================================================

        private string GetInnerExceptionMessage(
            Exception ex)
        {
            Exception current = ex;

            while (current.InnerException != null)
            {
                current =
                    current.InnerException;
            }

            string message =
                current.Message;

            if (message == null)
            {
                return "";
            }

            if (message.Length > 700)
            {
                message =
                    message.Substring(
                        0,
                        700);
            }

            return message;
        }

        // =========================================================
        // التخلص من DbContext
        // =========================================================

        protected override void OnUnload(
            EventArgs e)
        {
            if (db != null)
            {
                db.Dispose();
            }

            base.OnUnload(e);
        }
    }
}