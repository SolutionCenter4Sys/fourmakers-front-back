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
| R-01 | [Positivo] Visão padrão do módulo | Um colaborador sem perfil de gestor ou aprovador | acessa o módulo de Reembolso | visualiza apenas a aba "Meus Reembolsos" | as abas "Gestão ADM" e "Aprovações" permanecem ocultas | `Reembolso.tsx:57 — useReembolsos()` |
| R-02 | [Positivo] Visão do gestor no dashboard | Um colaborador com perfil de gestor | acessa o módulo de Reembolso | visualiza a aba "Gestão ADM" ao lado de "Meus Reembolsos" | ao acessar a aba, o painel de gestão administrativa é carregado | `Reembolso.tsx:319 — souGestor && TabsTrigger` |
| R-03 | [Positivo] Visão do aprovador no dashboard | Um colaborador com perfil de aprovador | acessa o módulo de Reembolso | visualiza a aba "Aprovações" ao lado de "Meus Reembolsos" | ao acessar a aba, as solicitações pendentes de aprovação são listadas | `Reembolso.tsx:328 — souAprovador && TabsTrigger` |
| R-04 | [Positivo] Indicadores de reembolsos | Um colaborador com reembolsos cadastrados | acessa a aba "Meus Reembolsos" | visualiza cards com os totais de reembolsos por status | os valores monetários são apresentados em formato de moeda brasileira | `Reembolso.tsx:361 — stats.map()` |
| R-05 | [Positivo] Filtragem por período | Um colaborador na aba "Meus Reembolsos" | seleciona uma Data Início e uma Data Fim no filtro | a tabela exibe somente reembolsos dentro do intervalo selecionado | os indicadores são recalculados com base no período filtrado | `Reembolso.tsx:46 — dataInicio, dataFim` |
| R-06 | [Positivo] Busca textual de reembolsos | Um colaborador com reembolsos listados | informa um termo no campo de busca | a tabela filtra por objetivo, destino, período, cliente/projeto, valor ou data | a paginação retorna para a primeira página | `Reembolso.tsx:144 — filteredReembolsos` |
| R-07 | [Positivo] Consulta de detalhes de um reembolso | Um colaborador com reembolsos na tabela | aciona "Detalhes" em uma linha | o modal exibe objetivo, destino, período, cliente/projeto e valor total | a tabela de itens apresenta categoria, valor solicitado, valor aprovado e status | `Reembolso.tsx:537 — modalDetalhesOpen` |
| R-08 | [Positivo] Download de documentos anexados | Um colaborador consultando um item com comprovantes | aciona o ícone de download no item da tabela de detalhes | o modal de documentos é aberto listando os arquivos disponíveis | cada documento oferece a opção de download individual | `Reembolso.tsx:621 — solicitacaoDocumentos` |
| R-09 | [Positivo] Início de nova solicitação | Um colaborador na tela de Reembolso | aciona o botão "Solicitar Reembolso" | é redirecionado para a tela de Inserir Reembolso | a URL passa a ser /inserir-reembolso | `Reembolso.tsx:297 — navigate('/inserir-reembolso')` |
| R-10 | [Negativo] Acesso indevido à Gestão ADM | Um colaborador sem perfil de gestor | acessa /reembolso?tab=gestaoadm diretamente pela URL | é redirecionado para a aba "Meus Reembolsos" | o parâmetro tab é removido da URL | `Reembolso.tsx:86 — souGestor guard` |
| R-11 | [Negativo] Acesso indevido às Aprovações | Um colaborador sem perfil de aprovador | acessa /reembolso?tab=aprovacoes diretamente pela URL | é redirecionado para a aba "Meus Reembolsos" | o parâmetro tab é removido da URL | `Reembolso.tsx:91 — souAprovador guard` |
| R-12 | [Negativo] Busca sem resultados | Um colaborador na aba "Meus Reembolsos" | informa um termo inexistente no campo de busca | a tabela apresenta a mensagem "Nenhum reembolso encontrado" | a paginação não é exibida | `Reembolso.tsx:503 — emptyMessage` |
| R-13 | [Regressivo] Limpeza dos filtros de data | Um colaborador com filtros de data aplicados | aciona o botão "Limpar" nos filtros | os campos Data Início e Data Fim são esvaziados | a tabela volta a exibir todos os reembolsos | `Reembolso.tsx:453 — setDataInicio(undefined)` |
| R-14 | [Regressivo] Persistência da aba na URL | Um colaborador alterna entre as abas disponíveis | seleciona uma aba diferente | a URL é atualizada com o parâmetro tab correspondente | ao recarregar a página a mesma aba permanece ativa | `Reembolso.tsx:101 — handleTabChange()` |
| R-15 | [Regressivo] Paginação da tabela | Um colaborador com mais reembolsos do que o limite por página | navega para a próxima página | a tabela exibe o próximo conjunto de registros | é possível alterar a quantidade de itens por página | `Reembolso.tsx:506 — TablePagination` |

## Inserir Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| I-01 | [Positivo] Adição de item com dados completos | Um colaborador com projeto selecionado | preenche todos os campos obrigatórios e aciona "Adicionar ao Carrinho" | o item passa a constar no carrinho com categoria, valor e descrição | os campos do item são limpos mantendo objetivo, destino e datas | `InserirReembolso.tsx:543 — handleAdicionarCarrinho()` |
| I-02 | [Positivo] Preenchimento automático por OCR | Um colaborador com categoria selecionada | anexa um comprovante fiscal em formato válido | os campos data da despesa e valor são preenchidos automaticamente | um indicador de análise é exibido durante o processamento | `InserirReembolso.tsx:419 — analisarComprovantes()` |
| I-03 | [Positivo] Carregamento dinâmico de categorias | Um colaborador na tela de Inserir Reembolso | seleciona um cliente/projeto no combo | as categorias compatíveis com o projeto são carregadas | os campos de unidade e valor unitário refletem a verba escolhida | `InserirReembolso.tsx:234 — carregarVerbas()` |
| I-04 | [Positivo] Envio do carrinho com sucesso | Um colaborador com itens no carrinho | aciona o botão "Enviar solicitações" | o modal de sucesso é exibido confirmando o envio | ao fechar o modal, o colaborador retorna ao dashboard de reembolsos | `InserirReembolso.tsx:773 — handleEnviarSolicitacoes()` |
| I-05 | [Positivo] Edição de item no carrinho | Um colaborador com itens no carrinho | aciona o ícone de editar em um item | o formulário é pré-preenchido com os dados do item | o botão principal passa a exibir "Atualizar Item" | `InserirReembolso.tsx:681 — handleEditarItem()` |
| I-06 | [Positivo] Remoção de item do carrinho | Um colaborador com mais de um item no carrinho | aciona o ícone de excluir em um item | o item é retirado da lista | o valor total do carrinho é recalculado | `InserirReembolso.tsx:767 — handleExcluirItem()` |
| I-07 | [Positivo] Cálculo por quantidade × valor | Um colaborador seleciona uma categoria baseada em quantidade | informa a quantidade desejada | o valor total é calculado como quantidade vezes valor unitário | os campos de unidade e valor unitário permanecem como somente leitura | `InserirReembolso.tsx:479 — mostrarCamposQuantidadeValor` |
| I-08 | [Negativo] Submissão com campos obrigatórios vazios | Um colaborador na tela de Inserir Reembolso | aciona "Adicionar ao Carrinho" sem preencher campos obrigatórios | os campos pendentes ficam destacados com borda vermelha | uma notificação alerta sobre campos obrigatórios não preenchidos | `InserirReembolso.tsx:544 — novosErros` |
| I-09 | [Negativo] Comprovante com prazo de validade excedido | Um colaborador informa uma data de despesa antiga | seleciona uma data anterior ao prazo configurado | o campo exibe borda amarela e ícone de alerta | o tooltip informa quantos dias foram excedidos | `InserirReembolso.tsx:305 — validarDataComprovante()` |
| I-10 | [Negativo] Valor acima do teto da verba | Um colaborador com categoria selecionada | informa um valor acima do limite definido para a verba | o campo de valor exibe borda amarela e ícone de alerta | o tooltip informa o valor máximo permitido | `InserirReembolso.tsx:401 — validarValorMaximo()` |
| I-11 | [Negativo] Anexo em formato não suportado | Um colaborador na seção de comprovantes | anexa um arquivo fora dos formatos aceitos | uma notificação informa que o formato é inválido | o arquivo não é adicionado à lista de comprovantes | `InserirReembolso.tsx:1246 — formatosPermitidos` |
| I-12 | [Negativo] Envio com carrinho vazio | Um colaborador sem itens no carrinho | aciona o botão "Enviar solicitações" | uma notificação informa que o carrinho está vazio | nenhuma requisição é disparada ao servidor | `InserirReembolso.tsx:774 — carrinho.length === 0` |
| I-13 | [Regressivo] Limpeza do formulário | Um colaborador com campos preenchidos | aciona o botão "Limpar" | todos os campos retornam ao estado inicial | as mensagens de erro de validação são descartadas | `InserirReembolso.tsx:514 — handleLimpar()` |
| I-14 | [Regressivo] Atualização do total do carrinho | Um colaborador altera a composição do carrinho | adiciona ou remove itens | o valor total do resumo é recalculado | o contador de itens reflete a quantidade atual | `InserirReembolso.tsx:926 — valorTotalCarrinho` |
| I-15 | [Regressivo] Navegação de retorno ao dashboard | Um colaborador na tela de Inserir Reembolso | aciona o botão de voltar | retorna à página anterior do histórico de navegação | os dados não salvos do formulário são descartados | `InserirReembolso.tsx:943 — navigate(-1)` |
