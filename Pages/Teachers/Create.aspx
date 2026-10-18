<%@ Page Title="إضافة معلم/معلمة" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Teachers.Create" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />

    <div class="container-fluid" dir="rtl">

        <div class="academia-card">

            <div class="card-header border-bottom d-flex align-items-center py-2">
                <h6 class="mb-0 text-academia">
                    <i class="fas fa-user-plus me-2"></i>
                    إضافة معلم/معلمة
                </h6>
            </div>

            <div class="card-body">

                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    CssClass="validation-msg d-none" />

                <!-- ========================= -->
                <!-- البيانات الشخصية -->
                <!-- ========================= -->

                <div class="border-bottom pb-2 mb-3 fw-bold text-academia">
                    <i class="fas fa-id-card me-1"></i>
                    البيانات الشخصية
                </div>


                <!-- الاسم -->
                <div class="row mb-3">

                    <div class="col-md-3">
                        <label class="form-label fw-bold text-academia">
                            الاسم الأول
                            <span class="text-danger">*</span>
                        </label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="الاسم الأول" />
                        <asp:RequiredFieldValidator ID="rfvFirstName" runat="server"
                            ControlToValidate="txtFirstName"
                            ErrorMessage="الاسم الأول مطلوب."
                            CssClass="text-danger validation-msg"
                            Display="Dynamic"
                            ValidationGroup="vgSave" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label fw-bold text-academia">
                            اسم الأب
                            <span class="text-danger">*</span>
                        </label>
                        <asp:TextBox ID="txtFatherName" runat="server" CssClass="form-control" placeholder="اسم الأب" />
                        <asp:RequiredFieldValidator ID="rfvFatherName" runat="server"
                            ControlToValidate="txtFatherName"
                            ErrorMessage="اسم الأب مطلوب."
                            CssClass="text-danger validation-msg"
                            Display="Dynamic"
                            ValidationGroup="vgSave" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label fw-bold text-academia">
                            اسم العائلة
                            <span class="text-danger">*</span>
                        </label>
                        <asp:TextBox ID="txtFamilyName" runat="server" CssClass="form-control" placeholder="اسم العائلة" />
                        <asp:RequiredFieldValidator ID="rfvFamilyName" runat="server"
                            ControlToValidate="txtFamilyName"
                            ErrorMessage="اسم العائلة مطلوب."
                            CssClass="text-danger validation-msg"
                            Display="Dynamic"
                            ValidationGroup="vgSave" />
                    </div>

                </div>


                <!-- الرقم الوطني + الجنس + الميلاد -->
                <div class="row mb-3">

                    <div class="col-md-4">
                        <label class="form-label fw-bold text-academia">
                            الرقم الوطني
                        </label>
                        <asp:TextBox ID="txtNationalId" runat="server" CssClass="form-control" placeholder="الرقم الوطني" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label fw-bold text-academia">
                            الجنس
                        </label>
                        <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-select">
                            <asp:ListItem Text="معلم" Value="Male" />
                            <asp:ListItem Text="معلمة" Value="Female" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label fw-bold text-academia">
                            تاريخ الميلاد
                        </label>
                        <asp:TextBox ID="txtBirthDate" runat="server" TextMode="Date" CssClass="form-control" />
                    </div>

                </div>


                <!-- الهاتف + التخصص -->
                <div class="row mb-3">

                    <div class="col-md-6">
                        <label class="form-label fw-bold text-academia">
                            الهاتف
                        </label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="رقم الهاتف" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label fw-bold text-academia">
                            التخصص (المادة الدراسية)
                        </label>
                        <asp:DropDownList ID="ddlSpecialty" runat="server" CssClass="form-select" AppendDataBoundItems="true">
                            <asp:ListItem Text="-- اختر المادة --" Value="" />
                        </asp:DropDownList>
                    </div>

                </div>

                <!-- السنة الدراسية -->
                <div class="row mb-3">
                    <div class="col-md-6">
                        <label class="form-label fw-bold text-academia">
                            السنة الدراسية <span class="text-danger">*</span>
                        </label>
                        <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-select">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvAcademicYear" runat="server"
                            ControlToValidate="ddlAcademicYear"
                            InitialValue=""
                            ErrorMessage="السنة الدراسية مطلوبة."
                            CssClass="text-danger validation-msg"
                            Display="Dynamic"
                            ValidationGroup="vgSave" />
                    </div>
                </div>


                <!-- العنوان + الحالة -->
                <div class="row mb-4">

                    <div class="col-md-8">
                        <label class="form-label fw-bold text-academia">
                            العنوان
                        </label>
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="العنوان" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label fw-bold text-academia">
                            الحالة
                        </label>
                        <asp:DropDownList ID="ddlIsActive" runat="server" CssClass="form-select">
                            <asp:ListItem Text="نشط" Value="True" Selected="True" />
                            <asp:ListItem Text="غير نشط" Value="False" />
                        </asp:DropDownList>
                    </div>

                </div>


                <!-- ========================= -->
                <!-- البيانات المالية -->
                <!-- ========================= -->

                <div class="border-bottom pb-2 mb-3 fw-bold text-academia">
                    <i class="fas fa-money-bill-wave me-1"></i>
                    البيانات المالية
                </div>

                <div class="alert alert-info d-flex align-items-start gap-2 mb-3" style="font-size:12.5px;border-radius:10px;">
                    <i class="fas fa-info-circle" style="font-size:16px;margin-top:2px;"></i>
                    <div>
                        <strong>مهم:</strong> قيمة الحصة تُستخدم لحساب الاستحقاق المالي اليومي في صفحة
                        <strong>«مراجعة واعتماد اليوم»</strong>. كل حصة مدتها <strong>45 دقيقة</strong>،
                        ويُحسب الاستحقاق بناءً على حالة الحضور:
                        <ul class="mb-0 mt-1" style="padding-inline-start:20px;">
                            <li>حضور كامل → <strong>قيمة الحصة كاملة</strong></li>
                            <li>حضور جزئي / غياب بعذر → <strong>تُحسب بالدقائق الفعلية</strong></li>
                            <li>غياب بدون عذر → <strong>صفر</strong></li>
                        </ul>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-4">
                        <label class="form-label fw-bold text-academia">نوع التعاقد <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlEmploymentType" runat="server" CssClass="form-select">
                            <asp:ListItem Text="معلم دائم - راتب شهري" Value="Permanent" />
                            <asp:ListItem Text="معلم بالحصة" Value="PerLesson" />
                            <asp:ListItem Text="مشرف دائم - راتب شهري" Value="PermanentSupervisor" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label fw-bold text-academia">الراتب الشهري (د.ل)</label>
                        <asp:TextBox ID="txtBaseSalary" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" placeholder="0.00" />
                    </div>
                </div>

                <div class="row mb-4">

                    <div class="col-md-4">
                        <label class="form-label fw-bold text-academia">قيمة الحصة (د.ل)</label>
                        <asp:TextBox ID="txtLessonRate" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            step="0.01"
                            min="0"
                            placeholder="0.00" />
                        <asp:RangeValidator ID="rvLessonRate" runat="server"
                            ControlToValidate="txtLessonRate"
                            MinimumValue="0.01"
                            MaximumValue="100000"
                            Type="Double"
                            ErrorMessage="قيمة الحصة يجب أن تكون أكبر من صفر."
                            CssClass="text-danger validation-msg"
                            Display="Dynamic"
                            ValidationGroup="vgSave" />
                        <small class="text-muted" style="font-size:11px;">سعر الحصة لمعلم الحصة، ويمكن أن يختلف حسب المادة.</small>
                    </div>
                </div>


                <!-- ========================= -->
                <!-- الأزرار -->
                <!-- ========================= -->

                <div class="d-flex gap-2">
                    <asp:Button ID="btnSave" runat="server" Text="حفظ" CssClass="btn btn-academia"
                        ValidationGroup="vgSave" OnClick="btnSave_Click" />

                    <asp:Button ID="btnCancel" runat="server" Text="إلغاء" CssClass="btn btn-outline-secondary"
                        CausesValidation="false" OnClick="btnCancel_Click" />
                </div>

            </div>

        </div>

    </div>

</asp:Content>