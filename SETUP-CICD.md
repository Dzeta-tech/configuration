# CI/CD Setup Guide

## 🔧 Setup Instructions

### 1. Create GitHub Repository
1. Create new repository: `https://github.com/Dzeta-tech/configuration`
2. Push this code to the repository

### 2. Configure NuGet API Key
1. Go to [NuGet.org](https://www.nuget.org/account/apikeys)
2. Create new API key with:
   - **Key Name**: `Dzeta.Configuration`
   - **Select Scopes**: `Push new packages and package versions`
   - **Select Packages**: `Dzeta.Configuration` (after first manual publish)
3. Copy the generated API key

### 3. Add GitHub Secret
1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `NUGET_API_KEY`
5. Value: Paste your NuGet API key

## 🚀 Publishing Process

### Automatic Publishing
- Push to `master` branch triggers automatic NuGet publish
- Version is specified in `src/Dzeta.Configuration/Dzeta.Configuration.csproj`
- Update version in `.csproj` before pushing to master

### Manual Version Update
```xml
<PropertyGroup>
  <Version>1.0.1</Version>  <!-- Update this -->
</PropertyGroup>
```

## 📋 Checklist

- [ ] GitHub repository created
- [ ] NuGet API key configured
- [ ] GitHub secret `NUGET_API_KEY` added
- [ ] First push to master completed
- [ ] Package published to NuGet successfully

## 🔍 Troubleshooting

**Build fails?**
- Check .NET 8 SDK is available
- Verify all dependencies are compatible

**NuGet publish fails?**
- Verify API key is correct
- Check package name doesn't conflict
- Ensure version number is unique

**Package not found?**
- NuGet indexing takes ~10 minutes
- Check [NuGet.org](https://www.nuget.org/packages/Dzeta.Configuration) directly 