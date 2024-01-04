using Application.Interfaces;
using Domain.Entities;
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

builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();

app.AddPostsEndpoints();
app.AddUsersEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();