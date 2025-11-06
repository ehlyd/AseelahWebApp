Imports System.IO
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

End Module
