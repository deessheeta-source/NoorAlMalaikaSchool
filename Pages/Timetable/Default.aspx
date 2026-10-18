<%@ Page Title="الجدول الدراسي" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Timetable.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Timetable
           ========================================================= */
        .tt-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 0; }

        /* ---------- HERO ---------- */
        .tt-hero { position: relative; background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .tt-hero::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%), radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%); pointer-events: none; }
        .tt-hero::after { content: ''; position: absolute; top: -40%; left: -10%; width: 340px; height: 340px; border-radius: 50%; background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%); pointer-events: none; }
        .tt-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 1280px; margin: 0 auto; }
        .tt-hero-left { display: flex; align-items: center; gap: 20px; }
        .tt-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; box-shadow: 0 8px 24px rgba(0,0,0,0.12); }
        .tt-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px; }
        .tt-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; }
        .tt-stage-label { display: inline-flex; align-items: center; gap: 6px; margin-inline-start: 8px; padding: 3px 12px; border-radius: 999px; background: rgba(255,255,255,0.15); border: 1px solid rgba(255,255,255,0.3); font-size: 12px; font-weight: 700; color: #fff; }
        .tt-stage-label i { font-size: 11px; }
        .tt-hero-actions { display: flex; gap: 10px; flex-wrap: wrap; flex-shrink: 0; }
        .tt-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .tt-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }
        .tt-btn-glass.primary { background: #fff; color: #00b14f; border-color: #fff; }
        .tt-btn-glass.primary:hover { background: #f8f9fa; color: #007a33; }

        /* ---------- BODY ---------- */
        .tt-body { max-width: 1280px; margin: -56px auto 0; padding: 0 24px 0; position: relative; z-index: 3; }

        /* ---------- CARD ---------- */
        .tt-card { background: #ffffff; border-radius: 20px; border: 1px solid #99f6c3; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 20px; }
        .tt-card-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .tt-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .tt-card-header-icon.blue { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .tt-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .tt-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .tt-card-body { padding: 20px 24px; }

        /* ---------- FILTERS ---------- */
        .tt-filters { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 12px; align-items: end; margin-bottom: 0; }
        .tt-field { display: flex; flex-direction: column; gap: 6px; min-width: 0; }
        .tt-field label { font-size: 12.5px; font-weight: 700; color: #495057; letter-spacing: 0.2px; display: flex; align-items: center; gap: 6px; }
        .tt-field label i { color: #00b14f; font-size: 12px; }
        .tt-field label .req { color: #dc3545; }
        .tt-field .form-control,
        .tt-field .form-select { border: 1.5px solid #e9ecef; border-radius: 11px; padding: 10px 14px; font-size: 13px; font-weight: 600; color: #212529; transition: all 0.2s ease; background: #f9fafb; font-family: 'Droid Arabic Kufi', sans-serif; height: 44px; width: 100%; }
        .tt-field .form-control:focus,
        .tt-field .form-select:focus { border-color: #00b14f; background: #fff; box-shadow: 0 0 0 4px rgba(0,177,79,0.12); outline: none; }
        .tt-filter-actions { display: flex; gap: 8px; flex-wrap: wrap; }

        .tt-btn-primary { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 11px 22px; border-radius: 11px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); border: none; color: #fff; font-size: 13px; font-weight: 800; cursor: pointer; transition: all 0.25s ease; box-shadow: 0 6px 16px rgba(0,177,79,0.3); height: 44px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .tt-btn-primary:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(0,177,79,0.4); color: #fff; }
        .tt-btn-outline { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 11px 22px; border-radius: 11px; background: #fff; border: 1.5px solid #dee2e6; color: #495057; font-size: 13px; font-weight: 700; cursor: pointer; transition: all 0.2s ease; height: 44px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; text-decoration: none; }
        .tt-btn-outline:hover { border-color: #6c757d; background: #f8f9fa; color: #343a40; text-decoration: none; }
        .tt-btn-blue { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 11px 22px; border-radius: 11px; background: #fff; border: 1.5px solid #0d6efd; color: #0d6efd; font-size: 13px; font-weight: 700; cursor: pointer; transition: all 0.2s ease; height: 44px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; text-decoration: none; }
        .tt-btn-blue:hover { background: rgba(13,110,253,0.06); color: #0d6efd; text-decoration: none; }

        /* ---------- MESSAGE ---------- */
        .tt-message { display: block; padding: 14px 18px; border-radius: 12px; font-size: 13px; font-weight: 700; margin-bottom: 16px; }
        .tt-message.danger { background: rgba(220,53,69,0.08); color: #a51c1c; border: 1px solid rgba(220,53,69,0.25); }
        .tt-message.success { background: rgba(0,177,79,0.08); color: #007a33; border: 1px solid rgba(0,177,79,0.25); }
        .tt-message:empty { display: none; padding: 0; margin: 0; }

        /* ---------- TABLE ---------- */
        .table-timetable { table-layout: fixed; width: 100%; border-collapse: separate; border-spacing: 0; font-size: 13px; margin: 0; }
        .table-timetable thead th {
            background: linear-gradient(180deg, #f8fdfa 0%, #f0f8f3 100%);
            color: #1f3d33; font-weight: 800; font-size: 12px;
            padding: 14px 6px; border: 1px solid #d8e6dd;
            text-align: center; white-space: nowrap;
        }
        .table-timetable thead th:first-child { border-radius: 12px 0 0 0; }
        .table-timetable thead th:last-child { border-radius: 0 12px 0 0; }
        .table-timetable tbody th {
            background: #f9fbfa; font-size: 13px; font-weight: 800;
            color: #007a33; padding: 12px 6px;
            border: 1px solid #e0e6ea; text-align: center;
            width: 90px;
        }
        .table-timetable tbody tr:last-child th { border-radius: 0 0 0 12px; }
        .table-timetable tbody tr:last-child td:last-child { border-radius: 0 0 12px 0; }
        .table-timetable .timetable-cell {
            min-width: 100px;
            height: 90px;
            vertical-align: middle;
            padding: 8px 6px;
            border: 1px solid #e0e6ea;
            text-align: center;
            background: #fff;
            transition: all 0.15s ease;
        }
        .table-timetable .timetable-cell.has-entry { background: linear-gradient(135deg, #f8fdfa 0%, #f0f8f3 100%); }
        .table-timetable .timetable-cell.has-entry:hover { background: #e8f4ec; }
        .tt-subject { font-weight: 800; color: #007a33; font-size: 12.5px; margin-bottom: 4px; line-height: 1.3; }
        .tt-teacher { font-size: 11px; color: #6c757d; margin-bottom: 8px; line-height: 1.3; }
        .tt-actions { display: flex; justify-content: center; gap: 4px; }
        .tt-actions .btn { padding: 3px 8px; font-size: 11px; border-radius: 7px; background: #fff; transition: all 0.15s ease; }
        .tt-actions .btn:hover { transform: translateY(-1px); }
        .add-cell {
            display: inline-flex; align-items: center; justify-content: center;
            width: 34px; height: 34px; border-radius: 50%;
            background: rgba(0,177,79,0.08); color: #00b14f;
            font-size: 13px; text-decoration: none;
            transition: all 0.15s ease;
            border: 1.5px dashed rgba(0,177,79,0.4);
        }
        .add-cell:hover {
            background: #00b14f; color: #fff;
            text-decoration: none; transform: scale(1.1);
            border-style: solid;
        }

        /* ---------- MODAL ---------- */
        .modal-overlay { display: none; position: fixed; inset: 0; background: rgba(15, 23, 42, 0.55); backdrop-filter: blur(3px); z-index: 1050; align-items: center; justify-content: center; padding: 16px; }
        .modal-overlay.show { display: flex; animation: fadeIn .15s ease; }
        @keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
        .modal-box { background: #fff; border-radius: 20px; max-width: 440px; width: 100%; box-shadow: 0 20px 60px rgba(0,0,0,0.2); overflow: hidden; animation: slideUp .2s ease; font-family: 'Droid Arabic Kufi', sans-serif; border: 1px solid #99f6c3; }
        @keyframes slideUp { from { transform: translateY(20px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }
        .modal-icon { width: 68px; height: 68px; border-radius: 20px; background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%); color: #fff; display: flex; align-items: center; justify-content: center; font-size: 28px; margin: 24px auto 14px; box-shadow: 0 8px 20px rgba(220,53,69,0.35); }
        .modal-title { text-align: center; font-size: 16px; font-weight: 800; color: #212529; margin: 0 0 8px; }
        .modal-text { text-align: center; font-size: 13.5px; color: #6c757d; margin: 0 24px 22px; line-height: 1.7; }
        .modal-actions { display: flex; gap: 10px; padding: 0 24px 24px; }
        .modal-btn { flex: 1; padding: 12px 16px; font-size: 13px; font-weight: 800; border-radius: 11px; border: 1.5px solid transparent; cursor: pointer; transition: all .15s ease; font-family: 'Droid Arabic Kufi', sans-serif; }
        .modal-btn-cancel { background: #fff; color: #495057; border-color: #dee2e6; }
        .modal-btn-cancel:hover { background: #f8f9fa; color: #212529; }
        .modal-btn-confirm { background: linear-gradient(135deg, #dc3545 0%, #f0616d 100%); color: #fff; border-color: transparent; box-shadow: 0 4px 12px rgba(220,53,69,0.3); }
        .modal-btn-confirm:hover { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(220,53,69,0.4); }

        /* ---------- EMPTY ---------- */
        .tt-empty { text-align: center; padding: 60px 20px; }
        .tt-empty-icon { width: 80px; height: 80px; border-radius: 22px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 36px; margin-bottom: 18px; }
        .tt-empty h4 { color: #1f3d33; font-weight: 800; font-size: 16px; margin: 0 0 8px; }
        .tt-empty p { color: #6c757d; font-size: 13px; margin: 0; }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .tt-hero { padding: 26px 20px 72px; }
            .tt-hero-titles h1 { font-size: 19px; }
            .tt-body { padding: 0 16px 40px; }
        }
        @media (max-width: 768px) {
            .table-timetable { font-size: 11px; min-width: 900px; }
            .table-timetable .timetable-cell { height: 70px; padding: 6px 4px; }
        }
        @media (max-width: 576px) {
            .tt-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .tt-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .tt-hero-titles h1 { font-size: 16px; }
            .tt-card-body { padding: 16px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .no-print, .tt-hero, .tt-hero-actions, .tt-btn-glass,
            .tt-filter-actions, .tt-actions, .add-cell,
            .modal-overlay { display: none !important; }
            .tt-page { background: #fff !important; }
            .tt-hero { background: #00923f !important; -webkit-print-color-adjust: exact; print-color-adjust: exact; border-radius: 0 !important; padding: 16px 20px !important; }
            .tt-body { margin-top: 0 !important; padding: 0 !important; }
            .tt-card { box-shadow: none !important; border: 1px solid #ddd !important; }
        }

        /* عمود اليوم: 14mm بالضبط */
        .table-timetable thead th:first-child {
            width: 14mm !important;
            min-width: 14mm !important;
            max-width: 14mm !important;
        }

        /* كل عمود حصة: 27.4mm بالضبط */
        .table-timetable thead th:nth-child(n+2),
        .table-timetable tbody td {
            width: 27.4mm !important;
            min-width: 27.4mm !important;
            max-width: 27.4mm !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="tt-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="tt-hero">
            <div class="tt-hero-content">
                <div class="tt-hero-left">
                    <div class="tt-hero-icon">
                        <i class="fas fa-calendar-alt"></i>
                    </div>
                    <div class="tt-hero-titles">
                        <h1>
                            الجدول الدراسي
                            <asp:Panel ID="pnlStageLabel" runat="server" CssClass="tt-stage-label">
                                <i class="fas fa-graduation-cap"></i>
                                <asp:Label ID="lblStage" runat="server" />
                            </asp:Panel>
                        </h1>
                        <p>عرض وإدارة الجدول الأسبوعي للفصول</p>
                    </div>
                </div>
                <div class="tt-hero-actions no-print">
                    <asp:Button ID="btnAddEntryTop" runat="server" Text="إضافة حصة"
                        CssClass="tt-btn-glass primary"
                        OnClick="btnAddEntry_Click" />
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="tt-body">

            <asp:Label ID="lblMessage" runat="server" CssClass="tt-message" />
            <asp:HiddenField ID="hfDeleteEntryId" runat="server" />
            <asp:HiddenField ID="hfForceDelete" runat="server" Value="false" />
            <asp:Button ID="btnDeleteEntry" runat="server" Text="حذف" CssClass="d-none"
                OnClick="btnDeleteEntry_Click" UseSubmitBehavior="false" />

            <!-- Filter Card -->
            <div class="tt-card no-print">
                <div class="tt-card-header">
                    <div class="tt-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="tt-card-header-text">
                        <h3>معايير العرض</h3>
                        <p>اختر الصف الدراسي والقسم والشعبة لعرض الجدول الأسبوعي</p>
                    </div>
                </div>
                <div class="tt-card-body">
                    <div class="tt-filters">

                        <div class="tt-field">
                            <label><i class="fas fa-graduation-cap"></i> الصف الدراسي <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlGradeLevel" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged" />
                        </div>

                        <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="tt-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">
                                <asp:ListItem Text="بدون قسم" Value="" Selected="True" />
                                <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="tt-field">
                            <label><i class="fas fa-chair"></i> الشعبة <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">
                                <asp:ListItem Text="بدون شعبة" Value="" Selected="True" />
                                <asp:ListItem Text="شعبة أ" Value="أ" />
                                <asp:ListItem Text="شعبة ب" Value="ب" />
                                <asp:ListItem Text="شعبة ج" Value="ج" />
                                <asp:ListItem Text="شعبة د" Value="د" />
                            </asp:DropDownList>
                        </div>

                        <div class="tt-field">
                            <label>&nbsp;</label>
                            <div class="tt-filter-actions">
                                <asp:Button ID="btnAddEntry" runat="server" Text="إضافة حصة"
                                    CssClass="tt-btn-primary"
                                    OnClick="btnAddEntry_Click" />
                                <button type="button" class="tt-btn-outline" onclick="window.print()">
                                    <i class="fas fa-print"></i> طباعة
                                </button>
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <!-- Timetable Card -->
            <div class="tt-card">
                <div class="tt-card-header no-print">
                    <div class="tt-card-header-icon">
                        <i class="fas fa-table"></i>
                    </div>
                    <div class="tt-card-header-text">
                        <h3>الجدول الأسبوعي</h3>
                        <p>الحصص من الأحد إلى الخميس</p>
                    </div>
                </div>

                <div class="tt-card-body" style="padding: 0;">
                    <asp:Literal ID="litTimetable" runat="server" />
                </div>
            </div>

        </div>
    </div>

    <%-- ============= MODAL: تأكيد الحذف ============= --%>
    <div id="deleteModal" class="modal-overlay no-print" role="dialog" aria-hidden="true">
        <div class="modal-box" role="document">
            <div class="modal-icon">
                <i class="fas fa-trash-alt"></i>
            </div>
            <h5 class="modal-title">تأكيد حذف الحصة</h5>
            <p class="modal-text">
                هل أنت متأكد من حذف هذه الحصة؟<br />
                <span style="color:#dc3545;font-weight:700;">
                    ⚠️ سيتم حذف جميع الدروس اليومية المرتبطة بها (حضور/غياب/درجات) نهائياً.
                </span>
                <br />
                <span style="color:#dc3545;font-weight:700;">لا يمكن التراجع عن هذا الإجراء.</span>
            </p>
            <div class="modal-actions">
                <button type="button" class="modal-btn modal-btn-cancel" onclick="closeDeleteModal();">
                    إلغاء
                </button>
                <button type="button" class="modal-btn modal-btn-confirm" onclick="confirmDelete();">
                    <i class="fas fa-trash-alt me-1"></i> نعم، احذف
                </button>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function openDeleteModal(entryId) {
            var modal = document.getElementById('deleteModal');
            var hf = document.getElementById('<%= hfDeleteEntryId.ClientID %>');
            var hfForce = document.getElementById('<%= hfForceDelete.ClientID %>');
            if (!modal || !hf) return false;

            hf.value = entryId;
            if (hfForce) hfForce.value = "false";   // إعادة تصفير علم التأكيد
            modal.classList.add('show');
            modal.setAttribute('aria-hidden', 'false');
            return false;
        }

        function closeDeleteModal() {
            var modal = document.getElementById('deleteModal');
            if (!modal) return;
            modal.classList.remove('show');
            modal.setAttribute('aria-hidden', 'true');
        }

        function confirmDelete() {
            var btn = document.getElementById('<%= btnDeleteEntry.ClientID %>');
            if (btn) {
                closeDeleteModal();
                btn.click();
            }
        }

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') closeDeleteModal();
        });

        document.addEventListener('click', function (e) {
            var modal = document.getElementById('deleteModal');
            if (modal && e.target === modal) closeDeleteModal();
        });
    </script>
</asp:Content>