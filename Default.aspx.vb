Public Class Login2
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Session("AuthSession") = Nothing

        'If Not Session("AuthSession") Is Nothing Then Logout()

        If Not IsPostBack Then

            forgotPswrdContainer.Visible = False
            'changPswrdContainer.Visible = False

            Dim Script As String = $"document.getElementById('{txtUserID.ClientID}').focus();"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "SetFocusScript", Script, True)

        End If

    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Try

            'Dim mclsAPI As New clsPrismAPI
            'If mclsAPI.IsAPI_LoginSuccessfull(txtUserID.Text, txtPswrd.Text) Then
            '    Session("AuthSession") = mclsAPI.authSession
            '    Session("EmpSID") = mclsAPI.EmpSID
            '    Session("UserID") = txtUserID.Text

            '    mclsAPI.LogoutWebClient()

            '    Response.Redirect("Home.aspx")
            'Else
            '    ShowMessageAlert(Me, "Invalid username or password!", "error")
            'End If

            Dim mclsSQL As New clsSQLDB
            Dim strQuery, strPassword As String
            Dim dt As DataTable

            Dim mclsEncrypt As New clsEncryptDecrypt
            strPassword = mclsEncrypt.Encrypt(txtPswrd.Text.Trim() & "--" & txtUserID.Text.Trim())

            mclsSQL.OpenDB()

            strQuery = "select * from WEBAPP_USERS where upper(user_id)='" & txtUserID.Text.Trim & "' and password='" & strPassword & "'"
            dt = mclsSQL.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count <> 0 Then
                Dim strGroupID As String = dt.Rows(0).Item("group_id").ToString()
                strGroupID = mclsEncrypt.Decrypt(strGroupID)
                strGroupID = strGroupID.Substring(0, strGroupID.IndexOf("--"))

                Session("groupid") = strGroupID
                Session("AuthSession") = GenerateNewSessionKey()
                Session("username") = dt.Rows(0).Item("user_name")
                Session("userid") = txtUserID.Text.Trim

                Response.Redirect("Home.aspx")
            Else
                ShowMessageAlert(Me, "Invalid username or password!", "error")
            End If

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub lnkBtnForgotPswrd_Click(sender As Object, e As EventArgs) Handles lnkBtnForgotPswrd.Click
        Try
            loginContainer.Visible = False
            forgotPswrdContainer.Visible = True
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        loginContainer.Visible = True
        forgotPswrdContainer.Visible = False
    End Sub

    'Protected Sub lnkBtnChangePswrd_Click(sender As Object, e As EventArgs) Handles lnkBtnChangePswrd.Click
    '    loginContainer.Visible = False
    '    changPswrdContainer.Visible = True
    'End Sub

    'Protected Sub btnChangeCancel_Click(sender As Object, e As EventArgs) Handles btnChangeCancel.Click
    '    loginContainer.Visible = True
    '    changPswrdContainer.Visible = False
    'End Sub

    'Protected Sub btnChangePswrdSubmit_Click(sender As Object, e As EventArgs) Handles btnChangePswrdSubmit.Click
    '    Try
    '        Dim strQuery As String = ""
    '        Dim mclsSQL As New clsSQLDB, dt As DataTable
    '        Dim mclsEncrypt As New clsEncryptDecrypt
    '        mclsSQL.OpenDB()

    '        If txtChangeUserID.Text.Trim <> String.Empty Then
    '            strQuery = "SELECT * FROM WEBAPP_USERS where user_id='" & txtChangeUserID.Text.Trim & "'"
    '            dt = mclsSQL.GetDataSet(strQuery).Tables(0)
    '            If dt.Rows.Count = 0 Then
    '                ShowMessageAlert(Me, "User ID not found!", "error")
    '                Exit Sub
    '            End If
    '        Else
    '            ShowMessageAlert(Me, "User ID cannot be empty!", "error")
    '        End If

    '        If txtChangeOldPswrd.Text.Trim <> String.Empty Then


    '            Dim strOldPassword As String = mclsEncrypt.Encrypt(txtChangeOldPswrd.Text.Trim() & "--" & txtChangeUserID.Text.Trim())

    '            strQuery = "SELECT * FROM WEBAPP_USERS where user_id='" & txtChangeUserID.Text.Trim & "' and password='" & strOldPassword & "'"
    '            dt = mclsSQL.GetDataSet(strQuery).Tables(0)
    '            If dt.Rows.Count = 0 Then
    '                ShowMessageAlert(Me, "Old password is wrong!", "error")
    '                Exit Sub
    '            End If
    '        Else
    '            ShowMessageAlert(Me, "Old password cannot be empty!", "error")
    '        End If

    '        If txtChangeNewPswrd.Text.Trim = String.Empty Then
    '            ShowMessageAlert(Me, "New password cannot be empty!", "error")
    '            Exit Sub
    '        End If

    '        If txtChangeNewPswrd.Text.Trim <> txtChangeConfirmPswrd.Text.Trim Then
    '            ShowMessageAlert(Me, "Confirm password do not match!", "error")
    '            Exit Sub
    '        End If

    '        Dim strNewPswrd As String = mclsEncrypt.Encrypt(txtChangeNewPswrd.Text.Trim() & "--" & txtChangeUserID.Text.Trim())
    '        strQuery = "update WEBAPP_USERS set password='" & strNewPswrd & "' where user_id='" & txtChangeUserID.Text.Trim & "'"
    '        mclsSQL.ExecuteNonQuery(strQuery)
    '        mclsSQL.CloseDB()

    '        ShowMessageAlert(Me, "Password was changed successfully!", "success")

    '    Catch ex As Exception
    '        ShowMessageAlert(Me, ex.Message, "error")
    '    End Try
    'End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Try

            Dim strQuery As String = ""
            Dim mclsSQL As New clsSQLDB, dt As DataTable
            Dim mclsEncrypt As New clsEncryptDecrypt
            mclsSQL.OpenDB()

            If txtForgotUserID.Text.Trim <> String.Empty Then
                strQuery = "SELECT * FROM WEBAPP_USERS where user_id='" & txtForgotUserID.Text.Trim & "'"
                dt = mclsSQL.GetDataSet(strQuery).Tables(0)
                If dt.Rows.Count = 0 Then
                    ShowMessageAlert(Me, "User ID not found!", "error")
                    Exit Sub
                End If
            Else
                ShowMessageAlert(Me, "User ID cannot be empty!", "error")
            End If

            If txtForgotEmail.Text.Trim <> String.Empty Then

                strQuery = "SELECT * FROM WEBAPP_USERS where user_id='" & txtForgotUserID.Text.Trim & "' and email='" & txtForgotEmail.Text.Trim & "'"
                dt = mclsSQL.GetDataSet(strQuery).Tables(0)
                If dt.Rows.Count = 0 Then
                    ShowMessageAlert(Me, "Entered email do not match with the registered email!", "error")
                    Exit Sub

                Else

                    Dim strNewRandomPswrd As String = RandomString(10)
                    SendEmail("Password reset from reportmanager.aseelah.com", "Your new password is: " & strNewRandomPswrd & vbCr & "Please change it on your next login." & vbCr & vbCr _
                              & "This is an automated email.  Please do not reply", txtForgotEmail.Text.Trim)

                    Dim strNewPswrd As String = mclsEncrypt.Encrypt(strNewRandomPswrd & "--" & txtForgotUserID.Text.Trim())
                    strQuery = "update WEBAPP_USERS set password='" & strNewPswrd & "' where user_id='" & txtForgotUserID.Text.Trim & "'"
                    mclsSQL.ExecuteNonQuery(strQuery)

                    ShowMessageAlert(Me, "A new password was sent to the entered email.", "info")
                    loginContainer.Visible = True
                    forgotPswrdContainer.Visible = False

                End If
            Else
                ShowMessageAlert(Me, "Email cannot be empty!", "error")
            End If

            mclsSQL.CloseDB()


        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    'Private Sub Logout()
    '    Try

    '        Dim mclsAPI As New clsPrismAPI
    '        mclsAPI.authSession = Session("AuthSession")
    '        mclsAPI.LogoutWebClient()

    '        Session("AuthSession") = Nothing

    '    Catch ex As Exception
    '        ShowMessageAlert(Me, ex.Message, "error")
    '    End Try
    'End Sub

End Class