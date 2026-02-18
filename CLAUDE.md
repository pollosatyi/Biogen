# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

- **Build solution**: `dotnet build Biogen.sln`
- **Run web API**: `dotnet run --project Biogen/Biogen.csproj`
- **Restore packages**: `dotnet restore Biogen.sln`
- **Clean**: `dotnet clean Biogen.sln`

The API runs at `http://localhost:5148` (HTTP) or `https://localhost:7261` (HTTPS). Swagger UI is enabled in development mode at `/swagger`.

No test projects exist yet. When added, use `dotnet test Biogen.sln` to run all tests or `dotnet test <project> --filter "FullyQualifiedName~TestName"` for a single test.

## Architecture

This is an ASP.NET Core 8.0 Web API using a layered (N-tier) architecture with four solution folders:

- **PL (Presentation Layer)** — `Biogen/`: ASP.NET Core Web API project with controllers. Entry point is `Program.cs`.
- **BLL (Business Logic Layer)** — Split into two projects:
  - `Biogen.BLL.LogicExtention/`: Interfaces/contracts (e.g., `IImageForDetectionLogic`)
  - `Biogen.BLL.Logic/`: Implementations (e.g., `ImageForDetectionLogic`)
- **Common** — `Biogen.Common.Entities/`: Shared data models used across layers (`ImageModelForDetection`, `ImageDetectionOutcome`)
- **DAL (Data Access Layer)** — Exists as a solution folder but has no projects yet.

Dependency flow: PL → BLL → Common. The DAL layer will sit below BLL when implemented.

## Key Configuration

- .NET SDK 8.0 (`global.json` pins to 8.0.0 with `latestMinor` roll-forward)
- Nullable reference types enabled across all projects
- Implicit usings enabled
- Swagger via Swashbuckle.AspNetCore 6.6.2
