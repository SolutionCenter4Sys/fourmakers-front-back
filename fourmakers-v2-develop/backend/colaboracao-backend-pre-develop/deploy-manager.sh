#!/usr/bin/env bash

# Deploy Manager 

# v1.0 - Script interativo para reorganizar .gitlab-ci.yml e incrementar versões CFN
# Autor: Felipe Martins
# Data: 2025-09-18

# v2.0 - Script para organizar versão da projects.json da main e da develop antes de incrementar, garantindo assim, um merge mais "limpo"
# Autor: Felipe Martins
# Data: 2025-11-19
# v2.1 - `.gitlab-ci.yml` regenerado a partir de `.gitlab-ci.template.yml` (sem sed no miolo); só comenta/descomenta jobs *_ci.

# Verificar versão do bash ANTES de qualquer coisa (precisa ser 4.0+ para arrays associativos)
# Esta verificação precisa funcionar mesmo no bash 3.2, então usamos uma abordagem simples
BASH_VER_STR=$(bash --version | head -n1)
BASH_MAJOR=$(echo "$BASH_VER_STR" | sed -n 's/.*version \([0-9]\)\.[0-9].*/\1/p')
if [ -z "$BASH_MAJOR" ] || [ "$BASH_MAJOR" -lt 4 ]; then
    echo "✗ Este script requer Bash 4.0 ou superior."
    echo "ℹ Versão atual: $BASH_VER_STR"
    echo ""
    echo "ℹ Para instalar Bash mais recente no macOS:"
    echo "  1. brew install bash"
    echo "  2. Execute com: /opt/homebrew/bin/bash deploy-manager.sh"
    echo "     (ou /usr/local/bin/bash se Homebrew estiver em /usr/local)"
    exit 1
fi

set -e

# Cores para output 
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Funções utilitárias
print_header() {
    echo -e "${BLUE}================================================${NC}"
    echo -e "${BLUE}           DEPLOY MANAGER v2.0                 ${NC}"
    echo -e "${BLUE}================================================${NC}"
    echo ""
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ $1${NC}"
}

# Verificar se arquivos necessários existem
check_files() {
    local files_missing=false

    if [[ ! -f ".gitlab-ci.template.yml" ]]; then
        print_error "Arquivo .gitlab-ci.template.yml não encontrado (fonte canônica da esteira raiz)."
        files_missing=true
    fi

    if [[ ! -f "Cfn/projects.json" ]]; then
        print_error "Arquivo Cfn/projects.json não encontrado!"
        files_missing=true
    fi

    if [[ "$files_missing" == "true" ]]; then
        exit 1
    fi
}

# Lista de serviços disponíveis (ordem alfabética)
declare -A SERVICES=(
    ["1"]="apontamento"
    ["2"]="bancotalentosrs-consumer"
    ["3"]="bi"
    ["4"]="botfourmakers"
    ["5"]="colaborador"
    ["6"]="competencia"
    ["7"]="crm"
    ["8"]="curriculo-batch-consumer"
    ["9"]="financeiro"
    ["10"]="firebase"
    ["11"]="folha-consumer"
    ["12"]="foursys"
	["13"]="gestaopessoa"
    ["14"]="mapaalocacao"
    ["15"]="maparelacionamento"
    ["16"]="marketing"
	["17"]="organograma"
    ["18"]="perfilcorporativo"
    ["19"]="projeto"
    ["20"]="rotinasbackoffice"
    ["21"]="social"
    ["22"]="srs"
    ["23"]="uploadfiles"
    ["24"]="usuario"
    ["25"]="labs"
    ["26"]="admissao"
)

# Mapeamento de nomes de serviços para projectPath
declare -A SERVICE_TO_PROJECT=(
    ["colaborador"]="Colaborador.API"
    ["mapaalocacao"]="MapaDeAlocacao.API"
    ["maparelacionamento"]="MapaDeRelacionamento.API"
    ["marketing"]="Marketing.API"
    ["perfilcorporativo"]="PerfilCorporativo.API"
    ["srs"]="SRS.API"
    ["projeto"]="Projeto.API"
    ["uploadfiles"]="UploadFiles.API"
    ["social"]="Social.API"
    ["curriculo-batch-consumer"]="CurriculoBatchConsumer"
    ["foursys"]="Foursys.API"
    ["bancotalentosrs-consumer"]="BancoTalentoSRSConsumer"
    ["folha-consumer"]="FolhaColaboradorConsumer"
    ["apontamento"]="Apontamento.API"
    ["usuario"]="Usuario.API"
    ["competencia"]="Competencia.API"
    ["botfourmakers"]="BotFourmakers.API"
    ["rotinasbackoffice"]="RotinasBackoffice.API"
    ["crm"]="CRM.API"
    ["bi"]="BI.API"
    ["financeiro"]="Financeiro.API"
    ["firebase"]="Firebase.API"
	["organograma"]="Organograma.API"
	["gestaopessoa"]="GestaoPessoa.API"
	["labs"]="Labs.API"
	["admissao"]="Admissao.API"
)

# Função para mostrar menu de serviços
show_services_menu() {
    echo -e "${YELLOW}Selecione os serviços para deploy (digite os números separados por espaço):${NC}"
    echo ""

    for key in $(printf '%s\n' "${!SERVICES[@]}" | sort -n); do
        local service="${SERVICES[$key]}"
        local project_path="${SERVICE_TO_PROJECT[$service]}"
        printf "  %2d) %s\n" "$key" "${project_path:-$service}"
    done

    echo ""
    echo -e "${BLUE}Exemplos:${NC}"
    echo "  - Para um serviço: 1"
    echo "  - Para múltiplos: 1 12 18"
    echo "  - Para todos: all"
    echo "  - Para sair: quit"
    echo ""
}

# Função para validar seleção
validate_selection() {
    local input="$1"
    local valid_numbers=()

    if [[ "$input" == "all" ]]; then
        for key in "${!SERVICES[@]}"; do
            valid_numbers+=("$key")
        done
        echo "${valid_numbers[@]}"
        return 0
    fi

    if [[ "$input" == "quit" ]]; then
        echo "quit"
        return 0
    fi

    for num in $input; do
        if [[ "$num" =~ ^[0-9]+$ ]] && [[ -n "${SERVICES[$num]}" ]]; then
            valid_numbers+=("$num")
        else
            print_error "Número inválido: $num"
            return 1
        fi
    done

    if [[ ${#valid_numbers[@]} -eq 0 ]]; then
        print_error "Nenhum serviço válido selecionado"
        return 1
    fi

    echo "${valid_numbers[@]}"
    return 0
}

# Compila o utilitário .NET se necessário; escreve o caminho do DLL em stdout (logs em stderr).
gitlab_ci_tool_dll_path() {
    local script_dir
    script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    local proj_dir="$script_dir/automation/GitLabCiApplyJobSelection"
    local csproj="$proj_dir/GitLabCiApplyJobSelection.csproj"
    local dll="$proj_dir/bin/Release/net8.0/GitLabCiApplyJobSelection.dll"

    if ! command -v dotnet >/dev/null 2>&1; then
        print_error "dotnet SDK é necessário para ajustar *_ci (instale .NET 8+)." >&2
        exit 1
    fi

    local needs_build=false
    if [[ ! -f "$dll" ]]; then
        needs_build=true
    elif [[ "$csproj" -nt "$dll" ]]; then
        needs_build=true
    else
        local f
        while IFS= read -r -d '' f; do
            if [[ "$f" -nt "$dll" ]]; then
                needs_build=true
                break
            fi
        done < <(find "$proj_dir" -name '*.cs' -print0 2>/dev/null)
    fi

    if [[ "$needs_build" == true ]]; then
        print_info "Compilando GitLabCiApplyJobSelection (Release)..." >&2
        # Build vai para stderr: stdout desta função é só o caminho do DLL (evita poluir dll="$(...)").
        dotnet build "$csproj" --configuration Release --nologo -v minimal >&2 || exit 1
    fi

    if [[ ! -f "$dll" ]]; then
        print_error "DLL não encontrado após build: $dll" >&2
        exit 1
    fi

    printf '%s\n' "$dll"
}

# Copia `.gitlab-ci.template.yml` -> `.gitlab-ci.yml`.
materialize_gitlab_ci_from_template() {
    local repo_root
    repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    local tpl="$repo_root/.gitlab-ci.template.yml"
    local out="$repo_root/.gitlab-ci.yml"

    if [[ ! -f "$tpl" ]]; then
        print_error "Arquivo .gitlab-ci.template.yml não encontrado."
        exit 1
    fi

    cp "$tpl" "$out"
    print_success ".gitlab-ci.yml copiado a partir de .gitlab-ci.template.yml"
}

# Comenta blocos *_ci dos serviços não selecionados (ferramenta .NET no repositório).
apply_ci_job_selection() {
    local selected_services=("$@")
    local all_csv=""
    local key

    for key in $(printf '%s\n' "${!SERVICES[@]}" | sort -n); do
        [[ -n "$all_csv" ]] && all_csv+=","
        all_csv+="${SERVICES[$key]}"
    done

    local sel_csv=""
    local s
    for s in "${selected_services[@]}"; do
        [[ -n "$sel_csv" ]] && sel_csv+=","
        sel_csv+="$s"
    done

    local dll
    dll="$(gitlab_ci_tool_dll_path)"
    dotnet "$dll" .gitlab-ci.yml "$all_csv" "$sel_csv"

    print_success "Jobs *_ci: ativos só os selecionados (demais comentados)"
}

# Função para mostrar menu de incremento de versão
show_version_menu() {
    echo -e "${YELLOW}Deseja incrementar versões CFN dos serviços selecionados?${NC}"
    echo ""
    echo "1) Sim - DEV apenas"
    echo "2) Sim - PRD apenas"
    echo "3) Não"
    echo ""
}

# Função para incrementar versão (bash nativo)
increment_patch_version() {
    local version="$1"
    local major minor patch

    IFS='.' read -r major minor patch <<< "$version"
    patch=$((patch + 1))
    echo "$major.$minor.$patch"
}

# Função para comparar versões e retornar a maior
compare_versions() {
    local version1="$1"
    local version2="$2"

    # Se uma das versões estiver vazia, retornar a outra
    if [[ -z "$version1" ]]; then
        echo "$version2"
        return
    fi
    if [[ -z "$version2" ]]; then
        echo "$version1"
        return
    fi

    local IFS='.'
    read -ra v1_parts <<< "$version1"
    read -ra v2_parts <<< "$version2"

    # Comparar cada parte (major, minor, patch)
    for i in 0 1 2; do
        local part1=${v1_parts[$i]:-0}
        local part2=${v2_parts[$i]:-0}

        if ((part1 > part2)); then
            echo "$version1"
            return
        elif ((part1 < part2)); then
            echo "$version2"
            return
        fi
    done

    # Versões são iguais
    echo "$version1"
}

# Variáveis globais para cache dos JSONs remotos
MAIN_JSON_CACHE=""
DEVELOP_JSON_CACHE=""

# Função para carregar cache dos JSONs remotos
load_remote_json_cache() {
    print_info "Buscando atualizações das branches remotas..."

    # Fazer fetch das branches remotas
    if ! git fetch origin main develop 2>/dev/null; then
        print_warning "Não foi possível fazer fetch das branches remotas. Continuando com versões locais..."
        return 1
    fi

    # Carregar JSONs em cache
    MAIN_JSON_CACHE=$(git show "origin/main:Cfn/projects.json" 2>/dev/null)
    DEVELOP_JSON_CACHE=$(git show "origin/develop:Cfn/projects.json" 2>/dev/null)

    print_success "Branches remotas atualizadas"
    echo ""
    return 0
}

# Função para buscar versão de um projeto usando cache
get_version_from_cache() {
    local branch="$1"
    local project_path="$2"
    local env="$3"

    local json_content=""
    if [[ "$branch" == "origin/main" ]]; then
        json_content="$MAIN_JSON_CACHE"
    else
        json_content="$DEVELOP_JSON_CACHE"
    fi

    if [[ -z "$json_content" ]]; then
        echo ""
        return
    fi

    # Extrair a versão usando grep e sed (apenas primeira ocorrência no bloco)
    local version
    version=$(echo "$json_content" | grep -A 10 "\"projectPath\": \"$project_path\"" | grep "\"$env\"" | head -1 | sed 's/.*"'"$env"'": "\([^"]*\)".*/\1/')
    version="${version%%$'\n'*}"

    echo "$version"
}

# Função para sincronizar versões entre origin/main e origin/develop para TODOS os serviços
sync_versions_from_remote() {
    local env_choice="$1"

    # Carregar cache dos JSONs remotos
    if ! load_remote_json_cache; then
        return
    fi

    print_info "Sincronizando versões de TODOS os serviços com origin/main e origin/develop..."
    echo ""

    # Para cada serviço no mapeamento, verificar versões em origin/main e origin/develop
    for service in "${!SERVICE_TO_PROJECT[@]}"; do
        local project_path="${SERVICE_TO_PROJECT[$service]}"

        if [[ -n "$project_path" ]]; then
            # Sincronizar DEV e PRD para todos os serviços
            for env in "dev" "prd"; do
                # Buscar versões usando cache
                local version_main
                local version_develop
                version_main=$(get_version_from_cache "origin/main" "$project_path" "$env")
                version_develop=$(get_version_from_cache "origin/develop" "$project_path" "$env")

                # Encontrar a maior versão entre main e develop
                local max_version
                max_version=$(compare_versions "$version_main" "$version_develop")

                if [[ -n "$max_version" ]]; then
                    # Buscar versão atual no arquivo local (apenas primeira no bloco)
                    local current_version
                    current_version=$(grep -A 10 "\"projectPath\": \"$project_path\"" Cfn/projects.json | grep "\"$env\"" | head -1 | sed 's/.*"'"$env"'": "\([^"]*\)".*/\1/')
                    current_version="${current_version%%$'\n'*}"

                    # Comparar com a versão local atual
                    local final_version
                    final_version=$(compare_versions "$max_version" "$current_version")
                    # Garantir uma única linha (evita quebra do sed se houver resquício de newline)
                    final_version="${final_version%%$'\n'*}"

                    # Se a versão remota for maior que a local, atualizar
                    if [[ "$final_version" != "$current_version" ]]; then
                        print_warning "$project_path ($env): $current_version -> $final_version"

                        # Atualizar a versão no arquivo
                        sed -i.tmp "/\"projectPath\": \"$project_path\"/,/}/s/\"$env\": \"[^\"]*\"/\"$env\": \"$final_version\"/g" Cfn/projects.json
                        rm -f Cfn/projects.json.tmp 2>/dev/null || true
                    fi
                fi
            done
        fi
    done

    print_success "Sincronização de versões concluída"
    echo ""
}

# Array global para guardar as versões incrementadas
declare -a VERSION_CHANGES=()

# Função para atualizar versões no projects.json (sem jq)
update_cfn_versions() {
    local selected_services=("$@")
    local env_choice="$1"
    shift
    selected_services=("$@")

    VERSION_CHANGES=()

    if [[ "$env_choice" == "3" ]]; then
        print_info "Versões CFN não foram alteradas"
        return 0
    fi

    # Não criar backup pois está versionado no git

    # Processar cada serviço selecionado
    for service in "${selected_services[@]}"; do
        local project_path="${SERVICE_TO_PROJECT[$service]}"

        if [[ -n "$project_path" ]]; then
            local env_name=""
            local old_version=""
            local new_version=""

            # Implementação mais simples usando sed
            case "$env_choice" in
                "1") # DEV apenas
                    env_name="dev"
                    old_version=$(grep -A 10 "\"projectPath\": \"$project_path\"" Cfn/projects.json | grep "\"dev\"" | head -1 | sed 's/.*"dev": "\([^"]*\)".*/\1/')
                    new_version=$(increment_patch_version "$old_version")
                    new_version="${new_version%%$'\n'*}"
                    sed -i.tmp "/\"projectPath\": \"$project_path\"/,/}/s/\"dev\": \"[^\"]*\"/\"dev\": \"$new_version\"/g" Cfn/projects.json
                    rm -f Cfn/projects.json.tmp 2>/dev/null || true
                    ;;
                "2") # PRD apenas
                    env_name="prd"
                    old_version=$(grep -A 10 "\"projectPath\": \"$project_path\"" Cfn/projects.json | grep "\"prd\"" | head -1 | sed 's/.*"prd": "\([^"]*\)".*/\1/')
                    new_version=$(increment_patch_version "$old_version")
                    new_version="${new_version%%$'\n'*}"
                    sed -i.tmp "/\"projectPath\": \"$project_path\"/,/}/s/\"prd\": \"[^\"]*\"/\"prd\": \"$new_version\"/g" Cfn/projects.json
                    rm -f Cfn/projects.json.tmp 2>/dev/null || true
                    ;;
            esac

            # Guardar a mudança para mostrar no resumo
            VERSION_CHANGES+=("$project_path ($env_name): $old_version -> $new_version")
        else
            print_warning "Projeto não encontrado para serviço: $service"
        fi
    done
}

# Função para mostrar resumo das alterações
show_summary() {
    local selected_services=("$@")

    echo ""
    echo -e "${GREEN}================================================${NC}"
    echo -e "${GREEN}              RESUMO DAS ALTERAÇÕES            ${NC}"
    echo -e "${GREEN}================================================${NC}"
    echo ""

    print_info "Serviços selecionados para prioridade:"
    for service in "${selected_services[@]}"; do
        local project_path="${SERVICE_TO_PROJECT[$service]}"
        echo "  - ${project_path:-$service}"
    done

    # Mostrar versões incrementadas
    if [[ ${#VERSION_CHANGES[@]} -gt 0 ]]; then
        for change in "${VERSION_CHANGES[@]}"; do
            print_warning "$change"
        done
    fi

    echo ""
    print_info "Arquivos modificados:"
    echo "  - .gitlab-ci.yml (cópia do template + jobs *_ci ativos conforme seleção)"
    echo "  - Cfn/projects.json (versões atualizadas)"

    echo ""
    print_success "Deploy manager executado com sucesso!"
    echo ""
}

# Função principal
main() {
    print_header

    # Verificar arquivos necessários
    check_files

    # Verificar se estamos no Windows/Git Bash
    if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "win32" ]] || [[ -n "$WINDIR" ]]; then
        print_info "Detectado ambiente Windows - usando implementação nativa"
    elif ! command -v jq &> /dev/null; then
        print_error "jq não está instalado. Por favor, instale jq para continuar."
        print_info "Ubuntu/Debian: sudo apt-get install jq"
        print_info "CentOS/RHEL: sudo yum install jq"
        print_info "macOS: brew install jq"
        exit 1
    fi

    local selected_services=()
    local selection_validated=false

    # Loop para seleção de serviços
    while [[ "$selection_validated" == "false" ]]; do
        show_services_menu

        read -p "Digite sua seleção: " user_input

        if [[ "$user_input" == "quit" ]]; then
            print_info "Operação cancelada pelo usuário"
            exit 0
        fi

        local validated_selection
        if validated_selection=$(validate_selection "$user_input"); then
            if [[ "$validated_selection" == "quit" ]]; then
                print_info "Operação cancelada pelo usuário"
                exit 0
            fi

            # Converter números para nomes de serviços
            selected_services=()
            for num in $validated_selection; do
                selected_services+=("${SERVICES[$num]}")
            done

            selection_validated=true

            echo ""
            print_success "Serviços selecionados:"
            for service in "${selected_services[@]}"; do
                local project_path="${SERVICE_TO_PROJECT[$service]}"
                echo "  - ${project_path:-$service}"
            done
        else
            echo ""
            print_error "Seleção inválida. Tente novamente."
            echo ""
        fi
    done

    echo ""

    # Sempre partir do template (estrutura fixa) e só então comentar/descomentar *_ci
    materialize_gitlab_ci_from_template
    apply_ci_job_selection "${selected_services[@]}"

    echo ""

    # Menu para incremento de versões
    show_version_menu

    local version_choice
    while true; do
        read -p "Digite sua escolha (1-3): " version_choice

        if [[ "$version_choice" =~ ^[1-3]$ ]]; then
            break
        else
            print_error "Escolha inválida. Digite um número de 1 a 3."
        fi
    done

    # Atualizar versões CFN se necessário
    if [[ "$version_choice" != "3" ]]; then
        # Primeiro, sincronizar versões com as branches remotas
        sync_versions_from_remote "$version_choice" "${selected_services[@]}"

        # Depois, incrementar as versões
        update_cfn_versions "$version_choice" "${selected_services[@]}"
    fi

    # Mostrar resumo
    show_summary "${selected_services[@]}"
}

# Executar função principal
main "$@"