<%@ Page Title="User Management" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="UserList.aspx.vb" Inherits="AseelahWebApps.UserList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        @media (min-width: 576px) {
            .userList {
                width: 80%;
            }
        }

        @media (min-width: 768px) {
            .userList {
                width: 60%;
            }
        }

        @media (min-width: 992px) {
            .userList {
                width: 50%;
            }
        }

        @media (min-width: 1200px) {
            .userList {
                width: 50%;
            }
        }
    </style>

    <div class="container userList" style="border-style: solid; border-color: inherit; border-width: thin; background-color: whitesmoke; margin-left: 10%; padding-left: 40px; padding-right: 40px;">
        <h6 style="margin-top: 20px; margin-bottom: 20px;">User Management</h6>
        <hr />
        <div class="row" style="width: 100%;">
            <div class="col" style="overflow-x: auto">
                <asp:GridView ID="gvUsers" runat="server" AutoGenerateEditButton="True" CellPadding="4" ForeColor="#333333" GridLines="None" Font-Size="Small" Width="100%" HorizontalAlign="Center">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <EditRowStyle BackColor="#999999" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
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
            <div class="col" style="display: flex; justify-content: end;">
                <asp:Button ID="btnAdd" class="btn btn-secondary btn-sm" runat="server" Width="25%" Text="Add" />
            </div>
        </div>

    </div>
</asp:Content>
