---
name: cypress-agent
description: "Agente Cypress — gera testes E2E automatizados a partir do BDD, dos dados do DataForge e do JSON da UI, contra o backend real. Use quando o usuário disser: gerar testes, Etapa 3 da demo, automação Cypress."
---

# 🎬 Cypress Agent
> Especialista em geração de testes E2E com seletores reais da UI, dados realistas e backend real — nunca seletores inventados, nunca banco fictício.

---

## O que ele faz

Lê o JSON da UI (exportado da extensão do Chrome), os cenários BDD e os dados do DataForge, e gera:
- `cypress/e2e/reembolso.cy.js` — um `it()` por cenário Gherkin, com logs detalhados

Os testes rodam contra o **backend real** usando login OTP via `cy.loginFourMakers()`.

---

## Como ativar no Cursor Chat

Digite qualquer uma dessas frases:

```
automatizar tela
gerar testes
automação reembolso
cypress reembolso
```

---

## Inputs que ele consome

| Arquivo | Para que serve |
|---|---|
| `DEMO/ui-elements/reembolso-ui.json` | **Fonte dos seletores reais** |
| `DEMO/cenarios-bdd/REEMBOLSO-BDD-vN.md` | IDs e estrutura dos cenários |
| `DEMO/automacao/reembolso/reembolso.data.js` | Dados de teste do DataForge |
| `cypress/support/commands/fourmakers-auth.js` | Helper de login OTP |

> ⚠️ Se o JSON da UI não existir: **"Exporte da extensão do Chrome e salve como reembolso-ui.json"**

---

## Output que ele gera

```
cypress/e2e/reembolso.cy.js
```

Exemplo de teste gerado:

```javascript
it('[R-01] Colaborador solicita reembolso com dados completos', () => {
  console.log('\n🧪 TESTANDO: [R-01] — Solicitação completa');
  console.log('   → Navegando para Inserir Reembolso...');
  cy.visit('/inserir-reembolso');

  console.log('   → Preenchendo Objetivo...');
  cy.get('#objetivo').type(dadosR01.objetivo);
  // ... preenche demais campos

  console.log('   → Enviando solicitação...');
  cy.contains('Enviar solicitações').click();

  console.log('   ✅ Solicitação enviada via backend real');
  console.log('🏁 [R-01] CONCLUÍDO');
});
```

---

## Fluxo real

```
cy.loginFourMakers()          → OTP real → JWT no localStorage
cy.visit('/inserir-reembolso') → formulário real
preenche com dados DataForge   → dados realistas
cy.contains('Enviar').click() → POST /api/.../Inserir → backend real → banco real
cy.visit('/reembolso')        → valida no dashboard
```

---

## Prioridade de seletores

| Prioridade | Seletor | Quando usar |
|:---:|---|---|
| 1 | `cy.get('[data-testid="..."]')` | JSON tem `testId` |
| 2 | `cy.contains('Texto')` | Botão/label com texto único |
| 3 | `cy.get('#id')` | Elemento com id |
| 4 | `cy.get('[role="..."]')` | Elemento com role |

---

## Para rodar após a demo

```bash
npx cypress run --spec "cypress/e2e/reembolso.cy.js" --browser chrome
```

---

## Arquivo de definição completo

> `.cursor/skills/playwright-agent/SKILL.md`
