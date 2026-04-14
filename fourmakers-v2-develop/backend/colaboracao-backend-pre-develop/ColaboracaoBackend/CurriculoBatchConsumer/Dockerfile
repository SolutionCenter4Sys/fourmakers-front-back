# syntax=docker/dockerfile:1.7-labs

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Build libgdiplus with pango
RUN apt-get update && apt-get install  -y --allow-unauthenticated \
libgif-dev autoconf libtool automake build-essential gettext libglib2.0-dev libcairo2-dev libtiff-dev libexif-dev
RUN apt-get update && apt-get install -y libpango1.0-dev 
RUN apt-get install -y --allow-unauthenticated git
RUN git clone https://github.com/mono/libgdiplus.git /libgdiplus
WORKDIR /libgdiplus
RUN ./autogen.sh --with-pango --prefix=/usr
RUN make  
RUN make install

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY --parents */*.csproj /src/

RUN dotnet restore "CurriculoBatchConsumer/CurriculoBatchConsumer.csproj"
COPY . .
WORKDIR "/src/CurriculoBatchConsumer"

FROM build AS publish
RUN dotnet publish "CurriculoBatchConsumer.csproj" -c Release -o /app/publish
FROM base AS final
EXPOSE 80


ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CurriculoBatchConsumer.dll"]
