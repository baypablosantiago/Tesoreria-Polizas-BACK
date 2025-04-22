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
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _policyRepository.GetAll();

        return Ok(result);
    }
}
