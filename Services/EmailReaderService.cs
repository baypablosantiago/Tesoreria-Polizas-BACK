using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using DotNetEnv;

public class EmailReaderService
{
    private readonly string host = "mail.entrerios.gov.ar";
    private readonly int port = 993;
    private readonly string username = "pbay@entrerios.gov.ar";
    private readonly string password;

    public EmailReaderService()
    {
        Env.Load();
        password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

        if (string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException("Error en el archivo .env");
        }
    }
    public List<string> Test()
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
                        string filePath = Path.Combine("c:\\Users\\tepablob\\Downloads", part.FileName);
                        using (var stream = File.Create(filePath))
                        {
                            part.Content.DecodeTo(stream);
                        }
                        emailsInfo.Add($"PDF descargado: {filePath}");
                    }
                }
            }

            client.Disconnect(true);
        }

        return emailsInfo;
    }
}
