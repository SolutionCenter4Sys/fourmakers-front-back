# MR Code Review — Revisao de Merge Request

**Versao:** 1.1.0  
**Ultima Atualizacao:** 06/03/2026  
**Referencia:** `@.cursor/commands/passo-4-code-review.md`

---

## Sobre este Comando

Este comando é um **alias** para o `passo-4-code-review` adaptado especificamente para revisao de Merge Requests. Use comando GIT para obter a tree de commits desta branch caso nao tenha.

A unica diferenca: o arquivo de saida deve incluir o **nome da branch** e **MR ID** no nome do arquivo.

---

## Uso

Execute comando GIT para obter os commits e depois aplique as instrucoes de `@.cursor/commands/passo-4-code-review.md` com os seguintes ajustes:

### Nome do Arquivo de Saida

```
docs/code-reviews/MR_[MR_ID]_[branch-name]_CODE_REVIEW_[timestamp].md
```

Exemplo:
```
docs/code-reviews/MR_42_feat_permissionamento_CODE_REVIEW_20250225_143022.md
```

### Secao Adicional no Relatorio

Adicione no inicio do relatorio:

```markdown
# Relatorio de Code Review — Merge Request

**MR ID:** [!42](https://github.com/[ACCOUNT]]/[PROJECT]/...)
**Branch:** `feat/permissionamento`  
**Target:** `develop`  
**Autor:** [Nome do autor]  
**Data:** 25/02/2026  
**Revisor:** AI Agent

---

## Resumo para Merge

| Criterio | Status | Observacao |
|----------|--------|------------|
| Aprovacao Tecnica | ✅/⚠️/❌ | ... |
| Conformidade Arquitetural | ✅/⚠️/❌ | ... |
| Design System | ✅/⚠️/❌ | ... |
| Recomendacao | **APROVAR / APROVAR COM AJUSTES / REJEITAR** | |
```

---

## Instrucoes Completas

Para instrucoes completas de revisao, consulte:
**`@.cursor/commands/passo-4-code-review.md`**

Inclui:
- Scope of Analysis (6 categorias)
- Severity Classification (CRITICAL/WARNING/SUGGESTION)
- Tool Usage Strategy
- Output Format completo
- Regras e exemplos
