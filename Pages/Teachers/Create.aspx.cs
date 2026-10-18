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
    public partial class Create : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                PopulateAcademicYears();
                PopulateSpecialty();
            }
        }

        #endregion

        #region Academic Year

        private void PopulateAcademicYears()
        {
            ddlAcademicYear.Items.Clear();
            ddlAcademicYear.Items.Add(new ListItem("-- اختر السنة الدراسية --", ""));

            var years = _context.AcademicYears
                .OrderByDescending(y => y.Name)
                .ToList();

            foreach (var year in years)
                ddlAcademicYear.Items.Add(new ListItem(year.Name, year.Id.ToString()));

            var current = years.FirstOrDefault(y => y.IsCurrent);
            if (current != null)
                ddlAcademicYear.SelectedValue = current.Id.ToString();
        }

        #endregion

        #region Specialty

        private void PopulateSpecialty()
        {
            ddlSpecialty.Items.Clear();
            ddlSpecialty.Items.Add(new ListItem("-- اختر المادة --", ""));

            var stage = EnumHelper.GetCurrentStage();

            var subjectNames = _context.Subjects
                .Where(s => s.Stage == stage)
                .Select(s => s.Name)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            foreach (var name in subjectNames)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    ddlSpecialty.Items.Add(new ListItem(name, name));
                }
            }
        }

        #endregion

        #region Save

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // =========================
            // قراءة وتنظيف أجزاء الاسم
            // =========================
            string firstName = txtFirstName.Text.Trim();
            string fatherName = txtFatherName.Text.Trim();
            string familyName = txtFamilyName.Text.Trim();

            // =========================
            // التحقق من الأسماء المطلوبة
            // =========================
            if (string.IsNullOrWhiteSpace(firstName))
            {
                ShowError("الاسم الأول مطلوب.");
                return;
            }

            if (string.IsNullOrWhiteSpace(fatherName))
            {
                ShowError("اسم الأب مطلوب.");
                return;
            }

            if (string.IsNullOrWhiteSpace(familyName))
            {
                ShowError("اسم العائلة مطلوب.");
                return;
            }

            EmploymentType employmentType;
            if (!Enum.TryParse(ddlEmploymentType.SelectedValue, true, out employmentType))
            {
                ShowError("نوع التعاقد غير صحيح.");
                return;
            }

            decimal baseSalary = ParseMoney(txtBaseSalary.Text);
            decimal lessonRate = ParseMoney(txtLessonRate.Text);

            if ((employmentType == EmploymentType.Permanent || employmentType == EmploymentType.PermanentSupervisor) && baseSalary <= 0m)
            {
                ShowError("الراتب الشهري مطلوب ويجب أن يكون أكبر من صفر للمعلم الدائم أو المشرف الدائم.");
                return;
            }

            if (employmentType == EmploymentType.PerLesson && lessonRate <= 0m)
            {
                ShowError("قيمة الحصة مطلوبة ويجب أن تكون أكبر من صفر لمعلم الحصة.");
                return;
            }

            // =========================
            // تكوين الاسم الكامل
            // =========================
            string fullName = string.Join(" ", firstName, fatherName, familyName);

            int academicYearId;
            if (!int.TryParse(ddlAcademicYear.SelectedValue, out academicYearId) || academicYearId <= 0)
            {
                ShowError("السنة الدراسية مطلوبة.");
                return;
            }

            if (!_context.AcademicYears.Any(y => y.Id == academicYearId))
            {
                ShowError("السنة الدراسية المحددة غير موجودة.");
                return;
            }

            try
            {
                // =========================
                // الرقم الوطني
                // =========================
                string nationalId = string.IsNullOrWhiteSpace(txtNationalId.Text)
                    ? null
                    : txtNationalId.Text.Trim();

                if (!string.IsNullOrEmpty(nationalId))
                {
                    bool nationalIdExists = _context.Teachers
                        .Any(t => t.NationalId == nationalId);

                    if (nationalIdExists)
                    {
                        ShowError("الرقم الوطني مستخدم مسبقاً لمعلم آخر.");
                        return;
                    }
                }

                // =========================
                // الجنس
                // =========================
                Gender gender = Gender.Male;

                if (!string.IsNullOrWhiteSpace(ddlGender.SelectedValue))
                {
                    if (!Enum.TryParse(ddlGender.SelectedValue, out gender))
                    {
                        ShowError("قيمة الجنس غير صحيحة.");
                        return;
                    }
                }

                // =========================
                // الحالة (نشط / غير نشط)
                // =========================
                bool isActive = true;
                bool.TryParse(ddlIsActive.SelectedValue, out isActive);

                // =========================
                // إنشاء المعلم
                // =========================
                var teacher = new Teacher
                {
                    // الاسم
                    FirstName = firstName,
                    FatherName = fatherName,
                    GrandfatherName = null,
                    FamilyName = familyName,
                    Name = fullName,

                    // البيانات الشخصية
                    NationalId = nationalId,
                    Gender = gender,
                    BirthDate = ParseDate(txtBirthDate.Text),
                    Phone = txtPhone.Text.Trim(),
                    Specialty = string.IsNullOrWhiteSpace(ddlSpecialty.SelectedValue)
                        ? null
                        : ddlSpecialty.SelectedValue.Trim(),
                    Address = txtAddress.Text.Trim(),

                    // المرحلة
                    Stage = EnumHelper.GetCurrentStage(),
                    AcademicYearId = academicYearId,

                    // الحالة
                    IsActive = isActive,

                    // ✅ القيم المالية
                    EmploymentType = employmentType,
                    LessonRate = lessonRate,
                    BaseSalary = baseSalary,

                    // تاريخ التعيين الافتراضي
                    HireDate = DateTime.Today
                };

                // =========================
                // الحفظ
                // =========================
                _context.Teachers.Add(teacher);
                _context.SaveChanges();

                // =========================
                // العودة للقائمة
                // =========================
                Response.Redirect("Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "alert alert-danger d-block";
                lblMessage.Text = ErrorMessageHelper.GetUserMessage(ex, GetType().FullName);
            }
        }

        #endregion

        #region Helpers

        private decimal ParseMoney(string value)
        {
            decimal result;
            return decimal.TryParse(value, out result) && result > 0m ? result : 0m;
        }

        private DateTime? ParseDate(string value)
        {
            DateTime result;
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result) ? result : (DateTime?)null;
        }

        private void ShowError(string message)
        {
            lblMessage.CssClass = "alert alert-danger d-block";
            lblMessage.Text = Server.HtmlEncode(message);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_context != null) _context.Dispose();
            base.OnUnload(e);
        }

        #endregion
    }
}