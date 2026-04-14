using Colaboracao.Core.Interfaces;
using Core.Domain.Financeiro.IntegracaoBancaria;
using Dapper;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RetornoBancaria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Financeiro.IntegracaoBancaria
{
    public class CnabRetornoRepository : ICnabRetornoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public CnabRetornoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<Guid> CriarRetorno(
            Guid remessaId,
            string hashRemessa,
            string nomeArquivo,
            string usuarioProcessamento)
        {
            var connection = _dapperConnection.GetConnection();
            var id = Guid.NewGuid();

            var query = @"
                INSERT INTO tb_cnab_retorno (
                    id,
                    tb_cnab_remessa_id,
                    hash_remessa,
                    nome_arquivo,
                    data_processamento,
                    usuario_processamento
                ) VALUES (
                    @Id,
                    @RemessaId,
                    @HashRemessa,
                    @NomeArquivo,
                    NOW(),
                    @UsuarioProcessamento
                )";

            await connection.ExecuteAsync(query, new
            {
                Id = id,
                RemessaId = remessaId,
                HashRemessa = hashRemessa,
                NomeArquivo = nomeArquivo,
                UsuarioProcessamento = usuarioProcessamento
            });

            return id;
        }

        public async Task<Guid> CriarRetornoItem(
            Guid retornoId,
            string conteudoLinha,
            int ordem,
            string codigoOcorrencia,
            string tipoRegistro)
        {
            var connection = _dapperConnection.GetConnection();
            var id = Guid.NewGuid();

            var query = @"
                INSERT INTO tb_cnab_retorno_item (
                    id,
                    tb_cnab_retorno_id,
                    conteudo_linha,
                    ordem,
                    codigo_ocorrencia,
                    tipo_registro,
                    data_criacao
                ) VALUES (
                    @Id,
                    @RetornoId,
                    @ConteudoLinha,
                    @Ordem,
                    @CodigoOcorrencia,
                    @TipoRegistro,
                    NOW()
                )";

            await connection.ExecuteAsync(query, new
            {
                Id = id,
                RetornoId = retornoId,
                ConteudoLinha = conteudoLinha,
                Ordem = ordem,
                CodigoOcorrencia = codigoOcorrencia,
                TipoRegistro = tipoRegistro
            });

            return id;
        }

        public async Task<Guid> BuscarRemessaIdPorHash(string hashRemessa, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT id
                FROM tb_cnab_remessa
                WHERE hash_remessa = @HashRemessa
                AND tb_org_id = @OrgId
                LIMIT 1";

            var result = await connection.QueryFirstOrDefaultAsync<Guid>(query, new { HashRemessa = hashRemessa, OrgId = orgId });
            return result;
        }

        public async Task<List<string>> BuscarPagamentosCnabPorRemessa(Guid remessaId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT tcpc.id
                FROM tb_colaborador_pagamento_cnab tcpc
                INNER JOIN tb_cnab_remessa_item tcri ON tcpc.tb_cnab_remessa_item_id = tcri.id
                WHERE tcri.tb_cnab_remessa_id = @RemessaId";

            var result = await connection.QueryAsync<string>(query, new { RemessaId = remessaId });
            return result.ToList();
        }

        public async Task<List<DataTransferObject.Domain.Financeiro.IntegracaoBancaria.Externo.ReembolsoPagoExternoDTO>> BuscarReembolsosPagosViaCNAB(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                 SELECT DISTINCT
                    tco.cod_colaborador_externo AS CdProfissional,
                    tsr.tb_projeto_id AS CdProjeto,
                    MONTH(tsp.data_solicitacao) AS DtMesReferencia,
                    YEAR(tsp.data_solicitacao) AS DtAnoReferencia,
                    tsr.tb_verba_id AS CdTipoReembolso,
                    COALESCE(tsr.descricao, 'Reembolso') AS DsAdicionalReembolso,
                    tsr.data_despesa AS DtNotaFiscal,
                    tcpcsp.valor_pagamento AS VlReembolso,
                    'BRL' AS CodigoMoeda
                FROM tb_colaborador_pagamento_cnab tcpc
                INNER JOIN tb_cnab_remessa_item tcri ON tcpc.tb_cnab_remessa_item_id = tcri.id
                INNER JOIN tb_cnab_remessa tcr ON tcri.tb_cnab_remessa_id = tcr.id
                INNER JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = tcpc.codigo_interno_colaborador and tcpc.tb_org_id = tco.tb_org_id
                INNER JOIN tb_colaborador_pagamento_cnab_solicitacao_pagamento tcpcsp ON tcpcsp.tb_colaborador_pagamento_cnab_id = tcpc.id
                INNER JOIN tb_solicitacao_pagamento tsp ON tsp.id = tcpcsp.tb_solicitacao_pagamento_id
                INNER JOIN tb_solicitacao_reembolso tsr ON tsp.tb_solicitacao_reembolso_id = tsr.id
                WHERE tcpc.status_cnab = 'PAGO'
                    AND tcpc.tb_org_id = @OrgId
                ORDER BY tcr.data_criacao DESC";

            var result = await connection.QueryAsync<DataTransferObject.Domain.Financeiro.IntegracaoBancaria.Externo.ReembolsoPagoExternoDTO>(
                query,
                new { OrgId = orgId });

            return result.ToList();
        }

        public async Task AtualizarNomeArquivoRetorno(Guid retornoId, string urlArquivo)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_cnab_retorno
                SET nome_arquivo = @UrlArquivo
                WHERE id = @RetornoId";

            await connection.ExecuteAsync(query, new
            {
                RetornoId = retornoId,
                UrlArquivo = urlArquivo
            });
        }

        public async Task<List<int>> BuscarSolicitacoesPagamentoPorRemessa(Guid remessaId, string statusCnab)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT DISTINCT tcpcsp.tb_solicitacao_pagamento_id
                FROM tb_colaborador_pagamento_cnab tcpc
                INNER JOIN tb_cnab_remessa_item tcri ON tcpc.tb_cnab_remessa_item_id = tcri.id
                INNER JOIN tb_colaborador_pagamento_cnab_solicitacao_pagamento tcpcsp ON tcpcsp.tb_colaborador_pagamento_cnab_id = tcpc.id
                WHERE tcri.tb_cnab_remessa_id = @RemessaId
                AND tcpc.status_cnab = @StatusCnab";

            var result = await connection.QueryAsync<int>(query, new { RemessaId = remessaId, StatusCnab = statusCnab });
            return result.ToList();
        }
        
        public async Task<List<Guid>> BuscarSolicitacoesPagamentoNFPorRemessa(Guid remessaId, string statusCnab)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT DISTINCT tcpcsp.tb_nota_fiscal_id
                FROM tb_colaborador_pagamento_cnab tcpc
                INNER JOIN tb_cnab_remessa_item tcri ON tcpc.tb_cnab_remessa_item_id = tcri.id
                INNER JOIN tb_colaborador_pagamento_cnab_nota_fiscal tcpcsp ON tcpcsp.tb_colaborador_pagamento_cnab_id = tcpc.id
                WHERE tcri.tb_cnab_remessa_id = @RemessaId
                AND tcpc.status_cnab = @StatusCnab";

            var result = await connection.QueryAsync<Guid>(query, new { RemessaId = remessaId, StatusCnab = statusCnab });
            return result.ToList();
        }
        

        public async Task AtualizarStatusRemessa(Guid remessaId, string novoStatus)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_cnab_remessa
                SET status = @NovoStatus
                WHERE id = @RemessaId";

            await connection.ExecuteAsync(query, new
            {
                RemessaId = remessaId,
                NovoStatus = novoStatus
            });
        }

        public async Task<List<PagamentoCnabOrdenadoDTO>> 
        BuscarPagamentosCnabOrdenadosPorRemessa(Guid remessaId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tcpc.id AS Id,
                    tcpc.hash_pagamento AS HashPagamento
                FROM tb_colaborador_pagamento_cnab tcpc
                INNER JOIN tb_cnab_remessa_item tcri ON tcpc.tb_cnab_remessa_item_id = tcri.id
                WHERE tcri.tb_cnab_remessa_id = @RemessaId
                ORDER BY tcpc.data_criacao ASC";

            var result = await connection.QueryAsync<PagamentoCnabOrdenadoDTO>(query, new { RemessaId = remessaId });
            return result.ToList();
        }

        public async Task<List<int>> BuscarReembolsosPorRemessa(Guid remessaId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT DISTINCT tsp.tb_solicitacao_reembolso_id
                FROM tb_colaborador_pagamento_cnab tcpc
                INNER JOIN tb_cnab_remessa_item tcri ON tcpc.tb_cnab_remessa_item_id = tcri.id
                INNER JOIN tb_colaborador_pagamento_cnab_solicitacao_pagamento tcpcsp ON tcpcsp.tb_colaborador_pagamento_cnab_id = tcpc.id
                INNER JOIN tb_solicitacao_pagamento tsp ON tsp.id = tcpcsp.tb_solicitacao_pagamento_id
                WHERE tcri.tb_cnab_remessa_id = @RemessaId
                    AND tcpc.status_cnab = 'PAGO'
                    AND tsp.tb_solicitacao_reembolso_id IS NOT NULL";

            var result = await connection.QueryAsync<int>(query, new { RemessaId = remessaId });
            return result.ToList();
        }

        public async Task AtualizarStatusReembolso(int reembolsoId, int statusId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_solicitacao_reembolso
                SET status_id = @StatusId
                WHERE id = @ReembolsoId";

            await connection.ExecuteAsync(query, new
            {
                ReembolsoId = reembolsoId,
                StatusId = statusId
            });
        }
    }
}
