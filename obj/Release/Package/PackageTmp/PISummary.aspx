<%@ Page Title="PI Summary" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PISummary.aspx.vb" Inherits="AseelahWebApps.PISummary" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .gridLineDownload {
            color: blue !important;
            text-decoration: underline;
        }

        .gridLineCountForm {
            color: green !important;
            text-decoration: underline;
        }

        @media (min-width: 576px) {
            .subsidiary {
                width: 80%;
                max-width: 80%;
            }

            .store {
                width: 80%;
                max-width: 80%;
            }
        }

        @media (min-width: 768px) {
            .subsidiary {
                width: 70%;
                max-width: 70%;
            }

            .store {
                width: 70%;
                max-width: 70%;
            }
        }

        @media (min-width: 992px) {
            .subsidiary {
                width: 60%;
                max-width: 60%;
            }

            .store {
                width: 60%;
                max-width: 60%;
            }
        }

        @media (min-width: 1200px) {
            .subsidiary {
                width: 50%;
                max-width: 50%;
            }

            .store {
                width: 50%;
                max-width: 50%;
            }
        }
    </style>

    <div id="loadingOverlay" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(255,255,255,0.7); z-index: 9999; text-align: center; padding-top: 20%;">
        <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
            <span class="visually-hidden">Loading...</span>
        </div>
        <div style="margin-top: 10px; font-weight: bold; color: #007bff;">Downloading PI detail, please wait...</div>
    </div>

    <br />

    <asp:Panel ID="Panel1" runat="server" CssClass="download-panel">
        <div class="container" style="border-style: solid; border-color: inherit; border-width: thin; background-color: whitesmoke;">
            <h6 style="margin-top: 20px; margin-bottom: 20px;">PI Summary</h6>
            <hr />

            <div class="row" style="width: 100%; margin-top: 20px;">
                <div class="col">
                    <asp:Label ID="Label1" class="form-label-sm" runat="server" Text="Subsidiary"></asp:Label>
                    <asp:DropDownList ID="ddlSubsidiary" class="form-select-sm subsidiary" Style="margin-left: 10px;" runat="server" AutoPostBack="True"></asp:DropDownList>
                </div>
            </div>

            <div class="row" style="width: 100%;">
                <div class="col">
                    <asp:Label ID="Label2" class="form-label-sm" runat="server" Text="Store"></asp:Label>
                    <asp:DropDownList ID="ddlStore" class="form-select-sm store" Style="margin-left: 45px;" runat="server" AutoPostBack="True"></asp:DropDownList>
                </div>
            </div>

            <hr />

            <div class="row" style="width: 100%;">
                <div class="col" style="overflow-x: auto">
                    <asp:GridView ID="gridViewPI" runat="server" CellPadding="4" Width="100%" ForeColor="#333333" GridLines="None" Font-Size="Small" ShowFooter="False" AutoGenerateSelectButton="True" ShowHeaderWhenEmpty="True" HorizontalAlign="Center" AutoGenerateEditButton="True">
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

    </asp:Panel>

    <script type="text/javascript">
        var downloadTimer;

        function downloadClick() {
            //console.log("Download started... Showing Spinner.");

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
                    //console.log("Cookie found! Hiding Spinner.");

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
    </script>
</asp:Content>
