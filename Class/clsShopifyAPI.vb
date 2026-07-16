Imports System.IO
Imports System.Net
Imports Newtonsoft.Json

Public Class clsShopifyAPI

    Public Function DownloadOrders(createdAtMin As DateTime, createdAtMax As DateTime, accessToken As String, Optional shopDomain As String = "ipekyol-ksa-ae.myshopify.com") As DataSet
        Dim ds As New DataSet("ShopifyOrders")

        ' Orders table
        Dim dtOrders As New DataTable("Orders")
        dtOrders.Columns.Add("id", GetType(String))
        dtOrders.Columns.Add("name", GetType(String))
        dtOrders.Columns.Add("order_number", GetType(Integer))
        dtOrders.Columns.Add("created_at", GetType(DateTime))
        dtOrders.Columns.Add("email", GetType(String))
        dtOrders.Columns.Add("total_price", GetType(String))
        dtOrders.Columns.Add("total_tax", GetType(String))
        dtOrders.Columns.Add("total_shipping_price", GetType(String))
        dtOrders.Columns.Add("currency", GetType(String))
        dtOrders.Columns.Add("financial_status", GetType(String))
        dtOrders.Columns.Add("fulfillment_status", GetType(String))
        dtOrders.Columns.Add("status", GetType(String))
        dtOrders.Columns.Add("phone", GetType(String))
        dtOrders.Columns.Add("customer_id", GetType(String))
        dtOrders.Columns.Add("shipping_name", GetType(String))
        dtOrders.Columns.Add("line_items_count", GetType(Integer))
        dtOrders.Columns.Add("updated_at", GetType(DateTime))
        dtOrders.Columns.Add("cancelled_at", GetType(DateTime))
        dtOrders.Columns.Add("cancel_reason", GetType(String))

        ' Line items table
        Dim dtLineItems As New DataTable("LineItems")
        dtLineItems.Columns.Add("order_id", GetType(String))
        dtLineItems.Columns.Add("id", GetType(String))
        dtLineItems.Columns.Add("product_id", GetType(String))
        dtLineItems.Columns.Add("sku", GetType(String))
        dtLineItems.Columns.Add("name", GetType(String))
        dtLineItems.Columns.Add("current_quantity", GetType(Integer))
        dtLineItems.Columns.Add("fulfillable_quantity", GetType(Integer))
        dtLineItems.Columns.Add("quantity", GetType(Integer))
        dtLineItems.Columns.Add("price", GetType(String))
        dtLineItems.Columns.Add("total_discount", GetType(String))
        dtLineItems.Columns.Add("variant_id", GetType(String))
        dtLineItems.Columns.Add("variant_title", GetType(String))
        dtLineItems.Columns.Add("vendor", GetType(String))
        'dtLineItems.Columns.Add("taxable", GetType(Boolean))
        dtLineItems.Columns.Add("fulfillment_status", GetType(String))

        ' Refunds table
        Dim dtRefunds As New DataTable("Refunds")
        dtRefunds.Columns.Add("refund_id", GetType(String))
        dtRefunds.Columns.Add("admin_graphql_api_id", GetType(String))
        dtRefunds.Columns.Add("created_at", GetType(DateTime))
        dtRefunds.Columns.Add("note", GetType(String))
        dtRefunds.Columns.Add("order_id", GetType(String))
        dtRefunds.Columns.Add("processed_at", GetType(DateTime))
        dtRefunds.Columns.Add("restock", GetType(Boolean))
        dtRefunds.Columns.Add("user_id", GetType(String))
        dtRefunds.Columns.Add("total_duties_amount", GetType(String))

        ' Refund line items table
        Dim dtRefundLineItems As New DataTable("RefundLineItems")
        dtRefundLineItems.Columns.Add("refund_id", GetType(String))
        dtRefundLineItems.Columns.Add("id", GetType(String))
        dtRefundLineItems.Columns.Add("line_item_id", GetType(String))
        dtRefundLineItems.Columns.Add("location_id", GetType(String))
        dtRefundLineItems.Columns.Add("quantity", GetType(Integer))
        dtRefundLineItems.Columns.Add("restock_type", GetType(String))
        dtRefundLineItems.Columns.Add("subtotal", GetType(Double))
        dtRefundLineItems.Columns.Add("total_tax", GetType(Double))
        dtRefundLineItems.Columns.Add("line_item_sku", GetType(String))
        dtRefundLineItems.Columns.Add("line_item_product_id", GetType(String))
        dtRefundLineItems.Columns.Add("line_item_title", GetType(String))
        'dtRefundLineItems.Columns.Add("line_item_name", GetType(String))
        dtRefundLineItems.Columns.Add("variant_title", GetType(String))

        ' Refund transactions table (new)
        Dim dtRefundTransactions As New DataTable("RefundTransactions")
        dtRefundTransactions.Columns.Add("refund_id", GetType(String))
        dtRefundTransactions.Columns.Add("transaction_id", GetType(String))
        dtRefundTransactions.Columns.Add("parent_id", GetType(String))
        dtRefundTransactions.Columns.Add("amount", GetType(Double))
        dtRefundTransactions.Columns.Add("kind", GetType(String))
        dtRefundTransactions.Columns.Add("gateway", GetType(String))
        dtRefundTransactions.Columns.Add("source_name", GetType(String))
        dtRefundTransactions.Columns.Add("status", GetType(String))
        dtRefundTransactions.Columns.Add("created_at", GetType(DateTime))

        ' Fulfillments table (new) - per-fulfillment and per-line-item rows
        Dim dtFulfillments As New DataTable("Fulfillments")
        dtFulfillments.Columns.Add("order_id", GetType(String))
        dtFulfillments.Columns.Add("id", GetType(String)) ' fulfillment id
        dtFulfillments.Columns.Add("updated_at", GetType(DateTime))
        dtFulfillments.Columns.Add("created_at", GetType(DateTime))
        dtFulfillments.Columns.Add("status", GetType(String))
        dtFulfillments.Columns.Add("line_item_id", GetType(String))   ' single line item id (per-row)
        dtFulfillments.Columns.Add("line_item_ids", GetType(String))  ' comma-separated list (fulfillment-level)

        Try
            Dim minUtc = createdAtMin.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
            Dim maxUtc = createdAtMax.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

            Dim url = $"https://{shopDomain}/admin/api/2024-01/orders.json?status=any&limit=250&fulfillment_status=any&created_at_max={Uri.EscapeDataString(maxUtc)}&created_at_min={Uri.EscapeDataString(minUtc)}"
            'Dim url = "https://ipekyol-ksa-ae.myshopify.com/admin/api/2024-01/orders.json?status=any&name=IPK1550"

            ' Create HttpWebRequest (GET)
            Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
            request.Accept = "application/json"
            request.ContentType = "application/json"
            request.Method = "GET"

            ' If access token provided, add header
            If Not String.IsNullOrWhiteSpace(accessToken) Then
                request.Headers.Add("X-Shopify-Access-Token", accessToken)
            End If

            request.ServicePoint.ConnectionLimit = 10
            request.ServicePoint.MaxIdleTime = 5 * 1000
            request.Timeout = 60000
            request.KeepAlive = True

            Dim json As String = String.Empty

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using sr As New StreamReader(response.GetResponseStream(), Encoding.UTF8)
                    json = sr.ReadToEnd()
                End Using
            End Using

            Dim wrapper = JsonConvert.DeserializeObject(Of clsShopifyJsonOrders.Orders)(json)

            If Not (wrapper Is Nothing) AndAlso Not (wrapper.orders Is Nothing) Then
                For Each o In wrapper.orders
                    ' Orders row
                    Dim row = dtOrders.NewRow()
                    Dim orderIdStr As String = If(o.id Is Nothing, String.Empty, o.id.ToString())
                    row("id") = orderIdStr
                    row("name") = If(String.IsNullOrEmpty(o.name), String.Empty, o.name)
                    row("order_number") = o.order_number
                    row("created_at") = o.created_at
                    row("email") = If(String.IsNullOrEmpty(o.email), String.Empty, o.email)
                    row("total_price") = If(String.IsNullOrEmpty(o.total_price), String.Empty, o.total_price)
                    row("total_tax") = If(String.IsNullOrEmpty(o.total_tax), String.Empty, o.total_tax)

                    Dim shipPrice As String = String.Empty
                    If o.total_shipping_price_set IsNot Nothing AndAlso o.total_shipping_price_set.shop_money IsNot Nothing Then
                        shipPrice = If(String.IsNullOrEmpty(o.total_shipping_price_set.shop_money.amount), String.Empty, o.total_shipping_price_set.shop_money.amount)
                    End If
                    row("total_shipping_price") = shipPrice

                    row("currency") = If(String.IsNullOrEmpty(o.currency), String.Empty, o.currency)
                    row("financial_status") = If(String.IsNullOrEmpty(o.financial_status), String.Empty, o.financial_status)
                    row("fulfillment_status") = If(String.IsNullOrEmpty(o.fulfillment_status), String.Empty, o.fulfillment_status)

                    'assuming one fulfillment per order for status - if multiple, will take last one (could be enhanced to concatenate or create separate table if needed)
                    Dim orderStatus As String = String.Empty
                    If o.fulfillments IsNot Nothing Then
                        For Each fulfillstatus In o.fulfillments
                            orderStatus = fulfillstatus.status
                        Next
                    End If

                    row("status") = orderStatus

                    row("phone") = If(o.phone Is Nothing, String.Empty, Convert.ToString(o.phone))
                    row("customer_id") = If(o.customer Is Nothing OrElse o.customer.id Is Nothing, String.Empty, o.customer.id.ToString())

                    Dim shippingName As String = String.Empty
                    If o.shipping_address IsNot Nothing Then
                        shippingName = If(String.IsNullOrEmpty(o.shipping_address.name), String.Empty, o.shipping_address.name)
                    End If
                    row("shipping_name") = shippingName

                    ' line_items_count: compute safely
                    Dim licount As Integer = If(o.line_items Is Nothing, 0, o.line_items.Length)
                    row("line_items_count") = licount
                    row("updated_at") = IIf(IsNothing(o.updated_at), DBNull.Value, o.updated_at)
                    row("cancelled_at") = IIf(IsNothing(o.cancelled_at), DBNull.Value, o.cancelled_at)
                    row("cancel_reason") = o.cancel_reason

                    dtOrders.Rows.Add(row)

                    ' Fulfillments: capture fulfillment rows into dtFulfillments (per fulfillment and per-line)
                    If o.fulfillments IsNot Nothing Then
                        For Each f In o.fulfillments
                            Dim fulId As String = If(f.id Is Nothing, String.Empty, f.id.ToString())
                            Dim fulUpdated = IIf(IsNothing(f.updated_at), DBNull.Value, f.updated_at)
                            Dim fulCreated = IIf(IsNothing(f.created_at), DBNull.Value, f.created_at)
                            Dim fulStatus As String = If(String.IsNullOrEmpty(f.status), String.Empty, f.status)

                            ' collect all line item ids for this fulfillment
                            Dim ids As New System.Collections.Generic.List(Of String)
                            If f.line_items IsNot Nothing Then
                                For Each fil In f.line_items
                                    Dim lid As String = String.Empty

                                    ' Use reflection to support different JSON class shapes:
                                    ' some builds expose "line_item_id", others only "id".
                                    If fil IsNot Nothing Then
                                        Dim t = fil.GetType()
                                        Dim pi = t.GetProperty("line_item_id")
                                        If pi IsNot Nothing Then
                                            Dim v = pi.GetValue(fil, Nothing)
                                            If v IsNot Nothing Then lid = v.ToString()
                                        Else
                                            Dim pi2 = t.GetProperty("id")
                                            If pi2 IsNot Nothing Then
                                                Dim v2 = pi2.GetValue(fil, Nothing)
                                                If v2 IsNot Nothing Then lid = v2.ToString()
                                            End If
                                        End If
                                    End If

                                    If Not String.IsNullOrEmpty(lid) Then
                                        ids.Add(lid)
                                        ' add per-line row (makes lookup by line_item_id straightforward)
                                        Dim per = dtFulfillments.NewRow()
                                        per("order_id") = orderIdStr
                                        per("id") = fulId
                                        per("updated_at") = fulUpdated
                                        per("created_at") = fulCreated
                                        per("status") = fulStatus
                                        per("line_item_id") = lid
                                        per("line_item_ids") = DBNull.Value
                                        dtFulfillments.Rows.Add(per)
                                    End If
                                Next
                            End If

                            ' add a fulfillment-level row containing comma-separated line_item_ids (may be used as fallback)
                            Dim frat = dtFulfillments.NewRow()
                            frat("order_id") = orderIdStr
                            frat("id") = fulId
                            frat("updated_at") = fulUpdated
                            frat("created_at") = fulCreated
                            frat("status") = fulStatus
                            If ids.Count > 0 Then
                                frat("line_item_ids") = String.Join(",", ids.ToArray())
                            Else
                                frat("line_item_ids") = DBNull.Value
                            End If
                            frat("line_item_id") = DBNull.Value
                            dtFulfillments.Rows.Add(frat)
                        Next
                    End If

                    ' Line items rows
                    If o.line_items IsNot Nothing Then
                        For Each li In o.line_items
                            Dim lir = dtLineItems.NewRow()
                            lir("order_id") = orderIdStr
                            lir("id") = If(li.id Is Nothing, String.Empty, li.id.ToString())
                            lir("name") = If(String.IsNullOrEmpty(li.name), String.Empty, li.name)
                            lir("product_id") = If(String.IsNullOrEmpty(li.product_id), String.Empty, li.product_id)
                            lir("sku") = If(String.IsNullOrEmpty(li.sku), String.Empty, li.sku)
                            ' quantity is non-nullable Integer -> assign directly
                            lir("current_quantity") = li.current_quantity
                            lir("fulfillable_quantity") = li.fulfillable_quantity
                            lir("quantity") = li.quantity
                            lir("price") = If(String.IsNullOrEmpty(li.price), String.Empty, li.price)
                            lir("total_discount") = If(String.IsNullOrEmpty(li.total_discount), String.Empty, li.total_discount)
                            lir("variant_id") = If(li.variant_id Is Nothing, String.Empty, li.variant_id.ToString())
                            lir("variant_title") = If(String.IsNullOrEmpty(li.variant_title), String.Empty, li.variant_title)
                            lir("vendor") = If(String.IsNullOrEmpty(li.vendor), String.Empty, li.vendor)
                            ' taxable is non-nullable Boolean -> assign directly
                            'lir("taxable") = li.taxable
                            lir("fulfillment_status") = li.fulfillment_status
                            dtLineItems.Rows.Add(lir)
                        Next
                    End If

                    ' Refunds and refund_line_items
                    If o.refunds IsNot Nothing Then
                        For Each rf In o.refunds
                            Dim refundIdStr As String = If(rf.id Is Nothing, String.Empty, rf.id.ToString())
                            Dim rrow = dtRefunds.NewRow()
                            rrow("refund_id") = refundIdStr
                            rrow("admin_graphql_api_id") = If(String.IsNullOrEmpty(rf.admin_graphql_api_id), String.Empty, rf.admin_graphql_api_id)
                            rrow("created_at") = rf.created_at
                            rrow("note") = If(String.IsNullOrEmpty(rf.note), String.Empty, rf.note)
                            ' order_id on refund may be present — fallback to parent order id
                            rrow("order_id") = If(rf.order_id Is Nothing, orderIdStr, Convert.ToString(rf.order_id))
                            rrow("processed_at") = rf.processed_at
                            rrow("restock") = rf.restock
                            rrow("user_id") = If(rf.user_id.HasValue, rf.user_id.Value.ToString(), String.Empty)

                            Dim dutiesAmt As String = String.Empty
                            If rf.total_duties_set IsNot Nothing AndAlso rf.total_duties_set.shop_money IsNot Nothing Then
                                dutiesAmt = If(String.IsNullOrEmpty(rf.total_duties_set.shop_money.amount), String.Empty, rf.total_duties_set.shop_money.amount)
                            End If
                            rrow("total_duties_amount") = dutiesAmt

                            dtRefunds.Rows.Add(rrow)

                            ' Save refund transactions (if present) into RefundTransactions table
                            If rf.transactions IsNot Nothing Then
                                For Each tx In rf.transactions
                                    Dim trx = dtRefundTransactions.NewRow()
                                    trx("refund_id") = refundIdStr
                                    trx("transaction_id") = If(tx.id Is Nothing, String.Empty, tx.id.ToString())
                                    trx("parent_id") = If(tx.parent_id Is Nothing, String.Empty, Convert.ToString(tx.parent_id))
                                    trx("amount") = If(tx.amount Is Nothing, 0D, Convert.ToDouble(tx.amount))
                                    trx("kind") = If(String.IsNullOrEmpty(tx.kind), String.Empty, tx.kind)
                                    trx("gateway") = If(String.IsNullOrEmpty(tx.gateway), String.Empty, tx.gateway)
                                    trx("source_name") = If(String.IsNullOrEmpty(tx.source_name), String.Empty, tx.source_name)
                                    trx("status") = If(String.IsNullOrEmpty(tx.status), String.Empty, tx.status)
                                    trx("created_at") = IIf(IsNothing(tx.created_at), DBNull.Value, tx.created_at)
                                    dtRefundTransactions.Rows.Add(trx)
                                Next
                            End If

                            If rf.refund_line_items IsNot Nothing Then
                                For Each rli In rf.refund_line_items
                                    Dim rlir = dtRefundLineItems.NewRow()
                                    rlir("refund_id") = refundIdStr
                                    ' rli.id, rli.line_item_id and rli.location_id are value types (Long) in clsJsonOrders -> use Convert.ToString
                                    rlir("id") = Convert.ToString(rli.id)
                                    rlir("line_item_id") = Convert.ToString(rli.line_item_id)
                                    rlir("location_id") = Convert.ToString(rli.location_id)
                                    rlir("quantity") = rli.quantity
                                    rlir("restock_type") = If(String.IsNullOrEmpty(rli.restock_type), String.Empty, rli.restock_type)
                                    rlir("subtotal") = rli.subtotal
                                    rlir("total_tax") = rli.total_tax

                                    ' If the refund line item includes the original line item, capture its title/sku
                                    If rli.line_item IsNot Nothing Then
                                        rlir("line_item_title") = If(String.IsNullOrEmpty(rli.line_item.title), String.Empty, rli.line_item.title)
                                        rlir("line_item_sku") = If(String.IsNullOrEmpty(rli.line_item.sku), String.Empty, rli.line_item.sku)
                                        rlir("line_item_product_id") = If(String.IsNullOrEmpty(rli.line_item.product_id), String.Empty, rli.line_item.product_id)
                                        'rlir("line_item_name") = If(String.IsNullOrEmpty(rli.line_item.name), String.Empty, rli.line_item.name)
                                        rlir("variant_title") = If(String.IsNullOrEmpty(rli.line_item.variant_title), String.Empty, rli.line_item.variant_title)
                                    Else
                                        rlir("line_item_title") = String.Empty
                                        rlir("line_item_sku") = String.Empty
                                        rlir("line_item_product_id") = String.Empty
                                        'rlir("line_item_name") = String.Empty
                                        rlir("variant_title") = String.Empty
                                    End If

                                    dtRefundLineItems.Rows.Add(rlir)
                                Next
                            End If
                        Next
                    End If
                Next
            End If
        Catch ex As WebException
            ' Rethrow to caller — preserve stack
            Throw
        End Try

        ds.Tables.Add(dtOrders)
        ds.Tables.Add(dtLineItems)
        'ds.Tables.Add(dtCustomers)
        'ds.Tables.Add(dtBilling)
        'ds.Tables.Add(dtShippingLines)
        ds.Tables.Add(dtRefunds)
        ds.Tables.Add(dtRefundLineItems)
        ' add RefundTransactions table to dataset
        ds.Tables.Add(dtRefundTransactions)
        ' add Fulfillments table (last)
        ds.Tables.Add(dtFulfillments)

        Return ds
    End Function

End Class
