---
name: playwright-agent
description: "Agente Playwright — gera testes automatizados a partir do BDD, da massa SQL e do JSON da UI. Use quando o usuário disser: gerar testes, Etapa 3 da demo, automação Playwright."
---

# 🎬 Playwright Agent
> Especialista em geração de testes E2E com seletores reais da UI — nunca seletores inventados.

---

## O que ele faz

Lê o JSON da UI (exportado da extensão do Chrome), os cenários BDD e a massa SQL, e gera:
- `playwright.config.ts` — configuração do framework
- `reembolso.data.ts` — constantes TypeScript com dados de teste
- `reembolso.spec.ts` — um `test()` por cenário Gherkin, com logs detalhados

---

## Como ativar no Cursor Chat

Digite qualquer uma dessas frases:

```
automatizar tela
json da tela
gerar testes
automação reembolso
logs de teste
```

---

## Inputs que ele consome

| Arquivo | Para que serve |
|---|---|
| `DEMO/ui-elements/reembolso-ui.json` | **Fonte dos seletores reais** |
| `DEMO/cenarios-bdd/REEMBOLSO-BDD-vN.md` | IDs e estrutura dos cenários |
| `DEMO/massa-dados/REEMBOLSO-MASSA-vN.sql` | Dados usados em cada cenário |

> ⚠️ Se o JSON da UI não existir: **"Exporte da extensão do Chrome e salve como reembolso-ui.json"**

---

## Output que ele gera

```
DEMO/automacao/reembolso/playwright.config.ts
DEMO/automacao/reembolso/reembolso.data.ts
DEMO/automacao/reembolso/reembolso.spec.ts
```

Exemplo de teste gerado:

```typescript
test('[R-01] Colaborador visualiza apenas Meus Reembolsos', async ({ page }) => {
  console.log('\n🧪 TESTANDO: [R-01] — Acesso padrão');
  console.log('   → Autenticando como colaborador comum...');
  await page.getByRole('tab', { name: 'Meus Reembolsos' }).click();
  await expect(page.getByRole('tab', { name: 'Gestão ADM' })).not.toBeVisible();
  console.log('   ✅ Abas restritas não exibidas');
  console.log('🏁 [R-01] CONCLUÍDO');
});
```

---

## Prioridade de seletores

| Prioridade | Seletor | Quando usar |
|:---:|---|---|
| 1 | `getByTestId('...')` | JSON tem `testId` |
| 2 | `getByRole('...', { name: '...' })` | JSON tem `role` + `label` |
| 3 | `getByLabel('...')` | Input com label visível |
| 4 | `getByText('...')` | Texto único na tela |

---

## Para rodar após a demo

```bash
npx playwright test --reporter=list
```

---

## Arquivo de definição completo

> `.cursor/skills/playwright-agent/SKILL.md`
