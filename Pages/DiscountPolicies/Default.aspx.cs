using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.DiscountPolicies
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadPolicies();
            }
        }

        // =========================================================
        // تحميل سياسات الخصم والإعفاءات
        // =========================================================
        private void LoadPolicies()
        {
            var policies = db.DiscountPolicies
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Name)
                .ToList()
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.IsActive,

                    // ✅ "لا" للسياسات اليدوية
                    ApplyAutomatically = IsManualPolicy(p) ? false : p.ApplyAutomatically,

                    TypeName = p.Type.GetDisplayName(),
                    KindValue = BuildKindValue(p),

                    AutoText = (IsManualPolicy(p) ? false : p.ApplyAutomatically) ? "نعم" : "لا",

                    ScopeName = p.AppliesToFeeType.HasValue
                        ? p.AppliesToFeeType.Value.GetDisplayName()
                        : "كل الرسوم"
                }).ToList();

            rptPolicies.DataSource = policies;
            rptPolicies.DataBind();

            pnlTable.Visible = policies.Count > 0;
            pnlEmpty.Visible = policies.Count == 0;

            lblTotalPolicies.Text = policies.Count.ToString();
            lblCountPill.Text = policies.Count.ToString();

            lblActivePolicies.Text = policies.Count(p => p.IsActive).ToString();
            lblAutoPolicies.Text = policies.Count(p => p.ApplyAutomatically).ToString();
        }

        // =========================================================
        // ✅ كشف السياسات اليدوية
        //    (إخوة / تسجيل مبكر / اشتراك سنوي / أي سياسة بقيمة صفر)
        // =========================================================
        private static bool IsManualPolicy(DiscountPolicy p)
        {
            if (p == null) return false;

            // سياسة بقيمة صفر → مقدارها يُدخَل يدوياً
            if (p.Value == 0m) return true;

            string name = (p.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name)) return false;

            return name.Contains("إخوة")
                || name.Contains("اخوة")
                || name.Contains("أخوة")
                || name.Contains("تسجيل المبكر")
                || name.Contains("تسجيل مبكر")
                || name.Contains("الاشتراك السنوي") // ✅ تمت الإضافة
                || name.ToLower().Contains("sibling")
                || name.ToLower().Contains("early")
                || name.ToLower().Contains("annualsubscription"); // ✅ تمت الإضافة
        }

        // =========================================================
        // ✅ عرض قيمة السياسة
        // =========================================================
        private static string BuildKindValue(DiscountPolicy p)
        {
            if (IsManualPolicy(p))
                return "يدوي عند الفاتورة";

            return p.Kind == DiscountKind.Percentage
                ? p.Value.ToString("N0") + "%"
                : p.Value.ToString("N2") + " د.ل";
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }
    }
}