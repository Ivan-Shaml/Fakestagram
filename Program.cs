using Fakestagram.Data;
using Fakestagram.Data.AutoMapperProfile;
using Fakestagram.Data.Repositories;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Services;
using Fakestagram.Services.Contracts;
using Fakestagram.Services.Helpers;
using Fakestagram.Services.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Swagger UI with auth scheme support
builder.Services.AddSwaggerGen(opt => {
    opt.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard authorization header using the Bearer scheme ('Bearer {token}')",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    opt.OperationFilter<SecurityRequirementsOperationFilter>();
});


//Bearer Config
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddDbContext<FakestagramDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(conf => conf.AddProfile(new AutoMapperProfile()));

// Repositories

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<IPostsRepository, PostsRepository>();

// Services

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddHttpContextAccessor();

// Provider-services

builder.Services.AddScoped<IAuthProvider, JwtAuthProvider>();
builder.Services.AddScoped<IPasswordProvider, SHA512PasswordProvider>();

// Helper-services

builder.Services.AddScoped<IJsonErrorSerializerHelper, JsonErrorSerializerHelper>();

// Add webroot directory used for uploading the photos
builder.WebHost.UseWebRoot("wwwroot");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();
