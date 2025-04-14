using System;
using System.Text;
using System.Text.Json;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

public class ScannerPDF
{
    public PolicyModel ReadPdf(Stream stream)
    {
        stream.Position = 0;
        string fullText = "";
        using (var reader = new PdfReader(stream))
        using (var pdfDoc = new PdfDocument(reader))
        {
            var strategy = new LocationTextExtractionStrategy();
            var sb = new StringBuilder();

            var text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(1), strategy);
            sb.Append(" ").Append(text.Replace("\n", " ").Replace("\r", " "));

            fullText = sb.ToString().Trim();
            Console.WriteLine(fullText);

        }
        return ParsePolicyFromText(fullText);
    }

    private string ExtractBetween(string text, string start, string end)
    {
        int startIndex = text.IndexOf(start);
        if (startIndex == -1) return string.Empty;
        startIndex += start.Length;

        int endIndex = text.IndexOf(end, startIndex);
        if (endIndex == -1) return string.Empty;

        return text.Substring(startIndex, endIndex - startIndex).Trim();
    }

    private PolicyModel ParsePolicyFromText(string fullText)
    {
        var model = new PolicyModel();
        model.Number = ExtractBetween(fullText, "Póliza:", "Instituto").Trim();
        model.ReceiptDate = DateTime.Today.ToShortDateString();
        model.Concept = ExtractBetween(fullText, "OBJETO DE LA LICITACION O EL CONTRATO", "El presente seguro regira").Trim();
        model.CompanyName = ExtractBetween(fullText, "que resulte adeudarle", "30-").Trim();
        model.CompanyCuil = ExtractBetween(fullText, model.CompanyName, "con domicilio").Trim();
        model.Insurer = ExtractBetween(fullText, "Productor:", "Organizador").Trim();
        return model;
    }
}