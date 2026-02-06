FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/WMS/WMS.Consumer/WMS.Consumer.csproj", "src/Services/WMS/WMS.Consumer/"]
COPY ["src/Services/WMS/WMS.Domain/WMS.Domain.csproj", "src/Services/WMS/WMS.Domain/"]
COPY ["src/Services/WMS/WMS.Infrastructure/WMS.Infrastructure.csproj", "src/Services/WMS/WMS.Infrastructure/"]
COPY ["src/BuildingBlocks/Common.Events/Common.Events.csproj", "src/BuildingBlocks/Common.Events/"]
COPY ["src/BuildingBlocks/Common.Kafka/Common.Kafka.csproj", "src/BuildingBlocks/Common.Kafka/"]
COPY ["src/BuildingBlocks/Common.PostgreSQL/Common.PostgreSQL.csproj", "src/BuildingBlocks/Common.PostgreSQL/"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

RUN dotnet restore "src/Services/WMS/WMS.Consumer/WMS.Consumer.csproj"

# Copy source code
COPY . .
WORKDIR "/src/src/Services/WMS/WMS.Consumer"
RUN dotnet build "WMS.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WMS.Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Use built-in non-root user
USER app

ENTRYPOINT ["dotnet", "WMS.Consumer.dll"]
