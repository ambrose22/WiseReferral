FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore
COPY ReferralTracker.csproj ./
RUN dotnet restore ReferralTracker.csproj

# Copy everything else and publish
COPY . ./
RUN dotnet publish ReferralTracker.csproj -c Release -o /app --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Render uses port 10000 by default
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_EnableDiagnostics=0

EXPOSE 10000
ENTRYPOINT ["dotnet", "ReferralTracker.dll"]
