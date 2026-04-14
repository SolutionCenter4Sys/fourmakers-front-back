using Colaboracao.Core.Interfaces;
using Core.Domain.Financeiro.IntegracaoBancaria;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria;

namespace Colaboracao.Infra.Repositories.Financeiro.IntegracaoBancaria
{
    public class CnabRemessaRepository : ICnabRemessaRepository
    {
        private readonly IDBConnection _dapperConnection;

        public CnabRemessaRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<string> CriarRemessa(
            string hashRemessa,
            string nomeArquivo,
            string modeloCnab,
            string tipo,
            string codDiretoria,
            string status,
            string codigoInternoColaboradorCriacao,
            int orgId,
            string codigoBanco,
            string agencia,
            string agenciaDv,
            string conta,
            string contaDv,
            string codigoConvenio,
            string descricao,
            decimal valorTotal)
        {
            var connection = _dapperConnection.GetConnection();
            var id = Guid.NewGuid().ToString();

            var query = @"
                INSERT INTO tb_cnab_remessa (
                    id,
                    codigo_banco,
                    agencia,
                    agencia_dv,
                    conta,
                    conta_dv,
                    codigo_convenio,
                    descricao,
                    hash_remessa,
                    nome_arquivo,
                    modelo_cnab,
                    tipo,
                    cod_diretoria,
                    status,
                    codigo_interno_colaborador_criacao,
                    tb_org_id,
                    valor_total,
                    data_criacao
                ) VALUES (
                    @Id,
                    @CodigoBanco,
                    @Agencia,
                    @AgenciaDv,
                    @Conta,
                    @ContaDv,
                    @CodigoConvenio,
                    @Descricao,
                    @HashRemessa,
                    @NomeArquivo,
                    @ModeloCnab,
                    @Tipo,
                    @CodDiretoria,
                    @Status,
                    @CodigoInternoColaboradorCriacao,
                    @OrgId,
                    @ValorTotal,
                    NOW()
                )";

            await connection.ExecuteAsync(query, new
            {
                Id = id,
                CodigoBanco = codigoBanco,
                Agencia = agencia,
                AgenciaDv = agenciaDv,
                Conta = conta,
                ContaDv = contaDv,
                CodigoConvenio = codigoConvenio,
                Descricao = descricao ?? "",
                HashRemessa = hashRemessa,
                NomeArquivo = nomeArquivo,
                ModeloCnab = modeloCnab,
                Tipo = tipo,
                CodDiretoria = codDiretoria ?? "",
                Status = status,
                CodigoInternoColaboradorCriacao = codigoInternoColaboradorCriacao,
                OrgId = orgId,
                ValorTotal = valorTotal
            });

            return id;
        }

        public async Task<string> CriarRemessaItem(string remessaId, string conteudoLinha, int ordem, string tipoRegistro)
        {
            var connection = _dapperConnection.GetConnection();
            var id = Guid.NewGuid().ToString();

            var query = @"
                INSERT INTO tb_cnab_remessa_item (
                    id,
                    tb_cnab_remessa_id,
                    conteudo_linha,
                    ordem,
                    tipo_registro,
                    data_criacao
                ) VALUES (
                    @Id,
                    @RemessaId,
                    @ConteudoLinha,
                    @Ordem,
                    @TipoRegistro,
                    NOW()
                )";

            await connection.ExecuteAsync(query, new
            {
                Id = id,
                RemessaId = remessaId,
                ConteudoLinha = conteudoLinha,
                Ordem = ordem,
                TipoRegistro = tipoRegistro
            });

            return id;
        }

        public async Task AtualizarStatusRemessa(string remessaId, string status, string codigoInternoColaboradorAlteracao)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_cnab_remessa
                SET status = @Status,
                    codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao
                WHERE id = @RemessaId";

            await connection.ExecuteAsync(query, new
            {
                RemessaId = remessaId,
                Status = status,
                CodigoInternoColaboradorAlteracao = codigoInternoColaboradorAlteracao
            });
        }

        public async Task AtualizarNomeArquivo(string remessaId, string nomeArquivoRemessa, string codigoInternoColaboradorAlteracao)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE
                    tb_cnab_remessa
                SET
                    nome_arquivo = @NomeArquivo,
                    codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao
                WHERE
                    id = @RemessaId";

            await connection.ExecuteAsync(query, new
            {
                NomeArquivo = nomeArquivoRemessa,
                RemessaId = remessaId,
                CodigoInternoColaboradorAlteracao = codigoInternoColaboradorAlteracao
            });
        }

        public async Task<List<string>> BuscarItensRemessa(string remessaId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT conteudo_linha
                FROM tb_cnab_remessa_item
                WHERE tb_cnab_remessa_id = @RemessaId
                ORDER BY ordem";

            var result = await connection.QueryAsync<string>(query, new { RemessaId = remessaId });
            return result.ToList();
        }

        public async Task<List<string>> BuscarRemessaItemSegmentoAPorRemessaId(Guid remessaId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    conteudo_linha
                FROM tb_cnab_remessa_item
                WHERE tb_cnab_remessa_id = @RemessaId
                AND tipo_registro = 'DETALHE_SEGMENTO_A'
                ORDER BY ordem ASC
            ";
            var parametros = new
            {
                RemessaId = remessaId
            };
            
            var resultado = await connection.QueryAsync<string>(query, parametros);
            return resultado.ToList();
        }

        public async Task<DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria.ListarRemessasCnabResult> ListarRemessas(
            int orgId,
            string tipoRemessa,
            string mesAnoProcessamento = null,
            string status = null,
            List<string> ids = null)
        {
            var connection = _dapperConnection.GetConnection();

            var whereConditions = new List<string> { "tcr.tb_org_id = @OrgId" };
            if (!string.IsNullOrEmpty(mesAnoProcessamento))
                whereConditions.Add("DATE_FORMAT(tcr.data_criacao, '%m/%Y') = @MesAnoProcessamento");
            if (!string.IsNullOrEmpty(status))
                whereConditions.Add("tcr.status = @Status");
            if (ids?.Count > 0)
                whereConditions.Add("tcr.id IN @Ids");
            
            whereConditions.Add("tcr.tipo = @TipoRemessa");

            var whereClause = string.Join(" AND ", whereConditions);

            var query = GetRemessaQueryPorTipo(tipoRemessa, whereClause);

            var remessasDictionary = new Dictionary<Guid, DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria.RemessaCnabDTO>();
            var lancamentosDictionary = new Dictionary<Guid, DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria.LancamentoCnabDTO>();

            await connection.QueryAsync<
                DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria.RemessaCnabDTO,
                DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria.LancamentoCnabDTO,
                DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria.SolicitacaoPagamentoCnabDTO,
                DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria.RemessaCnabDTO>(
                query,
                (remessa, lancamento, solicitacao) =>
                {
                    if (!remessasDictionary.TryGetValue(remessa.Id, out var remessaExistente))
                    {
                        remessaExistente = remessa;
                        remessaExistente.Lancamentos = new List<DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria.LancamentoCnabDTO>();
                        remessasDictionary.Add(remessa.Id, remessaExistente);
                    }

                    if (lancamento?.Id != null)
                    {
                        if (!lancamentosDictionary.TryGetValue(lancamento.Id, out var lancamentoExistente))
                        {
                            lancamentoExistente = lancamento;
                            lancamentoExistente.Solicitacoes = new List<DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria.SolicitacaoPagamentoCnabDTO>();
                            remessaExistente.Lancamentos.Add(lancamentoExistente);
                            lancamentosDictionary.Add(lancamento.Id, lancamentoExistente);
                        }

                        if (solicitacao.Id != null)
                        {
                            lancamentoExistente.Solicitacoes.Add(solicitacao);
                        }
                    }

                    return remessaExistente;
                },
                new { OrgId = orgId, MesAnoProcessamento = mesAnoProcessamento, Status = status, Ids = ids, TipoRemessa = tipoRemessa },
                splitOn: "Id,Id,Id");

            var result = new DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria.ListarRemessasCnabResult
            {
                Remessas = remessasDictionary.Values.ToList(),
                TotalRemessas = remessasDictionary.Count
            };

            return result;
        }

        private string GetRemessaQueryPorTipo(string tipoRemessa, string whereClause)
        {
            var query = tipoRemessa switch
            {
                "REEMBOLSO" => $@"
                    SELECT
                        tcr.id AS Id,
                        tcr.hash_remessa AS HashRemessa,
                        tcr.tipo AS Tipo,
                        tcr.status AS Status,
                        tcr.nome_arquivo AS NomeArquivoRemessa,
                        tcret.nome_arquivo AS NomeArquivoRetorno,
                        tcr.valor_total AS ValorTotal,
                        tcr.data_criacao AS DataCriacao,
                        tcpc.id AS Id,
                        tcpc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tc.nome_completo AS NomeColaborador,
                        tc.documento_colaborador AS CpfColaborador,
                        tcpc.valor_pagamento_total AS ValorPagamentoTotal,
                        tcpc.forma_pagamento AS FormaPagamento,
                        tcpc.status_cnab AS StatusCnab,
                        tcpc.descricao_erro AS DescricaoErro,
                        tcpcsp.id AS Id,
                        tcr.tipo AS TipoSolicitacao,
                        tcpcsp.valor_pagamento AS ValorPagamento,
                        tsr.descricao AS Descricao,
                        tsp.data_solicitacao AS DataSolicitacao
                    FROM tb_cnab_remessa tcr
                    LEFT JOIN tb_cnab_retorno tcret ON tcret.tb_cnab_remessa_id = tcr.id
                    LEFT JOIN tb_cnab_remessa_item tcri ON tcri.tb_cnab_remessa_id = tcr.id
                    LEFT JOIN tb_colaborador_pagamento_cnab tcpc ON tcpc.tb_cnab_remessa_item_id = tcri.id
                    LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tcpc.codigo_interno_colaborador
                    LEFT JOIN tb_colaborador_pagamento_cnab_solicitacao_pagamento tcpcsp ON tcpcsp.tb_colaborador_pagamento_cnab_id = tcpc.id
                    LEFT JOIN tb_solicitacao_pagamento tsp ON tsp.id = tcpcsp.tb_solicitacao_pagamento_id
                    LEFT JOIN tb_solicitacao_reembolso tsr ON tsp.tb_solicitacao_reembolso_id = tsr.id
                    WHERE {whereClause}
                    ORDER BY tcr.data_criacao DESC, tcpc.codigo_interno_colaborador, tcpcsp.id
                    ",
                "NF" => $@"
                    SELECT
                        tcr.id AS Id,
                        tcr.hash_remessa AS HashRemessa,
                        tcr.tipo AS Tipo,
                        tcr.status AS Status,
                        tcr.nome_arquivo AS NomeArquivoRemessa,
                        tcret.nome_arquivo AS NomeArquivoRetorno,
                        tcr.valor_total AS ValorTotal,
                        tcr.data_criacao AS DataCriacao,
                        tcpc.id AS Id,
                        tcpc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tc.nome_completo AS NomeColaborador,
                        tc.documento_colaborador AS CpfColaborador,
                        tcpc.valor_pagamento_total AS ValorPagamentoTotal,
                        tcpc.forma_pagamento AS FormaPagamento,
                        tcpc.status_cnab AS StatusCnab,
                        tcpc.descricao_erro AS DescricaoErro,
                        tcpcsp.id AS Id,
                        tcr.tipo AS TipoSolicitacao,
                        tcpcsp.valor_pagamento AS ValorPagamento,
                        tnf.numero_nf AS Descricao,
                        tnf.data_criacao AS DataSolicitacao
                    FROM tb_cnab_remessa tcr
                    LEFT JOIN tb_cnab_retorno tcret ON tcret.tb_cnab_remessa_id = tcr.id
                    LEFT JOIN tb_cnab_remessa_item tcri ON tcri.tb_cnab_remessa_id = tcr.id
                    LEFT JOIN tb_colaborador_pagamento_cnab tcpc ON tcpc.tb_cnab_remessa_item_id = tcri.id
                    LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tcpc.codigo_interno_colaborador
                    LEFT JOIN tb_colaborador_pagamento_cnab_nota_fiscal tcpcsp ON tcpcsp.tb_colaborador_pagamento_cnab_id = tcpc.id
                    LEFT JOIN tb_nota_fiscal tnf ON tnf.id = tcpcsp.tb_nota_fiscal_id 
                    WHERE {whereClause}
                    ORDER BY tcr.data_criacao DESC, tcpc.codigo_interno_colaborador, tcpcsp.id
                "
            };
            return query;
        }

        public async Task<int> ObterProximoNumeroSequencial(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT COALESCE(MAX(CAST(hash_remessa AS UNSIGNED)), 0) + 1
                FROM tb_cnab_remessa
                WHERE tb_org_id = @OrgId
                  AND hash_remessa REGEXP '^[0-9]+$'";

            var resultado = await connection.QueryFirstOrDefaultAsync<int>(query, new { OrgId = orgId });
            return resultado == 0 ? 1 : resultado;
        }
        
        public async Task<List<SolicitacaoRemessaDTO>> BuscarSolicitacoesPagamentos(int orgId, string tipoRemessa)
        {
            var connection = _dapperConnection.GetConnection();
            var query = GetSolicitacaoQueryPorTipoRemessa(tipoRemessa);
            var parametros = new
            {
                OrgId = orgId
            };
        
            var result = await connection.QueryAsync<SolicitacaoRemessaDTO>(query, parametros);
            return result.ToList();
        }

        private string GetSolicitacaoQueryPorTipoRemessa(string tipoRemessa)
        {
            var query = tipoRemessa switch
            {
                "REEMBOLSO" => @"
                    SELECT DISTINCT
                        tsp.id AS Id,
                        tsp.valor AS ValorParaPagamento,
                        tco.codigo_interno_colaborador AS CodigoColaborador,
                        tc.nome_completo AS Nome,
                        tco.cod_diretoria AS CodDiretoria,
                        tpo.projeto AS Projeto,
                        tcliente.nome_cliente AS Cliente,
                        tsb.data_criacao AS DataSolicitacao
                    FROM tb_solicitacao_pagamento tsp
                    INNER JOIN tb_solicitacao_reembolso tsb ON tsb.id = tsp.tb_solicitacao_reembolso_id
                    INNER JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = tsb.codigo_interno_colaborador AND tco.tb_org_id = tsp.tb_org_id
                    INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                    LEFT JOIN tb_cliente_org tcliente ON tcliente.codigo_cliente = tsb.tb_cliente_id AND tcliente.tb_org_id = tsp.tb_org_id
                    LEFT JOIN tb_projeto_org tpo ON tpo.cod_projeto = tsb.tb_projeto_id AND tpo.tb_org_id = tsp.tb_org_id
                    WHERE tsp.tb_org_id = @OrgId
                    AND tsp.tb_status_solicitacao_pagamento_id = 1 -- AGUARDANDO PAGAMENTO",
                "NF" => @"
                    SELECT DISTINCT
                        CAST(tnf.id AS CHAR) AS Id,
                        tnf.valor AS ValorParaPagamento,
                        tnf.data_criacao AS DataSolicitacao,
                        tnf.codigo_interno_colaborador_criacao AS CodigoColaborador,
                        tc.nome_completo AS Nome,
                        tco.cod_diretoria AS CodDiretoria
                    FROM tb_nota_fiscal tnf
                    INNER JOIN tb_nota_fiscal_rubrica tnfr 
                        ON tnfr.tb_nota_fiscal_id = tnf.id 
                    INNER JOIN tb_rubrica_colaborador trc
                        ON trc.id = tnfr.tb_rubrica_colaborador_id 
                    INNER JOIN tb_colaborador tc
                        ON tc.codigo_interno_colaborador = trc.codigo_interno_colaborador
                    JOIN tb_colaborador_org tco
                        ON tco.codigo_interno_colaborador = trc.codigo_interno_colaborador AND tco.tb_org_id = tnf.tb_org_id
                    WHERE
                        tnf.tb_org_id = @OrgId
                    AND tnf.tb_nota_fiscal_status_id = 5 -- NF APROVADA
                ",
                _ => throw new ArgumentException("Tipo de remssa invalido")
            };

            return query;
        }
    }
}
