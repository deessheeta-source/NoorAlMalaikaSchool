using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>Centralized validation and storage for student photos.</summary>
    public static class FileUploadService
    {
        public const int MaxStudentPhotoBytes = 5 * 1024 * 1024;
        private const int MaxImageDimension = 4000;
        private const string StudentPhotoVirtualFolder = "~/Content/Uploads/Students";

        // =========================================================
        // التحقق من الصورة
        // =========================================================
        public static void ValidateStudentPhoto(FileUpload upload)
        {
            if (upload == null || !upload.HasFile) return;

            if (upload.PostedFile == null || upload.PostedFile.ContentLength <= 0)
                throw new InvalidOperationException("ملف الصورة فارغ.");

            if (upload.PostedFile.ContentLength > MaxStudentPhotoBytes)
                throw new InvalidOperationException("حجم الصورة يتجاوز الحد المسموح وهو 5 ميجابايت.");

            string extension = Path.GetExtension(upload.FileName);
            if (string.IsNullOrEmpty(extension))
                throw new InvalidOperationException("صيغة الصورة غير معروفة.");

            extension = extension.ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                throw new InvalidOperationException("يسمح فقط بصور JPG أو JPEG أو PNG.");

            Stream input = upload.PostedFile.InputStream;
            try
            {
                if (input.CanSeek) input.Position = 0;

                using (System.Drawing.Image source = System.Drawing.Image.FromStream(input, true, true))
                {
                    if (source.Width < 1 || source.Height < 1
                        || source.Width > MaxImageDimension
                        || source.Height > MaxImageDimension)
                    {
                        throw new InvalidOperationException(
                            "أبعاد الصورة تتجاوز الحد المسموح (4000×4000). ");
                    }
                }

                if (input.CanSeek) input.Position = 0;
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException("ملف الصورة غير صالح أو تالف.");
            }
        }

        // =========================================================
        // حفظ الصورة
        // =========================================================
        public static string SaveStudentPhoto(FileUpload upload, string physicalFolder)
        {
            if (upload == null || !upload.HasFile) return null;

            ValidateStudentPhoto(upload);

            if (string.IsNullOrEmpty(physicalFolder))
                throw new InvalidOperationException("مسار تخزين الصور غير متاح.");

            Directory.CreateDirectory(physicalFolder);

            string fileName = Guid.NewGuid().ToString("N") + ".jpg";
            string finalPath = Path.Combine(physicalFolder, fileName);
            string tempPath = finalPath + ".tmp";

            try
            {
                using (Stream input = upload.PostedFile.InputStream)
                using (System.Drawing.Image source = System.Drawing.Image.FromStream(input, true, true))
                {
                    if (source.Width < 1 || source.Height < 1
                        || source.Width > MaxImageDimension
                        || source.Height > MaxImageDimension)
                    {
                        throw new InvalidOperationException(
                            "أبعاد الصورة تتجاوز الحد المسموح (4000×4000). ");
                    }

                    using (var bitmap = new System.Drawing.Bitmap(
                        source.Width, source.Height, PixelFormat.Format24bppRgb))
                    using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        graphics.DrawImage(source, 0, 0, source.Width, source.Height);
                        bitmap.Save(tempPath, ImageFormat.Jpeg);
                    }
                }

                File.Move(tempPath, finalPath);

                return StudentPhotoVirtualFolder + "/" + fileName;
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException("ملف الصورة غير صالح أو تالف.");
            }
            catch
            {
                if (File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); } catch { }
                }
                throw;
            }
        }

        // =========================================================
        // الحصول على المسار الفعلي للصورة
        // =========================================================
        public static string GetStudentPhotoPhysicalPath(string photoPath, string physicalFolder)
        {
            if (string.IsNullOrWhiteSpace(photoPath) || string.IsNullOrWhiteSpace(physicalFolder))
                return null;

            string fileName = Path.GetFileName(photoPath);
            if (string.IsNullOrWhiteSpace(fileName)) return null;

            return Path.Combine(physicalFolder, fileName);
        }

        // =========================================================
        // حذف الصورة (يقبل مسار كامل أو اسم ملف)
        // =========================================================
        public static void DeleteStudentPhoto(string photoPathOrFileName, string physicalFolder)
        {
            if (string.IsNullOrWhiteSpace(photoPathOrFileName)) return;
            if (string.IsNullOrWhiteSpace(physicalFolder)) return;

            try
            {
                string fileName = Path.GetFileName(photoPathOrFileName);
                if (string.IsNullOrWhiteSpace(fileName)) return;

                string safePath = Path.GetFullPath(Path.Combine(physicalFolder, fileName));
                string safeRoot = Path.GetFullPath(physicalFolder);

                if (!safePath.StartsWith(safeRoot, StringComparison.OrdinalIgnoreCase))
                    return;

                if (File.Exists(safePath)) File.Delete(safePath);
            }
            catch
            {
                // لا تُفشل العملية بسبب خطأ حذف ملف
            }
        }
    }
}