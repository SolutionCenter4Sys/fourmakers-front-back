# Cenários BDD — Módulo de Reembolso

| Tipo | Qtd |
|---|:---:|
| ✅ Positivo — Dashboard | 9 |
| ❌ Negativo — Dashboard | 3 |
| 🔁 Regressivo — Dashboard | 3 |
| ✅ Positivo — Inserir | 7 |
| ❌ Negativo — Inserir | 5 |
| 🔁 Regressivo — Inserir | 3 |
| **Total** | **30** |

## Reembolso Dashboard

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| R-01 | [Positivo] Acesso padrão ao módulo | Colaborador sem perfil de gestor ou aprovador | acessa o módulo de Reembolso | visualiza apenas a aba "Meus Reembolsos" | as abas "Gestão ADM" e "Aprovações" não são exibidas | `Reembolso.tsx:57 — souGestor, souAprovador` |
| R-02 | [Positivo] Visualização do dashboard como gestor | Colaborador com perfil de gestor | acessa o módulo de Reembolso | visualiza a aba "Gestão ADM" além de "Meus Reembolsos" | a aba fica acessível e carrega o componente GestaoAdmTab | `Reembolso.tsx:319 — souGestor` |
| R-03 | [Positivo] Visualização do dashboard como aprovador | Colaborador com perfil de aprovador | acessa o módulo de Reembolso | visualiza a aba "Aprovações" além de "Meus Reembolsos" | a aba fica acessível e carrega o componente AprovacoesTab | `Reembolso.tsx:328 — souAprovador` |
| R-04 | [Positivo] Exibição dos indicadores numéricos | Colaborador com reembolsos cadastrados | acessa a aba "Meus Reembolsos" | visualiza os cards com estatísticas de reembolsos | os valores são apresentados com formatação de moeda | `Reembolso.tsx:361 — stats.map()` |
| R-05 | [Positivo] Filtro por período de datas | Colaborador na aba "Meus Reembolsos" | seleciona data de início e data de fim nos filtros | a tabela exibe apenas reembolsos dentro do período | os indicadores são atualizados com base no filtro | `Reembolso.tsx:46 — dataInicio, dataFim` |
| R-06 | [Positivo] Busca textual na tabela | Colaborador com reembolsos listados | informa um termo no campo de busca | a tabela filtra reembolsos por objetivo, destino, período, cliente/projeto ou valor | a paginação é reiniciada para a primeira página | `Reembolso.tsx:144 — filteredReembolsos` |
| R-07 | [Positivo] Visualização de detalhes do reembolso | Colaborador com reembolsos na tabela | aciona o botão "Detalhes" em uma linha da tabela | o modal exibe objetivo, destino, período, cliente/projeto e valor total | a tabela de itens lista categoria, valor solicitado, valor aprovado e status | `Reembolso.tsx:537 — modalDetalhesOpen` |
| R-08 | [Positivo] Download de documentos anexados | Colaborador visualizando detalhes de um item com documentos | aciona o botão de download no item da tabela de detalhes | o modal de documentos é exibido com lista de arquivos | cada documento apresenta opção de download individual | `Reembolso.tsx:621 — solicitacaoDocumentos` |
| R-09 | [Positivo] Navegação para nova solicitação | Colaborador na tela de Reembolso | aciona o botão "Solicitar Reembolso" | é redirecionado para a tela de Inserir Reembolso | a URL muda para /inserir-reembolso | `Reembolso.tsx:297 — navigate("/inserir-reembolso")` |
| R-10 | [Negativo] Acesso à aba Gestão ADM sem permissão | Colaborador sem perfil de gestor tenta acessar via URL | acessa /reembolso?tab=gestaoadm diretamente | é redirecionado para a aba "Meus Reembolsos" | o parâmetro tab é removido da URL | `Reembolso.tsx:86 — souGestor check` |
| R-11 | [Negativo] Acesso à aba Aprovações sem permissão | Colaborador sem perfil de aprovador tenta acessar via URL | acessa /reembolso?tab=aprovacoes diretamente | é redirecionado para a aba "Meus Reembolsos" | o parâmetro tab é removido da URL | `Reembolso.tsx:91 — souAprovador check` |
| R-12 | [Negativo] Busca sem resultados | Colaborador na aba "Meus Reembolsos" | informa um termo inexistente no campo de busca | a tabela exibe a mensagem "Nenhum reembolso encontrado" | a paginação não é exibida | `Reembolso.tsx:503 — emptyMessage` |
| R-13 | [Regressivo] Limpeza dos filtros de data | Colaborador com filtros de data aplicados | aciona o botão "Limpar" nos filtros | as datas de início e fim são removidas | a tabela volta a exibir todos os reembolsos sem filtro de período | `Reembolso.tsx:451 — setDataInicio(undefined)` |
| R-14 | [Regressivo] Sincronização da aba com URL | Colaborador alterna entre abas disponíveis | seleciona a aba "Gestão ADM" | a URL é atualizada com o parâmetro tab=gestaoadm | ao recarregar a página, a mesma aba permanece ativa | `Reembolso.tsx:101 — handleTabChange()` |
| R-15 | [Regressivo] Paginação da tabela de reembolsos | Colaborador com mais reembolsos do que o limite por página | navega para a próxima página na paginação | a tabela exibe o próximo conjunto de registros | o número de itens por página pode ser alterado | `Reembolso.tsx:505 — TablePagination` |

## Inserir Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| I-01 | [Positivo] Adição de item ao carrinho com dados completos | Colaborador com projeto selecionado | preenche todos os campos obrigatórios e aciona "Adicionar ao Carrinho" | o item aparece no carrinho com categoria, valor e descrição | o formulário é limpo para novo preenchimento | `InserirReembolso.tsx:543 — handleAdicionarCarrinho()` |
| I-02 | [Positivo] Análise automática de comprovante via OCR | Colaborador com categoria selecionada | anexa um comprovante fiscal em formato válido | os campos data da despesa e valor são preenchidos automaticamente | o indicador de análise é exibido durante o processamento | `InserirReembolso.tsx:419 — analisarComprovantes()` |
| I-03 | [Positivo] Carregamento dinâmico de categorias | Colaborador na tela de Inserir Reembolso | seleciona um cliente/projeto no combobox | as categorias disponíveis são carregadas conforme o projeto | os campos de unidade e valor unitário refletem a verba selecionada | `InserirReembolso.tsx:234 — carregarVerbas()` |
| I-04 | [Positivo] Envio do carrinho com sucesso | Colaborador com itens no carrinho | aciona o botão "Enviar solicitações" | o modal de sucesso é exibido confirmando o envio | ao fechar o modal, é redirecionado para o dashboard de reembolsos | `InserirReembolso.tsx:773 — handleEnviarSolicitacoes()` |
| I-05 | [Positivo] Edição de item existente no carrinho | Colaborador com itens no carrinho | aciona o botão de editar em um item do carrinho | o formulário é preenchido com os dados do item selecionado | o botão muda para "Atualizar Item" | `InserirReembolso.tsx:681 — handleEditarItem()` |
| I-06 | [Positivo] Remoção de item do carrinho | Colaborador com múltiplos itens no carrinho | aciona o botão de excluir em um item do carrinho | o item é removido da lista | o valor total do carrinho é recalculado | `InserirReembolso.tsx:767 — handleExcluirItem()` |
| I-07 | [Positivo] Solicitação com categoria baseada em quantidade | Colaborador seleciona uma categoria com tipoCodigo = 2 | preenche o campo quantidade | o valor total é calculado automaticamente como quantidade × valor unitário | os campos de unidade e valor unitário são exibidos como somente leitura | `InserirReembolso.tsx:479 — mostrarCamposQuantidadeValor` |
| I-08 | [Negativo] Submissão com campos obrigatórios vazios | Colaborador na tela de Inserir Reembolso | aciona "Adicionar ao Carrinho" sem preencher campos obrigatórios | os campos obrigatórios ficam destacados com borda vermelha | uma notificação informa que há campos pendentes | `InserirReembolso.tsx:544 — novosErros` |
| I-09 | [Negativo] Data do comprovante excede prazo de validade | Colaborador com comprovante cuja data ultrapassa o limite configurado | seleciona uma data de despesa antiga | o campo exibe borda amarela e ícone de alerta | o tooltip informa quantos dias foram excedidos | `InserirReembolso.tsx:305 — validarDataComprovante()` |
| I-10 | [Negativo] Valor do reembolso excede teto da verba | Colaborador com categoria selecionada | informa um valor acima do limite permitido pela verba | o campo de valor exibe borda amarela e ícone de alerta | o tooltip informa o valor máximo permitido | `InserirReembolso.tsx:401 — validarValorMaximo()` |
| I-11 | [Negativo] Upload de arquivo em formato não permitido | Colaborador na seção de comprovantes | anexa um arquivo que não é PNG, JPG ou PDF | uma notificação informa que o formato é inválido | o arquivo não é adicionado à lista de comprovantes | `InserirReembolso.tsx:1246 — formatosPermitidos` |
| I-12 | [Negativo] Envio com carrinho vazio | Colaborador sem itens no carrinho | aciona o botão "Enviar solicitações" | uma notificação informa que o carrinho está vazio | a solicitação não é enviada ao servidor | `InserirReembolso.tsx:774 — carrinho.length === 0` |
| I-13 | [Regressivo] Limpeza completa do formulário | Colaborador com campos preenchidos no formulário | aciona o botão "Limpar" | todos os campos retornam ao estado inicial | os erros de validação são removidos | `InserirReembolso.tsx:514 — handleLimpar()` |
| I-14 | [Regressivo] Atualização do valor total do carrinho | Colaborador adiciona ou remove itens do carrinho | modifica a composição do carrinho | o valor total exibido no resumo é recalculado | o contador de itens reflete a quantidade atual | `InserirReembolso.tsx:926 — valorTotalCarrinho` |
| I-15 | [Regressivo] Navegação de retorno ao dashboard | Colaborador na tela de Inserir Reembolso | aciona o botão de voltar | retorna à página anterior do histórico de navegação | os dados não salvos no formulário são descartados | `InserirReembolso.tsx:943 — navigate(-1)` |
