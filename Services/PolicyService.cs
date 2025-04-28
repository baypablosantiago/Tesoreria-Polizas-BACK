using Microsoft.AspNetCore.Http.HttpResults;

public class PolicyService
{
    private readonly IPolicyRepository _policyRepository;

    public PolicyService(IPolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }
    
    public async Task<List<Policy>> GetAll()
    {
        var result = await _policyRepository.GetAll();
        return result;
    }

}


