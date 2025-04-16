

public interface IPolicyRepository
{
    Task<PolicyModel> Insert(PolicyModel policy);
    Task<PolicyModel?> GetByNumber(int number);
}

public class PolicyRepository : IPolicyRepository
{
    private readonly TesoContext _context;
    
    public PolicyRepository(TesoContext context)
    {
        _context = context;
    }

    public Task<PolicyModel?> GetByNumber(int number)
    {
        throw new NotImplementedException();
    }

    public Task<PolicyModel> Insert(PolicyModel policy)
    {
        throw new NotImplementedException();
    }
}