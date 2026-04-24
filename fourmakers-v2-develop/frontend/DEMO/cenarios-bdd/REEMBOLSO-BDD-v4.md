# Cenários BDD — Módulo de Reembolso

| Tipo | Qtd |
|---|:---:|
| ✅ Positivo — Gestão ADM | 11 |
| ❌ Negativo — Gestão ADM | 3 |
| 🔁 Regressivo — Gestão ADM | 3 |
| **Total** | **17** |

## Gestão ADM

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| G-01 | [Positivo] Filtro por período, cliente e status | Um gestor na aba Gestão ADM | aplica Data Início, Data Fim, cliente/projeto e status e aciona Buscar | a lista de colaboradores mostra apenas solicitações dentro do recorte | os indicadores numéricos refletem os mesmos filtros aplicados | `GestaoAdmTab.tsx:187 — handleBuscar()` |
| G-02 | [Positivo] Carregamento de projetos por cliente | Um gestor filtrando por cliente | seleciona um cliente específico | o combo de projeto habilita opções relacionadas ao cliente | ao limpar ou escolher "Todos", o projeto selecionado é removido | `GestaoAdmTab.tsx:168 — useEffect(loadProjetos)` |
| G-03 | [Positivo] Busca textual de solicitações | Um gestor com colaboradores listados | informa um termo no campo de busca | a tabela exibe somente colaboradores cujo texto contém o termo | a paginação é reiniciada para a primeira página | `GestaoAdmTab.tsx:142 — filteredColaboradores` |
| G-04 | [Positivo] Visão de indicadores da gestão | Um gestor após carregar os dados filtrados | visualiza o painel de cartões de resumo | cada card exibe contagens e valores consolidados da gestão | skeletons aparecem somente enquanto os dados são carregados | `GestaoAdmTab.tsx:511 — stats.map()` |
| G-05 | [Positivo] Distribuição de status por colaborador | Um gestor com solicitações aprovadas, pagas ou pendentes | consulta a coluna de Status na tabela | badges exibem cada status encontrado para o colaborador | cada badge traz o contador de itens naquele status | `GestaoAdmTab.tsx:223 — contarStatusPorColaborador()` |
| G-06 | [Positivo] Consulta detalhada de uma solicitação | Um gestor com solicitações listadas | aciona Detalhes em um colaborador | o modal exibe objetivo, destino, período, cliente/projeto e valor total | a tabela de itens informa categoria, valores solicitados e aprovados | `GestaoAdmTab.tsx:590 — modalDetalhesOpen` |
| G-07 | [Positivo] Visualização de comprovantes anexados | Um gestor com item que possui documentos | abre a contagem de arquivos no item | o modal de Documentos lista cada comprovante disponível | cada arquivo oferece botão de download com URL assinada | `GestaoAdmTab.tsx:754 — solicitacaoDocumentos` |
| G-08 | [Positivo] Habilitação do modo Baixa | Um gestor em uma solicitação com itens aprovados | aciona Baixar na linha do colaborador | o modal entra em modo de baixa exibindo checkboxes apenas nos itens aprovados | o botão Confirmar baixas aparece com o total selecionável | `GestaoAdmTab.tsx:286 — temAprovado` |
| G-09 | [Positivo] Seleção em massa de itens aprovados | Um gestor no modo de baixa com itens aprovados | marca Selecionar todos os aprovados | todos os itens aprovados ficam selecionados para pagamento | o contador do botão Confirmar baixas exibe a quantidade escolhida | `GestaoAdmTab.tsx:672 — Checkbox select-all` |
| G-10 | [Positivo] Confirmação de baixas com sucesso | Um gestor com itens aprovados selecionados | confirma as baixas | os lançamentos são atualizados para status Pago e um toast confirma a operação | os modais fecham, o modo de baixa é encerrado e os dados são recarregados | `GestaoAdmTab.tsx:56 — confirmarBaixas()` |
| G-11 | [Positivo] Paginação e tamanho da página | Um gestor com mais colaboradores que o limite atual | navega para outra página ou altera a quantidade por página | a tabela exibe o conjunto correspondente de colaboradores | ao mudar a quantidade por página, a navegação retorna para a primeira página | `GestaoAdmTab.tsx:573 — TablePagination` |
| G-12 | [Negativo] Busca sem correspondência | Um gestor digitando um termo inexistente | envia a busca | a tabela exibe a mensagem "Nenhum colaborador encontrado" | a paginação deixa de ser exibida enquanto não houver resultados | `GestaoAdmTab.tsx:570 — emptyMessage` |
| G-13 | [Negativo] Solicitação sem documentos anexados | Um gestor em uma solicitação sem arquivos | abre a seção de arquivos do item | a coluna exibe apenas um traço indicando ausência de documentos | ao abrir Documentos, é exibida a mensagem "Nenhum documento disponível" | `GestaoAdmTab.tsx:768 — texto de ausência` |
| G-14 | [Negativo] Ausência de itens aprovados para baixa | Um gestor em solicitação sem itens aprovados | abre o modal de detalhes | a opção Baixar não é exibida na linha da tabela | apenas a ação Detalhes permanece disponível para consulta | `GestaoAdmTab.tsx:286 — temAprovado` |
| G-15 | [Regressivo] Limpeza completa dos filtros | Um gestor após aplicar filtros | aciona o botão Limpar | os campos de data, cliente, projeto e status são resetados | os filtros aplicados são removidos, retornando ao estado inicial | `GestaoAdmTab.tsx:199 — handleLimpar()` |
| G-16 | [Regressivo] Reset ao fechar o modal de detalhes | Um gestor com o modal de detalhes aberto | fecha o modal | a solicitação selecionada é descartada e o modo de baixa é desativado | a lista de IDs selecionados é limpa para a próxima abertura | `GestaoAdmTab.tsx:593 — onOpenChange` |
| G-17 | [Regressivo] Busca reinicia paginação | Um gestor navegando pela tabela | altera o termo de busca | a paginação retorna automaticamente para a página inicial | o conjunto exibido reflete a nova busca desde a primeira página | `GestaoAdmTab.tsx:163 — useEffect(busca)` |
