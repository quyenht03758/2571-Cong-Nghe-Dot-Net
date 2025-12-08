-- =============================================
-- K8s Manager - Migration: Create Initial Versions for Existing Templates
-- Version: 1.0.0
-- Date: December 4, 2025
-- Purpose: Migrate old templates (without versions) to new version-based structure
-- =============================================

USE K8sManager;
GO

PRINT 'Migrating existing templates to versioned structure...';
GO

-- Check if TemplateVersion table exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TemplateVersion')
BEGIN
    PRINT 'ERROR: TemplateVersion table does not exist. Please run create_tables.sql first.';
    RETURN;
END
GO

-- Migrate templates that don't have any versions yet
-- This handles templates created before the version system was implemented
DECLARE @TemplateId INT;
DECLARE @TemplateName NVARCHAR(200);
DECLARE @YamlContent NVARCHAR(MAX);
DECLARE @CreatedBy INT;
DECLARE @MigratedCount INT = 0;

DECLARE template_cursor CURSOR FOR
    SELECT t.Id, t.Name, t.YamlContent, t.CreatedBy
    FROM Template t
    LEFT JOIN TemplateVersion tv ON t.Id = tv.TemplateId
    WHERE tv.Id IS NULL  -- Templates without any versions
    AND t.YamlContent IS NOT NULL  -- Only migrate templates that have content
    AND LTRIM(RTRIM(t.YamlContent)) != '';  -- Non-empty content

OPEN template_cursor;
FETCH NEXT FROM template_cursor INTO @TemplateId, @TemplateName, @YamlContent, @CreatedBy;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Create initial version (v1) for this template
    INSERT INTO TemplateVersion (TemplateId, VersionNumber, YamlContent, ChangeLog, IsCurrent, CreatedBy)
    VALUES (
        @TemplateId,
        1,  -- Initial version
        @YamlContent,
        'Initial version (migrated from legacy template)',
        1,  -- Set as current version
        @CreatedBy
    );
    
    SET @MigratedCount = @MigratedCount + 1;
    PRINT 'Created version 1 for template: ' + @TemplateName + ' (ID: ' + CAST(@TemplateId AS VARCHAR) + ')';
    
    FETCH NEXT FROM template_cursor INTO @TemplateId, @TemplateName, @YamlContent, @CreatedBy;
END

CLOSE template_cursor;
DEALLOCATE template_cursor;

PRINT '';
PRINT 'Migration completed. Total templates migrated: ' + CAST(@MigratedCount AS VARCHAR);
GO

-- Verify migration
PRINT '';
PRINT 'Verification: Templates without versions:';
SELECT COUNT(*) AS TemplatesWithoutVersions
FROM Template t
LEFT JOIN TemplateVersion tv ON t.Id = tv.TemplateId
WHERE tv.Id IS NULL;
GO

PRINT 'Verification: Templates with versions:';
SELECT 
    t.Name AS TemplateName,
    COUNT(tv.Id) AS VersionCount,
    MAX(CASE WHEN tv.IsCurrent = 1 THEN tv.VersionNumber ELSE NULL END) AS CurrentVersion
FROM Template t
LEFT JOIN TemplateVersion tv ON t.Id = tv.TemplateId
GROUP BY t.Id, t.Name
ORDER BY t.Name;
GO

PRINT '';
PRINT 'Migration script completed successfully.';
GO
