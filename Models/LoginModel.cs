namespace AuthLearning.Models
{
	public class LoginModel
	{
		public string Username { get; set; } = default!;
		public string Password { get; set; } = default!;
		public bool RememberLogin { get; set; }
		public string ReturnUrl { get; set; } = default!;
	}
}
