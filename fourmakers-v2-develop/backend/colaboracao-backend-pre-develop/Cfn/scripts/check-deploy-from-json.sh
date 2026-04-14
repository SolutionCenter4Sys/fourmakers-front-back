#!/bin/bash

# Script auxiliar para verificar se um projeto deve ser executado
# Este script lê do arquivo JSON em vez de dotenv para evitar o limite de 20 variáveis

set -e

# Função para verificar se um projeto deve ser executado
should_execute_project() {
    local project_name="$1"
    local json_file="deploy_config.json"
    
    if [ ! -f "$json_file" ]; then
        echo "true"  # Se o arquivo não existir, executa por padrão
        return 0
    fi
    
    # Extrai o valor deploy_needed do JSON usando jq
    local deploy_needed=$(jq -r ".deploy_config.\"$project_name\".deploy_needed" "$json_file" 2>/dev/null)
    
    if [ "$deploy_needed" = "true" ]; then
        echo "true"
    else
        echo "false"
    fi
}

# Função para obter a versão do projeto
get_project_version() {
    local project_name="$1"
    local json_file="deploy_config.json"
    
    if [ ! -f "$json_file" ]; then
        echo "1.0.0"  # Versão padrão se arquivo não existir
        return 0
    fi
    
    # Extrai o valor version do JSON usando jq
    local project_version=$(jq -r ".deploy_config.\"$project_name\".version" "$json_file" 2>/dev/null)
    
    if [ "$project_version" = "null" ] || [ -z "$project_version" ]; then
        echo "1.0.0"  # Versão padrão se não encontrada
    else
        echo "$project_version"
    fi
}

# Função para exibir informações de debug
debug_info() {
    local project_name="$1"
    local json_file="deploy_config.json"
    
    echo "=== Informações de Debug ==="
    echo "Projeto: $project_name"
    echo "Arquivo deploy_config.json existe: $([ -f "$json_file" ] && echo "SIM" || echo "NÃO")"
    
    if [ -f "$json_file" ]; then
        echo "Conteúdo do arquivo:"
        cat "$json_file"
        echo ""
        
        echo "Variáveis específicas do projeto:"
        local deploy_needed=$(jq -r ".deploy_config.\"$project_name\".deploy_needed" "$json_file" 2>/dev/null)
        local version=$(jq -r ".deploy_config.\"$project_name\".version" "$json_file" 2>/dev/null)
        echo "  deploy_needed: $([ "$deploy_needed" = "null" ] && echo "NÃO ENCONTRADA" || echo "$deploy_needed")"
        echo "  version: $([ "$version" = "null" ] && echo "NÃO ENCONTRADA" || echo "$version")"
    fi
}

PROJECT_NAME="$1"
COMMAND="$2"

# Verifica se é debug
if [ "$COMMAND" = "--debug" ]; then
    debug_info "$PROJECT_NAME"
    exit 0
fi

# Se não há comando, apenas verifica
if [ -z "$COMMAND" ]; then
    should_execute=$(should_execute_project "$PROJECT_NAME")
    version=$(get_project_version "$PROJECT_NAME")
    
    echo "Projeto: $PROJECT_NAME"
    echo "Versão: $version"
    echo "Deploy necessário: $should_execute"
    
    if [ "$should_execute" = "true" ]; then
        echo "Status: EXECUTAR"
        exit 0
    else
        echo "Status: NÃO EXECUTAR"
        exit 1
    fi
fi

