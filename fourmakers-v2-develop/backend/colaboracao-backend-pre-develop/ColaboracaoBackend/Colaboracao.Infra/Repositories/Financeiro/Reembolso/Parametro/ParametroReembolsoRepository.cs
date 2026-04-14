using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Reembolso.Parametro;
using Dapper;
using DataTransferObject.Domain.Financeiro.Reembolso.Parametro;

namespace Colaboracao.Infra.Repositories.Financeiro.Reembolso.Parametro;

public class ParametroReembolsoRepository(IDBConnection dapperConnection) : IParametroReembolsoRepository
{
    private const string DEFAULT_SQL = @"
        SELECT 
            tpr.id AS Id,
            tpr.limite_envio AS LimiteEnvio, 
            tpr.dia_pagamento AS DiaPagamento, 
            tpr.validade_comprovante_dias AS ValidadeComprovanteDias, 
            tpr.limite_envio_alternativo AS LimiteEnvioAlternativo, 
            tpr.dia_pagamento_alternativo AS DiaPagamentoAlternativo, 
            tpr.tb_org_id AS OrgId,
            tpr.permitir_aprovar_minhas_solicitacoes AS PermitirAprovarMinhasSolicitacoes
        FROM tb_parametro_reembolso tpr
    ";

    public async Task<ParametroReembolsoDTO> InserirParametroAsync(int limiteEnvio, int diaPagamento, int validadeComprovanteDias, int orgId, int? limiteEnvioAlternativo, int? diaPagamentoAlternativo, string codigoInternoColaborador, bool permitirAprovarMinhasSolicitacoes)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            INSERT INTO tb_parametro_reembolso (
                limite_envio, 
                dia_pagamento, 
                validade_comprovante_dias, 
                limite_envio_alternativo, 
                dia_pagamento_alternativo, 
                tb_org_id, 
                colaborador_alteracao,
                permitir_aprovar_minhas_solicitacoes                           
            )
            VALUES (
                @LimiteEnvio, 
                @DiaPagamento, 
                @ValidadeComprovanteDias, 
                @LimiteEnvioAlternativo, 
                @DiaPagamentoAlternativo, 
                @OrgId, 
                @CodigoInternoColaborador,
                @PermitirAprovarMinhasSolicitacoes
            );
            SELECT LAST_INSERT_ID();
        ";
        var parametros = new
        {
            LimiteEnvio = limiteEnvio,
            DiaPagamento = diaPagamento,
            ValidadeComprovanteDias = validadeComprovanteDias,
            OrgId = orgId,
            LimiteEnvioAlternativo = limiteEnvioAlternativo,
            DiaPagamentoAlternativo = diaPagamentoAlternativo,
            CodigoInternoColaborador = codigoInternoColaborador,
            PermitirAprovarMinhasSolicitacoes = permitirAprovarMinhasSolicitacoes
        };

        var result = await connection.ExecuteScalarAsync<int>(query, parametros);
        return await BuscarPorIdAsync(result);
    }

    public async Task<ParametroReembolsoDTO> BuscarPorIdAsync(int id)
    {
        var connection = dapperConnection.GetConnection();
        var query = DEFAULT_SQL;
        query += @"
            WHERE 
                tpr.id = @Id;
        ";
        var parametros = new
        {
            Id = id,
        };

        var result = await connection.QueryFirstOrDefaultAsync<ParametroReembolsoDTO>(query, parametros);
        return result;
    }

    public async Task<ParametroReembolsoDTO> BuscarPorOrgIdAsync(int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = DEFAULT_SQL;
        query += @"
            WHERE 
                tpr.tb_org_id = @OrgId;
        ";
        var parametros = new
        {
            OrgId = orgId
        };

        var result = await connection.QueryFirstOrDefaultAsync<ParametroReembolsoDTO>(query, parametros);
        return result;
    }

    public async Task<ParametroReembolsoDTO> EditarAsync(int id, int limiteEnvio, int diaPagamento, int validadeComprovanteDias, int? limiteEnvioAlternativo, int? diaPagamentoAlternativo, string codigoInternoColaborador, bool permitirAprovarMinhasSolicitacoes)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            UPDATE tb_parametro_reembolso
                SET
                    limite_envio = @LimiteEnvio, 
                    dia_pagamento = @DiaPagamento, 
                    validade_comprovante_dias = @ValidadeComprovanteDias, 
                    limite_envio_alternativo = @LimiteEnvioAlternativo, 
                    dia_pagamento_alternativo = @DiaPagamentoAlternativo, 
                    colaborador_alteracao = @CodigoInternoColaborador,
                    permitir_aprovar_minhas_solicitacoes = @PermitirAprovarMinhasSolicitacoes
            WHERE 
                id = @Id;
        ";
        var parametros = new
        {
            LimiteEnvio = limiteEnvio,
            DiaPagamento = diaPagamento,
            ValidadeComprovanteDias = validadeComprovanteDias,
            LimiteEnvioAlternativo = limiteEnvioAlternativo,
            DiaPagamentoAlternativo = diaPagamentoAlternativo,
            CodigoInternoColaborador = codigoInternoColaborador,
            Id = id,
            PermitirAprovarMinhasSolicitacoes = permitirAprovarMinhasSolicitacoes
        };

        await connection.ExecuteAsync(query, parametros);
        return await BuscarPorIdAsync(id);
    }
}