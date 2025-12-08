// Api/Program.cs - REST API Server with Clean Architecture

using K8sManager.Api.Infrastructure;
using K8sManager.Api.Domain.Services;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.Infrastructure.Security;
using K8sManager.Api.Infrastructure.Repositories;
using K8sManager.Services;
using K8sManager.Api.Application.Auth.Commands;
using K8sManager.Api.Application.Auth.Queries;
using K8sManager.Api.Application.Users;
using K8sManager.Api.Application.Templates.Commands;
using K8sManager.Api.Application.Templates.Queries;
using K8sManager.Api.Application.AuditLogs.Commands;
using K8sManager.Api.Application.AuditLogs.Queries;
using K8sManager.Api.Application.Favorites;
using K8sManager.Api.Application.Sessions;
using K8sManager.Api.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

// Infrastructure

// Domain

// Infrastructure Implementations

// Application Layer
var builder = WebApplication.CreateBuilder(args);

// ============ LOGGING CONFIGURATION ============
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ============ CONFIGURATION ============
var jwtKey = builder.Configuration["Jwt:Key"] ?? "K8sManager-Secret-Key-Min32Chars!!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "K8sManager.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "K8sManager.Client";
var jwtExpiryMinutes = int.Parse(builder.Configuration["Jwt:ExpiryMinutes"] ?? "480");
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=K8sManager;Integrated Security=True;TrustServerCertificate=True";

Console.WriteLine("🏗️  Configuring Clean Architecture layers...");

// ============ DEPENDENCY INJECTION - Clean Architecture Layers ============

// 1. Infrastructure Layer - Database
builder.Services.AddSingleton(new DapperConnectionFactory(connectionString));

// 2. Domain Services (Infrastructure implementations)
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton<ITokenGenerator>(sp =>
    new JwtTokenService(jwtKey, jwtIssuer, jwtAudience, jwtExpiryMinutes));
var encryptionKey = builder.Configuration["Encryption:Key"] ?? "K8sManager-Encryption-Key-32Chars!";
builder.Services.AddSingleton<IEncryptionService>(sp => new AesEncryptionService(encryptionKey));

// 3. Domain Repositories (Infrastructure implementations)
builder.Services.AddScoped<IUserRepository, UserRepositoryAdapter>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepositoryAdapter>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepositoryAdapter>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepositoryAdapter>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepositoryAdapter>();
builder.Services.AddScoped<IAppSettingRepository, AppSettingRepositoryAdapter>();
builder.Services.AddScoped<IClusterRepository, ClusterRepositoryAdapter>();

// 3.1 Kubernetes Service
var kubeconfigPath = builder.Configuration["Kubernetes:ConfigPath"] ?? "%USERPROFILE%\\.kube\\config";
builder.Services.AddSingleton(sp => new K8sManager.Api.Infrastructure.K8sClientFactory(kubeconfigPath));
builder.Services.AddScoped<K8sManager.Api.Services.IK8sService, K8sManager.Api.Infrastructure.K8s.K8sServiceImplementation>();

// 4. Application Layer - Command Handlers (Use Cases)
// Auth
builder.Services.AddScoped<LoginCommandHandler>();
builder.Services.AddScoped<K8sManager.Api.Application.Auth.Queries.GetCurrentUserQueryHandler>();

// AuditLogs
builder.Services.AddScoped<CreateAuditLogCommandHandler>();
builder.Services.AddScoped<GetAuditLogsQueryHandler>();
builder.Services.AddScoped<GetAuditLogByIdQueryHandler>();

// Templates
builder.Services.AddScoped<CreateTemplateCommandHandler>();
builder.Services.AddScoped<UpdateTemplateCommandHandler>();
builder.Services.AddScoped<DeleteTemplateCommandHandler>();
builder.Services.AddScoped<CreateTemplateVersionCommandHandler>();
builder.Services.AddScoped<GetTemplatesQueryHandler>();
builder.Services.AddScoped<GetTemplateByIdQueryHandler>();
builder.Services.AddScoped<GetTemplateVersionsQueryHandler>();

// Favorites
builder.Services.AddScoped<CreateFavoriteCommandHandler>();
builder.Services.AddScoped<UpdateFavoriteCommandHandler>();
builder.Services.AddScoped<DeleteFavoriteCommandHandler>();
builder.Services.AddScoped<GetFavoritesQueryHandler>();

// Sessions
builder.Services.AddScoped<CreateSessionCommandHandler>();
builder.Services.AddScoped<DeleteSessionCommandHandler>();
builder.Services.AddScoped<GetActiveSessionsQueryHandler>();
builder.Services.AddScoped<GetMySessionsQueryHandler>();
builder.Services.AddScoped<CleanupExpiredSessionsHandler>();

// Settings
builder.Services.AddScoped<UpsertSettingCommandHandler>();
builder.Services.AddScoped<DeleteSettingCommandHandler>();
builder.Services.AddScoped<GetAllSettingsQueryHandler>();
builder.Services.AddScoped<GetSettingByKeyQueryHandler>();
builder.Services.AddScoped<GetSettingsByCategoryQueryHandler>();

// Users
builder.Services.AddScoped<CreateUserCommandHandler>();
builder.Services.AddScoped<UpdateUserCommandHandler>();
builder.Services.AddScoped<ChangePasswordCommandHandler>();
builder.Services.AddScoped<ResetPasswordCommandHandler>();
builder.Services.AddScoped<LockUserCommandHandler>();
builder.Services.AddScoped<UnlockUserCommandHandler>();
builder.Services.AddScoped<DeleteUserCommandHandler>();
builder.Services.AddScoped<GetUsersQueryHandler>();
builder.Services.AddScoped<GetUserByIdQueryHandler>();

Console.WriteLine("✅ Clean Architecture layers configured");

// ============ AUTHENTICATION & AUTHORIZATION ============
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        // Custom JWT events for logging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"🔴 Auth failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var username = context.Principal?.Identity?.Name;
                Console.WriteLine($"✅ Token validated for user: {username}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Define policies
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("OperatorOrHigher", policy => policy.RequireRole("Admin", "Operator"));
});

Console.WriteLine("🔐 JWT Authentication configured");

// ============ CORS ============
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWinFormClient", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ============ CONTROLLERS & API DOCUMENTATION ============
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serialize enums as strings instead of integers
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();

// Swagger with enhanced documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "K8s Manager API (Clean Architecture)",
        Version = "v1",
        Description = @"
RESTful API for K8s Manager Database Operations

**Architecture**: Clean Architecture (DDD-inspired)
- Domain Layer: Entities, Value Objects, Domain Services
- Application Layer: Use Cases (Commands/Queries)
- Infrastructure Layer: Repositories, External Services
- Presentation Layer: Controllers, DTOs

**Authentication**: JWT Bearer Token
**Authorization**: Role-based (Admin, Operator, Viewer)
"
    });

    // JWT Bearer Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token.
                      Example: 'Bearer eyJhbGci...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Enable XML comments if available
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

Console.WriteLine("📚 Swagger documentation configured");

// ============ BUILD APPLICATION ============
var app = builder.Build();

// Create logger for startup
var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation(new string('=', 60));
logger.LogInformation("   K8S MANAGER API - CLEAN ARCHITECTURE");
logger.LogInformation(new string('=', 60));

// ============ MIDDLEWARE PIPELINE ============

// 0. HTTP Request Logging
app.Use(async (context, next) =>
{
    var requestLogger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var startTime = DateTime.UtcNow;

    requestLogger.LogInformation(
        "🌐 [{Method}] {Path} - Started at {Time}",
        context.Request.Method,
        context.Request.Path,
        startTime.ToString("HH:mm:ss.fff")
    );

    try
    {
        await next().ConfigureAwait(false);

        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
        requestLogger.LogInformation(
            "✅ [{Method}] {Path} - {StatusCode} in {Duration}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            duration
        );
    }
    catch (Exception ex)
    {
        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
        requestLogger.LogError(
            ex,
            "❌ [{Method}] {Path} - Failed in {Duration}ms",
            context.Request.Method,
            context.Request.Path,
            duration
        );
        throw;
    }
});

// 1. Swagger (always enabled for API testing)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "K8s Manager API v1");
    c.RoutePrefix = "swagger"; // Swagger at /swagger
    c.DocumentTitle = "K8s Manager API - Clean Architecture";
});
logger.LogInformation("📖 Swagger UI available at: /swagger");

// 2. Security & CORS
// app.UseHttpsRedirection(); // Commented for local development
app.UseCors("AllowWinFormClient");

// 3. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 4. Map Controllers
app.MapControllers();

// 5. Health check endpoint
app.MapGet("/api/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName
})).WithName("HealthCheck").WithTags("Health");

// ============ STARTUP INFO ============
var urls = builder.Configuration["ASPNETCORE_URLS"]?.Split(';')
    ?? new[] { "http://localhost:5000" };
var baseUrl = urls[0];

logger.LogInformation("");
logger.LogInformation("📍 Endpoints:");
logger.LogInformation("   API Base:    {BaseUrl}", baseUrl);
logger.LogInformation("   Swagger UI:  {BaseUrl}/swagger", baseUrl);
logger.LogInformation("   Health:      {BaseUrl}/api/health", baseUrl);

logger.LogInformation("");
logger.LogInformation("🔑 JWT Configuration:");
logger.LogInformation("   Issuer:      {Issuer}", jwtIssuer);
logger.LogInformation("   Audience:    {Audience}", jwtAudience);
logger.LogInformation("   Expiry:      {Expiry} minutes", jwtExpiryMinutes);

logger.LogInformation("");
logger.LogInformation("💾 Database:");
logger.LogInformation("   Connection:  {ConnectionString}", connectionString);

logger.LogInformation("");
logger.LogInformation("🏗️  Architecture:");
logger.LogInformation("   Pattern:     Clean Architecture (DDD-inspired)");
logger.LogInformation("   Layers:      Domain → Application → Infrastructure → Presentation");
logger.LogInformation("   Auth:        JWT Bearer Token");
logger.LogInformation("   Security:    BCrypt password hashing");

logger.LogInformation("");
logger.LogInformation("✨ Status:      Ready");
logger.LogInformation(new string('=', 60));
logger.LogInformation("");

// ============ RUN APPLICATION ============
app.Run();
