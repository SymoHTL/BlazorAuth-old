using Domain.Exceptions;
using Domain.Exceptions.Token;
using Domain.Services.Implementations;
using Model.Entities.Auth.Models;

namespace Domain.Repositories.Implementations;

public class UserRepository : ARepository<User>, IUserRepository {
    private readonly DeviceService _deviceService;
    private readonly ITokenRepository _tokenRepo;

    public UserRepository(ModelDbContext context, ITokenRepository tokenRepo, DeviceService deviceService) :
        base(context) {
        _tokenRepo = tokenRepo;
        _deviceService = deviceService;
    }


    public async Task<User?> ReadAuthGraphAsync(string email, CancellationToken ct = default) {
        var user = await Table
            .Include(u => u.RoleClaims)
            .ThenInclude(rc => rc.Role)
            .Include(u => u.Tokens)
            .AsSplitQuery() // <--- this is the magic
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        return user?.ClearSensitiveData();
    }

    public async Task<User?> AuthorizeAsync(int id, CancellationToken ct = default) {
        var user = await ReadAuthGraphAsync(id, ct);

        return user;
    }

    public async Task<User?> AuthorizeAsync(string tokenIdentifier, CancellationToken ct = default) {
        // read token from db
        var tokenEntity = await _tokenRepo.FindByIdentifier(tokenIdentifier, ct);
        if (tokenEntity is null) throw new TokenNotFoundException();
        if (tokenEntity.IsRevoked) throw new TokenRevokedException();
        if (tokenEntity.IsExpired) throw new TokenExpiredException();

        tokenEntity.LastLogin = DateTime.UtcNow;
        await _tokenRepo.UpdateAsync(tokenEntity, ct);

        //if (!tokenEntity.AllowIpChange) {
        //    if (_deviceService.CurrentIpAddress is null) throw new IpAddressNotFoundException();

        //    var currentIp = _deviceService.CurrentIpAddress.ToString();
        //    if (currentIp != tokenEntity.IpAddress) throw new TokenIpChangedException();
        //}

        var user = await ReadAuthGraphAsync(tokenEntity.UserId, ct);

        return user;
    }

    public async Task<User?> AuthorizeAsync(LoginModel model, CancellationToken ct = default) {
        var user = await ReadAuthGraphAsync(model.Email, ct);

        if (user is null) return null;

        if (!User.VerifyPassword(model.Password, user.PasswordHash)) return null!;

        return user;
    }

    public async Task UpdateInfoAsync(User user, CancellationToken ct = default) {
        // check if email is already taken
        var emailExists = await Table.AnyAsync(u => u.Email == user.Email && u.Id != user.Id, ct);
        if (emailExists) throw new DuplicateEmailException();

        // update user
        await UpdateAsync(user, ct);
    }

    protected async Task<User?> ReadAuthGraphAsync(int id, CancellationToken ct = default) {
        var user = await Table
            .Include(u => u.RoleClaims)
            .ThenInclude(rc => rc.Role)
            .Include(u => u.Tokens)
            .AsSplitQuery() // <--- this is the magic
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        return user?.ClearSensitiveData();
    }
}