# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the entire project directory
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the official ASP.NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out ./

# Expose the port the app runs on
EXPOSE 5000

# Disable HTTPS redirection in Docker
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_HTTP_PORT=5000

# Set the entry point for the application
ENTRYPOINT ["dotnet", "P06Shop.Api.dll"]