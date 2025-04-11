using Microsoft.AspNetCore.Mvc;

[Route("api/emailreader")]
[ApiController]
public class EmailReaderController : ControllerBase
{
    private readonly EmailRetriverService _retriverService;
    private readonly EmailScannerService _scannerService;

    public EmailReaderController(EmailRetriverService retriverServiceservice, EmailScannerService scannerService)
    {
        _retriverService = retriverServiceservice;
        _scannerService = scannerService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var result = _scannerService.GetAndJson();
        return Ok(result);
    }
}
