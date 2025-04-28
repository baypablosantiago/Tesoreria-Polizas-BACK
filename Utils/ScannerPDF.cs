using System;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

public class ScannerPDF
{
    public Policy ReadPdf(Stream stream)
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

    private Policy ParsePolicyFromText(string fullText)
    {
        var model = new Policy();
        model.Number = ExtractBetween(fullText, "Póliza:", "Instituto").Trim();
        model.Reception = DateTime.Today.ToShortDateString().Trim();
        string fullConcept = ExtractBetween(fullText, "OBJETO DE LA LICITACION O EL CONTRATO", "El presente seguro regira").Trim();;
        string concept = Regex.Replace(fullConcept, @"-{10,}", " ").Trim();
        model.Concept = concept;
        model.CompanyName = ExtractBetween(fullText, "que resulte adeudarle", "30-").Trim();
        model.CompanyCuil = ExtractBetween(fullText, model.CompanyName, "con domicilio").Trim();
        model.Insurer = ExtractBetween(fullText, "Productor:", "Organizador").Trim() == "INSTITUTO AUTARQUICO PROVINCIAL DEL SEGURO"
        ? "IAPSER"
        : ExtractBetween(fullText, "Productor:", "Organizador").Trim();
        model.States = new List<State>
        {
            new State { PolicyNumber = model.Number, Name = "Recibida en correo", Checked = true },
            new State { PolicyNumber = model.Number, Name = "Cargada en SIAF", Checked = false },
            new State { PolicyNumber = model.Number, Name = "Retencion de fondo de reparo", Checked = false },
            new State { PolicyNumber = model.Number, Name = "Retencion pagada", Checked = false }
        };
        return model;
    }
}