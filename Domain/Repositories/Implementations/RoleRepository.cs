namespace Domain.Repositories.Implementations;

public class RoleRepository : ARepository<Role>, IRoleRepository {
    public RoleRepository(ModelDbContext context) : base(context) {
    }
}