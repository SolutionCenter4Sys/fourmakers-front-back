/**
 * [DataForge] Dados de teste — Módulo de Reembolso (BDD v4)
 * Uma constante por cenário: campos do cabeçalho + item (Inserir Reembolso).
 * CPF/CNPJ/CEP/telefone válidos quando citados em descrições ou metadados auxiliares.
 */

// --- Reembolso (Dashboard) — cenários 1 a 15 ---

/** 1 [Positivo] Aba inicial via URL — tab compatível com mapa de abas */
export const bddReembolsoDashboard01 = {
  objetivo: 'Revisão de contratos trimestrais — holding Sul',
  destino: 'Florianópolis - SC',
  dataInicio: '02/04/2026',
  dataFim: '05/04/2026',
  categoria: 'Hospedagem',
  dataDespesa: '03/04/2026',
  valor: '1.089,40',
  quantidade: '1',
  descricao:
    'Hospedagem próximo ao cliente Construtora Litorânea S.A., CNPJ 45.678.910/0001-24, contato (48) 99102-7438.',
};

/** 2 [Negativo] Bloqueio de Gestão Administrativa — colaborador sem perfil gestor */
export const bddReembolsoDashboard02 = {
  objetivo: '',
  destino: '',
  dataInicio: '',
  dataFim: '',
  categoria: '',
  dataDespesa: '',
  valor: '',
  quantidade: '',
  descricao: '',
};

/** 3 [Positivo] Troca de aba com sincronismo na URL */
export const bddReembolsoDashboard03 = {
  objetivo: 'Alinhamento de OKRs com diretoria regional',
  destino: 'São Paulo - SP',
  dataInicio: '10/03/2026',
  dataFim: '12/03/2026',
  categoria: 'Alimentação',
  dataDespesa: '11/03/2026',
  valor: '187,65',
  quantidade: '1',
  descricao:
    'Almoço executivo Av. Paulista — NF emitida para CNPJ 73.195.846/0001-07, CEP entrega 01310-100.',
};

/** 4 [Negativo] Impedir aba Aprovações sem perfil */
export const bddReembolsoDashboard04 = {
  objetivo: 'Suporte pós-go-live sistema fiscal',
  destino: '',
  dataInicio: '',
  dataFim: '',
  categoria: 'Transporte',
  dataDespesa: '',
  valor: '',
  quantidade: '',
  descricao: '',
};

/** 5 [Positivo] Busca textual na grade principal */
export const bddReembolsoDashboard05 = {
  objetivo: 'Implantação módulo analytics — cliente Nexum Pagamentos',
  destino: 'Belo Horizonte - MG',
  dataInicio: '18/03/2026',
  dataFim: '20/03/2026',
  categoria: 'Taxi / Aplicativo',
  dataDespesa: '19/03/2026',
  valor: '54,90',
  quantidade: '1',
  descricao:
    'Corridas entre Savassi e Pampulha — motorista registrado, CPF titular 342.719.086-60.',
};

/** 6 [Regressivo] Reset de paginação ao filtrar */
export const bddReembolsoDashboard06 = {
  objetivo: 'Workshop segurança da informação — equipe interna',
  destino: 'Curitiba - PR',
  dataInicio: '01/04/2026',
  dataFim: '04/04/2026',
  categoria: 'Material de Escritório',
  dataDespesa: '02/04/2026',
  valor: '312,00',
  quantidade: '1',
  descricao: 'Kit periféricos para sala de treinamento — entrega CEP 80010-030.',
};

/** 7 [Positivo] Paginação por fatia da lista filtrada */
export const bddReembolsoDashboard07 = {
  objetivo: 'Visita técnica AgroVale Cooperativa',
  destino: 'Ribeirão Preto - SP',
  dataInicio: '25/03/2026',
  dataFim: '27/03/2026',
  categoria: 'Combustível',
  dataDespesa: '26/03/2026',
  valor: '265,80',
  quantidade: '1',
  descricao: 'Abastecimento rodoviário — posto conveniado nota fiscal série 42.',
};

/** 8 [Positivo] Modal de detalhes da solicitação */
export const bddReembolsoDashboard08 = {
  objetivo: 'Homologação integração ERP — Indústrias Bramante Ltda.',
  destino: 'Joinville - SC',
  dataInicio: '06/04/2026',
  dataFim: '09/04/2026',
  categoria: 'Pedágio',
  dataDespesa: '07/04/2026',
  valor: '42,30',
  quantidade: '1',
  descricao: 'Trecho BR-101 — tag corporativo vinculado ao projeto 8847.',
};

/** 9 [Positivo] Botão Gerar Relatório na Gestão */
export const bddReembolsoDashboard09 = {
  objetivo: 'Fechamento folha de pagamentos terceirizados',
  destino: 'Brasília - DF',
  dataInicio: '01/03/2026',
  dataFim: '31/03/2026',
  categoria: 'Estacionamento',
  dataDespesa: '15/03/2026',
  valor: '78,00',
  quantidade: '1',
  descricao: 'Estacionamento Setor Comercial Sul — período integral, CEP 70040-010.',
};

/** 10 [Negativo] Ocultar relatório fora da Gestão ou com parâmetro falso */
export const bddReembolsoDashboard10 = {
  objetivo: 'Capacitação LGPD — squad atendimento',
  destino: 'Fortaleza - CE',
  dataInicio: '14/02/2026',
  dataFim: '16/02/2026',
  categoria: 'Alimentação',
  dataDespesa: '15/02/2026',
  valor: '96,40',
  quantidade: '1',
  descricao: 'Coffee break hotel Praia de Iracema — contato (85) 97405-8293.',
};

/** 11 [Positivo] Download do relatório de pagamentos */
export const bddReembolsoDashboard11 = {
  objetivo: 'Auditoria despesas administrativas Q1',
  destino: 'Porto Alegre - RS',
  dataInicio: '01/01/2026',
  dataFim: '31/03/2026',
  categoria: 'Correios / Frete',
  dataDespesa: '28/02/2026',
  valor: '134,75',
  quantidade: '1',
  descricao: 'Envio documentos sede matriz — AR para CNPJ 62.415.037/0001-59.',
};

/** 12 [Negativo] Geração sem token */
export const bddReembolsoDashboard12 = {
  objetivo: 'Consolidação despesas evento anual',
  destino: 'Salvador - BA',
  dataInicio: '05/03/2026',
  dataFim: '08/03/2026',
  categoria: 'Hospedagem',
  dataDespesa: '06/03/2026',
  valor: '2.340,00',
  quantidade: '1',
  descricao: 'Hospedagem equipe comercial — hotel Rio Vermelho, CEP 41940-280.',
};

/** 13 [Regressivo] Falha na API do relatório */
export const bddReembolsoDashboard13 = {
  objetivo: 'Suporte implantação WMS — LogiPrime Sistemas',
  destino: 'Campinas - SP',
  dataInicio: '11/04/2026',
  dataFim: '13/04/2026',
  categoria: 'Transporte',
  dataDespesa: '12/04/2026',
  valor: '119,00',
  quantidade: '1',
  descricao: 'Trem intermunicipal + metrô — bilhetes corporativos série março/2026.',
};

/** 14 [Positivo] Botão Remessa CNAB condicionado */
export const bddReembolsoDashboard14 = {
  objetivo: 'Pagamento fornecedores evento TechRio Summit',
  destino: 'Rio de Janeiro - RJ',
  dataInicio: '20/03/2026',
  dataFim: '23/03/2026',
  categoria: 'Alimentação',
  dataDespesa: '22/03/2026',
  valor: '458,20',
  quantidade: '1',
  descricao: 'Coffee break auditório Barra — fornecedor CNPJ 29.403.718/0001-50.',
};

/** 15 [Positivo] Indicadores e listagem com período */
export const bddReembolsoDashboard15 = {
  objetivo: 'Roadmap produto — workshops com clientes piloto',
  destino: 'Recife - PE',
  dataInicio: '01/04/2026',
  dataFim: '13/04/2026',
  categoria: 'Material de Escritório',
  dataDespesa: '10/04/2026',
  valor: '203,45',
  quantidade: '1',
  descricao: 'Impressões e cartazes Boa Viagem — entrega CEP 51020-280.',
};

// --- Inserir Reembolso — cenários 1 a 15 ---

/** 1 [Positivo] Carga de projetos do colaborador */
export const bddReembolsoInserir01 = {
  objetivo: 'Descoberta comercial — carteira Nordeste',
  destino: 'Maceió - AL',
  dataInicio: '07/04/2026',
  dataFim: '09/04/2026',
  categoria: 'Hospedagem',
  dataDespesa: '08/04/2026',
  valor: '890,00',
  quantidade: '1',
  descricao: 'Diárias hotel Ponta Verde — responsável CPF 529.982.247-25.',
};

/** 2 [Positivo] Carga de verbas ao escolher projeto */
export const bddReembolsoInserir02 = {
  objetivo: 'Treinamento certificação Scrum — equipe projetos',
  destino: 'São José dos Campos - SP',
  dataInicio: '16/04/2026',
  dataFim: '18/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '17/04/2026',
  valor: '76,30',
  quantidade: '1',
  descricao: 'Almoço grupo 8 pessoas — restaurante Jardim Aquarius, CEP 12246-280.',
};

/** 3 [Negativo] Comprovante fora da validade — data da despesa excede validadeComprovanteDias */
export const bddReembolsoInserir03 = {
  objetivo: 'Deslocamento reunião diretoria 2023',
  destino: 'Goiânia - GO',
  dataInicio: '10/11/2023',
  dataFim: '12/11/2023',
  categoria: 'Combustível',
  dataDespesa: '05/06/2023',
  valor: '198,40',
  quantidade: '1',
  descricao: 'Abastecimento viagem anterior — comprovante fora da janela parametrizada.',
};

/** 4 [Positivo] Análise OCR de comprovantes */
export const bddReembolsoInserir04 = {
  objetivo: 'Suporte go-live fábrica automotiva',
  destino: 'Betim - MG',
  dataInicio: '01/04/2026',
  dataFim: '05/04/2026',
  categoria: 'Taxi / Aplicativo',
  dataDespesa: '03/04/2026',
  valor: '38,70',
  quantidade: '1',
  descricao: 'Corridas noturnas plantão — telefone motorista (31) 99102-7438.',
};

/** 5 [Negativo] Adicionar ao carrinho sem obrigatórios */
export const bddReembolsoInserir05 = {
  objetivo: '',
  destino: 'Manaus - AM',
  dataInicio: '',
  dataFim: '',
  categoria: '',
  dataDespesa: '',
  valor: '0,00',
  quantidade: '0',
  descricao: '',
};

/** 6 [Positivo] Item válido no carrinho */
export const bddReembolsoInserir06 = {
  objetivo: 'Kickoff projeto sustentabilidade — cliente VerdeAzul',
  destino: 'Vitória - ES',
  dataInicio: '21/04/2026',
  dataFim: '23/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '22/04/2026',
  valor: '124,90',
  quantidade: '1',
  descricao: 'Jantar equipe multidisciplinar — churrascaria Enseada do Suá, CEP 29010-150.',
};

/** 7 [Positivo] Edição de item existente */
export const bddReembolsoInserir07 = {
  objetivo: 'Kickoff projeto sustentabilidade — cliente VerdeAzul',
  destino: 'Vitória - ES',
  dataInicio: '21/04/2026',
  dataFim: '23/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '22/04/2026',
  valor: '139,50',
  quantidade: '1',
  descricao:
    'Jantar equipe (ajuste pós-edição) — inclusão de sobremesas e taxa serviço, CNPJ 84.051.927/0001-33.',
};

/** 8 [Positivo] Remoção de item do carrinho */
export const bddReembolsoInserir08 = {
  objetivo: 'Visita cliente varejo — rede MinasBom',
  destino: 'Uberlândia - MG',
  dataInicio: '28/04/2026',
  dataFim: '29/04/2026',
  categoria: 'Estacionamento',
  dataDespesa: '28/04/2026',
  valor: '22,00',
  quantidade: '1',
  descricao: 'Estacionamento shopping Uberlândia — ticket digital anexo ao ZIP.',
};

/** 9 [Positivo] Envio com ZIP e sucesso */
export const bddReembolsoInserir09 = {
  objetivo: 'Entrega sprint review presencial — fintech Horizonte',
  destino: 'São Paulo - SP',
  dataInicio: '14/04/2026',
  dataFim: '15/04/2026',
  categoria: 'Material de Escritório',
  dataDespesa: '14/04/2026',
  valor: '567,80',
  quantidade: '1',
  descricao: 'Flip charts e suprimentos sala Pinheiros — CEP 05407-002, contato (11) 3842-7619.',
};

/** 10 [Negativo] Resposta da API com lista de erros */
export const bddReembolsoInserir10 = {
  objetivo: 'Workshop design system — holding financeira',
  destino: 'Curitiba - PR',
  dataInicio: '09/05/2026',
  dataFim: '11/05/2026',
  categoria: 'Hospedagem',
  dataDespesa: '10/05/2026',
  valor: '3.120,00',
  quantidade: '1',
  descricao: 'Pacote hospedagem + sala breakout — hotel Batel, CPF solicitante 046.321.874-10.',
};

/** 11 [Regressivo] Falha genérica no envio */
export const bddReembolsoInserir11 = {
  objetivo: 'Migração dados legado — Indústrias Carmo',
  destino: 'Juiz de Fora - MG',
  dataInicio: '03/05/2026',
  dataFim: '06/05/2026',
  categoria: 'Transporte',
  dataDespesa: '04/05/2026',
  valor: '412,00',
  quantidade: '1',
  descricao: 'Passagens rodoviárias equipe técnica — viação com nota fiscal eletrônica.',
};

/** 12 [Positivo] Solicitação sem projeto habilitada */
export const bddReembolsoInserir12 = {
  objetivo: 'Despesa administrativa interna — calibração notebooks',
  destino: 'Belém - PA',
  dataInicio: '18/04/2026',
  dataFim: '18/04/2026',
  categoria: 'Material de Escritório',
  dataDespesa: '18/04/2026',
  valor: '289,99',
  quantidade: '1',
  descricao: 'Serviço técnico autorizado Nazaré — CEP 66010-090, sem vínculo projeto obrigatório.',
};

/** 13 [Negativo] Valor acima do teto da verba */
export const bddReembolsoInserir13 = {
  objetivo: 'Deslocamento rotina escritório regional',
  destino: 'Londrina - PR',
  dataInicio: '20/04/2026',
  dataFim: '20/04/2026',
  categoria: 'Estacionamento',
  dataDespesa: '20/04/2026',
  valor: '18.750,00',
  quantidade: '1',
  descricao: 'Valor propositalmente acima do teto da verba Estacionamento para validar validarValorMaximo.',
};

/** 14 [Positivo] Máscara monetária consistente */
export const bddReembolsoInserir14 = {
  objetivo: 'Consultoria precificação — distribuidora SulMinas',
  destino: 'Varginha - MG',
  dataInicio: '24/04/2026',
  dataFim: '26/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '25/04/2026',
  valor: '1.250,87',
  quantidade: '1',
  descricao: 'Refeições período consultoria — teste máscara milhar/centavos 1.250,87.',
};

/** 15 [Regressivo] Encerramento pós-sucesso */
export const bddReembolsoInserir15 = {
  objetivo: 'Fechamento escopo piloto RPA — seguradora Atlas',
  destino: 'Florianópolis - SC',
  dataInicio: '29/04/2026',
  dataFim: '30/04/2026',
  categoria: 'Combustível',
  dataDespesa: '30/04/2026',
  valor: '87,50',
  quantidade: '1',
  descricao: 'Combustível retorno aeroporto — CNPJ posto 16.839.274/0001-45, CEP 88010-400.',
};
