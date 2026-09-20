using Microsoft.AspNetCore.WebUtilities;
using Npgsql;

namespace ZHamaster.Api.Configuration;

public static class PostgresConnection
{
    public static string Resolve(IConfiguration configuration)
    {
        var value = configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(value)) value = configuration["DATABASE_URL"];
        return Normalize(value);
    }

    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("Set ConnectionStrings__Default or DATABASE_URL to the PostgreSQL connection string.");

        try
        {
            value = value.Trim();
            NpgsqlConnectionStringBuilder result;
            if (value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(value, UriKind.Absolute);
                var credentials = uri.UserInfo.Split(':', 2);
                result = new NpgsqlConnectionStringBuilder
                {
                    Host = uri.Host,
                    Port = uri.Port > 0 ? uri.Port : 5432,
                    Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
                    Username = Uri.UnescapeDataString(credentials[0]),
                    Password = credentials.Length == 2 ? Uri.UnescapeDataString(credentials[1]) : "",
                    SslMode = SslMode.Require
                };
                foreach (var parameter in QueryHelpers.ParseQuery(uri.Query))
                {
                    var key = parameter.Key.ToLowerInvariant() switch
                    {
                        "sslmode" => "SSL Mode",
                        "connect_timeout" => "Timeout",
                        "application_name" => "Application Name",
                        "sslrootcert" => "Root Certificate",
                        _ => parameter.Key
                    };
                    var setting = parameter.Value.ToString();
                    if (key == "SSL Mode") setting = setting.Replace("-", "");
                    result[key] = setting;
                }
            }
            else
            {
                result = new NpgsqlConnectionStringBuilder(value);
            }
            if (string.IsNullOrWhiteSpace(result.Host) || string.IsNullOrWhiteSpace(result.Database) ||
                string.IsNullOrWhiteSpace(result.Username))
                throw new ArgumentException();
            return result.ConnectionString;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException)
        {
            // Never include the supplied value or inner exception: either can expose credentials.
            throw new InvalidOperationException("Invalid PostgreSQL configuration. Use a postgres:// URL or Host=...;Database=...;Username=...;Password=... format in ConnectionStrings__Default or DATABASE_URL.");
        }
    }
}
