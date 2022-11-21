namespace Model.Entities.Auth.Models;

public class LoginModel {
    public LoginModel() {
    }

    public LoginModel(string email, string password, string deviceName) {
        Email = email;
        Password = password;
        DeviceName = deviceName;
    }

    [Required] [EmailAddress] public string Email { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;
    
    [Required] [MaxLength(30)] public string DeviceName { get; set; } = null!;
}