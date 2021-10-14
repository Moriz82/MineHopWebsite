FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MineHopWebsite.csproj", "./"]
RUN dotnet restore "MineHopWebsite.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "MineHopWebsite.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MineHopWebsite.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MineHopWebsite.dll"]
