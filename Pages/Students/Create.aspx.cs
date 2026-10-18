using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Students
{
    public partial class Create : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                PopulateGradeLevels();
                ConfigureSectionVisibility();
                LoadSectionRoomOptions();
            }
        }

        private void PopulateGradeLevels()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();

            var gradeLevels = _context.GradeLevels
                .AsNoTracking()
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlClassRoom.Items.Clear();
            ddlClassRoom.Items.Add(new ListItem("-- اختر الصف --", "0"));

            foreach (var g in gradeLevels)
                ddlClassRoom.Items.Add(new ListItem(g.Name, g.Id.ToString()));
        }

        // استخراج حرف الشعبة من الاسم (حل احتياطي فقط)
        private string ExtractDivisionLetter(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            int idx = classRoomName.IndexOf("شعبة", StringComparison.Ordinal);
            if (idx < 0) return null;

            string after = classRoomName.Substring(idx + "شعبة".Length).Trim();
            if (string.IsNullOrWhiteSpace(after)) return null;

            return after[0].ToString();
        }

        // استخراج القسم من الاسم (حل احتياطي)
        private string ExtractSectionFromName(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            if (classRoomName.Contains("علمي")) return "علمي";
            if (classRoomName.Contains("أدبي")) return "أدبي";

            return null;
        }

        // =========================================================
        // ✅ تحميل الشعب: "شعبة أ" / "شعبة ب" من DivisionName
        // =========================================================
        private void LoadSectionRoomOptions()
        {
            ddlSectionRoom.Items.Clear();

            int gradeLevelId;
            if (!int.TryParse(ddlClassRoom.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
            {
                ddlSectionRoom.Items.Add(new ListItem("-- اختر الصف أولاً --", "0"));
                return;
            }

            string section = ddlSection.SelectedValue;

            var query = _context.ClassRooms
                .AsNoTracking()
                .Where(c => c.GradeLevelId == gradeLevelId);

            if (!string.IsNullOrWhiteSpace(section) && section != "0")
                query = query.Where(c => c.Name.Contains(section) || c.SectionName.Contains(section));

            var rooms = query
                .OrderBy(c => c.DivisionName)
                .ThenBy(c => c.Name)
                .Select(c => new { c.Id, c.Name, c.DivisionName, c.SectionName })
                .ToList();

            var roomInfos = rooms.Select(r => new
            {
                r.Id,
                r.Name,
                Letter = !string.IsNullOrWhiteSpace(r.DivisionName)
                            ? r.DivisionName.Trim()
                            : ExtractDivisionLetter(r.Name),
                Section = !string.IsNullOrWhiteSpace(r.SectionName)
                            ? r.SectionName.Trim()
                            : ExtractSectionFromName(r.Name)
            }).ToList();

            // توليد حرف تسلسلي للقاعات بدون DivisionName
            char nextLetter = 'أ';
            var finalRooms = roomInfos.Select(r =>
            {
                string letter = !string.IsNullOrWhiteSpace(r.Letter)
                    ? r.Letter
                    : (nextLetter++).ToString();

                return new { r.Id, Letter = letter, Section = r.Section };
            }).ToList();

            var duplicateLetters = finalRooms
                .GroupBy(r => r.Letter)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            ddlSectionRoom.Items.Add(new ListItem("-- اختر الشعبة --", "0"));

            var added = new HashSet<string>();

            foreach (var r in finalRooms)
            {
                string displayName = (duplicateLetters.Contains(r.Letter) && !string.IsNullOrWhiteSpace(r.Section))
                    ? "شعبة " + r.Letter + " - " + r.Section
                    : "شعبة " + r.Letter;

                if (added.Contains(displayName)) continue;

                added.Add(displayName);
                ddlSectionRoom.Items.Add(new ListItem(displayName, r.Id.ToString()));
            }
        }

        protected void ddlClassRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionVisibility();
            LoadSectionRoomOptions();
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSectionRoomOptions();
        }

        protected void ddlSectionRoom_SelectedIndexChanged(object sender, EventArgs e) { }

        private bool IsSecondaryGrade(string gradeLevelIdStr)
        {
            int id;
            if (!int.TryParse(gradeLevelIdStr, out id) || id <= 0) return false;

            var grade = _context.GradeLevels.AsNoTracking().FirstOrDefault(g => g.Id == id);
            if (grade == null || string.IsNullOrWhiteSpace(grade.Name)) return false;

            string name = grade.Name.Trim();
            return name == "ثاني ثانوي" || name == "ثالث ثانوي";
        }

        private void ConfigureSectionVisibility()
        {
            bool show = IsSecondaryGrade(ddlClassRoom.SelectedValue);
            pnlSection.Visible = show;
            rfvSection.Enabled = show;
            if (!show) ddlSection.SelectedValue = "0";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // ولي الأمر
                string parentNationalId = txtParentNationalId.Text.Trim();
                int? parentId = null;

                if (!string.IsNullOrEmpty(parentNationalId))
                {
                    var existingParent = _context.Parents.FirstOrDefault(p => p.NationalId == parentNationalId);

                    if (existingParent != null)
                    {
                        parentId = existingParent.Id;
                        existingParent.Name = txtParentName.Text.Trim();
                        existingParent.Phone = txtParentPhone.Text.Trim();
                        existingParent.Address = txtParentAddress.Text.Trim();
                        existingParent.Relation = ddlParentRelation.SelectedValue;
                        _context.SaveChanges();
                    }
                    else
                    {
                        var parent = new Parent
                        {
                            NationalId = parentNationalId,
                            Name = txtParentName.Text.Trim(),
                            Phone = txtParentPhone.Text.Trim(),
                            Address = txtParentAddress.Text.Trim(),
                            Relation = ddlParentRelation.SelectedValue
                        };
                        _context.Parents.Add(parent);
                        _context.SaveChanges();
                        parentId = parent.Id;
                    }
                }

                int gradeLevelId;
                if (!int.TryParse(ddlClassRoom.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
                    throw new InvalidOperationException("يرجى اختيار الصف.");

                var gradeLevel = _context.GradeLevels.FirstOrDefault(g => g.Id == gradeLevelId);
                if (gradeLevel == null)
                    throw new InvalidOperationException("الصف المختار غير صالح.");

                EducationStage currentStage = EnumHelper.GetCurrentStage();
                var classStage = EnumHelper.GetStageForGradeLevel(gradeLevel);

                if (classStage != currentStage)
                    throw new InvalidOperationException("الصف المختار لا ينتمي إلى المرحلة التعليمية الحالية.");

                bool isSecondary = gradeLevel.Name == "ثاني ثانوي" || gradeLevel.Name == "ثالث ثانوي";
                string section = ddlSection.SelectedValue;

                if (isSecondary && (string.IsNullOrWhiteSpace(section) || section == "0"))
                    throw new InvalidOperationException("يرجى اختيار القسم للصف الثاني أو الثالث الثانوي.");

                if (!isSecondary) section = null;

                int sectionRoomId;
                if (!int.TryParse(ddlSectionRoom.SelectedValue, out sectionRoomId) || sectionRoomId <= 0)
                    throw new InvalidOperationException("يرجى اختيار الشعبة.");

                var classRoom = _context.ClassRooms.Include("GradeLevel").FirstOrDefault(c => c.Id == sectionRoomId);
                if (classRoom == null)
                    throw new InvalidOperationException("الشعبة المختارة غير صالحة.");

                if (classRoom.GradeLevelId != gradeLevelId)
                    throw new InvalidOperationException("الشعبة المختارة لا تنتمي إلى الصف المحدد.");

                string firstName = txtFirstName.Text.Trim();
                string fatherName = txtFatherName.Text.Trim();
                string grandFatherName = txtGrandFatherName.Text.Trim();
                string surname = txtSurname.Text.Trim();

                if (fuPhoto.HasFile) FileUploadService.ValidateStudentPhoto(fuPhoto);

                var student = new Student
                {
                    FirstName = firstName,
                    FatherName = fatherName,
                    GrandfatherName = grandFatherName,
                    Nickname = surname,
                    Name = string.Join(" ", new[] { firstName, fatherName, grandFatherName, surname }),
                    NationalId = txtNationalId.Text.Trim(),
                    BirthDate = string.IsNullOrEmpty(txtBirthDate.Text) ? (DateTime?)null : DateTime.Parse(txtBirthDate.Text),
                    Gender = string.IsNullOrEmpty(ddlGender.SelectedValue) ? Gender.Male : (Gender)Enum.Parse(typeof(Gender), ddlGender.SelectedValue),
                    Nationality = txtNationality.Text.Trim(),
                    Stage = classStage,
                    ClassRoomId = classRoom.Id,
                    Section = section,
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    MotherName = txtMotherName.Text.Trim(),
                    MotherPhone = txtMotherPhone.Text.Trim(),
                    Notes = txtNotes.Text.Trim(),
                    RegistrationNo = string.IsNullOrWhiteSpace(txtRegistrationNo.Text) ? null : txtRegistrationNo.Text.Trim(),
                    ParentId = parentId,
                    IsActive = true,
                    EnrollmentDate = DateTime.Now
                };

                _context.Students.Add(student);
                _context.SaveChanges();

                ProcessPhoto(student);

                lblMessage.CssClass = "create-alert alert-success";
                lblMessage.Text = "تم إضافة الطالب بنجاح!";
                Response.Redirect("Default.aspx?stage=" + EnumHelper.GetCurrentStage());
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "create-alert alert-danger";
                lblMessage.Text = ErrorMessageHelper.GetUserMessage(ex, GetType().FullName);
            }
        }

        private void ProcessPhoto(Student student)
        {
            if (!fuPhoto.HasFile) return;

            string newFileName;
            try
            {
                newFileName = SaveStudentPhoto(student.Id, fuPhoto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Photo upload failed: " + ex.Message);
                return;
            }

            if (string.IsNullOrEmpty(newFileName)) return;

            try
            {
                student.PhotoPath = newFileName;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to update PhotoPath: " + ex.Message);
                TryDeletePhotoFile(newFileName);
            }
        }

        private string SaveStudentPhoto(int studentId, FileUpload upload)
        {
            string folderPath = Server.MapPath("~/Content/Uploads/Students");
            return FileUploadService.SaveStudentPhoto(upload, folderPath);
        }

        private void TryDeletePhotoFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            try
            {
                string folderPath = Server.MapPath("~/Content/Uploads/Students");
                FileUploadService.DeleteStudentPhoto(fileName, folderPath);
            }
            catch { }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?stage=" + EnumHelper.GetCurrentStage());
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_context != null) _context.Dispose();
            base.OnUnload(e);
        }
    }
}