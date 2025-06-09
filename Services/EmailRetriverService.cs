using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;


public class EmailRetriverService
{
    private readonly string host_mail = "mail.entrerios.gov.ar";
    private readonly int imap_port = 993;
    private readonly string username = "pbay@entrerios.gov.ar";
    private readonly string password = "testteso123";

    public EmailRetriverService()
    {
      
    }
    
    public List<string> GetAndDownload()
    {
        List<string> emailsInfo = new List<string>();

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string saveDir = Path.Combine(desktopPath, "Emails");
        Directory.CreateDirectory(saveDir);

        using (var client = new ImapClient())
        {
            client.Connect(host_mail, imap_port, true);
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
