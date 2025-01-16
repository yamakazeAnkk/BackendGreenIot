# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copy published output to runtime image
COPY --from=build-env /app/out .

# Expose port 8080 for Cloud Run
EXPOSE 8080

# Start application
CMD ["sh", "-c", "dotnet GreenIotApi.dll --urls http://0.0.0.0:${PORT:-8080}"]

