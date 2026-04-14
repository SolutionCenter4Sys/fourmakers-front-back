# Simulador — Lógica de Custo Hora Vaga e Remuneração Vaga (168h)

Este documento descreve como os campos **Custo Hora Vaga** e **Remuneração Vaga (168h)** são preenchidos e a regra de **limpeza do feedback** ao trocar vaga ou candidato.

- **Criado em:** 13/03/2026  
- **Última atualização:** 13/03/2026 (inclusão do bloco de controle conforme ORIENTACAO_DOCUMENTACAO_TECNICA_FEATURES.md)

---

## 1. Fonte dos valores (apenas API da vaga)

Os campos **Custo Hora Vaga** e **Remuneração Vaga (168h)** são **somente leitura** e vêm **exclusivamente** da API **ObterVagaRecrutamentoPorId** (`api/Vaga/ObterVagaRecrutamentoPorId`), pelo dado **`custoProfissional`** retornado na vaga.

- **Custo Hora Vaga** = `custoProfissional` (valor por hora da vaga).
- **Remuneração Vaga (168h)** = `custoProfissional × 168` (conta simples; constante `HOURS_PER_MONTH = 168` em `shared/utils/calculations.ts`).

Esses valores **não são alterados nem recalculados** no frontend. Eles só mudam quando o usuário **seleciona outra vaga** e os novos detalhes da vaga são carregados.

- **Salvar Dados** não altera esses campos.
- **Gerar Cálculo** não altera esses campos (o resultado do cálculo não sobrescreve o custo da vaga na tela).

---

## 2. Implementação no código

- No **hook** `useSimulator.ts`, `jobHourlyCost` é derivado diretamente de `vagaDetails?.custoProfissional` (não de `formData.grossSalary`):

```ts
const jobHourlyCost =
  typeof vagaDetails?.custoProfissional === 'number' ? vagaDetails.custoProfissional : 0;
```

- Na tela (`Simulator.tsx`):
  - **Custo Hora Vaga** = `jobHourlyCost`
  - **Remuneração Vaga (168h)** = `jobHourlyCost * HOURS_PER_MONTH`

`vagaDetails` é preenchido em `fetchVagaDetails.fulfilled` no `simulatorSlice` quando os detalhes da vaga são carregados (incluindo `custoProfissional`).

---

## 3. Feedback em verde/vermelho (validações do cálculo)

As mensagens de feedback em verde ou vermelho próximas dos campos (ex.: CLT, Vale Refeição, Mobilidade, etc.) vêm de **`calculationValidations`** e **`calculationResult`** (resultado do **Gerar Cálculo**).

**Regra:** ao trocar **vaga** ou **candidato/candidatura** nos Identificadores, esse feedback é **limpo** e oculto até que um novo **Gerar Cálculo** seja realizado.

- Ao disparar **`setSelectedVagaId`**, **`setSelectedCandidateId`** ou **`setSelectedCandidaturaId`**, o slice zera:
  - `calculationResult = null`
  - `calculationValidations = INITIAL_VALIDATIONS` (todas as mensagens em branco)

Assim, não permanece feedback de um cálculo anterior ao selecionar outro candidato ou outra vaga.

---

## 4. Referências no código

- **Hook:** `src/presentation/hooks/useSimulator.ts` — `jobHourlyCost` derivado de `vagaDetails?.custoProfissional`.
- **Slice:** `src/app/store/slices/simulatorSlice.ts` — `fetchVagaDetails.fulfilled` (preenche `vagaDetails` com `custoProfissional`); reducers `setSelectedVagaId`, `setSelectedCandidateId`, `setSelectedCandidaturaId` (limpam `calculationResult` e `calculationValidations`).
- **Constante:** `src/shared/utils/calculations.ts` — `HOURS_PER_MONTH = 168`.
- **Tela:** `src/presentation/pages/Simulator.tsx` — exibição de `jobHourlyCost` e `jobHourlyCost * HOURS_PER_MONTH`; uso de `calculationValidations` para as mensagens de validação.
