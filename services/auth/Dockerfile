# Use the official ASP.NET 8.0 runtime image as the base for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory inside the container
WORKDIR /App

# Expose port 5000 for the application
EXPOSE 5000

# Use the official .NET 8.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory for the build process
WORKDIR /Src

# Copy all files from the current directory to the container
COPY . .

# Publish the application in Release mode to the /app/publish directory
RUN dotnet publish -c Release -o /App/Publish

# Create the final image based on the runtime image
FROM base AS final

# Set the working directory for the final image
WORKDIR /App

# Copy the published output from the build stage to the final image
COPY --from=build /App/Publish .

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "Roza.AuthService.dll"]