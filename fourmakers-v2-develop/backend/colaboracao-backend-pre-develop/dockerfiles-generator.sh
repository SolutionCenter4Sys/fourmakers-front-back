#!/bin/bash

# Obtém o diretório onde o script está sendo executado
SCRIPT_DIR="$(dirname "$(realpath "$0")")"

# Cria o caminho completo ao adicionar "/ColaboracaoBackend"
ROOT_DIR="$SCRIPT_DIR/ColaboracaoBackend"

# Procura por pastas .API, incluindo BI.API, e refatora os Dockerfiles
find "$ROOT_DIR" -type d -name "*.API" | while read -r dir; do
    DOCKERFILE_PATH="$dir/Dockerfile"

    if [[ -f "$DOCKERFILE_PATH" ]]; then

        # Extrai o nome da pasta sem o sufixo .API
        API_NAME=$(basename "$dir")

        # Inicializa as variáveis de configuração
        ASPNET_VERSION=""
        SDK_VERSION=""
        PUBLISH_COMMAND=""
        GOOGLE_APPLICATION_CREDENTIALS=""

        # Define as versões com base no nome da API
        if [[ "$API_NAME" == "RotinasBackoffice.API" ]]; then
            ASPNET_VERSION="FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base"
            SDK_VERSION="FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build"
        else
            ASPNET_VERSION="FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base"
            SDK_VERSION="FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build"
        fi
        
        if [[ "$API_NAME" == "MockServer.API" || "$API_NAME" == "RotinasBackoffice.API" ]]; then
            PUBLISH_COMMAND="RUN dotnet publish \"API_NAME.csproj\" -c Release -o /app/publish /p:UseAppHost=false"
        else
            PUBLISH_COMMAND="RUN dotnet publish \"API_NAME.csproj\" -c Release -o /app/publish"
        fi

        # Para Firebase, adiciona a variável de ambiente antes do ENTRYPOINT
        if [[ "$API_NAME" == "Firebase.API" ]]; then
            GOOGLE_APPLICATION_CREDENTIALS="ENV GOOGLE_APPLICATION_CREDENTIALS=/app/app-foursys-e7a0216670a6.json"
        fi

        # Inicia o modelo base de Dockerfile (alinhado a Marketing.API/Dockerfile — base no ECR; estágio de build usa \$SDK_VERSION)
        NEW_DOCKERFILE="# syntax=docker/dockerfile:1.7-labs
# Base com libgdiplus: imagem pré-construída no registry (Docker/dotnet-aspnet-libgdiplus).
ARG REGISTRY_IMAGE
FROM \${REGISTRY_IMAGE}/dotnet-aspnet-libgdiplus:8.0 AS base
WORKDIR /app

$SDK_VERSION
WORKDIR /src

COPY --parents */*.csproj /src/

RUN dotnet restore \"API_NAME/API_NAME.csproj\"
COPY . .
WORKDIR \"/src/API_NAME\"

FROM build AS publish
$PUBLISH_COMMAND
FROM base AS final
EXPOSE 80
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
COPY --from=publish /app/publish .
$GOOGLE_APPLICATION_CREDENTIALS
ENTRYPOINT [\"dotnet\", \"API_NAME.dll\"]"

        # Substitui o nome da API nos lugares necessários
        NEW_DOCKERFILE="${NEW_DOCKERFILE//API_NAME/$API_NAME}"

        # Sobrescreve o Dockerfile
        echo "$NEW_DOCKERFILE" > "$DOCKERFILE_PATH"
        echo "Dockerfile refatorado: $DOCKERFILE_PATH"
    fi
done

echo "Todos os Dockerfiles foram refatorados com sucesso!"
