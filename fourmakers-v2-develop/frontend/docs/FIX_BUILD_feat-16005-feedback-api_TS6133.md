# Fix build — feat/16005-feedback-api (TS6133)

**Erro no pipeline:**  
`src/presentation/hooks/useFeedback360.ts(22,7): error TS6133: 'MIN_CHARS_PARA_BUSCA' is declared but its value is never read.`

**Causa:** No refactor do commit 6510d2e (busca/paginação de colaboradores), a constante `MIN_CHARS_PARA_BUSCA` foi declarada no hook `useFeedback360.ts` mas nunca utilizada — a checagem de “mínimo de caracteres” fica na página `Feedback360.tsx` (constante `MIN_CHARS_BUSCA_COLAB`).

**Correção:** Remover a linha da constante não utilizada no hook.

Em `src/presentation/hooks/useFeedback360.ts`, **remover** a linha:

```ts
const MIN_CHARS_PARA_BUSCA = 2;
```

Ou seja, deixar apenas:

```ts
const COLAB_PAGE_SIZE = 50;
const COLAB_INITIAL_LIMIT = 100;

export function useFeedback360() {
```

Após essa alteração, o `tsc` deixa de acusar TS6133 e o build conclui com sucesso.
