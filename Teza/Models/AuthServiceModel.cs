namespace Teza.Models
{
    public class UserInfo
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
    public class AuthServiceModel
    {
        public bool success { get; set; }
        public UserInfo data { get; set; }
        public string message { get; set; }
    }
}