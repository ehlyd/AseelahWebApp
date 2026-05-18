Imports System.Web.Services

Public Class SiteMaster
    Inherits MasterPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Session("AuthSession") IsNot Nothing Then
                Dim username As String = TryCast(Session("username"), String)

                If Not String.IsNullOrEmpty(username) Then
                    lblUsername.InnerText = username

                    liUserDropdown.Visible = True
                Else
                    liUserDropdown.Visible = False
                End If
            Else
                liUserDropdown.Visible = False
            End If

            'If Not IsPostBack Then
            ShowHideMenu(Me.Page)
            'End If

        End If
    End Sub

    Public Sub SetSecurityVisible(show As Boolean)
        liSecurity.Visible = show
    End Sub

    Public Sub SetSalesVisible(show As Boolean)
        liSales.Visible = show
    End Sub

    Public Sub SetPISummaryVisible(show As Boolean)
        PISummary.Visible = show

        If show = False And ExportItemMaster.Visible = False Then
            liInventory.Visible = False
        Else
            liInventory.Visible = True
        End If
    End Sub

    Public Sub SetInventoryVisible(show As Boolean)
        liInventory.Visible = show
    End Sub

    'Public Sub SetPIDetailVisible(show As Boolean)
    '    PIDetail.Visible = show
    'End Sub

    'Public Sub SetPI(show As Boolean)
    '    PI.Visible = show
    'End Sub

    Public Sub SetExportItemMasterVisible(show As Boolean)
        ExportItemMaster.Visible = show

        If show = False And PISummary.Visible = False Then
            liInventory.Visible = False
        Else
            liInventory.Visible = True
        End If
    End Sub

    Public ReadOnly Property SecurityVisible As Boolean
        Get
            Return liSecurity.Visible
        End Get
    End Property
End Class