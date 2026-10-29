using System;
using System.Globalization;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Services;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Pages.Payments
{
    public partial class Create : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        // =========================================================
        // ✅ رابط الرجوع
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

                return ResolveClientUrl("~/Pages/Payments/Default.aspx")
                     + "?stage=" + stageEnc;
            }
        }

        // =========================================================
        // Page Load
        // =========================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                lblStage.Text = EnumHelper.GetStageArabic(
                    Session["Stage"] as string ?? "");

                LoadAcademicYears();
                LoadGradeLevels();
                LoadDivisions();

                txtPaymentDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtReceiptNumber.Text = "يتم التوليد عند الحفظ";

                ConfigureSectionVisibility();

                ApplyQueryStringSelection();
            }
        }

        // =========================================================
        // تحميل السنوات الدراسية
        // =========================================================
        private void LoadAcademicYears()
        {
            var years = db.AcademicYears
                .OrderByDescending(y => y.StartDate)
                .ToList();

            ddlAcademicYear.Items.Clear();
            ddlAcademicYear.Items.Add(new ListItem("— اختر السنة —", ""));

            foreach (var y in years)
            {
                ddlAcademicYear.Items.Add(new ListItem(y.Name, y.Id.ToString()));
            }

            var current = years.FirstOrDefault(y => y.IsCurrent);
            if (current != null)
            {
                ddlAcademicYear.SelectedValue = current.Id.ToString();
            }
        }

        // =========================================================
        // تحميل الصفوف (GradeLevels)
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
            ddlClass.Items.Add(new ListItem("— اختر الصف —", ""));

            foreach (var gl in gradeLevels)
            {
                ddlClass.Items.Add(new ListItem(gl.Name, gl.Id.ToString()));
            }
        }

        // =========================================================
        // استخراج حرف الشعبة من اسم القاعة
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
        // استخراج القسم من اسم القاعة
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
        // قراءة studentId من الاستعلام
        // =========================================================
        private void ApplyQueryStringSelection()
        {
            int studentId;
            if (!int.TryParse(Request.QueryString["studentId"], out studentId)
                || studentId <= 0)
                return;

            var student = db.Students
                .Include(s => s.ClassRoom)
                .Include(s => s.ClassRoom.GradeLevel)
                .Include(s => s.ClassRoom.AcademicYear)
                .FirstOrDefault(s => s.Id == studentId && s.IsActive);

            if (student == null || student.ClassRoom == null)
                return;

            EducationStage stage = EnumHelper.GetCurrentStage();
            if (student.Stage != stage)
                return;

            int studentYearId = student.ClassRoom.AcademicYearId;
            if (ddlAcademicYear.Items.FindByValue(studentYearId.ToString()) != null)
            {
                ddlAcademicYear.SelectedValue = studentYearId.ToString();
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

            LoadFeesForSelectedStudent(studentId);
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
        // أحداث الفلاتر
        // =========================================================
        protected void ddlAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlStudent.Items.Clear();
            ddlStudent.Items.Add(new ListItem("— اختر الطالب —", ""));

            ddlFee.Items.Clear();
            ddlFee.Items.Add(new ListItem("— اختر الطالب أولاً —", ""));

            txtAmount.Text = "";
            hfInvoiceDate.Value = "";
            hfInvoiceId.Value = "";
            hfRemaining.Value = "";

            ConfigureSectionVisibility();
            LoadDivisions();

            LoadStudents();
        }

        protected void ddlClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlStudent.Items.Clear();
            ddlStudent.Items.Add(new ListItem("— اختر الطالب —", ""));

            ddlFee.Items.Clear();
            ddlFee.Items.Add(new ListItem("— اختر الطالب أولاً —", ""));

            txtAmount.Text = "";
            hfInvoiceDate.Value = "";
            hfInvoiceId.Value = "";
            hfRemaining.Value = "";

            ConfigureSectionVisibility();
            LoadDivisions();

            LoadStudents();
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlStudent.Items.Clear();
            ddlStudent.Items.Add(new ListItem("— اختر الطالب —", ""));

            ddlFee.Items.Clear();
            ddlFee.Items.Add(new ListItem("— اختر الطالب أولاً —", ""));

            txtAmount.Text = "";
            hfInvoiceDate.Value = "";
            hfInvoiceId.Value = "";
            hfRemaining.Value = "";

            LoadDivisions();
            LoadStudents();
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlStudent.Items.Clear();
            ddlStudent.Items.Add(new ListItem("— اختر الطالب —", ""));

            ddlFee.Items.Clear();
            ddlFee.Items.Add(new ListItem("— اختر الطالب أولاً —", ""));

            txtAmount.Text = "";
            hfInvoiceDate.Value = "";
            hfInvoiceId.Value = "";
            hfRemaining.Value = "";

            LoadStudents();
        }

        // =========================================================
        // تحميل الطلاب
        // =========================================================
        private void LoadStudents()
        {
            int gradeLevelId = GetSelectedGradeLevelId();
            if (gradeLevelId <= 0) return;

            EducationStage stage = EnumHelper.GetCurrentStage();
            string section = GetSelectedSection();
            string division = GetSelectedDivision();

            int yearId;
            int.TryParse(ddlAcademicYear.SelectedValue, out yearId);

            IQueryable<Student> query = db.Students
                .Where(s =>
                    s.IsActive &&
                    s.Stage == stage &&
                    s.ClassRoom != null &&
                    s.ClassRoom.GradeLevelId == gradeLevelId);

            if (yearId > 0)
                query = query.Where(s => s.ClassRoom.AcademicYearId == yearId);

            if (!string.IsNullOrWhiteSpace(section))
                query = query.Where(s => s.ClassRoom.SectionName == section);

            if (!string.IsNullOrWhiteSpace(division))
                query = query.Where(s => s.ClassRoom.DivisionName == division);

            var students = query
                .OrderBy(s => s.Name)
                .ToList();

            ddlStudent.Items.Clear();
            ddlStudent.Items.Add(new ListItem("— اختر الطالب —", ""));

            foreach (var s in students)
            {
                ddlStudent.Items.Add(new ListItem(s.Name, s.Id.ToString()));
            }
        }

        protected void ddlStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            int studentId;
            if (!int.TryParse(ddlStudent.SelectedValue, out studentId))
            {
                ddlFee.Items.Clear();
                ddlFee.Items.Add(new ListItem("— اختر الطالب أولاً —", ""));
                txtAmount.Text = "";
                hfInvoiceDate.Value = "";
                hfInvoiceId.Value = "";
                hfRemaining.Value = "";
                return;
            }

            LoadFeesForSelectedStudent(studentId);
        }

        // =========================================================
        // تحميل الرسوم للطالب
        // =========================================================
        private void LoadFeesForSelectedStudent(int studentId)
        {
            ddlFee.Items.Clear();
            ddlFee.Items.Add(new ListItem("— اختر الرسم —", ""));
            txtAmount.Text = "";
            hfInvoiceDate.Value = "";
            hfInvoiceId.Value = "";
            hfRemaining.Value = "";

            EducationStage stage = EnumHelper.GetCurrentStage();

            var student = db.Students
                .Include(s => s.ClassRoom)
                .FirstOrDefault(s =>
                    s.Id == studentId &&
                    s.IsActive &&
                    s.Stage == stage);

            if (student == null || student.ClassRoom == null)
                return;

            var feeCandidates = db.Fees
                .Where(f => f.ClassRoomId == student.ClassRoomId)
                .OrderBy(f => f.Name)
                .ToList();

            var fees = feeCandidates
                .Where(f => IsFeeSectionMatch(f, student))
                .ToList();

            foreach (var fee in fees)
            {
                ddlFee.Items.Add(new ListItem(fee.Name, fee.Id.ToString()));
            }
        }

        private bool IsFeeSectionMatch(Fee fee, Student student)
        {
            if (!fee.ClassRoomId.HasValue) return true;

            var feeCR = db.ClassRooms
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == fee.ClassRoomId.Value);

            if (feeCR == null) return true;

            string feeSec = (feeCR.SectionName ?? "").Trim();
            string stuSec = student.ClassRoom != null
                            ? (student.ClassRoom.SectionName ?? "").Trim()
                            : "";

            if (string.IsNullOrWhiteSpace(feeSec)) return true;
            if (string.IsNullOrWhiteSpace(stuSec)) return true;

            return string.Equals(feeSec, stuSec, StringComparison.OrdinalIgnoreCase);
        }

        // =========================================================
        // ✅ عند اختيار الرسم: احسب المتبقي واختر الفاتورة المناسبة
        // =========================================================
        // =========================================================
        // ✅ عند اختيار الرسم: احسب إجمالي المتبقي على الرسم (عبر كل الفواتير)
        // =========================================================
        protected void ddlFee_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtAmount.Text = "";
            hfInvoiceDate.Value = "";
            hfInvoiceId.Value = "";
            hfRemaining.Value = "";

            int feeId;
            if (!int.TryParse(ddlFee.SelectedValue, out feeId) || feeId <= 0)
                return;

            int studentId;
            if (!int.TryParse(ddlStudent.SelectedValue, out studentId) || studentId <= 0)
                return;

            DateTime selectedPaymentDate;
            if (!DateTime.TryParse(txtPaymentDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out selectedPaymentDate))
                selectedPaymentDate = DateTime.Today;

            // ✅ جلب كل الفواتير التي تحتوي هذا الرسم (غير ملغاة)
            var invoiceList = db.Invoices
                .Include(i => i.Lines)
                .Where(i =>
                    i.StudentId == studentId &&
                    i.Status != InvoiceStatus.Cancelled &&
                    i.Lines.Any(l => l.FeeId == feeId))
                .OrderByDescending(i => i.InvoiceDate)
                .ThenByDescending(i => i.Id)
                .ToList();

            if (!invoiceList.Any())
            {
                ShowError("لا توجد فاتورة صالحة لهذا الرسم. يجب إنشاء الفاتورة أولاً.");
                return;
            }

            // ✅ 1. حساب إجمالي المستحق للرسم (Net) عبر كل الفواتير
            decimal totalNetForFee = 0m;
            foreach (var inv in invoiceList)
            {
                var ctx = StudentFinancialService.LoadLineNetContext(db, inv);
                totalNetForFee += inv.Lines
                    .Where(l => l.FeeId == feeId)
                    .Sum(l => StudentFinancialService.GetLineNetAmount(inv, l, ctx));
            }

            // ✅ 2. حساب إجمالي المدفوع على الرسم (عبر كل الفواتير)
            decimal totalPaidForFee = db.Payments
                .Where(p => p.StudentId == studentId && p.FeeId == feeId)
                .Sum(p => (decimal?)p.Amount) ?? 0m;

            // ✅ 3. المتبقي الإجمالي
            decimal totalRemaining = Math.Max(0m, totalNetForFee - totalPaidForFee);

            // ✅ 4. اختيار الفاتورة الأنسب لتسجيل الدفعة (الأحدث ≤ تاريخ الدفع)
            var chosen = invoiceList
                .Where(i => i.InvoiceDate <= selectedPaymentDate)
                .FirstOrDefault();

            if (chosen == null)
            {
                chosen = invoiceList.OrderBy(i => i.InvoiceDate).First();
                txtPaymentDate.Text = chosen.InvoiceDate.ToString("yyyy-MM-dd");
                ShowError("تم تعديل تاريخ الدفع تلقائياً ليتوافق مع تاريخ الفاتورة ("
                    + chosen.InvoiceDate.ToString("yyyy-MM-dd") + ").");
            }

            // ✅ تخزين معلومات الفاتورة المختارة
            hfInvoiceId.Value = chosen.Id.ToString();
            hfInvoiceDate.Value = chosen.InvoiceDate.ToString("yyyy-MM-dd");
            hfRemaining.Value = totalRemaining.ToString(
                System.Globalization.CultureInfo.InvariantCulture);

            // ✅ عرض إجمالي المتبقي فقط في حقل المبلغ
            txtAmount.Text = totalRemaining.ToString("0.00");

            if (totalRemaining <= 0m)
            {
                ShowError("تم سداد كامل قيمة هذا الرسم. لا يوجد مبلغ متبقٍ.");
            }
        }

        // =========================================================
        // الحفظ
        // =========================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            int gradeLevelId = GetSelectedGradeLevelId();
            int studentId;
            int feeId;

            if (gradeLevelId <= 0 ||
                !int.TryParse(ddlStudent.SelectedValue, out studentId) ||
                !int.TryParse(ddlFee.SelectedValue, out feeId) ||
                string.IsNullOrWhiteSpace(ddlMethod.SelectedValue))
            {
                ShowError("يرجى اختيار السنة والصف والطالب والرسم وطريقة الدفع.");
                return;
            }

            decimal amount;
            if (!decimal.TryParse(txtAmount.Text.Trim(), out amount) || amount <= 0m)
            {
                ShowError("يرجى إدخال مبلغ صحيح أكبر من صفر.");
                return;
            }

            DateTime paymentDate;
            if (!DateTime.TryParse(txtPaymentDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out paymentDate))
            {
                ShowError("يرجى إدخال تاريخ دفع صحيح.");
                return;
            }

            EducationStage stage = EnumHelper.GetCurrentStage();

            var student = db.Students
                .Include(s => s.ClassRoom)
                .FirstOrDefault(s =>
                    s.Id == studentId &&
                    s.IsActive &&
                    s.Stage == stage &&
                    s.ClassRoom != null &&
                    s.ClassRoom.GradeLevelId == gradeLevelId);

            if (student == null)
            {
                ShowError("الطالب المحدد غير موجود أو لا يتبع الصف المختار.");
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

            var fee = db.Fees
                .FirstOrDefault(f =>
                    f.Id == feeId &&
                    f.ClassRoomId == student.ClassRoomId);

            if (fee == null)
            {
                ShowError("الرسم المحدد غير موجود أو لا يتبع صف الطالب.");
                return;
            }

            PaymentMethod method;
            if (!Enum.TryParse(ddlMethod.SelectedValue, true, out method))
            {
                ShowError("طريقة الدفع المحددة غير صحيحة.");
                return;
            }

            try
            {
                // ✅ اختيار الفاتورة الصحيحة بناءً على تاريخ الدفع
                int invoiceId = GetInvoiceIdForStudentFee(studentId, feeId, paymentDate);

                var payment = FinancialService.CreatePayment(
                    db,
                    invoiceId,
                    feeId,
                    amount,
                    method,
                    paymentDate,
                    txtNotes.Text.Trim());

                txtReceiptNumber.Text = payment.ReceiptNumber;

                string stageEnc = Server.UrlEncode(
                    Session["Stage"] as string ?? "Basic");

                Response.Redirect(
                    ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                        + "?studentId=" + studentId
                        + "&stage=" + stageEnc,
                    false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                ShowError("تعذر حفظ الدفعة بسبب تعارض أو مشكلة في بيانات قاعدة البيانات. يرجى المحاولة مرة أخرى.");
            }
            catch (Exception)
            {
                ShowError("حدث خطأ أثناء حفظ الدفعة. يرجى المحاولة مرة أخرى.");
            }
        }

        // =========================================================
        // ✅ اختيار الفاتورة المناسبة (التي تاريخها ≤ تاريخ الدفع)
        // =========================================================
        private int GetInvoiceIdForStudentFee(int studentId, int feeId, DateTime paymentDate)
        {
            // جرّب الفاتورة المخزّنة أولاً
            int cachedInvoiceId;
            if (int.TryParse(hfInvoiceId.Value, out cachedInvoiceId) && cachedInvoiceId > 0)
            {
                var cachedInvoice = db.Invoices
                    .AsNoTracking()
                    .FirstOrDefault(i => i.Id == cachedInvoiceId && i.Status != InvoiceStatus.Cancelled);

                if (cachedInvoice != null && cachedInvoice.InvoiceDate <= paymentDate)
                    return cachedInvoice.Id;
            }

            // ✅ اختر الأحدث التي تاريخها ≤ paymentDate
            var invoiceId = db.InvoiceLines
                .Where(l =>
                    l.FeeId == feeId &&
                    l.Invoice.StudentId == studentId &&
                    l.Invoice.Status != InvoiceStatus.Cancelled &&
                    l.Invoice.InvoiceDate <= paymentDate)
                .OrderByDescending(l => l.Invoice.InvoiceDate)
                .ThenByDescending(l => l.Invoice.Id)
                .Select(l => (int?)l.InvoiceId)
                .FirstOrDefault();

            if (invoiceId.HasValue)
                return invoiceId.Value;

            // ❌ لا توجد فاتورة مناسبة → اختر الأقدم
            var oldest = db.InvoiceLines
                .Where(l =>
                    l.FeeId == feeId &&
                    l.Invoice.StudentId == studentId &&
                    l.Invoice.Status != InvoiceStatus.Cancelled)
                .OrderBy(l => l.Invoice.InvoiceDate)
                .Select(l => new { l.InvoiceId, l.Invoice.InvoiceDate })
                .FirstOrDefault();

            if (oldest == null)
            {
                throw new InvalidOperationException(
                    "لا توجد فاتورة صالحة للطالب تحتوي على الرسم المحدد. يجب إنشاء الفاتورة أولاً.");
            }

            // صحّح تاريخ الدفع تلقائياً
            txtPaymentDate.Text = oldest.InvoiceDate.ToString("yyyy-MM-dd");

            throw new InvalidOperationException(
                "تاريخ الدفع المُدخل يسبق جميع الفواتير المتاحة. " +
                "تم اقتراح التاريخ " + oldest.InvoiceDate.ToString("yyyy-MM-dd") +
                " — يرجى إعادة المحاولة.");
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "pay-message danger";
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null)
            {
                db.Dispose();
            }

            base.OnUnload(e);
        }
    }
}