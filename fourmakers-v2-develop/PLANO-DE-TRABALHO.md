# Plano de Trabalho — POC de Qualidade com IA
### Metodologia: GherkinFlow + DataForge + Playwright

| | |
|---|---|
| **Início** | 08/04/2026 |
| **Prazo de entrega** | 22/04/2026 |
| **Duração** | 2 semanas (10 dias úteis) |
| **Projeto** | _(a definir)_ |
| **Telas em escopo** | _(a definir)_ |
| **Ambiente de testes** | _(URL do ambiente — a definir)_ |

---

## 1. Objetivo

Executar o ciclo completo de qualidade assistido por IA em um projeto real:

| Etapa | Ferramenta | Entrega |
|-------|-----------|---------|
| 1 — Cenários de teste | GherkinFlow | Arquivo `.md` com cenários BDD |
| 2 — Dados de teste | DataForge | Arquivo `.sql` com massa de dados |
| 3 — Testes automatizados | Playwright | Testes rodando + relatório HTML |

---

## 2. Participantes

| Papel | Responsabilidade |
|-------|-----------------|
| Letícia | Definir escopo, revisar artefatos nos checkpoints, validar apresentação |
| Colaborador | Executar as etapas com auxílio do guia e da IA, guardar evidências |

---

## 3. Entregas

| # | Entregável | Formato |
|---|-----------|---------|
| E1 | Cenários BDD das telas escolhidas | `.md` |
| E2 | Massa de dados SQL coerente com os cenários | `.sql` |
| E3 | Testes Playwright gerados e executando | `.spec.ts` |
| E4 | Relatório HTML dos resultados dos testes | `.html` |
| E5 | Screenshots e logs das execuções | `.png` |
| E6 | Este plano de trabalho preenchido | `.md` |

---

## 4. Pré-requisitos — Levantar Antes do Dia 1

| Item | Responsável | Status |
|------|:-----------:|:------:|
| Definir quais telas serão testadas | Time | `[ ]` |
| Acesso ao repositório do projeto | Letícia | `[ ]` |
| Schema/DDL do banco de dados | Letícia | `[ ]` |
| URL do ambiente de testes | Letícia | `[ ]` |
| Credenciais de acesso ao sistema | Letícia | `[ ]` |
| Node.js instalado na máquina do colaborador (`node -v`) | Colaborador | `[ ]` |
| **JSON da UI exportado da extensão do Chrome** | Letícia | `[ ]` |

> **Como capturar o JSON da UI:**
> 1. Abrir a tela de Reembolso no Chrome com a extensão ativa
> 2. Capturar todos os elementos (campos, botões, tabelas, abas, modais)
> 3. Exportar o JSON pela extensão
> 4. Salvar como `reembolso-ui.json` em `DEMO/ui-elements/`
> 5. Consultar `DEMO/ui-elements/COMO-USAR.md` para o formato esperado

---

## 5. Cronograma — 10 Dias Úteis

```
SEMANA 1 — ESCRITA E DADOS
──────────────────────────────────────────────────────────
Dia 1  Qua 08/04   Configuração do ambiente
                   • Node.js, Playwright instalados
                   • Projeto aberto no Cursor
                   • Telas e escopo alinhados com o time

Dia 2  Qui 09/04   GherkinFlow — gerar cenários BDD
                   • Analisar telas com o agente GherkinFlow
                   • Arquivo .md salvo e lido pelo colaborador

Dia 3  Sex 10/04   Revisão dos cenários  ◀ CHECKPOINT 1
                   • Revisar o .md em conjunto
                   • Ajustar ou complementar cenários faltantes
                   • Aprovação para seguir para a Etapa 2

Dia 4  Seg 13/04   DataForge — gerar massa de dados
                   • Gerar o script SQL referenciando os cenários
                   • Arquivo .sql salvo e conferido

Dia 5  Ter 14/04   Revisão da massa de dados  ◀ CHECKPOINT 2
                   • Revisar o .sql em conjunto
                   • Confirmar coerência com os cenários
                   • Aprovação para seguir para a Etapa 3

SEMANA 2 — AUTOMAÇÃO E ENTREGA
──────────────────────────────────────────────────────────
Dia 6  Qua 15/04   Playwright — gerar testes com a IA
                   • Configurar playwright.config.ts
                   • Gerar arquivo .spec.ts para as telas

Dia 7  Qui 16/04   Rodar testes e corrigir falhas
                   • Executar: npx playwright test --headed
                   • Investigar erros com ajuda da IA

Dia 8  Sex 17/04   Estabilização  ◀ CHECKPOINT 3  +  BUFFER
                   • Happy path passando com sucesso
                   • Dia livre para imprevistos ou ajustes

Dia 9  Seg 20/04   Evidências
                   • Rodar suíte completa
                   • Gerar relatório HTML
                   • Tirar screenshots para apresentação

Dia 10 Ter 21/04   Ensaio da apresentação
                   • Ensaiar roteiro com o time
                   • Organizar material final

──────────────────────────────────────────────────────────
ENTREGA  Qua 22/04  APRESENTAÇÃO FINAL
──────────────────────────────────────────────────────────
```

---

## 6. Roteiro da Apresentação Final (22/04)

| # | O que mostrar | Tempo estimado |
|---|--------------|:--------------:|
| 1 | Contextualizar o projeto e as telas testadas | 3 min |
| 2 | Mostrar os cenários BDD gerados pela IA (arquivo `.md`) | 5 min |
| 3 | Mostrar a massa de dados SQL e a relação com os cenários | 5 min |
| 4 | Rodar os testes ao vivo com o browser visível | 8 min |
| 5 | Abrir o relatório HTML com os resultados | 5 min |
| 6 | Demonstrar uma pergunta à IA em tempo real | 4 min |
| | **Total** | **~30 min** |

---

## 7. Riscos e Mitigações

| Risco | Prob. | Impacto | Mitigação |
|-------|:-----:|:-------:|-----------|
| Schema do banco indisponível no início | Média | Alto | Levantar na semana anterior — está nos pré-requisitos |
| Testes falham por instabilidade do ambiente | Média | Médio | Mockar chamadas de API nos testes de apresentação (`page.route()`) |
| Seletores gerados são frágeis e quebram | Média | Médio | Pedir à IA seletores por `role`, `label` ou `data-testid` |
| Colaborador trava em erro de terminal | Alta | Médio | Copiar o erro completo e perguntar à IA |
| Cenários não cobrem fluxos importantes | Baixa | Médio | Revisar no Checkpoint 1 e solicitar complemento à IA |

---

## 8. Critérios de Sucesso

```
[ ] Cenários BDD gerados e revisados no Checkpoint 1
[ ] Massa de dados SQL gerada e coerente com os cenários
[ ] Testes Playwright gerados para as telas escolhidas
[ ] Happy path (fluxo principal) passando nos testes automatizados
[ ] Evidências coletadas (screenshots + log do terminal)
[ ] Apresentação realizada em 22/04
```

---

## 9. Checklist de Evidências para a Apresentação

```
[ ] DEMO/cenarios-bdd/REEMBOLSO-BDD-vN.md (cenários BDD)
[ ] DEMO/massa-dados/REEMBOLSO-MASSA-vN.sql (massa de dados)
[ ] DEMO/ui-elements/reembolso-ui.json (JSON da UI exportado da extensão)
[ ] DEMO/automacao/reembolso/reembolso.spec.ts (testes gerados)
[ ] Screenshot do terminal com testes aprovados e logs detalhados
[ ] (Bônus) Gravação do browser sendo controlado automaticamente
```

---

## 10. Registro de Execução

| Fase | Data de conclusão | Executado por | Revisado |
|------|:-----------------:|:-------------:|:--------:|
| Configuração do ambiente | | | `[ ]` |
| GherkinFlow — cenários BDD | | | `[ ]` |
| DataForge — massa de dados | | | `[ ]` |
| Playwright — testes gerados | | | `[ ]` |
| Testes rodando (happy path) | | | `[ ]` |
| Evidências coletadas | | | `[ ]` |
| Apresentação realizada | | | `[ ]` |

---

## 11. Prompts Utilizados

> Prompts prontos para uso. Copiar e colar no chat do Cursor.

---

### Prompt A — Demo Completa E2E (10 min) ← USE ESTE PARA PRÉ-VENDAS

```
demo e2e reembolso

Execute a jornada completa de qualidade assistida por IA para a tela de Reembolso do Fourmakers v2, sem pausas entre etapas:

ETAPA 1 — GherkinFlow
Leia os arquivos src/presentation/pages/Reembolso.tsx e src/presentation/pages/InserirReembolso.tsx.
Execute internamente (sem exibir no output): Análise → Mapa de Visibilidade → Geração.
Modo: Curado (13–18 cenários por tela). Tipos: Positivo, Negativo e Regressivo.
Salve em DEMO/cenarios-bdd/ com próxima versão vN.

O arquivo gerado deve ter APENAS 3 blocos, nesta ordem:
1. # Título (uma linha H1)
2. Tabela de Sumário (Tipo | Qtd, com ✅ ❌ 🔁 e Total)
3. Tabelas BDD por tela (## H2 + tabela de 7 colunas)

NÃO incluir: metadados, Passo A/B/C, Contagem inline, Sumário Final, reg-notes, nenhum texto entre blocos.

ETAPA 2 — DataForge
Imediatamente após a Etapa 1, sem pedir confirmação.
Leia DEMO/banco-de-dados/MODELO-DADOS.sql e DEMO/banco-de-dados/DICIONARIO-DADOS.sql.
Use os cenários BDD gerados como fonte das regras de negócio — cada cenário dita qual estado de dados criar.
Gere massa de dados SQL realista e coerente, referenciando cada cenário nos comentários.
Salve em DEMO/massa-dados/ com próxima versão vN.
Exiba o sumário com dialeto, tabelas populadas e total de registros.

ETAPA 3 — Playwright
Imediatamente após a Etapa 2, sem pedir confirmação.
Leia os 3 inputs de arquivo (não peça ao usuário):
- DEMO/ui-elements/reembolso-ui.json → fonte dos seletores reais da tela (exportado da extensão do Chrome)
- DEMO/cenarios-bdd/ (mais recente) → IDs e estrutura dos cenários
- DEMO/massa-dados/ (mais recente) → dados de cada cenário
Crie os 3 arquivos:
- DEMO/automacao/reembolso/playwright.config.ts
- DEMO/automacao/reembolso/reembolso.data.ts
- DEMO/automacao/reembolso/reembolso.spec.ts
Um test() por cenário Gherkin, mapeado pelo ID (ex: [R-01]).
Seletores derivados do JSON da UI: getByTestId → getByRole → getByLabel → getByText.
Nunca inventar seletores — usar apenas o que o JSON fornece.
Logs obrigatórios com emojis em cada passo (🧪 → ✅ 🏁).
Ao final, exiba o sumário com contagem de testes gerados e o comando para executar.
```

---

### Prompt B — Apenas GherkinFlow (Etapa 1 isolada)

```
Atue como GherkinFlow Agent.
Analise as telas de Reembolso do Fourmakers v2:
- src/presentation/pages/Reembolso.tsx
- src/presentation/pages/InserirReembolso.tsx

Execute: Análise → Mapa de Visibilidade → Geração de cenários BDD em português.
Modo Curado: 13–18 cenários por tela. Tipos: Positivo, Negativo, Regressivo.
Salve em DEMO/cenarios-bdd/ com próxima versão vN.
```

---

### Prompt C — Apenas DataForge (Etapa 2 isolada)

```
Atue como DataForge Agent.
Leia os 3 inputs:
1. DEMO/banco-de-dados/MODELO-DADOS.sql
2. DEMO/banco-de-dados/DICIONARIO-DADOS.sql
3. DEMO/cenarios-bdd/ (arquivo mais recente)

Use os cenários Gherkin como fonte das regras de negócio — cada cenário dita qual estado de dados criar.
Gere massa de dados SQL realista para os cenários de Reembolso.
Distribua: 70% estados finais, 30% transicionais.
Referencie cada cenário Gherkin nos comentários do SQL (ex: -- Cenário R-01).
Salve em DEMO/massa-dados/ com próxima versão vN.
```

---

### Prompt D — Apenas Playwright (Etapa 3 isolada)

```
Gere a automação Playwright para a tela de Reembolso.
Leia os 3 inputs de arquivo:
- DEMO/ui-elements/reembolso-ui.json → fonte dos seletores reais (extensão do Chrome)
- DEMO/cenarios-bdd/ (arquivo mais recente) → IDs e estrutura dos cenários
- DEMO/massa-dados/ (arquivo mais recente) → dados de cada cenário

Crie os arquivos:
- DEMO/automacao/reembolso/playwright.config.ts
- DEMO/automacao/reembolso/reembolso.data.ts
- DEMO/automacao/reembolso/reembolso.spec.ts

Um test() por cenário Gherkin. Logs com emojis em cada passo.
Seletores derivados do JSON da UI: getByTestId → getByRole → getByLabel → getByText. Nunca CSS frágil.
```

---

## 12. Assinatura

| | Nome | Data |
|--|------|------|
| **Letícia** | | |
| **Colaborador** | | |

---

*Versão 3.0 · 08/04/2026 · Prazo: 22/04/2026*
