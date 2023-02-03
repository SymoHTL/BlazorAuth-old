namespace Model.Entities.Auth; 

[Table("ROLE_REQUEST")]
public class RoleRequest {
    [Column("USER_ID")]
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    
    [Column("ROLE_ID")]
    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;
    
    [Column("REQUEST_DATE")]
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
}