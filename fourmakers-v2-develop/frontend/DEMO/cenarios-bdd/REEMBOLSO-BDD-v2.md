# Cenários BDD — Módulo de Reembolso

| Tipo | Qtd |
|---|:---:|
| ✅ Positivo — Reembolso (Dashboard) | 8 |
| ❌ Negativo — Reembolso (Dashboard) | 4 |
| 🔁 Regressivo — Reembolso (Dashboard) | 3 |
| ✅ Positivo — Inserir Reembolso | 7 |
| ❌ Negativo — Inserir Reembolso | 6 |
| 🔁 Regressivo — Inserir Reembolso | 4 |
| **Total** | **32** |

## Reembolso (Dashboard)

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| R-01 | [Positivo] Visualização dos painéis de resumo | Um colaborador acessa a tela de Reembolso | A tela é carregada | Os painéis de resumo exibem o total de solicitações, valores e status | — | `Reembolso.tsx:361 — stats.map()` |
| R-02 | [Positivo] Acesso à aba Gestão ADM por gestor | Um gestor acessa a tela de Reembolso | A tela é carregada | A aba "Gestão ADM" aparece no menu de navegação | Ao selecioná-la, o sistema exibe a tela de gestão administrativa | `Reembolso.tsx:319 — souGestor` |
| R-03 | [Positivo] Acesso à aba Aprovações por aprovador | Um colaborador com permissão de aprovação acessa a tela de Reembolso | A tela é carregada | A aba "Aprovações" aparece no menu de navegação | Ao selecioná-la, o sistema exibe as solicitações aguardando aprovação | `Reembolso.tsx:328 — souAprovador` |
| R-04 | [Positivo] Filtro de solicitações por período | Um colaborador na aba "Meus Reembolsos" | Seleciona datas nos campos "Data Início" e "Data Fim" | A lista exibe apenas as solicitações do período selecionado | — | `Reembolso.tsx:57 — useReembolsos(dataInicio, dataFim)` |
| R-05 | [Positivo] Busca textual na lista de reembolsos | Um colaborador com solicitações cadastradas | Informa um termo no campo de busca | A lista filtra as solicitações que contêm o termo informado | A paginação é reiniciada para a primeira página | `Reembolso.tsx:144 — filteredReembolsos` |
| R-06 | [Positivo] Visualização dos detalhes de uma solicitação | Um colaborador com solicitações cadastradas | Acessa os detalhes de uma solicitação | Uma janela de detalhes exibe objetivo, destino, período, cliente/projeto e valor total | A lista de itens da solicitação com categoria, valores e status é apresentada | `Reembolso.tsx:537 — modalDetalhesOpen` |
| R-07 | [Positivo] Download de documentos anexados | Um colaborador visualizando os detalhes de uma solicitação com documentos | Acessa os documentos de um item | Uma janela exibe os documentos disponíveis para download | Cada documento pode ser baixado individualmente | `Reembolso.tsx:659 — modalDocumentosOpen` |
| R-08 | [Positivo] Geração de relatório de pagamentos | Um gestor na aba "Gestão ADM" com a funcionalidade de relatório habilitada | Confirma a geração do relatório | O sistema gera e inicia o download do relatório de solicitações aguardando pagamento | A janela de confirmação é fechada após a conclusão | `Reembolso.tsx:169 — handleGerarRelatorio()` |
| R-09 | [Negativo] Colaborador sem permissões especiais | Um colaborador sem permissão de gestão ou aprovação | Acessa a tela de Reembolso | Apenas a aba "Meus Reembolsos" é exibida | As abas "Gestão ADM" e "Aprovações" não aparecem | `Reembolso.tsx:319 — souGestor, souAprovador` |
| R-10 | [Negativo] Acesso direto a aba restrita sem permissão | Um colaborador sem permissão de aprovação | Tenta acessar a aba "Aprovações" diretamente | O sistema redireciona para a aba "Meus Reembolsos" | — | `Reembolso.tsx:91 — useEffect redirect` |
| R-11 | [Negativo] Lista vazia sem solicitações cadastradas | Um colaborador sem solicitações de reembolso | Acessa a aba "Meus Reembolsos" | A lista aparece vazia com a mensagem "Nenhum reembolso encontrado" | — | `Reembolso.tsx:503 — emptyMessage` |
| R-12 | [Negativo] Busca sem resultados | Um colaborador com solicitações cadastradas | Informa um termo que não corresponde a nenhuma solicitação | A lista aparece vazia com a mensagem "Nenhum reembolso encontrado" | — | `Reembolso.tsx:144 — filteredReembolsos.filter()` |
| R-13 | [Regressivo] Indicadores de carregamento na tela | Um colaborador acessa a tela de Reembolso | Os dados ainda estão sendo carregados | Os painéis de resumo e a lista exibem indicadores de carregamento | Os filtros de data também exibem indicador de carregamento | `Reembolso.tsx:344 — loading skeleton` |
| R-14 | [Regressivo] Botão de nova solicitação sempre acessível | Um colaborador na tela de Reembolso | A tela é carregada | O botão "Solicitar Reembolso" está visível e acessível | Ao acioná-lo, o sistema navega para a tela de nova solicitação | `Reembolso.tsx:297 — navigate("/inserir-reembolso")` |
| R-15 | [Regressivo] Limpar filtros de período | Um colaborador com filtro de período aplicado | Aciona a opção de limpar os filtros | Os campos de data são esvaziados | A lista exibe todas as solicitações sem restrição de período | `Reembolso.tsx:449 — setDataInicio/setDataFim(undefined)` |

## Inserir Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| I-01 | [Positivo] Adição de item ao carrinho | Um colaborador com projeto e categoria selecionados | Preenche todos os campos obrigatórios e adiciona ao carrinho | O item aparece no carrinho com categoria, valor e descrição | O total do carrinho é atualizado | `InserirReembolso.tsx:543 — handleAdicionarCarrinho()` |
| I-02 | [Positivo] Preenchimento automático via leitura do comprovante | Um colaborador com uma categoria selecionada | Envia um comprovante fiscal | O sistema preenche automaticamente a data, o valor e a quantidade extraídos do comprovante | — | `InserirReembolso.tsx:419 — analisarComprovantes()` |
| I-03 | [Positivo] Seleção de categoria cobrada por quantidade | Um colaborador seleciona uma categoria cobrada por distância ou quantidade | A categoria é selecionada | Os campos de quantidade, unidade e valor unitário são exibidos | O valor total é calculado automaticamente com base na quantidade e valor unitário | `InserirReembolso.tsx:1382 — mostrarCamposQuantidadeValor` |
| I-04 | [Positivo] Envio de solicitação com sucesso | Um colaborador com itens no carrinho | Confirma o envio das solicitações | O sistema envia os dados e exibe uma mensagem de sucesso | O colaborador é redirecionado para a tela de Reembolso | `InserirReembolso.tsx:773 — handleEnviarSolicitacoes()` |
| I-05 | [Positivo] Edição de item no carrinho | Um colaborador com itens adicionados ao carrinho | Seleciona a opção de editar um item | Os campos do formulário são preenchidos com os dados do item selecionado | O botão muda para "Atualizar Item" | `InserirReembolso.tsx:681 — handleEditarItem()` |
| I-06 | [Positivo] Exibição dos dados bancários | Um colaborador acessa a tela de nova solicitação | A tela é carregada | Os dados bancários do colaborador são exibidos | — | `InserirReembolso.tsx:952 — DadosBancariosCard` |
| I-07 | [Positivo] Envio de comprovante em formato válido | Um colaborador preenchendo uma solicitação | Envia um arquivo em formato PNG, JPG ou PDF | O arquivo aparece na lista de comprovantes anexados | A leitura automática do comprovante é iniciada | `InserirReembolso.tsx:1242 — file input onChange` |
| I-08 | [Negativo] Campos obrigatórios não preenchidos | Um colaborador na tela de nova solicitação | Tenta adicionar ao carrinho sem preencher os campos obrigatórios | Os campos obrigatórios são destacados visualmente | Uma notificação informa que há campos pendentes | `InserirReembolso.tsx:544 — handleAdicionarCarrinho() validação` |
| I-09 | [Negativo] Comprovante obrigatório não anexado | Um colaborador com uma categoria que exige comprovante | Tenta adicionar ao carrinho sem anexar comprovante | O campo de comprovante é destacado como pendente | O item não é adicionado ao carrinho | `InserirReembolso.tsx:604 — exigirComprovante` |
| I-10 | [Negativo] Data do comprovante fora da validade | Um colaborador com um comprovante cuja data excede o limite permitido | A data da despesa é informada | Um alerta visual indica que a data está fora do prazo de validade | O número de dias excedidos é exibido | `InserirReembolso.tsx:305 — validarDataComprovante()` |
| I-11 | [Negativo] Valor acima do teto da categoria | Um colaborador com uma categoria que possui teto de valor | Informa um valor acima do limite permitido | Um alerta visual indica que o valor excede o máximo | O valor máximo permitido é informado ao colaborador | `InserirReembolso.tsx:401 — validarValorMaximo()` |
| I-12 | [Negativo] Envio com carrinho vazio | Um colaborador na tela de nova solicitação sem itens no carrinho | Tenta enviar as solicitações | Uma notificação informa que o carrinho está vazio | O envio não é realizado | `InserirReembolso.tsx:774 — carrinho.length === 0` |
| I-13 | [Negativo] Formato de arquivo inválido no comprovante | Um colaborador preenchendo uma solicitação | Tenta enviar um arquivo em formato não permitido | Uma notificação informa que apenas PNG, JPG e PDF são aceitos | O arquivo inválido não é anexado | `InserirReembolso.tsx:1246 — formatosPermitidos` |
| I-14 | [Regressivo] Cálculo automático do valor total | Um colaborador preenchendo o formulário de solicitação | Informa o valor da despesa ou a quantidade | O valor total é atualizado automaticamente | O resumo exibe o valor em formato monetário brasileiro | `InserirReembolso.tsx:480 — valorTotal` |
| I-15 | [Regressivo] Exclusão de item do carrinho | Um colaborador com itens no carrinho | Remove um item do carrinho | O item é removido da lista | O total de itens e o valor total são atualizados | `InserirReembolso.tsx:767 — handleExcluirItem()` |
| I-16 | [Regressivo] Limpeza completa do formulário | Um colaborador com campos preenchidos no formulário | Aciona a opção de limpar | Todos os campos do formulário são esvaziados | Os indicadores de erro e alertas visuais são removidos | `InserirReembolso.tsx:514 — handleLimpar()` |
| I-17 | [Regressivo] Solicitação sem projeto quando permitido | Um colaborador em uma empresa que permite solicitação sem projeto vinculado | Tenta adicionar ao carrinho sem selecionar um projeto | O campo "Cliente/Projeto" não é obrigatório | O item é adicionado ao carrinho normalmente | `InserirReembolso.tsx:106 — permitirSolicitacaoSemProjeto` |
