using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NoorAlMalaikaSchool.Pages.Students
{
    public partial class Edit : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();
        private string _oldPhotoPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                string idStr = Request.QueryString["id"];
                int id;

                if (!string.IsNullOrEmpty(idStr) && int.TryParse(idStr, out id))
                    LoadStudent(id);
                else
                {
                    pnlForm.Visible = false;
                    pnlNotFound.Visible = true;
                }
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

        private string ExtractDivisionLetter(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            int idx = classRoomName.IndexOf("شعبة", StringComparison.Ordinal);
            if (idx < 0) return null;

            string after = classRoomName.Substring(idx + "شعبة".Length).Trim();
            if (string.IsNullOrWhiteSpace(after)) return null;

            return after[0].ToString();
        }

        private string ExtractSectionFromName(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            if (classRoomName.Contains("علمي")) return "علمي";
            if (classRoomName.Contains("أدبي")) return "أدبي";

            return null;
        }

        // =========================================================
        // ✅ تحميل الشعب: "شعبة أ" / "شعبة ب"
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

        private void LoadStudent(int id)
        {
            var student = _context.Students
                .Include("ClassRoom.GradeLevel")
                .FirstOrDefault(s => s.Id == id);

            if (student == null || student.Stage != EnumHelper.GetCurrentStage())
            {
                pnlForm.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            pnlForm.Visible = true;
            pnlNotFound.Visible = false;
            hfStudentId.Value = student.Id.ToString();

            txtFirstName.Text = student.FirstName;
            txtFatherName.Text = student.FatherName;
            txtGrandFatherName.Text = student.GrandfatherName;
            txtSurname.Text = student.Nickname;

            if (string.IsNullOrWhiteSpace(txtFirstName.Text)
                && string.IsNullOrWhiteSpace(txtFatherName.Text)
                && string.IsNullOrWhiteSpace(txtGrandFatherName.Text)
                && string.IsNullOrWhiteSpace(txtSurname.Text)
                && !string.IsNullOrWhiteSpace(student.Name))
            {
                string[] nameParts = student.Name.Trim()
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (nameParts.Length > 0) txtFirstName.Text = nameParts[0];
                if (nameParts.Length > 1) txtFatherName.Text = nameParts[1];
                if (nameParts.Length > 2) txtGrandFatherName.Text = nameParts[2];
                if (nameParts.Length > 3) txtSurname.Text = string.Join(" ", nameParts.Skip(3));
            }

            txtNationalId.Text = student.NationalId;
            txtRegistrationNo.Text = student.RegistrationNo;
            txtBirthDate.Text = student.BirthDate.HasValue ? student.BirthDate.Value.ToString("yyyy-MM-dd") : "";

            if (ddlGender.Items.FindByValue(student.Gender.ToString()) != null)
                ddlGender.SelectedValue = student.Gender.ToString();

            txtNationality.Text = student.Nationality;
            txtPhone.Text = student.Phone;
            txtAddress.Text = student.Address;
            txtMotherName.Text = student.MotherName;
            txtMotherPhone.Text = student.MotherPhone;
            txtNotes.Text = student.Notes;

            _oldPhotoPath = student.PhotoPath;
            DisplayCurrentPhoto(student.PhotoPath);

            PopulateGradeLevels();

            int? gradeLevelId = student.ClassRoom != null ? (int?)student.ClassRoom.GradeLevelId : null;
            if (gradeLevelId.HasValue && ddlClassRoom.Items.FindByValue(gradeLevelId.Value.ToString()) != null)
                ddlClassRoom.SelectedValue = gradeLevelId.Value.ToString();

            ConfigureSectionVisibility();

            if (IsSecondaryGrade(ddlClassRoom.SelectedValue) && !string.IsNullOrWhiteSpace(student.Section))
            {
                if (ddlSection.Items.FindByValue(student.Section) != null)
                    ddlSection.SelectedValue = student.Section;
            }

            LoadSectionRoomOptions();

            if (student.ClassRoomId > 0 && ddlSectionRoom.Items.FindByValue(student.ClassRoomId.ToString()) != null)
                ddlSectionRoom.SelectedValue = student.ClassRoomId.ToString();

            if (student.ParentId.HasValue)
            {
                var parent = _context.Parents.Find(student.ParentId.Value);
                if (parent != null)
                {
                    txtParentNationalId.Text = parent.NationalId;
                    txtParentName.Text = parent.Name;
                    txtParentPhone.Text = parent.Phone;
                    txtParentAddress.Text = parent.Address;

                    if (ddlParentRelation.Items.FindByValue(parent.Relation ?? "") != null)
                        ddlParentRelation.SelectedValue = parent.Relation ?? "";
                }
            }
        }

        private void DisplayCurrentPhoto(string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
            {
                imgCurrentPhoto.Visible = false;
                divCurrentPhoto.Visible = false;
                return;
            }

            try
            {
                string photoUrl = photoPath.Trim();
                if (!photoUrl.StartsWith("~") && !photoUrl.StartsWith("/"))
                    photoUrl = "~/" + photoUrl.TrimStart('/');

                string physicalPath = Server.MapPath(photoUrl);

                if (!string.IsNullOrEmpty(physicalPath) && File.Exists(physicalPath))
                {
                    imgCurrentPhoto.ImageUrl = ResolveUrl(photoUrl);
                    imgCurrentPhoto.Visible = true;
                    divCurrentPhoto.Visible = true;
                }
                else
                {
                    imgCurrentPhoto.Visible = false;
                    divCurrentPhoto.Visible = false;
                }
            }
            catch
            {
                imgCurrentPhoto.Visible = false;
                divCurrentPhoto.Visible = false;
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
                int studentId;
                if (!int.TryParse(hfStudentId.Value, out studentId) || studentId <= 0)
                    throw new InvalidOperationException("رقم الطالب غير صالح.");

                var student = _context.Students.Find(studentId);
                if (student == null)
                {
                    lblMessage.CssClass = "edit-alert alert-danger";
                    lblMessage.Text = "الطالب غير موجود!";
                    return;
                }

                string parentNationalId = txtParentNationalId.Text.Trim();

                if (!string.IsNullOrEmpty(parentNationalId))
                {
                    var existingParent = _context.Parents.FirstOrDefault(p => p.NationalId == parentNationalId);

                    if (existingParent != null)
                    {
                        student.ParentId = existingParent.Id;
                        existingParent.Name = txtParentName.Text.Trim();
                        existingParent.Phone = txtParentPhone.Text.Trim();
                        existingParent.Address = txtParentAddress.Text.Trim();
                        existingParent.Relation = ddlParentRelation.SelectedValue;
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
                        student.ParentId = parent.Id;
                    }
                }
                else
                {
                    student.ParentId = null;
                }

                string firstName = txtFirstName.Text.Trim();
                string fatherName = txtFatherName.Text.Trim();
                string grandFatherName = txtGrandFatherName.Text.Trim();
                string surname = txtSurname.Text.Trim();

                student.FirstName = firstName;
                student.FatherName = fatherName;
                student.GrandfatherName = grandFatherName;
                student.Nickname = surname;
                student.Name = string.Join(" ", new[] { firstName, fatherName, grandFatherName, surname });

                student.NationalId = txtNationalId.Text.Trim();
                student.RegistrationNo = string.IsNullOrWhiteSpace(txtRegistrationNo.Text) ? null : txtRegistrationNo.Text.Trim();
                student.BirthDate = string.IsNullOrEmpty(txtBirthDate.Text) ? (DateTime?)null : DateTime.Parse(txtBirthDate.Text);
                student.Gender = string.IsNullOrEmpty(ddlGender.SelectedValue) ? Gender.Male : (Gender)Enum.Parse(typeof(Gender), ddlGender.SelectedValue);
                student.Nationality = txtNationality.Text.Trim();

                int gradeLevelId;
                if (!int.TryParse(ddlClassRoom.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
                    throw new InvalidOperationException("يرجى اختيار الصف.");

                var gradeLevel = _context.GradeLevels.FirstOrDefault(g => g.Id == gradeLevelId);
                if (gradeLevel == null)
                    throw new InvalidOperationException("الصف المختار غير صالح.");

                var classStage = EnumHelper.GetStageForGradeLevel(gradeLevel);
                if (classStage != EnumHelper.GetCurrentStage())
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

                student.ClassRoomId = classRoom.Id;
                student.Section = section;
                student.Stage = classStage;
                student.Phone = txtPhone.Text.Trim();
                student.Address = txtAddress.Text.Trim();
                student.MotherName = txtMotherName.Text.Trim();
                student.MotherPhone = txtMotherPhone.Text.Trim();
                student.Notes = txtNotes.Text.Trim();

                _context.SaveChanges();

                ProcessPhoto(student);

                Response.Redirect("Default.aspx?stage=" + EnumHelper.GetCurrentStage());
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "edit-alert alert-danger";
                lblMessage.Text = ErrorMessageHelper.GetUserMessage(ex, GetType().FullName);
            }
        }

        private void ProcessPhoto(Student student)
        {
            if (fuPhoto.HasFile)
            {
                FileUploadService.ValidateStudentPhoto(fuPhoto);

                string newFileName;
                try
                {
                    newFileName = SaveStudentPhoto(student.Id, fuPhoto);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("تعذر رفع الصورة: " + ex.Message, ex);
                }

                if (string.IsNullOrEmpty(newFileName))
                    throw new InvalidOperationException("تعذر حفظ ملف الصورة.");

                string oldPhotoPath = student.PhotoPath;
                student.PhotoPath = newFileName;
                _context.SaveChanges();

                if (!string.IsNullOrEmpty(oldPhotoPath) && !string.Equals(oldPhotoPath, newFileName, StringComparison.OrdinalIgnoreCase))
                    DeletePhotoByPath(oldPhotoPath);
            }
            else if (chkRemovePhoto.Checked && !string.IsNullOrEmpty(student.PhotoPath))
            {
                string oldPhotoPath = student.PhotoPath;
                student.PhotoPath = null;
                _context.SaveChanges();
                DeletePhotoByPath(oldPhotoPath);
            }
        }

        private string SaveStudentPhoto(int studentId, FileUpload upload)
        {
            string folderPath = Server.MapPath("~/Content/Uploads/Students");
            return FileUploadService.SaveStudentPhoto(upload, folderPath);
        }

        private string GetPhotoAbsolutePath(string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath)) return null;
            string folderPath = Server.MapPath("~/Content/Uploads/Students");
            return FileUploadService.GetStudentPhotoPhysicalPath(photoPath, folderPath);
        }

        private void DeletePhotoByPath(string dbPhotoPath)
        {
            if (string.IsNullOrWhiteSpace(dbPhotoPath)) return;

            try
            {
                string absolutePath = GetPhotoAbsolutePath(dbPhotoPath);

                if (!string.IsNullOrEmpty(absolutePath) && File.Exists(absolutePath))
                {
                    string folderPath = Server.MapPath("~/Content/Uploads/Students");
                    string fileName = Path.GetFileName(absolutePath);

                    if (!string.IsNullOrEmpty(fileName))
                        FileUploadService.DeleteStudentPhoto(fileName, folderPath);
                }
            }
            catch { }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?stage=" + EnumHelper.GetCurrentStage());
        }

        protected void btnBackToList_Click(object sender, EventArgs e)
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