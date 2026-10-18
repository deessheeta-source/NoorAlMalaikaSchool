<%@ Page Title="سجل التدقيق" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AuditLog.aspx.cs" Inherits="NoorAlMalaikaSchool.Pages.Security.AuditLog" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveClientUrl("~/Content/css/fees-academia.css") %>" rel="stylesheet" />
    <link href="<%= ResolveClientUrl("~/Content/css/audit-log.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid audit-page" dir="rtl">
        <section class="audit-hero" aria-labelledby="auditPageTitle">
            <div class="audit-hero-icon" aria-hidden="true"><i class="fas fa-shield-alt"></i></div>
            <div class="audit-hero-copy">
                <span class="audit-eyebrow">الإدارة والمتابعة</span>
                <h1 id="auditPageTitle">سجل التدقيق</h1>
                <p>راجع التغييرات ومحاولات الدخول مع وقت العملية والمستخدم والبيانات المرتبطة بها.</p>
            </div>
            <span class="audit-admin-badge"><i class="fas fa-lock" aria-hidden="true"></i> خاص بمدير النظام</span>
        </section>

        <section class="audit-summary" aria-label="ملخص السجلات المطابقة">
            <article class="audit-stat-card">
                <span class="audit-stat-icon audit-stat-blue"><i class="fas fa-list-alt" aria-hidden="true"></i></span>
                <div><span class="audit-stat-label">السجلات المطابقة</span><strong><asp:Literal ID="litResultCount" runat="server" Text="0" /></strong></div>
            </article>
            <article class="audit-stat-card">
                <span class="audit-stat-icon audit-stat-red"><i class="fas fa-trash-alt" aria-hidden="true"></i></span>
                <div><span class="audit-stat-label">عمليات الحذف</span><strong><asp:Literal ID="litDeleteCount" runat="server" Text="0" /></strong></div>
            </article>
            <article class="audit-stat-card">
                <span class="audit-stat-icon audit-stat-amber"><i class="fas fa-user-lock" aria-hidden="true"></i></span>
                <div><span class="audit-stat-label">محاولات الدخول الفاشلة</span><strong><asp:Literal ID="litFailedLoginCount" runat="server" Text="0" /></strong></div>
            </article>
            <article class="audit-stat-card audit-stat-latest">
                <span class="audit-stat-icon audit-stat-green"><i class="fas fa-clock" aria-hidden="true"></i></span>
                <div><span class="audit-stat-label">أحدث نشاط ضمن النتائج</span><strong class="audit-latest-value"><asp:Literal ID="litLatestActivity" runat="server" Text="—" /></strong></div>
            </article>
        </section>

        <section class="audit-panel audit-filter-panel" aria-labelledby="auditFilterTitle">
            <div class="audit-panel-heading">
                <div>
                    <h2 id="auditFilterTitle"><i class="fas fa-sliders-h" aria-hidden="true"></i> تصفية السجلات</h2>
                    <p>حدد الفترة أو المستخدم أو نوع العملية لتصل إلى السجل المطلوب.</p>
                </div>
                <span class="audit-filter-hint"><i class="fas fa-info-circle" aria-hidden="true"></i> القائمة تعرض أحدث 1000 نتيجة، والتحديد الشامل يضم كل النتائج المطابقة</span>
            </div>

            <div class="audit-filter-grid">
                <div class="audit-filter-field">
                    <asp:Label ID="lblFrom" runat="server" AssociatedControlID="txtFrom" CssClass="form-label" Text="من تاريخ" />
                    <asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="audit-filter-field">
                    <asp:Label ID="lblTo" runat="server" AssociatedControlID="txtTo" CssClass="form-label" Text="إلى تاريخ" />
                    <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="audit-filter-field">
                    <asp:Label ID="lblUser" runat="server" AssociatedControlID="ddlUser" CssClass="form-label" Text="المستخدم" />
                    <asp:DropDownList ID="ddlUser" runat="server" CssClass="form-select" />
                </div>
                <div class="audit-filter-field">
                    <asp:Label ID="lblAction" runat="server" AssociatedControlID="ddlAction" CssClass="form-label" Text="نوع العملية" />
                    <asp:DropDownList ID="ddlAction" runat="server" CssClass="form-select">
                        <asp:ListItem Value="" Text="كل العمليات" />
                        <asp:ListItem Value="إنشاء" Text="إنشاء" />
                        <asp:ListItem Value="تعديل" Text="تعديل" />
                        <asp:ListItem Value="حذف" Text="حذف" />
                        <asp:ListItem Value="تسجيل دخول" Text="تسجيل دخول ناجح" />
                        <asp:ListItem Value="محاولة دخول فاشلة" Text="محاولة دخول فاشلة" />
                        <asp:ListItem Value="تغيير كلمة المرور" Text="تغيير كلمة المرور" />
                    </asp:DropDownList>
                </div>
                <div class="audit-filter-field">
                    <asp:Label ID="lblEntity" runat="server" AssociatedControlID="txtEntity" CssClass="form-label" Text="نوع السجل" />
                    <asp:TextBox ID="txtEntity" runat="server" CssClass="form-control" placeholder="مثال: Student أو Invoice" />
                </div>
                <div class="audit-filter-field audit-search-field">
                    <asp:Label ID="lblSearch" runat="server" AssociatedControlID="txtSearch" CssClass="form-label" Text="بحث سريع" />
                    <div class="audit-search-wrap">
                        <i class="fas fa-search" aria-hidden="true"></i>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="المستخدم، المعرف، التفاصيل أو عنوان IP" MaxLength="100" />
                    </div>
                </div>
                <div class="audit-filter-actions">
                    <asp:Button ID="btnFilter" runat="server" Text="تطبيق التصفية" CssClass="btn audit-primary-btn" OnClick="btnFilter_Click" CausesValidation="false" />
                    <asp:Button ID="btnReset" runat="server" Text="إعادة الضبط" CssClass="btn audit-reset-btn" OnClick="btnReset_Click" CausesValidation="false" />
                </div>
            </div>

            <asp:Panel ID="pnlFilterMessage" runat="server" CssClass="audit-filter-message" Visible="false" role="alert">
                <i class="fas fa-exclamation-circle" aria-hidden="true"></i>
                <asp:Literal ID="litFilterMessage" runat="server" Mode="Encode" />
            </asp:Panel>
        </section>

        <section class="audit-panel audit-results-panel" aria-labelledby="auditResultsTitle">
            <div class="audit-panel-heading audit-results-heading">
                <div>
                    <h2 id="auditResultsTitle"><i class="fas fa-stream" aria-hidden="true"></i> سجل العمليات</h2>
                    <p>اختر سجلات الصفحة أو جميع نتائج التصفية لحفظ نسخة منها أو حذفها.</p>
                </div>
                <span class="audit-results-count"><asp:Literal ID="litResultCountLabel" runat="server" Text="0 سجل" /></span>
            </div>

            <div class="audit-selection-toolbar" role="group" aria-label="حفظ أو حذف السجلات المحددة">
                <div class="audit-selection-guidance">
                    <strong><i class="fas fa-archive" aria-hidden="true"></i> حفظ نسخة قبل الحذف</strong>
                    <span>تُنزّل النسخة بصيغة JSON. يمكنك اختيار سجلات الصفحة أو جميع النتائج المطابقة للفلاتر.</span>
                </div>
                <div class="audit-selection-controls">
                    <label class="audit-select-all-control">
                        <input type="checkbox" class="audit-select-all-page" />
                        <span>تحديد الكل في الصفحة</span>
                    </label>
                    <asp:Panel ID="pnlSelectAllResults" runat="server" CssClass="audit-select-all-results-wrap" Visible="false">
                        <label class="audit-select-all-results-label">
                            <input type="checkbox" name="auditSelectAll" value="1" class="audit-select-all-results" />
                            <span>تحديد جميع النتائج المطابقة (<asp:Literal ID="litFilteredSelectionCount" runat="server" Text="0" />)</span>
                        </label>
                    </asp:Panel>
                    <asp:HiddenField ID="hfFilteredCount" runat="server" Value="0" />
                    <span class="audit-selection-count">المحدد: <strong id="auditSelectedCount">0</strong></span>
                    <asp:Button ID="btnBackupSelected" runat="server" Text="حفظ نسخة JSON" CssClass="btn audit-backup-btn"
                        OnClick="btnBackupSelected_Click" OnClientClick="return auditBackupSelection();" CausesValidation="false" />
                    <asp:Button ID="btnDeleteSelected" runat="server" Text="حذف المحدد" CssClass="btn audit-delete-btn"
                        OnClick="btnDeleteSelected_Click" OnClientClick="return auditConfirmDelete();" CausesValidation="false" />
                </div>
            </div>

            <asp:Panel ID="pnlActionMessage" runat="server" CssClass="audit-action-message" Visible="false" role="alert">
                <i class="fas fa-info-circle" aria-hidden="true"></i>
                <asp:Literal ID="litActionMessage" runat="server" Mode="Encode" />
            </asp:Panel>

            <div class="audit-desktop-table table-responsive">
                <asp:GridView ID="gvLog" runat="server" CssClass="table-custom audit-grid"
                    AutoGenerateColumns="false" EmptyDataText="لا توجد سجلات مطابقة للفلاتر المحددة."
                    GridLines="None" AllowPaging="true" PageSize="25" UseAccessibleHeader="true"
                    OnPageIndexChanging="gvLog_PageIndexChanging">
                    <PagerSettings Mode="NumericFirstLast" FirstPageText="الأولى" LastPageText="الأخيرة" />
                    <Columns>
                        <asp:TemplateField HeaderText="تحديد">
                            <ItemTemplate><input type="checkbox" name="auditSelect" value='<%# Eval("Id") %>' class="audit-select-checkbox" aria-label="تحديد السجل" /></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="التاريخ والوقت">
                            <ItemTemplate><span class="audit-date-value"><i class="far fa-calendar-alt" aria-hidden="true"></i><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("TimestampText") %>' /></span></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="المستخدم">
                            <ItemTemplate><span class="audit-user-value"><i class="fas fa-user-circle" aria-hidden="true"></i><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("UserName") %>' /></span></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="العملية">
                            <ItemTemplate><span class='audit-action-badge <%# GetActionClass(Eval("Action")) %>'><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("Action") %>' /></span></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="السجل المرتبط">
                            <ItemTemplate>
                                <div class="audit-entity-value"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("EntityType") %>' /></div>
                                <small class="audit-entity-id">المعرف: <asp:Literal runat="server" Mode="Encode" Text='<%# Eval("EntityId") %>' /></small>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ملخص التغيير">
                            <ItemTemplate>
                                <div class="audit-description"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("Details") %>' /></div>
                                <asp:PlaceHolder runat="server" Visible='<%# HasChangedFields(Eval("ChangedFields")) %>'>
                                    <div class="audit-changed-fields"><span>الحقول:</span> <asp:Literal runat="server" Mode="Encode" Text='<%# Eval("ChangedFields") %>' /></div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" Visible='<%# HasAuditValues(Eval("OldValues"), Eval("NewValues")) %>'>
                                    <details class="audit-value-details">
                                        <summary>عرض القيم السابقة والجديدة</summary>
                                        <div class="audit-value-grid">
                                            <div><span>القيم السابقة</span><pre dir="ltr"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("OldValues") %>' /></pre></div>
                                            <div><span>القيم الجديدة</span><pre dir="ltr"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("NewValues") %>' /></pre></div>
                                        </div>
                                    </details>
                                </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="عنوان IP">
                            <ItemTemplate><code class="audit-ip" dir="ltr"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("IpAddress") %>' /></code></ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="audit-pager" />
                </asp:GridView>
            </div>

            <div class="audit-mobile-list">
                <asp:Repeater ID="rptLogMobile" runat="server">
                    <ItemTemplate>
                        <article class="audit-mobile-card">
                            <div class="audit-mobile-card-head">
                                <div class="audit-mobile-card-selection">
                                    <label class="audit-select-label"><input type="checkbox" name="auditSelect" value='<%# Eval("Id") %>' class="audit-select-checkbox" aria-label="تحديد السجل" /><span>تحديد</span></label>
                                    <div class="audit-mobile-date"><i class="far fa-calendar-alt" aria-hidden="true"></i><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("TimestampText") %>' /></div>
                                </div>
                                <span class='audit-action-badge <%# GetActionClass(Eval("Action")) %>'><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("Action") %>' /></span>
                            </div>
                            <div class="audit-mobile-user"><i class="fas fa-user-circle" aria-hidden="true"></i><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("UserName") %>' /></div>
                            <div class="audit-mobile-meta">
                                <div><span>السجل</span><strong><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("EntityType") %>' /></strong></div>
                                <div><span>المعرف</span><strong><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("EntityId") %>' /></strong></div>
                                <div><span>عنوان IP</span><code dir="ltr"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("IpAddress") %>' /></code></div>
                            </div>
                            <div class="audit-mobile-description"><span>ملخص العملية</span><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("Details") %>' /></div>
                            <asp:PlaceHolder runat="server" Visible='<%# HasChangedFields(Eval("ChangedFields")) %>'>
                                <div class="audit-mobile-fields"><span>الحقول المتغيرة</span><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("ChangedFields") %>' /></div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" Visible='<%# HasAuditValues(Eval("OldValues"), Eval("NewValues")) %>'>
                                <details class="audit-value-details">
                                    <summary>عرض القيم السابقة والجديدة</summary>
                                    <div class="audit-value-grid">
                                        <div><span>القيم السابقة</span><pre dir="ltr"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("OldValues") %>' /></pre></div>
                                        <div><span>القيم الجديدة</span><pre dir="ltr"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("NewValues") %>' /></pre></div>
                                    </div>
                                </details>
                            </asp:PlaceHolder>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlMobileEmpty" runat="server" CssClass="audit-empty-state" Visible="false">
                    <i class="fas fa-inbox" aria-hidden="true"></i>
                    <strong>لا توجد سجلات مطابقة</strong>
                    <span>غيّر الفلاتر أو الفترة الزمنية ثم أعد البحث.</span>
                </asp:Panel>

                <asp:Panel ID="pnlMobilePager" runat="server" CssClass="audit-mobile-pager" Visible="false">
                    <asp:Button ID="btnMobilePrevious" runat="server" Text="السابق" CssClass="btn audit-page-btn" OnClick="btnMobilePrevious_Click" CausesValidation="false" />
                    <asp:Literal ID="litMobilePageInfo" runat="server" />
                    <asp:Button ID="btnMobileNext" runat="server" Text="التالي" CssClass="btn audit-page-btn" OnClick="btnMobileNext_Click" CausesValidation="false" />
                </asp:Panel>
            </div>
        </section>
    </div>

    <script type="text/javascript">
        (function () {
            function allResultsCheckbox() {
                return document.querySelector('.audit-select-all-results');
            }

            function getSelectedAuditIds() {
                var checkboxes = document.querySelectorAll('.audit-select-checkbox:checked');
                var ids = [];
                for (var i = 0; i < checkboxes.length; i++) {
                    if (ids.indexOf(checkboxes[i].value) === -1) ids.push(checkboxes[i].value);
                }
                return ids;
            }

            function updateAuditSelection() {
                var checkboxes = document.querySelectorAll('.audit-select-checkbox');
                var selected = getSelectedAuditIds();
                var count = document.getElementById('auditSelectedCount');
                var allResults = allResultsCheckbox();
                var allCount = document.getElementById('<%= hfFilteredCount.ClientID %>');
                var selectedCount = allResults && allResults.checked && allCount
                    ? parseInt(allCount.value, 10) || 0
                    : selected.length;
                if (count) count.textContent = selectedCount;

                var allSelected = checkboxes.length > 0 && selected.length >= checkboxes.length / 2;
                var selectAllPage = document.querySelector('.audit-select-all-page');
                if (selectAllPage) selectAllPage.checked = !allResults || !allResults.checked ? allSelected : false;
            }

            document.addEventListener('change', function (event) {
                if (event.target.classList.contains('audit-select-checkbox')) {
                    var allResults = allResultsCheckbox();
                    if (allResults) allResults.checked = false;
                    var checkboxes = document.querySelectorAll('.audit-select-checkbox');
                    for (var i = 0; i < checkboxes.length; i++) {
                        if (checkboxes[i].value === event.target.value) checkboxes[i].checked = event.target.checked;
                    }
                    updateAuditSelection();
                }
                if (event.target.classList.contains('audit-select-all-page')) {
                    var allResultsCheckboxElement = allResultsCheckbox();
                    if (allResultsCheckboxElement) allResultsCheckboxElement.checked = false;
                    var allCheckboxes = document.querySelectorAll('.audit-select-checkbox');
                    for (var j = 0; j < allCheckboxes.length; j++) allCheckboxes[j].checked = event.target.checked;
                    updateAuditSelection();
                }
                if (event.target.classList.contains('audit-select-all-results')) {
                    var currentCheckboxes = document.querySelectorAll('.audit-select-checkbox');
                    for (var k = 0; k < currentCheckboxes.length; k++) currentCheckboxes[k].checked = false;
                    var pageSelector = document.querySelector('.audit-select-all-page');
                    if (pageSelector) pageSelector.checked = false;
                    updateAuditSelection();
                }
            });

            updateAuditSelection();
        })();

        function auditBackupSelection() {
            var allResults = document.querySelector('.audit-select-all-results');
            if ((!allResults || !allResults.checked) && document.querySelectorAll('.audit-select-checkbox:checked').length === 0) {
                window.alert('حدد السجلات التي تريد حفظ نسخة منها أولاً.');
                return false;
            }
            return true;
        }

        function auditConfirmDelete() {
            var allResults = document.querySelector('.audit-select-all-results');
            var ids = [];
            var checkboxes = document.querySelectorAll('.audit-select-checkbox:checked');
            for (var i = 0; i < checkboxes.length; i++) {
                if (ids.indexOf(checkboxes[i].value) === -1) ids.push(checkboxes[i].value);
            }
            var count = allResults && allResults.checked
                ? parseInt(document.getElementById('<%= hfFilteredCount.ClientID %>').value, 10) || 0
                : ids.length;
            if (count === 0) {
                window.alert('حدد السجلات التي تريد حذفها أولاً.');
                return false;
            }
            return window.confirm('سيتم حذف ' + count + ' سجلًا نهائيًا. يُنصح بحفظ نسخة JSON قبل المتابعة. هل تريد الاستمرار؟');
        }
    </script>
</asp:Content>
