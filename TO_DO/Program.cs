using TO_DO.Servises;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using TO_DO.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using TO_DO.Auth;
using TO_DO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TO_DOContext"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ToDoDbContext>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer("Bearer", options=>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = "https://localhost:5000",
            ValidAudience = "https://localhost:5000",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Super Sequrity Key")),

        };
    });

builder.Services.AddAuthorization(options=>
{
    options.AddPolicy("CanTest", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new CanTestRequirment());
    });    
});

builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title="ToDo",
            Version="v1",
        });

    setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
    });
    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            }, new string[]{}
        }
    });
    var filePath = Path.Combine(AppContext.BaseDirectory, "TO_DO.xml");
    setup.IncludeXmlComments(filePath);
}
    );
builder.Services.AddScoped<IToDoService, ToDoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
///<summary></summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();



app.MapControllers();

app.Run();
