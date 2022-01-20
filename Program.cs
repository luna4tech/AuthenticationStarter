using AuthLearning.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

services.AddSingleton<UserRepository>();
services.AddControllersWithViews();
services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
	.AddCookie(options => options.LoginPath = "/Accounts/Login")
	.AddCookie("External")
	.AddFacebook(facebookOptions =>
	{
		facebookOptions.SignInScheme = "External";
		facebookOptions.AppId = configuration["Authentication:Facebook:AppId"];
		facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
