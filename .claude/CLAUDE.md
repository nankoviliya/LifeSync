# CLAUDE.md - LifeSync AI Assistant Guide

> **Last Updated**: 2025-11-24
> **Purpose**: High-level guide for AI assistants working on the LifeSync codebase.

---

## Project Overview

**LifeSync** is a unified platform for managing personal tasks on a daily basis, featuring personal finance management, user authentication, and multi-language/multi-currency support.

### Project Philosophy
- **Clean Architecture**: Separation of concerns, maintainable codebase
- **Security First**: Proper authentication, authorization, and input validation
- **Modern Stack**: .NET 10, React, Angular
- **Developer Experience**: Clear conventions, comprehensive documentation

---

## Repository Structure

```
LifeSync/
├── .github/workflows/          # CI/CD pipelines
├── .claude/                    # AI assistant documentation
│   ├── CLAUDE.md              # This file
│   ├── agents/                # Specialized agent prompts
│   └── prompts/               # Reusable prompt templates
├── src/
│   ├── backend/               # .NET 10 API (FastEndpoints, EF Core, SQL Server)
│   ├── frontend/
│   │   ├── life-sync-react-client/    # React + TypeScript + Vite
│   │   └── life-sync-angular-client/  # Angular 19
│   └── scripts/               # Development scripts
├── docker-compose.yml         # Multi-container orchestration
└── README.md
```

---

## Technology Stack

### Backend
- **.NET 10** with C# 14
- **FastEndpoints** for API endpoints (replaces MVC controllers)
- **Entity Framework Core 10** with SQL Server
- **ASP.NET Core Identity** + JWT authentication
- **Serilog** for structured logging
- **xUnit** for testing

### Frontend
- **React 18** (TypeScript, Vite, TanStack Query, PrimeReact, i18next)
- **Angular 19** (TypeScript)

### Infrastructure
- **Docker & Docker Compose** for development
- **GitHub Actions** for CI/CD
- **AWS Secrets Manager** for production secrets

---

## Architecture Overview

### Backend Architecture
- **Feature-based organization**: Vertical slices (all related code in one feature folder)
- **FastEndpoints pattern**: `Endpoint<TRequest, TResponse>` classes instead of controllers
- **Result pattern**: `DataResult<T>` and `MessageResult` for operation outcomes
- **Soft delete**: Entities marked as deleted (not physically removed)
- **Dependency injection**: Services registered via extension methods

### Frontend Architecture
- **Component-based**: React functional components, Angular modules
- **State management**: TanStack Query for server state
- **i18n**: Multi-language support via i18next
- **TypeScript**: Strict mode enabled

---

## Development Workflows

### Local Development

**Backend Setup:**
```bash
cd src/backend/src/LifeSync.API
dotnet user-secrets set "AppSecrets:JWT:SecretKey" "YOUR_KEY"
dotnet user-secrets set "AppSecrets:Database:Username" "YOUR_USER"
dotnet user-secrets set "AppSecrets:Database:Password" "YOUR_PASS"
cd ../..
dotnet ef database update --project src/LifeSync.API
dotnet run --project src/LifeSync.API
```

**Frontend Setup:**
```bash
cd src/frontend/life-sync-react-client  # or life-sync-angular-client
npm install
npm start
```

**Docker Development:**
```bash
docker-compose pull
docker-compose up  # Starts API, frontends, and SQL Server
```

### Git Workflow
- **Branch naming**: `feature/`, `fix/`, `hotfix/`, `claude/`
- **Commit format**: Conventional commits (`feat:`, `fix:`, `docs:`, etc.)
- **CI/CD**: GitHub Actions runs on push/PR to `master` (restore, build, test)

---

## Key Conventions

### Backend (.NET)
- **File-scoped namespaces** (C# 10+)
- **Explicit types** (avoid `var` unless obvious)
- **Nullable reference types** enabled
- **TreatWarningsAsErrors**: true (builds fail on warnings)
- See `src/backend/.editorconfig` for complete style rules

### Frontend
- **TypeScript strict mode**
- **Functional components** (React)
- **Component naming**: PascalCase for components, camelCase for functions

---

## Important Patterns

### Result Pattern
Services return `DataResult<T>` (with data) or `MessageResult` (success/failure with errors).

### Soft Delete
Entities implement `ISoftDeletable` with `IsDeleted` and `DeletedAt`. Global query filters automatically exclude soft-deleted records. Use `.IgnoreQueryFilters()` to access deleted records.

### Authentication
- **JWT-based** authentication
- Protected endpoints require `Policies(JwtBearerDefaults.AuthenticationScheme)`
- Secrets: User Secrets (dev), AWS Secrets Manager (prod)

### Validation
FluentValidation via FastEndpoints. Validators auto-run before `HandleAsync()`.

---

## Common Tasks

### Add New API Endpoint
1. Create feature folder: `Features/MyFeature/`
2. Add `MyFeatureEndpoint.cs`, request/response models, validator
3. Create service interface and implementation
4. Register service in `Extensions/ServiceCollectionExtensions.cs`
5. Write tests in `tests/LifeSync.Tests.Unit/`

### Database Migration
```bash
cd src/backend
dotnet ef migrations add MigrationName --project src/LifeSync.API
dotnet ef database update --project src/LifeSync.API
```

### Update Dependencies
```bash
# Backend
dotnet list package --outdated
dotnet add package PackageName

# Frontend
npm outdated
npm update
```

---

## Security Guidelines

- **Never commit secrets**: Use user-secrets (dev) or AWS Secrets Manager (prod)
- **Always validate input**: Use FluentValidation
- **Parameterized queries**: Use EF Core LINQ (avoid raw SQL)
- **Rate limiting**: Apply to public endpoints
- **CORS**: Restrict to trusted origins only
- **JWT secrets**: Strong, randomly generated
- **HTTPS**: Always in production

---

## Testing

- **Framework**: xUnit
- **Pattern**: AAA (Arrange, Act, Assert)
- **Naming**: `MethodName_Scenario_ExpectedBehavior`
- **Run**: `cd src/backend && dotnet test`

---

## Troubleshooting

### Build Errors
- Fix all warnings (TreatWarningsAsErrors=true)
- Check nullable reference type annotations

### Database Issues
- Verify SQL Server is running
- Check connection string in user secrets
- Use `TrustServerCertificate=True` if needed

### Authentication Issues
- Verify JWT token in `Authorization: Bearer {token}` header
- Check token expiration
- Ensure JWT secret matches

### Docker Issues
- Check `.env` file exists with required variables
- Verify ports not in use
- Check logs: `docker-compose logs`

---

## Documentation

### Project-Specific Documentation
- **Backend**: `src/backend/docs/` (soft delete, migrations, etc.)
- **API Changes**: `src/backend/CHANGELOG.md`
- **Agent Prompts**: `.claude/agents/*.md`

### External Resources
- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [FastEndpoints Documentation](https://fast-endpoints.com/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [React Documentation](https://react.dev/)
- [Angular Documentation](https://angular.dev/)

---

## AI Assistant Guidelines

### Best Practices
1. **Always read files before modifying** - Never propose changes to unread code
2. **Follow existing patterns** - Match feature-based structure and conventions
3. **Use Result pattern** - Return `DataResult<T>` or `MessageResult` from services
4. **Validate input** - Create FluentValidation validators
5. **Handle errors** - Provide clear messages, appropriate status codes
6. **Write tests** - Add unit tests for business logic
7. **Document endpoints** - Use FastEndpoints `Summary()` for OpenAPI
8. **Check for warnings** - Build must succeed without warnings
9. **Consider security** - Auth, validation, rate limiting, secrets management
10. **Update documentation** - Keep CLAUDE.md and project docs current

### When Adding Features
- Review similar features first
- Plan database changes carefully
- Write testable code
- Consider performance (caching, query optimization)

### When Reviewing Code
- Correctness, security, performance, maintainability
- Adequate test coverage
- Follows conventions and best practices

---

**For AI Assistants**: Consult project-specific documentation in `src/backend/docs/` and `.claude/agents/` for detailed implementation guidance. When in doubt, ask the user for clarification or refer to existing code examples.
