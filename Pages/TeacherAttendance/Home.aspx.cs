using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;


namespace NoorAlMalaikaSchool.Pages.TeacherAttendance
{
    public partial class Home : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        private const string FlashKey = "TeacherAttendance_FlashMessage";

        // ====== حالات خطوات العمل اليومي ======
        public bool Step1Done { get; set; }
        public bool Step2Done { get; set; }
        public bool Step3Done { get; set; }

        public bool Step1Active { get; set; }
        public bool Step2Active { get; set; }
        public bool Step3Active { get; set; }
        public bool Step4Active { get; set; }

        // ====== اسم المرحلة الحالية ======
        public string CurrentStageName { get; set; }

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            if (IsPostBack) return;

            try
            {
                ShowFlashIfAny();
                SetTodayHeroLabels();
                ComputeStepStates();
                pnlTodayStatus.Visible = true;
            }
            catch
            {
                pnlTodayStatus.Visible = false;
            }
        }

        #endregion

        #region Hero Labels (اليوم + التاريخ)

        /// <summary>
        /// يعرض اسم اليوم والتاريخ بالعربية في بطاقات الـ Hero.
        /// </summary>
        private void SetTodayHeroLabels()
        {
            try
            {
                var cultAr = new CultureInfo("ar-LY");

                string dayName = DateTime.Today.ToString("dddd", cultAr);
                string dateShort = DateTime.Today.ToString("dd MMM yyyy", cultAr);

                if (lblTodayDayOnly != null)
                    lblTodayDayOnly.Text = dayName;

                if (lblTodayDateShort != null)
                    lblTodayDateShort.Text = dateShort;
            }
            catch
            {
                // fallback بصيغة إنجليزية إن فشل الـ culture
                if (lblTodayDayOnly != null)
                    lblTodayDayOnly.Text = DateTime.Today.ToString("dddd");

                if (lblTodayDateShort != null)
                    lblTodayDateShort.Text = DateTime.Today.ToString("dd MMM yyyy");
            }
        }

        #endregion

        #region Step States Computation

        /// <summary>
        /// يحسب حالة كل خطوة من البيانات الفعلية لليوم — مقيّد بالمرحلة الحالية.
        /// </summary>
        private void ComputeStepStates()
        {
            DateTime today = DateTime.Today;
            lblTodayDate.Text = today.ToString("yyyy-MM-dd");

            // ✅ المرحلة الحالية (من Session عبر EnumHelper)
            EducationStage stage = EnumHelper.GetCurrentStage();
            CurrentStageName = stage == EducationStage.Basic
                ? "المرحلة الأساسية"
                : "المرحلة الثانوية";

            if (lblCurrentStage != null)
                lblCurrentStage.Text = CurrentStageName;

            // ✅ قائمة معرفات المعلمين في المرحلة الحالية فقط
            var teacherIds = db.Teachers
                .Where(t => t.Stage == stage && t.IsActive)
                .Select(t => t.Id)
                .ToList();

            // ✅ الإحصائيات مقيّدة بهذه القائمة
            int lessons = teacherIds.Count == 0
                ? 0
                : db.DailyLessons.Count(d =>
                    d.Date == today &&
                    teacherIds.Contains(d.TeacherId));

            int recorded = teacherIds.Count == 0
                ? 0
                : db.AttendanceRecords.Count(r =>
                    r.Date == today &&
                    teacherIds.Contains(r.TeacherId));

            int approved = teacherIds.Count == 0
                ? 0
                : db.AttendanceRecords.Count(r =>
                    r.Date == today &&
                    r.IsApproved &&
                    teacherIds.Contains(r.TeacherId));

            // ====== تحديد اكتمال الخطوات ======
            Step1Done = lessons > 0;
            Step2Done = lessons > 0 && recorded >= lessons;
            Step3Done = recorded > 0 && approved >= recorded;

            // ====== الخطوة النشطة (واحدة فقط) ======
            Step1Active = !Step1Done;
            Step2Active = Step1Done && !Step2Done;
            Step3Active = Step1Done && Step2Done && !Step3Done;
            Step4Active = Step1Done && Step2Done && Step3Done;

            // ====== ملخص اليوم ======
            if (teacherIds.Count == 0)
            {
                lblTodaySummary.Text =
                    "لا يوجد معلمون في " + CurrentStageName + " — راجع بيانات المعلمين.";
            }
            else if (lessons == 0)
            {
                lblTodaySummary.Text =
                    "لا توجد حصص في " + CurrentStageName + " — ابدأ بـ «توليد حصص اليوم».";
            }
            else if (recorded == 0)
            {
                lblTodaySummary.Text =
                    lessons + " حصة في " + CurrentStageName + " بانتظار تسجيل الساعات.";
            }
            else if (approved < recorded)
            {
                lblTodaySummary.Text =
                    recorded + " سجل — بانتظار الاعتماد المالي.";
            }
            else
            {
                lblTodaySummary.Text =
                    "اليوم مكتمل ومعتمد مالياً (" + approved + " سجل) في " + CurrentStageName + ".";
            }
        }

        #endregion

        #region Helpers for ASPX

        protected string GetStep1Class() { return BuildCardClass(Step1Done, Step1Active); }
        protected string GetStep2Class() { return BuildCardClass(Step2Done, Step2Active); }
        protected string GetStep3Class() { return BuildCardClass(Step3Done, Step3Active); }
        protected string GetStep4Class() { return BuildCardClass(false, Step4Active); }

        protected string GetStep1Badge() { return BuildBadge(Step1Done, Step1Active); }
        protected string GetStep2Badge() { return BuildBadge(Step2Done, Step2Active); }
        protected string GetStep3Badge() { return BuildBadge(Step3Done, Step3Active); }
        protected string GetStep4Badge() { return BuildBadge(false, Step4Active); }

        private static string BuildCardClass(bool done, bool active)
        {
            var cls = "home-card";
            if (done) cls += " is-done";
            if (active) cls += " is-active";
            return cls;
        }

        private static string BuildBadge(bool done, bool active)
        {
            if (done)
                return "<span class='card-status st-done'><i class='fas fa-check'></i> مكتمل</span>";

            if (active)
                return "<span class='card-status st-active'><i class='fas fa-play'></i> الخطوة الحالية</span>";

            return "<span class='card-status st-neutral'><i class='fas fa-hourglass-half'></i> لاحقاً</span>";
        }

        #endregion

        #region Flash Message

        /// <summary>
        /// يقرأ رسالة النجاح من Session ويعرضها مرة واحدة ثم يمسحها.
        /// </summary>
        private void ShowFlashIfAny()
        {
            string msg = Session[FlashKey] as string;
            if (string.IsNullOrWhiteSpace(msg)) return;

            Session.Remove(FlashKey);

            lblFlash.CssClass = "flash-card";
            lblFlash.Text =
                "<i class='fas fa-check-circle'></i>" +
                "<span>" + Server.HtmlEncode(msg) + "</span>";
        }

        #endregion

        #region Dispose

        protected override void OnUnload(EventArgs e)
        {
            if (db != null)
                db.Dispose();

            base.OnUnload(e);
        }

        #endregion
    }
}