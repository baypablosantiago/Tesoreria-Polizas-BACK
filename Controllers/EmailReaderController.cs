using Microsoft.AspNetCore.Mvc;

[Route("api/emailreader")]
[ApiController]
public class EmailReaderController : ControllerBase
{
    private readonly IPolicyRepository _policyRepository;
    private readonly EmailScannerService _scannerService;
    private readonly EmailRetriverService _retriverService;

    public EmailReaderController(EmailScannerService scannerService, IPolicyRepository policyRepository, EmailRetriverService retriverService)
    {
        _scannerService = scannerService;
        _policyRepository = policyRepository;
        _retriverService = retriverService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _retriverService.GetAndDownload();
        var result = await _scannerService.GetAsync();

        foreach (Policy a in result)
        {
            await _policyRepository.Insert(a);
        }

        return Ok(result);
    }

}
