Imports System.Globalization
Imports Oracle.ManagedDataAccess.Client

Public Class PIDetails
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'If Session("AuthSession") Is Nothing Then
            '    Response.Redirect("Default.aspx")
            'End If

            If Not IsPostBack Then

                FillSubsidiary()

            End If

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub FillSubsidiary()
        Try
            Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
            Dim dt As DataTable

            mclsOra.OpenDB()
            dt = mclsOra.GetDataSet("select sbs_no,sbs_name from XXASH_SUBSIDIARY_V ORDER BY sbs_name").Tables(0)

            If dt.Rows.Count <> 0 Then
                For Each dRow As DataRow In dt.Rows
                    ddlSubsidiary.Items.Add(dRow.Item("sbs_no") & "-" & dRow.Item("sbs_name"))
                Next

                Session("sbs_no") = Mid(ddlSubsidiary.SelectedValue, 1, InStr(ddlSubsidiary.SelectedValue, "-") - 1)

                FillStore()
            End If
            mclsOra.CloseDB()

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub FillStore()
        Try
            Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
            Dim dt As DataTable
            Dim strQuery As String

            mclsOra.OpenDB()
            strQuery = "select store_code,store_name from XXASH_STORE_V where sbs_no='" & Session("sbs_no") & "' and active=1"
            dt = mclsOra.GetDataSet(strQuery).Tables(0)

            ddlStore.Items.Clear()

            If dt.Rows.Count <> 0 Then

                For Each dRow As DataRow In dt.Rows
                    ddlStore.Items.Add(dRow.Item("store_code") & "-" & dRow.Item("store_name"))
                Next

                Session("store_code") = Mid(ddlStore.SelectedValue, 1, InStr(ddlStore.SelectedValue, "-") - 1)

                FillPIGrid()
            End If
            mclsOra.CloseDB()

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub FillPIGrid()
        Try
            Dim strSBSNo, strStoreCode As String
            Dim strQuery As String

            strSBSNo = Session("sbs_no")
            strStoreCode = Session("store_code")

            Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
            Dim dt As DataTable

            mclsOra.OpenDB()

            strQuery = "SELECT EBS_PI_NO,PI_NAME,CREATED_DATE,START_QTY,COUNTED_QTY,DIFFERENCES_QTY,DIFFERENCES_SALES_PRICE,PI_STATUS,PIS_SID
                         FROM XXASH_PISUMMARY_V WHERE STORE_CODE='" & strStoreCode & "' and SBS_NO=" & strSBSNo & "  
                        ORDER BY CREATED_DATE DESC"

            dt = mclsOra.GetDataSet(strQuery).Tables(0)

            If dt.Rows.Count <> 0 Then
                gridViewPI.DataSource = dt
                gridViewPI.DataBind()
            Else
                gridViewPI.DataSource = Nothing
                gridViewPI.DataBind()
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Protected Sub ddLSubsidiary_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSubsidiary.SelectedIndexChanged
        Try
            Session("sbs_no") = Mid(ddlSubsidiary.SelectedValue, 1, InStr(ddlSubsidiary.SelectedValue, "-") - 1)

            FillStore()
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub ddlStore_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlStore.SelectedIndexChanged
        Try
            Session("store_code") = Mid(ddlStore.SelectedValue, 1, InStr(ddlStore.SelectedValue, "-") - 1)

            FillPIGrid()
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub gridViewPI_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gridViewPI.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then

            Dim selectButton As LinkButton = TryCast(e.Row.Cells(0).Controls(0), LinkButton)
            If selectButton IsNot Nothing Then
                selectButton.Text = "Download"
                'selectButton.OnClientClick = "downloadClick();"
                selectButton.Attributes.Add("onclick", "downloadClick();")
            End If

            If IsNumeric(e.Row.Cells(4).Text) Then
                e.Row.Cells(4).Text = Format(CDbl(e.Row.Cells(4).Text), "#,##0")
            End If
            If IsNumeric(e.Row.Cells(5).Text) Then
                e.Row.Cells(5).Text = Format(CDbl(e.Row.Cells(5).Text), "#,##0")
            End If
            If IsNumeric(e.Row.Cells(6).Text) Then
                e.Row.Cells(6).Text = Format(CDbl(e.Row.Cells(6).Text), "#,##0")
            End If
            If IsNumeric(e.Row.Cells(7).Text) Then
                e.Row.Cells(7).Text = Format(CDbl(e.Row.Cells(7).Text), "#,##0")
            End If

            e.Row.Cells(9).Visible = False

        ElseIf e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(1).Text = "EBS PI#"

            e.Row.Cells(2).Text = "PI Name"

            e.Row.Cells(3).Text = "Created Date"

            e.Row.Cells(4).Text = "System Qty"
            e.Row.Cells(5).Text = "Scanned Qty"
            e.Row.Cells(6).Text = "Final Result + -"
            e.Row.Cells(7).Text = "Diff. Selling Price"

            e.Row.Cells(8).Text = "Status"

            e.Row.Cells(9).Visible = False

        End If
    End Sub

    Private Sub gridViewPI_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gridViewPI.SelectedIndexChanged
        Try
            Dim strPiSID As String, strPIName As String
            strPiSID = gridViewPI.SelectedRow.Cells(9).Text
            strPIName = gridViewPI.SelectedRow.Cells(2).Text

            DownloadPIDetails(strPiSID, strPIName)
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub DownloadPIDetails(PiSID As String, PIName As String)
        Try
            Dim dt As DataTable
            Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
            mclsOra.OpenDB()

            Dim strStoreCode As String = Session("store_code")

            dt = mclsOra.GetDatatableSP("XXASH_INV_DETAIL_RPT", New String() {"p_StoreCode", "p_PiSID"}, New OracleDbType() {OracleDbType.Varchar2, OracleDbType.Varchar2}, New String() {strStoreCode, PiSID}, "p_InvDetail")
            If dt.Rows.Count <> 0 Then

                Dim strFilename As String = PIName & " - PI Detail.xlsx"
                Dim filePath As String = Server.MapPath("~/Exports/" & strFilename)

                Dim totalColumnNames As String() = {"START_QTY", "COUNTED_QTY", "DIFFERENCES_QTY", "LAST_COUNTED_QTY", "SHIPMENT_RECEIVED_QTY", "SOLD_QTY", "IBT_IN_QTY", "IBT_OUT_QTY", "INTRANSIT_QTY", "PI_ADJUST_QTY", "MANUAL_ADJUST_QTY", "SALES_QTY"}
                ExportToExcel_EPPlus(dt, filePath, totalColumnNames)

                'Dim fileCookie As New HttpCookie("downloadStarted", "true")
                'fileCookie.Path = "/"
                'fileCookie.Expires = DateTime.Now.AddMinutes(1)
                'Response.AppendCookie(fileCookie)

                Dim fileCookie As New HttpCookie("downloadStarted", "true")
                fileCookie.Path = "/" ' Match the JS path
                fileCookie.HttpOnly = False
                fileCookie.Expires = DateTime.Now.AddMinutes(5)
                Response.Cookies.Add(fileCookie) ' Use .Add instead of .Append

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                Response.AddHeader("content-disposition", "attachment;filename=" & strFilename)
                Response.TransmitFile(filePath)
                Response.Flush()
                Response.End()

            End If

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "cursorDefault", "defaultCursor();", True)
            Throw ex
        End Try
    End Sub
End Class