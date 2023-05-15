#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/KeyCloak.Services.Api/KeyCloak.Services.Api.csproj", "src/KeyCloak.Services.Api/"]
RUN dotnet restore "src/KeyCloak.Services.Api/KeyCloak.Services.Api.csproj"
COPY . .
WORKDIR "/src/src/KeyCloak.Services.Api"
RUN dotnet build "KeyCloak.Services.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KeyCloak.Services.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KeyCloak.Services.Api.dll"]