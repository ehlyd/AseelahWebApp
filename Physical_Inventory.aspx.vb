Imports System.Security.Cryptography
Imports System.Web.UI.WebControls.Expressions

Public Class Physical_Inventory
    Inherits System.Web.UI.Page
    Implements System.Web.UI.IPostBackEventHandler

    Public Event YesButtonClicked As EventHandler
    Public Event NoButtonClicked As EventHandler

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then
                FillSubsidiary()
            End If
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Public Sub RaisePostBackEvent(eventArgument As String) Implements IPostBackEventHandler.RaisePostBackEvent


        Select Case eventArgument
            Case "YesClicked"

                If Session("QuestionType") = "CreateNewPISheet" Then
                    Panelinv.Enabled = False
                    TabInv.Enabled = False
                    TabInv.Attributes("style") = "pointer-events: none !important; opacity: 0.6;"
                    panelCreatePI.Visible = True
                End If

            Case "NoClicked"

                ShowMessageAlert(Me, "no was clicked", "info")

            Case Else

        End Select
    End Sub


    Private Sub FillSubsidiary()
        Try
            'Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
            Dim mclsOra As New clsOracleDB(RetailPro_OracleConnectionString)
            Dim dt As DataTable

            mclsOra.OpenDB()
            dt = mclsOra.GetDataSet("select sbs_no,sbs_name from rps.subsidiary where sbs_no not in(1,3,7) order by sbs_name").Tables(0)

            If dt.Rows.Count <> 0 Then
                For Each dRow As DataRow In dt.Rows
                    ddlSubsidiary.Items.Add(dRow.Item("sbs_no") & "-" & dRow.Item("sbs_name"))
                Next
                ddlSubsidiary.SelectedIndex = 0

                Session("pisbs_no") = Mid(ddlSubsidiary.SelectedValue, 1, InStr(ddlSubsidiary.SelectedValue, "-") - 1)

                FillStore()

            End If
            mclsOra.CloseDB()

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub FillStore()
        Try
            'Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
            Dim mclsOra As New clsOracleDB(RetailPro_OracleConnectionString)
            Dim dt As DataTable

            mclsOra.OpenDB()
            dt = mclsOra.GetDataSet("select store_code,store_name,sid from rps.store where sbs_sid in
                                    (select sid from rps.subsidiary where sbs_no='" & Session("pisbs_no") & "') 
                                    AND upper(store_name) NOT LIKE '%HOUSE%'
                                    AND upper(store_name) NOT LIKE '%STOCK%' AND upper(store_name) NOT LIKE '%REPLENISH%'
                                    AND upper(store_name) NOT LIKE '%DEFAULT%'
                                    and active=1
                                    order by store_name").Tables(0)

            ddlStore.Items.Clear()

            If dt.Rows.Count <> 0 Then

                For Each dRow As DataRow In dt.Rows
                    ddlStore.Items.Add(dRow.Item("store_code") & "-" & dRow.Item("store_name"))
                Next
                ddlStore.SelectedIndex = 0

                Session("store_code") = Mid(ddlStore.SelectedValue, 1, InStr(ddlStore.SelectedValue, "-") - 1)
                FillPIGridHeader()

            End If
            mclsOra.CloseDB()

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub FillPIGridHeader()
        Try
            Dim strSBSNo As String, strStoreCode As String
            Dim strQuery As String

            strSBSNo = Session("pisbs_no")
            strStoreCode = Session("store_code")

            'Dim mclsOra As New clsOracleDB("EBS_STG_OracleConnection")
            Dim mclsOra As New clsOracleDB(EBSSTG_OracleConnectionString)
            Dim dt As DataTable

            mclsOra.OpenDB()

            strQuery = "select I.STORE_CODE,ST.STORE_NAME,INV_NO,INV_NAME,TO_CHAR(INV_DATE,'YYYY-MM-DD')INV_DATE,
                        NVL(PS.START_QTY,0) START_QTY,
                        sum(QTY) EBS_QTY,
                        NVL(PS.SCAN_QTY_GOOD,0) GOOD_SCAN,
                        NVL(PS.SCAN_QTY_BAD,0) BAD_SCAN,
						NVL(PS.IMPORTED_QTY,0)IMPORTED_QTY,NVL(PS.MANUAL_QTY,0)MANUAL_QTY,                        
                        NVL(PS.DISCREPANCY_QTY,0)DISCREPANCY_QTY,
                        CASE PS.IN_PROGRESS WHEN 0 THEN 'Not Started'
                        when 1 then 'In progress'
                        when 2 then 'Error occured'
                        when 3 then 'Marked for deletion'
                        when 4 then 'Completed' end as STATUS,
                        nvl(I.RP_SHEET_SID,'0')RP_SHEET_SID
                        from XXTMP_ORA_INV i 
                        left outer join RPS.STORE@RETAILPROD.ALJEDAIE.COM.SA st on i.store_code=st.store_code
                        LEFT OUTER JOIN RPS.PI_SHEET@RETAILPROD.ALJEDAIE.COM.SA PS ON PS.SID=i.RP_SHEET_SID                         
                        WHERE I.STATUS_FLAG=0 AND SBS_NO='" & strSBSNo & "' and I.STORE_CODE='" & strStoreCode & "'
                        GROUP BY I.STORE_CODE,ST.STORE_NAME, INV_NO,INV_NAME,TO_CHAR(INV_DATE,'YYYY-MM-DD'),
                        PS.NAME,PS.IN_PROGRESS,TRUNC(PS.CREATED_DATETIME),I.RP_SHEET_SID,NVL(PS.IMPORTED_QTY,0),NVL(PS.MANUAL_QTY,0),
                        NVL(PS.START_QTY,0),NVL(PS.SCAN_QTY_GOOD,0),NVL(PS.SCAN_QTY_BAD,0),NVL(PS.DISCREPANCY_QTY,0)"

            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            If Not IsNothing(dt) Then
                gridViewPInv.DataSource = dt
                gridViewPInv.DataBind()
            Else
                gridViewPInv.DataSource = Nothing
                gridViewPInv.DataBind()
            End If
            mclsOra.CloseDB()

            'Session("storecode") = ""
            Session("invno") = ""
            Session("RPSheetSID") = "0"
            Session("ScanQty") = "0"
            Session("ImportedQty") = "0"
            Session("ManualQty") = "0"

            InitializeGridHeader()
            FillGridDetail()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub ddlSubsidiary_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSubsidiary.SelectedIndexChanged
        Try
            Session("pisbs_no") = Mid(ddlSubsidiary.SelectedValue, 1, InStr(ddlSubsidiary.SelectedValue, "-") - 1)

            FillStore()

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub gridViewPInv_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gridViewPInv.RowDataBound
        Try
            Dim headerText As String = ""
            If e.Row.RowType = DataControlRowType.DataRow Then

                For i As Integer = 1 To gridViewPInv.HeaderRow.Cells.Count
                    headerText = gridViewPInv.HeaderRow.Cells(i).Text

                    Select Case UCase(headerText)

                        Case "START_QTY", "EBS_QTY", "GOOD_SCAN", "BAD_SCAN", "IMMPORTED_QTY", "MANUAL_QTY", "DISCREPANCY_QTY"
                            e.Row.Cells(i).Text = Format(CDbl(e.Row.Cells(i).Text), "#,##0")

                        Case "RP_SHEET_SID"
                            e.Row.Cells(i).Visible = False
                    End Select

                Next

            ElseIf e.Row.RowType = DataControlRowType.Header Then

                For i As Integer = 1 To e.Row.Cells.Count
                    headerText = e.Row.Cells(i).Text
                    If headerText = "RP_SHEET_SID" Then
                        e.Row.Cells(i).Visible = False
                    End If
                Next

            End If
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub gridViewPInv_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gridViewPInv.SelectedIndexChanged

        For i As Integer = 1 To gridViewPInv.SelectedRow.Cells.Count - 1

            Dim cellValue As String = gridViewPInv.SelectedRow.Cells(i).Text
            Dim headerText As String = gridViewPInv.HeaderRow.Cells(i).Text

            Select Case UCase(headerText)

                Case "STORE_CODE"
                    Session("storecode") = cellValue

                Case "INV_NO"
                    Session("invno") = cellValue

                Case "IMPORTED_QTY"
                    Session("ImportedQty") = cellValue

                Case "MANUAL_QTY"
                    Session("ManualQty") = cellValue

                Case "GOOD_SCAN"
                    Session("ScanQty") = cellValue

                Case "RP_SHEET_SID"
                    Session("RPSheetSID") = cellValue

            End Select

        Next

        FillGridDetail()
    End Sub

    Private Sub InitializeGridHeader()
        Try
            Dim strQuery As String, dt As DataTable
            'Dim mclsOra = New clsOracleDB("RetailPro_OracleConnection")
            Dim mclsOra = New clsOracleDB(RetailPro_OracleConnectionString)
            mclsOra.OpenDB()
            strQuery = "SELECT ''barcode,''alu,''style,''color,''item_size,''description, ''arabic_desription,
		                          ''start_qty,''scan_qty,'' discrepancy_qty from dual"
            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            Session("PIsheetData") = dt.DefaultView
            BindgridViewPISheet()

            strQuery = "SELECT ''barcode,''alu,''style,''color,''item_size,''description, ''arabic_desription,
		                          ''start_qty,''imported_qty,'' discrepancy_qty from dual"
            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            Session("PIImported") = dt.DefaultView
            BindgridViewImported()


            strQuery = "SELECT ''barcode,''alu,''style,''color,''item_size,''description, ''arabic_desription,
		                          ''start_qty,''add_counts,'' discrepancy_qty from dual"
            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            Session("AddCountData") = dt.DefaultView
            BindgridViewAddCount()

            strQuery = "SELECT ''BARCODE,''STYLE,''Notes,''SCAN_QTY FROM dual"
            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            Session("BadScanData") = dt.DefaultView
            BindgridViewBadScans()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub FillGridDetail()
        Try
            Dim mclsOra As clsOracleDB
            Dim dtPiSheet, dtImported, dtEBS, dtBadScans, dtAddCount As DataTable
            Dim strQuery As String = ""


            Dim strSbsNo As String = "", strStoreCode As String = "", strInvNo As String = ""
            'If Not IsNothing(Session("storecode")) And Not IsNothing(Session("invno")) Then
            If Not IsNothing(Session("store_code")) And Not IsNothing(Session("invno")) Then
                strSbsNo = Session("pisbs_no")
                'strStoreCode = Session("storecode")
                strStoreCode = Session("store_code")
                strInvNo = Session("invno")
            End If


            If Session("RPSheetSID") <> "0" Then
                btnCreatePI.Enabled = False



                If Session("ScanQty") <> "0" And Session("ImportedQty") = "0" Then

                    '------------------------pi sheet
                    'mclsOra = New clsOracleDB("RetailPro_OracleConnection")
                    mclsOra = New clsOracleDB(RetailPro_OracleConnectionString)
                    mclsOra.OpenDB()
                    strQuery = "SELECT i.upc barcode, i.alu,i.description1 style, i.ATTRIBUTE color, i.item_size,
                                i.description2 description, i.description3 arabic_desription,
		                          st.qty start_qty,nvl(q.scan_qty,0)scan_qty,
		                        nvl(q.scan_qty,0)-nvl(st.qty,0) discrepancy_qty
                           from
                             rps.pi_start st
                             INNER JOIN rps.pi_sheet ps ON ps.sid=st.SHEET_SID 
                             left join rps.invn_sbs_item i on i.sid = st.invn_sbs_item_sid
                             left join rps.pi_zone z on st.sheet_sid = z.sheet_sid
                             left join rps.pi_start_subloc sq on sq.pi_start_sid = st.sid and sq.subloc_id = z.subloc_id
                             left outer join rps.vendor v on v.sid = i.vend_sid
                             join rps.dcs d on d.sid = i.dcs_sid
                             left outer join rps.pi_zone_qty q on q.pi_zone_sid = z.sid and q.pi_start_sid = st.sid
                        WHERE ps.sid='" & Session("RPSheetSID") & "'"

                    dtPiSheet = mclsOra.GetDataSet(strQuery).Tables(0)
                    mclsOra.CloseDB()
                    Session("PIsheetData") = dtPiSheet.DefaultView
                    BindgridViewPISheet()

                ElseIf Session("ScanQty") = "0" And Session("ImportedQty") <> "0" Then
                    btnImport.Enabled = False

                    '------------------------pi imported
                    mclsOra = New clsOracleDB(RetailPro_OracleConnectionString)
                    mclsOra.OpenDB()
                    strQuery = "SELECT i.upc barcode, i.alu,i.description1 style, i.ATTRIBUTE color, i.item_size,
                                i.description2 description, i.description3 arabic_desription,
		                          st.qty start_qty,nvl(q.imported_qty,0)imported_qty,
		                        nvl(st.qty,0)-nvl(q.imported_qty,0) discrepancy_qty
                           from
                             rps.pi_start st
                             INNER JOIN rps.pi_sheet ps ON ps.sid=st.SHEET_SID 
                             left join rps.invn_sbs_item i on i.sid = st.invn_sbs_item_sid
                             left join rps.pi_zone z on st.sheet_sid = z.sheet_sid
                             left join rps.pi_start_subloc sq on sq.pi_start_sid = st.sid and sq.subloc_id = z.subloc_id
                             left outer join rps.vendor v on v.sid = i.vend_sid
                             join rps.dcs d on d.sid = i.dcs_sid
                             left outer join rps.pi_zone_qty q on q.pi_zone_sid = z.sid and q.pi_start_sid = st.sid
                        WHERE ps.sid='" & Session("RPSheetSID") & "'"

                    dtImported = mclsOra.GetDataSet(strQuery).Tables(0)
                    mclsOra.CloseDB()
                    Session("PIImported") = dtImported.DefaultView
                    BindgridViewImported()

                    '------------------------bad scans
                    strQuery = "SELECT SKU BARCODE,nvl(i.DESCRIPTION1,' ') STYLE,
                            CASE WHEN b.FAILURE_CODE=1 THEN 'Item not recognized'
                            WHEN b.FAILURE_CODE=3 THEN 'Item does not belong to this PI Sheet' ELSE 'others' END Notes,b.SCAN_QTY,''CORRECT_BARCODE                        
                            FROM RPS.PI_SCAN_BAD b LEFT outer JOIN rps.INVN_SBS_ITEM i ON b.ITEM_SID=i.sid
                            WHERE PI_ZONE_SID IN
                            (SELECT SID FROM RPS.PI_ZONE pz WHERE pz.SHEET_SID='" & Session("RPSheetSID") & "')"

                    dtBadScans = mclsOra.GetDataSet(strQuery).Tables(0)
                    Session("BadScanData") = dtBadScans.DefaultView

                    BindgridViewBadScans()
                    '--------------------------

                ElseIf Session("ScanQty") = "0" And Session("ImportedQty") = "0" Then

                    '------------------------ebs qty
                    'mclsOra = New clsOracleDB("EBS_STG_OracleConnection")
                    mclsOra = New clsOracleDB(EBSSTG_OracleConnectionString)
                    mclsOra.OpenDB()

                    strQuery = "SELECT BARCODE,ALU,DESCRIPTION1 STYLE,""ATTRIBUTE""COLOR,ITEM_SIZE,DESCRIPTION2,DESCRIPTION3,I.QTY
                        FROM XXTMP_ORA_INV I LEFT OUTER JOIN RPS.INVN_SBS_ITEM@RETAILPROD.ALJEDAIE.COM.SA ITM ON I.BARCODE=ITM.UPC                        
                        LEFT OUTER JOIN RPS.STORE@RETAILPROD.ALJEDAIE.COM.SA ST ON I.STORE_CODE=ST.STORE_CODE
                        WHERE DESCRIPTION1 IS NOT NULL AND I.SBS_NO='" & strSbsNo & "' AND I.STORE_CODE='" & strStoreCode & "' AND INV_NO='" & strInvNo & "'"

                    dtEBS = mclsOra.GetDataSet(strQuery).Tables(0)
                    mclsOra.CloseDB()
                    Session("EBSQty") = dtEBS.DefaultView
                    BindgridViewEBSQty()

                    '------------------------bad scans
                    strQuery = "SELECT BARCODE,NVL(DESCRIPTION1,' ') STYLE,'Item not recognized' Notes,I.QTY SCAN_QTY,''CORRECT_BARCODE                        
                                FROM XXTMP_ORA_INV I LEFT OUTER JOIN RPS.INVN_SBS_ITEM@RETAILPROD.ALJEDAIE.COM.SA ITM ON I.BARCODE=ITM.UPC                        
                                LEFT OUTER JOIN RPS.STORE@RETAILPROD.ALJEDAIE.COM.SA ST ON I.STORE_CODE=ST.STORE_CODE
                                WHERE DESCRIPTION1 IS NULL AND I.SBS_NO='" & strSbsNo & "' AND I.STORE_CODE='" & strStoreCode & "' AND INV_NO=" & strInvNo & "'"

                    dtBadScans = mclsOra.GetDataSet(strQuery).Tables(0)
                    Session("BadScanData") = dtBadScans.DefaultView

                    BindgridViewBadScans()
                    '--------------------------

                End If

                '------------------------add counts
                mclsOra = New clsOracleDB(RetailPro_OracleConnectionString)
                mclsOra.OpenDB()
                strQuery = "SELECT i.upc barcode, i.alu,i.description1 style, i.ATTRIBUTE color, i.item_size,
                            i.description2 description, i.description3 arabic_desription,
                            st.qty start_qty,q.MANUAL_QTY add_counts,		                   
		                    nvl(q.MANUAL_QTY,0)-nvl(st.qty,0) disrepancy_qty
                       from
                         rps.pi_start st
                         INNER JOIN rps.pi_sheet ps ON ps.sid=st.SHEET_SID 
                         left join rps.invn_sbs_item i on i.sid = st.invn_sbs_item_sid
                         left join rps.pi_zone z on st.sheet_sid = z.sheet_sid
                         left join rps.pi_start_subloc sq on sq.pi_start_sid = st.sid and sq.subloc_id = z.subloc_id
                         left outer join rps.vendor v on v.sid = i.vend_sid
                         join rps.dcs d on d.sid = i.dcs_sid
                         left outer join rps.pi_zone_qty q on q.pi_zone_sid = z.sid and q.pi_start_sid = st.sid
                    WHERE ps.sid='" & Session("RPSheetSID") & "'
                    AND q.manual_qty<>0
                    ORDER BY i.upc"

                dtAddCount = mclsOra.GetDataSet(strQuery).Tables(0)
                mclsOra.CloseDB()
                Session("AddCountData") = dtAddCount.DefaultView

                BindgridViewAddCount()
                '--------------------------

            Else

                '------------------------ebs qty
                'mclsOra = New clsOracleDB("EBS_STG_OracleConnection")
                mclsOra = New clsOracleDB(EBSSTG_OracleConnectionString)
                mclsOra.OpenDB()

                'strQuery = "SELECT BARCODE,ALU,DESCRIPTION1 STYLE,""ATTRIBUTE""COLOR,ITEM_SIZE,DESCRIPTION2,DESCRIPTION3,I.QTY START_QTY,Q.QTY SCAN_QTY,
                '        I.QTY-NVL(Q.QTY,0) DISCR_QTY
                '        FROM XXTMP_ORA_INV I LEFT OUTER JOIN RPS.INVN_SBS_ITEM@RETAILPROD.ALJEDAIE.COM.SA ITM ON I.BARCODE=ITM.UPC
                '        LEFT OUTER JOIN RPS.INVN_SBS_ITEM_QTY@RETAILPROD.ALJEDAIE.COM.SA Q ON Q.INVN_SBS_ITEM_SID=ITM.SID
                '        LEFT OUTER JOIN RPS.STORE@RETAILPROD.ALJEDAIE.COM.SA ST ON ST.SID=Q.STORE_SID AND I.STORE_CODE=ST.STORE_CODE
                '        WHERE DESCRIPTION1 IS NOT NULL AND I.SBS_NO='" & strSbsNo & "' AND I.STORE_CODE='" & strStoreCode & "' AND INV_NO='" & strInvNo & "'"

                strQuery = "SELECT BARCODE,ALU,DESCRIPTION1 STYLE,""ATTRIBUTE""COLOR,ITEM_SIZE,DESCRIPTION2,DESCRIPTION3,I.QTY
                        FROM XXTMP_ORA_INV I LEFT OUTER JOIN RPS.INVN_SBS_ITEM@RETAILPROD.ALJEDAIE.COM.SA ITM ON I.BARCODE=ITM.UPC                        
                        LEFT OUTER JOIN RPS.STORE@RETAILPROD.ALJEDAIE.COM.SA ST ON I.STORE_CODE=ST.STORE_CODE
                        WHERE DESCRIPTION1 IS NOT NULL AND I.SBS_NO='" & strSbsNo & "' AND I.STORE_CODE='" & strStoreCode & "' AND INV_NO='" & strInvNo & "'"

                dtEBS = mclsOra.GetDataSet(strQuery).Tables(0)
                mclsOra.CloseDB()
                Session("EBSQty") = dtEBS.DefaultView
                BindgridViewEBSQty()

                '--------------------------

                '------------------------bad scans
                strQuery = "SELECT BARCODE,NVL(DESCRIPTION1,' ') STYLE,'Item not recognized' Notes,I.QTY SCAN_QTY,''CORRECT_BARCODE                        
                                FROM XXTMP_ORA_INV I LEFT OUTER JOIN RPS.INVN_SBS_ITEM@RETAILPROD.ALJEDAIE.COM.SA ITM ON I.BARCODE=ITM.UPC                        
                                LEFT OUTER JOIN RPS.STORE@RETAILPROD.ALJEDAIE.COM.SA ST ON I.STORE_CODE=ST.STORE_CODE
                                WHERE DESCRIPTION1 IS NULL AND I.SBS_NO='" & strSbsNo & "' AND I.STORE_CODE='" & strStoreCode & "' AND INV_NO='" & strInvNo & "'"

                dtBadScans = mclsOra.GetDataSet(strQuery).Tables(0)
                Session("BadScanData") = dtBadScans.DefaultView

                BindgridViewBadScans()
                '--------------------------

            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BindgridViewPISheet()
        Try
            Dim dvPISheet As DataView = DirectCast(Session("PIsheetData"), DataView)

            If Not IsNothing(dvPISheet) Then
                gridViewPISheet.DataSource = dvPISheet
                gridViewPISheet.DataBind()
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BindgridViewImported()
        Try
            Dim dvPIImported As DataView = DirectCast(Session("PIImported"), DataView)

            If Not IsNothing(dvPIImported) Then
                gridViewImported.DataSource = dvPIImported
                gridViewImported.DataBind()
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BindgridViewEBSQty()
        Try
            Dim dvEBSQty As DataView = DirectCast(Session("EBSQty"), DataView)

            If Not IsNothing(dvEBSQty) Then
                gridViewEBSQty.DataSource = dvEBSQty
                gridViewEBSQty.DataBind()
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BindgridViewBadScans()
        Try
            Dim dvBadScans As DataView = DirectCast(Session("BadScanData"), DataView)

            If Not IsNothing(dvBadScans) Then
                gridViewBadScans.DataSource = dvBadScans
                gridViewBadScans.DataBind()
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BindgridViewAddCount()
        Try
            Dim dvAddCount As DataView = DirectCast(Session("AddCountData"), DataView)

            If Not IsNothing(dvAddCount) Then
                gridViewAddCounts.DataSource = dvAddCount
                gridViewAddCounts.DataBind()
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub gridViewBadScancs_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gridViewBadScans.PageIndexChanging
        gridViewBadScans.PageIndex = e.NewPageIndex
        BindgridViewBadScans()
    End Sub

    Private Sub gridViewImported_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gridViewImported.PageIndexChanging
        gridViewImported.PageIndex = e.NewPageIndex
        BindgridViewImported()
    End Sub

    Private Sub gridViewPISheet_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gridViewPISheet.PageIndexChanging
        gridViewPISheet.PageIndex = e.NewPageIndex
        BindgridViewPISheet()
    End Sub

    Private Sub gridViewAddCounts_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gridViewAddCounts.PageIndexChanging
        gridViewAddCounts.PageIndex = e.NewPageIndex
        BindgridViewAddCount()
    End Sub

    Private Sub gridViewImported_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gridViewImported.RowDataBound
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                For i As Integer = 0 To 6
                    e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Center
                Next

                For i As Integer = 7 To 9
                    e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right
                    'e.Row.Cells(i).Text = Format(CDbl(e.Row.Cells(i).Text), "#,##0")
                Next

                'e.Row.Cells(8).Text = Format(CDbl(e.Row.Cells(8).Text), "#,##0")
                'e.Row.Cells(9).Text = Format(CDbl(e.Row.Cells(9).Text), "#,##0")
                'e.Row.Cells(10).Text = Format(CDbl(e.Row.Cells(10).Text), "#,##0")

            ElseIf e.Row.RowType = DataControlRowType.Header Then

                For i As Integer = 0 To 6
                    'e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Center
                    e.Row.Cells(i).CssClass = "alignCenter"
                Next

                For i As Integer = 7 To 9
                    'e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right
                    e.Row.Cells(i).CssClass = "alignRightHeader"
                Next

            End If
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub gridViewEBSQty_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gridViewEBSQty.PageIndexChanging
        gridViewEBSQty.PageIndex = e.NewPageIndex
        BindgridViewEBSQty()
    End Sub

    Protected Sub btnCreatePI_Click(sender As Object, e As EventArgs) Handles btnCreatePI.Click
        Try
            Dim strQuery As String = ""
            Dim dt As DataTable
            Dim mclsOra As New clsOracleDB(RetailPro_OracleConnectionString)
            mclsOra.OpenDB()
            strQuery = "SELECT * FROM RPS.PI_sheet p INNER JOIN rps.STORE st ON p.STORE_SID=st.sid
                        INNER JOIN rps.SUBSIDIARY sb ON sb.sid=st.SBS_SID
                        WHERE sb.SBS_NO='" & Session("pisbs_no") & "' and st.store_code='" & Session("store_code") & "' AND p.IN_PROGRESS=0 AND p.active=1"
            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count <> 0 Then
                Session("QuestionType") = "CreateNewPISheet"
                ShowMessageAlert(Me, "There is already an ACTIVE P.I. Sheet. Creating a new one will make the current one INACTIVE. Would you like to proceed?", "question")
            End If
            mclsOra.CloseDB()

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub ddlStore_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlStore.SelectedIndexChanged
        Try
            Session("store_code") = Mid(ddlStore.SelectedValue, 1, InStr(ddlStore.SelectedValue, "-") - 1)
            FillPIGridHeader()
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Try

            Dim dvBadScans As DataView = DirectCast(Session("BadScanData"), DataView)

            Dim dt As DataTable
            dt = dvBadScans.Table.Copy

            For Each row As DataRow In dt.Rows
                If row.Item("barcode") = Session("EditBarcode") Then
                    row.BeginEdit()
                    row.Item("CORRECT_BARCODE") = lblCorrectBarcode.Text
                    row.EndEdit()
                End If
            Next

            Session("BadScanData") = dt.DefaultView
            BindgridViewBadScans()

            lblCorrectBarcode.Text = ""
            Session("EditBarcode") = ""

            Panelinv.Enabled = True
            panelEditBarcode.Visible = False

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub


    Private Sub gridViewBadScans_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles gridViewBadScans.RowEditing

    End Sub



    Private Sub FillGridBarcodeList()
        Try
            Dim strQuery As String = ""
            Dim dt As DataTable
            Dim mclsOra As New clsOracleDB(RetailPro_OracleConnectionString)
            mclsOra.OpenDB()
            strQuery = "SELECT to_char(UPC) BARCODE,i.DESCRIPTION1 STYLE,i.""ATTRIBUTE"" COLOR,ITEM_SIZE ""SIZE"" FROM 
                        rps.INVN_SBS_ITEM i INNER JOIN rps.SUBSIDIARY sb ON i.SBS_SID=sb.sid
                        WHERE sbs_no=" & Session("pisbs_no") & " AND i.ACTIVE=1
                        ORDER BY STYLE,i.""ATTRIBUTE"",ITEM_SIZE"
            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count <> 0 Then

                Session("BarcodeList") = dt.DefaultView

                gridViewBarcodeList.DataSource = dt
                gridViewBarcodeList.DataBind()


                For Each col As DataColumn In dt.Columns
                    ddlFilterColumn.Items.Add(col.ColumnName)
                Next

            End If

            lblBarcode.Text = Session("EditBarcode")


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub gridViewBarcodeList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gridViewBarcodeList.PageIndexChanging
        Try
            gridViewBarcodeList.PageIndex = e.NewPageIndex
            BindGridViewBarcodeList()
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub BindGridViewBarcodeList()
        Try
            Dim dvBarcodeList As DataView = DirectCast(Session("BarcodeList"), DataView)

            If Not IsNothing(dvBarcodeList) Then
                gridViewBarcodeList.DataSource = dvBarcodeList
                gridViewBarcodeList.DataBind()
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub gridViewBarcodeList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gridViewBarcodeList.SelectedIndexChanged
        Try
            lblCorrectBarcode.Text = gridViewBarcodeList.SelectedRow.Cells(1).Text
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub btnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
        Try
            FilterBarcodes()
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub FilterBarcodes()
        Try
            Dim dvBarcodeList As DataView = DirectCast(Session("BarcodeList"), DataView)

            Dim filterText As String = txtFilter.Text.Trim.Replace("'", "''").ToUpper()

            dvBarcodeList.RowFilter = ddlFilterColumn.Text & " Like '" & filterText & "%'"

            gridViewBarcodeList.DataSource = dvBarcodeList
            gridViewBarcodeList.DataBind()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub gridViewBadScans_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gridViewBadScans.SelectedIndexChanged
        Try

            Session("EditBarcode") = gridViewBadScans.SelectedRow.Cells(1).Text
            Panelinv.Enabled = False
            txtFilter.Text = ""
            panelEditBarcode.Visible = True

            FillGridBarcodeList()
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub gridViewBadScans_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gridViewBadScans.RowDataBound
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then

                Dim selectButton As LinkButton = TryCast(e.Row.Cells(0).Controls(0), LinkButton)
                If selectButton IsNot Nothing Then
                    selectButton.Text = "Edit"
                End If
            End If
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Try
            lblCorrectBarcode.Text = ""
            Panelinv.Enabled = True
            panelEditBarcode.Visible = False
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        Try
            txtFilter.Text = ""
            FilterBarcodes()
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub btnMergeBadScan_Click(sender As Object, e As EventArgs) Handles btnMergeBadScan.Click
        Try
            If Session("RPSheetSID") <> "0" Then
            Else

                Dim dvBadScans As DataView = DirectCast(Session("BadScanData"), DataView)
                Dim dt As DataTable
                Dim strQuery As String

                Dim strSbsNo As String = "", strStoreCode As String = "", strInvNo As String = ""
                If Not IsNothing(Session("store_code")) And Not IsNothing(Session("invno")) Then
                    strSbsNo = Session("pisbs_no")
                    strStoreCode = Session("store_code")
                    strInvNo = Session("invno")
                End If

                'Dim mclsOra As New clsOracleDB("EBS_STG_OracleConnection")
                Dim mclsOra As New clsOracleDB(EBSSTG_OracleConnectionString)
                mclsOra.OpenDB()

                dt = dvBadScans.Table.Copy
                For Each dRow As DataRow In dt.Rows
                    If Not IsDBNull(dRow.Item("CORRECT_BARCODE")) Then

                        strQuery = "update XXTMP_ORA_INV set barcode='" & dRow.Item("CORRECT_BARCODE") & "' where barcode='" & dRow.Item("BARCODE") & "'
                        and SBS_NO='" & strSbsNo & "' AND STORE_CODE='" & strStoreCode & "' AND INV_NO='" & strInvNo & "'"
                        mclsOra.ExecuteNonQuery(strQuery)

                    End If
                Next

                ShowMessageAlert(Me, "Merge successfull!", "success")

                '------------------------refresh bad scans-------------------------
                strQuery = "SELECT BARCODE,NVL(DESCRIPTION1,' ') STYLE,'Item not recognized' Notes,I.QTY SCAN_QTY,''CORRECT_BARCODE                        
                                FROM XXTMP_ORA_INV I LEFT OUTER JOIN RPS.INVN_SBS_ITEM@RETAILPROD.ALJEDAIE.COM.SA ITM ON I.BARCODE=ITM.UPC                        
                                LEFT OUTER JOIN RPS.STORE@RETAILPROD.ALJEDAIE.COM.SA ST ON I.STORE_CODE=ST.STORE_CODE
                                WHERE DESCRIPTION1 IS NULL AND I.SBS_NO='" & strSbsNo & "' AND I.STORE_CODE='" & strStoreCode & "' AND INV_NO='" & strInvNo & "'"

                Dim dtBadScans As DataTable
                dtBadScans = mclsOra.GetDataSet(strQuery).Tables(0)
                Session("BadScanData") = dtBadScans.DefaultView

                BindgridViewBadScans()
                '---------------------------------------------------------------

                '--------------refresh EBS Qty------------------------------------
                strQuery = "SELECT BARCODE,ALU,DESCRIPTION1 STYLE,""ATTRIBUTE""COLOR,ITEM_SIZE,DESCRIPTION2,DESCRIPTION3,I.QTY
                        FROM XXTMP_ORA_INV I LEFT OUTER JOIN RPS.INVN_SBS_ITEM@RETAILPROD.ALJEDAIE.COM.SA ITM ON I.BARCODE=ITM.UPC                        
                        LEFT OUTER JOIN RPS.STORE@RETAILPROD.ALJEDAIE.COM.SA ST ON I.STORE_CODE=ST.STORE_CODE
                        WHERE DESCRIPTION1 IS NOT NULL AND I.SBS_NO='" & strSbsNo & "' AND I.STORE_CODE='" & strStoreCode & "' AND INV_NO='" & strInvNo & "'"

                Dim dtEBS As DataTable
                dtEBS = mclsOra.GetDataSet(strQuery).Tables(0)
                mclsOra.CloseDB()
                Session("EBSQty") = dtEBS.DefaultView
                BindgridViewEBSQty()
                '-------------------------------------------------------------

            End If
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub btnCancelCreate_Click(sender As Object, e As EventArgs) Handles btnCancelCreate.Click
        Panelinv.Enabled = True
        TabInv.Enabled = True
        TabInv.Attributes.Remove("style")
        panelCreatePI.Visible = False
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As EventArgs) Handles btnCreate.Click
        Try

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub
End Class