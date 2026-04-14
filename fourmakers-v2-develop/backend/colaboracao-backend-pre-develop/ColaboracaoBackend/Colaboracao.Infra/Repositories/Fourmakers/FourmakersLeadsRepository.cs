using System.Data;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.FourmakersLead;
using Dapper;
using DataTransferObject.Domain.Fourmakers;

namespace Colaboracao.Infra.Repositories.Fourmakers;

public class FourmakersLeadsRepository(IDBConnection dapperConnection) : IFourmakersLeadsRepository
{
    private readonly IDbConnection connection = dapperConnection.GetConnection();

    public async Task InserirLeadAsync(string nome, string email, string telefone, string nomeEmpresa, CaputraLeadColaboradorQuantidadeEnum opcaoColaboradorEnum)
    {
        var query = @"
            INSERT INTO tb_landpage_lead (nome, email, telefone, nome_empresa, opcao_colaborador_enum)
            VALUES (@Nome, @Email, @Telefone, @NomeEmpresa, @OpcaoColaboradorEnum);
        ";

        var parametros = new
        {
            Nome = nome,
            Email = email,
            Telefone = telefone,
            NomeEmpresa = nomeEmpresa,
            OpcaoColaboradorEnum = opcaoColaboradorEnum
        };

        await connection.ExecuteAsync(query, parametros);
    }
}