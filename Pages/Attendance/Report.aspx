<%@ Page Title="تقرير الحضور" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Attendance.Report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid" dir="rtl">
        <div class="card-custom">
            <div class="card-header-custom d-flex align-items-center gap-2">
                <h6 class="mb-0">
                    <i class="fas fa-clipboard-check me-2"></i>تقرير الحضور والغياب
                </h6>

                <%-- ✅ زر تصدير ورد --%>
                <asp:Button ID="btnExportWord" runat="server" Text="تصدير ورد"
                    CssClass="btn btn-primary-green btn-sm ms-auto"
                    OnClick="btnExportWord_Click" />
            </div>
            <div class="card-body-custom">
                <div class="row mb-3">
                    <div class="col-md-3">
                        <label class="form-label fw-bold">من تاريخ</label>
                        <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-control" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-bold">إلى تاريخ</label>
                        <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-control" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-bold">الصف</label>
                        <asp:DropDownList ID="ddlClassRoom" runat="server" CssClass="form-select" AppendDataBoundItems="true">
                            <asp:ListItem Value="" Text="الكل" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3 d-flex align-items-end">
                        <asp:Button ID="btnFilter" runat="server" Text="عرض التقرير" CssClass="btn btn-primary-green" OnClick="btnFilter_Click" />
                    </div>
                </div>

                <asp:GridView ID="gvReport" runat="server" CssClass="table-custom"
                    AutoGenerateColumns="false" EmptyDataText="لا توجد بيانات" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="StudentName" HeaderText="اسم الطالب" />
                        <asp:BoundField DataField="ClassName" HeaderText="الصف" />
                        <asp:BoundField DataField="TotalPresent" HeaderText="أيام الحضور" />
                        <asp:BoundField DataField="TotalAbsent" HeaderText="أيام الغياب" />
                        <asp:BoundField DataField="Percentage" HeaderText="نسبة الحضور" DataFormatString="{0:F1}%" />
                    </Columns>
                </asp:GridView>

                <asp:Panel ID="divSummary" runat="server" Visible="false" CssClass="card-custom mt-4">
                    <div class="card-body-custom">
                        <h5 style="color: #00b14f;">ملخص التقرير</h5>
                        <div class="row">
                            <div class="col-md-3">
                                <strong>إجمالي الطلاب:</strong>
                                <asp:Label ID="lblTotalStudents" runat="server" CssClass="ms-2" />
                            </div>
                            <div class="col-md-3">
                                <strong>متوسط الحضور:</strong>
                                <asp:Label ID="lblAvgAttendance" runat="server" CssClass="ms-2" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function setDefaultDates() {
            var start = document.getElementById('<%= txtStartDate.ClientID %>');
            var end = document.getElementById('<%= txtEndDate.ClientID %>');
            if (start && !start.value) {
                var d = new Date();
                d.setDate(1);
                start.value = d.toISOString().split('T')[0];
            }
            if (end && !end.value) {
                end.value = new Date().toISOString().split('T')[0];
            }
        }
        window.onload = setDefaultDates;
    </script>
</asp:Content>