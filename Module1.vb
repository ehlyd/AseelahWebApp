Imports System.IO
Imports OfficeOpenXml
Imports OfficeOpenXml.Style

Module Module1

    Public APIUsername, APIPassword As String

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
                cancelButtonText: 'No'
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
End Module
