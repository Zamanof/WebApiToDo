using TO_DO.Servises;
using Microsoft.EntityFrameworkCore;
using TO_DO.Data;
using TO_DO.Auth;
using TO_DO;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TO_DOContext"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

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

app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();



app.MapControllers();

app.Run();
