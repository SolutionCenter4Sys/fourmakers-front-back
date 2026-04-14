using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.DomainModel.Projeto;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Projeto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Projeto
{
    public class ClienteOrgRepository : IClienteOrgRepository
    {
        private ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;

        public ClienteOrgRepository(ColaboradorContext colaboradorContext, IDBConnection dapperConnection)
        {
            _colaboradorContext = colaboradorContext;
            _dapperConnection = dapperConnection;
        }

        public bool CadastrarClienteOrg(string codigoCliente, string nomeCliente, int orgId, bool ativo, bool clienteOcultoNaGestaoAlocados, TipoCadastroClienteEnum tipoCadastro)
        {
            try
            {
                var listaClientes = ListarClientesPorNome(nomeCliente, orgId);

                if (listaClientes.Count == 0)
                {
                    tb_cliente_org clienteOrg = new tb_cliente_org()
                    {
                        id = Guid.NewGuid(),
                        codigo_cliente = codigoCliente,
                        nome_cliente = nomeCliente,
                        tb_org_id = orgId,
                        tipo_cadastro = tipoCadastro.ToString(),
                        deve_ocultar_na_gestao_de_alocados = clienteOcultoNaGestaoAlocados,
                        ativo = ativo
                    };

                    _colaboradorContext.Add(clienteOrg);
                    _colaboradorContext.SaveChanges();
                    return true;
                }
                throw new Exception("Já existe cliente com esse nome: " + listaClientes.Select(l => l.NomeCliente));
            }
            catch (Exception e)
            {
                throw;
            }
        }
        
        public async Task<bool> CadastrarClienteOrgAsync( string codigoCliente, string nomeCliente, int orgId, bool ativo, bool clienteOcultoNaGestaoAlocados, TipoCadastroClienteEnum tipoCadastro)
        {
            var connection = _dapperConnection.GetConnection();

            // 1️⃣ Verificar se já existe cliente com o mesmo nome
            var clientesExistentes = await connection.QueryAsync<string>(
                @"SELECT nome_cliente
                      FROM tb_cliente_org
                      WHERE nome_cliente = @NomeCliente
                        AND tb_org_id = @OrgId",
                new { NomeCliente = nomeCliente, OrgId = orgId }
            );

            if (clientesExistentes.Any())
                throw new Exception("Já existe cliente com esse nome: " + string.Join(", ", clientesExistentes));

            // 2️⃣ Inserir novo cliente
            var queryInsert = @"
                INSERT INTO tb_cliente_org 
                    (id, codigo_cliente, nome_cliente, tb_org_id, tipo_cadastro, deve_ocultar_na_gestao_de_alocados, ativo)
                VALUES 
                    (@Id, @CodigoCliente, @NomeCliente, @OrgId, @TipoCadastro, @ClienteOculto, @Ativo)
            ";

            await connection.ExecuteAsync(
                queryInsert,
                new
                {
                    Id = Guid.NewGuid(),
                    CodigoCliente = codigoCliente,
                    NomeCliente = nomeCliente,
                    OrgId = orgId,
                    TipoCadastro = tipoCadastro.ToString(),
                    ClienteOculto = clienteOcultoNaGestaoAlocados,
                    Ativo = ativo
                }
            );

            return true;
        }

        public async Task UpsertClienteOrg(ClienteOrgDTO cliente, int orgId, bool deveOcultarNaGestaoDeAlocados, TipoCadastroClienteEnum tipoCadastro)
        {
            var connection = _dapperConnection.GetConnection();

            string query = @"
                            INSERT INTO
                                tb_cliente_org (id, codigo_cliente, nome_cliente, tb_org_id, tipo_cadastro, deve_ocultar_na_gestao_de_alocados, ativo)
                            VALUES
                                (@Id, @CodigoCliente, @NomeCliente, @OrgId, @TipoCadastro, @DeveOcultarNaGestaoDeAlocados, @Ativo)
                            ON DUPLICATE KEY UPDATE
                                codigo_cliente = @CodigoCliente,
                                nome_cliente = @NomeCliente,
                                tipo_cadastro = @TipoCadastro,
                                deve_ocultar_na_gestao_de_alocados = @DeveOcultarNaGestaoDeAlocados,
                                ativo = @Ativo;";

            var parameters = new
            {
                Id = Guid.NewGuid(),
                cliente.CodigoCliente,
                cliente.NomeCliente,
                OrgId = orgId,
                TipoCadastro = tipoCadastro.ToString(),
                DeveOcultarNaGestaoDeAlocados = deveOcultarNaGestaoDeAlocados,
                Ativo = cliente.Ativo ?? true
            };

            await connection.ExecuteAsync(query, parameters);
        }



        public async Task DesativarClientesQueForamSubstituidosPorCodigoClienteDoCRM(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            string query = @"
                            UPDATE 
                                tb_cliente_org tco
                            SET 
                                ativo = 0
                            WHERE 
                                tb_org_id = @OrgId
                                AND EXISTS (
                                    SELECT 
                                        1
                                    FROM 
                                        tb_projeto_org
                                    WHERE 
                                        tb_org_id = @OrgId
                                        AND cod_cliente LIKE 'ACC%'
                                        AND tco.codigo_cliente = cod_cliente_registro_carga
                                        AND cod_cliente_registro_carga NOT LIKE 'ACC%'
                                );";

            var parameters = new { OrgId = orgId };

            await connection.ExecuteAsync(query, parameters);
        }


        public async Task<List<ClienteOrgDTO>> ListarClientesOrg(int orgId, string codigoClienteFiltro, string codigoGerenteProjeto)
        {
            var connection = _dapperConnection.GetConnection();

            List<ClienteOrgDTO> clienteOrgLista = new List<ClienteOrgDTO>();

            string sql = @" SELECT DISTINCT
                                        tco.codigo_cliente,
                                        tco.nome_cliente,
                                        tco.ativo
                                    FROM
                                        tb_cliente_org tco
                                    LEFT JOIN
                                        tb_projeto_org tpo ON tpo.cod_cliente = tco.codigo_cliente
                                    LEFT JOIN
                                        tb_projeto_gerente tpg ON tpg.cod_projeto = tpo.cod_projeto
                                    WHERE
                                        tco.tb_org_id = @OrgId
                                        AND (@CodigoCliente IS NULL OR tco.codigo_cliente LIKE CONCAT('%', @CodigoCliente, '%'))
                                        AND tco.ativo = true
                                        AND (@CodigoGerenteProjeto IS NULL OR tpg.cod_colaborador_gerente = @CodigoGerenteProjeto);";

            var result = await connection.QueryAsync<dynamic>(sql,
                new
                {
                    OrgId = orgId,
                    CodigoCliente = codigoClienteFiltro,
                    CodigoGerenteProjeto = codigoGerenteProjeto
                }
            );

            var resultListDB = result.ToList();
            if (resultListDB != null && resultListDB.Any())
            {
                clienteOrgLista = resultListDB.Select(x => new ClienteOrgDTO
                {
                    CodigoCliente = x.codigo_cliente,
                    NomeCliente = x.nome_cliente,
                    LabelCodigoCliente = ProjetoConstants.RetornarLabelCliente(x.codigo_cliente, x.nome_cliente),
                    Ativo = x.ativo
                }).ToList()
                ;
            }

            return clienteOrgLista;
        }

        public List<ClienteOrgDTO> ListarClientesPorNome(string nomeCliente, int orgId)
        {
            try
            {
                List<ClienteOrgDTO> clienteOrgLista = _colaboradorContext.tb_cliente_org
                                                    .Where(x => x.tb_org_id == orgId
                                                                && x.ativo == true
                                                                && x.nome_cliente.ToUpper() == nomeCliente.ToUpper())
                                                    .Select(x => new ClienteOrgDTO
                                                    {
                                                        CodigoCliente = x.codigo_cliente,
                                                        NomeCliente = x.nome_cliente,
                                                        Ativo = x.ativo
                                                    })
                                                    .AsNoTracking()
                                                    .OrderBy(x => x.NomeCliente)
                                                    .ToList();

                return clienteOrgLista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ClienteOrgDTO ObterClientePorCodigo(string codCliente, int orgId)
        {
            try
            {
                var cliente = _colaboradorContext.tb_cliente_org.Where(x => x.codigo_cliente == codCliente && x.tb_org_id == orgId).AsNoTracking().SingleOrDefault();
                if (cliente != null)
                {
                    var clienteOrgDTO = new ClienteOrgDTO()
                    {
                        CodigoCliente = cliente.codigo_cliente,
                        NomeCliente = cliente.nome_cliente,
                        Ativo = cliente.ativo
                    };

                    return clienteOrgDTO;
                }
                return null;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Erro na ClienteOrgRepo: ObterClientePorCodigo");
                System.Console.WriteLine(e.Message);
                throw;
            }
        }
        
        public async Task<ClienteOrgDTO> ObterClientePorCodigoAsync(string codCliente, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    codigo_cliente AS CodigoCliente,
                    nome_cliente   AS NomeCliente,
                    ativo          AS Ativo
                FROM tb_cliente_org
                WHERE codigo_cliente = @CodCliente
                  AND tb_org_id = @OrgId
                LIMIT 1
            ";

            var result = await connection.QuerySingleOrDefaultAsync<ClienteOrgDTO>(
                query,
                new { CodCliente = codCliente, OrgId = orgId }
            );

            return result;
        }

        public List<string> ListarCodigosDosProjetosAssociadosAoCliente(string codCliente, int orgId)
        {
            return _colaboradorContext.tb_projeto_org.Where(x => x.cod_cliente == codCliente.ToString() && x.tb_org_id == orgId).Select(x => x.cod_projeto).ToList();
        }
        
        public async Task<List<string>> ListarCodigosDosProjetosAssociadosAoAsync(string codCliente, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    cod_projeto
                FROM tb_projeto_org
                WHERE cod_cliente = @CodCliente
                  AND tb_org_id = @OrgId
            ";

            var result = await connection.QueryAsync<string>(
                query,
                new { CodCliente = codCliente, OrgId = orgId }
            );

            return result.ToList();
        }

        public void InativarCliente(string codCliente, int orgId)
        {
            var cliente = _colaboradorContext.tb_cliente_org
                                     .Where(x => x.codigo_cliente == codCliente
                                         && x.tb_org_id == orgId).SingleOrDefault();

            if (cliente != null)
            {
                cliente.ativo = false;
                _colaboradorContext.tb_cliente_org.Update(cliente);
                _colaboradorContext.SaveChanges();
            }
        }
        
        public async Task InativarClienteAsync(string codCliente, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_cliente_org
                SET ativo = 0
                WHERE codigo_cliente = @CodCliente
                  AND tb_org_id = @OrgId
            ";

            await connection.ExecuteAsync(
                query,
                new { CodCliente = codCliente, OrgId = orgId }
            );
        }

        public void AtivarCliente(string codCliente, int orgId)
        {
            var cliente = _colaboradorContext.tb_cliente_org
                                 .Where(x => x.codigo_cliente == codCliente
                                     && x.tb_org_id == orgId).SingleOrDefault();

            if (cliente != null)
            {
                cliente.ativo = true;
                _colaboradorContext.tb_cliente_org.Update(cliente);
                _colaboradorContext.SaveChanges();
            }
        }
        
        public async Task AtivarClienteAsync(string codCliente, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_cliente_org
                SET ativo = 1
                WHERE codigo_cliente = @CodCliente
                  AND tb_org_id = @OrgId
            ";

            await connection.ExecuteAsync(
                query,
                new { CodCliente = codCliente, OrgId = orgId }
            );
        }

        public async Task<List<ClienteSumarioDTO>> ListarClienteSumario(int orgId)
        {
            string sql = @$"
                                    SELECT
			                            COUNT(DISTINCT tcpa.codigo_interno_colaborador) as qtd, tpo.cod_cliente, tclo.nome_cliente as cliente
                                    FROM
			                            tb_colaborador_periodo_alocacao tcpa
                                    JOIN
			                            tb_projeto_org tpo ON tpo.cod_projeto = tcpa.codigo_projeto and tcpa.tb_org_id = tpo.tb_org_id
		                            LEFT JOIN
			                            tb_cliente_org tclo ON tpo.cod_cliente = tclo.codigo_cliente AND tpo.tb_org_id = tclo.tb_org_id
                                    JOIN
			                            tb_colaborador_org tco ON tco.codigo_interno_colaborador = tcpa.codigo_interno_colaborador and tcpa.tb_org_id = tco.tb_org_id
                                    WHERE
			                            tclo.nome_cliente IS NOT NULL AND tclo.nome_cliente <> '' AND tcpa.data_fim >= CURDATE() AND tcpa.ativo = 1 AND tco.ativo = 1 AND tcpa.tb_org_id = @OrgId
                                    GROUP BY
			                            tpo.cod_cliente
                                    ORDER BY
			                            qtd
                                    DESC;
                                    ";

            var connection = _dapperConnection.GetConnection();

            var resultdb = await connection.QueryAsync<dynamic>(sql, new { OrgId = orgId });

            var result = resultdb
                        .Select(x => new ClienteSumarioDTO
                        {
                            CodigoCliente = x.cod_cliente,
                            NomeCliente = x.cliente,
                            QtdAlocados = (int)x.qtd
                        })
                        .ToList();

            return result;
        }
    }
}