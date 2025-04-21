using Microsoft.AspNetCore.Mvc;

[Route("api/policy")]
[ApiController]
public class PolicyController : ControllerBase
{
    private readonly IPolicyRepository _policyRepository;

    public PolicyController(IPolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = _policyRepository.GetAll();

        return Ok(result);
    }
}
