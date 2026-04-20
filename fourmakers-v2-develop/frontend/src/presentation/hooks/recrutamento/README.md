# Hooks — módulo Recrutamento

Hooks e helpers usados pelas páginas `@presentation/pages/recrutamento` e pelos componentes em `presentation/components/gestao-vagas` (vagas/candidatos).

| Arquivo / pasta | Uso |
|-----------------|-----|
| `useGestaoVagas.ts` | Kanban / gestão de vagas |
| `useGestaoVagasCandidatos.ts` | Tela de candidatos por vaga |
| `useDashboardRecrutamento.ts` | Dashboard de métricas |
| `usePublicVagaDetalhe.ts` | Vaga pública + inscrição |
| `useTemplateContratacaoCandidato.ts` | Template de contratação |
| `usePerfilAtuacao.ts`, `usePerfilAtuacaoPage.tsx` | Criar perfil de atuação |
| `gestaoVagas/` | `gestaoVagasUtils`, `gestaoVagasStatusFlows` |

**Import:** `import { useGestaoVagas, formatDatePtBr } from '@presentation/hooks/recrutamento'`

Playbook: `docs/MODULARIZACAO_PRESENTATION_RECRUTAMENTO.md`.
