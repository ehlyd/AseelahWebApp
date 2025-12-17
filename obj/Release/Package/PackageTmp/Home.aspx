<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.vb" Inherits="AseelahWebApps._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main aria-labelledby="Home">

        <div class="container" style="border: none; margin-top: 20px;">

            <div class="row PISummary">

                <div class="col">
                    <asp:ImageButton ID="imgBtnPI" runat="server" Style="height: 80px; width: 80px;" ImageUrl="~/images/inventory.png" OnClick="imgBtnPI_Click" OnClientClick="showWaitCursor()" />
                    <asp:ImageButton ID="imgBtnExportItems" runat="server" Style="margin-left: 10%; height: 80px; width: 80px;" ImageUrl="~/images/export textfile.png" OnClick="imBtnExportItems_Click" OnClientClick="showWaitCursor()" />
                    <%--<asp:ImageButton ID="imBtnGroupAccess" runat="server" Style="margin-left: 30px; height: 80px; width: 80px;" ImageUrl="~/images/access-control.png" OnClick="imgBtnPI_Click" OnClientClick="showWaitCursor()" />--%>
                </div>

            </div>

            <div class="row PISummary">
                <div class="col">
                    <asp:HyperLink ID="HyperLinkPISummary" runat="server" NavigateUrl="~/PISummary.aspx" OnClientClick="showWaitCursor()" Font-Size="Small">PI Summary</asp:HyperLink>
                    <asp:HyperLink ID="HyperLinkExportItem" Style="margin-left: 10%;" runat="server" NavigateUrl="~/ExportItemMaster.aspx" OnClientClick="showWaitCursor()" Font-Size="Small">Export Item Master</asp:HyperLink>
                    <%--<asp:HyperLink ID="HyperLinkGroupAccess" Style="margin-left: 40px;" runat="server" NavigateUrl="~/GroupAccess.aspx" OnClientClick="showWaitCursor()" Font-Size="Small">Group Access</asp:HyperLink>--%>
                </div>

            </div>

        </div>

    </main>

</asp:Content>
