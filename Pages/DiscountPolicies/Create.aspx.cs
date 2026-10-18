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
    public partial class Create : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

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
                    LoadTypes();
                }
            }
            catch (Exception ex)
            {
                ShowError("تعذر تحميل صفحة إضافة سياسة الخصم.", ex);
            }
        }

        // =========================================================
        // تحميل أنواع السياسات
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
        // حفظ السياسة
        // =========================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ClearMessage();

                string name = txtName.Text == null ? "" : txtName.Text.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    ShowValidationError("يرجى إدخال اسم السياسة.");
                    return;
                }

                DiscountType discountType;
                if (!Enum.TryParse(ddlType.SelectedValue, true, out discountType))
                {
                    ShowValidationError("نوع السياسة غير صحيح.");
                    return;
                }

                DiscountKind discountKind;
                if (!Enum.TryParse(ddlKind.SelectedValue, true, out discountKind))
                {
                    ShowValidationError("نوع القيمة غير صحيح.");
                    return;
                }

                // ✅ السياسات التي يُدخَل مقدارها يدوياً (تمت إضافة AnnualSubscription)
                bool isManualType =
                    discountType == DiscountType.Sibling ||
                    discountType.ToString().Equals("EarlyBird", StringComparison.OrdinalIgnoreCase) ||
                    discountType.ToString().Equals("AnnualSubscription", StringComparison.OrdinalIgnoreCase);

                bool manualValue = isManualType && chkManualValue.Checked;

                int integerValue = 0;

                if (manualValue)
                {
                    integerValue = 0;
                }
                else
                {
                    if (!int.TryParse(txtValue.Text.Trim(), out integerValue) ||
                        integerValue < 0)
                    {
                        ShowValidationError("القيمة يجب أن تكون رقمًا صحيحًا موجبًا أو صفرًا.");
                        return;
                    }

                    if (discountKind == DiscountKind.Percentage && integerValue > 100)
                    {
                        ShowValidationError("نسبة الخصم لا يمكن أن تتجاوز 100%.");
                        return;
                    }
                }

                FeeType parsedFeeType;
                FeeType? appliesToFeeType =
                    Enum.TryParse(ddlAppliesToFeeType.SelectedValue, true, out parsedFeeType)
                        ? (FeeType?)parsedFeeType
                        : null;

                var policy = new DiscountPolicy
                {
                    Name = name,
                    Type = discountType,
                    Kind = manualValue ? DiscountKind.Amount : discountKind,
                    Value = integerValue,
                    ApplyAutomatically = manualValue ? false : chkAuto.Checked,
                    IsActive = chkActive.Checked,
                    Description = txtDescription.Text == null ? "" : txtDescription.Text.Trim(),
                    AppliesToFeeType = appliesToFeeType
                };

                db.DiscountPolicies.Add(policy);
                db.SaveChanges();

                Response.Redirect("Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                ShowError("تعذر إضافة سياسة الخصم.", ex);
            }
        }

        // =========================================================
        // رسائل
        // =========================================================
        private void ShowValidationError(string message)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.CssClass = "dp-message danger";
        }

        private void ShowError(string message, Exception ex)
        {
            System.Diagnostics.Trace.TraceError("DiscountPolicies.Create Error: " + ex);

            string details = GetInnerExceptionMessage(ex);
            string finalMessage = message;
            if (!string.IsNullOrWhiteSpace(details))
                finalMessage += " التفاصيل: " + details;

            lblMessage.Text = Server.HtmlEncode(finalMessage);
            lblMessage.CssClass = "dp-message danger";
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

        private void ClearMessage()
        {
            lblMessage.Text = "";
            lblMessage.CssClass = "dp-message";
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