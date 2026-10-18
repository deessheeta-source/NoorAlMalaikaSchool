<%@ Page Title="إدارة الحصة" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Timetable.Edit" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <style>
        /* =========================================================
           Premium Design — Edit Lesson
           ========================================================= */
        .tt-page { font-family: 'Droid Arabic Kufi', sans-serif; background: #f7f9f8; min-height: 100vh; padding: 0; }

        .tt-hero { position: relative; background: linear-gradient(135deg, #00b14f 0%, #00923f 50%, #007a33 100%); padding: 32px 32px 78px; overflow: hidden; border-radius: 0 0 32px 32px; }
        .tt-hero::before { content: ''; position: absolute; inset: 0; background-image: radial-gradient(circle at 15% 20%, rgba(255,255,255,0.12) 0%, transparent 35%), radial-gradient(circle at 85% 80%, rgba(255,255,255,0.08) 0%, transparent 40%), radial-gradient(circle at 50% 50%, rgba(255,255,255,0.05) 0%, transparent 60%); pointer-events: none; }
        .tt-hero::after { content: ''; position: absolute; top: -40%; left: -10%; width: 340px; height: 340px; border-radius: 50%; background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%); pointer-events: none; }
        .tt-hero-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 24px; flex-wrap: wrap; max-width: 1100px; margin: 0 auto; }
        .tt-hero-left { display: flex; align-items: center; gap: 20px; }
        .tt-hero-icon { width: 68px; height: 68px; border-radius: 20px; background: rgba(255,255,255,0.18); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 30px; flex-shrink: 0; box-shadow: 0 8px 24px rgba(0,0,0,0.12); }
        .tt-hero-titles h1 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 4px; line-height: 1.3; letter-spacing: -0.2px; }
        .tt-hero-titles p { font-size: 13px; color: rgba(255,255,255,0.85); margin: 0; line-height: 1.5; }
        .tt-hero-actions { display: flex; gap: 10px; flex-wrap: wrap; flex-shrink: 0; }
        .tt-btn-glass { display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; border-radius: 12px; background: rgba(255,255,255,0.15); backdrop-filter: blur(10px); border: 1.5px solid rgba(255,255,255,0.3); color: #fff; font-size: 13px; font-weight: 700; text-decoration: none; transition: all 0.2s ease; cursor: pointer; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .tt-btn-glass:hover { background: rgba(255,255,255,0.25); color: #fff; text-decoration: none; transform: translateY(-2px); }

        .tt-body { max-width: 1100px; margin: -56px auto 0; padding: 0 24px 0; position: relative; z-index: 3; }

        .tt-card { background: #ffffff; border-radius: 20px; border: 1px solid #99f6c3; box-shadow: 0 8px 24px rgba(0,0,0,0.04); overflow: visible; margin-bottom: 20px; }
        .tt-card-header { display: flex; align-items: center; gap: 14px; padding: 18px 24px; border-bottom: 1px solid #f1f3f5; background: linear-gradient(180deg, #ffffff 0%, #fbfdfc 100%); flex-wrap: wrap; }
        .tt-card-header-icon { width: 44px; height: 44px; border-radius: 13px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); color: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; box-shadow: 0 6px 16px rgba(0,177,79,0.3); }
        .tt-card-header-icon.blue { background: linear-gradient(135deg, #0d6efd 0%, #3d8bfd 100%); box-shadow: 0 6px 16px rgba(13,110,253,0.3); }
        .tt-card-header-icon.purple { background: linear-gradient(135deg, #6f42c1 0%, #9d6ee6 100%); box-shadow: 0 6px 16px rgba(111,66,193,0.3); }
        .tt-card-header-icon.orange { background: linear-gradient(135deg, #fd7e14 0%, #ffa94d 100%); box-shadow: 0 6px 16px rgba(253,126,20,0.3); }
        .tt-card-header-text h3 { margin: 0; font-size: 16px; font-weight: 800; color: #212529; }
        .tt-card-header-text p { margin: 3px 0 0; font-size: 12.5px; color: #6c757d; font-weight: 500; }
        .tt-card-body { padding: 20px 24px; }

        .tt-fields { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 16px; }
        .tt-field { display: flex; flex-direction: column; gap: 6px; min-width: 0; }
        .tt-field.full { grid-column: 1 / -1; }
        .tt-field label { font-size: 12.5px; font-weight: 700; color: #495057; letter-spacing: 0.2px; display: flex; align-items: center; gap: 6px; }
        .tt-field label i { color: #00b14f; font-size: 12px; }
        .tt-field label .req { color: #dc3545; }

        .tt-field .form-control,
        .tt-field .form-select { border: 1.5px solid #e9ecef; border-radius: 11px; padding: 10px 14px; font-size: 13px; font-weight: 600; color: #212529; transition: all 0.2s ease; background: #f9fafb; font-family: 'Droid Arabic Kufi', sans-serif; height: 44px; width: 100%; }
        .tt-field .form-control:focus,
        .tt-field .form-select:focus { border-color: #00b14f; background: #fff; box-shadow: 0 0 0 4px rgba(0,177,79,0.12); outline: none; }
        .tt-field .text-danger { font-size: 11.5px; font-weight: 700; margin-top: 2px; }
        .tt-field small { font-size: 11px; color: #6c757d; margin-top: 4px; }

        /* =========================================================
           Custom Select — حل نهائي لقائمة الحالة
           ========================================================= */
        .custom-select { position: relative; width: 100%; }

        .custom-select-trigger {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 10px 14px;
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            background: #ffffff;
            font-size: 13px;
            font-weight: 600;
            color: #212529;
            cursor: pointer;
            height: 44px;
            font-family: 'Droid Arabic Kufi', sans-serif;
            transition: all 0.2s ease;
            user-select: none;
            -webkit-user-select: none;
        }

        .custom-select-trigger:hover {
            border-color: #00b14f;
            background: #f9fafb;
        }

        .custom-select.open .custom-select-trigger {
            border-color: #00b14f;
            background: #ffffff;
            box-shadow: 0 0 0 4px rgba(0,177,79,0.12);
        }

        .custom-select-trigger i {
            color: #00b14f;
            font-size: 12px;
            transition: transform 0.2s ease;
            flex-shrink: 0;
        }

        .custom-select.open .custom-select-trigger i {
            transform: rotate(180deg);
        }

        .custom-select-options {
            display: none;
            position: absolute;
            top: calc(100% + 6px);
            left: 0;
            right: 0;
            background: #ffffff;
            border: 1.5px solid #e9ecef;
            border-radius: 11px;
            box-shadow: 0 12px 32px rgba(0,0,0,0.14);
            z-index: 1000;
            overflow: hidden;
            padding: 6px;
            animation: fadeSlideDown 0.18s ease;
        }

        @keyframes fadeSlideDown {
            from { opacity: 0; transform: translateY(-6px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        .custom-select.open .custom-select-options {
            display: block;
        }

        .custom-select-option {
            padding: 10px 14px;
            font-size: 13px;
            font-weight: 600;
            color: #000000;
            background: #ffffff;
            cursor: pointer;
            border-radius: 8px;
            transition: background 0.15s ease, color 0.15s ease;
            font-family: 'Droid Arabic Kufi', sans-serif;
            user-select: none;
            -webkit-user-select: none;
            margin-bottom: 2px;
        }

        .custom-select-option:last-child { margin-bottom: 0; }

        .custom-select-option:hover {
            background: #e8f7ee;
            color: #007a33;
        }

        .custom-select-option.selected {
            background: #d1f2dd;
            color: #007a33;
        }

        .custom-select-option.selected::after {
            content: '\f00c';
            font-family: 'Font Awesome 5 Free';
            font-weight: 900;
            font-size: 11px;
            float: left;
            color: #007a33;
        }

        .tt-btn-primary { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 12px 32px; border-radius: 11px; background: linear-gradient(135deg, #00b14f 0%, #4ed17e 100%); border: none; color: #fff; font-size: 14px; font-weight: 800; cursor: pointer; transition: all 0.25s ease; box-shadow: 0 6px 16px rgba(0,177,79,0.3); height: 46px; white-space: nowrap; font-family: 'Droid Arabic Kufi', sans-serif; }
        .tt-btn-primary:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(0,177,79,0.4); color: #fff; }
        .tt-btn-outline { display: inline-flex; align-items: center; justify-content: center; gap: 8px; padding: 12px 28px; border-radius: 11px; background: #fff; border: 1.5px solid #dee2e6; color: #495057; font-size: 14px; font-weight: 700; cursor: pointer; transition: all 0.2s ease; height: 46px; text-decoration: none; font-family: 'Droid Arabic Kufi', sans-serif; }
        .tt-btn-outline:hover { border-color: #6c757d; background: #f8f9fa; color: #343a40; text-decoration: none; }
        .tt-actions { display: flex; justify-content: flex-end; gap: 10px; flex-wrap: wrap; margin-bottom: 40px; }

        .tt-message { display: block; padding: 14px 18px; border-radius: 12px; font-size: 13px; font-weight: 700; margin-bottom: 16px; }
        .tt-message.danger { background: rgba(220,53,69,0.08); color: #a51c1c; border: 1px solid rgba(220,53,69,0.25); }
        .tt-message.success { background: rgba(0,177,79,0.08); color: #007a33; border: 1px solid rgba(0,177,79,0.25); }
        .tt-message.d-none { display: none !important; }

        @media (max-width: 992px) {
            .tt-hero { padding: 26px 20px 72px; }
            .tt-hero-titles h1 { font-size: 19px; }
            .tt-body { padding: 0 16px 40px; }
        }
        @media (max-width: 576px) {
            .tt-hero { padding: 20px 16px 66px; border-radius: 0 0 24px 24px; }
            .tt-hero-icon { width: 54px; height: 54px; font-size: 24px; }
            .tt-hero-titles h1 { font-size: 16px; }
            .tt-card-body { padding: 16px; }
        }

        @media print {
            .tt-hero-actions, .tt-actions, .no-print { display: none !important; }
            .tt-page { background: #fff !important; }
            .tt-hero { background: #00923f !important; -webkit-print-color-adjust: exact; print-color-adjust: exact; border-radius: 0 !important; padding: 16px 20px !important; }
            .tt-body { margin-top: 0 !important; padding: 0 !important; }
            .tt-card { box-shadow: none !important; border: 1px solid #ddd !important; }
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
                        <i class="fas fa-clock"></i>
                    </div>
                    <div class="tt-hero-titles">
                        <h1>
                            <asp:Literal ID="litTitle" runat="server" Text="إضافة حصة" />
                        </h1>
                        <p>اختر الصف، اليوم، الحصة، المادة والمعلم ثم اضغط حفظ</p>
                    </div>
                </div>
                <div class="tt-hero-actions no-print">
                    <a href="Default.aspx" class="tt-btn-glass">
                        <i class="fas fa-arrow-right"></i> العودة
                    </a>
                </div>
            </div>
        </div>

        <!-- ================== BODY ================== -->
        <div class="tt-body">

            <asp:Label ID="lblMessage" runat="server" CssClass="tt-message d-none" />
            <asp:HiddenField ID="hfEntryId" runat="server" />

            <!-- ===== بطاقة: الصف واليوم ===== -->
            <div class="tt-card">
                <div class="tt-card-header">
                    <div class="tt-card-header-icon blue">
                        <i class="fas fa-chalkboard"></i>
                    </div>
                    <div class="tt-card-header-text">
                        <h3>الصف واليوم</h3>
                        <p>حدد الصف الدراسي واليوم والحصة</p>
                    </div>
                </div>
                <div class="tt-card-body">
                    <div class="tt-fields">

                        <div class="tt-field">
                            <label><i class="fas fa-graduation-cap"></i> الصف الدراسي <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlGradeLevel" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlGradeLevel_SelectedIndexChanged" />
                            <asp:RequiredFieldValidator ID="rfvGrade" runat="server"
                                ControlToValidate="ddlGradeLevel"
                                InitialValue=""
                                ErrorMessage="اختر الصف الدراسي"
                                CssClass="text-danger"
                                Display="Dynamic"
                                ValidationGroup="vgSave" />
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
                            <label><i class="fas fa-chair"></i> الشعبة</label>
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
                            <label><i class="fas fa-calendar-day"></i> اليوم <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlDay" runat="server" CssClass="form-select" />
                        </div>

                        <div class="tt-field">
                            <label><i class="fas fa-clock"></i> الحصة <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlPeriod" runat="server" CssClass="form-select" />
                        </div>

                    </div>
                </div>
            </div>

            <!-- ===== بطاقة: المادة والمعلم ===== -->
            <div class="tt-card">
                <div class="tt-card-header">
                    <div class="tt-card-header-icon purple">
                        <i class="fas fa-book"></i>
                    </div>
                    <div class="tt-card-header-text">
                        <h3>المادة والمعلم</h3>
                        <p>اختر المادة الدراسية والمعلم المسؤول</p>
                    </div>
                </div>
                <div class="tt-card-body">
                    <div class="tt-fields">

                        <div class="tt-field">
                            <label><i class="fas fa-book-open"></i> المادة الدراسية <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSubject_SelectedIndexChanged" />
                            <asp:RequiredFieldValidator ID="rfvSubject" runat="server"
                                ControlToValidate="ddlSubject"
                                InitialValue=""
                                ErrorMessage="اختر المادة الدراسية"
                                CssClass="text-danger"
                                Display="Dynamic"
                                ValidationGroup="vgSave" />
                        </div>

                        <div class="tt-field" style="grid-column: span 2;">
                            <label><i class="fas fa-user-tie"></i> المعلم <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlTeacher" runat="server" CssClass="form-select" />
                            <asp:RequiredFieldValidator ID="rfvTeacher" runat="server"
                                ControlToValidate="ddlTeacher"
                                InitialValue=""
                                ErrorMessage="اختر المعلم"
                                CssClass="text-danger"
                                Display="Dynamic"
                                ValidationGroup="vgSave" />
                        </div>

                    </div>
                </div>
            </div>

            <!-- ===== بطاقة: الحالة ===== -->
            <div class="tt-card">
                <div class="tt-card-header">
                    <div class="tt-card-header-icon orange">
                        <i class="fas fa-info-circle"></i>
                    </div>
                    <div class="tt-card-header-text">
                        <h3>حالة الحصة</h3>
                        <p>حدد حالة الحصة في الجدول</p>
                    </div>
                </div>
                <div class="tt-card-body">
                    <div class="tt-fields">
                        <div class="tt-field" style="grid-column: span 2;">
                            <label><i class="fas fa-toggle-on"></i> الحالة <span class="req">*</span></label>

                            <!-- قائمة مخصصة (Custom Dropdown) -->
                            <div class="custom-select" id="statusDropdown">
                                <div class="custom-select-trigger" onclick="toggleStatusDropdown(event)">
                                    <span id="statusDisplay">مسودة</span>
                                    <i class="fas fa-chevron-down"></i>
                                </div>
                                <div class="custom-select-options" id="statusOptions">
                                    <div class="custom-select-option selected" data-value="0"
                                         onclick="selectStatus('0', 'مسودة', this)">مسودة</div>
                                    <div class="custom-select-option" data-value="1"
                                         onclick="selectStatus('1', 'معتمد', this)">معتمد</div>
                                    <div class="custom-select-option" data-value="2"
                                         onclick="selectStatus('2', 'مؤرشف', this)">مؤرشف</div>
                                </div>
                                <asp:HiddenField ID="hfStatus" runat="server" Value="0" />
                            </div>

                            <small>
                                <i class="fas fa-info-circle"></i>
                                الحصص «المعتمدة» فقط تُستخدم في توليد الحصص اليومية وحساب الاستحقاق المالي.
                            </small>
                        </div>
                    </div>
                </div>
            </div>

            <!-- الأزرار -->
            <div class="tt-actions no-print">
                <asp:Button ID="btnCancel" runat="server" Text="إلغاء"
                    CssClass="tt-btn-outline"
                    OnClick="btnCancel_Click"
                    CausesValidation="false" />
                <asp:Button ID="btnSave" runat="server" Text="حفظ"
                    CssClass="tt-btn-primary"
                    ValidationGroup="vgSave"
                    OnClick="btnSave_Click" />
            </div>

        </div>
    </div>

    <script>
        function toggleStatusDropdown(e) {
            if (e) e.stopPropagation();
            document.getElementById('statusDropdown').classList.toggle('open');
        }

        function selectStatus(value, text, element) {
            document.getElementById('statusDisplay').textContent = text;
            document.getElementById('<%= hfStatus.ClientID %>').value = value;

            // إزالة التحديد من الكل
            document.querySelectorAll('#statusOptions .custom-select-option').forEach(function (el) {
                el.classList.remove('selected');
            });
            // تحديد العنصر المختار
            if (element) element.classList.add('selected');

            document.getElementById('statusDropdown').classList.remove('open');
        }

        // إغلاق عند الضغط خارج القائمة
        document.addEventListener('click', function (e) {
            var dd = document.getElementById('statusDropdown');
            if (dd && !dd.contains(e.target)) {
                dd.classList.remove('open');
            }
        });

        // إغلاق بزر Escape
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                var dd = document.getElementById('statusDropdown');
                if (dd) dd.classList.remove('open');
            }
        });
    </script>
</asp:Content>