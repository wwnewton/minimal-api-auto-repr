# Publishing MinimalApiAutoRepr to NuGet.org

## Summary of Changes Made

### 1. Updated `MinimalApiAutoRepr.csproj`
- ✅ Fixed `RepositoryUrl` to point to your actual GitHub repo
- ✅ Added `PackageProjectUrl` for the package listing
- ✅ Added `PackageReadmeFile` to include README in the package
- ✅ Added `PackageLicenseExpression` (MIT)
- ✅ Added SourceLink support (`Microsoft.SourceLink.GitHub`)
- ✅ Enabled symbol packages (`.snupkg`) for better debugging
- ✅ Updated Authors and Company to `wwnewton`
- ✅ Added more package tags for better discoverability

### 2. Created GitHub Actions Workflows
- ✅ `.github/workflows/build.yml` - Runs on every push/PR
- ✅ `.github/workflows/publish-nuget.yml` - Publishes to NuGet.org

### 3. Created LICENSE File
- ✅ Added MIT License (you can change this if preferred)

## Steps to Publish

### One-Time Setup

1. **Get a NuGet API Key**
   - Go to https://www.nuget.org
   - Sign in (or create an account)
   - Go to your profile → API Keys
   - Click "Create"
   - Set a name (e.g., "MinimalApiAutoRepr GitHub Actions")
   - Select scope: "Push new packages and package versions"
   - Set glob pattern: `MinimalApiAutoRepr`
   - Copy the API key (you'll only see it once!)

2. **Add the API Key to GitHub Secrets**
   - Go to your GitHub repo: https://github.com/wwnewton/minimal-api-auto-repr
   - Click Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `NUGET_API_KEY`
   - Value: [paste your NuGet API key]
   - Click "Add secret"

### Publishing Options

#### Option A: Publish via GitHub Release (Recommended)
1. Commit and push all changes
2. Go to your GitHub repo → Releases → "Create a new release"
3. Click "Choose a tag" → Type a new tag (e.g., `v0.1.8`)
4. Fill in the release title and description
5. Click "Publish release"
6. The workflow will automatically run and publish to NuGet.org

#### Option B: Manual Trigger
1. Go to Actions → "Publish NuGet Package" → "Run workflow"
2. Click "Run workflow" button
3. The package will be published to NuGet.org

#### Option C: Local Publishing
```bash
# Build and pack
dotnet pack MinimalApiAutoRepr/MinimalApiAutoRepr.csproj -c Release

# Push to NuGet (replace with your actual API key)
dotnet nuget push MinimalApiAutoRepr/bin/Release/MinimalApiAutoRepr.0.1.8.nupkg \
    --api-key YOUR_NUGET_API_KEY \
    --source https://api.nuget.org/v3/index.json
```

## Package Metadata in NuGet.org

Your package will display with:
- **Project URL**: https://github.com/wwnewton/minimal-api-auto-repr
- **Repository**: Link to GitHub with commit information
- **README**: Your full README.md will be shown on the package page
- **License**: MIT License
- **Tags**: minimal-api, generator, routegroup, openapi, aspnetcore, source-generator

## Version Management

To release a new version:
1. Update the `<Version>` in `MinimalApiAutoRepr.csproj`
2. Commit and push
3. Create a new GitHub release with matching tag (e.g., `v0.1.9`)
4. The workflow will automatically build and publish

## Validation Before Publishing

Run these commands locally to ensure everything works:

```bash
# Restore and build
dotnet restore
dotnet build -c Release

# Run tests
dotnet test -c Release

# Create the package
dotnet pack MinimalApiAutoRepr/MinimalApiAutoRepr.csproj -c Release

# Inspect the package (optional)
dotnet nuget verify MinimalApiAutoRepr/bin/Release/MinimalApiAutoRepr.0.1.8.nupkg
```

## What's Included in the Package

- ✅ Analyzer DLL in `analyzers/dotnet/cs/` folder
- ✅ README.md
- ✅ License information
- ✅ Repository URL and commit SHA (via SourceLink)
- ✅ Symbol package (.snupkg) for debugging

## Notes

- **SourceLink**: Enables debugging into your source code directly from NuGet
- **Symbol Package**: Allows developers to step into your generator code while debugging
- **README in Package**: Shows your documentation on the NuGet.org package page
- **License**: MIT is permissive and common for OSS, but you can change to Apache-2.0, BSD-3-Clause, etc.

## Troubleshooting

If publish fails:
- Verify your NuGet API key is correct in GitHub Secrets
- Ensure the version number is unique (NuGet doesn't allow overwriting)
- Check that the package ID is available (search on nuget.org first)
- Review the GitHub Actions logs for specific errors
