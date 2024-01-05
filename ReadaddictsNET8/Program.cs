using Application.Abstractions;
using Application.Interfaces;
using CloudinaryDotNet;
using Domain.Entities;
using dotenv.net;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using ReadaddictsNET8.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();
builder.Services.AddAntiforgery();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICloudinaryImage, CloudinaryRepository>();

builder.Services.AddDbContext<ApplicationDbContext>();

// Cloudinary
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
Cloudinary cloudinary = new(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
cloudinary.Api.Secure = true;

builder.Services.AddSingleton(cloudinary);

var app = builder.Build();

// Add endpoints
app.AddPostsEndpoints();
app.AddUsersEndpoints();
app.AddAntiForgeryEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.Run();