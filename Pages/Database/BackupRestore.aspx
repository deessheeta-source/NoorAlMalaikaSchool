<%@ Page Title="النسخ الاحتياطي والاستعادة" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BackupRestore.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Database.BackupRestore" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />

    <div class="container-fluid py-3" dir="rtl">
        <div class="academia-card">
            <div class="card-header border-bottom d-flex align-items-center justify-content-between py-3">
                <h5 class="mb-0 text-academia"><i class="fas fa-database me-2"></i>النسخ الاحتياطي ونقل قاعدة البيانات</h5>
                <asp:Button ID="btnRefresh" runat="server" Text="تحديث الملفات" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnRefresh_Click" CausesValidation="false" />
            </div>
            <div class="card-body">
                <asp:Label ID="litMessage" runat="server" EnableViewState="false" CssClass="d-block mb-3" />

                <div class="alert alert-info" role="note">
                    قاعدة التطبيق الحالية: <strong><asp:Literal ID="litDatabaseName" runat="server" /></strong>
                    <div class="small mt-1">مكان حفظ الملفات على الخادم: <code dir="ltr"><asp:Literal ID="litBackupPath" runat="server" /></code></div>
                    <div class="small mt-1">يجب أن يستطيع حساب خدمة SQL Server الكتابة إلى هذا المجلد، وأن يستطيع حساب الموقع قراءة الملفات وكتابتها. ضع ملفات النقل القادمة من بيئة أخرى في هذا المجلد ثم اختر «تحديث الملفات».</div>
                </div>

                <div class="row g-3 mb-3">
                    <div class="col-12 col-lg-6">
                        <section class="academia-card h-100">
                            <div class="card-body">
                                <h6 class="text-academia"><i class="fas fa-shield-alt me-2"></i>نسخة احتياطية كاملة (.bak)</h6>
                                <p class="text-muted small">نسخة SQL Server للاستعادة على قاعدة التطبيق الحالية. يتحقق النظام من سلامة النسخة بعد إنشائها.</p>
                                <asp:Button ID="btnCreateBackup" runat="server" Text="إنشاء نسخة احتياطية الآن" CssClass="btn btn-academia w-100" OnClick="btnCreateBackup_Click" />
                            </div>
                        </section>
                    </div>
                    <div class="col-12 col-lg-6">
                        <section class="academia-card h-100">
                            <div class="card-body">
                                <h6 class="text-academia"><i class="fas fa-exchange-alt me-2"></i>نقل القاعدة بين البيئات (.bacpac)</h6>
                                <p class="text-muted small">يتضمن ملف BACPAC مخطط قاعدة البيانات وبيانات الجداول، ويمكن استيراده كقاعدة جديدة في خادم آخر.</p>
                                <asp:Button ID="btnExportBacpac" runat="server" Text="تصدير ملف BACPAC" CssClass="btn btn-outline-success w-100" OnClick="btnExportBacpac_Click" />
                            </div>
                        </section>
                    </div>
                </div>

                <div class="row g-3">
                    <div class="col-12 col-lg-6">
                        <section class="academia-card h-100">
                            <div class="card-body">
                                <h6 class="text-danger"><i class="fas fa-history me-2"></i>استعادة نسخة .bak</h6>
                                <div class="alert alert-warning small">الاستعادة تستبدل بيانات قاعدة التطبيق الحالية. ينشئ النظام نسخة أمان تلقائية قبل الاستبدال. اكتب اسم القاعدة الحالية للتأكيد.</div>
                                <label for="ddlBackup" class="form-label fw-bold">النسخة الاحتياطية</label>
                                <asp:DropDownList ID="ddlBackup" runat="server" CssClass="form-select mb-3" />
                                <label for="txtRestoreConfirm" class="form-label fw-bold">اكتب اسم قاعدة البيانات للتأكيد</label>
                                <asp:TextBox ID="txtRestoreConfirm" runat="server" CssClass="form-control mb-3" dir="ltr" autocomplete="off" />
                                <div class="d-grid d-sm-flex gap-2">
                                    <asp:Button ID="btnRestoreBackup" runat="server" Text="استعادة واستبدال القاعدة الحالية" CssClass="btn btn-danger" OnClick="btnRestoreBackup_Click" OnClientClick="return confirm('سيتم استبدال قاعدة التطبيق الحالية. هل تريد المتابعة؟');" />
                                    <asp:Button ID="btnDownloadBackup" runat="server" Text="تنزيل النسخة المحددة" CssClass="btn btn-outline-secondary" OnClick="btnDownloadBackup_Click" CausesValidation="false" />
                                </div>
                            </div>
                        </section>
                    </div>

                    <div class="col-12 col-lg-6">
                        <section class="academia-card h-100">
                            <div class="card-body">
                                <h6 class="text-success"><i class="fas fa-file-import me-2"></i>استيراد BACPAC إلى قاعدة جديدة</h6>
                                <div class="alert alert-secondary small">لا يستبدل هذا الإجراء قاعدة التطبيق ولا يغيّر اتصال الموقع. بعد الاستيراد، افحص القاعدة ثم حدّث إعداد الاتصال عند نقل التطبيق إليها. استورد ملفات موثوقة فقط؛ فالملف يحتوي بيانات قاعدة المدرسة.</div>
                                <label for="ddlBacpac" class="form-label fw-bold">ملف BACPAC</label>
                                <asp:DropDownList ID="ddlBacpac" runat="server" CssClass="form-select mb-3" />
                                <label for="txtTargetDatabaseName" class="form-label fw-bold">اسم قاعدة البيانات الجديدة</label>
                                <asp:TextBox ID="txtTargetDatabaseName" runat="server" CssClass="form-control mb-3" dir="ltr" autocomplete="off" />
                                <div class="d-grid d-sm-flex gap-2">
                                    <asp:Button ID="btnImportBacpac" runat="server" Text="استيراد كقاعدة جديدة" CssClass="btn btn-success" OnClick="btnImportBacpac_Click" OnClientClick="return confirm('سيتم إنشاء قاعدة بيانات جديدة بهذا الاسم. هل تريد المتابعة؟');" />
                                    <asp:Button ID="btnDownloadBacpac" runat="server" Text="تنزيل الملف المحدد" CssClass="btn btn-outline-secondary" OnClick="btnDownloadBacpac_Click" CausesValidation="false" />
                                </div>
                            </div>
                        </section>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
