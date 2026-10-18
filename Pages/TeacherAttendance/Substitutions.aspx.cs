using NoorAlMalaikaSchool.Data;
using NoorAlMalaikaSchool.Data.Helpers;
using NoorAlMalaikaSchool.Models;
using NoorAlMalaikaSchool.Services;
using System;
using System.Globalization;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SubstitutionEntity = NoorAlMalaikaSchool.Models.Substitution;

namespace NoorAlMalaikaSchool.Pages.TeacherAttendance
{
    public partial class Substitutions : Page
    {
        private readonly SchoolDbContext db = new SchoolDbContext();

        #region Page Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityHelper.DenyUnlessPath();
            try
            {
                if (!IsPostBack)
                {
                    txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    LoadClasses();
                    LoadOriginalTeachers();
                    LoadSubstituteTeachers();
                    LoadLessons();
                    BindSubstitutions();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "Substitutions.Page_Load");
            }
        }

        #endregion

        #region Load

        private void LoadClasses()
        {
            var stage = EnumHelper.GetCurrentStage();
            var classRoomIds = EnumHelper.GetClassRoomIdsForStage(db, stage);

            ddlClass.Items.Clear();
            ddlClass.Items.Add(new ListItem("جميع الفصول", ""));

            var classes = db.ClassRooms
                .Where(c => classRoomIds.Contains(c.Id))
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            foreach (var c in classes)
                ddlClass.Items.Add(new ListItem(c.Name, c.Id.ToString()));
        }

        private void LoadOriginalTeachers()
        {
            var stage = EnumHelper.GetCurrentStage();

            ddlOriginalTeacher.Items.Clear();
            ddlOriginalTeacher.Items.Add(new ListItem("اختر المعلم الأصلي", ""));

            var teachers = db.Teachers
                .Where(t => t.Stage == stage && t.IsActive)
                .OrderBy(t => t.Name)
                .Select(t => new { t.Id, t.Name })
                .ToList();

            foreach (var t in teachers)
                ddlOriginalTeacher.Items.Add(new ListItem(t.Name, t.Id.ToString()));
        }

        private void LoadSubstituteTeachers()
        {
            var stage = EnumHelper.GetCurrentStage();

            ddlSubstituteTeacher.Items.Clear();
            ddlSubstituteTeacher.Items.Add(new ListItem("اختر المعلم المُعوِّض", ""));

            var teachers = db.Teachers
                .Where(t => t.Stage == stage && t.IsActive)
                .OrderBy(t => t.Name)
                .Select(t => new { t.Id, t.Name })
                .ToList();

            foreach (var t in teachers)
                ddlSubstituteTeacher.Items.Add(new ListItem(t.Name, t.Id.ToString()));
        }

        private void LoadLessons()
        {
            DateTime date;
            if (!DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                LoadEmptyLesson();
                return;
            }

            int originalTeacherId;
            if (!int.TryParse(ddlOriginalTeacher.SelectedValue, out originalTeacherId) || originalTeacherId <= 0)
            {
                LoadEmptyLesson();
                return;
            }

            var lessons = db.DailyLessons
                .Where(d => d.Date == date && d.TeacherId == originalTeacherId)
                .OrderBy(d => d.PeriodNumber)
                .Select(d => new
                {
                    d.Id,
                    d.PeriodNumber,
                    SubjectName = d.Subject.Name,
                    ClassRoomName = d.ClassRoom.Name
                })
                .ToList();

            ddlLesson.Items.Clear();
            ddlLesson.Items.Add(new ListItem("اختر الحصة", ""));

            foreach (var l in lessons)
                ddlLesson.Items.Add(new ListItem(
                    "الحصة " + l.PeriodNumber + " — " + l.SubjectName + " — " + l.ClassRoomName,
                    l.Id.ToString()));
        }

        private void LoadEmptyLesson()
        {
            ddlLesson.Items.Clear();
            ddlLesson.Items.Add(new ListItem("اختر الحصة", ""));
        }

        #endregion

        #region Add

        protected void ddlOriginalTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLessons();
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            LoadLessons();
            BindSubstitutions();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                int lessonId;
                if (!int.TryParse(ddlLesson.SelectedValue, out lessonId) || lessonId <= 0)
                {
                    ShowMessage("اختر الحصة أولاً", false);
                    return;
                }

                int substituteId;
                if (!int.TryParse(ddlSubstituteTeacher.SelectedValue, out substituteId) || substituteId <= 0)
                {
                    ShowMessage("اختر المعلم المُعوِّض", false);
                    return;
                }

                DateTime date;
                if (!DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    ShowMessage("التاريخ غير صحيح", false);
                    return;
                }

                string lockMessage;
                if (new TeacherAttendanceFinancialService(db).IsDateLocked(date, out lockMessage))
                {
                    ShowMessage(lockMessage, false);
                    return;
                }

                var lesson = db.DailyLessons
                    .Include(d => d.Subject)
                    .Include(d => d.ClassRoom)
                    .FirstOrDefault(d => d.Id == lessonId);

                if (lesson == null) { ShowMessage("الحصة غير موجودة", false); return; }

                if (lesson.Date.Date != date.Date)
                {
                    ShowMessage("تاريخ التعويض يجب أن يطابق تاريخ الحصة اليومية", false);
                    return;
                }

                int selectedOriginalId;
                if (int.TryParse(ddlOriginalTeacher.SelectedValue, out selectedOriginalId) && selectedOriginalId > 0)
                {
                    if (lesson.TeacherId != selectedOriginalId)
                    {
                        ShowMessage("الحصة المختارة لا تنتمي للمعلم الأصلي المحدد", false);
                        return;
                    }
                }

                var substitute = db.Teachers.FirstOrDefault(t => t.Id == substituteId && t.IsActive);
                if (substitute == null) { ShowMessage("المعلم المُعوِّض غير موجود أو غير نشط", false); return; }

                if (lesson.TeacherId == substituteId)
                {
                    ShowMessage("المعلم المُعوِّض هو المعلم الأصلي نفسه", false);
                    return;
                }

                var originalTeacher = db.Teachers.FirstOrDefault(t => t.Id == lesson.TeacherId);
                if (originalTeacher != null && substitute.Stage != originalTeacher.Stage)
                {
                    ShowMessage("لا يمكن تعيين معلم من مرحلة تعليمية مختلفة", false);
                    return;
                }

                AttendanceStatus status;
                if (!Enum.TryParse<AttendanceStatus>(ddlStatus.SelectedValue, true, out status) ||
                    (status != AttendanceStatus.SubstitutionFull && status != AttendanceStatus.SubstitutionPartial))
                {
                    ShowMessage("نوع التعويض غير صحيح", false);
                    return;
                }

                if (db.Substitutions.Any(s => s.DailyLessonId == lessonId))
                {
                    ShowMessage("يوجد تعويض مسجل مسبقاً لهذه الحصة", false);
                    return;
                }

                // ⭐ فحص تعارض 1: المعلم المُعوِّض لديه حصة أساسية بنفس الوقت
                var substituteOwnLesson = db.DailyLessons
                    .Include(d => d.Subject)
                    .Include(d => d.ClassRoom)
                    .FirstOrDefault(d => d.TeacherId == substituteId
                                         && d.Date == date.Date
                                         && d.PeriodNumber == lesson.PeriodNumber);

                if (substituteOwnLesson != null)
                {
                    ShowMessage(
                        "المعلم المُعوِّض لديه حصة في جدوله الأساسي بنفس الوقت " +
                        "(الحصة " + lesson.PeriodNumber + " — " +
                        substituteOwnLesson.Subject.Name + " — " +
                        substituteOwnLesson.ClassRoom.Name + "). اختر معلمًا آخر.",
                        false);
                    return;
                }

                // ⭐ فحص تعارض 2: المعلم المُعوِّض مسجل كمعوِّض لحصة أخرى بنفس الوقت
                var substituteOtherSub = db.Substitutions
                    .Include(s => s.Subject)
                    .Include(s => s.ClassRoom)
                    .FirstOrDefault(s => s.SubstituteTeacherId == substituteId
                                         && s.Date == date.Date
                                         && s.PeriodNumber == lesson.PeriodNumber);

                if (substituteOtherSub != null)
                {
                    ShowMessage(
                        "المعلم المُعوِّض مسجل بالفعل كمعلم مُعوِّض لحصة أخرى في نفس الوقت " +
                        "(الحصة " + lesson.PeriodNumber + " — " +
                        substituteOtherSub.Subject.Name + " — " +
                        substituteOtherSub.ClassRoom.Name + ").",
                        false);
                    return;
                }

                var originalAttendance = db.AttendanceRecords
                    .FirstOrDefault(r => r.DailyLessonId == lessonId && r.TeacherId == lesson.TeacherId);

                if (originalAttendance != null &&
                    (originalAttendance.Status == AttendanceStatus.PresentFull ||
                     originalAttendance.Status == AttendanceStatus.PresentPartial))
                {
                    ShowMessage("لا يمكن تسجيل تعويض لحصة مسجل فيها حضور للمعلم الأصلي", false);
                    return;
                }

                db.Substitutions.Add(new SubstitutionEntity
                {
                    DailyLessonId = lesson.Id,
                    Date = lesson.Date.Date,
                    ClassRoomId = lesson.ClassRoomId,
                    PeriodNumber = lesson.PeriodNumber,
                    SubjectId = lesson.SubjectId,
                    OriginalTeacherId = lesson.TeacherId,
                    SubstituteTeacherId = substituteId,
                    Status = status,
                    Notes = txtNotes.Text.Trim(),
                    RecordedBy = EnumHelper.CurrentUserName,
                    RecordedAt = DateTime.Now,
                    IsApproved = false
                });
                db.SaveChanges();

                txtNotes.Text = "";
                ShowMessage("تم تسجيل التعويضية — لا تدخل في الرواتب إلا بعد الاعتماد", true);
                BindSubstitutions();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Substitutions.btnAdd_Click");
            }
        }

        #endregion

        #region Grid Command

        protected void gvSubstitutions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (!SecurityHelper.CanAccess(UserRole.Supervisor))
                {
                    ShowMessage("هذا الإجراء يتطلب صلاحية مشرف", false);
                    BindSubstitutions();
                    return;
                }

                int id;
                if (!int.TryParse(e.CommandArgument as string, out id))
                {
                    ShowMessage("معرّف التعويضية غير صالح.", false);
                    return;
                }

                var sub = db.Substitutions.Find(id);
                if (sub == null)
                {
                    ShowMessage("التعويضية غير موجودة.", false);
                    return;
                }

                string lockMessage;
                if (new TeacherAttendanceFinancialService(db).IsDateLocked(sub.Date, out lockMessage))
                {
                    ShowMessage(lockMessage, false);
                    BindSubstitutions();
                    return;
                }

                var lesson = sub.DailyLessonId.HasValue
                    ? db.DailyLessons.FirstOrDefault(d => d.Id == sub.DailyLessonId.Value)
                    : null;

                if (e.CommandName == "Approve")
                {
                    ApproveSubstitution(sub, lesson);
                }
                else if (e.CommandName == "Unapprove")
                {
                    UnapproveSubstitution(sub);
                }
                else if (e.CommandName == "RemoveSub")
                {
                    DeleteSubstitution(sub);
                }

                BindSubstitutions();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Substitutions.gvSubstitutions_RowCommand");
                try { BindSubstitutions(); } catch { }
            }
        }

        /// <summary>
        /// حماية إضافية: منع GridView من محاولة الحذف الافتراضية.
        /// لا يُستدعى هذا الحدث لأن اسم الأمر تغيّر إلى "RemoveSub"،
        /// لكن إبقاؤه احتياطاً في حال تغيّر الاسم مستقبلاً.
        /// </summary>
        protected void gvSubstitutions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>اعتماد تعويضية وإنشاء سجلات الحضور المالية.</summary>
        private void ApproveSubstitution(SubstitutionEntity sub, DailyLesson lesson)
        {
            if (lesson == null)
            {
                ShowMessage("لا يمكن اعتماد تعويضية غير مرتبطة بحصة", false);
                return;
            }

            if (lesson.Date.Date != sub.Date.Date || lesson.ClassRoomId != sub.ClassRoomId ||
                lesson.PeriodNumber != sub.PeriodNumber || lesson.SubjectId != sub.SubjectId ||
                lesson.TeacherId != sub.OriginalTeacherId)
            {
                ShowMessage("بيانات التعويضية لا تطابق الحصة اليومية", false);
                return;
            }

            var substituteOwnLesson = db.DailyLessons
                .Include(d => d.Subject)
                .Include(d => d.ClassRoom)
                .FirstOrDefault(d => d.TeacherId == sub.SubstituteTeacherId
                                     && d.Date == sub.Date.Date
                                     && d.PeriodNumber == sub.PeriodNumber);

            if (substituteOwnLesson != null)
            {
                ShowMessage(
                    "لا يمكن اعتماد التعويضية — المعلم المُعوِّض لديه حصة أساسية " +
                    "بنفس الوقت (الحصة " + sub.PeriodNumber + " — " +
                    substituteOwnLesson.Subject.Name + " — " +
                    substituteOwnLesson.ClassRoom.Name + ").",
                    false);
                return;
            }

            var substituteOtherSub = db.Substitutions
                .Include(s => s.Subject)
                .Include(s => s.ClassRoom)
                .FirstOrDefault(s => s.Id != sub.Id
                                     && s.SubstituteTeacherId == sub.SubstituteTeacherId
                                     && s.Date == sub.Date.Date
                                     && s.PeriodNumber == sub.PeriodNumber);

            if (substituteOtherSub != null)
            {
                ShowMessage(
                    "لا يمكن اعتماد التعويضية — المعلم المُعوِّض مسجل كمعلم مُعوِّض " +
                    "لحصة أخرى في نفس الوقت (الحصة " + sub.PeriodNumber + " — " +
                    substituteOtherSub.Subject.Name + " — " +
                    substituteOtherSub.ClassRoom.Name + ").",
                    false);
                return;
            }

            var original = db.AttendanceRecords.FirstOrDefault(r =>
                r.DailyLessonId == lesson.Id && r.TeacherId == lesson.TeacherId);

            if (original != null &&
                (original.Status == AttendanceStatus.PresentFull || original.Status == AttendanceStatus.PresentPartial))
            {
                ShowMessage("لا يمكن اعتماد التعويضية لأن المعلم الأصلي مسجل له حضور", false);
                return;
            }

            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    if (original == null)
                    {
                        var originalTeacher = db.Teachers.Find(lesson.TeacherId);
                        original = new AttendanceRecord
                        {
                            DailyLessonId = lesson.Id,
                            Date = lesson.Date.Date,
                            TeacherId = lesson.TeacherId,
                            PeriodNumber = lesson.PeriodNumber,
                            Status = AttendanceStatus.AbsentUnexcused,
                            ActualMinutes = 0,
                            AbsentMinutes = 0,
                            LessonRateApplied = originalTeacher != null ? originalTeacher.LessonRate : 0m,
                            HourlyRateApplied = originalTeacher != null ? originalTeacher.HourlyRate : 0m,
                            Notes = "غياب بسبب تعويضية معتمدة",
                            RecordedBy = EnumHelper.CurrentUserName,
                            RecordedAt = DateTime.Now,
                            IsApproved = true,
                            ApprovedBy = EnumHelper.CurrentUserName,
                            ApprovedAt = DateTime.Now
                        };
                        db.AttendanceRecords.Add(original);
                    }

                    var substituteAttendance = db.AttendanceRecords.FirstOrDefault(r =>
                        r.DailyLessonId == lesson.Id && r.TeacherId == sub.SubstituteTeacherId);

                    var substitute = db.Teachers.Find(sub.SubstituteTeacherId);

                    if (substituteAttendance == null)
                    {
                        substituteAttendance = new AttendanceRecord
                        {
                            DailyLessonId = lesson.Id,
                            Date = lesson.Date.Date,
                            TeacherId = sub.SubstituteTeacherId,
                            PeriodNumber = lesson.PeriodNumber,
                            LessonRateApplied = substitute != null ? substitute.LessonRate : 0m,
                            HourlyRateApplied = substitute != null ? substitute.HourlyRate : 0m,
                            RecordedBy = EnumHelper.CurrentUserName,
                            RecordedAt = DateTime.Now
                        };
                        db.AttendanceRecords.Add(substituteAttendance);
                    }

                    substituteAttendance.Status = sub.Status;
                    substituteAttendance.ActualMinutes = sub.Status == AttendanceStatus.SubstitutionFull
                        ? lesson.DurationMinutes : lesson.DurationMinutes / 2;
                    substituteAttendance.AbsentMinutes = lesson.DurationMinutes - substituteAttendance.ActualMinutes;
                    substituteAttendance.SourceSubstitutionId = sub.Id;
                    substituteAttendance.Notes = "تعويضية معتمدة رقم " + sub.Id;
                    substituteAttendance.IsApproved = true;
                    substituteAttendance.ApprovedBy = EnumHelper.CurrentUserName;
                    substituteAttendance.ApprovedAt = DateTime.Now;

                    sub.IsApproved = true;
                    sub.ApprovedBy = EnumHelper.CurrentUserName;
                    sub.ApprovedAt = DateTime.Now;
                    db.SaveChanges();
                    tx.Commit();
                    ShowMessage("تم اعتماد التعويضية وربطها بسجل الحضور المالي", true);
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    try { AppErrorLogger.Log(ex, "Substitutions.ApproveSubstitution"); } catch { }
                    ShowMessage("تعذر اعتماد التعويضية: " + ex.Message, false);
                }
            }
        }

        /// <summary>
        /// التراجع عن اعتماد تعويضية وحذف سجلات الحضور المرتبطة.
        /// يحذف أسطر PayrollLessonLines أولاً لتجنب تعارض قيود المفاتيح الأجنبية.
        /// </summary>
        private void UnapproveSubstitution(SubstitutionEntity sub)
        {
            var period = db.PayrollPeriods
                .AsNoTracking()
                .FirstOrDefault(p => p.StartDate <= sub.Date && p.EndDate >= sub.Date);

            if (period != null)
            {
                if (period.Status == PayrollPeriodStatus.Closed)
                {
                    ShowMessage(
                        "لا يمكن التراجع — الفترة المالية مغلقة. " +
                        "يجب فتح الفترة أولاً من إدارة الفترات المالية.",
                        false);
                    return;
                }

                bool isCalculated = db.TeacherPayrolls
                    .Any(tp => tp.PayrollPeriodId == period.Id);

                if (isCalculated)
                {
                    ShowMessage(
                        "تنبيه: الفترة المالية محسوبة. سيتم حذف سجلات الحضور، " +
                        "لكن يجب إعادة حساب الفترة لتحديث الرواتب.",
                        true);
                }
            }

            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    var sourceRecords = db.AttendanceRecords
                        .Where(r => r.SourceSubstitutionId == sub.Id)
                        .ToList();

                    foreach (var sourceRecord in sourceRecords)
                    {
                        var lines = db.PayrollLessonLines
                            .Where(l => l.AttendanceRecordId == sourceRecord.Id)
                            .ToList();

                        if (lines.Count > 0)
                            db.PayrollLessonLines.RemoveRange(lines);
                    }

                    db.SaveChanges();

                    if (sourceRecords.Count > 0)
                    {
                        db.AttendanceRecords.RemoveRange(sourceRecords);
                        db.SaveChanges();
                    }

                    sub.IsApproved = false;
                    sub.ApprovedBy = null;
                    sub.ApprovedAt = null;
                    db.SaveChanges();

                    tx.Commit();
                    ShowMessage("تم التراجع عن اعتماد التعويضية. أعد حساب الفترة المالية لتحديث الرواتب.", true);
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    try { AppErrorLogger.Log(ex, "Substitutions.UnapproveSubstitution"); } catch { }
                    ShowMessage("تعذر التراجع عن الاعتماد: " + ex.Message, false);
                }
            }
        }

        /// <summary>
        /// حذف تعويضية نهائياً.
        ///  - إذا كانت معتمدة: يحذف سجلات الحضور و PayrollLessonLines المرتبطة بها أولاً.
        ///  - ثم يحذف سجل التعويضية نفسه.
        /// </summary>
        private void DeleteSubstitution(SubstitutionEntity sub)
        {
            var period = db.PayrollPeriods
                .AsNoTracking()
                .FirstOrDefault(p => p.StartDate <= sub.Date && p.EndDate >= sub.Date);

            if (period != null)
            {
                if (period.Status == PayrollPeriodStatus.Closed)
                {
                    ShowMessage(
                        "لا يمكن حذف التعويضية — الفترة المالية مغلقة. " +
                        "يجب فتح الفترة أولاً من إدارة الفترات المالية.",
                        false);
                    return;
                }

                bool isCalculated = db.TeacherPayrolls
                    .Any(tp => tp.PayrollPeriodId == period.Id);

                if (isCalculated)
                {
                    ShowMessage(
                        "تنبيه: الفترة المالية محسوبة. سيتم حذف التعويضية وسجلاتها، " +
                        "لكن يجب إعادة حساب الفترة لتحديث الرواتب.",
                        true);
                }
            }

            bool wasApproved = sub.IsApproved;

            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    if (wasApproved)
                    {
                        var sourceRecords = db.AttendanceRecords
                            .Where(r => r.SourceSubstitutionId == sub.Id)
                            .ToList();

                        foreach (var sourceRecord in sourceRecords)
                        {
                            var lines = db.PayrollLessonLines
                                .Where(l => l.AttendanceRecordId == sourceRecord.Id)
                                .ToList();

                            if (lines.Count > 0)
                                db.PayrollLessonLines.RemoveRange(lines);
                        }

                        db.SaveChanges();

                        if (sourceRecords.Count > 0)
                        {
                            db.AttendanceRecords.RemoveRange(sourceRecords);
                            db.SaveChanges();
                        }
                    }

                    db.Substitutions.Remove(sub);
                    db.SaveChanges();

                    tx.Commit();

                    ShowMessage(
                        "تم حذف التعويضية نهائياً. " +
                        (wasApproved ? "أعد حساب الفترة المالية لتحديث الرواتب." : ""),
                        true);
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    try { AppErrorLogger.Log(ex, "Substitutions.DeleteSubstitution"); } catch { }
                    ShowMessage("تعذر حذف التعويضية: " + ex.Message, false);
                }
            }
        }

        #endregion

        #region Bind

        private void BindSubstitutions()
        {
            DateTime date;
            if (!DateTime.TryParse(txtDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) return;

            int classId = 0;
            bool hasClass = int.TryParse(ddlClass.SelectedValue, out classId) && classId > 0;

            var stage = EnumHelper.GetCurrentStage();
            var stageTeacherIds = db.Teachers
                .Where(t => t.Stage == stage)
                .Select(t => t.Id)
                .ToList();

            if (stageTeacherIds.Count == 0)
            {
                gvSubstitutions.DataSource = null;
                gvSubstitutions.DataBind();
                pnlEmpty.Visible = true;
                return;
            }

            var rows = db.Substitutions
                .Where(s => s.Date == date)
                .Where(s => !hasClass || s.ClassRoomId == classId)
                .Where(s => stageTeacherIds.Contains(s.OriginalTeacherId))
                .OrderBy(s => s.PeriodNumber)
                .Select(s => new
                {
                    s.Id,
                    s.Date,
                    s.PeriodNumber,
                    s.IsApproved,
                    ClassName = s.ClassRoom.Name,
                    SubjectName = s.Subject.Name,
                    OriginalTeacherName = s.OriginalTeacher.Name,
                    SubstituteTeacherName = s.SubstituteTeacher.Name,
                    s.Status
                })
                .ToList()
                .Select(s => new
                {
                    s.Id,
                    s.PeriodNumber,
                    s.IsApproved,
                    DateText = s.Date.ToString("yyyy-MM-dd"),
                    s.ClassName,
                    s.SubjectName,
                    s.OriginalTeacherName,
                    s.SubstituteTeacherName,
                    StatusText = s.Status.GetDisplayName()
                })
                .ToList();

            gvSubstitutions.DataSource = rows;
            gvSubstitutions.DataBind();

            pnlEmpty.Visible = rows.Count == 0;
        }

        #endregion

        #region Messages / Error / Dispose

        private void ShowMessage(string text, bool success)
        {
            if (lblMessage == null) return;

            lblMessage.CssClass = success ? "ta-alert success" : "ta-alert danger";
            lblMessage.Text =
                "<i class='fas " + (success ? "fa-check-circle" : "fa-exclamation-triangle") + "'></i>" +
                "<span>" + Server.HtmlEncode(text) + "</span>";
        }

        private void HandleException(Exception ex, string source)
        {
            try { AppErrorLogger.Log(ex, source); }
            catch { }

            string userMessage = "حدث خطأ غير متوقع أثناء تنفيذ العملية. حاول مرة أخرى.";
            try
            {
                string mapped = ErrorMessageHelper.GetUserMessage(ex, source);
                if (!string.IsNullOrWhiteSpace(mapped))
                    userMessage = mapped;
            }
            catch { }

            ShowMessage(userMessage, false);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (db != null) db.Dispose();
            base.OnUnload(e);
        }

        #endregion
    }
}