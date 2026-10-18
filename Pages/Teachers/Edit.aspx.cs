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
    public partial class Edit : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();

        #region Page Lifecycle

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
                    LoadTeacher(id);
                }
                else
                {
                    ShowNotFound();
                }
            }
        }

        #endregion

        #region Load

        private void LoadTeacher(int id)
        {
            var teacher = _context.Teachers.Find(id);

            if (teacher == null)
            {
                ShowNotFound();
                return;
            }

            hfTeacherId.Value = teacher.Id.ToString();

            // أجزاء الاسم
            txtFirstName.Text = teacher.FirstName;
            txtFatherName.Text = teacher.FatherName;
            txtFamilyName.Text = teacher.FamilyName;

            // الرقم الوطني
            txtNationalId.Text = teacher.NationalId;

            // الجنس
            ddlGender.SelectedValue = teacher.Gender.ToString();

            // تاريخ الميلاد
            txtBirthDate.Text = teacher.BirthDate.HasValue
                ? teacher.BirthDate.Value.ToString("yyyy-MM-dd")
                : "";

            // الهاتف
            txtPhone.Text = teacher.Phone;

            // العنوان
            txtAddress.Text = teacher.Address;

            // ✅ الحالة
            ddlIsActive.SelectedValue = teacher.IsActive ? "True" : "False";

            var employmentItem = ddlEmploymentType.Items.FindByValue(teacher.EmploymentType.ToString());
            ddlEmploymentType.SelectedValue = employmentItem != null ? employmentItem.Value : EmploymentType.Permanent.ToString();
            txtBaseSalary.Text = teacher.BaseSalary > 0m ? teacher.BaseSalary.ToString("0.00") : "";
            txtLessonRate.Text = teacher.LessonRate > 0m ? teacher.LessonRate.ToString("0.00") : "";

            PopulateAcademicYears(teacher.AcademicYearId);

            // التخصص
            PopulateSpecialty(teacher.Specialty);
        }

        private void PopulateAcademicYears(int? selectedId)
        {
            ddlAcademicYear.Items.Clear();
            ddlAcademicYear.Items.Add(new ListItem("-- اختر السنة الدراسية --", ""));

            var years = _context.AcademicYears
                .OrderByDescending(y => y.Name)
                .ToList();

            foreach (var year in years)
                ddlAcademicYear.Items.Add(new ListItem(year.Name, year.Id.ToString()));

            if (selectedId.HasValue && ddlAcademicYear.Items.FindByValue(selectedId.Value.ToString()) != null)
            {
                ddlAcademicYear.SelectedValue = selectedId.Value.ToString();
            }
            else
            {
                var current = years.FirstOrDefault(y => y.IsCurrent);
                if (current != null)
                    ddlAcademicYear.SelectedValue = current.Id.ToString();
            }
        }

        private void PopulateSpecialty(string selectedValue)
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

            // في حال كان التخصص القديم غير موجود ضمن المواد الحالية
            if (!string.IsNullOrWhiteSpace(selectedValue))
            {
                bool exists = ddlSpecialty.Items.Cast<ListItem>()
                    .Any(i => i.Value == selectedValue);

                if (!exists)
                {
                    ddlSpecialty.Items.Add(new ListItem(selectedValue, selectedValue));
                }

                ddlSpecialty.SelectedValue = selectedValue;
            }
        }

        #endregion

        #region Save

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // قراءة أجزاء الاسم
            string firstName = txtFirstName.Text.Trim();
            string fatherName = txtFatherName.Text.Trim();
            string familyName = txtFamilyName.Text.Trim();

            // التحقق
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

            // تكوين الاسم الكامل

            try
            {
                int id;
                if (!int.TryParse(hfTeacherId.Value, out id))
                {
                    ShowError("معرف المعلم غير صحيح.");
                    return;
                }

                var teacher = _context.Teachers.Find(id);
                if (teacher == null)
                {
                    ShowError("المعلم/المعلمة غير موجود.");
                    return;
                }

                string fullName = string.IsNullOrWhiteSpace(teacher.GrandfatherName)
                    ? string.Join(" ", firstName, fatherName, familyName)
                    : string.Join(" ", firstName, fatherName, teacher.GrandfatherName, familyName);

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

                // الرقم الوطني
                string nationalId = string.IsNullOrWhiteSpace(txtNationalId.Text)
                    ? null
                    : txtNationalId.Text.Trim();

                if (!string.IsNullOrEmpty(nationalId))
                {
                    bool exists = _context.Teachers
                        .Any(t => t.NationalId == nationalId && t.Id != id);

                    if (exists)
                    {
                        ShowError("الرقم الوطني مستخدم مسبقاً لمعلم آخر.");
                        return;
                    }
                }

                // الجنس
                Gender gender = Gender.Male;
                if (!string.IsNullOrWhiteSpace(ddlGender.SelectedValue))
                {
                    if (!Enum.TryParse(ddlGender.SelectedValue, out gender))
                    {
                        ShowError("قيمة الجنس غير صحيحة.");
                        return;
                    }
                }

                // ✅ الحالة
                bool isActive = true;
                bool.TryParse(ddlIsActive.SelectedValue, out isActive);

                // تحديث
                teacher.EmploymentType = employmentType;
                teacher.AcademicYearId = academicYearId;
                teacher.BaseSalary = baseSalary;
                teacher.LessonRate = lessonRate;

                teacher.FirstName = firstName;
                teacher.FatherName = fatherName;
                teacher.FamilyName = familyName;
                teacher.Name = fullName;

                teacher.NationalId = nationalId;
                teacher.Gender = gender;
                teacher.BirthDate = ParseDate(txtBirthDate.Text);
                teacher.Phone = txtPhone.Text.Trim();

                teacher.Specialty = string.IsNullOrWhiteSpace(ddlSpecialty.SelectedValue)
                    ? null
                    : ddlSpecialty.SelectedValue.Trim();

                teacher.Address = txtAddress.Text.Trim();

                // ✅ الحالة
                teacher.IsActive = isActive;

                // ✅ قيمة الحصة
                teacher.LessonRate = lessonRate;

                _context.SaveChanges();

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

        private void ShowNotFound()
        {
            pnlForm.Visible = false;
            pnlNotFound.Visible = true;
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