using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Util.Competencia;
using Core.Domain;
using Dapper;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.Nivel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra
{
    public class PerfilRepository : IPerfilRepository
    {
        private readonly IDBConnection _dapperConnection;

        public PerfilRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<int> CountPerfis(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        SELECT 
                            count(*)
                        FROM 
                            tb_gestor_externo_perfil tgep
                        INNER JOIN 
                            tb_gestor_externo tge ON tgep.cod_gestor_externo = tge.cod_gestor_externo AND tge.tb_org_id = tgep.tb_org_id
                        LEFT JOIN 
                            tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente
                        LEFT JOIN 
                            tb_colaborador tc ON tc.codigo_interno_colaborador = tgep.codigo_interno_colaborador_alteracao
                        WHERE
	                        tgep.ativo = 1
                            AND tge.tb_org_id = @OrgId;
                    ";

            var perfis = await connection.ExecuteScalarAsync<int>(
                query,
                new
                {
                    OrgId = orgId
                }
            );

            return perfis;
        }

        public async Task<IEnumerable<ListarPerfisResult>> ListarPerfis(DateTime dataInicio, DateTime dataFim, string cliente, string cpf, int limite, int cursor, string busca, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        SELECT 
                            tgep.nome_perfil as GestorExternoPerfilNome, 
                            tgep.id as GestorExternoPerfilId, 
                            tgep.data_criacao as DataCriacao, 
                            tco.nome_cliente as NomeCliente, 
                            tco.codigo_cliente as CodCliente, 
                            tgep.cod_gestor_externo as CodGestorExterno, 
                            tgep.data_alteracao as UltimaAlteracao,
                            tge.nome as NomeGestorExterno,
                            tgep.codigo_interno_colaborador_criacao AS CodColaboradorCriador,
                            tcCriador.nome_completo AS NomeColaboradorCriador,
                            tgep.codigo_interno_colaborador_alteracao AS CodColaboradorAlteracao,
                            tcAlteracao.nome_completo AS NomeColaboradorAlteracao,
                            tcAlteracao.nome_completo AS UsuarioAlterador
                        FROM 
                            tb_gestor_externo_perfil tgep
                        INNER JOIN 
                            tb_gestor_externo tge ON tgep.cod_gestor_externo = tge.cod_gestor_externo AND tge.tb_org_id = tgep.tb_org_id
                        LEFT JOIN 
                            tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = tgep.tb_org_id
                        LEFT JOIN 
	                        tb_colaborador tcCriador ON tcCriador.codigo_interno_colaborador = tgep.codigo_interno_colaborador_criacao
                        LEFT JOIN 
	                        tb_colaborador tcAlteracao ON tcAlteracao.codigo_interno_colaborador = tgep.codigo_interno_colaborador_alteracao
                        WHERE
                            tge.ativo = 1
                            AND tgep.ativo = 1
                            AND tge.tb_org_id = @OrgId
                            AND (@CodigoCliente IS NULL OR tge.codigo_cliente = @CodigoCliente)
                            AND
                            (
                                (@Busca IS NULL OR (
                                    LOWER(tge.nome) LIKE CONCAT('%', @Busca, '%') OR
                                    LOWER(tgep.nome_perfil) LIKE CONCAT('%', @Busca, '%') OR
                                    LOWER(tco.nome_cliente ) LIKE CONCAT('%', @Busca, '%') OR
                                    LOWER(tcCriador.nome_completo) LIKE CONCAT('%', LOWER(@Busca), '%')
                                ))
                            )
                            AND tgep.data_criacao between @DataInicio AND @DataFim
                        ORDER BY
                            tgep.data_criacao DESC
                        LIMIT @Limite
                        OFFSET @Offset;
                    ";

            var perfis = (await connection.QueryAsync<ListarPerfisResult>(
                query,
                new
                {
                    OrgId = orgId,
                    CodigoCliente = cliente,
                    Busca = string.IsNullOrWhiteSpace(busca) ? null : busca.ToLower(),
                    Limite = limite,
                    Offset = cursor,
                    DataInicio = dataInicio,
                    DataFim = dataFim
                }
            )).ToList();

            return perfis;
        }
    }
}