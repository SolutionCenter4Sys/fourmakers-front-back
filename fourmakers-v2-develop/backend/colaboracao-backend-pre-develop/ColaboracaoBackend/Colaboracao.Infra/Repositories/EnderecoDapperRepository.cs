using System.Data;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain;
using Dapper;
using DataTransferObject.Domain.Endereco;

namespace Colaboracao.Infra.Repositories;

public class EnderecoDapperRepository(IDBConnection dapperConnection) : IEnderecoDapperRepository
{
    private readonly IDbConnection _connection = dapperConnection.GetConnection();
    
    public async Task<EnderecoDTO?> ObterEnderecoPorCodigoInternoColaboradorAsync(string codigoInternoColaborador)
    {
        var sql = @"
                SELECT
                    te.id,
                    te.cep,
                    te.endereco,
                    te.numero,
                    te.complemento,
                    te.bairro,
                    te.cidade,
                    te.estado,
                    te.ativo,
                    te.data_criacao,
                    te.data_alteracao,
                    te.com_quem_mora,
                    te.internacional_linha_um,
                    te.internacional_linha_dois
                FROM tb_colaborador tc 
                INNER JOIN tb_endereco te ON te.id = tc.endereco_id
                WHERE
                    tc.codigo_interno_colaborador = @CodigoInternoColaborador
            ";

        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador
        };
        
        var result = await _connection.QueryFirstOrDefaultAsync<EnderecoDTO?>(sql, parametros);
        return result;
    }
    
    public async Task<EnderecoDTO> ObterEnderecoPorIdAsync(long id)
    {
        var sql = @"
                SELECT
                    te.id,
                    te.cep,
                    te.endereco,
                    te.numero,
                    te.complemento,
                    te.bairro,
                    te.cidade,
                    te.estado,
                    te.data_criacao,
                    te.data_alteracao,
                    te.com_quem_mora,
                    te.internacional_linha_um,
                    te.internacional_linha_dois
                FROM tb_endereco te
                WHERE
                    te.id = @Id
            ";

        var parametros = new
        {
            Id = id
        };
        
        var result = await _connection.QueryFirstOrDefaultAsync<EnderecoDTO>(sql, parametros);
        return result;
    }
    
    public async Task<EnderecoDTO> CriarEnderecoAsync(EnderecoDTO input)
    {
        var sql = @"
            INSERT INTO tb_endereco
            (
                cep,
                endereco,
                numero,
                complemento,
                bairro,
                cidade,
                estado,
                ativo,
                data_criacao,
                data_alteracao
            )
            VALUES
            (
                @Cep,
                @Endereco,
                @Numero,
                @Complemento,
                @Bairro,
                @Cidade,
                @Estado,
                true,
                NOW(),
                NOW()
            );

            SELECT LAST_INSERT_ID() AS id;
        ";

        var parametros = new
        {
            input.Cep,
            input.Endereco,
            input.Numero,
            input.Complemento,
            input.Bairro,
            input.Cidade,
            input.Estado
        };

        var id = await _connection.ExecuteScalarAsync<long>(sql, parametros);

        return await ObterEnderecoPorIdAsync(id);
    }
    
    public async Task<EnderecoDTO> AtualizarEnderecoAsync(long id, EnderecoDTO input)
    {
        var sql = @"
            UPDATE tb_endereco
            SET
                cep = @Cep,
                endereco = @Endereco,
                numero = @Numero,
                complemento = @Complemento,
                bairro = @Bairro,
                cidade = @Cidade,
                estado = @Estado,
                data_alteracao = NOW()
            WHERE
                id = @Id;
        ";

        var parametros = new
        {
            Id = id,
            input.Cep,
            input.Endereco,
            input.Numero,
            input.Complemento,
            input.Bairro,
            input.Cidade,
            input.Estado
        };

        await _connection.ExecuteAsync(sql, parametros);

        return await ObterEnderecoPorIdAsync(id);
    }
}