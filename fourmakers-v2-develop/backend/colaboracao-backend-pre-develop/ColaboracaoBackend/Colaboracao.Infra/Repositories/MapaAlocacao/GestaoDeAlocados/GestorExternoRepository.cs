using Colaboracao.Core.Interfaces;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.AreaAtuacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.Permanencia;
using DataTransferObject.Domain.Projeto.GestorExterno;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.MapaAlocacao.GestaoDeAlocados
{
    public class GestorExternoRepository : IGestorExternoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public GestorExternoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private string SELECT_DEFAULT => @" SELECT
                                                cod_gestor_externo AS CodGestorExterno,
                                                codigo_interno_colaborador AS CodigoInternoColaborador,
                                                tb_org_id AS OrgId,
                                                nome AS Nome,
                                                email AS Email,
                                                telefone AS Telefone,
                                                codigo_cliente AS CodigoCliente,
                                                perfil_linkedin AS PerfilLinkedin,
                                                preferencias_pessoais AS PreferenciasPessoais,
                                                data_criacao AS DataCriacao,
                                                data_alteracao AS DataAlteracao
                                            FROM
                                                tb_gestor_externo
                                            WHERE
                                                ativo = 1";

        public async Task<IEnumerable<GestorExternoResult>> ListarGestoresExternosAsync(string codigoCliente, int orgId, string busca)
        {
            var connection = _dapperConnection.GetConnection();
            var query = SELECT_DEFAULT + @" AND tb_org_id = @OrgId 
                                            AND (@CodigoCliente IS NULL OR codigo_cliente = @CodigoCliente)
                                            AND (
                                                    @Busca IS NULL 
                                                    OR @Busca = '' 
                                                    OR nome COLLATE utf8_general_ci LIKE CONCAT('%', @Busca, '%')
                                                )";

            // Buscar gestores externos
            var gestoresExternos = await connection.QueryAsync<GestorExternoResult>(query, new { OrgId = orgId, CodigoCliente = codigoCliente, Busca = busca });

            // Para cada gestor, buscar as �reas de atua��o usando o m�todo j� existente
            foreach (var gestor in gestoresExternos)
            {
                var areasDeAtuacao = await ObterAreasDeAtuacaoPorGestorAsync(gestor.CodGestorExterno);
                gestor.AreasDeAtuacao = areasDeAtuacao;
            }

            return gestoresExternos;
        }

        public async Task<IEnumerable<GestorExternoResult>> ListarGestoresExternosTodosAsync(int orgId, bool filtrarAtivos = true)
        {
            var query = SELECT_DEFAULT + " AND tb_org_id = @OrgId";

            if (!filtrarAtivos)
            {
                query = query.Replace("ativo = 1 AND", "");
            }

            var connection = _dapperConnection.GetConnection();
            
            return await connection.QueryAsync<GestorExternoResult>(query, new { OrgId = orgId });
            
        }

        public async Task<GestorExternoResult> ObterGestorExternoPorCodigoAsync(string codGestorExterno, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = SELECT_DEFAULT + " AND cod_gestor_externo = @CodGestorExterno AND tb_org_id = @OrgId";

            var result = await connection.QueryFirstOrDefaultAsync<GestorExternoResult>(query, new
            {
                CodGestorExterno = codGestorExterno,
                OrgId = orgId
            });

            if (result != null)
            {
                // Usar o m�todo refatorado para obter as �reas de atua��o
                result.AreasDeAtuacao = await ObterAreasDeAtuacaoPorGestorAsync(codGestorExterno);
            }

            return result;
        }

        public async Task<List<GestorExternoAreaAtuacaoResult>> ObterAreasDeAtuacaoPorGestorAsync(string codGestorExterno)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"SELECT
                            tgeaa.cod_gestor_externo AS CodGestorExterno,
                            taa.id AS AreaAtuacaoId,
                            taa.descricao AS AreaAtuacaoDescricao,
                            tp.id AS PermanenciaId,
                            tp.descricao AS PermanenciaDescricao
                          FROM
                            tb_gestor_externo_area_atuacao tgeaa
                          LEFT JOIN
                            tb_area_atuacao taa ON taa.id = tgeaa.tb_area_atuacao_id AND tgeaa.tb_org_id = taa.tb_org_id
                          LEFT JOIN
                            tb_permanencia tp ON tgeaa.tb_permanencia_id = tp.id
                          WHERE
                            tgeaa.cod_gestor_externo = @CodGestorExterno";

            var areasDeAtuacao = await connection.QueryAsync<dynamic>(query, new { CodGestorExterno = codGestorExterno });

            // Mapear os resultados para o DTO
            return areasDeAtuacao.Select(area => new GestorExternoAreaAtuacaoResult
            {
                AreaDeAtuacao = new AreaAtuacaoResult
                {
                    Id = area.AreaAtuacaoId,
                    Descricao = area.AreaAtuacaoDescricao
                },
                Permanencia = area.PermanenciaId != null ? new PermanenciaResult
                {
                    Id = area.PermanenciaId,
                    Descricao = area.PermanenciaDescricao
                } : null
            }).ToList();
        }

        public async Task<GestorExternoResult> InserirGestorExternoAsync(GestorExternoInput input, TipoCadastrogGestorExternoEnum tipoCadastroEnum)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"INSERT INTO tb_gestor_externo
                           (cod_gestor_externo, codigo_interno_colaborador, tb_org_id, nome, email, telefone, codigo_cliente, tipo_cadastro, codigo_interno_colaborador_criacao, codigo_interno_colaborador_alteracao, perfil_linkedin, preferencias_pessoais,ativo)
                           VALUES
                           (@CodGestorExterno, @CodigoInternoColaborador, @OrgId, @Nome, @Email, @Telefone, @CodigoCliente, @TipoCadastro, @CodigoInternoColaboradorAlteracao, @CodigoInternoColaboradorAlteracao, @PerfilLinkedin, @PreferenciasPessoais, @Ativo)";

            var parameters = new
            {
                input.CodGestorExterno,
                input.CodigoInternoColaborador,
                input.OrgId,
                input.Nome,
                input.Email,
                input.Telefone,
                input.CodigoCliente,
                TipoCadastro = tipoCadastroEnum.ToString(),
                input.CodigoInternoColaboradorAlteracao,
                input.PerfilLinkedin,
                input.PreferenciasPessoais,
                input.Ativo
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            return rowsAffected > 0 ? await ObterGestorExternoPorCodigoAsync(input.CodGestorExterno.ToString(), input.OrgId) : null;
        }

        public async Task AssociarAreasDeAtuacaoAoGestorExterno(string codGestorExterno, List<GestorExternoAreaAtuacaoInput> areasDeAtuacao, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var queryExcluir = @"   DELETE FROM
                                        tb_gestor_externo_area_atuacao
                                    WHERE
                                        cod_gestor_externo = @CodGestorExterno;";

            await connection.ExecuteAsync(queryExcluir, new { CodGestorExterno = codGestorExterno });

            if (areasDeAtuacao != null && areasDeAtuacao.Any())
            {
                var queryInserir = @"   INSERT INTO
                                            tb_gestor_externo_area_atuacao (cod_gestor_externo, tb_area_atuacao_id, tb_permanencia_id, tb_org_id)
                                        VALUES
                                            (@CodGestorExterno, @AreaAtuacaoId, @PermanenciaId, @OrgId);";

                // preparar os parâmetros para execução em lote
                var parametros = areasDeAtuacao.Select(area => new
                {
                    CodGestorExterno = codGestorExterno,
                    AreaAtuacaoId = area.AreaDeAtuacao.Id,
                    PermanenciaId = area.Permanencia.Id,
                    OrgId = orgId
                });

                await connection.ExecuteAsync(queryInserir, parametros);
            }
        }

        public async Task<GestorExternoResult> AtualizarGestorExternoAsync(GestorExternoInput input, string codGestorExternoChave, TipoCadastrogGestorExternoEnum tipoCadastroEnum, bool forcarAtualizacaoCodClienteCarga = false)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @$"UPDATE tb_gestor_externo
                          SET
                              nome = @Nome,
                              cod_gestor_externo = @CodGestorExterno,
                              email = @Email,
                              telefone = @Telefone,
                              codigo_cliente = @CodigoCliente,
                              tipo_cadastro = @TipoCadastro,
                              perfil_linkedin = @PerfilLinkedin,
                              preferencias_pessoais = @PreferenciasPessoais,
                              codigo_interno_colaborador = @CodigoInternoColaborador,
                              codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao,
                              data_alteracao = NOW()
                              {(forcarAtualizacaoCodClienteCarga ? ",codigo_cliente_registro_carga = @CodigoCliente" : "")}
                          WHERE
                              cod_gestor_externo = @CodGestorExternoChave AND tb_org_id = @OrgId";

            var parameters = new
            {
                input.Nome,
                input.CodGestorExterno,
                input.Email,
                input.Telefone,
                input.CodigoCliente,
                TipoCadastro = tipoCadastroEnum.ToString(),
                input.PerfilLinkedin,
                input.PreferenciasPessoais,
                input.CodigoInternoColaborador,
                input.CodigoInternoColaboradorAlteracao,
                input.OrgId,
                CodGestorExternoChave = codGestorExternoChave
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);

            return rowsAffected > 0 ? await ObterGestorExternoPorCodigoAsync(input.CodGestorExterno.ToString(), input.OrgId) : null;
        }

        public async Task<bool> DeletarGestorExternoAsync(string codGestorExterno, string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"UPDATE tb_gestor_externo
                          SET ativo = 0,
                              codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao
                          WHERE cod_gestor_externo = @CodGestorExterno AND tb_org_id = @OrgId";

            int rowsAffected = await connection.ExecuteAsync(query, new { CodGestorExterno = codGestorExterno, CodigoInternoColaboradorAlteracao = codigoInternoColaborador, OrgId = orgId });
            return rowsAffected > 0;
        }

        public async Task<GestorExternoResult> ObterGestorExternoPorNomeAsync(string nome, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND nome = @Nome AND tb_org_id = @OrgId";
            return await connection.QueryFirstOrDefaultAsync<GestorExternoResult>(query, new { Nome = nome, OrgId = orgId });
        }

        public async Task<GestorExternoResult> ObterGestorExternoPorEmailAsync(string email, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND email = @Email AND tb_org_id = @OrgId";
            return await connection.QueryFirstOrDefaultAsync<GestorExternoResult>(query, new { Email = email, OrgId = orgId });
        }

        public async Task<string> GerarCodigoGestorExternoAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = "SELECT fn_gestor_externo_gerar_prox_codigo(@OrgId);";

            var proximoCodigo = await connection.QueryFirstOrDefaultAsync<string>(query, new { OrgId = orgId });

            return proximoCodigo;
        }

        public async Task AtualizarCodColaboradorGestorExternoAsync(string codigoInternoColaborador, string codGestorExterno, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            const string query = @"UPDATE tb_gestor_externo
                                   SET codigo_interno_colaborador = @CodigoInternoColaborador,
                                       data_alteracao = NOW()
                                   WHERE cod_gestor_externo = @CodGestorExterno AND tb_org_id = @OrgId";

            await connection.ExecuteAsync(query, new
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                CodGestorExterno = codGestorExterno,
                OrgId = orgId
            });
        }
    }
}