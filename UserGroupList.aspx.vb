Public Class UserGroupList
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
            dt = mclsSQL.GetDataSet("select * from WEBAPP_GROUP order by GROUP_ID").Tables(0)
            If dt.Rows.Count > 0 Then
                gvUserGroup.DataSource = dt
                gvUserGroup.DataBind()
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub gvUserGroup_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles gvUserGroup.RowEditing
        Try
            Session("EditUserGroupID") = gvUserGroup.Rows(e.NewEditIndex).Cells(1).Text
            Response.Redirect("UserGroup.aspx")
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Session("EditUserGroupID") = Nothing
        Response.Redirect("UserGroup.aspx")
    End Sub

    Private Sub gvUserGroup_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvUserGroup.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(1).Text = "Group ID"
            e.Row.Cells(2).Text = "Group Name"
        End If
    End Sub
End Class