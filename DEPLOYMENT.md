# Render deployment

Set `ConnectionStrings__Default` to the Render PostgreSQL **internal connection URL** for a database in the same region. Both `postgresql://user:password@host/database` URLs and Npgsql `Host=...;Database=...;Username=...;Password=...` strings are accepted. `DATABASE_URL` is a fallback when Default is absent. URL credentials are percent-decoded, and URL connections require TLS unless an explicit sslmode is supplied. Never commit real credentials.

Set `Database__ApplyMigrations=true` to apply the checked-in EF migrations at startup. On a fresh database this creates the application tables. Back up existing databases before future schema changes.

Set `Cors__AllowedOrigins__0=https://z-hamaster.web.app` and `Cors__AllowedOrigins__1=https://z-hamaster.firebaseapp.com` for the web client. Render supplies `PORT`; otherwise the service listens on 8080. Render terminates HTTPS.

`GET /health` checks the process. `GET /health/ready` returns 200 only when PostgreSQL is reachable, otherwise 503. Use the readiness URL as Render's health check. `GET /api/content` verifies schema-backed queries.

Run parser regression checks with `dotnet run --project tests/ConnectionConfiguration` and build with `dotnet build`.

Build the Flutter client with `flutter build web --dart-define=API_BASE_URL=https://zhamaster-api.onrender.com`, then deploy its configured Firebase Hosting site. Firebase Authentication must allow the hosting domain and enable the intended sign-in providers.
