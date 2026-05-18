SmartCity Web ServerA robust, enterprise-ready ASP.NET Core Web API powering the SmartCity ecosystem. This service handles real estate management, automated commission calculations, security mapping, and high-performance caching for property lookups.🚀 FeaturesProperty Management: Complete CRUD workflow for retail and commercial properties.Dynamic Commission Engine: Automated commission calculations based on tiered data slabs.Layered Caching: Hybrid caching strategy utilizing slide/absolute expiration via an abstraction layer to reduce database bottlenecks.Secure Authentication: Integrated ASP.NET Core Identity engine using customized JWT bearer tokens for Role-Based Access Control (Broker, HouseSeeker).Testing Infrastructure: Comprehensive unit testing pipeline powered by NUnit and Moq.🛠️ Tech Stack & ArchitectureFramework: .NET 8.0 / ASP.NET Core Web APIDatabase: Entity Framework Core (SQL Server / PostgreSQL)Security: ASP.NET Core Identity + JWT Bearer AuthenticationTesting: NUnit, MoqCaching: MemoryCache / Distributed Cache📋 PrerequisitesBefore running this project locally, ensure you have the following installed:.NET 8.0 SDK or laterVisual Studio 2022 (v17.8+) or JetBrains RiderAn instance of SQL Server (LocalDB or Docker instance)⚙️ Getting Started1. Clone the RepositoryBashgit clone https://github.com/your-username/smartcity-webserver.git
cd smartcity-webserver
2. Configure Environment SettingsNavigate to the Web Server directory and update the connection string and app settings in appsettings.json (or use appsettings.Development.json):JSON{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartCityDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "CacheSettings": {
    "CommissionRatesExpiryMinutes": 30
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsLongEnoughToSatisfySecurityRequirements",
    "Issuer": "SmartCityIssuer",
    "Audience": "SmartCityAudience"
  }
}
3. Run Database MigrationsEnsure your connection string is active, then execute the following command in the Package Manager Console or your CLI terminal:Bashdotnet ef database update --project SmartCity.Infrastructure --startup-project SmartCity_WebServer
4. Build and Run the ApplicationBashdotnet build
dotnet run --project SmartCity_WebServer
The API will spin up and should be accessible by default at https://localhost:7001 or http://localhost:5001.🧪 Running Unit TestsThe project includes a comprehensive unit testing layer checking logic isolation models across controllers, validation pipelines, and business services.To execute the test suite, run:Bashdotnet test
📌 Core API EndpointsMethodEndpointAllowed RolesDescriptionPOST/api/auth/loginAnonymousAuthenticates a user and returns a JWT token.GET/api/RetailProperty/PropertiesBroker, HouseSeekerFetches active property listings (Cached).POST/api/RetailProperty/CreateBrokerAdds a new listing & calculates commission.PUT/api/RetailProperty/Update/{id}BrokerModifies an existing listing.DELETE/api/RetailProperty/Delete/{id}BrokerEvicts a listing from database and flushes cache.📂 Project StructurePlaintext├── SmartCity.Application/      # Interfaces, DTOs, Business Services & Domain Logic
├── SmartCity.Domain/           # Database Entities, Models, Core Constants
├── SmartCity.Infrastructure/   # Data Context, EF Core Migrations, Repository Implementations
├── SmartCity_WebServer/        # ASP.NET Core Controllers, Dependency Injection, Middleware Configuration
└── SmartCity.Tests/            # NUnit / Moq Test Frameworks
📄 LicenseThis project is licensed under the MIT License - see the LICENSE file for details.
