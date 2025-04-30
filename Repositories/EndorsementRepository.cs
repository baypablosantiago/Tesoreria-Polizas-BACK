using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
public interface IEndorsementRepository
{
    Task<Endorsement> Insert(Endorsement endorsement);
    Task<Endorsement?> GetByNumber(string number);
    Task<List<Endorsement>> GetAll();
}

public class EndorsementRepository : IEndorsementRepository
{
    private readonly TesoContext _context;

    public EndorsementRepository(TesoContext context)
    {
        _context = context;
    }

    public async Task<Endorsement?> GetByNumber(string number)
    {
        return await _context.Endorsements.FirstOrDefaultAsync(e => e.OriginalPolicy == number);
    }

    public async Task<List<Endorsement>> GetAll()
    {
        return await _context.Endorsements.ToListAsync();
    }

    public async Task<Endorsement> Insert(Endorsement endorsement)
    {
        var existing = await GetByNumber(endorsement.OriginalPolicy);
        if (existing != null)
        {
            return null;
        }

        var inserted = await _context.Endorsements.AddAsync(endorsement);
        await _context.SaveChangesAsync();
        return inserted.Entity;
    }
}
