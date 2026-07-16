<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.vb" Inherits="AseelahWebApps._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main aria-labelledby="Home">

        <div class="container" style="border: none; margin-top: 20px;">

            <div class="row PISummary" style="width: 100%;">

                <div class="col-2 colMnu" id="imgBtnPICol" runat="server">
                    <asp:ImageButton ID="imgBtnPI" runat="server" Style="height: 80px; width: 80px;" ImageUrl="~/images/inventory.png" OnClick="imgBtnPI_Click" OnClientClick="showWaitCursor()" ToolTip="PI Summary" />
                </div>
                
                <div class="col-2 colMnu" id="imgBtnPhysicalInvCol" runat="server">
                    <asp:ImageButton ID="imgBtnPhysicalInv" runat="server" Style="height: 80px; width: 80px;" ImageUrl="~/images/physical_inventory.png" OnClick="imgBtnPhysicalInv_Click" OnClientClick="showWaitCursor()" ToolTip="Physical Inventory" />
                </div>
                <div class="col-2 colMnu" id="imgBtnExportItemsCol" runat="server">
                    <asp:ImageButton ID="imgBtnExportItems" runat="server" Style="height: 80px; width: 80px;" ImageUrl="~/images/export textfile.png" OnClick="imBtnExportItems_Click" OnClientClick="showWaitCursor()" ToolTip="Export Items" />
                </div>
                <div class="col-2 colMnu" id="imgBtnOnlineSalesCol" runat="server">
                    <asp:ImageButton ID="imgBtnOnlineSales" runat="server" Style="height: 80px; width: 80px;" ImageUrl="~/images/shopping-list.png" OnClientClick="showWaitCursor()" ToolTip="Online Sales Comparison" />
                </div>
            </div>

            <div class="row PISummary" style="width: 100%;">
                <div class="col-2 colMnu" id="HyperLinkPISummaryCol" runat="server">
                    <asp:HyperLink ID="HyperLinkPISummary" runat="server" NavigateUrl="~/PISummary.aspx" OnClientClick="showWaitCursor()" Font-Size="Small">PI Summary</asp:HyperLink>
                </div>
                
                <div class="col-2 colMnu" id="HyperLinkPysicalInvCol" runat="server">
                    <asp:HyperLink ID="HyperLinkPysicalInv" runat="server" NavigateUrl="~/Physical_Inventory.aspx" OnClientClick="showWaitCursor()" Font-Size="Small">Physical Inventory</asp:HyperLink>
                </div>
                <div class="col-2 colMnu" id="HyperLinkExportItemCol" runat="server">
                    <asp:HyperLink ID="HyperLinkExportItem" runat="server" NavigateUrl="~/ExportItemMaster.aspx" OnClientClick="showWaitCursor()" Font-Size="Small">Export Item Master</asp:HyperLink>
                </div>

                <div class="col-2 colMnu" id="HyperLinkOnlineSalesCol" runat="server">
                    <asp:HyperLink ID="HyperLinkOnlineSales" runat="server" NavigateUrl="~/OnlineSalesComparison.aspx" OnClientClick="showWaitCursor()" Font-Size="Small">Online Sales Comparison</asp:HyperLink>
                </div>
            </div>

        </div>

    </main>

</asp:Content>
