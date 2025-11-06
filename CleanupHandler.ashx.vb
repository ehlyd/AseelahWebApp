Imports System.Web
Imports System.Web.Services

Public Class CleanupHandler
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim action As String = context.Request("action")

        If action = "LeaveGroupAccess" Then

            Dim mclsSQL As New clsSQLDB
            mclsSQL.OpenDB()
            mclsSQL.ExecuteNonQuery("If OBJECT_ID('_TMPWGA','U') is not null drop table _TMPWGA")
            mclsSQL.CloseDB()

        End If

        ' The handler must return a response, even if the client ignores it.
        context.Response.ContentType = "text/plain"
        context.Response.Write("OK")

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class