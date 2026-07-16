<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="AseelahWebApps.Login2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-sRIl4kxILFvY47J16cr9ZwB07vP4J8+LH7qKQnuqkuIAvNWLzeN8tE5YBujZqJLB" crossorigin="anonymous">

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css">

    <style>
        @media (min-width: 576px) {
            .login {
                width: 80%;
            }

            .pswrdChange {
                width: 60%;
            }
        }

        @media (min-width: 768px) {
            .login {
                width: 50%;
            }

            .pswrdChange {
                width: 50%;
            }
        }

        @media (min-width: 992px) {
            .login {
                width: 40%;
            }

            .pswrdChange {
                width: 40%;
            }
        }

        @media (min-width: 1200px) {
            .login {
                width: 40%;
            }

            .pswrdChange {
                width: 40%;
            }
        }
    </style>
</head>
<body style="background-color: #F5EFE7;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div style="display: flex; justify-content: center; align-items: center; height: 70vh;">
            <div class="container login" id="loginContainer" runat="server" style="border: solid; border-width: thin; border-radius: 5px; padding: 30px; background-color: whitesmoke;">
                <div class="row">

                    <div class="col-4">
                        <asp:Image ID="Image1" runat="server" Style="height: 100%; width: 100%; border-radius: 10px;" ImageUrl="~/images/aseelah.jpg" />
                    </div>

                    <div class="col-8" style="margin-top: 5%;">
                        <div class="row">
                            <div class="col-3">
                                <asp:Label ID="Label1" runat="server" Class="col-form-label-sm" Text="User ID" Font-Names="Segoe UI Semibold" Font-Size="Small"></asp:Label>

                            </div>
                            <div class="col-9" style="display: flex; justify-content: center; align-items: center;">
                                <asp:TextBox ID="txtUserID" Class="form-control-sm" runat="server" Style="width: 100%; max-width: 100%;" Font-Names="Segoe UI" Font-Size="Small" TabIndex="1" autofocus="autofocus"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row" style="margin-top: 10px;">
                            <div class="col-3">
                                <asp:Label ID="Label2" runat="server" Class="col-form-label-sm" Text="Password" Font-Names="Segoe UI Semibold" Font-Size="Small"></asp:Label>
                            </div>
                            <div class="col-9" style="display: flex; justify-content: center; align-items: center;">
                                <asp:TextBox ID="txtPswrd" runat="server" Class="form-control-sm" TextMode="Password" Style="width: 100%; max-width: 100%;" Font-Names="Segoe UI" Font-Size="Small"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-3">
                            </div>
                            <div class="col-9">
                                <asp:LinkButton ID="lnkBtnForgotPswrd" runat="server" Style="float: right;" Font-Size="X-Small" ForeColor="Blue" OnClick="lnkBtnForgotPswrd_Click">Forgot Password?</asp:LinkButton>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-3">
                            </div>
                            <div class="col-9" style="display: flex; justify-content: center; align-items: center;">
                                <asp:Button ID="btnLogin" Class="btn btn-secondary btn-sm" runat="server" Text="Login" Width="150px" OnClientClick="showWaitCursor()" />
                            </div>
                        </div>


                    </div>

                </div>
            </div>

            <%--forgot password--%>
            <div class="container pswrdChange" id="forgotPswrdContainer" runat="server" style="border: solid; border-width: thin; border-radius: 5px; padding: 30px; background-color: whitesmoke;">
                <div class="row">
                    <div class="col">
                        <p style="margin-bottom: 0px;">A new password will be sent to your registered email.</p>
                        <p>Please change on your next login.</p>
                    </div>
                </div>
                <hr />
                <div class="row">
                    <div class="col-3">
                        <asp:Label ID="Label3" runat="server" Text="User ID" Class="col-form-label-sm"></asp:Label>
                    </div>
                    <div class="col-9">
                        <asp:TextBox ID="txtForgotUserID" runat="server" Class="form-control-sm" Style="width: 100%; max-width: 100%;" Font-Names="Segoe UI" Font-Size="Small"></asp:TextBox>
                    </div>
                </div>
                <div class="row" style="margin-top: 10px;">
                    <div class="col-3">
                        <asp:Label ID="Label4" runat="server" Text="Email" Class="col-form-label-sm"></asp:Label>
                    </div>
                    <div class="col-9">
                        <asp:TextBox ID="txtForgotEmail" runat="server" TextMode="Email" Class="form-control-sm" Style="width: 100%; max-width: 100%;" Font-Names="Segoe UI" Font-Size="Small"></asp:TextBox>
                    </div>
                </div>
                <hr />
                <div class="row">
                    <div class="col-3">
                    </div>
                    <div class="col-9" style="display: flex; justify-content: end;">
                        <asp:Button ID="btnSubmit" Class="btn btn-success btn-sm" runat="server" Text="Submit" />
                        <asp:Button ID="btnCancel" Style="margin-left: 10px;" Class="btn btn-secondary btn-sm" runat="server" Text="Cancel" />
                    </div>
                </div>
            </div>

        </div>


        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/js/bootstrap.bundle.min.js" integrity="sha384-FKyoEForCGlyvwx9Hj09JcYn3nv7wiPVlz7YYwJrWVcXK/BmnVDxM+D2scQbITxI" crossorigin="anonymous"></script>
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

        <script type="text/javascript">

            function showWaitCursor() {
                document.body.style.cursor = 'wait';
            }

        </script>

    </form>


</body>
</html>
