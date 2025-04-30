using Microsoft.EntityFrameworkCore;

public class TesoContext : DbContext
{
    public DbSet<Policy> Policies { get; set; }
    public DbSet<Endorsement> Endorsements { get; set; }

    public TesoContext()
    {

    }

    public TesoContext(DbContextOptions<TesoContext> options) :
    base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<State>()
            .HasKey(ps => new { ps.PolicyNumber, ps.Name });

        base.OnModelCreating(modelBuilder);
    }
}
