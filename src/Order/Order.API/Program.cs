using Order.Application;
using Order.Infrastructure;
using Order.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOrderApplication();
builder.Services.AddOrderInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

await OrderDbInitializer.MigrateAsync(app.Services);

app.Run();
