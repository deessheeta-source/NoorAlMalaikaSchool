using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Data.Services;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Fees
{
    public partial class StudentFinances : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private EducationStage CurrentStage
        {
            get { return GetCurrentStage(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            lblStage.Text = EnumHelper.GetStageArabic(
                Session["Stage"] as string ?? "Basic");

            if (!IsPostBack)
            {
                LoadAcademicYears();
                LoadGradeLevels();
                LoadDivisions();

                bool loadedFromQuery = ApplyQueryStringFilters();

                if (!loadedFromQuery)
                {
                    if (string.IsNullOrEmpty(ddlGradeLevel.SelectedValue))
                        ResetStudentList();
                    else
                        LoadStudentsForFilters();
                }
            }
        }

        // =========================================================
        // قراءة معامِلات الاستعلام
        // =========================================================
        private bool ApplyQueryStringFilters()
        {
            int studentId;
            bool hasStudent = int.TryParse(Request.QueryString["studentId"], out studentId)
                              && studentId > 0;

            if (hasStudent)
            {
                var student = db.Students
                    .Include(s => s.ClassRoom)
                    .FirstOrDefault(s => s.Id == studentId && s.IsActive);

                if (student != null && student.ClassRoom != null)
                {
                    int yearIdFromStudent = student.ClassRoom.AcademicYearId;
                    if (ddlAcademicYear.Items.FindByValue(yearIdFromStudent.ToString()) != null)
                    {
                        ddlAcademicYear.SelectedValue = yearIdFromStudent.ToString();
                    }

                    int gradeLevelIdFromStudent = student.ClassRoom.GradeLevelId;
                    if (ddlGradeLevel.Items.FindByValue(gradeLevelIdFromStudent.ToString()) != null)
                    {
                        ddlGradeLevel.SelectedValue = gradeLevelIdFromStudent.ToString();
                    }

                    ConfigureSectionFilter();
                    LoadDivisions();

                    if (IsSecondaryGrade(gradeLevelIdFromStudent)
                        && !string.IsNullOrWhiteSpace(student.ClassRoom.SectionName))
                    {
                        SelectSection(student.ClassRoom.SectionName);
                        LoadDivisions();
                    }

                    if (!string.IsNullOrWhiteSpace(student.ClassRoom.DivisionName))
                    {
                        SelectDivision(student.ClassRoom.DivisionName);
                    }

                    LoadStudentsForFilters();

                    if (ddlStudent.Items.FindByValue(studentId.ToString()) != null)
                    {
                        ddlStudent.SelectedValue = studentId.ToString();
                    }

                    LoadStudentData(studentId);

                    return true;
                }
            }

            int yearId;
            string yearParam = Request.QueryString["year"];
            if (int.TryParse(yearParam, out yearId) && yearId > 0
                && ddlAcademicYear.Items.FindByValue(yearId.ToString()) != null)
            {
                ddlAcademicYear.SelectedValue = yearId.ToString();
            }

            int gradeLevelId;
            string gradeParam = Request.QueryString["grade"];
            if (int.TryParse(gradeParam, out gradeLevelId) && gradeLevelId > 0
                && ddlGradeLevel.Items.FindByValue(gradeLevelId.ToString()) != null)
            {
                ddlGradeLevel.SelectedValue = gradeLevelId.ToString();
            }

            ConfigureSectionFilter();
            LoadDivisions();

            string sectionParam = Request.QueryString["section"];
            if (!string.IsNullOrWhiteSpace(sectionParam))
            {
                string s = sectionParam.Trim();
                if ((s == "علمي" || s == "أدبي")
                    && ddlSection.Items.FindByValue(s) != null)
                {
                    ddlSection.SelectedValue = s;
                    LoadDivisions();
                }
            }

            string divisionParam = Request.QueryString["division"];
            if (!string.IsNullOrWhiteSpace(divisionParam)
                && ddlDivision.Items.FindByValue(divisionParam) != null)
            {
                ddlDivision.SelectedValue = divisionParam;
            }

            return false;
        }

        // =========================================================
        // قراءة المستوى الحالي
        // =========================================================
        private EducationStage GetCurrentStage()
        {
            string stageParam = Request.QueryString["stage"];
            EducationStage stage;

            if (!string.IsNullOrEmpty(stageParam)
                && Enum.TryParse(stageParam, true, out stage))
            {
                Session["Stage"] = stage.ToString();
                return stage;
            }

            string sessionStage = Session["Stage"] as string;
            if (!string.IsNullOrEmpty(sessionStage)
                && Enum.TryParse(sessionStage, true, out stage))
            {
                return stage;
            }

            return EducationStage.Basic;
        }

        // =========================================================
        // دوال مساعدة للقسم والشعبة
        // =========================================================
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

                if (string.IsNullOrWhiteSpace(gradeName))
                    return false;

                gradeName = gradeName.Trim();
                return gradeName == "ثاني ثانوي" || gradeName == "ثالث ثانوي";
            }
            catch { return false; }
        }

        private void ConfigureSectionFilter()
        {
            int gradeLevelId;
            int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId);

            bool showSection = IsSecondaryGrade(gradeLevelId);
            pnlSectionFilter.Visible = showSection;

            if (!showSection)
            {
                SelectSection("");
            }
        }

        private void ResetSectionFilter()
        {
            SelectSection("");
            pnlSectionFilter.Visible = false;
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
        // استخراج حرف الشعبة من اسم القاعة (حل احتياطي)
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
        // استخراج القسم من اسم القاعة (حل احتياطي)
        // =========================================================
        private string ExtractSectionFromName(string classRoomName)
        {
            if (string.IsNullOrWhiteSpace(classRoomName)) return null;

            if (classRoomName.Contains("علمي")) return "علمي";
            if (classRoomName.Contains("أدبي")) return "أدبي";

            return null;
        }

        // =========================================================
        // تحميل قائمة الشعب ديناميكياً من ClassRooms
        // =========================================================
        private void LoadDivisions()
        {
            if (ddlDivision == null) return;

            string previousValue = ddlDivision.SelectedValue;

            ddlDivision.Items.Clear();
            ddlDivision.Items.Add(new ListItem("جميع الشعب", ""));

            int gradeLevelId;
            if (!int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
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
        // تحميل السنة الدراسية
        // =========================================================
        private void LoadAcademicYears()
        {
            var years = db.AcademicYears
                .OrderByDescending(y => y.Name)
                .ToList();

            ddlAcademicYear.Items.Clear();
            ddlAcademicYear.Items.Add(new ListItem("-- اختر السنة --", ""));

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
        // تحميل الصفوف الدراسية
        // =========================================================
        private void LoadGradeLevels()
        {
            EducationStage stage = CurrentStage;

            var gradeLevels = db.GradeLevels
                .AsNoTracking()
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlGradeLevel.Items.Clear();
            ddlGradeLevel.Items.Add(new ListItem("-- اختر الصف --", ""));

            foreach (var gl in gradeLevels)
            {
                ddlGradeLevel.Items.Add(new ListItem(gl.Name, gl.Id.ToString()));
            }
        }

        // =========================================================
        // تصفير قائمة الطلاب
        // =========================================================
        private void ResetStudentList()
        {
            if (ddlStudent == null) return;

            ddlStudent.Items.Clear();
            ddlStudent.Items.Add(new ListItem("-- اختر الطالب --", ""));
            ddlStudent.Enabled = false;

            pnlFees.Visible = false;
            gvStudentFees.DataSource = null;
            gvStudentFees.DataBind();

            if (pnlPayments != null)
                pnlPayments.Visible = false;

            if (rptPaymentGroups != null)
            {
                rptPaymentGroups.DataSource = null;
                rptPaymentGroups.DataBind();
            }

            if (pnlInvoices != null)
                pnlInvoices.Visible = false;

            if (gvStudentInvoices != null)
            {
                gvStudentInvoices.DataSource = null;
                gvStudentInvoices.DataBind();
            }

            if (lblInvoiceMessage != null)
                lblInvoiceMessage.Text = "";

            if (pnlQuickActions != null)
                pnlQuickActions.Visible = false;
        }

        // =========================================================
        // تحميل الطلاب (مُفلترين بالصف + القسم + الشعبة)
        // =========================================================
        private void LoadStudentsForFilters()
        {
            if (ddlStudent == null) return;

            ddlStudent.Items.Clear();
            ddlStudent.Items.Add(new ListItem("-- اختر الطالب --", ""));
            ddlStudent.Enabled = false;

            int gradeLevelId;
            if (!int.TryParse(ddlGradeLevel.SelectedValue, out gradeLevelId) || gradeLevelId <= 0)
                return;

            int academicYearId;
            if (!int.TryParse(ddlAcademicYear.SelectedValue, out academicYearId) || academicYearId <= 0)
                return;

            EducationStage stage = CurrentStage;
            string section = GetSelectedSection();
            string division = GetSelectedDivision();

            IQueryable<Student> query = db.Students
                .Where(s => s.IsActive
                         && s.Stage == stage
                         && s.ClassRoom != null
                         && s.ClassRoom.GradeLevelId == gradeLevelId
                         && s.ClassRoom.AcademicYearId == academicYearId);

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

            ddlStudent.DataSource = students;
            ddlStudent.DataTextField = "Name";
            ddlStudent.DataValueField = "Id";
            ddlStudent.DataBind();

            ddlStudent.Items.Insert(0, new ListItem("-- اختر الطالب --", ""));
            ddlStudent.Enabled = students.Count > 0;
        }

        // =========================================================
        // فلترة الرسوم حسب قسم الطالب
        // =========================================================
        private bool IsFeeSectionMatch(Fee fee, Student student)
        {
            if (!fee.ClassRoomId.HasValue) return true;

            var feeCR = db.ClassRooms
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == fee.ClassRoomId.Value);

            if (feeCR == null) return true;

            string feeSec = (feeCR.SectionName ?? "").Trim();
            string stuSec = (student.ClassRoom != null
                             ? (student.ClassRoom.SectionName ?? "").Trim()
                             : "");

            if (string.IsNullOrWhiteSpace(feeSec)) return true;
            if (string.IsNullOrWhiteSpace(stuSec)) return true;

            return string.Equals(feeSec, stuSec, StringComparison.OrdinalIgnoreCase);
        }

        // =========================================================
        // تحميل بيانات الطالب
        // =========================================================
        private void LoadStudentData(int studentId)
        {
            pnlFees.Visible = false;
            pnlPayments.Visible = false;
            pnlInvoices.Visible = false;

            if (pnlQuickActions != null)
                pnlQuickActions.Visible = false;

            if (lblInvoiceMessage != null)
                lblInvoiceMessage.Text = "";

            if (studentId <= 0) return;

            var student = db.Students
                .Include(s => s.ClassRoom)
                .Include(s => s.ClassRoom.GradeLevel)
                .FirstOrDefault(s => s.Id == studentId && s.IsActive);

            if (student == null) return;

            int academicYearId;
            if (!int.TryParse(ddlAcademicYear.SelectedValue, out academicYearId) || academicYearId <= 0)
                return;

            if (student.ClassRoom == null || student.ClassRoom.AcademicYearId != academicYearId)
                return;

            StudentFinancialService.EnsureEnrollment(db, student, academicYearId);
            db.SaveChanges();

            // ==============================
            // الرسوم
            // ==============================
            var allFees = StudentFinancialService.GetApplicableFees(db, student, academicYearId);

            var fees = allFees
                .Where(f => IsFeeSectionMatch(f, student))
                .ToList();

            if (fees.Count == 0 && student.ClassRoom != null)
            {
                int studentGradeLevelId = student.ClassRoom.GradeLevelId;

                fees = db.Fees
                    .Where(f => f.AcademicYearId == academicYearId
                             && f.GradeLevelId == studentGradeLevelId)
                    .ToList()
                    .Where(f => IsFeeSectionMatch(f, student))
                    .ToList();
            }

            var invoices = db.Invoices
                .Include(i => i.Lines)
                .Where(i => i.StudentId == studentId
                         && i.AcademicYearId == academicYearId
                         && i.Status != InvoiceStatus.Cancelled)
                .ToList();

            var lineContexts = StudentFinancialService.LoadLineNetContexts(db, invoices);
            var payments = db.Payments.Where(p => p.StudentId == studentId).ToList();

            var feeRows = new List<object>();

            foreach (var fee in fees)
            {
                var lines = invoices
                    .SelectMany(i => i.Lines
                        .Where(l => l.FeeId == fee.Id)
                        .Select(l => new { Invoice = i, Line = l }))
                    .ToList();

                decimal billed = lines.Sum(x => x.Line.Amount);

                decimal netDue = lines
                    .GroupBy(x => x.Invoice.Id)
                    .Sum(g => g.Sum(x => StudentFinancialService.GetLineNetAmount(
                        x.Invoice, x.Line, lineContexts[x.Invoice.Id])));

                if (lines.Count == 0) netDue = fee.Amount;

                decimal paid = payments
                    .Where(p => p.FeeId == fee.Id
                             && (!p.InvoiceId.HasValue
                                 || invoices.Any(i => i.Id == p.InvoiceId.Value)))
                    .Sum(p => p.Amount);

                decimal remaining = Math.Max(0m, netDue - paid);
                decimal adjustment = Math.Max(0m, billed - netDue);

                string status = lines.Count == 0
                    ? "لم تصدر فاتورة"
                    : (remaining <= 0 ? "مدفوع" : (paid > 0 ? "مدفوع جزئياً" : "غير مدفوع"));

                feeRows.Add(new
                {
                    FeeName = fee.Name,
                    Amount = billed > 0 ? billed : fee.Amount,
                    DiscountAmount = adjustment,
                    NetAmount = netDue,
                    PaidAmount = paid,
                    Remaining = remaining,
                    Status = status
                });
            }

            gvStudentFees.DataSource = feeRows;
            gvStudentFees.DataBind();
            pnlFees.Visible = true;

            // ==============================
            // سجل المدفوعات - مجمّع حسب نوع الرسم
            // ==============================
            var allPayments = db.Payments
                .Include(p => p.Fee)
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.PaymentDate)
                .ThenByDescending(p => p.Id)
                .ToList();

            var paymentGroups = allPayments
                .GroupBy(p => p.Fee != null ? p.Fee.Name : "أخرى")
                .Select(g => new
                {
                    FeeName = g.Key,
                    Count = g.Count(),
                    TotalAmountText = g.Sum(p => p.Amount).ToString("N2"),
                    Payments = g.Select(p => new
                    {
                        p.Id,
                        ReceiptNumber = string.IsNullOrEmpty(p.ReceiptNumber)
                            ? "—"
                            : p.ReceiptNumber,
                        AmountText = p.Amount.ToString("N2"),
                        p.PaymentDate,
                        MethodName = EnumHelper.GetPaymentMethodName(p.Method),
                        Notes = p.Notes
                    }).ToList()
                })
                .OrderBy(g => g.FeeName)
                .ToList();

            rptPaymentGroups.DataSource = paymentGroups;
            rptPaymentGroups.DataBind();

            pnlPayments.Visible = allPayments.Count > 0;

            // ==============================
            // فواتير الطالب
            // ==============================
            string stageName = Session["Stage"] as string ?? "Basic";
            string stageEnc = Server.UrlEncode(stageName);

            // ✅ تمت إضافة StudentId للـ projection لاستخدامه في زر التعديل
            var invoiceRows = db.Invoices
                .AsNoTracking()
                .Include(i => i.Student.ClassRoom.GradeLevel)
                .Where(i => i.StudentId == studentId
                         && i.AcademicYearId == academicYearId
                         && i.Status != InvoiceStatus.Cancelled)
                .OrderByDescending(i => i.InvoiceDate)
                .ThenByDescending(i => i.Id)
                .ToList()
                .Select(i => new
                {
                    i.Id,
                    i.StudentId,  // ✅ إضافة جديدة
                    StudentName = i.Student != null ? i.Student.Name : "",
                    GradeLevelName =
                        i.Student != null
                        && i.Student.ClassRoom != null
                        && i.Student.ClassRoom.GradeLevel != null
                            ? i.Student.ClassRoom.GradeLevel.Name
                            : (i.Student != null && i.Student.ClassRoom != null
                                ? i.Student.ClassRoom.Name
                                : ""),
                    Section = i.Student != null && i.Student.ClassRoom != null
                              ? i.Student.ClassRoom.SectionName : null,
                    i.InvoiceDate,
                    i.TotalAmount,
                    i.DiscountAmount,
                    i.NetAmount,
                    i.PaidAmount,
                    RemainingAmount = i.NetAmount - i.PaidAmount
                })
                .ToList();

            gvStudentInvoices.DataSource = invoiceRows;
            gvStudentInvoices.DataBind();

            pnlInvoices.Visible = true;

            // ==============================
            // الأزرار السياقية
            // ==============================
            pnlQuickActions.Visible = true;

            lnkGoToInvoice.HRef = ResolveClientUrl("~/Pages/Invoices/Create.aspx")
                + "?studentId=" + studentId + "&stage=" + stageEnc;

            lnkGoToAddPayment.HRef = ResolveClientUrl("~/Pages/Payments/Create.aspx")
                + "?studentId=" + studentId + "&stage=" + stageEnc;

            lnkGoToStudentReport.HRef = ResolveClientUrl("~/Pages/Payments/StudentReport.aspx")
                + "?studentId=" + studentId + "&stage=" + stageEnc;

            lnkGoToInvoices.HRef = ResolveClientUrl("~/Pages/Invoices/Default.aspx")
                + "?stage=" + stageEnc
                + "&studentId=" + studentId;
        }

        // =========================================================
        // ✅ عرض الملاحظة (تظهر كـ badge أو "—" إذا فارغة)
        // =========================================================
        protected string RenderNote(object value)
        {
            string note = value == null || value == DBNull.Value
                ? ""
                : Convert.ToString(value).Trim();

            if (string.IsNullOrWhiteSpace(note))
            {
                return "<span class=\"sf-note-badge empty\">—</span>";
            }

            string display = note.Length > 40
                ? note.Substring(0, 40) + "..."
                : note;

            return "<span class=\"sf-note-badge\" title=\""
                + System.Web.HttpUtility.HtmlEncode(note)
                + "\">"
                + System.Web.HttpUtility.HtmlEncode(display)
                + "</span>";
        }

        // =========================================================
        // ✅ معالج أوامر الدفعات
        // =========================================================
        protected void rptGroupPayments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "EditPayment" && e.CommandName != "DeletePayment")
                return;

            int paymentId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out paymentId) || paymentId <= 0)
            {
                ShowInvoiceError("رقم الدفعة غير صالح.");
                return;
            }

            // ============ تعديل ============
            if (e.CommandName == "EditPayment")
            {
                int studentId;
                if (!int.TryParse(ddlStudent.SelectedValue, out studentId) || studentId <= 0)
                {
                    ShowInvoiceError("لم يتم تحديد الطالب.");
                    return;
                }

                string stageEnc = Server.UrlEncode(Session["Stage"] as string ?? "Basic");

                Response.Redirect(
                    ResolveClientUrl("~/Pages/Payments/Edit.aspx")
                    + "?id=" + paymentId
                    + "&studentId=" + studentId
                    + "&stage=" + stageEnc,
                    false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            // ============ حذف ============
            try
            {
                var payment = db.Payments.FirstOrDefault(p => p.Id == paymentId);
                if (payment == null)
                {
                    ShowInvoiceError("الدفعة غير موجودة.");
                    return;
                }

                if (payment.InvoiceId.HasValue)
                {
                    var inv = db.Invoices.FirstOrDefault(i => i.Id == payment.InvoiceId.Value);
                    if (inv != null && inv.Status == InvoiceStatus.Paid)
                    {
                        ShowInvoiceError(
                            "لا يمكن حذف الدفعة لأنها مرتبطة بفاتورة مدفوعة بالكامل. " +
                            "قم بإلغاء الفاتورة أولاً أو تعديل الدفعة.");
                        return;
                    }
                }

                int? invoiceIdToRefresh = payment.InvoiceId;

                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Payments.Remove(payment);

                        if (invoiceIdToRefresh.HasValue)
                        {
                            var invoice = db.Invoices
                                .FirstOrDefault(i => i.Id == invoiceIdToRefresh.Value);

                            if (invoice != null)
                            {
                                decimal newPaid = db.Payments
                                    .Where(p => p.InvoiceId == invoice.Id && p.Id != paymentId)
                                    .Select(p => (decimal?)p.Amount)
                                    .Sum() ?? 0m;

                                invoice.PaidAmount = Math.Min(invoice.NetAmount, Math.Max(0m, newPaid));

                                if (invoice.PaidAmount <= 0)
                                    invoice.Status = InvoiceStatus.Issued;
                                else if (invoice.PaidAmount >= invoice.NetAmount)
                                    invoice.Status = InvoiceStatus.Paid;
                                else
                                    invoice.Status = InvoiceStatus.PartiallyPaid;
                            }
                        }

                        db.SaveChanges();
                        tx.Commit();

                        ShowInvoiceSuccess("تم حذف الدفعة بنجاح.");
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }

                int reloadStudentId;
                if (int.TryParse(ddlStudent.SelectedValue, out reloadStudentId) && reloadStudentId > 0)
                {
                    LoadStudentData(reloadStudentId);
                }
            }
            catch (Exception ex)
            {
                Exception error = ex;
                while (error.InnerException != null)
                    error = error.InnerException;

                ShowInvoiceError("تعذر حذف الدفعة: " + Server.HtmlEncode(error.Message));
            }
        }

        // =========================================================
        // حذف فاتورة
        // =========================================================
        protected void gvStudentInvoices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteInvoice") return;

            int invoiceId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out invoiceId) || invoiceId <= 0)
            {
                ShowInvoiceError("رقم الفاتورة غير صالح.");
                return;
            }

            try
            {
                var invoice = db.Invoices
                    .Include(i => i.Lines)
                    .Include(i => i.Discounts)
                    .FirstOrDefault(i => i.Id == invoiceId);

                if (invoice == null)
                {
                    ShowInvoiceError("الفاتورة غير موجودة.");
                    return;
                }

                bool hasPayments = db.Payments.Any(p => p.InvoiceId == invoiceId);
                if (hasPayments)
                {
                    ShowInvoiceError("لا يمكن حذف الفاتورة لوجود مدفوعات مسجلة عليها.");
                    return;
                }

                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (invoice.Lines != null && invoice.Lines.Any())
                            db.InvoiceLines.RemoveRange(invoice.Lines);

                        if (invoice.Discounts != null && invoice.Discounts.Any())
                            db.InvoiceDiscounts.RemoveRange(invoice.Discounts);

                        var subscriptions = db.StudentSubscriptions
                            .Where(s => s.StudentId == invoice.StudentId
                                     && s.AcademicYearId == invoice.AcademicYearId)
                            .ToList();

                        if (subscriptions.Any())
                            db.StudentSubscriptions.RemoveRange(subscriptions);

                        db.Invoices.Remove(invoice);
                        db.SaveChanges();
                        tx.Commit();

                        ShowInvoiceSuccess("تم حذف الفاتورة بنجاح.");
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }

                int studentId;
                if (int.TryParse(ddlStudent.SelectedValue, out studentId) && studentId > 0)
                {
                    LoadStudentData(studentId);
                }
            }
            catch (Exception ex)
            {
                Exception error = ex;
                while (error.InnerException != null)
                    error = error.InnerException;

                ShowInvoiceError("تعذر حذف الفاتورة: " + Server.HtmlEncode(error.Message));
            }
        }

        private void ShowInvoiceError(string message)
        {
            lblInvoiceMessage.Text = message;
            lblInvoiceMessage.CssClass = "alert alert-danger d-block mb-2";
        }

        private void ShowInvoiceSuccess(string message)
        {
            lblInvoiceMessage.Text = message;
            lblInvoiceMessage.CssClass = "alert alert-success d-block mb-2";
        }

        // =========================================================
        // تنسيق القسم
        // =========================================================
        protected string FormatSection(object section)
        {
            if (section == null || section == DBNull.Value)
                return "<span class=\"text-muted\">—</span>";

            string value = Convert.ToString(section).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return "<span class=\"text-muted\">—</span>";

            string display;
            if (value == "علمي") display = "القسم العلمي";
            else if (value == "أدبي") display = "القسم الأدبي";
            else display = value;

            return "<span class=\"status-badge paid\">"
                + System.Web.HttpUtility.HtmlEncode(display) + "</span>";
        }

        // =========================================================
        // الأحداث
        // =========================================================
        protected void ddlAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlGradeLevel.Items.Count > 0)
                ddlGradeLevel.SelectedIndex = 0;

            ResetSectionFilter();
            LoadDivisions();

            ResetStudentList();
        }

        protected void ddlGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
            LoadDivisions();
            ResetStudentList();
            LoadStudentsForFilters();
        }

        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDivisions();
            ResetStudentList();
            LoadStudentsForFilters();
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetStudentList();
            LoadStudentsForFilters();
        }

        protected void ddlStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            int studentId;
            if (!int.TryParse(ddlStudent.SelectedValue, out studentId))
                return;

            LoadStudentData(studentId);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}