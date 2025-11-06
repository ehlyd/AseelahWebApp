Public Class ExportItemMaster
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("AuthSession") Is Nothing Then
            Response.Redirect("Default.aspx")
        End If

        If Not IsPostBack Then
            FillOrganizationDropDown()
        End If
    End Sub

    Private Sub FillOrganizationDropDown()
        Try
            Dim mclsOra As New clsOracleDB("EBS_STG_OracleConnection")
            Dim dt As DataTable

            mclsOra.OpenDB()
            Dim strQuery As String
            strQuery = "SELECT * FROM GET_ORGANIZATIONS_EXPORTITEM ORDER BY ORGANIZATION_ID"
            dt = mclsOra.GetDataSet(strQuery).Tables(0)

            If dt.Rows.Count <> 0 Then

                For Each dRow As DataRow In dt.Rows
                    ddListOrganization.Items.Add(dRow.Item(0) & "-" & dRow.Item(1))
                Next

                Session("OrgID") = ddListOrganization.Text.Substring(0, ddListOrganization.Text.ToString.IndexOf("-"))

            End If
            mclsOra.CloseDB()
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Try
            CreateTextFile()
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub CreateTextFile()
        Try

            Dim mclsOra As New clsOracleDB("EBS_STG_OracleConnection")
            Dim dtExport As DataTable
            Dim strQuery As String

            Dim strOrgID As String = Session("OrgID")

            mclsOra.OpenDB()

            'strQuery = "select count(*) from MTL_SYSTEM_ITEMS_b WHERE ORGANIZATION_ID=" & Session("OrgID")
            'intRecordCnt = mclsOra.GetDataSet(strQuery).Tables(0).Rows(0).Item(0)

            Select Case strOrgID
                Case "626", "827"
                    strQuery = " select distinct SEGMENT1||';'||DESCRIPTION||';'||case when ATTRIBUTE1 is null then SEGMENT1 else ATTRIBUTE1 end||';'||ATTRIBUTE2||';'||ATTRIBUTE3||';'||'5;'|| ORGANIZATION_ID AS COL1 from MTL_SYSTEM_ITEMS_b WHERE ORGANIZATION_ID=" & strOrgID _
                        & " and (ATTRIBUTE1 is not null or ATTRIBUTE2 is not null or ATTRIBUTE3 is not null)"
                Case Else
                    strQuery = " select distinct SEGMENT1||';'||DESCRIPTION||';'||ATTRIBUTE1||';'||ATTRIBUTE2||';'||ATTRIBUTE3||';'|| ORGANIZATION_ID AS COL1 from MTL_SYSTEM_ITEMS_b WHERE ORGANIZATION_ID=" & strOrgID _
                        & " and (ATTRIBUTE1 is not null or ATTRIBUTE2 is not null or ATTRIBUTE3 is not null)"
            End Select

            dtExport = mclsOra.GetDataSet(strQuery).Tables(0)

            If dtExport.Rows.Count <> 0 Then

                Dim strFilename As String = "aseel_Data.txt"


                Dim filePath As String = Server.MapPath("~/Exports/" & strFilename)

                ExportToTextFile(dtExport, filePath, rdoWindows.Checked)

                Session("filename") = strFilename
                Session("filePath") = filePath

                ShowMessageAlert(Me, "Done exporting.", "success")

            End If

            mclsOra.CloseDB()


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub ddListOrganization_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddListOrganization.SelectedIndexChanged
        Try
            Session("OrgID") = ddListOrganization.Text.Substring(0, ddListOrganization.Text.ToString.IndexOf("-"))
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub btnDownload_Click(sender As Object, e As EventArgs) Handles btnDownload.Click
        If Not IsNothing(Session("filename")) Then

            Response.ContentType = "text/plain"
            Response.AddHeader("content-disposition", "attachment;filename=" & Session("filename"))
            Response.TransmitFile(Session("filePath"))
            Response.End()

        Else
            ShowMessageAlert(Me, "File not found! Click export then download.", "error")
        End If
    End Sub
End Class