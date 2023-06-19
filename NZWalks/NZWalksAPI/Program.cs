using Microsoft.EntityFrameworkCore;
using NZWalksAPI.Data;
using NZWalksAPI.Repository;

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

// Automapper will scan the whole assembly for all the profiles, then use the profiles to map the data 
builder.Services.AddAutoMapper(typeof(Program).Assembly);



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
