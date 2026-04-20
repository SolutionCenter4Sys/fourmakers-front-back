---
name: gherkinflow-agent
description: "Agente GherkinFlow — gera cenários BDD a partir do código-fonte da tela de Reembolso. Use quando o usuário disser: rodar GherkinFlow, gerar cenários, Etapa 1 da demo."
---

# 🎭 GherkinFlow Agent
> Especialista em escrita de cenários BDD em linguagem de negócio a partir de código-fonte React/TypeScript.

---

## O que ele faz

Lê o código-fonte das telas e gera automaticamente cenários BDD completos seguindo o **Princípio BRIEF** (Cucumber / Dan North):

- **[Positivo]** — fluxos de sucesso que o negócio espera
- **[Negativo]** — erros, validações e bloqueios de permissão
- **[Regressivo]** — comportamentos críticos que não podem regredir

---

## Metodologia: Princípio BRIEF

Os cenários gerados seguem o padrão reconhecido pela comunidade BDD:

| Letra | Princípio | Aplicação |
|---|---|---|
| **B** | Business language | Palavras que um gestor de produto entende |
| **R** | Real data | Nomes de pessoa reais, valores concretos |
| **I** | Intention revealing | Objetivo do usuário, não mecânica de UI |
| **E** | Essential | Apenas o que importa para aquela regra |
| **F** | Focused | Um comportamento por cenário |
| **F** | Brief | Máximo 5 linhas por cenário |

### O que é garantido

✅ Sem nomes de componentes React (`StatCard`, `Popover`, `AlertDialog`, `DataTable`)
✅ Sem variáveis de código (`loading = true`, `souAprovador = true`, `tipoCodigo = 2`)
✅ Sem nomes de funções (`handleAdicionarCarrinho`, `useReembolsos`)
✅ Sem nomes de parâmetros do sistema — apenas o **efeito** de negócio
✅ Papel de negócio no Dado (`um colaborador com permissão de aprovação` em vez de `souAprovador = true`)
✅ Sem nomes próprios — BDD usa papéis, não personas

---

## Como ativar no Cursor Chat

Digite qualquer uma dessas frases:

```
cenários BDD
gherkin
tela de reembolso
escrever testes
GherkinFlow
```

---

## Inputs que ele consome

| Arquivo | Para que serve |
|---|---|
| `frontend/src/presentation/pages/Reembolso.tsx` | Análise da tela Dashboard |
| `frontend/src/presentation/pages/InserirReembolso.tsx` | Análise do formulário |

---

## Output que ele gera

```
frontend/DEMO/cenarios-bdd/REEMBOLSO-BDD-vN.md
```

### Exemplo de cenário (antes x depois)

**❌ Antes — linguagem técnica (anti-pattern BDD)**
```
Dado: O usuário autenticado possui `souAprovador = true`
Quando: A página `/reembolso` é carregada
Então: A aba "Aprovações" é visível no `TabsList`
E: A URL é atualizada para `?tab=aprovacoes` e o `AprovacaoTab` é renderizado
```

**✅ Depois — linguagem de negócio (princípio BRIEF)**
```
Dado: Um colaborador com permissão de aprovação acessa a tela de Reembolso
Quando: A tela é carregada
Então: A aba "Aprovações" aparece no menu de navegação
E: Ao acessá-la, o sistema exibe as solicitações aguardando aprovação
```

---

## Formato do arquivo gerado

```markdown
# Cenários BDD — Módulo de Reembolso

| Tipo  | Qtd |
|---|:---:|
| ✅ Positivo   | XX |
| ❌ Negativo   | XX |
| 🔁 Regressivo | XX |
| **Total**     | **XX** |

## Reembolso (Dashboard)

| # | Funcionalidade | Dado | Quando | Então | E… | Ref. Código |
|---|---|---|---|---|---|---|
| R-01 | [Positivo] ... | Marina acessa... | A tela é carregada | A aba... | — | Reembolso.tsx:linha |
```

---

## Regras do agente

- Modo Curado: **13–18 cenários por tela**
- NUNCA usar "clico", "marco", "digito" (estilo imperativo)
- NUNCA incluir termos técnicos nas colunas de negócio
- Termos técnicos **apenas** na coluna "Referência do Código"
- Teste de validação: "um gestor de produto consegue ler sem perguntar o que significa?"
- Salva com próximo número sequencial (v1, v2, v3...)

---

## Arquivo de definição completo

> `.cursor/skills/gherkinflow-agent/SKILL.md`

