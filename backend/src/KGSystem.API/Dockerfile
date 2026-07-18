FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["src/KGSystem.Domain/KGSystem.Domain.csproj", "src/KGSystem.Domain/"]
COPY ["src/KGSystem.Application/KGSystem.Application.csproj", "src/KGSystem.Application/"]
COPY ["src/KGSystem.Infrastructure/KGSystem.Infrastructure.csproj", "src/KGSystem.Infrastructure/"]
COPY ["src/KGSystem.API/KGSystem.API.csproj", "src/KGSystem.API/"]
RUN dotnet restore "src/KGSystem.API/KGSystem.API.csproj"

COPY . .
WORKDIR /src/src/KGSystem.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "KGSystem.API.dll"]
