Imports Microsoft.Ajax.Utilities

Public Class GroupAccess
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("AuthSession") Is Nothing Then
            Response.Redirect("Default.aspx")
        End If

        If Not IsPostBack Then
            FillUserGroup()
            FillGridModules()
            CreateGoupAccessTemptable()
        End If
    End Sub

    Private Sub FillUserGroup()
        Try

            Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
            mclsOra.OpenDB()

            Dim strQuery As String
            strQuery = "SELECT SID,USER_GROUP_NAME  FROM rps.USER_GROUP ORDER BY USER_GROUP_NAME"
            Dim dt As DataTable = mclsOra.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count <> 0 Then

                'For Each dRow As DataRow In dt.Rows
                '    lstUserGroup.Items.Add(dRow("USER_GROUP_NAME").ToString().Trim())
                '    lstUserGroup.Items(lstUserGroup.Items.Count - 1).Value = dRow("SID").ToString().Trim()
                'Next
                'lstUserGroup.SelectedIndex = 0

                GridViewUserGroup.DataSource = dt
                GridViewUserGroup.DataBind()

            End If

            mclsOra.CloseDB()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub GridViewUserGroup_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewUserGroup.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            ' Hide the header cell
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Text = "User Group"

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            ' Hide the data cell
            e.Row.Cells(1).Visible = False
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        Dim selectedUserIDs As New List(Of String)()

        For Each row As GridViewRow In GridViewUserGroup.Rows
            ' Find the checkbox control in the current row
            Dim chk As CheckBox = TryCast(row.FindControl("chkSelect"), CheckBox)

            ' Check if the checkbox exists and is checked
            If chk IsNot Nothing AndAlso chk.Checked Then
                ' Get the value from the corresponding cell.
                ' Cells are indexed starting from 0.
                ' In this case, UserID is at index 1 and UserName is at index 2.
                Dim userID As String = row.Cells(2).Text
                selectedUserIDs.Add(userID)
            End If
        Next

        '' Now you have a list of all selected UserIDs and can perform your action
        'If selectedUserIDs.Count > 0 Then
        '    ' Example: Display the selected IDs in a label
        '    lblStatus.Text = "Selected User IDs: " & String.Join(", ", selectedUserIDs)
        'Else
        '    lblStatus.Text = "No rows were selected."
        'End If

    End Sub

    Protected Sub chkSelectAll_CheckedChanged(sender As Object, e As EventArgs)
        ' Get the "Select All" checkbox from the header
        Dim chkSelectAll As CheckBox = TryCast(sender, CheckBox)

        ' Iterate through each row and set the individual checkbox's checked state
        For Each row As GridViewRow In GridViewModules.Rows
            Dim chk As CheckBox = TryCast(row.FindControl("chkSelect"), CheckBox)
            If chk IsNot Nothing Then
                chk.Checked = chkSelectAll.Checked
            End If
        Next

    End Sub

    Protected Sub chkSelect_CheckedChanged(sender As Object, e As EventArgs)
        ' Call your function to update the temporary table.
        ' This will iterate through ALL rows/checkboxes and update the table's contents
        ' based on the current state of the entire GridView.

        If Session("SelectedUserGroupSID") IsNot Nothing Then
            UpdateGroupAccessTemptable(Session("SelectedUserGroupSID").ToString)
        Else
            ' Handle the case where the session variable is not set, if necessary.
            ' e.g., Response.Write("Error: User Group SID not selected.")
        End If
    End Sub

    Private Sub GridViewUserGroup_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GridViewUserGroup.SelectedIndexChanged
        Try
            Session("SelectedUserGroupSID") = GridViewUserGroup.SelectedRow.Cells(1).Text
            GetModulesAccess(GridViewUserGroup.SelectedRow.Cells(1).Text)
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "Error")
        End Try
    End Sub

    Private Sub CreateGoupAccessTemptable()
        Try
            Dim mclsSQL As New clsSQLDB
            mclsSQL.OpenDB()
            mclsSQL.ExecuteNonQuery("If OBJECT_ID('_TMPWGA','U') is not null drop table _TMPWGA")
            mclsSQL.ExecuteNonQuery("SELECT * INTO _TMPWGA FROM WEBAPP_GROUP_ACCESS")
            mclsSQL.CloseDB()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub UpdateGroupAccessTemptable(GroupSID As String)
        Try
            Dim mclsSQL As New clsSQLDB, strQuery As String
            mclsSQL.OpenDB()

            mclsSQL.ExecuteNonQuery("DELETE FROM _TMPWGA WHERE USER_GROUP_SID='" & GroupSID & "'")

            For Each row As GridViewRow In GridViewModules.Rows

                Dim chk As CheckBox = TryCast(row.FindControl("chkSelect"), CheckBox)
                If chk.Checked Then
                    strQuery = "INSERT INTO _TMPWGA (USER_GROUP_SID,WEBAPP_MODULENAME) VALUES ('" & GroupSID & "','" & row.Cells(1).Text & "')"
                    mclsSQL.ExecuteNonQuery(strQuery)
                End If

            Next
            mclsSQL.CloseDB()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub FillGridModules()
        Try
            Dim mclsSQL As New clsSQLDB, strQuery As String
            Dim dt As DataTable
            mclsSQL.OpenDB()

            strQuery = "SELECT DISTINCT WEBAPP_MODULENAME FROM WEBAPP_GROUP_ACCESS"
            dt = mclsSQL.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count <> 0 Then

                GridViewModules.DataSource = dt
                GridViewModules.DataBind()

            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub GetModulesAccess(UserGroupSID As String)
        Try
            For Each row As GridViewRow In GridViewModules.Rows
                Dim chk As CheckBox = TryCast(row.FindControl("chkSelect"), CheckBox)
                chk.Checked = False
            Next

            Dim mclsSQL As New clsSQLDB, strQuery As String
            Dim dt As DataTable
            mclsSQL.OpenDB()

            strQuery = "SELECT DISTINCT WEBAPP_MODULENAME FROM WEBAPP_GROUP_ACCESS where USER_GROUP_SID='" & UserGroupSID & "'"
            dt = mclsSQL.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count <> 0 Then

                For Each dRow As DataRow In dt.Rows

                    For Each row As GridViewRow In GridViewModules.Rows

                        Dim chk As CheckBox = TryCast(row.FindControl("chkSelect"), CheckBox)
                        If row.Cells(1).Text = dRow.Item(0).ToString Then
                            chk.Checked = True
                        End If

                    Next

                Next

            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub GridViewModules_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewModules.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(1).Text = "Module Name"
        End If
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Try
            If Session("SelectedUserGroupSID") IsNot Nothing Then

                Dim strQuery As String
                Dim mclsSQL As New clsSQLDB
                mclsSQL.OpenDB()

                mclsSQL.ExecuteNonQuery("DELETE FROM WEBAPP_GROUP_ACCESS WHERE USER_GROUP_SID='" & Session("SelectedUserGroupSID").ToString & "'")
                strQuery = "INSERT INTO WEBAPP_GROUP_ACCESS (USER_GROUP_SID,WEBAPP_MODULENAME) SELECT USER_GROUP_SID,WEBAPP_MODULENAME FROM _TMPWGA WHERE USER_GROUP_SID='" & Session("SelectedUserGroupSID").ToString & "'"
                mclsSQL.ExecuteNonQuery(strQuery)
                mclsSQL.ExecuteNonQuery("DELETE FROM _TMPWGA")
                mclsSQL.ExecuteNonQuery("INSERT INTO _TMPWGA SELECT * FROM WEBAPP_GROUP_ACCESS")
                ShowMessageAlert(Me, "Group access saved successfully", "success")

                Me.ClientScript.RegisterStartupScript(Me.GetType(), "resetFlag", "<script type='text/javascript'>resetPostBackFlag();</script>", False)

                mclsSQL.CloseDB()

            End If
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "Error")
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Try
            Dim mclsSQL As New clsSQLDB
            mclsSQL.OpenDB()
            mclsSQL.ExecuteNonQuery("DELETE FROM _TMPWGA")
            mclsSQL.ExecuteNonQuery("INSERT INTO _TMPWGA SELECT * FROM WEBAPP_GROUP_ACCESS")
            mclsSQL.CloseDB()

            Response.Redirect("Home.aspx")
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "Error")
        End Try

    End Sub

End Class