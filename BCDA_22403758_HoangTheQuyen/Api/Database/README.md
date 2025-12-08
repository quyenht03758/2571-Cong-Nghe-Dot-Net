# K8s Manager - Database Setup Guide

## Quick Setup

### Option 1: Using SQL Server Management Studio (SSMS)

1. Open SSMS and connect to your SQL Server instance
2. Open and execute scripts in this order:
   - `create_database.sql` - Creates K8sManager database
   - `create_tables.sql` - Creates all tables with indexes
   - `seed_data.sql` - Inserts default users and settings

### Option 2: Using sqlcmd (Command Line)

```powershell
# Create database
sqlcmd -S (localdb)\MSSQLLocalDB -i create_database.sql

# Create tables
sqlcmd -S (localdb)\MSSQLLocalDB -i create_tables.sql

# Seed initial data
sqlcmd -S (localdb)\MSSQLLocalDB -i seed_data.sql
```

### Option 3: Using PowerShell Script

```powershell
# Run all scripts in order
$server = "(localdb)\MSSQLLocalDB"
sqlcmd -S $server -i create_database.sql
sqlcmd -S $server -i create_tables.sql
sqlcmd -S $server -i seed_data.sql
```

## Default Login Credentials

After running `seed_data.sql`, you can login with:

| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | Admin |
| operator | Operator@123 | Operator |
| viewer | Viewer@123 | Viewer |

**IMPORTANT**: Change these passwords immediately after first login!

## Database Schema

### Tables Created

1. **AppUser** - User accounts with authentication
2. **UserSession** - Active user sessions with JWT tokens
3. **ClusterConfig** - Kubernetes cluster configurations
4. **AuditLog** - Immutable audit trail (7-year retention)
5. **Template** - YAML template metadata
6. **TemplateVersion** - Version-controlled YAML content
7. **Favorite** - User bookmarks for resources
8. **AppSetting** - Application configuration settings

### Connection String

Update your `appsettings.json` with:

```json
{
  "ConnectionStrings": {
    "Default": "Server=(localdb)\\MSSQLLocalDB;Database=K8sManager;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Verification

Check if database was created successfully:

```sql
USE K8sManager;
GO

-- List all tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Count users
SELECT COUNT(*) AS UserCount FROM AppUser;

-- View default settings
SELECT * FROM AppSetting;
```

## Troubleshooting

### Error: Database already exists

If you get "database already exists" error, you can drop and recreate:

```sql
USE master;
GO

DROP DATABASE K8sManager;
GO
```

Then run the scripts again.

### Error: Cannot connect to server

Make sure SQL Server LocalDB is running:

```powershell
# Start LocalDB
sqllocaldb start MSSQLLocalDB

# Check status
sqllocaldb info MSSQLLocalDB
```

## Migration from Existing Database

If you have an existing K8sManager database, backup first:

```sql
BACKUP DATABASE K8sManager
TO DISK = 'C:\Backups\K8sManager_Backup.bak'
WITH FORMAT, COMPRESSION;
```

## Next Steps

1. Update cluster configuration in `ClusterConfig` table with your actual kubeconfig path
2. Change default passwords
3. Configure application settings in `AppSetting` table
4. Run the application and test login

## Support

For issues or questions, check the documentation:
- `COMPREHENSIVE_TECHNICAL_DOCUMENTATION_Part4.md` - Database schema details
- `APPENDIX.md` - SQL commands and troubleshooting
