using System;
using System.Data;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Financeiro.Banco;
using Dapper;
using DataTransferObject.Domain.Financeiro.Banco.DadosBancariosColaborador;

namespace Colaboracao.Infra.Repositories.Financeiro.Banco;

public class DadosBancariosColaboradorRepository(IDBConnection dapperConnection) : IDadosBancariosColaboradorRepository
{
    private readonly IDbConnection _connection = dapperConnection.GetConnection();
    private const string DadosBancariosDefaultSql = @"
        SELECT
            tcdb.id AS Id,
            tcdb.codigo_interno_colaborador AS CodigoInternoColaborador,
            tcdb.codigo_banco_ted AS CodigoBancoTEd,
            tcdb.agencia_ted AS AgenciaTed,
            tcdb.agencia_dv_ted AS AgenciaDvTed,
            tcdb.conta_ted AS ContaTed,
            tcdb.conta_dv_ted AS ContaDvTed,

            -- Dados para PIX
            -- codigo_banco_pix CHAR(3) DEFAULT NULL,
            tcdb.chave_pix AS ChavePix,
            tcdb.tipo_chave_pix AS TipoChavePix, -- 'C'=CPF, 'J'=CNPJ, 'E'=e-mail, 'T'=telefone, 'R'=EVP

            tcdb.data_criacao AS DataCriacao,
            tcdb.data_atualizacao AS DataAtualizacao,
            tcdb.tb_org_id AS OrgId,
            tcdb.forma_pagamento AS FormaPagamento
        FROM tb_colaborador_dados_bancarios tcdb    
    ";

    public async Task<DadosBancariosColaboradorResult?> BuscarDadosBancariosPorColaboradorIdAsync(string id, int orgId)
    {
        var sql = DadosBancariosDefaultSql;
        sql += @"
            WHERE
                tcdb.codigo_interno_colaborador = @Id
                AND tcdb.tb_org_id = @OrgId
        ";

        var parametros = new
        {
            Id = id,
            OrgId = orgId
        };

        return (await _connection.QuerySingleOrDefaultAsync<DadosBancariosColaboradorResult>(sql, parametros));
    }

    public async Task<DadosBancariosColaboradorResult> CriarDadosBancariosAsync(DadosBancariosColaboradorBase input, string codigoInternoColaborador, int orgId)
    {
        var verificaColaborador = await BuscarDadosBancariosPorColaboradorIdAsync(codigoInternoColaborador, orgId);

        if (verificaColaborador != null)
        {
            throw new ApplicationException("Dados bancários já existentes para esse colaborador");
        }
        
        var query = @"
        INSERT INTO tb_colaborador_dados_bancarios (
            id,
            codigo_interno_colaborador,
            codigo_banco_ted,
            agencia_ted,
            agencia_dv_ted,
            conta_ted,
            conta_dv_ted,
            chave_pix,
            tipo_chave_pix,
            forma_pagamento,
            data_criacao,
            data_atualizacao,
            tb_org_id
        )
        VALUES (
            @Id,
            @CodigoInternoColaborador,
            @CodigoBancoTed,
            @AgenciaTed,
            @AgenciaDvTed,
            @ContaTed,
            @ContaDvTed,
            @ChavePix,
            @TipoChavePix,
            @FormaPagamento,
            CURRENT_TIMESTAMP,
            CURRENT_TIMESTAMP,
            @OrgId
        );
    ";

        var parametros = new
        {
            Id = Guid.NewGuid(),
            CodigoInternoColaborador = codigoInternoColaborador,
            CodigoBancoTed = input.CodigoBancoTed,
            AgenciaTed = input.AgenciaTed,
            AgenciaDvTed = input.AgenciaDvTed,
            ContaTed = input.ContaTed,
            ContaDvTed = input.ContaDvTed,
            ChavePix = input.ChavePix,
            TipoChavePix = input.TipoChavePix,
            FormaPagamento = input.FormaPagamento.ToString(),
            OrgId = orgId
        };

        var insert = await _connection.ExecuteAsync(query, parametros);

        if (insert == 0)
        {
            throw new ApplicationException("Erro ao inserir dados bancários");
        }

        var result = await BuscarDadosBancariosPorColaboradorIdAsync(codigoInternoColaborador, orgId);

        if (result == null)
        {
            throw new ApplicationException("Erro ao resgatar informações após criação");
        }

        return result;
    }

    public async Task<DadosBancariosColaboradorResult> EditarDadosBancariosAsync(DadosBancariosColaboradorBase input, string codigoInternoColaborador, int orgId)
    {
        var verificaColaborador = await BuscarDadosBancariosPorColaboradorIdAsync(codigoInternoColaborador, orgId);

        if (verificaColaborador == null)
        {
            throw new ApplicationException("Dados bancários não encontrados para esse colaborador");
        }

        var query = @"
        UPDATE tb_colaborador_dados_bancarios
        SET 
            codigo_banco_ted = @CodigoBancoTed,
            agencia_ted = @AgenciaTed,
            agencia_dv_ted = @AgenciaDvTed,
            conta_ted = @ContaTed,
            conta_dv_ted = @ContaDvTed,
            chave_pix = @ChavePix,
            tipo_chave_pix = @TipoChavePix,
            forma_pagamento = @FormaPagamento,
            data_atualizacao = CURRENT_TIMESTAMP
        WHERE 
            codigo_interno_colaborador = @CodigoInternoColaborador
            AND tb_org_id = @OrgId;
    ";

        var parametros = new
        {
            CodigoBancoTed = input.CodigoBancoTed,
            AgenciaTed = input.AgenciaTed,
            AgenciaDvTed = input.AgenciaDvTed,
            ContaTed = input.ContaTed,
            ContaDvTed = input.ContaDvTed,
            ChavePix = input.ChavePix,
            TipoChavePix = input.TipoChavePix,
            FormaPagamento = input.FormaPagamento.ToString(),
            CodigoInternoColaborador = codigoInternoColaborador,
            OrgId = orgId
        };

        await _connection.ExecuteAsync(query, parametros);

        var result = await BuscarDadosBancariosPorColaboradorIdAsync(codigoInternoColaborador, orgId);

        if (result == null)
        {
            throw new ApplicationException("Erro ao resgatar informações após edição");
        }

        return result;
    }
}