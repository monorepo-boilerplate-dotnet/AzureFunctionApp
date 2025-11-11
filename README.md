# Azure Function App Boilerplate with Cosmos DB

A production-ready boilerplate for building Azure Function applications with Azure Cosmos DB integration. This project follows clean architecture principles with clear separation of concerns across Domain, Infrastructure, and Application layers.

## Features

- **Azure Functions Integration** - Serverless computing with Azure Functions runtime
- **Cosmos DB Integration** - NoSQL database with pre-configured repositories and Unit of Work pattern
- **Clean Architecture** - Well-organized project structure following SOLID principles
- **Authentication & Authorization** - JWT token generation and claims-based security
- **Message Queuing** - RabbitMQ integration for asynchronous communication
- **Logging & Monitoring** - Built-in logging service for application insights
- **Error Handling** - Comprehensive error handling and exception utilities
- **.NET 8.0** - Latest .NET framework with modern features
- **Pre-configured Setup** - All dependencies and configurations ready to use

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Azure Storage Emulator](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-emulator) (optional, for local development)
- Active Azure subscription with:
  - Azure Cosmos DB account
  - Azure Storage account
  - (Optional) Azure Service Bus / RabbitMQ instance

## Project Structure

```
BookReviewer/
├── Domain/                          # Core domain entities and DTOs
│   ├── Entities/                    # Domain entities (BaseEntity.cs)
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Enums/                       # Domain enumerations
│   ├── CosmosDbContext.cs           # Cosmos DB context configuration
│   └── Domain.csproj
│
├── Infrastructure/                  # Infrastructure layer
│   ├── Repositories/                # Cosmos DB generic repository
│   ├── Commons/                     # Common services
│   │   ├── ClaimsService.cs         # Claims extraction
│   │   ├── CurrentTime.cs           # Current time service
│   │   ├── LoggerService.cs         # Logging service
│   │   └── RabbitMqService.cs       # Message queue service
│   ├── Interfaces/                  # Service interfaces
│   ├── Utils/                       # Utility functions
│   │   ├── AuthenTools.cs           # Authentication utilities
│   │   ├── GenerateJWTToken.cs      # JWT token generation
│   │   ├── HashHelper.cs            # Password hashing
│   │   └── ResourceHelper.cs        # Resource utilities
│   ├── UnitOfWork.cs                # Unit of Work pattern
│   └── Infrastructure.csproj
│
├── Application/                     # Application layer
│   ├── Services/                    # Business logic services
│   ├── Interfaces/                  # Service interfaces
│   ├── Worker/                      # Azure Function worker triggers
│   ├── Utils/                       # Application utilities
│   │   ├── ApiResult.cs             # API response wrapper
│   │   ├── ErrorHelper.cs           # Error handling
│   │   ├── ErrorMessages.cs         # Error message constants
│   │   ├── ExceptionUtils.cs        # Exception utilities
│   │   ├── JwtUtils.cs              # JWT utilities
│   │   ├── OtpGenerator.cs          # OTP generation
│   │   └── PasswordHasher.cs        # Password hashing
│   └── Application.csproj
│
└── FuncAppCosmos.sln               # Solution file
```

## Tech Stack

### Core Framework
- **[.NET 8.0](https://dotnet.microsoft.com/)** - Latest .NET platform

### Database
- **[Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/)** - NoSQL database with SDK v3.54.1
- **[Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Microsoft.Azure.Cosmos/)** - Official Cosmos DB client

### Azure Services
- **[Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/)** - Serverless compute
- **[Azure Storage](https://learn.microsoft.com/en-us/azure/storage/)** - Cloud storage

### Security & Authentication
- **[System.IdentityModel.Tokens.Jwt](https://www.nuget.org/packages/System.IdentityModel.Tokens.Jwt/)** - JWT token handling
- **ASP.NET Core Identity** - Authentication and authorization

### Messaging & Integration
- **[RabbitMQ.Client](https://www.nuget.org/packages/RabbitMQ.Client/)** - Message queue client
- **[Resend](https://www.nuget.org/packages/Resend/)** - Email service integration

### Storage & Files
- **[Minio](https://www.nuget.org/packages/Minio/)** - S3-compatible object storage client

### Utilities
- **[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)** - JSON serialization
- **[Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Abstractions/)** - Configuration management
- **[Microsoft.Extensions.Options](https://www.nuget.org/packages/Microsoft.Extensions.Options/)** - Options pattern

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd BookReviewer
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Configure Environment Variables

Create a `local.settings.json` file in the Application project root (if using local development):

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDbConnectionString": "your-cosmos-db-connection-string",
    "CosmosDbDatabaseId": "your-database-name",
    "RabbitMqHostName": "localhost",
    "RabbitMqUserName": "guest",
    "RabbitMqPassword": "guest",
    "JwtSecret": "your-jwt-secret-key",
    "JwtIssuer": "your-app",
    "JwtAudience": "your-app-users"
  }
}
```

### 5. Run the Application

#### Using Visual Studio
- Open `FuncAppCosmos.sln` in Visual Studio
- Set the Application project as the startup project
- Press `F5` to run

#### Using .NET CLI
```bash
cd Application
dotnet run
```

#### Using Azure Functions Core Tools
```bash
func start
```

## Key Components

### Domain Layer
- **Entities**: Base domain entities with common properties
- **DTOs**: Data Transfer Objects for API communication
- **CosmosDbContext**: Cosmos DB configuration and connection management

### Infrastructure Layer
- **CosmosGenericRepository**: Generic repository pattern for Cosmos DB operations
- **UnitOfWork**: Unit of Work pattern for transaction management
- **Services**: Cross-cutting concerns (authentication, logging, messaging)

### Application Layer
- **Services**: Business logic implementation
- **Workers**: Azure Function triggers and handlers
- **Utils**: Helper functions and API response wrappers

## Authentication & Security

The boilerplate includes built-in authentication features:

- **JWT Token Generation** - `GenerateJWTToken.cs` for creating secure tokens
- **Password Hashing** - Secure password hashing with salt
- **Claims Service** - Extract and validate user claims from JWT tokens
- **Token Tools** - Utilities for token validation and refresh

### Example JWT Setup
```csharp
var tokenGenerator = new GenerateJWTToken();
var token = tokenGenerator.GenerateToken(userId, jwtSecret, jwtIssuer, jwtAudience);
```

## Database Operations

The infrastructure layer provides ready-to-use database operations:

```csharp
// Using the Unit of Work pattern
var unitOfWork = new UnitOfWork(cosmosDbContext);
var repository = unitOfWork.GetRepository<YourEntity>();

// Query operations
var items = await repository.GetAllAsync();
var item = await repository.GetByIdAsync(id);

// Create/Update/Delete operations
await repository.AddAsync(entity);
await repository.UpdateAsync(entity);
await repository.DeleteAsync(id);
```

## Email & Messaging

### Resend Email Integration
- Pre-configured email service using Resend
- Suitable for transactional emails

### RabbitMQ Integration
- Asynchronous message processing
- Service implementation in `RabbitMqService.cs`

## API Response Format

All API responses follow a consistent format using `ApiResult<T>`:

```csharp
// Success response
return ApiResult<T>.Success(data, "Operation completed successfully");

// Error response
return ApiResult<T>.Failure("Error message");
```

## Testing

The project structure supports unit testing:

```bash
# Create a test project
dotnet new xunit -n BookReviewer.Tests

# Add reference to projects
dotnet add reference ../Application/Application.csproj

# Run tests
dotnet test
```

## Common Tasks

### Adding a New Entity

1. Create entity in `Domain/Entities/`
2. Add to `CosmosDbContext.cs`
3. Create DTO in `Domain/DTOs/`
4. Create service in `Application/Services/`
5. Add repository methods if needed

### Adding a New Azure Function

1. Create new class in `Application/Worker/`
2. Add `[Function("FunctionName")]` attribute
3. Define trigger (HttpTrigger, TimerTrigger, QueueTrigger, etc.)
4. Implement function logic
5. Update `local.settings.json` if needed

### Setting Up Logging

```csharp
// Inject ILoggerService
private readonly ILoggerService _logger;

// Use logging
_logger.LogInformation("Info message");
_logger.LogError("Error message");
```

## Deployment to Azure

### Deploy using Azure Functions Core Tools

```bash
# Install Azure Functions Core Tools
npm install -g azure-functions-core-tools@4 --unsafe-perm true

# Login to Azure
az login

# Create Azure resources (if not exists)
az functionapp create --resource-group <resource-group> \
  --consumption-plan-location <location> \
  --runtime dotnet-isolated \
  --runtime-version 8.0 \
  --functions-version 4 \
  --name <function-app-name>

# Deploy
func azure functionapp publish <function-app-name>
```

### Deploy using Visual Studio

1. Right-click on Application project
2. Select "Publish"
3. Choose "Azure" as target
4. Select existing function app or create new
5. Click "Publish"

### Configure Application Settings

1. Go to Azure Portal
2. Navigate to your Function App
3. Go to "Configuration" → "Application settings"
4. Add required settings (same as in `local.settings.json`)

## Configuration

### Cosmos DB Configuration

Update `Domain/CosmosDbContext.cs`:

```csharp
private const string ConnectionString = "your-connection-string";
private const string DatabaseId = "your-database-id";
```

Or use environment variables:

```csharp
var connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
var databaseId = Environment.GetEnvironmentVariable("CosmosDbDatabaseId");
```

## Contributing

1. Create a feature branch (`git checkout -b feature/AmazingFeature`)
2. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
3. Push to the branch (`git push origin feature/AmazingFeature`)
4. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions:
- Check existing [GitHub Issues](https://github.com/your-repo/issues)
- Create a new issue with detailed description
- Contact the development team

## Resources

- [Azure Functions Documentation](https://learn.microsoft.com/en-us/azure/azure-functions/)
- [Cosmos DB Documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/)
- [.NET 8.0 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Clean Architecture Guide](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## Next Steps

After cloning this boilerplate:

1. Update `appsettings.json` with your Azure credentials
2. Modify entities in `Domain/Entities/` for your use case
3. Implement business logic in `Application/Services/`
4. Create Azure Function workers in `Application/Worker/`
5. Test locally with Azure Functions Core Tools
6. Deploy to Azure
