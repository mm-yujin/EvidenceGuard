using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EvidenceGuard.Core.Interface;
using EvidenceGuard.Core.Models;

using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace EvidenceGuard.Services
{

    /// <summary>
    /// CSV 또는 PDF 로 결과물을 출력하는 서비스 구현 클래스입니다.
    /// PDF 출력은 iTEXT 라이브러리를 사용했습니다.
    /// </summary>


    public class ReportService : IReportService
    {
        public void GenerateCsvReport(string savePath, List<ExtractionResult> results, string hash1, string hash2, double avgCpu, double avgRam)
        {
            var sb = new StringBuilder();

            sb.AppendLine("=== EVIDENCE GUARD FORENSIC REPORT ===");
            sb.AppendLine($"Generated, {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Average CPU Usage, {avgCpu}%");      
            sb.AppendLine($"Average Available RAM, {avgRam} MB"); 
            sb.AppendLine($"Machine Name, {Environment.MachineName}"); 
            sb.AppendLine();

            sb.AppendLine("[ Integrity Verification ]");
            sb.AppendLine($"Primary Hash, {hash1}");
            sb.AppendLine($"Secondary Hash, {hash2}");
            sb.AppendLine();

            sb.AppendLine("파일명,라인,유형,키워드,오프셋,시간,결과");

            foreach (var item in results)
            {
                string fileName = item.FileName.Contains(",") ? $"\"{item.FileName}\"" : item.FileName;
                string keyword = item.Keyword.Contains(",") ? $"\"{item.Keyword}\"" : item.Keyword;

                string resultStatus = item.IsIntegrityVerified ? "PASS" : "FAIL";

                string line = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                    fileName,
                    item.LineNumber,
                    item.TypeName,
                    keyword,
                    item.Offset,
                    item.FoundTime.ToString("HH:mm:ss"),
                    resultStatus);

                sb.AppendLine(line);
            }

            // UTF-8 with BOM (Byte Order Mark) 설정: 엑셀로 열었을 때 한글 깨짐 방지 때문에 잡아두어야 함
            var encoding = new UTF8Encoding(true);
            File.WriteAllText(savePath, sb.ToString(), encoding);
        }

        public void GeneratePdfReport(string savePath, List<ExtractionResult> results, string hash1, string hash2, double avgCpu, double avgRam)
        {
            try
            {
                using (var writer = new PdfWriter(savePath))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf, PageSize.A4))
                {

                    string fontDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                    string normalPath = System.IO.Path.Combine(fontDir, "malgun.ttf");
                    string boldPath = System.IO.Path.Combine(fontDir, "malgunbd.ttf");

                    // 만약 맑은고딕이나 맑은고딕 볼드 파일이 없으면 = 기본 폰트로.
                    if (!System.IO.File.Exists(normalPath)) normalPath = "C:\\Windows\\Fonts\\arial.ttf";
                    if (!System.IO.File.Exists(boldPath)) boldPath = "C:\\Windows\\Fonts\\arial.ttf";

                    // IDENTITY_H 옵션은 한글 깨짐 방지를 위해 필수라고 함
                    PdfFont font = PdfFontFactory.CreateFont(normalPath, PdfEncodings.IDENTITY_H);
                    PdfFont boldFont = PdfFontFactory.CreateFont(boldPath, PdfEncodings.IDENTITY_H);

                    document.SetFont(font);

                    document.Add(new Paragraph("FORENSIC ANALYSIS REPORT")
                        .SetFontSize(22)
                        .SetFontColor(iText.Kernel.Colors.ColorConstants.DARK_GRAY)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFont(boldFont));

                    document.Add(new Paragraph($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").SetFontSize(10));
                    document.Add(new Paragraph($"Machine Name: {Environment.MachineName}").SetFontSize(10));

                    Paragraph perfInfo = new Paragraph($"Analysis Resource Usage -> Avg CPU: {avgCpu}% | Avg Available RAM: {avgRam}MB")
                        .SetFontSize(10)
                        .SetFontColor(iText.Kernel.Colors.ColorConstants.GRAY);
                    document.Add(perfInfo);

                    document.Add(new LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.SolidLine()));

                    document.Add(new Paragraph("\n[ Integrity Verification ]").SetFont(boldFont));
                    document.Add(new Paragraph($"Primary Hash: {hash1}").SetFontSize(9).SetFontColor(iText.Kernel.Colors.ColorConstants.BLUE));
                    document.Add(new Paragraph($"Secondary Hash: {hash2}").SetFontSize(9).SetFontColor(iText.Kernel.Colors.ColorConstants.BLUE));
                    document.Add(new Paragraph("\n"));

                    // 결과 테이블 (7개 컬럼 : 상대 너비비율로 간격 정한 것 (PercentArray))
                    Table table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 1, 2, 4, 1, 2, 1 })).UseAllAvailableWidth();

                    string[] headers = { "파일명", "라인", "유형", "키워드", "오프셋", "시간", "결과" };
                    foreach (var header in headers)
                    {
                        table.AddHeaderCell(new Cell().Add(new Paragraph(header).SetFont(boldFont))
                            .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)
                            .SetTextAlignment(TextAlignment.CENTER));
                    }

                    foreach (var item in results)
                    {
                        table.AddCell(new Cell().Add(new Paragraph(item.FileName ?? "").SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(item.LineNumber.ToString() ?? "").SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(item.TypeName ?? "").SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(item.Keyword ?? "").SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(item.Offset.ToString() ?? "").SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(item.FoundTime.ToString("HH:mm:ss") ?? "").SetFontSize(8)));

                        var resultColor = item.IsIntegrityVerified ? iText.Kernel.Colors.ColorConstants.GREEN : iText.Kernel.Colors.ColorConstants.RED;
                        table.AddCell(new Cell().Add(new Paragraph(item.IsIntegrityVerified ? "PASS" : "FAIL")
                            .SetFontSize(8).SetFontColor(resultColor).SetFont(boldFont)));
                    }

                    document.Add(table);

                    // 하단 페이지 번호는 iText에서 자동으로 처리하거나 Close 시점에 확정된다고 한다
                    document.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"PDF 생성 중 상세 오류: {ex.Message}", ex);
            }
        }
    }
}