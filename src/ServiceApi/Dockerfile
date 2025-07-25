# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as separate layer for better caching
COPY ServiceApi.csproj ./
RUN dotnet restore

# Copy the remaining application files
COPY . .

# Publish the application to a directory
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# Copy the published output from the build container
COPY --from=build /app/publish .

# Set the startup command
ENTRYPOINT ["dotnet", "ServiceApi.dll"]
