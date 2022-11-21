using Domain.Services.Implementations;

namespace Domain.Repositories.Implementations;

public class TokenRepository : ARepository<Token>, ITokenRepository {
    private readonly DeviceService _deviceService;

    public TokenRepository(ModelDbContext context, DeviceService deviceService) : base(context) {
        _deviceService = deviceService;
    }

    public async Task<Token?> FindByIdentifier(string token, CancellationToken ct = default) =>
        await Table.FirstOrDefaultAsync(t => t.Identifier == token, ct);

    public async Task<Token?> GenerateTokenAsync(User user, string deviceName, CancellationToken ct = default) {
        // check if device name exists
        var existingToken = await Table.FirstOrDefaultAsync(t => t.UserId == user.Id && t.DeviceName == deviceName, ct);
        if (existingToken is null) {
            var token = new Token {
                UserId = user.Id,
                DeviceName = deviceName,
            };

            return await CreateAsync(token, ct);
        }

        existingToken.Update();
        await UpdateAsync(existingToken, ct);

        return existingToken;
    }
}