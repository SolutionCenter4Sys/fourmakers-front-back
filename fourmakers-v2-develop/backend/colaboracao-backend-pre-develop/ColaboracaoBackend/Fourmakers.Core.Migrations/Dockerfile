FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY docker_image_dbup.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app

COPY --from=build /out .

CMD ["dotnet", "docker_image_dbup.dll"]