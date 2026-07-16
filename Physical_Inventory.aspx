<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Physical_Inventory.aspx.vb" Inherits="AseelahWebApps.Physical_Inventory" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /*.pinv {
            height: 100vh !important;*/ /* Use vh for reliable viewport-based height */
        /*}*/



        /*@media (min-width: 576px) {
            .pinv {
                width: 90%;
            }
        }

        @media (min-width: 768px) {
            .pinv {
                width: 80%;
            }
        }

        @media (min-width: 992px) {
            .pinv {
                width: 70%;
            }
        }

        @media (min-width: 1200px) {
            .pinv {
                width: 70%;
            }
        }*/

        .pibuttons {
            padding: 0px;
        }

        .pnlbtn {
            width: 100%;
        }

        .alignRightHeader {
            text-align: right;
        }

        .alignCenter {
            text-align: center;
        }
        /*
        .modalBackground {
        background-color: Gray;
        filter: alpha(opacity=70);
        opacity: 0.7;*/
        }

        /* backdrop */
        /*.modal-backdrop-custom {
            position: fixed;
            inset: 0;*/ /* top:0;right:0;bottom:0;left:0 */
            /*background: rgba(0,0,0,0.5);
            z-index: 1040;
            display: none;
        }*/
        /* centering modal panel */
        /*.modal-panel {
            position: fixed;
            left: 50%;
            top: 50%;
            transform: translate(-50%, -50%);
            z-index: 1050;
            background: whitesmoke;
            box-shadow: 0 10px 40px rgba(0,0,0,0.45);
            border-radius: 6px;
            padding: 16px;
            max-height: 90vh;
            overflow: auto;
            display: none;
            width: 60%;
        }*/
            /* optional small variant */
       /*     .modal-panel.small {
                width: 40%;
            }*/

        /* disable page scroll while modal open */
        /*.body-modal-open {
            overflow: hidden;
        }*/
    </style>


    <asp:Panel ID="Panelinv" runat="server">
        <div class="container pinv" style="border-style: solid; border-color: inherit; border-width: thin; background-color: whitesmoke; width: 90%;">

            <h6 style="margin-top: 20px; margin-bottom: 20px;">Physical Inventory</h6>
            <hr />

            <div class="row" style="width: 100%; margin-top: 20px;">
                <div class="col">
                    <asp:Label ID="Label1" class="form-label-sm" runat="server" Text="Subsidiary" Font-Names="Segoe UI"></asp:Label>
                    <asp:DropDownList ID="ddlSubsidiary" class="form-select-sm" Style="width: 40%; max-width: 40%; margin-left: 10px;" runat="server" AutoPostBack="True" Font-Names="Segoe UI"></asp:DropDownList>
                </div>
            </div>

            <div class="row" style="width: 100%;">
                <div class="col">
                    <asp:Label ID="Label2" class="form-label-sm" runat="server" Text="Store" Font-Names="Segoe UI"></asp:Label>
                    <asp:DropDownList ID="ddlStore" class="form-select-sm" Style="width: 40%; max-width: 40%; margin-left: 45px;" runat="server" AutoPostBack="True" Font-Names="Segoe UI"></asp:DropDownList>
                </div>
            </div>

            <hr />

            <div class="row" style="width: 100%;">
                <div class="col" style="overflow-x: auto;">
                    <asp:GridView ID="gridViewPInv" runat="server" CellPadding="4" Width="100%" ForeColor="#333333" GridLines="None" Font-Size="Small" ShowFooter="False" AutoGenerateSelectButton="True" ShowHeaderWhenEmpty="True" HorizontalAlign="Center" RowStyle-HorizontalAlign="Center">
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <EditRowStyle BackColor="#999999" HorizontalAlign="NotSet" />
                        <EmptyDataRowStyle HorizontalAlign="Center" />
                        <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
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
                <ajaxToolkit:TabContainer ID="TabInv" runat="server">
                    <ajaxToolkit:TabPanel ID="TabPanel1" runat="server" HeaderText="PI Sheet">
                        <%-- PI SHEET--%>
                        <ContentTemplate>
                            <div class="gridcontainer" style="overflow-x: auto; overflow-y: auto;">
                                <asp:GridView ID="gridViewPISheet" runat="server" CellPadding="4" Width="100%" ForeColor="#333333" GridLines="None" Font-Size="Small" ShowHeaderWhenEmpty="True" HorizontalAlign="Center" AllowPaging="True" Height="60%">
                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    <EditRowStyle BackColor="#999999" />
                                    <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" />
                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Left" />
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                </asp:GridView>
                            </div>

                            <hr />
                            <div class="row" style="width: 100%;">
                                <div class="col">
                                    <asp:Button ID="btnCreatePI" Class="btn btn-secondary btn-sm" runat="server" Text="Create PI" />
                                    <asp:Button ID="btnDeletePI" Style="margin-left: 10px;" Class="btn btn-danger btn-sm" runat="server" Text="Delete PI" />
                                    <asp:Button ID="btnUpdatePI" Style="margin-left: 10px;" Class="btn btn-success btn-sm" runat="server" Text="Update PI" />

                                </div>
                            </div>

                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>

                    <ajaxToolkit:TabPanel ID="TabPanel5" runat="server" HeaderText="EBS Qty">
                        <%-- EBS QTY--%>
                        <ContentTemplate>
                            <div class="gridcontainer" style="overflow-x: auto; overflow-y: auto;">
                                <asp:GridView ID="gridViewEBSQty" runat="server" CellPadding="4" Width="100%" ForeColor="#333333" GridLines="None" Font-Size="Small" ShowHeaderWhenEmpty="True" HorizontalAlign="Center" AllowPaging="True" Height="60%" PageSize="15">
                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    <EditRowStyle BackColor="#999999" />
                                    <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" />
                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Left" />
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                </asp:GridView>
                            </div>
                            <hr />
                            <div style="margin-top: 10px;">
                                <asp:Button ID="btnImport" Class="btn btn-info btn-sm" runat="server" Text="Import" />
                            </div>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>

                    <ajaxToolkit:TabPanel ID="TabPanel4" runat="server" HeaderText="Imported">
                        <%-- IMPORTED --%>
                        <ContentTemplate>
                            <div class="gridcontainer" style="overflow-x: auto; overflow-y: auto;">
                                <asp:GridView ID="gridViewImported" runat="server" CellPadding="4" Width="100%" ForeColor="#333333" GridLines="None" Font-Size="Small" ShowHeaderWhenEmpty="True" HorizontalAlign="Center" AllowPaging="True" Height="60%" PageSize="10">
                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    <EditRowStyle BackColor="#999999" />
                                    <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" />
                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Left" />
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                </asp:GridView>
                            </div>
                            <hr />
                            <div style="margin-top: 10px;">
                                <asp:Button ID="btnMergeImported" Class="btn btn-info btn-sm" runat="server" Text="Merge" />
                            </div>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>

                    <ajaxToolkit:TabPanel ID="TabPanel2" runat="server" HeaderText="Add Counts">
                        <%-- ADD COUNTS--%>
                        <ContentTemplate>
                            <div class="gridcontainer" style="overflow-x: auto; overflow-y: auto;">
                                <asp:GridView ID="gridViewAddCounts" runat="server" CellPadding="4" Width="100%" ForeColor="#333333" GridLines="None" Font-Size="Small" ShowHeaderWhenEmpty="True" HorizontalAlign="Center" AllowPaging="True" Height="60%" ShowFooter="False">
                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    <EditRowStyle BackColor="#999999" />
                                    <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" />
                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Left" />
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                </asp:GridView>
                            </div>
                            <hr />
                            <div style="margin-top: 10px;">
                                <asp:Button ID="btnMergeAddCounts" Class="btn btn-info btn-sm" runat="server" Text="Merge" />
                            </div>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>

                    <ajaxToolkit:TabPanel ID="TabPanel3" runat="server" HeaderText="Bad Scans">
                        <%--BAD SCANS--%>
                        <ContentTemplate>
                            <div class="gridcontainer" style="overflow-x: auto; overflow-y: auto;">
                                <asp:GridView ID="gridViewBadScans" runat="server" CellPadding="4" Width="100%" ForeColor="#333333" GridLines="None" Font-Size="Small" ShowHeaderWhenEmpty="True"
                                    HorizontalAlign="Center" AllowPaging="True" Height="60%" ShowFooter="False" AutoGenerateEditButton="False" AutoGenerateSelectButton="True">
                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    <EditRowStyle BackColor="#999999" />
                                    <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" />
                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Left" />
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                </asp:GridView>
                            </div>
                            <hr />
                            <div style="margin-top: 10px;">
                                <asp:Button ID="btnMergeBadScan" Class="btn btn-info btn-sm" runat="server" Text="Merge" />
                            </div>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>

                </ajaxToolkit:TabContainer>

            </div>





            <%--      <hr />

        <div class="row" style="width: 100%;">
            <div class="col-4" style="display: flex; justify-content: left;">
                <asp:Label ID="Label2" class="form-label-sm" runat="server" Text="Total Records:" Font-Names="Segoe UI" Font-Size="Small" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblTotalRecords" class="form-label-sm" Style="margin-left: 10px;" runat="server" Text="1000" Font-Names="Segoe UI" Font-Size="Small" Font-Bold="True"></asp:Label>
            </div>
            <div class="col-8" style="display: flex; justify-content: right;">
                <asp:Label ID="Label3" class="form-label-sm" runat="server" Text="Total Start Qty:" Font-Names="Segoe UI" Font-Size="Small" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblTotalStartQty" class="form-label-sm" Style="margin-left: 10px;" runat="server" Text="1000" Font-Names="Segoe UI" Font-Size="Small" Font-Bold="True"></asp:Label>

                <asp:Label ID="Label4" class="form-label-sm" Style="margin-left: 10px;" runat="server" Text="Total Scan Qty:" Font-Names="Segoe UI" Font-Size="Small" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblTotalScanQty" class="form-label-sm" Style="margin-left: 10px;" runat="server" Text="1000" Font-Names="Segoe UI" Font-Size="Small" Font-Bold="True"></asp:Label>

                <asp:Label ID="Label5" class="form-label-sm" Style="margin-left: 10px;" runat="server" Text="Total Discr. Qty:" Font-Names="Segoe UI" Font-Size="Small" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblTotalDiscrQty" class="form-label-sm" Style="margin-left: 10px;" runat="server" Text="1000" Font-Names="Segoe UI" Font-Size="Small" Font-Bold="True"></asp:Label>
            </div>
        </div>--%>
        </div>
    </asp:Panel>

    <asp:Panel ID="panelEditBarcode" runat="server" Visible="false">
        <div class="container" id="editbarcode" style="position: absolute; top: 10%; left: 25%; width: 60%; height: 80vh; background-color: whitesmoke; border: ridge; box-shadow: -10px 10px 7px gray;">
            <h6 style="margin-top: 20px; margin-bottom: 20px;">Barcode Correction</h6>
            <hr />

            <div class="row" style="width: 100%;">
                <div class="col-4">
                    <asp:Label ID="Label3" Class="form-label-sm" runat="server" Text="Wrong Barcode:" Style="color: red"></asp:Label>
                    <asp:Label ID="lblBarcode" Class="form-label-sm" Style="margin-left: 10px; color: red; border: thin;" runat="server" Text=""></asp:Label>
                </div>
                <div class="col-4">
                    <asp:Label ID="Label4" Class="form-label-sm" runat="server" Text="Correct Barcode:" Style="color: green;"></asp:Label>
                    <asp:Label ID="lblCorrectBarcode" Class="form-label-sm" Style="margin-left: 10px; color: green; border: thin;" runat="server"></asp:Label>
                </div>
                <div class="col-4">
                    <asp:Button ID="btnUpdate" Class="btn btn-secondary btn-sm" runat="server" Text="Update" Width="100px" />
                    <asp:Button ID="btnCancel" Class="btn btn-secondary btn-sm" runat="server" Text="Cancel" Width="100px" Style="margin-left: 10px;" />
                </div>

            </div>
            <hr />

            <div class="row" style="width: 100%;">
                <div class="col-4">
                    <asp:Label ID="Label6" Class="form-label-sm" runat="server" Text="Filter by:"></asp:Label>
                    <asp:DropDownList ID="ddlFilterColumn" class="form-select-sm" Style="margin-left: 10px; width: 70%; max-width: 70%;" runat="server"></asp:DropDownList>
                </div>
                <div class="col-4">
                    <asp:Label ID="Label5" Class="form-label-sm" runat="server" Text="Filter text:"></asp:Label>
                    <asp:TextBox ID="txtFilter" Class="form-control-sm" Style="margin-left: 10px; width: 70%; max-width: 70%;" runat="server"></asp:TextBox>
                </div>
                <div class="col-4">
                    <asp:Button ID="btnFilter" Class="btn btn-secondary btn-sm" runat="server" Text="Filter" Width="100px" />
                    <asp:Button ID="btnClear" Class="btn btn-secondary btn-sm" runat="server" Text="Clear" Width="100px" Style="margin-left: 10px;" />
                </div>
            </div>
            <hr />

            <%--<div class="row" style="width: 100%; height: calc(100% - 100px);">--%>
            <div class="row" style="width: 100%;">

                <div class="col" style="overflow-x: auto; overflow-y: auto; height: 100%;">
                    <asp:GridView ID="gridViewBarcodeList" runat="server" CellPadding="4" Width="100%" ForeColor="#333333" GridLines="None" Font-Size="Small" ShowHeaderWhenEmpty="True" HorizontalAlign="Center" AllowPaging="True" PageSize="15" AutoGenerateSelectButton="True">
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Left" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                    </asp:GridView>
                </div>
            </div>
        </div>

    </asp:Panel>
   
    <asp:Panel ID="panelCreatePI" runat="server" Visible="false" >
        <div class="container" id="createPI" style="height:30vh; width: 40%; background-color: whitesmoke; position: absolute; top: 20%; left: 25%; border: ridge; box-shadow: -10px 10px 7px gray;">
            <h6 style="margin-top: 20px; margin-bottom: 20px;">Create PI</h6>
            <hr />
            <div class="row" style="width: 100%;">
                <div class="col-2">
                    <asp:Label ID="Label7" Class="form-label-sm" runat="server" Text="PI Name:"></asp:Label>

                </div>
                <div class="col-10">
                    <asp:TextBox ID="txtPIName" Class="form-control-sm" Style="width: 100%; max-width: 100%;" runat="server"></asp:TextBox>
                </div>
            </div>
            <div class="row" style="width: 100%;">
                <div class="col-2">
                    <asp:Label ID="Label8" Class="form-label-sm" runat="server" Text="PI Note:"></asp:Label>

                </div>
                <div class="col-10">
                    <asp:TextBox ID="txtPINote" Class="form-control-sm" Style="width: 100%; max-width: 100%;" runat="server"></asp:TextBox>
                </div>
            </div>
            <hr />
            <div class="row" style="width: 100%;">
                <div class="col-2">
                </div>
                <div class="col-10" style="display: flex; justify-content: right;">
                    <asp:Button ID="btnCreate" Class="btn btn-secondary btn-sm" runat="server" Text="Create" Width="100px" />
                    <asp:Button ID="btnCancelCreate" Class="btn btn-secondary btn-sm" runat="server" Text="Cancel" Width="100px" Style="margin-left: 10px;" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <%--<div id="modalBackdrop" class="modal-backdrop-custom" runat="server"></div>--%>

    <%--    <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="btnUpdatePI" PopupControlID="panelEditBarcode" CancelControlID="btnClose" BackgroundCssClass="modalBackground">

    </ajaxToolkit:ModalPopupExtender>--%>

    <%--    <script>
        function showEditBarcode() {
            var panel = document.getElementById('<%= Panelinv.ClientID %>');
            if (panel) {
                panel.style.pointerEvents = 'none';
                panel.style.opacity = '0.5';
            }

            var overlay = document.getElementById('editbarcode');
            if (overlay) {
                overlay.style.display = 'block';
            }
        }
    </script>--%>

   <%-- <script type="text/javascript">
        function _showModal(clientId) {
            var panel = document.getElementById(clientId);
            var backdrop = document.getElementById('<%= modalBackdrop.ClientID %>');
            if (!panel || !backdrop) return;
            backdrop.style.display = 'block';
            panel.style.display = 'block';
            // prevent page scroll
            document.documentElement.classList.add('body-modal-open');
            // optional: focus first input
            var inp = panel.querySelector('input, button, textarea, select');
            if (inp) try { inp.focus(); } catch (e) { }
        }

        function _hideModal(clientId) {
            var panel = document.getElementById(clientId);
            var backdrop = document.getElementById('<%= modalBackdrop.ClientID %>');
            if (panel) panel.style.display = 'none';
            if (backdrop) backdrop.style.display = 'none';
            document.documentElement.classList.remove('body-modal-open');
        }

        // convenience wrappers for your two panels:
        function showCreatePI() { _showModal('<%= panelCreatePI.ClientID %>'); }
        function hideCreatePI() { _hideModal('<%= panelCreatePI.ClientID %>'); }
        function showEditBarcode() { _showModal('<%= panelEditBarcode.ClientID %>'); }
        function hideEditBarcode() { _hideModal('<%= panelEditBarcode.ClientID %>'); }

        // close when clicking backdrop
        (function () {
            var b = document.getElementById('<%= modalBackdrop.ClientID %>');
            if (b) b.addEventListener('click', function () {
                hideCreatePI(); hideEditBarcode();
            });
        })();
</script>--%>

</asp:Content>
