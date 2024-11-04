using System.Security.Claims;
using System.Text;
using Auth.Context;
using Auth.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddScoped<IDatabaseContext, DatabaseContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new ArgumentNullException("Jwt:Secret");


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret))

        // IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("4d82a63bbdc67c1e4784ed6587f3730c"))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("levelA", policy => policy.RequireClaim(ClaimTypes.Email).RequireClaim(ClaimTypes.Role, "A"));
    options.AddPolicy("levelB", policy => policy.RequireClaim(ClaimTypes.Email).RequireClaim(ClaimTypes.Role, "B"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
