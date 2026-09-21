# Z Hamaster — production release, 21 September 2026

## Live destinations

- Client: https://z-hamaster.web.app
- Backend: https://zhamaster-api.onrender.com
- Backend source: https://github.com/durxoyking/ZHamaster.Api
- Frontend source (private): https://github.com/durxoyking/z-hamaster-app
- Backend release commit: 9c119f3dbd8f7269a8226474d1bc081788a24e63
- Frontend release commit: 7820007
- Render deployment: dep-daog6rajnfac739lt38g (live)
- Firebase Hosting version: 99d7fa41a5e220d0

## Applied updates

Login redesign and matching animation; consistent Z Hamaster display names;
Home Audio removal; server-side video-country filtering with a global country
list, search and pagination; only playable media in the client catalog.
Removed the unimplemented Premium offer/page from the client.

Database backup completed before release. EF migration
20260921071332_AddVideoCountry is present in the production migration history,
and Videos.CountryCode was verified in information_schema.

Firebase public signing certificates validate backend bearer tokens without
transferring a service-account private key. Public user listing/creation was
removed. Admin access requires an explicitly configured verified email.

## Verification

- Backend build passed with no warnings/errors.
- 12 PostgreSQL connection parser checks passed.
- Flutter analyze: no issues; 13 widget/service tests passed.
- Flutter production web build passed.
- Live index.html, manifest.json, main.dart.js and flutter_bootstrap.js hashes
  match the local production build.
- Live health, readiness, catalog, photo and BD country/page-2 requests: HTTP 200.
- Invalid country and incompatible country/type queries: HTTP 400.
- Unauthenticated users endpoint and invalid-token login: HTTP 401.
- Firebase Hosting origin allowed by production CORS.

## Remaining scope / constraints

- Database currently has zero videos and zero photos; no demo content was seeded.
- No upload/admin UI or payment/entitlement workflow exists. Admin email and
  payment-provider details were requested; no answer received during release.
- A live positive authentication test with a disposable Firebase account was
  blocked by automatic approval review because creation and permanent cleanup of
  that test account/database row need specific user approval. Read-only checks
  were completed; no test account was created. The user chose to test using their
  own real account instead. Real Google OAuth sign-in and media playback with
  actual uploaded media were not verified by the agent in this release.
- Render PostgreSQL is on the existing free plan and reports expiry on
  20 October 2026. No paid upgrade or infrastructure relocation was performed.
- The supplied VPS was inspected successfully. Its existing services were not
  modified; the app remains on its existing Firebase/Render deployment.
- Native store releases were not published; Android production signing remains
  unconfigured in the existing app template.

The pre-release database backup is local at
/tmp/zhamaster-release/pre-release.dump (private permissions). It is not committed
or publicly hosted. Move it to approved durable backup storage before /tmp cleanup.
