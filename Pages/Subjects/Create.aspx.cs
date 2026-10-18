using System;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Pages.Subjects
{
    public partial class Create : Page
    {
        private readonly SchoolDbContext _context = new SchoolDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Local authorization guard: this page must not depend solely on the Site.Master check.
            SecurityHelper.DenyUnlessPath();
            if (!IsPostBack)
            {
                LoadCurrentStage();
            }
        }

        // =========================================================
        // المرحلة الافتراضية
        // =========================================================
        private void LoadCurrentStage()
        {
            var stage = NoorAlMalaikaSchool.Data.Helpers.EnumHelper.GetCurrentStage();

            string stageValue = stage.ToString();

            if (ddlStage.Items.FindByValue(stageValue) != null)
            {
                ddlStage.SelectedValue = stageValue;
            }

            UpdateCategory();
        }

        // =========================================================
        // الصف / الفرع حسب المرحلة
        // =========================================================
        private void UpdateCategory()
        {
            bool secondary =
                ddlStage.SelectedValue ==
                EducationStage.Secondary.ToString();

            ddlCategory.Enabled = secondary;

            if (!secondary)
            {
                ddlCategory.SelectedValue = "None";
            }
        }

        // =========================================================
        // تغيير المرحلة
        // =========================================================
        protected void ddlStage_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            UpdateCategory();
        }

        // =========================================================
        // حفظ المادة
        // =========================================================
        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                Page.Validate("vgSave");

                if (!Page.IsValid)
                {
                    return;
                }

                // -------------------------------------------------
                // اسم المادة
                // -------------------------------------------------
                string name = txtName.Text.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    ShowError("يرجى إدخال اسم المادة.");
                    return;
                }

                // -------------------------------------------------
                // المرحلة
                // -------------------------------------------------
                EducationStage stage;

                if (!Enum.TryParse(
                    ddlStage.SelectedValue,
                    true,
                    out stage))
                {
                    ShowError("المرحلة الدراسية غير صحيحة.");
                    return;
                }

                // -------------------------------------------------
                // الصف / الفرع
                // -------------------------------------------------
                SubjectCategory category;

                if (!Enum.TryParse(
                    ddlCategory.SelectedValue,
                    true,
                    out category))
                {
                    ShowError("الصف أو الفرع غير صحيح.");
                    return;
                }

                // -------------------------------------------------
                // في المرحلة الأساسية لا يوجد فرع ثانوي
                // -------------------------------------------------
                if (stage != EducationStage.Secondary)
                {
                    category = SubjectCategory.None;
                }

                // -------------------------------------------------
                // الرمز
                // -------------------------------------------------
                string code = txtCode.Text.Trim();

                if (string.IsNullOrWhiteSpace(code))
                {
                    code = null;
                }

                if (code != null && code.Length > 20)
                {
                    ShowError("رمز المادة يجب ألا يتجاوز 20 حرفاً.");
                    return;
                }

                // -------------------------------------------------
                // الوصف
                // -------------------------------------------------
                string description = txtDescription.Text.Trim();

                if (description.Length > 500)
                {
                    ShowError("وصف المادة يجب ألا يتجاوز 500 حرف.");
                    return;
                }

                // -------------------------------------------------
                // منع تكرار المادة بنفس المرحلة والفرع
                // -------------------------------------------------
                bool duplicateName =
                    _context.Subjects.Any(s =>
                        s.Name == name &&
                        s.Stage == stage &&
                        s.Category == category);

                if (duplicateName)
                {
                    ShowError(
                        "هذه المادة موجودة مسبقاً لنفس المرحلة والصف / الفرع.");
                    return;
                }

                // -------------------------------------------------
                // منع تكرار الرمز إذا تم إدخاله
                // -------------------------------------------------
                if (!string.IsNullOrWhiteSpace(code))
                {
                    bool duplicateCode =
                        _context.Subjects.Any(s =>
                            s.Code == code);

                    if (duplicateCode)
                    {
                        ShowError(
                            "رمز المادة '" + code +
                            "' مستخدم مسبقاً. يرجى إدخال رمز آخر.");
                        return;
                    }
                }

                // -------------------------------------------------
                // إنشاء المادة
                // -------------------------------------------------
                var subject = new Subject
                {
                    Name = name,
                    Code = code,
                    Description = description,
                    Stage = stage,
                    Category = category
                };

                _context.Subjects.Add(subject);

                _context.SaveChanges();

                // -------------------------------------------------
                // نجاح
                // -------------------------------------------------
                Response.Redirect(
                    "Default.aspx",
                    false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string message =
                    string.Join(
                        " | ",
                        ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x =>
                                x.PropertyName + ": " + x.ErrorMessage));

                ShowError(
                    "تعذر حفظ المادة بسبب بيانات غير صحيحة: " +
                    message);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                string details = GetInnerExceptionMessage(ex);

                ShowError(
                    "تعذر حفظ المادة في قاعدة البيانات. " +
                    details);
            }
            catch (Exception ex)
            {
                string details = GetInnerExceptionMessage(ex);

                ShowError(
                    "حدث خطأ أثناء تنفيذ العملية: " +
                    details);
            }
        }

        // =========================================================
        // إلغاء
        // =========================================================
        protected void btnCancel_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect(
                "Default.aspx",
                false);

            Context.ApplicationInstance.CompleteRequest();
        }

        // =========================================================
        // استخراج سبب الخطأ الحقيقي
        // =========================================================
        private string GetInnerExceptionMessage(Exception ex)
        {
            Exception current = ex;

            while (current.InnerException != null)
            {
                current = current.InnerException;
            }

            string message = current.Message;

            if (string.IsNullOrWhiteSpace(message))
            {
                return "يرجى مراجعة سجل النظام.";
            }

            return message;
        }

        // =========================================================
        // عرض الخطأ
        // =========================================================
        private void ShowError(string message)
        {
            lblMessage.CssClass =
                "alert alert-danger d-block mb-3";

            lblMessage.Text =
                Server.HtmlEncode(message);
        }

        // =========================================================
        // التخلص من DbContext
        // =========================================================
        protected override void OnUnload(EventArgs e)
        {
            if (_context != null)
            {
                _context.Dispose();
            }

            base.OnUnload(e);
        }
    }
}