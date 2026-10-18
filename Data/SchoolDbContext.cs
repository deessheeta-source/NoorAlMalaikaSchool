using System;
using System.Data.Entity;
using System.Linq;
using System.Globalization;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext() : base("name=DefaultConnection") { }

        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<GradeLevel> GradeLevels { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<InvoiceDiscount> InvoiceDiscounts { get; set; }
        public DbSet<DiscountPolicy> DiscountPolicies { get; set; }
        public DbSet<StudentAcademicYear> StudentAcademicYears { get; set; }
        public DbSet<StudentSubscription> StudentSubscriptions { get; set; }
        public DbSet<StudentDiscount> StudentDiscounts { get; set; }
        public DbSet<StudentExemption> StudentExemptions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<StudentStaffRelation> StudentStaffRelations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TeacherAttendance> TeacherAttendances { get; set; }
        public DbSet<LessonPeriod> LessonPeriods { get; set; }
        public DbSet<DailyLesson> DailyLessons { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<PayrollPeriod> PayrollPeriods { get; set; }
        public DbSet<TeacherPayroll> TeacherPayrolls { get; set; }
        public DbSet<PayrollItem> PayrollItems { get; set; }
        public DbSet<TeacherSubjectAssignment> TeacherSubjectAssignments { get; set; }
        public DbSet<PayrollLessonLine> PayrollLessonLines { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TimetableEntry> TimetableEntries { get; set; }
        public DbSet<MonthlySheetGrade> MonthlySheetGrades { get; set; }
        public DbSet<MonthlySheetEvaluation> MonthlySheetEvaluations { get; set; }
        public DbSet<MonthlySheetMonth> MonthlySheetMonths { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Substitution> Substitutions { get; set; }
        public DbSet<WhatsAppCampaign> WhatsAppCampaigns { get; set; }
        public DbSet<WhatsAppMessage> WhatsAppMessages { get; set; }
        public DbSet<WhatsAppTemplate> WhatsAppTemplates { get; set; }
        public DbSet<WhatsAppConsent> WhatsAppConsents { get; set; }
        public DbSet<WhatsAppClassMapping> WhatsAppClassMappings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table names
            modelBuilder.Entity<AcademicYear>().ToTable("AcademicYears");
            modelBuilder.Entity<GradeLevel>().ToTable("GradeLevels");
            modelBuilder.Entity<ClassRoom>().ToTable("ClassRooms");
            modelBuilder.Entity<Parent>().ToTable("Parents");
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Attendance>().ToTable("Attendances");
            modelBuilder.Entity<Fee>().ToTable("Fees");
            modelBuilder.Entity<Payment>().ToTable("Payments");
            modelBuilder.Entity<Invoice>().ToTable("Invoices");
            modelBuilder.Entity<InvoiceLine>().ToTable("InvoiceLines");
            modelBuilder.Entity<InvoiceDiscount>().ToTable("InvoiceDiscounts");
            modelBuilder.Entity<DiscountPolicy>().ToTable("DiscountPolicies");
            modelBuilder.Entity<StudentAcademicYear>().ToTable("StudentAcademicYears");
            modelBuilder.Entity<StudentSubscription>().ToTable("StudentSubscriptions");
            modelBuilder.Entity<StudentDiscount>().ToTable("StudentDiscounts");
            modelBuilder.Entity<StudentExemption>().ToTable("StudentExemptions");
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<StudentStaffRelation>().ToTable("StudentStaffRelations");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Teacher>().ToTable("Teachers");
            modelBuilder.Entity<TeacherAttendance>().ToTable("TeacherAttendances");
            modelBuilder.Entity<LessonPeriod>().ToTable("LessonPeriods");
            modelBuilder.Entity<DailyLesson>().ToTable("DailyLessons");
            modelBuilder.Entity<AttendanceRecord>().ToTable("AttendanceRecords");
            modelBuilder.Entity<PayrollPeriod>().ToTable("PayrollPeriods");
            modelBuilder.Entity<TeacherPayroll>().ToTable("TeacherPayrolls");
            modelBuilder.Entity<PayrollItem>().ToTable("PayrollItems");
            modelBuilder.Entity<TeacherSubjectAssignment>().ToTable("TeacherSubjectAssignments");
            modelBuilder.Entity<PayrollLessonLine>().ToTable("PayrollLessonLines");
            modelBuilder.Entity<Subject>().ToTable("Subjects");
            modelBuilder.Entity<TimetableEntry>().ToTable("TimetableEntries");
            modelBuilder.Entity<MonthlySheetGrade>().ToTable("MonthlySheetGrades");
            modelBuilder.Entity<MonthlySheetEvaluation>().ToTable("MonthlySheetEvaluations");
            modelBuilder.Entity<MonthlySheetMonth>().ToTable("MonthlySheetMonths");
            modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
            modelBuilder.Entity<Substitution>().ToTable("Substitutions");
            modelBuilder.Entity<WhatsAppCampaign>().ToTable("WhatsAppCampaigns");
            modelBuilder.Entity<WhatsAppMessage>().ToTable("WhatsAppMessages");
            modelBuilder.Entity<WhatsAppTemplate>().ToTable("WhatsAppTemplates");
            modelBuilder.Entity<WhatsAppConsent>().ToTable("WhatsAppConsents");
            modelBuilder.Entity<WhatsAppClassMapping>().ToTable("WhatsAppClassMappings");

            modelBuilder.Entity<WhatsAppCampaign>().Property(x => x.Title).HasMaxLength(200);
            modelBuilder.Entity<WhatsAppCampaign>().Property(x => x.MessageKind).HasMaxLength(30);
            modelBuilder.Entity<WhatsAppCampaign>().Property(x => x.Status).HasMaxLength(30);
            modelBuilder.Entity<WhatsAppCampaign>().Property(x => x.AudienceType).HasMaxLength(20);
            modelBuilder.Entity<WhatsAppCampaign>().Property(x => x.CreatedBy).HasMaxLength(100);
            modelBuilder.Entity<WhatsAppMessage>().Property(x => x.RecipientName).HasMaxLength(200);
            modelBuilder.Entity<WhatsAppMessage>().Property(x => x.PhoneNumber).HasMaxLength(30);
            modelBuilder.Entity<WhatsAppMessage>().Property(x => x.Direction).HasMaxLength(20);
            modelBuilder.Entity<WhatsAppMessage>().Property(x => x.Status).HasMaxLength(30);
            modelBuilder.Entity<WhatsAppMessage>().Property(x => x.ProviderMessageId).HasMaxLength(200);
            modelBuilder.Entity<WhatsAppMessage>().Property(x => x.ProviderTemplateName).HasMaxLength(100);
            modelBuilder.Entity<WhatsAppMessage>().Property(x => x.TemplateLanguage).HasMaxLength(20);
            modelBuilder.Entity<WhatsAppMessage>().Property(x => x.TemplateParametersJson).HasMaxLength(4000);
            modelBuilder.Entity<WhatsAppMessage>().Property(x => x.CreatedBy).HasMaxLength(100);
            modelBuilder.Entity<WhatsAppMessage>().HasIndex(x => x.ProviderMessageId);
            modelBuilder.Entity<WhatsAppMessage>().HasIndex(x => new { x.Status, x.ScheduledAt });
            modelBuilder.Entity<WhatsAppTemplate>().Property(x => x.Name).HasMaxLength(150);
            modelBuilder.Entity<WhatsAppTemplate>().Property(x => x.Category).HasMaxLength(30);
            modelBuilder.Entity<WhatsAppTemplate>().Property(x => x.ProviderTemplateName).HasMaxLength(100);
            modelBuilder.Entity<WhatsAppTemplate>().Property(x => x.LanguageCode).HasMaxLength(20);
            modelBuilder.Entity<WhatsAppTemplate>().Property(x => x.CreatedBy).HasMaxLength(100);
            modelBuilder.Entity<WhatsAppConsent>().Property(x => x.PhoneNumber).HasMaxLength(30);
            modelBuilder.Entity<WhatsAppConsent>().Property(x => x.ConsentSource).HasMaxLength(300);
            modelBuilder.Entity<WhatsAppConsent>().Property(x => x.RevokeReason).HasMaxLength(300);
            modelBuilder.Entity<WhatsAppConsent>().Property(x => x.UpdatedBy).HasMaxLength(100);
            modelBuilder.Entity<WhatsAppConsent>().HasIndex(x => x.ParentId).IsUnique();
            modelBuilder.Entity<WhatsAppClassMapping>().Property(x => x.DisplayName).HasMaxLength(150);
            modelBuilder.Entity<WhatsAppClassMapping>().Property(x => x.UpdatedBy).HasMaxLength(100);
            modelBuilder.Entity<WhatsAppClassMapping>().HasIndex(x => x.ClassRoomId).IsUnique();

            // Name fields: HasMaxLength(200)
            modelBuilder.Entity<AcademicYear>().Property(a => a.Name).HasMaxLength(200);
            modelBuilder.Entity<GradeLevel>().Property(g => g.Name).HasMaxLength(200);
            modelBuilder.Entity<ClassRoom>().Property(c => c.Name).HasMaxLength(200);
            modelBuilder.Entity<Parent>().Property(p => p.Name).HasMaxLength(200);
            modelBuilder.Entity<Student>().Property(s => s.Name).HasMaxLength(200);
            modelBuilder.Entity<Teacher>().Property(t => t.Name).HasMaxLength(200);
            modelBuilder.Entity<Subject>().Property(s => s.Name).HasMaxLength(200);

            // NationalId is stored as NVARCHAR(255) to accommodate imported legacy identifiers.
            modelBuilder.Entity<Student>().Property(s => s.NationalId).HasMaxLength(255);
            modelBuilder.Entity<Parent>().Property(p => p.NationalId).HasMaxLength(20);

            // Amount fields: HasPrecision(18,2)
            modelBuilder.Entity<Fee>().Property(f => f.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Fee>().HasIndex(f => new { f.ClassRoomId, f.Type, f.AcademicYearId });
            modelBuilder.Entity<Payment>().Property(p => p.CreatedBy).HasMaxLength(200);
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Payment>().Property(p => p.ReceiptNumber).HasMaxLength(50);

            // Teacher financial fields: HasPrecision(18,2)
            modelBuilder.Entity<Teacher>().Property(t => t.BaseSalary).HasPrecision(18, 2);
            modelBuilder.Entity<Teacher>().Property(t => t.LessonRate).HasPrecision(18, 2);
            modelBuilder.Entity<Teacher>().Property(t => t.HourlyRate).HasPrecision(18, 2);
            modelBuilder.Entity<Teacher>().Property(t => t.MonthlyHours).HasPrecision(18, 2);
            modelBuilder.Entity<TeacherSubjectAssignment>().Property(a => a.LessonRate).HasPrecision(18, 2);
            modelBuilder.Entity<TeacherSubjectAssignment>().Property(a => a.HourlyRate).HasPrecision(18, 2);
            modelBuilder.Entity<TeacherSubjectAssignment>().Property(a => a.Notes).HasMaxLength(500);
            modelBuilder.Entity<TeacherPayroll>().Property(t => t.BaseSalaryApplied).HasPrecision(18, 2);
            modelBuilder.Entity<TeacherPayroll>().Property(t => t.MonthlyHoursApplied).HasPrecision(18, 2);
            modelBuilder.Entity<PayrollLessonLine>().Property(l => l.LessonRate).HasPrecision(18, 2);
            modelBuilder.Entity<PayrollLessonLine>().Property(l => l.EarnedAmount).HasPrecision(18, 2);
            modelBuilder.Entity<PayrollLessonLine>().Property(l => l.DeductionAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Teacher>().Property(t => t.EmployeeNo).HasMaxLength(50);
            modelBuilder.Entity<Teacher>().Property(t => t.NationalId).HasMaxLength(20);
            modelBuilder.Entity<Teacher>().Property(t => t.Email).HasMaxLength(200);
            modelBuilder.Entity<Teacher>().Property(t => t.BankAccount).HasMaxLength(50);

            // ClassRoom -> GradeLevel (GradeLevelId, Restrict)
            modelBuilder.Entity<ClassRoom>()
                .HasRequired(c => c.GradeLevel)
                .WithMany(g => g.ClassRooms)
                .HasForeignKey(c => c.GradeLevelId)
                .WillCascadeOnDelete(false);

            // ClassRoom -> AcademicYear (AcademicYearId, Restrict)
            modelBuilder.Entity<ClassRoom>()
                .HasRequired(c => c.AcademicYear)
                .WithMany(a => a.ClassRooms)
                .HasForeignKey(c => c.AcademicYearId)
                .WillCascadeOnDelete(false);

            // Teacher -> AcademicYear (AcademicYearId, optional, Restrict)
            // Legacy teachers may have no year until they are edited/assigned.
            modelBuilder.Entity<Teacher>()
                .HasOptional(t => t.AcademicYear)
                .WithMany(a => a.Teachers)
                .HasForeignKey(t => t.AcademicYearId)
                .WillCascadeOnDelete(false);

            // Student -> ClassRoom (ClassRoomId, Restrict)
            modelBuilder.Entity<Student>()
                .HasRequired(s => s.ClassRoom)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassRoomId)
                .WillCascadeOnDelete(false);

            // Student -> Parent (ParentId, SetNull)
            modelBuilder.Entity<Student>()
                .HasOptional(s => s.Parent)
                .WithMany(p => p.Students)
                .HasForeignKey(s => s.ParentId)
                .WillCascadeOnDelete(false);

            // Attendance -> Student (StudentId, Restrict)
            modelBuilder.Entity<Attendance>()
                .HasRequired(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .WillCascadeOnDelete(false);

            // TeacherAttendance -> Teacher (TeacherId, Restrict)
            modelBuilder.Entity<TeacherAttendance>()
                .HasRequired(a => a.Teacher)
                .WithMany(t => t.Attendances)
                .HasForeignKey(a => a.TeacherId)
                .WillCascadeOnDelete(false);

            // LessonPeriod: unique PeriodNumber
            modelBuilder.Entity<LessonPeriod>()
                .Property(l => l.StartTime).HasColumnType("time");
            modelBuilder.Entity<LessonPeriod>()
                .Property(l => l.EndTime).HasColumnType("time");
            modelBuilder.Entity<LessonPeriod>()
                .HasIndex(l => l.PeriodNumber).IsUnique();

            // DailyLesson -> Teacher (Restrict)
            modelBuilder.Entity<DailyLesson>()
                .HasRequired(d => d.Teacher)
                .WithMany()
                .HasForeignKey(d => d.TeacherId)
                .WillCascadeOnDelete(false);

            // DailyLesson -> Subject (Restrict)
            modelBuilder.Entity<DailyLesson>()
                .HasRequired(d => d.Subject)
                .WithMany()
                .HasForeignKey(d => d.SubjectId)
                .WillCascadeOnDelete(false);

            // DailyLesson -> ClassRoom (Restrict)
            modelBuilder.Entity<DailyLesson>()
                .HasRequired(d => d.ClassRoom)
                .WithMany()
                .HasForeignKey(d => d.ClassRoomId)
                .WillCascadeOnDelete(false);

            // DailyLesson -> TimetableEntry (Optional, Restrict)
            modelBuilder.Entity<DailyLesson>()
                .HasOptional(d => d.ScheduleEntry)
                .WithMany()
                .HasForeignKey(d => d.ScheduleEntryId)
                .WillCascadeOnDelete(false);

            // DailyLesson StartTime/EndTime as time columns
            modelBuilder.Entity<DailyLesson>()
                .Property(d => d.StartTime).HasColumnType("time");
            modelBuilder.Entity<DailyLesson>()
                .Property(d => d.EndTime).HasColumnType("time");
            modelBuilder.Entity<DailyLesson>()
                .HasIndex(d => new { d.Date, d.TeacherId, d.PeriodNumber, d.ClassRoomId }).IsUnique();

            // AttendanceRecord -> DailyLesson (Restrict)
            modelBuilder.Entity<AttendanceRecord>()
                .HasRequired(r => r.DailyLesson)
                .WithMany()
                .HasForeignKey(r => r.DailyLessonId)
                .WillCascadeOnDelete(false);
            // يسمح بسجل أصلي وسجل تعويض لنفس الحصة، لكن لكل معلم سجل واحد فقط.
            modelBuilder.Entity<AttendanceRecord>()
                .HasIndex(r => new { r.DailyLessonId, r.TeacherId }).IsUnique();

            // AttendanceRecord -> Teacher (Restrict)
            modelBuilder.Entity<AttendanceRecord>()
                .HasRequired(r => r.Teacher)
                .WithMany()
                .HasForeignKey(r => r.TeacherId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<AttendanceRecord>()
                .Property(r => r.RecordedBy).HasMaxLength(100);
            modelBuilder.Entity<AttendanceRecord>()
                .Property(r => r.ApprovedBy).HasMaxLength(100);

            modelBuilder.Entity<AttendanceRecord>().Property(r => r.LessonRateApplied).HasPrecision(18, 2);
            modelBuilder.Entity<AttendanceRecord>().Property(r => r.HourlyRateApplied).HasPrecision(18, 2);

            // PayrollPeriod: unique (Year, Month)
            modelBuilder.Entity<PayrollPeriod>()
                .HasIndex(p => new { p.Year, p.Month }).IsUnique();
            modelBuilder.Entity<PayrollPeriod>()
                .Property(p => p.ClosedBy).HasMaxLength(100);
            modelBuilder.Entity<PayrollPeriod>()
                .Property(p => p.OpenedBy).HasMaxLength(100);

            // TeacherPayroll -> PayrollPeriod (Restrict)
            modelBuilder.Entity<TeacherPayroll>()
                .HasRequired(t => t.PayrollPeriod)
                .WithMany(p => p.TeacherPayrolls)
                .HasForeignKey(t => t.PayrollPeriodId)
                .WillCascadeOnDelete(false);

            // TeacherPayroll -> Teacher (Restrict)
            modelBuilder.Entity<TeacherPayroll>()
                .HasRequired(t => t.Teacher)
                .WithMany()
                .HasForeignKey(t => t.TeacherId)
                .WillCascadeOnDelete(false);

            // TeacherPayroll financial precision
            modelBuilder.Entity<TeacherPayroll>().Property(t => t.LessonRateApplied).HasPrecision(18, 2);
            modelBuilder.Entity<TeacherPayroll>().Property(t => t.HourlyRateApplied).HasPrecision(18, 2);
            modelBuilder.Entity<TeacherPayroll>().Property(t => t.EarnedAmount).HasPrecision(18, 2);
            modelBuilder.Entity<TeacherPayroll>().Property(t => t.DeductionAmount).HasPrecision(18, 2);
            modelBuilder.Entity<TeacherPayroll>().Property(t => t.AdjustmentAmount).HasPrecision(18, 2);
            modelBuilder.Entity<TeacherPayroll>().Property(t => t.NetAmount).HasPrecision(18, 2);

            // PayrollItem -> TeacherPayroll (Cascade)
            modelBuilder.Entity<PayrollItem>()
                .HasRequired(i => i.TeacherPayroll)
                .WithMany(t => t.Items)
                .HasForeignKey(i => i.TeacherPayrollId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<PayrollItem>().Property(i => i.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<PayrollItem>().Property(i => i.Reason).HasMaxLength(500);
            modelBuilder.Entity<PayrollItem>().Property(i => i.ApprovedBy).HasMaxLength(100);

            // TeacherSubjectAssignment -> Teacher (Restrict)
            modelBuilder.Entity<TeacherSubjectAssignment>()
                .HasRequired(a => a.Teacher)
                .WithMany(t => t.SubjectAssignments)
                .HasForeignKey(a => a.TeacherId)
                .WillCascadeOnDelete(false);

            // TeacherSubjectAssignment -> Subject (Restrict)
            modelBuilder.Entity<TeacherSubjectAssignment>()
                .HasRequired(a => a.Subject)
                .WithMany()
                .HasForeignKey(a => a.SubjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TeacherSubjectAssignment>()
                .HasIndex(a => new { a.TeacherId, a.SubjectId, a.IsActive })
                .IsUnique();

            // PayrollLessonLine -> TeacherPayroll (Cascade)
            modelBuilder.Entity<PayrollLessonLine>()
                .HasRequired(l => l.TeacherPayroll)
                .WithMany(p => p.LessonLines)
                .HasForeignKey(l => l.TeacherPayrollId)
                .WillCascadeOnDelete(true);

            // PayrollLessonLine -> AttendanceRecord (Restrict)
            modelBuilder.Entity<PayrollLessonLine>()
                .HasRequired(l => l.AttendanceRecord)
                .WithMany()
                .HasForeignKey(l => l.AttendanceRecordId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PayrollLessonLine>()
                .HasIndex(l => new { l.TeacherPayrollId, l.AttendanceRecordId })
                .IsUnique();

            modelBuilder.Entity<PayrollLessonLine>()
                .HasOptional(l => l.Subject)
                .WithMany()
                .HasForeignKey(l => l.SubjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PayrollLessonLine>()
                .HasOptional(l => l.ClassRoom)
                .WithMany()
                .HasForeignKey(l => l.ClassRoomId)
                .WillCascadeOnDelete(false);


            // TimetableEntry times as time columns + defaults
            modelBuilder.Entity<TimetableEntry>()
                .Property(t => t.StartTime).HasColumnType("time");
            modelBuilder.Entity<TimetableEntry>()
                .Property(t => t.EndTime).HasColumnType("time");

            // Fee -> AcademicYear (AcademicYearId, Restrict)
            modelBuilder.Entity<Fee>()
                .HasRequired(f => f.AcademicYear)
                .WithMany(a => a.Fees)
                .HasForeignKey(f => f.AcademicYearId)
                .WillCascadeOnDelete(false);

            // Fee -> GradeLevel (GradeLevelId, SetNull)
            modelBuilder.Entity<Fee>()
                .HasOptional(f => f.GradeLevel)
                .WithMany(g => g.Fees)
                .HasForeignKey(f => f.GradeLevelId)
                .WillCascadeOnDelete(false);

            // Fee -> ClassRoom (ClassRoomId, SetNull)
            modelBuilder.Entity<Fee>()
                .HasOptional(f => f.ClassRoom)
                .WithMany(c => c.Fees)
                .HasForeignKey(f => f.ClassRoomId)
                .WillCascadeOnDelete(false);

            // Payment -> Student (StudentId, Restrict)
            modelBuilder.Entity<Payment>()
                .HasRequired(p => p.Student)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.StudentId)
                .WillCascadeOnDelete(false);

            // Payment -> Fee (FeeId, Restrict)
            modelBuilder.Entity<Payment>()
                .HasRequired(p => p.Fee)
                .WithMany(f => f.Payments)
                .HasForeignKey(p => p.FeeId)
                .WillCascadeOnDelete(false);

            // Payment -> Invoice (optional for legacy payments; required for new invoice-based payments)
            modelBuilder.Entity<Payment>()
                .HasOptional(p => p.Invoice)
                .WithMany()
                .HasForeignKey(p => p.InvoiceId)
                .WillCascadeOnDelete(false);

            // TimetableEntry -> ClassRoom (ClassRoomId, Restrict)
            modelBuilder.Entity<TimetableEntry>()
                .HasRequired(t => t.ClassRoom)
                .WithMany()
                .HasForeignKey(t => t.ClassRoomId)
                .WillCascadeOnDelete(false);

            // TimetableEntry -> Subject (SubjectId, Restrict)
            modelBuilder.Entity<TimetableEntry>()
                .HasRequired(t => t.Subject)
                .WithMany()
                .HasForeignKey(t => t.SubjectId)
                .WillCascadeOnDelete(false);

            // TimetableEntry -> Teacher (TeacherId, Restrict)
            modelBuilder.Entity<TimetableEntry>()
                .HasRequired(t => t.Teacher)
                .WithMany()
                .HasForeignKey(t => t.TeacherId)
                .WillCascadeOnDelete(false);

            // MonthlySheetGrade -> Student (StudentId, Cascade)
            modelBuilder.Entity<MonthlySheetGrade>()
                .HasRequired(g => g.Student)
                .WithMany(s => s.MonthlySheetGrades)
                .HasForeignKey(g => g.StudentId)
                .WillCascadeOnDelete(true);

            // MonthlySheetEvaluation -> Student (StudentId, Cascade)
            modelBuilder.Entity<MonthlySheetEvaluation>()
                .HasRequired(e => e.Student)
                .WithMany(s => s.MonthlySheetEvaluations)
                .HasForeignKey(e => e.StudentId)
                .WillCascadeOnDelete(true);

            // MonthlySheetMonth -> Student (StudentId, Cascade, unique per student)
            modelBuilder.Entity<MonthlySheetMonth>()
                .HasRequired(m => m.Student)
                .WithMany()
                .HasForeignKey(m => m.StudentId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<MonthlySheetMonth>()
                .Property(m => m.Month1).HasMaxLength(50);
            modelBuilder.Entity<MonthlySheetMonth>()
                .Property(m => m.Month2).HasMaxLength(50);
            modelBuilder.Entity<MonthlySheetMonth>()
                .Property(m => m.Month3).HasMaxLength(50);
            modelBuilder.Entity<MonthlySheetMonth>()
                .HasIndex(m => m.StudentId).IsUnique();


            // Amount fields: HasPrecision(18,2)
            modelBuilder.Entity<Invoice>().Property(i => i.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.DiscountAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.NetAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.PaidAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.InvoiceNumber).HasMaxLength(50);
            modelBuilder.Entity<InvoiceLine>().Property(l => l.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<InvoiceLine>().Property(l => l.Description).HasMaxLength(500);
            modelBuilder.Entity<InvoiceDiscount>().Property(d => d.Value).HasPrecision(18, 2);
            modelBuilder.Entity<InvoiceDiscount>().Property(d => d.DiscountAmount).HasPrecision(18, 2);
            modelBuilder.Entity<InvoiceDiscount>().Property(d => d.Name).HasMaxLength(200);
            modelBuilder.Entity<InvoiceDiscount>().Property(d => d.Reason).HasMaxLength(500);
            modelBuilder.Entity<InvoiceDiscount>().HasOptional(d => d.StudentDiscount).WithMany().HasForeignKey(d => d.StudentDiscountId).WillCascadeOnDelete(false);
            modelBuilder.Entity<InvoiceDiscount>().HasOptional(d => d.StudentExemption).WithMany().HasForeignKey(d => d.StudentExemptionId).WillCascadeOnDelete(false);
            modelBuilder.Entity<DiscountPolicy>().Property(p => p.Value).HasPrecision(18, 2);
            modelBuilder.Entity<DiscountPolicy>().Property(p => p.Name).HasMaxLength(200);
            modelBuilder.Entity<StudentSubscription>().Property(s => s.AmountOverride).HasPrecision(18, 2);
            modelBuilder.Entity<StudentDiscount>().Property(d => d.Value).HasPrecision(18, 2);
            modelBuilder.Entity<StudentExemption>().Property(e => e.Value).HasPrecision(18, 2);
            modelBuilder.Entity<Employee>().Property(e => e.EmployeeNo).HasMaxLength(30);
            modelBuilder.Entity<Employee>().Property(e => e.Name).HasMaxLength(200);
            modelBuilder.Entity<Employee>().Property(e => e.NationalId).HasMaxLength(20);
            modelBuilder.Entity<StudentStaffRelation>().Property(r => r.Relationship).HasMaxLength(50);

            // Invoice -> Student (Restrict)
            modelBuilder.Entity<Invoice>()
                .HasRequired(i => i.Student)
                .WithMany(s => s.Invoices)
                .HasForeignKey(i => i.StudentId)
                .WillCascadeOnDelete(false);

            // Invoice -> AcademicYear (Restrict)
            modelBuilder.Entity<Invoice>()
                .HasRequired(i => i.AcademicYear)
                .WithMany(a => a.Invoices)
                .HasForeignKey(i => i.AcademicYearId)
                .WillCascadeOnDelete(false);

            // InvoiceLine -> Invoice (Cascade)
            modelBuilder.Entity<InvoiceLine>()
                .HasRequired(l => l.Invoice)
                .WithMany(i => i.Lines)
                .HasForeignKey(l => l.InvoiceId)
                .WillCascadeOnDelete(true);

            // InvoiceLine -> Fee (optional for manual lines)
            modelBuilder.Entity<InvoiceLine>()
                .HasOptional(l => l.Fee)
                .WithMany()
                .HasForeignKey(l => l.FeeId)
                .WillCascadeOnDelete(false);

            // InvoiceDiscount -> Invoice (Cascade)
            modelBuilder.Entity<InvoiceDiscount>()
                .HasRequired(d => d.Invoice)
                .WithMany(i => i.Discounts)
                .HasForeignKey(d => d.InvoiceId)
                .WillCascadeOnDelete(true);

            // InvoiceDiscount -> DiscountPolicy (SetNull)
            modelBuilder.Entity<InvoiceDiscount>()
                .HasOptional(d => d.DiscountPolicy)
                .WithMany()
                .HasForeignKey(d => d.DiscountPolicyId)
                .WillCascadeOnDelete(false);

            // Substitution -> DailyLesson (Optional, Restrict)
            modelBuilder.Entity<Substitution>()
                .HasOptional(s => s.DailyLesson)
                .WithMany()
                .HasForeignKey(s => s.DailyLessonId)
                .WillCascadeOnDelete(false);

            // Substitution -> ClassRoom (Restrict)
            modelBuilder.Entity<Substitution>()
                .HasRequired(s => s.ClassRoom)
                .WithMany()
                .HasForeignKey(s => s.ClassRoomId)
                .WillCascadeOnDelete(false);

            // Substitution -> Subject (Restrict)
            modelBuilder.Entity<Substitution>()
                .HasRequired(s => s.Subject)
                .WithMany()
                .HasForeignKey(s => s.SubjectId)
                .WillCascadeOnDelete(false);

            // Substitution -> OriginalTeacher (Restrict)
            modelBuilder.Entity<Substitution>()
                .HasRequired(s => s.OriginalTeacher)
                .WithMany()
                .HasForeignKey(s => s.OriginalTeacherId)
                .WillCascadeOnDelete(false);

            // Substitution -> SubstituteTeacher (Restrict)
            modelBuilder.Entity<Substitution>()
                .HasRequired(s => s.SubstituteTeacher)
                .WithMany()
                .HasForeignKey(s => s.SubstituteTeacherId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AuditLog>()
                .Property(c => c.UserName).HasMaxLength(100);
            modelBuilder.Entity<AuditLog>()
                .Property(c => c.IpAddress).HasMaxLength(50);
            // TimestampText is a display-only property; ignore it via Fluent API to avoid
            // DataAnnotations/EntityFramework NotMappedAttribute assembly ambiguity.
            modelBuilder.Entity<AuditLog>()
                .Ignore(c => c.TimestampText);

            // User -> Teacher link (EmployeeNo, unique per user)
            modelBuilder.Entity<User>()
                .Property(u => u.EmployeeNo).HasMaxLength(50);
            // EmployeeNo is nullable; uniqueness is enforced by the Phase 8 filtered SQL index.
            // EF6 has no portable filtered-index definition, so do not model this as a plain unique index.

            // =========================================================
            // Student Finance - explicit column and relationship mapping
            // The database uses StudentId / AcademicYearId / EmployeeId / FeeId.
            // Explicit mapping prevents EF6 from generating Student_Id.
            // =========================================================

            // StudentAcademicYear
            modelBuilder.Entity<StudentAcademicYear>()
                .Property(x => x.StudentId)
                .HasColumnName("StudentId");
            modelBuilder.Entity<StudentAcademicYear>()
                .Property(x => x.AcademicYearId)
                .HasColumnName("AcademicYearId");
            modelBuilder.Entity<StudentAcademicYear>()
                .Property(x => x.ClassRoomId)
                .HasColumnName("ClassRoomId");
            modelBuilder.Entity<StudentAcademicYear>()
                .HasRequired(x => x.Student)
                .WithMany(s => s.AcademicYearEnrollments)
                .HasForeignKey(x => x.StudentId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<StudentAcademicYear>()
                .HasRequired(x => x.AcademicYear)
                .WithMany()
                .HasForeignKey(x => x.AcademicYearId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<StudentAcademicYear>()
                .HasRequired(x => x.ClassRoom)
                .WithMany()
                .HasForeignKey(x => x.ClassRoomId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<StudentAcademicYear>()
                .HasIndex(x => new { x.StudentId, x.AcademicYearId })
                .IsUnique();

            // StudentSubscription
            modelBuilder.Entity<StudentSubscription>()
                .Property(x => x.StudentId)
                .HasColumnName("StudentId");
            modelBuilder.Entity<StudentSubscription>()
                .Property(x => x.AcademicYearId)
                .HasColumnName("AcademicYearId");
            modelBuilder.Entity<StudentSubscription>()
                .Property(x => x.FeeId)
                .HasColumnName("FeeId");
            modelBuilder.Entity<StudentSubscription>()
                .HasRequired(x => x.Student)
                .WithMany(s => s.Subscriptions)
                .HasForeignKey(x => x.StudentId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<StudentSubscription>()
                .HasRequired(x => x.AcademicYear)
                .WithMany()
                .HasForeignKey(x => x.AcademicYearId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<StudentSubscription>()
                .HasRequired(x => x.Fee)
                .WithMany()
                .HasForeignKey(x => x.FeeId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<StudentSubscription>()
                .HasIndex(x => new { x.StudentId, x.AcademicYearId, x.FeeId })
                .IsUnique();

            // StudentDiscount
            modelBuilder.Entity<StudentDiscount>()
                .Property(x => x.StudentId)
                .HasColumnName("StudentId");
            modelBuilder.Entity<StudentDiscount>()
                .Property(x => x.AcademicYearId)
                .HasColumnName("AcademicYearId");
            modelBuilder.Entity<StudentDiscount>()
                .Property(x => x.DiscountPolicyId)
                .HasColumnName("DiscountPolicyId");
            modelBuilder.Entity<StudentDiscount>()
                .HasRequired(x => x.Student)
                .WithMany(s => s.Discounts)
                .HasForeignKey(x => x.StudentId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<StudentDiscount>()
                .HasRequired(x => x.AcademicYear)
                .WithMany()
                .HasForeignKey(x => x.AcademicYearId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<StudentDiscount>()
                .HasOptional(x => x.DiscountPolicy)
                .WithMany()
                .HasForeignKey(x => x.DiscountPolicyId)
                .WillCascadeOnDelete(false);

            // StudentExemption
            modelBuilder.Entity<StudentExemption>()
                .Property(x => x.StudentId)
                .HasColumnName("StudentId");
            modelBuilder.Entity<StudentExemption>()
                .Property(x => x.AcademicYearId)
                .HasColumnName("AcademicYearId");
            modelBuilder.Entity<StudentExemption>()
                .HasRequired(x => x.Student)
                .WithMany(s => s.Exemptions)
                .HasForeignKey(x => x.StudentId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<StudentExemption>()
                .HasRequired(x => x.AcademicYear)
                .WithMany()
                .HasForeignKey(x => x.AcademicYearId)
                .WillCascadeOnDelete(false);

            // Employee / StudentStaffRelation
            modelBuilder.Entity<Employee>().HasIndex(x => x.EmployeeNo).IsUnique();
            modelBuilder.Entity<StudentStaffRelation>()
                .Property(x => x.StudentId)
                .HasColumnName("StudentId");
            modelBuilder.Entity<StudentStaffRelation>()
                .Property(x => x.EmployeeId)
                .HasColumnName("EmployeeId");
            modelBuilder.Entity<StudentStaffRelation>()
                .HasRequired(x => x.Student)
                .WithMany(s => s.StaffRelations)
                .HasForeignKey(x => x.StudentId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<StudentStaffRelation>()
                .HasRequired(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<StudentStaffRelation>()
                .HasIndex(x => new { x.StudentId, x.EmployeeId })
                .IsUnique();

            // Phase 5: keep the EF model aligned with the database integrity indexes.
            modelBuilder.Entity<AcademicYear>().HasIndex(a => a.Name).IsUnique();
            modelBuilder.Entity<GradeLevel>().HasIndex(g => g.SortOrder).IsUnique();
            // NationalId is nullable; uniqueness is enforced by a filtered SQL index.
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Attendance>().HasIndex(a => new { a.StudentId, a.Date }).IsUnique();
            modelBuilder.Entity<TimetableEntry>().HasIndex(t => new { t.ClassRoomId, t.DayOfWeek, t.PeriodNumber }).IsUnique();
            modelBuilder.Entity<TeacherPayroll>().HasIndex(t => new { t.PayrollPeriodId, t.TeacherId }).IsUnique();
            modelBuilder.Entity<AuditLog>().Property(a => a.ChangedFields).HasMaxLength(4000);

        }

        public override int SaveChanges()
        {
            var now = DateTime.Now;
            var userName = "";
            var ip = "";
            try
            {
                var context = System.Web.HttpContext.Current;

                if (context != null)
                {
                    // Session قد تكون null في بعض سياقات ASP.NET.
                    // لذلك لا يتم الوصول إلى Session مباشرة دون التحقق منها.
                    var session = context.Session;

                    if (session != null)
                    {
                        userName = (session["DisplayName"] as string)
                                   ?? (session["Username"] as string)
                                   ?? "";
                    }

                    if (context.Request != null)
                    {
                        ip = context.Request.UserHostAddress ?? "";
                    }
                }
            }
            catch { }

            var drafts = new System.Collections.Generic.List<AuditDraft>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == System.Data.Entity.EntityState.Unchanged)
                    continue;

                if (entry.State == System.Data.Entity.EntityState.Added ||
                    entry.State == System.Data.Entity.EntityState.Modified ||
                    entry.State == System.Data.Entity.EntityState.Deleted)
                {
                    drafts.Add(new AuditDraft
                    {
                        Entry = entry,
                        State = entry.State,
                        EntityType = entry.Entity.GetType().Name,
                        OldValues = BuildAuditValues(entry, entry.State == System.Data.Entity.EntityState.Added ? null : entry.OriginalValues),
                        NewValues = BuildAuditValues(entry, entry.State == System.Data.Entity.EntityState.Deleted ? null : entry.CurrentValues),
                        ChangedFields = BuildChangedFields(entry),
                        Timestamp = now,
                        UserName = userName,
                        IpAddress = ip
                    });
                }
            }

            if (drafts.Count == 0)
                return base.SaveChanges();

            // Save business rows first so identity keys are available to the audit records.
            // If this context owns the transaction, both saves commit atomically.
            System.Data.Entity.DbContextTransaction transaction = null;
            bool ownsTransaction = Database.CurrentTransaction == null;
            try
            {
                if (ownsTransaction)
                    transaction = Database.BeginTransaction();

                var result = base.SaveChanges();

                foreach (var d in drafts)
                {
                    var entityId = GetKeyValue(d.Entry);
                    AuditLogs.Add(new AuditLog
                    {
                        Timestamp = d.Timestamp,
                        UserName = d.UserName,
                        Action = d.State == System.Data.Entity.EntityState.Added ? "إنشاء"
                               : d.State == System.Data.Entity.EntityState.Modified ? "تعديل"
                               : "حذف",
                        EntityType = d.EntityType,
                        EntityId = entityId,
                        Details = d.EntityType + " (معرف: " + entityId + ")",
                        OldValues = d.OldValues,
                        NewValues = d.NewValues,
                        ChangedFields = d.ChangedFields,
                        IpAddress = d.IpAddress
                    });
                }

                result += base.SaveChanges();

                if (ownsTransaction)
                    transaction.Commit();

                return result;
            }
            catch
            {
                if (ownsTransaction && transaction != null)
                    transaction.Rollback();
                throw;
            }
            finally
            {
                if (transaction != null) transaction.Dispose();
            }
        }

        private sealed class AuditDraft
        {
            public System.Data.Entity.Infrastructure.DbEntityEntry Entry { get; set; }
            public System.Data.Entity.EntityState State { get; set; }
            public string EntityType { get; set; }
            public string OldValues { get; set; }
            public string NewValues { get; set; }
            public string ChangedFields { get; set; }
            public DateTime Timestamp { get; set; }
            public string UserName { get; set; }
            public string IpAddress { get; set; }
        }

        private static readonly System.Collections.Generic.HashSet<string> AuditSensitiveProperties =
            new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "PasswordHash" };

        private static string BuildAuditValues(System.Data.Entity.Infrastructure.DbEntityEntry entry, System.Data.Entity.Infrastructure.DbPropertyValues values)
        {
            if (values == null) return null;
            var parts = new System.Collections.Generic.List<string>();
            foreach (var name in values.PropertyNames.OrderBy(n => n, StringComparer.OrdinalIgnoreCase))
            {
                if (AuditSensitiveProperties.Contains(name)) continue;
                object value = null;
                try { value = values[name]; } catch { }
                parts.Add("\"" + EscapeJson(name) + "\":" + ToJsonValue(value));
            }
            return "{" + string.Join(",", parts) + "}";
        }

        private static string BuildChangedFields(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            if (entry.State != System.Data.Entity.EntityState.Modified) return null;
            var changed = new System.Collections.Generic.List<string>();
            foreach (var name in entry.CurrentValues.PropertyNames)
            {
                if (AuditSensitiveProperties.Contains(name)) continue;
                try
                {
                    var oldValue = entry.OriginalValues[name];
                    var newValue = entry.CurrentValues[name];
                    if (!object.Equals(oldValue, newValue)) changed.Add(name);
                }
                catch { }
            }
            return string.Join(",", changed);
        }

        private static string ToJsonValue(object value)
        {
            if (value == null) return "null";
            if (value is bool) return ((bool)value) ? "true" : "false";
            if (value is DateTime) return "\"" + EscapeJson(((DateTime)value).ToString("o", CultureInfo.InvariantCulture)) + "\"";
            if (value is TimeSpan) return "\"" + EscapeJson(((TimeSpan)value).ToString()) + "\"";
            if (value is decimal) return ((decimal)value).ToString(CultureInfo.InvariantCulture);
            if (value is double) return ((double)value).ToString(CultureInfo.InvariantCulture);
            if (value is float) return ((float)value).ToString(CultureInfo.InvariantCulture);
            if (value is byte || value is short || value is int || value is long || value is sbyte || value is ushort || value is uint || value is ulong)
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            return "\"" + EscapeJson(Convert.ToString(value, CultureInfo.InvariantCulture)) + "\"";
        }

        private static string EscapeJson(string value)
        {
            if (value == null) return "";
            return value.Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\r", "\\r")
                        .Replace("\n", "\\n")
                        .Replace("\t", "\\t");
        }

        private static string GetKeyValue(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            try
            {
                var values = entry.CurrentValues;
                var candidates = new[] { "Id", entry.Entity.GetType().Name + "Id" };
                foreach (var name in candidates)
                {
                    if (values.PropertyNames.Contains(name))
                    {
                        var v = values[name];
                        return v == null ? "" : v.ToString();
                    }
                }
            }
            catch { }
            return "";
        }

    }
}
