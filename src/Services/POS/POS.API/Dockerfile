FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/POS/POS.API/POS.API.csproj", "src/Services/POS/POS.API/"]
COPY ["src/Services/POS/POS.Application/POS.Application.csproj", "src/Services/POS/POS.Application/"]
COPY ["src/Services/POS/POS.Domain/POS.Domain.csproj", "src/Services/POS/POS.Domain/"]
COPY ["src/Services/POS/POS.Infrastructure/POS.Infrastructure.csproj", "src/Services/POS/POS.Infrastructure/"]
COPY ["src/BuildingBlocks/Common.Events/Common.Events.csproj", "src/BuildingBlocks/Common.Events/"]
COPY ["src/BuildingBlocks/Common.Kafka/Common.Kafka.csproj", "src/BuildingBlocks/Common.Kafka/"]
COPY ["src/BuildingBlocks/Common.PostgreSQL/Common.PostgreSQL.csproj", "src/BuildingBlocks/Common.PostgreSQL/"]
COPY ["src/BuildingBlocks/Common.Observability/Common.Observability.csproj", "src/BuildingBlocks/Common.Observability/"]
COPY ["src/BuildingBlocks/Common.Resilience/Common.Resilience.csproj", "src/BuildingBlocks/Common.Resilience/"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

RUN dotnet restore "src/Services/POS/POS.API/POS.API.csproj"

# Copy source code
COPY . .
WORKDIR "/src/src/Services/POS/POS.API"
RUN dotnet build "POS.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "POS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "POS.API.dll"]
