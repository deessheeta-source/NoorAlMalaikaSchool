namespace NoorAlMalaikaSchool.Models
{
    public class MonthlySheetEvaluation
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        // السلوك / الاهتمام بالواجب / الانتباه / علاقته مع زملائه / الغياب
        public string Category { get; set; }

        public string Month1 { get; set; }

        public string Month2 { get; set; }

        public string Month3 { get; set; }

        public virtual Student Student { get; set; }
    }
}
