'Imports System.ComponentModel
Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Controls.Primitives

Imports OfficeOpenXml
Imports OfficeOpenXml.Style

Public Class clsModules

    Public Sub ExportToExcel_EPPlus(dtExcelData As DataTable, strFilePath As String)

        Try

            ExcelPackage.License.SetNonCommercialPersonal("ehly")

            Using package As New ExcelPackage()
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add("Sheet1")

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

                'worksheet.Columns.AutoFit()

                package.SaveAs(New FileInfo(strFilePath))

                package.Dispose()

            End Using

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub ExportToCSV(ByVal dataTable As System.Data.DataTable, ByVal filePath As String)
        Try
            Dim stringBuilder As New StringBuilder
            'Dim encoding As Encoding = encoding.UTF8

            Dim includeHeaders As Boolean = True, delimiter As String = "|"

            If includeHeaders Then
                For colIndex As Integer = 0 To dataTable.Columns.Count - 1
                    stringBuilder.Append(dataTable.Columns(colIndex).ColumnName)
                    If colIndex < dataTable.Columns.Count - 1 Then
                        stringBuilder.Append(delimiter)
                    End If
                Next
                stringBuilder.AppendLine()
            End If

            For rowIndex As Integer = 0 To dataTable.Rows.Count - 1
                For colIndex As Integer = 0 To dataTable.Columns.Count - 1
                    Dim cellValue As String = dataTable.Rows(rowIndex).Item(colIndex).ToString()
                    stringBuilder.Append(dataTable.Rows(rowIndex).Item(colIndex).ToString())

                    If colIndex < dataTable.Columns.Count - 1 Then
                        stringBuilder.Append(delimiter)
                    End If
                Next
                stringBuilder.AppendLine() ' Add newline after each row
            Next

            File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub ReadTextFile(ByVal strfileFullPath As String, ByRef dt As DataTable)
        Try

            Dim i As Integer
            Dim rows As String(), lineCount As Long

            lineCount = File.ReadAllLines(strfileFullPath).Length - 1

            Using reader As New StreamReader(strfileFullPath)

                While Not reader.EndOfStream
                    If i > 0 Then
                        rows = reader.ReadLine().Split(";"c)
                        dt.Rows.Add(rows)
                    Else
                        rows = reader.ReadLine().Split(";"c)
                    End If
                    i += 1
                End While
            End Using

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function Import_ExceFile(ByVal excelFilePath As String, Optional strSheetName As String = "") As DataTable

        Dim excelConnection As New System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & excelFilePath & ";Extended Properties='Excel 12.0 Xml;HDR=YES;FMT=Delimited'")
        Try
            excelConnection.Open()

            Dim sheetName As String

            If strSheetName = "" Then
                sheetName = GetFirstSheetName(excelConnection)
            Else
                sheetName = strSheetName
            End If

            Dim dtExcel As New System.Data.DataTable

            Dim sqlCommand As New System.Data.OleDb.OleDbCommand("SELECT * FROM [" & sheetName & "]", excelConnection)
            Dim dataAdapter As New System.Data.OleDb.OleDbDataAdapter(sqlCommand)
            dataAdapter.Fill(dtExcel)

            Return dtExcel
        Catch ex As Exception
            Throw ex
        End Try

    End Function

    Private Function GetFirstSheetName(ByVal connection As System.Data.OleDb.OleDbConnection) As String
        Try
            Dim dt As System.Data.DataTable

            dt = connection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, Nothing)
            Return dt.Rows(0).Item("TABLE_NAME").ToString()

        Catch ex As Exception
            Throw ex
        End Try

    End Function


    Public Function CreateORacleTempTablePO() As String
        Dim strCreateTableOraPO As String
        strCreateTableOraPO = "CREATE TABLE XXJED_ORA_PO_RECEIPTS_IMP(" _
                              & "STORE_CODE           VARCHAR2(6 BYTE)         NOT NULL," _
                              & "ORA_PO_REFERENCE     VARCHAR2(20 BYTE)," _
                              & "CREATED_DATE         DATE," _
                              & "ITEM_CODE            VARCHAR2(20 BYTE)        NOT NULL," _
                              & "ITEM_QUANTITY        NUMBER                   NOT NULL," _
                              & "UNIT_PRICE           NUMBER," _
                              & "ORA_VENDOR_NUMBER    NUMBER," _
                              & "UNIT_OF_MEASURE      VARCHAR2(3 BYTE)," _
                              & "STATUS_FLAG          NUMBER(1)                NOT NULL," _
                              & "ERROR_MESSAGE        VARCHAR2(2000 BYTE)," _
                              & "SEQ                  VARCHAR2(1 BYTE)," _
                              & "ORA_SHIPMENT_NO      VARCHAR2(20 BYTE)," _
                              & "ORA_VENDOR_ASN       VARCHAR2(60 BYTE)        NOT NULL," _
                              & "BOX_NUMBER           VARCHAR2(50 BYTE)        NOT NULL," _
                              & "STATUS_DATE          TIMESTAMP(6)," _
                              & "SUBSIDIARY_NO        NUMBER(5)," _
                              & "SUBMITTED_TO_ORACLE  CHAR(1 BYTE)," _
                              & "RP_FLAG              CHAR(1 BYTE)," _
                              & "RP_VENDOR_CODE       VARCHAR2(6 BYTE)," _
                              & "PR_PO_NO             VARCHAR2(20 BYTE)," _
                              & "PR_ASN_NO            VARCHAR2(20 BYTE)," _
                              & "EMAIL_SENT           VARCHAR2(1 BYTE)," _
                              & "VENDOR_ASN_TOTALQTY  NUMBER)"

        Return strCreateTableOraPO
    End Function

    Public Function CreateTempTableItems()
        Dim strCreateTable_TMPItems As String

        strCreateTable_TMPItems = "CREATE TABLE XXJED_TMPITEMS(" _
                              & "SBS_NO             NUMBER(5)                  NOT NULL," _
                              & "VEND_CODE          NVARCHAR2(20)              NOT NULL," _
                              & "DESCRIPTION1       NVARCHAR2(30)              NOT NULL," _
                              & "DESCRIPTION3       NVARCHAR2(30)," _
                              & "ALU                NVARCHAR2(20)," _
                              & "UPC                NUMBER(18)                 NOT NULL," _
                              & "ATTRIBUTE          NVARCHAR2(8)," _
                              & "UDF1_FLOAT         NUMBER(10)," _
                              & "UDF3_STRING        NVARCHAR2(100)," _
                              & "UDF4_STRING        NVARCHAR2(100)," _
                              & "ITEM_SIZE          NVARCHAR2(8)," _
                              & "UDF9_STRING        NVARCHAR2(50)," _
                              & "UDF10_STRING       NVARCHAR2(50)," _
                              & "DCS_CODE           NVARCHAR2(20)              NOT NULL," _
                              & "UDF1_STRING        NVARCHAR2(100)," _
                              & "UDF5_STRING        NVARCHAR2(100)," _
                              & "DESCRIPTION2       NVARCHAR2(30)," _
                              & "UDF2_STRING        NVARCHAR2(100)," _
                              & "UDF8_STRING        NVARCHAR2(50)," _
                              & "DESCRIPTION4       NVARCHAR2(30)," _
                              & "PRICE2             NUMBER(16,4)," _
                              & "PRICE1             NUMBER(16,4), " _
                              & "UDF15_STRING       NVARCHAR2(50), " _
                              & "UDF14_STRING       NVARCHAR2(50)," _
                              & "UDF1_LARGE_STRING  NVARCHAR2(2000)," _
                              & "UDF6_STRING        NVARCHAR2(50)," _
                              & "UDF13_STRING       NVARCHAR2(50)," _
                              & "UDF12_STRING       NVARCHAR2(50)," _
                              & "ACTIVE             NUMBER(1)                  NOT NULL," _
                              & "MAX_DISC_PERC1     NUMBER(16,4)               NOT NULL," _
                              & "MAX_DISC_PERC2     NUMBER(16,4)               NOT NULL," _
                              & "SERIAL_TYPE        NUMBER(1)                  NOT NULL," _
                              & "LOT_TYPE           NUMBER(1)                  NOT NULL," _
                              & "KIT_TYPE           NUMBER(1)                  NOT NULL," _
                              & "NON_INVENTORY      NUMBER(1)                  NOT NULL," _
                              & "ORDERABLE          NUMBER(1)                  NOT NULL," _
                              & "STATUS_FLAG        NUMBER(1)                  NOT NULL," _
                              & "ACTION             VARCHAR2(1 BYTE)           NOT NULL," _
                              & "D_NAME             VARCHAR2(30 BYTE)," _
                              & "C_NAME             VARCHAR2(30 BYTE)," _
                              & "S_NAME             VARCHAR2(30 BYTE))"

        Return strCreateTable_TMPItems
    End Function

    Public Function CreateTempTableNewAttribute()
        Dim strCreateTable_TMPNewAttr As String

        strCreateTable_TMPNewAttr = "CREATE TABLE XXJED_TMPNEWATTR(" _
                                  & "NEW_BARCODE        VARCHAR2(50 BYTE)," _
                                  & "NEW_PRODUCT_CODE   VARCHAR2(50 BYTE)," _
                                  & "NEW_COLOUR_CODE    VARCHAR2(50 BYTE)," _
                                  & "NEW_SIZE_CODE      VARCHAR2(100 BYTE)," _
                                  & "NEW_DCS_CODE       VARCHAR2(15 BYTE)," _
                                  & "NEW_DEPT_NAME      VARCHAR2(20 BYTE)," _
                                  & "NEW_CLASS_NAME     VARCHAR2(30 BYTE)," _
                                  & "NEW_SUBCLASS_NAME  VARCHAR2(30 BYTE)," _
                                  & "OLD_BARCODE        NUMBER(18)," _
                                  & "OLD_PRODUCT_CODE   NVARCHAR2(30)," _
                                  & "OLD_COLOUR_CODE    NVARCHAR2(8)," _
                                  & "OLD_SIZE_CODE      NVARCHAR2(8)," _
                                  & "OLD_DCS_CODE       NVARCHAR2(9) NOT NULL," _
                                  & "OLD_DEPT_NAME      NVARCHAR2(20)," _
                                  & "OLD_CLASS_NAME     NVARCHAR2(20)," _
                                  & "OLD_SUBCLASS_NAME  NVARCHAR2(20)," _
                                  & "DIFRNT_STYLE       VARCHAR2(1 BYTE)," _
                                  & "DIFRNT_COLOR       VARCHAR2(1 BYTE)," _
                                  & "DIFRNT_SIZE        VARCHAR2(1 BYTE)," _
                                  & "DIFRNT_DEPT        VARCHAR2(1 BYTE)," _
                                  & "DIFRNT_CLASS       VARCHAR2(1 BYTE)," _
                                  & "DIFRNT_SUBCLASS    VARCHAR2(1 BYTE)," _
                                  & "DIFRNT_BARCODE     CHAR(1 BYTE))"

        Return strCreateTable_TMPNewAttr
    End Function

    Public Function CreateTableCSVItemCreate() As String
        Return "CREATE TABLE XXJED_CSVITEMCREATE_IMP(" _
                & "BARCODE                 NUMBER(18)            NOT NULL," _
                & "DEPT_ID                 NVARCHAR2(20)," _
                & "DEPT_NAME               NVARCHAR2(20)," _
                & "CLASS_ID                NVARCHAR2(20)," _
                & "CLASS_NAME              NVARCHAR2(20)," _
                & "SUBCLASS_ID             NVARCHAR2(20)," _
                & "SUBCLASS_NAME           NVARCHAR2(20)," _
                & "VENDOR_CODE             NVARCHAR2(10)," _
                & "ALU                     NVARCHAR2(20)," _
                & "PRODUCT_CODE            NVARCHAR2(20)," _
                & "COLOUR_CODE             NVARCHAR2(20)," _
                & "ENGLISH_COLOUR_WORDING  NVARCHAR2(30)," _
                & "ENGLISH_SIZE_WORDING    NVARCHAR2(30)," _
                & "BP_SEASON_CODE          VARCHAR2(10 BYTE)," _
                & "SEASON_CODE             VARCHAR2(10 BYTE)," _
                & "SEASON_DESCRIPTION      NVARCHAR2(30)," _
                & "FAMILY_CODE             NVARCHAR2(100)," _
                & "ENGLISH_FAMILY_WORDING  NVARCHAR2(30)," _
                & "ARABIC_DESCRIPTION      NVARCHAR2(30)," _
                & "LINE_OF_PRODUCT         NVARCHAR2(100)," _
                & "SUBFAMILY_CODE          NVARCHAR2(50)," _
                & "ENGLISHDESCRIPTION      NVARCHAR2(50)," _
                & "PRODUCT_WORDING         NVARCHAR2(50)," _
                & "SALES_PRICE             VARCHAR2(10)," _
                & "ACTION                  VARCHAR2(1 BYTE))"
    End Function

    Public function CreateTableJACItemMasterIMP()  As String

        return "CREATE TABLE XXJED_JAC_MASTER_ITEMS_IMP(" _
             & "ITEM_BARCODE            VARCHAR2(50 BYTE)," _
                & "PRODUCT_CODE            VARCHAR2(50 BYTE)," _
                & "PRODUCT_WORDING         VARCHAR2(128 BYTE)," _
                & "COLOUR_CODE             VARCHAR2(50 BYTE)," _
                & "FRENCH_COLOUR_WORDING   VARCHAR2(100 BYTE)," _
                & "ENGLISH_COLOUR_WORDING  VARCHAR2(100 BYTE)," _
                & "FRENCH_SIZE_WORDING     VARCHAR2(100 BYTE)," _
                & "ENGLISH_SIZE_WORDING    VARCHAR2(100 BYTE)," _
                & "SALES_PRICE             NUMBER(12,2)," _
                & "START_VALIDITY_DATE     DATE," _
                & "SEASON_CODE             VARCHAR2(30 BYTE)," _
                & "LINE_OF_PRODUCT         VARCHAR2(30 BYTE)," _
                & "OFFER_STRUCTURE         VARCHAR2(30 BYTE)," _
                & "THEME_NAME              VARCHAR2(30 BYTE)," _
                & "ZONING_CODE             VARCHAR2(30 BYTE)," _
                & "STYLISTIC_THEME_NAME    VARCHAR2(30 BYTE)," _
                & "TARGET                  VARCHAR2(30 BYTE)," _
                & "AGE_TARGET_ENG          VARCHAR2(30 BYTE)," _
                & "GENDER_TARGET           VARCHAR2(30 BYTE)," _
                & "GENDER_TARGET_ENG       VARCHAR2(30 BYTE)," _
                & "BRAND_NAME              VARCHAR2(30 BYTE)," _
                & "SUBFAMILY_CODE          VARCHAR2(30 BYTE)," _
                & "FRENCHDESCRIPTION       VARCHAR2(300 BYTE)," _
                & "ENGLISHDESCRIPTION      VARCHAR2(300 BYTE)," _
                & "FAMILYCODE              VARCHAR2(30 BYTE)," _
                & "MAINFAMILYDESC_F        VARCHAR2(100 BYTE)," _
                & "MAINFAMILYDESC_E        VARCHAR2(100 BYTE)," _
                & "BRANDCODE               VARCHAR2(30 BYTE)," _
                & "BRANDNAME_F             VARCHAR2(100 BYTE)," _
                & "BRANDNAME_E             VARCHAR2(100 BYTE)," _
                & "ARABIC_DESCRIPTION3     VARCHAR2(250 BYTE)," _
                & "SKU_ARTICLE             NUMBER," _
                & "SBS_NO                  VARCHAR2(2 BYTE)," _
                & "DCS_CODE                VARCHAR2(15 BYTE)," _
                & "VENDOR_CODE             VARCHAR2(10 BYTE)," _
                & "UDF3_STRING             VARCHAR2(30 BYTE)," _
                & "UDF4_STRING             VARCHAR2(30 BYTE)," _
                & "D_NAME                  VARCHAR2(20 BYTE)," _
                & "C_NAME                  VARCHAR2(30 BYTE)," _
                & "S_NAME                  VARCHAR2(30 BYTE)," _
                & "UDF1_FLOAT              VARCHAR2(10 BYTE))"

    End function

    Public function CreateTableJACNewAttr As String 
        Return "CREATE TABLE XXJED_JAC_NEWATTR_IMP(" _
                & "NEW_BARCODE        VARCHAR2(50 BYTE)," _
                & "NEW_PRODUCT_CODE   VARCHAR2(50 BYTE)," _
                & "NEW_COLOUR_CODE    VARCHAR2(50 BYTE)," _
                & "NEW_SIZE_CODE      VARCHAR2(100 BYTE)," _
                & "NEW_DCS_CODE       VARCHAR2(15 BYTE)," _
                & "NEW_DEPT_NAME      VARCHAR2(20 BYTE)," _
                & "NEW_CLASS_NAME     VARCHAR2(30 BYTE)," _
                & "NEW_SUBCLASS_NAME  VARCHAR2(30 BYTE)," _
                & "OLD_BARCODE        NUMBER(18)," _
                & "OLD_PRODUCT_CODE   NVARCHAR2(30)," _
                & "OLD_COLOUR_CODE    NVARCHAR2(8)," _
                & "OLD_SIZE_CODE      NVARCHAR2(8)," _
                & "OLD_DCS_CODE       NVARCHAR2(9) NOT NULL," _
                & "OLD_DEPT_NAME      NVARCHAR2(20)," _
                & "OLD_CLASS_NAME     NVARCHAR2(20)," _
                & "OLD_SUBCLASS_NAME  NVARCHAR2(20)," _
                & "DIFRNT_STYLE       VARCHAR2(1 BYTE)," _
                & "DIFRNT_COLOR       VARCHAR2(1 BYTE)," _
                & "DIFRNT_SIZE        VARCHAR2(1 BYTE)," _
                & "DIFRNT_DEPT        VARCHAR2(1 BYTE)," _
                & "DIFRNT_CLASS       VARCHAR2(1 BYTE)," _
                & "DIFRNT_SUBCLASS    VARCHAR2(1 BYTE)," _
                & "DIFRNT_BARCODE     CHAR(1 BYTE))"
    End function


    Public Function CreateTableNocItems() As String
        Return "CREATE TABLE XXTMP_NOCITEMS
                            (
                              SBS_NO            NUMBER(5)                   NOT NULL,
                              UPC               NUMBER(18)                  NOT NULL,
                              ALU               NVARCHAR2(20),
                              DEPT_ID           VARCHAR2(3 BYTE),
                              DEPT_NAME         VARCHAR2(20 BYTE),
                              CLASS_ID          VARCHAR2(3 BYTE),
                              CLASS_NAME        VARCHAR2(20 BYTE),
                              SUBCLASS_ID       VARCHAR2(3 BYTE),
                              SUBCLASS_NAME     VARCHAR2(20 BYTE),
                              VEND_CODE         NVARCHAR2(20)               NOT NULL,
                              DESCRIPTION1      NVARCHAR2(30)               NOT NULL,
                              DESCRIPTION2      NVARCHAR2(30),
                              DESCRIPTION3      NVARCHAR2(30),
                              DESCRIPTION4      NVARCHAR2(30),
                              ATTRIBUTE         NVARCHAR2(8),
                              UDF1_FLOAT        NUMBER(10),
                              ITEM_SIZE         NVARCHAR2(8),
                              UDF1_STRING       NVARCHAR2(100),
                              UDF3_STRING       NVARCHAR2(100),
                              UDF4_STRING       NVARCHAR2(100),
                              UDF6_STRING       NVARCHAR2(50),
                              UDF11_STRING      NVARCHAR2(50),
                              LONG_DESCRIPTION  NVARCHAR2(2000),
                              TEXT1             NVARCHAR2(255),
                              TEXT2             NVARCHAR2(255),
                              TEXT3             NVARCHAR2(255),
                              TEXT4             NVARCHAR2(255),
                              TEXT5             NVARCHAR2(255),
                              TEXT6             NVARCHAR2(255),
                              PRICE1            NUMBER(16,4),
                              PRICE2            NUMBER(16,4),
                              UDF14_STRING      NVARCHAR2(50),
                              ACTION            VARCHAR2(1 BYTE)            NOT NULL
                            )"
    End Function

    Public Function CreateTableCSVItemUpdate() As String
        Return "CREATE TABLE XXJED_CSVITEMUPDATE_IMP(" _
                & "UPC                     NUMBER(18)            NOT NULL," _
                & "ENGLISH_FAMILY_WORDING  NVARCHAR2(30)," _
                & "ARABIC_DESCRIPTION      NVARCHAR2(30)," _
                & "SEASON_CODE             VARCHAR2(10 BYTE)," _
                & "LINE_OF_PRODUCT         NVARCHAR2(100)," _
                & "BP_SEASON_CODE          NVARCHAR2(100)," _
                & "FAMILY_CODE             NVARCHAR2(100)," _
                & "SUBFAMILY_CODE          NVARCHAR2(50)," _
                & "ENGLISHDESCRIPTION      NVARCHAR2(50)," _
                & "PRODUCT_WORDING         NVARCHAR2(50)," _
                & "ACTION                  VARCHAR2(1 BYTE))"
    End Function

    Public Function CreateTableNoctureItemUpdate() As String
        Return "CREATE TABLE XXTMP_NOCITEMUPD
                ( UPC               NUMBER(18)                  NOT NULL,
                    ALU               NVARCHAR2(20),
                    DESCRIPTION2      NVARCHAR2(30),
                    DESCRIPTION3      NVARCHAR2(30),
                    UDF1_FLOAT        NUMBER(10),
                    UDF1_STRING       NVARCHAR2(100),
                    UDF3_STRING       NVARCHAR2(100),
                    UDF4_STRING       NVARCHAR2(100),
                    UDF6_STRING       NVARCHAR2(50),
                    UDF11_STRING      NVARCHAR2(50),
                    LONG_DESCRIPTION  NVARCHAR2(2000),
                    TEXT1             NVARCHAR2(255),
                    TEXT2             NVARCHAR2(255),
                    TEXT3             NVARCHAR2(255),
                    TEXT4             NVARCHAR2(255),
                    TEXT5             NVARCHAR2(255),
                    TEXT6             NVARCHAR2(255),
                    ACTION            VARCHAR2(1 BYTE)            NOT NULL
                )"
    End Function

    Public Function CreateTableItemUpdate() As String
        Return "CREATE TABLE XXJED_ITEMUPDATE_IMP(" _
                & "SBS_NO             NUMBER(5)," _
                & "UPC                NUMBER(18)," _
                & "ALU                NVARCHAR2(20)," _
                & "DCS_CODE           NVARCHAR2(9)," _
                & "VEND_CODE          NVARCHAR2(6)," _
                & "DESCRIPTION1       NVARCHAR2(30)," _
                & "DESCRIPTION2       NVARCHAR2(30)," _
                & "DESCRIPTION3       NVARCHAR2(30)," _
                & "DESCRIPTION4       NVARCHAR2(30)," _
                & "ATTRIBUTE          NVARCHAR2(8)," _
                & "COST               NUMBER(16,4)," _
                & "TAX_CODE           NUMBER," _
                & "UDF1_FLOAT         VARCHAR2(10 BYTE)," _
                & "UDF2_FLOAT         NUMBER(10)," _
                & "UDF3_FLOAT         NUMBER(10)," _
                & "UDF1_DATE          TIMESTAMP(0) WITH TIME ZONE," _
                & "UDF2_DATE          TIMESTAMP(0) WITH TIME ZONE," _
                & "UDF3_DATE          TIMESTAMP(0) WITH TIME ZONE," _
                & "ITEM_SIZE          NVARCHAR2(8)," _
                & "UDF1_STRING        NVARCHAR2(100)," _
                & "UDF2_STRING        NVARCHAR2(100)," _
                & "UDF3_STRING        NVARCHAR2(30)," _
                & "UDF4_STRING        NVARCHAR2(100)," _
                & "UDF5_STRING        NVARCHAR2(100)," _
                & "UDF6_STRING        NVARCHAR2(50)," _
                & "UDF7_STRING        NVARCHAR2(50)," _
                & "UDF8_STRING        NVARCHAR2(50)," _
                & "UDF9_STRING        NVARCHAR2(50)," _
                & "UDF10_STRING       NVARCHAR2(50)," _
                & "UDF11_STRING       NVARCHAR2(50)," _
                & "UDF12_STRING       NVARCHAR2(50)," _
                & "UDF13_STRING       NVARCHAR2(50)," _
                & "UDF14_STRING       NVARCHAR2(50)," _
                & "UDF15_STRING       NVARCHAR2(50)," _
                & "UDF1_LARGE_STRING  NVARCHAR2(2000)," _
                & "UDF2_LARGE_STRING  NVARCHAR2(2000)," _
                & "FST_PRICE          NUMBER(16,4)," _
                & "DESCRIPTION        NVARCHAR2(30)," _
                & "ACTIVE             NUMBER(1)," _
                & "QTY_PER_CASE       NUMBER(10,3)," _
                & "MAX_DISC_PERC1     NUMBER(16,4)," _
                & "MAX_DISC_PERC2     NUMBER(16,4)," _
                & "ITEM_NO            NUMBER(10)," _
                & "SERIAL_TYPE        NUMBER(1)," _
                & "LOT_TYPE           NUMBER(1)," _
                & "KIT_TYPE           NUMBER(1)," _
                & "NON_INVENTORY      NUMBER(1)," _
                & "ORDERABLE          NUMBER(1)," _
                & "LONG_DESCRIPTION   NVARCHAR2(2000)," _
                & "TEXT1              NVARCHAR2(255)," _
                & "TEXT2              NVARCHAR2(255)," _
                & "TEXT3              NVARCHAR2(255)," _
                & "TEXT4              NVARCHAR2(255)," _
                & "TEXT5              NVARCHAR2(255)," _
                & "TEXT6              NVARCHAR2(255)," _
                & "TEXT7              NVARCHAR2(255)," _
                & "TEXT8              NVARCHAR2(255)," _
                & "HEIGHT             NUMBER(7,2)," _
                & "LENGTH             NUMBER(7,2)," _
                & "WIDTH              NUMBER(7,2)," _
                & "PRICE1             NUMBER(16,4)," _
                & "PRICE2             NUMBER(16,4)," _
                & "SELLABLE_DATE      DATE," _
                & "ACTION             VARCHAR2(1 BYTE))"
    End Function

    Public Function CreateTable_TMPItems() As String
        Return "CREATE TABLE XXJED_TMPITEMS(" _
                & "SBS_NO             NUMBER(5)                  NOT NULL," _
                & "VEND_CODE          NVARCHAR2(20)              NOT NULL," _
                & "DESCRIPTION1       NVARCHAR2(30)              NOT NULL," _
                & "DESCRIPTION3       NVARCHAR2(30)," _
                & "ALU                NVARCHAR2(20)," _
                & "UPC                NUMBER(18)                 NOT NULL," _
                & "ATTRIBUTE          NVARCHAR2(8)," _
                & "UDF1_FLOAT         NUMBER(10)," _
                & "UDF3_STRING        NVARCHAR2(100)," _
                & "UDF4_STRING        NVARCHAR2(100)," _
                & "ITEM_SIZE          NVARCHAR2(8)," _
                & "UDF9_STRING        NVARCHAR2(50)," _
                & "UDF10_STRING       NVARCHAR2(50)," _
                & "DCS_CODE           NVARCHAR2(20)              NOT NULL," _
                & "UDF1_STRING        NVARCHAR2(100)," _
                & "UDF5_STRING        NVARCHAR2(100)," _
                & "DESCRIPTION2       NVARCHAR2(30)," _
                & "UDF2_STRING        NVARCHAR2(100)," _
                & "UDF8_STRING        NVARCHAR2(50)," _
                & "DESCRIPTION4       NVARCHAR2(30)," _
                & "PRICE2             NUMBER(16,4)," _
                & "PRICE1             NUMBER(16,4), " _
                & "UDF15_STRING       NVARCHAR2(50), " _
                & "UDF14_STRING       NVARCHAR2(50)," _
                & "UDF1_LARGE_STRING  NVARCHAR2(2000)," _
                & "UDF6_STRING        NVARCHAR2(50)," _
                & "UDF13_STRING       NVARCHAR2(50)," _
                & "UDF12_STRING       NVARCHAR2(50)," _
                & "ACTIVE             NUMBER(1)                  NOT NULL," _
                & "MAX_DISC_PERC1     NUMBER(16,4)               NOT NULL," _
                & "MAX_DISC_PERC2     NUMBER(16,4)               NOT NULL," _
                & "SERIAL_TYPE        NUMBER(1)                  NOT NULL," _
                & "LOT_TYPE           NUMBER(1)                  NOT NULL," _
                & "KIT_TYPE           NUMBER(1)                  NOT NULL," _
                & "NON_INVENTORY      NUMBER(1)                  NOT NULL," _
                & "ORDERABLE          NUMBER(1)                  NOT NULL," _
                & "STATUS_FLAG        NUMBER(1)                  NOT NULL," _
                & "ACTION             VARCHAR2(1 BYTE)           NOT NULL," _
                & "D_NAME             VARCHAR2(30 BYTE)," _
                & "C_NAME             VARCHAR2(30 BYTE)," _
                & "S_NAME             VARCHAR2(30 BYTE))"
    End Function

    Public Function CreateTable_TMPNewAttr() As String
        Return "CREATE TABLE XXJED_TMPNEWATTR(" _
                & "NEW_BARCODE        VARCHAR2(50 BYTE)," _
                & "NEW_PRODUCT_CODE   VARCHAR2(50 BYTE)," _
                & "NEW_COLOUR_CODE    VARCHAR2(50 BYTE)," _
                & "NEW_SIZE_CODE      VARCHAR2(100 BYTE)," _
                & "NEW_DCS_CODE       VARCHAR2(15 BYTE)," _
                & "NEW_DEPT_NAME      VARCHAR2(20 BYTE)," _
                & "NEW_CLASS_NAME     VARCHAR2(30 BYTE)," _
                & "NEW_SUBCLASS_NAME  VARCHAR2(30 BYTE)," _
                & "OLD_BARCODE        NUMBER(18)," _
                & "OLD_PRODUCT_CODE   NVARCHAR2(30)," _
                & "OLD_COLOUR_CODE    NVARCHAR2(8)," _
                & "OLD_SIZE_CODE      NVARCHAR2(8)," _
                & "OLD_DCS_CODE       NVARCHAR2(9) NOT NULL," _
                & "OLD_DEPT_NAME      NVARCHAR2(20)," _
                & "OLD_CLASS_NAME     NVARCHAR2(20)," _
                & "OLD_SUBCLASS_NAME  NVARCHAR2(20)," _
                & "DIFRNT_STYLE       VARCHAR2(1 BYTE)," _
                & "DIFRNT_COLOR       VARCHAR2(1 BYTE)," _
                & "DIFRNT_SIZE        VARCHAR2(1 BYTE)," _
                & "DIFRNT_DEPT        VARCHAR2(1 BYTE)," _
                & "DIFRNT_CLASS       VARCHAR2(1 BYTE)," _
                & "DIFRNT_SUBCLASS    VARCHAR2(1 BYTE)," _
                & "DIFRNT_BARCODE     CHAR(1 BYTE))"
    End Function
End Class
