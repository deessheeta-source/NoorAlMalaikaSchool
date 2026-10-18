using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Data.Helpers
{
    public static class EnumHelper
    {
        private static readonly CultureInfo EnUs = new CultureInfo("en-US");

        public static EducationStage GetCurrentStage()
        {
            var context = HttpContext.Current;
            if (context == null) return EducationStage.Basic;

            string stageParam = context.Request.QueryString["stage"];
            EducationStage stage;
            if (!string.IsNullOrEmpty(stageParam) && Enum.TryParse<EducationStage>(stageParam, true, out stage))
            {
                context.Session["Stage"] = stage.ToString();
                return stage;
            }

            string sessionStage = context.Session["Stage"] as string;
            if (!string.IsNullOrEmpty(sessionStage) && Enum.TryParse<EducationStage>(sessionStage, true, out stage))
                return stage;

            return EducationStage.Basic;
        }

        // ترتيب الصفوف تصاعدياً
        public static List<ClassRoom> SortClassRooms(IEnumerable<ClassRoom> classRooms)
        {
            return classRooms
                .OrderBy(c => GetClassSortKey(c.Name))
                .ThenBy(c => c.Name)
                .ToList();
        }

        private static int GetClassSortKey(string name)
        {
            string n = (name ?? "").Trim();
            if (n.Contains("ثانوي"))
            {
                if (n.Contains("أول") || n.Contains("الأول")) return 200 + 1;
                if (n.Contains("ثاني") || n.Contains("الثاني")) return 200 + 2;
                if (n.Contains("ثالث") || n.Contains("الثالث")) return 200 + 3;
                return 200 + 99;
            }
            return 100 + GetBasicClassNumber(n);
        }

        private static int GetBasicClassNumber(string name)
        {
            var words = new string[][]
            {
                new[] { "الأول", "أول" }, new[] { "الثاني", "ثاني" }, new[] { "الثالث", "ثالث" },
                new[] { "الرابع", "رابع" }, new[] { "الخامس", "خامس" }, new[] { "السادس", "سادس" },
                new[] { "السابع", "سابع" }, new[] { "الثامن", "ثامن" }, new[] { "التاسع", "تاسع" }
            };
            for (int i = 0; i < words.Length; i++)
            {
                foreach (var w in words[i])
                {
                    if (name.Contains(w)) return i + 1;
                }
            }
            return 99;
        }

        public static EducationStage GetStageForGradeLevel(GradeLevel gradeLevel)
        {
            if (gradeLevel == null) return EducationStage.Basic;
            return gradeLevel.Stage;
        }

        public static IQueryable<int> GetGradeLevelIdsForStage(SchoolDbContext db, EducationStage stage)
        {
            return db.GradeLevels
                .Where(g => g.Stage == stage)
                .Select(g => g.Id);
        }

        public static int[] GetClassRoomIdsForStage(SchoolDbContext db, EducationStage stage)
        {
            var gradeLevelIds = GetGradeLevelIdsForStage(db, stage);
            return db.ClassRooms
                .Where(c => gradeLevelIds.Contains(c.GradeLevelId))
                .Select(c => c.Id)
                .Distinct()
                .ToArray();
        }

        // =========================================================
        // هذه الدالة تقرأ [Display(Name)] تلقائياً من الـ Enum
        // لذا أي تعديل على Display(Name) يظهر فوراً دون تعديل هنا
        // =========================================================
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
            if (attribute != null) return attribute.Name;
            return value.ToString();
        }

        public static string GetStageArabic(string stageValue)
        {
            if (string.IsNullOrEmpty(stageValue)) return "";
            if (stageValue == "Basic" || stageValue == "1") return "المرحلة الأساسية";
            if (stageValue == "Secondary" || stageValue == "2") return "المرحلة الثانوية";
            return stageValue;
        }

        public static string CurrentUserName
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.Session == null) return "";
                return (HttpContext.Current.Session["DisplayName"] as string)
                    ?? (HttpContext.Current.Session["Username"] as string)
                    ?? "";
            }
        }

        public static string FormatCurrency(decimal amount)
        {
            return amount.ToString("N2", EnUs);
        }

        public static string FormatNumber(int number)
        {
            return number.ToString("N0", EnUs);
        }

        public static string FormatNumber(decimal number)
        {
            return number.ToString("N0", EnUs);
        }

        public static string FormatPercent(double percent)
        {
            return percent.ToString("F1", EnUs) + "%";
        }

        public static string GetPaymentMethodName(PaymentMethod method)
        {
            switch (method)
            {
                case PaymentMethod.Cash: return "نقدي";
                case PaymentMethod.BankTransfer: return "تحويل بنكي";
                case PaymentMethod.Check: return "شيك";
                case PaymentMethod.Electronic: return "بطاقة مصرفية";
                default: return method.ToString();
            }
        }
    }
}