# ============================================================
# SmartHealthcare – EF Core Migration Script (PowerShell)
# Run from the solution root:  .\scripts\migrate.ps1
# ============================================================

Write-Host ""
Write-Host "=== SmartHealthcare Database Setup ===" -ForegroundColor Cyan
Write-Host ""

Set-Location $PSScriptRoot\..

# Step 1 – Create migration
Write-Host "[1/2] Creating InitialCreate migration..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate `
    --project SmartHealthcare.Infrastructure `
    --startup-project SmartHealthcare.API `
    --output-dir Migrations

if ($LASTEXITCODE -ne 0) {
    Write-Host "Migration already exists or failed – trying update only." -ForegroundColor Yellow
}

# Step 2 – Apply migration
Write-Host ""
Write-Host "[2/2] Applying migration to SQL Server..." -ForegroundColor Yellow
dotnet ef database update `
    --project SmartHealthcare.Infrastructure `
    --startup-project SmartHealthcare.API

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅  Database ready!" -ForegroundColor Green
    Write-Host "    Admin login: admin@healthcare.com  /  Admin@12345" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "❌  Migration failed. Check connection string in appsettings.json." -ForegroundColor Red
}
