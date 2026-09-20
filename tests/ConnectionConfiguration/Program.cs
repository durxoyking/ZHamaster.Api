using Npgsql;
using Microsoft.Extensions.Configuration;
using ZHamaster.Api.Configuration;
var count = 0;
void Check(bool pass) { if (!pass) throw new Exception($"Test {count + 1} failed"); count++; }
var x = new NpgsqlConnectionStringBuilder(PostgresConnection.Normalize("postgresql://user:p%40ss%3Bword%3D%3A%2B@db.render.com/data"));
Check(x.Host == "db.render.com" && x.Port == 5432 && x.Username == "user" && x.Password == "p@ss;word=:+" && x.Database == "data" && x.SslMode == SslMode.Require);
x = new(PostgresConnection.Normalize("postgres://user:pass@localhost:5439/test?sslmode=disable&connect_timeout=7&application_name=test"));
Check(x.Port == 5439 && x.SslMode == SslMode.Disable && x.Timeout == 7 && x.ApplicationName == "test");
x = new(PostgresConnection.Normalize("postgresql://user:pass@host/db?sslmode=verify-full"));
Check(x.SslMode == SslMode.VerifyFull);
x = new(PostgresConnection.Normalize(" Host=localhost;Database=test;Username=user;Password=\"p;word\" "));
Check(x.Password == "p;word");
foreach (var bad in new string?[] {null, "", "postgresql://host/db", "garbage secret", "Host=x;Password=secret", "postgres://user:secret@host/db?bogus=secret"}) {
try { PostgresConnection.Normalize(bad); throw new Exception("Accepted invalid configuration"); }
catch (InvalidOperationException e) { Check(!e.Message.Contains("secret") && e.InnerException == null); }
}
var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> { ["DATABASE_URL"] = "postgres://user:pass@host/db" }).Build();
Check(new NpgsqlConnectionStringBuilder(PostgresConnection.Resolve(config)).Database == "db");
config["ConnectionStrings:Default"] = "Host=preferred;Database=db;Username=user";
Check(new NpgsqlConnectionStringBuilder(PostgresConnection.Resolve(config)).Host == "preferred");
Console.WriteLine($"Passed {count} connection configuration checks.");
