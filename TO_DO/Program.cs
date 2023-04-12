using TO_DO.Servises;
using Microsoft.EntityFrameworkCore;
using TO_DO.Data;
using TO_DO.Auth;
using TO_DO;
using FluentValidation.AspNetCore;
using FluentValidation;
using TO_DO.DTOs.Validation;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
//builder.Services.AddLogging(options =>
//{
//    options.SetMinimumLevel(LogLevel.Information);

//    //options.AddJsonConsole();

//}
//);

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithProcessName()
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:w3}] {Message:j}{NewLine}" +
                "ThreadId: {ThreadId}{NewLine}" +
                "ThreadName: {ThreadName}{NewLine}" +
                "ProcessNmae: {ProcessName}{NewLine}{Exception}")
                //.WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
builder.Host.UseSerilog();


builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TO_DOContext"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMemoryCache();


builder.Services.AddFluentValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AuthenticationAndAuthorization(builder.Configuration);


builder.Services.AddSwagger();

builder.Services.AddScoped<IToDoService, ToDoService>();



var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x => x.EnablePersistAuthorization());
}

//app.UseSerilogRequestLogging();

app.UseResponseCaching();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
