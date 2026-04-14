# syntax=docker/dockerfile:1.7-labs
# Base com libgdiplus: imagem pré-construída no registry (Docker/dotnet-aspnet-libgdiplus).
ARG REGISTRY_IMAGE
FROM ${REGISTRY_IMAGE}/dotnet-aspnet-libgdiplus:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY --parents */*.csproj /src/

RUN dotnet restore "Firebase.API/Firebase.API.csproj"
COPY . .
WORKDIR "/src/Firebase.API"

FROM build AS publish
RUN dotnet publish "Firebase.API.csproj" -c Release -o /app/publish
FROM base AS final
EXPOSE 80
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
COPY --from=publish /app/publish .
# Montar o JSON da service account em runtime (secret/ volume) — não commitar o arquivo real
ENV GOOGLE_APPLICATION_CREDENTIALS=/app/app-foursys-e7a0216670a6.json
ENTRYPOINT ["dotnet", "Firebase.API.dll"]
