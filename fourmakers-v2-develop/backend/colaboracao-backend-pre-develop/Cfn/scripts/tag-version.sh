#!/bin/bash

# Script para criar tag de versão após deploy em produção
# Uso: ./tag-version.sh <PROJECT_VAR>
# Exemplo: ./tag-version.sh APONTAMENTO_API

set -e

PROJECT_VAR=$1

if [ -z "$PROJECT_VAR" ]; then
    echo "Erro: Informe o identificador do projeto"
    echo "Uso: $0 <PROJECT_VAR>"
    echo "Exemplo: $0 APONTAMENTO_API"
    exit 1
fi

# Verificar se as variáveis necessárias estão definidas
if [ -z "$CI_PROJECT_TOKEN" ]; then
    echo "Erro: CI_PROJECT_TOKEN não está definida"
    exit 1
fi

if [ -z "$CI_API_V4_URL" ]; then
    echo "Erro: CI_API_V4_URL não está definida"
    exit 1
fi

if [ -z "$CI_PROJECT_ID" ]; then
    echo "Erro: CI_PROJECT_ID não está definida"
    exit 1
fi

if [ -z "$CI_COMMIT_SHA" ]; then
    echo "Erro: CI_COMMIT_SHA não está definida"
    exit 1
fi

echo "=== Criando tag de versão para o projeto ==="
echo "Projeto: $PROJECT_VAR"
echo "Commit: $CI_COMMIT_SHA"

# Ler o arquivo projects.json
PROJECTS_FILE="./Cfn/projects.json"

if [ ! -f "$PROJECTS_FILE" ]; then
    echo "Erro: Arquivo $PROJECTS_FILE não encontrado"
    exit 1
fi

# Extrair informações do projeto
SERVICE_NAME=$(jq -r --arg var "$PROJECT_VAR" '.[] | select(.serviceName | ascii_upcase | gsub("-"; "_") == $var) | .serviceName' "$PROJECTS_FILE")
PRD_VERSION=$(jq -r --arg var "$PROJECT_VAR" '.[] | select(.serviceName | ascii_upcase | gsub("-"; "_") == $var) | .version.prd' "$PROJECTS_FILE")

if [ -z "$SERVICE_NAME" ] || [ "$SERVICE_NAME" = "null" ]; then
    echo "Erro: Projeto $PROJECT_VAR não encontrado no arquivo projects.json"
    exit 1
fi

if [ -z "$PRD_VERSION" ] || [ "$PRD_VERSION" = "null" ]; then
    echo "Erro: Versão PRD não encontrada para o projeto $PROJECT_VAR"
    exit 1
fi

# Montar o nome da tag
TAG_NAME="${SERVICE_NAME}-${PRD_VERSION}"

echo "Service Name: $SERVICE_NAME"
echo "Versão PRD: $PRD_VERSION"
echo "Nome da Tag: $TAG_NAME"

# Verificar se a tag já existe
echo ""
echo "Verificando se a tag já existe..."
TAG_EXISTS=$(curl --silent --header "PRIVATE-TOKEN: ${CI_PROJECT_TOKEN}" \
    "${CI_API_V4_URL}/projects/${CI_PROJECT_ID}/repository/tags/${TAG_NAME}" \
    | jq -r '.name // empty')

if [ "$TAG_EXISTS" = "$TAG_NAME" ]; then
    echo "⚠️  Tag $TAG_NAME já existe. Pulando criação."
    exit 0
fi

# Criar a tag via API do GitLab
echo ""
echo "Criando tag via API do GitLab..."

RESPONSE=$(curl --silent --request POST \
    --header "PRIVATE-TOKEN: ${CI_PROJECT_TOKEN}" \
    --header "Content-Type: application/json" \
    --data "{
        \"tag_name\": \"${TAG_NAME}\",
        \"ref\": \"${CI_COMMIT_SHA}\",
        \"message\": \"Release ${TAG_NAME} - Deployed to Production\n\n[ci skip]\"
    }" \
    "${CI_API_V4_URL}/projects/${CI_PROJECT_ID}/repository/tags")

# Verificar se a tag foi criada com sucesso
TAG_CREATED=$(echo "$RESPONSE" | jq -r '.name // empty')

if [ "$TAG_CREATED" = "$TAG_NAME" ]; then
    echo "✅ Tag $TAG_NAME criada com sucesso!"
    echo "Commit: $(echo "$RESPONSE" | jq -r '.commit.short_id')"
    echo "Message: $(echo "$RESPONSE" | jq -r '.message' | head -1)"
else
    echo "❌ Erro ao criar tag:"
    echo "$RESPONSE" | jq '.'

    # Verificar se o erro é porque a tag já existe
    ERROR_MSG=$(echo "$RESPONSE" | jq -r '.message // empty')
    if echo "$ERROR_MSG" | grep -q "already exists"; then
        echo "⚠️  Tag já existe. Continuando..."
        exit 0
    fi

    exit 1
fi

echo ""
echo "=== Processo de tagueamento concluído ==="
