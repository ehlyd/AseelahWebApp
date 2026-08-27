<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="OnlineSalesComparison.aspx.vb" Inherits="AseelahWebApps.OnlineSalesComparison" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        @media (min-width: 576px) {
            .OnlineSalesComp {
                width: 90%;
            }
        }

        @media (min-width: 768px) {
            .OnlineSalesComp {
                width: 80%;
            }
        }

        @media (min-width: 992px) {
            .OnlineSalesComp {
                width: 80%;
            }
        }

        @media (min-width: 1200px) {
            .OnlineSalesComp {
                width: 70%;
            }
        }
    </style>

    <div id="loadingOverlay" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(255,255,255,0.7); z-index: 9999; text-align: center; padding-top: 20%;">
        <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
            <span class="visually-hidden">Loading...</span>
        </div>
        <div style="margin-top: 10px; font-weight: bold; color: #007bff;">Downloading, please wait...</div>
    </div>

    <br />
    <asp:Panel ID="Panel1" runat="server" CssClass="download-panel">

        <div class="container OnlineSalesComp" style="border-style: solid; border-color: inherit; border-width: thin; background-color: whitesmoke;">
            <h6 style="margin-top: 20px; margin-bottom: 20px;">Online Sales Comparison Report</h6>
            <hr />

            <div class="row" style="width: 100%; margin-top: 20px;">
                <div class="col-2">
                    <asp:Label ID="Label1" class="form-label-sm" runat="server" Text="Subsidiary"></asp:Label>
                </div>
                <div class="col-10">
                    <asp:DropDownList ID="ddlSubsidiary" class="form-select-sm subsidiary" Style="width: 100%; max-width: 100%;" runat="server" AutoPostBack="True"></asp:DropDownList>
                </div>
            </div>

            <div class="row" style="width: 100%;">
                <div class="col-2">
                    <asp:Label ID="Label2" class="form-label-sm" runat="server" Text="Store"></asp:Label>
                </div>
                <div class="col-10">
                    <asp:DropDownList ID="ddlStore" class="form-select-sm store" Style="width: 100%; max-width: 100%;" runat="server" AutoPostBack="True"></asp:DropDownList>
                </div>
            </div>

            <div class="row" style="width: 100%;">
                <div class="col-2">
                    <asp:Label ID="Label3" class="form-label-sm" runat="server" Text="From"></asp:Label>
                </div>
                <div class="col-4">
                    <asp:TextBox ID="txtFromDate" class="form-control-sm fromdate" Style="width: 100%; max-width: 100%;" runat="server" TextMode="Date"></asp:TextBox>
                </div>
                <div class="col-1" style="text-align: center;">
                    <asp:Label ID="Label4" class="form-label-sm" runat="server" Text="To"></asp:Label>
                </div>
                <div class="col-5">
                    <asp:TextBox ID="txtToDate" class="form-control-sm todate" Style="width: 100%; max-width: 100%;" runat="server" TextMode="Date"></asp:TextBox>
                </div>
            </div>

            <div class="row" style="margin-top: 20px; width: 100%;">
                <div class="col" style="display: flex; align-items: end; justify-content: end;">
                    <asp:Button ID="btnDownload" class="btn btn-secondary btn-sm" Style="width: 120px;" runat="server" Text="Download" OnClick="btnDownload_Click" OnClientClick="downloadClick();" />
                </div>
            </div>
        </div>

        <%--   <hr id="hr1" runat="server" />--%>
        <div class="container OnlineSalesComp" id="ipkaddnote" runat="server" style="border-style: solid; border-color: inherit; border-width: thin; background-color: whitesmoke;">
            <div class="row" style="margin-top: 20px; width: 100%;">
                <div class="col">
                    <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch">
                        <asp:Label ID="Label5" class="form-label-sm" runat="server" Text="Order Name"></asp:Label>
                        <asp:TextBox ID="txtSearch" class="form-control-sm" Style="margin-left: 17px; width: 343px;" runat="server"></asp:TextBox>

                        <asp:Button ID="btnSearch" runat="server" class="btn btn-secondary btn-sm" Style="margin-left: 10px; width: 120px;" Text="Search" OnClick="btnSearch_Click" OnClientClick="WaitCursor();" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear" class="btn btn-secondary btn-sm" Style="margin-left: 10px; width: 120px;" />
                    </asp:Panel>
                </div>
            </div>
            <div class="row" style="margin-top: 5px; width: 100%;">
                <div class="col">
                    <asp:Label ID="Label6" runat="server" class="form-label-sm" Text="Filter by SKU"></asp:Label>
                    <asp:TextBox ID="txtFilter" class="form-control-sm" Style="margin-left: 15px;" runat="server" Width="343px"></asp:TextBox>
                    <asp:Button ID="btnFilter" runat="server" Text="Filter" class="btn btn-secondary btn-sm" Style="margin-left: 10px; width: 120px;" />
                </div>
            </div>
            <div class="row" style="margin-top: 5px; width: 100%;">
                <div class="col">
                    <asp:Label ID="Label7" runat="server" Text="Enter note"></asp:Label>
                    <asp:TextBox ID="txtNote" class="form-control-sm" Style="margin-left: 31px;" runat="server" Width="368px"></asp:TextBox>                   
                    <asp:Button ID="btnUpdateNote" runat="server" Text="Update" class="btn btn-secondary btn-sm" Style="margin-left: 10px; width: 120px;" OnClientClick="WaitCursor();" />
                    <asp:CheckBox ID="chkNote" class="form-control-sm" runat="server" Style="margin-left: 10px;" Text="Note" />
                     <asp:CheckBox ID="chkRetailPro" class="form-control-sm" runat="server" Text="RP Fixed" />
                </div>
            </div>

            <div class="row" style="margin-top: 10px; width: 100%;">
                <div class="col" style="overflow-x: auto;">

                    <asp:GridView ID="gvOrderDetail" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="100%" Font-Size="Small">
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#CCFFCC" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />

                        <Columns>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectAll_CheckedChanged" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelect_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                </div>
            </div>

        </div>

    </asp:Panel>



    <script type="text/javascript">
        var downloadTimer;

        function downloadClick() {
            console.log("Download started... Showing Spinner.");

            // 1. Show the Loading Overlay
            var overlay = document.getElementById('loadingOverlay');
            if (overlay) { overlay.style.display = 'block'; }

            // 2. Lock the Panel (Visual backup)
            var panel = document.getElementById('<%= Panel1.ClientID %>');
            if (panel) {
                panel.style.pointerEvents = "none";
                panel.style.opacity = "0.4";
            }

            // 3. Start the Watcher
            if (window.downloadTimer) clearInterval(window.downloadTimer);

            window.downloadTimer = setInterval(function () {
                if (document.cookie.indexOf("downloadStarted=true") !== -1) {
                    console.log("Cookie found! Hiding Spinner.");

                    // STOP TIMER
                    clearInterval(window.downloadTimer);

                    // HIDE OVERLAY
                    if (overlay) { overlay.style.display = 'none'; }

                    // RESET PANEL
                    var pnl = document.getElementById('<%= Panel1.ClientID %>');
                    if (pnl) {
                        pnl.style.pointerEvents = "auto";
                        pnl.style.opacity = "1.0";
                    }

                    // RESET CURSOR
                    document.body.style.cursor = 'default';

                    // DELETE COOKIE
                    document.cookie = "downloadStarted=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;";
                }
            }, 1000);
        }

        function WaitCursor() {
            document.documentElement.style.cursor = 'wait';
        }
    </script>

</asp:Content>
