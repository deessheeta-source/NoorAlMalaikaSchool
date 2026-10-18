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
    public partial class Create : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadDropdowns();
                LoadFeeTypes();
            }
        }

        private void LoadDropdowns()
        {
            string stageValue = Session["Stage"] as string ?? "";
            lblStage.Text = EnumHelper.GetStageArabic(stageValue);

            ddlAcademicYear.DataSource = db.AcademicYears
                .OrderByDescending(a => a.StartDate)
                .ToList();

            ddlAcademicYear.DataTextField = "Name";
            ddlAcademicYear.DataValueField = "Id";
            ddlAcademicYear.DataBind();

            var currentYear = db.AcademicYears.FirstOrDefault(a => a.IsCurrent);

            if (currentYear != null &&
                ddlAcademicYear.Items.FindByValue(currentYear.Id.ToString()) != null)
            {
                ddlAcademicYear.SelectedValue = currentYear.Id.ToString();
            }

            LoadGradeLevelsForYear();
        }

        // =========================================================
        // تحميل الصفوف الدراسية (GradeLevels) حسب السنة والمرحلة:
        // - المرحلة الأساسية: الصف الأول → الصف التاسع
        // - المرحلة الثانوية: أول ثانوي → ثالث ثانوي
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

            // أخفِ القسم بعد إعادة تحميل الصفوف
            ResetSection();
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
        }

        protected void ddlClassRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionVisibility();
        }

        private void LoadFeeTypes()
        {
            ddlType.Items.Clear();

            foreach (FeeType type in Enum.GetValues(typeof(FeeType)))
            {
                ddlType.Items.Add(new ListItem(type.GetDisplayName(), type.ToString()));
            }

            if (ddlType.Items.FindByValue(FeeType.Other.ToString()) != null)
                ddlType.SelectedValue = FeeType.Other.ToString();
        }

        // =========================================================
        // الحفظ
        // =========================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Text = string.Empty;

            // ==============================
            // التحقق من البيانات الأساسية
            // ==============================
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowError("يرجى إدخال اسم الرسم.");
                return;
            }

            if (string.IsNullOrEmpty(ddlAcademicYear.SelectedValue))
            {
                ShowError("يرجى اختيار السنة الدراسية.");
                return;
            }

            if (string.IsNullOrEmpty(ddlType.SelectedValue))
            {
                ShowError("يرجى اختيار نوع الرسم.");
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
            // السنة الدراسية
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
                ShowError("السنة الدراسية المحددة غير موجودة.");
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
            // إنشاء الرسم
            // ==============================
            var fee = new Fee
            {
                Name = txtName.Text.Trim(),
                Amount = amount,
                Type = feeType,
                AcademicYearId = academicYearId,
                Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim()
            };

            // ==============================
            // الصف (GradeLevel) والقسم
            // ==============================
            int gradeLevelId = GetSelectedGradeLevelId();
            if (gradeLevelId > 0)
            {
                var gradeLevel = db.GradeLevels.FirstOrDefault(g => g.Id == gradeLevelId);
                if (gradeLevel == null)
                {
                    ShowError("الصف المختار غير صالح.");
                    return;
                }

                EducationStage stage = EnumHelper.GetCurrentStage();
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

                // ==============================
                // إيجاد الفصل المطابق (GradeLevel + Section) في السنة المحددة
                // ==============================
                var classRoomQuery = db.ClassRooms
                    .Where(c => c.AcademicYearId == academicYearId
                             && c.GradeLevelId == gradeLevelId);

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

                fee.ClassRoomId = classRoom.Id;
                fee.GradeLevelId = gradeLevelId;
            }

            // ==============================
            // منع التكرار
            // ==============================
            bool duplicateExists = db.Fees.Any(f =>
                f.Name == fee.Name &&
                f.AcademicYearId == fee.AcademicYearId &&
                f.ClassRoomId == fee.ClassRoomId &&
                f.GradeLevelId == fee.GradeLevelId &&
                f.Type == fee.Type);

            if (duplicateExists)
            {
                ShowError("يوجد رسم بنفس الاسم والنوع والسنة والصف.");
                return;
            }

            // ==============================
            // الحفظ
            // ==============================
            try
            {
                db.Fees.Add(fee);
                db.SaveChanges();

                string stageValue = Session["Stage"] as string ?? "";

                Response.Redirect(
                    "Default.aspx?stage=" + Server.UrlEncode(stageValue),
                    false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "حدث خطأ أثناء حفظ الرسم: " + Server.HtmlEncode(ex.Message);
                lblMessage.CssClass = "text-danger mb-3 d-block";
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