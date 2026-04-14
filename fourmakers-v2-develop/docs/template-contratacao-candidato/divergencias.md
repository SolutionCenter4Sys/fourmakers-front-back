# Template de Contratação do Candidato – Divergências (Documentação vs Código)

## [Categoria: Desenvolvimento]

### Divergência 1 – Performance do seletor de profissionais

**De:**  
Documentação não descrevia causa do flicker (aparecer e sumir) do autocomplete de profissionais.

**Para:**  
Implementado: uso do Use Case em ref (`useCaseRef`) no `ColaboradorSearchField` e `fetchColaboradores` com dependência apenas em `[token]`, para que o efeito de busca não rode a cada re-render do pai. Comportamento esperado: popover estável, sem fechar/abrir ou alternar “Carregando…”/lista de forma indevida.

**Explicação:**  
Problema era de implementação (referência instável do Use Case e do callback), não de especificação. Documentação de desenvolvimento atualizada para registrar o padrão (Use Case via ref para estabilizar callback e evitar chamadas redundantes).

---

## [Categoria: Design]

### Divergência 2 – Placeholder do Analista de R&S

**De:**  
Imagem de referência com placeholder “Pesquise o nome do anali” (texto truncado).

**Para:**  
Código usa `placeholder` configurável; na página é passado “Buscar por nome ou código…” (e dentro do popover também “Buscar por nome ou código…”).

**Explicação:**  
Pode ser intencional (texto completo). Se o produto exigir exatamente “Pesquise o nome do analista” no trigger, basta ajustar a prop `placeholder` na página para o Analista de R&S.

---

### Divergência 3 – Hooks após returns condicionais (corrigido)

**De:**  
`useCallback` de `onSelectAnalista` e `onSelectSuperior` estavam após os `return` condicionais (`if (loading)`, `if (!template && !loading)`), causando "Rendered more hooks than during the previous render".

**Para:**  
Os dois `useCallback` foram movidos para antes de qualquer `return` condicional (logo após `handleRemoverOutroGrupo`), garantindo a mesma quantidade de hooks em toda renderização.

**Explicação:**  
Regra do React: hooks devem ser chamados na mesma ordem em todo render; não podem estar após early returns.

---

A documentação técnica completa (regras de negócio, contratos, endpoints, paridade com o React) está em **DOCUMENTACAO_TECNICA.md**. Nenhuma outra divergência crítica pendente.
