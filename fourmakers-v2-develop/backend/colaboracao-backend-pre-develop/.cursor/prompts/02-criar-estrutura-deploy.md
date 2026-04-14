# Comando: Criar Estrutura de Deploy

**Uso:** `@criar-estrutura-deploy` — forneça o nome do projeto e os controllers ao invocar.

**Exemplo de invocação no chat:**
> `@criar-estrutura-deploy` Crie a estrutura de deploy para o projeto **MeuContexto.API** com os controllers: `MeuContexto`, `MeuContextoDetalhe`.

---

## Prompt de Criação

Crie e/ou valide todos os arquivos necessários para o deploy do projeto `{NOME_DO_PROJETO}` seguindo as diretrizes do Colaboração Backend.

**Parâmetros:**
- **Nome do projeto:** `{NOME_DO_PROJETO}` (ex.: `Financeiro.API`, `ProcessamentoConsumer`)
- **Nome do serviço (kebab-case):** `{nome-do-servico}` (ex.: `financeiro-api`, `processamento-consumer`)
- **Controllers (lista):** `{LISTA_DE_CONTROLLERS}` (ex.: `["Financeiro", "Lancamento"]`) — usar `[]` para Consumers
- **Tipo:** `{TIPO}` = `API` ou `Consumer`

---

## Arquivos a criar/validar

### 1. `ColaboracaoBackend/{NOME_DO_PROJETO}/Dockerfile`

Gere o Dockerfile seguindo o padrão multi-stage obrigatório:

```dockerfile
# syntax=docker/dockerfile:1.7-labs

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY --parents */*.csproj /src/
RUN dotnet restore "{NOME_DO_PROJETO}/{NOME_DO_PROJETO}.csproj"
COPY . .
WORKDIR "/src/{NOME_DO_PROJETO}"

FROM build AS publish
RUN dotnet publish "{NOME_DO_PROJETO}.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 80
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "{NOME_DO_PROJETO}.dll"]
```

> Se for **Consumer** (Worker), substituir `aspnet` por `aspnet` e ajustar o ENTRYPOINT para o `.dll` do Worker.

---

### 2. `ColaboracaoBackend/{NOME_DO_PROJETO}/.dockerignore`

```
**/bin/
**/obj/
**/.vs/
**/.env
**/*.user
**/TestResults/
```

---

### 3. `ColaboracaoBackend/{NOME_DO_PROJETO}/.gitlab-ci.yml`

```yaml
include:
  - local: .gitlab-rules.yml

stages:
  - init
  - setup
  - delivery

init:
  stage: init
  script:
    - echo "Iniciando CI/CD do projeto {NOME_DO_PROJETO}"

delivery:
  stage: delivery
  rules:
    - *is_develop
    - *is_release
  trigger:
    include: ColaboracaoBackend/{NOME_DO_PROJETO}/.gitlab-cd.yml
    strategy: depend
```

---

### 4. `ColaboracaoBackend/{NOME_DO_PROJETO}/.gitlab-cd.yml`

```yaml
include:
  - .gitlab-rules.yml
  - .gitlab-cd.yml

variables:
  FF_USE_FASTZIP: "true"

.cd_project_env: &cd_project_env
  - export ECR_PROJECT_PATH=backoffice/{nome-do-servico}
  - export CI_AWS_ECS_SERVICE=srv-{nome-do-servico}
  - export PROJECT_CFN_PATH=Cfn/{nome-do-servico}/
  - export PROJECT_NAME={nome-do-servico}
  - export PROJECT_DOCKER_PATH={NOME_DO_PROJETO}/Dockerfile

stages:
  - check-versions
  - check-deploy
  - iac-resources
  - publish
  - iac-services
  - deploy
  - deploy-alb-rules
  - tag-version

# --- Jobs herdados do .gitlab-cd.yml central ---
# (incluir os jobs padrão referenciando os scripts centrais via !reference)
```

---

### 5. `ColaboracaoBackend/{NOME_DO_PROJETO}/Cfn/.env`

```bash
HOST_NAME_BASE="{nome-do-servico}"
HEALTH_CHECK_PATH="/health"
PROJECT="{nome-do-servico}"

# Variáveis exclusivas do projeto (adicionar conforme necessário)
# MINHA_VARIAVEL_EXCLUSIVA=valor
```

> As três primeiras variáveis são **obrigatórias** e lidas pelo `build-template.py`.

---

### 6. Entrada em `Cfn/projects.json`

Adicionar o seguinte objeto no array do arquivo `Cfn/projects.json`:

```json
{
  "projectPath": "{NOME_DO_PROJETO}",
  "serviceName": "{nome-do-servico}",
  "version": {
    "dev": "1.0.0",
    "prd": "1.0.0"
  },
  "controllers": {LISTA_DE_CONTROLLERS}
}
```

> Para **Consumers**, `"controllers"` deve ser `[]`.

---

### 7. Atualizar `deploy-manager.sh` (raiz do repositório)

Incluir o novo serviço no script de deploy local:

1. No array **SERVICES**, adicionar uma entrada em ordem alfabética (ex.: `["N"]="{nome-do-servico}"`) e renumerar as entradas seguintes se necessário.
2. No array **SERVICE_TO_PROJECT**, adicionar o mapeamento (ex.: `["{nome-do-servico}"]="{NOME_DO_PROJETO}"`).

Assim o deploy manager poderá priorizar e incrementar versões do novo projeto.

---

### 8. Trigger no `.gitlab-ci.template.yml` (raiz do repositório)

A esteira raiz é gerada a partir de **`.gitlab-ci.template.yml`** (fonte canônica; o GitLab não avalia esse nome — só `.gitlab-ci.yml`). O `deploy-manager.sh` copia o template para `.gitlab-ci.yml` e descomenta os `*_ci` escolhidos.

1. Em **`stages:`**, incluir `startup-{nome-do-servico}` na posição alfabética correta (mesma ordem dos demais `startup-*`).
2. Adicionar o bloco do job **comentado** (padrão do repositório), no mesmo formato dos outros serviços:

```yaml
# {nome-do-servico}_ci:
#   stage: startup-{nome-do-servico}
#   allow_failure: true
#   trigger:
#     include: ColaboracaoBackend/{NOME_DO_PROJETO}/.gitlab-ci.yml
#     strategy: depend
#   rules:
#     - *is_config
#     - *is_feature
#     - *is_develop
#     - *is_release
#     - *is_bugfix
#     - *is_hotfix
#     - *is_merge
```

3. Rodar `./deploy-manager.sh` para regenerar `.gitlab-ci.yml` a partir do template com os serviços desejados ativos.

---

## Passos pós-criação

Após criar os arquivos acima:

1. ✅ Commitar os arquivos gerados.
2. 🔐 Solicitar ao **administrador** que execute (carregar variáveis de ambiente com `setenv` antes de cada comando Python):
   ```bash
   cd Cfn
   # Alterar setenv linha 7 → export AWS_ENVIRONMENT=dev
   # Ambiente DEV: carregar setenv e gerar templates para develop
   . setenv dev
   python build-template.py develop

   # Alterar setenv linha 7 → export AWS_ENVIRONMENT=prd
   # Ambiente PRD: carregar setenv e gerar templates para main
   . setenv prd
   python build-template.py main
   ```
   > **Importante:** antes de `. setenv dev` ou `. setenv prd`, alterar no arquivo `setenv` a linha 7 para `export AWS_ENVIRONMENT=dev` ou `export AWS_ENVIRONMENT=prd` conforme o ambiente. Em seguida executar `. setenv <ambiente>` antes de cada `python build-template.py`.
3. ✅ Commitar os templates gerados em `Cfn/{nome-do-servico}/`.
4. 🔐 Configurar variáveis sensíveis no **AWS Secrets Manager** (não em plain text).
5. ✅ Verificar e criar variáveis GitLab CI/CD necessárias:
   - `VERSAO_PROJETO_{NOME_EM_MAIUSCULO}` (development + production)
   - `CI_AWS_ECR`, `CI_AWS_ECS_CLUSTER` (se ainda não existirem)

---

## Validação final

Após criar todos os arquivos, execute o checklist de validação:

- [ ] `Dockerfile` segue o padrão multi-stage
- [ ] `.dockerignore` exclui `bin/`, `obj/`, `.vs/`
- [ ] `.gitlab-ci.yml` possui trigger para `.gitlab-cd.yml` do projeto
- [ ] `.gitlab-cd.yml` possui todas as 5 variáveis de identidade preenchidas corretamente
- [ ] `Cfn/.env` possui as 3 variáveis obrigatórias
- [ ] Entrada adicionada em `Cfn/projects.json`
- [ ] Serviço adicionado no `deploy-manager.sh` (SERVICES e SERVICE_TO_PROJECT)
- [ ] Trigger comentado adicionado em `.gitlab-ci.template.yml` (e `stages:` atualizado); `.gitlab-ci.yml` via deploy manager
- [ ] Nenhum segredo em plain text nos arquivos commitados
