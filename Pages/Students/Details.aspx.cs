using System;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Students
{
    public partial class Details : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();
        private string _stage = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            string stageParam = Request.QueryString["stage"];
            if (!string.IsNullOrEmpty(stageParam))
                _stage = stageParam;
            else if (Session["Stage"] != null)
                _stage = Session["Stage"].ToString();

            if (!IsPostBack)
            {
                string idStr = Request.QueryString["id"];
                int id;
                if (!string.IsNullOrEmpty(idStr) && int.TryParse(idStr, out id))
                    LoadStudent(id);
                else
                {
                    pnlDetails.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
        }

        private static string GetRelationArabic(string relation)
        {
            switch (relation)
            {
                case "Father": return "أب";
                case "Mother": return "أم";
                case "Guardian": return "وصي";
                case "Other": return "أخرى";
                default: return string.IsNullOrEmpty(relation) ? null : relation;
            }
        }

        private void SetValue(System.Web.UI.WebControls.Label lbl, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                lbl.Text = "غير محدد";
                lbl.CssClass = "info-item-value empty";
            }
            else
            {
                lbl.Text = Server.HtmlEncode(value);
                lbl.CssClass = "info-item-value";
            }
        }

        private void SetSectionBadge(System.Web.UI.WebControls.Label lbl, string section)
        {
            if (string.IsNullOrWhiteSpace(section))
            {
                lbl.Text = "غير محدد";
                lbl.CssClass = "info-item-value empty";
                return;
            }

            string display;
            string cls = "section-badge";
            if (section == "علمي") { display = "القسم العلمي"; cls += " sci"; }
            else if (section == "أدبي") { display = "القسم الأدبي"; cls += " lit"; }
            else display = section;

            lbl.Text = "<span class=\"" + cls + "\">" + Server.HtmlEncode(display) + "</span>";
            lbl.CssClass = "info-item-value";
        }

        private string ExtractSectionRoomName(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            classRoomName = classRoomName.Trim();

            int idx = classRoomName.IndexOf("شعبة", StringComparison.Ordinal);
            if (idx < 0) return null;

            return classRoomName.Substring(idx).Trim();
        }

        // =========================================================
        // ✅ تحميل صورة الطالب مع Fallback
        // =========================================================
        private void LoadStudentPhoto(string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
            {
                pnlStudentPhoto.Visible = false;
                pnlStudentPhotoPlaceholder.Visible = true;
                return;
            }

            try
            {
                string photoUrl = photoPath.Trim();

                // إذا كان المسار لا يبدأ بـ ~ أو / → أضف البادئة
                if (!photoUrl.StartsWith("~") && !photoUrl.StartsWith("/"))
                    photoUrl = "~/" + photoUrl.TrimStart('/');

                // تحقق من وجود الملف على القرص
                string physicalPath = Server.MapPath(photoUrl);

                System.Diagnostics.Debug.WriteLine(
                    "LoadStudentPhoto: " + photoPath + " → " + physicalPath + " → " +
                    (System.IO.File.Exists(physicalPath) ? "EXISTS" : "MISSING"));

                if (!string.IsNullOrEmpty(physicalPath) && System.IO.File.Exists(physicalPath))
                {
                    imgStudent.ImageUrl = ResolveUrl(photoUrl);
                    pnlStudentPhoto.Visible = true;
                    pnlStudentPhotoPlaceholder.Visible = false;
                }
                else
                {
                    pnlStudentPhoto.Visible = false;
                    pnlStudentPhotoPlaceholder.Visible = true;
                }
            }
            catch
            {
                pnlStudentPhoto.Visible = false;
                pnlStudentPhotoPlaceholder.Visible = true;
            }
        }

        // =========================================================
        // تحميل بيانات الطالب
        // =========================================================
        private void LoadStudent(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null)
            {
                pnlDetails.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            EducationStage stageEnum;
            if (!string.IsNullOrEmpty(_stage) && Enum.TryParse<EducationStage>(_stage, true, out stageEnum))
            {
                if (student.Stage != stageEnum)
                {
                    pnlDetails.Visible = false;
                    pnlNotFound.Visible = true;
                    return;
                }
            }

            hfStudentId.Value = student.Id.ToString();

            // ===== Hero =====
            string fullName = (student.Name ?? "").Trim();
            lblFullName.Text = Server.HtmlEncode(fullName);

            if (student.ClassRoom != null)
            {
                if (student.ClassRoom.GradeLevel != null)
                    lblHeroGradeLevel.Text = Server.HtmlEncode(student.ClassRoom.GradeLevel.Name);
                else
                    lblHeroGradeLevel.Text = Server.HtmlEncode(student.ClassRoom.Name ?? "—");
            }
            else
            {
                lblHeroGradeLevel.Text = "—";
            }

            lblHeroRegistrationNo.Text = Server.HtmlEncode(
                string.IsNullOrWhiteSpace(student.RegistrationNo) ? "—" : student.RegistrationNo);

            // ===== بيانات الطالب =====
            SetValue(lblNationalId2, student.NationalId);
            SetValue(lblRegistrationNo2, student.RegistrationNo);

            SetValue(lblBirthDate, student.BirthDate.HasValue
                ? student.BirthDate.Value.ToString("dd/MM/yyyy") : null);

            SetValue(lblGender, student.Gender == Gender.Male ? "ذكر" : "أنثى");
            SetValue(lblNationality, student.Nationality);

            SetValue(lblPhone, student.Phone);
            SetValue(lblAddress, student.Address);

            if (student.ClassRoom != null)
            {
                if (student.ClassRoom.GradeLevel != null)
                    SetValue(lblGradeLevel, student.ClassRoom.GradeLevel.Name);
                else
                    SetValue(lblGradeLevel, student.ClassRoom.Name);
            }
            else
            {
                SetValue(lblGradeLevel, null);
            }

            SetSectionBadge(lblSection, student.Section);

            if (student.ClassRoom != null)
            {
                string sectionRoom = ExtractSectionRoomName(student.ClassRoom.Name);
                SetValue(lblSectionRoom, sectionRoom);
            }
            else
            {
                SetValue(lblSectionRoom, null);
            }

            //SetValue(lblStage, EnumHelper.GetDisplayName(student.Stage));

            // ===== الحالة =====
            lblStatus.Text = student.IsActive
                ? "<i class=\"fas fa-check-circle\"></i> نشط"
                : "<i class=\"fas fa-clock\"></i> غير نشط";
            lblStatus.CssClass = student.IsActive
                ? "status-badge active"
                : "status-badge inactive";

            // ===== ✅ الصورة =====
            LoadStudentPhoto(student.PhotoPath);

            // ===== بيانات الأم =====
            SetValue(lblMotherName, student.MotherName);
            SetValue(lblMotherPhone, student.MotherPhone);

            // ===== ولي الأمر =====
            pnlParent.Visible = true;

            if (student.ParentId.HasValue)
            {
                var parent = _context.Parents.Find(student.ParentId.Value);
                if (parent != null)
                {
                    SetValue(lblParentNationalId, parent.NationalId);
                    SetValue(lblParentName, parent.Name);
                    SetValue(lblParentPhone, parent.Phone);
                    SetValue(lblParentRelation, GetRelationArabic(parent.Relation));
                    SetValue(lblParentAddress, parent.Address);
                }
                else
                {
                    SetValue(lblParentNationalId, null);
                    SetValue(lblParentName, null);
                    SetValue(lblParentPhone, null);
                    SetValue(lblParentRelation, null);
                    SetValue(lblParentAddress, null);
                }
            }
            else
            {
                SetValue(lblParentNationalId, null);
                SetValue(lblParentName, null);
                SetValue(lblParentPhone, null);
                SetValue(lblParentRelation, null);
                SetValue(lblParentAddress, null);
            }

            // ===== ملاحظات =====
            if (!string.IsNullOrEmpty(student.Notes))
            {
                pnlNotes.Visible = true;
                lblNotes.Text = Server.HtmlEncode(student.Notes);
            }
        }

        protected void btnBackToList_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?stage=" + _stage);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_context != null) _context.Dispose();
            base.OnUnload(e);
        }
    }
}