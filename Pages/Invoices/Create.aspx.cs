using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Data.Services;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Invoices
{
    public partial class Create : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        // ✅ NEW: مدخلات الخصومات اليدوية
        private sealed class ManualDiscountInput
        {
            public DiscountPolicy Policy { get; set; }
            public string TextValue { get; set; }
        }

        // =========================================================
        // رابط الرجوع
        // =========================================================
        protected string ReturnUrl
        {
            get
            {
                string stage = Session["Stage"] as string ?? "Basic";
                string stageEnc = HttpUtility.UrlEncode(stage);
                string studentParam = Request.QueryString["studentId"];

                if (!string.IsNullOrEmpty(studentParam))
                {
                    return ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                         + "?studentId=" + Server.HtmlEncode(studentParam)
                         + "&stage=" + stageEnc;
                }

                return ResolveClientUrl("~/Pages/Invoices/Default.aspx")
                     + "?stage=" + stageEnc;
            }
        }

        // =========================================================
        // Page Load
        // =========================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            try
            {
                if (!IsPostBack)
                {
                    LoadGradeLevels();
                    LoadDivisions();
                    txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

                    ApplyQueryStringSelection();
                }
            }
            catch (Exception ex)
            {
                ShowDetailedError("حدث خطأ أثناء تحميل صفحة إنشاء الفاتورة", ex);
            }
        }

        // =========================================================
        // تحميل الصفوف الدراسية
        // =========================================================
        private void LoadGradeLevels()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();

            var gradeLevels = db.GradeLevels
                .AsNoTracking()
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlClass.Items.Clear();
            ddlClass.Items.Add(new ListItem("-- اختر الصف --", ""));

            foreach (var gl in gradeLevels)
            {
                ddlClass.Items.Add(new ListItem(gl.Name, gl.Id.ToString()));
            }
        }

        // =========================================================
        // استخراج حرف الشعبة
        // =========================================================
        private string ExtractDivisionLetter(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            int idx = classRoomName.IndexOf("شعبة", StringComparison.Ordinal);
            if (idx < 0) return null;

            string after = classRoomName.Substring(idx + "شعبة".Length).Trim();
            if (string.IsNullOrWhiteSpace(after)) return null;

            return after[0].ToString();
        }

        // =========================================================
        // استخراج القسم
        // =========================================================
        private string ExtractSectionFromName(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            if (classRoomName.Contains("علمي")) return "علمي";
            if (classRoomName.Contains("أدبي")) return "أدبي";

            return null;
        }

        // =========================================================
        // تحميل الشعب
        // =========================================================
        private void LoadDivisions()
        {
            if (ddlDivision == null) return;

            string previousValue = ddlDivision.SelectedValue;

            ddlDivision.Items.Clear();
            ddlDivision.Items.Add(new ListItem("جميع الشعب", ""));

            int gradeLevelId = GetSelectedGradeLevelId();
            if (gradeLevelId <= 0)
                return;

            string selectedSection = GetSelectedSection();

            IQueryable<ClassRoom> query = db.ClassRooms
                .AsNoTracking()
                .Where(c => c.GradeLevelId == gradeLevelId);

            if (!string.IsNullOrWhiteSpace(selectedSection))
            {
                query = query.Where(c => c.SectionName == selectedSection ||
                                          c.Name.Contains(selectedSection));
            }

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

            var added = new HashSet<string>();

            foreach (var r in finalRooms)
            {
                string displayName = (duplicateLetters.Contains(r.Letter)
                                      && !string.IsNullOrWhiteSpace(r.Section))
                    ? "شعبة " + r.Letter + " - " + r.Section
                    : "شعبة " + r.Letter;

                if (added.Contains(displayName)) continue;

                added.Add(displayName);
                ddlDivision.Items.Add(new ListItem(displayName, r.Letter));
            }

            if (!string.IsNullOrWhiteSpace(previousValue))
            {
                ListItem prev = ddlDivision.Items.FindByValue(previousValue);
                if (prev != null)
                {
                    ddlDivision.ClearSelection();
                    prev.Selected = true;
                }
            }
        }

        // =========================================================
        // تحديد الطالب من Query String
        // =========================================================
        private void ApplyQueryStringSelection()
        {
            int studentId;
            if (!int.TryParse(Request.QueryString["studentId"], out studentId)
                || studentId <= 0)
            {
                return;
            }

            var student = db.Students
                .Include(s => s.ClassRoom)
                .Include(s => s.ClassRoom.GradeLevel)
                .Include(s => s.Parent)
                .FirstOrDefault(s => s.Id == studentId && s.IsActive);

            if (student == null || student.ClassRoom == null)
            {
                return;
            }

            EducationStage stage = EnumHelper.GetCurrentStage();
            if (student.Stage != stage)
            {
                return;
            }

            int gradeLevelId = student.ClassRoom.GradeLevelId;
            if (ddlClass.Items.FindByValue(gradeLevelId.ToString()) != null)
            {
                ddlClass.SelectedValue = gradeLevelId.ToString();
            }

            ConfigureSectionVisibility();

            if (IsSecondaryGrade(gradeLevelId)
                && !string.IsNullOrWhiteSpace(student.ClassRoom.SectionName))
            {
                SelectSection(student.ClassRoom.SectionName);
            }

            LoadDivisions();

            if (!string.IsNullOrWhiteSpace(student.ClassRoom.DivisionName))
            {
                SelectDivision(student.ClassRoom.DivisionName);
            }

            LoadStudents();

            if (ddlStudent.Items.FindByValue(studentId.ToString()) != null)
            {
                ddlStudent.SelectedValue = studentId.ToString();
            }

            BuildForSelectedStudent(student);
        }

        // =========================================================
        // بناء الفاتورة
        // =========================================================
        private void BuildForSelectedStudent(Student student)
        {
            pnlBuild.Visible = false;
            pnlSummary.Visible = false;

            lblStudentInfo.Text = "";
            lblMessage.Text = "";

            // ✅ NEW: تصفير الحقول اليدوية
            txtSiblingDiscount.Text = "";
            txtEarlyBirdDiscount.Text = "";
            txtAnnualSubscriptionDiscount.Text = "";

            if (student.ClassRoom != null)
            {
                db.Entry(student.ClassRoom)
                    .Reference(c => c.AcademicYear)
                    .Load();
            }

            lblStudentInfo.Text =
                "الطالب: " + student.Name +
                (student.ClassRoom != null ? " — الصف: " + student.ClassRoom.Name : "") +
                (student.Parent != null ? " — ولي الأمر: " + student.Parent.Name : "");

            try { LoadFeeLines(student); }
            catch (Exception ex)
            {
                ShowDetailedError("خطأ أثناء تحميل رسوم الطالب", ex);
                return;
            }

            try { LoadDiscountPolicies(student); }
            catch (Exception ex)
            {
                ShowDetailedError("خطأ أثناء تحميل سياسات الخصم واستحقاقات الطالب", ex);
                return;
            }

            pnlBuild.Visible = true;
        }

        // =========================================================
        // دوال مساعدة
        // =========================================================
        private int GetSelectedGradeLevelId()
        {
            int id;
            if (!int.TryParse(ddlClass.SelectedValue, out id)) return 0;
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
            int gradeLevelId = GetSelectedGradeLevelId();
            bool showSection = IsSecondaryGrade(gradeLevelId);

            pnlSection.Visible = showSection;

            if (!showSection)
            {
                SelectSection("");
            }
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

        private void SelectDivision(string value)
        {
            if (ddlDivision == null) return;
            if (value == null) value = "";

            ListItem item = ddlDivision.Items.FindByValue(value);
            if (item != null)
            {
                ddlDivision.ClearSelection();
                item.Selected = true;
            }
            else if (ddlDivision.Items.Count > 0)
            {
                ddlDivision.SelectedIndex = 0;
            }
        }

        private string GetSelectedSection()
        {
            if (ddlSection == null) return null;
            string value = ddlSection.SelectedValue;
            if (string.IsNullOrWhiteSpace(value) || value == "0") return null;
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
        // Events
        // =========================================================
        protected void ddlClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddlStudent.Items.Clear();
                ddlStudent.Items.Add(new ListItem("-- اختر الطالب --", ""));

                pnlBuild.Visible = false;
                pnlSummary.Visible = false;

                lblStudentInfo.Text = "";
                lblMessage.Text = "";

                ConfigureSectionVisibility();
                LoadDivisions();

                LoadStudents();
            }
            catch (Exception ex)
            {
                ShowDetailedError("حدث خطأ أثناء تحميل طلاب الصف", ex);
            }
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddlStudent.Items.Clear();
                ddlStudent.Items.Add(new ListItem("-- اختر الطالب --", ""));

                pnlBuild.Visible = false;
                pnlSummary.Visible = false;

                lblStudentInfo.Text = "";
                lblMessage.Text = "";

                LoadDivisions();
                LoadStudents();
            }
            catch (Exception ex)
            {
                ShowDetailedError("حدث خطأ أثناء تحميل طلاب القسم", ex);
            }
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddlStudent.Items.Clear();
                ddlStudent.Items.Add(new ListItem("-- اختر الطالب --", ""));

                pnlBuild.Visible = false;
                pnlSummary.Visible = false;

                lblStudentInfo.Text = "";
                lblMessage.Text = "";

                LoadStudents();
            }
            catch (Exception ex)
            {
                ShowDetailedError("حدث خطأ أثناء تحميل طلاب الشعبة", ex);
            }
        }

        private void LoadStudents()
        {
            int gradeLevelId = GetSelectedGradeLevelId();
            if (gradeLevelId <= 0) return;

            EducationStage stage = EnumHelper.GetCurrentStage();
            string section = GetSelectedSection();
            string division = GetSelectedDivision();

            IQueryable<Student> query = db.Students
                .Where(s => s.IsActive
                         && s.Stage == stage
                         && s.ClassRoom != null
                         && s.ClassRoom.GradeLevelId == gradeLevelId);

            if (!string.IsNullOrWhiteSpace(section))
            {
                query = query.Where(s => s.ClassRoom.SectionName == section);
            }

            if (!string.IsNullOrWhiteSpace(division))
            {
                query = query.Where(s => s.ClassRoom.DivisionName == division);
            }

            var students = query
                .OrderBy(s => s.Name)
                .ToList();

            ddlStudent.Items.Clear();
            ddlStudent.Items.Add(new ListItem("-- اختر الطالب --", ""));

            foreach (var s in students)
            {
                ddlStudent.Items.Add(new ListItem(s.Name, s.Id.ToString()));
            }
        }

        protected void ddlStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlBuild.Visible = false;
            pnlSummary.Visible = false;

            lblStudentInfo.Text = "";
            lblMessage.Text = "";

            try
            {
                int studentId;
                if (!int.TryParse(ddlStudent.SelectedValue, out studentId) || studentId <= 0)
                    return;

                var student = db.Students
                    .Include(s => s.ClassRoom)
                    .Include(s => s.Parent)
                    .FirstOrDefault(s => s.Id == studentId);

                if (student == null)
                {
                    ShowError("الطالب غير موجود.");
                    return;
                }

                BuildForSelectedStudent(student);
            }
            catch (Exception ex)
            {
                ShowDetailedError("حدث خطأ أثناء اختيار الطالب", ex);
            }
        }

        // =========================================================
        // Fee Lines
        // =========================================================
        private void LoadFeeLines(Student student)
        {
            int academicYearId = GetCurrentAcademicYearId();
            if (academicYearId <= 0)
                throw new Exception("لم يتم العثور على السنة الدراسية الحالية.");

            int gradeLevelId = student.ClassRoom != null ? student.ClassRoom.GradeLevelId : 0;

            var fees = db.Fees
                .Where(f =>
                    f.AcademicYearId == academicYearId &&
                    (
                        f.ClassRoomId == student.ClassRoomId
                        ||
                        (!f.ClassRoomId.HasValue
                         && f.GradeLevelId.HasValue
                         && f.GradeLevelId.Value == gradeLevelId)
                    )
                )
                .OrderBy(f => f.Name)
                .ToList();

            gvLines.DataSource = fees;
            gvLines.DataBind();

            if (gvLines.Rows.Count == 0)
            {
                gvLines.DataSource = new List<object>();
                gvLines.DataBind();
            }
        }

        // =========================================================
        // ✅ سياسات الخصم: إخوة + تسجيل مبكر + اشتراك سنوي
        // =========================================================
        private DiscountPolicy FindDiscountPolicyByName(params string[] keywords)
        {
            if (keywords == null || keywords.Length == 0) return null;

            var policies = db.DiscountPolicies
                .AsNoTracking()
                .Where(p => p.IsActive)
                .ToList();

            return policies.FirstOrDefault(p =>
            {
                string name = (p.Name ?? "").Trim();
                if (string.IsNullOrWhiteSpace(name)) return false;

                return keywords.Any(k =>
                    !string.IsNullOrWhiteSpace(k) &&
                    name.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0);
            });
        }

        private DiscountPolicy GetSiblingDiscountPolicy()
        {
            return FindDiscountPolicyByName("إخوة", "اخوة", "أخوة", "sibling");
        }

        private DiscountPolicy GetEarlyBirdDiscountPolicy()
        {
            return FindDiscountPolicyByName("تسجيل المبكر", "تسجيل مبكر", "earlybird", "early");
        }

        private DiscountPolicy GetAnnualSubscriptionDiscountPolicy()
        {
            return FindDiscountPolicyByName("الاشتراك السنوي", "اشتراك سنوي", "annualsubscription", "annual");
        }

        // =========================================================
        // Discount Policies
        // =========================================================
        private void LoadDiscountPolicies(Student student)
        {
            var policies = db.DiscountPolicies
                .Where(p => p.IsActive)
                .OrderBy(p => p.SortOrder)
                .ToList();

            cblDiscounts.Items.Clear();
            lblNoPolicies.Visible = policies.Count == 0;

            int currentAcademicYearId = StudentFinancialService.GetCurrentAcademicYearId(db);

            var assignedPolicyIds = new HashSet<int>(
                db.StudentDiscounts
                    .Where(d =>
                        d.StudentId == student.Id &&
                        d.AcademicYearId == currentAcademicYearId &&
                        d.IsActive &&
                        d.DiscountPolicyId.HasValue)
                    .Select(d => d.DiscountPolicyId.Value)
                    .ToList());

            var siblingPolicy = GetSiblingDiscountPolicy();
            var earlyBirdPolicy = GetEarlyBirdDiscountPolicy();
            var annualSubPolicy = GetAnnualSubscriptionDiscountPolicy();

            foreach (var p in policies)
            {
                bool isSibling = siblingPolicy != null && p.Id == siblingPolicy.Id;
                bool isEarlyBird = earlyBirdPolicy != null && p.Id == earlyBirdPolicy.Id;
                bool isAnnualSub = annualSubPolicy != null && p.Id == annualSubPolicy.Id;

                string label = p.Name;

                if (isSibling || isEarlyBird || isAnnualSub)
                {
                    // ✅ سياسة يدوية
                    label += " (يُدخَل يدوياً)";
                }
                else if (p.Value <= 0m)
                {
                    // ✅ أي سياسة بقيمة صفر → يدوية
                    label += " (يُدخَل يدوياً)";
                }
                else if (p.Kind == DiscountKind.Percentage)
                {
                    label += " (" + p.Value.ToString("N0") + "%)";
                }
                else
                {
                    label += " (" + p.Value.ToString("N2") + " د.ل)";
                }

                ListItem item = new ListItem(label, p.Id.ToString());

                // ✅ سياسة الإخوة والتسجيل المبكر والاشتراك السنوي لا تُحدَّدان مسبقاً
                bool skipAutoCheck = isSibling || isEarlyBird || isAnnualSub;
                item.Selected = !skipAutoCheck && assignedPolicyIds.Contains(p.Id);

                cblDiscounts.Items.Add(item);
            }

            // ✅ NEW: تخزين معرّفي السياسات اليدوية
            hfSiblingPolicyId.Value = siblingPolicy != null
                ? siblingPolicy.Id.ToString()
                : "";

            hfEarlyBirdPolicyId.Value = earlyBirdPolicy != null
                ? earlyBirdPolicy.Id.ToString()
                : "";

            hfAnnualSubscriptionPolicyId.Value = annualSubPolicy != null
                ? annualSubPolicy.Id.ToString()
                : "";
        }

        // =========================================================
        // ✅ تحويل نص إلى decimal بشكل آمن
        // =========================================================
        private static decimal? ParseDecimalInvariant(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            // ✅ تطبيع الفواصل العربية/الأوروبية إلى نقطة
            string normalized = text.Trim()
                .Replace("\u066B", ".")  // Arabic decimal separator ٫
                .Replace("\u066C", "")   // Arabic thousands separator ٬
                .Replace("\u00A0", "")   // non-breaking space
                .Replace("\u200F", "")   // RTL mark
                .Replace("\u200E", "")   // LTR mark
                .Replace(",", ".");      // Latin comma → dot

            decimal value;
            if (decimal.TryParse(normalized,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value))
            {
                return value;
            }

            // محاولة ثانية: استخدام ثقافة عربية
            if (decimal.TryParse(normalized,
                    NumberStyles.Any,
                    CultureInfo.GetCultureInfo("ar-LY"),
                    out value))
            {
                return value;
            }

            return null;
        }

        // =========================================================
        // Selected Fees
        // =========================================================
        private List<StudentFinancialService.FeeAmount> GetSelectedFeeAmounts()
        {
            var result = new List<StudentFinancialService.FeeAmount>();

            System.Diagnostics.Trace.TraceInformation(
                "[Create] GetSelectedFeeAmounts: GridViewRows={0}", gvLines.Rows.Count);

            foreach (GridViewRow row in gvLines.Rows)
            {
                var chk = (CheckBox)row.FindControl("chkInclude");
                var txt = (TextBox)row.FindControl("txtAmount");

                if (chk == null || txt == null)
                {
                    System.Diagnostics.Trace.TraceWarning(
                        "[Create] Row {0}: chk or txt is null", row.RowIndex);
                    continue;
                }

                if (!chk.Checked)
                {
                    System.Diagnostics.Trace.TraceInformation(
                        "[Create] Row {0}: checkbox not checked", row.RowIndex);
                    continue;
                }

                int feeId;
                var keyValue = gvLines.DataKeys[row.RowIndex]?.Value;
                if (keyValue == null || !int.TryParse(keyValue.ToString(), out feeId))
                {
                    System.Diagnostics.Trace.TraceWarning(
                        "[Create] Row {0}: DataKey parse failed (value='{1}')",
                        row.RowIndex, keyValue);
                    continue;
                }

                var amount = ParseDecimalInvariant(txt.Text);
                if (!amount.HasValue)
                {
                    System.Diagnostics.Trace.TraceWarning(
                        "[Create] Row {0}: amount parse failed (raw='{1}')",
                        row.RowIndex, txt.Text);
                    continue;
                }

                if (amount.Value <= 0)
                {
                    System.Diagnostics.Trace.TraceInformation(
                        "[Create] Row {0}: amount <= 0", row.RowIndex);
                    continue;
                }

                var fee = db.Fees.Find(feeId);
                if (fee != null)
                {
                    result.Add(new StudentFinancialService.FeeAmount
                    {
                        Fee = fee,
                        Amount = amount.Value
                    });
                    System.Diagnostics.Trace.TraceInformation(
                        "[Create] Row {0}: added feeId={1}, amount={2}",
                        row.RowIndex, feeId, amount.Value);
                }
            }

            System.Diagnostics.Trace.TraceInformation(
                "[Create] GetSelectedFeeAmounts: returning {0} items", result.Count);

            return result;
        }

        // =========================================================
        // ✅ جمع مدخلات الخصومات اليدوية
        // =========================================================
        private List<ManualDiscountInput> GetManualDiscountInputs()
        {
            return new List<ManualDiscountInput>
            {
                new ManualDiscountInput
                {
                    Policy = GetSiblingDiscountPolicy(),
                    TextValue = txtSiblingDiscount.Text
                },
                new ManualDiscountInput
                {
                    Policy = GetEarlyBirdDiscountPolicy(),
                    TextValue = txtEarlyBirdDiscount.Text
                },
                new ManualDiscountInput
                {
                    Policy = GetAnnualSubscriptionDiscountPolicy(),
                    TextValue = txtAnnualSubscriptionDiscount.Text
                }
            };
        }

        // =========================================================
        // Compute Financials
        // =========================================================
        private StudentFinancialService.BenefitCalculation ComputeFinancials(
            out decimal subtotal, out decimal totalDiscount, out decimal net)
        {
            var feeLines = GetSelectedFeeAmounts();

            subtotal = feeLines.Sum(x => x.Amount);

            int academicYearId = StudentFinancialService.GetCurrentAcademicYearId(db);
            int studentId;

            var student = int.TryParse(ddlStudent.SelectedValue, out studentId)
                ? db.Students.Include(s => s.ClassRoom).FirstOrDefault(s => s.Id == studentId)
                : null;

            var selectedPolicyIds = cblDiscounts.Items
                .Cast<ListItem>()
                .Where(i => i.Selected)
                .Select(i =>
                {
                    int id;
                    return int.TryParse(i.Value, out id) ? id : 0;
                })
                .Where(i => i > 0)
                .ToList();

            // ============================================================
            // ✅ NEW: جمع الخصومات اليدوية (إخوة + تسجيل مبكر + اشتراك سنوي)
            // ============================================================
            var manualApplied = new List<InvoiceDiscount>();
            var manualInputs = GetManualDiscountInputs();

            foreach (var input in manualInputs)
            {
                if (input.Policy == null) continue;
                if (!selectedPolicyIds.Contains(input.Policy.Id)) continue;

                var amount = ParseDecimalInvariant(input.TextValue);
                if (amount.HasValue && amount.Value > 0)
                {
                    // استبعد السياسة من الحساب التلقائي
                    selectedPolicyIds.Remove(input.Policy.Id);

                    manualApplied.Add(new InvoiceDiscount
                    {
                        DiscountPolicyId = input.Policy.Id,
                        Name = input.Policy.Name,
                        Type = input.Policy.Type,
                        Value = amount.Value,
                        DiscountAmount = amount.Value,
                        AppliedToFeeType = input.Policy.AppliesToFeeType
                    });
                }
                // إن لم يُدخل مبلغ → تُترك للحساب التلقائي (لن يُطبَّق شيء إذا Value=0).
            }
            // ============================================================

            var calc = student == null
                ? new StudentFinancialService.BenefitCalculation()
                : StudentFinancialService.Calculate(
                    db, student, academicYearId, feeLines, selectedPolicyIds, DateTime.Today);

            // ============================================================
            // ✅ NEW: إضافة الخصومات اليدوية إلى النتيجة
            // ============================================================
            foreach (var d in manualApplied)
            {
                calc.TotalDiscount += d.DiscountAmount;
                AppendAdjustment(calc, d);
            }
            // ============================================================

            totalDiscount = calc.TotalDiscount;
            net = Math.Max(0m, subtotal - totalDiscount);

            return calc;
        }

        // =========================================================
        // ✅ إضافة عنصر إلى Adjustments بأمان
        // =========================================================
        private static void AppendAdjustment(
            StudentFinancialService.BenefitCalculation calc,
            InvoiceDiscount discount)
        {
            if (calc == null || discount == null) return;

            var prop = typeof(StudentFinancialService.BenefitCalculation)
                .GetProperty("Adjustments",
                    BindingFlags.Public | BindingFlags.Instance);

            if (prop == null) return;

            var current = prop.GetValue(calc);

            var list = current as IList;
            if (list != null)
            {
                list.Add(discount);
                return;
            }

            if (current == null)
            {
                var setter = prop.GetSetMethod(true);
                if (setter != null)
                {
                    var newList = new List<InvoiceDiscount> { discount };
                    try { setter.Invoke(calc, new object[] { newList }); }
                    catch { }
                }
                return;
            }

            var addMethod = current.GetType().GetMethod("Add",
                new[] { typeof(InvoiceDiscount) });

            if (addMethod != null)
            {
                try { addMethod.Invoke(current, new object[] { discount }); }
                catch { }
            }
        }

        // =========================================================
        // Preview
        // =========================================================
        protected void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                decimal subtotal, totalDiscount, net;
                var calc = ComputeFinancials(out subtotal, out totalDiscount, out net);

                lblTotal.Text = subtotal.ToString("N2") + " د.ل";
                lblDiscount.Text = totalDiscount.ToString("N2") + " د.ل";
                lblNet.Text = net.ToString("N2") + " د.ل";

                hfTotal.Value = subtotal.ToString(CultureInfo.InvariantCulture);
                hfDiscount.Value = totalDiscount.ToString(CultureInfo.InvariantCulture);
                hfNet.Value = net.ToString(CultureInfo.InvariantCulture);

                pnlSummary.Visible = true;

                lblMessage.Text = calc.Adjustments.Count == 0
                    ? ""
                    : "تم احتساب الخصومات والإعفاءات المستحقة تلقائياً.";

                lblMessage.CssClass = "inv-message success";
            }
            catch (Exception ex)
            {
                ShowDetailedError("حدث خطأ أثناء حساب الفاتورة", ex);
            }
        }

        // =========================================================
        // Save Invoice
        // =========================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int gradeLevelId = GetSelectedGradeLevelId();
            int studentId;

            if (gradeLevelId <= 0 || !int.TryParse(ddlStudent.SelectedValue, out studentId))
            {
                ShowError("يرجى اختيار الصف والطالب بشكل صحيح.");
                return;
            }

            try
            {
                var student = db.Students
                    .Include(s => s.ClassRoom)
                    .Include(s => s.Parent)
                    .FirstOrDefault(s => s.Id == studentId);

                if (student == null || student.ClassRoom == null
                    || student.ClassRoom.GradeLevelId != gradeLevelId)
                {
                    ShowError("الطالب المحدد لا ينتمي إلى الصف المختار.");
                    return;
                }

                string selectedSection = GetSelectedSection();
                if (!string.IsNullOrWhiteSpace(selectedSection)
                    && !string.Equals(selectedSection,
                        student.ClassRoom.SectionName ?? "",
                        StringComparison.OrdinalIgnoreCase))
                {
                    ShowError("الطالب المحدد لا ينتمي إلى القسم المختار.");
                    return;
                }

                string selectedDivision = GetSelectedDivision();
                if (!string.IsNullOrWhiteSpace(selectedDivision)
                    && !string.Equals(selectedDivision,
                        student.ClassRoom.DivisionName ?? "",
                        StringComparison.OrdinalIgnoreCase))
                {
                    ShowError("الطالب المحدد لا ينتمي إلى الشعبة المختارة.");
                    return;
                }

                if (student.ClassRoom != null)
                {
                    db.Entry(student.ClassRoom)
                        .Reference(c => c.AcademicYear)
                        .Load();
                }

                int academicYearId = GetCurrentAcademicYearId();
                if (academicYearId <= 0)
                {
                    ShowError("لم يتم العثور على السنة الدراسية الحالية.");
                    return;
                }

                if (student.ClassRoom == null || student.ClassRoom.AcademicYearId != academicYearId)
                {
                    ShowError("لا يمكن إصدار فاتورة: السنة الدراسية الحالية لا تتطابق مع صف الطالب.");
                    return;
                }

                decimal subtotal, totalDiscount, net;
                var calculation = ComputeFinancials(out subtotal, out totalDiscount, out net);
                var discounts = calculation.Adjustments;

                if (subtotal <= 0)
                {
                    ShowError("لا توجد بنود في الفاتورة.");
                    return;
                }

                DateTime invoiceDate;
                if (!DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out invoiceDate))
                {
                    ShowError("تاريخ الفاتورة غير صحيح.");
                    return;
                }

                var selectedFees = new List<Tuple<int, decimal>>();

                foreach (GridViewRow row in gvLines.Rows)
                {
                    var chk = (CheckBox)row.FindControl("chkInclude");
                    var txt = (TextBox)row.FindControl("txtAmount");

                    if (chk == null || !chk.Checked || txt == null) continue;

                    int feeId;
                    if (!int.TryParse(gvLines.DataKeys[row.RowIndex].Value.ToString(), out feeId))
                        continue;

                    var amount = ParseDecimalInvariant(txt.Text);
                    if (!amount.HasValue || amount.Value <= 0)
                        continue;

                    selectedFees.Add(Tuple.Create(feeId, amount.Value));
                }

                var feeIds = selectedFees.Select(x => x.Item1).Distinct().ToList();
                var validFees = db.Fees.Where(f => feeIds.Contains(f.Id)).ToList();

                if (validFees.Count(f => f.Type == FeeType.AnnualSubscription) > 1)
                {
                    ShowError("لا يمكن إدراج أكثر من اشتراك سنوي في نفس الفاتورة. اختر الاشتراك المطابق لمسار الطالب.");
                    return;
                }

                foreach (var selected in selectedFees)
                {
                    var fee = validFees.FirstOrDefault(f => f.Id == selected.Item1);

                    if (fee == null ||
                        fee.AcademicYearId != academicYearId ||
                        (fee.ClassRoomId.HasValue && fee.ClassRoomId.Value != student.ClassRoomId) ||
                        (fee.GradeLevelId.HasValue && fee.GradeLevelId.Value != student.ClassRoom.GradeLevelId) ||
                        selected.Item2 > fee.Amount)
                    {
                        ShowError("أحد الرسوم المحددة غير صالح للطالب أو يتجاوز قيمة الرسم الأصلية.");
                        return;
                    }
                }

                using (var tx = db.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        StudentFinancialService.EnsureEnrollment(db, student, academicYearId);

                        var invoice = new Invoice
                        {
                            InvoiceNumber = FinancialService.GetNextInvoiceNumber(db, invoiceDate),
                            StudentId = studentId,
                            AcademicYearId = academicYearId,
                            InvoiceDate = invoiceDate,
                            TotalAmount = subtotal,
                            DiscountAmount = totalDiscount,
                            NetAmount = net,
                            PaidAmount = 0,
                            Status = InvoiceStatus.Issued,
                            Notes = "",
                            CreatedBy = (Session["DisplayName"] as string)
                                        ?? (Session["Username"] as string)
                                        ?? "admin",
                            CreatedAt = DateTime.Now,
                            Lines = new List<InvoiceLine>(),
                            Discounts = new List<InvoiceDiscount>()
                        };

                        foreach (var selected in selectedFees)
                        {
                            var fee = validFees.First(f => f.Id == selected.Item1);
                            invoice.Lines.Add(new InvoiceLine
                            {
                                FeeId = fee.Id,
                                Description = fee.Name,
                                IsManual = false,
                                Amount = selected.Item2
                            });
                        }

                        foreach (var d in discounts)
                            invoice.Discounts.Add(d);

                        foreach (var selected in selectedFees)
                        {
                            var fee = validFees.First(f => f.Id == selected.Item1);

                            if (fee.Type == FeeType.AnnualSubscription ||
                                fee.Type == FeeType.Transport ||
                                fee.Type == FeeType.Activity ||
                                fee.Type == FeeType.Trip)
                            {
                                bool exists = db.StudentSubscriptions.Any(x =>
                                    x.StudentId == studentId &&
                                    x.AcademicYearId == academicYearId &&
                                    x.FeeId == fee.Id);

                                if (!exists)
                                {
                                    db.StudentSubscriptions.Add(new StudentSubscription
                                    {
                                        StudentId = studentId,
                                        AcademicYearId = academicYearId,
                                        FeeId = fee.Id,
                                        Status = SubscriptionStatus.Active,
                                        StartDate = student.ClassRoom.AcademicYear.StartDate,
                                        EndDate = student.ClassRoom.AcademicYear.EndDate,
                                        Notes = "تم إنشاء الاشتراك عند إصدار الفاتورة."
                                    });
                                }
                            }
                        }

                        db.Invoices.Add(invoice);
                        db.SaveChanges();

                        tx.Commit();

                        Response.Redirect("Details.aspx?id=" + invoice.Id, false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }

                        System.Diagnostics.Trace.TraceError("Invoice creation failed: " + ex);
                        ShowDetailedError("تعذر إصدار الفاتورة", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowDetailedError("حدث خطأ أثناء تجهيز الفاتورة للحفظ", ex);
            }
        }

        // =========================================================
        // Error Handling
        // =========================================================
        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "inv-message danger";
        }

        private void ShowDetailedError(string title, Exception ex)
        {
            Exception error = ex;
            while (error.InnerException != null)
                error = error.InnerException;

            lblMessage.Text = title + "<br/><strong>التفاصيل:</strong> " + Server.HtmlEncode(error.Message);
            lblMessage.CssClass = "inv-message danger";

            System.Diagnostics.Trace.TraceError(title + Environment.NewLine + ex);
        }

        private int GetCurrentAcademicYearId()
        {
            return StudentFinancialService.GetCurrentAcademicYearId(db);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}