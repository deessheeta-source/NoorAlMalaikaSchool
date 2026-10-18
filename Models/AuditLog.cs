using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class AuditLog
    {
        [Display(Name = "المعرف")]
        public int Id { get; set; }

        [Display(Name = "التاريخ والوقت")]
        public DateTime Timestamp { get; set; }

        [Display(Name = "المستخدم")]
        [StringLength(100)]
        public string UserName { get; set; }

        [Display(Name = "الإجراء")]
        [StringLength(50)]
        public string Action { get; set; }

        [Display(Name = "الكيان")]
        [StringLength(100)]
        public string EntityType { get; set; }

        [Display(Name = "معرف الكيان")]
        [StringLength(50)]
        public string EntityId { get; set; }

        [Display(Name = "التفاصيل")]
        public string Details { get; set; }

        [Display(Name = "القيم السابقة")]
        public string OldValues { get; set; }

        [Display(Name = "القيم الجديدة")]
        public string NewValues { get; set; }

        [Display(Name = "الحقول المتغيرة")]
        public string ChangedFields { get; set; }

        [Display(Name = "عنوان IP")]
        [StringLength(50)]
        public string IpAddress { get; set; }

        public string TimestampText { get; set; }
    }
}