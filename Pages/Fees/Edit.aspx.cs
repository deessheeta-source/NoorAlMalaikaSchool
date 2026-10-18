using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Fees
{
    public partial class Edit : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();
        private int feeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            if (!int.TryParse(Request.QueryString["id"], out feeId) || feeId <= 0)
            {
                ShowNotFound();
                return;
            }

            if (!IsPostBack)
            {
                LoadDropdowns();
                LoadFeeTypes();
                LoadFee();
            }
        }

        private void ShowNotFound()
        {
            pnlNotFound.Visible = true;
            pnlForm.Visible = false;
        }

        private void ShowForm()
        {
            pnlNotFound.Visible = false;
            pnlForm.Visible = true;
        }

        // =========================================================
        // تحميل القوائم الأولية
        // =========================================================
        private void LoadDropdowns()
        {
            string stageValue = Session["Stage"] as string ?? string.Empty;
            lblStage.Text = EnumHelper.GetStageArabic(stageValue);

            ddlAcademicYear.Items.Clear();
            ddlAcademicYear.Items.Add(new ListItem("— اختر السنة الدراسية —", ""));

            var academicYears = db.AcademicYears
                .OrderByDescending(a => a.StartDate)
                .ToList();

            foreach (var year in academicYears)
                ddlAcademicYear.Items.Add(new ListItem(year.Name, year.Id.ToString()));

            var currentYear = db.AcademicYears.FirstOrDefault(a => a.IsCurrent);
            if (currentYear != null &&
                ddlAcademicYear.Items.FindByValue(currentYear.Id.ToString()) != null)
            {
                ddlAcademicYear.SelectedValue = currentYear.Id.ToString();
            }

            LoadGradeLevelsForYear();
        }

        // =========================================================
        // تحميل الصفوف الدراسية (GradeLevels) حسب السنة والمرحلة
        // =========================================================
        private void LoadGradeLevelsForYear()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();

            int yearId;
            if (!int.TryParse(ddlAcademicYear.SelectedValue, out yearId) || yearId <= 0)
            {
                ddlClassRoom.Items.Clear();
                ddlClassRoom.Items.Add(new ListItem("— اختر السنة أولاً —", ""));
                ResetSection();
                return;
            }

            // الصفوف التي لها فصول في هذه السنة ضمن المرحلة الحالية
            var gradeLevelIdsInYear = db.ClassRooms
                .Where(c => c.AcademicYearId == yearId
                         && c.GradeLevel != null
                         && c.GradeLevel.Stage == stage)
                .Select(c => c.GradeLevelId)
                .Distinct()
                .ToList();

            var gradeLevels = db.GradeLevels
                .AsNoTracking()
                .Where(g => g.Stage == stage && gradeLevelIdsInYear.Contains(g.Id))
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlClassRoom.Items.Clear();
            ddlClassRoom.Items.Add(new ListItem("— اختر الصف —", ""));

            foreach (var g in gradeLevels)
                ddlClassRoom.Items.Add(new ListItem(g.Name, g.Id.ToString()));
        }

        // =========================================================
        // دوال القسم
        // =========================================================
        private int GetSelectedGradeLevelId()
        {
            int id;
            if (!int.TryParse(ddlClassRoom.SelectedValue, out id)) return 0;
            return id > 0 ? id : 0;
        }

        private bool IsSecondaryGrade(int gradeLevelId)
        {
            if (gradeLevelId <= 0) return false;

            try
            {
                string gradeName = db.GradeLevels
                    .AsNoTracking()
                    .Where(g => g.Id == gradeLevelId)
                    .Select(g => g.Name)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(gradeName)) return false;
                gradeName = gradeName.Trim();
                return gradeName == "ثاني ثانوي" || gradeName == "ثالث ثانوي";
            }
            catch { return false; }
        }

        private void ConfigureSectionVisibility()
        {
            bool show = IsSecondaryGrade(GetSelectedGradeLevelId());
            pnlSection.Visible = show;

            if (!show && ddlSection.Items.FindByValue("0") != null)
                ddlSection.SelectedValue = "0";
        }

        private void ResetSection()
        {
            pnlSection.Visible = false;
            if (ddlSection.Items.FindByValue("0") != null)
                ddlSection.SelectedValue = "0";
        }

        private string GetSelectedSection()
        {
            if (ddlSection == null) return null;
            string value = ddlSection.SelectedValue;
            if (string.IsNullOrWhiteSpace(value) || value == "0") return null;
            return value;
        }

        protected void ddlAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGradeLevelsForYear();
            ResetSection();
        }

        protected void ddlClassRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionVisibility();
        }

        private void LoadFeeTypes()
        {
            ddlType.Items.Clear();

            foreach (FeeType type in Enum.GetValues(typeof(FeeType)))
                ddlType.Items.Add(new ListItem(type.GetDisplayName(), type.ToString()));
        }

        // =========================================================
        // تحميل بيانات الرسم المُراد تعديله
        // =========================================================
        private void LoadFee()
        {
            var fee = db.Fees
                .Include("ClassRoom")
                .FirstOrDefault(f => f.Id == feeId);

            if (fee == null)
            {
                ShowNotFound();
                return;
            }

            ShowForm();

            txtName.Text = fee.Name ?? string.Empty;
            txtAmount.Text = fee.Amount.ToString("0.##");

            if (ddlType.Items.FindByValue(fee.Type.ToString()) != null)
                ddlType.SelectedValue = fee.Type.ToString();

            if (ddlAcademicYear.Items.FindByValue(fee.AcademicYearId.ToString()) != null)
                ddlAcademicYear.SelectedValue = fee.AcademicYearId.ToString();

            // إعادة تحميل الصفوف بعد تحديد السنة
            LoadGradeLevelsForYear();

            // تحديد الصف (GradeLevelId) إن وُجد
            int? gradeLevelId = fee.GradeLevelId
                ?? (fee.ClassRoom != null ? (int?)fee.ClassRoom.GradeLevelId : null);

            if (gradeLevelId.HasValue
                && ddlClassRoom.Items.FindByValue(gradeLevelId.Value.ToString()) != null)
            {
                ddlClassRoom.SelectedValue = gradeLevelId.Value.ToString();
            }

            ConfigureSectionVisibility();

            // تحديد القسم إن كان ثانوي
            if (IsSecondaryGrade(GetSelectedGradeLevelId()))
            {
                string section = null;

                // أولاً: من ClassRoom.SectionName
                if (fee.ClassRoom != null && !string.IsNullOrWhiteSpace(fee.ClassRoom.SectionName))
                {
                    string s = fee.ClassRoom.SectionName.Trim();
                    if (s == "علمي" || s == "أدبي")
                        section = s;
                }

                if (!string.IsNullOrWhiteSpace(section)
                    && ddlSection.Items.FindByValue(section) != null)
                {
                    ddlSection.SelectedValue = section;
                }
            }

            txtNotes.Text = fee.Notes ?? string.Empty;
        }

        // =========================================================
        // حفظ التعديلات
        // =========================================================
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            var fee = db.Fees.FirstOrDefault(f => f.Id == feeId);
            if (fee == null)
            {
                ShowNotFound();
                return;
            }

            // ==============================
            // الاسم
            // ==============================
            string name = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowError("يرجى إدخال اسم الرسوم.");
                return;
            }

            // ==============================
            // المبلغ
            // ==============================
            decimal amount;
            if (!decimal.TryParse(txtAmount.Text.Trim(), out amount) || amount <= 0)
            {
                ShowError("يرجى إدخال مبلغ رسوم أكبر من صفر.");
                return;
            }

            // ==============================
            // السنة
            // ==============================
            int academicYearId;
            if (!int.TryParse(ddlAcademicYear.SelectedValue, out academicYearId) || academicYearId <= 0)
            {
                ShowError("السنة الدراسية غير صحيحة.");
                return;
            }

            var academicYear = db.AcademicYears.FirstOrDefault(a => a.Id == academicYearId);
            if (academicYear == null)
            {
                ShowError("السنة الدراسية المختارة غير موجودة.");
                return;
            }

            // ==============================
            // نوع الرسم
            // ==============================
            FeeType feeType;
            if (!Enum.TryParse(ddlType.SelectedValue, true, out feeType))
            {
                ShowError("نوع الرسم غير صحيح.");
                return;
            }

            // ==============================
            // الصف (GradeLevel) والقسم
            // ==============================
            EducationStage stage = EnumHelper.GetCurrentStage();

            int? gradeLevelId = null;
            int? classRoomId = null;

            int selectedGradeLevelId = GetSelectedGradeLevelId();
            if (selectedGradeLevelId > 0)
            {
                var gradeLevel = db.GradeLevels
                    .FirstOrDefault(g => g.Id == selectedGradeLevelId);

                if (gradeLevel == null)
                {
                    ShowError("الصف المختار غير صالح.");
                    return;
                }

                if (gradeLevel.Stage != stage)
                {
                    ShowError("الصف المختار لا ينتمي إلى المرحلة الحالية.");
                    return;
                }

                bool isSecondary = gradeLevel.Name == "ثاني ثانوي"
                                || gradeLevel.Name == "ثالث ثانوي";

                string section = GetSelectedSection();

                if (isSecondary && string.IsNullOrWhiteSpace(section))
                {
                    ShowError("يرجى اختيار القسم للصف الثاني أو الثالث الثانوي.");
                    return;
                }

                // إيجاد الفصل المطابق
                var classRoomQuery = db.ClassRooms
                    .Where(c => c.AcademicYearId == academicYearId
                             && c.GradeLevelId == selectedGradeLevelId);

                ClassRoom classRoom = null;

                if (isSecondary && !string.IsNullOrWhiteSpace(section))
                {
                    string sectionArabic = section == "علمي" ? "علمي" : "أدبي";

                    classRoom = classRoomQuery
                        .FirstOrDefault(c => c.Name.Contains(sectionArabic));

                    if (classRoom == null)
                        classRoom = classRoomQuery.FirstOrDefault();
                }
                else
                {
                    classRoom = classRoomQuery.FirstOrDefault();
                }

                if (classRoom == null)
                {
                    ShowError("لا يوجد فصل مطابق للصف المختار. يرجى مراجعة إعدادات الفصول.");
                    return;
                }

                classRoomId = classRoom.Id;
                gradeLevelId = selectedGradeLevelId;
            }

            // ==============================
            // منع التكرار (نستثني السجل الحالي)
            // ==============================
            bool duplicateExists = db.Fees.Any(f =>
                f.Id != feeId &&
                f.Name == name &&
                f.AcademicYearId == academicYearId &&
                f.Type == feeType &&
                f.ClassRoomId == classRoomId &&
                f.GradeLevelId == gradeLevelId);

            if (duplicateExists)
            {
                ShowError("يوجد بالفعل رسم بنفس الاسم والنوع والسنة الدراسية والصف المحدد.");
                return;
            }

            // ==============================
            // التحديث
            // ==============================
            fee.Name = name;
            fee.Amount = amount;
            fee.Type = feeType;
            fee.AcademicYearId = academicYearId;
            fee.ClassRoomId = classRoomId;
            fee.GradeLevelId = gradeLevelId;
            fee.Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim();

            try
            {
                db.SaveChanges();

                string stageValue = Server.UrlEncode(Session["Stage"] as string ?? "");

                Response.Redirect("Default.aspx?stage=" + stageValue, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception)
            {
                ShowError("حدث خطأ أثناء حفظ التعديلات. يرجى المحاولة مرة أخرى.");
            }
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "text-danger mb-3 d-block";
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}