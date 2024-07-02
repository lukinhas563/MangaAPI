using CloudinaryDotNet;
using FluentValidation;
using MangaAPI.Mappers;
using MangaAPI.Persistence;
using MangaAPI.Services;
using MangaAPI.Utils;
using MangaAPI.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Token
builder.Services.AddTransient<TokenService>();

// Add services to the container.
string connection = builder.Configuration.GetConnectionString("MangaDb");
builder.Services.AddDbContext<MangasContext>(options => options.UseNpgsql(connection));
builder.Services.AddAutoMapper(typeof(MangaProfiles));
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

builder.Services.AddControllers();

// Provider
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    return new Cloudinary(new Account(
        config.CloudName,
        config.ApiKey,
        config.ApiSecret
    ));
});

// Token
var tokenSettings = builder.Configuration.GetSection("JwtSettings").Get<TokenSettings>();
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddTransient(provider =>
{
    var settings = provider.GetRequiredService<IOptions<TokenSettings>>().Value;
    return settings;
});

// Bearer authentication
byte[] key = Encoding.ASCII.GetBytes(tokenSettings.PrivateKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOptions =>
{
    jwtOptions.RequireHttpsMetadata = false;
    jwtOptions.SaveToken = true;
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

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

app.Run();
