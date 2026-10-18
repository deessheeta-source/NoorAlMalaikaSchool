using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public enum Gender
    {
        [Display(Name = "ذكر")]
        Male,

        [Display(Name = "أنثى")]
        Female
    }

    public enum EducationStage
    {
        [Display(Name = "أساسي")]
        Basic,

        [Display(Name = "ثانوي")]
        Secondary
    }

    public enum SubjectCategory
    {
        [Display(Name = "عام")]
        None,

        [Display(Name = "أول ثانوي")]
        FirstSecondary,

        [Display(Name = "ثاني وثالث ثانوي علمي")]
        Scientific,

        [Display(Name = "ثاني وثالث ثانوي أدبي")]
        Literary
    }

    public enum EmploymentType
    {
        [Display(Name = "عقد دائم")]
        Permanent,

        [Display(Name = "عقد مؤقت")]
        Temporary,

        [Display(Name = "جزئي")]
        PartTime,

        [Display(Name = "أجر بالحصص")]
        PerLesson,

        [Display(Name = "مشرف دائم - راتب شهري")]
        PermanentSupervisor
    }

    public enum FeeType
    {
        [Display(Name = "رسوم أخرى")]
        Other,

        [Display(Name = "الاشتراك السنوي")]
        AnnualSubscription,

        [Display(Name = "رسوم دراسية")]
        Tuition,

        [Display(Name = "النقل المدرسي")]
        Transport,

        [Display(Name = "النشاط المدرسي")]
        Activity,

        [Display(Name = "رحلة مدرسية")]
        Trip
    }

    public enum StudentEnrollmentStatus
    {
        [Display(Name = "نشط")] Active,
        [Display(Name = "معلق")] Pending,
        [Display(Name = "منسحب")] Withdrawn,
        [Display(Name = "منقول")] Transferred,
        [Display(Name = "متخرج")] Graduated
    }

    public enum SubscriptionStatus
    {
        [Display(Name = "نشط")] Active,
        [Display(Name = "معلق")] Suspended,
        [Display(Name = "ملغى")] Cancelled,
        [Display(Name = "مكتمل")] Completed
    }

    public enum PaymentMethod
    {
        [Display(Name = "نقدي")]
        Cash,

        [Display(Name = "تحويل بنكي")]
        BankTransfer,

        [Display(Name = "شيك")]
        Check,

        [Display(Name = "بطاقة مصرفية")]
        Electronic
    }

    public enum DiscountType
    {
        [Display(Name = "خصم الإخوة")]
        Sibling,

        [Display(Name = "إعفاء")]
        StaffChild,

        [Display(Name = "التسجيل المبكر")]
        EarlyBird,

        [Display(Name = "التفوق الدراسي")]
        Excellence,

        [Display(Name = "خصم خاص")]  
        General
    }

    public enum DiscountKind
    {
        [Display(Name = "نسبة مئوية")]
        Percentage,

        [Display(Name = "مبلغ ثابت")]
        Amount
    }

    public enum InvoiceStatus
    {
        [Display(Name = "مسودة")]
        Draft,

        [Display(Name = "صادرة")]
        Issued,

        [Display(Name = "مدفوعة جزئياً")]
        PartiallyPaid,

        [Display(Name = "مدفوعة")]
        Paid,

        [Display(Name = "ملغاة")]
        Cancelled
    }

    public enum LessonStatus
    {
        [Display(Name = "مسودة")]
        Draft,

        [Display(Name = "معتمد")]
        Approved,

        [Display(Name = "مؤرشف")]
        Archived
    }

    public enum AttendanceStatus
    {
        [Display(Name = "حضور كامل")]
        PresentFull,

        [Display(Name = "حضور جزئي")]
        PresentPartial,

        [Display(Name = "غياب بعذر")]
        AbsentExcused,

        [Display(Name = "غياب بدون عذر")]
        AbsentUnexcused,

        [Display(Name = "تعويضية كاملة")]
        SubstitutionFull,

        [Display(Name = "تعويضية جزئية")]
        SubstitutionPartial,

        [Display(Name = "حصة ملغاة")]
        Cancelled
    }

    public enum PayrollPeriodStatus
    {
        [Display(Name = "مفتوح")]
        Open,

        [Display(Name = "قيد المراجعة")]
        UnderReview,

        [Display(Name = "معتمد")]
        Approved,

        [Display(Name = "مغلق")]
        Closed
    }

    public enum PayrollItemType
    {
        [Display(Name = "مكافأة")]
        Bonus,

        [Display(Name = "خصم")]
        Deduction,

        [Display(Name = "بدل")]
        Allowance,

        [Display(Name = "تصحيح مالي")]
        FinancialCorrection,

        [Display(Name = "تسوية")]
        Adjustment
    }

    public enum UserRole
    {
        [Display(Name = "مدير النظام")]
        Admin,

        [Display(Name = "محاسب")]
        Accountant,

        [Display(Name = "مشرف")]
        Supervisor,

        [Display(Name = "مدخل بيانات")]
        DataEntry,

        [Display(Name = "معلم")]
        Teacher,

        // يضاف في نهاية التعداد للحفاظ على القيم الرقمية للأدوار الحالية.
        [Display(Name = "عرض البيانات (View)")]
        View
    }

    public enum WeekDay
    {
        [Display(Name = "السبت")]
        Saturday = 1,

        [Display(Name = "الأحد")]
        Sunday = 2,

        [Display(Name = "الاثنين")]
        Monday = 3,

        [Display(Name = "الثلاثاء")]
        Tuesday = 4,

        [Display(Name = "الأربعاء")]
        Wednesday = 5,

        [Display(Name = "الخميس")]
        Thursday = 6
    }
}
