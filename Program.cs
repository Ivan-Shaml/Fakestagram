using Fakestagram.Data;
using Fakestagram.Data.AutoMapperProfile;
using Fakestagram.Data.Repositories;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Services;
using Fakestagram.Services.Contracts;
using Fakestagram.Services.Providers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FakestagramDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(conf => conf.AddProfile(new AutoMapperProfile()));

// Repositories

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<IPostsRepository, PostsRepository>();

// Services

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordProvider, SHA512PasswordProvider>();
builder.Services.AddHttpContextAccessor();

// Provider-services

builder.Services.AddScoped<IAuthProvider, JwtAuthProvider>();
builder.Services.AddScoped<IAuthProvider, JwtAuthProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
