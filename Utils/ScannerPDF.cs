using System;
using System.Text;
using System.Text.Json;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

public class ScannerPDF
{
    public string ReadPdf(Stream stream)
    {
        stream.Position = 0;
        using (var pdfReader = new PdfReader(stream))
        using (var pdfDoc = new PdfDocument(pdfReader))
        {
            return ExtractText(pdfDoc);
        }
    }

    public string ExtractText(PdfDocument pdfDoc)
    {
        StringBuilder content = new StringBuilder();
        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
        {
            var page = pdfDoc.GetPage(i);
            var strategy = new SimpleTextExtractionStrategy();
            content.Append(PdfTextExtractor.GetTextFromPage(page, strategy));
        }
        return content.ToString();
    }

    public string MakeJson(string content)
    {
        var model = new PolicyModel
        {
            Number = ExtractField(content, "Número de póliza:", "Fecha de recepción:"),
            ReceiptDate = ExtractField(content, "Fecha de recepción:", "Concepto:"),
            Concept = ExtractField(content, "Concepto:", "Empresa:"),
            CompanyName = ExtractField(content, "Empresa:", "CUIL:"),
            CompanyCuil = ExtractField(content, "CUIL:", "Aseguradora:"),
            Insurer = ExtractField(content, "Aseguradora:", ""),
            States = new List<StateModel>
            {
                new StateModel { Name = "Validado", Checked = false },
                new StateModel { Name = "Aprobado", Checked = false },
                new StateModel { Name = "Rechazado", Checked = false }
            }
        };
        return JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
    }

    private string ExtractField(string content, string startTag, string endTag)
    {
        try
        {
            int start = content.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
            if (start == -1)
            {
                Console.WriteLine($"[WARN] No se encontró el inicio: '{startTag}'");
                return string.Empty;
            }

            start += startTag.Length;

            int end = string.IsNullOrEmpty(endTag)
                ? content.Length
                : content.IndexOf(endTag, start, StringComparison.OrdinalIgnoreCase);

            if (end == -1)
            {
                Console.WriteLine($"[WARN] No se encontró el final: '{endTag}'. Se usa fin del texto.");
                end = content.Length;
            }

            var result = content.Substring(start, end - start).Trim();
            Console.WriteLine($"[INFO] Extraído entre '{startTag}' y '{endTag}': {result}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] ExtractField: {ex.Message}");
            return string.Empty;
        }
    }
}