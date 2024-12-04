using az204Sample.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
         .AddMicrosoftIdentityWebApp(builder.Configuration, "AzureAd");


builder.Services.AddRazorPages().AddMvcOptions(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                  .RequireAuthenticatedUser()
                  .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

string appConfigConnection = "Endpoint=https://freeappconfigmy.azconfig.io;Id=eKeX;Secret=cae8MsXTBM9cZPXI7ZCKRa9uPE5Yj1Pu4oL0QUxlUyKTilg0qe9eJQQJ99AKACYeBjFgUQmNAAACAZAC1OYp";

builder.Configuration.AddAzureAppConfiguration(options => {
    options.Connect(appConfigConnection).Select("Az204Sample:*", LabelFilter.Null).ConfigureRefresh(refreshOptions =>
    {
        refreshOptions.Register("Az204Sample:AppSettings:Sentinel", refreshAll: true);
    });
});

builder.Services.Configure<Settings>(builder.Configuration.GetSection("Az204Sample:AppSettings"));
builder.Services.AddAzureAppConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAzureAppConfiguration();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAzureAppConfiguration();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
