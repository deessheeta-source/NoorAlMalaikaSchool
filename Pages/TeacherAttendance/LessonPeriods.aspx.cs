using System;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.TeacherAttendance
{
    public partial class LessonPeriods : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadPeriods();
            }
        }

        #endregion

        #region Load

        private void LoadPeriods()
        {
            var periods = db.LessonPeriods
                .OrderBy(p => p.PeriodNumber)
                .Select(p => new
                {
                    p.Id,
                    p.PeriodNumber,
                    p.StartTime,
                    p.EndTime,
                    p.DurationMinutes,
                    p.IsActive
                })
                .ToList()
                .Select(p => new
                {
                    p.Id,
                    p.PeriodNumber,
                    StartTimeText = p.StartTime.ToString(@"hh\:mm"),
                    EndTimeText = p.EndTime.ToString(@"hh\:mm"),
                    p.DurationMinutes,
                    p.IsActive
                })
                .ToList();

            gvPeriods.DataSource = periods;
            gvPeriods.DataBind();

            // ✅ تحديث عدد الحصص
            if (lblPeriodsCount != null)
                lblPeriodsCount.Text = periods.Count.ToString();

            // ✅ إظهار Empty State عند عدم وجود حصص
            if (pnlEmpty != null)
                pnlEmpty.Visible = periods.Count == 0;
        }

        #endregion

        #region Grid Events

        protected void gvPeriods_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvPeriods.EditIndex = e.NewEditIndex;
            LoadPeriods();
        }

        protected void gvPeriods_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvPeriods.EditIndex = -1;
            LoadPeriods();
        }

        protected void gvPeriods_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvPeriods.DataKeys[e.RowIndex].Value);
                var period = db.LessonPeriods.Find(id);
                if (period == null)
                {
                    ShowMessage("الحصة غير موجودة", false);
                    return;
                }

                var row = gvPeriods.Rows[e.RowIndex];
                var txtStart = (System.Web.UI.WebControls.TextBox)row.FindControl("txtStart");
                var txtEnd = (System.Web.UI.WebControls.TextBox)row.FindControl("txtEnd");
                var ddlActive = (System.Web.UI.WebControls.DropDownList)row.FindControl("ddlActiveEdit");

                TimeSpan start, end;
                if (!TimeSpan.TryParse(txtStart.Text.Trim(), out start) ||
                    !TimeSpan.TryParse(txtEnd.Text.Trim(), out end))
                {
                    ShowMessage("أوقات الحصة غير صحيحة (الصيغة: HH:mm)", false);
                    return;
                }
                if (end <= start)
                {
                    ShowMessage("وقت النهاية يجب أن يكون بعد وقت البداية", false);
                    return;
                }

                period.StartTime = start;
                period.EndTime = end;
                period.DurationMinutes = (int)Math.Round((end - start).TotalMinutes);
                period.IsActive = ddlActive.SelectedValue != "False";

                db.SaveChanges();

                gvPeriods.EditIndex = -1;
                LoadPeriods();
                ShowMessage("تم حفظ أوقات الحصة بنجاح", true);
            }
            catch (Exception ex)
            {
                ShowMessage(ErrorMessageHelper.GetUserMessage(ex, GetType().FullName), false);
            }
        }

        #endregion

        #region Messages / Dispose

        private void ShowMessage(string text, bool success)
        {
            if (lblMessage == null) return;

            lblMessage.CssClass = success ? "ta-alert success" : "ta-alert danger";
            lblMessage.Text =
                "<i class='fas " + (success ? "fa-check-circle" : "fa-exclamation-triangle") + "'></i>" +
                "<span>" + Server.HtmlEncode(text) + "</span>";
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }

        #endregion
    }
}