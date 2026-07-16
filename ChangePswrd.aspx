<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ChangePswrd.aspx.vb" Inherits="AseelahWebApps.ChangePswrd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        @media (min-width: 576px) {

            .pswrdChange {
                width: 60%;
            }
        }

        @media (min-width: 768px) {

            .pswrdChange {
                width: 50%;
            }
        }

        @media (min-width: 992px) {

            .pswrdChange {
                width: 40%;
            }
        }

        @media (min-width: 1200px) {

            .pswrdChange {
                width: 40%;
            }
        }

        .row {
            width: 100%;
        }
    </style>

    <div class="container pswrdChange" style="border-style: solid; border-color: inherit; border-width: thin; background-color: whitesmoke; margin-left: 10%; padding-left: 40px; padding-right: 40px;">
        <h6 style="margin-top: 20px; margin-bottom: 20px;">Change Password</h6>
        <hr />
        <div class="row">
            <div class="col-4">
                <asp:Label ID="Label5" runat="server" Text="User ID" Class="col-form-label-sm"></asp:Label>
            </div>
            <div class="col-8">
                <asp:TextBox ID="txtChangeUserID" runat="server" Class="form-control-sm" Style="width: 100%; max-width: 100%;" Font-Names="Segoe UI" Font-Size="Small"></asp:TextBox>
            </div>
        </div>
        <div class="row" style="margin-top: 10px;">
            <div class="col-4">
                <asp:Label ID="Label6" runat="server" Text="Old Password" Class="col-form-label-sm"></asp:Label>
            </div>
            <div class="col-8">
                <asp:TextBox ID="txtChangeOldPswrd" runat="server" TextMode="Password" Class="form-control-sm" Style="width: 100%; max-width: 100%;" Font-Names="Segoe UI" Font-Size="Small"></asp:TextBox>
            </div>
        </div>
        <div class="row" style="margin-top: 10px;">
            <div class="col-4">
                <asp:Label ID="Label7" runat="server" Text="New Password" Class="col-form-label-sm"></asp:Label>
            </div>
            <div class="col-8">
                <asp:TextBox ID="txtChangeNewPswrd" runat="server" TextMode="Password" Class="form-control-sm" Style="width: 100%; max-width: 100%;" Font-Names="Segoe UI" Font-Size="Small"></asp:TextBox>
            </div>
        </div>
        <div class="row" style="margin-top: 10px;">
            <div class="col-4">
                <asp:Label ID="Label8" runat="server" Text="Confirm Password" Class="col-form-label-sm"></asp:Label>
            </div>
            <div class="col-8">
                <asp:TextBox ID="txtChangeConfirmPswrd" runat="server" TextMode="Password" Class="form-control-sm" Style="width: 100%; max-width: 100%;" Font-Names="Segoe UI" Font-Size="Small"></asp:TextBox>
            </div>
        </div>
        <hr />
        <div class="row">
            <div class="col-4">
            </div>
            <div class="col-8" style="display: flex; justify-content: end;">
                <asp:Button ID="btnChangePswrdSubmit" Class="btn btn-success btn-sm" runat="server" Text="Submit" />
                <asp:Button ID="btnChangeCancel" Style="margin-left: 10px;" Class="btn btn-secondary btn-sm" runat="server" Text="Cancel" />
            </div>
        </div>
    </div>
</asp:Content>
