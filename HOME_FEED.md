# Home feed integration

The Flutter home page reads `GET /api/content` from `API_BASE_URL`.
It displays records from the existing `Videos`, `Musics`, and `Photos` tables,
newest first. No demo content is inserted. Admin uploads must create a record
in these tables and supply a publicly reachable media URL and thumbnail URL.

Query parameters: `kind=music|video|photo` (optional), `q` (optional, max 200
characters), `page` (1–100), `pageSize` (1–60). Response:
`{ "items": [...], "hasMore": false, "page": 1 }`.
Items have `id`, `kind`, `title`, `thumbnailUrl`, `mediaUrl`, `createdAt`.
Paid videos expose metadata only, with an empty `mediaUrl`, until a separate
purchase/entitlement service is implemented. Protect paid files at the storage
layer too; publicly accessible storage URLs are not protected by this endpoint.

Run Flutter from `../z_hamaster_app` with your deployed HTTPS endpoint:

```sh
flutter run --dart-define=API_BASE_URL=https://YOUR_API_HOST
```

For Flutter web, configure `Cors:AllowedOrigins` (or
`Cors__AllowedOrigins__0`) with the website's origin. Development allows
loopback origins. Physical phones cannot access the computer using localhost;
use a reachable HTTPS API host. Rebuild the app after changing this define.

Pull down to refresh after admin uploads. Missing configuration, network errors,
empty catalogs, thumbnail errors, and media failures have explicit UI states.
Search, category filters, pagination, photo zoom, audio/video playback, Firebase
profile and logout are connected. Premium checkout, community, games and push
notifications are not configured; the corresponding UI does not claim otherwise.
View counts and popularity ranking are not invented: the current schema has
neither, so the home feed is labelled as new content.

The banner is generated promotional artwork, not a catalog item. Bengali fonts
are Noto Sans Bengali (SIL Open Font License).

The home design uses ZHAMASTER branding, emerald accents, a two-slide music
banner, vector country flags, and horizontal audio/photo/video shelves. Country
shortcuts search the selected country name using the existing `q` parameter;
they are keyword discovery, not a country metadata filter. Shelves group the
currently loaded catalog page by media kind; “See all” requests that kind's
catalog. Missing media never becomes a fabricated sample listing. The banner's
concert artwork is promotional, and Lato typography is bundled under OFL with
Noto Sans Bengali fallback. The existing API configuration still applies.
