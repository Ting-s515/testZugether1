using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using testZugether1.Hubs;
using testZugether1.Models;

var builder = WebApplication.CreateBuilder(args);
// 添加身份驗證服務
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
	// 指定google api callback path
	googleOptions.CallbackPath = new PathString("/signin-google");
});


builder.Services.AddControllersWithViews();
//添加SignalR service
builder.Services.AddSignalR();
// 添加分布式內存緩存
builder.Services.AddDistributedMemoryCache();
// 添加 Session 支援										
builder.Services.AddSession(options =>
{
	options.Cookie.HttpOnly = true; // 設置為 HttpOnly
	options.Cookie.IsEssential = true; // 將 Session 設置為必要 Cookie
});

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
// 啟用 Session
app.UseSession();

// 配置 SignalR 的路由
app.MapHub<ChatHub>("/chatHub");
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
