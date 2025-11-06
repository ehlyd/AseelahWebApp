Public Class PISummaryReport
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Create a new DataTable
            Dim dt As New DataTable()

            ' Add columns to the DataTable
            dt.Columns.Add("SystemQty", GetType(Integer))
            dt.Columns.Add("ScannedQty", GetType(Integer))
            dt.Columns.Add("FinalResult", GetType(Integer))
            dt.Columns.Add("SellingPrice", GetType(String))

            ' Add sample data rows
            Dim row As DataRow
            row = dt.NewRow()
            row("SystemQty") = -14
            row("ScannedQty") = 808
            row("FinalResult") = 822
            row("SellingPrice") = "Diff. Selling Price" ' This is a header row, not a data value
            dt.Rows.Add(row)

            ' You can add more rows here if needed
            ' row = dt.NewRow()
            ' row("SystemQty") = 10
            ' row("ScannedQty") = 20
            ' row("FinalResult") = 10
            ' dt.Rows.Add(row)

            ' Bind the DataTable to the GridView
            GridView1.DataSource = dt
            GridView1.DataBind()

            ' Handle the header row separately for styling
            If GridView1.Rows.Count > 0 Then
                GridView1.HeaderRow.Visible = False
                Dim headerRow As GridViewRow = New GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal)
                Dim headerCell1 As TableCell = New TableCell()
                Dim headerCell2 As TableCell = New TableCell()
                Dim headerCell3 As TableCell = New TableCell()
                Dim headerCell4 As TableCell = New TableCell()

                headerCell1.Text = "Diff. Selling Price"
                headerCell2.Text = "اجمالي الفروقات - النتيجة النهائية"
                headerCell3.Text = "الكمية الجرد الفعلي"
                headerCell4.Text = "الكمية السابقة على النظام"

                headerCell1.CssClass = "data-table th"
                headerCell2.CssClass = "data-table th"
                headerCell3.CssClass = "data-table th"
                headerCell4.CssClass = "data-table th"

                headerRow.Cells.Add(headerCell1)
                headerRow.Cells.Add(headerCell2)
                headerRow.Cells.Add(headerCell3)
                headerRow.Cells.Add(headerCell4)

                GridView1.Controls(0).Controls.AddAt(0, headerRow)
            End If
        End If
    End Sub

End Class