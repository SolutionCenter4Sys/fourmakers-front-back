# Cenários BDD — Módulo de Reembolso

| Tipo | Qtd |
|---|:---:|
| ✅ Positivo | 6 |
| ❌ Negativo | 2 |
| 🔁 Regressivo | 2 |
| **Total** | **10** |

## Reembolso (Dashboard)

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| R-01 | [Positivo] Acesso ao dashboard de reembolsos | um colaborador autenticado | acessa o módulo de Reembolso | visualiza a aba Meus Reembolsos com painéis de resumo e lista de solicitações | os totais e a tabela refletem o período selecionado | `Reembolso.tsx:60 — useReembolsos()` · `useReembolsos.ts:30 — useReembolsos()` |
| R-02 | [Negativo] Aba de gestão restrita sem perfil de gestor | um colaborador sem permissão de gestão | tenta abrir a aba Gestão ADM | permanece na aba Meus Reembolsos | a aba Gestão ADM não fica disponível no menu | `Reembolso.tsx:89 — souGestor` · `Reembolso.tsx:322 — souGestor` |
| R-04 | [Positivo] Navegação para nova solicitação | um colaborador na tela de Reembolso | solicita um novo reembolso | é direcionado à tela de inserção de solicitação | o formulário de nova solicitação fica disponível | `Reembolso.tsx:44 — ROTA_NOVA_SOLICITACAO_REEMBOLSO` · `Reembolso.tsx:300 — navigate()` |

## Inserir Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| I-08 | [Positivo] Inclusão de item no carrinho com comprovante | um colaborador com projeto e categoria que exige comprovante | adiciona um item com valor, data e comprovante válidos | o item aparece no carrinho com valor e descrição | o total do carrinho é atualizado | `InserirReembolso.tsx:543 — handleAdicionarCarrinho()` |
| I-09 | [Negativo] Categoria com comprovante obrigatório sem anexo | um colaborador com categoria que exige comprovante | tenta adicionar o item sem anexar comprovante | o sistema impede a inclusão no carrinho | uma indicação de comprovante obrigatório é exibida | `InserirReembolso.tsx:604 — exigirComprovante` |
| I-13 | [Positivo] Envio da solicitação com itens no carrinho | um colaborador com ao menos um item válido no carrinho | envia a solicitação de reembolso | a solicitação é registrada com sucesso | o colaborador retorna ao fluxo de acompanhamento | `InserirReembolso.tsx:773 — handleEnviarSolicitacoes()` |
| I-14 | [Regressivo] Leitura automática de dados do comprovante | um colaborador anexando um comprovante fiscal legível | o sistema analisa o arquivo | valor e data do comprovante são sugeridos no formulário | o colaborador pode confirmar ou ajustar os dados antes de incluir no carrinho | `InserirReembolso.tsx:437 — AnalisarComprovantes` · `InserirReembolso.tsx:305 — validarDataComprovante()` |

## Aprovar Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| AP-01 | [Positivo] Aprovação de solicitações pendentes | um colaborador com permissão de aprovação e solicitações selecionadas | confirma a aprovação das solicitações | as solicitações passam para o status aprovado | a lista de pendências é atualizada | `AprovarReembolso.tsx:371 — handleAprovar()` · `AprovarReembolso.tsx:380 — confirmarAprovar()` |
| AP-02 | [Negativo] Reprovação sem justificativa | um colaborador com permissão de aprovação e solicitações selecionadas | tenta reprovar sem informar justificativa | o sistema impede a conclusão da reprovação | a justificativa permanece obrigatória | `AprovarReembolso.tsx:375 — handleReprovar()` · `AprovarReembolso.tsx:436 — confirmarReprovar()` |

## Parâmetros de Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| RP-01 | [Regressivo] Consulta das regras e verbas parametrizadas | um gestor acessando os parâmetros do módulo | abre a aba de regras | visualiza as verbas e regras configuradas | as informações ficam disponíveis para manutenção | `ReembolsoParametros.tsx:13 — Tabs` · `RegrasTab.tsx:117 — VerbaCard` |
