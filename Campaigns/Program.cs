using Campaigns.Models;
using Campaigns.Services;
using Campaigns.Services.Interfaces;
using Campaigns.Services.Repository;
using Campaigns.Services.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var dbConfig = builder.Configuration.GetSection("MongoDbSettings");
builder.Services.Configure<MongoDbSettings>(dbConfig);

builder.Services.AddSingleton<MongoDbSettings>(serviceProvider => serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);


// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<CampaignMongoRepository>();
builder.Services.AddSingleton<DailyRecordRepository>();
builder.Services.AddSingleton<ICampaignInterface, CampaignServices>();
builder.Services.AddMemoryCache();



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

