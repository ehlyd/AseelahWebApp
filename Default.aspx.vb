Public Class Login2
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Session("AuthSession") = Nothing
        'If Not Session("AuthSession") Is Nothing Then Logout()
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
            Dim strQuery As String
            Dim dt As DataTable

            mclsSQL.OpenDB()

            strQuery = "select * from WEBAPP_USERS where upper(user_id)='" & txtUserID.Text & "' and password='" & txtPswrd.Text & "'"
            dt = mclsSQL.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count <> 0 Then
                Session("groupid") = dt.Rows(0).Item("group_id")
                Session("AuthSession") = GenerateNewSessionKey()

                Response.Redirect("Home.aspx")
            Else
                ShowMessageAlert(Me, "Invalid username or password!", "error")
            End If

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