using System;
using System.Globalization;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Attendance
{
    public partial class Take : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();
        private DateTime? _editDate;

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadGradeLevels();
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

                // دعم legacy: تمرير class=classRoomId
                if (!string.IsNullOrEmpty(Request.QueryString["class"]))
                {
                    int legacyClassRoomId;
                    if (int.TryParse(Request.QueryString["class"], out legacyClassRoomId)
                        && legacyClassRoomId > 0)
                    {
                        var cr = db.ClassRooms
                            .AsNoTracking()
                            .FirstOrDefault(c => c.Id == legacyClassRoomId);

                        if (cr != null)
                        {
                            if (ddlGradeLevel.Items.FindByValue(cr.GradeLevelId.ToString()) != null)
                                ddlGradeLevel.SelectedValue = cr.GradeLevelId.ToString();

                            ConfigureSectionFilter();

                            if (!string.IsNullOrEmpty(cr.SectionName)
                                && ddlSection.Items.FindByValue(cr.SectionName) != null)
                                ddlSection.SelectedValue = cr.SectionName;

                            if (!string.IsNullOrEmpty(cr.DivisionName)
                                && ddlDivision.Items.FindByValue(cr.DivisionName) != null)
                                ddlDivision.SelectedValue = cr.DivisionName;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Request.QueryString["date"]))
                {
                    txtDate.Text = Request.QueryString["date"];
                }

                ConfigureSectionFilter();

                if (GetSelectedGradeLevelId() > 0)
                {
                    LoadStudents();
                }
            }
        }

        // =========================================================
        // دوال مساعدة
        // =========================================================
        private int GetSelectedGradeLevelId()
        {
            int id;
            if (!int.TryParse(ddlGradeLevel.SelectedValue, out id)) return 0;
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

        private void ConfigureSectionFilter()
        {
            int gradeLevelId = GetSelectedGradeLevelId();
            bool showSection = IsSecondaryGrade(gradeLevelId);
            pnlSectionFilter.Visible = showSection;

            if (!showSection)
                SelectSection("");
        }

        private void SelectSection(string value)
        {
            if (ddlSection == null) return;
            if (value == null) value = "";

            ListItem item = ddlSection.Items.FindByValue(value);
            if (item != null)
            {
                ddlSection.ClearSelection();
                item.Selected = true;
            }
            else if (ddlSection.Items.Count > 0)
            {
                ddlSection.SelectedIndex = 0;
            }
        }

        private string GetSelectedSection()
        {
            if (ddlSection == null) return null;
            string value = ddlSection.SelectedValue;
            if (string.IsNullOrWhiteSpace(value)) return null;
            return value;
        }

        private string GetSelectedDivision()
        {
            if (ddlDivision == null) return null;
            string value = ddlDivision.SelectedValue;
            if (string.IsNullOrWhiteSpace(value)) return null;
            return value;
        }

        // =========================================================
        // تحميل الصفوف
        // =========================================================
        private void LoadGradeLevels()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();

            var gradeLevels = db.GradeLevels
                .AsNoTracking()
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlGradeLevel.Items.Clear();
            ddlGradeLevel.Items.Add(new ListItem("-- اختر الصف --", ""));

            foreach (var g in gradeLevels)
                ddlGradeLevel.Items.Add(new ListItem(g.Name, g.Id.ToString()));
        }

        // =========================================================
        // تحميل الطلاب حسب (الصف + القسم + الشعبة)
        // =========================================================
        private void LoadStudents()
        {
            int gradeLevelId = GetSelectedGradeLevelId();
            if (gradeLevelId <= 0)
            {
                pnlStudents.Visible = false;
                pnlEmpty.Visible = true;
                lblCountPill.Text = "0";
                return;
            }

            EducationStage stage = EnumHelper.GetCurrentStage();
            string section = GetSelectedSection();
            string division = GetSelectedDivision();

            var query = db.Students
                .Include("ClassRoom.GradeLevel")
                .Where(s => s.Stage == stage
                         && s.ClassRoom != null
                         && s.ClassRoom.GradeLevelId == gradeLevelId);

            if (!string.IsNullOrWhiteSpace(section))
                query = query.Where(s => s.ClassRoom.SectionName == section);
            else
                query = query.Where(s => s.ClassRoom.SectionName == null
                                      || s.ClassRoom.SectionName == "");

            if (!string.IsNullOrWhiteSpace(division))
                query = query.Where(s => s.ClassRoom.DivisionName == division);
            else
                query = query.Where(s => s.ClassRoom.DivisionName == null
                                      || s.ClassRoom.DivisionName == "");

            var students = query.OrderBy(s => s.Name).ToList();

            DateTime d;
            _editDate = DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out d) ? d : (DateTime?)null;

            gvStudents.DataSource = students;
            gvStudents.DataBind();

            // ✅ إظهار الجدول أو رسالة "لا يوجد طلاب"
            pnlStudents.Visible = students.Any();
            pnlEmpty.Visible = !students.Any();

            // ✅ تحديث العدّاد
            lblCountPill.Text = students.Count.ToString();
        }

        // =========================================================
        // RowDataBound — تحميل الحضور السابق
        // =========================================================
        protected void gvStudents_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = (e.Row.CssClass + " take-table-header-row").Trim();
        }

        protected void gvStudents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            string[] mobileLabels = { "رقم", "اسم الطالب", "الحضور", "ملاحظات" };
            for (int i = 0; i < e.Row.Cells.Count && i < mobileLabels.Length; i++)
                e.Row.Cells[i].Attributes["data-label"] = mobileLabels[i];

            var student = e.Row.DataItem as Models.Student;
            if (student == null) return;

            CheckBox chk = (CheckBox)e.Row.FindControl("chkPresent");
            TextBox txt = (TextBox)e.Row.FindControl("txtNotes");
            if (chk == null) return;

            if (_editDate.HasValue)
            {
                DateTime cutOff = _editDate.Value.Date;
                var att = db.Attendances
                    .FirstOrDefault(a => a.StudentId == student.Id
                        && DbFunctions.TruncateTime(a.Date) == cutOff);
                if (att != null)
                {
                    chk.Checked = att.IsPresent;
                    if (txt != null) txt.Text = att.Notes ?? "";
                }
            }
        }

        // =========================================================
        // الأحداث
        // =========================================================
        protected void ddlGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();

            if (GetSelectedGradeLevelId() > 0)
                LoadStudents();
            else
            {
                pnlStudents.Visible = false;
                pnlEmpty.Visible = true;
                lblCountPill.Text = "0";
            }
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GetSelectedGradeLevelId() > 0)
                LoadStudents();
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GetSelectedGradeLevelId() > 0)
                LoadStudents();
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            ConfigureSectionFilter();

            if (GetSelectedGradeLevelId() > 0)
                LoadStudents();
            else
            {
                pnlStudents.Visible = false;
                pnlEmpty.Visible = true;
                lblCountPill.Text = "0";
            }
        }

        // =========================================================
        // الحفظ
        // =========================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (GetSelectedGradeLevelId() <= 0 || string.IsNullOrEmpty(txtDate.Text))
            {
                lblMessage.Text = "<i class='fas fa-exclamation-triangle'></i> يرجى اختيار الصف والتاريخ";
                lblMessage.CssClass = "take-validation text-danger";
                return;
            }

            DateTime attendanceDate;
            if (!DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out attendanceDate))
            {
                lblMessage.Text = "<i class='fas fa-exclamation-triangle'></i> التاريخ غير صحيح";
                lblMessage.CssClass = "take-validation text-danger";
                return;
            }

            for (int i = 0; i < gvStudents.Rows.Count; i++)
            {
                GridViewRow row = gvStudents.Rows[i];
                CheckBox chkPresent = (CheckBox)row.FindControl("chkPresent");
                TextBox txtNotes = (TextBox)row.FindControl("txtNotes");

                int studentId = Convert.ToInt32(gvStudents.DataKeys[i].Value);
                bool isPresent = chkPresent != null && chkPresent.Checked;
                string notes = txtNotes != null ? txtNotes.Text.Trim() : "";

                var existing = db.Attendances
                    .FirstOrDefault(a => a.StudentId == studentId
                        && DbFunctions.TruncateTime(a.Date) == attendanceDate.Date);

                if (existing != null)
                {
                    existing.IsPresent = isPresent;
                    existing.Notes = notes;
                }
                else
                {
                    var attendance = new Models.Attendance
                    {
                        StudentId = studentId,
                        Date = attendanceDate,
                        IsPresent = isPresent,
                        Notes = notes
                    };
                    db.Attendances.Add(attendance);
                }
            }

            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                lblMessage.Text = "<i class='fas fa-exclamation-triangle'></i> تعذر الحفظ لأن بعض السجلات أُنشئت للتو بواسطة محاولة أخرى. أعد المحاولة.";
                lblMessage.CssClass = "take-validation text-danger";
                return;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "<i class='fas fa-exclamation-triangle'></i> تعذر الحفظ: " + Server.HtmlEncode(ex.Message);
                lblMessage.CssClass = "take-validation text-danger";
                return;
            }

            Response.Clear();
            Response.ClearHeaders();
            Response.Redirect(ResolveUrl("~/Pages/Attendance/Default.aspx"), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}
