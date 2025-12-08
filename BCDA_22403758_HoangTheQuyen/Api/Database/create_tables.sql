-- =============================================
-- K8s Manager - Table Creation Script
-- Version: 1.0.0
-- Date: December 4, 2025
-- =============================================

USE K8sManager;
GO

PRINT 'Creating tables...';
GO

-- =============================================
-- Table: AppUser
-- Purpose: User accounts with authentication
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppUser')
BEGIN
    CREATE TABLE AppUser (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(200) NOT NULL,
        Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin','Operator','Viewer')),
        FullName NVARCHAR(200) NULL,
        Email NVARCHAR(200) NULL,
        IsLocked BIT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        FailedLoginAttempts INT NOT NULL DEFAULT 0,
        LastLoginAt DATETIME2 NULL,
        LastPasswordChangedAt DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

        INDEX IX_AppUser_Username (Username),
        INDEX IX_AppUser_Email (Email),
        INDEX IX_AppUser_IsLocked (IsLocked)
    );
    PRINT 'Table AppUser created.';
END
GO

-- =============================================
-- Table: UserSession
-- Purpose: Active user sessions with JWT tokens
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserSession')
BEGIN
    CREATE TABLE UserSession (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL FOREIGN KEY REFERENCES AppUser(Id) ON DELETE CASCADE,
        SessionToken NVARCHAR(500) NOT NULL UNIQUE,
        IpAddress NVARCHAR(50) NULL,
        UserAgent NVARCHAR(500) NULL,
        ExpiresAt DATETIME2 NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

        INDEX IX_UserSession_Token (SessionToken),
        INDEX IX_UserSession_UserId (UserId),
        INDEX IX_UserSession_ExpiresAt (ExpiresAt)
    );
    PRINT 'Table UserSession created.';
END
GO

-- =============================================
-- Table: ClusterConfig
-- Purpose: Kubernetes cluster configurations
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ClusterConfig')
BEGIN
    CREATE TABLE ClusterConfig (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL UNIQUE,
        KubeconfigPath NVARCHAR(500) NOT NULL,
        ContextName NVARCHAR(200) NOT NULL,
        IsDefault BIT NOT NULL DEFAULT 0,
        Environment NVARCHAR(50) NULL,
        Description NVARCHAR(500) NULL,
        AddedBy INT NOT NULL FOREIGN KEY REFERENCES AppUser(Id),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

        INDEX IX_ClusterConfig_AddedBy (AddedBy),
        INDEX IX_ClusterConfig_IsDefault (IsDefault),
        INDEX IX_ClusterConfig_Environment (Environment)
    );
    PRINT 'Table ClusterConfig created.';
END
GO

-- =============================================
-- Table: AuditLog
-- Purpose: Immutable audit trail for compliance
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLog')
BEGIN
    CREATE TABLE AuditLog (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL FOREIGN KEY REFERENCES AppUser(Id),
        ClusterId INT NULL FOREIGN KEY REFERENCES ClusterConfig(Id),
        Action NVARCHAR(100) NOT NULL,
        ResourceKind NVARCHAR(50) NULL,
        ResourceName NVARCHAR(300) NULL,
        Namespace NVARCHAR(200) NULL,
        Success BIT NOT NULL,
        ErrorMessage NVARCHAR(1000) NULL,
        RequestPayload NVARCHAR(MAX) NULL,
        ResponseData NVARCHAR(MAX) NULL,
        IpAddress NVARCHAR(50) NULL,
        Duration INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

        INDEX IX_AuditLog_UserId (UserId),
        INDEX IX_AuditLog_ClusterId (ClusterId),
        INDEX IX_AuditLog_Action (Action),
        INDEX IX_AuditLog_CreatedAt (CreatedAt),
        INDEX IX_AuditLog_Success (Success)
    );
    PRINT 'Table AuditLog created.';
END
GO

-- =============================================
-- Table: Template
-- Purpose: YAML template metadata
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Template')
BEGIN
    CREATE TABLE Template (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Category NVARCHAR(50) NULL,
        Description NVARCHAR(500) NULL,
        Tags NVARCHAR(500) NULL,
        IsPublic BIT NOT NULL DEFAULT 0,
        CreatedBy INT NOT NULL FOREIGN KEY REFERENCES AppUser(Id),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

        UNIQUE (Name, CreatedBy),
        INDEX IX_Template_CreatedBy (CreatedBy),
        INDEX IX_Template_Category (Category),
        INDEX IX_Template_IsPublic (IsPublic)
    );
    PRINT 'Table Template created.';
END
GO

-- =============================================
-- Table: TemplateVersion
-- Purpose: Version-controlled YAML content
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TemplateVersion')
BEGIN
    CREATE TABLE TemplateVersion (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TemplateId INT NOT NULL FOREIGN KEY REFERENCES Template(Id) ON DELETE CASCADE,
        VersionNumber INT NOT NULL,
        YamlContent NVARCHAR(MAX) NOT NULL,
        ChangeLog NVARCHAR(1000) NULL,
        IsCurrent BIT NOT NULL DEFAULT 1,
        CreatedBy INT NOT NULL FOREIGN KEY REFERENCES AppUser(Id),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

        UNIQUE (TemplateId, VersionNumber),
        INDEX IX_TemplateVersion_TemplateId (TemplateId),
        INDEX IX_TemplateVersion_IsCurrent (IsCurrent)
    );
    PRINT 'Table TemplateVersion created.';
END
GO

-- =============================================
-- Table: Favorite
-- Purpose: User bookmarks for resources
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Favorite')
BEGIN
    CREATE TABLE Favorite (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL FOREIGN KEY REFERENCES AppUser(Id) ON DELETE CASCADE,
        ClusterId INT NOT NULL FOREIGN KEY REFERENCES ClusterConfig(Id) ON DELETE CASCADE,
        Namespace NVARCHAR(200) NULL,
        ResourceKind NVARCHAR(50) NULL,
        ResourceName NVARCHAR(300) NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        Notes NVARCHAR(500) NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

        INDEX IX_Favorite_UserId (UserId),
        INDEX IX_Favorite_ClusterId (ClusterId),
        INDEX IX_Favorite_SortOrder (SortOrder)
    );
    PRINT 'Table Favorite created.';
END
GO

-- =============================================
-- Table: AppSetting
-- Purpose: Application configuration settings
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppSetting')
BEGIN
    CREATE TABLE AppSetting (
        [Key] NVARCHAR(100) PRIMARY KEY,
        [Value] NVARCHAR(MAX) NULL,
        Category NVARCHAR(50) NULL,
        Description NVARCHAR(500) NULL,
        IsEncrypted BIT NOT NULL DEFAULT 0,
        UpdatedBy INT NULL FOREIGN KEY REFERENCES AppUser(Id),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
    PRINT 'Table AppSetting created.';
END
GO

PRINT 'All tables created successfully.';
GO
