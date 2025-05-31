# Use the official ASP.NET 8.0 runtime image as the base for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /App
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /Src
COPY . .
RUN dotnet publish -c Release -o /App/Publish

FROM base AS final
WORKDIR /App
COPY --from=build /App/Publish .
COPY Resources /App/Resources
ENTRYPOINT ["dotnet", "Roza.AuthService.dll"]