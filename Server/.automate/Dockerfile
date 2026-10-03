FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project descriptors (when known)

COPY ["Api/Api.csproj", "Api/"]

COPY ["Application/Application.csproj", "Application/"]

COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

COPY ["Domain/Domain.csproj", "Domain/"]


# Restore the main project when metadata is available

RUN dotnet restore "Api/Api.csproj"


# Copy the remaining files
COPY . .

FROM build AS publish

# Build and publish using the discovered main project metadata
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


# Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 65494
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]

