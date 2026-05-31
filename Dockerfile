FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["IndieVault/IndieVault.csproj", "IndieVault/"]
RUN dotnet restore "IndieVault/IndieVault.csproj"
COPY . .
WORKDIR "/src/IndieVault"
RUN dotnet publish "IndieVault.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IndieVault.dll"]