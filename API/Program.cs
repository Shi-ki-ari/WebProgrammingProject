using System.Text;
using Common.Persistence;
using Common.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies()
           .UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<LanguageService>();
builder.Services.AddScoped<ActorService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenServices>();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MovieAPI", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = "az",
            ValidAudience = "movieapi",
            IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes("mnogosigurnaparola123456789123456789")),
        };
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieAPI");
    options.RoutePrefix = string.Empty; 
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
