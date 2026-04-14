# syntax=docker/dockerfile:1.7-labs
# Base com libgdiplus: imagem pré-construída no registry (Docker/dotnet-aspnet-libgdiplus).
ARG REGISTRY_IMAGE
FROM ${REGISTRY_IMAGE}/dotnet-aspnet-libgdiplus:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY --parents */*.csproj /src/

RUN dotnet restore "SRS.API/SRS.API.csproj"
COPY . .
WORKDIR "/src/SRS.API"

FROM build AS publish
RUN dotnet publish "SRS.API.csproj" -c Release -o /app/publish
FROM base AS final
EXPOSE 80
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SRS.API.dll"]
