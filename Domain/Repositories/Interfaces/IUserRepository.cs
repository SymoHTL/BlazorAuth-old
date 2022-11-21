using Model.Entities.Auth.Models;

namespace Domain.Repositories.Interfaces;

public interface IUserRepository : IRepository<User> {
    Task<User?> ReadAuthGraphAsync(string email, CancellationToken ct = default);

    [Obsolete("Use AuthorizeAsync(string) instead")]
    Task<User?> AuthorizeAsync(int id, CancellationToken ct = default);

    Task<User?> AuthorizeAsync(string tokenIdentifier, CancellationToken ct = default);
    Task<User?> AuthorizeAsync(LoginModel model, CancellationToken ct = default);
    Task UpdateInfoAsync(User user, CancellationToken ct = default);
}