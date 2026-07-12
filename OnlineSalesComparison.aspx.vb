Imports System.Globalization
Imports System.Net
Imports System.Windows.Documents.Serialization
Imports System.Windows.Media
Imports AseelahWebApps.clsShopifyJsonOrders
Imports Oracle.ManagedDataAccess.Client

Public Class OnlineSalesComparison
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Session("AuthSession") Is Nothing Then
                Response.Redirect("Default.aspx")
            End If

            If Not IsPostBack Then
                FillSubsidiary()

                txtFromDate.Text = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd")
                txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd")

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
            dt = mclsOra.GetDataSet("SELECT DISTINCT SBS_NO,SBS_NAME FROM xxash_store_v WHERE UPPER(STORE_TYPE) LIKE '%ON%LINE%' AND SBS_NO NOT IN(1,7) ORDER BY SBS_NAME").Tables(0)

            If dt.Rows.Count <> 0 Then
                For Each dRow As DataRow In dt.Rows
                    ddlSubsidiary.Items.Add(dRow.Item("SBS_NO") & "-" & dRow.Item("SBS_NAME"))
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
            strQuery = "select store_code,store_name from XXASH_STORE_V where sbs_no='" & Session("sbs_no") & "' and active=1 AND UPPER(STORE_TYPE) LIKE '%ON%LINE%'"
            dt = mclsOra.GetDataSet(strQuery).Tables(0)

            ddlStore.Items.Clear()

            If dt.Rows.Count <> 0 Then

                For Each dRow As DataRow In dt.Rows
                    ddlStore.Items.Add(dRow.Item("store_code") & "-" & dRow.Item("store_name"))
                Next

                Session("store_code") = Mid(ddlStore.SelectedValue, 1, InStr(ddlStore.SelectedValue, "-") - 1)

            End If
            mclsOra.CloseDB()

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub ddlSubsidiary_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSubsidiary.SelectedIndexChanged
        Try
            Session("sbs_no") = Mid(ddlSubsidiary.SelectedValue, 1, InStr(ddlSubsidiary.SelectedValue, "-") - 1)

            FillStore()

            If Session("sbs_no") = "2" Then
                pnlSearch.Visible = False
                gvOrderDetail.Visible = False
                ipkaddnote.Visible = False
            Else
                pnlSearch.Visible = True
                gvOrderDetail.Visible = True
                ipkaddnote.Visible = True
            End If
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub btnDownload_Click(sender As Object, e As EventArgs) Handles btnDownload.Click
        Try
            Dim strStoreCode As String = Session("store_code")

            Select Case UCase(strStoreCode)

                Case "JC04"
                    Download_JacadiOnlineSalesComparison()

                Case "IP17"
                    Download_IpekyolShopifySalesComparison()

            End Select

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub Download_JacadiOnlineSalesComparison()
        Try
            Dim dt As DataTable, strQuery As String = ""


            Dim mclsOra As New clsOracleDB("EBS_STG_OracleConnection")
            mclsOra.OpenDB()

            strQuery = "SELECT TO_CHAR(ORDER_CREATED_DATE,'YYYY-MM-DD')ORDER_CREATED_DATE,ORDER_ID,ORDER_REFERENCE,INVOICE_NUMBER,PRODUCT_REFERENCE,TO_CHAR(PRODUCT_EAN13)BARCODE,QUANTITY,UNIT_PRICE,LINE_TOTAL,
                            ORDER_STATE,CREDIT_NOTE,REFUND_QTY,REFUND_AMOUNT,ORDER_REFUNDED_DATE,NET_QTY,NET_AMOUNT,RP_SALES_QTY,RP_UNIT_PRICE,RP_SALES_AMOUNT,RP_RETURN_QTY,
                            RP_RETURN_AMOUNT,RP_NET_QTY,RP_NET_AMOUNT,NET_QTY_STATUS
                            FROM XXJED_JACADIECOM_STG_VS_RPRO_V 
                            WHERE trunc(order_created_date) BETWEEN '" & Format(CDate(txtFromDate.Text), "dd-MMM-yyyy") & "' AND '" & Format(CDate(txtToDate.Text), "dd-MMM-yyyy") & "'"

            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            mclsOra.CloseDB()

            'Session("JacadiDT") = dt
            '' Populate filter dropdown
            'PopulateFilterColumns(dt)
            '' Bind to GridView (ViewState holds filter if any)
            'BindGrid(0)

            If dt.Rows.Count <> 0 Then

                Dim strFilename As String = "JACADI online orders Staging vs Retail PRO - " & Format(Now, "yyyyMMdd_hhmmsstt") & ".xlsx"

                Dim filePath As String = Server.MapPath("~/Exports/" & strFilename)

                Dim totalColumnNames As String() = {"QUANTITY", "LINE_TOTAL", "REFUND_QTY", "REFUND_AMOUNT", "NET_QTY", "NET_AMOUNT", "RP_SALES_QTY", "RP_SALES_AMOUNT", "RP_RETURN_QTY", "RP_RETURN_AMOUNT", "RP_NET_QTY", "RP_NET_AMOUNT"}
                ExportToExcel_EPPlus_Jacadi(dt, filePath, totalColumnNames)

                Dim fileCookie As New HttpCookie("downloadStarted", "true")
                fileCookie.Path = "/"
                fileCookie.HttpOnly = False
                fileCookie.Expires = DateTime.Now.AddMinutes(5)
                Response.Cookies.Add(fileCookie)

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

    Dim dtOrders, dtLineItems, dtCustomers, dtBilling, dtShippingLines As DataTable
    Dim dtRefunds, dtRefundLineItems, dtRefundTransactions, dtRefundLineItemsComp As DataTable
    Dim dtFulfillments As DataTable

    Private Sub Download_IpekyolShopifySalesComparison()
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim ds As New DataSet
            Dim mclsAPI As New clsShopifyAPI

            Dim strFromDate As String = txtFromDate.Text.Trim() & "T00:00:00Z"
            Dim strtoDate As String = txtToDate.Text.Trim() & "T23:59:59Z"

            Dim strShopifyURL As String = ConfigurationManager.AppSettings("IpekyolShopify_URL")
            Dim strShopifyToken As String = ConfigurationManager.AppSettings("IpekyolShopify_AccessToken")

            '=====================================================
            ' normalize date boundaries
            Dim overallStart As Date = CDate(txtFromDate.Text.Trim())
            Dim overallEnd As Date = CDate(txtToDate.Text.Trim())

            ' batch window: 20 days per request (keeps existing behaviour but ensures final day is included)
            Dim batchStart As Date = overallStart
            Dim batchEnd As Date = DateAdd(DateInterval.Day, 20, batchStart)

            While batchStart <= overallEnd

                ' cap batchEnd to the overall requested end date
                If batchEnd > overallEnd Then
                    batchEnd = overallEnd
                End If

                ' build API args
                Dim apiFrom As String = Format(batchStart, "yyyy-MM-dd") & "T00:00:00Z"
                Dim apiTo As String = Format(batchEnd, "yyyy-MM-dd") & "T23:59:59Z"

                ds = mclsAPI.DownloadOrders(apiFrom, apiTo, strShopifyToken, strShopifyURL)

                If batchStart = overallStart Then
                    dtOrders = ds.Tables(0).Copy()
                    dtLineItems = ds.Tables(1).Copy()
                    dtRefunds = ds.Tables(2).Copy()
                    dtRefundLineItems = ds.Tables(3).Copy()
                    dtRefundTransactions = ds.Tables(4).Copy()
                    dtFulfillments = ds.Tables(5).Copy()
                Else
                    dtOrders.Merge(ds.Tables(0))
                    dtLineItems.Merge(ds.Tables(1))
                    dtRefunds.Merge(ds.Tables(2))
                    dtRefundLineItems.Merge(ds.Tables(3))
                    dtRefundTransactions.Merge(ds.Tables(4))
                    dtFulfillments.Merge(ds.Tables(5))
                End If

                ' advance to the next batch (day after current batchEnd)
                batchStart = DateAdd(DateInterval.Day, 1, batchEnd)
                batchEnd = DateAdd(DateInterval.Day, 20, batchStart)

            End While
            '=====================================================

            'ds = mclsAPI.DownloadOrders(strFromDate, strtoDate, strShopifyToken, strShopifyURL)
            'dtOrders = ds.Tables(0)
            'dtLineItems = ds.Tables(1)
            'dtRefunds = ds.Tables(2)
            'dtRefundLineItems = ds.Tables(3)
            'dtRefundTransactions = ds.Tables(4)
            'dtFulfillments = ds.Tables(5)

            dtRefundLineItemsComp = BuildRefundLineItemsComp()

            Dim dtShopify As DataTable = BuildShopifyReportTable()


            If dtShopify.Rows.Count <> 0 Then
                SaveIpekyoltoDB(dtShopify.Copy)
            End If

            Dim minOrderDate As DateTime
            If dtShopify.Rows.Count <> 0 Then

                'minOrderDate = CDate(dtShopify.Compute("MIN(order_date)", ""))

                '================================
                ' Compute minimum order_date robustly because order_date is stored as string.
                Dim minDt As Nullable(Of DateTime) = Nothing
                Dim formats() As String = {"yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-ddTHH:mm:ss.fffZ", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd"}
                For Each r As DataRow In dtShopify.Rows
                    Dim s As String = TryCast(r("order_date"), String)
                    If Not String.IsNullOrWhiteSpace(s) Then
                        Dim parsed As DateTime
                        If DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, parsed) _
                           OrElse DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, parsed) Then
                            If Not minDt.HasValue OrElse parsed < minDt.Value Then
                                minDt = parsed
                            End If
                        End If
                    End If
                Next

                If minDt.HasValue Then
                    minOrderDate = minDt.Value
                Else
                    ' No parsable dates found — fallback to txtFromDate to avoid exceptions
                    minOrderDate = CDate(txtFromDate.Text)
                End If
                '================================

                If Format(CDate(strFromDate), "yyyy-MM-dd") <> Format(minOrderDate, "yyyy-MM-dd") Then
                    AddMissingOrders(dtShopify, CDate(strFromDate), DateAdd(DateInterval.Day, -1, minOrderDate))
                End If
            Else
                AddMissingOrders(dtShopify, CDate(txtFromDate.Text), CDate(txtToDate.Text))
            End If


            Dim dtRetailPRO As DataTable = GetRetailPRO_IpekyolOnlineSales()

            Dim dtIPKOrderNotes As DataTable = GetIpekyolOrderNotes()

            Dim dtFinal As DataTable = JoinIpekyolandRetailPROTables(dtShopify, dtRetailPRO, dtIPKOrderNotes)

            Dim strFilename As String = "Ipekyol Shopify orders vs Retail PRO - " & Format(Now, "yyyyMMdd_hhmmsstt") & ".xlsx"
            If dtFinal.Rows.Count <> 0 Then

                Dim filePath As String = Server.MapPath("~/Exports/" & strFilename)

                Dim totalColumnNames As String() = {"QUANTITY", "LINE_TOTAL", "REFUND_QTY", "REFUND_AMOUNT", "NET_QTY", "NET_AMOUNT", "RP_SALES_QTY", "RP_SALES_AMOUNT", "RP_RETURN_QTY", "RP_RETURN_AMOUNT", "RP_NET_QTY", "RP_NET_AMOUNT"}
                ExportToExcel_EPPlus_Ipekyol(dtFinal, filePath, totalColumnNames)

                Dim fileCookie As New HttpCookie("downloadStarted", "true")
                fileCookie.Path = "/"
                fileCookie.HttpOnly = False
                fileCookie.Expires = DateTime.Now.AddMinutes(5)
                Response.Cookies.Add(fileCookie)

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                Response.AddHeader("content-disposition", "attachment;filename=" & strFilename)
                Response.TransmitFile(filePath)
                Response.Flush()
                Response.End()

            Else
                ShowMessageAlert(Me, "Orders were not found in Shopify API for the date specified.", "info")
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Function GetIpekyolOrderNotes() As DataTable
        Try
            Dim dt As DataTable, strQuery As String = ""
            Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
            mclsOra.OpenDB()
            strQuery = "select * from XXASH_IPK_SHOPIFY_NOTES"
            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            mclsOra.CloseDB()

            Return dt

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "Error")
        End Try
    End Function

    Private Sub SaveIpekyoltoDB(dtIPK As DataTable)
        Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
        Try
            Dim strQuery As String = ""
            mclsOra.OpenDB()
            strQuery = "select table_name from all_tables where table_name='XXASH_TMPIPKECOM'"
            Dim dt As DataTable = mclsOra.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count = 0 Then
                mclsOra.ExecuteNonQuery(CreateTempTable_XXASH_TMPIPKECOM)
            End If

            Dim RunID As String = System.Guid.NewGuid().ToString()
            dtIPK.Columns.Add("RUN_ID", GetType(String))
            For Each row As DataRow In dtIPK.Rows
                row("RUN_ID") = RunID
            Next

            mclsOra.BulkInsert("XXASH_TMPIPKECOM", dtIPK)

            mclsOra.ExecuteNonQuery("DELETE FROM XXASH_IPK_SHOPIFY_ORDERS WHERE ORDER_ID IN
                                    (SELECT DISTINCT ORDER_ID FROM XXASH_TMPIPKECOM WHERE RUN_ID='" & RunID & "')")

            strQuery = "INSERT INTO XXASH_IPK_SHOPIFY_ORDERS(ORDER_ID,ORDER_NAME,ORDER_DATE,SKU,VARIANT_TITLE,QUANTITY,PRICE,LINE_TOTAL,FULFILLMENT_DATE,FULFILLMENT_STATUS,RETURN_DATE,
                                                    RETURN_QTY,RETURN_TYPE,REFUND_DATE,REFUNDED_QTY,REFUND_AMOUNT,ITEM_STATUS,ITEM_RETURNED,NET_QTY,NET_AMOUNT,NET_AMOUNT_US,
                                                    KUR_SAR_USD,ORDER_STATUS,CANCEL_DATE,CANCEL_REASON,INSERTED_DATE)
                            SELECT ORDER_ID,ORDER_NAME,ORDER_DATE,SKU,VARIANT_TITLE,QUANTITY,PRICE,LINE_TOTAL,FULFILLMENT_DATE,FULFILLMENT_STATUS,RETURN_DATE,
                               RETURN_QTY,RETURN_TYPE,REFUND_DATE,REFUNDED_QTY,REFUND_AMOUNT,ITEM_STATUS,ITEM_RETURNED,NET_QTY,NET_AMOUNT,NET_AMOUNT_US,
                               KUR_SAR_USD,ORDER_STATUS,CANCEL_DATE,CANCEL_REASON,SYSDATE FROM XXASH_TMPIPKECOM WHERE RUN_ID='" & RunID & "'"
            mclsOra.ExecuteNonQuery(strQuery)

            mclsOra.ExecuteNonQuery("DELETE FROM XXASH_TMPIPKECOM WHERE RUN_ID='" & RunID & "'")

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "Error")
        Finally
            mclsOra.CloseDB()
        End Try
    End Sub

    Private Sub AddMissingOrders(ByRef dtShopify As DataTable, fromDate As Date, toDate As Date)
        Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
        Dim strQuery As String = ""
        Try
            mclsOra.OpenDB()
            strQuery = "SELECT ORDER_ID,ORDER_NAME,ORDER_DATE,SKU,VARIANT_TITLE,QUANTITY,PRICE,LINE_TOTAL,FULFILLMENT_DATE,FULFILLMENT_STATUS,RETURN_DATE,RETURN_QTY,RETURN_TYPE,
                        REFUND_DATE,REFUNDED_QTY,REFUND_AMOUNT,ITEM_STATUS,ITEM_RETURNED,NET_QTY,NET_AMOUNT,ORDER_STATUS,CANCEL_DATE,CANCEL_REASON
                        fROM XXASH_IPK_SHOPIFY_ORDERS where trunc(order_date) between '" & Format(fromDate, "dd-MMM-yyyy") & "' and '" & Format(toDate, "dd-MMM-yyyy") & "'
                         order by order_date desc,order_id"
            Dim dt As DataTable = mclsOra.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count <> 0 Then

                For Each dRow As DataRow In dt.Rows
                    Dim newRow As DataRow = dtShopify.NewRow()
                    newRow.ItemArray = dRow.ItemArray
                    dtShopify.Rows.Add(newRow)
                Next

            End If

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "Error")
        Finally
            mclsOra.CloseDB()
        End Try
    End Sub

    Public Function BuildShopifyReportTable() As DataTable

        '=========================================================
        ' RESULT TABLE
        '=========================================================
        Dim dtResult As New DataTable()

        dtResult.Columns.Add("ORDER_ID", GetType(String))
        dtResult.Columns.Add("ORDER_NAME", GetType(String))
        dtResult.Columns.Add("ORDER_DATE", GetType(String))

        dtResult.Columns.Add("SKU", GetType(String))
        dtResult.Columns.Add("VARIANT_TITLE", GetType(String))

        dtResult.Columns.Add("QUANTITY", GetType(Integer))
        dtResult.Columns.Add("PRICE", GetType(Decimal))
        dtResult.Columns.Add("LINE_TOTAL", GetType(Decimal))

        dtResult.Columns.Add("FULFILLMENT_DATE", GetType(String))
        dtResult.Columns.Add("FULFILLMENT_STATUS", GetType(String))

        dtResult.Columns.Add("RETURN_DATE", GetType(String))
        dtResult.Columns.Add("RETURN_QTY", GetType(Integer))
        dtResult.Columns.Add("RETURN_TYPE", GetType(String))

        dtResult.Columns.Add("REFUND_DATE", GetType(String))
        dtResult.Columns.Add("REFUNDED_QTY", GetType(Integer))
        dtResult.Columns.Add("REFUND_AMOUNT", GetType(Decimal))

        dtResult.Columns.Add("ITEM_STATUS", GetType(String))
        dtResult.Columns.Add("ITEM_RETURNED", GetType(String))

        dtResult.Columns.Add("NET_QTY", GetType(Integer))
        dtResult.Columns.Add("NET_AMOUNT", GetType(Decimal))

        'dtResult.Columns.Add("NET_AMOUNT_US", GetType(Decimal))
        'dtResult.Columns.Add("Kur_SAR_USD", GetType(Decimal))

        dtResult.Columns.Add("ORDER_STATUS", GetType(String))

        dtResult.Columns.Add("CANCEL_DATE", GetType(String))
        dtResult.Columns.Add("CANCEL_REASON", GetType(String))

        '=========================================================
        ' LOOP ORDER LINE ITEMS
        '=========================================================
        For Each ol As DataRow In dtLineItems.Rows

            Dim orderId As String = ol("ORDER_ID").ToString()
            Dim lineItemId As String = ol("ID").ToString()

            '=========================================================
            ' ORDER
            '=========================================================
            Dim orderRows() As DataRow =
            dtOrders.Select("ID='" & orderId.Replace("'", "''") & "'")

            If orderRows.Length = 0 Then
                Continue For
            End If

            Dim o As DataRow = orderRows(0)

            '=========================================================
            ' FULFILLMENT
            '=========================================================
            Dim fulfillmentRows() As DataRow =
            dtFulfillments.Select(
                "ORDER_ID='" & orderId.Replace("'", "''") &
                "' AND LINE_ITEM_ID='" & lineItemId.Replace("'", "''") & "'")

            Dim fulfillmentDate As String = ""
            Dim fulfillmentStatus As String = "unfulfilled"

            If fulfillmentRows.Length > 0 Then

                Dim f As DataRow = fulfillmentRows(0)

                If Not IsDBNull(f("UPDATED_AT")) AndAlso
               f("UPDATED_AT").ToString() <> "" Then

                    fulfillmentDate = f("UPDATED_AT").ToString()

                ElseIf Not IsDBNull(f("CREATED_AT")) Then

                    fulfillmentDate = f("CREATED_AT").ToString()

                End If

                Dim fStatus As String = f("STATUS").ToString().ToLower()

                If fStatus <> "" OrElse fStatus = "success" Then
                    fulfillmentStatus = "fulfilled"
                End If

            Else

                Dim lineFulfillmentStatus As String =
                ol("FULFILLMENT_STATUS").ToString().ToLower()

                If lineFulfillmentStatus <> "" OrElse lineFulfillmentStatus = "success" Then
                    fulfillmentStatus = "fulfilled"
                End If

            End If

            '=========================================================
            ' REFUND / RETURN
            '=========================================================
            Dim refundRows() As DataRow =
            dtRefundLineItemsComp.Select(
                "LINE_ITEM_ID='" & lineItemId.Replace("'", "''") & "'")

            Dim returnDate As String = ""
            Dim returnQty As Integer = 0
            Dim returnType As String = ""

            Dim refundDate As String = ""
            Dim refundedQty As Integer = 0
            Dim refundAmount As Decimal = 0D

            Dim itemStatus As String = ""
            Dim itemReturned As String = ""

            If refundRows.Length > 0 Then

                Dim rt As DataRow = refundRows(0)

                returnDate = rt("RETURN_DATE").ToString()

                Integer.TryParse(rt("RETURN_QTY").ToString(), returnQty)

                returnType = rt("RETURN_TYPE").ToString()

                refundDate = rt("REFUND_DATE").ToString()

                Integer.TryParse(rt("REFUNDED_QTY").ToString(), refundedQty)

                Decimal.TryParse(rt("REFUND_AMOUNT").ToString(), refundAmount)

                itemStatus = rt("ITEM_STATUS").ToString()

                itemReturned = rt("ITEM_RETURNED").ToString()

            End If

            '=========================================================
            ' ITEM STATUS FALLBACK
            '=========================================================
            If String.IsNullOrWhiteSpace(itemStatus) Then

                If fulfillmentDate <> "" Then
                    itemStatus = "fulfilled"
                Else
                    itemStatus = "unfulfilled"
                End If

            End If

            '=========================================================
            ' QUANTITY / PRICE
            '=========================================================
            Dim qty As Integer = 0
            Integer.TryParse(ol("QUANTITY").ToString(), qty)

            Dim price As Decimal = 0D
            Decimal.TryParse(ol("PRICE").ToString(), price)

            Dim lineTotal As Decimal = qty * price

            Dim netQty As Integer = 0
            'If refundAmount = 0 Then
            '    netQty = qty - returnQty
            'Else
            '    netQty = qty - refundedQty
            'End If
            netQty = qty - refundedQty


            Dim netAmount As Decimal = lineTotal - refundAmount

            Dim kurSarUsd As Decimal = 0.27D

            Dim netAmountUs As Decimal = netAmount * kurSarUsd

            Dim orderStatus As String = o("FINANCIAL_STATUS").ToString()

            Dim cancelDate As String = ""

            If Not IsDBNull(o("CANCELLED_AT")) Then
                cancelDate = o("CANCELLED_AT").ToString()
                orderStatus = "cancelled"
                itemStatus = "cancelled"
            End If

            Dim cancelReason As String = ""

            If Not IsDBNull(o("CANCEL_REASON")) Then
                cancelReason = o("CANCEL_REASON").ToString()
            End If

            '=========================================================
            ' ADD RESULT ROW
            '=========================================================
            Dim nr As DataRow = dtResult.NewRow()

            nr("ORDER_ID") = o("ID").ToString()
            nr("ORDER_NAME") = o("NAME").ToString()
            nr("ORDER_DATE") = o("CREATED_AT").ToString()

            nr("SKU") = ol("SKU").ToString()
            nr("VARIANT_TITLE") = ol("VARIANT_TITLE").ToString()

            nr("QUANTITY") = qty
            nr("PRICE") = price
            nr("LINE_TOTAL") = lineTotal

            nr("FULFILLMENT_DATE") = fulfillmentDate
            nr("FULFILLMENT_STATUS") = fulfillmentStatus

            nr("RETURN_DATE") = returnDate
            nr("RETURN_QTY") = returnQty
            nr("RETURN_TYPE") = returnType

            nr("REFUND_DATE") = refundDate
            nr("REFUNDED_QTY") = refundedQty
            nr("REFUND_AMOUNT") = refundAmount

            nr("ITEM_STATUS") = itemStatus
            nr("ITEM_RETURNED") = itemReturned

            nr("NET_QTY") = netQty
            nr("NET_AMOUNT") = netAmount

            'nr("NET_AMOUNT_US") = netAmountUs
            'nr("Kur_SAR_USD") = kurSarUsd

            nr("ORDER_STATUS") = orderStatus

            nr("CANCEL_DATE") = cancelDate
            nr("CANCEL_REASON") = cancelReason

            dtResult.Rows.Add(nr)

        Next

        Return dtResult

    End Function

    Public Function BuildRefundLineItemsComp() As DataTable

        '=========================================================
        ' CREATE RESULT TABLE
        '=========================================================
        Dim dtRefundLineItemsComp As DataTable = dtRefundLineItems.Copy()

        If Not dtRefundLineItemsComp.Columns.Contains("RETURN_DATE") Then
            dtRefundLineItemsComp.Columns.Add("RETURN_DATE", GetType(String))
        End If

        If Not dtRefundLineItemsComp.Columns.Contains("RETURN_QTY") Then
            dtRefundLineItemsComp.Columns.Add("RETURN_QTY", GetType(Integer))
        End If

        If Not dtRefundLineItemsComp.Columns.Contains("RETURN_TYPE") Then
            dtRefundLineItemsComp.Columns.Add("RETURN_TYPE", GetType(String))
        End If

        If Not dtRefundLineItemsComp.Columns.Contains("REFUND_DATE") Then
            dtRefundLineItemsComp.Columns.Add("REFUND_DATE", GetType(String))
        End If

        If Not dtRefundLineItemsComp.Columns.Contains("REFUNDED_QTY") Then
            dtRefundLineItemsComp.Columns.Add("REFUNDED_QTY", GetType(Integer))
        End If

        If Not dtRefundLineItemsComp.Columns.Contains("REFUND_AMOUNT") Then
            dtRefundLineItemsComp.Columns.Add("REFUND_AMOUNT", GetType(Decimal))
        End If

        If Not dtRefundLineItemsComp.Columns.Contains("ITEM_STATUS") Then
            dtRefundLineItemsComp.Columns.Add("ITEM_STATUS", GetType(String))
        End If

        If Not dtRefundLineItemsComp.Columns.Contains("ITEM_RETURNED") Then
            dtRefundLineItemsComp.Columns.Add("ITEM_RETURNED", GetType(String))
        End If

        '=========================================================
        ' BUILD FULFILLMENT MAP
        '=========================================================
        Dim fulfillmentMap As New Dictionary(Of String, Tuple(Of String, String))

        For Each f As DataRow In dtFulfillments.Rows

            Dim lineItemId As String = f("LINE_ITEM_ID").ToString()

            Dim fulfillDate As String = ""

            If Not IsDBNull(f("UPDATED_AT")) Then
                fulfillDate = f("UPDATED_AT").ToString()
            ElseIf Not IsDBNull(f("CREATED_AT")) Then
                fulfillDate = f("CREATED_AT").ToString()
            End If

            Dim status As String = f("STATUS").ToString()

            If status = "success" Then
                status = "fulfilled"
            End If

            If Not fulfillmentMap.ContainsKey(lineItemId) Then
                fulfillmentMap.Add(lineItemId, Tuple.Create(fulfillDate, status))
            End If

        Next

        '=========================================================
        ' PRICE MAP
        '=========================================================
        Dim lineItemPriceMap As New Dictionary(Of String, Decimal)
        Dim lineItemQtyMap As New Dictionary(Of String, Integer)

        For Each li As DataRow In dtLineItems.Rows

            Dim lineItemId As String = li("ID").ToString()

            Dim price As Decimal = 0D
            Decimal.TryParse(li("PRICE").ToString(), price)

            Dim qty As Integer = 0
            Integer.TryParse(li("QUANTITY").ToString(), qty)

            If Not lineItemPriceMap.ContainsKey(lineItemId) Then
                lineItemPriceMap.Add(lineItemId, price)
            End If

            If Not lineItemQtyMap.ContainsKey(lineItemId) Then
                lineItemQtyMap.Add(lineItemId, qty)
            End If

        Next

        '=========================================================
        ' REFUND MAP
        '=========================================================
        Dim refundMap As New Dictionary(Of String, RefundInfo)

        For Each rli As DataRow In dtRefundLineItems.Rows

            Dim refundId As String = rli("REFUND_ID").ToString()
            Dim lineItemId As String = rli("LINE_ITEM_ID").ToString()

            If String.IsNullOrWhiteSpace(lineItemId) Then
                Continue For
            End If

            Dim refundRows() As DataRow =
                dtRefunds.Select("REFUND_ID='" & refundId.Replace("'", "''") & "'")

            If refundRows.Length = 0 Then
                Continue For
            End If

            Dim refundRow As DataRow = refundRows(0)

            Dim refundDate As String = refundRow("CREATED_AT").ToString()

            Dim orderID As String = refundRow("ORDER_ID").ToString()

            Dim transactionRows() As DataRow =
                dtRefundTransactions.Select("REFUND_ID='" & refundId.Replace("'", "''") & "'")

            Dim hasSuccessfulTx As Boolean = False
            Dim hasFailedTx As Boolean = False

            Dim noTransaction As Boolean = False
            If transactionRows.Length = 0 Then noTransaction = True

            Dim sourceName As String = ""

            If transactionRows.Length > 0 Then

                sourceName = transactionRows(0)("SOURCE_NAME").ToString()

                hasSuccessfulTx =
                    transactionRows.Any(Function(t)
                                            Return t("KIND").ToString().ToLower() = "refund" AndAlso
                                                   t("STATUS").ToString().ToLower() = "success"
                                        End Function)

                hasFailedTx =
                    transactionRows.All(Function(t)
                                            Dim s As String = t("STATUS").ToString().ToLower()
                                            Return s = "failure" OrElse s = "error"
                                        End Function)

            End If

            Dim onlineSources As New List(Of String) From {"web", "1830279", "shopify_draft_order", "pos"}

            Dim sourceNameLower As String = sourceName.Trim().ToLower()

            Dim storeReturn As Boolean = False

            If Not String.IsNullOrWhiteSpace(sourceNameLower) Then
                storeReturn = Not onlineSources.Contains(sourceNameLower)
            End If

            If Not refundMap.ContainsKey(lineItemId) Then
                refundMap.Add(lineItemId, New RefundInfo())
            End If

            Dim info As RefundInfo = refundMap(lineItemId)

            Dim qty As Integer = 0
            Integer.TryParse(rli("QUANTITY").ToString(), qty)

            Dim amount As Decimal = 0D
            Decimal.TryParse(rli("SUBTOTAL").ToString(), amount)

            If amount = 0D AndAlso lineItemPriceMap.ContainsKey(lineItemId) Then
                amount = lineItemPriceMap(lineItemId) * qty
            End If

            Dim restockType As String = rli("RESTOCK_TYPE").ToString()

            info.RestockType = restockType

            info.OrderID = orderID

            If restockType = "no_restock" OrElse restockType = "cancel" Then
                If Not storeReturn Then
                    info.IsRemoval = True
                End If
            End If

            If noTransaction Then

                info.ReturnDate = refundDate
                info.ReturnQty += qty
                info.ReturnType = "Online"
                info.IsReturn = True

            ElseIf hasFailedTx Then

                info.RefundDate = refundDate
                info.RefundQty += qty
                info.RefundAmount += amount
                info.RefundFailed = True

                If storeReturn Then
                    info.ReturnType = "Store"
                End If

            ElseIf hasSuccessfulTx Then

                If storeReturn Then

                    info.ReturnDate = refundDate
                    info.ReturnQty += qty
                    info.ReturnType = "Store"

                    info.RefundDate = refundDate
                    info.RefundQty += qty
                    info.RefundAmount += amount

                    info.IsReturn = False

                Else

                    info.RefundDate = refundDate
                    info.RefundQty += qty
                    info.RefundAmount += amount
                    info.IsReturn = False

                    If String.IsNullOrWhiteSpace(info.ReturnType) Then
                        info.ReturnType = "Online"
                    End If

                End If

            End If

        Next

        '=========================================================
        ' 2ND PASS:
        ' refund_line_items empty but successful refund transaction exists
        '=========================================================
        For Each refundRow As DataRow In dtRefunds.Rows

            Dim refundId As String = refundRow("REFUND_ID").ToString()
            Dim orderId As String = refundRow("ORDER_ID").ToString()

            Dim refundDate As String = ""

            If Not IsDBNull(refundRow("CREATED_AT")) Then
                refundDate = refundRow("CREATED_AT").ToString()
            End If

            ' Transactions for this refund
            Dim transactionRows() As DataRow = dtRefundTransactions.Select("REFUND_ID='" & refundId.Replace("'", "''") & "'")

            ' Check successful refund transaction
            Dim hasSuccessfulTx As Boolean = transactionRows.Any(Function(t)
                                                                     Return t("KIND").ToString().ToLower() = "refund" AndAlso t("STATUS").ToString().ToLower() = "success"
                                                                 End Function)

            ' Check if refund has refund_line_items
            Dim refundLineRows() As DataRow = dtRefundLineItems.Select("REFUND_ID='" & refundId.Replace("'", "''") & "'")

            Dim hasLineItems As Boolean = refundLineRows.Length > 0

            If Not hasSuccessfulTx OrElse hasLineItems Then
                Continue For
            End If

            ' Loop through refundMap
            For Each kvp In refundMap

                Dim lineItemId As String = kvp.Key
                Dim info As RefundInfo = kvp.Value

                If info.IsReturn AndAlso info.RestockType = "return" And info.OrderID = orderId Then
                    'If info.IsReturn AndAlso info.RestockType = "return" Then

                    Dim unitPrice As Decimal = 0D

                    If lineItemPriceMap.ContainsKey(lineItemId) Then
                        unitPrice = lineItemPriceMap(lineItemId)
                    End If

                    Dim refundAmount As Decimal = unitPrice * info.ReturnQty

                    info.IsReturn = False
                    info.RefundDate = refundDate
                    info.RefundQty = info.ReturnQty
                    info.RefundAmount = refundAmount

                    If String.IsNullOrWhiteSpace(info.ReturnType) Then
                        info.ReturnType = "Online"
                    End If

                End If

            Next

        Next

        '=========================================================
        ' UPDATE RESULT TABLE
        '=========================================================
        For Each row As DataRow In dtRefundLineItemsComp.Rows

            Dim lineItemId As String = row("LINE_ITEM_ID").ToString()

            Dim info As RefundInfo = Nothing

            If refundMap.ContainsKey(lineItemId) Then
                info = refundMap(lineItemId)
            Else
                info = New RefundInfo()
            End If

            row("RETURN_DATE") = info.ReturnDate
            row("RETURN_QTY") = info.ReturnQty
            row("RETURN_TYPE") = info.ReturnType
            row("REFUND_DATE") = info.RefundDate
            row("REFUNDED_QTY") = info.RefundQty
            row("REFUND_AMOUNT") = info.RefundAmount

            Dim fulfillDate As String = ""
            Dim fulfillStatus As String = "unfulfilled"

            If fulfillmentMap.ContainsKey(lineItemId) Then
                fulfillDate = fulfillmentMap(lineItemId).Item1
                fulfillStatus = fulfillmentMap(lineItemId).Item2
            End If

            Dim qty As Integer = 0

            If lineItemQtyMap.ContainsKey(lineItemId) Then
                qty = lineItemQtyMap(lineItemId)
            End If

            Dim itemStatus As String =
                ComputeItemStatus(
                    False,
                    fulfillStatus,
                    fulfillDate,
                    qty,
                    info.RefundQty,
                    info.ReturnQty,
                    info.IsRemoval,
                    info.RefundFailed,
                    info.IsReturn
                )

            row("ITEM_STATUS") = itemStatus

            Dim itemReturned As String = ""

            If itemStatus = "refund_failed" Then
                itemReturned = "No"
            ElseIf itemStatus = "returned" OrElse
                   itemStatus = "refunded" OrElse
                   itemStatus = "partially_refunded" Then

                itemReturned = "Yes"
            End If

            row("ITEM_RETURNED") = itemReturned

        Next

        Return dtRefundLineItemsComp

    End Function

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Try
            Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
            Dim dt As DataTable
            Dim strQuery As String

            mclsOra.OpenDB()
            strQuery = "SELECT NOTES,O.ORDER_NAME,TO_CHAR(ORDER_DATE,'DD-MON-YYYY') ORDER_DATE,O.SKU,VARIANT_TITLE VARIANT,QUANTITY QTY,PRICE,LINE_TOTAL,RETURN_QTY,
                        TO_CHAR(RETURN_DATE,'DD-MON-YYYY')RETURN_DATE,O.ORDER_ID
                        FROM XXASH_IPK_SHOPIFY_ORDERS O LEFT OUTER JOIN XXASH_IPK_SHOPIFY_NOTES C ON O.ORDER_ID=C.ORDER_ID
                        AND O.ORDER_NAME=C.ORDER_NAME AND O.SKU=C.SKU where O.order_name='" & txtSearch.Text.Trim() & "'"
            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            If dt.Rows.Count <> 0 Then
                gvOrderDetail.DataSource = dt
                gvOrderDetail.DataBind()

                Session("dtOrderDetail") = dt
            Else
                gvOrderDetail.DataSource = Nothing
                gvOrderDetail.DataBind()

                Session("dtOrderDetail") = Nothing
            End If
            mclsOra.CloseDB()

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub chkSelectAll_CheckedChanged(sender As Object, e As EventArgs)
        Dim chkSelectAll As CheckBox = TryCast(sender, CheckBox)

        For Each row As GridViewRow In gvOrderDetail.Rows
            Dim chk As CheckBox = TryCast(row.FindControl("chkSelect"), CheckBox)
            If chk IsNot Nothing Then
                chk.Checked = chkSelectAll.Checked
            End If
        Next

    End Sub

    Protected Sub chkSelect_CheckedChanged(sender As Object, e As EventArgs)
        'If Session("SelectedUserGroupID") IsNot Nothing Then
        'End If
    End Sub

    Private Function GetRetailPRO_IpekyolOnlineSales() As DataTable
        Try
            Dim dt As DataTable, strQuery As String = ""
            Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
            mclsOra.OpenDB()

            'strQuery = "SELECT S.*,S.SALES_QTY-S.RETURN_QTY RP_NET_QTY,S.SALES_AMOUNT-S.RETURN_AMOUNT RP_NET_AMOUNT FROM
            '            (SELECT 
            '                SALLA_DOC_NO,
            '                ALU,0 ORDER_QTY,0 ORDER_AMOUNT,
            '                SUM(CASE WHEN INVOICE_TYPE = 'Sale' 
            '                         THEN ITEM_ORDER_QTY ELSE 0 END) AS SALES_QTY,
            '                SUM(CASE WHEN INVOICE_TYPE = 'Sale' 
            '                         THEN ITEM_ORDER_QTY * UNIT_SELLING_PRICE ELSE 0 END) AS SALES_AMOUNT,
            '                SUM(CASE WHEN INVOICE_TYPE = 'Return' 
            '                         THEN ABS(ITEM_ORDER_QTY) ELSE 0 END) AS RETURN_QTY,
            '                SUM(CASE WHEN INVOICE_TYPE = 'Return' 
            '                         THEN ABS(ITEM_ORDER_QTY * UNIT_SELLING_PRICE) ELSE 0 END) AS RETURN_AMOUNT
            '            FROM XXASH_RP_ORDER_STATUS_BYITEM
            '            WHERE SUBSIDIARY_NO = 4
            '              AND SALLA_DOC_NO LIKE '%IPK%'
            '              AND INVOICE_TYPE IN ('Sale','Return')        
            '              AND UPC <> '64269' 
            '            GROUP BY SALLA_DOC_NO, ALU
            '        ORDER BY SALLA_DOC_NO)S    
            '    UNION ALL    
            '        SELECT S.*,S.ORDER_QTY RP_NET_QTY,S.ORDER_AMOUNT RP_NET_AMOUNT FROM
            '            (SELECT 
            '                SALLA_DOC_NO,
            '                ALU,
            '                SUM(CASE WHEN INVOICE_TYPE = 'Order' 
            '                         THEN ITEM_ORDER_QTY ELSE 0 END) AS ORDER_QTY,
            '                SUM(CASE WHEN INVOICE_TYPE = 'Order' 
            '                         THEN ITEM_ORDER_QTY * UNIT_SELLING_PRICE ELSE 0 END) AS ORDER_AMOUNT,
            '                         0 SALES_QTY,0 SALES_AMOUNT,0 RETURN_QTY,0 RETURN_AMOUNT           
            '            FROM XXASH_RP_ORDER_STATUS_BYITEM
            '            WHERE SUBSIDIARY_NO = 4
            '              AND SALLA_DOC_NO LIKE '%IPK%'          
            '            AND INVOICE_TYPE IN ('Order') and ORDER_STATUS='Pending'
            '              AND UPC <> '64269' 
            '            GROUP BY SALLA_DOC_NO, ALU
            '        ORDER BY SALLA_DOC_NO)S"

            strQuery = "SELECT * FROM XXASH_IPK_ORDER_STATUS_V"

            dt = mclsOra.GetDataSet(strQuery).Tables(0)
            mclsOra.CloseDB()

            Return dt

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Protected Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtSearch.Text = ""
        txtFilter.Text = ""
        txtNote.Text = ""
        gvOrderDetail.DataSource = Nothing
        gvOrderDetail.DataBind()
    End Sub

    Private Function JoinIpekyolandRetailPROTables(dtShopify As DataTable, dtRetailPRO As DataTable, dtIPKOrderNotes As DataTable) As DataTable
        Try
            ' Build final joined datatable: dtFinal = dtShopify + RetailPRO columns joined on ORDER_NAME=SALLA_DOC_NO and SKU=ALU
            Dim dtFinal As DataTable = dtShopify.Clone()
            If Not dtFinal.Columns.Contains("RP_ORDER_QTY") Then dtFinal.Columns.Add("RP_ORDER_QTY", GetType(Integer))
            If Not dtFinal.Columns.Contains("RP_ORDER_AMOUNT") Then dtFinal.Columns.Add("RP_ORDER_AMOUNT", GetType(Decimal))
            If Not dtFinal.Columns.Contains("RP_SALES_QTY") Then dtFinal.Columns.Add("RP_SALES_QTY", GetType(Integer))
            If Not dtFinal.Columns.Contains("RP_SALES_AMOUNT") Then dtFinal.Columns.Add("RP_SALES_AMOUNT", GetType(Decimal))
            If Not dtFinal.Columns.Contains("RP_RETURN_QTY") Then dtFinal.Columns.Add("RP_RETURN_QTY", GetType(Integer))
            If Not dtFinal.Columns.Contains("RP_RETURN_AMOUNT") Then dtFinal.Columns.Add("RP_RETURN_AMOUNT", GetType(Decimal))
            If Not dtFinal.Columns.Contains("RP_NET_QTY") Then dtFinal.Columns.Add("RP_NET_QTY", GetType(Integer))
            If Not dtFinal.Columns.Contains("RP_NET_AMOUNT") Then dtFinal.Columns.Add("RP_NET_AMOUNT", GetType(Decimal))

            If Not dtFinal.Columns.Contains("NET_QTY_STATUS") Then dtFinal.Columns.Add("NET_QTY_STATUS", GetType(String))
            If Not dtFinal.Columns.Contains("NET_AMOUNT_STATUS") Then dtFinal.Columns.Add("NET_AMOUNT_STATUS", GetType(String))

            If Not dtFinal.Columns.Contains("NOTES") Then dtFinal.Columns.Add("NOTES", GetType(String))

            ' Build lookup from RetailPRO: key = SALLA_DOC_NO + "|" + ALU (normalized)
            Dim rpLookup As New Dictionary(Of String, DataRow)(StringComparer.OrdinalIgnoreCase)
            If dtRetailPRO IsNot Nothing Then
                For Each r As DataRow In dtRetailPRO.Rows
                    Dim salla = Convert.ToString(r("SALLA_DOC_NO")).Trim()
                    Dim alu = Convert.ToString(r("ALU")).Trim()
                    Dim key As String = String.Format("{0}|{1}", salla, alu)
                    If Not rpLookup.ContainsKey(key) Then
                        rpLookup.Add(key, r)
                    End If
                Next
            End If

            ' Build lookup for comments: key = ORDER_NAME + "|" + SKU
            Dim noteLookup As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            If dtIPKOrderNotes IsNot Nothing Then
                For Each cRow As DataRow In dtIPKOrderNotes.Rows
                    Dim oName As String = Convert.ToString(cRow("ORDER_NAME")).Trim()
                    Dim sku As String = Convert.ToString(cRow("SKU")).Trim()
                    Dim note As String = If(IsDBNull(cRow("NOTES")), String.Empty, Convert.ToString(cRow("NOTES")))
                    Dim key As String = String.Format("{0}|{1}", oName, sku)
                    If Not NoteLookup.ContainsKey(key) Then
                        NoteLookup.Add(key, note)
                    End If
                Next
            End If

            ' Helper lambdas (same parsing rules as used elsewhere)
            Dim inv = CultureInfo.InvariantCulture
            Dim ToDecimal = Function(obj As Object) As Decimal
                                If obj Is Nothing OrElse obj Is DBNull.Value Then Return 0D
                                Dim s = Convert.ToString(obj)
                                Dim d As Decimal
                                If Decimal.TryParse(s, NumberStyles.Any, inv, d) Then Return d
                                Return 0D
                            End Function

            Dim ToInt = Function(obj As Object) As Integer
                            If obj Is Nothing OrElse obj Is DBNull.Value Then Return 0
                            Dim s = Convert.ToString(obj)
                            Dim i As Integer
                            If Integer.TryParse(s, NumberStyles.Any, inv, i) Then Return i
                            Return 0
                        End Function

            ' Populate dtFinal by iterating shopify rows and appending matched RP values (or defaults)
            For Each s As DataRow In dtShopify.Rows
                Dim newRow As DataRow = dtFinal.NewRow()
                ' copy all shopify columns
                For Each c As DataColumn In dtShopify.Columns
                    newRow(c.ColumnName) = If(IsDBNull(s(c.ColumnName)), DBNull.Value, s(c.ColumnName))
                Next

                ' lookup retailpro (normalize keys)
                Dim lookupKey As String = String.Format("{0}|{1}", Convert.ToString(s("ORDER_NAME")).Trim(), Convert.ToString(s("SKU")).Trim())
                If rpLookup.ContainsKey(lookupKey) Then
                    Dim rp As DataRow = rpLookup(lookupKey)
                    newRow("RP_ORDER_QTY") = If(IsDBNull(rp("ORDER_QTY")), 0, Convert.ToInt32(rp("ORDER_QTY")))
                    newRow("RP_ORDER_AMOUNT") = If(IsDBNull(rp("ORDER_AMOUNT")), 0D, Convert.ToDecimal(rp("ORDER_AMOUNT")))
                    newRow("RP_SALES_QTY") = If(IsDBNull(rp("SALES_QTY")), 0, Convert.ToInt32(rp("SALES_QTY")))
                    newRow("RP_SALES_AMOUNT") = If(IsDBNull(rp("SALES_AMOUNT")), 0D, Convert.ToDecimal(rp("SALES_AMOUNT")))
                    newRow("RP_RETURN_QTY") = If(IsDBNull(rp("RETURN_QTY")), 0, Convert.ToInt32(rp("RETURN_QTY")))
                    newRow("RP_RETURN_AMOUNT") = If(IsDBNull(rp("RETURN_AMOUNT")), 0D, Convert.ToDecimal(rp("RETURN_AMOUNT")))
                    newRow("RP_NET_QTY") = If(IsDBNull(rp("RP_NET_QTY")), 0, Convert.ToInt32(rp("RP_NET_QTY")))
                    newRow("RP_NET_AMOUNT") = If(IsDBNull(rp("RP_NET_AMOUNT")), 0D, Convert.ToDecimal(rp("RP_NET_AMOUNT")))

                    ' Compute status comparisons
                    Dim shopNetQty As Integer = ToInt(s("NET_QTY"))
                    Dim rpNetQty As Integer = ToInt(rp("RP_NET_QTY"))
                    newRow("NET_QTY_STATUS") = If(shopNetQty = rpNetQty, "Matched", "Unmatched")

                    Dim shopNetAmount As Decimal = ToDecimal(s("NET_AMOUNT"))
                    Dim rpNetAmount As Decimal = ToDecimal(rp("RP_NET_AMOUNT"))
                    ' allow small rounding differences (1 cent)
                    Dim amtMatched As Boolean = Math.Abs(shopNetAmount - rpNetAmount) <= 0.01D
                    newRow("NET_AMOUNT_STATUS") = If(amtMatched, "Matched", "Unmatched")
                Else
                    ' defaults when no match
                    newRow("RP_ORDER_QTY") = 0
                    newRow("RP_ORDER_AMOUNT") = 0D
                    newRow("RP_SALES_QTY") = 0
                    newRow("RP_SALES_AMOUNT") = 0D
                    newRow("RP_RETURN_QTY") = 0
                    newRow("RP_RETURN_AMOUNT") = 0D
                    newRow("RP_NET_QTY") = 0
                    newRow("RP_NET_AMOUNT") = 0D

                    If UCase(s("ORDER_STATUS")) = "PENDING" Then
                        newRow("NET_QTY_STATUS") = "Matched"
                        newRow("NET_AMOUNT_STATUS") = "Matched"
                    Else
                        Dim shopNetQty As Integer = ToInt(s("NET_QTY"))
                        Dim rpNetQty As Integer = 0
                        newRow("NET_QTY_STATUS") = If(shopNetQty = rpNetQty, "Matched", "Unmatched")

                        Dim shopNetAmount As Decimal = ToDecimal(s("NET_AMOUNT"))
                        Dim rpNetAmount As Decimal = 0D
                        Dim amtMatched As Boolean = Math.Abs(shopNetAmount - rpNetAmount) <= 0.01D
                        newRow("NET_AMOUNT_STATUS") = If(amtMatched, "Matched", "Unmatched")

                    End If

                End If

                ' lookup note
                Dim noteVal As String = ""
                If NoteLookup.Count > 0 Then
                    Dim cKey As String = lookupKey
                    If NoteLookup.ContainsKey(cKey) Then
                        noteVal = NoteLookup(cKey)
                    End If
                End If
                newRow("NOTES") = noteVal

                dtFinal.Rows.Add(newRow)
            Next

            Return dtFinal

        Catch ex As Exception
            Throw ex
        End Try

    End Function




    Private Function ComputeItemStatus(isCancelled As Boolean, fulfillStatus As String, fulfillDate As DateTime, qty As Integer, refundQty As Integer, refundAmount As Decimal, isRemoval As Boolean, Optional returnQty As Integer = 0, Optional isReturn As Boolean = False, Optional refundFailed As Boolean = False) As String
        Try
            If isCancelled Then
                Return "cancelled"
            End If

            Dim fulfilled As Boolean = False

            If Not String.IsNullOrEmpty(fulfillStatus) AndAlso String.Equals(fulfillStatus, "fulfilled", StringComparison.OrdinalIgnoreCase) Then
                fulfilled = True
            End If

            If Not String.IsNullOrEmpty(fulfillStatus) AndAlso String.Equals(fulfillStatus, "success", StringComparison.OrdinalIgnoreCase) Then
                fulfilled = True
            End If

            If refundQty > 0 AndAlso refundFailed Then
                Return "refund_failed"
            End If

            If refundQty > 0 AndAlso Not refundFailed Then
                If fulfilled AndAlso refundQty >= qty Then
                    Return "refunded"
                End If
                If fulfilled AndAlso refundQty < qty Then
                    Return "partially_refunded"
                End If
            End If

            If returnQty > 0 AndAlso isReturn Then
                Return "returned"
            End If

            If Not fulfilled AndAlso (refundQty > 0 OrElse isRemoval) Then
                Return "removed"
            End If

            If fulfilled Then
                Return "fulfilled"
            End If


            Return "unfulfilled"
        Catch ex As Exception
            ' in case of unexpected data, fall back to unfulfilled
            Return "unfulfilled"
        End Try
    End Function

    Protected Sub btnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
        Try

            If Not Session("dtOrderDetail") Is Nothing Then

                Dim dv As DataView
                dv = DirectCast(Session("dtOrderDetail"), DataTable).DefaultView
                dv.RowFilter = "SKU like '" & txtFilter.Text & "%'"

                gvOrderDetail.DataSource = dv
                gvOrderDetail.DataBind()

                Session("dtOrderDetail") = dv.ToTable

            End If
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub btnUpdateNote_Click(sender As Object, e As EventArgs) Handles btnUpdateNote.Click
        Try

            If Not Session("dtOrderDetail") Is Nothing Then
                For Each row As GridViewRow In gvOrderDetail.Rows

                    Dim chk As CheckBox = TryCast(row.FindControl("chkSelect"), CheckBox)
                    If chk.Checked Then
                        row.Cells(1).Text = txtNote.Text.Trim

                        UpdateNote(txtNote.Text.Trim, row.Cells(11).Text.Trim, row.Cells(2).Text.Trim, row.Cells(4).Text.Trim)
                    End If

                Next

            End If
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub UpdateNote(strNote As String, strOrderID As String, strOrderName As String, strSKU As String)
        Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
        Try
            mclsOra.OpenDB()

            Dim strChk As String = "SELECT COUNT(*) CNT FROM XXASH_IPK_SHOPIFY_NOTES WHERE ORDER_ID='" & strOrderID & "' AND ORDER_NAME='" & strOrderName & "' AND SKU='" & strSKU & "'"
            Dim dtChk As DataTable = mclsOra.GetDataSet(strChk).Tables(0)
            Dim cnt As Integer = 0
            If dtChk.Rows.Count > 0 Then
                Integer.TryParse(dtChk.Rows(0)("CNT").ToString(), cnt)
            End If

            Dim strUsername As String = TryCast(Session("username"), String)

            Dim strSql As String = ""
            If cnt > 0 Then
                strSql = "UPDATE XXASH_IPK_SHOPIFY_NOTES SET NOTES='" & strNote & "', UPDATED_DATE=SYSDATE,UPDATED_BY='" & strUsername & "' 
                          WHERE ORDER_ID='" & strOrderID & "' AND ORDER_NAME='" & strOrderName & "' AND SKU='" & strSKU & "'"
            Else
                strSql = "INSERT INTO XXASH_IPK_SHOPIFY_NOTES(ORDER_ID,ORDER_NAME,SKU,NOTES,UPDATED_DATE,UPDATED_BY) VALUES(" &
                         "'" & strOrderID & "','" & strOrderName & "','" & strSKU & "','" & strNote & "',SYSDATE,'" & strUsername & "')"
            End If

            mclsOra.ExecuteNonQuery(strSql)

            ShowMessageAlert(Me, "Note updated successfully.", "success")

        Catch ex As Exception
            Throw ex
        Finally
            mclsOra.CloseDB()
        End Try

    End Sub

    Private Sub ddlStore_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlStore.SelectedIndexChanged
        Session("store_code") = Mid(ddlStore.SelectedValue, 1, InStr(ddlStore.SelectedValue, "-") - 1)
    End Sub

    Private Function ComputeItemStatus(
    isCancelled As Boolean,
    fulfillStatus As String,
    fulfillDate As String,
    qty As Integer,
    refundQty As Integer,
    returnQty As Integer,
    isRemoval As Boolean,
    refundFailed As Boolean,
    isReturn As Boolean
    ) As String

        If isCancelled Then
            Return "cancelled"
        End If

        Dim fulfilled As Boolean = Not String.IsNullOrWhiteSpace(fulfillDate) OrElse fulfillStatus = "fulfilled"

        If refundQty > 0 AndAlso refundFailed Then
            Return "refund_failed"
        End If

        If refundQty > 0 AndAlso Not refundFailed Then

            If fulfilled AndAlso refundQty >= qty Then
                Return "refunded"
            End If

            If fulfilled AndAlso refundQty < qty Then
                Return "partially_refunded"
            End If

        End If

        If returnQty > 0 AndAlso isReturn Then
            Return "returned"
        End If

        If Not fulfilled AndAlso (refundQty > 0 OrElse isRemoval) Then
            Return "removed"
        End If

        If fulfilled Then
            Return "fulfilled"
        End If

        Return "unfulfilled"

    End Function

    Private Sub gvOrderDetail_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles gvOrderDetail.RowEditing
        Try
            gvOrderDetail.EditIndex = e.NewEditIndex
            ' rebind to put GridView in edit mode; btnSearch_Click will re-query using txtSearch
            btnSearch_Click(Nothing, Nothing)
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub gvOrderDetail_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles gvOrderDetail.RowCancelingEdit
        Try
            gvOrderDetail.EditIndex = -1
            btnSearch_Click(Nothing, Nothing)
        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Protected Sub gvOrderDetail_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles gvOrderDetail.RowUpdating
        Try
            Dim row As GridViewRow = gvOrderDetail.Rows(e.RowIndex)

            Dim txtComments As TextBox = CType(row.FindControl("txtComments"), TextBox)

            If txtComments Is Nothing Then
                ShowMessageAlert(Me, "Comments control not found. Ensure GridView uses a TemplateField with TextBox ID='txtComments' in EditItemTemplate.", "error")
                Return
            End If

            Dim comments As String = txtComments.Text.Trim()

            Dim orderId As String = Convert.ToString(gvOrderDetail.DataKeys(e.RowIndex).Values("ORDER_ID"))
            Dim orderName As String = Convert.ToString(gvOrderDetail.DataKeys(e.RowIndex).Values("ORDER_NAME"))
            Dim sku As String = Convert.ToString(gvOrderDetail.DataKeys(e.RowIndex).Values("SKU"))

            Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
            Try
                mclsOra.OpenDB()

                ' sanitize single quotes for direct SQL (consistent with existing code style)
                Dim sOrderId As String = orderId.Replace("'", "''")
                Dim sOrderName As String = orderName.Replace("'", "''")
                Dim sSku As String = sku.Replace("'", "''")
                Dim sComments As String = comments.Replace("'", "''")

                Dim strChk As String = "SELECT COUNT(*) CNT FROM XXASH_IPK_SHOPIFY_COMMENTS WHERE ORDER_ID='" & sOrderId & "' AND ORDER_NAME='" & sOrderName & "' AND SKU='" & sSku & "'"
                Dim dtChk As DataTable = mclsOra.GetDataSet(strChk).Tables(0)
                Dim cnt As Integer = 0
                If dtChk.Rows.Count > 0 Then
                    Integer.TryParse(dtChk.Rows(0)("CNT").ToString(), cnt)
                End If

                Dim strSql As String = ""
                If cnt > 0 Then
                    strSql = "UPDATE XXASH_IPK_SHOPIFY_COMMENTS SET COMMENTS='" & sComments & "', UPDATED_DATE=SYSDATE " &
                         "WHERE ORDER_ID='" & sOrderId & "' AND ORDER_NAME='" & sOrderName & "' AND SKU='" & sSku & "'"
                Else
                    strSql = "INSERT INTO XXASH_IPK_SHOPIFY_COMMENTS(ORDER_ID,ORDER_NAME,SKU,COMMENTS,UPDATED_DATE) VALUES(" &
                         "'" & sOrderId & "','" & sOrderName & "','" & sSku & "','" & sComments & "',SYSDATE)"
                End If

                mclsOra.ExecuteNonQuery(strSql)
            Finally
                mclsOra.CloseDB()
            End Try

            ' exit edit mode and rebind to show updated comments
            gvOrderDetail.EditIndex = -1
            btnSearch_Click(Nothing, Nothing)

        Catch ex As Exception
            ShowMessageAlert(Me, ex.Message, "error")
        End Try
    End Sub

    Private Sub gvOrderDetail_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvOrderDetail.RowDataBound
        e.Row.Cells(1).Wrap = False
        e.Row.Cells(5).Wrap = False
        e.Row.Cells(11).Visible = False
    End Sub
    Private Class RefundEntry
        Public Property ReturnDate As String = ""
        Public Property ReturnQty As Integer = 0
        Public Property ReturnType As String = ""
        Public Property RefundDate As String = ""
        Public Property RefundQty As Integer = 0
        Public Property RefundAmount As Decimal = 0
        Public Property IsRemoval As Boolean = False
        Public Property RefundFailed As Boolean = False
        Public Property IsReturn As Boolean = False
    End Class

    Public Class RefundInfo

        Public Property ReturnDate As String = ""
        Public Property ReturnQty As Integer = 0
        Public Property ReturnType As String = ""

        Public Property RefundDate As String = ""
        Public Property RefundQty As Integer = 0
        Public Property RefundAmount As Decimal = 0D

        Public Property IsRemoval As Boolean = False
        Public Property RefundFailed As Boolean = False
        Public Property IsReturn As Boolean = False

        Public Property RestockType As String = ""

        Public Property OrderID As String = ""

    End Class

End Class