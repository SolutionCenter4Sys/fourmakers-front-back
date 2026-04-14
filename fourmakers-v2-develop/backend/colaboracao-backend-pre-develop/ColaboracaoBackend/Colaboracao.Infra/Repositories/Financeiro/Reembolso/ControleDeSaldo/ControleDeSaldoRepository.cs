using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Reembolso.ControleDeSaldo;
using Dapper;
using DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;

namespace Colaboracao.Infra.Repositories.Financeiro.Reembolso.ControleDeSaldo;

public class ControleDeSaldoRepository(IDBConnection dapperConnection) : IControleDeSaldoRepository
{
    public async Task<decimal> BuscarSaldoColaborador(string codigoInternoColaborador, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT
                valor
            FROM tb_reembolso_saldo_colaborador 
            WHERE 
                codigo_interno_colaborador = @CodigoInternoColaborador
                AND tb_org_id = @OrgId;
        ";
        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador,
            OrgId = orgId
        };

        var result = await connection.QuerySingleOrDefaultAsync<decimal?>(query, parametros);
        if (result == null)
        {
            await CriarSaldoColaborador(codigoInternoColaborador, orgId);
        }
        return result?? 0;
    }

    public async Task<Dictionary<string, decimal>> BuscarSaldosColaboradores(IEnumerable<string> codigosColaboradores, int orgId)
    {
        var lista = codigosColaboradores?.Distinct().ToList() ?? new List<string>();
        if (lista.Count == 0)
            return new Dictionary<string, decimal>();

        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT codigo_interno_colaborador AS Codigo, valor AS Valor
            FROM tb_reembolso_saldo_colaborador
            WHERE tb_org_id = @OrgId
              AND codigo_interno_colaborador IN @Codigos;
        ";
        var parametros = new { OrgId = orgId, Codigos = lista };
        var rows = await connection.QueryAsync<(string Codigo, decimal Valor)>(query, parametros);
        var dict = rows.ToDictionary(x => x.Codigo, x => x.Valor);
        foreach (var cod in lista)
        {
            if (!dict.ContainsKey(cod))
                dict[cod] = 0;
        }
        return dict;
    }

    private async Task CriarSaldoColaborador(string codigoInternoColaborador, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            INSERT INTO tb_reembolso_saldo_colaborador(valor, codigo_interno_colaborador, tb_org_id)
            VALUES (0, @CodigoInternoColaborador, @OrgId);
        ";
        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador,
            OrgId = orgId
        };

        await connection.ExecuteAsync(query, parametros);
    }

    public async Task EditarValorSaldoColaborador(string codigoInternoColaborador, int orgId, decimal valor)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            UPDATE tb_reembolso_saldo_colaborador
            SET valor = @Valor
            WHERE 
                tb_org_id = @OrgId
                AND codigo_interno_colaborador = @CodigoInternoColaborador;
        ";
        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador,
            OrgId = orgId,
            Valor = valor
        };
        
        await connection.ExecuteAsync(query, parametros);
    }

    public async Task<int> CriarSolicitacaoDePagamento(StatusSolicitacaoPagamentoEnum status, string codigoInternoColaborador, int orgId, int solicitacaoReembolsoId, string origem, decimal valor, bool usouSaldo)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            INSERT INTO tb_solicitacao_pagamento(tb_status_solicitacao_pagamento_id, codigo_interno_colaborador, tb_org_id, tb_solicitacao_reembolso_id, origem, valor, saldo_abatido)
            VALUES(@Status, @CodigoColaborador, @OrgId, @SolicitacaoReembolsoId, @Origem, @Valor, @UsouSaldo);
            
            SELECT LAST_INSERT_ID();
        ";
        var parametros = new
        {
            Status = status,
            CodigoColaborador = codigoInternoColaborador,
            OrgId = orgId,
            SolicitacaoReembolsoId = solicitacaoReembolsoId,
            Origem = origem,
            Valor = valor,
            UsouSaldo = usouSaldo
        };
        
        return await connection.ExecuteScalarAsync<int>(query, parametros);
    }

    public async Task GerarExtratoPagamento(TipoMovimentacaoPagamentoEnum tipoMovimentacao, int solicitacaoPagamentoId, int orgId, decimal valor, string codigoInternoColaborador)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            INSERT INTO tb_extrato_pagamento(tipo_pagamento, tb_solicitacao_pagamento_id, tb_org_id, valor, codigo_interno_colaborador)
            VALUES(@Tipo, @SolicitacaoPagamentoId, @OrgId, @Valor, @CodigoColaborador);
            
            SELECT LAST_INSERT_ID();
        ";
        var parametros = new
        {
            SolicitacaoPagamentoId = solicitacaoPagamentoId,
            Tipo = tipoMovimentacao,
            CodigoColaborador = codigoInternoColaborador,
            OrgId = orgId,
            Valor = valor
        };
        
        await connection.ExecuteScalarAsync<int>(query, parametros);
    }

    public async Task<List<SolicitacaoPagamentoRelatorioDTO>> BuscarSolicitacoesPagamentoPorStatusRelatorio(StatusSolicitacaoPagamentoEnum status, int orgId, string competencia = "", string codDiretoria = "")
    {
        var connection = dapperConnection.GetConnection();
        DateTime? dataInicial = null;
        DateTime? dataFinal = null;
        if (!string.IsNullOrEmpty(competencia))
        {
            // Espera-se que competencia esteja no formato "mm/yyyy"
            var partes = competencia.Split('/');
            if (partes.Length == 2 && int.TryParse(partes[0], out int mes) && int.TryParse(partes[1], out int ano))
            {
                dataInicial = new DateTime(ano, mes, 1);
                dataFinal = dataInicial.Value.AddMonths(1).AddDays(-1);
            }
            else
            {
                throw new ArgumentException("Competência em formato inválido. Esperado: mm/yyyy");
            }
        }
        var query = @"
            SELECT
 	            tsp.id AS Id,
                tsp.codigo_interno_colaborador AS CodigoColaborador,
                colaborador.nome_completo AS Nome,
                tco.nome_cliente AS Cliente,
                tpo.projeto AS Projeto,
                tsr.data_despesa AS DataDaDespesa,
                tsr.data_criacao AS DataDoPedido,
                tsr.data_aprovacao AS DataDaAprovacao,
                aprovador.nome_completo AS NomeDoAprovador,
                tsr.valor AS ValorSolicitado,
                tsr.valor_aprovado AS ValorAprovado,
                tsp.valor AS ValorParaPagamento,
                (tsr.valor_aprovado - tsp.valor) AS ValorAbatidoDoSaldo, -- Valor abatido do saldo
                tv.custo_cliente AS CustoCliente,
                tvt.operacao AS Operacao,
                tsr.id AS SolicitacaoReembolsoId,
                CASE 
                    WHEN tcdb.forma_pagamento = 'PIX' 
                        THEN CONCAT('PIX: ', tcdb.chave_pix)
                    WHEN tcdb.forma_pagamento = 'TED' 
                        THEN CONCAT(
                            'BANCO: ', tcdb.codigo_banco_ted,
                            ' AG: ', tcdb.agencia_ted,
                            ' CC: ', tcdb.conta_ted, '-', tcdb.conta_dv_ted
                        )
                    ELSE NULL
                END AS FormaPagamento
            FROM tb_solicitacao_pagamento tsp
            INNER JOIN tb_solicitacao_reembolso tsr ON tsr.id = tsp.tb_solicitacao_reembolso_id
            INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tsr.tb_cliente_id AND tco.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_projeto_org tpo ON tpo.cod_projeto = tsr.tb_projeto_id AND tpo.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_colaborador colaborador ON colaborador.codigo_interno_colaborador = tsr.codigo_interno_colaborador
            INNER JOIN tb_colaborador aprovador ON aprovador.codigo_interno_colaborador = tsr.codigo_interno_colaborador_aprovador
            INNER JOIN tb_verba tv ON tv.id = tsr.tb_verba_id
            INNER JOIN tb_verba_tipo tvt ON tvt.id = tv.tipo_custo
            LEFT JOIN tb_colaborador_dados_bancarios tcdb ON tcdb.codigo_interno_colaborador = tsr.codigo_interno_colaborador
            LEFT JOIN tb_colaborador_org tcolaborador_org ON tcolaborador_org.codigo_interno_colaborador = tsr.codigo_interno_colaborador AND tcolaborador_org.tb_org_id = tsr.tb_org_id
            WHERE
                tsp.tb_org_id = @OrgId
                AND tsp.tb_status_solicitacao_pagamento_id = @Status
        ";
        if (dataInicial != null && dataFinal != null)
        {
            query += @"
                AND tsr.data_despesa BETWEEN @DataInicial AND @DataFinal
                AND tsp.saldo_abatido = 1
            ";
        }
        if (!string.IsNullOrEmpty(codDiretoria))
        {
            query += @"
                AND tcolaborador_org.cod_diretoria = @CodDiretoria
            ";
        }
        query += @";";
        
        var parametros = new
        {
            Status = status,
            OrgId = orgId,
            DataInicial = dataInicial,
            DataFinal = dataFinal,
            CodDiretoria = codDiretoria
        };
        
        var result = await connection.QueryAsync<SolicitacaoPagamentoRelatorioDTO>(query, parametros);
        return result.ToList();
    }
    
    public async Task EditarStatusPagamento(int id, StatusSolicitacaoPagamentoEnum status, bool usouSaldo)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            UPDATE tb_solicitacao_pagamento
            SET 
                tb_status_solicitacao_pagamento_id = @Status,
                saldo_abatido = @UsouSaldo
            WHERE 
                id = @Id
        ";
        var parametros = new
        {
            Id = id,
            Status = status,
            UsouSaldo = usouSaldo
        };
        
        await connection.ExecuteAsync(query, parametros);
    }

    public async Task AtualizarApenasStatusPagamento(int id, StatusSolicitacaoPagamentoEnum status)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            UPDATE tb_solicitacao_pagamento
            SET
                tb_status_solicitacao_pagamento_id = @Status
            WHERE
                id = @Id
        ";
        var parametros = new
        {
            Id = id,
            Status = status
        };

        await connection.ExecuteAsync(query, parametros);
    }

    public async Task<List<SolicitacaoPagamentoDTO>> BuscarSolicitacoesPagamentos(int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"  
            SELECT
                tsp.tb_solicitacao_reembolso_id AS ReembolsoId,
                tsp.saldo_abatido AS SaldoAbatido,
                tsp.valor AS Valor,
                tsp.tb_status_solicitacao_pagamento_id AS Status
            FROM tb_solicitacao_pagamento tsp
            WHERE tsp.tb_org_id = @OrgId
        ";
        var parametros = new
        {
            OrgId = orgId
        };
        
        var result = await connection.QueryAsync<SolicitacaoPagamentoDTO>(query, parametros);
        return result.ToList();
    }
    
    public async Task<List<SolicitacaoPagamentoRelatorioDTO>> BuscarSolicitacoesPagamentoPorIds(List<int> ids, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT
 	            tsp.id AS Id,
                tsp.codigo_interno_colaborador AS CodigoColaborador,
                colaborador.nome_completo AS Nome,
                tco.nome_cliente AS Cliente,
                tpo.projeto AS Projeto,
                tsr.data_despesa AS DataDaDespesa,
                tsr.data_criacao AS DataDoPedido,
                tsr.data_aprovacao AS DataDaAprovacao,
                aprovador.nome_completo AS NomeDoAprovador,
                tsr.valor AS ValorSolicitado,
                tsr.valor_aprovado AS ValorAprovado,
                tsp.valor AS ValorParaPagamento,
                (tsr.valor_aprovado - tsp.valor) AS ValorAbatidoDoSaldo, -- Valor abatido do saldo
                tv.custo_cliente AS CustoCliente,
                tvt.operacao AS Operacao,
                tsr.id AS SolicitacaoReembolsoId
            FROM tb_solicitacao_pagamento tsp
            INNER JOIN tb_solicitacao_reembolso tsr ON tsr.id = tsp.tb_solicitacao_reembolso_id
            INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tsr.tb_cliente_id AND tco.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_projeto_org tpo ON tpo.cod_projeto = tsr.tb_projeto_id AND tpo.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_colaborador colaborador ON colaborador.codigo_interno_colaborador = tsr.codigo_interno_colaborador
            INNER JOIN tb_colaborador aprovador ON aprovador.codigo_interno_colaborador = tsr.codigo_interno_colaborador_aprovador
            INNER JOIN tb_verba tv ON tv.id = tsr.tb_verba_id
            INNER JOIN tb_verba_tipo tvt ON tvt.id = tv.tipo_custo
            WHERE 
                tsp.tb_org_id = @OrgId
                AND tsp.tb_status_solicitacao_pagamento_id = @Status
                AND tsp.id IN @Ids
        ";
       
        
        var parametros = new
        {
            
            OrgId = orgId,
            Ids = ids,
            Status = StatusSolicitacaoPagamentoEnum.AGUARDANDO_PAGAMENTO
        };
        
        var result = await connection.QueryAsync<SolicitacaoPagamentoRelatorioDTO>(query, parametros);
        return result.ToList();
    }
    
    public async Task<List<SolicitacaoPagamentoRelatorioDTO>> BuscarSolicitacoesPagamentoPorListaDeSolicitacaoDeReembolsoIds(List<int> ids, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT
 	            tsp.id AS Id,
                tsp.codigo_interno_colaborador AS CodigoColaborador,
                colaborador.nome_completo AS Nome,
                tco.nome_cliente AS Cliente,
                tpo.projeto AS Projeto,
                tsr.data_despesa AS DataDaDespesa,
                tsr.data_criacao AS DataDoPedido,
                tsr.data_aprovacao AS DataDaAprovacao,
                aprovador.nome_completo AS NomeDoAprovador,
                tsr.valor AS ValorSolicitado,
                tsr.valor_aprovado AS ValorAprovado,
                tsp.valor AS ValorParaPagamento,
                (tsr.valor_aprovado - tsp.valor) AS ValorAbatidoDoSaldo, -- Valor abatido do saldo
                tv.custo_cliente AS CustoCliente,
                tvt.operacao AS Operacao,
                tsr.id AS SolicitacaoReembolsoId,
                tcolabOrg.cod_diretoria AS CodDiretoria
            FROM tb_solicitacao_pagamento tsp
            INNER JOIN tb_solicitacao_reembolso tsr ON tsr.id = tsp.tb_solicitacao_reembolso_id
            LEFT JOIN tb_cliente_org tco ON tco.codigo_cliente = tsr.tb_cliente_id AND tco.tb_org_id = tsr.tb_org_id
            LEFT JOIN tb_projeto_org tpo ON tpo.cod_projeto = tsr.tb_projeto_id AND tpo.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_colaborador colaborador ON colaborador.codigo_interno_colaborador = tsr.codigo_interno_colaborador
            INNER JOIN tb_colaborador_org tcolabOrg ON tcolabOrg.codigo_interno_colaborador = tsr.codigo_interno_colaborador AND tcolabOrg.tb_org_id = tsr.tb_org_id
            INNER JOIN tb_colaborador aprovador ON aprovador.codigo_interno_colaborador = tsr.codigo_interno_colaborador_aprovador
            INNER JOIN tb_verba tv ON tv.id = tsr.tb_verba_id
            INNER JOIN tb_verba_tipo tvt ON tvt.id = tv.tipo_custo
            WHERE
                tsp.tb_org_id = @OrgId
                AND tsp.tb_status_solicitacao_pagamento_id = @Status
                AND tsr.id IN @Ids
        ";
       
        
        var parametros = new
        {
            
            OrgId = orgId,
            Ids = ids,
            Status = StatusSolicitacaoPagamentoEnum.AGUARDANDO_PAGAMENTO
        };
        
        var result = await connection.QueryAsync<SolicitacaoPagamentoRelatorioDTO>(query, parametros);
        return result.ToList();
    }
}