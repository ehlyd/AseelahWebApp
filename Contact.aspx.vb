Public Class Contact
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Session("AuthSession") Is Nothing Then
            Response.Redirect("Default.aspx")
        End If
    End Sub
End Class