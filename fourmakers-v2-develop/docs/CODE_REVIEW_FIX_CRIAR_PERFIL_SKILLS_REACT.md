# Code review: fix/criar-perfil-skills-react

**Branch:** `fix/criar-perfil-skills-react`  
**Escopo:** Página Criar/Editar Perfil de Atuação (`/gestaodevagas/perfil`) — habilidades com seletor relevante (imprescindível/desejável) e estilo primário/secundário.

---

## Objetivo da alteração

- Permitir marcar cada habilidade como **relevante** (imprescindível) ou **não relevante** (desejável) via ícone de estrela.
- Payload de criação/edição já suporta `relevante: true | false` em `gestorExternoPerfilSkills`; garantir que a UI reflita e persista esse valor.
- **Visual:** tag com aspecto de botão **primário** (fundo escuro, texto e ícones brancos) quando relevante; aspecto **secundário** quando não relevante. Padding vertical aumentado e ícones centralizados na tag.

---

## Arquivos alterados

| Arquivo | Alteração |
|--------|-----------|
| `src/domain/entities/PerfilAtuacao.ts` | `PerfilSkill` com campo opcional `relevante?: boolean`. |
| `src/shared/utils/perfilAtuacao.ts` | `mapSkillsToPayload` usa `skill.relevante ?? true`. |
| `src/data/api/PerfilAtuacaoApi.ts` | Tipo de `gestorExternoPerfilSkills` em `obterGestorExternoPerfilPorId` com `relevante?: boolean`. |
| `src/components/ui/system-icons.tsx` | `Star` (outlined) e `StarFilled` (default) para estados desejável/imprescindível. |
| `src/presentation/hooks/usePerfilAtuacao.ts` | Carregamento de `relevante` da API; default `true` ao adicionar skill no modal; `handleToggleSkillRelevante`, `handleTogglePendingSkillRelevante`; payload envia `relevante` por skill. |
| `src/presentation/pages/CriarPerfilAtuacao.tsx` | Tags de skill com estrela clicável, estilo primário (relevante) / secundário (não relevante), padding `py-2`, ícones centralizados; mesmo comportamento no modal; texto de ajuda (imprescindível dourada / desejável azul) e hover destrutivo no botão remover quando tag secundária. |

---

## Auditoria de conformidade

### Design System (`public/design-toolkit.md`)

- **Tokens:** Uso de tokens do DS para botões/tags: `bg-btnPrimary`, `text-inverseText`, `border-primary` (relevante); `bg-btnSecondary`, `text-btnSecondaryText`, `border-borderDefault` (não relevante). Estado de erro: `border-destructive`, `bg-surfaceSubtle`, `text-primaryText`. Nenhuma cor hardcoded.
- **Componentes:** Badge, Button, Alert, Tooltip, Dialog (Title, Description), ScrollArea, Input, Select, etc. da lib `@/components/ui`. Ícones via `system-icons` (Star, StarFilled, Info, Trash2).
- **Acessibilidade:** `aria-label` e `title` nos botões da estrela e da lixeira; texto de ajuda no modal e na seção Skills.
- **Consistência:** Estilo das tags alinhado ao padrão de botões primário/secundário do DS; radius e padding coerentes.

### Arquitetura

- **Camadas:** A **página** `CriarPerfilAtuacao` não importa `@data/api`; consome apenas o hook `usePerfilAtuacao`. O **hook** segue o padrão já existente desta feature e importa funções de `@data/api/PerfilAtuacaoApi` (não alterado por este fix). Domain (`PerfilSkill.relevante`), shared (`mapSkillsToPayload`), data (tipos e API) e presentation (UI + estado) estão alinhados.
- **Payload:** `gestorExternoPerfilSkills` enviado na criação/edição inclui `relevante` por item; leitura em edição usa `relevante` retornado pela API.

### Frontend e UX

- **Feedback visual:** Diferença clara entre imprescindível (tag primária) e desejável (tag secundária); ícones preenchido/contorno e herança de cor (`text-inherit`); hover destrutivo apenas na tag secundária no botão remover.
- **Alinhamento:** Tags com `inline-flex items-center gap-2`, `py-2` e botões com `flex items-center justify-center` para centralizar ícones com o texto.

---

## Resultado

- **Conformidade:** Atende ao Design System (tokens, componentes, acessibilidade) e ao padrão arquitetural atual da feature (hook → API). Nenhuma nova violação introduzida.
- **Build:** `npm run build:homolog` executado com sucesso.

---

*Auditoria realizada em Fev 2026 (fix/criar-perfil-skills-react).*
