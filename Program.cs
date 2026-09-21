using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ZHamaster.Api.Data;
using ZHamaster.Api.Configuration;
using ZHamaster.Api.Services;

var builder = WebApplication.CreateBuilder(args);


var connectionString = PostgresConnection.Resolve(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, postgres => postgres.EnableRetryOnFailure(3)));

var firebaseProject = builder.Configuration["Firebase:ProjectId"] ?? "z-hamaster";
var issuer = $"https://securetoken.google.com/{firebaseProject}";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
        "https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com",
        new FirebaseSigningKeys(issuer), new HttpDocumentRetriever { RequireHttps = true });
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidIssuer = issuer,
        ValidateAudience = true, ValidAudience = firebaseProject,
        ValidateLifetime = true, RequireExpirationTime = true,
        ValidateIssuerSigningKey = true, RequireSignedTokens = true,
        ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
        NameClaimType = "sub", ClockSkew = TimeSpan.FromSeconds(30)
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var subject = context.Principal?.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(subject) || subject.Length > 128)
                context.Fail("Invalid Firebase subject.");
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization(options => options.AddPolicy("Admin", policy =>
    policy.RequireAuthenticatedUser().RequireAssertion(context =>
        context.User.FindFirst("email_verified")?.Value == "true" &&
        (builder.Configuration.GetSection("Admin:Emails").Get<string[]>() ?? [])
            .Contains(context.User.FindFirst("email")?.Value, StringComparer.OrdinalIgnoreCase))));


builder.Services.AddControllers();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    policy.SetIsOriginAllowed(origin => origins.Contains(origin, StringComparer.OrdinalIgnoreCase) ||
        (builder.Environment.IsDevelopment() && Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
            uri.IsLoopback)).WithMethods("GET", "POST", "OPTIONS").AllowAnyHeader();
}));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


var port = builder.Configuration["PORT"] ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();


if(app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// Render terminates HTTPS at its proxy and forwards HTTP to this container.
if (app.Environment.IsDevelopment()) app.UseHttpsRedirection();

if (builder.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/health/ready", async (AppDbContext db, CancellationToken cancellationToken) =>
{
    return await db.Database.CanConnectAsync(cancellationToken)
        ? Results.Ok(new { status = "ready" })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
});

app.UseCors();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
