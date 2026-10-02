FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project descriptors (when known)


# Restore the main project when metadata is available


# Copy the remaining files
COPY . .

FROM build AS publish

# Fallback for remote repositories: auto-discover the first .csproj in the repository
RUN project_file="$(find . -name '*.csproj' | head -n 1)" && \
    if [ -z "$project_file" ]; then echo "No .csproj file found in repository." >&2; exit 1; fi && \
    app_dll="$(basename "$project_file" .csproj).dll" && \
    dotnet restore "$project_file" && \
    dotnet publish "$project_file" -c Release -o /app/publish /p:UseAppHost=false && \
    echo "$app_dll" > /app/publish/.entrypoint


# Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .

ENTRYPOINT ["sh", "-c", "dotnet \"$(cat .entrypoint)\""]

