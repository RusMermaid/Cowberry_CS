﻿FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Cowberry_CS.csproj", "./"]
RUN dotnet restore "Cowberry_CS.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Cowberry_CS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cowberry_CS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cowberry_CS.dll"]
