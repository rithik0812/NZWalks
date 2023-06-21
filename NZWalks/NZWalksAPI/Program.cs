using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NZWalksAPI.Data;
using NZWalksAPI.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext to the services container, with a conntection string to a SQL DB using EF
builder.Services.AddDbContext<NZWalksDBContext>(options => 
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalks"));
});

// Add IRegionRepository interface and the SQL implementation of it into the service container
builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();

// Add IWalksRepository interface and the SQL implementation of it into the service container
builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();

// Add IWalksDiffucltyRepository interface and the SQL implementation of it into the service container
builder.Services.AddScoped<IWalksDifficultyRepository, SQLWalksDifficultyRepository>();

// Add IUserRepository interface and a "static implementation" for it called StaticUserRepository
builder.Services.AddSingleton<IUserRepository, StaticUserRepository>();

// Add IUserRepository interface and a "static implementation" for it called StaticUserRepository
builder.Services.AddScoped<ITokenHandler, NZWalksAPI.Repository.TokenHandler>();



// Automapper will scan the whole assembly for all the profiles, then use the profiles to map the data 
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
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
