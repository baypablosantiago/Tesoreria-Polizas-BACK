using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public interface IPolicyRepository
{
    Task<Policy> Insert(Policy policy);
    Task<Policy?> GetByNumber(string number);
    Task<List<Policy>> GetAll();
}

public class PolicyRepository : IPolicyRepository
{
    private readonly TesoContext _context;

    public PolicyRepository(TesoContext context)
    {
        _context = context;
    }

    public async Task<Policy?> GetByNumber(string number)
    {
        return await _context.Policies.FirstOrDefaultAsync(p => p.Number == number);
    }

    public async Task<List<Policy>> GetAll()
    {
        return await _context.Policies.Include(p => p.States).ToListAsync();
    }

    public async Task<Policy> Insert(Policy policy)
    {
        var existing = await GetByNumber(policy.Number);
        if (existing != null)
        {
            return null;
        }
        EntityEntry<Policy> insertedPolicy = await _context.Policies.AddAsync(policy);
        await _context.SaveChangesAsync();
        return insertedPolicy.Entity;
    }
}