namespace Model.Configuration;

public class ModelDbContext : DbContext {
    public ModelDbContext(DbContextOptions<ModelDbContext> options) : base(options) {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<RoleClaim> RoleClaims { get; set; } = null!;
    public DbSet<Token> Tokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder) {
// UNIQUE

        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<Role>()
            .HasIndex(r => r.Identifier)
            .IsUnique();

        builder.Entity<Token>()
            .HasIndex(t => t.Value)
            .IsUnique();

// HAS KEY

        builder.Entity<RoleClaim>()
            .HasKey(rc => new { rc.UserId, rc.RoleId });
        // RELATIONSHIPS
        // 1:1
        // 1:N

        builder.Entity<Token>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(t => t.UserId);

        // N:M

        builder.Entity<RoleClaim>()
            .HasOne(rc => rc.Role)
            .WithMany(r => r.RoleClaims)
            .HasForeignKey(rc => rc.RoleId);

        builder.Entity<RoleClaim>()
            .HasOne(rc => rc.User)
            .WithMany(u => u.RoleClaims)
            .HasForeignKey(rc => rc.UserId);
        // OTHER
        // SEEDING
        builder.Entity<Role>()
            .HasData(new Role { Id = 1, Identifier = "Admin", Description = "Administrator" });
    }
}