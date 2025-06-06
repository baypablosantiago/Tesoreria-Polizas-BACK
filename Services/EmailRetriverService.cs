using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using DotNetEnv;

public class EmailRetriverService
{
    private readonly string host;
    private readonly int imapport;
    private readonly string username;
    private readonly string password;

    public EmailRetriverService()
    {
        Env.Load();
        host = Environment.GetEnvironmentVariable("HOST") ?? throw new InvalidOperationException("Error en el .env");
        imapport = Convert.ToInt16(Environment.GetEnvironmentVariable("IMAP_PORT")); 
        password = Environment.GetEnvironmentVariable("PASSWORD") ?? throw new InvalidOperationException("Error en el .env");
        username = Environment.GetEnvironmentVariable("USERNAME") ?? throw new InvalidOperationException("Error en el .env");
    }
    
    public List<string> GetAndDownload()
    {
        List<string> emailsInfo = new List<string>();

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string saveDir = Path.Combine(desktopPath, "Emails");
        Directory.CreateDirectory(saveDir);

        using (var client = new ImapClient())
        {
            client.Connect(host, imapport, true);
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
                        string filePath = Path.Combine(saveDir, part.FileName);                        
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
