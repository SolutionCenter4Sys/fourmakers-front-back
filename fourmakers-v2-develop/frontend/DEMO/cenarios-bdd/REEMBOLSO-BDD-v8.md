# Cenários BDD — Módulo de Reembolso

| Tipo | Qtd |
|---|:---:|
| ✅ Positivo — Dashboard | 8 |
| ❌ Negativo — Dashboard | 3 |
| 🔁 Regressivo — Dashboard | 3 |
| ✅ Positivo — Inserir | 7 |
| ❌ Negativo — Inserir | 4 |
| 🔁 Regressivo — Inserir | 3 |
| **Total** | **28** |

## Reembolso (Dashboard)

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| R-01 | [Positivo] Visualização padrão do dashboard | Colaborador sem perfil de gestor acessa a tela de Reembolso | A tela é carregada | Apenas a aba "Meus Reembolsos" é exibida | Os cards de estatísticas mostram totais, pendentes e aprovados | `Reembolso.tsx:57 — souGestor, souAprovador` |
| R-02 | [Positivo] Filtrar reembolsos por período | Colaborador possui reembolsos registrados | Seleciona intervalo de Data Início e Data Fim | A tabela exibe apenas registros dentro do intervalo | A paginação reflete o total filtrado | `Reembolso.tsx:389 — filtros de período` |
| R-03 | [Positivo] Buscar reembolsos por texto livre | Reembolsos estão carregados na tabela | Informa termo de busca no campo de pesquisa | A tabela filtra linhas por objetivo, destino, período ou cliente/projeto | A página volta para 1 ao digitar | `Reembolso.tsx:144 — filteredReembolsos` |
| R-04 | [Positivo] Abrir modal de detalhes da solicitação | Existe ao menos uma solicitação na tabela | Aciona o botão "Detalhes" na linha | O modal exibe objetivo, destino, período e cliente/projeto | A lista de itens mostra categoria, valor e status de cada despesa | `Reembolso.tsx:236 — renderCell acoes` |
| R-05 | [Positivo] Baixar comprovante com token autenticado | Item da solicitação possui documentos anexados | Aciona o botão "Baixar" no painel de documentos | O download inicia com URL processada pelo token | O arquivo abre com nome e tipo corretos | `Reembolso.tsx:693 — processarUrlComToken` |
| R-06 | [Positivo] Gerar relatório de solicitações aguardando pagamento | Botão "Gerar Relatório" está visível na aba Gestão ADM | Confirma a geração do relatório | O download inicia com o nome retornado pela API | O modal de confirmação fecha após conclusão | `Reembolso.tsx:169 — handleGerarRelatorio()` |
| R-07 | [Positivo] Solicitar novo reembolso pelo dashboard | Colaborador está na tela principal | Aciona o botão "Solicitar Reembolso" | O sistema navega para o formulário de inserção | O breadcrumb reflete o novo caminho | `Reembolso.tsx:297 — navigate /inserir-reembolso` |
| R-08 | [Positivo] Exibir botão Remessa CNAB para usuários autorizados | Parâmetro HABILITA_BOTAO_REMESSA_CNAB_REEMBOLSO está ativo | Acessa a aba Gestão ADM | O botão "Remessa CNAB" aparece nas ações | O clique navega para /reembolso/remessa-cnab | `Reembolso.tsx:62 — exibirBotaoRemessaCNAB` |
| R-09 | [Negativo] Busca sem resultados exibe mensagem vazia | Reembolsos estão carregados | Pesquisa um termo que não corresponde a nenhum registro | A tabela fica vazia | A mensagem "Nenhum reembolso encontrado" é exibida | `Reembolso.tsx:503 — DataTable emptyMessage` |
| R-10 | [Negativo] Impedir seleção de data fim futura | Colaborador abre o filtro de Data Fim | Tenta selecionar uma data posterior a hoje | O calendário desabilita datas futuras | O campo permanece sem valor selecionado | `Reembolso.tsx:437 — disabled date > today` |
| R-11 | [Negativo] Falha ao gerar relatório exibe alerta | Botão de relatório está visível | Confirma a geração e a API retorna erro | Um alerta "Erro ao gerar relatório" é exibido | O modal permanece aberto para nova tentativa | `Reembolso.tsx:199 — catch handleGerarRelatorio` |
| R-12 | [Regressivo] Bloquear aba Gestão ADM sem perfil de gestor | Colaborador não possui perfil de gestor | A URL contém tab=gestaoadm | A aba ativa retorna para "Meus Reembolsos" | O parâmetro tab é removido da URL | `Reembolso.tsx:86 — validação souGestor` |
| R-13 | [Regressivo] Bloquear aba Aprovações sem perfil de aprovador | Colaborador não possui perfil de aprovador | A URL contém tab=aprovacoes | A aba ativa retorna para "Meus Reembolsos" | A navegação é impedida enquanto não houver permissão | `Reembolso.tsx:91 — validação souAprovador` |
| R-14 | [Regressivo] Sincronizar aba ativa com parâmetro da URL | Colaborador tem permissão para a aba informada na URL | A página é carregada com tab=gestaoadm ou tab=aprovacoes | A aba correspondente fica selecionada | O estado permanece após recarregar a página | `Reembolso.tsx:81 — tabUrlToValue + setActiveTab` |

## Inserir Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| I-01 | [Positivo] Adicionar item com categoria tipo 1 ao carrinho | Campos obrigatórios estão preenchidos e categoria tipo valor livre selecionada | Confirma a inclusão do item | O item aparece no carrinho com valor informado e descrição | Os campos do item são limpos para nova inclusão | `InserirReembolso.tsx:543 — handleAdicionarCarrinho()` |
| I-02 | [Positivo] Calcular valor total para categoria tipo 2 | Categoria com valor unitário fixo selecionada e quantidade informada | Adiciona o item ao carrinho | O valor total é calculado por quantidade × valor unitário | O card de totais mostra o valor formatado em reais | `InserirReembolso.tsx:480 — valorTotal` |
| I-03 | [Positivo] Pré-preenchimento por OCR de comprovante fiscal | Colaborador anexa comprovante para análise | A API de OCR retorna data e valor extraídos | Os campos Data da despesa e Valor são preenchidos automaticamente | A validação de valor máximo é reavaliada com o novo valor | `InserirReembolso.tsx:418 — analisarComprovantes()` |
| I-04 | [Positivo] Editar item já adicionado ao carrinho | Carrinho contém itens previamente incluídos | Aciona editar em um dos itens | Os campos do formulário são preenchidos com os dados do item selecionado | O formulário rola para o topo para facilitar a edição | `InserirReembolso.tsx:681 — handleEditarItem()` |
| I-05 | [Positivo] Excluir item do carrinho | Carrinho contém ao menos um item | Aciona excluir em um item | O item é removido da lista | O total de itens e o valor consolidado são recalculados | `InserirReembolso.tsx:767 — handleExcluirItem()` |
| I-06 | [Positivo] Enviar solicitações em lote via ZIP | Carrinho possui itens válidos com comprovantes | Confirma o envio | Um ZIP é montado com dados.json e arquivos nomeados archive_N | A API retorna sucesso e o modal de confirmação abre | `InserirReembolso.tsx:873 — handleEnviarSolicitacoes()` |
| I-07 | [Positivo] Redirecionar para dashboard após envio com sucesso | O envio retornou sucesso | Fecha o modal de confirmação | A página redireciona para /reembolso | O carrinho é esvaziado | `InserirReembolso.tsx:920 — handleFecharModalSucesso()` |
| I-08 | [Negativo] Bloquear inclusão sem campos obrigatórios preenchidos | Formulário está incompleto (objetivo ou categoria ausente) | Tenta adicionar ao carrinho | O toast "Campos obrigatórios" é exibido | Os campos com erro ficam destacados em vermelho | `InserirReembolso.tsx:544 — validação obrigatória` |
| I-09 | [Negativo] Impedir envio com carrinho vazio | Não há itens no carrinho | Tenta enviar as solicitações | O sistema exibe aviso de carrinho vazio | Nenhuma requisição é enviada à API | `InserirReembolso.tsx:773 — handleEnviarSolicitacoes()` |
| I-10 | [Negativo] Bloquear comprovante obrigatório ausente | Categoria selecionada exige comprovante | Tenta adicionar item sem anexar arquivo | A validação impede a inclusão | O campo de comprovante é destacado como obrigatório | `InserirReembolso.tsx:604 — exigirComprovante` |
| I-11 | [Negativo] Rejeitar data de despesa inválida no envio | Carrinho possui item com data de início mal formatada | Tenta enviar solicitações | A rotina detecta data inválida e interrompe o envio | Um toast informa para corrigir a data | `InserirReembolso.tsx:803 — validação de data` |
| I-12 | [Regressivo] Sinalizar comprovante vencido | Validade de comprovante configurada em dias | Comprovante enviado possui data além da validade | O estado de comprovante excedido é ativado | Um alerta visual aparece no card do item | `InserirReembolso.tsx:305 — validarDataComprovante()` |
| I-13 | [Regressivo] Alertar valor acima do teto da verba | Categoria tem valor máximo definido | Colaborador informa valor que ultrapassa o teto | O campo recebe aviso visual com borda âmbar | O card de total exibe ícone de alerta | `InserirReembolso.tsx:401 — validarValorMaximo()` |
| I-14 | [Regressivo] Permitir solicitação sem projeto vinculado | Parâmetro REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO habilitado | Deixa o campo Cliente/Projeto vazio e adiciona ao carrinho | O item entra no carrinho sem projeto associado | A validação de projeto não bloqueia a inclusão | `InserirReembolso.tsx:561 — permitirSolicitacaoSemProjeto` |
