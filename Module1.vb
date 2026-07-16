Imports System.Drawing
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Module Module1

    Public APIUsername, APIPassword As String

    Public Function RetailPro_OracleConnectionString() As String

        Dim strDatasource As String = "", strUserID As String = "", strPassword As String = ""
        strDatasource = GetSettingValue("RP_DATASOURCE")
        strUserID = GetSettingValue("RP_USERID")
        strPassword = GetSettingValue("RP_PSWRD")

        Return "Data Source=" & strDatasource & ";User ID=" & strUserID & ";Password=" & strPassword

    End Function

    Public Function EBSSTG_OracleConnectionString() As String

        Dim strDatasource As String = "", strUserID As String = "", strPassword As String = ""
        strDatasource = GetSettingValue("EBSSTG_DATASOURCE")
        strUserID = GetSettingValue("EBSSTG_USERID")
        strPassword = GetSettingValue("EBSSTG_PSWRD")

        Return "Data Source=" & strDatasource & ";User ID=" & strUserID & ";Password=" & strPassword

    End Function

    Public Function EBSCloud_OracleConnectionString() As String
        Dim mclsSQL As New clsSQLDB
        Try
            Dim strDatasource As String = "", strUserID As String = "", strPassword As String = ""
            strDatasource = GetSettingValue("EBSCLOUD_DATASOURCE")
            strUserID = GetSettingValue("EBSCLOUD_USERID")
            strPassword = GetSettingValue("EBSCLOUD_PSWRD")

            Return "Data Source=" & strDatasource & ";User ID=" & strUserID & ";Password=" & strPassword

        Catch ex As Exception
            Throw ex
        Finally
            mclsSQL.CloseDB()
        End Try
    End Function

    Public Function IpekyolShopify_URL() As String
        Return GetSettingValue("IPEKYOLSHOPIFY_URL")
    End Function

    Public Function IpekyolShopify_AccessToken() As String
        Return GetSettingValue("IPEKYOLSHOPIFY_ACCESSTOKEN")
    End Function

    Public Function EmailSender() As String
        Return GetSettingValue("EMAIL_SENDER")
    End Function

    Public Function EmailPassword() As String
        Return GetSettingValue("EMAIL_PASSWORD")
    End Function

    Private Function GetSettingValue(strSettingName As String) As String
        Dim mclsSQL As New clsSQLDB
        Try
            Dim strValue As String = ""
            Dim dt As DataTable
            mclsSQL.OpenDB()
            dt = mclsSQL.GetDataSet("select * from BrandIntegration_Settings where upper(IntegrationName)='WEB_REPORTMANAGER' and upper(SettingName)='" & strSettingName & "'").Tables(0)
            If dt.Rows.Count <> 0 Then
                strValue = IIf(IsDBNull(dt.Rows(0).Item("SettingValue")), "", dt.Rows(0).Item("SettingValue"))
            End If

            Return strValue

        Catch ex As Exception
            Throw ex
        Finally
            mclsSQL.CloseDB()
        End Try
    End Function

    Public Sub ShowMessageAlert(ByVal page As System.Web.UI.Page, strMessage As String, strMessageType As String)

        Dim script As String

        Dim strMessageIcon As String = ""

        Select Case strMessageType
            Case "error"
                strMessageIcon = "Error!"
            Case "success"
                strMessageIcon = "Success!"
            Case "warning"
                strMessageIcon = "Warning!"
            Case "info"
                strMessageIcon = "Info!"
            Case "question"
                strMessageIcon = "Question!"
        End Select

        strMessage = Replace(strMessage, "'", "")

        If strMessageType <> "question" Then

            script = $"<script>Swal.fire('{strMessageIcon}', '{strMessage}', '{strMessageType}');</script>"
            page.ClientScript.RegisterStartupScript(page.GetType(), "showSweetAlert", script)

        Else

            script = $"<script>
            Swal.fire({{
                title: 'Confirmation',
                text: '{strMessage}',
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No',
                allowOutsideClick: false
            }}).then((result) => {{
                if (result.isConfirmed) {{
                    // User clicked 'Yes'
                     //waitCursor();  //Show wait cursor *before* initiating postback
                    __doPostBack('{page.UniqueID}', 'YesClicked');
                }} else if (result.dismiss === Swal.DismissReason.cancel) {{
                    // User clicked 'No' or closed the dialog
                    __doPostBack('{page.UniqueID}', 'NoClicked');
                }}
            }})</script>"
            page.ClientScript.RegisterStartupScript(page.GetType(), "showConfirmationAlert", script)

        End If
    End Sub

    Public Sub ExportToTextFile(ByVal dataTable As System.Data.DataTable, ByVal filePath As String, ByVal isOldDevice As Boolean)
        Try
            Dim stringBuilder As New StringBuilder

            Dim includeHeaders As Boolean = True, delimiter As String = ";"

            Dim strLine As String

            For rowIndex As Integer = 0 To dataTable.Rows.Count - 1
                For colIndex As Integer = 0 To dataTable.Columns.Count - 1
                    Dim cellValue As String = dataTable.Rows(rowIndex).Item(colIndex).ToString()
                    strLine = dataTable.Rows(rowIndex).Item(colIndex).ToString()
                    strLine = strLine + StrDup(150 - Len(strLine), " ")

                    stringBuilder.Append(strLine)
                    If colIndex < dataTable.Columns.Count - 1 Then
                        stringBuilder.Append(delimiter)
                    End If
                Next
                stringBuilder.AppendLine()
            Next

            If isOldDevice Then
                File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.GetEncoding(1256))
            Else
                File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8)
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function GenerateNewSessionKey() As String
        Return Guid.NewGuid().ToString("N") ' "N" format removes hyphens
    End Function

    Public Function RandomString(ByVal length As Integer) As String
        Dim chars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
        Dim rand As New Random
        Dim sb As New StringBuilder

        For i As Integer = 1 To length
            Dim randomIndex As Integer = rand.Next(0, chars.Length)
            sb.Append(chars(randomIndex))
        Next

        Return sb.ToString()
    End Function

    Public Sub SendEmail(ByVal subject As String, ByVal body As String, ByVal EmailRecipient As String)

        Dim strEmailSender, strEmailPswrd As String
        'strEmailSender = "noreply@aseelah.com"
        'strEmailPswrd = "Gop01140"
        strEmailSender = EmailSender()
        strEmailPswrd = EmailPassword()

        Dim smtpClient As New SmtpClient("smtp-mail.outlook.com", 587)
        smtpClient.Credentials = New NetworkCredential(strEmailSender, strEmailPswrd)
        smtpClient.EnableSsl = True

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Dim mailMessage As New MailMessage(strEmailSender, EmailRecipient, subject, body)

        Try
            smtpClient.Send(mailMessage)
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Private Sub HideAllMenu(page As System.Web.UI.Page)

        Dim m As AseelahWebApps.SiteMaster = TryCast(page.Master, AseelahWebApps.SiteMaster)
        If m IsNot Nothing Then
            m.SetSecurityVisible(False)
            m.SetPISummaryVisible(False)
            m.SetExportItemMasterVisible(False)
            m.SetSalesVisible(False)
            m.SetInventoryVisible(False)
        End If

    End Sub

    Public Sub ShowHideMenu(page As System.Web.UI.Page)
        Try
            HideAllMenu(page)

            Dim strQuery As String
            Dim mclsSQL As New clsSQLDB
            Dim dt As DataTable

            mclsSQL.OpenDB()

            strQuery = "select distinct WEBAPP_MODULENAME,USER_GROUP_ID from WEBAPP_GROUP_ACCESS"
            dt = mclsSQL.GetDataSet(strQuery).Tables(0)

            Dim mclsEncrypt As New clsEncryptDecrypt
            Dim strModule As String, strGroupID As String

            Dim m As AseelahWebApps.SiteMaster = TryCast(page.Master, AseelahWebApps.SiteMaster)

            For Each dRow As DataRow In dt.Rows
                strModule = dRow.Item("WEBAPP_MODULENAME")
                strGroupID = mclsEncrypt.Decrypt(dRow.Item("USER_GROUP_ID"))
                strGroupID = Mid(strGroupID, InStr(strGroupID, "--") + 2, 3)

                If page.Session("groupid") = strGroupID Then

                    Select Case strModule

                        Case "PI SUMMARY"

                            If m IsNot Nothing Then
                                m.SetPISummaryVisible(True)
                            End If

                        'Case "PHYSICAL INVENTORY"

                            'If m IsNot Nothing Then
                            '    m.SetPI(True)
                            'End If

                        Case "EXPORT ITEMS"

                            If m IsNot Nothing Then
                                m.SetExportItemMasterVisible(True)
                            End If

                        Case "SECURITY"

                            If m IsNot Nothing Then
                                m.SetSecurityVisible(True)
                            End If

                        Case "ONLINE SALES COMPARISON"

                            If m IsNot Nothing Then
                                m.SetSalesVisible(True)
                            End If

                    End Select

                End If
            Next



            'strQuery = "select distinct WEBAPP_MODULENAME from WEBAPP_GROUP_ACCESS where USER_GROUP_ID=" & page.Session("groupid")
            'dt = mclsSQL.GetDataSet(strQuery).Tables(0)

            'If dt.Rows.Count <> 0 Then

            '    For Each dRow As DataRow In dt.Rows

            '        Dim m As AseelahWebApps.SiteMaster = TryCast(page.Master, AseelahWebApps.SiteMaster)
            '        Select Case dRow.Item(0).ToString

            '            Case "SECURITY"

            '                If m IsNot Nothing Then
            '                    m.SetSecurityVisible(True)
            '                End If

            '            Case "PI SUMMARY"

            '                If m IsNot Nothing Then
            '                    m.SetPISummaryVisible(True)
            '                End If

            '            Case "EXPORT ITEMS"

            '                If m IsNot Nothing Then
            '                    m.SetExportItemMasterVisible(True)
            '                End If

            '        End Select

            '    Next

            'End If

            mclsSQL.CloseDB()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub ExportToExcel_EPPlus(dtExcelData As DataTable, strFilePath As String, Optional totalColumnNames As String() = Nothing)

        Try
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial

            Using package As New ExcelPackage()
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add("Sheet1")

                'worksheet.Cells.Style.Font.Name = "Arial" ' Or "Calibri", "Tahoma", "Segoe UI"
                'worksheet.Cells.Style.Font.Name = "Segoe UI"

                For col As Integer = 0 To dtExcelData.Columns.Count - 1
                    worksheet.Cells(1, col + 1).Value = dtExcelData.Columns(col).ColumnName
                    worksheet.Cells(1, col + 1).Style.Font.Bold = True
                    worksheet.Cells(1, col + 1).Style.Fill.PatternType = ExcelFillStyle.Solid
                    worksheet.Cells(1, col + 1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray)
                Next

                For row As Integer = 0 To dtExcelData.Rows.Count - 1
                    For col As Integer = 0 To dtExcelData.Columns.Count - 1
                        worksheet.Cells(row + 2, col + 1).Value = dtExcelData.Rows(row)(col)
                    Next
                Next

                '=====================================add total row 
                Dim lastRow As Integer = dtExcelData.Rows.Count + 1
                For col As Integer = 0 To dtExcelData.Columns.Count - 1
                    worksheet.Cells(lastRow + 1, col + 1).Style.Font.Bold = True
                    worksheet.Cells(lastRow + 1, col + 1).Style.Fill.PatternType = ExcelFillStyle.Solid
                    worksheet.Cells(lastRow + 1, col + 1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray)
                Next

                ' Calculate and add totals for each column specified
                If totalColumnNames IsNot Nothing AndAlso totalColumnNames.Length > 0 Then
                    worksheet.Cells(lastRow + 1, 1).Value = "TOTAL" ' Label row

                    For Each totalColumnName As String In totalColumnNames
                        Dim totalColumnIndex As Integer = -1
                        For i As Integer = 0 To dtExcelData.Columns.Count - 1
                            If dtExcelData.Columns(i).ColumnName = totalColumnName Then
                                totalColumnIndex = i + 1
                                Exit For
                            End If
                        Next

                        If totalColumnIndex <> -1 Then
                            If totalColumnIndex > 26 Then
                                worksheet.Cells(lastRow + 1, totalColumnIndex).Formula = $"SUM({"A" & Chr(Asc("A") + ((totalColumnIndex - 26) - 1))}2:{"A" & Chr(Asc("A") + ((totalColumnIndex - 26) - 1))}{lastRow})"
                            Else
                                worksheet.Cells(lastRow + 1, totalColumnIndex).Formula = $"SUM({Chr(Asc("A") + totalColumnIndex - 1)}2:{Chr(Asc("A") + totalColumnIndex - 1)}{lastRow})"
                            End If

                            worksheet.Cells(lastRow + 1, totalColumnIndex).Style.Numberformat.Format = "#,##0"
                        End If

                    Next

                End If
                '=====================================

                worksheet.Columns.AutoFit()

                package.SaveAs(New FileInfo(strFilePath))

                package.Dispose()

            End Using

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub ExportToExcel_EPPlus_Jacadi(dtExcelData As DataTable, strFilePath As String, Optional totalColumnNames As String() = Nothing)

        Try
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial

            Using package As New ExcelPackage()
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add("Sheet1")

                'worksheet.Cells.Style.Font.Name = "Arial" ' Or "Calibri", "Tahoma", "Segoe UI"
                'worksheet.Cells.Style.Font.Name = "Segoe UI"

                worksheet.Cells(1, 1).Value = "Generated on: " & DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")

                For col As Integer = 0 To dtExcelData.Columns.Count - 1
                    worksheet.Cells(3, col + 1).Value = dtExcelData.Columns(col).ColumnName
                    worksheet.Cells(3, col + 1).Style.Font.Bold = True
                    worksheet.Cells(3, col + 1).Style.Fill.PatternType = ExcelFillStyle.Solid
                    worksheet.Cells(3, col + 1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray)
                Next

                For row As Integer = 0 To dtExcelData.Rows.Count - 1
                    For col As Integer = 0 To dtExcelData.Columns.Count - 1
                        worksheet.Cells(row + 4, col + 1).Value = dtExcelData.Rows(row)(col)
                    Next
                Next

                '=====================================add total row 
                Dim lastRow As Integer = dtExcelData.Rows.Count + 1
                For col As Integer = 0 To dtExcelData.Columns.Count - 1
                    worksheet.Cells(lastRow + 3, col + 1).Style.Font.Bold = True
                    worksheet.Cells(lastRow + 3, col + 1).Style.Fill.PatternType = ExcelFillStyle.Solid
                    worksheet.Cells(lastRow + 3, col + 1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray)
                Next

                ' Calculate and add totals for each column specified
                If totalColumnNames IsNot Nothing AndAlso totalColumnNames.Length > 0 Then
                    worksheet.Cells(lastRow + 3, 1).Value = "TOTAL" ' Label row

                    For Each totalColumnName As String In totalColumnNames
                        Dim totalColumnIndex As Integer = -1
                        For i As Integer = 0 To dtExcelData.Columns.Count - 1
                            If dtExcelData.Columns(i).ColumnName = totalColumnName Then
                                totalColumnIndex = i + 1
                                Exit For
                            End If
                        Next

                        If totalColumnIndex <> -1 Then
                            If totalColumnIndex > 26 Then
                                worksheet.Cells(lastRow + 3, totalColumnIndex).Formula = $"SUM({"A" & Chr(Asc("A") + ((totalColumnIndex - 26) - 1))}2:{"A" & Chr(Asc("A") + ((totalColumnIndex - 26) - 1))}{lastRow + 2})"
                            Else
                                worksheet.Cells(lastRow + 3, totalColumnIndex).Formula = $"SUM({Chr(Asc("A") + totalColumnIndex - 1)}2:{Chr(Asc("A") + totalColumnIndex - 1)}{lastRow + 2})"
                            End If

                            worksheet.Cells(lastRow + 3, totalColumnIndex).Style.Numberformat.Format = "#,##0"
                        End If

                    Next

                End If
                '=====================================

                ' Highlight rows where ORDER_STATE is Canceled or Refunded
                Dim totalCols As Integer = dtExcelData.Columns.Count
                Dim totalRows As Integer = dtExcelData.Rows.Count

                Dim orderStateColIndex As Integer = -1
                For i As Integer = 0 To dtExcelData.Columns.Count - 1
                    If String.Equals(dtExcelData.Columns(i).ColumnName, "ORDER_STATE", StringComparison.OrdinalIgnoreCase) Then
                        orderStateColIndex = i + 1
                        Exit For
                    End If
                Next

                '' fallback check for ORDER_STATUS if needed
                'If orderStateColIndex = -1 Then
                '    For i As Integer = 0 To dtExcelData.Columns.Count - 1
                '        If String.Equals(dtExcelData.Columns(i).ColumnName, "ORDER_STATUS", StringComparison.OrdinalIgnoreCase) Then
                '            orderStateColIndex = i + 1
                '            Exit For
                '        End If
                '    Next
                'End If



                If orderStateColIndex > 0 Then
                    For r As Integer = 0 To dtExcelData.Rows.Count - 1
                        Dim excelRow As Integer = r + 4
                        Dim state As String = Convert.ToString(dtExcelData.Rows(r)(orderStateColIndex - 1)).Trim()

                        Dim fillColor As Color? = Nothing
                        If String.Equals(state, "Canceled", StringComparison.OrdinalIgnoreCase) Then
                            fillColor = ColorTranslator.FromHtml("#FFE0E0")
                        ElseIf String.Equals(state, "Refunded", StringComparison.OrdinalIgnoreCase) Then
                            fillColor = ColorTranslator.FromHtml("#FFE5B4")
                        End If

                        If fillColor.HasValue Then
                            Dim rng = worksheet.Cells(excelRow, 1, excelRow, totalCols)
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid
                            rng.Style.Fill.BackgroundColor.SetColor(fillColor.Value)
                        End If
                    Next
                End If

                '======================================================

                ' --------------------------------------------------
                ' Highlight columns Q to X (columns 17 to 24) with light green
                Dim lightBlue As Color = ColorTranslator.FromHtml("#DAF2D0")
                Dim startCol As Integer = 17 ' Q
                Dim endCol As Integer = 24   ' X
                Dim highlightFromRow As Integer = 4
                Dim highlightToRow As Integer = lastRow + 2   ' include totals row

                ' ensure we don't attempt to highlight columns that don't exist in the table
                For col As Integer = startCol To Math.Min(endCol, totalCols)
                    Dim colRange = worksheet.Cells(highlightFromRow, col, highlightToRow, col)
                    colRange.Style.Fill.PatternType = ExcelFillStyle.Solid
                    colRange.Style.Fill.BackgroundColor.SetColor(lightBlue)
                Next
                ' --------------------------------------------------


                worksheet.View.FreezePanes(4, 1)
                worksheet.Columns.AutoFit()

                package.SaveAs(New FileInfo(strFilePath))

                package.Dispose()

            End Using

        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Public Sub ExportToExcel_EPPlus_Ipekyol(dt As DataTable, filePath As String, Optional totalColumnNames As String() = Nothing)
        Try
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial

            Dim fi As New FileInfo(filePath)
            If fi.Directory IsNot Nothing AndAlso Not fi.Directory.Exists Then
                fi.Directory.Create()
            End If

            Using package As New ExcelPackage(fi)
                ' If existing worksheet with the same name exists, delete it to avoid the "worksheet already exists" error
                Dim sheetName As String = "Sheet1"
                Dim existing = package.Workbook.Worksheets.FirstOrDefault(Function(w) String.Equals(w.Name, sheetName, StringComparison.OrdinalIgnoreCase))
                If existing IsNot Nothing Then
                    package.Workbook.Worksheets.Delete(existing)
                End If

                Dim ws = package.Workbook.Worksheets.Add(sheetName)

                ws.Cells(1, 1).Value = "Generated on: " & DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")

                ' Load DataTable with headers
                ws.Cells(3, 1).LoadFromDataTable(dt, True)

                Dim totalCols As Integer = dt.Columns.Count
                Dim totalRows As Integer = dt.Rows.Count

                '=====================================add total row 
                Dim lastRow As Integer = dt.Rows.Count + 1
                For col As Integer = 0 To dt.Columns.Count - 1
                    ws.Cells(lastRow + 3, col + 1).Style.Font.Bold = True
                    ws.Cells(lastRow + 3, col + 1).Style.Fill.PatternType = ExcelFillStyle.Solid
                    ws.Cells(lastRow + 3, col + 1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray)
                Next

                ' Calculate and add totals for each column specified
                If totalColumnNames IsNot Nothing AndAlso totalColumnNames.Length > 0 Then
                    ws.Cells(lastRow + 3, 1).Value = "TOTAL" ' Label row

                    For Each totalColumnName As String In totalColumnNames
                        Dim totalColumnIndex As Integer = -1
                        For i As Integer = 0 To dt.Columns.Count - 1
                            If dt.Columns(i).ColumnName = totalColumnName Then
                                totalColumnIndex = i + 1
                                Exit For
                            End If
                        Next

                        If totalColumnIndex <> -1 Then
                            If totalColumnIndex > 26 Then
                                ws.Cells(lastRow + 3, totalColumnIndex).Formula = $"SUM({"A" & Chr(Asc("A") + ((totalColumnIndex - 26) - 1))}2:{"A" & Chr(Asc("A") + ((totalColumnIndex - 26) - 1))}{lastRow + 2})"
                            Else
                                ws.Cells(lastRow + 3, totalColumnIndex).Formula = $"SUM({Chr(Asc("A") + totalColumnIndex - 1)}2:{Chr(Asc("A") + totalColumnIndex - 1)}{lastRow + 2})"
                            End If

                            ws.Cells(lastRow + 3, totalColumnIndex).Style.Numberformat.Format = "#,##0"
                        End If

                    Next

                End If
                '=====================================


                ' Header style
                Using hdr = ws.Cells(3, 1, 3, totalCols)
                    hdr.Style.Font.Bold = True
                    hdr.Style.Fill.PatternType = ExcelFillStyle.Solid
                    'hdr.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#F2F2F2"))
                    hdr.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray)
                End Using

                ' Find column indexes (1-based for EPPlus)
                Dim itemStatusColIndex As Integer = 0
                Dim orderStatusColIndex As Integer = 0
                If dt.Columns.Contains("ITEM_STATUS") Then
                    itemStatusColIndex = dt.Columns.IndexOf("ITEM_STATUS") + 1
                End If
                If dt.Columns.Contains("ORDER_STATUS") Then
                    orderStatusColIndex = dt.Columns.IndexOf("ORDER_STATUS") + 1
                End If

                ' Highlight rows based on ITEM_STATUS and ORDER_STATUS
                For r As Integer = 0 To totalRows - 1
                    Dim excelRow As Integer = r + 4 ' data starts at row 2
                    Dim itemStatus As String = ""
                    Dim orderStatus As String = ""

                    If itemStatusColIndex > 0 Then
                        itemStatus = Convert.ToString(dt.Rows(r)("ITEM_STATUS")).Trim().ToLower()
                    End If
                    If orderStatusColIndex > 0 Then
                        orderStatus = Convert.ToString(dt.Rows(r)("ORDER_STATUS")).Trim().ToLower()
                    End If

                    Dim fillColor As Color? = Nothing

                    If orderStatus = "cancelled" Then
                        fillColor = ColorTranslator.FromHtml("#FFE0E0")
                    ElseIf itemStatus = "removed" Then
                        fillColor = ColorTranslator.FromHtml("#FFD6D6")
                    ElseIf itemStatus = "refunded" Then
                        fillColor = ColorTranslator.FromHtml("#FFE5B4")
                    ElseIf itemStatus = "partially_refunded" Then
                        fillColor = ColorTranslator.FromHtml("#FFF3CD")
                    ElseIf itemStatus = "returned" Then
                        fillColor = ColorTranslator.FromHtml("#E3F2FD")
                    ElseIf itemStatus = "refund_failed" Then
                        fillColor = ColorTranslator.FromHtml("#F3E5F5")
                    ElseIf itemStatus = "unfulfilled" Then
                        fillColor = ColorTranslator.FromHtml("#FFF9C4")
                    End If

                    If fillColor.HasValue Then
                        Dim rng = ws.Cells(excelRow, 1, excelRow, totalCols)
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid
                        rng.Style.Fill.BackgroundColor.SetColor(fillColor.Value)
                    End If
                Next

                ' --------------------------------------------------
                ' Highlight columns Z to AI (columns 26 to 35) with light green
                ' include header (row 1), all data rows and the total row
                Dim lightBlue As Color = ColorTranslator.FromHtml("#DAF2D0")
                Dim startCol As Integer = 24 ' X
                Dim endCol As Integer = 34   ' AH
                Dim highlightFromRow As Integer = 4
                Dim highlightToRow As Integer = lastRow + 2  ' include totals row

                ' ensure we don't attempt to highlight columns that don't exist in the table
                For col As Integer = startCol To Math.Min(endCol, totalCols)
                    Dim colRange = ws.Cells(highlightFromRow, col, highlightToRow, col)
                    colRange.Style.Fill.PatternType = ExcelFillStyle.Solid
                    colRange.Style.Fill.BackgroundColor.SetColor(lightBlue)
                Next
                ' --------------------------------------------------


                ' Auto-fit columns and freeze header
                ws.View.FreezePanes(4, 1)
                ws.Cells(ws.Dimension.Address).AutoFitColumns()

                'Save package
                package.Save()
            End Using

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function CreateTempTable_XXASH_TMPIPKECOM() As String
        Return "CREATE TABLE XXASH_TMPIPKECOM
                (
                  ORDER_ID            NUMBER,
                  ORDER_NAME          VARCHAR2(10 BYTE),
                  ORDER_DATE          TIMESTAMP(6),
                  SKU                 VARCHAR2(30 BYTE),
                  VARIANT_TITLE       VARCHAR2(30 BYTE),
                  QUANTITY            NUMBER,
                  PRICE               NUMBER,
                  LINE_TOTAL          NUMBER,
                  FULFILLMENT_DATE    TIMESTAMP(6),
                  FULFILLMENT_STATUS  VARCHAR2(20 BYTE),
                  RETURN_DATE         TIMESTAMP(6),
                  RETURN_QTY          NUMBER,
                  RETURN_TYPE         VARCHAR2(20 BYTE),
                  REFUND_DATE         TIMESTAMP(6),
                  REFUNDED_QTY        NUMBER,
                  REFUND_AMOUNT       NUMBER,
                  ITEM_STATUS         VARCHAR2(30 BYTE),
                  ITEM_RETURNED       VARCHAR2(10 BYTE),
                  NET_QTY             NUMBER,
                  NET_AMOUNT          NUMBER,
                  NET_AMOUNT_US       NUMBER,
                  KUR_SAR_USD         NUMBER,
                  ORDER_STATUS        VARCHAR2(30 BYTE),
                  CANCEL_DATE         TIMESTAMP(6),
                  CANCEL_REASON       VARCHAR2(30 BYTE),
                  RUN_ID              VARCHAR2(100 BYTE)
                )"
    End Function

End Module
