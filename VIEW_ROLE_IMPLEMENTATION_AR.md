# تنفيذ دور عرض البيانات (View)

تمت إضافة الدور `View` في نهاية `UserRole` للحفاظ على القيم الرقمية للأدوار الحالية.

## الصفحات المسموح بها
- `Pages/Default.aspx`
- `Pages/ChangePassword.aspx`
- `Pages/AccessDenied.aspx`
- `Pages/Error.aspx`
- `Pages/Reports/StudentList.aspx`
- `Pages/Reports/AttendanceReport.aspx`
- `Pages/Attendance/Report.aspx`
- `Pages/Teachers/TeacherReport.aspx`
- `Pages/Payments/StudentReport.aspx`
- `Pages/Timetable/SchoolTimetable.aspx`
- `Pages/Reports/FinancialReport.aspx`
- `Pages/Reports/StudentFinancialTotals.aspx`
- `Pages/Reports/StudentFinancialStatement.aspx`

يتم تطبيق قائمة السماح في `SecurityHelper.RequireRolesForPath` على الخادم. أي مسار آخر يُرفض لدور View، بما في ذلك صفحات الإدارة والتعديل والحذف والفواتير والمدفوعات والحضور والتعويضيات وإدارة المستخدمين.

تُخفى أزرار التصدير والطباعة في صفحات التقارير المسموح بها، كما تتحقق معالجات التصدير على الخادم من الدور قبل التنفيذ. تبقى فلاتر التقارير وعرض النتائج متاحة.

أُضيف الدور إلى قوائم إنشاء المستخدم وتعديل الدور. لا يحتاج هذا التغيير إلى تعديل قاعدة البيانات لأن الدور محفوظ كقيمة enum عددية، والقيمة الجديدة أُضيفت في نهاية التعداد.

## التحقق
تم فحص التعديلات نصيًا وربط الملفات بالمشروع الحالي. لم يتم تشغيل بناء ASP.NET Web Forms/.NET Framework 4.8 في بيئة التنفيذ هذه؛ يرجى فتح الحل في Visual Studio وتشغيل Rebuild واختبار حساب View عمليًا قبل النشر.
