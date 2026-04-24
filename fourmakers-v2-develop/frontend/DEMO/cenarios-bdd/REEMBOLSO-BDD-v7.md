# Cenários BDD — Módulo de Reembolso

| Tipo | Qtd |
|---|:---:|
| ✅ Positivo — Dashboard | 9 |
| ❌ Negativo — Dashboard | 2 |
| 🔁 Regressivo — Dashboard | 3 |
| ✅ Positivo — Inserir | 8 |
| ❌ Negativo — Inserir | 3 |
| 🔁 Regressivo — Inserir | 3 |
| **Total** | **28** |

## Reembolso (Dashboard)
| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| R-01 | [Positivo] Filtrar período de solicitação | Há reembolsos carregados | Usuário seleciona Data Início e Data Fim | A lista mostra apenas registros no intervalo | A paginação mantém o total filtrado | Reembolso.tsx:389 — filtros de período |
| R-02 | [Positivo] Buscar por objetivo/destino | Há reembolsos carregados | Usuário digita texto no campo Busca | A tabela exibe apenas linhas que correspondem ao termo | O contador de linhas reflete o filtro aplicado | Reembolso.tsx:143 — filteredReembolsos |
| R-03 | [Negativo] Busca sem correspondência | Há reembolsos carregados | Usuário pesquisa um termo inexistente | A tabela não exibe linhas | A mensagem “Nenhum reembolso encontrado” aparece | Reembolso.tsx:498 — DataTable emptyMessage |
| R-04 | [Positivo] Abrir detalhes da solicitação | Existe linha de reembolso na tabela | Usuário aciona Ações › Detalhes | O modal exibe cabeçalho com objetivo, destino e período | A seção de itens lista categoria, valores e status | Reembolso.tsx:232 — renderCell Detalhes |
| R-05 | [Positivo] Baixar comprovantes do item | Um item possui documentos anexados | Usuário abre Documentos dentro do modal | A lista mostra cada arquivo com nome e tipo | O botão Baixar abre o arquivo processado com token | Reembolso.tsx:693 — processarUrlComToken |
| R-06 | [Positivo] Gerar relatório aguardando pagamento | Usuário visualiza dashboard | Usuário confirma gerar relatório | O download inicia com nome retornado pela API | O modal fecha após concluir a geração | Reembolso.tsx:169 — handleGerarRelatorio() |
| R-07 | [Regressivo] Bloqueio da aba Gestão ADM sem permissão | Usuário não é gestor | A URL contém tab=gestaoadm | A aba ativa volta para Meus Reembolsos | O parâmetro tab é removido da URL | Reembolso.tsx:75 — validação de permissões |
| R-08 | [Regressivo] Bloqueio da aba Aprovações sem permissão | Usuário não é aprovador | A URL contém tab=aprovacoes | A aba ativa volta para Meus Reembolsos | A navegação não muda enquanto não houver permissão | Reembolso.tsx:75 — validação de permissões |
| R-09 | [Regressivo] Sincronizar aba com URL quando permitido | Usuário tem permissão para a aba informada | A URL contém tab=gestaoadm ou tab=aprovacoes | A aba correspondente fica selecionada | O estado ativo permanece após recarregar a página | Reembolso.tsx:81 — tabUrlToValue + setActiveTab |
| R-10 | [Positivo] Exibir Remessa CNAB apenas com parâmetro ativo | Parâmetro HABILITA_BOTAO_REMESSA_CNAB_REEMBOLSO está habilitado | Usuário acessa a aba Gestão ADM | O botão Remessa CNAB aparece nas ações da página | O clique leva para /reembolso/remessa-cnab | Reembolso.tsx:61 — exibirBotaoRemessaCNAB |
| R-11 | [Positivo] Resetar paginação ao alterar busca | Há itens paginados | Usuário altera o termo de busca | A página atual volta para 1 | A paginação usa o novo total filtrado | Reembolso.tsx:163 — reset da página ao buscar |
| R-12 | [Positivo] Alternar carregamento entre skeleton e cards | A tela inicia em loading | O hook conclui o carregamento | Os skeletons somem | Os StatCards exibem títulos e valores carregados | Reembolso.tsx:343 — loading vs StatCard |
| R-13 | [Positivo] Acesso rápido para criar novo reembolso | Usuário está no dashboard | Usuário aciona “Solicitar Reembolso” | A navegação abre o formulário de inserção | O histórico mantém o caminho /reembolso | Reembolso.tsx:273 — PageHeader actions |
| R-14 | [Negativo] Impedir data fim futura | Usuário abre filtro de Data Fim | Seleciona uma data maior que hoje | O calendário desabilita datas futuras | O filtro permanece vazio sem alterar a lista | Reembolso.tsx:437 — disabled futura data |

## Inserir Reembolso
| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| I-01 | [Positivo] Adicionar item tipo 1 ao carrinho | Campos obrigatórios preenchidos e categoria tipo 1 selecionada | Usuário confirma Adicionar ao Carrinho | O item entra no carrinho com valor informado | Os campos do item são limpos para nova inclusão | InserirReembolso.tsx:543 — handleAdicionarCarrinho() |
| I-02 | [Positivo] Calcular valor total para tipo 2 | Categoria tipoCodigo 2 selecionada com quantidade e valor unitário válidos | Usuário adiciona o item | O valor total é calculado por quantidade × valor unitário | O card verde mostra o total formatado | InserirReembolso.tsx:480 — valorTotal |
| I-03 | [Positivo] OCR de comprovante pré-preenche data e valor | Usuário envia comprovante para análise | A API retorna data e valor da despesa | Os campos Data da despesa e Valor são preenchidos automaticamente | A validação de valor máximo é reavaliada | InserirReembolso.tsx:418 — analisarComprovantes() |
| I-04 | [Positivo] Exigir comprovante quando a verba obriga | Categoria selecionada exige comprovante | Usuário anexa arquivos antes de salvar | O item é salvo com arquivos base64 e nomes preservados | O carrinho indica a presença de comprovantes | InserirReembolso.tsx:604 — validação de comprovante |
| I-05 | [Positivo] Editar item do carrinho | Carrinho possui itens | Usuário aciona editar em um item | Os campos do formulário são preenchidos com os dados do item | O formulário rola para o topo para edição | InserirReembolso.tsx:681 — handleEditarItem() |
| I-06 | [Positivo] Excluir item do carrinho | Carrinho possui itens | Usuário aciona excluir em um item | O item some da lista do carrinho | O total de itens exibido é atualizado | InserirReembolso.tsx:767 — handleExcluirItem() |
| I-07 | [Positivo] Enviar solicitações consolidadas em ZIP | Carrinho contém pelo menos um item válido | Usuário confirma o envio | Um ZIP é montado com dados.json e arquivos nomeados archive_N | A API de inserção retorna sucesso | InserirReembolso.tsx:873 — handleEnviarSolicitacoes() |
| I-08 | [Positivo] Redirecionar após sucesso | Envio retorna sucesso | Usuário fecha o modal de sucesso | A página redireciona para /reembolso | O modal é fechado | InserirReembolso.tsx:920 — handleFecharModalSucesso() |
| I-09 | [Regressivo] Sinalizar comprovante vencido | Validade de comprovante configurada | Comprovante enviado tem data acima da validade | O estado comprovanteExcedido é marcado | Um alerta é exibido no card do item | InserirReembolso.tsx:305 — validarDataComprovante() |
| I-10 | [Regressivo] Alertar teto da verba excedido | Categoria tem valor máximo definido | Usuário informa valor acima do teto | O campo é marcado com aviso e borda âmbar | O card total exibe ícone de alerta | InserirReembolso.tsx:401 — validarValorMaximo() |
| I-11 | [Regressivo] Solicitação sem projeto quando permitido | Parâmetro REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO habilitado | Usuário deixa Cliente/Projeto vazio | A validação não bloqueia o envio do item | O item entra no carrinho sem projeto associado | InserirReembolso.tsx:560 — validação condicional |
| I-12 | [Negativo] Impedir envio com carrinho vazio | Não há itens no carrinho | Usuário tenta enviar solicitações | Um toast de “Carrinho vazio” é exibido | Nenhum arquivo ZIP é gerado | InserirReembolso.tsx:773 — handleEnviarSolicitacoes() |
| I-13 | [Negativo] Bloquear inclusão sem campos obrigatórios | Formulário está incompleto (objetivo ou valor ausente) | Usuário tenta adicionar ao carrinho | O toast “Campos obrigatórios” é exibido | Os campos com erro ficam destacados | InserirReembolso.tsx:544 — validação obrigatória |
| I-14 | [Negativo] Recusar datas inválidas na submissão | Carrinho possui item com data início inválida | Usuário tenta enviar solicitações | A rotina detecta data inválida e aborta | O toast informa para corrigir a data | InserirReembolso.tsx:803 — validação de data início |
