
Imports System.Xml
Imports Microsoft.Reporting.WebForms

Public Class PISummary
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Session("AuthSession") Is Nothing Then
                Response.Redirect("Default.aspx")
            End If

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
            dt = mclsOra.GetDataSet("select sbs_no,sbs_name from rps.subsidiary where sbs_no not in(1,3,7) order by sbs_name").Tables(0)

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

            mclsOra.OpenDB()
            dt = mclsOra.GetDataSet("select store_code,store_name from rps.store where sbs_sid in
                                    (select sid from rps.subsidiary where sbs_no='" & Session("sbs_no") & "') 
                                    AND upper(store_name) NOT LIKE '%ONLINE%' AND upper(store_name) NOT LIKE '%HOUSE%'
                                    AND upper(store_name) NOT LIKE '%STOCK%' AND upper(store_name) NOT LIKE '%REPLENISH%'
                                    AND upper(store_name) NOT LIKE '%WH%' AND upper(store_name) NOT LIKE '%DEFAULT%'
                                    and active=1
                                    order by store_name").Tables(0)

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
            'strQuery = "SELECT DISTINCT PIS.NAME,TO_CHAR(PIS.CREATED_DATETIME,'YYYY-MM-DD')CREATED_DATE,
            '            START_QTY
            '            FROM rps.PI_SHEET PIS INNER JOIN RPS.PI_START PST ON PST.SHEET_SID=PIS.SID
            '            INNER JOIN rps.store st ON ST.sid=PIS.STORE_SID 
            '            INNER JOIN RPS.SUBSIDIARY SB ON SB.SID=ST.SBS_SID 
            '            LEFT JOIN RPS.PI_ZONE PIZ ON PIZ.SHEET_SID = PIS.SID and PIZ.TENANT_SID=PST.TENANT_SID
            '            LEFT JOIN RPS.PI_ZONE_QTY PZQ ON PIZ.SID=PZQ.PI_ZONE_SID AND PZQ.PI_START_SID=PST.SID
            '            WHERE store_code='" & strStoreCode & "' AND SBS_NO=" & strSBSNo & " 
            '            AND PIS.IN_PROGRESS<>3 AND (PST.QTY<>0 OR (NVL(PZQ.SCAN_QTY,0)+NVL(PZQ.IMPORTED_QTY,0)+NVL(PZQ.MANUAL_QTY,0)) <>0)
            '            ORDER BY TO_CHAR(PIS.CREATED_DATETIME,'YYYY-MM-DD') DESC"

            strQuery = "SELECT PI_NAME,CREATED_DATE,START_QTY,COUNTED_QTY,DIFFERENCES_QTY,DIFFERENCES_SALES_PRICE,PI_STATUS
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

            'ddlPIName.Items.Clear()
            'If dt.Rows.Count <> 0 Then

            '    For Each dRow As DataRow In dt.Rows
            '        ddlPIName.Items.Add(dRow.Item("NAME"))
            '    Next

            '    Session("pi_name") = ddlPIName.SelectedValue

            'End If

            BindReport(Nothing)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BindReport(ByVal dt As DataTable)
        ReportViewer1.LocalReport.DataSources.Clear()

        If Not IsNothing(dt) Then

            ReportViewer1.Visible = True

            Dim rds As New Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", dt)
            ReportViewer1.LocalReport.DataSources.Add(rds)

            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/Report1.rdlc")

            ReportViewer1.LocalReport.DisplayName = "Showroom Count Form"

            ReportViewer1.LocalReport.Refresh()

        Else
            ReportViewer1.Visible = False
        End If
    End Sub


    Protected Sub ddLSubsidiary_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSubsidiary.SelectedIndexChanged
        Try
            Session("sbs_no") = Mid(ddlSubsidiary.SelectedValue, 1, InStr(ddlSubsidiary.SelectedValue, "-") - 1)

            FillStore()
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
            'ClientScript.RegisterStartupScript(Me.GetType, "error", "<script> alert('" & ex.Message & "')</script>")
        End Try
    End Sub

    'Protected Sub ShowMessageAlert(strMessage As String, strMessageType As String)

    '    Dim script As String

    '    Dim strMessageIcon As String = ""

    '    Select Case strMessageType
    '        Case "error"
    '            strMessageIcon = "Error!"
    '        Case "success"
    '            strMessageIcon = "Success!"
    '        Case "warning"
    '            strMessageIcon = "Warning!"
    '        Case "info"
    '            strMessageIcon = "Info!"
    '        Case "question"
    '            strMessageIcon = "Question!"
    '    End Select

    '    strMessage = Replace(strMessage, "'", "")

    '    If strMessageType <> "question" Then
    '        script = $"<script>Swal.fire('{strMessageIcon}', '{strMessage}', '{strMessageType}');</script>"

    '        ClientScript.RegisterStartupScript(Me.GetType(), "showSweetAlert", script)

    '    Else

    '        'WaitCursor()

    '        Dim yesPostBackScript As String = ClientScript.GetPostBackEventReference(Me, "YesClicked")
    '        Dim noPostBackScript As String = ClientScript.GetPostBackEventReference(Me, "NoClicked")

    '        script = $"<script>
    '        Swal.fire({{
    '            title: 'Confirmation',
    '            text: '{strMessage}',
    '            icon: 'question',
    '            showCancelButton: true,
    '            confirmButtonText: 'Yes',
    '            cancelButtonText: 'No'
    '        }}).then((result) => {{
    '             if (result.isConfirmed) {{
    '                {yesPostBackScript};
    '             }} else if (result.dismiss === Swal.DismissReason.cancel) {{
    '                {noPostBackScript};
    '             }}
    '        }})</script>"
    '        ClientScript.RegisterStartupScript(Me.GetType(), "showConfirmationAlert", script)

    '    End If
    'End Sub

    'Protected Sub btnPISummary_Click(sender As Object, e As EventArgs) Handles btnPISummary.Click
    Private Sub ShowCountReport(strPIName As String)

        Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
        Dim dt As DataTable
        Dim strQuery As String = ""

        Try
            Dim strSbsNo, strStoreCode As String ', strPIName As String
            strSbsNo = Session("sbs_no")
            strStoreCode = Session("store_code")
            'strPIName = Session("pi_name")

            mclsOra.OpenDB()

            strQuery = "SELECT * FROM XXASH_PISUMMARY_V WHERE STORE_CODE='" & strStoreCode & "' AND SBS_NO=" & strSbsNo & " AND PI_NAME='" & strPIName & "'"

            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count <> 0 Then
                BindReport(dt)
            Else
                ShowMessageAlert(Me, "No data found.", "info")
                ClientScript.RegisterStartupScript(Me.GetType, "error", "<script> alert('no record found.')</script>")
                ReportViewer1.LocalReport.DataSources.Clear()
                ReportViewer1.LocalReport.Refresh()
            End If

            mclsOra.CloseDB()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    'Private Sub ddlPIName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlPIName.SelectedIndexChanged
    '    Try
    '        Session("pi_name") = ddlPIName.SelectedValue
    '    Catch ex As Exception
    '        ShowMessageAlert(ex.Message, "error")
    '        'ClientScript.RegisterStartupScript(Me.GetType, "error", "<script> alert('" & ex.Message & "')</script>")
    '    End Try
    'End Sub

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
                selectButton.Text = "Show Count Form"
            End If

            'If IsNumeric(e.Row.Cells(1).Text) Then
            '    e.Row.Cells(1).HorizontalAlign = HorizontalAlign.Center
            'End If
            'If IsNumeric(e.Row.Cells(2).Text) Then
            '    e.Row.Cells(2).HorizontalAlign = HorizontalAlign.Center
            'End If
            'If IsNumeric(e.Row.Cells(7).Text) Then
            '    e.Row.Cells(7).HorizontalAlign = HorizontalAlign.Center
            'End If

            If IsNumeric(e.Row.Cells(3).Text) Then
                'e.Row.Cells(3).HorizontalAlign = HorizontalAlign.Right
                e.Row.Cells(3).Text = Format(CDbl(e.Row.Cells(3).Text), "#,##0")
            End If
            If IsNumeric(e.Row.Cells(4).Text) Then
                'e.Row.Cells(4).HorizontalAlign = HorizontalAlign.Right
                e.Row.Cells(4).Text = Format(CDbl(e.Row.Cells(4).Text), "#,##0")
            End If
            If IsNumeric(e.Row.Cells(5).Text) Then
                'e.Row.Cells(5).HorizontalAlign = HorizontalAlign.Right
                e.Row.Cells(5).Text = Format(CDbl(e.Row.Cells(5).Text), "#,##0")
            End If
            If IsNumeric(e.Row.Cells(6).Text) Then
                'e.Row.Cells(6).HorizontalAlign = HorizontalAlign.Right
                e.Row.Cells(6).Text = Format(CDbl(e.Row.Cells(6).Text), "#,##0")
            End If

        ElseIf e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(1).Text = "PI Name"
            'e.Row.Cells(1).CssClass = "alignCenter"

            e.Row.Cells(2).Text = "Created Date"
            'e.Row.Cells(2).CssClass = "alignCenter"

            e.Row.Cells(3).Text = "System Qty"
            e.Row.Cells(4).Text = "Scanned Qty"
            e.Row.Cells(5).Text = "Final Result + -"
            e.Row.Cells(6).Text = "Diff. Selling Price"

            e.Row.Cells(7).Text = "Status"
            'e.Row.Cells(7).CssClass = "alignCenter"

            For i As Integer = 3 To 6
                'e.Row.Cells(i).CssClass = "alignRightHeader"
            Next


        End If
    End Sub

    Private Sub gridViewPI_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gridViewPI.SelectedIndexChanged
        Try
            ShowCountReport(gridViewPI.SelectedRow.Cells(1).Text)
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub
End Class