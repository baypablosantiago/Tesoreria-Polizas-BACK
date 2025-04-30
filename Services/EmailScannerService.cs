using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using DotNetEnv;
using Sprache;

public class EmailScannerService
{
    private readonly string host;
    private readonly int port;
    private readonly string username;
    private readonly string password;
    ScannerPDF scannerPDF;
    private readonly IPolicyRepository _policyRepository;

    public EmailScannerService(IPolicyRepository policyRepository)
    {
        Env.Load();
        _policyRepository = policyRepository;
        host = Environment.GetEnvironmentVariable("HOST") ?? throw new InvalidOperationException("Error en el .env");
        port = Convert.ToInt16(Environment.GetEnvironmentVariable("PORT")); 
        password = Environment.GetEnvironmentVariable("PASSWORD") ?? throw new InvalidOperationException("Error en el .env");
        username = Environment.GetEnvironmentVariable("USERNAME") ?? throw new InvalidOperationException("Error en el .env");
        scannerPDF = new ScannerPDF();
    }

    public async Task<List<Policy>> GetAsync()
    {
        List<Policy> policies = new List<Policy>();
        List<Endorsement> endorsements = new List<Endorsement>(); 
        Policy policyModel = new Policy();
        Endorsement endorsementModel = new Endorsement();

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
                        if (part.FileName.Contains("P"))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                part.Content.DecodeTo(memoryStream);
                                policyModel = scannerPDF.ReadPdfToPolicy(memoryStream);
                                var exists = await _policyRepository.GetByNumber(policyModel.Number);
                                if (exists == null)
                                {
                                    policies.Add(policyModel);
                                }
                            }
                        }
                        else if (part.FileName.Contains("E"))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                part.Content.DecodeTo(memoryStream);
                                endorsementModel = scannerPDF.ReadPdfToEndorsement(memoryStream);
                                endorsements.Add(endorsementModel);
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
