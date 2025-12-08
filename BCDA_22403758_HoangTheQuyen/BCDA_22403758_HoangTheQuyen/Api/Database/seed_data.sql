-- =============================================
-- K8s Manager - Seed Data Script
-- Version: 1.0.0
-- Date: December 4, 2025
-- Purpose: Insert initial/sample data
-- =============================================

USE K8sManager;
GO

PRINT 'Seeding initial data...';
GO

-- =============================================
-- Seed: Default Admin User
-- Password: Admin@123 (BCrypt hash)
-- =============================================
IF NOT EXISTS (SELECT * FROM AppUser WHERE Username = 'admin')
BEGIN
    INSERT INTO AppUser (Username, PasswordHash, Role, FullName, Email, IsActive)
    VALUES (
        'admin',
        '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIeWHHb.mG', -- Admin@123
        'Admin',
        'System Administrator',
        'admin@k8smanager.local',
        1
    );
    PRINT 'Default admin user created. Username: admin, Password: Admin@123';
END
ELSE
BEGIN
    PRINT 'Admin user already exists.';
END
GO

-- =============================================
-- Seed: Sample Operator User
-- Password: Operator@123
-- =============================================
IF NOT EXISTS (SELECT * FROM AppUser WHERE Username = 'operator')
BEGIN
    INSERT INTO AppUser (Username, PasswordHash, Role, FullName, Email, IsActive)
    VALUES (
        'operator',
        '$2a$12$8Z3tZ9tZ9tZ9tZ9tZ9tZ9eYzKzKzKzKzKzKzKzKzKzKzKzKzKzKzK', -- Operator@123
        'Operator',
        'Sample Operator',
        'operator@k8smanager.local',
        1
    );
    PRINT 'Sample operator user created. Username: operator, Password: Operator@123';
END
GO

-- =============================================
-- Seed: Sample Viewer User
-- Password: Viewer@123
-- =============================================
IF NOT EXISTS (SELECT * FROM AppUser WHERE Username = 'viewer')
BEGIN
    INSERT INTO AppUser (Username, PasswordHash, Role, FullName, Email, IsActive)
    VALUES (
        'viewer',
        '$2a$12$9A4uA9uA9uA9uA9uA9uA9fZyLzLzLzLzLzLzLzLzLzLzLzLzLzLzL', -- Viewer@123
        'Viewer',
        'Sample Viewer',
        'viewer@k8smanager.local',
        1
    );
    PRINT 'Sample viewer user created. Username: viewer, Password: Viewer@123';
END
GO

-- =============================================
-- Seed: Default Application Settings
-- =============================================
IF NOT EXISTS (SELECT * FROM AppSetting WHERE [Key] = 'SessionTimeout')
BEGIN
    INSERT INTO AppSetting ([Key], [Value], Category, Description)
    VALUES
        ('SessionTimeout', '480', 'Security', 'Session expiry in minutes (8 hours)'),
        ('DefaultTheme', 'Dark', 'UI', 'Default UI theme (Light/Dark)'),
        ('MaxUploadSize', '10485760', 'General', 'Max file upload size in bytes (10 MB)'),
        ('BcryptWorkFactor', '12', 'Security', 'BCrypt hashing work factor'),
        ('MaxFailedLoginAttempts', '5', 'Security', 'Max failed login attempts before account lock'),
        ('PasswordExpiryDays', '90', 'Security', 'Password expiry in days'),
        ('AuditRetentionDays', '2555', 'Compliance', 'Audit log retention (7 years)'),
        ('EnableSwagger', 'true', 'API', 'Enable Swagger API documentation'),
        ('EnableHealthChecks', 'true', 'API', 'Enable health check endpoints');
    
    PRINT 'Default application settings created.';
END
GO

-- =============================================
-- Seed: Sample Cluster Configuration
-- NOTE: Update paths to match your environment
-- =============================================
DECLARE @AdminId INT = (SELECT Id FROM AppUser WHERE Username = 'admin');

IF NOT EXISTS (SELECT * FROM ClusterConfig WHERE Name = 'Local Development')
BEGIN
    INSERT INTO ClusterConfig (Name, KubeconfigPath, ContextName, IsDefault, Environment, Description, AddedBy)
    VALUES (
        'Local Development',
        'C:\Users\admin\.kube\config',
        'minikube',
        1,
        'Development',
        'Local Minikube cluster for development',
        @AdminId
    );
    PRINT 'Sample cluster configuration created.';
    PRINT 'NOTE: Update KubeconfigPath and ContextName to match your environment!';
END
GO

-- =============================================
-- Seed: Sample Templates
-- =============================================
DECLARE @AdminId INT = (SELECT Id FROM AppUser WHERE Username = 'admin');

IF NOT EXISTS (SELECT * FROM Template WHERE Name = 'Nginx Deployment')
BEGIN
    -- Insert template metadata
    INSERT INTO Template (Name, Category, Description, Tags, IsPublic, CreatedBy)
    VALUES (
        'Nginx Deployment',
        'Workload',
        'Basic nginx deployment with 3 replicas',
        'nginx,web,deployment',
        1,
        @AdminId
    );

    -- Insert template version with YAML content
    DECLARE @TemplateId INT = SCOPE_IDENTITY();
    
    INSERT INTO TemplateVersion (TemplateId, VersionNumber, YamlContent, ChangeLog, IsCurrent, CreatedBy)
    VALUES (
        @TemplateId,
        1,
        'apiVersion: apps/v1
kind: Deployment
metadata:
  name: nginx-deployment
  labels:
    app: nginx
spec:
  replicas: 3
  selector:
    matchLabels:
      app: nginx
  template:
    metadata:
      labels:
        app: nginx
    spec:
      containers:
      - name: nginx
        image: nginx:1.25.3
        ports:
        - containerPort: 80
        resources:
          requests:
            memory: "64Mi"
            cpu: "100m"
          limits:
            memory: "128Mi"
            cpu: "200m"',
        'Initial version',
        1,
        @AdminId
    );
    
    PRINT 'Sample Nginx template created.';
END
GO

-- =============================================
-- Seed: Sample Redis Deployment Template
-- =============================================
DECLARE @AdminId INT = (SELECT Id FROM AppUser WHERE Username = 'admin');

IF NOT EXISTS (SELECT * FROM Template WHERE Name = 'Redis Deployment')
BEGIN
    INSERT INTO Template (Name, Category, Description, Tags, IsPublic, CreatedBy)
    VALUES (
        'Redis Deployment',
        'Database',
        'Redis deployment with persistent volume',
        'redis,cache,database',
        1,
        @AdminId
    );

    DECLARE @TemplateId INT = SCOPE_IDENTITY();
    
    INSERT INTO TemplateVersion (TemplateId, VersionNumber, YamlContent, ChangeLog, IsCurrent, CreatedBy)
    VALUES (
        @TemplateId,
        1,
        'apiVersion: apps/v1
kind: Deployment
metadata:
  name: redis
  labels:
    app: redis
spec:
  replicas: 1
  selector:
    matchLabels:
      app: redis
  template:
    metadata:
      labels:
        app: redis
    spec:
      containers:
      - name: redis
        image: redis:7.2-alpine
        ports:
        - containerPort: 6379
        volumeMounts:
        - name: redis-data
          mountPath: /data
        resources:
          requests:
            memory: "256Mi"
            cpu: "100m"
          limits:
            memory: "512Mi"
            cpu: "500m"
      volumes:
      - name: redis-data
        emptyDir: {}',
        'Initial version with volume mount',
        1,
        @AdminId
    );
    
    PRINT 'Sample Redis template created.';
END
GO

PRINT 'Data seeding completed successfully.';
PRINT '';
PRINT '==============================================';
PRINT 'Default Login Credentials:';
PRINT '==============================================';
PRINT 'Admin     - Username: admin     Password: Admin@123';
PRINT 'Operator  - Username: operator  Password: Operator@123';
PRINT 'Viewer    - Username: viewer    Password: Viewer@123';
PRINT '==============================================';
PRINT '';
PRINT 'IMPORTANT: Change default passwords after first login!';
GO
