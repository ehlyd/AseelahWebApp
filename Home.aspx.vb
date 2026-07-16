Imports System.Web.Services
Imports System.Xml

Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Session("AuthSession") Is Nothing Then
            Response.Redirect("Default.aspx")
        End If

        If Not IsPostBack Then
            ShowHideMenu()
        End If

    End Sub

    Protected Sub imBtnExportItems_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnExportItems.Click
        Response.Redirect("ExportItemMaster.aspx")
    End Sub

    Protected Sub imgBtnPI_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnPI.Click
        Response.Redirect("PISummary.aspx")
    End Sub

    Private Sub HideAllMenu()
        'imgBtnPI.Visible = False
        'HyperLinkPISummary.Visible = False

        imgBtnPICol.Visible = False
        HyperLinkPISummaryCol.Visible = False

        imgBtnPhysicalInvCol.Visible = False
        HyperLinkPysicalInvCol.Visible = False

        imgBtnExportItemsCol.Visible = False
        HyperLinkExportItemCol.Visible = False

        imgBtnOnlineSalesCol.Visible = False
        HyperLinkOnlineSalesCol.Visible = False

        Dim m As AseelahWebApps.SiteMaster = TryCast(Me.Master, AseelahWebApps.SiteMaster)
        If m IsNot Nothing Then
            m.SetSecurityVisible(False)
            m.SetPISummaryVisible(False)
            m.SetExportItemMasterVisible(False)
            m.SetSalesVisible(False)
            m.SetInventoryVisible(False)
        End If

    End Sub

    Private Sub ShowHideMenu()
        Try
            HideAllMenu()

            Dim strQuery As String
            Dim mclsSQL As New clsSQLDB
            Dim dt As DataTable

            mclsSQL.OpenDB()

            'strQuery = "select distinct WEBAPP_MODULENAME from WEBAPP_GROUP_ACCESS where USER_GROUP_ID=" & Session("groupid")

            strQuery = "select distinct WEBAPP_MODULENAME,USER_GROUP_ID from WEBAPP_GROUP_ACCESS"
            dt = mclsSQL.GetDataSet(strQuery).Tables(0)

            Dim mclsEncrypt As New clsEncryptDecrypt
            Dim strModule As String, strGroupID As String

            Dim m As AseelahWebApps.SiteMaster = TryCast(Me.Master, AseelahWebApps.SiteMaster)

            For Each dRow As DataRow In dt.Rows
                strModule = dRow.Item("WEBAPP_MODULENAME")
                strGroupID = mclsEncrypt.Decrypt(dRow.Item("USER_GROUP_ID"))
                strGroupID = Mid(strGroupID, InStr(strGroupID, "--") + 2, 3)

                If Session("groupid") = strGroupID Then

                    Select Case strModule

                        Case "PI SUMMARY"

                            imgBtnPICol.Visible = True
                            HyperLinkPISummaryCol.Visible = True

                            If m IsNot Nothing Then
                                m.SetPISummaryVisible(True)
                            End If

                        'Case "PHYSICAL INVENTORY"


                            'imgBtnPhysicalInvCol.Visible = True
                            'HyperLinkPysicalInvCol.Visible = True

                            'If m IsNot Nothing Then
                            '    m.SetPI(True)
                            'End If

                        Case "EXPORT ITEMS"

                            imgBtnExportItemsCol.Visible = True
                            HyperLinkExportItemCol.Visible = True

                            If m IsNot Nothing Then
                                m.SetExportItemMasterVisible(True)
                            End If

                        Case "SECURITY"

                            If m IsNot Nothing Then
                                m.SetSecurityVisible(True)
                            End If

                        Case "ONLINE SALES COMPARISON"
                            imgBtnOnlineSalesCol.Visible = True
                            HyperLinkOnlineSalesCol.Visible = True

                            If m IsNot Nothing Then
                                m.SetSalesVisible(True)
                            End If

                    End Select

                End If
            Next

            'If dt.Rows.Count <> 0 Then

            '    For Each dRow As DataRow In dt.Rows

            '        Dim m As AseelahWebApps.SiteMaster = TryCast(Me.Master, AseelahWebApps.SiteMaster)

            '        Select Case dRow.Item(0).ToString

            '            Case "PI SUMMARY"

            '                imgBtnPICol.Visible = True
            '                HyperLinkPISummaryCol.Visible = True

            '                'imgBtnPI.Visible = True
            '                'HyperLinkPISummary.Visible = True

            '                If m IsNot Nothing Then
            '                    m.SetPISummaryVisible(True)
            '                End If

            '            Case "PHYSICAL INVENTORY"

            '                'imgBtnPhysicalInv.Visible = True
            '                'HyperLinkPysicalInv.Visible = True

            '                imgBtnPhysicalInvCol.Visible = True
            '                HyperLinkPysicalInvCol.Visible = True

            '                If m IsNot Nothing Then
            '                    m.SetPI(True)
            '                End If

            '            Case "EXPORT ITEMS"
            '                'imgBtnExportItems.Visible = True
            '                'HyperLinkExportItem.Visible = True

            '                imgBtnExportItemsCol.Visible = True
            '                HyperLinkExportItemCol.Visible = True

            '                If m IsNot Nothing Then
            '                    m.SetExportItemMasterVisible(True)
            '                End If

            '            Case "SECURITY"

            '                If m IsNot Nothing Then
            '                    m.SetSecurityVisible(True)
            '                End If

            '            Case "GROUP ACCESS"

            '                'imBtnGroupAccess.Visible = True
            '                'HyperLinkGroupAccess.Visible = True

            '        End Select

            '    Next

            'End If

            mclsSQL.CloseDB()

        Catch ex As Exception
            ShowMessageAlert(ex.Message, "error")
        End Try
    End Sub

    'Private Function GetUserGroupSID(EmpSID As String) As String
    '    Try
    '        Dim strQuery As String, dt As DataTable
    '        Dim mclsOra As New clsOracleDB("RetailPro_OracleConnection")
    '        mclsOra.OpenDB()
    '        strQuery = "SELECT USER_GROUP_SID FROM rps.USER_GROUP_USER WHERE EMPLOYEE_SID ='" & EmpSID & "'"
    '        dt = mclsOra.GetDataSet(strQuery).Tables(0)
    '        If dt.Rows.Count <> 0 Then
    '            Return dt.Rows(0).Item(0)
    '        Else
    '            Return ""
    '        End If
    '        mclsOra.CloseDB
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Function

    Protected Sub ShowMessageAlert(strMessage As String, strMessageType As String)

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

            ClientScript.RegisterStartupScript(Me.GetType(), "showSweetAlert", script)

        Else

            Dim yesPostBackScript As String = ClientScript.GetPostBackEventReference(Me, "YesClicked")
            Dim noPostBackScript As String = ClientScript.GetPostBackEventReference(Me, "NoClicked")

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
                    {yesPostBackScript};
                 }} else if (result.dismiss === Swal.DismissReason.cancel) {{
                    {noPostBackScript};
                 }}
            }})</script>"
            ClientScript.RegisterStartupScript(Me.GetType(), "showConfirmationAlert", script)

        End If
    End Sub

    Protected Sub imgBtnPhysicalInv_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnPhysicalInv.Click
        Response.Redirect("Physical_Inventory.aspx")
    End Sub

    Private Sub imgBtnOnlineSales_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnOnlineSales.Click
        Response.Redirect("OnlineSalesComparison.aspx")
    End Sub

    'Private Sub imBtnGroupAccess_Click(sender As Object, e As ImageClickEventArgs) Handles imBtnGroupAccess.Click
    '    Response.Redirect("GroupAccess.aspx")
    'End Sub

    '<WebMethod()>
    'Public Shared Function PerformLogout() As String
    '    ' PerformLogout is a Shared (static) method, so it can't directly access 
    '    ' the current page's members (like Me, Session, etc.) directly. 
    '    ' You must use the HttpContext.Current.Session.

    '    Try
    '        ' Ensure the Session exists and contains the necessary data
    '        Dim authSessionValue As Object = System.Web.HttpContext.Current.Session("AuthSession")

    '        If authSessionValue IsNot Nothing Then
    '            ' Replicate the Logout logic here
    '            Dim mclsAPI As New clsPrismAPI()
    '            mclsAPI.authSession = authSessionValue
    '            mclsAPI.LogoutWebClient()

    '            System.Web.HttpContext.Current.Session("AuthSession") = Nothing
    '        End If

    '        Return "Logout Successful" ' Return a result, though the client won't usually wait for it

    '    Catch ex As Exception
    '        ' Log the exception here instead of trying to show an alert to the user,
    '        ' as the browser is closing/navigating away.
    '        System.Diagnostics.Trace.TraceError("Error during PerformLogout: " & ex.Message)
    '        Return "Logout Failed: " & ex.Message
    '    End Try
    'End Function

End Class