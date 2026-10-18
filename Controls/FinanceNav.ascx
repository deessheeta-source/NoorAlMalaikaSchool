<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FinanceNav.ascx.cs" Inherits="NoorAlMalaikaSchool.Controls.FinanceNav" %>

<style>
    .fin-nav {
        display: flex; flex-wrap: wrap; align-items: center; gap: 6px;
        padding: 10px 14px; margin-bottom: 16px;
        background: #fff; border: 1px solid #e9ecef; border-radius: 12px;
        box-shadow: 0 1px 2px rgba(0,0,0,0.02);
    }
    .fin-nav-title {
        font-size: 12.5px; font-weight: 700; color: #00b14f;
        margin-inline-end: 8px; display: inline-flex; align-items: center; gap: 6px;
    }
    .fin-nav-title i { font-size: 13px; }

    .fin-chip {
        display: inline-flex; align-items: center; gap: 6px;
        padding: 5px 13px; font-size: 12.5px; font-weight: 600;
        color: #495057; background: #f9fafb;
        border: 1px solid #dee2e6; border-radius: 20px;
        text-decoration: none; transition: all .15s ease; white-space: nowrap;
    }
    .fin-chip:hover {
        background: rgba(0,177,79,0.08); color: #00b14f;
        border-color: #00b14f; text-decoration: none;
    }
    .fin-chip.active {
        background: #00b14f; color: #fff; border-color: #00b14f;
    }
    .fin-chip.active:hover { color: #fff; }

    .quick-actions { display: flex; flex-wrap: wrap; gap: 8px; margin-bottom: 16px; }
    .quick-action-btn {
        display: inline-flex; align-items: center; gap: 8px;
        padding: 8px 16px; font-size: 13px; font-weight: 600;
        border-radius: 10px; text-decoration: none;
        transition: all .15s ease; border: 1.5px solid transparent;
    }
    .quick-action-btn i { font-size: 13px; }
    .quick-action-btn.primary { background: #00b14f; color: #fff; border-color: #00b14f; }
    .quick-action-btn.primary:hover { background: #009a44; color: #fff; text-decoration: none; }
    .quick-action-btn.outline { background: #fff; color: #495057; border-color: #dee2e6; }
    .quick-action-btn.outline:hover { color: #00b14f; border-color: #00b14f; background: rgba(0,177,79,0.04); text-decoration: none; }
    .quick-action-btn.blue { background: #fff; color: #0d6efd; border-color: #0d6efd; }
    .quick-action-btn.blue:hover { background: rgba(13,110,253,0.06); color: #0d6efd; text-decoration: none; }

    @media print { .fin-nav, .quick-actions { display: none !important; } }
</style>

<div class="fin-nav">
    <span class="fin-nav-title">
        <i class="fas fa-sitemap"></i> الإدارة المالية للظلاب :
    </span>
    <asp:Literal ID="litNav" runat="server" />
</div>