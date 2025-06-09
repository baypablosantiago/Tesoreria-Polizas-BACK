using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;


public class EmailScannerService
{
    private readonly string host_mail = "mail.entrerios.gov.ar";
    private readonly int imap_port = 993;
    private readonly string username = "pbay@entrerios.gov.ar";
    private readonly string password = "testteso123";
    ScannerPDF scannerPDF;
    private readonly IPolicyRepository _policyRepository;

    public EmailScannerService(IPolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
        scannerPDF = new ScannerPDF();
    }

    public async Task<List<Policy>> GetAsync()
    {
        List<Policy> policies = new List<Policy>();
        Policy policyModel = new Policy();

        using (var client = new ImapClient())
        {
            client.Connect(host_mail, imap_port, true);
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
                        // else if (part.FileName.Contains("E"))
                        // {
                           
                        // }
                    }
                }
                inbox.AddFlags(uid, MessageFlags.Seen, true);
            }
            client.Disconnect(true);
        }
        return policies;
    }
}
