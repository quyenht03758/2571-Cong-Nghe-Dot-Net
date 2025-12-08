-- =============================================
-- K8s Manager - Database Creation Script
-- Version: 1.0.0
-- Date: December 4, 2025
-- =============================================

-- Create database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'K8sManager')
BEGIN
    CREATE DATABASE K8sManager;
    PRINT 'Database K8sManager created successfully.';
END
ELSE
BEGIN
    PRINT 'Database K8sManager already exists.';
END
GO

USE K8sManager;
GO

PRINT 'Using database: K8sManager';
GO
