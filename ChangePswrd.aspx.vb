Public Class ChangePswrd
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("AuthSession") Is Nothing Then
            Response.Redirect("Default.aspx")
        End If
    End Sub

    Protected Sub btnChangePswrdSubmit_Click(sender As Object, e As EventArgs) Handles btnChangePswrdSubmit.Click
        Try
            Dim strQuery As String = ""
            Dim mclsSQL As New clsSQLDB, dt As DataTable
            Dim mclsEncrypt As New clsEncryptDecrypt
            mclsSQL.OpenDB()

            If txtChangeUserID.Text.Trim <> String.Empty Then
                strQuery = "SELECT * FROM WEBAPP_USERS where user_id='" & txtChangeUserID.Text.Trim & "'"
                dt = mclsSQL.GetDataSet(strQuery).Tables(0)
                If dt.Rows.Count = 0 Then
                    ShowMessageAlert(Me, "User ID not found!", "error")
                    Exit Sub
                End If
            Else
                ShowMessageAlert(Me, "User ID cannot be empty!", "error")
            End If

            If txtChangeOldPswrd.Text.Trim <> String.Empty Then


                Dim strOldPassword As String = mclsEncrypt.Encrypt(txtChangeOldPswrd.Text.Trim() & "--" & txtChangeUserID.Text.Trim())

                strQuery = "SELECT * FROM WEBAPP_USERS where user_id='" & txtChangeUserID.Text.Trim & "' and password='" & strOldPassword & "'"
                dt = mclsSQL.GetDataSet(strQuery).Tables(0)
                If dt.Rows.Count = 0 Then
                    ShowMessageAlert(Me, "Old password is wrong!", "error")
                    Exit Sub
                End If
            Else
                ShowMessageAlert(Me, "Old password cannot be empty!", "error")
            End If

            If txtChangeNewPswrd.Text.Trim = String.Empty Then
                ShowMessageAlert(Me, "New password cannot be empty!", "error")
                Exit Sub
            End If

            If txtChangeNewPswrd.Text.Trim <> txtChangeConfirmPswrd.Text.Trim Then
                ShowMessageAlert(Me, "Confirm password do not match!", "error")
                Exit Sub
            End If

            Dim strNewPswrd As String = mclsEncrypt.Encrypt(txtChangeNewPswrd.Text.Trim() & "--" & txtChangeUserID.Text.Trim())
            strQuery = "update WEBAPP_USERS set password='" & strNewPswrd & "' where user_id='" & txtChangeUserID.Text.Trim & "'"
            mclsSQL.ExecuteNonQuery(strQuery)
            mclsSQL.CloseDB()

            ShowMessageAlert(Me, "Password was changed successfully!", "success")

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub btnChangeCancel_Click(sender As Object, e As EventArgs) Handles btnChangeCancel.Click
        Try

            Dim returnUrl As String = Request.QueryString("returnUrl")
            If Not String.IsNullOrEmpty(returnUrl) Then
                returnUrl = Server.UrlDecode(returnUrl)

                If returnUrl.StartsWith("~") Then
                    returnUrl = VirtualPathUtility.ToAbsolute(returnUrl)
                End If

                If Not returnUrl.Contains("://") AndAlso Not returnUrl.StartsWith("//") Then
                    Response.Redirect(returnUrl, False)
                    HttpContext.Current.ApplicationInstance.CompleteRequest()
                    Return
                End If
            End If

            If Request.UrlReferrer IsNot Nothing Then
                Try
                    If String.Equals(Request.UrlReferrer.Host, Request.Url.Host, StringComparison.OrdinalIgnoreCase) Then
                        Response.Redirect(Request.UrlReferrer.ToString(), False)
                        HttpContext.Current.ApplicationInstance.CompleteRequest()
                        Return
                    End If
                Catch ex As Exception
                    ' ignore and fall through to client-side fallback
                End Try
            End If

            Dim script As String = "if (document.referrer && document.referrer.indexOf(location.hostname) !== -1) { window.location = document.referrer; } else { history.back(); }"
            Dim sm As ScriptManager = TryCast(ScriptManager.GetCurrent(Page), ScriptManager)
            If sm IsNot Nothing Then
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "goBack", script, True)
            Else
                ClientScript.RegisterStartupScript(Me.GetType(), "goBack", script, True)
            End If

        Catch ex As Exception
            Response.Redirect("Home.aspx", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        End Try
    End Sub
End Class