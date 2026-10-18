using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Controls
{
    /// <summary>
    /// شريط المراحل الموحد لسير عمل الاستحقاق المالي للمعلمين:
    /// 1) توليد الحصص، 2) تسجيل الساعات، 3) اعتماد اليوم، 4) جاهزية الفترة، 5) الاستحقاق المالي.
    /// حالة كل مرحلة تُحسب من البيانات (وليست مجرد عنصر تجميلي).
    /// </summary>
    public partial class AttendanceStepper : UserControl
    {
        public int CurrentStep
        {
            get
            {
                object o = ViewState["AS_CurrentStep"];
                return o is int ? (int)o : 1;
            }
            set { ViewState["AS_CurrentStep"] = value; }
        }

        public string ContextDate
        {
            get { return ViewState["AS_ContextDate"] as string; }
            set { ViewState["AS_ContextDate"] = value; }
        }

        public int ContextPeriodId
        {
            get
            {
                object o = ViewState["AS_ContextPeriodId"];
                return o is int ? (int)o : 0;
            }
            set { ViewState["AS_ContextPeriodId"] = value; }
        }

        private class StepInfo
        {
            public int Number;
            public string Title;
            public string StateClass;
            public string StateText;
        }

        protected string RenderSteps()
        {
            DateTime? date = null;
            DateTime parsedDate;
            if (!string.IsNullOrEmpty(ContextDate) && DateTime.TryParse(ContextDate, out parsedDate))
                date = parsedDate.Date;

            var steps = new List<StepInfo>();
            using (var db = new SchoolDbContext())
            {
                steps.Add(BuildStep1(db, date));
                steps.Add(BuildStep2(db, date));
                steps.Add(BuildStep3(db, date));
                steps.Add(BuildStep4(db));
                steps.Add(BuildStep5(db));
            }

            if (CurrentStep < 1) CurrentStep = 1;
            if (CurrentStep > 5) CurrentStep = 5;

            var sb = new StringBuilder();
            sb.Append("<div class=\"att-stepper-wrap\" dir=\"rtl\"><style>");
            sb.Append(".att-stepper{display:flex;align-items:stretch;gap:6px;flex-wrap:wrap;}")
              .Append(".att-step{flex:1 1 0;min-width:112px;display:flex;align-items:center;gap:8px;padding:8px 10px;border:1px solid #e2e8f0;border-radius:10px;background:#fff;color:#334155;position:relative;text-decoration:none!important;}")
              .Append(".att-step:hover{border-color:#b7cdc2;box-shadow:0 1px 4px rgba(0,0,0,.06);}")
              .Append(".att-num{width:26px;height:26px;border-radius:50%;display:flex;align-items:center;justify-content:center;font-size:12px;font-weight:700;background:#eef2f7;color:#475569;flex:0 0 26px;}")
              .Append(".att-body{min-width:0;}.att-title{font-size:12px;font-weight:700;color:#334155;line-height:1.35;}.att-state{font-size:11px;color:#64748b;line-height:1.3;}")
              .Append(".att-step.now{border-color:#0d6efd;background:#eef6ff;}.att-step.now .att-num{background:#0d6efd;color:#fff;}")
              .Append(".att-step.done{border-color:#9dc9ac;background:#f4fbf6;}.att-step.done .att-num{background:#198754;color:#fff;}")
              .Append(".att-step.warn{border-color:#f1b7b7;background:#fff7f7;}.att-step.warn .att-num{background:#dc3545;color:#fff;}")
              .Append(".att-step.idle{background:#fafbfc;}.att-step.idle .att-num{background:#eef1f5;color:#94a3b8;}")
              .Append("</style><div class=\"att-stepper\">");

            string basePath = ResolveUrl("~/Pages/TeacherAttendance/");
            string payrollPath = ContextPeriodId > 0
                ? ResolveUrl("~/Pages/Payroll/Details.aspx?id=" + ContextPeriodId)
                : ResolveUrl("~/Pages/Payroll/Create.aspx");

            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                if (step.Number == CurrentStep)
                {
                    step.StateClass = "now";
                    step.StateText = "أنت هنا";
                }

                if (i > 0)
                {
                    sb.Append("<div class=\"att-step-arrow\" style=\"display:flex;align-items:center;color:#cbd5e1;\"><i class=\"fas fa-chevron-left\"></i></div>");
                }

                string url = "#";
                string dateQuery = date.HasValue
                    ? "date=" + HttpUtility.UrlEncode(date.Value.ToString("yyyy-MM-dd"))
                    : "";

                switch (step.Number)
                {
                    case 1:
                        url = basePath + "Generate.aspx" + (dateQuery.Length > 0 ? "?" + dateQuery : "");
                        break;
                    case 2:
                        url = basePath + "Record.aspx?tab=record"
                            + (dateQuery.Length > 0 ? "&" + dateQuery : "");
                        break;
                    case 3:
                        url = basePath + "Record.aspx?tab=review"
                            + (dateQuery.Length > 0 ? "&" + dateQuery : "");
                        break;
                    case 4:
                        url = basePath + "Readiness.aspx"
                            + (ContextPeriodId > 0 ? "?periodId=" + ContextPeriodId : "");
                        break;
                    case 5:
                        url = payrollPath;
                        break;
                }

                sb.Append("<a class=\"att-step ").Append(step.StateClass).Append("\" href=\"")
                  .Append(HttpUtility.HtmlAttributeEncode(url)).Append("\">");
                sb.Append("<div class=\"att-num\">").Append(step.Number).Append("</div>");
                sb.Append("<div class=\"att-body\">");
                sb.Append("<div class=\"att-title\">").Append(HttpUtility.HtmlEncode(step.Title)).Append("</div>");
                sb.Append("<div class=\"att-state\">").Append(HttpUtility.HtmlEncode(step.StateText)).Append("</div>");
                sb.Append("</div></a>");
            }

            sb.Append("</div></div>");
            return sb.ToString();
        }

        private StepInfo BuildStep1(SchoolDbContext db, DateTime? date)
        {
            var step = new StepInfo { Number = 1, Title = "توليد الحصص" };
            if (!date.HasValue)
            {
                step.StateClass = "idle";
                step.StateText = "—";
                return step;
            }

            if (date.Value.DayOfWeek == DayOfWeek.Friday || date.Value.DayOfWeek == DayOfWeek.Saturday)
            {
                step.StateClass = "idle";
                step.StateText = "عطلة";
                return step;
            }

            if (db.DailyLessons.Any(d => d.Date == date.Value))
            {
                step.StateClass = "done";
                step.StateText = "مكتمل";
            }
            else
            {
                step.StateClass = "warn";
                step.StateText = "غير منجز";
            }
            return step;
        }

        private StepInfo BuildStep2(SchoolDbContext db, DateTime? date)
        {
            var step = new StepInfo { Number = 2, Title = "تسجيل الساعات" };
            if (!date.HasValue)
            {
                step.StateClass = "idle";
                step.StateText = "—";
                return step;
            }

            var validation = new TeacherAttendanceFinancialService(db).ValidateDate(date.Value, false);
            if (validation.ScheduledLessons == 0)
            {
                step.StateClass = "idle";
                step.StateText = "لا حصص";
                return step;
            }

            if (validation.MissingLessons == 0 && validation.RecordedLessons == validation.ScheduledLessons)
            {
                step.StateClass = "done";
                step.StateText = "مكتمل";
            }
            else if (validation.RecordedLessons > 0)
            {
                step.StateClass = "warn";
                step.StateText = "جزئي";
            }
            else
            {
                step.StateClass = "warn";
                step.StateText = "لم يبدأ";
            }
            return step;
        }

        private StepInfo BuildStep3(SchoolDbContext db, DateTime? date)
        {
            var step = new StepInfo { Number = 3, Title = "اعتماد اليوم" };
            if (!date.HasValue)
            {
                step.StateClass = "idle";
                step.StateText = "—";
                return step;
            }

            int total = db.AttendanceRecords.Count(r => r.Date == date.Value);
            int approved = db.AttendanceRecords.Count(r => r.Date == date.Value && r.IsApproved);

            if (total == 0)
            {
                step.StateClass = "idle";
                step.StateText = "لا سجلات";
                return step;
            }

            if (approved == total)
            {
                step.StateClass = "done";
                step.StateText = "معتمد";
            }
            else
            {
                step.StateClass = "warn";
                step.StateText = "غير معتمد";
            }
            return step;
        }

        private StepInfo BuildStep4(SchoolDbContext db)
        {
            var step = new StepInfo { Number = 4, Title = "جاهزية الفترة" };
            if (ContextPeriodId <= 0)
            {
                step.StateClass = "idle";
                step.StateText = "—";
                return step;
            }

            var validation = new TeacherAttendanceFinancialService(db).ValidatePeriod(ContextPeriodId);
            if (validation.IsReady)
            {
                step.StateClass = "done";
                step.StateText = "جاهز";
            }
            else
            {
                step.StateClass = "warn";
                step.StateText = "غير جاهز";
            }
            return step;
        }

        private StepInfo BuildStep5(SchoolDbContext db)
        {
            var step = new StepInfo { Number = 5, Title = "الاستحقاق المالي" };
            if (ContextPeriodId <= 0)
            {
                step.StateClass = "idle";
                step.StateText = "—";
                return step;
            }

            var period = db.PayrollPeriods.Find(ContextPeriodId);
            if (period == null)
            {
                step.StateClass = "idle";
                step.StateText = "—";
                return step;
            }

            if (period.Status == PayrollPeriodStatus.Approved || period.Status == PayrollPeriodStatus.Closed)
            {
                step.StateClass = "done";
                step.StateText = "منفذ";
            }
            else if (db.TeacherPayrolls.Any(p => p.PayrollPeriodId == ContextPeriodId))
            {
                step.StateClass = "warn";
                step.StateText = "محسوب وغير معتمد";
            }
            else
            {
                step.StateClass = "idle";
                step.StateText = "لم يُحسب";
            }
            return step;
        }
    }
}