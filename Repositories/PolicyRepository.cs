using Microsoft.EntityFrameworkCore.ChangeTracking;

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

    public async Task<PolicyModel> Insert(PolicyModel policy)
    {
        EntityEntry<PolicyModel> insertedPolicy = await _context.Policies.AddAsync(policy);
        await _context.SaveChangesAsync();
        return insertedPolicy.Entity;
    }
}