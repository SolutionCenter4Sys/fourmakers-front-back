# Orientações para criação de documentação técnica de features

Este documento é um **guia de orientação** para gerar a documentação técnica de **novas features** (telas, fluxos ou protótipos). O objetivo é que o **output** gerado siga o mesmo padrão e nível de detalhe do arquivo **`docs/FEEDBACK360_DOCUMENTACAO_TECNICA.md`**, adequando qualquer prototipação — desenvolvida no projeto React ou fora dele (ex.: Lovable, Gemini Studio, FlutterFlow, Figma + código gerado) — à realidade do projeto (arquitetura, contratos, backend .NET 8).

- **Criado em:** 09/02/2026  
- **Última atualização:** 09/02/2026 (inclusão da seção 4 – controle de criação/atualização do documento)

---

## 1. Quando usar este guia

- **Front desenvolveu uma tela nova** no projeto e é preciso documentar para o backend.
- **Designer ou produto** criou um protótipo em ferramenta generativa (Lovable, Gemini Studio, Bolt, etc.) e o código será integrado ou reimplementado no projeto.
- **Documentação técnica** de uma feature para alinhamento entre times (front, back, produto).
- **Auditoria de Design System** (ver `DESIGN_SYSTEM_AUDIT.md`): se a feature auditada **não tiver** documentação em `docs/`, a documentação técnica deve ser **criada** quando forem realizadas as correções apontadas na auditoria; se já existir, deve ser **atualizada** conforme as correções, quando necessário.

**Protótipos externos sem acesso ao repositório:** Se o protótipo foi criado fora do projeto (ex.: Firebase Studio, Lovable, Gemini Studio) e **não há** acesso aos arquivos base do repositório, use o guia **`ORIENTACAO_DOCUMENTACAO_TECNICA_PROTOTIPOS_EXTERNOS.md`**: ele é autocontido (sem referências a arquivos internos) e contém todas as regras e a estrutura para a plataforma generativa gerar a documentação; o resultado pode ser trazido para o projeto e validado pelo time com esforço mínimo.

---

## 2. Referência de output

**Arquivo de referência (modelo de resultado):** `docs/FEEDBACK360_DOCUMENTACAO_TECNICA.md`

O documento gerado deve ser o **mais parecido possível** com esse arquivo em:
- Estrutura de seções (visão geral, parâmetros, regras de negócio, funcionalidades, APIs, modelos, dependências, fluxos, melhorias, erros, resumo).
- Nível de detalhe (tabelas de campos, exemplos JSON de request/response, envelope padrão, contratos sugeridos para .NET, dependências e tabelas).
- Padrões de nomenclatura e contratos (ver seção 3 abaixo).

Para features com **múltiplos perfis de usuário**, **ações com confirmação**, **estados bloqueados** ou **mensagens sensíveis ao tom** (ex.: candidato, usuário final), enriqueça a documentação com: objetivos principais e personas na visão geral; fluxos por persona; eventos e ações desencadeadas (gatilho, confirmação, efeitos); regras de sucesso, erro e bloqueios (o que o usuário vê e pode fazer); linguagem e tom. O modelo de **Módulo de Admissão Digital** (documentação técnica) é uma boa referência desse estilo “documentação rica”.

---

## 3. Padrões de contratos do projeto (obrigatório)

Todo documento técnico de feature deve **reproduzir ou referenciar** estes padrões para consistência com o restante do sistema:

| Aspecto | Padrão | Uso |
|---------|--------|-----|
| **Nomenclatura JSON** | camelCase | `codigoInternoColaborador`, `dataCriacao`, `nomeColaborador` |
| **Envelope de resposta** | `retorno`, `sucesso`, `mensagem`, `erros?` | Todas as APIs de listagem/criação/atualização |
| **Data de criação** | `dataCriacao` (string ISO ou YYYY-MM-DD) | Em entidades e DTOs de resposta |
| **Data de alteração** | `dataAlteracao` (quando houver auditoria) | Idem |
| **Identificador de colaborador** | `codigoInternoColaborador` nos contratos de domínio | Request/response; nota: `ListarColaboradoresOrg` retorna `codigoColaboradorInterno` (mesmo valor) |
| **Erros de validação** | `erros?: string[] \| null` no envelope | Respostas de erro ou validação |

Referência de APIs existentes: `FuncionalidadeSistemaApi`, `GrupoAcessoApi`, `VagaApi`, `CandidaturaApi`, entidades em `src/domain/entities`.

---

## 4. Controle de criação e atualização do documento

**Obrigatório:** Todo arquivo de documentação técnica de feature ou protótipo deve incluir, de forma explícita, a informação de **quando a documentação foi criada** e **quando foi última vez atualizada**, para controle de criações e alterações.

- **Onde colocar:** No início do documento (após o título ou na primeira seção) ou ao final, em bloco único e visível.
- **Formato sugerido:**
  - **Criado em:** DD/MM/AAAA (opcional: responsável ou origem).
  - **Última atualização:** DD/MM/AAAA (opcional: breve motivo — ex.: "Ajuste de contratos após code review").
- **Ao atualizar o documento:** Sempre alterar a data (e, se fizer sentido, o motivo) de "Última atualização" para a data da alteração.

Isso permite rastrear a evolução das documentações e garantir que alterações de features e protótipos fiquem registradas no próprio arquivo.

---

## 5. Estrutura mínima do documento gerado

O arquivo de documentação técnica da feature deve conter as seções abaixo. Adapte títulos e subseções ao contexto (ex.: “Aba X” só se a tela tiver abas).

1. **Visão geral e objetivo**  
   - **Objetivos principais (visão de produto):** lista em tópicos (centralizar, automatizar, guiar, fornecer visibilidade, garantir conformidade, etc.).  
   - Rota(s), título, descrição (UI), objetivo de negócio, escopo atual (o que está e o que não está).  
   - **Personas (se aplicável):** quando houver mais de um perfil (ex.: RH vs candidato), nomear e descrever a experiência de cada um.

2. **Parâmetros de entrada e contexto**  
   - Autenticação (token); parâmetros de rota/query; dependências de APIs existentes (quais endpoints a tela já chama).

3. **Padrões de contratos do projeto (consistência)**  
   - Tabela resumida (como na seção 3 acima) ou referência a este arquivo de orientação; nota sobre alinhar nomes de campos (ex.: `dataCriacao` no backend, mapeamento no front se necessário).

4. **Regras de negócio**  
   - Regras por domínio (ex.: níveis, obrigatoriedade condicional, quem pode fazer o quê, o que é listado/filtrado).  
   - **Regras críticas em destaque:** bloqueios de avanço (quando o usuário não pode prosseguir e o que vê na tela), obrigatoriedade de motivo (ex.: reprovação exige motivo e candidato vê o motivo), auditoria (quais ações geram log: usuário, ação, data/hora).

5. **Fluxos por persona (quando houver mais de um perfil)**  
   - Um fluxo por tipo de usuário (ex.: “Fluxo do RH”, “Experiência do Candidato”), com visão geral da experiência e subfluxos/telas. Se houver apenas um perfil, fundir com a seção 6.

6. **Funcionalidades, experiência do usuário e eventos**  
   - Passo a passo por fluxo (ex.: “Ao carregar”, “Ao submeter”, “Filtros”, “Modais”).  
   - **Eventos e ações desencadeadas:** para ações críticas (ex.: enviar convite, aprovar, reprovar): gatilho (o que o usuário faz), confirmação (modal: título e descrição), ações desencadeadas (mudança de estado, auditoria, notificação/toast).  
   - **Estados da interface:** bloqueado, em carregamento, vazio, erro; indicadores visuais (SLA, status, badges) e o que cada um significa.  
   - **Explicação de itens e módulos:** conteúdo de listas, checklists, abas (ex.: “Aba Documentos: checklist com status por item; botão Ver para itens Em revisão”); labels e mensagens que o usuário vê.

7. **Regras de sucesso, erro e bloqueios (UX)**  
   - **Sucesso:** o que acontece quando a ação dá certo (mensagem, redirecionamento, atualização); textos de toast/modal quando relevante.  
   - **Erro:** por cenário de falha, o que o usuário vê e o que pode fazer; sugerir mensagens amigáveis para `mensagem`/`erros`.  
   - **Bloqueios:** condição, o que a tela mostra (mensagem, botão desabilitado, seção bloqueada) e o que desbloqueia.

8. **Linguagem e tom (visão de produto)**  
   - Orientações para mensagens: tom positivo e simples; evitar jargões (ex.: “Precisa de ajuste” em vez de “Reprovado”); explicar termos técnicos. Exemplos de frases que a interface deve usar ou evitar.

9. **APIs necessárias (backend .NET 8)**  
   - Para cada endpoint: método e rota, headers, parâmetros de query ou corpo, exemplo de resposta (JSON) com envelope `retorno`, `sucesso`, `mensagem`, `erros`. Contrato sugerido em C# (request/response/DTOs).  
   - **Serviços em abstração (quando aplicável):** operações além de CRUD (ex.: OCR, confirmação por IA): nome, entrada, saída, lógica em uma frase.

10. **Modelos de dados (contratos)**  
    - Tabelas: entidade/DTO (propriedade, tipo, obrigatório, descrição); payload de criação/edição; envelope de respostas padrão.

11. **Dependências de APIs e dados existentes**  
    - Quais APIs ou tabelas do projeto a feature usa ou impacta; alinhamento de nomes (ex.: `codigoColaboradorInterno` vs `codigoInternoColaborador`).

12. **Fluxo resumido (backend)**  
    - Passos em texto ou lista numerada (ex.: obter usuário do token → validar → persistir → retornar).

13. **Propostas de melhorias (evolução da feature)**  
    - Filtros no servidor, paginação, edição/exclusão, auditoria, etc.

14. **Cenários de erro e pontos de atenção**  
    - Erros não mapeados no front; impacto em desenvolvimento (tabelas, FKs, segurança).

15. **Resumo para o time**  
    - Feature em uma frase; APIs a implementar; APIs já usadas; contratos em uma linha; pontos de atenção.

Ao final: *“Documento alinhado à stack React (ARCHITECTURE.md) e ao padrão de APIs do projeto. Última atualização: [data].”*

**Documentação mais rica (visão produto e UX):** As seções 1 (objetivos e personas), 4 (regras críticas), 5 (fluxos por persona), 6 (eventos e estados), 7 (sucesso/erro/bloqueios) e 8 (linguagem e tom) seguem o estilo de documentação “rica” que combina regras de negócio, contratos e visão de experiência do usuário — como no modelo de documentação do Módulo de Admissão Digital. Inclua essas seções sempre que a feature tiver fluxos multi-perfil, ações com confirmação, estados bloqueados ou mensagens sensíveis ao tom (ex.: candidato, usuário final).

---

## 6. Nome e local do arquivo

- **Pasta:** `docs/`
- **Nome sugerido:** `[NOME_DA_FEATURE]_DOCUMENTACAO_TECNICA.md`  
  Exemplos: `FEEDBACK360_DOCUMENTACAO_TECNICA.md`, `GESTAO_VAGAS_CANDIDATOS_FF.md` (quando vier de FlutterFlow ou outro protótipo).
- **Encoding:** UTF-8.

---

## 7. Dicas para prototipação generativa (Lovable, Gemini, etc.)

- **Entrada para o gerador:** Descrever a tela/fluxo em linguagem de negócio e técnico (rotas, abas, formulários, listagens, filtros, modais). Informar que o backend será .NET 8 e que os contratos devem seguir os padrões deste projeto (envelope, camelCase, `dataCriacao`, `codigoInternoColaborador`, `erros`).
- **Pós-geração:** Conferir o documento gerado contra `FEEDBACK360_DOCUMENTACAO_TECNICA.md` e contra a seção **Padrões de contratos** (item 3) deste guia; ajustar nomes de campos e envelope se a ferramenta tiver usado convenções diferentes.
- **Sem backend ainda:** Documentar “APIs necessárias” e “Modelos” com base no que o protótipo ou o front espera consumir; assim o backend pode implementar com esforço mínimo.

---

## 8. Relação com a auditoria de Design System

Conforme **`DESIGN_SYSTEM_AUDIT.md`**:

- Se a feature **não possuir** documentação técnica em `docs/`, ao rodar a auditoria e realizar as **correções** apontadas, deve ser **criado** um novo arquivo em `docs/` seguindo esta orientação (e o modelo do Feedback 360).
- Se a feature **já possuir** documentação em `docs/`, as **correções** da auditoria devem ser refletidas na documentação **quando necessário** (ex.: mudança de contratos, novos endpoints, novas regras de negócio).

Isso garante que toda feature nova auditada passe a ter (ou manter atualizada) a documentação técnica para o time e para o backend.

---

*Referência de exemplo completo: **`docs/FEEDBACK360_DOCUMENTACAO_TECNICA.md`**. Arquitetura do projeto: **`ARCHITECTURE.md`**.*
