using System;
using System.ComponentModel.DataAnnotations;

namespace NoorAlMalaikaSchool.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        // PBKDF2 encoded hash; legacy SHA-256 hashes are migrated after successful login.
        [Required]
        [StringLength(200)]
        public string PasswordHash { get; set; }

        // Forces the user to choose a new password after an administrative reset or initial provisioning.
        public bool MustChangePassword { get; set; }

        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }

        public bool IsActive { get; set; }

        // Persistent account lockout state. These fields are backed by Phase 7 DB migration.
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutUntil { get; set; }
        public DateTime? LastFailedLoginAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public UserRole Role { get; set; }

        // Optional link to a Teacher record (used when Role == Teacher)
        [StringLength(50)]
        public string EmployeeNo { get; set; }
    }
}
