# Cenários BDD — Módulo de Reembolso

| Tipo | Qtd |
|------|-----|
| ✅ Positivo | 16 |
| ❌ Negativo | 8 |
| 🔁 Regressivo | 6 |
| **Total** | **30** |

## Reembolso (Dashboard)

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|--------------------------|--------------|---------------|--------------|----------|----------------------|
| 1 | [Positivo] Aba inicial via URL | existe `tab` na query compatível com o mapa de abas | a página é carregada com `searchParams` válidos | a aba interna correspondente é selecionada na primeira renderização | valores desconhecidos caem em “meus-reembolsos” | Reembolso.tsx:74 — Reembolso() |
| 2 | [Negativo] Bloqueio de Gestão Administrativa | o colaborador não possui perfil de gestor (`!souGestor`) | a URL indica `tab=gestaoadm` após o fim do carregamento | a aba ativa volta para “meus-reembolsos” | os parâmetros de rota são limpos com `replace` | Reembolso.tsx:101 — useEffect() |
| 3 | [Positivo] Troca de aba com sincronismo na URL | o usuário tem permissão para a aba desejada | seleciona outra aba na lista de tabs | `activeTab` é atualizado e `setSearchParams` grava o slug da aba | abas sem slug dedicado removem o parâmetro `tab` | Reembolso.tsx:108 — handleTabChange() |
| 4 | [Negativo] Impedir aba Aprovações sem perfil | `souAprovador` é falso | tenta ativar a aba “aprovacoes” | a função retorna sem alterar estado nem URL | gestores/aprovadores seguem regras separadas de gestão | Reembolso.tsx:109 — handleTabChange() |
| 5 | [Positivo] Busca textual na grade principal | existem linhas com objetivo, destino, período, cliente/projeto, valores ou data | informa texto no campo de busca de reembolsos | apenas linhas cujo conteúdo contém o termo (case insensitive) permanecem visíveis | campos vazios na busca exibem toda a lista filtrada pelo período carregado | Reembolso.tsx:125 — filteredReembolsos |
| 6 | [Regressivo] Reset de paginação ao filtrar | a lista está em página > 1 | altera o texto de busca | a página atual de reembolsos retorna para 1 | evita página vazia após redução do conjunto filtrado | Reembolso.tsx:148 — useEffect() |
| 7 | [Positivo] Paginação por fatia da lista filtrada | há mais itens que o limite por página | avança ou altera itens por página | `slice` usa `startIndex` e `endIndex` coerentes com `currentPage` e `itemsPerPage` | o total exibido reflete `filteredReembolsos.length` | Reembolso.tsx:139 — Reembolso() |
| 8 | [Positivo] Modal de detalhes da solicitação | existe correspondência entre linha agregada e `solicitacoes` | aciona “Detalhes” na coluna de ações | `reembolsoSelecionado` recebe a solicitação completa e o modal abre | matching prioriza `objeto[0].id` quando ambos existem | Reembolso.tsx:196 — renderCell() |
| 9 | [Positivo] Botão Gerar Relatório na Gestão | parâmetro `EXIBIR_BOTAO_GERAR_PAGAMENTOS` resolve para verdadeiro e aba é gestão | a área do cabeçalho é renderizada | o fluxo condicional exibe o botão apenas nessa combinação | valores “false”/“0” mantêm o botão oculto | Reembolso.tsx:116 — exibirBotaoGerarRelatorio |
| 10 | [Negativo] Ocultar relatório fora da Gestão ou com parâmetro falso | parâmetro é falso ou aba diferente de gestão-adm | o layout principal é montado | o botão de gerar relatório não é apresentado | evita disparo fora do escopo administrativo | Reembolso.tsx:118 — exibirBotaoGerarRelatorio |
| 11 | [Positivo] Download do relatório de pagamentos | JWT disponível e confirmação do usuário | a API devolve `arrayBuffer`, `contentType` e `fileName` | um `Blob` é criado, link temporário dispara download e URLs são revogadas | o modal de confirmação é fechado ao sucesso | Reembolso.tsx:165 — handleGerarRelatorio() |
| 12 | [Negativo] Geração sem token | `token` ausente no estado de autenticação | dispara a geração do relatório | a execução encerra com log de erro sem download | não prossegue para chamada autenticada | Reembolso.tsx:166 — handleGerarRelatorio() |
| 13 | [Regressivo] Falha na API do relatório | o backend ou rede falha durante `gerarRelatorioSolicitacoesAguardandoPagamento` | a promessa rejeita | usuário vê alerta genérico e `gerandoRelatorio` volta a falso | erro é registrado no console | Reembolso.tsx:179 — handleGerarRelatorio() |
| 14 | [Positivo] Botão Remessa CNAB condicionado | parâmetro `HABILITA_BOTAO_REMESSA_CNAB_REEMBOLSO` consta na lista carregada | o cabeçalho avalia `parametros` | o botão de remessa CNAB só aparece quando o código existe | não depende da aba ativa além do que o JSX consolidar | Reembolso.tsx:84 — exibirBotaoRemessaCNAB |
| 15 | [Positivo] Indicadores e listagem com período | `dataInicio`/`dataFim` podem ser definidos e o hook está ativo | o período aplicado muda | `useReembolsos` alimenta `stats`, `reembolsos` e flags `souGestor`/`souAprovador` | loading impede reatribuição prematura de aba por URL | Reembolso.tsx:69 — useReembolsos() |

## Inserir Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|--------------------------|--------------|---------------|--------------|----------|----------------------|
| 1 | [Positivo] Carga de projetos do colaborador | token e caso de uso resolvido no container | o efeito de montagem executa | lista `projetos` é preenchida a partir de `GetProjetosColaboradorUseCase` | estados de carregamento refletem a requisição | InserirReembolso.tsx:182 — useEffect() |
| 2 | [Positivo] Carga de verbas ao escolher projeto | projeto válido selecionado | o efeito dependente dispara | `GetVerbasUseCase` popula `verbas` e prepara categorias | erros de rede podem ser tratados no fluxo existente | InserirReembolso.tsx:234 — useEffect() |
| 3 | [Negativo] Comprovante fora da validade | parâmetro define `validadeComprovanteDias` e data do comprovante excede o limite | valida data do comprovante | retorno indica inconsistência impeditiva para seguir com o item | mensagem segue regra de `validarDataComprovante` | InserirReembolso.tsx:305 — validarDataComprovante() |
| 4 | [Positivo] Análise OCR de comprovantes | arquivos elegíveis anexados e caso de uso disponível | solicita análise automática | campos como data, quantidade e valores são sugeridos conforme retorno do OCR | loading dedicado evita duplo disparo | InserirReembolso.tsx:419 — analisarComprovantes() |
| 5 | [Negativo] Adicionar ao carrinho sem obrigatórios | campos mandatórios vazios (objetivo, datas, categoria, etc.) | tenta adicionar item | validações impedem push em `carrinho` e sinalizam erros | projeto exigido salvo se `permitirSolicitacaoSemProjeto` falso | InserirReembolso.tsx:543 — handleAdicionarCarrinho() |
| 6 | [Positivo] Item válido no carrinho | todos os requisitos do item atual passam | confirma adição | novo `CarrinhoItem` entra no array com anexos e totais | totais e flags `exigirComprovante` respeitam verba | InserirReembolso.tsx:543 — handleAdicionarCarrinho() |
| 7 | [Positivo] Edição de item existente | carrinho contém item alvo | escolhe editar | formulário é reidratado com dados do item selecionado | mantém rastreio de arquivos e metadados necessários | InserirReembolso.tsx:681 — handleEditarItem() |
| 8 | [Positivo] Remoção de item do carrinho | carrinho não vazio | confirma exclusão do item | array `carrinho` perde o elemento sem corromper os demais | totais globais são recalculados na renderização | InserirReembolso.tsx:767 — handleExcluirItem() |
| 9 | [Positivo] Envio com ZIP e sucesso | há itens no carrinho e ZIP montado com `dados.json` e anexos | envia para `/api/Financeiro/Reembolso/Solicitacao/Inserir` | resposta bem-sucedida abre `modalSucesso` | fluxo usa `InserirSolicitacaoUseCase` | InserirReembolso.tsx:773 — handleEnviarSolicitacoes() |
| 10 | [Negativo] Resposta da API com lista de erros | backend retorna payload com `erros[]` | finaliza requisição com falha de negócio | `modalErro` exibe cada mensagem retornada | usuário pode corrigir e reenviar | InserirReembolso.tsx:773 — handleEnviarSolicitacoes() |
| 11 | [Regressivo] Falha genérica no envio | erro sem lista estruturada | a requisição falha | `toast` comunica falha rápida sem travar a tela | estados de envio são revertidos adequadamente | InserirReembolso.tsx:773 — handleEnviarSolicitacoes() |
| 12 | [Positivo] Solicitação sem projeto habilitada | parâmetro `REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO` ativo via `useParametros` | preenche formulário sem vínculo obrigatório de projeto | validação de cliente/projeto não bloqueia o envio quando política permite | mantém demais regras de verba e comprovante | InserirReembolso.tsx:97 — InserirReembolso() |
| 13 | [Negativo] Valor acima do teto da verba | categoria com limite e valor informado ultrapassa teto | valida valor máximo antes de adicionar | operação é barrada com feedback de excedente | usa `validarValorMaximo` com dados da verba selecionada | InserirReembolso.tsx:401 — validarValorMaximo() |
| 14 | [Positivo] Máscara monetária consistente | usuário informa valores em campo monetário | o conteúdo do campo monetário é alterado | centavos e separadores obedecem `formatarValorMonetario` / `desformatarValorMonetario` | exibição usa `formatarValorParaExibicao` quando aplicável | InserirReembolso.tsx:339 — formatarValorMonetario() |
| 15 | [Regressivo] Encerramento pós-sucesso | `modalSucesso` está aberto após envio OK | confirma fechamento do modal | navegação retorna para `/reembolso` | estado efêmero do formulário é descartado ao sair | InserirReembolso.tsx:920 — handleFecharModalSucesso() |
