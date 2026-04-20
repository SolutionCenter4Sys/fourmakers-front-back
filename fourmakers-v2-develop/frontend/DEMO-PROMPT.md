# 🎬 Script da Demo Pré-Vendas — FourMakers

> Cole o prompt abaixo no Cursor Chat para disparar a demo completa E2E.
> O agente executará as 3 etapas automaticamente, sem interrupção.

---

## ✅ CHECKLIST PRÉ-DEMO (fazer antes de apresentar)

```
[ ] 1. Exportar o JSON da tela de Reembolso pela extensão do Chrome
[ ]    └ Salvar como: DEMO/ui-elements/reembolso-ui.json
[ ] 2. Confirmar que DEMO/banco-de-dados/MODELO-DADOS.sql existe
[ ] 3. Confirmar que DEMO/banco-de-dados/DICIONARIO-DADOS.sql existe
[ ] 4. Confirmar acesso à homolog: https://app.dev.fourmakers.io
[ ] 5. Abrir o Cursor com o projeto FourMakers v2
[ ] 6. Abrir um novo chat (Ctrl+L ou Cmd+L)
```

---

## ▶ PROMPT DE ATIVAÇÃO

> Os arquivos são anexados com `@` diretamente no prompt — o agente não precisa procurá-los.
> Isso elimina o tempo de busca e reduz alucinação (o modelo lê o código real, não infere).

Cole o bloco abaixo inteiro no chat. Os `@` funcionam no Cursor Chat e pré-carregam os arquivos:

```
demo e2e reembolso

@src/presentation/pages/Reembolso.tsx
@src/presentation/pages/InserirReembolso.tsx
@DEMO/banco-de-dados/MODELO-DADOS.sql
@DEMO/banco-de-dados/DICIONARIO-DADOS.sql
@DEMO/ui-elements/reembolso-ui.json

Execute a jornada completa de qualidade assistida por IA para a tela de Reembolso do Fourmakers v2, sem pausas entre etapas:

ETAPA 1 — GherkinFlow
Os arquivos TSX acima já estão anexados — use-os diretamente, sem buscar novamente.
Execute internamente (sem exibir no output): Análise → Mapa de Visibilidade → Geração.
Gere TODOS os cenários que o código suporta. Tipos: Positivo, Negativo e Regressivo.
Salve em DEMO/cenarios-bdd/ com próxima versão vN.

O arquivo gerado deve ter APENAS 3 blocos, nesta ordem:
1. # Título (uma linha H1)
2. Tabela de Sumário (Tipo | Qtd, com ✅ ❌ 🔁 e Total)
3. Tabelas BDD por tela (## H2 + tabela de 7 colunas)

NÃO incluir: metadados, Passo A/B/C, Contagem inline, Sumário Final, reg-notes, nenhum texto entre blocos.

ETAPA 2 — DataForge
Imediatamente após a Etapa 1, sem pedir confirmação.
Os arquivos MODELO-DADOS.sql e DICIONARIO-DADOS.sql já estão anexados — use-os diretamente.
Use os cenários BDD gerados como fonte das regras de negócio — cada cenário dita qual estado de dados criar.
Gere massa de dados SQL realista. Formato mínimo: cabeçalho de 2 linhas (título + dialeto) + INSERTs com -- ID · Tipo Cenário.
Salve em DEMO/massa-dados/ com próxima versão vN.
No chat, liste os cenários atendidos.

ETAPA 3 — Playwright
Imediatamente após a Etapa 2, sem pedir confirmação.
O arquivo reembolso-ui.json já está anexado — use-o diretamente para os seletores.
Leia também DEMO/cenarios-bdd/ e DEMO/massa-dados/ (arquivos mais recentes).
Crie os 3 arquivos:
- DEMO/automacao/reembolso/playwright.config.ts
- DEMO/automacao/reembolso/reembolso.data.ts
- DEMO/automacao/reembolso/reembolso.spec.ts
Um test() por cenário Gherkin, mapeado pelo ID (ex: [R-01]).
Seletores derivados do JSON da UI: getByTestId → getByRole → getByLabel → getByText.
Nunca inventar seletores — usar apenas o que o JSON fornece.
Logs obrigatórios com emojis em cada passo (🧪 → ✅ 🏁).
baseURL padrão: https://app.dev.fourmakers.io (homolog — sempre testar lá).
```

---

## 📋 Roteiro da Demo (~10 minutos)

| Tempo | O que acontece | O que você fala |
|-------|----------------|-----------------|
| 0:00 | Cola o prompt | "Vou mostrar a jornada completa — da escrita do cenário ao teste rodando — em menos de 10 minutos." |
| 0:45 | GherkinFlow analisa as telas | "O agente leu o código e está mapeando as regras de negócio..." |
| 3:30 | Tabela BDD aparece | "Cenários gerados: positivos, negativos e regressivos — em linguagem de negócio." |
| 4:00 | DataForge inicia | "Agora ele lê o banco de dados e cria a massa de teste coerente com cada cenário." |
| 6:30 | SQL gerado | "Cada INSERT está vinculado ao cenário que ele suporta." |
| 7:00 | Playwright inicia | "Com o JSON da tela e os cenários prontos, ele monta o código de automação." |
| 9:30 | Spec.ts gerado | "Logs detalhados, seletores reais, um teste por cenário — pronto para rodar." |
| 10:00 | Sumário final | "Da escrita à automação — em menos de 10 minutos." |

---

## 🌐 Executar os Testes em Homolog

Os testes são gerados para rodar em **`https://app.dev.fourmakers.io`** por padrão.

### Via VS Code Tasks (Ctrl+Shift+P → "Run Task"):

| Task | Ambiente |
|------|----------|
| `⚡ Rodar — Testes Playwright (Homolog)` | `https://app.dev.fourmakers.io` |
| `⚡ Rodar — Testes Playwright (Local)` | `http://localhost:5173` |

### Via terminal:

```bash
# Homolog (padrão da demo)
npx playwright test --reporter=list

# Local (sobrescrever baseURL)
BASE_URL=http://localhost:5173 npx playwright test --reporter=list

# Homolog explícito
BASE_URL=https://app.dev.fourmakers.io npx playwright test --reporter=list
```

---

## 🗂 Onde ficam os arquivos gerados

| Arquivo | Caminho |
|---------|---------|
| Cenários BDD | `DEMO/cenarios-bdd/REEMBOLSO-BDD-vN.md` |
| Massa de dados SQL | `DEMO/massa-dados/REEMBOLSO-MASSA-vN.sql` |
| Config Playwright | `DEMO/automacao/reembolso/playwright.config.ts` |
| Dados de teste TS | `DEMO/automacao/reembolso/reembolso.data.ts` |
| Testes automatizados | `DEMO/automacao/reembolso/reembolso.spec.ts` |

---

## ⚡ Comandos rápidos

```bash
# Rodar testes em homolog
npx playwright test --reporter=list

# Ver relatório interativo após a execução
npx playwright show-report

# Coletar seletores em homolog (para atualizar reembolso-ui.json)
npx playwright codegen https://app.dev.fourmakers.io/reembolso
```
