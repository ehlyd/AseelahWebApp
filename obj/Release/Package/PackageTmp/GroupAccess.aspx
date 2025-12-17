<%@ Page Title="Group Access" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="GroupAccess.aspx.vb" Inherits="AseelahWebApps.GroupAccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <div class="container" style="border-style: solid; border-color: inherit; border-width: thin; background-color: whitesmoke;">
        <h6 style="margin-top: 20px; margin-bottom: 20px;">Group Access</h6>
        <hr />
    
        <div class="row" style="margin-bottom: 0px; margin-left:0px;  width: 100%;">
            <div class="col-6" style="padding: 0px; border: solid; border-width: thin;">
                <%--<asp:ListBox ID="lstUserGroup" runat="server" SelectionMode="Single" Height="100%" width="100%"></asp:ListBox>--%>

                <asp:GridView ID="GridViewUserGroup" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="100%" Font-Size="Small" AutoGenerateSelectButton="True">
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
                </asp:GridView>

            </div>
            <div class="col-6" style="padding: 0px; border: solid; border-width: thin;">
                <asp:GridView ID="GridViewModules" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="100%" Font-Size="Small">
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
        <div class="row" style="margin-top: 20px; margin-left:0px; width:100%;">
            <div class="col-6">
                
            </div>
            <div class="col-6" style="display:flex; justify-content:flex-end;">
                <asp:Button ID="btnSave" Class="btn btn-secondary btn-sm" runat="server" Text="Save" Style="margin-right: 10px; width: 80px;" OnClick="btnSave_Click" OnClientClick="isPostBack = true;"/>
                <asp:Button ID="btnCancel" Class="btn btn-secondary btn-sm" runat="server" Text="Cancel" Style="width: 80px;" OnClick="btnCancel_Click" OnClientClick="isPostBack = true;"/>
            </div>
        </div>
    </div>

    <script type="text/javascript"> 

        var isPostBack = false;

        function resetPostBackFlag() {
            isPostBack = false;
        }


        function pageCleanup() {
     
            if (isPostBack) {
                return;
            }

            var img = new Image();
            img.src = "CleanupHandler.ashx?action=LeaveGroupAccess&t=" + new Date().getTime();
        }

        window.addEventListener('beforeunload', pageCleanup, false);

        var originalDoPostBack = window.__doPostBack;

        window.__doPostBack = function (eventTarget, eventArgument) {
            isPostBack = true;

            originalDoPostBack(eventTarget, eventArgument);
        };
</script>

</asp:Content>
