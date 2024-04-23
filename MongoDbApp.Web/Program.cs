using Microsoft.Extensions.Options;

using MongoDbApp;
using MongoDbApp.Repositories;

using StackExchange.Redis;

using Microsoft.Extensions.Caching.Distributed;

using MongoDB.Driver;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(mongoDbSettings => builder.Configuration.GetSection("MongoDbSettings").Bind(mongoDbSettings));

builder.Services.AddScoped<IUserRepository, UserRepository>(serviceProvider =>
{
    MongoDbSettings mongoDbSettings = serviceProvider.GetService<IOptions<MongoDbSettings>>().Value;
    MongoClient client = new MongoClient(mongoDbSettings.ConnectionString);
    IMongoDatabase userDatabase = client.GetDatabase(mongoDbSettings.UserDatabaseName);

    IDistributedCache distributedCache = serviceProvider.GetService<IDistributedCache>();

    return new UserRepository(userDatabase, distributedCache);

});

builder.Services.AddStackExchangeRedisCache(
    redisCacheOptions =>
    {
        var redisSettings = builder.Configuration.GetSection("RedisSettings").Get<RedisSettings>();
        redisCacheOptions.ConfigurationOptions = new ConfigurationOptions
        {
            AllowAdmin = true,
            DefaultDatabase = 0,
            Ssl = true,
            Password = redisSettings.Password,
            EndPoints = { { redisSettings.Host, redisSettings.Port } }
        };
    });


builder.Services.AddAutoMapper(typeof(UserMapperProfile).Assembly);


WebApplication app = builder.Build();

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
