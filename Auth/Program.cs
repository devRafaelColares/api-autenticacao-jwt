using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Context;
using Auth.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddScoped<IDatabaseContext, DatabaseContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new ArgumentNullException("Jwt:Secret");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict; // Proteção contra CSRF
    options.Cookie.Name = "jwt_token";
    options.Events.OnValidatePrincipal = async context =>
    {
        var token = context.Request.Cookies["jwt_token"];
        if (string.IsNullOrEmpty(token))
        {
            context.RejectPrincipal();
            return;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
            ValidateLifetime = true
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            context.Principal = principal;
        }
        catch
        {
            context.RejectPrincipal();
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("levelA", policy => policy.RequireClaim(ClaimTypes.Email).RequireClaim(ClaimTypes.Role, "A"));
    options.AddPolicy("levelB", policy => policy.RequireClaim(ClaimTypes.Email).RequireClaim(ClaimTypes.Role, "B"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Adicione essa linha para habilitar a autenticação de cookies
app.UseAuthorization();

app.MapControllers();

app.Run();
