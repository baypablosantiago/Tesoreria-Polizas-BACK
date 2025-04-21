using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public interface IPolicyRepository
{
    Task<PolicyModel> Insert(PolicyModel policy);
    Task<PolicyModel?> GetByNumber(string number);
}

public class PolicyRepository : IPolicyRepository
{
    private readonly TesoContext _context;

    public PolicyRepository(TesoContext context)
    {
        _context = context;
    }

    public async Task<PolicyModel?> GetByNumber(string number)
    {
        return await _context.Policies.FirstOrDefaultAsync(p => p.Number == number);
    }


    public async Task<PolicyModel> Insert(PolicyModel policy)
    {
        var existing = await GetByNumber(policy.Number);
        if (existing != null)
        {
            return null;
        }
        EntityEntry<PolicyModel> insertedPolicy = await _context.Policies.AddAsync(policy);
        await _context.SaveChangesAsync();
        return insertedPolicy.Entity;
    }
}