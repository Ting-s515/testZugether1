using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using testZugether1.Hubs;

using testZugether1.Models;
var builder = WebApplication.CreateBuilder(args);
// �K�[�������ҪA��
builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(googleOptions =>
{
	googleOptions.ClientId = "862911357811-k8dpm7i1bampeqthskhq1m7csdj0qnb1.apps.googleusercontent.com";
	googleOptions.ClientSecret = "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";
	var scopes = new[] { "openid", "profile", "email", "https://www.googleapis.com/auth/gmail.send" };
	foreach (var scope in scopes)
	{
		googleOptions.Scope.Add(scope);
	}
	// ���wgoogle api callback path
	googleOptions.CallbackPath = new PathString("/signin-google");
});
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();
builder.Services.AddDistributedMemoryCache(); // �K�[���������s�w�s
											  // �K�[ Session �䴩
builder.Services.AddSession(options =>
{
	options.Cookie.HttpOnly = true; // �]�m�� HttpOnly
	options.Cookie.IsEssential = true; // �N Session �]�m�����n Cookie
});
// �K�[ AutoMapper �A�ȡA�ë��w�M�g�t�m�ɮ�

builder.Services.AddDbContext<ZugetherContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

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

app.UseAuthorization();
// �ҥ� Session
app.UseSession();

// �t�m SignalR ������
app.MapHub<ChatHub>("/chatHub");
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
