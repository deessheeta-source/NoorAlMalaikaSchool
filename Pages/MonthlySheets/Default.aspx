<%@ Page Title="الصحيفة الشهرية" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.MonthlySheets.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        
        .ms-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 0; }

        /* ---------- HERO ---------- */
        .ms-hero { position: relative; background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .ms-hero::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%), radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%); pointer-events: none; }
        .ms-hero::after { content: ''; position: absolute; top: -40%; left: -10%; width: 340px; height: 340px; border-radius: 50%; background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%); pointer-events: none; }
        .ms-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 1280px; margin: 0 auto; }
        .ms-hero-left { display: flex; align-items: center; gap: 20px; }
        .ms-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; box-shadow: 0 8px 24px rgba(0,0,0,0.12); }
        .ms-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px; }
        .ms-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; }
        .ms-hero-actions { display: flex; gap: 10px; flex-wrap: wrap; flex-shrink: 0; }
        .ms-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .ms-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }
        .ms-btn-glass.primary { background: #fff; color: #00b14f; border-color: #fff; }
        .ms-btn-glass.primary:hover { background: #f8f9fa; color: #007a33; }

        /* ---------- BODY ---------- */
        .ms-body { max-width: 1280px; margin: -56px auto 0;padding: 0 24px 0; position: relative; z-index: 3; }

        /* ---------- CARD ---------- */
        .ms-card { background: #ffffff; border-radius: 20px; border: 1px solid #99f6c3; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 20px; }
        .ms-card-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .ms-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .ms-card-header-icon.blue { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .ms-card-header-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .ms-card-header-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .ms-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .ms-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .ms-card-body { padding: 20px 24px; }
        .ms-card-body.no-padding { padding: 0; }

        /* ---------- FILTERS ---------- */
        .ms-filters { display: grid; grid-template-columns: repeat(auto-fit, minmax(190px, 1fr)); gap: 14px; }
        .ms-field { display: flex; flex-direction: column; gap: 6px; min-width: 0; }
        .ms-field label { font-size: 12.5px; font-weight: 700; color: #495057; letter-spacing: 0.2px; display: flex; align-items: center; gap: 6px; }
        .ms-field label i { color: #00b14f; font-size: 12px; }
        .ms-field .form-control,
        .ms-field .form-select { border: 1.5px solid #e9ecef; border-radius: 11px; padding: 10px 14px; font-size: 13px; font-weight: 600; color: #212529; transition: all 0.2s ease; background: #f9fafb; font-family: 'Droid Arabic Kufi', sans-serif; height: 44px; width: 100%; }
        .ms-field .form-control:focus,
        .ms-field .form-select:focus { border-color: #00b14f; background: #fff; box-shadow: 0 0 0 4px rgba(0,177,79,0.12); outline: none; }

        /* ---------- HINT ---------- */
        .ms-hint { display: flex; align-items: center; gap: 12px; padding: 14px 18px; border-radius: 12px; background: linear-gradient(135deg, rgba(13,110,253,0.05) 0%, rgba(13,110,253,0.02) 100%); border: 1px solid rgba(13,110,253,0.2); color: #0d6efd; font-size: 12.5px; font-weight: 600; margin-top: 16px; line-height: 1.6; }
        .ms-hint-icon { width: 36px; height: 36px; border-radius: 10px; background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 15px; flex-shrink: 0; box-shadow: 0 4px 12px rgba(13,110,253,0.3); }

        /* ---------- SHEET WRAPPER ---------- */
        .ms-sheet-wrap { padding: 24px; }

        /* ---------- EMPTY STATE ---------- */
        .ms-empty { text-align: center; padding: 60px 20px; }
        .ms-empty-icon { width: 80px; height: 80px; border-radius: 22px; background: linear-gradient(135deg, #e6f7ec 0%, #d0eedd 100%); color: #00b14f; display: inline-flex; align-items: center; justify-content: center; font-size: 36px; margin-bottom: 18px; }
        .ms-empty h4 { color: #1f3d33; font-weight: 800; font-size: 16px; margin: 0 0 8px; }
        .ms-empty p { color: #6c757d; font-size: 13px; margin: 0; }

        /* ---------- RESPONSIVE ---------- */
        @media (max-width: 992px) {
            .ms-hero { padding: 26px 20px 72px; }
            .ms-hero-titles h1 { font-size: 19px; }
            .ms-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .ms-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .ms-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .ms-hero-titles h1 { font-size: 16px; }
            .ms-card-body { padding: 16px; }
            .ms-sheet-wrap { padding: 16px; }
        }

        /* ---------- PRINT ---------- */
        @media print {
            .ms-hero, .ms-hero-actions, .ms-btn-glass,
            .ms-card-header, .no-print,
            .ms-hint { display: none !important; }
            .ms-page { background: #fff !important; }
            .ms-body { margin: 0 !important; padding: 0 !important; max-width: 100% !important; }
            .ms-card { box-shadow: none !important; border: none !important; margin: 0 !important; border-radius: 0 !important; }
            .ms-card-body { padding: 0 !important; }
            .ms-sheet-wrap { padding: 0 !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ms-page" dir="rtl">

        <!-- ================== HERO ================== -->
        <div class="ms-hero">
            <div class="ms-hero-content">
                <div class="ms-hero-left">
                    <div class="ms-hero-icon">
                        <i class="fas fa-file-alt"></i>
                    </div>
                    <div class="ms-hero-titles">
                        <h1>الصحيفة الشهرية للتلميذ</h1>
                        <p>عرض، تعديل، وطباعة الصحيفة الشهرية لمستوى التلميذ</p>
                    </div>
                </div>
                <div class="ms-hero-actions no-print">
                    <asp:Button ID="btnSave" runat="server" Text="حفظ الدرجات"
                        CssClass="ms-btn-glass primary"
                        OnClick="btnSave_Click" />
                    <asp:Button ID="btnPrint" runat="server" Text="طباعة الصحيفة"
                        CssClass="ms-btn-glass"
                        OnClientClick="window.print();return false;" />
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="ms-body">

            <!-- ============ Filter Card ============ -->
            <div class="ms-card no-print">
                <div class="ms-card-header">
                    <div class="ms-card-header-icon blue">
                        <i class="fas fa-filter"></i>
                    </div>
                    <div class="ms-card-header-text">
                        <h3>اختيار التلميذ</h3>
                        <p>حدد السنة، الصف، القسم، الشعبة ثم اختر التلميذ</p>
                    </div>
                </div>
                <div class="ms-card-body">
                    <div class="ms-filters">

                        <div class="ms-field">
                            <label><i class="fas fa-calendar-alt"></i> السنة الدراسية</label>
                            <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="— اختر السنة —" />
                            </asp:DropDownList>
                        </div>

                        <div class="ms-field">
                            <label><i class="fas fa-graduation-cap"></i> الصف الدراسي</label>
                            <asp:DropDownList ID="ddlGradeLevel" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="— اختر الصف —" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlSectionFilter" runat="server" CssClass="ms-field" Visible="false">
                            <label><i class="fas fa-stream"></i> القسم</label>
                            <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSection_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الأقسام" Value="" Selected="True" />
                                <asp:ListItem Text="القسم العلمي" Value="علمي" />
                                <asp:ListItem Text="القسم الأدبي" Value="أدبي" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="ms-field">
                            <label><i class="fas fa-chair"></i> الشعبة</label>
                            <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">
                                <asp:ListItem Text="جميع الشعب" Value="" Selected="True" />
                            </asp:DropDownList>
                        </div>

                        <div class="ms-field">
                            <label><i class="fas fa-user-graduate"></i> التلميذ</label>
                            <asp:DropDownList ID="ddlStudent" runat="server" CssClass="form-select"
                                AppendDataBoundItems="true" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlStudent_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="— اختر التلميذ —" />
                            </asp:DropDownList>
                        </div>

                    </div>

                    <div class="ms-hint">
                        <div class="ms-hint-icon"><i class="fas fa-info"></i></div>
                        <div>
                            اختر الصف ثم التلميذ لعرض الصحيفة الشهرية. أدخل الدرجات في الخلايا ثم اضغط «حفظ الدرجات».
                            عند الطباعة تُعرض الصحيفة بمقاس A4 مع إمكانية تعبئة الحقول يدوياً.
                        </div>
                    </div>
                </div>
            </div>

            <!-- ============ Sheet Card ============ -->
            <div class="ms-card">
                <div class="ms-card-header no-print">
                    <div class="ms-card-header-icon">
                        <i class="fas fa-file-invoice"></i>
                    </div>
                    <div class="ms-card-header-text">
                        <h3>الصحيفة الشهرية</h3>
                        <p>الدرجات والتقييمات الشهرية للتلميذ</p>
                    </div>
                </div>

                <div class="ms-sheet-wrap">
                    <asp:Literal ID="litSheet" runat="server" />
                </div>
            </div>

        </div>
    </div>
</asp:Content>