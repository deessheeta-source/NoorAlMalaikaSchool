namespace NoorAlMalaikaSchool.Models
{
    public class MonthlySheetMonth
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        // تسمية (رقم) الشهر يتم إدخالها يدوياً من الصفحة
        public string Month1 { get; set; }

        public string Month2 { get; set; }

        public string Month3 { get; set; }

        public virtual Student Student { get; set; }
    }
}
