Imports System.Web.Script.Serialization
Imports Microsoft.Ajax.Utilities
Imports WebGrease.Css
Public Class Users
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Session("AuthSession") Is Nothing Then
                Response.Redirect("Default.aspx")
            End If

            If Not IsPostBack Then
                FillUserGroup()
                Session("SaveRecord") = "New"

                If Session("EditUserID") <> Nothing Then
                    EditUser()
                Else
                    ClearFields()
                End If

            Else

                If Session("SaveRecord") = "Edit" Then

                    Dim scriptText As String = String.Format("togglePasswordFields('{0}', '{1}', true);", lblPswrd.ClientID, "oldPswrdContainer")
                    ClientScript.RegisterStartupScript(Me.GetType(), "togglepswrd_new", scriptText, True)

                End If

            End If

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub FillUserGroup()
        Try
            Dim mclsSQL As New clsSQLDB
            Dim dt As DataTable

            mclsSQL.OpenDB()
            Dim strSQL As String = "select GROUP_ID,GROUP_NAME from WEBAPP_GROUP ORDER BY GROUP_NAME"
            dt = mclsSQL.GetDataSet(strSQL).Tables(0)
            If dt.Rows.Count > 0 Then
                ddlUserGroup.DataSource = dt
                ddlUserGroup.DataTextField = "GROUP_NAME"
                ddlUserGroup.DataValueField = "GROUP_ID"
                ddlUserGroup.DataBind()
            End If

            mclsSQL.CloseDB()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub EditUser()
        Try

            Dim strUserID As String = Session("EditUserID").ToString()
            Dim mclsSQL As New clsSQLDB
            Dim dt As DataTable
            mclsSQL.OpenDB()
            Dim strSQL As String = "SELECT * FROM WEBAPP_USERS WHERE USER_ID='" & strUserID & "'"
            dt = mclsSQL.GetDataSet(strSQL).Tables(0)
            If dt.Rows.Count > 0 Then
                txtUserID.Text = dt.Rows(0).Item("USER_ID").ToString()
                txtUserID.Enabled = False
                txtUserName.Text = dt.Rows(0).Item("USER_NAME").ToString()
                txtEmail.Text = dt.Rows(0).Item("EMAIL").ToString()

                Dim mclsEncrypt As New clsEncryptDecrypt
                Dim strGroupID As String = dt.Rows(0).Item("GROUP_ID").ToString()

                strGroupID = mclsEncrypt.Decrypt(strGroupID)
                strGroupID = strGroupID.Substring(0, strGroupID.IndexOf("--"))

                ddlUserGroup.SelectedValue = strGroupID
                Session("SaveRecord") = "Edit"

                If txtUserName.Text.Trim.ToUpper = "ADMINISTRATOR" Then
                    txtUserName.Enabled = False
                    ddlUserGroup.Enabled = False
                End If

                Dim scriptText As String = String.Format("togglePasswordFields('{0}', '{1}', true);", lblPswrd.ClientID, "oldPswrdContainer")
                ClientScript.RegisterStartupScript(Me.GetType(), "togglepswrd_new", scriptText, True)

            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Private Sub ClearFields()
        txtUserID.Text = String.Empty
        txtUserID.Enabled = True
        txtUserName.Text = String.Empty
        txtOldPassword.Text = String.Empty
        txtPassword.Text = String.Empty
        txtConfirmPswrd.Text = String.Empty
        txtEmail.Text = String.Empty
        Session("SaveRecord") = "New"

        Dim scriptText As String = String.Format("togglePasswordFields('{0}', '{1}', false);", lblPswrd.ClientID, "oldPswrdContainer")
        ClientScript.RegisterStartupScript(Me.GetType(), "togglepswrd_save", scriptText, True)
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try

            Save()

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub Save()
        Try

            If Session("SaveRecord") = "New" Then

                If txtUserID.Text.Trim() = String.Empty Then
                    Throw New Exception("User ID is required.")
                ElseIf txtPassword.Text.Trim() = String.Empty Then
                    Throw New Exception("Password is required.")
                End If

                If txtPassword.Text.Trim() <> txtConfirmPswrd.Text.Trim() Then
                    Throw New Exception("Password and Confirm Password do not match.")
                End If

                Dim mclsSQL As New clsSQLDB
                Dim strQuery As String
                Dim mclsEncrypt As New clsEncryptDecrypt


                Dim strPassword As String = mclsEncrypt.Encrypt(txtPassword.Text.Trim() & "--" & txtUserID.Text.Trim())
                Dim strGroupID As String = mclsEncrypt.Encrypt(ddlUserGroup.SelectedValue & "--" & txtUserID.Text.Trim)

                mclsSQL.OpenDB()
                strQuery = "SELECT * FROM WEBAPP_USERS WHERE USER_ID='" & txtUserID.Text.Trim & "'"
                Dim dt As DataTable = mclsSQL.GetDataSet(strQuery).Tables(0)
                If dt.Rows.Count <> 0 Then
                    ShowMessageAlert(Me, "User ID already exists!", "error")
                Else

                    strQuery = "INSERT INTO WEBAPP_USERS (User_ID, User_Name, Password,Email,Group_ID) 
                                        VALUES ('" & txtUserID.Text.Trim & "','" & txtUserName.Text.Trim & "','" & strPassword & "','" & txtEmail.Text.Trim & "','" & strGroupID & "')"
                    mclsSQL.ExecuteNonQuery(strQuery)

                    ShowMessageAlert(Me, "User saved successfully.", "success")

                    ClearFields()

                End If
                mclsSQL.CloseDB()

            ElseIf Session("SaveRecord") = "Edit" Then

                Dim mclsSQL As New clsSQLDB
                Dim mclsEncrypt As New clsEncryptDecrypt
                Dim strQuery As String
                mclsSQL.OpenDB()
                Dim dt As DataTable

                strQuery = "select * from WEBAPP_USERS where user_id='" & txtUserID.Text.Trim & "' and password='" & mclsEncrypt.Encrypt(txtOldPassword.Text.Trim & "--" & txtUserID.Text.Trim()) & "'"

                'strQuery = "select * from WEBAPP_USERS where user_id='" & txtUserID.Text.Trim & "' and password='" & txtOldPassword.Text.Trim & "'"

                dt = mclsSQL.GetDataSet(strQuery).Tables(0)
                If dt.Rows.Count = 0 Then
                    ShowMessageAlert(Me, "Old password is incorrect!", "error")
                Else

                    Dim strPassword As String = mclsEncrypt.Encrypt(txtPassword.Text.Trim() & "--" & txtUserID.Text.Trim())
                    Dim strGroupID As String = mclsEncrypt.Encrypt(ddlUserGroup.SelectedValue & "--" & txtUserID.Text.Trim)

                    strQuery = "update WEBAPP_USERS set user_name='" & txtUserName.Text.Trim & "',email='" & txtEmail.Text.Trim & "',password='" & strPassword & "',group_id='" & strGroupID & "'
                                where user_id='" & txtUserID.Text.Trim & "'"
                    mclsSQL.ExecuteNonQuery(strQuery)

                    ShowMessageAlert(Me, "User saved successfully.", "success")

                    ClearFields()

                End If
                mclsSQL.CloseDB()

            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Protected Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Try
            Response.Redirect("UserList.aspx")
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub
End Class