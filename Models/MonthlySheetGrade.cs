namespace NoorAlMalaikaSchool.Models
{
    public class MonthlySheetGrade
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string Subject { get; set; }

        public int MonthNumber { get; set; }

        // الأنشطة اليومية
        public string DailyActivity { get; set; }

        // متوسط التطبيقات
        public string ApplicationAverage { get; set; }

        // المجموع
        public string Total { get; set; }

        // درجة التلميذ في الامتحان النصفي
        public string MidtermStudent { get; set; }

        public virtual Student Student { get; set; }
    }
}
