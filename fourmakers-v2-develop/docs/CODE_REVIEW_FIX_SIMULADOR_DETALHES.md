# Code review: fix/simulador-detalhes

**Branch:** `fix/simulador-detalhes`  
**Escopo:** Simulador (`/simulador`) — botão "Detalhes" e fluxo "Gerar Cálculo".

---

## Objetivo da correção

- Botão **Detalhes** permanece desabilitado até o usuário clicar em **Gerar Cálculo** e a simulação ser concluída com sucesso (snackbar "Simulação realizada com sucesso").
- Habilitar no **primeiro** clique em Gerar Cálculo (não apenas no segundo).
- Desabilitar novamente se o usuário alterar vaga, candidato ou campos do formulário de cálculo; reabilitar somente após novo **Gerar Cálculo** bem-sucedido.

---

## Arquivos alterados

| Arquivo | Alteração |
|--------|-----------|
| `src/presentation/hooks/useSimulator.ts` | Ref `lastValidCalculationContextRef`, função `getCalculationContext()`, state `calculationSuccessVersion`, `useEffect` que invalida o ref quando o contexto muda, em `handleGenerateCalculation` no `.then()`: atualiza ref, `setCalculationSuccessVersion`, `toast.success(mensagem)`; retorno `detailsEnabled` considerando `calculationResult`, ref, contexto atual e `calculationSuccessVersion >= 0`. |
| `src/presentation/pages/Simulator.tsx` | Botão Detalhes com `disabled={!detailsEnabled}` e `title` explicativo (acessibilidade). |

---

## Auditoria de conformidade

### Design System e referência (`public/design-toolkit.md`)

- **Button:** Uso do componente `Button` do DS; estado desabilitado via `disabled={!detailsEnabled}`.
- **Feedback:** Toast (sonner) com mensagem do back ("Simulação realizada com sucesso") em caso de sucesso.
- **Tokens:** Nenhuma cor hardcoded introduzida; página já utiliza tokens em outras partes.
- **Acessibilidade:** `title` no botão Detalhes quando desabilitado ("Clique em Gerar Cálculo para habilitar") e quando habilitado ("Ver detalhes do cálculo").

### Arquitetura

- **Presentation:** Não há import de `@data/api`; o hook utiliza apenas o store (slices/actions) e tipos de domínio.
- **Fluxo:** Cálculo via `dispatch(calculateRemuneracaoTotal(...)).unwrap()`; resultado e mensagem tratados no `.then()`; ref e state atualizados após sucesso para habilitar o botão no mesmo ciclo.

### Comportamento e regras

- **Contexto de cálculo:** Serialização (vaga, candidato, candidatura e campos do payload) em `getCalculationContext()`; comparação com o valor armazenado no ref para decidir se o botão Detalhes permanece habilitado.
- **Invalidação:** `useEffect` zera `lastValidCalculationContextRef` apenas quando o contexto atual é **diferente** do armazenado (evita zerar logo após o sucesso do cálculo).
- **Re-render no primeiro clique:** `setCalculationSuccessVersion((v) => v + 1)` no `.then()` do cálculo força atualização da UI e faz `detailsEnabled` refletir o sucesso imediatamente.

---

## Resultado

- **Conformidade:** Atende ao Design System (componentes, tokens, feedback, acessibilidade) e à arquitetura (sem vazamento de camada na presentation).
- **Build:** `npm run build:homolog` executado com sucesso.
- **Preview:** Disponível em `http://localhost:8080` para validação.

---

*Auditoria realizada em Fev 2026 (fix/simulador-detalhes).*
