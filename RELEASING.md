# Releasing

## Branches

- `develop` — day-to-day work. All commits go here.
- `release` — every push builds the installer and publishes a GitHub Release. Only merge `develop` into it, never commit to it directly.

## Versions

The version lives in one place: `<Version>` in [`Directory.Build.props`](Directory.Build.props).
It is stamped into the app (shown in the log on start) and the installer, and becomes the release tag `v<Version>`.

Use [semantic versioning](https://semver.org): `MAJOR.MINOR.PATCH`

- PATCH (`1.0.1`) — bug fixes only
- MINOR (`1.1.0`) — new features, e.g. a new window
- MAJOR (`2.0.0`) — big changes, e.g. settings that no longer carry over

## Making a release

1. On `develop`, bump `<Version>` in `Directory.Build.props` and commit, e.g. `Version 1.1.0`.
2. Merge into `release` and push:
   ```
   git checkout release
   git merge develop
   git push
   git checkout develop
   ```
3. GitHub Actions (`.github/workflows/release.yml`) builds `VsAssistant-Setup-<Version>.exe` and publishes it under
   [Releases](https://github.com/1OL1/VsAssistant/releases) with the tag `v<Version>` and notes generated from the commits.

If the version was not bumped, the workflow fails with "Tag v… already exists" and nothing is published.
Bump the version on `develop` and merge again.

## Building the installer locally

Needs [Inno Setup 6](https://jrsoftware.org/isinfo.php).

```
dotnet publish VsAssistant\VsAssistant.csproj -c Release -r win-x64 --self-contained -o publish
"%LocalAppData%\Programs\Inno Setup 6\ISCC.exe" /DAppVersion=1.0.0 installer\VsAssistant.iss
```

The installer is written to `artifacts\`.
