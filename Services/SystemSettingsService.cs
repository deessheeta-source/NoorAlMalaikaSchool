using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;
using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Models;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>
    /// Key/value settings persisted in dbo.SystemSettings. Values are cached briefly
    /// because WhatsAppApiSettings reads them on every send/normalize call.
    /// Non-secret settings only: secrets belong in IIS environment variables.
    /// </summary>
    public static class SystemSettingsService
    {
        private const string CacheKey = "NoorAlMalaikaSchool.SystemSettings";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

        public static string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return string.Empty;
            string value;
            return LoadSnapshot().TryGetValue(key, out value) ? value : string.Empty;
        }

        public static IReadOnlyDictionary<string, string> GetAll()
        {
            return LoadSnapshot();
        }

        public static void Set(string key, string value, string updatedBy)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Setting key is required.", "key");

            using (var db = new SchoolDbContext())
            {
                string normalizedKey = key.Trim();
                SystemSetting row = db.SystemSettings.Find(normalizedKey);
                if (row == null)
                {
                    row = new SystemSetting { Key = normalizedKey };
                    db.SystemSettings.Add(row);
                }
                row.Value = value ?? string.Empty;
                row.UpdatedAt = DateTime.UtcNow;
                row.UpdatedBy = string.IsNullOrWhiteSpace(updatedBy) ? "system" : updatedBy.Trim();
                db.SaveChanges();
            }
            Invalidate();
        }

        public static void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            using (var db = new SchoolDbContext())
            {
                SystemSetting row = db.SystemSettings.Find(key.Trim());
                if (row == null)
                {
                    Invalidate();
                    return;
                }
                db.SystemSettings.Remove(row);
                db.SaveChanges();
            }
            Invalidate();
        }

        public static void Invalidate()
        {
            try { HttpRuntime.Cache.Remove(CacheKey); }
            catch { /* cache is an optimization; never fail the caller */ }
        }

        private static Dictionary<string, string> LoadSnapshot()
        {
            Cache cache = HttpRuntime.Cache;
            var cached = cache == null ? null : cache[CacheKey] as Dictionary<string, string>;
            if (cached != null) return cached;

            var snapshot = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using (var db = new SchoolDbContext())
                {
                    foreach (SystemSetting row in db.SystemSettings.AsNoTracking().ToList())
                        snapshot[row.Key] = row.Value ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                // The table may not exist yet on a fresh database; settings fall back
                // to Web.config and the module keeps working.
                Trace.TraceWarning("SystemSettings unavailable: " + ex.Message);
            }

            if (cache != null)
            {
                cache.Insert(CacheKey, snapshot, null,
                    DateTime.UtcNow.Add(CacheDuration), Cache.NoSlidingExpiration);
            }
            return snapshot;
        }
    }
}
