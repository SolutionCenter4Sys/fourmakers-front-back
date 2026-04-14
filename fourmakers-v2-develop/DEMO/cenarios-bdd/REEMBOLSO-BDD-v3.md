# Cenários BDD — Módulo de Reembolso

| Tipo | Qtd |
|------|-----|
| ✅ Positivo | 18 |
| ❌ Negativo | 8 |
| 🔁 Regressivo | 4 |
| **Total** | **30** |

## Reembolso (Dashboard)

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|--------------------------|--------------|---------------|--------------|----------|----------------------|
| R-01 | [Positivo] Aba padrão da listagem | um colaborador acessa o módulo de reembolso | a tela é carregada | a aba de despesas próprias é exibida por padrão | os indicadores resumidos são apresentados com estado de carregamento quando aplicável | Reembolso.tsx:101 — handleTabChange() |
| R-02 | [Positivo] Alternância entre abas permitidas | um gestor visualiza o painel de reembolso | solicita mudar para a aba de gestão administrativa | a listagem e os controles da gestão são exibidos | a URL reflete a aba selecionada | Reembolso.tsx:101 — handleTabChange() |
| R-03 | [Positivo] Aba de aprovações | um colaborador com permissão de aprovação acessa o módulo | solicita abrir a aba de aprovações | as solicitações pendentes de decisão são listadas | a navegação por parâmetro de URL mantém a aba correta | Reembolso.tsx:101 — handleTabChange() |
| R-04 | [Negativo] Aba sem permissão | um colaborador sem perfil de gestor está no painel | tenta acessar a aba de gestão administrativa | o sistema impede ou não exibe essa visão | permanece em uma aba autorizada | Reembolso.tsx:101 — handleTabChange() |
| R-05 | [Positivo] Filtro por período | um colaborador está na listagem de reembolsos | define intervalo de datas no calendário | os registros exibidos respeitam o período informado | o filtro permanece visível para ajuste | Reembolso.tsx:144 — filteredReembolsos() |
| R-06 | [Positivo] Busca textual na tabela | um colaborador possui várias solicitações listadas | informa termo no campo de busca | somente linhas compatíveis com o texto permanecem visíveis | a paginação continua disponível | Reembolso.tsx:144 — filteredReembolsos() |
| R-07 | [Positivo] Detalhes da solicitação | um colaborador visualiza uma linha com dados da solicitação | solicita abrir o painel de detalhes | o diálogo exibe informações e itens associados | a tabela de itens no detalhe está legível | Reembolso.tsx:216 — renderCell() |
| R-08 | [Positivo] Documentos anexados | um colaborador consulta uma solicitação com comprovantes | solicita visualizar documentos | o modal de documentos é aberto | os anexos podem ser consultados conforme regra de negócio | Reembolso.tsx:216 — renderCell() |
| R-09 | [Positivo] Nova solicitação | um colaborador está no painel de reembolso | escolhe iniciar nova solicitação | é direcionado ao fluxo de inclusão de reembolso | a rota de inclusão é carregada | Reembolso.tsx:216 — renderCell() |
| R-10 | [Positivo] Gerar relatório (gestão) | um gestor está na aba de gestão administrativa e o parâmetro comercial habilita a ação | confirma a geração do relatório no alerta | o arquivo é obtido e disponibilizado para download | o fluxo encerra sem travar a tela | Reembolso.tsx:169 — handleGerarRelatorio() |
| R-11 | [Negativo] Relatório indisponível | um colaborador comum está na visão padrão | não há botão ou fluxo de relatório de pagamentos | nenhum download administrativo é iniciado | a experiência permanece focada em despesas próprias | Reembolso.tsx:169 — handleGerarRelatorio() |
| R-12 | [Positivo] Remessa CNAB (gestão) | um gestor está na aba administrativa e o parâmetro habilita a remessa | aciona a geração de remessa CNAB | o processo correspondente é disparado | feedback adequado é apresentado ao usuário | Reembolso.tsx:216 — renderCell() |
| R-13 | [Negativo] Remessa CNAB oculta | o parâmetro de remessa está desativado | um gestor navega pela gestão administrativa | o atalho de remessa não aparece | não há tentativa de geração por interface | Reembolso.tsx:216 — renderCell() |
| R-14 | [Regressivo] Sincronização de aba pela URL | um colaborador com permissões abre o endereço com parâmetro de aba | a página carrega com o parâmetro informado | a aba correta fica ativa | mudanças de aba atualizam a URL | Reembolso.tsx:101 — handleTabChange() |
| R-15 | [Regressivo] Cartões de estatística após carga | os dados do painel terminam de carregar | o colaborador permanece na mesma aba | os cartões deixam o esqueleto e mostram valores | não há inconsistência entre totais e tabela vazia indevida | Reembolso.tsx:216 — renderCell() |

## Inserir Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|--------------------------|--------------|---------------|--------------|----------|----------------------|
| I-01 | [Positivo] Preenchimento do cabeçalho | um colaborador inicia uma nova solicitação | informa objetivo, destino opcional e datas de viagem válidas | o cabeçalho fica pronto para inclusão de itens | campos obrigatórios do topo são respeitados | InserirReembolso.tsx:514 — handleLimpar() |
| I-02 | [Negativo] Objetivo ou datas ausentes | um colaborador tenta incluir item sem completar o cabeçalho | solicita adicionar ao carrinho sem preencher obrigatórios | o sistema bloqueia a inclusão | mensagem orienta corrigir o cabeçalho | InserirReembolso.tsx:543 — handleAdicionarCarrinho() |
| I-03 | [Positivo] Item completo no carrinho | um colaborador preenche cliente ou projeto, categoria, data da despesa, valor e descrição | confirma adicionar ao carrinho | o item aparece na lista de solicitações | o formulário de item pode ser reutilizado para novo lançamento | InserirReembolso.tsx:543 — handleAdicionarCarrinho() |
| I-04 | [Negativo] Valor acima do teto da verba | um colaborador informa valor que ultrapassa o limite da verba selecionada | tenta adicionar o item | a inclusão é recusada | orientação sobre o limite é apresentada | InserirReembolso.tsx:401 — validarValorMaximo() |
| I-05 | [Negativo] Comprovante fora do prazo | a política define prazo máximo para data do comprovante | o colaborador informa data de despesa fora da validade | o sistema impede prosseguir com aquele comprovante | o prazo permitido fica claro na mensagem | InserirReembolso.tsx:305 — validarDataComprovante() |
| I-06 | [Positivo] Análise automática de comprovantes | um colaborador anexa comprovantes elegíveis | solicita análise dos arquivos | data, quantidade e valores sugeridos são preenchidos quando possível | o colaborador pode ajustar antes de confirmar | InserirReembolso.tsx:419 — analisarComprovantes() |
| I-07 | [Positivo] Formatação monetária | um colaborador informa valores em centavos ou formato monetário esperado | os campos de valor são atualizados | a exibição segue o padrão brasileiro de moeda | reduz erros de leitura no total | InserirReembolso.tsx:339 — formatarValorMonetario() |
| I-08 | [Positivo] Edição de item do carrinho | existem itens já incluídos | o colaborador escolhe editar um item | o formulário é preenchido com os dados daquele item | após salvar, a lista reflete a alteração | InserirReembolso.tsx:681 — handleEditarItem() |
| I-09 | [Positivo] Exclusão de item | o carrinho contém mais de um lançamento | o colaborador remove um item específico | o item deixa de constar na lista | totais e resumo são recalculados | InserirReembolso.tsx:767 — handleExcluirItem() |
| I-10 | [Positivo] Envio da solicitação | o carrinho possui itens válidos e o cabeçalho está completo | o colaborador confirma o envio | o pacote com dados e anexos é montado e transmitido | modal de sucesso confirma o recebimento | InserirReembolso.tsx:773 — handleEnviarSolicitacoes() |
| I-11 | [Negativo] Envio com carrinho vazio | não há itens na lista de solicitações | o colaborador tenta enviar | o envio não é realizado | feedback indica incluir ao menos um item | InserirReembolso.tsx:773 — handleEnviarSolicitacoes() |
| I-12 | [Negativo] Falha no envio | o serviço de envio retorna erro | o colaborador confirma o envio | modal de erro descreve a falha | o colaborador pode corrigir dados e tentar novamente | InserirReembolso.tsx:773 — handleEnviarSolicitacoes() |
| I-13 | [Positivo] Limpar formulário de item | o colaborador preencheu parcialmente um item | solicita limpar o formulário | os campos do item voltam ao estado inicial | o carrinho existente não é apagado sem ação explícita | InserirReembolso.tsx:514 — handleLimpar() |
| I-14 | [Regressivo] Item por quantidade e unidade | a categoria exige informar quantidade e unidade | o colaborador preenche todos os campos numéricos obrigatórios | o item é aceito no carrinho | o valor total do item é coerente com quantidade e unitário | InserirReembolso.tsx:543 — handleAdicionarCarrinho() |
| I-15 | [Regressivo] Item somente por valor total | a categoria usa apenas valor consolidado | o colaborador informa valor e descrição sem quantidade | o item é aceito no carrinho | não há exigência indevida de quantidade | InserirReembolso.tsx:543 — handleAdicionarCarrinho() |
