using Colaboracao.Core.Interfaces;
using Core.Domain.Financeiro.IntegracaoBancaria;
using Dapper;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Financeiro.IntegracaoBancaria
{
    public class PagamentoCnabRepository : IPagamentoCnabRepository
    {
        private readonly IDBConnection _dapperConnection;

        public PagamentoCnabRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<DadosBancariosColaboradorDTO> BuscarDadosBancariosColaborador(string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    tcdb.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc.documento_colaborador AS CpfColaborador,
                    tc.nome_completo AS NomeColaborador,
                    tcdb.forma_pagamento AS FormaPagamento,
                    tcdb.codigo_banco_ted AS CodigoBanco,
                    tcdb.agencia_ted AS Agencia,
                    tcdb.agencia_dv_ted AS AgenciaDv,
                    tcdb.conta_ted AS Conta,
                    tcdb.conta_dv_ted AS ContaDv,
                    tcdb.chave_pix AS ChavePix,
                    tcdb.tipo_chave_pix AS TipoChavePix
                FROM tb_colaborador_dados_bancarios tcdb
                INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tcdb.codigo_interno_colaborador
                WHERE tcdb.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND tcdb.tb_org_id = @OrgId";

            var result = await connection.QueryFirstOrDefaultAsync<DadosBancariosColaboradorDTO>(query, new
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                OrgId = orgId
            });

            return result;
        }

        public async Task<string> CriarColaboradorPagamentoCnab(
            int orgId,
            string codigoInternoColaborador,
            string cnabRemessaItemId,
            decimal valor,
            string formaPagamento,
            string codigoBanco,
            string agencia,
            string agenciaDv,
            string conta,
            string contaDv,
            string chavePix,
            string tipoChavePix,
            string statusCnab)
        {
            var connection = _dapperConnection.GetConnection();
            var id = Guid.NewGuid().ToString();
            var hashPagamento = Guid.NewGuid().ToString();

            var query = @"
                INSERT INTO tb_colaborador_pagamento_cnab (
                    id,
                    tb_org_id,
                    codigo_interno_colaborador,
                    tb_cnab_remessa_item_id,
                    valor_pagamento_total,
                    forma_pagamento,
                    codigo_banco,
                    agencia,
                    agencia_dv,
                    conta,
                    conta_dv,
                    chave_pix,
                    tipo_chave_pix,
                    status_cnab,
                    hash_pagamento,
                    data_criacao,
                    data_atualizacao
                ) VALUES (
                    @Id,
                    @OrgId,
                    @CodigoInternoColaborador,
                    @CnabRemessaItemId,
                    @Valor,
                    @FormaPagamento,
                    @CodigoBanco,
                    @Agencia,
                    @AgenciaDv,
                    @Conta,
                    @ContaDv,
                    @ChavePix,
                    @TipoChavePix,
                    @StatusCnab,
                    @HashPagamento,
                    NOW(),
                    NOW()
                )";

            await connection.ExecuteAsync(query, new
            {
                Id = id,
                OrgId = orgId,
                CodigoInternoColaborador = codigoInternoColaborador,
                CnabRemessaItemId = cnabRemessaItemId,
                Valor = valor,
                FormaPagamento = formaPagamento,
                CodigoBanco = codigoBanco,
                Agencia = agencia,
                AgenciaDv = agenciaDv,
                Conta = conta,
                ContaDv = contaDv,
                ChavePix = chavePix,
                TipoChavePix = tipoChavePix,
                StatusCnab = statusCnab,
                HashPagamento = hashPagamento
            });

            return id;
        }

        public async Task CriarRelacaoComSolicitacaoPagamento(
            string pagamentoCnabId,
            string solicitacaoPagamentoId,
            decimal valorPagamento,
            string tipoRemessa)
        {
            var connection = _dapperConnection.GetConnection();
            var id = Guid.NewGuid().ToString();
            var query = GetInsertRelacaoComSolicitacaoDePagamento(tipoRemessa);

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("PagamentoCnabId", pagamentoCnabId);
            parameters.Add("ValorPagamento", valorPagamento);

            if (tipoRemessa == "REEMBOLSO")
            {
                parameters.Add(
                    "SolicitacaoPagamentoIdInt",
                    int.Parse(solicitacaoPagamentoId),
                    DbType.Int32);
            }
            else // NF
            {
                parameters.Add(
                    "SolicitacaoPagamentoIdChar",
                    solicitacaoPagamentoId,
                    DbType.String);
            }

            await connection.ExecuteAsync(query, parameters);
        }

        private string GetInsertRelacaoComSolicitacaoDePagamento(string tipoRemessa)
        {
            return tipoRemessa switch
            {
                "REEMBOLSO" => @"
                    INSERT INTO tb_colaborador_pagamento_cnab_solicitacao_pagamento (
                        id,
                        tb_colaborador_pagamento_cnab_id,
                        tb_solicitacao_pagamento_id,
                        valor_pagamento
                    ) VALUES (
                        @Id,
                        @PagamentoCnabId,
                        @SolicitacaoPagamentoIdInt,
                        @ValorPagamento
                    )
                ",
                        "NF" => @"
                    INSERT INTO tb_colaborador_pagamento_cnab_nota_fiscal (
                        id,
                        tb_colaborador_pagamento_cnab_id,
                        tb_nota_fiscal_id,
                        valor_pagamento
                    ) VALUES (
                        @Id,
                        @PagamentoCnabId,
                        @SolicitacaoPagamentoIdChar,
                        @ValorPagamento
                    )
                "
            };
        }

        public async Task AtualizarStatusPagamentoCnab(Guid pagamentoCnabId, string novoStatus, string codigoOcorrencia = null, string descricaoErro = null)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_colaborador_pagamento_cnab
                SET status_cnab = @NovoStatus,
                    codigo_ocorrencia = @CodigoOcorrencia,
                    data_atualizacao = NOW(),
                    descricao_erro = @DescricaoErro
                WHERE id = @PagamentoCnabId";

            await connection.ExecuteAsync(query, new
            {
                PagamentoCnabId = pagamentoCnabId,
                NovoStatus = novoStatus,
                CodigoOcorrencia = codigoOcorrencia,
                DescricaoErro = descricaoErro
            });
        }

        public async Task AtualizarStatusSolicitacaoPagamento(int solicitacaoPagamentoId, DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo.StatusSolicitacaoPagamentoEnum status)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_solicitacao_pagamento
                SET tb_status_solicitacao_pagamento_id = @Status
                WHERE id = @Id";

            await connection.ExecuteAsync(query, new
            {
                Id = solicitacaoPagamentoId,
                Status = status
            });
        }
    }
}
