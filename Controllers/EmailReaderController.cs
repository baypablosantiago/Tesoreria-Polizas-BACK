using Microsoft.AspNetCore.Mvc;

[Route("api/emailreader")]
[ApiController]
public class EmailReaderController : ControllerBase
{
    private readonly IPolicyRepository _policyRepository;
    private readonly EmailScannerService _scannerService;

    public EmailReaderController(EmailScannerService scannerService, IPolicyRepository policyRepository)
    {
        _scannerService = scannerService;
        _policyRepository = policyRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _scannerService.GetAsync(); 

        foreach (PolicyModel a in result)
        {
            await _policyRepository.Insert(a); 
        }

        return Ok(result);
    }

}
