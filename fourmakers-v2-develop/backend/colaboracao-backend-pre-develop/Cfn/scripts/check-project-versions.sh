#!/bin/bash

# Script para verificar versões dos projetos e configurar variáveis de ambiente no GitLab via API
# Este script deve ser executado antes da pipeline para configurar as variáveis necessárias
# Uso: ./check-project-versions.sh [PROJECT_NAME]
# Se PROJECT_NAME não for fornecido, processa todos os projetos

set -e

echo "=== Verificando versões dos projetos ==="

# Configurações da API do GitLab
GITLAB_TOKEN="${CI_PROJECT_TOKEN}"
GITLAB_API_URL="${CI_API_V4_URL}"
PROJECT_ID="${CI_PROJECT_ID}"

# Verifica se as variáveis necessárias estão definidas
if [ -z "$GITLAB_TOKEN" ] || [ -z "$GITLAB_API_URL" ] || [ -z "$PROJECT_ID" ]; then
    echo "Erro: Variáveis de ambiente necessárias não estão definidas:"
    echo "  CI_API_V4_URL: $([ -z "$GITLAB_API_URL" ] && echo "NÃO DEFINIDA" || echo "OK")"
    echo "  CI_PROJECT_ID: $([ -z "$PROJECT_ID" ] && echo "NÃO DEFINIDA" || echo "OK")"
    exit 1
fi

echo "Configurações da API:"
echo "  URL: $GITLAB_API_URL"
echo "  Project ID: $PROJECT_ID"
echo ""

# Função para obter versão do projeto do arquivo projects.json
get_project_version_from_json() {
    local project_service_name="$1"
    local json_file="Cfn/projects.json"
    
    if [ ! -f "$json_file" ]; then
        echo "Erro: Arquivo $json_file não encontrado!" >&2
        exit 1
    fi
    
    # Determina o ambiente baseado na variável CI_ENV
    local environment="${CI_ENV:-dev}"
    echo "Ambiente detectado: $environment" >&2
    
    # Extrai a versão do projeto específico usando jq
    # Converte o serviceName para o formato do JSON (lowercase com hífens)
    local json_service_name=$(echo "$project_service_name" | tr '[:upper:]' '[:lower:]' | sed 's/_/-/g')
    local version=$(jq -r ".[] | select(.serviceName == \"$json_service_name\") | .version.$environment" "$json_file")
    
    if [ "$version" = "null" ] || [ -z "$version" ]; then
        echo "1.0.0"  # Versão padrão se não encontrada
    else
        echo "$version"
    fi
}

# Função para obter variável de ambiente do GitLab via API
get_gitlab_variable() {
    local variable_name="$1"
    
    # Determina o environment scope baseado na variável CI_ENV
    local environment_scope="*"
    if [ "$CI_ENV" = "dev" ]; then
        environment_scope="development"
    elif [ "$CI_ENV" = "prd" ]; then
        environment_scope="production" 
    fi
    
    # Busca a variável com o environment scope específico
    local response=$(curl --location --globoff "$GITLAB_API_URL/projects/$PROJECT_ID/variables/$variable_name?filter[environment_scope]=$environment_scope" --header "PRIVATE-TOKEN: $GITLAB_TOKEN"  2>/dev/null)
    if echo "$response" | grep -q "404 Variable Not Found"; then
        echo ""  # Variável não existe
    else
        # Extrai o valor da resposta JSON
        echo "$response" | grep -o '"value":"[^"]*"' | cut -d'"' -f4
    fi
}

# Função para criar/atualizar variável de ambiente no GitLab via API
set_gitlab_variable() {
    local variable_name="$1"
    local variable_value="$2"
    
    # Determina o environment scope baseado na variável CI_ENV
    local environment_scope="*"
    if [ "$CI_ENV" = "dev" ]; then
        environment_scope="development"
    elif [ "$CI_ENV" = "prd" ]; then
        environment_scope="production"
    fi
    
    echo "Configurando variável para environment: $environment_scope"
    
    # Verifica se a variável já existe
    local existing_value=$(get_gitlab_variable "$variable_name")
    
    if [ -z "$existing_value" ]; then
        # Cria nova variável
        echo "Criando variável $variable_name = $variable_value"
        curl --location --globoff --request POST \
            --header "PRIVATE-TOKEN: $GITLAB_TOKEN" \
            --header "Content-Type: application/json" \
            --data "{\"key\":\"$variable_name\",\"value\":\"$variable_value\",\"protected\":false,\"masked\":false,\"environment_scope\":\"$environment_scope\"}" \
            "$GITLAB_API_URL/projects/$PROJECT_ID/variables?filter[environment_scope]=$environment_scope" > /dev/null
    else
        # Atualiza variável existente
        echo "Atualizando variável $variable_name: $existing_value → $variable_value"
        curl --location --globoff --request PUT \
            --header "PRIVATE-TOKEN: $GITLAB_TOKEN" \
            --header "Content-Type: application/json" \
            --data "{\"value\":\"$variable_value\",\"protected\":false,\"masked\":false,\"environment_scope\":\"$environment_scope\"}" \
            "$GITLAB_API_URL/projects/$PROJECT_ID/variables/$variable_name?filter[environment_scope]=$environment_scope" > /dev/null
    fi
}

# Função para verificar se um projeto deve ser executado
should_execute_project() {
    local project_name="$1"
    local deploy_vars_file="deploy_vars.env"
    
    if [ ! -f "$deploy_vars_file" ]; then
        echo "true"  # Se o arquivo não existir, executa por padrão
        return 0
    fi
    
    # Busca pela variável DEPLOY_NEEDED do projeto
    local deploy_needed=$(grep "^DEPLOY_NEEDED_${project_name}=" "$deploy_vars_file" | cut -d'=' -f2)
    
    if [ "$deploy_needed" = "true" ]; then
        echo "true"
    else
        echo "false"
    fi
}

# Função para obter lista de projetos do arquivo projects.json
get_projects_from_json() {
    local json_file="Cfn/projects.json"
    
    if [ ! -f "$json_file" ]; then
        echo "Erro: Arquivo $json_file não encontrado!" >&2
        exit 1
    fi
    
    # Extrai os serviceNames do JSON usando jq
    # Converte para maiúsculo e substitui hífens por underscores para manter compatibilidade
    jq -r '.[].serviceName' "$json_file" | tr '[:lower:]' '[:upper:]' | sed 's/-/_/g'
}

# Função para processar um projeto específico
process_single_project() {
    local project="$1"
    
    # Converte nome do projeto para diretório (para exibição)
    local project_dir=$(echo "$project" | tr '[:upper:]' '[:lower:]' | sed 's/_/-/g')
    
    # Obtém versão do arquivo projects.json
    local env_version=$(get_project_version_from_json "$project")
    
    # Verifica se deve fazer deploy
    local gitlab_var="VERSAO_PROJETO_${project}"
    local gitlab_version=$(get_gitlab_variable "$gitlab_var")
    
    if [ -z "$gitlab_version" ]; then
        local deploy_needed="true"  # Deploy necessário - variável não existe
    elif [ "$env_version" != "$gitlab_version" ]; then
        local deploy_needed="true"  # Deploy necessário - versões diferentes
    else
        local deploy_needed="false" # Deploy não necessário - versões iguais
    fi
    
    # Exibe informações
    echo "Projeto: $project"
    echo "  Diretório: $project_dir"
    echo "  Versão JSON: $env_version"
    echo "  Variável GitLab: $gitlab_var"
    echo "  Versão GitLab: $([ -z "$gitlab_version" ] && echo "NÃO EXISTE" || echo "$gitlab_version")"
    
    if [ "$deploy_needed" = "true" ]; then
        echo "  Status: DEPLOY NECESSÁRIO"
        echo "  Ação: Configurando variável $gitlab_var=$env_version"
        
        # Configura a variável no GitLab
        set_gitlab_variable "$gitlab_var" "$env_version"
        
        # Define variável de ambiente para a pipeline (JSON)
        echo "    \"${project}\": {" >> deploy_config.json
        echo "      \"deploy_needed\": true," >> deploy_config.json
        echo "      \"version\": \"$env_version\"" >> deploy_config.json
        echo "    }," >> deploy_config.json
        
        echo "1"  # Retorna 1 para indicar que deploy é necessário
    else
        echo "  Status: Deploy não necessário"
        echo "  Ação: Nenhuma ação necessária"
        
        # Define variável de ambiente para a pipeline (JSON)
        echo "    \"${project}\": {" >> deploy_config.json
        echo "      \"deploy_needed\": false," >> deploy_config.json
        echo "      \"version\": \"$env_version\"" >> deploy_config.json
        echo "    }," >> deploy_config.json
        
        echo "0"  # Retorna 0 para indicar que deploy não é necessário
    fi
    
    echo ""
}

# Verifica se foi passado um projeto específico
if [ $# -eq 1 ]; then
    PROJECT_NAME="$1"
    echo "Processando projeto específico: $PROJECT_NAME"
    echo ""
    
    # Inicializa arquivo JSON
    echo "{" > deploy_config.json
    echo "  \"deploy_config\": {" >> deploy_config.json
    
    # Processa apenas o projeto especificado
    deploy_needed=$(process_single_project "$PROJECT_NAME")
    
    # Remove a última vírgula e fecha o JSON (compatível com macOS e Linux)
    if [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS
        sed -i '' '$ s/,$//' deploy_config.json
    else
        # Linux
        sed -i '$ s/,$//' deploy_config.json
    fi
    echo "  }" >> deploy_config.json
    echo "}" >> deploy_config.json
    
    echo "=== Configuração de deploy gerada ==="
    cat deploy_config.json
    echo ""
    
    exit 0
fi

# Se não foi passado projeto específico, processa todos os projetos (comportamento original)
echo "Processando todos os projetos..."
echo ""

# Lista de projetos (obtida dinamicamente do JSON)
echo "Lendo lista de projetos do arquivo projects.json..."
projects=($(get_projects_from_json))

if [ ${#projects[@]} -eq 0 ]; then
    echo "Erro: Nenhum projeto encontrado no arquivo projects.json!" >&2
    exit 1
fi

echo "Projetos encontrados: ${#projects[@]}"
echo "Lista: ${projects[*]}"
echo ""

echo "Verificando versões dos projetos..."
echo ""

# Inicializa arquivo JSON
echo "{" > deploy_config.json
echo "  \"deploy_config\": {" >> deploy_config.json

deploy_count=0
skip_count=0

for project in "${projects[@]}"; do
    # Converte nome do projeto para diretório (para exibição)
    project_dir=$(echo "$project" | tr '[:upper:]' '[:lower:]' | sed 's/_/-/g')
    
    # Obtém versão do arquivo projects.json
    env_version=$(get_project_version_from_json "$project")
    
    # Verifica se deve fazer deploy
    gitlab_var="VERSAO_PROJETO_${project}"
    gitlab_version=$(get_gitlab_variable "$gitlab_var")
    
    if [ -z "$gitlab_version" ]; then
        deploy_needed="true"  # Deploy necessário - variável não existe
    elif [ "$env_version" != "$gitlab_version" ]; then
        deploy_needed="true"  # Deploy necessário - versões diferentes
    else
        deploy_needed="false" # Deploy não necessário - versões iguais
    fi
    
    # Exibe informações
    echo "Projeto: $project"
    echo "  Diretório: $project_dir"
    echo "  Versão JSON: $env_version"
    echo "  Variável GitLab: $gitlab_var"
    echo "  Versão GitLab: $([ -z "$gitlab_version" ] && echo "NÃO EXISTE" || echo "$gitlab_version")"
    
    if [ "$deploy_needed" = "true" ]; then
        echo "  Status: DEPLOY NECESSÁRIO"
        echo "  Ação: Configurando variável $gitlab_var=$env_version"
        
        # Configura a variável no GitLab
        set_gitlab_variable "$gitlab_var" "$env_version"
        
        # Define variável de ambiente para a pipeline (JSON)
        echo "    \"${project}\": {" >> deploy_config.json
        echo "      \"deploy_needed\": true," >> deploy_config.json
        echo "      \"version\": \"$env_version\"" >> deploy_config.json
        echo "    }," >> deploy_config.json
        
        deploy_count=$((deploy_count + 1))
      else
        echo "  Status: Deploy não necessário"
        echo "  Ação: Nenhuma ação necessária"
        
        # Define variável de ambiente para a pipeline (JSON)
        echo "    \"${project}\": {" >> deploy_config.json
        echo "      \"deploy_needed\": false," >> deploy_config.json
        echo "      \"version\": \"$env_version\"" >> deploy_config.json
        echo "    }," >> deploy_config.json
        
        skip_count=$((skip_count + 1))
      fi
    
    echo ""
done

echo "=== Resumo ==="
echo "Projetos que precisam de deploy: $deploy_count"
echo "Projetos que não precisam de deploy: $skip_count"
echo ""
echo "Variáveis de ambiente configuradas no GitLab via API com environment scope"
echo "Configuração de deploy exportada para deploy_config.json"
echo ""

# Remove a última vírgula e fecha o JSON
sed -i '$ s/,$//' deploy_config.json
echo "  }" >> deploy_config.json
echo "}" >> deploy_config.json

echo "=== Configuração de deploy gerada ==="
cat deploy_config.json
echo ""
echo "Para forçar um novo deploy, atualize a versão no arquivo projects.json ou delete a variável via GitLab"

# Exporta função para uso em outros jobs
echo ""
echo "=== Função de verificação disponível ==="
echo "Para verificar se um projeto deve ser executado, use:"
echo "should_execute_project \"NOME_DO_PROJETO\""
echo ""
echo "Exemplo: should_execute_project \"FINANCEIRO_API\""
echo "Retorna: true (executar) ou false (não executar)"
echo ""
echo "=== Uso do script ==="
echo "Para processar um projeto específico: ./check-project-versions.sh NOME_DO_PROJETO"
echo "Para processar todos os projetos: ./check-project-versions.sh"
