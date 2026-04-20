# Template de Contratação do Candidato – Desenvolvimento

## Arquitetura e camadas

- **Presentation:** `pages/recrutamento/TemplateContratacaoCandidato.tsx`, `.constants.ts`, hook em `hooks/recrutamento/useTemplateContratacaoCandidato.ts`, `ColaboradorSearchField.tsx`.
- **Domain:** entidades em `TemplateContratacao.ts`; use cases `ObterTemplateContratacaoPorCandidaturaUseCase`, `SalvarEBaixarTemplateUseCase`, `ListarColaboradoresOrgUseCase`.
- **Data:** `VagaApi`, `CandidaturaApi`, `TemplateContratacaoRepositoryImpl`; repositórios implementam interfaces do domain.

## APIs e integrações

- **ObterCandidaturaPorId** (GET): mapeamento de `retorno` para `CandidaturaDetalhesVaga` no repositório.
- **ObterTemplatePorCandidatura** (GET): template por id da candidatura.
- **ListarDiretorios**, **ListarSistemasLiberados**, **ListarGruposEmails** (GET): listas para Acessos do Usuário; fallback para constantes se API falhar.
- **ListarColaboradoresOrg** (GET): usado pelo `ColaboradorSearchField` via `ListarColaboradoresOrgUseCase` (Analista de R&S, Superior Imediato); **limite 50** na requisição para evitar travar a tela.
- **ListarEquipamentosPadroesAninhados**, **ListarModelosTrabalho**: Tipo equipamento e Modelo de trabalho.
- **SalvarEBaixarTemplate** (POST): salva e retorna blob para download.

## Contratos e tipos

- `TemplateContratacaoData`, `TemplateContratacaoEndereco`, `TemplateContratacaoSaude`, `CandidaturaDetalhesVaga`, `TemplateContratacaoItemLiberado` em `domain/entities/TemplateContratacao.ts`.
- Respostas das APIs de listagem (Diretórios, Sistemas, Grupos) tipadas em `VagaApi.ts`.

## Padrões obrigatórios

- Uso de `httpClient` (nenhum `fetch`/`axios` direto).
- Componentes de apresentação não acessam `data/` diretamente; uso de Use Cases.
- ColaboradorSearchField: Use Case resolvido via ref para estabilizar callback e evitar efeito de busca em todo re-render (performance/flicker); lista limitada a 50 itens; requestId para ignorar respostas obsoletas.
- Página: todos os hooks (incl. `useCallback` de onSelect) declarados **antes** de qualquer `return` condicional para evitar "Rendered more hooks than during the previous render".

**Documentação técnica completa (agnóstica, paridade React):** `docs/template-contratacao-candidato/DOCUMENTACAO_TECNICA.md`.
