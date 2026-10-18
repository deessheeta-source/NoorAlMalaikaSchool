using System.Linq;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;

namespace NoorAlMalaikaSchool.Data
{
    public static class SeedData
    {

        public static void Initialize(SchoolDbContext context)
        {
            // Never provision hard-coded production credentials.
            // A fresh installation may set NOOR_INITIAL_ADMIN_PASSWORD in the IIS process environment.
            if (!context.Users.Any())
            {
                string initialPassword = System.Environment.GetEnvironmentVariable("NOOR_INITIAL_ADMIN_PASSWORD");
                if (!string.IsNullOrEmpty(initialPassword) && initialPassword.Length >= 8)
                {
                    context.Users.Add(new User
                    {
                        Username = "admin",
                        PasswordHash = PasswordHasher.Hash(initialPassword),
                        MustChangePassword = true,
                        DisplayName = "مدير النظام",
                        IsActive = true,
                        Role = UserRole.Admin
                    });
                }
            }

            context.SaveChanges();

            if (context.GradeLevels.Any()) { context.SaveChanges(); return; }

            // Stage invariant: SortOrder 1..9 = basic; 10.. = secondary.
            context.GradeLevels.AddRange(new[] {
                new GradeLevel { Id = 1, Name = "الصف الأول", SortOrder = 1, Stage = EducationStage.Basic },
                new GradeLevel { Id = 2, Name = "الصف الثاني", SortOrder = 2, Stage = EducationStage.Basic },
                new GradeLevel { Id = 3, Name = "الصف الثالث", SortOrder = 3, Stage = EducationStage.Basic },
                new GradeLevel { Id = 4, Name = "الصف الرابع", SortOrder = 4, Stage = EducationStage.Basic },
                new GradeLevel { Id = 5, Name = "الصف الخامس", SortOrder = 5, Stage = EducationStage.Basic },
                new GradeLevel { Id = 6, Name = "الصف السادس", SortOrder = 6, Stage = EducationStage.Basic },
                new GradeLevel { Id = 7, Name = "الصف السابع", SortOrder = 7, Stage = EducationStage.Basic },
                new GradeLevel { Id = 8, Name = "الصف الثامن", SortOrder = 8, Stage = EducationStage.Basic },
                new GradeLevel { Id = 9, Name = "الصف التاسع", SortOrder = 9, Stage = EducationStage.Basic },
                new GradeLevel { Id = 10, Name = "أول ثانوي", SortOrder = 10, Stage = EducationStage.Secondary },
                new GradeLevel { Id = 11, Name = "ثاني ثانوي", SortOrder = 11, Stage = EducationStage.Secondary },
                new GradeLevel { Id = 12, Name = "ثالث ثانوي", SortOrder = 12, Stage = EducationStage.Secondary }
            });

            context.AcademicYears.Add(new AcademicYear
            {
                Id = 1,
                Name = "2026-2027",
                StartDate = new System.DateTime(2026, 9, 1),
                EndDate = new System.DateTime(2027, 6, 30),
                IsCurrent = true
            });

            context.SaveChanges();
        }
    }
}
