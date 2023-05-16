using MongoDbApp;
using MongoDbApp.Repositories;

using StackExchange.Redis;

using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(mongoDbSettings => builder.Configuration.GetSection("MongoDbSettings").Bind(mongoDbSettings));

builder.Services.AddScoped<IUserRepository, UserRepository>();

//Configure other services up here
var multiplexer = ConnectionMultiplexer.Connect("bpashkovskyi.redis.cache.windows.net:6380,password=FHfJ8zwNItRqK3ksuHHyEh4ufgZ6Z2lqAAzCaKXIujI=,ssl=True,abortConnect=False");
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

builder.Services.AddStackExchangeRedisCache(
    redisCacheOptions =>
    {
        redisCacheOptions.ConfigurationOptions = new ConfigurationOptions
        {
            AllowAdmin = true,
            DefaultDatabase = 0,
            Ssl = true,
            Password = "FHfJ8zwNItRqK3ksuHHyEh4ufgZ6Z2lqAAzCaKXIujI=",
            EndPoints = { { "bpashkovskyi.redis.cache.windows.net", 6380 } }
        };
    });


builder.Services.AddAutoMapper(typeof(UserMapperProfile).Assembly);


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
