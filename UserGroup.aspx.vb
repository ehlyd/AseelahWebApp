Public Class UserGroup
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Session("AuthSession") Is Nothing Then
                Response.Redirect("Default.aspx")
            End If

            If Not IsPostBack Then

                AddEditUserGroup()

                Dim script As String = $"document.getElementById('{txtGroupName.ClientID}').focus();"
                ClientScript.RegisterStartupScript(Me.GetType(), "setFocus", script, True)

            End If

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub AddEditUserGroup()
        Try

            Dim mclsSQL As New clsSQLDB
            Dim dt As DataTable

            If Session("EditUserGroupID") IsNot Nothing Then
                Dim strUserGroupID As String = Session("EditUserGroupID").ToString()

                mclsSQL.OpenDB()
                dt = mclsSQL.GetDataSet("select * from WEBAPP_GROUP where GROUP_ID='" & strUserGroupID & "'").Tables(0)
                If dt.Rows.Count > 0 Then
                    txtGroupID.Text = dt.Rows(0).Item("GROUP_ID").ToString()
                    txtGroupName.Text = dt.Rows(0).Item("GROUP_NAME").ToString()

                    If txtGroupName.Text.Trim.ToUpper = "ADMINISTRATOR" Then txtGroupName.Enabled = False

                    Session("SaveGroup") = "Edit"
                End If
                mclsSQL.CloseDB()

            Else
                mclsSQL.OpenDB()
                dt = mclsSQL.GetDataSet("select max(group_id)maxid from WEBAPP_GROUP").Tables(0)

                Dim intGroupID As Int16
                If dt.Rows.Count <> 0 Then
                    intGroupID = dt.Rows(0).Item(0) + 1
                Else
                    intGroupID = 1
                End If
                txtGroupID.Text = intGroupID.ToString
                txtGroupName.Text = ""

                Session("SaveGroup") = "Add"

                mclsSQL.CloseDB()
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            Dim strQuery As String = ""
            Dim mclsSQL As New clsSQLDB
            mclsSQL.OpenDB()

            strQuery = "select * from WEBAPP_GROUP where upper(group_name)='" & UCase(txtGroupName.Text.Trim) & "'"
            Dim dt As DataTable = mclsSQL.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count <> 0 Then
                ShowMessageAlert(Me, "Group name already exists!", "error")
            Else

                If Session("SaveGroup") = "Edit" Then

                    strQuery = "update WEBAPP_GROUP set GROUP_NAME='" & txtGroupName.Text.Replace("'", "''") & "' where GROUP_ID='" & txtGroupID.Text & "'"
                    mclsSQL.ExecuteNonQuery(strQuery)

                    Session("EditUserGroupID") = Nothing

                ElseIf Session("SaveGroup") = "Add" Then

                    strQuery = "insert into WEBAPP_GROUP (GROUP_NAME) values ('" & txtGroupName.Text.Replace("'", "''") & "')"
                    mclsSQL.ExecuteNonQuery(strQuery)

                End If

                txtGroupName.Enabled = False
                btnSave.Enabled = False

                ShowMessageAlert(Me, "User Group saved successfully.", "success")

            End If

            mclsSQL.CloseDB()
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Response.Redirect("UserGroupList.aspx")
    End Sub

End Class