using System;
using System.Data;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Colaborador;
using Dapper;
using DataTransferObject.Domain.Colaborador;

namespace Colaboracao.Infra.Repositories.Colaborador;

public class ColaboradorOrgRepository(IDBConnection dapperConnection) : IColaboradorOrgRepository
{
    private readonly IDbConnection _connection = dapperConnection.GetConnection();

    public async Task EditarModeloTrabalhoAsync(int orgId, string codigoInternoColaborador, ModeloTrabalhoColaboradorDTO input)
    {
        var query = @"
            UPDATE tb_colaborador_org
                SET 
                    modelo_trabalho = @ModeloTrabalho,
                    dias_por_semana = @DiasPorSemana
            WHERE
                codigo_interno_colaborador = @CodigoInternoColaborador
                AND tb_org_id = @OrgId
        ";

        await _connection.ExecuteAsync(query, new
        {
            input.ModeloTrabalho,
            input.DiasPorSemana,
            CodigoInternoColaborador = codigoInternoColaborador,
            OrgId = orgId
        });
        
    }
}