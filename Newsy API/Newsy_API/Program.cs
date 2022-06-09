using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newsy_API.AuthenticationModel;
using Newsy_API.DAL;
using Newsy_API.DAL.Repositories.Articles;
using Newsy_API.DAL.Repositories.Authors;
using Newsy_API.DAL.Repositories.Users;
using Newsy_API.Helpers;
using Newsy_API.Settings;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//configure identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
        options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
        })
    .AddEntityFrameworkStores<NewsyDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


//configure Db context
builder.Services.AddDbContext<NewsyDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection"));
});

//configure repositories
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


//configure automapper
builder.Services.AddAutoMapper(typeof(Program));

var tokenSettingsSection = builder.Configuration.GetSection("TokenSettings");
builder.Services.Configure<TokenSettings>(tokenSettingsSection);
var tokenSettings = tokenSettingsSection.Get<TokenSettings>();
var key = Encoding.ASCII.GetBytes(tokenSettings.Secret);

// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = tokenSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = tokenSettings.Audience,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MyArticles",
        policy => policy.Requirements.Add(new MyArticlesRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, MyArticlesRequirementHandler>();

//configure logging
//TODO: can be done better and easier to read
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = false;
    options.TimestampFormat = "dd.MM.yyyy hh:mm:ss ";
    options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() /*Only for Five purposes*/)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
