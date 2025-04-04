using Microsoft.AspNetCore.Mvc;

[Route("api/emailreader")]
[ApiController]
public class EmailReaderController : ControllerBase
{
    private readonly EmailReaderService _service;

    public EmailReaderController(EmailReaderService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var result = _service.Test();
        return Ok(result);
    }
}
