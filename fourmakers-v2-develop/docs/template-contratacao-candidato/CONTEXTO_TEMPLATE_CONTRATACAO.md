# Contexto: Template de Contratação – Dados para alterações

Documento de referência com todos os arquivos, entidades, fluxos e endpoints do **Template de Contratação do Candidato** para suportar alterações no âmbito desta feature.

---

## 1. Rota e entrada

| Item | Valor |
|------|--------|
| **Rota** | `/gestaodevagas/template-contratacao/:idCandidatura` |
| **Página** | `TemplateContratacaoCandidato` |
| **Parâmetro URL** | `idCandidatura` |
| **State (opcional)** | `location.state.vagaId`, `location.state.vagaTitle` (para link Voltar e breadcrumb) |
| **Page tracking** | `usePageTracking.ts`: `pathname.startsWith('/gestaodevagas/template-contratacao')` → título "Template de Contratação" |

**Acesso:** ícone "Template de contratação" no card do candidato em Gestão de Vagas → navega para `/gestaodevagas/template-contratacao/{idCandidatura}` com `state: { vagaId }`.

---

## 2. Arquivos por camada

### 2.1 Domain

| Arquivo | Conteúdo |
|---------|----------|
| `src/domain/entities/TemplateContratacao.ts` | `CandidaturaDetalhesVaga`, `TemplateContratacaoEndereco`, `TemplateContratacaoSaude`, `TemplateContratacaoItemLiberado`, `TemplateContratacaoData`, `ObterTemplateContratacaoResult`; tipos de listagem: `EquipamentoPadraoOpcao`, `EquipamentoPadraoCargoFuncao`, `EquipamentosPadroesAninhadosGrupo`, `DiretorioContratacaoItem`, `SistemaLiberadoContratacaoItem`, `GrupoEmailContratacaoItem`, `ListagemAcessosUsuarioResult` |
| `src/domain/repositories/TemplateContratacaoRepository.ts` | Interface: `obterPorCandidatura`, `salvarTemplate`, `listarEquipamentosPadroesAninhados`, `listarDiretorios`, `listarSistemasLiberados`, `listarGruposEmails`, `enviarEmailTemplateCandidato` |
| `src/domain/usecases/ObterTemplateContratacaoPorCandidaturaUseCase.ts` | Delega `obterPorCandidatura` ao repository |
| `src/domain/usecases/SalvarEBaixarTemplateUseCase.ts` | Delega `salvarTemplate` ao repository |
| `src/domain/usecases/EnviarEmailTemplateCandidatoUseCase.ts` | Delega `enviarEmailTemplateCandidato` ao repository |
| `src/domain/usecases/ListarDiretoriosSistemasGruposUseCase.ts` | Chama em paralelo `listarDiretorios`, `listarSistemasLiberados`, `listarGruposEmails` |
| `src/domain/usecases/ListarEquipamentosPadroesAninhadosUseCase.ts` | Delega `listarEquipamentosPadroesAninhados` ao repository |

### 2.2 Data

| Arquivo | Conteúdo |
|---------|----------|
| `src/data/repositories/TemplateContratacaoRepositoryImpl.ts` | Implementa `TemplateContratacaoRepository`; injeta `VagaApi` e `CandidaturaApi`; mapeia `ObterCandidaturaPorIdRetorno` → `CandidaturaDetalhesVaga`; monta template vazio (id `00000000-0000-0000-0000-000000000000`) quando API não retorna template; normaliza sistemasLiberados, gruposEmails, diretorios; `salvarTemplate` decide Criar vs Atualizar pelo `payload.id` |
| `src/data/api/VagaApi.ts` | `obterTemplatePorCandidatura`, `criarTemplate`, `atualizarTemplate`, `enviarEmailTemplateCandidato`, `listarEquipamentosPadroesAninhados`, `listarDiretorios`, `listarSistemasLiberados`, `listarGruposEmails`; tipos: `TemplateContratacaoRetorno`, `TemplateContratacaoEndereco`, `TemplateContratacaoSaude`, `TemplateContratacaoItemLiberado` |
| `src/data/api/CandidaturaApi.ts` | `obterCandidaturaPorId(token, idCandidatura)`; tipos `ObterCandidaturaPorIdRetorno`, `ObterCandidaturaPorIdResponse` |

### 2.3 Core (DI)

| Arquivo | Conteúdo |
|---------|----------|
| `src/core/di/tokens.ts` | `templateContratacaoRepository` |
| `src/core/di/container.ts` | Registro de `TemplateContratacaoRepositoryImpl`, `ObterTemplateContratacaoPorCandidaturaUseCase`, `SalvarEBaixarTemplateUseCase`, `EnviarEmailTemplateCandidatoUseCase`, `ListarDiretoriosSistemasGruposUseCase`, `ListarEquipamentosPadroesAninhadosUseCase` |

### 2.4 Presentation

| Arquivo | Conteúdo |
|---------|----------|
| `src/presentation/pages/TemplateContratacaoCandidato.tsx` | Página principal: breadcrumb, header da vaga, formulário em seções; usa `useTemplateContratacaoCandidato`; botões Salvar e baixar, Gerar PDF (com/sem remuneração), Enviar e-mail; modal de envio e modal de gerar PDF |
| `src/presentation/pages/TemplateContratacaoCandidato.constants.ts` | `TIPO_JORNADA_OPCOES`, `DIAS_PRESENCIAL_OPCOES`, `DIAS_PRESENCIAL_DEFAULT_OCULTO`, `TAMANHO_CAMISETA_OPCOES`, `UF_ESTADOS_BR`, `TIPO_DEFICIENCIA_OPCOES`, `PROPRIETARIO_MAQUINA_OPCOES`, `TIPO_ACESSO_OPCOES`, `SISTEMAS_LIBERADOS_LABELS`, `DIRETORIOS_REDE_LABELS`, `GRUPOS_EMAIL_LABELS` |
| `src/presentation/hooks/useTemplateContratacaoCandidato.ts` | Estado: template, candidatura, loading, saving, formErrors, equipamentosAninhados, listasDiretorios/SistemasLiberados/GruposEmails; `updateTemplate`, `updateEndereco`, `updateSaude`; `handleSalvarValidar` (validação obrigatórios + salvar); carrega template por Use Case, equipamentos e listas em useEffects; limpa formErrors quando campo fica válido; `ROTULOS_CAMPOS_OBRIGATORIOS` para mensagens |
| `src/presentation/components/template-contratacao/ColaboradorSearchField.tsx` | Autocomplete de colaboradores (Analista R&S, Superior Imediato); Use Case via ref; debounce; limite 50 |
| `src/presentation/components/template-contratacao/EnvioTemplateContratacaoModal.tsx` | Modal de envio de e-mail (com/sem remuneração, anexo); DialogTitle/DialogDescription (sr-only); usa `EnviarEmailTemplateCandidatoUseCase` |

### 2.5 Shared

| Arquivo | Conteúdo |
|---------|----------|
| `src/shared/utils/generateTemplateContratacaoPDF.ts` | `generateTemplateContratacaoPDF(template, tipoPdf?)`; tipos `com_remuneracao` \| `sem_remuneracao`; gera PDF no cliente a partir do template |

### 2.6 App

| Arquivo | Conteúdo |
|---------|----------|
| `src/app/routes/AppRoutes.tsx` | Rota `path="/gestaodevagas/template-contratacao/:idCandidatura"` → `TemplateContratacaoCandidato` |

---

## 3. Endpoints (API)

Base: `/api/`. Todas com `Authorization: Bearer {token}`.

| # | Endpoint | Método | Uso |
|---|----------|--------|-----|
| 1 | `Candidatura/ObterCandidaturaPorId?idCandidatura={id}` | GET | Detalhes da candidatura/vaga para o cabeçalho |
| 2 | `Vaga/Contratacao/ObterTemplatePorCandidatura/{idCandidatura}` | GET | Template principal; 404 = modo criação |
| 3 | `Vaga/Contratacao/ListarDiretorios` | GET | Catálogo diretórios (Acessos do Usuário) |
| 4 | `Vaga/Contratacao/ListarSistemasLiberados` | GET | Catálogo sistemas |
| 5 | `Vaga/Contratacao/ListarGruposEmails` | GET | Catálogo grupos de e-mail |
| 6 | `Vaga/Contratacao/ListarEquipamentosPadroesAninhados` | GET | Equipamentos/cargos por área |
| 7 | `Vaga/Contratacao/CriarTemplate` | POST | Persistência quando template.id é GUID vazio |
| 8 | `Vaga/Contratacao/AtualizarTemplate/{idTemplate}` | PUT | Persistência quando template já existe |
| 9 | `Vaga/Contratacao/EnviarEmailTemplateCandidato` | POST (FormData) | Envio de e-mail (param + File opcional) |
| 10 | `MapaDeAlocacao/ListarColaboradoresOrg` (ou GestaoDeAlocados) | GET | Autocomplete Analista/Superior (busca, cursor, limite 50) |
| 11 | ViaCEP `https://viacep.com.br/ws/{cep}/json/` | GET | Auto preenchimento de endereço por CEP |

---

## 4. Regras de negócio (resumo)

- **Create vs Update:** `template.id === '00000000-0000-0000-0000-000000000000'` ou vazio → POST CriarTemplate; senão → PUT AtualizarTemplate/{id}.
- **Campos obrigatórios (validação no Salvar e baixar):** Data Início, Analista de R&S, Grupo Área/Equipamento, Dias no presencial, CPF, RG, Data Nascimento, DDD, Telefone, Nome Completo, Tamanho Camiseta, E-mail Pessoal, CEP, Número, Bairro, Cidade, Estado, Superior Imediato, Tipo de Login Rede, Observações Acesso Usuário, Cargo x Máquina.
- **Telefone:** payload envia `contatoPrincipal` = DDD + número (sem hífen).
- **Endereço:** CEP sem máscara (só dígitos); preenchimento por ViaCEP.
- **Diretório:** marcar `escrita` implica `leitura` na UI.
- **Modelo Híbrido:** habilita campo "Dias no presencial" (1–5).
- **Org 9 (Royal):** `useTemplateContratacaoCandidato` usa `orgId === 9` para comportamentos especiais se necessário.

---

## 5. Estrutura da tela (seções)

1. Informações sobre a vaga (somente leitura)
2. Dados da Vaga (formulário)
3. Informações Pessoais (formulário)
4. Saúde do Candidato
5. Acessórios Foursys
6. Checklist de Instalação
7. Acessos do Usuário
8. Botão "Salvar e baixar Template"

---

## 6. Documentação existente

| Documento | Conteúdo |
|-----------|----------|
| `docs/template-contratacao-candidato/negocio.md` | Objetivo, regras, validadores, critérios de aceite, restrições |
| `docs/template-contratacao-candidato/desenvolvimento.md` | Arquitetura, APIs, contratos, padrões (Use Case, ref no ColaboradorSearchField) |
| `docs/template-contratacao-candidato/design.md` | Ordem das seções, componentes de interação, estados e feedback |
| `docs/template-contratacao-candidato/DOCUMENTACAO_TECNICA.md` | Visão geral, endpoints, fluxo React, payload, validações |
| `docs/template-contratacao-candidato/divergencias.md` | Divergências documentação vs código (performance seletor, placeholder, hooks após return) |
| `docs/CODE_REVIEW_TEMPLATE_CONTRATACAO.md` | Code review e sugestões de refactor (repository, hook, PAGE_TITLES) |

---

## 7. Dependências externas ao fluxo

- **ListarColaboradoresOrgUseCase** (lista de colaboradores para Analista/Superior) — usado pelo hook e pelo `ColaboradorSearchField`.
- **Modelos de trabalho:** página usa `useModelosTrabalho(token)` (Redux/slice ou hook próprio) para o select Modelo de trabalho.
- **CandidaturaApi** e **VagaApi** injetados apenas em `TemplateContratacaoRepositoryImpl`; presentation não importa `@data/api`.

---

*Contexto carregado para alterações no âmbito do Template de Contratação. Atualizar este ficheiro quando forem adicionados novos arquivos ou contratos.*
