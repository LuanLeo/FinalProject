using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Protocol.Core.Types;
using TablesideOrdering.Data;
using System.Configuration;
using TablesideOrdering.Models;
using TablesideOrdering;
using TablesideOrdering.Models.Momo;
using TablesideOrdering.PaymentServices.VNPay;
using TablesideOrdering.PaymentServices.Momo;
using TablesideOrdering.SignalR.SubscribeTableDependencies;
using TablesideOrdering.SignalR.MiddlewareExtensions;
using TablesideOrdering.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

//Call SignalR Hub
builder.Services.AddSingleton<OrderHub>();
builder.Services.AddSingleton<SubscribeOrderTableDependency>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TablesideOrdering")));
builder.Services.AddDefaultIdentity<IdentityUser>().AddDefaultTokenProviders().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

//Call From AppSetting
builder.Services.Configure<SMSMessage>(builder.Configuration.GetSection("SMSTwilio"));
builder.Services.Configure<SignInPass>(builder.Configuration.GetSection("SignInPass"));
builder.Services.Configure<Email>(builder.Configuration.GetSection("Email"));

//Call Additional Services or Packages
builder.Services.AddNotyf(config => { config.IsDismissable = true; config.DurationInSeconds = 5; config.Position = NotyfPosition.TopRight; });
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
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
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapHub<OrderHub>("/orderHub");
app.MapHub<ChatHub>("/chatHub");

app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});
app.MapControllerRoute(

     /*name: "default",
    pattern: "{controller=Home}/{action=TableCheck}/{id?}"*/
    name: "default",
    pattern: "{controller=Home}/{action=TableCheck}/{id?}"
    /*  name: "default",
      pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}"*/
    );
/*
 * call SubscribeTableDependency() here
 * create a middleware and call SubscribeTableDependency() method in the middleware
 */

app.UseOrderTableDependency();
app.Run();
