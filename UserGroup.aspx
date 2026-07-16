<%@ Page Title="User Group Management" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="UserGroup.aspx.vb" Inherits="AseelahWebApps.UserGroup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container smallContainer" style="border-style: solid; border-color: inherit; border-width: thin; background-color: whitesmoke; margin-left: 10%; padding-left: 40px; padding-right: 40px;">
    <h6 style="margin-top: 20px; margin-bottom: 20px;">User Group Management</h6>
    <hr />
    <div class="row" style="width: 100%;">
        <div class="col-5">
            <asp:Label ID="Label1" class="col-form-label-sm" runat="server" Text="Group ID"></asp:Label>
        </div>
        <div class="col-7">
            <asp:TextBox ID="txtGroupID" Style="width: 100%;" class="form-control-sm" runat="server" MaxLength="10" Enabled="False"></asp:TextBox>
        </div>
    </div>
    <div class="row" style="width: 100%;">
        <div class="col-5">
            <asp:Label ID="Label2" class="col-form-label-sm" runat="server" Text="Group Name"></asp:Label>
        </div>
        <div class="col-7">
            <asp:TextBox ID="txtGroupName" Style="width: 100%;" class="form-control-sm" runat="server" MaxLength="20"></asp:TextBox>
        </div>
    </div>
    
        
    <hr />
    <div class="row" style="width: 100%;">
        <div class="col" style="margin-top: 10px; display: flex; justify-content: end;">
            <asp:Button ID="btnSave" class="btn btn-success btn-sm" Style="width: 100px;" runat="server" Text="Save" OnClientClick="showWaitCursor()" OnClick="btnSave_Click" />
            <asp:Button ID="btnClose" class="btn btn-secondary btn-sm" Style="width: 100px; margin-left: 10px;" runat="server" Text="Close"  />               
        </div>
    </div>
</div>
</asp:Content>
