# UI Elements — JSON da Extensão do Chrome

## O que é isso?

Esta pasta guarda o JSON exportado pela extensão do Chrome que captura os elementos da tela.
O agente de automação (Etapa 3) lê este arquivo para usar **seletores reais** nos testes — nunca seletores chutados.

---

## Como gerar o arquivo

1. Abra a tela de Reembolso no browser com a extensão ativa
2. Capture todos os elementos da tela (formulário, botões, tabelas, modais)
3. Exporte o JSON pela extensão
4. Salve como `reembolso-ui.json` **nesta pasta** (`DEMO/ui-elements/`)

---

## Formato esperado

O agente aceita qualquer JSON com estrutura de elementos de UI. Exemplos de campos úteis:

```json
{
  "tela": "Reembolso",
  "url": "/reembolso",
  "elementos": [
    {
      "tipo": "input",
      "label": "Valor",
      "testId": "input-valor",
      "role": "textbox",
      "obrigatorio": true
    },
    {
      "tipo": "button",
      "label": "Adicionar ao carrinho",
      "testId": "btn-adicionar",
      "role": "button"
    },
    {
      "tipo": "select",
      "label": "Tipo de despesa",
      "testId": "select-tipo-despesa",
      "role": "combobox"
    },
    {
      "tipo": "table",
      "label": "Lista de reembolsos",
      "testId": "tabela-reembolsos",
      "colunas": ["Objetivo", "Destino", "Período", "Valor", "Status"]
    },
    {
      "tipo": "tab",
      "label": "Meus Reembolsos",
      "role": "tab"
    },
    {
      "tipo": "modal",
      "label": "Confirmar envio",
      "testId": "modal-confirmar-envio"
    }
  ]
}
```

> O formato exato depende da extensão usada. O agente interpreta o JSON e extrai os seletores disponíveis.

---

## Prioridade de seletores (do mais robusto ao menos)

| Prioridade | Seletor Playwright | Quando usar |
|:---:|---|---|
| 1 | `getByTestId('...')` | Quando o JSON tiver `testId` |
| 2 | `getByRole('...', { name: '...' })` | Quando tiver `role` + `label` |
| 3 | `getByLabel('...')` | Para inputs com label visível |
| 4 | `getByText('...')` | Para textos únicos na tela |

---

## Arquivos desta pasta

| Arquivo | Descrição |
|---|---|
| `reembolso-ui.json` | JSON exportado da extensão — **fonte dos seletores** |

> Versionar: sim. Um JSON por tela. Sem datas no nome.
