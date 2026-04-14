using System;
using System.Data;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Colaborador;
using Dapper;

namespace Colaboracao.Infra.Repositories.Colaborador;

public class CertificadoRepository(IDBConnection dapperConnection) : ICertificadoRepository
{
    private readonly IDbConnection _connection = dapperConnection.GetConnection();
    
    
    public async Task SalvarCertificadoAsync(string codigoInternoColaborador, string path, string descricao, string instituicao, DateTime dataConclusao, int cargaHoraria)
    {
        var sql = @"
            INSERT INTO tb_certificado
            (codigo_interno_colaborador, path, descricao, instituicao, data_conclusao, carga_horaria, ativo)
            VALUES 
            (@CodigoInternoColaborador, @Path, @Descricao, @Instituicao, @DataConclusao, @CargaHoraria, 1)
        ";
        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador,
            Path = path,
            Descricao = descricao,
            Instituicao = instituicao,
            DataConclusao = dataConclusao,
            CargaHoraria = cargaHoraria
        };
        
        await _connection.ExecuteAsync(sql, parametros);
    }
}