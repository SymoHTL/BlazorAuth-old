namespace Model.Entities.Auth;

[Table("USERS_HAVE_ROLES_JT")]
public class RoleClaim {
    [Column("USER_ID")] public int UserId { get; set; }

    public User User { get; set; } = null!;

    [Column("ROLE_ID")] public int RoleId { get; set; }

    public Role Role { get; set; } = null!;
}