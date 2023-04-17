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
using TO_DO.HostedServices;
using Quartz;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
//builder.Services.AddLogging(options =>
//{
//    options.SetMinimumLevel(LogLevel.Information);

//    //options.AddJsonConsole();

//}
//);

//builder.Services.AddHostedService<DatabaseClearJob>();
//builder.Services.AddSingleton<MessageQueue>();
//builder.Services.AddHostedService<TransactionProcessorJob>();
//builder.Services.AddHostedService<ResetTransactionStatusBackgroundService>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
    q.ScheduleJob<DatabaseClearCronJob>(trigger => trigger.WithCronSchedule("0/3 * * ? * * *"));
    //// Just use the name of your job that you created in the Jobs folder.
    //var jobKey = new JobKey("DatabaseClearCronJob");
    //q.AddJob<DatabaseClearCronJob>(opts => opts.WithIdentity(jobKey));

    //q.AddTrigger(opts => opts
    //    .ForJob(jobKey)
    //    .WithIdentity("DatabaseClearCronJob-trigger")
    //    //This Cron interval can be described as "run every minute" (when second is zero)
    //    .WithCronSchedule("0 * * ? * *")
    //);
});

builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

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
