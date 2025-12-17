<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PISummaryReport.aspx.vb" Inherits="AseelahWebApps.PISummaryReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Showroom Count Report</title>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 20px;
            direction: rtl; /* Set direction to right-to-left for Arabic text */
        }

        .report-container {
            width: 900px;
            margin: auto;
            border: 1px solid #000;
            padding: 20px;
        }

        .header-title {
            text-align: center;
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 20px;
            border-bottom: 1px solid #000;
            padding-bottom: 10px;
        }

        .header-info {
            display: flex;
            justify-content: space-between;
            margin-bottom: 20px;
        }

            .header-info div {
                flex: 1;
            }

        .info-left {
            text-align: left;
            font-size: 12px;
            direction: ltr; /* Override for English text */
        }

        .info-right {
            text-align: right;
            font-size: 12px;
        }

        .info-center {
            text-align: center;
            font-size: 12px;
        }

        .signatures {
            display: flex;
            justify-content: space-around;
            text-align: center;
            margin-top: 50px;
            width: 100%;
        }

        .signature-box {
            border: 1px solid #000;
            padding: 10px;
            width: 40%;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
        }

        .signature-line {
            width: 80%;
            border-bottom: 1px dashed #000;
            margin-top: 50px;
            margin-bottom: 10px;
        }

        .signature-text {
            font-size: 12px;
        }

        .disclaimer {
            font-size: 12px;
            line-height: 1.5;
            margin-top: 30px;
            text-align: right;
            border-top: 1px solid #000;
            padding-top: 10px;
            direction: rtl;
        }

        .disclaimer-en {
            direction: ltr;
            text-align: left;
            margin-top: 20px;
            font-style: italic;
        }

        .gridview-container {
            width: 100%;
            border: 1px solid #000;
            margin-top: 20px;
            padding: 10px;
        }

        .data-table {
            width: 100%;
            border-collapse: collapse;
            text-align: center;
            margin-top: 20px;
        }

            .data-table th, .data-table td {
                border: 1px solid #000;
                padding: 8px;
                white-space: nowrap;
            }

            .data-table th {
                background-color: #f2f2f2;
                font-weight: bold;
            }

        .system-qty-cell {
            color: red;
        }

        #exportBtn {
            padding: 10px 20px;
            background-color: #4CAF50;
            color: white;
            border: none;
            cursor: pointer;
            margin-bottom: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <button id="exportBtn" onclick="exportToPdf(); return false;">Export to PDF</button>

        <div class="report-container">
            <div class="header-title">
                محضر جرد معرض تفصيلي
                <br />
                <asp:Label ID="lblTitle" runat="server" Text="Showroom Count Form"></asp:Label>
            </div>

            <div class="header-info">
                <div class="info-left" style="float: left;">
                    Name: Machka - Red Sea Opening Count<br />
                    Created Date: Jan 16, 25 4:07 PM<br />
                    Created By: 23020815<br />
                    Modified Date: Jan 16, 25 4:11 PM<br />
                    Exported: Apr 16, 25 1:10 PM
                </div>
                <div style="float: right;  direction: ltr;">
                    <div style="display:inline-block;">
                        <p style="font-size: 12px; margin: 0px; border:solid; border-width:1px;">TIME STARTED</p>
                        <p style="font-size: 12px; margin: 0px; border:solid; border-width:1px;">FINISHED SCANNING</p>
                        <p style="font-size: 12px; margin: 0px; border:solid; border-width:1px;">TIME FINISHED</p>
                        <p style="font-size: 12px; margin: 0px; border:solid; border-width:1px;">STORE NAME</p>
                    </div>
                    <div style="display:inline-block;">
                        <p style="font-size: 12px;margin: 0px; border:solid; border-width:1px; width:100px;"></p>
                        <p style="font-size: 12px;margin: 0px;border:solid; border-width:1px; width:100px;"></p>
                        <p style="font-size: 12px;margin: 0px;border:solid; border-width:1px; width:100px;"></p>
                        <p style="font-size: 12px;margin: 0px;border:solid; border-width:1px; width:100px;"></p>
                    </div>

                </div>


                <div class="info-right" style="clear: both;">
                    <%= DateTime.Now.ToString("MMM dd, yy hh:mm tt") %> :في يوم الموافق<br />
                    <%= DateTime.Now.ToString("MMM dd, yy hh:mm tt") %> :تاريخ الجرد<br />
                    Machka - Red Sea Mall Jeddah
                </div>
            </div>

            <p style="text-align: center;">وكالة تلبية الجرد بالمرفق وملاحقتها على النحو التالي:</p>

            <div class="gridview-container">
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="data-table">
                    <Columns>
                        <asp:BoundField DataField="SellingPrice" HeaderText="Diff. Selling Price" />
                        <asp:BoundField DataField="FinalResult" HeaderText="اجمالي الفروقات - النتيجة النهائية" />
                        <asp:BoundField DataField="ScannedQty" HeaderText="الكمية الجرد الفعلي" />
                        <asp:BoundField DataField="SystemQty" HeaderText="الكمية السابقة على النظام" ItemStyle-CssClass="system-qty-cell" />
                    </Columns>
                </asp:GridView>
            </div>

            <div style="display: flex; justify-content: space-between; margin-top: 20px;">
                <p class="disclaimer">
                    نقر أنا مدير الفرع المسؤول عن الجرد أنه لا يوجد أي مخلفات خلاف ما ذكر في هذا المحضر ومسئوليتي مسئولية كاملة عن البضاعة داخل معرض الفرع حسب ما تم جرده فعليا.
                </p>
                <p class="disclaimer-en">
                    That the quantity scanned is accurate & final, that there will<br />
                    be no pending claims on Inventory regarding items that were<br />
                    not scanned in accurate way. All Sections/Boxes were recounted by us.<br />
                    & we will be responsible for any missing/extra items as the result of the Inventory
                </p>
            </div>

            <div class="signatures">
                <div class="signature-box">
                    <p>رئيس لجنة الجرد</p>
                    <p style="text-align: left; width: 100%;">الاسم / ..................</p>
                    <p style="text-align: left; width: 100%;">التوقيع / ..................</p>
                    <div class="signature-line"></div>
                    <p class="signature-text">
                        (اسم و توقيع مدير المعرض (المستلم للعهدة
                    </p>
                </div>
                <div class="signature-box">
                    <p>رئيس لجنة الجرد</p>
                    <p style="text-align: left; width: 100%;">الاسم / ..................</p>
                    <p style="text-align: left; width: 100%;">التوقيع / ..................</p>
                    <div class="signature-line"></div>
                    <p class="signature-text">
                        (اسم و توقيع مدير المعرض (المستلم للعهدة
                    </p>
                </div>
            </div>
        </div>
    </form>
</body>

<script>
    function exportToPdf() {
        const { jsPDF } = window.jspdf;
        const element = document.querySelector('.report-container');

        // Hide the button before converting to PDF
        const exportBtn = document.getElementById('exportBtn');
        exportBtn.style.display = 'none';

        html2canvas(element, {
            scale: 2, // Increase scale for higher resolution
            useCORS: true // This is important for images from external sources
        }).then(canvas => {
            const imgData = canvas.toDataURL('image/png');
            const pdf = new jsPDF('p', 'mm', 'a4');
            const imgWidth = 210; // A4 width in mm
            const pageHeight = 297; // A4 height in mm
            const imgHeight = canvas.height * imgWidth / canvas.width;
            let heightLeft = imgHeight;
            let position = 0;

            pdf.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
            heightLeft -= pageHeight;

            while (heightLeft >= 0) {
                position = heightLeft - imgHeight;
                pdf.addPage();
                pdf.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
                heightLeft -= pageHeight;
            }

            pdf.save('Showroom_Report.pdf');

            // Show the button again after conversion
            exportBtn.style.display = 'block';
        });
    }

</script>

</html>
