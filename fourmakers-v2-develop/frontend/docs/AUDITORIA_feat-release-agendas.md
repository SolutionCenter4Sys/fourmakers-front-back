# Auditoria: Feature Release (Novidades por tela) – feat/release-agendas

**Data:** 09/03/2026  
**Escopo:** Modal de novidades por tela, sino no PageHeader, conteúdos em `releaseContents.ts`  
**Referências:** `ARCHITECTURE.md`, `DESIGN_SYSTEM_AUDIT.md`, `.cursor/rules/common-patterns-to-avoid-*`

---

## 1. Resumo executivo

A feature de **release/novidades** (modal ao acessar uma tela + sino ao lado do título) foi auditada quanto à conformidade com **arquitetura**, **frontend** e **design system**. O resultado é **conformidade** com um ajuste aplicado: extração de lógica duplicada (menu → nome amigável) para `@shared/utils/releaseContentMenuUtils.ts` (DRY).

---

## 2. Arquitetura (Clean Architecture)

### 2.1 Regra de dependência

| Camada        | Importa de                    | Status |
|---------------|--------------------------------|--------|
| **Presentation** | `@app/store`, `@domain` (tipos), `@shared` | ✅ OK  |
| **Shared**      | `@shared` apenas (tipos, dados, utils); `releaseContentMenuUtils` importa tipo `MenuResource` de `@domain` (padrão já usado no projeto em outros utils) | ✅ OK  |
| **Domain**      | Não é usado pela feature (apenas tipo existente `MenuResource`) | ✅ OK  |
| **Data**        | Não é usado pela feature (sem chamadas HTTP) | ✅ OK  |

- **Presentation não importa `@data/api`:** Nenhum componente da feature faz requisição HTTP; dados vêm de `@shared/data/releaseContents` (estático) e de Redux (menu). ✅  
- **Shared não importa presentation/app:** Utils e tipos em `shared/` não dependem de camadas externas, exceto tipo de entidade em `releaseContentMenuUtils` (alinhado ao restante do projeto). ✅  

### 2.2 Camadas utilizadas

- **Presentation:** `ReleaseContentModal`, `ReleaseContentGate`, `ReleaseModalContext`, `PageHeader` (uso do contexto).  
- **App:** Uso de `useAppSelector` (state.menu) no Provider e no Gate – permitido para leitura de estado global.  
- **Shared:** Tipos (`releaseContent.ts`), dados estáticos (`releaseContents.ts`), utils (`releaseContentUtils.ts`, `releaseContentMenuUtils.ts`).  
- **Domain:** Apenas tipo `MenuResource` em util compartilhado (sem regras de negócio novas na domain).  
- **Data/Core:** Nenhuma API nem DI nova; sem uso de `httpClient`/`fetch` na feature. ✅  

### 2.3 Ajuste aplicado (DRY)

- **Problema:** `flattenMenuItems`, `formatMenuCodeAsName` e `getNomeMenuByMenuCode` estavam duplicados em `ReleaseModalContext.tsx` e `ReleaseContentGate.tsx`.  
- **Correção:** Criado `src/shared/utils/releaseContentMenuUtils.ts` com as três funções; Provider e Gate passam a importar `getNomeMenuByMenuCode` de `@shared/utils/releaseContentMenuUtils`.  

---

## 3. Frontend e Design System

### 3.1 Modais (`common-patterns-to-avoid-specific`, DESIGN_SYSTEM_AUDIT)

| Requisito            | Implementação                                                                 | Status |
|----------------------|-------------------------------------------------------------------------------|--------|
| **DialogTitle**      | `ReleaseContentModal`: título "Novidades fresquinhas – {nomeTela}"            | ✅     |
| **DialogDescription**| `DialogDescription` presente com `className="sr-only"` para acessibilidade     | ✅     |
| **Componentes DS**   | Dialog, DialogContent, DialogHeader, DialogFooter, Button de `@/components/ui`| ✅     |

### 3.2 Cores e tokens

- Uso de tokens do DS: `text-primaryText`, `text-secondaryText`, `bg-primary/10`, `border-borderSoft`, `bg-surfaceSubtle/50`, `text-muted-foreground`, `hover:text-foreground`.  
- Nenhuma cor Tailwind hardcoded (`bg-green-`, `text-red-`, `bg-blue-`, etc.). ✅  

### 3.3 Loading e ícones

- Não há estado de loading com ícone na feature (conteúdo estático + localStorage).  
- Não é usado `Loader2` nem `RefreshCw` para loading. ✅  
- Ícones: `Rocket`, `Play`, `Bell` (lucide-react) para UI, não para loading. ✅  

### 3.4 Acessibilidade

- Botão do sino: `aria-label="Ver novidades desta tela"`.  
- Ícones decorativos: `aria-hidden` onde aplicável.  
- Modal: `DialogTitle` + `DialogDescription` (sr-only) atendem ao padrão de modais acessíveis. ✅  

### 3.5 Estrutura de pastas

- **Tipos:** `src/shared/types/releaseContent.ts`  
- **Dados estáticos:** `src/shared/data/releaseContents.ts`  
- **Utils:** `src/shared/utils/releaseContentUtils.ts`, `src/shared/utils/releaseContentMenuUtils.ts`  
- **Componentes:** `src/presentation/components/common/` (modal, gate, context, PageHeader)  
- Alinhado à estrutura esperada (shared para tipos/dados/utils; presentation para UI). ✅  

---

## 4. Checklist final

| Item                                      | Status |
|-------------------------------------------|--------|
| Presentation não importa `@data/api`      | ✅     |
| Domain não importa app/data/presentation  | ✅     |
| Modais com DialogTitle + DialogDescription| ✅     |
| Cores via tokens (sem hardcode)           | ✅     |
| Sem Loader2/RefreshCw para loading       | ✅     |
| Sem fetch()/axios direto                  | ✅     |
| Lógica de menu centralizada (DRY)         | ✅ (após ajuste) |
| Acessibilidade (aria-label, sr-only)     | ✅     |

---

## 5. Conclusão

A feature **Release (novidades por tela)** está **em conformidade** com as políticas de arquitetura, frontend e design system. Foi aplicada uma única melhoria: extração da lógica de menu para `releaseContentMenuUtils.ts`, eliminando duplicação entre `ReleaseModalContext` e `ReleaseContentGate`.

**Recomendação:** Manter o padrão ao adicionar novos conteúdos em `releaseContents.ts` e, se no futuro houver carregamento de releases via API, garantir que as chamadas passem por `@data/api` e, se fizer sentido, por Use Cases/Repository no domínio.
