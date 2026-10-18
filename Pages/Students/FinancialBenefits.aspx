<%@ Page Title="استحقاقات الطالب المالية" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FinancialBenefits.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Students.FinancialBenefits" %>
<%@ Register Src="~/Controls/FinanceNav.ascx" TagPrefix="uc" TagName="FinanceNav" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />

    <div class="container-fluid" dir="rtl">

        <uc:FinanceNav runat="server" ActivePage="FinancialBenefits" />

        <!-- =========================================================
             بيانات الطالب
             ========================================================= -->

        <div class="academia-card mb-3">

            <div class="card-header border-bottom d-flex align-items-center py-2">
                <h6 class="mb-0 text-academia">
                    <i class="fas fa-hand-holding-usd me-2"></i>
                    استحقاقات الطالب المالية
                </h6>
            </div>

            <div class="card-body">

                <div class="row g-3">

                    <!-- الصف -->
                    <div class="col-md-4">

                        <label class="form-label fw-bold text-academia">
                            الصف
                            <span class="text-danger">*</span>
                        </label>

                        <asp:DropDownList
                            ID="ddlClassRoom"
                            runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlClassRoom_SelectedIndexChanged">

                            <asp:ListItem
                                Value=""
                                Text="-- اختر الصف --" />

                        </asp:DropDownList>

                    </div>

                    <!-- الطالب -->
                    <div class="col-md-4">

                        <label class="form-label fw-bold text-academia">
                            الطالب
                            <span class="text-danger">*</span>
                        </label>

                        <asp:DropDownList
                            ID="ddlStudent"
                            runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlStudent_SelectedIndexChanged">

                            <asp:ListItem
                                Value=""
                                Text="-- اختر الطالب --" />

                        </asp:DropDownList>

                    </div>

                    <!-- العام الدراسي -->
                    <div class="col-md-4">

                        <label class="form-label fw-bold text-academia">
                            العام الدراسي
                            <span class="text-danger">*</span>
                        </label>

                        <asp:DropDownList
                            ID="ddlAcademicYear"
                            runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged" />

                    </div>

                </div>

                <div class="row mt-3">

                    <div class="col-12">

                        <asp:Label
                            ID="lblEligibility"
                            runat="server"
                            CssClass="alert alert-info py-1 px-3 w-100 mb-0 d-block" Font-Size="13px" />

                    </div>

                </div>

            </div>

        </div>


        <!-- =========================================================
             الخصم والإعفاء
             ========================================================= -->

        <div class="row g-3">

            <!-- =====================================================
                 إضافة خصم
                 ===================================================== -->

            <div class="col-lg-6">

                <div class="academia-card h-100">

                    <div class="card-header border-bottom d-flex align-items-center py-2">
                        <h6 class="mb-0 text-academia">
                            إضافة خصم مستحق
                        </h6>
                    </div>

                    <div class="card-body">

                        <!-- سياسة الخصم -->
                        <div class="mb-2">

                            <label class="form-label fw-bold text-academia">
                                سياسة الخصم
                            </label>

                            <asp:DropDownList
                                ID="ddlDiscountPolicy"
                                runat="server"
                                CssClass="form-select" />

                        </div>


                        <div class="row g-2">

                            <!-- القيمة -->
                            <div class="col-4">

                                <label class="form-label fw-bold text-academia">
                                    القيمة
                                </label>

                                <asp:TextBox
                                    ID="txtDiscountValue"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="Number"
                                    step="0.01" />

                            </div>


                            <!-- نوع القيمة -->
                            <div class="col-4">

                                <label class="form-label fw-bold text-academia">
                                    نوع القيمة
                                </label>

                                <asp:DropDownList
                                    ID="ddlDiscountKind"
                                    runat="server"
                                    CssClass="form-select">

                                    <asp:ListItem
                                        Value="Percentage"
                                        Text="%" />

                                    <asp:ListItem
                                        Value="Amount"
                                        Text="مبلغ" />

                                </asp:DropDownList>

                            </div>


                            <!-- نطاق الرسم -->
                            <div class="col-4">

                                <label class="form-label fw-bold text-academia">
                                    ينطبق على
                                </label>

                                <asp:DropDownList
                                    ID="ddlDiscountFeeType"
                                    runat="server"
                                    CssClass="form-select" />

                            </div>

                        </div>


                        <div class="row g-2 mt-1">

                            <!-- رقم القرار -->
                            <div class="col-6">

                                <label class="form-label fw-bold text-academia">
                                    رقم القرار
                                </label>

                                <asp:TextBox
                                    ID="txtDiscountDecisionNo"
                                    runat="server"
                                    CssClass="form-control" />

                            </div>


                            <!-- السبب -->
                            <div class="col-6">

                                <label class="form-label fw-bold text-academia">
                                    السبب
                                </label>

                                <asp:TextBox
                                    ID="txtDiscountReason"
                                    runat="server"
                                    CssClass="form-control" />

                            </div>

                        </div>


                        <asp:Button
                            ID="btnAddDiscount"
                            runat="server"
                            Text="إضافة الخصم"
                            CssClass="btn btn-academia mt-3"
                            OnClick="btnAddDiscount_Click" />

                    </div>

                </div>

            </div>


            <!-- =====================================================
                 إضافة إعفاء
                 ===================================================== -->

            <div class="col-lg-6">

                <div class="academia-card h-100">

                    <div class="card-header border-bottom d-flex align-items-center py-2">
                        <h6 class="mb-0 text-academia">
                            إضافة إعفاء
                        </h6>
                    </div>

                    <div class="card-body">

                        <!-- نطاق الرسم -->
                        <div class="mb-2">

                            <label class="form-label fw-bold text-academia">
                                ينطبق على
                            </label>

                            <asp:DropDownList
                                ID="ddlExemptionFeeType"
                                runat="server"
                                CssClass="form-select" />

                        </div>


                        <div class="row g-2">

                            <!-- القيمة -->
                            <div class="col-4">

                                <label class="form-label fw-bold text-academia">
                                    القيمة
                                </label>

                                <asp:TextBox
                                    ID="txtExemptionValue"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="Number"
                                    step="0.01" />

                            </div>


                            <!-- النوع -->
                            <div class="col-4">

                                <label class="form-label fw-bold text-academia">
                                    نوع القيمة
                                </label>

                                <asp:DropDownList
                                    ID="ddlExemptionKind"
                                    runat="server"
                                    CssClass="form-select">

                                    <asp:ListItem
                                        Value="Percentage"
                                        Text="%" />

                                    <asp:ListItem
                                        Value="Amount"
                                        Text="مبلغ" />

                                </asp:DropDownList>

                            </div>


                            <!-- القرار -->
                            <div class="col-4">

                                <label class="form-label fw-bold text-academia">
                                    رقم القرار
                                </label>

                                <asp:TextBox
                                    ID="txtExemptionDecisionNo"
                                    runat="server"
                                    CssClass="form-control" />

                            </div>

                        </div>


                        <!-- السبب -->
                        <div class="mb-2 mt-2">

                            <label class="form-label fw-bold text-academia">
                                السبب
                            </label>

                            <asp:TextBox
                                ID="txtExemptionReason"
                                runat="server"
                                CssClass="form-control" />

                        </div>


                        <asp:Button
                            ID="btnAddExemption"
                            runat="server"
                            Text="إضافة الإعفاء"
                            CssClass="btn btn-academia mt-2"
                            OnClick="btnAddExemption_Click" />

                    </div>

                </div>

            </div>

        </div>


        <!-- =========================================================
             الرسائل
             ========================================================= -->

        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="validation-msg d-block mt-3" />


        <!-- =========================================================
             القوائم الحالية
             ========================================================= -->

        <div class="row g-3 mt-1">

            <!-- الخصومات -->
            <div class="col-lg-6">

                <div class="academia-card">

                    <div class="card-header border-bottom d-flex align-items-center py-2">
                        <h6 class="mb-0 text-academia">
                            الخصومات الفعالة
                        </h6>
                    </div>

                    <div class="card-body">

                        <asp:GridView
                            ID="gvDiscounts"
                            runat="server"
                            CssClass="table-custom"
                            AutoGenerateColumns="false"
                            OnRowCommand="gvDiscounts_RowCommand"
                            EmptyDataText="لا توجد خصومات فعالة."
                            GridLines="None">

                            <Columns>

                                <asp:BoundField
                                    DataField="Name"
                                    HeaderText="السياسة" />

                                <asp:BoundField
                                    DataField="ValueText"
                                    HeaderText="القيمة" />

                                <asp:BoundField
                                    DataField="FeeTypeName"
                                    HeaderText="النطاق" />

                                <asp:BoundField
                                    DataField="Reason"
                                    HeaderText="السبب" />

                                <asp:ButtonField
                                    Text="إلغاء"
                                    CommandName="Deactivate"
                                    ButtonType="Button"
                                    ControlStyle-CssClass="btn btn-sm btn-danger" />

                            </Columns>

                        </asp:GridView>

                    </div>

                </div>

            </div>


            <!-- الإعفاءات -->
            <div class="col-lg-6">

                <div class="academia-card">

                    <div class="card-header border-bottom d-flex align-items-center py-2">
                        <h6 class="mb-0 text-academia">
                            الإعفاءات الفعالة
                        </h6>
                    </div>

                    <div class="card-body">

                        <asp:GridView
                            ID="gvExemptions"
                            runat="server"
                            CssClass="table-custom"
                            AutoGenerateColumns="false"
                            OnRowCommand="gvExemptions_RowCommand"
                            EmptyDataText="لا توجد إعفاءات فعالة."
                            GridLines="None">

                            <Columns>

                                <asp:BoundField
                                    DataField="ValueText"
                                    HeaderText="القيمة" />

                                <asp:BoundField
                                    DataField="FeeTypeName"
                                    HeaderText="النطاق" />

                                <asp:BoundField
                                    DataField="Reason"
                                    HeaderText="السبب" />

                                <asp:ButtonField
                                    Text="إلغاء"
                                    CommandName="Deactivate"
                                    ButtonType="Button"
                                    ControlStyle-CssClass="btn btn-sm btn-danger" />

                            </Columns>

                        </asp:GridView>

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>