using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Travidia API",
        Version = "v1"
    });
});

// Setting cors 
builder.Services.AddCors((options) => {
    // Development enviroment cors origin
    options.AddPolicy("DevCors", (corsBuilder) => {
        corsBuilder.WithOrigins("")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });

    // Production enviroment cors origin
    options.AddPolicy("ProdCors", (corsBuilder) => {
        corsBuilder.WithOrigins("")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
// Settings authenticacion
string? tokenKeyString = builder.Configuration.GetSection("Appsettings:TokenKey").Value;

SymmetricSecurityKey tokenKey = new(
    Encoding.UTF8.GetBytes(
        tokenKeyString ?? ""
    )
);

TokenValidationParameters tokenValidationParameters = new()
{
    IssuerSigningKey = tokenKey, 
    ValidateIssuer = false, 
    ValidateIssuerSigningKey = false,
    ValidateAudience = false
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = tokenValidationParameters;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
