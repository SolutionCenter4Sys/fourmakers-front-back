using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.MapaAlocacao.GestaoDeAlocados
{
    public class GestorExternoPerfilRepository : IGestorExternoPerfilRepository
    {
        private readonly IDBConnection _dapperConnection;

        public GestorExternoPerfilRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        /// <summary>
        /// Converte string para bytes usando encoding UTF8MB4
        /// </summary>
        private byte[] StringToBlob(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            
            return Encoding.UTF8.GetBytes(text);
        }

        /// <summary>
        /// Converte bytes para string usando encoding UTF8MB4
        /// </summary>

        private string SELECT_DEFAULT => @"
            SELECT
                tgep.id AS Id,
                tgep.tb_org_id AS OrgId,
                tgep.cod_gestor_externo AS CodGestorExterno,
                tgep.nome_perfil AS NomePerfil,
                tgep.custo_perfil AS CustoPerfil,
                tgep.ratecard_perfil AS RatecardPerfil,
                COALESCE(CONVERT(tgep.informacoes_relevantes USING utf8mb4), '') AS InformacoesRelevantes,
                tgep.tb_permanencia_id AS PermanenciaId,
                tgep.tb_modelo_trabalho_id AS ModeloTrabalhoId,
                tgep.tb_profissional_localidade_id AS ProfissionalLocalidadeId,
                tgep.ativo AS Ativo,
                tgep.data_criacao AS DataCriacao,
                tgep.data_alteracao AS DataAlteracao,
                tgep.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                tgep.codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao,
                tgep.cidade AS Cidade,
                tgep.estado AS Estado,
                tgep.hibrido_dias AS HibridoDias,
                tgep.cep AS Cep,
                tmt.descricao AS ModeloTrabalhoDescricao,
                tgep.tipo_emprego_linkedin AS TipoEmpregoLinkedin,
                tgep.nivel_experiencia_linkedin AS NivelExperienciaLinkedin,
                tgep.atribuicoes AS Atribuicoes
            FROM
                tb_gestor_externo_perfil tgep
            LEFT JOIN
                tb_modelo_trabalho tmt ON tmt.id = tgep.tb_modelo_trabalho_id
            WHERE
                tgep.ativo = 1";

        public async Task<IEnumerable<GestorExternoPerfilResult>> ListarGestorExternoPerfisAsync()
        {
            var connection = _dapperConnection.GetConnection();

            var result = await connection.QueryAsync<GestorExternoPerfilResult>(SELECT_DEFAULT);
            return result;
        }

        public async Task<GestorExternoPerfilResult> ObterGestorExternoPerfilPorIdAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND tgep.id = @Id ";
            return await connection.QueryFirstOrDefaultAsync<GestorExternoPerfilResult>(query, new { Id = id });
        }
        public async Task<GestorExternoPerfilResult> ObterGestorExternoPerfilPorIdLimiteAsync(Guid id, int cursor, int limite)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND tgep.id = @Id LIMIT @Limite OFFSET @Cursor";
            return await connection.QueryFirstOrDefaultAsync<GestorExternoPerfilResult>(query, new { Id = id, Limite = limite, Cursor = cursor });
        }

        public async Task<IEnumerable<GestorExternoPerfilResult>> ObterGestorExternoPerfilPorListaIdsAsync(List<Guid> ids)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND FIND_IN_SET(CAST(tgep.id AS CHAR), @Ids)";

            var idsComoStrings = ids.Select(id => id.ToString()).ToList();
            var idsConcatenados = string.Join(",", idsComoStrings);

            return await connection.QueryAsync<GestorExternoPerfilResult>(
                query,
                new { Ids = idsConcatenados }
            );
        }

        public async Task<IEnumerable<GestorExternoPerfilResult>> ObterGestorExternoPerfilsPorCodigoGestorExternoAsync(string codGestorExterno, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND tgep.cod_gestor_externo = @Codigo AND tgep.tb_org_id = @OrgId";
            return await connection.QueryAsync<GestorExternoPerfilResult>(query, new { Codigo = codGestorExterno, OrgId = orgId });
        }

        public async Task<GestorExternoPerfilResult> InserirGestorExternoPerfilAsync(GestorExternoPerfilInput input)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        INSERT INTO tb_gestor_externo_perfil
                            (id, tb_org_id, cod_gestor_externo, nome_perfil, custo_perfil, ratecard_perfil,
                            informacoes_relevantes, tb_permanencia_id, tb_modelo_trabalho_id,
                            tb_profissional_localidade_id, ativo, codigo_interno_colaborador_criacao, codigo_interno_colaborador_alteracao, cidade, estado, hibrido_dias, cep,
                            tipo_emprego_linkedin, nivel_experiencia_linkedin, atribuicoes, pais)
                        VALUES
                            (@Id, @OrgId, @CodGestorExterno, @NomePerfil, @CustoPerfil, @RatecardPerfil,
                            @InformacoesRelevantes, @PermanenciaId, @ModeloTrabalhoId,
                            @ProfissionalLocalidadeId, @Ativo, @CodigoInternoColaboradorAlteracao, @CodigoInternoColaboradorAlteracao, @Cidade, @Estado, @HibridoDias, @Cep,
                            @TipoEmpregoLinkedin, @NivelExperienciaLinkedin, @Atribuicoes, @Pais);
                        ";
            var cepNormalizado = input.Cep != null ? input.Cep.Replace("-", "") : null;
            var informacoesRelevantesBlob = StringToBlob(input.InformacoesRelevantes);
            var parameters = new
            {
                input.Id,
                input.OrgId,
                input.CodGestorExterno,
                input.NomePerfil,
                input.CustoPerfil,
                input.RatecardPerfil,
                InformacoesRelevantes = informacoesRelevantesBlob,
                input.PermanenciaId,
                input.ModeloTrabalhoId,
                input.ProfissionalLocalidadeId,
                input.Ativo,
                input.CodigoInternoColaboradorAlteracao,
                input.Cidade,
                input.Estado,
                input.HibridoDias,
                Cep = cepNormalizado,
                input.TipoEmpregoLinkedin,
                input.NivelExperienciaLinkedin,
                input.Atribuicoes,
                input.Pais
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                await CriarLogGestorExternoPerfilAsync(input.Id);
                return await ObterGestorExternoPerfilPorIdAsync(input.Id);
            }

            return null;
        }

        public async Task<GestorExternoPerfilResult> AtualizarGestorExternoPerfilAsync(GestorExternoPerfilInput input)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        UPDATE
                            tb_gestor_externo_perfil
                        SET
                            cod_gestor_externo = @CodGestorExterno,
                            nome_perfil = @NomePerfil,
                            custo_perfil = @CustoPerfil,
                            ratecard_perfil = @RatecardPerfil,
                            informacoes_relevantes = @InformacoesRelevantes,
                            tb_permanencia_id = @PermanenciaId,
                            tb_modelo_trabalho_id = @ModeloTrabalhoId,
                            tb_profissional_localidade_id = @ProfissionalLocalidadeId,
                            codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao,
                            cidade = @Cidade,
                            estado = @Estado,
                            hibrido_dias = @HibridoDias,
                            cep = @Cep,
                            tipo_emprego_linkedin = @TipoEmpregoLinkedin,
                            nivel_experiencia_linkedin = @NivelExperienciaLinkedin,
                            atribuicoes = @Atribuicoes
                        WHERE
                            id = @Id and tb_org_id = @OrgId;
                        ";
            var cepNormalizado = input.Cep != null ? input.Cep.Replace("-", "") : null;
            var informacoesRelevantesBlob = StringToBlob(input.InformacoesRelevantes);
            var parameters = new
            {
                input.Id,
                input.OrgId,
                input.CodGestorExterno,
                input.NomePerfil,
                input.CustoPerfil,
                input.RatecardPerfil,
                InformacoesRelevantes = informacoesRelevantesBlob,
                input.PermanenciaId,
                input.ModeloTrabalhoId,
                input.ProfissionalLocalidadeId,
                input.Ativo,
                input.CodigoInternoColaboradorAlteracao,
                input.Cidade,
                input.Estado,
                input.HibridoDias,
                Cep = cepNormalizado,
                input.TipoEmpregoLinkedin,
                input.NivelExperienciaLinkedin,
                input.Atribuicoes
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                await CriarLogGestorExternoPerfilAsync(input.Id);
                return await ObterGestorExternoPerfilPorIdAsync(input.Id);
            }

            return null;
        }

        public async Task<bool> DeletarGestorExternoPerfilAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"UPDATE
                             tb_gestor_externo_perfil
                          SET
                             ativo = @Ativo
                          WHERE
                             id = @Id";

            int rowsAffected = await connection.ExecuteAsync(query, new { Id = id, Ativo = false });
            return rowsAffected > 0;
        }

        private async Task CriarLogGestorExternoPerfilAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var logQuery = @"
                            INSERT INTO tb_gestor_externo_perfil_log
                                (id, tb_gestor_externo_perfil_id, tb_org_id, cod_gestor_externo, nome_perfil, custo_perfil, ratecard_perfil,
                                informacoes_relevantes, tb_permanencia_id, tb_modelo_trabalho_id, tb_profissional_localidade_id, ativo, data_criacao,
                                data_alteracao, codigo_interno_colaborador_criacao, codigo_interno_colaborador_alteracao, cidade, estado, hibrido_dias, cep, pais)
                            SELECT
                                UUID(), id, tb_org_id, cod_gestor_externo, nome_perfil, custo_perfil, ratecard_perfil,
                                informacoes_relevantes, tb_permanencia_id, tb_modelo_trabalho_id, tb_profissional_localidade_id, ativo, data_criacao,
                                data_alteracao, codigo_interno_colaborador_criacao, codigo_interno_colaborador_alteracao, cidade, estado, hibrido_dias, cep, pais
                            FROM
                                tb_gestor_externo_perfil where id = @Id;";

            var logParameters = new
            {
                Id = id
            };

            await connection.ExecuteAsync(logQuery, logParameters);
        }
        
        public async Task<IEnumerable<CadastrarPerfisLegadosEmLoteDTO>> BuscarIdsPerfis(int quantidade)
        {
            var connection = _dapperConnection.GetConnection();

            var query = "SELECT " +
                            "tb_gestor_externo_perfil_id AS TbGestorExternoPerfilId, " +
                            "processado AS Processado, " +
                            "data_inicio_processamento AS DataInicioProcessamento, " +
                            "data_fim_processamento AS DataFimProcessamento, " +
                            "obs AS Obs " +
                        "FROM tb_cadastrar_perfis_legados_em_lote " +
                        "WHERE processado IS null " +
                        "LIMIT @quantidade;";
            return await connection.QueryAsync<CadastrarPerfisLegadosEmLoteDTO>(query, new { quantidade });
        }

        public async Task IniciarProcessamento(CadastrarPerfisLegadosEmLoteDTO idPerfil)
        {
            var connection = _dapperConnection.GetConnection();
            var query = "UPDATE tb_cadastrar_perfis_legados_em_lote SET data_inicio_processamento = NOW() WHERE tb_gestor_externo_perfil_id = @idPerfil;";
            await connection.ExecuteAsync(query, new { idPerfil = idPerfil.TbGestorExternoPerfilId });
        }

        public async Task FinalizarProcessamento(CadastrarPerfisLegadosEmLoteDTO idPerfil, string retorno)
        {
            var connection = _dapperConnection.GetConnection();
            var query = "UPDATE tb_cadastrar_perfis_legados_em_lote SET processado = 1, data_fim_processamento = NOW(), obs = @obs WHERE tb_gestor_externo_perfil_id = @idPerfil;";
            await connection.ExecuteAsync(query, new { idPerfil = idPerfil.TbGestorExternoPerfilId, obs = retorno });
        }
    }
}