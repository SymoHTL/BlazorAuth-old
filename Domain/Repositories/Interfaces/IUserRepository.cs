namespace Domain.Repositories.Interfaces;

public interface IUserRepository : IRepository<User> {
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> AuthorizeAsync(int id, CancellationToken ct = default);
    Task<User?> AuthorizeAsync(string tokenValue, CancellationToken ct = default);
    Task<User?> AuthorizeAsync(LoginModel model, CancellationToken ct = default);

    Task UpdateInfoAsync(User user, CancellationToken ct = default);
}