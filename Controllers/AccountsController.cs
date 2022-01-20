using AuthLearning.Models;
using AuthLearning.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthLearning.Controllers
{
	public class AccountsController : Controller
	{
		private UserRepository _userRepository;
		public AccountsController(UserRepository userRepository) { 
			this._userRepository = userRepository;
		}
		public IActionResult Login(string returnUrl = "/")
		{
			return View(new LoginModel { 
				ReturnUrl = returnUrl.Equals(null) || returnUrl.Equals("") ? "/" : returnUrl
			});
		}

		public IActionResult LoginWithFacebook(string returnUrl = "/")
		{
			var props = new AuthenticationProperties
			{
				RedirectUri = Url.Action("FacebookLoginCallback"),
				Items = { { "returnUrl", returnUrl } }
			};
			return Challenge(props, FacebookDefaults.AuthenticationScheme);
		}

		[HttpPost]
		public async Task<ActionResult> Login(LoginModel loginModel) 
		{
			var user = this._userRepository.GetUserByUsernameAndPassword(loginModel.Username, loginModel.Password);
			if (user == null) return Unauthorized();
			var claims = new List<Claim> {
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, loginModel.Username),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Role, user.Role),
				new Claim("House", user.House)
			};
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = loginModel.RememberLogin });
			return LocalRedirect(loginModel.ReturnUrl ?? "/");
		}

		public async Task<IActionResult> FacebookLoginCallback()
        {
			var result = await HttpContext.AuthenticateAsync("External");
			var externalClaims = result.Principal.Claims.ToList();
			var subjectId = externalClaims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var claims = new List<Claim> {
				new Claim(ClaimTypes.NameIdentifier, subjectId),
				new Claim(ClaimTypes.Name, externalClaims.First(c => c.Type == ClaimTypes.Name).Value),
				new Claim(ClaimTypes.Email, externalClaims.First(c => c.Type == ClaimTypes.Email).Value),
				new Claim(ClaimTypes.Role, "Student"),
				new Claim("House", "Slytherin")
			};
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);

			await HttpContext.SignOutAsync("External");
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
			return LocalRedirect(result?.Properties?.Items["returnUrl"] ?? "/");
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return LocalRedirect("/");
		}
	}
}
