using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using DotNetEnv;

public class EmailScannerService
{
    private readonly string host = "mail.entrerios.gov.ar";
    private readonly int port = 993;
    private readonly string username;
    private readonly string password;

    ScannerPDF scannerPDF;

    public EmailScannerService()
    {
        Env.Load();
        password = Environment.GetEnvironmentVariable("PASSWORD") ?? throw new InvalidOperationException("Error en el .env");
        username = Environment.GetEnvironmentVariable("USERNAME") ?? throw new InvalidOperationException("Error en el .env");
        scannerPDF = new ScannerPDF();
    }

    public List<string> GetAndJson()
    {
        List<string> emailsInfo = new List<string>();

        using (var client = new ImapClient())
        {
            client.Connect(host, port, true);
            client.Authenticate(username, password);

            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadOnly);

            var uids = inbox.Search(SearchQuery.NotSeen);
            emailsInfo.Add($"Correos no leídos: {uids.Count}");

            foreach (var uid in uids)
            {
                var message = inbox.GetMessage(uid);
                emailsInfo.Add($"Asunto: {message.Subject}");

                foreach (var attachment in message.Attachments)
                {
                    if (attachment is MimePart part && part.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            part.Content.DecodeTo(memoryStream);
                            string json = scannerPDF.ReadPdf(memoryStream);
                            emailsInfo.Add($"Contenido del PDF: {json}");
                        }
                    }
                }
            }
            client.Disconnect(true);
        }
        return emailsInfo;
    }
}
