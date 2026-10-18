using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.DiscountPolicies
{
    public partial class Edit : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private int PolicyId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
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
                if (PolicyId <= 0)
                {
                    Response.Redirect("Default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                if (!IsPostBack)
                {
                    LoadTypes();
                    LoadPolicy();
                }
            }
            catch (Exception ex)
            {
                HandleException("تعذر تحميل سياسة الخصم.", ex);
            }
        }

        // =========================================================
        // تحميل الأنواع
        // =========================================================
        private void LoadTypes()
        {
            var types = Enum.GetValues(typeof(DiscountType))
                .Cast<DiscountType>()
                .Select(t => new ListItem(t.GetDisplayName(), t.ToString()))
                .ToList();

            ddlType.DataSource = types;
            ddlType.DataTextField = "Text";
            ddlType.DataValueField = "Value";
            ddlType.DataBind();
        }

        // =========================================================
        // هل النوع يدعم "المبلغ اليدوي"؟ (Sibling أو EarlyBird أو AnnualSubscription)
        // =========================================================
        private static bool IsManualValueType(DiscountType type)
        {
            string typeStr = type.ToString();
            return type == DiscountType.Sibling
                || typeStr.Equals("EarlyBird", StringComparison.OrdinalIgnoreCase)
                || typeStr.Equals("AnnualSubscription", StringComparison.OrdinalIgnoreCase); // ✅ تمت الإضافة
        }

        // =========================================================
        // تحميل السياسة
        // =========================================================
        private void LoadPolicy()
        {
            var policy = db.DiscountPolicies.FirstOrDefault(x => x.Id == PolicyId);
            if (policy == null)
            {
                Response.Redirect("Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            txtName.Text = policy.Name ?? "";

            if (ddlType.Items.FindByValue(policy.Type.ToString()) != null)
                ddlType.SelectedValue = policy.Type.ToString();

            if (ddlKind.Items.FindByValue(policy.Kind.ToString()) != null)
                ddlKind.SelectedValue = policy.Kind.ToString();

            // ✅ إذا كانت السياسة من نوع يدوي (Sibling / EarlyBird / AnnualSubscription) والقيمة = 0 → نعتبرها "مبلغ يدوي"
            bool isManualType = IsManualValueType(policy.Type);
            bool isManual = isManualType && policy.Value == 0m;

            chkManualValue.Checked = isManual;

            if (!isManual)
            {
                // عرض القيمة بشكل عادي
                if (policy.Value == decimal.Truncate(policy.Value))
                    txtValue.Text = decimal.Truncate(policy.Value).ToString("0");
                else
                    txtValue.Text = policy.Value.ToString("0.##");
            }
            else
            {
                txtValue.Text = "";
            }

            chkAuto.Checked = policy.ApplyAutomatically;
            chkActive.Checked = policy.IsActive;
            txtDescription.Text = policy.Description ?? "";

            if (policy.AppliesToFeeType.HasValue)
            {
                string feeTypeValue = policy.AppliesToFeeType.Value.ToString();
                if (ddlAppliesToFeeType.Items.FindByValue(feeTypeValue) != null)
                    ddlAppliesToFeeType.SelectedValue = feeTypeValue;
            }
            else
            {
                if (ddlAppliesToFeeType.Items.FindByValue("") != null)
                    ddlAppliesToFeeType.SelectedValue = "";
            }
        }

        // =========================================================
        // حفظ السياسة
        // =========================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ClearMessage();

                // ------------- اسم السياسة -------------
                string name = txtName.Text == null ? "" : txtName.Text.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    ShowError("يرجى إدخال اسم السياسة.");
                    return;
                }

                // ------------- النوع -------------
                if (string.IsNullOrWhiteSpace(ddlType.SelectedValue))
                {
                    ShowError("يرجى اختيار نوع السياسة.");
                    return;
                }

                DiscountType discountType;
                if (!Enum.TryParse(ddlType.SelectedValue, true, out discountType))
                {
                    ShowError("نوع سياسة الخصم غير صحيح.");
                    return;
                }

                // ------------- نوع القيمة -------------
                DiscountKind kind;
                if (!Enum.TryParse(ddlKind.SelectedValue, true, out kind))
                {
                    ShowError("نوع القيمة غير صحيح.");
                    return;
                }

                // ------------- ✅ هل المبلغ يدوي؟ -------------
                bool isManualType = IsManualValueType(discountType);
                bool manualValue = isManualType && chkManualValue.Checked;

                int integerValue = 0;

                if (manualValue)
                {
                    integerValue = 0;
                }
                else
                {
                    if (!int.TryParse(txtValue.Text.Trim(), out integerValue))
                    {
                        ShowError("القيمة يجب أن تكون رقماً صحيحاً فقط، مثل 5 أو 10 أو 25.");
                        return;
                    }

                    if (integerValue < 0)
                    {
                        ShowError("القيمة لا يمكن أن تكون سالبة.");
                        return;
                    }

                    if (kind == DiscountKind.Percentage && integerValue > 100)
                    {
                        ShowError("نسبة الخصم لا يمكن أن تتجاوز 100%.");
                        return;
                    }
                }

                // ------------- البحث عن السياسة -------------
                var policy = db.DiscountPolicies.FirstOrDefault(x => x.Id == PolicyId);
                if (policy == null)
                {
                    ShowError("سياسة الخصم غير موجودة.");
                    return;
                }

                // ------------- نطاق الرسم -------------
                FeeType parsedFeeType;
                FeeType? feeType =
                    Enum.TryParse(ddlAppliesToFeeType.SelectedValue, true, out parsedFeeType)
                        ? (FeeType?)parsedFeeType
                        : null;

                // ------------- تحديث البيانات -------------
                policy.Name = name;
                policy.Type = discountType;

                // ✅ عند التفعيل اليدوي: نجبر النوع Amount والقيمة 0، ونُلغي التطبيق التلقائي
                policy.Kind = manualValue ? DiscountKind.Amount : kind;
                policy.Value = Convert.ToDecimal(integerValue);
                policy.ApplyAutomatically = manualValue ? false : chkAuto.Checked;
                policy.IsActive = chkActive.Checked;
                policy.Description = txtDescription.Text == null ? "" : txtDescription.Text.Trim();
                policy.AppliesToFeeType = feeType;

                db.SaveChanges();

                Response.Redirect("Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                HandleException("تعذر حفظ سياسة الخصم.", ex);
            }
        }

        // =========================================================
        // رسائل
        // =========================================================
        private void ShowError(string message)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.CssClass = "dp-message danger";
        }

        private void ClearMessage()
        {
            lblMessage.Text = "";
            lblMessage.CssClass = "dp-message";
        }

        private void HandleException(string friendlyMessage, Exception ex)
        {
            System.Diagnostics.Trace.TraceError("DiscountPolicies.Edit Error: " + ex);

            string message = GetInnerExceptionMessage(ex);
            if (string.IsNullOrWhiteSpace(message))
            {
                ShowError(friendlyMessage);
                return;
            }

            ShowError(friendlyMessage + " التفاصيل: " + message);
        }

        private string GetInnerExceptionMessage(Exception ex)
        {
            Exception current = ex;
            while (current.InnerException != null)
                current = current.InnerException;

            string message = current.Message;
            if (string.IsNullOrWhiteSpace(message)) return "";
            if (message.Length > 700) message = message.Substring(0, 700);
            return message;
        }

        // =========================================================
        // Dispose
        // =========================================================
        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}