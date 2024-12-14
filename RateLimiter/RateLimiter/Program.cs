using RateLimiter.Middlewares;
using RateLimiter.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<TokenBucketManagementService>();
builder.Services.AddHostedService<LeakingBucketManagementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// app.UseMiddleware<TokenBucketRateLimiterMiddleware>();
app.UseMiddleware<LeakingBucketRateLimiterMiddleware>();

app.MapControllers();

app.Run();
