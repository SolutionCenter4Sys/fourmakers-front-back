# Cenários BDD — Módulo de Reembolso

| Tipo | Qtd |
|------|-----|
| ✅ [Positivo] | 16 |
| ❌ [Negativo] | 12 |
| 🔁 [Regressivo] | 2 |
| **Total** | **30** |

## Reembolso (Dashboard)

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|--------------------------|--------------|---------------|--------------|----------|----------------------|
| 1 | [Positivo] Exibir dashboard de reembolsos | O colaborador está autenticado e possui dados de reembolsos carregados | a página Reembolso é renderizada | os cards de estatísticas e a tabela principal são exibidos | o estado de carregamento é respeitado enquanto `loading` é verdadeiro | Reembolso.tsx:47 — Reembolso() |
| 2 | [Positivo] Sincronizar aba com parâmetro de URL | Existe `?tab=` válido na URL (`gestaoadm` ou `aprovacoes`) e o perfil permite a aba | o componente monta ou a URL muda | a aba ativa corresponde ao mapeamento `tabUrlToValue` | abas sem permissão não são aplicadas (vide cenários de perfil) | Reembolso.tsx:76 — useState inicial |
| 3 | [Positivo] Alternar aba com permissão de gestor | O usuário é gestor (`souGestor`) | o valor da aba muda para `gestao-adm` | a query `tab` é atualizada para `gestaoadm` | a interface permanece consistente com a aba selecionada | Reembolso.tsx:105 — handleTabChange() |
| 4 | [Positivo] Alternar aba com permissão de aprovador | O usuário é aprovador (`souAprovador`) | o valor da aba muda para `aprovacoes` | a query `tab` é atualizada para `aprovacoes` | outras abas não aplicáveis não são forçadas | Reembolso.tsx:105 — handleTabChange() |
| 5 | [Positivo] Filtrar reembolsos por intervalo de datas | `dataInicio` e/ou `dataFim` são definidos no estado | o hook `useReembolsos` recebe o intervalo | a lista reflete o recorte temporal aplicado pela camada de dados | os controles de calendário permanecem vinculados ao estado | Reembolso.tsx:66 — useReembolsos() |
| 6 | [Positivo] Buscar na lista de reembolsos | Existem linhas com objetivo, destino, período, cliente/projeto, valores ou data | o texto de busca é alterado em `buscaReembolsos` | apenas linhas que casam com o termo permanecem em `filteredReembolsos` | a busca é case-insensitive | Reembolso.tsx:133 — filteredReembolsos |
| 7 | [Positivo] Paginar resultados filtrados | Há mais itens que o tamanho de página | a página ou itens por página são alterados | apenas o slice `paginatedReembolsos` é exibido | o total considera `filteredReembolsos.length` | Reembolso.tsx:145 — paginatedReembolsos |
| 8 | [Positivo] Exibir botão de relatório na Gestão ADM | O parâmetro `EXIBIR_BOTAO_GERAR_PAGAMENTOS` está habilitado e a aba ativa é `gestao-adm` | o memo `exibirBotaoGerarRelatorio` é recalculado | o fluxo de geração de relatório fica disponível conforme regra | valores `false`/`0` mantêm o botão oculto | Reembolso.tsx:121 — exibirBotaoGerarRelatorio |
| 9 | [Positivo] Concluir download do relatório de solicitações | Existe token válido e a API responde com arquivo | a função de geração de relatório é executada | um blob é criado e o download é disparado | o modal é fechado e `gerandoRelatorio` retorna ao estado inicial em sucesso | Reembolso.tsx:157 — handleGerarRelatorio() |
| 10 | [Negativo] Bloquear Gestão ADM sem perfil de gestor | O usuário não é gestor e a URL aponta para `gestaoadm` | o carregamento termina e o efeito de aba roda | a aba ativa volta para `meus-reembolsos` | os parâmetros de busca são limpos com `replace` | Reembolso.tsx:86 — useEffect |
| 11 | [Negativo] Bloquear Aprovações sem perfil de aprovador | O usuário não é aprovador e a URL aponta para `aprovacoes` | o carregamento termina e o efeito de aba roda | a aba ativa volta para `meus-reembolsos` | os parâmetros de busca são limpos com `replace` | Reembolso.tsx:86 — useEffect |
| 12 | [Negativo] Impedir troca manual para aba restrita | O usuário não possui `souGestor` ou `souAprovador` conforme a aba | `handleTabChange` recebe `gestao-adm` ou `aprovacoes` sem permissão | o estado da aba não é alterado para o valor restrito | a URL não é atualizada indevidamente | Reembolso.tsx:105 — handleTabChange() |
| 13 | [Negativo] Falha ao gerar relatório sem token | O seletor de autenticação não retorna `token` | a geração de relatório é acionada | a execução interrompe com log de erro e não prossegue no fluxo feliz | nenhum download é iniciado | Reembolso.tsx:157 — handleGerarRelatorio() |
| 14 | [Negativo] Erro na API ao gerar relatório | A chamada `gerarRelatorioSolicitacoesAguardandoPagamento` falha | a promise rejeita | uma mensagem de alerta informa falha | `gerandoRelatorio` é finalizado no `finally` | Reembolso.tsx:157 — handleGerarRelatorio() |
| 15 | [Regressivo] Resetar página ao mudar busca | O usuário altera o texto de busca | o efeito dependente de `buscaReembolsos` dispara | `currentPageReembolsos` retorna para 1 | a visualização não mantém página antiga incompatível com o filtro | Reembolso.tsx:155 — useEffect |

## Inserir Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|--------------------------|--------------|---------------|--------------|----------|----------------------|
| 1 | [Positivo] Carregar projetos e verbas | O colaborador está autenticado | a tela monta e os casos de uso de projetos/verbas executam | lista de projetos e verbas fica disponível para o formulário | mudança de projeto recarrega verbas conforme regra | InserirReembolso.tsx:952 — render DadosBancariosCard / montagem |
| 2 | [Positivo] Preencher cabeçalho obrigatório | Objetivo e `dataInicio` estão preenchidos e projeto é exigido | os campos principais recebem valores válidos | o estado do formulário reflete objetivo, destino opcional e período | validações de cabeçalho passam antes do item | InserirReembolso.tsx:964 — objetivo |
| 3 | [Positivo] Incluir item por valor direto | A categoria não é de tipo quantidade (`tipoCodigo !== 2`) | valor monetário válido e descrição são informados | o item pode ser adicionado ao carrinho | `formatarValorMonetario` mantém máscara em centavos | InserirReembolso.tsx:1462 — valor |
| 4 | [Positivo] Incluir item por quantidade | A verba é de tipo quantidade (`tipoCodigo === 2`) | quantidade e valores unitários são maiores que zero | o item satisfaz regras específicas de quantidade | unidade e valor unitário readonly são respeitados | InserirReembolso.tsx:1386 — quantidade |
| 5 | [Positivo] Analisar comprovantes via OCR | Arquivos de comprovante são selecionados | o caso de uso de análise é executado | data, quantidade e valores podem ser preenchidos automaticamente | o fluxo segue após `analisarComprovantes` | InserirReembolso.tsx:419 — analisarComprovantes() |
| 6 | [Positivo] Editar item no carrinho | Existe item salvo no carrinho | a ação de edição é disparada | o formulário é preenchido para atualização | o rótulo alterna para atualizar o item | InserirReembolso.tsx:681 — handleEditarItem() |
| 7 | [Positivo] Enviar solicitação com ZIP | O carrinho possui itens válidos | `handleEnviarSolicitacoes` conclui com sucesso | o ZIP com `dados.json` e anexos é enviado | o modal de sucesso é exibido | InserirReembolso.tsx:773 — handleEnviarSolicitacoes() |
| 8 | [Negativo] Objetivo obrigatório vazio | O campo objetivo está vazio | `handleAdicionarCarrinho` valida o cabeçalho | a inclusão no carrinho é bloqueada | feedback impede envio inconsistente | InserirReembolso.tsx:543 — handleAdicionarCarrinho() |
| 9 | [Negativo] Data de início ausente | `dataInicio` não foi selecionada | a validação do cabeçalho roda | a inclusão é bloqueada até definir o período inicial | demais campos não compensam a ausência | InserirReembolso.tsx:543 — handleAdicionarCarrinho() |
| 10 | [Negativo] Cliente/projeto obrigatório | O parâmetro não permite solicitação sem projeto | projeto não selecionado | a validação falha para projeto | a regra `REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO` governa o cenário | InserirReembolso.tsx:1078 — clienteProjeto |
| 11 | [Negativo] Valor inválido fora do tipo quantidade | `tipoCodigo !== 2` e valor não é maior que zero | a validação de valor dispara | o item não entra no carrinho | mensagem ou estado impede avanço | InserirReembolso.tsx:543 — handleAdicionarCarrinho() |
| 12 | [Negativo] Comprovante obrigatório ausente | `exigirComprovante` é verdadeiro e não há arquivo | a validação de anexos ocorre | a inclusão é bloqueada até anexar comprovante | OCR não substitui ausência quando exigido | InserirReembolso.tsx:1237 — comprovantes |
| 13 | [Negativo] Data do comprovante fora da validade | A data extraída excede `validadeComprovanteDias` | `validarDataComprovante` é avaliado | o fluxo sinaliza inconsistência temporal | o usuário deve corrigir data ou comprovante | InserirReembolso.tsx:305 — validarDataComprovante() |
| 14 | [Negativo] Valor acima do teto da verba | O valor informado ultrapassa o limite da verba selecionada | `validarValorMaximo` é executado | a inclusão é impedida ou alertada conforme regra | o teto vem do objeto verba | InserirReembolso.tsx:401 — validarValorMaximo() |
| 15 | [Regressivo] Limpar formulário e remover item | Existem dados preenchidos ou itens no carrinho | limpar ou excluir item é acionado | campos ou linhas são resetados | total do carrinho reflete exclusões | InserirReembolso.tsx:767 — handleExcluirItem() |
