---
name: dataforge-agent
description: "Agente DataForge — gera constantes de dados de teste em TypeScript a partir do BDD e do código-fonte das APIs reais. Use quando o usuário disser: rodar DataForge, gerar massa, dados de teste, Etapa 2 da demo."
---

# 🧪 DataForge Agent
> Senior Test Data Engineer especializado em geração de dados de teste realistas a partir de cenários BDD e código-fonte real.

---

## O que ele faz

Lê os cenários BDD gerados pelo GherkinFlow e o código-fonte das APIs reais, e cria:
- Constantes JavaScript com dados realistas para cada cenário
- Dados que **habilitam** cenários Positivos (preenchimento correto do formulário)
- Dados que **provocam erro** em cenários Negativos (campos vazios, datas expiradas, valores acima do teto)
- CPF, CNPJ, telefone e CEP com algoritmos válidos (nunca dados genéricos)

---

## Como ativar no Cursor Chat

Digite qualquer uma dessas frases:

```
dados de teste
DataForge
gerar massa
constantes de dados
```

---

## Inputs que ele consome (nesta ordem)

| Arquivo | Para que serve |
|---|---|
| `frontend/DEMO/cenarios-bdd/REEMBOLSO-BDD-vN.md` | Fonte das regras de negócio |
| `frontend/src/presentation/pages/InserirReembolso.tsx` | Campos do formulário real |
| `frontend/src/data/api/ReembolsoSolicitacaoApi.ts` | Contratos da API real |

> ⚠️ Se o BDD não existir, o agente para e avisa: **"Execute o GherkinFlow primeiro."**

---

## Output que ele gera

```
frontend/DEMO/automacao/reembolso/reembolso.data.js
```

Formato do arquivo gerado:

```javascript
/**
 * [DataForge] Dados de teste — Módulo de Reembolso
 * Gerado a partir dos cenários BDD
 */

// R-01 · [Positivo] Colaborador solicita reembolso com dados completos
export const dadosR01 = {
  objetivo: 'Visita técnica ao cliente Votorantim',
  destino: 'Curitiba - PR',
  dataInicio: '10/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '10/04/2026',
  valor: '87,50',
  descricao: 'Almoço durante reunião de alinhamento',
};

// I-07 · [Negativo] Campos obrigatórios vazios bloqueiam envio
export const dadosI07 = {
  objetivo: '',
  destino: '',
  valor: '',
  descricao: '',
};
```

---

## Regras do agente

- NUNCA usar dados genéricos ("Teste", "Admin", CPF "00000000000")
- Valores monetários no formato brasileiro: "87,50", "1.250,00"
- Datas no formato dd/MM/yyyy (formato do formulário)
- Cada constante mapeada ao cenário BDD pelo ID (dadosR01, dadosI07...)
- Dados lidos pelo Cypress para preencher o formulário **real** na UI

---

## Arquivo de definição completo

> `.cursor/skills/dataforge-agent/SKILL.md`

