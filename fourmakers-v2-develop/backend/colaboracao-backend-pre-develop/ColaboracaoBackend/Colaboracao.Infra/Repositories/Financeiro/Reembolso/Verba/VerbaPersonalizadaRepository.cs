using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Reembolso.Verba;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Colaboracao.Infra.Repositories.Financeiro.Reembolso.Verba;

public class VerbaPersonalizadaRepository(IDBConnection dapperConnection) : IVerbaPersonalizadaRepository
{
	private const string DEFAULT_READ_SQL = @"
		SELECT
		    tvp.id AS Id,
		    tvp.ativo AS Ativo,
		    tvp.custo_cliente AS CustoCliente,
		    tvp.valor_customizado AS Valor,
		    tvp.projeto_id AS ProjetoId,
		    tvp.cliente_id AS ClienteId,
		    tvp.codigo_interno_colaborador AS Cpf,
		   	tc.nome_completo AS NomeCompleto,
		   	tv.id AS Id,
		    tv.categoria AS Categoria,
		    tvt.id AS Id,
		    tvt.descricao AS Descricao
		FROM tb_verba_personalizada tvp
		INNER JOIN tb_verba tv 
		    ON tv.id = tvp.tb_verba_id
		INNER JOIN tb_verba_tipo tvt 
		    ON tvt.id = tv.tipo_custo
		LEFT JOIN tb_colaborador tc 
		    ON tc.codigo_interno_colaborador = tvp.codigo_interno_colaborador
	";
	
    public async Task<List<SimpleColaboradorDTO>> ListarColaboradoresAlocadosPorClienteOuProjeto(int orgId, string clienteId = null, string projetoId = null)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT 
					tc.nome_completo AS NomeCompleto,
					tc.codigo_interno_colaborador AS Cpf
				FROM tb_colaborador_projeto_org tcpo
				INNER JOIN tb_projeto_org tpo
					ON tpo.cod_projeto = tcpo.cod_projeto AND tpo.tb_org_id = tcpo.tb_org_id
				INNER JOIN tb_colaborador_org tco 
					ON tco.cod_colaborador_externo = tcpo.cod_colaborador 
					AND tco.tb_org_id = tcpo.tb_org_id
				INNER JOIN tb_colaborador tc 
					ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
				WHERE 
					tcpo.tb_org_id = 2
			    AND (
				    @CodigoCliente IS NULL 
				    OR (tpo.cod_cliente IS NOT NULL AND tpo.cod_cliente = @CodigoCliente)
				)
				AND (
				    @CodigoProjeto IS NULL 
				    OR (tpo.cod_projeto IS NOT NULL AND tpo.cod_projeto = @CodigoProjeto)
				)
            GROUP BY
                tc.codigo_interno_colaborador;
        ";
        var parametros = new
        {
            OrgId = orgId,
            CodigoCliente = string.IsNullOrEmpty(clienteId) ? null : clienteId,
            CodigoProjeto = string.IsNullOrEmpty(projetoId) ? null : projetoId
        };
        
        var result = await connection.QueryAsync<SimpleColaboradorDTO>(query, parametros);
        return result.ToList();
    }

    public async Task<VerbaPersonalizadaColaboradorDTO> InserirVerbaPersonalizada(int orgId, string codigoInternoColaborador, int verbaId, string projetoId, string clienteId, decimal valor, bool ativo, bool custoCliente)
    {
	    var connection = dapperConnection.GetConnection();
	    var query = @"
			INSERT INTO tb_verba_personalizada (
			    tb_org_id,
			    projeto_id,
			    cliente_id,
			    codigo_interno_colaborador,
			    valor_customizado,
			    custo_cliente,
			    ativo,
			    tb_verba_id
			)
			VALUES (
			    @OrgId,
			    @ProjetoId,
			    @ClienteId,
			    @CodigoInternoColaborador,
			    @Valor,
			    @CustoCliente,
			    @Ativo,
			    @VerbaId
			);
			SELECT LAST_INSERT_ID();"
		;
	    var parametro = new
	    {
		    OrgId = orgId,
		    CodigoInternoColaborador = string.IsNullOrEmpty(codigoInternoColaborador) ? null : codigoInternoColaborador,
		    VerbaId = verbaId,
		    ProjetoId = string.IsNullOrEmpty(projetoId) ? null : projetoId,
		    ClienteId = string.IsNullOrEmpty(clienteId) ? null : clienteId,
		    Valor = valor,
		    Ativo = ativo,
		    CustoCliente = custoCliente
	    };
	    
	    var result = await connection.ExecuteScalarAsync<int>(query, parametro);
	    var search = await BuscarVerbaPersonalizadaPorId(result);
	    return search;
    }
	
    public async Task<VerbaPersonalizadaColaboradorDTO> EditarVerbaPersonalizada(
	    int id,
	    string codigoInternoColaborador,
	    decimal valor,
	    bool ativo,
	    bool custoCliente)
    {
	    var connection = dapperConnection.GetConnection();

	    var query = @"
	        UPDATE tb_verba_personalizada
	        SET 
	            codigo_interno_colaborador = @CodigoInternoColaborador,
	            valor_customizado = @Valor,
	            custo_cliente = @CustoCliente,
	            ativo = @Ativo
	        WHERE id = @Id;
	    ";

	    var parametro = new
	    {
		    Id = id,
		    CodigoInternoColaborador = string.IsNullOrEmpty(codigoInternoColaborador) ? null : codigoInternoColaborador,
		    Valor = valor,
		    Ativo = ativo,
		    CustoCliente = custoCliente
	    };

	    await connection.ExecuteAsync(query, parametro);

	    var search = await BuscarVerbaPersonalizadaPorId(id);
	    return search;
    }

    public async Task<VerbaPersonalizadaColaboradorDTO> BuscarVerbaPersonalizadaPorId(int id)
    {
	    var connection = dapperConnection.GetConnection();
	    var query = DEFAULT_READ_SQL;
	    query += @"
			WHERE tvp.id = @Id
		";
	    var parametro = new
	    {
		    Id = id
	    };
	    
	    var result = await connection.QueryAsync<VerbaPersonalizadaColaboradorDTO, SimpleColaboradorDTO, SimpleVerbaDTO, SimpleVerbaTipoDTO, VerbaPersonalizadaColaboradorDTO>(
		    sql: query,
		    map: (verba, colaboradorDto, verbaDto, verbaTipoDto) =>
		    {
			    verba.Verba = verbaDto;
			    verba.VerbaTipo = verbaTipoDto;
			    verba.Colaborador = colaboradorDto;
			    return verba;
		    },
		    param: parametro,
		    splitOn: "Id,Cpf,Id,Id"
	    );
	    return result.ToList().FirstOrDefault();
    }

    public async Task<List<VerbaPersonalizadaColaboradorDTO>> ListarVerbasPersonalizadasPorOrg(int orgId, string clienteId, string projetoId)
    {
	    var connection = dapperConnection.GetConnection();
	    var query = DEFAULT_READ_SQL;
	    query += @"
		    WHERE tvp.tb_org_id = @OrgId
		      AND (
		            (@ClienteId IS NULL AND tvp.cliente_id IS NULL)
		         OR (@ClienteId IS NOT NULL AND tvp.cliente_id = @ClienteId)
		      )
		      AND (
		            (@ProjetoId IS NULL AND tvp.projeto_id IS NULL)
		         OR (@ProjetoId IS NOT NULL AND tvp.projeto_id = @ProjetoId)
		      )
			  AND tvp.deletado = 0;
		";
	    var parametros = new
	    {
		    OrgId = orgId,
		    ClienteId = string.IsNullOrWhiteSpace(clienteId) ? null : clienteId,
		    ProjetoId = string.IsNullOrWhiteSpace(projetoId) ? null : projetoId
	    };
	    
	    var result = await connection.QueryAsync<VerbaPersonalizadaColaboradorDTO, SimpleColaboradorDTO, SimpleVerbaDTO, SimpleVerbaTipoDTO, VerbaPersonalizadaColaboradorDTO>(
		    sql: query,
		    map: (verba, colaboradorDto, verbaDto, verbaTipoDto) =>
		    {
			    verba.Verba = verbaDto;
			    verba.VerbaTipo = verbaTipoDto;
			    verba.Colaborador = colaboradorDto;
			    return verba;
		    },
		    param: parametros,
		    splitOn: "Id,Cpf,Id,Id"
	    );
	    return result.ToList();
    }
    
    public async Task<bool> InativarVerbaPersonalizadaPorId(int id)
    {
	    var connection = dapperConnection.GetConnection();
	    var query = @"
	        UPDATE tb_verba_personalizada
	        SET deletado = 1,
	            ativo = 0,
	            desativado_em = @DesativadoEm
	        WHERE id = @Id;
	    ";

	    var parametro = new
	    {
		    Id = id,
		    DesativadoEm = DateTime.UtcNow
	    };

	    var linhasAfetadas = await connection.ExecuteAsync(query, parametro);

	    if (linhasAfetadas == 0)
	    {
		    throw new ArgumentException($"Nenhuma verba personalizada encontrada com o id {id}.");
	    }

	    return true;
    }

    public async Task<VerbaPersonalizadaColaboradorDTO> BuscarVerbasPersonalizadaPorOrg(int orgId, int verbaId, string clienteId, string projetoId, string codigoInternoColaborador)
    {
	    var connection = dapperConnection.GetConnection();
	    var query = DEFAULT_READ_SQL;
	    query += @"
			WHERE tvp.tb_org_id = @OrgId
				AND tvp.cliente_id = @ClienteId
				AND tvp.projeto_id = @ProjetoId
				AND tvp.codigo_interno_colaborador = @CodigoInternoColaborador
				AND tvp.tb_verba_id = @VerbaId;
		";
	    var parametro = new
	    {
		    OrgId = orgId,
		    CodigoInternoColaborador = string.IsNullOrEmpty(codigoInternoColaborador) ? null : codigoInternoColaborador,
		    ProjetoId = string.IsNullOrEmpty(projetoId) ? null : projetoId,
		    ClienteId = string.IsNullOrEmpty(clienteId) ? null : clienteId,
		    VerbaId = verbaId
	    };
	    
	    var result = await connection.QueryAsync<VerbaPersonalizadaColaboradorDTO, SimpleColaboradorDTO, SimpleVerbaDTO, SimpleVerbaTipoDTO, VerbaPersonalizadaColaboradorDTO>(
		    sql: query,
		    map: (verba, colaboradorDto, verbaDto, verbaTipoDto) =>
		    {
			    verba.Verba = verbaDto;
			    verba.VerbaTipo = verbaTipoDto;
			    verba.Colaborador = colaboradorDto;
			    return verba;
		    },
		    param: parametro,
		    splitOn: "Id,Cpf,Id,Id"
	    );
	    return result.ToList().FirstOrDefault();
    }
}