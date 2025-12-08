-- Fix duplicate IsCurrent=1 versions for each template
-- Keep only the highest version number as current

-- Find templates with multiple IsCurrent=1 versions
DECLARE @TemplateId INT;
DECLARE @MaxVersion INT;

DECLARE template_cursor CURSOR FOR
    SELECT tv.TemplateId
    FROM TemplateVersion tv
    WHERE tv.IsCurrent = 1
    GROUP BY tv.TemplateId
    HAVING COUNT(*) > 1;

OPEN template_cursor;
FETCH NEXT FROM template_cursor INTO @TemplateId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Get the highest version number for this template
    SELECT @MaxVersion = MAX(VersionNumber)
    FROM TemplateVersion
    WHERE TemplateId = @TemplateId AND IsCurrent = 1;
    
    -- Set all versions to IsCurrent=0 first
    UPDATE TemplateVersion
    SET IsCurrent = 0
    WHERE TemplateId = @TemplateId;
    
    -- Set only the highest version as current
    UPDATE TemplateVersion
    SET IsCurrent = 1
    WHERE TemplateId = @TemplateId 
    AND VersionNumber = @MaxVersion;
    
    PRINT 'Fixed template ' + CAST(@TemplateId AS VARCHAR) + ' - set version ' + CAST(@MaxVersion AS VARCHAR) + ' as current';
    
    FETCH NEXT FROM template_cursor INTO @TemplateId;
END

CLOSE template_cursor;
DEALLOCATE template_cursor;

-- Verify results
SELECT t.Id, t.Name, 
       COUNT(CASE WHEN tv.IsCurrent = 1 THEN 1 END) as CurrentVersionCount,
       MAX(CASE WHEN tv.IsCurrent = 1 THEN tv.VersionNumber END) as CurrentVersion
FROM Template t
LEFT JOIN TemplateVersion tv ON t.Id = tv.TemplateId
GROUP BY t.Id, t.Name
HAVING COUNT(CASE WHEN tv.IsCurrent = 1 THEN 1 END) > 1;

PRINT 'Done! Above query should return 0 rows if all duplicates are fixed.';
