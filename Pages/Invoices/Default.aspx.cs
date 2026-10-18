using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.Invoices
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        // ← الحقل الذي يستدعيه الـ ASPX في: <a href="<%= _returnUrl %>">
        protected string _returnUrl = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            // ✅ الإصلاح: استدعاء ConfigureReturnUrl في كل تحميل للصفحة
            // حتى لا يُفقد رابط العودة عند الـ PostBack (بحث/ترقيم/تصفية)
            ConfigureReturnUrl();

            if (!IsPostBack)
            {
                LoadStatusFilter();
                LoadYearFilter();
                LoadGradeLevels();
                ConfigureSectionFilter();
                LoadInvoices();
            }
        }

        // =========================================================
        // ضبط رابط العودة العام (_returnUrl)
        // ✅ يعود إلى StudentFinances.aspx مع الحفاظ على رقم الطالب
        // =========================================================
        private void ConfigureReturnUrl()
        {
            string stageEnc = Server.UrlEncode(Session["Stage"] as string ?? "Basic");

            // الأولوية 1: returnUrl صريح في الرابط (لو جاء من صفحة التفاصيل)
            string returnUrl = Request.QueryString["returnUrl"];
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                string decoded = Server.UrlDecode(returnUrl);
                if (IsLocalUrl(decoded))
                {
                    _returnUrl = decoded;
                    return;
                }
            }

            // الأولوية 2: studentId في الرابط → ارجع لصفحة الطالب بنفس البيانات
            int studentId;
            if (int.TryParse(Request.QueryString["studentId"], out studentId) && studentId > 0)
            {
                _returnUrl = ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                           + "?studentId=" + studentId
                           + "&stage=" + stageEnc;
                return;
            }

            // الأولوية 3: studentId من الـ Session (لو تم حفظه سابقاً)
            object sessionStudentId = Session["CurrentStudentId"];
            int sessionId;
            if (sessionStudentId != null
                && int.TryParse(sessionStudentId.ToString(), out sessionId)
                && sessionId > 0)
            {
                _returnUrl = ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                           + "?studentId=" + sessionId
                           + "&stage=" + stageEnc;
                return;
            }

            // الافتراضي: صفحة الطالب العامة
            _returnUrl = ResolveClientUrl("~/Pages/Fees/StudentFinances.aspx")
                       + "?stage=" + stageEnc;
        }

        private bool IsLocalUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            if (!(url.StartsWith("/") || url.StartsWith("~/"))) return false;
            if (url.StartsWith("//") || url.StartsWith("/\\")) return false;
            return true;
        }

        // =========================================================
        // بناء رابط صفحة التفاصيل مع تمرير الفلاتر الحالية + returnUrl
        // =========================================================
        protected string BuildDetailsUrl(object invoiceId)
        {
            string baseUrl = ResolveClientUrl("~/Pages/Invoices/Details.aspx");
            string stageEnc = Server.UrlEncode(Session["Stage"] as string ?? "Basic");

            string qs = "?id=" + Convert.ToString(invoiceId);
            qs += "&stage=" + stageEnc;

            int studentId;
            if (int.TryParse(Request.QueryString["studentId"], out studentId) && studentId > 0)
                qs += "&studentId=" + studentId;

            qs += "&returnUrl=" + Server.UrlEncode(Request.RawUrl);

            return baseUrl + qs;
        }

        // =========================================================
        // تحميل الفلاتر
        // =========================================================
        private void LoadStatusFilter()
        {
            foreach (InvoiceStatus status in Enum.GetValues(typeof(InvoiceStatus)))
            {
                ddlStatus.Items.Add(new ListItem(status.GetDisplayName(), status.ToString()));
            }
        }

        private void LoadYearFilter()
        {
            var years = db.AcademicYears.OrderByDescending(y => y.Id).ToList();
            foreach (var y in years)
            {
                ddlYear.Items.Add(new ListItem(y.Name, y.Id.ToString()));
            }
        }

        private void LoadGradeLevels()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();

            var gradeLevels = db.GradeLevels
                .AsNoTracking()
                .Where(g => g.Stage == stage)
                .OrderBy(g => g.SortOrder)
                .ToList();

            ddlGradeLevel.Items.Clear();
            ddlGradeLevel.Items.Add(new ListItem("الكل", ""));

            foreach (var gl in gradeLevels)
            {
                ddlGradeLevel.Items.Add(new ListItem(gl.Name, gl.Id.ToString()));
            }
        }

        // =========================================================
        // دوال مساعدة للقسم
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
            {
                if (ddlSection.Items.FindByValue("") != null)
                {
                    ddlSection.ClearSelection();
                    ddlSection.Items.FindByValue("").Selected = true;
                }
            }
        }

        private string GetSelectedSection()
        {
            if (ddlSection == null) return null;
            string value = ddlSection.SelectedValue;
            if (string.IsNullOrWhiteSpace(value)) return null;
            return value;
        }

        protected void ddlGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigureSectionFilter();
            gvInvoices.PageIndex = 0;
            LoadInvoices();
        }

        // =========================================================
        // ✅ بناء استعلام الفواتير (مع كل الفلاتر)
        // =========================================================
        private IQueryable<Invoice> BuildInvoicesQuery()
        {
            EducationStage stage = EnumHelper.GetCurrentStage();
            string search = txtSearch.Text.Trim();
            string statusValue = ddlStatus.SelectedValue;
            string yearValue = ddlYear.SelectedValue;
            int gradeLevelId = GetSelectedGradeLevelId();
            string section = GetSelectedSection();

            IQueryable<Invoice> query = db.Invoices
                .AsNoTracking()
                .Where(i => i.Student.Stage == stage);

            int qsStudentId;
            if (int.TryParse(Request.QueryString["studentId"], out qsStudentId) && qsStudentId > 0)
            {
                query = query.Where(i => i.StudentId == qsStudentId);
            }

            if (!string.IsNullOrEmpty(statusValue))
            {
                InvoiceStatus status;
                if (Enum.TryParse(statusValue, out status))
                    query = query.Where(i => i.Status == status);
            }

            if (!string.IsNullOrEmpty(yearValue))
            {
                int yearId;
                if (int.TryParse(yearValue, out yearId))
                    query = query.Where(i => i.AcademicYearId == yearId);
            }

            if (gradeLevelId > 0)
            {
                query = query.Where(i =>
                    i.Student.ClassRoom != null
                    && i.Student.ClassRoom.GradeLevelId == gradeLevelId);
            }

            if (!string.IsNullOrWhiteSpace(section))
            {
                query = query.Where(i =>
                    i.Student.Section == section ||
                    (i.Student.Section == null && i.Student.ClassRoom.SectionName == section));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i =>
                    (i.Student != null && i.Student.Name.Contains(search))
                    || i.InvoiceNumber.Contains(search));
            }

            return query;
        }

        // =========================================================
        // ✅ تحميل الإحصائيات
        // =========================================================
        private void LoadStats()
        {
            try
            {
                var q = BuildInvoicesQuery();

                int totalCount = q.Count();
                decimal totalAmount = q.Sum(i => (decimal?)i.NetAmount) ?? 0m;
                decimal totalPaid = q.Sum(i => (decimal?)i.PaidAmount) ?? 0m;
                decimal totalRemaining = totalAmount - totalPaid;

                lblTotalInvoices.Text = totalCount.ToString();
                lblTotalAmount.Text = totalAmount.ToString("N2") + " د.ل";
                lblTotalPaid.Text = totalPaid.ToString("N2") + " د.ل";
                lblTotalRemaining.Text = totalRemaining.ToString("N2") + " د.ل";
            }
            catch
            {
                lblTotalInvoices.Text = "0";
                lblTotalAmount.Text = "0.00 د.ل";
                lblTotalPaid.Text = "0.00 د.ل";
                lblTotalRemaining.Text = "0.00 د.ل";
            }
        }

        // =========================================================
        // تحميل الفواتير
        // =========================================================
        private void LoadInvoices()
        {
            try
            {
                var query = BuildInvoicesQuery();

                int totalCount = query.Count();

                var invoices = query
                    .OrderByDescending(i => i.InvoiceDate)
                    .ThenByDescending(i => i.Id)
                    .Skip(gvInvoices.PageIndex * gvInvoices.PageSize)
                    .Take(gvInvoices.PageSize)
                    .Select(i => new
                    {
                        i.Id,
                        i.InvoiceNumber,
                        StudentName = i.Student != null ? i.Student.Name : "",
                        GradeLevelName =
                            i.Student != null
                            && i.Student.ClassRoom != null
                            && i.Student.ClassRoom.GradeLevel != null
                                ? i.Student.ClassRoom.GradeLevel.Name
                                : (i.Student != null && i.Student.ClassRoom != null
                                    ? i.Student.ClassRoom.Name
                                    : ""),
                        Section = i.Student != null ? i.Student.Section : null,
                        i.InvoiceDate,
                        i.TotalAmount,
                        i.DiscountAmount,
                        i.NetAmount,
                        i.PaidAmount,
                        RemainingAmount = i.NetAmount - i.PaidAmount,
                        i.Status
                    })
                    .ToList();

                gvInvoices.AllowCustomPaging = true;
                gvInvoices.VirtualItemCount = totalCount;
                gvInvoices.DataSource = invoices;
                gvInvoices.DataBind();

                // ✅ تحديث الإحصائيات
                LoadStats();
            }
            catch (Exception ex)
            {
                ShowError("تعذر تحميل الفواتير: " + Server.HtmlEncode(ex.Message));
            }
        }

        // =========================================================
        // RowCommand — حذف الفاتورة
        // =========================================================
        protected void gvInvoices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteInvoice") return;

            int invoiceId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out invoiceId) || invoiceId <= 0)
            {
                ShowError("رقم الفاتورة غير صالح.");
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
                    ShowError("الفاتورة غير موجودة.");
                    LoadInvoices();
                    return;
                }

                bool hasPayments = db.Payments.Any(p => p.InvoiceId == invoiceId);

                if (hasPayments)
                {
                    ShowError("لا يمكن حذف الفاتورة لوجود مدفوعات مسجلة عليها. " +
                              "قم بإلغاء الفاتورة بدلاً من ذلك.");
                    LoadInvoices();
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

                        ShowSuccess("تم حذف الفاتورة بنجاح.");
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw ex;
                    }
                }

                LoadInvoices();
            }
            catch (Exception ex)
            {
                Exception error = ex;
                while (error.InnerException != null)
                    error = error.InnerException;

                ShowError("تعذر حذف الفاتورة: " + Server.HtmlEncode(error.Message));
                LoadInvoices();
            }
        }

        // =========================================================
        // دوال مساعدة للعرض
        // =========================================================
        protected string GetStatusName(string statusValue)
        {
            InvoiceStatus status;
            if (Enum.TryParse(statusValue, out status))
                return status.GetDisplayName();
            return statusValue;
        }

        protected string GetStatusCss(string statusValue)
        {
            InvoiceStatus status;
            if (Enum.TryParse(statusValue, out status))
            {
                switch (status)
                {
                    case InvoiceStatus.Paid: return "status-badge paid";
                    case InvoiceStatus.PartiallyPaid: return "status-badge partial";
                    case InvoiceStatus.Issued: return "status-badge issued";
                    case InvoiceStatus.Draft: return "status-badge draft";
                    case InvoiceStatus.Cancelled: return "status-badge cancelled";
                }
            }
            return "status-badge draft";
        }

        protected string FormatSection(object section)
        {
            if (section == null || section == DBNull.Value)
                return "<span class='text-muted'>—</span>";

            string value = Convert.ToString(section).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return "<span class='text-muted'>—</span>";

            string display;
            if (value == "علمي") display = "القسم العلمي";
            else if (value == "أدبي") display = "القسم الأدبي";
            else display = value;

            return "<span class='status-badge issued'>"
                + HttpUtility.HtmlEncode(display) + "</span>";
        }

        // =========================================================
        // رسائل الخطأ والنجاح
        // =========================================================
        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "inv-message danger";
        }

        private void ShowSuccess(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "inv-message success";
        }

        // =========================================================
        // الأحداث
        // =========================================================
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            gvInvoices.PageIndex = 0;
            ConfigureSectionFilter();
            LoadInvoices();
        }

        protected void gvInvoices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInvoices.PageIndex = e.NewPageIndex;
            LoadInvoices();
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}