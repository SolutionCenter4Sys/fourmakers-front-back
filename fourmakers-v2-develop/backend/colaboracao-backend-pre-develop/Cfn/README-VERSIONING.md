# Sistema de Versionamento para Deploy Condicional

## Visão Geral

Este sistema permite que cada projeto execute seu deploy de forma condicional baseada na comparação entre a versão definida no arquivo `deploy_config.json` (gerado pelo stage `check-versions`) e a lógica implementada no stage `check-deploy`. Apenas projetos com versões diferentes ou que não possuem variáveis configuradas serão executados.

## Como Funciona

### 1. Arquitetura por Projeto

Cada projeto agora possui **stages específicos** em seu arquivo `.gitlab-cd.yml`:

#### **check-versions** (Primeiro Stage)
- **Objetivo**: Verificar versões dos projetos e gerar `deploy_config.json`
- **Execução**: Sempre executado quando `*is_develop` ou `*is_release` são verdadeiros
- **Saída**: Arquivo `deploy_config.json` com informações de versão

#### **check-deploy** (Segundo Stage)
- **Objetivo**: Determinar se o deploy é necessário para o projeto específico
- **Execução**: Executa o script `check-deploy-from-json.sh` com o identificador do projeto
- **Lógica**:
  - Se deploy necessário → `exit 0` (sucesso)
  - Se deploy não necessário → `exit 1` (falha, para a pipeline)

#### **tag-version** (Último Stage - Apenas em PRODUCTION)
- **Objetivo**: Criar tag de versão automaticamente após deploy bem-sucedido em produção
- **Execução**: Executa o script `tag-version.sh` com o identificador do projeto
- **Formato da Tag**: `{serviceName}-{version.prd}` (exemplo: `apontamento-api-1.0.2`)
- **Comportamento**:
  - Cria tag via API do GitLab sem disparar nova pipeline (usa `[ci skip]`)
  - Verifica se a tag já existe antes de criar
  - Executa apenas após sucesso do stage `deploy-alb-rules`
- **Importante**: ⚠️ A tag é criada via API para **NÃO** disparar um novo ciclo de pipeline

### 2. Jobs de Verificação de Versões

Cada projeto inclui **dois jobs separados** para controle de ambiente:

#### **check_project_versions_develop** (Ambiente DEVELOPMENT)
- Executa na branch `develop`
- Usa versões do ambiente `dev`
- Variável `CI_ENV=dev`

#### **check_project_versions_release** (Ambiente PRODUCTION)  
- Executa em tags (`CI_COMMIT_TAG`)
- Usa versões do ambiente `prd`
- Variável `CI_ENV=prd`

**Funcionalidades de ambos os jobs:**
- **Lê dinamicamente a lista de projetos** do arquivo `Cfn/projects.json`
- **Lê as versões específicas do ambiente** (propriedade `version.dev` ou `version.prd`)
- **Conecta diretamente à API do GitLab** usando `$CI_PROJECT_TOKEN`
- Compara versões em tempo real
- **Configura automaticamente as variáveis no GitLab** via API
- Gera arquivo `deploy_config.json` com informações de versão

**Nota**: O script está localizado em `Cfn/scripts/check-project-versions.sh` para manter o `.gitlab-cd.yml` limpo e organizado.

**Importante**: A lista de projetos é lida dinamicamente do arquivo `Cfn/projects.json`, tornando o sistema totalmente flexível e fácil de manter. Para adicionar ou remover projetos, basta editar o JSON.

### 3. Controle de Execução por Projeto

**IMPORTANTE**: O stage `check-deploy` agora controla a execução de cada projeto individualmente.

#### **Como funciona o controle:**

1. **Stage `check-versions`** gera o arquivo `deploy_config.json`
2. **Stage `check-deploy`** executa `check-deploy-from-json.sh <PROJECT_VAR>`
3. **Script determina se deploy é necessário:**
   - **Deploy necessário** → `exit 0` (pipeline continua)
   - **Deploy não necessário** → `exit 1` (pipeline para)

#### **Vantagens do controle por projeto:**
- ✅ **Isolamento**: Cada projeto controla sua própria execução
- ✅ **Simplicidade**: Lógica clara e direta
- ✅ **Controle**: Pipeline para se deploy não for necessário
- ✅ **Rastreabilidade**: Fácil identificar qual projeto falhou
- ✅ **Flexibilidade**: Cada projeto pode ter sua própria lógica de deploy

### 4. Como Funciona a Leitura Dinâmica

O script `check-project-versions.sh` agora:

1. **Lê o arquivo `projects.json`** usando `jq`
2. **Detecta o ambiente** através da variável `CI_ENV` (dev/prd)
3. **Extrai todos os `serviceName`** de cada projeto
4. **Extrai as versões específicas do ambiente** (`version.dev` ou `version.prd`)
5. **Converte para o formato correto** (maiúsculo com underscores)
6. **Processa cada projeto** automaticamente comparando versões do ambiente correto
7. **Gera `deploy_config.json`** com informações de versão

**Detecção automática de ambiente:**
- **DEVELOPMENT**: `CI_ENV=dev` → usa `version.dev`
- **PRODUCTION**: `CI_ENV=prd` → usa `version.prd`
- **Fallback**: se `CI_ENV` não estiver definida, usa `dev` por padrão

#### Regras de Execução por Ambiente

**Job `check_project_versions_develop`:**
- **Trigger**: Branch `develop` ou pipeline filha na branch `develop`
- **Ambiente**: DEVELOPMENT
- **Versões**: Lê `version.dev` de cada projeto
- **Variáveis GitLab**: `VERSAO_PROJETO_{PROJETO}` para ambiente dev

**Job `check_project_versions_release`:**
- **Trigger**: Tags (`CI_COMMIT_TAG`) ou pipeline filha com tag
- **Ambiente**: PRODUCTION  
- **Versões**: Lê `version.prd` de cada projeto
- **Variáveis GitLab**: `VERSAO_PROJETO_{PROJETO}` para ambiente prd

**Benefícios da separação:**
- ✅ **Isolamento**: Versões de dev e produção são independentes
- ✅ **Controle**: Deploy em produção só acontece via tags
- ✅ **Rastreabilidade**: Cada ambiente tem suas próprias variáveis
- ✅ **Flexibilidade**: Pode ter versões diferentes por ambiente

**Exemplo de saída:**
```bash
=== Verificando versões dos projetos ===
Lendo lista de projetos do arquivo projects.json...
Projetos encontrados: 29
Lista: USUARIO_API COLABORADOR_API COMPETENCIA_API CRM_API FOURSYS_API MAPAALOCACAO_API PROJETO_API SRS_API UPLOADFILES_API ROTINASBACKOFFICE_API APONTAMENTO_API BI_API FIREBASE_API BOTFOURMAKERS_API FINANCEIRO_API SOCIAL_API CURRICULO_BATCH_CONSUMER FOLHA_CONSUMER BANCOTALENTOSRS_CONSUMER

Verificando versões dos projetos...

Ambiente detectado: dev
Projeto: USUARIO_API
  Diretório: usuario-api
  Versão JSON (dev): 1.0.0
  Variável GitLab: VERSAO_PROJETO_USUARIO_API
  Versão GitLab: NÃO EXISTE
  Status: DEPLOY NECESSÁRIO
  Ação: Configurando variável VERSAO_PROJETO_USUARIO_API=1.0.0
...
```

### 5. Arquivos de Versão

**IMPORTANTE**: As versões dos projetos agora são gerenciadas centralmente no arquivo `Cfn/projects.json` com **controle de ambiente**!

Cada projeto no JSON deve ter a propriedade `version` com versões específicas por ambiente:

```json
{
    "projectPath": "Usuario.API",
    "serviceName": "usuario-api",
    "version": {
        "dev": "1.0.0",    // Versão para ambiente DEVELOPMENT
        "prd": "1.0.0"     // Versão para ambiente PRODUCTION
    },
    "controllers": [...]
}
```

**Vantagens desta abordagem:**
- ✅ **Centralizada**: Todas as versões em um único arquivo
- ✅ **Ambiente-específica**: Versões diferentes para dev e produção
- ✅ **Consistente**: Formato padronizado para todos os projetos
- ✅ **Versionável**: Mudanças de versão são rastreadas no Git
- ✅ **Automática**: Script lê automaticamente versões do ambiente correto
- ✅ **Flexível**: Fácil atualizar versões por ambiente

### 6. Variáveis de Ambiente do GitLab

Para cada projeto, deve ser criada uma variável de ambiente no GitLab com a nomenclatura:
`VERSAO_PROJETO_{PROJETO}`

**Importante**: As variáveis são configuradas automaticamente pelo job `check_project_versions` via API do GitLab.

### 7. Lógica de Deploy

O deploy será executado quando:
- A variável de ambiente `VERSAO_PROJETO_{PROJETO}` **NÃO EXISTIR** (primeira execução)
- A variável de ambiente `VERSAO_PROJETO_{PROJETO}` for **DIFERENTE** da versão no arquivo `projects.json`

## Fluxo da Pipeline por Projeto

```
1. check-versions stage
   ↓
   check_project_versions_develop/check_project_versions_release
   ↓
   Lê arquivo projects.json
   ↓
   Conecta à API do GitLab (CI_PROJECT_TOKEN, CI_API_V4_URL, CI_PROJECT_ID)
   ↓
   Compara versões em tempo real
   ↓
   Configura variáveis no GitLab via API
   ↓
   Gera deploy_config.json
   ↓
2. check-deploy stage
   ↓
   check_deploy job
   ↓
   Executa check-deploy-from-json.sh <PROJECT_VAR>
   ↓
   Se deploy necessário → exit 0 (continua)
   Se deploy não necessário → exit 1 (para pipeline)
   ↓
3. Stages subsequentes (iac-resources, publish, iac-services, deploy, deploy-alb-rules)
   ↓
   Executam apenas se check-deploy passou
   ↓
4. tag-version stage (APENAS em PRODUCTION)
   ↓
   tag_version_release job
   ↓
   Executa tag-version.sh <PROJECT_VAR>
   ↓
   Cria tag via API GitLab: {serviceName}-{version.prd}
   ↓
   Tag criada com [ci skip] - NÃO dispara nova pipeline
```

## Configuração Automática

### Variáveis do GitLab CI

O sistema usa automaticamente as seguintes variáveis do GitLab:
- `$CI_PROJECT_TOKEN`: Token de acesso para a API
- `$CI_API_V4_URL`: URL base da API v4 do GitLab
- `$CI_PROJECT_ID`: ID do projeto atual

### Permissões Necessárias

O token `$CI_PROJECT_TOKEN` deve ter permissões para:
- Ler variáveis de projeto
- Criar variáveis de projeto
- Atualizar variáveis de projeto

## Como Atualizar Versões

### Opção 1: Atualizar via Arquivo projects.json (Recomendado)

1. **Edite o arquivo `Cfn/projects.json`**
2. **Localize o projeto desejado** na lista
3. **Altere a propriedade `version.dev` ou `version.prd`** para a nova versão
4. **Faça commit e push** das mudanças
5. **Execute a pipeline** do projeto
6. **O sistema automaticamente detectará a diferença** e executará o deploy se necessário
7. **A variável no GitLab será atualizada automaticamente** via API

**Exemplo de atualização:**
```json
{
    "projectPath": "Usuario.API",
    "serviceName": "usuario-api",
    "version": {
        "dev": "1.1.0",    // Nova versão para desenvolvimento
        "prd": "1.0.0"     // Versão de produção mantida
    }
}
```

### Opção 2: Atualizar via GitLab

1. **Acesse o projeto no GitLab**
2. **Vá em Settings > CI/CD > Variables**
3. **Atualize a variável `VERSAO_PROJETO_{PROJETO}`** para a nova versão
4. **Execute a pipeline**

### Opção 3: Forçar Deploy (Removendo Variável)

1. **Acesse o projeto no GitLab**
2. **Vá em Settings > CI/CD > Variables**
3. **Delete a variável `VERSAO_PROJETO_{PROJETO}`**
4. **Execute a pipeline** (será executado automaticamente)
5. **Após o deploy, a variável será recriada automaticamente** via API

## Exemplo de Uso

### Cenário 1: Primeira Execução
- Arquivo `projects.json`: `"version": {"dev": "1.0.0", "prd": "1.0.0"}`
- Variável GitLab: `VERSAO_PROJETO_FINANCEIRO_API` **não existe**
- **Resultado**: Deploy será executado + variável criada automaticamente

### Cenário 2: Versão Atualizada no projects.json
- Arquivo `projects.json`: `"version": {"dev": "1.1.0", "prd": "1.0.0"}`
- Variável GitLab: `VERSAO_PROJETO_FINANCEIRO_API=1.0.0`
- **Resultado**: Deploy será executado + variável atualizada automaticamente

### Cenário 3: Versão Igual
- Arquivo `projects.json`: `"version": {"dev": "1.0.0", "prd": "1.0.0"}`
- Variável GitLab: `VERSAO_PROJETO_FINANCEIRO_API=1.0.0`
- **Resultado**: Deploy **NÃO** será executado (versões iguais)

### Cenário 4: Forçar Deploy
- Arquivo `projects.json`: `"version": {"dev": "1.0.0", "prd": "1.0.0"}`
- Variável GitLab: `VERSAO_PROJETO_FINANCEIRO_API` **deletada**
- **Resultado**: Deploy será executado + variável recriada automaticamente

## Lista de Projetos

### APIs
- `financeiro-api` → `VERSAO_PROJETO_FINANCEIRO_API`
- `social-api` → `VERSAO_PROJETO_SOCIAL_API`
- `colaborador-api` → `VERSAO_PROJETO_COLABORADOR_API`
- `botfourmakers-api` → `VERSAO_PROJETO_BOTFOURMAKERS_API`
- `competencia-api` → `VERSAO_PROJETO_COMPETENCIA_API`
- `crm-api` → `VERSAO_PROJETO_CRM_API`
- `firebase-api` → `VERSAO_PROJETO_FIREBASE_API`
- `foursys-api` → `VERSAO_PROJETO_FOURSYS_API`
- `mapaalocacao-api` → `VERSAO_PROJETO_MAPAALOCACAO_API`
- `projeto-api` → `VERSAO_PROJETO_PROJETO_API`
- `srs-api` → `VERSAO_PROJETO_SRS_API`
- `uploadfiles-api` → `VERSAO_PROJETO_UPLOADFILES_API`
- `usuario-api` → `VERSAO_PROJETO_USUARIO_API`
- `rotinasbackoffice-api` → `VERSAO_PROJETO_ROTINASBACKOFFICE_API`
- `apontamento-api` → `VERSAO_PROJETO_APONTAMENTO_API`
- `bi-api` → `VERSAO_PROJETO_BI_API`
- `candidato-api` → `VERSAO_PROJETO_CANDIDATO_API`
- `dominio-api` → `VERSAO_PROJETO_DOMINIO_API`
- `formacao-api` → `VERSAO_PROJETO_FORMACAO_API`
- `hobby-api` → `VERSAO_PROJETO_HOBBY_API`
- `idioma-api` → `VERSAO_PROJETO_IDIOMA_API`
- `interesse-api` → `VERSAO_PROJETO_INTERESSE_API`
- `lg-api` → `VERSAO_PROJETO_LG_API`
- `metodologia-api` → `VERSAO_PROJETO_METODOLOGIA_API`
- `modeloreferencia-api` → `VERSAO_PROJETO_MODELOREFERENCIA_API`
- `reembolso-api` → `VERSAO_PROJETO_REEMBOLSO_API`
- `softskill-api` → `VERSAO_PROJETO_SOFTSKILL_API`

### Consumers
- `curriculo-batch-consumer` → `VERSAO_PROJETO_CURRICULO_BATCH_CONSUMER`
- `bancotalentosrs-consumer` → `VERSAO_PROJETO_BANCOTALENTOSRS_CONSUMER`
- `folha-consumer` → `VERSAO_PROJETO_FOLHA_CONSUMER`

## Tagueamento Automático de Versões

### Visão Geral

Após um deploy bem-sucedido em **PRODUCTION**, o sistema cria automaticamente uma tag Git com o nome da versão deployada. Esta tag serve como marco histórico e facilita o rastreamento de releases.

### Como Funciona

1. **Quando**: Executa apenas após deploy em produção (stage `tag-version`)
2. **Formato da Tag**: `{serviceName}-{version.prd}`
   - Exemplo: `apontamento-api-1.0.2`
3. **Criação via API**: Usa API do GitLab para criar tag **sem disparar nova pipeline**
4. **Script**: `Cfn/scripts/tag-version.sh`

### Características

- ✅ **Automático**: Cria tags sem intervenção manual
- ✅ **Seguro**: Não dispara novo ciclo de pipeline (usa `[ci skip]`)
- ✅ **Inteligente**: Verifica se tag já existe antes de criar
- ✅ **Rastreável**: Vincula tag ao commit exato do deploy
- ✅ **Consistente**: Usa sempre a versão do `projects.json`

### Exemplo de Uso

**Cenário**: Deploy do `apontamento-api` versão `1.0.2` em produção

```bash
# 1. Pipeline de release é disparada por uma tag manual
git tag v1.0.0
git push origin v1.0.0

# 2. Durante a pipeline, o sistema:
#    - Faz check-versions
#    - Faz check-deploy
#    - Executa deploy em produção
#    - Após sucesso, cria tag automaticamente

# 3. Tag criada: apontamento-api-1.0.2
#    - Vinculada ao commit do deploy
#    - NÃO dispara nova pipeline
#    - Visível em Repository > Tags
```

### Configuração no Projeto

Para adicionar tagueamento automático em um projeto:

1. **Adicionar stage** no `.gitlab-cd.yml`:
```yaml
stages:
  - check-versions
  - check-deploy
  - iac-resources
  - publish
  - iac-services
  - deploy
  - deploy-alb-rules
  - tag-version  # ← Adicionar este stage
```

2. **Adicionar job** no final do `.gitlab-cd.yml`:
```yaml
tag_version_release:
  stage: tag-version
  image: alpine:latest
  environment: PRODUCTION
  needs:
    - deploy_alb_rules_release
  before_script:
    - apk add --no-cache bash grep curl jq
  script:
    - chmod +x ./Cfn/scripts/tag-version.sh
    - ./Cfn/scripts/tag-version.sh <PROJECT_VAR>  # Ex: APONTAMENTO_API
  rules:
    - *is_release
```

### Verificação de Tags

Para verificar tags criadas:

```bash
# Listar todas as tags do projeto
git tag -l "*-api-*"

# Ver detalhes de uma tag específica
git show apontamento-api-1.0.2

# No GitLab: Repository > Tags
```

## Benefícios

1. **Automatização Completa**: Comparação e configuração automática de versões
2. **Integração Direta**: Conecta diretamente à API do GitLab
3. **Eficiência**: Apenas projetos modificados são executados
4. **Controle**: Controle granular sobre quais projetos são deployados
5. **Rastreabilidade**: Histórico de versões de cada projeto via tags automáticas
6. **Flexibilidade**: Pode ser usado para deploy seletivo em diferentes ambientes
7. **Manutenção Zero**: Não requer configuração manual de variáveis
8. **Arquitetura Simples**: Cada projeto controla sua própria execução
9. **Pipeline Limpa**: Lógica de deploy contida nos stages específicos
10. **Tagueamento Automático**: Tags de versão criadas automaticamente sem disparar pipelines

## Notas Importantes

- **Cada projeto agora possui seus próprios stages `check-versions`, `check-deploy` e `tag-version`**
- **O stage `check-deploy` controla se a pipeline continua ou para**
- **Se deploy não for necessário, a pipeline para no stage `check-deploy`**
- **O sistema funciona apenas para branches protegidas (`develop`, tags)**
- **A versão inicial de todos os projetos é `1.0.0`**
- **As variáveis de ambiente são configuradas automaticamente via API do GitLab**
- **O sistema lê automaticamente o arquivo `projects.json` e compara com as variáveis GitLab**
- **Para forçar um novo deploy, atualize a versão no arquivo `projects.json` ou delete a variável**
- **Os jobs `check_project_versions` devem ser executados antes do job `check_deploy`**
- **Requer permissões adequadas no token `$CI_PROJECT_TOKEN`**
- **Usa as variáveis padrão do GitLab CI: `$CI_PROJECT_TOKEN`, `$CI_API_V4_URL`, `$CI_PROJECT_ID`**
- **O arquivo `deploy_config.json` é gerado automaticamente e usado pelo stage `check-deploy`**
- **Tags criadas automaticamente usam `[ci skip]` para NÃO disparar nova pipeline**
- **O stage `tag-version` só executa em produção após sucesso do deploy**
