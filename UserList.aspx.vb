Public Class UserList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Session("AuthSession") Is Nothing Then
                Response.Redirect("Default.aspx")
            End If

            If Not IsPostBack Then
                FillGrid()
            End If

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub FillGrid()
        Try
            Dim mclsSQL As New clsSQLDB
            Dim dt As DataTable
            mclsSQL.OpenDB()
            Dim strSQL As String = "SELECT USER_ID,USER_NAME,EMAIL,GROUP_ID GROUP_NAME FROM WEBAPP_USERS"
            dt = mclsSQL.GetDataSet(strSQL).Tables(0)
            If dt.Rows.Count > 0 Then
                gvUsers.DataSource = dt
                gvUsers.DataBind()
            End If
            mclsSQL.CloseDB()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub gvUsers_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles gvUsers.RowEditing
        Try
            Session("EditUserID") = gvUsers.Rows(e.NewEditIndex).Cells(1).Text
            Response.Redirect("Users.aspx")
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Session("EditUserID") = Nothing
        Response.Redirect("Users.aspx")
    End Sub

    Private Sub gvUsers_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvUsers.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then

            Dim strGroupID As String = "", strGroupName As String = ""
            Dim mclsEncrypt As New clsEncryptDecrypt

            strGroupID = mclsEncrypt.Decrypt(e.Row.Cells(4).Text)
            strGroupID = strGroupID.Substring(0, strGroupID.IndexOf("--"))

            Dim mclsSQl As New clsSQLDB, dt As DataTable
            mclsSQl.OpenDB()
            dt = mclsSQl.GetDataSet("select group_name from webapp_group where group_id='" & strGroupID & "'").Tables(0)
            If dt.Rows.Count <> 0 Then
                strGroupName = dt.Rows(0).Item(0)
            End If

            e.Row.Cells(4).Text = strGroupName

        ElseIf e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(1).Text = "User ID"
            e.Row.Cells(2).Text = "User Name"
            e.Row.Cells(3).Text = "Email"
            e.Row.Cells(4).Text = "Group Name"
        End If
    End Sub
End Class