using Microsoft.EntityFrameworkCore;
using ZHamaster.Api.Data;
using ZHamaster.Api.Configuration;
using ZHamaster.Api.Services;

var builder = WebApplication.CreateBuilder(args);


var connectionString = PostgresConnection.Resolve(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, postgres => postgres.EnableRetryOnFailure(3)));

builder.Services.AddSingleton<FirebaseService>();


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


app.MapControllers();


app.Run();
