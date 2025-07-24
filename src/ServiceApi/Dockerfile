# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
# This leverages Docker's layer caching for faster builds if only code changes
COPY ServiceApi.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Publish the application to a specific directory
# The -o /app/publish specifies the output directory within this build stage
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# Copy the published application from the build stage
# IMPORTANT: We assume dotnet publish creates a subdirectory named 'ServiceApi'
# inside /app/publish based on the project name. Adjust 'ServiceApi' if your
# project's output assembly name is different.
COPY --from=build /app/publish/ServiceApi .

# Set the entrypoint for the container
# This tells Docker to run your application when the container starts
ENTRYPOINT ["dotnet", "ServiceApi.dll"]
