using Backend.TopUp.Api.Configuration;
using Backend.TopUp.Api.Middlewares;
using Backend.TopUp.Infrastructure.Configuration.Db;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomDependencyInjection(builder.Configuration);

var app = builder.Build();


app.UseGlobalExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    // Disclaimer: in a real world scenario, sometimes this is unsafe 
    Seed.EnsureDatabaseCreated(app);
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();