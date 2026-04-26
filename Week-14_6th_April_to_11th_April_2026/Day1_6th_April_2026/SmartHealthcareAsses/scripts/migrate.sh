#!/bin/bash
# ============================================================
# SmartHealthcare – EF Core Migration & Database Setup Script
# Run from the solution root: bash scripts/migrate.sh
# ============================================================

set -e
cd "$(dirname "$0")/.."

echo ""
echo "=== SmartHealthcare Database Setup ==="
echo ""

# Add initial migration (only run once)
echo "[1/2] Creating initial migration..."
dotnet ef migrations add InitialCreate \
  --project SmartHealthcare.Infrastructure \
  --startup-project SmartHealthcare.API \
  --output-dir Migrations

echo ""
echo "[2/2] Applying migration to SQL Server..."
dotnet ef database update \
  --project SmartHealthcare.Infrastructure \
  --startup-project SmartHealthcare.API

echo ""
echo "✅  Database created and seeded successfully!"
echo "    Admin user:  admin@healthcare.com / Admin@12345"
echo ""
