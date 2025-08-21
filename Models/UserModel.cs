namespace EnvanterYonetimPaneli.Models
{
    public class UserModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool IsLoggedIn { get; set; }

        public static UserModel User = new UserModel{IsLoggedIn = false};

    }
}