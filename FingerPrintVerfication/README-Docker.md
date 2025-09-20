# FingerPrint Verification - Docker Setup

This project includes a complete Docker setup with PostgreSQL database.

## Prerequisites

- Docker Desktop
- Docker Compose

## Quick Start

### 1. Build and Run with Docker Compose

```bash
# Navigate to the project root directory
cd FingerPrintVerfication

# Build and start all services
docker-compose up --build

# Or run in detached mode
docker-compose up --build -d
```

### 2. Access the Application

- **API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger
- **PgAdmin**: http://localhost:5050 (admin@fingerprint.com / admin123)

### 3. Stop Services

```bash
# Stop all services
docker-compose down

# Stop and remove volumes (WARNING: This will delete all data)
docker-compose down -v
```

## Services

### PostgreSQL Database
- **Container**: fingerprint-postgres
- **Port**: 5432
- **Database**: FingerPrintVerificationDb
- **Username**: fingerprint_user
- **Password**: fingerprint_password123

### ASP.NET Core Application
- **Container**: fingerprint-app
- **Port**: 8080
- **Environment**: Development

### PgAdmin (Database Management)
- **Container**: fingerprint-pgadmin
- **Port**: 5050
- **Email**: admin@fingerprint.com
- **Password**: admin123

## API Endpoints

### Person Management
- `POST /api/person` - Add person with 5 fingerprints
- `GET /api/person/verify?fingerprintPath={path}` - Verify person by fingerprint
- `GET /api/person/audit-logs` - Get audit logs
- `GET /api/person` - Get all persons (paginated)
- `GET /api/person/{id}` - Get person by ID
- `DELETE /api/person/{id}` - Delete person

### File Management
- `POST /api/file` - Upload single file
- `POST /api/file/multi` - Upload multiple files
- `GET /api/file/download/{fileName}` - Download file

## Development

### Running Locally (without Docker)

1. Start PostgreSQL:
```bash
docker run -d --name postgres-dev \
  -e POSTGRES_DB=FingerPrintVerificationDb \
  -e POSTGRES_USER=fingerprint_user \
  -e POSTGRES_PASSWORD=fingerprint_password123 \
  -p 5432:5432 \
  postgres:15-alpine
```

2. Run the application:
```bash
cd FingerPrintVerfication
dotnet run
```

### Database Migrations

The application will automatically create the database schema on first run using Entity Framework's `EnsureCreated()` method.

## Troubleshooting

### Common Issues

1. **Port Conflicts**: If ports 8080, 5432, or 5050 are in use, modify the ports in docker-compose.yml

2. **Database Connection Issues**: Ensure PostgreSQL container is healthy before starting the app

3. **File Upload Issues**: Check that the Attachments directory has proper permissions

### Logs

```bash
# View application logs
docker-compose logs fingerprint-app

# View database logs
docker-compose logs postgres

# View all logs
docker-compose logs
```

## Environment Variables

You can override default settings using environment variables:

```bash
# Custom database connection
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=MyDb;Username=myuser;Password=mypass"

# Custom application port
export ASPNETCORE_URLS="http://+:8081"
```
