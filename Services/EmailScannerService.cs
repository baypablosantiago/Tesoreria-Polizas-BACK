using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using DotNetEnv;

public class EmailScannerService
{
    private readonly string host;
    private readonly int port = 993;
    private readonly string username;
    private readonly string password;
    ScannerPDF scannerPDF;
    private readonly IPolicyRepository _policyRepository;

    public EmailScannerService(IPolicyRepository policyRepository)
    {
        Env.Load();
        _policyRepository = policyRepository;
        host = Environment.GetEnvironmentVariable("HOST") ?? throw new InvalidOperationException("Error en el .env");
        password = Environment.GetEnvironmentVariable("PASSWORD") ?? throw new InvalidOperationException("Error en el .env");
        username = Environment.GetEnvironmentVariable("USERNAME") ?? throw new InvalidOperationException("Error en el .env");
        scannerPDF = new ScannerPDF();
    }

    public async Task<List<PolicyModel>> GetAsync()
    {
        List<PolicyModel> models = new List<PolicyModel>();
        PolicyModel model = new PolicyModel();

        using (var client = new ImapClient())
        {
            client.Connect(host, port, true);
            client.Authenticate(username, password);

            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadWrite);

            var uids = inbox.Search(SearchQuery.NotSeen);

            foreach (var uid in uids)
            {
                var message = inbox.GetMessage(uid);

                foreach (var attachment in message.Attachments)
                {
                    if (attachment is MimePart part && part.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            part.Content.DecodeTo(memoryStream);
                            model = scannerPDF.ReadPdf(memoryStream);
                            var exists = await _policyRepository.GetByNumber(model.Number);
                            if (exists == null)
                            {
                                models.Add(model);
                            }
                        }
                    }
                }
                inbox.AddFlags(uid, MessageFlags.Seen, true);
            }
            client.Disconnect(true);
        }
        return models;
    }
}
