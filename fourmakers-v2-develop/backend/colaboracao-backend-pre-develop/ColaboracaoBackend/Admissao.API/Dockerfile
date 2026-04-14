# syntax=docker/dockerfile:1.7-labs

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY --parents */*.csproj /src/
RUN dotnet restore "Admissao.API/Admissao.API.csproj"
COPY . .
WORKDIR "/src/Admissao.API"

FROM build AS publish
RUN dotnet publish "Admissao.API.csproj" -c Release -o /app/publish
FROM base AS final
EXPOSE 80
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Admissao.API.dll"]
