# Divergências — Atribuição de Recrutador

## [Design]

### Divergência 1 — Scroll do mouse
**De:** Documentação de design espera que "Scroll do mouse deve rolar a lista de opções dentro do popover".  
**Para:** Na implementação anterior, o scroll do mouse não funcionava (apenas a barra de rolagem da funcionalidade).  
**Explicação:** Comportamento conhecido com Popover dentro de Dialog (Radix/cmdk): gestão de foco e RemoveScroll impediam o wheel na lista. Correção: `Popover modal` + `ScrollArea` no `CommandList`.

### Divergência 2 — Autocomplete ao pesquisar
**De:** "Digitação no campo de busca deve filtrar a lista em tempo real (autocomplete), com busca case-insensitive e tolerante a acentos".  
**Para:** O Command já tinha `shouldFilter={true}` com filtro padrão do cmdk, que pode ser sensível a acentos e a maiúsculas.  
**Explicação:** Melhoria de usabilidade: filtro customizado com `normalizarParaBusca` para garantir tolerância a acentos e case-insensitive.

---

## [Negócio]
Nenhuma divergência encontrada entre documentação de negócio e código (regras de atribuição, Use Cases, pré-seleção).

## [Desenvolvimento]
Nenhuma divergência encontrada entre documentação de desenvolvimento e código (camadas, Use Cases, sem @data/api na presentation).
