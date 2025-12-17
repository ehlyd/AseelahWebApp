<%@ Page Title="PI Summary" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PISummary.aspx.vb" Inherits="AseelahWebApps.PISummary" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />

    <div class="container" style="border-style: solid; border-color: inherit; border-width: thin; background-color: whitesmoke;">
        <h6 style="margin-top: 20px; margin-bottom: 20px;">PI Summary</h6>
        <hr />
        
        <div class="row" style="width: 100%; margin-top: 20px;">
            <div class="col">
                <asp:Label ID="Label1" class="form-label-sm" runat="server" Text="Subsidiary"></asp:Label>
                <asp:DropDownList ID="ddlSubsidiary" class="form-select-sm" Style="width: 70%; max-width: 70%; margin-left: 10px;" runat="server" AutoPostBack="True"></asp:DropDownList>
            </div>
        </div>

        <div class="row" style="width: 100%;">
            <div class="col">
                <asp:Label ID="Label2" class="form-label-sm" runat="server" Text="Store"></asp:Label>
                <asp:DropDownList ID="ddlStore" class="form-select-sm" Style="width: 70%; max-width: 70%; margin-left: 45px;" runat="server" AutoPostBack="True"></asp:DropDownList>
            </div>
        </div>

        <hr />

        <div class="row" style="width: 100%;">
            <div class="col" style="overflow-x: auto">
                <asp:GridView ID="gridViewPI" runat="server" CellPadding="4" Width="100%" ForeColor="#333333" GridLines="None" Font-Size="Small" ShowFooter="False" AutoGenerateSelectButton="True" ShowHeaderWhenEmpty="True" HorizontalAlign="Center">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <EditRowStyle BackColor="#999999" />
                    <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                </asp:GridView>
            </div>
        </div>

        <hr />

        <div class="row" style="width: 100%;">
            <div class="col">
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="700px" ShowFindControls="False" ShowBackButton="False" ShowPageNavigationControls="False" ShowRefreshButton="False"></rsweb:ReportViewer>
            </div>
        </div>

    </div>

</asp:Content>
