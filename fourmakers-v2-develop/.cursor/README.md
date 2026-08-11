# Proteção da Pasta .cursor

**Versão:** 1.2.0  
**Última Atualização:** 06/03/2026

Esta pasta contém configurações locais do Cursor que não devem ser versionadas ou alteradas durante checkout de branches.

## Comandos Disponíveis (commands/)

| Arquivo | Versão | Descrição | Última Atualização |
|---------|--------|-----------|-------------------|
| `DESENVOLVIMENTO-passo-1-git-investigator` | 1.1.0 | Análise de feature via histórico Git | 06/03/2026 |
| `DESENVOLVIMENTO-passo-2-planejar-implementacao` | 1.1.0 | Planejamento e geração de TODO | 06/03/2026 |
| `DESENVOLVIMENTO-passo-3-desenvolver` | 1.1.0 | Desenvolvimento da feature | 06/03/2026 |
| `DESENVOLVIMENTO-passo-4-code-review` | 1.1.0 | Revisão de código | 06/03/2026 |
| `GIT-passo-1-investigar-implementacao` | 1.1.0 | Investigação e idealização da solução | 06/03/2026 |
| `GIT-passo-2-git-feature-aggregator` | 1.1.0 | Agregação de arquivos por branch | 06/03/2026 |
| `DOCUMENTACAO-agregar-documentacao-da-feature` | 2.0.0 | Geração de documentação técnica | 06/03/2026 |
| `DOCUMENTACAO-formatar-documentacao-wiki` | 1.1.0 | Entrevista guiada para documentação Wiki | 06/03/2026 |
| `DEBUG-revisar-erros-comuns-front` | 1.1.0 | Detecção de erros comuns frontend | 06/03/2026 |
| `NEGOCIO-mapear-user-stories-edge-cases` | 1.1.0 | Mapeamento de user stories e edge cases | 06/03/2026 |
| `AUDIT-performance-frontend` | 1.0.0 | Auditoria de performance frontend | 06/03/2026 |
| `MR-code-review` | 1.1.0 | Revisão de merge request | 06/03/2026 |

## Rules Disponíveis (rules/) — English for better AI processing

| Rule | Versão | Categoria | Última Atualização |
|------|--------|-----------|-------------------|
| `agent-build-and-fix-errors` | 1.0.0 | Build/Fix | 06/03/2026 |
| `agent-missing-api-response-structure` | 1.2.0 | Integration | 06/03/2026 |
| `agent-nao-tem-response-integracao` | 1.2.0 | Integration | 06/03/2026 |
| `common-patterns-to-avoid-architecture` | 1.2.0 | Architecture | 06/03/2026 |
| `common-patterns-to-avoid-clean-code` | 1.2.0 | Clean Code | 06/03/2026 |
| `common-patterns-to-avoid-dead-code` | 1.0.0 | Dead Code | 06/03/2026 |
| `common-patterns-to-avoid-ddd` | 1.2.0 | DDD | 06/03/2026 |
| `common-patterns-to-avoid-dry` | 1.2.0 | DRY | 06/03/2026 |
| `common-patterns-to-avoid-duplicacao` | 1.0.0 | Duplicação | 06/03/2026 |
| `common-patterns-to-avoid-solid` | 1.2.0 | SOLID | 06/03/2026 |
| `common-patterns-to-avoid-specific` | 1.2.0 | Specific Patterns | 06/03/2026 |
| `common-patterns-error-message-utils` | 1.0.0 | Error Messages | 06/03/2026 |
| `common-patterns-backend-csharp-contracts` | 1.1.0 | Backend C# / API Contracts (alinhado a @ARCHITECTURE.md) | 06/03/2026 |

**Nota:** Todas as rules são escritas em inglês para melhor compreensão pela IA, mas cada uma exige que a saída final seja em português (PT-BR).

## Estrutura de Versionamento

Todos os prompts (comandos) e rules possuem cabeçalho com:
- **Versão / Version:** número semântico (ex: 1.1.0)
- **Última Atualização / Last Updated:** data no formato DD/MM/AAAA

**Improvements in v1.2.0 (rules):**
- Trigger-specific descriptions for better Cursor rule matching
- Concrete ❌/✅ code examples for every pattern
- Detection sections with grep commands
- Alignment with `@ARCHITECTURE.md` and `@DESIGN_SYSTEM_AUDIT.md`
- Hierarchical structure optimized for LLM semantic understanding

## Documentos de Referencia Externos

| Documento | Caminho | Proposito |
|-----------|---------|-----------|
| `ARCHITECTURE.md` | `/ARCHITECTURE.md` | Arquitetura Clean Architecture, fluxo de dados |
| `DESIGN_SYSTEM_AUDIT.md` | `/DESIGN_SYSTEM_AUDIT.md` | Auditoria de conformidade com Design System |
| `design-toolkit.md` | `/frontend/public/design-toolkit.md` | Tokens, componentes, hooks, padroes UI/UX |
| `docs/` | `/docs/README.md` | Indice de documentacao transversal |
| `DEMO/` | `/DEMO/README.md` | Demo pre-vendas Reembolso |

## Se arquivos estiverem versionados

Se por algum motivo arquivos dentro de `.cursor` estiverem versionados no git, você pode protegê-los de duas formas:

### Opção 1: Remover do índice (Recomendado)

```bash
# Verificar quais arquivos estão versionados
git ls-files .cursor/

# Remover do índice (mantém os arquivos localmente)
git ls-files .cursor/ | xargs git rm --cached

# Confirmar que está no .gitignore
grep -q "^\.cursor$" .gitignore || echo ".cursor" >> .gitignore
```

### Opção 2: Usar skip-worktree (para arquivos que devem permanecer no índice)

```bash
# Aplicar skip-worktree em todos os arquivos versionados
git ls-files .cursor/ | xargs git update-index --skip-worktree

# Para reverter depois (se necessário)
git ls-files .cursor/ | xargs git update-index --no-skip-worktree
```

## Verificar proteção

```bash
# Verificar se há arquivos versionados
git ls-files .cursor/

# Se não retornar nada, está protegido! ✅
```

## Nota

Como a pasta já está no `.gitignore`, ela não deveria ser afetada por checkouts de branches. Se você ainda assim encontrar problemas, use uma das opções acima.
