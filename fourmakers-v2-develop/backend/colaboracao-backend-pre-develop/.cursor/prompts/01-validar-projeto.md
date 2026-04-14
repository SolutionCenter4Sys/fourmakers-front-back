# Comando: Validar Projeto

**Uso:** `@validar-projeto` — forneça o nome do projeto como parâmetro ao invocar.

**Exemplo de invocação no chat:**
> `@validar-projeto` Analise o projeto **Financeiro.API** e valide sua conformidade com as diretrizes.

---

## Prompt de Validação

Analise o projeto `{NOME_DO_PROJETO}` (localizado em `ColaboracaoBackend/{NOME_DO_PROJETO}/`) e valide sua conformidade com as **Diretrizes de Backend** do projeto Colaboração.

Para cada item abaixo, indique: ✅ Conforme | ⚠️ Atenção | ❌ Não conforme | ➖ Não aplicável.
Ao final, gere um relatório com os pontos críticos e sugestões de correção prioritárias.

---

### 1. Arquitetura e Estrutura de Projetos

- [ ] O projeto possui os projetos esperados: `{Nome}.API`, `{Nome}.Application`, `{Nome}.Domain`?
- [ ] Existe referência a `Colaboracao.Infra` (repositórios) e a `Core.DomainModel` (interfaces)?
- [ ] O `Program.cs` usa `ApplicationConfigurator.ConfigureApplication` com o `Initializer` da Application?
- [ ] Não existem lógica de negócio em Controllers ou acesso direto ao banco fora da Infra?

### 2. Camada API — Controllers

- [ ] Todos os Controllers herdam de `ControllerBase`?
- [ ] Todos os Controllers possuem `[Authorize]`, `[HandleException]`, `[ApiController]` e `[LogAction]` na **classe**?
- [ ] A rota segue o padrão `api/{Contexto}/{Recurso}`?
- [ ] O usuário logado é obtido via `IAspNetUser.GetUsuarioLogado()` no construtor do Controller?
- [ ] Os Controllers apenas repassa parâmetros ao Service (sem lógica de negócio)?
- [ ] Os Controllers retornam `Ok(result)` com `ApiGenericResult`?
- [ ] Não há injeção de repositórios, `IDBConnection` ou dependências de Infra nos Controllers?

### 3. Camada Application — Initializer

- [ ] Existe a classe `Initializer` implementando `IInitializerConfigurator`?
- [ ] O método `ConfigureServices` chama `AddBaseGeralServicosColaboracao` e `AddServicosComunsColaboracao`?
- [ ] Cada domínio tem seu método `Add{Contexto}Dependencies()` chamado no Initializer?
- [ ] O método `ConfigureAppStandard` chama `app.ConfigureAppStandard(env, projectName)`?

### 4. Camada Domain — Services

- [ ] Todas as classes de Service possuem o atributo `[LogDomainClass]`?
- [ ] Os Services retornam `ApiGenericResult<T>` ou `ApiGenericResult`?
- [ ] Os Services usam `ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.{Acao}, DESCRICAO_ENTIDADE)` nos blocos `catch`?
- [ ] Existe a constante `DESCRICAO_ENTIDADE` em cada Service?
- [ ] Os Services não dependem de `IAspNetUser` diretamente (recebem `cpf` e `orgId` como parâmetros)?
- [ ] Existem ValidatorServices separados para regras de validação de acesso e input?
- [ ] Transações são gerenciadas no Service via `IDBConnectionUnitOfWork` (não no Repository)?
- [ ] Não existem Factory para models (padrão legado)?

### 5. DI — Colaboracao.Initializer

- [ ] Services são registrados com `TryAddScopedDomainService<I, T>()` (não `AddScoped` simples)?
- [ ] ValidatorServices são registrados com `TryAddScoped<I, T>()`?
- [ ] Repositórios são registrados com `TryAddScoped<I, T>()` (interface Core.Domain, impl Colaboracao.Infra)?
- [ ] Há um método de extensão `Add{Contexto}Dependencies` no `Colaboracao.Initializer` para cada domínio?

### 6. Camada Infra — Repositórios

- [ ] Todas as interfaces de repositório estão em `Core.DomainModel` (namespace `Core.Domain.*`)?
- [ ] As implementações estão em `Colaboracao.Infra/Repositories/{Contexto}/{Recurso}/`?
- [ ] Os repositórios usam apenas **Dapper** (`QueryAsync`, `QueryFirstOrDefaultAsync`, `ExecuteAsync`)?
- [ ] Não existem novos `DbContext` ou uso de Entity Framework em novas implementações?
- [ ] Os repositórios recebem `IDBConnection` pelo construtor?
- [ ] As queries SQL usam aliases em **PascalCase** para mapeamento direto nos DTOs?
- [ ] Métodos de escrita aceitam `IDbTransaction? transaction = null`?
- [ ] Os repositórios retornam tipos de `DataTransferObject.Domain`?

### 7. DTOs — DataTransferObject.Domain

- [ ] Todos os DTOs (Input, Result, Base) estão em `DataTransferObject.Domain/{Contexto}/{Recurso}/`?
- [ ] Não existem DTOs duplicados no Domain ou na API?
- [ ] O padrão Base → Input / Result é seguido?
- [ ] Campos sensíveis (senha, token, chave) estão marcados com `[LogMasked]`?

### 8. Observabilidade e Logging

- [ ] `[LogAction]` aplicado na **classe** de todos os Controllers?
- [ ] `[LogDomainClass]` aplicado na **classe** de todos os Services?
- [ ] Services com `[LogDomainClass]` registrados com `TryAddScopedDomainService` (não `AddScoped`)?
- [ ] `[HandleException]` aplicado na **classe** de todos os Controllers?
- [ ] Dados sensíveis nos DTOs marcados com `[LogMasked]`?
- [ ] `ILogCore` usado apenas para logs **pontuais e contextuais** (não como substituto do interceptor)?

### 9. Autenticação e Segurança

- [ ] Todos os endpoints protegidos com `[Authorize]` (exceto health check e login)?
- [ ] JWT configurado via `AddBaseGeralServicosColaboracao` (não manualmente)?
- [ ] Segredos (JWT, tokens) em variáveis de ambiente ou AWS Secrets Manager (não hardcoded)?

### 10. Integração HTTP (ApiClient)

- [ ] Novos clients HTTP definidos em `ApiClient.Domain` (Interfaces + Impl)?
- [ ] Não há uso direto de `HttpClient` ou `HttpClientFactory` nos projetos de Domain/API?
- [ ] URLs base em `EnvironmentVariables.cs`; paths em `ClientConfig.cs`?

### 11. Deploy e CI/CD (se aplicável)

- [ ] Existe `Dockerfile` com padrão multi-stage (`base` → `build` → `publish` → `final`)?
- [ ] Existe `.dockerignore` excluindo `bin/`, `obj/`, `.vs/`?
- [ ] Existe `.gitlab-ci.yml` com stages `init`, `setup`, `delivery`?
- [ ] Existe `.gitlab-cd.yml` com as variáveis `ECR_PROJECT_PATH`, `CI_AWS_ECS_SERVICE`, `PROJECT_CFN_PATH`, `PROJECT_NAME`, `PROJECT_DOCKER_PATH`?
- [ ] Existe `Cfn/.env` com `HOST_NAME_BASE`, `HEALTH_CHECK_PATH`, `PROJECT`?
- [ ] O projeto está registrado em `Cfn/projects.json`?

### 12. Migrations de Banco

- [ ] Novos scripts SQL estão em `Fourmakers.Core.Migrations/scripts/{ANO}/{MM_Mes}/{DD}/{CARD}/`?
- [ ] Nomenclatura segue `NN_acao_tabela.sql`?
- [ ] Scripts existentes não foram modificados (apenas novos scripts adicionados)?
- [ ] Scripts de risco possuem pasta `rollback/`?

---

## Resultado esperado

Apresente:
1. **Score por categoria** (quantidade de itens conformes / total verificáveis)
2. **Itens críticos** (❌ Não conforme) com localização exata no código e sugestão de correção
3. **Itens de atenção** (⚠️) que podem se tornar problemas
4. **Resumo executivo** em até 5 linhas com os principais pontos de melhoria
