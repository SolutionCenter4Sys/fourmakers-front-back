using System;
using System.Data;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Financeiro.NotaFiscal;
using Dapper;
using DataTransferObject.Domain.Financeiro.NotaFiscal;

namespace Colaboracao.Infra.Repositories.Financeiro.NotaFiscal;

public class NotaFiscalLogRepository(IDBConnection dapperConnection) : INotaFiscalLogRepository
{

    public async Task InserirLogAsync(NotaFiscalLogInput notaFiscalLogInput)
    {
        var connection = dapperConnection.GetConnection();
        
        const string query = @"
            INSERT INTO tb_nota_fiscal_log
            (
                 id,
                 tb_nota_fiscal_id,
                 tb_nota_fiscal_status_anterior_id,
                 tb_nota_fiscal_status_novo_id,
                 numero_nf,
                 data_emissao_nota_fiscal,
                 observacao,
                 data_criacao,
                 data_alteracao,
                 codigo_interno_colaborador_criacao,
                 codigo_interno_colaborador_alteracao
            )
            VALUES 
            (
                UUID(),
                @NotaFiscalId,
                @NotaFiscalStatusAnteriorId,
                @NotaFiscalStatusNovoId,
                @NumeroNf,
                @DataEmissaoNotaFiscal,
                @Observacao,
                @DataCriacao,
                @DataAlteracao,
                @CodigoInternoColaboradorCriacao,
                @CodigoInternoColaboradorAlteracao
            )
        ";
        
        await connection.ExecuteAsync(query, notaFiscalLogInput);
    }
}