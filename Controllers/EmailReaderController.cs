using Microsoft.AspNetCore.Mvc;

[Route("api/emailreader")]
[ApiController]
public class EmailReaderController : ControllerBase
{
    private readonly IPolicyRepository _policyRepository;
    private readonly EmailRetriverService _retriverService;
    private readonly EmailScannerService _scannerService;

    public EmailReaderController(EmailRetriverService retriverServiceservice, EmailScannerService scannerService, IPolicyRepository policyRepository)
    {
        _retriverService = retriverServiceservice;
        _scannerService = scannerService;
        _policyRepository = policyRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = _scannerService.Get(); 

        foreach (PolicyModel a in result)
        {
            await _policyRepository.Insert(a); 
        }

        return Ok(result);
    }

}
