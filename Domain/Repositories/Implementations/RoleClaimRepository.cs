namespace Domain.Repositories.Implementations;

public class RoleClaimRepository : ARepository<RoleClaim>, IRoleClaimRepository {
    public RoleClaimRepository(ModelDbContext context) : base(context) {
    }
}