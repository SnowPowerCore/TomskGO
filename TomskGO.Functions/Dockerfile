#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-stretch AS build
WORKDIR /src
COPY ["TomskGO.Functions/TomskGO.Functions.csproj", "TomskGO.Functions/"]
COPY ["TomskGO.Models/TomskGO.Models.csproj", "TomskGO.Models/"]
RUN dotnet restore "TomskGO.Functions/TomskGO.Functions.csproj"
COPY . .
WORKDIR "/src/TomskGO.Functions"
RUN dotnet build "TomskGO.Functions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TomskGO.Functions.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/app