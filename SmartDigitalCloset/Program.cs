using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SmartDigitalCloset.Data;
using SmartDigitalCloset.Services;
using Blazored.SessionStorage;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<UserDbHelper>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<SmartDigitalCloset.Data.UserStore>();
builder.Services.AddScoped<SmartDigitalCloset.Data.ClosetService>();
builder.Services.AddScoped<SmartDigitalCloset.Data.OutfitService>();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();

// Get connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton(new SmartDigitalCloset.Data.FavoritesService(connectionString));
builder.Services.AddScoped<SmartDigitalCloset.Data.InterestService>();

// Add error handling middleware
builder.Services.AddExceptionHandler(options =>
{
    options.ExceptionHandlingPath = "/Error";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
