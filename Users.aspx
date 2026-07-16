<%@ Page Title="User Management" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Users.aspx.vb" Inherits="AseelahWebApps.Users" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container smallContainer" style="border-style: solid; border-color: inherit; border-width: thin; background-color: whitesmoke; margin-left: 10%; padding-left: 40px; padding-right: 40px;">
        <h6 style="margin-top: 20px; margin-bottom: 20px;">User Management</h6>
        <hr />
        <div class="row" style="width: 100%;">
            <div class="col-5">
                <asp:Label ID="Label1" class="col-form-label-sm" runat="server" Text="User ID"></asp:Label>
            </div>
            <div class="col-7">
                <asp:TextBox ID="txtUserID" Style="width: 100%;" class="form-control-sm" runat="server" MaxLength="10"></asp:TextBox>
            </div>
        </div>
        <div class="row" style="width: 100%;">
            <div class="col-5">
                <asp:Label ID="Label2" class="col-form-label-sm" runat="server" Text="User Name"></asp:Label>
            </div>
            <div class="col-7">
                <asp:TextBox ID="txtUserName" Style="width: 100%;" class="form-control-sm" runat="server" MaxLength="20"></asp:TextBox>
            </div>
        </div>
        <div class="row" style="width: 100%;">
            <div class="col-5">
                <asp:Label ID="Label3" class="col-form-label-sm" runat="server" Text="Email"></asp:Label>
            </div>
            <div class="col-7">
                <asp:TextBox ID="txtEmail" Style="width: 100%;" class="form-control-sm" runat="server" TextMode="Email" MaxLength="30"></asp:TextBox>
            </div>
        </div>
        <div class="row" id="oldPswrdContainer" style="width: 100%; display: none;">
            <div class="col-5">
                <asp:Label ID="Label6" class="col-form-label-sm" runat="server" Text="Old password"></asp:Label>
            </div>
            <div class="col-7">
                <asp:TextBox ID="txtOldPassword" Style="width: 100%;" class="form-control-sm" runat="server" TextMode="Password"></asp:TextBox>
            </div>
        </div>
        <div class="row" style="width: 100%;">
            <div class="col-5">
                <asp:Label ID="lblPswrd" class="col-form-label-sm" runat="server" Text="Password"></asp:Label>
            </div>
            <div class="col-7">
                <asp:TextBox ID="txtPassword" Style="width: 100%;" class="form-control-sm" runat="server" TextMode="Password" MaxLength="20"></asp:TextBox>
            </div>
        </div>
        <div class="row" style="width: 100%;">
            <div class="col-5">
                <asp:Label ID="Label5" class="col-form-label-sm" runat="server" Text="Confirm password"></asp:Label>
            </div>
            <div class="col-7">
                <asp:TextBox ID="txtConfirmPswrd" Style="width: 100%;" class="form-control-sm" runat="server" TextMode="Password"></asp:TextBox>
            </div>
        </div>
        <div class="row" style="width: 100%;">
            <div class="col-5">
                <asp:Label ID="Label4" class="col-form-label-sm" runat="server" Text="Group"></asp:Label>
            </div>
            <div class="col-7">
                <asp:DropDownList ID="ddlUserGroup" style="width:100%;" class="form-select-sm" runat="server"></asp:DropDownList>                
            </div>
        </div>
        <hr />
        <div class="row" style="width: 100%;">
            <div class="col" style="margin-top: 10px; margin-bottom: 10px; display: flex; justify-content: end;">
                <asp:Button ID="btnSave" class="btn btn-success btn-sm" Style="width: 100px;" runat="server" Text="Save" OnClientClick="showWaitCursor()" OnClick="btnSave_Click" />
                <asp:Button ID="btnClose" class="btn btn-secondary btn-sm" Style="width: 100px; margin-left: 10px;" runat="server" Text="Close"  />               
            </div>
        </div>
    </div>

    <script>
        /**
         * Toggles the display of the 'oldpswrd' row and updates the text of 'lblPswrd'.
         *
         * @param {string} labelId The actual ClientID of the ASP.NET label control (lblPswrd).
         * @param {string} containerId The ID of the container to toggle (oldPswrdContainer).
         * @param {boolean | null} forceState If true/false, forces the state. If null/undefined, it toggles.
         */
        function togglePasswordFields(labelId, containerId, forceState) {
            // 1. Get the element to toggle (The Old Password row)
            const oldPswrdRow = document.getElementById(containerId);

            // 2. Get the label element to update (The New/General Password label)
            const newPswrdLabel = document.getElementById(labelId);

            if (!oldPswrdRow || !newPswrdLabel) {
                console.error("Required elements not found. Check ClientIDs.");
                return;
            }

            // Determine the target state (isHiding) based on forceState or by toggling the current display state
            let isHiding;
            if (typeof forceState === 'boolean') {
                // Force the state: if forceState is true (SHOW), we are NOT HIDING. If false (HIDE), we ARE HIDING.
                isHiding = !forceState;
            } else {
                // TOGGLE logic: if display is not 'none', it is currently visible, so we should HIDE it.
                isHiding = oldPswrdRow.style.display !== 'none';
            }

            if (isHiding) {
                // ACTION: HIDE the Old Password row
                oldPswrdRow.style.display = 'none';

                // Update the label text back to "Password"
                newPswrdLabel.textContent = 'Password';
            } else {
                // ACTION: SHOW the Old Password row
                oldPswrdRow.style.display = 'flex'; // Restore row/column layout

                // Update the label text to "New password"
                newPswrdLabel.textContent = 'New password';
            }
        }
    </script>

</asp:Content>
