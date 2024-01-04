using Infrastructure;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
