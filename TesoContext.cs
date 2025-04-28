using Microsoft.EntityFrameworkCore;

public class TesoContext : DbContext
{
    public DbSet<Policy> Policies { get; set; }
    
    public TesoContext()
    {

    }

    public TesoContext(DbContextOptions<TesoContext> options):
    base(options)
    {

    }
}
