using System;
using System.Text;
using System.Web;
using System.Web.UI;

namespace NoorAlMalaikaSchool.Controls
{
    public partial class FinanceNav : System.Web.UI.UserControl
    {
        /// <summary>
        /// مفتاح الصفحة الحالية لتمييزها داخل الشريط.
        /// القيم: Fees, StudentFees, Invoices, Payments,
        /// DiscountPolicies, FinancialBenefits, FinancialStatement
        /// </summary>
        public string ActivePage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            litNav.Text = BuildNav();
        }

        private string BuildNav()
        {
            string stage = Session["Stage"] as string ?? "Basic";
            string stageEnc = HttpUtility.UrlEncode(stage);

            var items = new[]
            {

               
                new { Key = "Fees",               Text = "إدارة الرسوم",           Url = "~/Pages/Fees/Default.aspx" },
                new { Key = "DiscountPolicies",   Text = "إدارة الخصم",            Url = "~/Pages/DiscountPolicies/Default.aspx" },
                new { Key = "Invoices",           Text = "إدارة الفواتير",               Url = "~/Pages/Invoices/Default.aspx" },
                new { Key = "FinancialReport",    Text = "التقرير المالي",         Url = "~/Pages/Reports/FinancialReport.aspx" },
                new { Key = "FinancialStatement", Text = "الكشف المالي",           Url = "~/Pages/Reports/StudentFinancialStatement.aspx" }
            };

            var sb = new StringBuilder();

            foreach (var item in items)
            {
                string cssClass = "fin-chip";

                if (!string.IsNullOrWhiteSpace(ActivePage)
                    && string.Equals(ActivePage, item.Key, StringComparison.OrdinalIgnoreCase))
                {
                    cssClass += " active";
                }

                string href = ResolveClientUrl(item.Url) + "?stage=" + stageEnc;

                sb.AppendFormat("<a href=\"{0}\" class=\"{1}\">{2}</a>",
                    HttpUtility.HtmlAttributeEncode(href),
                    cssClass,
                    HttpUtility.HtmlEncode(item.Text));
            }

            return sb.ToString();
        }
    }
}