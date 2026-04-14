using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Util.Competencia;
using Colaboracao.Infra.Context;
using Competencia.Domain.Enums;
using Core.Domain.Vaga;
using Dapper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Util.Enum;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using DataTransferObject.Domain.VagasSRS;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UglyToad.PdfPig.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Colaboracao.Infra.Repositories.Vaga
{
    internal class SkillResult
    {
        public string VagaId { get; set; }
        public string Id { get; set; }
        public long SkillId { get; set; }
        public string SkillDescription { get; set; }
        public long SkillNivelId { get; set; }
        public string SkillNivelDescription { get; set; }
        public int TipoSkillId { get; set; }
        public string TypeSkillsDescription { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public bool Relevante { get; set; }
    }

    internal class SlaResult
    {
        public string VagaId { get; set; }
        public DateTime? DataUltimaModificacaoStatus { get; set; }
        public DateTime? DataEntradaRefinamento { get; set; }
        public string StatusVagaCod { get; set; }
        public bool SlaContando { get; set; }
    }

    internal class SlaSaidaResult
    {
        public string VagaId { get; set; }
        public DateTime? DataSaidaSla { get; set; }
    }

    internal class UsuarioAlteradorResult
    {
        public string VagaId { get; set; }
        public string NomeUsuarioAlterador { get; set; }
    }

    internal class HistoricoStatusVagaRow
    {
        public DateTime DataAlteracao { get; set; }
        public int StatusCod { get; set; }
        public string StatusDescricao { get; set; }
        public string NomeUsuarioAlterador { get; set; }
    }

    public class VagaFourmakersRepository : IVagaFourmakersRepository
    {
        private readonly IDBConnection _dapperConnection;
        private readonly ColaboradorContext _colaboradorContext;

        public VagaFourmakersRepository(IDBConnection dapperConnection, ColaboradorContext colaboradorContext)
        {
            _dapperConnection = dapperConnection;
            _colaboradorContext = colaboradorContext;
        }

        /// <summary>
        /// Query base para listagem de vagas de recrutamento
        /// </summary>
        private static readonly string QueryBaseListarVagasRecrutamento = @"
                    SELECT 
                        tv.id AS Id,
                        tv.codigo AS Codigo,
                        titulo AS Titulo,
                        numero_de_vagas AS NumeroDeVagas,
                        custo_profissional AS CustoProfissional,
                        rate_card AS RateCard,
                        CONVERT(tv.descricao USING utf8mb4) AS Descricao,
                        cargo AS Cargo,
                        tv.data_criacao AS DataCriacao,
                        data_ultima_alteracao AS DataUltimaAlteracao,
                        localizacao AS Localizacao,
                        estado AS Estado,
                        cidade AS Cidade,
                        cep AS Cep,
                        tb_usuario_criador_cpf AS CpfUsuarioCriador,
                        tb_usuario_aprovador_cpf AS CpfUsuarioAprovador,
                        tb_gestor_cod AS CodigoGestor,
                        tv.tb_status_vaga_cod AS StatusVagaCod,
                        tb_origem_vaga_cod AS OrigemVagaCod,
                        tv.tb_org_id AS OrgId,
                        tb_gestor_externo_perfil_id AS IdPerfilGerador,
                        frequencia AS Frequencia,
                        modelo_trabalho_cod AS ModeloTrabalhoCod,
                        tmt.descricao AS modeloTrabalhoDescricao,
                        tge.nome AS NomeGestor,
                        tco.nome_cliente as NomeCliente,
                        tco.codigo_cliente as CodigoCliente,
                        tv.sla_contando as SlaContando,
                        tv.tb_colaborador_codigo_interno_colaborador_gestor_org_logada AS ColaboradorCodigoInternoColaboradorGestorOrgLogada,
                        tv.proposta_crm AS PropostaCrm,
                        tv.tb_tipo_vaga_id AS TipoVagaId,
                        ttv.descricao AS TipoVagaDescricao,
                        tv.tb_tipo_contratacao_id AS TipoContratacaoId,
                        tv.tb_unidade_id AS UnidadeId,
                        tv.tracking AS Tracking,
                        tv.numero_vaga_cliente AS NumeroVagaCliente,
                        tv.tb_cliente_org_codigo_fourmakers AS CodigoClienteFourmakers,
                        tv.tb_permanencia_id AS PermanenciaId,
                        tcCriador.nome_completo AS NomeUsuarioCriador,
                        tv.tb_colaborador_codigo_interno_colaborador_recrutador AS RecrutadorVaga,
                        tcRecrutador.nome_completo AS NomeRecrutadorVaga,
                        tv.tipo_emprego_linkedin AS TipoEmpregoLinkedin,
                        tv.nivel_experiencia_linkedin AS NivelExperienciaLinkedin,
                        tv.tb_modelo_trabalho_id AS ModeloTrabalhoId,
                        tv.tb_vaga_id_parent AS IdVagaParent,
                        tv.maquina AS MaquinaColaborador,
                        tv.candidatos_contratados AS CandidatosContratados,
                        (tv.numero_de_vagas - tv.candidatos_contratados) AS PosicoesRestantes,
                        tv.observacoes_internas AS ObservacoesInternas
                    FROM 
                        tb_vaga tv
                    LEFT JOIN 
	                    tb_gestor_externo tge ON tv.tb_gestor_cod  = tge.cod_gestor_externo AND tv.tb_org_id = tge.tb_org_id
                    LEFT JOIN 
                        tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = tv.tb_org_id
                    LEFT JOIN 
                        tb_modelo_trabalho tmt ON tmt.codigo = tv.modelo_trabalho_cod
                    LEFT JOIN
	                    tb_colaborador tcCriador ON tcCriador.codigo_interno_colaborador = tv.tb_usuario_criador_cpf
                    LEFT JOIN
	                    tb_colaborador tcRecrutador ON tcRecrutador.codigo_interno_colaborador = tv.tb_colaborador_codigo_interno_colaborador_recrutador
                    LEFT JOIN
                        tb_tipo_vaga ttv ON ttv.id = tv.tb_tipo_vaga_id";

        /// <summary>
        /// Converte string para bytes usando encoding UTF8
        /// </summary>
        private byte[] StringToBlob(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            
            return Encoding.UTF8.GetBytes(text);
        }

        public List<VagaFourmakersSRSDTO> SalvarVagasSRS(List<VagaFourmakersSRSDTO> vagasSRSList)
        {
            try
            {
                var idsVagasExistente = vagasSRSList.Select(v => v.Id_vaga).ToList();

                var vagasExistentes = _colaboradorContext.tb_vagas_srs
                    .Where(v => idsVagasExistente.Contains(v.id_vaga))
                    .ToList();

                var vagasParaInserir = new List<VagaFourmakersSRSDTO>();

                foreach (var vagasSRS in vagasSRSList)
                {
                    var existeVaga = vagasExistentes.FirstOrDefault(v => v.id_vaga == vagasSRS.Id_vaga);

                    if (existeVaga == null)
                    {
                        var vagaRow = new tb_vagas_srs();
                        vagaRow.id_vaga = vagasSRS.Id_vaga;
                        vagaRow.titulo = vagasSRS.Titulo;
                        vagaRow.vagas_abertas = vagasSRS.Vagas_abertas;
                        vagaRow.nivel = vagasSRS.Nivel;
                        vagaRow.data_abertura = vagasSRS.Data_abertura;
                        vagaRow.status_vaga = vagasSRS.Status_vaga;
                        vagaRow.descricao = vagasSRS.Descricao;
                        vagaRow.cargo = vagasSRS.Cargo;
                        vagaRow.data_criacao = vagasSRS.Data_criacao;
                        vagaRow.data_alteracao = vagasSRS.Data_alteracao;
                        vagaRow.loc_trabalho = vagasSRS.LocTrabalho;
                        vagaRow.state = vagasSRS.Estado;
                        vagaRow.textForLinkedin = vagasSRS.TextoLinkedin;
                        vagaRow.confidential_job = vagasSRS.Confidencial;
                        vagaRow.tipoVaga = vagasSRS.TipoVaga;
                        vagaRow.termometro = vagasSRS.Termometro;
                        vagaRow.ativo = 1;
                        vagaRow.gestor_foursys = vagasSRS.GestorFoursys;
                        vagaRow.frequencia = vagasSRS.Frequencia;
                        vagaRow.nome_aprovador = vagasSRS.NomeAprovador;
                        vagaRow.maquina_cliente = string.IsNullOrEmpty(vagasSRS.MaquinaCliente) ? 0 : int.Parse(vagasSRS.MaquinaCliente);
                        vagaRow.maquina_four = string.IsNullOrEmpty(vagasSRS.MaquinaFour) ? 0 : int.Parse(vagasSRS.MaquinaFour);
                        vagaRow.notes = vagasSRS.Notes;

                        _colaboradorContext.tb_vagas_srs.Add(vagaRow);

                        foreach (var skill in vagasSRS.Skills)
                        {
                            var skillVaga = new tb_skill_vaga();
                            skillVaga.skill_id = skill.SkillId;
                            skillVaga.tipo_skill_id = skill.TypeSkills;
                            skillVaga.tb_vagas_srs_id = vagaRow.id_vaga;
                            skillVaga.skill_nivel_id = skill.SkillNivelId;
                            _colaboradorContext.tb_skill_vaga.AddRange(skillVaga);
                        }

                        vagasParaInserir.Add(vagasSRS);
                    }
                }

                _colaboradorContext.SaveChanges();

                return vagasParaInserir;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<VagaFourmakersSRSDTO> ObterTodasAsVagas()
        {
            try
            {
                var todasAsVagas = _colaboradorContext.tb_vagas_srs
                    .Select(vaga => new VagaFourmakersSRSDTO
                    {
                        Id_vaga = vaga.id_vaga,
                        Titulo = vaga.titulo,
                        Vagas_abertas = vaga.vagas_abertas ?? 0,
                        Nivel = vaga.nivel,
                        Data_abertura = vaga.data_abertura,
                        Status_vaga = vaga.status_vaga,
                        Descricao = vaga.descricao,
                        Cargo = vaga.cargo,
                        Data_criacao = vaga.data_criacao,
                        Data_alteracao = vaga.data_alteracao,
                        LocTrabalho = vaga.loc_trabalho,
                        Estado = vaga.state,
                        TextoLinkedin = vaga.textForLinkedin,
                        Confidencial = vaga.confidential_job,
                        TipoVaga = vaga.tipoVaga,
                        Ativo = vaga.ativo,
                        Skills = _colaboradorContext.tb_skill_vaga
                            .Where(skill => skill.tb_vagas_srs_id == vaga.id_vaga)
                            .Select(skill => new SkillsVagasDTO
                            {
                                SkillId = skill.skill_id,
                                TypeSkills = skill.tipo_skill_id,
                                SkillNivelId = skill.skill_nivel_id
                            })
                            .ToList(),
                        Frequencia = vaga.frequencia,
                        Data_aprovacao = vaga.data_aprovacao,
                        NomeAprovador = vaga.nome_aprovador,
                        MaquinaCliente = vaga.maquina_cliente.ToString(),
                        MaquinaFour = vaga.maquina_four.ToString(),
                        Notes = vaga.notes
                    })
                    .ToList();

                return todasAsVagas;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<VagaFourmakersSRSDTO> AtualizarVagas(List<VagaFourmakersSRSDTO> vagasAtualizadas)
        {
            var vagasAtualizadasRetorno = new List<VagaFourmakersSRSDTO>();

            try
            {
                var idsVagasAtualizadas = vagasAtualizadas.Select(v => v.Id_vaga).ToList();

                var vagasExistentes = _colaboradorContext.tb_vagas_srs
                    .Where(v => idsVagasAtualizadas.Contains(v.id_vaga))
                    .ToList();

                foreach (var vagaExistente in vagasExistentes)
                {
                    var vagaAtualizada = vagasAtualizadas.FirstOrDefault(v => v.Id_vaga == vagaExistente.id_vaga);

                    if (vagaAtualizada != null)
                    {
                        vagaExistente.titulo = vagaAtualizada.Titulo;
                        vagaExistente.vagas_abertas = vagaAtualizada.Vagas_abertas;
                        vagaExistente.nivel = vagaAtualizada.Nivel;
                        vagaExistente.data_abertura = vagaAtualizada.Data_abertura;
                        vagaExistente.status_vaga = vagaAtualizada.Status_vaga;
                        vagaExistente.descricao = vagaAtualizada.Descricao;
                        vagaExistente.cargo = vagaAtualizada.Cargo;
                        vagaExistente.data_criacao = vagaAtualizada.Data_criacao;
                        vagaExistente.data_alteracao = vagaAtualizada.Data_alteracao;
                        vagaExistente.loc_trabalho = vagaAtualizada.LocTrabalho;
                        vagaExistente.state = vagaAtualizada.Estado;
                        vagaExistente.textForLinkedin = vagaAtualizada.TextoLinkedin;
                        vagaExistente.confidential_job = vagaAtualizada.Confidencial;
                        vagaExistente.tipoVaga = vagaAtualizada.TipoVaga;
                        vagaExistente.termometro = vagaAtualizada.Termometro;
                        vagaExistente.frequencia = vagaAtualizada.Frequencia;
                        vagaExistente.ativo = vagaAtualizada.Ativo ?? 0;
                        vagaExistente.maquina_cliente = string.IsNullOrEmpty(vagaAtualizada.MaquinaCliente) ? 0 : int.Parse(vagaAtualizada.MaquinaCliente);
                        vagaExistente.maquina_four = string.IsNullOrEmpty(vagaAtualizada.MaquinaFour) ? 0 : int.Parse(vagaAtualizada.MaquinaFour);
                        vagaExistente.notes = vagaAtualizada.Notes;

                        var habilidadesExistente = _colaboradorContext.tb_skill_vaga
                            .Where(skill => skill.tb_vagas_srs_id == vagaAtualizada.Id_vaga)
                            .ToList();

                        _colaboradorContext.tb_skill_vaga.RemoveRange(habilidadesExistente);

                        foreach (var skill in vagaAtualizada.Skills)
                        {
                            var novaHabilidade = new tb_skill_vaga
                            {
                                skill_id = skill.SkillId,
                                tipo_skill_id = skill.TypeSkills,
                                tb_vagas_srs_id = vagaAtualizada.Id_vaga,
                                skill_nivel_id = skill.SkillNivelId
                            };

                            _colaboradorContext.tb_skill_vaga.Add(novaHabilidade);
                        }

                        vagasAtualizadasRetorno.Add(vagaAtualizada);
                    }
                }

                _colaboradorContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return vagasAtualizadasRetorno;
        }

        public async Task<VagaFourmakersSRSDTO> ObterVagaPorId(int vagaId)
        {
            try
            {
                var vaga = await _colaboradorContext.tb_vagas_srs
                    .Where(v => v.id_vaga == vagaId)
                    .Select(vaga => new VagaFourmakersSRSDTO
                    {
                        Id_vaga = vaga.id_vaga,
                        Titulo = vaga.titulo,
                        Vagas_abertas = vaga.vagas_abertas ?? 0,
                        Nivel = vaga.nivel,
                        Data_abertura = vaga.data_abertura,
                        Status_vaga = vaga.status_vaga,
                        Descricao = vaga.descricao,
                        Cargo = vaga.cargo,
                        Data_criacao = vaga.data_criacao,
                        Data_alteracao = vaga.data_alteracao,
                        LocTrabalho = vaga.loc_trabalho,
                        Estado = vaga.state,
                        TextoLinkedin = vaga.textForLinkedin,
                        Confidencial = vaga.confidential_job,
                        TipoVaga = vaga.tipoVaga,
                        Ativo = vaga.ativo,
                        GestorFoursys = vaga.gestor_foursys,
                        Skills = _colaboradorContext.tb_skill_vaga
                            .Where(skill => skill.tb_vagas_srs_id == vaga.id_vaga)
                            .Select(skill => new SkillsVagasDTO
                            {
                                SkillId = skill.skill_id,
                                TypeSkills = skill.tipo_skill_id,
                                SkillNivelId = skill.skill_nivel_id
                            })
                            .ToList(),
                        Frequencia = vaga.frequencia,
                        MaquinaCliente = vaga.maquina_cliente.ToString(),
                        MaquinaFour = vaga.maquina_four.ToString(),
                        Notes = vaga.notes
                    })
                    .FirstOrDefaultAsync();

                return vaga;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<VagaFourmakersDTO> ObterVagaPorIdEPerfilId(int vagaId, string perfilId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"  SELECT
                                    tvgep.codigo_vaga as IdVaga,
                                    cast(tvgep.tb_gestor_externo_perfil_id as char) as PerfilId
                                FROM
                                    tb_vaga_gestor_externo_perfil tvgep
                                WHERE
                                    tvgep.codigo_vaga = @VagaId
                                    AND tvgep.tb_gestor_externo_perfil_id = @PerfilId
                                    AND tvgep.tb_org_id = @OrgId;";

            var vaga = await connection.QueryFirstOrDefaultAsync<VagaFourmakersDTO>(
                sql, new { VagaId = vagaId, PerfilId = perfilId, OrgId = orgId }
            );

            return vaga;
        }

        public async Task<string> InserirVagaGestorExternoPerfil(int orgId, string codigoInternoColaborador, int codigoVaga, string gestorExternoPerfilId)
        {
            var _connection = _dapperConnection.GetConnection();
            try
            {
                var id = Guid.NewGuid().ToString();
                var query = @"
                    INSERT INTO tb_vaga_gestor_externo_perfil (
                        id,
                        tb_org_id,
                        ativo,
                        codigo_interno_colaborador_criacao,
                        codigo_vaga,
                        tb_gestor_externo_perfil_id
                    ) VALUES (
                        @Id,
                        @OrgId,
                        1,
                        @CodigoInternoColaborador,
                        @CodigoVaga,
                        @GestorExternoPerfilId
                    )";

                var parametros = new
                {
                    Id = id,
                    OrgId = orgId,
                    CodigoInternoColaborador = codigoInternoColaborador,
                    CodigoVaga = codigoVaga,
                    GestorExternoPerfilId = gestorExternoPerfilId
                };

                await _connection.ExecuteAsync(query, parametros);
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> InserirLogVaga(int orgId, string codigoInternoColaborador, int codigoVaga, string objetoVaga, AcaoLogVagaEnum acao)
        {
            var _connection = _dapperConnection.GetConnection();
            try
            {
                var id = Guid.NewGuid().ToString();
                var query = @"
                    INSERT INTO tb_log_vaga (
                        id,
                        tb_org_id,
                        codigo_interno_colaborador_criacao,
                        codigo_vaga,
                        objeto_vaga,
                        acao
                    ) VALUES (
                        @Id,
                        @OrgId,
                        @CodigoInternoColaborador,
                        @CodigoVaga,
                        @ObjetoVaga,
                        @Acao
                    )";

                var parametros = new
                {
                    Id = id,
                    OrgId = orgId,
                    CodigoInternoColaborador = codigoInternoColaborador,
                    CodigoVaga = codigoVaga,
                    ObjetoVaga = objetoVaga,
                    Acao = (int)acao
                };

                await _connection.ExecuteAsync(query, parametros);
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ListarVagasCadastradasFourmakersResult>> ListarVagasCadastradas(int limite, int cursor, int orgId, string codCliente = null, string gestorExternoPerfilId = null, string busca = null)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @$"
                            SELECT
                                tvgep.codigo_vaga AS CodigoVaga,
                                tvs.data_aprovacao AS DataAprovacao,
                                CAST(tgep.id AS CHAR) AS PerfilId,
                                tgep.nome_perfil AS NomePerfil,
                                tco.codigo_cliente AS CodigoCliente,
                                tco.nome_cliente AS NomeCliente,
                                COALESCE(tvs.numero_candidaturas, 0) AS Candidaturas,
                                    NULLIF(
                                        GROUP_CONCAT(
                                            DISTINCT CONCAT(hab.id, '$', hab.descricao, '$', tsv.skill_nivel_id, '$', tn.descricao, '$', hab.tipo_skill_id)
                                            ORDER BY hab.descricao
                                            SEPARATOR ''
                                        ),
                                        ''
                                    ) AS Habilidades,
                                tvs.nome_aprovador AS NomeAprovador,
                                tvs.status_vaga AS StatusVaga
                            FROM
                                tb_vaga_gestor_externo_perfil tvgep
                            LEFT JOIN
                                tb_vagas_srs tvs ON tvgep.codigo_vaga = tvs.id_vaga
                            LEFT JOIN
                                tb_gestor_externo_perfil tgep ON tvgep.tb_gestor_externo_perfil_id = tgep.id AND tvgep.tb_org_id = tgep.tb_org_id
                            LEFT JOIN
                                tb_gestor_externo tge ON tgep.cod_gestor_externo = tge.cod_gestor_externo AND tgep.tb_org_id = tge.tb_org_id
                            LEFT JOIN
                                tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tge.tb_org_id = tco.tb_org_id
                            LEFT JOIN
                                tb_skill_vaga tsv ON tvs.id_vaga = tsv.tb_vagas_srs_id
                            LEFT JOIN
                                (
                                    SELECT
                                        tc.id,
                                        tc.descricao,
                                        tc.ativo,
                                        1 AS tipo_skill_id
                                    FROM
                                        tb_competencia tc
                                    WHERE
                                        tc.ativo = 1
                                    UNION ALL
                                    SELECT
                                        ts.id,
                                        ts.descricao,
                                        ts.ativo,
                                        2 AS tipo_skill_id
                                    FROM
                                        tb_softskill ts
                                    WHERE
                                        ts.ativo = 1
                                ) hab
                                ON tsv.skill_id = hab.id AND tsv.tipo_skill_id = hab.tipo_skill_id
                            LEFT JOIN
                            	tb_nivel tn ON tsv.skill_nivel_id = tn.id
                            WHERE
                                tvgep.tb_org_id = @OrgId
                                AND (@CodCliente IS NULL OR tco.codigo_cliente = @CodCliente)
                                AND (@CodPerfil IS NULL OR tgep.id = @CodPerfil)
                                AND (@Busca IS NULL OR @Busca = ''
                                    OR CAST(tvgep.codigo_vaga AS CHAR) LIKE CONCAT('%', @Busca, '%')
                                    OR tgep.nome_perfil LIKE CONCAT('%', @Busca, '%')
                                    OR tco.nome_cliente LIKE CONCAT('%', @Busca, '%')
                                    OR hab.descricao LIKE CONCAT('%', @Busca, '%')
                                    OR tvs.status_vaga LIKE CONCAT('%', @Busca, '%')
                                    OR tvs.nome_aprovador LIKE CONCAT('%', @Busca, '%')
                                )
                            GROUP BY
                                tvgep.codigo_vaga,
                                tvs.data_aprovacao,
                                tgep.nome_perfil,
                                tco.nome_cliente,
                                tvs.numero_candidaturas,
                                tvs.nome_aprovador,
                                tvs.status_vaga
                            ORDER BY
                                tvs.data_aprovacao DESC
                            LIMIT
                                @Limite
                            OFFSET
                                @Cursor;
                ";

            var result = await connection.QueryAsync<dynamic>(
                query,
                new
                {
                    OrgId = orgId,
                    CodCliente = codCliente,
                    CodPerfil = gestorExternoPerfilId,
                    Busca = busca,
                    Limite = limite,
                    Cursor = cursor
                }
            );

            var vagas = result.Select(row => new ListarVagasCadastradasFourmakersResult
            {
                CodigoVaga = row.CodigoVaga,
                DataAprovacao = row.DataAprovacao,
                PerfilId = row.PerfilId,
                NomePerfil = row.NomePerfil,
                CodigoCliente = row.CodigoCliente,
                NomeCliente = row.NomeCliente,
                Candidaturas = (int)row.Candidaturas,
                Habilidades = string.IsNullOrEmpty(row.Habilidades) ? new List<SkillNivelDTO>() : CompetenciaUtils.ConverterStringHabilidadesParaListaSkillNivelDTO(row.Habilidades, "", "$"),
                NomeAprovador = row.NomeAprovador,
                StatusVaga = row.StatusVaga
            });

            return vagas;
        }

        public async Task<List<DropDownItemDTO>> ListarClientesComVagasVigentes(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @$"
                            SELECT DISTINCT
                            	tco.codigo_cliente AS Id,
                                tco.nome_cliente AS Descricao
                            FROM
                                tb_vaga_gestor_externo_perfil tvgep
                            LEFT JOIN
                                tb_vagas_srs tvs ON tvgep.codigo_vaga = tvs.id_vaga
                            LEFT JOIN
                                tb_gestor_externo_perfil tgep ON tvgep.tb_gestor_externo_perfil_id = tgep.id AND tvgep.tb_org_id = tgep.tb_org_id
                            LEFT JOIN
                                tb_gestor_externo tge ON tgep.cod_gestor_externo = tge.cod_gestor_externo AND tgep.tb_org_id = tge.tb_org_id
                            LEFT JOIN
                            	tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tge.tb_org_id = tco.tb_org_id
                            WHERE
                            	tvgep.tb_org_id  = @OrgId;";

            var result = await connection.QueryAsync<DropDownItemDTO>(
                query,
                new
                {
                    OrgId = orgId,
                }
            );

            return result.ToList();
        }

        public async Task<List<DropDownItemDTO>> ListarGestorExternoPerfilComVagasVigentesPorCliente(string codigoCliente, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @$"
                            SELECT DISTINCT
                            	CAST(tgep.id AS CHAR) AS Id,
                                tgep.nome_perfil AS Descricao
                            FROM
                                tb_vaga_gestor_externo_perfil tvgep
                            LEFT JOIN
                                tb_vagas_srs tvs ON tvgep.codigo_vaga = tvs.id_vaga
                            LEFT JOIN
                                tb_gestor_externo_perfil tgep ON tvgep.tb_gestor_externo_perfil_id = tgep.id AND tvgep.tb_org_id = tgep.tb_org_id
                            LEFT JOIN
                                tb_gestor_externo tge ON tgep.cod_gestor_externo = tge.cod_gestor_externo AND tgep.tb_org_id = tge.tb_org_id
                            WHERE
                            	tvgep.tb_org_id  = @OrgId AND (@CodigoCliente IS NULL OR tge.codigo_cliente = @CodigoCliente);";

            var result = await connection.QueryAsync<DropDownItemDTO>(
                query,
                new
                {
                    OrgId = orgId,
                    CodigoCliente = codigoCliente
                }
            );

            return result.ToList();
        }

        public async Task<List<SkillNivelDTO>> GetSkillsPorIds(List<int> list)
        {
            var connection = _dapperConnection.GetConnection();

            var sqlSkills = @"  SELECT
                                    vs.descricao as Descricao,
                                    vs.id as Id,
                                    tip.descricao as TipoSkill
                                FROM
                                    vw_skills vs
                                INNER JOIN
                                    tb_item_perfil tip on tip.id = vs.tipo_id
                                WHERE
                                    vs.id in @List;";

            var result = await connection.QueryAsync<SkillNivelDTO>(
                sqlSkills,
                new
                {
                    List = list
                }
            );

            return result.ToList();
        }

        public async Task<List<NivelDTO>> GetTodosNiveis()
        {
            var connection = _dapperConnection.GetConnection();

            var sqlNiveis = @"  SELECT
                                    tn.id as Id,
                                    tn.descricao as Descricao
                                FROM
                                    tb_nivel tn;";

            var listaNivel = await connection.QueryAsync<NivelDTO>(
                sqlNiveis
            );

            return listaNivel.ToList();
        }

        public async Task<int> InserirRecomendacaoProfissionalAderencia(int tbOrgId, string codigoInternoColaboradorCriacao, int codigoVaga, string? objeto, int acao)
        {
            var connection = _dapperConnection.GetConnection();

            const string sql = @" INSERT INTO tb_log_recomendacao_profissionais_aderencia 
                                      (tb_org_id, data_criacao, codigo_interno_colaborador_criacao, codigo_vaga, objeto, acao)
                                  VALUES 
                                      (@tbOrgId, NOW(), @codigoInternoColaboradorCriacao, @codigoVaga, @objeto, @acao);

                    SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int>(sql, new { tbOrgId, codigoInternoColaboradorCriacao, codigoVaga, objeto, acao });

        }

        public async Task<List<ListarVagasEmBancoDeTalentosResult>> ListarVagasPipelinePorOrg(DateTime dataInicio, DateTime dataFim, string cliente, int cursor, int limite, string busca, int? orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                            SELECT 
                                tv.id as Id,
                                tco.nome_cliente as Cliente,
                                tv.titulo as Titulo,
                                tge.nome as Gestor,
                                tv.tb_usuario_criador_cpf as Criador,
                                tv.data_criacao as Criacao,
                                tv.numero_de_vagas as Posicoes,
                                CONVERT(tv.descricao USING utf8mb4) AS Descricao,
                                tv.localizacao as Localizacao,
                                tv.estado as Estado,
                                tv.codigo as Codigo,
                                tmt.descricao as ModalidadeDescricao,
                                tv.cargo as Cargo,
                                tv.cidade as Cidade,
                                tv.tracking as Tracking,
                                tv.numero_vaga_cliente as NumeroVagaCliente,
                                tv.tb_cliente_org_codigo_fourmakers as CodigoClienteFourmakers,
                                tv.tb_org_id as OrgId
                            FROM 
                                tb_vaga tv
                            LEFT JOIN 
                                tb_gestor_externo tge ON tv.tb_gestor_cod = tge.cod_gestor_externo
                            LEFT JOIN 
                                tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente
                            LEFT JOIN 
                                tb_modelo_trabalho tmt ON tmt.codigo = tv.modelo_trabalho_cod
                            WHERE 1=1
                                AND (@CodigoCliente IS NULL OR tge.codigo_cliente = @CodigoCliente)
                                AND
                                (
                                    (@Busca IS NULL OR (
                                        LOWER(tge.nome) LIKE CONCAT('%', @Busca, '%') OR
                                        LOWER(tv.titulo) LIKE CONCAT('%', @Busca, '%')
                                    ))
                                )
                                AND tv.data_criacao between @DataInicio AND @DataFim
                                AND tv.tb_status_vaga_cod = @Pipeline
                                AND (@OrgId IS NULL OR tv.tb_org_id = @OrgId)
                            ORDER BY 
	                            tv.data_criacao DESC
                            LIMIT 
	                            @Limite
                            OFFSET
                                @Offset;";

            var vagas = await connection.QueryAsync<ListarVagasEmBancoDeTalentosResult>(
                query,
                new
                {
                    DataInicio = dataInicio.ToString("yyyy-MM-dd"),
                    DataFim = dataFim.ToString("yyyy-MM-dd"),
                    CodigoCliente = cliente,
                    Busca = busca,
                    Limite = limite,
                    Offset = cursor,
                    Pipeline = StatusVagaRecrutamento.BancoDeTalentos.ToInt().ToString(),
                    OrgId = orgId
                }
            );

            foreach (var vaga in vagas)
            {
                var idVaga = vaga.Id;
                var queryKills = @"SELECT 
                                        tvs.id as Id,
                                        tvs.skill_id as SkillId,
                                        tvs.skill_nivel_id as SkillNivelId,
                                        tn.descricao as SkillNivelDescription,
                                        tvs.tb_item_perfil_id as TipoSkillId,
                                        tip.descricao as TypeSkillsDescription
                                    FROM 
                                        tb_vaga_skill tvs
                                    INNER JOIN 
	                                    tb_item_perfil tip ON tip.id = tvs.tb_item_perfil_id
                                    INNER JOIN 
	                                    tb_nivel tn ON tn.id = tvs.skill_nivel_id
                                WHERE 
	                                tb_vaga_id  =  @idVaga;";

                vaga.Skills = (await connection.QueryAsync<VagaSkillRecrutamentoDTO>(
                    queryKills,
                    new
                    {
                        idVaga
                    }
                )).ToList();

                foreach (var skill in vaga.Skills)
                {
                    switch (skill.TipoSkillId)
                    {
                        case (int)ItemPerfilEnum.COMPETENCIA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_competencia WHERE id = {skill.SkillId}")).FirstOrDefault().ToString();
                            break;
                        case (int)ItemPerfilEnum.SOFTSKILL:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_softskill WHERE id = {skill.SkillId}")).FirstOrDefault().ToString();
                            break;
                        case (int)ItemPerfilEnum.METODOLOGIA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_metodologia WHERE id = {skill.SkillId}")).FirstOrDefault().ToString();
                            break;
                        case (int)ItemPerfilEnum.IDIOMA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_idioma WHERE id = {skill.SkillId}")).FirstOrDefault().ToString();
                            break;
                        case (int)ItemPerfilEnum.DOMINIONEGOCIO:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_dominionegocio WHERE id = {skill.SkillId}")).FirstOrDefault().ToString();
                            break;
                    }
                }

            }

            return vagas.ToList();
        }

        public async Task<List<ListarVagasEmBancoDeTalentosResult>> ListarVagasPipelinePorOrgs(DateTime dataInicio, DateTime dataFim, string cliente, int cursor, int limite, string busca, List<int> orgIds)
        {
            var connection = _dapperConnection.GetConnection();
            
            if (orgIds == null || !orgIds.Any())
            {
                return new List<ListarVagasEmBancoDeTalentosResult>();
            }

            // Cria parâmetros dinâmicos para a lista de orgIds
            var orgIdsParams = string.Join(",", orgIds.Select((id, index) => $"@OrgId{index}"));
            var orgIdsDict = orgIds.Select((id, index) => new KeyValuePair<string, object>($"OrgId{index}", id))
                                   .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var query = $@"
                            SELECT 
                                tv.id as Id,
                                tco.nome_cliente as Cliente,
                                tv.titulo as Titulo,
                                tge.nome as Gestor,
                                tv.tb_usuario_criador_cpf as Criador,
                                tv.data_criacao as Criacao,
                                tv.numero_de_vagas as Posicoes,
                                CONVERT(tv.descricao USING utf8mb4) AS Descricao,
                                tv.localizacao as Localizacao,
                                tv.estado as Estado,
                                tv.codigo as Codigo,
                                tmt.descricao as ModalidadeDescricao,
                                tv.cargo as Cargo,
                                tv.cidade as Cidade,
                                tv.tracking as Tracking,
                                tv.numero_vaga_cliente as NumeroVagaCliente,
                                tv.tb_cliente_org_codigo_fourmakers as CodigoClienteFourmakers,
                                tv.tb_org_id as OrgId
                            FROM 
                                tb_vaga tv
                            LEFT JOIN 
                                tb_gestor_externo tge ON tv.tb_gestor_cod = tge.cod_gestor_externo
                            LEFT JOIN 
                                tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente
                            LEFT JOIN 
                                tb_modelo_trabalho tmt ON tmt.codigo = tv.modelo_trabalho_cod
                            WHERE 1=1
                                AND (@CodigoCliente IS NULL OR tge.codigo_cliente = @CodigoCliente)
                                AND
                                (
                                    (@Busca IS NULL OR (
                                        LOWER(tge.nome) LIKE CONCAT('%', @Busca, '%') OR
                                        LOWER(tv.titulo) LIKE CONCAT('%', @Busca, '%')
                                    ))
                                )
                                AND tv.data_criacao between @DataInicio AND @DataFim
                                AND tv.tb_status_vaga_cod = @Pipeline
                                AND tv.tb_org_id IN ({orgIdsParams})
                            ORDER BY 
	                            tv.data_criacao DESC
                            LIMIT 
	                            @Limite
                            OFFSET
                                @Offset;";

            var parameters = new DynamicParameters();
            parameters.Add("DataInicio", dataInicio.ToString("yyyy-MM-dd"));
            parameters.Add("DataFim", dataFim.ToString("yyyy-MM-dd"));
            parameters.Add("CodigoCliente", cliente);
            parameters.Add("Busca", busca);
            parameters.Add("Limite", limite);
            parameters.Add("Offset", cursor);
            parameters.Add("Pipeline", StatusVagaRecrutamento.BancoDeTalentos.ToInt().ToString());
            
            foreach (var kvp in orgIdsDict)
            {
                parameters.Add(kvp.Key, kvp.Value);
            }

            var vagas = await connection.QueryAsync<ListarVagasEmBancoDeTalentosResult>(
                query,
                parameters
            );

            foreach (var vaga in vagas)
            {
                var idVaga = vaga.Id;
                var queryKills = @"SELECT 
                                        tvs.id as Id,
                                        tvs.skill_id as SkillId,
                                        tvs.skill_nivel_id as SkillNivelId,
                                        tn.descricao as SkillNivelDescription,
                                        tvs.tb_item_perfil_id as TipoSkillId,
                                        tip.descricao as TypeSkillsDescription
                                    FROM 
                                        tb_vaga_skill tvs
                                    INNER JOIN 
	                                    tb_item_perfil tip ON tip.id = tvs.tb_item_perfil_id
                                    INNER JOIN 
	                                    tb_nivel tn ON tn.id = tvs.skill_nivel_id
                                WHERE 
	                                tb_vaga_id  =  @idVaga;";

                vaga.Skills = (await connection.QueryAsync<VagaSkillRecrutamentoDTO>(
                    queryKills,
                    new { idVaga }
                )).ToList();

                foreach (var skill in vaga.Skills)
                {
                    switch (skill.TipoSkillId)
                    {
                        case (int)ItemPerfilEnum.COMPETENCIA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_competencia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault()?.ToString();
                            break;
                        case (int)ItemPerfilEnum.SOFTSKILL:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_softskill WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault()?.ToString();
                            break;
                        case (int)ItemPerfilEnum.METODOLOGIA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_metodologia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault()?.ToString();
                            break;
                        case (int)ItemPerfilEnum.IDIOMA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_idioma WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault()?.ToString();
                            break;
                        case (int)ItemPerfilEnum.DOMINIONEGOCIO:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_dominionegocio WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault()?.ToString();
                            break;
                    }
                }
            }

            return vagas.ToList();
        }

        public async Task<VagaRecrutamentoDTO> ObterVagaPorCodigo(int codigo)
        {
            var connection = _dapperConnection.GetConnection();

            var query = QueryBaseListarVagasRecrutamento + @"
                    WHERE 
                        tv.codigo = @codigo";

            var vaga = await connection.QueryFirstOrDefaultAsync<VagaRecrutamentoDTO>(query, new { codigo = codigo });

            if (vaga is null)
                return vaga;

            await BuscarSkillsVaga(connection, vaga);
            await BuscarSla(connection, vaga);
            await BuscarUsuarioAlterador(connection, vaga);

            return vaga;
        }

        public async Task<VagaRecrutamentoDTO> InserirVaga(VagaRecrutamentoDTO vagaDto, string cpfUsuarioLogado)
        {
            var connection = _dapperConnection.GetConnection();

            var codigoAtual = await connection.ExecuteScalarAsync<long>("SELECT codigo FROM tb_vaga ORDER BY data_criacao DESC LIMIT 1");
            vagaDto.Codigo = codigoAtual + 1;
            var numeroVagaClienteFourmakersAtual = await connection.ExecuteScalarAsync<long>("SELECT numero_vaga_cliente FROM tb_vaga WHERE tb_cliente_org_codigo_fourmakers = @CodigoClienteFourmakers AND tb_org_id = @OrgId ORDER BY data_criacao DESC LIMIT 1;", new { vagaDto.CodigoClienteFourmakers, vagaDto.OrgId });

            try
            {
                vagaDto.NumeroVagaCliente = (numeroVagaClienteFourmakersAtual + 1).ToString();
                vagaDto.Tracking = $"{vagaDto.OrgId}-{DateTime.Now.Year}-{vagaDto.CodigoCliente}-{vagaDto.CodigoGestor}-{vagaDto.NumeroVagaCliente}";
            }
            catch (Exception ex)
            {

            }

            var trackingAtualDesteGestor = await connection.ExecuteScalarAsync<string>("SELECT tracking FROM tb_vaga WHERE tb_gestor_cod = @CodigoGestor AND tb_org_id = @OrgId AND tb_cliente_org_codigo_fourmakers = @CodigoClienteFourmakers ORDER BY data_criacao DESC LIMIT 1;", new { vagaDto.CodigoGestor, vagaDto.OrgId, vagaDto.CodigoClienteFourmakers });
            
            if(trackingAtualDesteGestor is null)
                vagaDto.Tracking = $"{vagaDto.Tracking}-1";
            else
            {
                var split = trackingAtualDesteGestor.Split('-');
                var proximoNumeroVagaGestor = split[5];
                vagaDto.Tracking = $"{vagaDto.Tracking}-{int.Parse(proximoNumeroVagaGestor)+1}";
            }


            var query = @"
                    INSERT INTO tb_vaga (
                        id,
                        codigo,
                        titulo,
                        numero_de_vagas,
                        custo_profissional,
                        rate_card,
                        descricao,
                        cargo,
                        data_criacao,
                        localizacao,
                        estado,
                        cidade,
                        cep,
                        tb_usuario_criador_cpf,
                        tb_usuario_aprovador_cpf,
                        tb_gestor_cod,
                        tb_status_vaga_cod,
                        tb_origem_vaga_cod,
                        tb_org_id,
                        tb_gestor_externo_perfil_id,
                        frequencia,
                        modelo_trabalho_cod,
                        tb_colaborador_codigo_interno_colaborador_gestor_org_logada,
                        proposta_crm,
                        tb_tipo_vaga_id,
                        tb_tipo_contratacao_id,
                        tb_unidade_id,
                        tracking,
                        numero_vaga_cliente,
                        tb_cliente_org_codigo_fourmakers,
                        tipo_emprego_linkedin,
                        nivel_experiencia_linkedin,
                        tb_modelo_trabalho_id,
                        tb_permanencia_id,
                        pais,
                        maquina,
                        tb_colaborador_codigo_interno_colaborador_recrutador,
                        tb_vaga_id_parent,
                        observacoes_internas
                    ) VALUES (
                        @Id,
                        @Codigo,
                        @Titulo,
                        @NumeroDeVagas,
                        @CustoProfissional,
                        @RateCard,
                        @Descricao,
                        @Cargo,
                        Now(),
                        @Localizacao,
                        @Estado,
                        @Cidade,
                        @Cep,
                        @CpfUsuarioCriador,
                        @CpfUsuarioAprovador,
                        @CodigoGestor,
                        @StatusVagaCod,
                        @OrigemVagaCod,
                        @OrgId,
                        @IdPerfilGerador,
                        @Frequencia,
                        @ModeloTrabalhoCod,
                        @ColaboradorCodigoInternoColaboradorGestorOrgLogada,
                        @PropostaCrm,
                        @TipoVagaId,
                        @TipoContratacaoId,
                        @UnidadeId,
                        @Tracking,
                        @NumeroVagaCliente,
                        @CodigoClienteFourmakers,
                        @TipoEmpregoLinkedin,
                        @NivelExperienciaLinkedin,
                        @ModeloTrabalhoId,
                        @PermanenciaId,
                        @Pais,
                        @MaquinaColaborador,
                        @RecrutadorVaga,
                        @IdVagaParent,
                        @ObservacoesInternas
                    )";

            var parameters = new
            {
                vagaDto.Id,
                vagaDto.Codigo,
                vagaDto.Titulo,
                vagaDto.NumeroDeVagas,
                vagaDto.CustoProfissional,
                vagaDto.RateCard,
                Descricao = StringToBlob(vagaDto.Descricao),
                vagaDto.Cargo,
                vagaDto.Localizacao,
                vagaDto.Estado,
                vagaDto.Cidade,
                vagaDto.Cep,
                vagaDto.CpfUsuarioCriador,
                vagaDto.CpfUsuarioAprovador,
                vagaDto.CodigoGestor,
                vagaDto.StatusVagaCod,
                vagaDto.OrigemVagaCod,
                vagaDto.OrgId,
                vagaDto.IdPerfilGerador,
                vagaDto.Frequencia,
                vagaDto.ModeloTrabalhoCod,
                vagaDto.ColaboradorCodigoInternoColaboradorGestorOrgLogada,
                vagaDto.PropostaCrm,
                vagaDto.TipoVagaId,
                vagaDto.TipoContratacaoId,
                vagaDto.UnidadeId,
                vagaDto.Tracking,
                vagaDto.NumeroVagaCliente,
                vagaDto.CodigoClienteFourmakers,
                vagaDto.TipoEmpregoLinkedin,
                vagaDto.NivelExperienciaLinkedin,
                vagaDto.ModeloTrabalhoId,
                vagaDto.PermanenciaId,
                vagaDto.Pais,
                vagaDto.MaquinaColaborador,
                vagaDto.RecrutadorVaga,
                vagaDto.IdVagaParent,
                ObservacoesInternas = vagaDto.ObservacoesInternas
            };

            await connection.ExecuteAsync(query, parameters);

            var queryLog = @"
                            INSERT INTO tb_vaga_fourmakers_log (
                                id,
                                tb_vaga_id,
                                tb_status_vaga_cod,
                                tb_usuario_cpf,
                                data_alteracao,
                                objeto
                            ) VALUES (
                                @guidLog,
                                @idVaga,
                                @statusVagaCod,
                                @usuarioCriador,
                                NOW(),
                                @objectVaga
                            );";

            var guidLog = Guid.NewGuid().ToString();
            var idVaga = vagaDto.Id;
            var statusVagaCod = vagaDto.StatusVagaCod;
            var usuarioCriador = cpfUsuarioLogado;
            var objectVaga = JsonConvert.SerializeObject(vagaDto);

            await connection.ExecuteAsync(
                queryLog,
                new
                {
                    guidLog,
                    idVaga,
                    statusVagaCod,
                    usuarioCriador,
                    objectVaga
                }
            );

            foreach (var skill in vagaDto.Skills)
            {
                var id = Guid.NewGuid().ToString();
                var skill_id = skill.SkillId;
                var tb_item_perfil_id = skill.TipoSkillId;
                var tb_vaga_id = vagaDto.Id;
                var skill_nivel_id = skill.SkillNivelId;
                var relevante = skill.Relevante;

                var querySkills = @"
                            INSERT INTO tb_vaga_skill (
                              id,
                              tb_item_perfil_id,
                              skill_id,
                              skill_nivel_id,
                              tb_vaga_id,
                              ativo,
                              data_criacao,
                              data_alteracao,
                              relevante
                            )
                            VALUES (
                              @id,
                              @tb_item_perfil_id,                     
                              @skill_id,                          
                              @skill_nivel_id,                    
                              @tb_vaga_id,
                              1,                                  
                              NOW(),                              
                              NOW(),
                              @relevante
                            );";

                await connection.ExecuteAsync(
                    querySkills,
                    new
                    {
                        id,
                        skill_id,
                        tb_item_perfil_id,
                        tb_vaga_id,
                        skill_nivel_id,
                        relevante
                    }
                );
            }

            return vagaDto;
        }

        public async Task AtualizarVaga(VagaRecrutamentoDTO vagaDto, string cpfUsuarioLogado)
        {
            var connection = _dapperConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var query = @"
                        UPDATE tb_vaga SET
                            codigo = @Codigo,
                            titulo = @Titulo,
                            numero_de_vagas = @NumeroDeVagas,
                            custo_profissional = @CustoProfissional,
                            rate_card = @RateCard,
                            descricao = @Descricao,
                            cargo = @Cargo,
                            data_criacao = @DataCriacao,
                        localizacao = @Localizacao,
                        estado = @Estado,
                        cidade = @Cidade,
                        cep = @Cep,
                        tb_usuario_criador_cpf = @CpfUsuarioCriador,
                            tb_usuario_aprovador_cpf = @CpfUsuarioAprovador,
                            tb_gestor_cod = @CodigoGestor,
                            tb_status_vaga_cod = @StatusVagaCod,
                            tb_origem_vaga_cod = @OrigemVagaCod,
                            tb_org_id = @OrgId,
                            tb_gestor_externo_perfil_id = @IdPerfilGerador,
                            frequencia = @Frequencia,
                            modelo_trabalho_cod = @ModeloTrabalhoCod,
                            tb_colaborador_codigo_interno_colaborador_gestor_org_logada = @ColaboradorCodigoInternoColaboradorGestorOrgLogada,
                            proposta_crm = @PropostaCrm,
                            tb_tipo_vaga_id = @TipoVagaId,
                            tb_tipo_contratacao_id = @TipoContratacaoId,
                            tb_unidade_id = @UnidadeId,
                            data_ultima_alteracao = NOW(),
                            tipo_emprego_linkedin = @TipoEmpregoLinkedin,
                            nivel_experiencia_linkedin = @NivelExperienciaLinkedin,
                            tb_modelo_trabalho_id = @ModeloTrabalhoId,
                            tb_permanencia_id = @PermanenciaId,
                            tb_colaborador_codigo_interno_colaborador_recrutador = @RecrutadorVaga,
                            maquina = @MaquinaColaborador,
                            candidatos_contratados = @CandidatosContratados
                        WHERE 
                            codigo = @Codigo";

                var parameters = new
                {
                    vagaDto.Codigo,
                    vagaDto.Titulo,
                    vagaDto.NumeroDeVagas,
                    vagaDto.CustoProfissional,
                    vagaDto.RateCard,
                    Descricao = StringToBlob(vagaDto.Descricao),
                    vagaDto.Cargo,
                    vagaDto.DataCriacao,
                    vagaDto.Localizacao,
                    vagaDto.Estado,
                    vagaDto.Cidade,
                    vagaDto.Cep,
                    vagaDto.CpfUsuarioCriador,
                    vagaDto.CpfUsuarioAprovador,
                    vagaDto.CodigoGestor,
                    vagaDto.StatusVagaCod,
                    vagaDto.OrigemVagaCod,
                    vagaDto.OrgId,
                    vagaDto.IdPerfilGerador,
                    vagaDto.Frequencia,
                    vagaDto.ModeloTrabalhoCod,
                    vagaDto.ColaboradorCodigoInternoColaboradorGestorOrgLogada,
                    vagaDto.PropostaCrm,
                    vagaDto.TipoVagaId,
                    vagaDto.TipoContratacaoId,
                    vagaDto.UnidadeId,
                    vagaDto.TipoEmpregoLinkedin,
                    vagaDto.NivelExperienciaLinkedin,
                    vagaDto.ModeloTrabalhoId,
                    vagaDto.PermanenciaId,
                    vagaDto.RecrutadorVaga,
                    vagaDto.MaquinaColaborador,
                    vagaDto.CandidatosContratados
                };

                await connection.ExecuteAsync(query, parameters, transaction);

                // Atualizar entrevistadores se fornecidos
                if (vagaDto.CodColaboradoresEntrevistadores != null && vagaDto.CodColaboradoresEntrevistadores.Count > 0)
                {
                    var deleteEntrevistadores = @"DELETE FROM tb_vaga_entrevistador WHERE tb_vaga_id = @vagaId;";
                    await connection.ExecuteAsync(deleteEntrevistadores, new { vagaId = vagaDto.Id }, transaction);

                    var insertEntrevistador = @"INSERT INTO tb_vaga_entrevistador (tb_vaga_id, tb_colaborador_codigo_interno_colaborador_entrevistador, data_criacao) VALUES (@vagaId, @codigo, NOW());";
                    foreach (var codigo in vagaDto.CodColaboradoresEntrevistadores)
                    {
                        await connection.ExecuteAsync(insertEntrevistador, new { vagaId = vagaDto.Id, codigo }, transaction);
                    }
                }

                var queryLog = @"
                                INSERT INTO tb_vaga_fourmakers_log (
                                    id,
                                    tb_vaga_id,
                                    tb_status_vaga_cod,
                                    tb_usuario_cpf,
                                    data_alteracao,
                                    objeto
                                ) VALUES (
                                    @guidLog,
                                    @idVaga,
                                    @statusVagaCod,
                                    @usuarioCriador,
                                    NOW(),
                                    @objectVaga
                                );";

                var guidLog = Guid.NewGuid().ToString();
                var idVaga = vagaDto.Id;
                var statusVagaCod = vagaDto.StatusVagaCod;
                var usuarioCriador = cpfUsuarioLogado;
                var objectVaga = JsonConvert.SerializeObject(vagaDto);

                await connection.ExecuteAsync(
                    queryLog,
                    new
                    {
                        guidLog,
                        idVaga,
                        statusVagaCod,
                        usuarioCriador,
                        objectVaga
                    },
                    transaction
                );

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            await connection.ExecuteAsync($"DELETE FROM tb_vaga_skill WHERE tb_vaga_id = @idVaga", new { idVaga = vagaDto.Id });

            foreach (var skill in vagaDto.Skills)
            {
                var id = Guid.NewGuid().ToString();
                var skill_id = skill.SkillId;
                var tb_item_perfil_id = skill.TipoSkillId;
                var tb_vaga_id = vagaDto.Id;
                var skill_nivel_id = skill.SkillNivelId;
                var relevante = skill.Relevante;

                var querySkills = @"
                            INSERT INTO tb_vaga_skill (
                              id,
                              tb_item_perfil_id,
                              skill_id,
                              skill_nivel_id,
                              tb_vaga_id,
                              ativo,
                              data_criacao,
                              data_alteracao,
                              relevante
                            )
                            VALUES (
                              @id,
                              @tb_item_perfil_id,                     
                              @skill_id,                          
                              @skill_nivel_id,                    
                              @tb_vaga_id,
                              1,                                  
                              NOW(),                              
                              NOW(),  
                              @relevante                       
                            );";

                await connection.ExecuteAsync(
                    querySkills,
                    new
                    {
                        id,
                        skill_id,
                        tb_item_perfil_id,
                        tb_vaga_id,
                        skill_nivel_id,
                        relevante
                    }
                );
            }
        }

        public async Task CancelarVagaRecrutamento(string idVaga, string cpfUsuarioLogado, int statusVagaCod)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    UPDATE tb_vaga SET
                        `tb_status_vaga_cod` = @statusVagaCod
                    WHERE 
                        id = @idVaga";

            await connection.ExecuteAsync(query, new
            {
                idVaga = idVaga,
                statusVagaCod = statusVagaCod
            });

            var queryLog = @"
                            INSERT INTO tb_vaga_fourmakers_log (
                                id,
                                tb_vaga_id,
                                tb_status_vaga_cod,
                                tb_usuario_cpf,
                                data_alteracao
                            ) VALUES (
                                @guidLog,
                                @idVaga,
                                @statusVagaCod,
                                @usuarioCriador,
                                NOW()
                            );";

            var guidLog = Guid.NewGuid().ToString();
            var usuarioCriador = cpfUsuarioLogado;

            await connection.ExecuteAsync(
                queryLog,
                new
                {
                    guidLog,
                    idVaga,
                    statusVagaCod,
                    usuarioCriador
                }
            );
        }

        public async Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamentoPorOrg(int limite, int cursor, string busca = null, string status = null, int? orgId = null, string dataInicio = null, string dataFim = null, List<string> clientesPermitidos = null)
        {
            var connection = _dapperConnection.GetConnection();

            var query = QueryBaseListarVagasRecrutamento + @"
                    WHERE 
                        1=1
                        AND (@Busca IS NULL OR @Busca = '' OR 
                            LOWER(tco.nome_cliente) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(tge.nome) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(titulo) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(CONVERT(tv.descricao USING utf8mb4)) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(cargo) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(tv.codigo) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(tcCriador.nome_completo) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(tcRecrutador.nome_completo) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(ttv.descricao) LIKE CONCAT('%', LOWER(@Busca), '%'))
                        AND (@Status IS NULL OR @Status = '' OR FIND_IN_SET(tv.tb_status_vaga_cod, @Status))
                        AND (@OrgId IS NULL OR tv.tb_org_id = @OrgId)
                        AND tv.data_criacao BETWEEN @DataInicio AND @DataFim";

            // Filtrar por clientes permitidos se houver restrições
            if (clientesPermitidos != null && clientesPermitidos.Any())
            {
                query += @"
                        AND tco.codigo_cliente IN @ClientesPermitidos";
            }
            else if (clientesPermitidos != null && clientesPermitidos.Count == 0)
            {
                // Se a lista estiver vazia, não retornar nenhuma vaga
                query += @"
                        AND 1=0";
            }

            query += @"
                    ORDER BY 
                        tv.data_ultima_alteracao DESC
                    LIMIT 
                        @Limite
                    OFFSET
                        @Cursor";

            var vagas = await connection.QueryAsync<VagaRecrutamentoDTO>(
                query,
                new
                {
                    Busca = busca,
                    Status = status,
                    Limite = limite,
                    Cursor = cursor,
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    OrgId = orgId,
                    ClientesPermitidos = clientesPermitidos
                }
            );

            var vagasList = vagas.ToList();
            if (!vagasList.Any())
                return vagasList;

            await PopularDadosComplementaresVagas(vagasList, connection);

            return vagasList;

        }

        private async Task PopularDadosComplementaresVagas(List<VagaRecrutamentoDTO> vagasList, MySql.Data.MySqlClient.MySqlConnection connection)
        {
            if (!vagasList.Any())
                return;

            var vagaIds = vagasList.Select(v => v.Id).ToList();
            var statusVagaCods = vagasList.Select(v => v.StatusVagaCod).Distinct().ToList();
            var slaContandoVagas = vagasList.Where(v => v.SlaContando).Select(v => v.Id).ToList();

            // Query multi-tabela para buscar todos os dados de uma vez
            var queryMulti = @"
                -- Skills com descrições via JOIN
                SELECT 
                    tvs.tb_vaga_id as VagaId,
                    tvs.id as Id,
                    tvs.skill_id as SkillId,
                    COALESCE(tc.descricao, ts.descricao, tm.descricao, ti.descricao, td.descricao) as SkillDescription,
                    tvs.skill_nivel_id as SkillNivelId,
                    tn.descricao as SkillNivelDescription,
                    tvs.tb_item_perfil_id as TipoSkillId,
                    tip.descricao as TypeSkillsDescription,
                    tvs.ativo as Ativo,
                    tvs.data_criacao as DataCriacao,
                    tvs.data_alteracao as DataAlteracao,
                    tvs.relevante as Relevante
                FROM 
                    tb_vaga_skill tvs
                INNER JOIN 
                    tb_item_perfil tip ON tip.id = tvs.tb_item_perfil_id
                INNER JOIN 
                    tb_nivel tn ON tn.id = tvs.skill_nivel_id
                LEFT JOIN 
                    tb_competencia tc ON tc.id = tvs.skill_id AND tvs.tb_item_perfil_id = @competenciaTipo
                LEFT JOIN 
                    tb_softskill ts ON ts.id = tvs.skill_id AND tvs.tb_item_perfil_id = @softskillTipo
                LEFT JOIN 
                    tb_metodologia tm ON tm.id = tvs.skill_id AND tvs.tb_item_perfil_id = @metodologiaTipo
                LEFT JOIN 
                    tb_idioma ti ON ti.id = tvs.skill_id AND tvs.tb_item_perfil_id = @idiomaTipo
                LEFT JOIN 
                    tb_dominionegocio td ON td.id = tvs.skill_id AND tvs.tb_item_perfil_id = @dominioNegocioTipo
                WHERE 
                    tvs.tb_vaga_id IN @vagaIds;

                -- SLA: Data última modificação de status (primeira ocorrência de cada status por vaga)
                SELECT 
                    tvfl.tb_vaga_id as VagaId,
                    MIN(tvfl.data_alteracao) as DataUltimaModificacaoStatus,
                    NULL as DataEntradaRefinamento,
                    tvfl.tb_status_vaga_cod as StatusVagaCod,
                    tv.sla_contando as SlaContando
                FROM 
                    tb_vaga_fourmakers_log tvfl
                INNER JOIN 
                    tb_vaga tv ON tv.id = tvfl.tb_vaga_id
                WHERE 
                    tvfl.tb_vaga_id IN @vagaIds
                    AND tvfl.tb_status_vaga_cod IN @statusVagaCods
                GROUP BY 
                    tvfl.tb_vaga_id, tvfl.tb_status_vaga_cod, tv.sla_contando;

                -- SLA: Data entrada em refinamento (apenas para vagas com sla_contando = 1)
                SELECT 
                    tvfl.tb_vaga_id as VagaId,
                    NULL as DataUltimaModificacaoStatus,
                    MIN(tvfl.data_alteracao) as DataEntradaRefinamento,
                    tvfl.tb_status_vaga_cod as StatusVagaCod,
                    tv.sla_contando as SlaContando
                FROM 
                    tb_vaga_fourmakers_log tvfl
                INNER JOIN 
                    tb_vaga tv ON tv.id = tvfl.tb_vaga_id
                WHERE 
                    tvfl.tb_vaga_id IN @slaContandoVagas
                    AND tvfl.tb_status_vaga_cod = @emRefinamento
                GROUP BY 
                    tvfl.tb_vaga_id, tvfl.tb_status_vaga_cod, tv.sla_contando;

                -- SLA: Data primeira entrada em status que encerra a contagem (Contratacao, Cancelada, Perdida)
                SELECT 
                    tvfl.tb_vaga_id as VagaId,
                    MIN(tvfl.data_alteracao) as DataSaidaSla
                FROM 
                    tb_vaga_fourmakers_log tvfl
                WHERE 
                    tvfl.tb_vaga_id IN @vagaIds
                    AND tvfl.tb_status_vaga_cod IN @statusFinaisSla
                GROUP BY 
                    tvfl.tb_vaga_id;

                -- Usuário Alterador (última alteração de cada vaga)
                -- Usa MAX(nome_completo) com GROUP BY para garantir apenas 1 linha por vaga
                -- mesmo quando há múltiplas alterações com a mesma data/hora máxima
                SELECT 
                    tvfl.tb_vaga_id as VagaId,
                    MAX(tc.nome_completo) as NomeUsuarioAlterador
                FROM 
                    (SELECT tb_vaga_id, MAX(data_alteracao) as max_data
                     FROM tb_vaga_fourmakers_log
                     WHERE tb_vaga_id IN @vagaIds
                     GROUP BY tb_vaga_id) ultima_alteracao
                INNER JOIN 
                    tb_vaga_fourmakers_log tvfl ON tvfl.tb_vaga_id = ultima_alteracao.tb_vaga_id 
                    AND tvfl.data_alteracao = ultima_alteracao.max_data
                LEFT JOIN 
                    tb_colaborador tc ON tc.codigo_interno_colaborador = tvfl.tb_usuario_cpf
                GROUP BY 
                    tvfl.tb_vaga_id;";

            var statusFinaisSla = new[]
            {
                StatusVagaRecrutamento.Contratacao.ToInt(),
                StatusVagaRecrutamento.Cancelada.ToInt(),
                StatusVagaRecrutamento.Perdida.ToInt()
            };

            var parameters = new
            {
                vagaIds = vagaIds,
                statusVagaCods = statusVagaCods,
                slaContandoVagas = slaContandoVagas,
                statusFinaisSla = statusFinaisSla,
                emRefinamento = StatusVagaRecrutamento.EmFoco,
                competenciaTipo = (int)ItemPerfilEnum.COMPETENCIA,
                softskillTipo = (int)ItemPerfilEnum.SOFTSKILL,
                metodologiaTipo = (int)ItemPerfilEnum.METODOLOGIA,
                idiomaTipo = (int)ItemPerfilEnum.IDIOMA,
                dominioNegocioTipo = (int)ItemPerfilEnum.DOMINIONEGOCIO
            };

            using var multi = await connection.QueryMultipleAsync(queryMulti, parameters);

            // Ler todos os resultados (5 queries)
            var skills = (await multi.ReadAsync<SkillResult>()).ToList();
            var slaStatusResults = (await multi.ReadAsync<SlaResult>()).ToList();
            var slaRefinamentoResults = (await multi.ReadAsync<SlaResult>()).ToList();
            var slaSaidaResults = (await multi.ReadAsync<SlaSaidaResult>()).ToList();
            var usuariosAlteradores = (await multi.ReadAsync<UsuarioAlteradorResult>()).ToList();

            // Concatenação intencional: ambos são SlaResult mas preenchem campos diferentes.
            // slaStatusResults -> DataUltimaModificacaoStatus (por status) -> usado para SlaDecorridoDaEtapaAtual
            // slaRefinamentoResults -> DataEntradaRefinamento (entrada EmFoco) -> usado para SlaDecorridoTotal
            var slaResults = slaStatusResults.Concat(slaRefinamentoResults).ToList();

            // Processar em memória para popular as vagas
            var skillsPorVaga = skills.GroupBy(s => s.VagaId).ToDictionary(g => g.Key, g => g.ToList());
            var slaPorVaga = slaResults.GroupBy(s => s.VagaId).ToDictionary(g => g.Key, g => g.ToList());
            var dataSaidaSlaPorVaga = slaSaidaResults
                .Where(s => s.DataSaidaSla.HasValue)
                .ToDictionary(s => s.VagaId, s => s.DataSaidaSla!.Value);
            // Proteção contra chaves duplicadas (pode ocorrer se houver múltiplas alterações com mesma data/hora)
            var usuarioAlteradorPorVaga = usuariosAlteradores
                .GroupBy(u => u.VagaId)
                .ToDictionary(g => g.Key, g => g.First().NomeUsuarioAlterador);

            foreach (var vaga in vagasList)
            {
                // Popular Skills
                if (skillsPorVaga.TryGetValue(vaga.Id, out var vagaSkills))
                {
                    vaga.Skills = vagaSkills.Select(s => new VagaSkillRecrutamentoDTO
                    {
                        Id = s.Id,
                        VagaId = s.VagaId,
                        SkillId = s.SkillId,
                        SkillDescription = s.SkillDescription,
                        SkillNivelId = s.SkillNivelId,
                        SkillNivelDescription = s.SkillNivelDescription,
                        TipoSkillId = s.TipoSkillId,
                        TypeSkillsDescription = s.TypeSkillsDescription,
                        Ativo = s.Ativo,
                        DataCriacao = s.DataCriacao,
                        DataAlteracao = s.DataAlteracao,
                        Relevante = s.Relevante
                    }).ToList();
                }
                else
                {
                    vaga.Skills = new List<VagaSkillRecrutamentoDTO>();
                }

                // Popular SLA
                if (slaPorVaga.TryGetValue(vaga.Id, out var vagaSla))
                {
                    var slaStatusAtual = vagaSla.FirstOrDefault(s => s.StatusVagaCod == vaga.StatusVagaCod && s.DataUltimaModificacaoStatus.HasValue);
                    if (slaStatusAtual != null && slaStatusAtual.DataUltimaModificacaoStatus.HasValue)
                    {
                        vaga.SlaDecorridoDaEtapaAtual = DateTime.Now.Subtract(slaStatusAtual.DataUltimaModificacaoStatus.Value);
                    }

                    if (vaga.SlaContando)
                    {
                        var slaRefinamento = vagaSla.FirstOrDefault(s => s.DataEntradaRefinamento.HasValue);
                        if (slaRefinamento != null && slaRefinamento.DataEntradaRefinamento.HasValue)
                        {
                            // SLA para quando entra EmFoco; para de contar ao atingir Contratacao, Cancelada ou Perdida.
                            // Se a vaga voltar de um desses 3 status para outro, volta a contar (usa DateTime.Now).
                            var vagaEstaEmStatusQueEncerraSla = int.TryParse(vaga.StatusVagaCod, out var codAtual)
                                && statusFinaisSla.Contains(codAtual);
                            var dataFimSla = vagaEstaEmStatusQueEncerraSla && dataSaidaSlaPorVaga.TryGetValue(vaga.Id, out var dataSaida)
                                ? dataSaida
                                : DateTime.Now;
                            vaga.SlaDecorridoTotal = dataFimSla.Subtract(slaRefinamento.DataEntradaRefinamento.Value);
                        }
                    }
                }

                // Popular Usuário Alterador
                if (usuarioAlteradorPorVaga.TryGetValue(vaga.Id, out var nomeUsuario))
                {
                    vaga.NomeUsuarioAlterador = nomeUsuario;
                }
            }
        }

        private static async Task BuscarUsuarioAlterador(MySql.Data.MySqlClient.MySqlConnection connection, VagaRecrutamentoDTO vaga)
        {
            var queryUsuarioAlterador = @"
                    SELECT 
                        tc.nome_completo AS NomeUsuarioAlterador,
                        tvfl.data_alteracao
                    FROM 
                        tb_vaga_fourmakers_log tvfl
                    LEFT JOIN 
                        tb_colaborador tc ON tc.codigo_interno_colaborador = tvfl.tb_usuario_cpf
                    WHERE 
                        1=1
	                    AND tvfl.tb_vaga_id = @idVaga
                    ORDER BY 
	                    data_alteracao DESC
                    LIMIT 1;";

            vaga.NomeUsuarioAlterador = (await connection.QueryAsync<string>(queryUsuarioAlterador, new { idVaga = vaga.Id, emRefinamento = StatusVagaRecrutamento.EmFoco })).FirstOrDefault();
        }

        private static async Task BuscarSla(MySql.Data.MySqlClient.MySqlConnection connection, VagaRecrutamentoDTO vaga)
        {
            var querydataUltimaModificacaoStatus = @"SELECT 
	                                                    data_alteracao 
                                                    FROM 
	                                                    tb_vaga_fourmakers_log 
                                                    WHERE 
	                                                    tb_vaga_id = @idVaga 
	                                                    AND tb_status_vaga_cod = @vagaStatus 
                                                    ORDER BY 
	                                                    data_alteracao ASC 
                                                    LIMIT 1";
            var dataUltimaModificacaoStatus = (await connection.QueryAsync<DateTime>(querydataUltimaModificacaoStatus, new { idVaga = vaga.Id, vagaStatus = vaga.StatusVagaCod })).FirstOrDefault();
            vaga.SlaDecorridoDaEtapaAtual = DateTime.Now.Subtract(dataUltimaModificacaoStatus);

            if (vaga.SlaContando)
            {
                var querydataEntradaRefinamento = @"SELECT 
                                                        data_alteracao 
                                                    FROM 
                                                        tb_vaga_fourmakers_log 
                                                    WHERE 
                                                        tb_vaga_id = @idVaga 
                                                    AND tb_status_vaga_cod = @emRefinamento 
                                                    ORDER BY 
                                                        data_alteracao ASC 
                                                    LIMIT 1";
                var dataEntradaRefinamento = (await connection.QueryAsync<DateTime>(querydataEntradaRefinamento, new { idVaga = vaga.Id, emRefinamento = StatusVagaRecrutamento.EmFoco })).FirstOrDefault();
                // SLA para quando entra EmFoco; para de contar ao atingir Contratacao, Cancelada ou Perdida.
                // Se a vaga voltar de um desses 3 status para outro, volta a contar (usa DateTime.Now).
                var statusFinaisSla = new[] {
                    StatusVagaRecrutamento.Contratacao.ToInt(),
                    StatusVagaRecrutamento.Cancelada.ToInt(),
                    StatusVagaRecrutamento.Perdida.ToInt()
                };
                var queryDataSaidaSla = @"SELECT MIN(data_alteracao) FROM tb_vaga_fourmakers_log
                    WHERE tb_vaga_id = @idVaga AND tb_status_vaga_cod IN @statusFinaisSla";
                var dataSaidaSla = (await connection.QueryAsync<DateTime?>(queryDataSaidaSla, new { idVaga = vaga.Id, statusFinaisSla })).FirstOrDefault();
                var vagaEstaEmStatusQueEncerraSla = int.TryParse(vaga.StatusVagaCod, out var codAtual) && statusFinaisSla.Contains(codAtual);
                var dataFimSla = vagaEstaEmStatusQueEncerraSla && dataSaidaSla.HasValue ? dataSaidaSla.Value : DateTime.Now;
                vaga.SlaDecorridoTotal = dataFimSla.Subtract(dataEntradaRefinamento);
            }
        }

        private static async Task BuscarSkillsVaga(MySql.Data.MySqlClient.MySqlConnection connection, VagaRecrutamentoDTO vaga)
        {
            var idVaga = vaga.Id;
            var queryKills = @"SELECT 
                                    tvs.id as Id,
                                    tvs.tb_vaga_id as VagaId,
                                    tvs.skill_id as SkillId,
                                    tvs.skill_nivel_id as SkillNivelId,
                                    tn.descricao as SkillNivelDescription,
                                    tvs.tb_item_perfil_id as TipoSkillId,
                                    tip.descricao as TypeSkillsDescription,
                                    tvs.ativo as Ativo,
                                    tvs.data_criacao as DataCriacao,
                                    tvs.data_alteracao as DataAlteracao,
                                    tvs.relevante as Relevante
                                FROM 
                                    tb_vaga_skill tvs
                                INNER JOIN 
                                    tb_item_perfil tip ON tip.id = tvs.tb_item_perfil_id
                                INNER JOIN 
                                    tb_nivel tn ON tn.id = tvs.skill_nivel_id
                                WHERE 
                                    tb_vaga_id  = @idVaga;";

            vaga.Skills = (await connection.QueryAsync<VagaSkillRecrutamentoDTO>(queryKills, new { idVaga })).ToList();

            foreach (var skill in vaga.Skills)
            {
                switch (skill.TipoSkillId)
                {
                    case (int)ItemPerfilEnum.COMPETENCIA:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_competencia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.SOFTSKILL:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_softskill WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.METODOLOGIA:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_metodologia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.IDIOMA:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_idioma WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.DOMINIONEGOCIO:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_dominionegocio WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                }
            }
        }

        // Métodos auxiliares assíncronos para ObterVagaRecrutamentoPorId
        private async Task BuscarSkillsVagaAsync(VagaRecrutamentoDTO vaga, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
                await connection.OpenAsync();

                var idVaga = vaga.Id;
                var queryKills = @"SELECT 
                                        tvs.id as Id,
                                        tvs.tb_vaga_id as VagaId,
                                        tvs.skill_id as SkillId,
                                        tvs.skill_nivel_id as SkillNivelId,
                                        tn.descricao as SkillNivelDescription,
                                        tvs.tb_item_perfil_id as TipoSkillId,
                                        tip.descricao as TypeSkillsDescription,
                                        tvs.ativo as Ativo,
                                        tvs.data_criacao as DataCriacao,
                                        tvs.data_alteracao as DataAlteracao,
                                        tvs.relevante as Relevante
                                    FROM 
                                        tb_vaga_skill tvs
                                    INNER JOIN 
                                        tb_item_perfil tip ON tip.id = tvs.tb_item_perfil_id
                                    INNER JOIN 
                                        tb_nivel tn ON tn.id = tvs.skill_nivel_id
                                    WHERE 
                                        tb_vaga_id  = @idVaga;";

                vaga.Skills = (await connection.QueryAsync<VagaSkillRecrutamentoDTO>(queryKills, new { idVaga })).ToList();

                foreach (var skill in vaga.Skills)
                {
                    switch (skill.TipoSkillId)
                    {
                        case (int)ItemPerfilEnum.COMPETENCIA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_competencia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault()?.ToString();
                            break;
                        case (int)ItemPerfilEnum.SOFTSKILL:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_softskill WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault()?.ToString();
                            break;
                        case (int)ItemPerfilEnum.METODOLOGIA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_metodologia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault()?.ToString();
                            break;
                        case (int)ItemPerfilEnum.IDIOMA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_idioma WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault()?.ToString();
                            break;
                        case (int)ItemPerfilEnum.DOMINIONEGOCIO:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_dominionegocio WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault()?.ToString();
                            break;
                    }
                }

                await connection.CloseAsync();
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task BuscarSlaAsync(VagaRecrutamentoDTO vaga, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
                await connection.OpenAsync();

                var querydataUltimaModificacaoStatus = @"SELECT 
                                                            data_alteracao 
                                                        FROM 
                                                            tb_vaga_fourmakers_log 
                                                        WHERE 
                                                            tb_vaga_id = @idVaga 
                                                            AND tb_status_vaga_cod = @vagaStatus 
                                                        ORDER BY 
                                                            data_alteracao ASC 
                                                        LIMIT 1";
                var dataUltimaModificacaoStatus = (await connection.QueryAsync<DateTime>(querydataUltimaModificacaoStatus, new { idVaga = vaga.Id, vagaStatus = vaga.StatusVagaCod })).FirstOrDefault();
                vaga.SlaDecorridoDaEtapaAtual = DateTime.Now.Subtract(dataUltimaModificacaoStatus);

                if (vaga.SlaContando)
                {
                    var querydataEntradaRefinamento = @"SELECT 
                                                            data_alteracao 
                                                        FROM 
                                                            tb_vaga_fourmakers_log 
                                                        WHERE 
                                                            tb_vaga_id = @idVaga 
                                                        AND tb_status_vaga_cod = @emRefinamento 
                                                        ORDER BY 
                                                            data_alteracao ASC 
                                                        LIMIT 1";
                    var dataEntradaRefinamento = (await connection.QueryAsync<DateTime>(querydataEntradaRefinamento, new { idVaga = vaga.Id, emRefinamento = StatusVagaRecrutamento.EmFoco })).FirstOrDefault();
                    vaga.SlaDecorridoTotal = DateTime.Now.Subtract(dataEntradaRefinamento);
                }

                await connection.CloseAsync();
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task BuscarUsuarioAlteradorAsync(VagaRecrutamentoDTO vaga, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
                await connection.OpenAsync();

                var queryUsuarioAlterador = @"
                        SELECT 
                            tc.nome_completo AS NomeUsuarioAlterador,
                            tvfl.data_alteracao
                        FROM 
                            tb_vaga_fourmakers_log tvfl
                        LEFT JOIN 
                            tb_colaborador tc ON tc.codigo_interno_colaborador = tvfl.tb_usuario_cpf
                        WHERE 
                            1=1
                            AND tvfl.tb_vaga_id = @idVaga
                        ORDER BY 
                            data_alteracao DESC
                        LIMIT 1;";

                vaga.NomeUsuarioAlterador = (await connection.QueryAsync<string>(queryUsuarioAlterador, new { idVaga = vaga.Id, emRefinamento = StatusVagaRecrutamento.EmFoco })).FirstOrDefault();

                await connection.CloseAsync();
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task BuscarEmailsAnaliseGestorAsync(VagaRecrutamentoDTO vaga, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
                await connection.OpenAsync();

                var queryEmailsAnaliseGestor = @"
                    SELECT email 
                    FROM tb_emails_analise_gestor 
                    WHERE tb_vaga_id = @vagaId 
                    AND ativo = 1";

                var emails = await connection.QueryAsync<string>(queryEmailsAnaliseGestor, new { vagaId = vaga.Id });
                vaga.EmailsAnaliseGestor = emails.ToList();

                await connection.CloseAsync();
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<VagaRecrutamentoDTO> ObterVagaPorIdEGestorExternoPerfilIdRecrutamento(Guid idPerfil)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                        id AS Id,
                        codigo AS Codigo,
                        titulo AS Titulo,
                        numero_de_vagas AS NumeroDeVagas,
                        custo_profissional AS CustoProfissional,
                        rate_card AS RateCard,
                        CONVERT(descricao USING utf8mb4) AS Descricao,
                        cargo AS Cargo,
                        data_criacao AS DataCriacao,
                        localizacao AS Localizacao,
                        estado AS Estado,
                        cidade AS Cidade,
                        tb_usuario_criador_cpf AS CpfUsuarioCriador,
                        tb_usuario_aprovador_cpf AS CpfUsuarioAprovador,
                        tb_gestor_cod AS CodigoGestor,
                        tb_status_vaga_cod AS StatusVagaCod,
                        tb_origem_vaga_cod AS OrigemVagaCod,
                        tb_org_id AS OrgId,
                        tb_gestor_externo_perfil_id AS IdPerfilGerador,
                        frequencia AS Frequencia,
                        modelo_trabalho_cod AS ModeloTrabalhoCod,
                        tb_colaborador_codigo_interno_colaborador_gestor_org_logada AS ColaboradorCodigoInternoColaboradorGestorOrgLogada,
                        proposta_crm AS PropostaCrm,
                        tb_tipo_vaga_id AS TipoVagaId,
                        tb_tipo_contratacao_id AS TipoContratacaoId,
                        tb_unidade_id AS UnidadeId,
                        tracking AS Tracking,
                        numero_vaga_cliente AS NumeroVagaCliente,
                        tb_cliente_org_codigo_fourmakers AS CodigoClienteFourmakers
                    FROM 
                        tb_vaga 
                    WHERE 
                        tb_gestor_externo_perfil_id = @idPerfil";

            var vaga = await connection.QueryFirstOrDefaultAsync<VagaRecrutamentoDTO>(query, new { idPerfil = idPerfil });

            if (vaga is null)
                return vaga;

            var idVaga = vaga.Id;
            var queryKills = @"SELECT 
                                    tvs.id as Id,
                                    tvs.skill_id as SkillId,
                                    tvs.skill_nivel_id as SkillNivelId,
                                    tn.descricao as SkillNivelDescription,
                                    tvs.tb_item_perfil_id as TypeSkills,
                                    tip.descricao as TypeSkillsDescription
                                FROM 
                                    tb_vaga_skill tvs
                                INNER JOIN 
                                    tb_item_perfil tip ON tip.id = tvs.tb_item_perfil_id
                                INNER JOIN 
                                    tb_nivel tn ON tn.id = tvs.skill_nivel_id
                                WHERE 
                                    tb_vaga_id  = @idVaga;";

            vaga.Skills = (await connection.QueryAsync<VagaSkillRecrutamentoDTO>(
                queryKills,
                new
                {
                    idVaga
                }
            )).ToList();

            foreach (var skill in vaga.Skills)
            {
                switch (skill.TipoSkillId)
                {
                    case (int)ItemPerfilEnum.COMPETENCIA:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_competencia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.SOFTSKILL:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_softskill WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.METODOLOGIA:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_metodologia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.IDIOMA:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_idioma WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.DOMINIONEGOCIO:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_dominionegocio WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                }
            }

            return vaga;
        }

        public async Task<IEnumerable<VagaRecrutamentoDTO>> ObterTodasVagasPorIdEGestorExternoPerfilIdRecrutamento(Guid idPerfil)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                        id AS Id,
                        codigo AS Codigo,
                        titulo AS Titulo,
                        numero_de_vagas AS NumeroDeVagas,
                        custo_profissional AS CustoProfissional,
                        rate_card AS RateCard,
                        CONVERT(descricao USING utf8mb4) AS Descricao,
                        cargo AS Cargo,
                        data_criacao AS DataCriacao,
                        localizacao AS Localizacao,
                        estado AS Estado,
                        cidade AS Cidade,
                        tb_usuario_criador_cpf AS CpfUsuarioCriador,
                        tb_usuario_aprovador_cpf AS CpfUsuarioAprovador,
                        tb_gestor_cod AS CodigoGestor,
                        tb_status_vaga_cod AS StatusVagaCod,
                        tb_origem_vaga_cod AS OrigemVagaCod,
                        tb_org_id AS OrgId,
                        tb_gestor_externo_perfil_id AS IdPerfilGerador,
                        frequencia AS Frequencia,
                        modelo_trabalho_cod AS ModeloTrabalhoCod,
                        tb_colaborador_codigo_interno_colaborador_gestor_org_logada AS ColaboradorCodigoInternoColaboradorGestorOrgLogada,
                        proposta_crm AS PropostaCrm,
                        tb_tipo_vaga_id AS TipoVagaId,
                        tb_tipo_contratacao_id AS TipoContratacaoId,
                        tb_unidade_id AS UnidadeId,
                        tracking AS Tracking,
                        numero_vaga_cliente AS NumeroVagaCliente,
                        tb_cliente_org_codigo_fourmakers AS CodigoClienteFourmakers
                    FROM 
                        tb_vaga 
                    WHERE 
                        tb_gestor_externo_perfil_id = @idPerfil
                    ORDER BY data_criacao DESC";

            var vagas = await connection.QueryAsync<VagaRecrutamentoDTO>(query, new { idPerfil = idPerfil });

            // Buscar informações complementares para cada vaga
            foreach (var vaga in vagas)
            {
                await BuscarSkillsVaga(connection, vaga);
                await BuscarSla(connection, vaga);
                await BuscarUsuarioAlterador(connection, vaga);
            }

            return vagas;
        }

        public async Task<IEnumerable<OpcaoContatoDTO>> ListarOpcoesContato()
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"SELECT 
                            id as Id, 
                            descricao as Descricao 
                        FROM 
                            tb_opcao_contato";

            return await connection.QueryAsync<OpcaoContatoDTO>(query);
        }

        public async Task<List<string>> ListarIdsDasOpcoesContato()
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"SELECT 
                                id
                            FROM 
                                tb_opcao_contato";

            return (await connection.QueryAsync<string>(query)).ToList();
        }

        public async Task<IEnumerable<StatusVagaRecrutamentoDTO>> ListarStatusVagaRecrutamento(int? orgIdUsuarioLogado)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"SELECT 
                            codigo as Codigo, 
                            descricao as Descricao 
                        FROM 
                            tb_status_vaga";

            if (orgIdUsuarioLogado == EnumORG.ROYAL_9.ToInt())
            {
                query += $" WHERE codigo NOT IN ({StatusVagaRecrutamento.AplicacaoDeTestes.ToInt()}, {StatusVagaRecrutamento.EntrevistaTecnica.ToInt()}, {StatusVagaRecrutamento.EntrevistaComGestorDaVaga.ToInt()})";
            }

            query += " ORDER BY ordem ASC";

            return (await connection.QueryAsync<StatusVagaRecrutamentoDTO>(query)).ToList();
        }

        public async Task<IEnumerable<HistoricoStatusVagaItemDTO>> ListarHistoricoStatusVaga(string vagaId)
        {
            var connection = _dapperConnection.GetConnection();
            // A log guarda qualquer alteração na vaga; o histórico de status deve listar só linhas em que o status mudou.
            var query = @"
                SELECT 
                    filtrado.DataAlteracao,
                    filtrado.StatusCod,
                    filtrado.StatusDescricao,
                    filtrado.NomeUsuarioAlterador
                FROM (
                    SELECT 
                        tvfl.id AS SortId,
                        tvfl.data_alteracao AS DataAlteracao,
                        tvfl.tb_status_vaga_cod AS StatusCod,
                        tsv.descricao AS StatusDescricao,
                        tc.nome_completo AS NomeUsuarioAlterador,
                        LAG(tvfl.tb_status_vaga_cod) OVER (
                            PARTITION BY tvfl.tb_vaga_id
                            ORDER BY tvfl.data_alteracao ASC, tvfl.id ASC
                        ) AS PrevStatusCod
                    FROM 
                        tb_vaga_fourmakers_log tvfl
                    LEFT JOIN 
                        tb_status_vaga tsv ON tsv.codigo = tvfl.tb_status_vaga_cod
                    LEFT JOIN 
                        tb_colaborador tc ON tc.codigo_interno_colaborador = tvfl.tb_usuario_cpf
                    WHERE 
                        tvfl.tb_vaga_id = @vagaId
                ) filtrado
                WHERE 
                    filtrado.PrevStatusCod IS NULL
                    OR filtrado.PrevStatusCod <> filtrado.StatusCod
                ORDER BY 
                    filtrado.DataAlteracao ASC,
                    filtrado.SortId ASC";

            var rows = (await connection.QueryAsync<HistoricoStatusVagaRow>(query, new { vagaId })).ToList();
            var now = DateTime.Now;
            var resultado = new List<HistoricoStatusVagaItemDTO>(rows.Count);

            for (var i = 0; i < rows.Count; i++)
            {
                var dataChegou = rows[i].DataAlteracao;
                var dataSaiu = i < rows.Count - 1 ? (DateTime?)rows[i + 1].DataAlteracao : null;
                var fimPeriodo = dataSaiu ?? now;
                var tempo = fimPeriodo - dataChegou;

                resultado.Add(new HistoricoStatusVagaItemDTO
                {
                    DataQueChegouNesteStatus = dataChegou,
                    DataQueSaiuDesteStatus = dataSaiu,
                    StatusDescricao = rows[i].StatusDescricao,
                    StatusCod = rows[i].StatusCod,
                    TempoQueFicouNesteStatus = tempo,
                    TempoQueFicouNesteStatusFormatado = FormatarDuracao(tempo),
                    NomeUsuarioAlterador = rows[i].NomeUsuarioAlterador
                });
            }

            return resultado;
        }

        private static string FormatarDuracao(TimeSpan duracao)
        {
            if (duracao.TotalDays >= 1)
                return $"{(int)duracao.TotalDays}d {duracao.Hours}h {duracao.Minutes}m";
            if (duracao.TotalHours >= 1)
                return $"{(int)duracao.TotalHours}h {duracao.Minutes}m";
            if (duracao.TotalMinutes >= 1)
                return $"{(int)duracao.TotalMinutes}m";
            return $"{duracao.Seconds}s";
        }

        public async Task<IEnumerable<MotivoPerdaVagaDTO>> ListarMotivosPerdaVaga()
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"SELECT 
                            id AS Id,
                            descricao AS Descricao,
                            explicacao AS Explicacao,
                            ordem AS Ordem
                        FROM 
                            tb_vaga_motivos_perda
                        ORDER BY 
                            ordem ASC";

            return await connection.QueryAsync<MotivoPerdaVagaDTO>(query);
        }

        public async Task AtualizarOrdemStatusVagaRecrutamento(List<StatusOrdemItem> statusOrdem, string codColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            // Obter ordens anteriores para log
            var codigos = statusOrdem.Select(s => s.Codigo).ToList();
            var queryOrdensAnteriores = @"SELECT codigo, ordem FROM tb_status_vaga WHERE codigo IN @Codigos";
            var ordensAnteriores = await connection.QueryAsync<(int Codigo, int Ordem)>(queryOrdensAnteriores, new { Codigos = codigos });

            var dictOrdensAnteriores = ordensAnteriores.ToDictionary(x => x.Codigo, x => x.Ordem);

            // Verificar se todos os códigos existem
            foreach (var item in statusOrdem)
            {
                if (!dictOrdensAnteriores.ContainsKey(item.Codigo))
                    throw new Exception($"Status de vaga com código {item.Codigo} não encontrado.");
            }

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Atualizar todas as ordens
                    var queryUpdate = @"UPDATE tb_status_vaga SET ordem = @Ordem WHERE codigo = @Codigo";
                    foreach (var item in statusOrdem)
                    {
                        await connection.ExecuteAsync(queryUpdate, new { Codigo = item.Codigo, Ordem = item.Ordem }, transaction);
                    }

                    // Inserir logs para cada alteração
                    var queryLog = @"
                        INSERT INTO tb_status_vaga_ordem_log (
                            id,
                            tb_status_vaga_codigo,
                            ordem_anterior,
                            ordem_nova,
                            tb_colaborador_codigo_interno_colaborador_alterador,
                            data_alteracao
                        ) VALUES (
                            @IdLog,
                            @Codigo,
                            @OrdemAnterior,
                            @OrdemNova,
                            @CodColaborador,
                            NOW()
                        )";

                    foreach (var item in statusOrdem)
                    {
                        var guidLog = Guid.NewGuid().ToString();
                        var ordemAnterior = dictOrdensAnteriores[item.Codigo];

                        await connection.ExecuteAsync(queryLog, new
                        {
                            IdLog = guidLog,
                            Codigo = item.Codigo,
                            OrdemAnterior = ordemAnterior,
                            OrdemNova = item.Ordem,
                            CodColaborador = codColaborador
                        }, transaction);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamentoPorParent(string vagaIdParent, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = QueryBaseListarVagasRecrutamento + @"
                    WHERE 
                        tv.tb_vaga_id_parent = @vagaIdParent
                        AND tv.tb_org_id = @orgId
                    ORDER BY 
                        tv.data_criacao DESC";

            var vagas = await connection.QueryAsync<VagaRecrutamentoDTO>(
                query,
                new
                {
                    vagaIdParent,
                    orgId
                }
            );

            foreach (var vaga in vagas)
            {
                await BuscarSkillsVaga(connection, vaga);
                await BuscarSla(connection, vaga);
                await BuscarUsuarioAlterador(connection, vaga);
            }

            return vagas;
        }

        public async Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamentoPorParentEmAndamento(string vagaIdParent, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            // Lista dos status de Vagas Finalizadas/Fechadas que devem ser EXCLUÍDAS da listagem
            var statusExcluidos = new[]
            {
                StatusVagaRecrutamento.Contratacao.ToInt(),
                StatusVagaRecrutamento.Cancelada.ToInt(),
                StatusVagaRecrutamento.Perdida.ToInt()
            };

            var query = QueryBaseListarVagasRecrutamento + @"
                    WHERE 
                        tv.tb_vaga_id_parent = @vagaIdParent
                        AND tv.tb_org_id = @orgId
                        AND tv.tb_status_vaga_cod NOT IN @statusExcluidos
                    ORDER BY 
                        tv.data_criacao DESC";

            var vagas = await connection.QueryAsync<VagaRecrutamentoDTO>(
                query,
                new
                {
                    vagaIdParent,
                    orgId,
                    statusExcluidos
                }
            );

            foreach (var vaga in vagas)
            {
                await BuscarSkillsVaga(connection, vaga);
                await BuscarSla(connection, vaga);
                await BuscarUsuarioAlterador(connection, vaga);
            }

            return vagas;
        }

        public async Task<VagaAnonymousDTO> ObterVagaRecrutamentoPorCodigoAnonymous(int codigo)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                        id AS Id,
                        codigo AS Codigo,
                        titulo AS Titulo,
                        CONVERT(descricao USING utf8mb4) AS Descricao,
                        cargo AS Cargo,
                        data_criacao AS DataCriacao,
                        localizacao AS Localizacao,
                        estado AS Estado,
                        cidade AS Cidade
                    FROM 
                        tb_vaga 
                    WHERE 
                        codigo = @codigo";

            var vaga = await connection.QueryFirstOrDefaultAsync<VagaRecrutamentoDTO>(query, new { codigo = codigo });

            if (vaga is null)
                throw new ApplicationException("Vaga não encontrada.");

            var vagaAnonymous = new VagaAnonymousDTO
            {
                Codigo = vaga.Codigo,
                Titulo = vaga.Titulo,
                Descricao = vaga.Descricao,
                Cargo = vaga.Cargo,
                DataCriacao = vaga.DataCriacao,
                Localizacao = vaga.Localizacao,
                Estado = vaga.Estado,
                Cidade = vaga.Cidade,
            };

            var idVaga = vaga.Id;
            var queryKills = @"SELECT 
                                    tvs.id as Id,
                                    tvs.tb_vaga_id as VagaId,
                                    tvs.skill_id as SkillId,
                                    tvs.skill_nivel_id as SkillNivelId,
                                    tn.descricao as SkillNivelDescription,
                                    tvs.tb_item_perfil_id as TipoSkillId,
                                    tip.descricao as TypeSkillsDescription
                                FROM 
                                    tb_vaga_skill tvs
                                INNER JOIN 
                                    tb_item_perfil tip ON tip.id = tvs.tb_item_perfil_id
                                INNER JOIN 
                                    tb_nivel tn ON tn.id = tvs.skill_nivel_id
                                WHERE 
                                    tb_vaga_id  = @idVaga;";

            vagaAnonymous.Skills = (await connection.QueryAsync<VagaSkillAnonymousRecrutamentoDTO>(
                queryKills,
                new
                {
                    idVaga
                }
            )).ToList();

            foreach (var skill in vagaAnonymous.Skills)
            {
                switch (skill.TipoSkillId)
                {
                    case (int)ItemPerfilEnum.COMPETENCIA:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_competencia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.SOFTSKILL:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_softskill WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.METODOLOGIA:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_metodologia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.IDIOMA:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_idioma WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                    case (int)ItemPerfilEnum.DOMINIONEGOCIO:
                        skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_dominionegocio WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                        break;
                }
            }

            return vagaAnonymous;
        }

        public async Task MudarStatusVaga(int codVaga, int codigoStatus, string codColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    UPDATE tb_vaga SET
                        tb_status_vaga_cod = @codigoStatus
                    WHERE 
                        codigo = @codVaga;

                    SELECT 
                        id AS Id,
                        codigo AS Codigo,
                        titulo AS Titulo,
                        numero_de_vagas AS NumeroDeVagas,
                        custo_profissional AS CustoProfissional,
                        rate_card AS RateCard,
                        CONVERT(descricao USING utf8mb4) AS Descricao,
                        cargo AS Cargo,
                        data_criacao AS DataCriacao,
                        localizacao AS Localizacao,
                        estado AS Estado,
                        cidade AS Cidade,
                        tb_usuario_criador_cpf AS CpfUsuarioCriador,
                        tb_usuario_aprovador_cpf AS CpfUsuarioAprovador,
                        tb_gestor_cod AS CodigoGestor,
                        tb_status_vaga_cod AS StatusVagaCod,
                        tb_origem_vaga_cod AS OrigemVagaCod,
                        tb_org_id AS OrgId,
                        tb_gestor_externo_perfil_id AS IdPerfilGerador,
                        frequencia AS Frequencia,
                        modelo_trabalho_cod AS ModeloTrabalhoCod,
                        tracking AS Tracking,
                        numero_vaga_cliente AS NumeroVagaCliente,
                        tb_cliente_org_codigo_fourmakers AS CodigoClienteFourmakers,
                        tb_modelo_trabalho_id AS ModeloTrabalhoId,
                        tb_permanencia_id AS PermanenciaId
                    FROM 
                        tb_vaga 
                    WHERE 
                        codigo = @codVaga;";

            var vaga = await connection.QueryFirstOrDefaultAsync<VagaRecrutamentoDTO>(query, new { codVaga, codigoStatus });

            var queryLog = @"
                            INSERT INTO tb_vaga_fourmakers_log (
                                id,
                                tb_vaga_id,
                                tb_status_vaga_cod,
                                tb_usuario_cpf,
                                data_alteracao,
                                objeto
                            ) VALUES (
                                @guidLog,
                                @idVaga,
                                @codigoStatus,
                                @codColaborador,
                                NOW(),
                                @objectVaga
                            );";

            var guidLog = Guid.NewGuid().ToString();
            var objectVaga = JsonConvert.SerializeObject(vaga);
            var idVaga = vaga.Id;

            await connection.ExecuteAsync(
                queryLog,
                new
                {
                    guidLog,
                    idVaga,
                    codigoStatus,
                    codColaborador,
                    objectVaga
                }
            );
        }

        public async Task AtualizarMotivoPerdaVaga(int codigoVaga, Guid idMotivoPerda)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    UPDATE tb_vaga SET
                        tb_vaga_motivos_perda_id = @idMotivoPerda,
                        data_ultima_alteracao = NOW()
                    WHERE 
                        codigo = @codigoVaga;";

            await connection.ExecuteAsync(query, new { codigoVaga, idMotivoPerda });
        }

        public async Task<IEnumerable<int>> ListarIdsStatusVagaRecrutamento()
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"SELECT 
                                codigo
                            FROM 
                                tb_status_vaga";

            return (await connection.QueryAsync<int>(query)).ToList();
        }

        public async Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamento(int limite, int cursor, string busca, string status, string dataInicio, string dataFim, List<string> clientesPermitidos = null)
        {
            var connection = _dapperConnection.GetConnection();

            var query = QueryBaseListarVagasRecrutamento + @"
                    WHERE 
                        1=1
                        AND (@Busca IS NULL OR @Busca = '' OR
                            LOWER(tco.nome_cliente) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(tge.nome) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(titulo) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(CONVERT(tv.descricao USING utf8mb4)) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(cargo) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(tv.codigo) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(tcCriador.nome_completo) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(tcRecrutador.nome_completo) LIKE CONCAT('%', LOWER(@Busca), '%') OR
                            LOWER(ttv.descricao) LIKE CONCAT('%', LOWER(@Busca), '%'))
                        AND (@Status IS NULL OR @Status = '' OR FIND_IN_SET(tv.tb_status_vaga_cod, @Status))
                        AND tv.data_criacao BETWEEN @DataInicio AND @DataFim";

            // Filtrar por clientes permitidos se houver restrições
            if (clientesPermitidos != null && clientesPermitidos.Any())
            {
                query += @"
                        AND tco.codigo_cliente IN @ClientesPermitidos";
            }
            else if (clientesPermitidos != null && clientesPermitidos.Count == 0)
            {
                // Se a lista estiver vazia, não retornar nenhuma vaga
                query += @"
                        AND 1=0";
            }

            query += @"
                    ORDER BY 
                        tv.data_ultima_alteracao DESC
                    LIMIT 
                        @Limite
                    OFFSET
                        @Cursor;";

            var vagas = await connection.QueryAsync<VagaRecrutamentoDTO>(
                query,
                new
                {
                    Busca = busca,
                    Status = status,
                    Limite = limite,
                    Cursor = cursor,
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    ClientesPermitidos = clientesPermitidos
                }
            );

            var vagasList = vagas.ToList();
            await PopularDadosComplementaresVagas(vagasList, connection);

            return vagasList;
        }

        public async Task<List<ListarVagasEmBancoDeTalentosResult>> ListarVagasPipeline(DateTime dataInicio, DateTime dataFim, string cliente, int cursor, int limite, string busca)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                            SELECT 
                                tv.id as Id,
                                tco.nome_cliente as Cliente,
                                tv.titulo as Titulo,
                                tge.nome as Gestor,
                                tv.tb_usuario_criador_cpf as Criador,
                                tv.data_criacao as Criacao,
                                tv.numero_de_vagas as Posicoes,
                                CONVERT(tv.descricao USING utf8mb4) AS Descricao,
                                tv.localizacao as Localizacao,
                                tv.estado as Estado,
                                tv.codigo as Codigo,
                                tmt.descricao as ModalidadeDescricao,
                                tv.cargo as Cargo,
                                tv.cidade as Cidade,
                                tv.tracking as Tracking,
                                tv.numero_vaga_cliente as NumeroVagaCliente,
                                tv.tb_cliente_org_codigo_fourmakers as CodigoClienteFourmakers,
                                tv.tb_org_id as OrgId
                            FROM 
                                tb_vaga tv
                            LEFT JOIN 
                                tb_gestor_externo tge ON tv.tb_gestor_cod = tge.cod_gestor_externo
                            LEFT JOIN 
                                tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente
                            LEFT JOIN 
                                tb_modelo_trabalho tmt ON tmt.codigo = tv.modelo_trabalho_cod
                            WHERE 1=1
                                AND (@CodigoCliente IS NULL OR tge.codigo_cliente = @CodigoCliente)
                                AND
                                (
                                    (@Busca IS NULL OR (
                                        LOWER(tge.nome) LIKE CONCAT('%', @Busca, '%') OR
                                        LOWER(tv.titulo) LIKE CONCAT('%', @Busca, '%')
                                    ))
                                )
                                AND tv.data_criacao between @DataInicio AND @DataFim
                                AND tv.tb_status_vaga_cod = @Pipeline
                            ORDER BY 
	                            tv.data_criacao DESC
                            LIMIT 
	                            @Limite
                            OFFSET
                                @Offset;";

            var vagas = await connection.QueryAsync<ListarVagasEmBancoDeTalentosResult>(
                query,
                new
                {
                    DataInicio = dataInicio.ToString("yyyy-MM-dd"),
                    DataFim = dataFim.ToString("yyyy-MM-dd"),
                    CodigoCliente = cliente,
                    Busca = busca,
                    Limite = limite,
                    Offset = cursor,
                    Pipeline = StatusVagaRecrutamento.BancoDeTalentos.ToInt().ToString()
                }
            );

            foreach (var vaga in vagas)
            {
                var idVaga = vaga.Id;
                var queryKills = @"SELECT 
                                        tvs.id as Id,
                                        tvs.skill_id as SkillId,
                                        tvs.skill_nivel_id as SkillNivelId,
                                        tn.descricao as SkillNivelDescription,
                                        tvs.tb_item_perfil_id as TipoSkillId,
                                        tip.descricao as TypeSkillsDescription
                                    FROM 
                                        tb_vaga_skill tvs
                                    INNER JOIN 
	                                    tb_item_perfil tip ON tip.id = tvs.tb_item_perfil_id
                                    INNER JOIN 
	                                    tb_nivel tn ON tn.id = tvs.skill_nivel_id
                                WHERE 
	                                tb_vaga_id  =  @idVaga;";

                vaga.Skills = (await connection.QueryAsync<VagaSkillRecrutamentoDTO>(
                    queryKills,
                    new
                    {
                        idVaga
                    }
                )).ToList();

                foreach (var skill in vaga.Skills)
                {
                    switch (skill.TipoSkillId)
                    {
                        case (int)ItemPerfilEnum.COMPETENCIA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_competencia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                            break;
                        case (int)ItemPerfilEnum.SOFTSKILL:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_softskill WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                            break;
                        case (int)ItemPerfilEnum.METODOLOGIA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_metodologia WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                            break;
                        case (int)ItemPerfilEnum.IDIOMA:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_idioma WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                            break;
                        case (int)ItemPerfilEnum.DOMINIONEGOCIO:
                            skill.SkillDescription = (await connection.QueryAsync<string>($"SELECT descricao FROM tb_dominionegocio WHERE id = @skillId", new { skillId = skill.SkillId })).FirstOrDefault().ToString();
                            break;
                    }
                }

            }

            return vagas.ToList();
        }

        public async Task IniciarSla(string idVaga)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    UPDATE tb_vaga SET
                        sla_contando = 1
                    WHERE 
                        id = @idVaga";

            await connection.ExecuteAsync(query, new { idVaga });
        }

        public async Task<string> ObterIdVagaPorCodigo(int codVaga)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                        id
                    FROM 
                        tb_vaga 
                    WHERE 
                        codigo = @codigo";

            return await connection.QueryFirstOrDefaultAsync<string>(query, new { codigo = codVaga });
        }

        public async Task<bool> EstaVagaExiste(int codVaga)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        SELECT EXISTS (
                                       SELECT 
					                         1
					                     FROM 
					                         tb_vaga tv
					                     WHERE 
					                         tv.codigo = @codVaga
                                   );";

            var parameters = new { codVaga };

            return await connection.ExecuteScalarAsync<bool>(query, parameters);
        }

        public async Task<bool> EstaVagaExistePorId(string idVaga)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        SELECT EXISTS (
                                       SELECT 
					                         1
					                     FROM 
					                         tb_vaga tv
					                     WHERE 
					                         tv.id = @idVaga
                                   );";

            var parameters = new { idVaga };

            return await connection.ExecuteScalarAsync<bool>(query, parameters);
        }

        public async Task<VagaRecrutamentoDTO> ObterVagaRecrutamentoPorId(string vagaId)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = QueryBaseListarVagasRecrutamento + @"
                        WHERE 
                            tv.id = @vagaId";

                var vaga = await connection.QueryFirstOrDefaultAsync<VagaRecrutamentoDTO>(query, new { vagaId = vagaId });

                if (vaga is null)
                    return vaga;

                var semaphore = new SemaphoreSlim(4, 4);
                var tasks = new List<Task>();

                tasks.Add(BuscarSkillsVagaAsync(vaga, semaphore));
                tasks.Add(BuscarSlaAsync(vaga, semaphore));
                tasks.Add(BuscarUsuarioAlteradorAsync(vaga, semaphore));
                tasks.Add(BuscarEmailsAnaliseGestorAsync(vaga, semaphore));

                await Task.WhenAll(tasks);

                return vaga;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task InserirInformacoesComplementaresVagaRecrutamento(InserirInformacoesComplementaresVagaRecrutamentoParam param, VagaRecrutamentoDTO vaga, string codColaboradorLogado)
        {
            var connection = _dapperConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var query = @"
                    UPDATE tb_vaga 
                    SET 
                        tb_colaborador_codigo_interno_colaborador_gestor_org_logada = @colaboradorGestorOrgLogada,
                        proposta_crm = @propostaCrm,
                        tb_tipo_vaga_id = @tipoVagaId,
                        tb_tipo_contratacao_id = @tipoContratacaoId,
                        tb_unidade_id = @unidadeId,
                        data_ultima_alteracao = NOW(),
                        numero_de_vagas = @numeroDeVagas,
                        tb_colaborador_codigo_interno_colaborador_recrutador = @RecrutadorVaga,
                        maquina = @MaquinaColaborador,
                        observacoes_internas = @ObservacoesInternas
                    WHERE 
                        id = @idVaga;";

                var parametros = new
                {
                    idVaga = param.IdVaga,
                    colaboradorGestorOrgLogada = param.ColaboradorCodigoInternoColaboradorGestorOrgLogada,
                    propostaCrm = param.PropostaCrm,
                    tipoVagaId = param.TipoVagaId,
                    tipoContratacaoId = param.TipoContratacaoId,
                    unidadeId = param.UnidadeId,
                    numeroDeVagas = param.NumeroDeVagas,
                    param.RecrutadorVaga,
                    param.MaquinaColaborador,
                    ObservacoesInternas = param.ObservacoesInternas
                };

                await connection.ExecuteAsync(query, parametros, transaction);

                var deleteEntrevistadores = @"DELETE FROM tb_vaga_entrevistador WHERE tb_vaga_id = @vagaId;";
                await connection.ExecuteAsync(deleteEntrevistadores, new { vagaId = param.IdVaga }, transaction);

                if (param.CodColaboradoresEntrevistadores != null && param.CodColaboradoresEntrevistadores.Count > 0)
                {
                    var insertEntrevistador = @"INSERT INTO tb_vaga_entrevistador (tb_vaga_id, tb_colaborador_codigo_interno_colaborador_entrevistador, data_criacao) VALUES (@vagaId, @codigo, NOW());";
                    foreach (var codigo in param.CodColaboradoresEntrevistadores)
                    {
                        await connection.ExecuteAsync(insertEntrevistador, new { vagaId = param.IdVaga, codigo }, transaction);
                    }
                }

                // Inserir emails de análise de gestor
                if (param.EmailsAnaliseGestor != null && param.EmailsAnaliseGestor.Count > 0)
                {
                    // Primeiro, remover emails existentes para esta vaga
                    var deleteEmails = @"DELETE FROM tb_emails_analise_gestor WHERE tb_vaga_id = @vagaId;";
                    await connection.ExecuteAsync(deleteEmails, new { vagaId = param.IdVaga }, transaction);

                    // Inserir novos emails
                    var insertEmail = @"INSERT INTO tb_emails_analise_gestor (tb_vaga_id, email, data_criacao, ativo) VALUES (@vagaId, @email, NOW(), 1);";
                    foreach (var email in param.EmailsAnaliseGestor)
                    {
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            await connection.ExecuteAsync(insertEmail, new { vagaId = param.IdVaga, email }, transaction);
                        }
                    }
                }

                var queryLog = @"
                    INSERT INTO tb_vaga_fourmakers_log (
                        id,
                        tb_vaga_id,
                        tb_status_vaga_cod,
                        tb_usuario_cpf,
                        data_alteracao,
                        objeto
                    ) VALUES (
                        @guidLog,
                        @idVaga,
                        @statusVagaCod,
                        @usuarioCriador,
                        NOW(),
                        @objectVaga
                    );";

                var guidLog = Guid.NewGuid().ToString();
                var idVaga = param.IdVaga;
                var statusVagaCod = vaga.StatusVagaCod;
                var usuarioCriador = codColaboradorLogado;
                var objectVaga = JsonConvert.SerializeObject(vaga);

                await connection.ExecuteAsync(
                    queryLog,
                    new
                    {
                        guidLog,
                        idVaga,
                        statusVagaCod,
                        usuarioCriador,
                        objectVaga
                    },
                    transaction
                );

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<IEnumerable<TipoVagaDTO>> ListarTiposVaga()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    id AS Id,
                    descricao AS Descricao
                FROM 
                    tb_tipo_vaga";

            return await connection.QueryAsync<TipoVagaDTO>(query);
        }

        public async Task<IEnumerable<NivelVagaDTO>> ListarNiveisVaga()
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    CAST(id AS CHAR(36)) AS Id,
                    descricao AS Descricao,
                    codigo AS Codigo
                FROM
                    tb_nivel_vaga
                ORDER BY
                    codigo ASC";

            return await connection.QueryAsync<NivelVagaDTO>(query);
        }

        public async Task<IEnumerable<TipoContratacaoDTO>> ListarTiposContratacao()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    id AS Id,
                    descricao AS Descricao
                FROM 
                    tb_tipo_contratacao;";

            return await connection.QueryAsync<TipoContratacaoDTO>(query);
        }

        public async Task<IEnumerable<UnidadeDTO>> ListarUnidades()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    id AS Id,
                    descricao AS Descricao
                FROM 
                    tb_unidade;";

            return await connection.QueryAsync<UnidadeDTO>(query);
        }

        public async Task<int> CountVagasRecrutamento()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                        count(*)
                    FROM 
                        tb_vaga tv
                    LEFT JOIN 
	                    tb_gestor_externo tge ON tv.tb_gestor_cod  = tge.cod_gestor_externo
                    LEFT JOIN 
                        tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = tv.tb_org_id";

            var vagas = await connection.QueryFirstOrDefaultAsync<int>(
                query
            );

            return vagas;
        }

        public async Task<int> CountVagasRecrutamentoPorOrg(int? orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                        count(*)
                    FROM 
                        tb_vaga tv
                    LEFT JOIN 
	                    tb_gestor_externo tge ON tv.tb_gestor_cod  = tge.cod_gestor_externo
                    LEFT JOIN 
                        tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = tv.tb_org_id
                    WHERE 
                        tv.tb_org_id = @OrgId";

            var vagas = await connection.QueryFirstOrDefaultAsync<int>(
                query,
                    new
                    {
                        OrgId = orgId
                    }
            );

            return vagas;
        }

        public async Task<ContadorVagasPorStatusResultDTO> CountVagasRecrutamentoPorStatus(string status = null, string dataInicio = null, string dataFim = null, List<string> clientesPermitidos = null)
        {
            var connection = _dapperConnection.GetConnection();

            // Primeiro, buscar todos os status possíveis
            var queryTodosStatus = @"
                SELECT 
                    codigo as Codigo,
                    descricao as Descricao
                FROM 
                    tb_status_vaga
                ORDER BY 
                    codigo ASC";

            var todosStatus = await connection.QueryAsync<StatusVagaRecrutamentoDTO>(queryTodosStatus);

            // Inicializar a lista com todos os status com contagem zero
            var contadoresPorStatus = new List<ContadorVagasPorStatusDTO>();
            foreach (var statusItem in todosStatus)
            {
                contadoresPorStatus.Add(new ContadorVagasPorStatusDTO
                {
                    CodigoStatus = statusItem.Codigo.ToInt(),
                    DescricaoStatus = statusItem.Descricao,
                    Quantidade = 0
                });
            }

            // Agora buscar as contagens reais com os filtros aplicados
            var query = @"
                    SELECT 
                        tv.tb_status_vaga_cod AS CodigoStatus,
                        tsv.descricao AS DescricaoStatus,
                        COUNT(*) AS Quantidade
                    FROM 
                        tb_vaga tv
                    LEFT JOIN 
                        tb_status_vaga tsv ON tsv.codigo = tv.tb_status_vaga_cod
                    LEFT JOIN 
                        tb_gestor_externo tge ON tv.tb_gestor_cod = tge.cod_gestor_externo AND tv.tb_org_id = tge.tb_org_id
                    LEFT JOIN 
                        tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = tv.tb_org_id
                    WHERE 
                        1=1
                        AND (@Status IS NULL OR @Status = '' OR FIND_IN_SET(tv.tb_status_vaga_cod, @Status))
                        AND (@DataInicio IS NULL OR @DataFim IS NULL OR tv.data_criacao BETWEEN @DataInicio AND @DataFim)";

            // Filtrar por clientes permitidos se houver restrições
            if (clientesPermitidos != null && clientesPermitidos.Any())
            {
                query += @"
                        AND tco.codigo_cliente IN @ClientesPermitidos";
            }
            else if (clientesPermitidos != null && clientesPermitidos.Count == 0)
            {
                // Se a lista estiver vazia, não retornar nenhuma vaga
                query += @"
                        AND 1=0";
            }

            query += @"
                    GROUP BY 
                        tv.tb_status_vaga_cod, tsv.descricao
                    ORDER BY 
                        tv.tb_status_vaga_cod";

            var parametros = new
            {
                Status = status,
                DataInicio = dataInicio,
                DataFim = dataFim,
                ClientesPermitidos = clientesPermitidos != null && clientesPermitidos.Any() ? clientesPermitidos : null
            };

            var contadoresReais = await connection.QueryAsync<ContadorVagasPorStatusDTO>(query, parametros);
            
            // Atualizar as contagens reais
            foreach (var contadorReal in contadoresReais)
            {
                var contadorExistente = contadoresPorStatus.FirstOrDefault(c => c.CodigoStatus == contadorReal.CodigoStatus);
                if (contadorExistente != null)
                {
                    contadorExistente.Quantidade = contadorReal.Quantidade;
                }
            }
            
            var totalGeral = contadoresPorStatus.Sum(c => c.Quantidade);

            return new ContadorVagasPorStatusResultDTO
            {
                ContadoresPorStatus = contadoresPorStatus,
                TotalGeral = totalGeral
            };
        }

        public async Task<ContadorVagasPorStatusResultDTO> CountVagasRecrutamentoPorStatusPorOrg(int? orgId, string status = null, string dataInicio = null, string dataFim = null, List<string> clientesPermitidos = null)
        {
            var connection = _dapperConnection.GetConnection();

            // Primeiro, buscar todos os status possíveis
            var queryTodosStatus = @"
                SELECT 
                    codigo as Codigo,
                    descricao as Descricao
                FROM 
                    tb_status_vaga
                ORDER BY 
                    codigo ASC";

            var todosStatus = await connection.QueryAsync<StatusVagaRecrutamentoDTO>(queryTodosStatus);

            // Inicializar a lista com todos os status com contagem zero
            var contadoresPorStatus = new List<ContadorVagasPorStatusDTO>();
            foreach (var statusItem in todosStatus)
            {
                contadoresPorStatus.Add(new ContadorVagasPorStatusDTO
                {
                    CodigoStatus = statusItem.Codigo.ToInt(),
                    DescricaoStatus = statusItem.Descricao,
                    Quantidade = 0
                });
            }

            // Agora buscar as contagens reais com os filtros aplicados
            var query = @"
                    SELECT 
                        tv.tb_status_vaga_cod AS CodigoStatus,
                        tsv.descricao AS DescricaoStatus,
                        COUNT(*) AS Quantidade
                    FROM 
                        tb_vaga tv
                    LEFT JOIN 
                        tb_status_vaga tsv ON tsv.codigo = tv.tb_status_vaga_cod
                    LEFT JOIN 
                        tb_gestor_externo tge ON tv.tb_gestor_cod = tge.cod_gestor_externo AND tv.tb_org_id = tge.tb_org_id
                    LEFT JOIN 
                        tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = tv.tb_org_id
                    WHERE 
                        1=1
                        AND (@OrgId IS NULL OR tv.tb_org_id = @OrgId)
                        AND (@Status IS NULL OR @Status = '' OR FIND_IN_SET(tv.tb_status_vaga_cod, @Status))";

            // Aplicar filtro de data se ambos os valores estiverem presentes
            if (!string.IsNullOrEmpty(dataInicio) && !string.IsNullOrEmpty(dataFim))
            {
                query += @"
                        AND tv.data_criacao BETWEEN @DataInicio AND @DataFim";
            }
            else if (!string.IsNullOrEmpty(dataInicio))
            {
                query += @"
                        AND tv.data_criacao >= @DataInicio";
            }
            else if (!string.IsNullOrEmpty(dataFim))
            {
                query += @"
                        AND tv.data_criacao <= @DataFim";
            }

            // Filtrar por clientes permitidos se houver restrições
            if (clientesPermitidos != null && clientesPermitidos.Any())
            {
                query += @"
                        AND tco.codigo_cliente IN @ClientesPermitidos";
            }
            else if (clientesPermitidos != null && clientesPermitidos.Count == 0)
            {
                // Se a lista estiver vazia, não retornar nenhuma vaga
                query += @"
                        AND 1=0";
            }

            query += @"
                    GROUP BY 
                        tv.tb_status_vaga_cod, tsv.descricao
                    ORDER BY 
                        tv.tb_status_vaga_cod";

            var parametros = new
            {
                OrgId = orgId,
                Status = status,
                DataInicio = dataInicio,
                DataFim = dataFim,
                ClientesPermitidos = clientesPermitidos != null && clientesPermitidos.Any() ? clientesPermitidos : null
            };

            var contadoresReais = await connection.QueryAsync<ContadorVagasPorStatusDTO>(query, parametros);
            
            // Atualizar as contagens reais
            foreach (var contadorReal in contadoresReais)
            {
                var contadorExistente = contadoresPorStatus.FirstOrDefault(c => c.CodigoStatus == contadorReal.CodigoStatus);
                if (contadorExistente != null)
                {
                    contadorExistente.Quantidade = contadorReal.Quantidade;
                }
            }
            
            var totalGeral = contadoresPorStatus.Sum(c => c.Quantidade);

            return new ContadorVagasPorStatusResultDTO
            {
                ContadoresPorStatus = contadoresPorStatus,
                TotalGeral = totalGeral
            };
        }
        public async Task<IEnumerable<TipoEmpregoLinkedin>> ListarTiposEmpregosLinkedin()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    id AS Id,
                    descricao AS Descricao
                FROM 
                    tb_tipos_emprego_linkedin;";

            return await connection.QueryAsync<TipoEmpregoLinkedin>(query);
        }

        public async Task<IEnumerable<NivelExperienciaLinkedin>> ListarNiveisExperienciaLinkedin()
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    id AS Id,
                    descricao AS Descricao
                FROM 
                    tb_niveis_experiencia_linkedin;";

            return await connection.QueryAsync<NivelExperienciaLinkedin>(query);
        }

        public async Task<TipoEmpregoLinkedin> ObterTipoEmpregoLinkedinPorId(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    id AS Id,
                    descricao AS Descricao
                FROM 
                    tb_tipos_emprego_linkedin
                WHERE 
                    id = @Id;";

            return await connection.QueryFirstOrDefaultAsync<TipoEmpregoLinkedin>(query, new { Id = id });
        }

        public async Task<NivelExperienciaLinkedin> ObterNivelExperienciaLinkedinPorId(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    id AS Id,
                    descricao AS Descricao
                FROM 
                    tb_niveis_experiencia_linkedin
                WHERE 
                    id = @Id;";

            return await connection.QueryFirstOrDefaultAsync<NivelExperienciaLinkedin>(query, new { Id = id });
        }

        public async Task<List<dynamic>> BuscaRelatorioVagas(int orgId, DateTime dataInicio, DateTime dataFim)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    tv.codigo AS 'Cód. Vaga', 
                    tv.numero_de_vagas AS 'Quantidade', 
                    tu.descricao AS 'Unidade Resp.', 
                    DATE_FORMAT(tv.data_criacao, '%d/%m/%Y') AS 'Dt. Abertura', 
                    tgep.nome_perfil AS 'Perfil', 
                    ttv.descricao AS 'Tipo da Vaga',
                    tsv.descricao AS 'Status', 
                    tc.nome_completo AS 'Solicitante', 
                    tco.nome_cliente AS 'Cliente',
                    tge.nome AS 'Gestor no Cliente',
                    DATE_FORMAT(tv.data_criacao, '%d/%m/%Y') AS 'Data Criação',
                    tv.maquina AS Maquina,
                    tcRecrutador.nome_completo AS Recrutador,
                    COALESCE(stats.status_1, 0) AS 'Inscrição registrada',
                    COALESCE(stats.status_2, 0) AS 'Entrevista Inicial',
                    COALESCE(stats.status_3, 0) AS 'Pesquisa Interna',
                    COALESCE(stats.status_4, 0) AS 'Analise do Gestor',
                    COALESCE(stats.status_5, 0) AS 'Testes Comportamentais',
                    COALESCE(stats.status_6, 0) AS 'Testes Técnicos',
                    COALESCE(stats.status_7, 0) AS 'Entrevista com Cliente',
                    COALESCE(stats.status_8, 0) AS 'Carta-Oferta',
                    COALESCE(stats.status_9, 0) AS 'Aprovado',
                    COALESCE(stats.status_10, 0) AS 'Reprovado',
                    COALESCE(stats.status_11, 0) AS 'Contratado'
                FROM tb_vaga tv
                    INNER JOIN tb_gestor_externo_perfil tgep ON tgep.id = tv.tb_gestor_externo_perfil_id
                    INNER JOIN tb_gestor_externo tge ON tv.tb_gestor_cod = tge.cod_gestor_externo AND tge.tb_org_id = tv.tb_org_id
                    INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente 
                    INNER JOIN tb_unidade tu ON tv.tb_unidade_id = tu.id
                    INNER JOIN tb_colaborador tc ON tc.codigo_Interno_colaborador = tv.tb_usuario_criador_cpf 
                    INNER JOIN tb_status_vaga tsv ON tsv.codigo = tv.tb_status_vaga_cod
                    LEFT JOIN tb_tipo_vaga ttv ON ttv.id = tv.tb_tipo_vaga_id
                    LEFT JOIN tb_colaborador tcRecrutador ON tcRecrutador.codigo_Interno_colaborador = tv.tb_colaborador_codigo_interno_colaborador_recrutador
                    LEFT JOIN (
                        SELECT 
                            tcv.tb_vaga_id,
                            SUM(CASE WHEN tcv.tb_candidato_status_id = 1 THEN 1 ELSE 0 END) AS status_1,
                            SUM(CASE WHEN tcv.tb_candidato_status_id = 2 THEN 1 ELSE 0 END) AS status_2,
                            SUM(CASE WHEN tcv.tb_candidato_status_id = 3 THEN 1 ELSE 0 END) AS status_3,
                            SUM(CASE WHEN tcv.tb_candidato_status_id = 4 THEN 1 ELSE 0 END) AS status_4,
                            SUM(CASE WHEN tcv.tb_candidato_status_id = 5 THEN 1 ELSE 0 END) AS status_5,
                            SUM(CASE WHEN tcv.tb_candidato_status_id = 6 THEN 1 ELSE 0 END) AS status_6,
                            SUM(CASE WHEN tcv.tb_candidato_status_id = 7 THEN 1 ELSE 0 END) AS status_7,
                            SUM(CASE WHEN tcv.tb_candidato_status_id = 8 THEN 1 ELSE 0 END) AS status_8,
                            SUM(CASE WHEN tcv.tb_candidato_status_id = 9 THEN 1 ELSE 0 END) AS status_9,
                            SUM(CASE WHEN tcv.tb_candidato_status_id = 10 THEN 1 ELSE 0 END) AS status_10,
                            SUM(CASE WHEN tcv.tb_candidato_status_id = 11 THEN 1 ELSE 0 END) AS status_11
                        FROM tb_candidato_vaga tcv
                        INNER JOIN tb_candidato_status tcs ON tcs.id = tcv.tb_candidato_status_id
                        WHERE tcs.origem = 'Fourmakers'
                        GROUP BY tcv.tb_vaga_id
                    ) stats ON stats.tb_vaga_id = tv.id
                WHERE tv.tb_org_id = @OrgId
                      AND tv.data_criacao BETWEEN @dataInicio AND @dataFim;";

            var resultado = await connection.QueryAsync<dynamic>(query, new { OrgId = orgId, dataInicio, dataFim});
            return resultado.ToList();
        }

        public async Task<List<dynamic>> BuscaRelatorioProdutividade(int orgId, DateTime dataInicio, DateTime dataFim)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT  tv.codigo AS 'Codigo da vaga',
		                DATE_FORMAT(tv.data_criacao, '%d/%m/%Y') AS 'Data criacao da vaga',
		                DATE_FORMAT(tv.data_ultima_alteracao, '%d/%m/%Y') AS 'Data ultima alteracao da vaga',
                        tsv.descricao AS 'Status Vaga',
                        tvmp.descricao AS 'Motivo da Perda',
		                tv.titulo AS 'Nome da vaga',
		                tcCandidato.nome_completo AS 'Nome do candidato',
                        (SELECT nome_completo FROM tb_colaborador WHERE codigo_interno_colaborador = tcv.tb_colaborador_codigo_interno_colaborador_responsavel) AS 'Responsavel pelo Candidato',
		                DATE_FORMAT(tcv.data_ultima_alteracao, '%d/%m/%Y') AS 'Data ultima alteracao da candidatura',
    	                tcs.descricao AS 'Status da candidatura',
    	                (
                        SELECT tc_sub.nome_completo
                        FROM tb_candidato_vaga_log tcvl_sub
                        INNER JOIN tb_colaborador tc_sub
        	                ON tc_sub.codigo_interno_colaborador = tcvl_sub.tb_colaborador_codigo_interno_colaborador
                        WHERE tcvl_sub.tb_candidato_vaga_id = tcv.id
                        ORDER BY tcvl_sub.data_alteracao DESC
                        LIMIT 1
    	                ) AS 'Colaborador ultima alteracao da candidatura',
    	                tmd.descricao AS 'Motivo declinio',
                        tmr.descricao AS 'Motivo reprovacao'
                FROM tb_vaga tv
                INNER JOIN tb_candidato_vaga tcv 
	                ON tcv.tb_vaga_id = tv.id
                INNER JOIN tb_colaborador tcCandidato
	                ON tcCandidato.codigo_interno_colaborador = tcv.tb_colaborador_codigo_interno_colaborador
                INNER JOIN tb_candidato_status tcs
	                ON tcs.id = tcv.tb_candidato_status_id
                LEFT JOIN tb_motivo_declinio tmd 
	                ON tmd.id = tcv.tb_motivo_declinio_id
                LEFT JOIN tb_motivo_reprovacao tmr
                    ON tmr.id = tcv.tb_motivo_reprovacao_id 
                LEFT JOIN tb_status_vaga tsv 
	                ON tsv.codigo = tv.tb_status_vaga_cod
                LEFT JOIN tb_vaga_motivos_perda tvmp 
	                ON tvmp.id  = tv.tb_vaga_motivos_perda_id
                WHERE tv.tb_status_vaga_cod NOT IN (1, 9)
	                  AND tcv.tb_candidato_status_id != 1
	                  AND tv.tb_org_id = @orgId
                      AND tv.data_criacao BETWEEN @dataInicio AND @dataFim
                ORDER BY tv.codigo";

            var resultado = await connection.QueryAsync<dynamic>(query, new { orgId , dataInicio, dataFim});
            return resultado.ToList();
        }

        public async Task<List<dynamic>> RelatorioVagasCandidaturas(DateTime dataInicio, DateTime dataFim, int orgIdUsuarioLogado)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tv.proposta_crm                            AS 'Cód Proposta',
                    tv.codigo                                  AS 'Cód Vaga',
                    (tv.numero_de_vagas - tv.candidatos_contratados) AS Posições,
                    tc2.nome_completo                          AS 'Recrutador Responsável',
                    tv.titulo                                  AS Perfil,
                    tsv.descricao                              AS Status,
                    tvmp.descricao                             AS 'Motivo da Perda',
                    COUNT(DISTINCT CASE
                        WHEN tcs.origem = 'Fourmakers'
                         AND tcv.ativo = 1
                         AND tcs.descricao NOT IN (
                            'Inscrição registrada',
                            'Entrevista Inicial',
                            'Pesquisa Interna'
                         )
                        THEN tcv.id
                    END) AS 'CVs Encaminhados',
                    COUNT(DISTINCT CASE
                        WHEN tcs.origem = 'Fourmakers'
                         AND tcv.ativo = 1
                         AND tcs.descricao NOT IN (
                            'Inscrição registrada',
                            'Entrevista Inicial',
                            'Pesquisa Interna',
                            'Reprovado',
                            'Declinou'
                         )
                        THEN tcv.id
                    END) AS 'CVs Encaminhados Ativos',
                    MAX(CASE
                        WHEN tvfl.tb_status_vaga_cod = 3
                        THEN DATE(tvfl.data_alteracao)
                    END) AS 'Data Aprovação',
                    MAX(CASE
                        WHEN tvfl2.tb_status_vaga_cod = 2
                        THEN tc_sol.nome_completo
                    END) AS 'Nome do Solicitante'
                FROM tb_vaga tv
                INNER JOIN tb_status_vaga tsv
                    ON tsv.codigo = tv.tb_status_vaga_cod
                LEFT JOIN tb_colaborador tc2
                    ON tc2.codigo_interno_colaborador =
                       tv.tb_colaborador_codigo_interno_colaborador_recrutador
                LEFT JOIN tb_vaga_motivos_perda tvmp
                    ON tvmp.id = tv.tb_vaga_motivos_perda_id
                LEFT JOIN tb_candidato_vaga tcv
                    ON tcv.tb_vaga_id = tv.id
                LEFT JOIN tb_candidato_status tcs
                    ON tcs.id = tcv.tb_candidato_status_id
                LEFT JOIN tb_vaga_fourmakers_log tvfl
                    ON tvfl.tb_vaga_id = tv.id
                LEFT JOIN tb_vaga_fourmakers_log tvfl2
                    ON tvfl2.tb_vaga_id = tv.id
                LEFT JOIN tb_colaborador tc_sol
                    ON tc_sol.codigo_interno_colaborador = tvfl2.tb_usuario_cpf
                WHERE tv.data_criacao >= @dataInicio
                  AND tv.data_criacao < DATE_ADD(@dataFim, INTERVAL 1 DAY)
                  AND tv.tb_org_id = 2
                GROUP BY
                    tv.id,
                    tv.proposta_crm,
                    tv.codigo,
                    tv.numero_de_vagas,
                    tv.candidatos_contratados,
                    tc2.nome_completo,
                    tv.titulo,
                    tsv.descricao,
                    tvmp.descricao
                ORDER BY tv.codigo;";

            var resultado = await connection.QueryAsync<dynamic>(query, new { dataInicio, dataFim, orgIdUsuarioLogado });
            return resultado.ToList();
        }

        public async Task<List<string>> ObterEmailsAnaliseGestorPorVagaId(string vagaId)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();
            try
            {
                var query = @"
                    SELECT email 
                    FROM tb_emails_analise_gestor 
                    WHERE tb_vaga_id = @vagaId 
                    AND ativo = 1";

                var emails = await connection.QueryAsync<string>(query, new { vagaId = vagaId });
                return emails.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<CandidatoQuePassouPorAnaliseGestorDTO> ObterCandidatosQuePassaramPorAnaliseGestor(string codigoCandidato, string idCandidatura)
        {
            if (string.IsNullOrEmpty(codigoCandidato))
                return new CandidatoQuePassouPorAnaliseGestorDTO();

            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            try
            {
                var query = @"
                    SELECT DISTINCT 
                        tcv.tb_colaborador_codigo_interno_colaborador AS CodigoColaborador,
                        tc.nome_completo AS NomeCandidato,
                        tcs.descricao AS StatusAtual,
                        tcv.data_criacao AS DataCandidatura
                    FROM tb_candidato_vaga tcv
                    INNER JOIN tb_candidato_vaga_log tcvl ON tcvl.tb_candidato_vaga_id = tcv.id
                    INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tcv.tb_colaborador_codigo_interno_colaborador
                    INNER JOIN tb_candidato_status tcs ON tcs.id = tcv.tb_candidato_status_id AND tcs.origem = 'Fourmakers'
                    WHERE tcv.tb_colaborador_codigo_interno_colaborador = @codigosCandidatos
                    AND tcvl.tb_candidato_status_id = 4
                    AND tcv.id = @idCandidatura";

                var candidato = await connection.QueryAsync<CandidatoQuePassouPorAnaliseGestorDTO>(query, new { codigosCandidatos = codigoCandidato, idCandidatura = idCandidatura });
                return candidato?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<bool> AdicionarRecrutadorVaga(string vagaId, string codInternoColaboradorRecrutador)
        {
            var connection = _dapperConnection.GetConnection();

            try
            {
                var query = @"
                        UPDATE tb_vaga
                        SET tb_colaborador_codigo_interno_colaborador_recrutador = @codInternoColaboradorRecrutador
                        WHERE id = @vagaId";

                var parameters = new
                {
                    vagaId,
                    codInternoColaboradorRecrutador
                };

                var linhasAfetadas = await connection.ExecuteAsync(query, parameters);

                return linhasAfetadas > 0;
            }
            catch(Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CopiarCandidatosEInformacoesComplementares(string idVagaParent, string idVagaChild, string codColaborador, bool movidaAutomaticamente = false)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {

                // 1. Copiar informações complementares da vaga pai para a vaga filha
                var copiarInformacoesQuery = @"
                    UPDATE tb_vaga v_child
                    INNER JOIN tb_vaga v_parent ON v_parent.id = @idVagaParent
                    SET 
                        v_child.tb_colaborador_codigo_interno_colaborador_gestor_org_logada = v_parent.tb_colaborador_codigo_interno_colaborador_gestor_org_logada,
                        v_child.proposta_crm = v_parent.proposta_crm,
                        v_child.tb_tipo_vaga_id = v_parent.tb_tipo_vaga_id,
                        v_child.tb_tipo_contratacao_id = v_parent.tb_tipo_contratacao_id,
                        v_child.tb_unidade_id = v_parent.tb_unidade_id,
                        v_child.numero_de_vagas = v_parent.numero_de_vagas,
                        v_child.tb_colaborador_codigo_interno_colaborador_recrutador = v_parent.tb_colaborador_codigo_interno_colaborador_recrutador,
                        v_child.maquina = v_parent.maquina,
                        v_child.data_ultima_alteracao = NOW()
                    WHERE v_child.id = @idVagaChild";

                if(movidaAutomaticamente)
                    copiarInformacoesQuery = @"
                        UPDATE tb_vaga v_child
                        INNER JOIN tb_vaga v_parent ON v_parent.id = @idVagaParent
                        SET 
                            v_child.tb_colaborador_codigo_interno_colaborador_gestor_org_logada = v_parent.tb_colaborador_codigo_interno_colaborador_gestor_org_logada,
                            v_child.proposta_crm = v_parent.proposta_crm,
                            v_child.tb_tipo_vaga_id = v_parent.tb_tipo_vaga_id,
                            v_child.tb_tipo_contratacao_id = v_parent.tb_tipo_contratacao_id,
                            v_child.tb_unidade_id = v_parent.tb_unidade_id,
                            v_child.tb_colaborador_codigo_interno_colaborador_recrutador = v_parent.tb_colaborador_codigo_interno_colaborador_recrutador,
                            v_child.maquina = v_parent.maquina,
                            v_child.data_ultima_alteracao = NOW()
                        WHERE v_child.id = @idVagaChild";

                await connection.ExecuteAsync(copiarInformacoesQuery, new { idVagaParent, idVagaChild }, transaction);

                // 2. Copiar emails de análise gestor da vaga pai para a vaga filha
                var copiarEmailsQuery = @"
                    INSERT INTO tb_emails_analise_gestor (tb_vaga_id, email, data_criacao, ativo)
                    SELECT @idVagaChild, email, NOW(), ativo
                    FROM tb_emails_analise_gestor
                    WHERE tb_vaga_id = @idVagaParent AND ativo = 1";

                await connection.ExecuteAsync(copiarEmailsQuery, new { idVagaParent, idVagaChild }, transaction);

                // 3. Copiar candidatos da vaga pai para a vaga filha
                var copiarCandidatosQuery = @"
                    INSERT INTO tb_candidato_vaga (
                        id,
                        tb_vaga_id,
                        tb_colaborador_codigo_interno_colaborador,
                        tb_org_id,
                        tb_candidato_status_id,
                        data_criacao,
                        ativo
                    )
                    SELECT 
                        UUID(),
                        @idVagaChild,
                        tb_colaborador_codigo_interno_colaborador,
                        tb_org_id,
                        tb_candidato_status_id,
                        NOW(),
                        ativo
                    FROM tb_candidato_vaga
                    WHERE tb_vaga_id = @idVagaParent AND ativo = 1";

                await connection.ExecuteAsync(copiarCandidatosQuery, new { idVagaParent, idVagaChild }, transaction);

                // 4. Copiar logs de candidatos
                var copiarLogsQuery = @"
                    INSERT INTO tb_candidato_vaga_log (
                        id,
                        tb_candidato_vaga_id,
                        tb_candidato_status_id,
                        data_alteracao,
                        tb_colaborador_codigo_interno_colaborador
                    )
                    SELECT 
                        UUID(),
                        cv_child.id,
                        cvl.tb_candidato_status_id,
                        NOW(),
                        @codColaborador
                    FROM tb_candidato_vaga cv_parent
                    INNER JOIN tb_candidato_vaga cv_child ON cv_child.tb_colaborador_codigo_interno_colaborador = cv_parent.tb_colaborador_codigo_interno_colaborador
                    INNER JOIN tb_candidato_vaga_log cvl ON cvl.tb_candidato_vaga_id = cv_parent.id
                    WHERE cv_parent.tb_vaga_id = @idVagaParent 
                    AND cv_child.tb_vaga_id = @idVagaChild
                    AND cvl.data_alteracao = (
                        SELECT MAX(data_alteracao) 
                        FROM tb_candidato_vaga_log cvl2 
                        WHERE cvl2.tb_candidato_vaga_id = cv_parent.id
                    )";

                await connection.ExecuteAsync(copiarLogsQuery, new { idVagaParent, idVagaChild, codColaborador }, transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }        

        public async Task<TemplateDescricaoVagaDTO> BuscarTemplateDescricaoVaga(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                            SELECT texto_introducao AS TextoIntroducao, texto_finalizacao AS TextoFinalizacao
                            FROM tb_template_descricao_vaga
                            WHERE tb_org_id = @orgId;
                        ";

            return await connection.QuerySingleOrDefaultAsync<TemplateDescricaoVagaDTO>(
                query,
                new { orgId }
            );
        }

        public async Task AdicionarTemplateDescricaoVaga(int orgId, string textoIntroducao, string textoFinalizacao)
        {
            if (string.IsNullOrWhiteSpace(textoIntroducao))
                throw new ArgumentException("O texto de introdução do template é obrigatório.");

            if (string.IsNullOrWhiteSpace(textoFinalizacao))
                throw new ArgumentException("O texto de finalização do template é obrigatório.");

            if (await ExisteTemplateDescricaoVaga(orgId))
                throw new InvalidOperationException(
                    "Já existe um template de descrição de vaga cadastrado para esta organização."
                );

            var connection = _dapperConnection.GetConnection();

            var query = @"
                            INSERT INTO tb_template_descricao_vaga (id, tb_org_id, texto_introducao, texto_finalizacao)
                            VALUES (@id, @orgId, @textoIntroducao, @textoFinalizacao);
                        ";

            await connection.ExecuteAsync(query, new
            {
                id = Guid.NewGuid().ToString(),
                orgId,
                textoIntroducao,
                textoFinalizacao
            });
        }

        public async Task AtualizarTemplateDescricaoVaga(int orgId, string textoIntroducao, string textoFinalizacao)
        {
            var connection = _dapperConnection.GetConnection();

            if (string.IsNullOrWhiteSpace(textoIntroducao))
                throw new ArgumentException("O texto de introdução do template é obrigatório.");

            if (string.IsNullOrWhiteSpace(textoFinalizacao))
                throw new ArgumentException("O texto de finalização do template é obrigatório.");

            if (!await ExisteTemplateDescricaoVaga(orgId))
                throw new InvalidOperationException(
                    "Não existe template cadastrado para essa organização."
                );

            var query = @"
                            UPDATE tb_template_descricao_vaga
                            SET texto_introducao = @textoIntroducao, texto_finalizacao = @textoFinalizacao
                            WHERE tb_org_id = @orgId;
                        ";

            await connection.ExecuteAsync(query, new
            {
                orgId,
                textoIntroducao,
                textoFinalizacao
            });
        }

        public async Task ExcluirTemplateDescricaoVaga(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var existe = await ExisteTemplateDescricaoVaga(orgId);
            if (!existe)
                throw new Exception("Não existe template cadastrado para essa organização.");

            var query = @"
                            DELETE FROM tb_template_descricao_vaga
                            WHERE tb_org_id = @orgId;
                        ";

            await connection.ExecuteAsync(query, new { orgId });
        }

        private async Task<bool> ExisteTemplateDescricaoVaga(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                            SELECT 1
                            FROM tb_template_descricao_vaga
                            WHERE tb_org_id = @orgId
                            LIMIT 1;
                        ";

            var result = await connection.QueryFirstOrDefaultAsync<int?>(
                query,
                new { orgId }
            );

            return result.HasValue;
        }
    }
}