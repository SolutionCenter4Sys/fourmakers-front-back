using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.DomainModel.Competencia;
using Dapper;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Competencia
{
    public class CompetenciaColaboradorRepository : ICompetenciaColaboradorRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;

        public CompetenciaColaboradorRepository(ColaboradorContext colaboradorContext,
                                                IDBConnection dapperConnection)
        {
            _colaboradorContext = colaboradorContext;
            _dapperConnection = dapperConnection;
        }
        public List<CompetenciaColaboradorDTO> GetCompetenciaColaborador(string cpf)
        {
            return _colaboradorContext.tb_colaborador_competencia
                .Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1)
                .OrderBy(x => x.id)
                .ToList()
                .Select(x => new CompetenciaColaboradorDTO
            {
                ColaboradorCpf = cpf,
                Competencia = new CompetenciaDTO
                {
                    Id = x.competencia.id,
                    Descricao = x.competencia.descricao
                },
                Id = x.id,
                IdCompetencia = x.competencia_id,
                IdNivel = x.tb_nivel_id,
                Nivel = x.tb_nivel != null ? new NivelDTO
                {
                    Descricao = x.tb_nivel.descricao,
                    Id = x.tb_nivel.id
                } : null,
                Data = x.data_criacao,
                ListaIdCertificado = x.tb_colaborador_competencia_certificado
                    .Select(y => y.tb_certificado_id)
                    .ToList(),
                Certificados = x.tb_colaborador_competencia_certificado
                    .Select(y => BuildCertificadoDTO(y.tb_certificado))
                    .ToList()
            }).ToList();
        }

        public async Task<List<CompetenciaColaboradorDTO>> BuscarCompetenciaColaboradorPorCompetenciaIds(long[] competenciaIds)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                        SELECT
                            tcc.codigo_interno_colaborador AS ColaboradorCpf,
                            c.id AS CompetenciaId,
                            c.descricao AS CompetenciaDescricao,
                            tcc.id AS Id,
                            tcc.competencia_id AS IdCompetencia,
                            tcc.tb_nivel_id AS IdNivel,
                            tn.id AS NivelId,
                            tn.descricao AS NivelDescricao,
                            tn.prioridade_unificacao AS NivelPrioridadeUnificacao,
                            tn.ordem_exibicao as OrdemExibicao
                        FROM
                            tb_colaborador_competencia tcc
                        LEFT JOIN
                            tb_nivel tn ON tcc.tb_nivel_id = tn.id
                        LEFT JOIN
                            tb_competencia c ON tcc.competencia_id = c.id
                        WHERE
                            tcc.competencia_id IN @CompetenciaIds;";

            var result = await connection.QueryAsync<dynamic>(sql, new { CompetenciaIds = competenciaIds });

            var competencias = result.Select(x => new CompetenciaColaboradorDTO
            {
                ColaboradorCpf = x.ColaboradorCpf,
                Id = x.Id,
                IdCompetencia = x.IdCompetencia,
                IdNivel = x.IdNivel,
                Competencia = new CompetenciaDTO
                {
                    Id = x.CompetenciaId,
                    Descricao = x.CompetenciaDescricao
                },
                Nivel = x.NivelId != null ? new NivelDTO
                {
                    Id = x.NivelId,
                    Descricao = x.NivelDescricao,
                    PrioridadeUnificacao = x.NivelPrioridadeUnificacao,
                    OrdemExibicao = x.OrdemExibicao
                } : null
            }).ToList();

            return competencias;
        }

        public async Task ExcluirColaboradorCompetenciaPorId(long id)
        {
            var sql = @"DELETE FROM tb_colaborador_competencia WHERE id = @Id";

            var connection = _dapperConnection.GetConnection();

            await connection.ExecuteAsync(sql, new { Id = id }); // Executa o comando de exclus�o com o par�metro `id`
        }

        public List<long> ListarCompetenciasAtribuidas(string cpfColaborador)
        {
            try
            {
                var listaCompetenciasExistentes = _colaboradorContext.tb_colaborador_competencia
                                                         .Join(_colaboradorContext.tb_competencia,
                                                               cc => cc.competencia_id,
                                                               c => c.id,
                                                               (cc, c) => new { cc, c })
                                                         .Where(x => x.cc.codigo_interno_colaborador == cpfColaborador
                                                                     && x.cc.ativo == 1
                                                                     && x.c.ativo == 1)
                                                         .Select(x => x.cc.competencia_id)
                                                         .ToList();

                return listaCompetenciasExistentes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CertificadoDTO> GetCertificadoColaborador(string cpf)
        {
            return _colaboradorContext.tb_certificado.Where(x => x.codigo_interno_colaborador == cpf && x.tb_colaborador_formacao.Any() == false && x.ativo == (sbyte)1).ToList().Select(x => BuildCertificadoDTO(x)).ToList();
        }

        private CertificadoDTO BuildCertificadoDTO(tb_certificado certificado)
        {
            if (certificado == null)
                return null;
            string pathThumb = null;
            if (certificado.path.IndexOf("pdf") != -1)
                pathThumb = certificado.path.Replace(".pdf", "_thumb.jpg");

            if (certificado.path == "")
            {
                certificado.path = null;
            }
            return new CertificadoDTO
            {
                Path = certificado.path,
                Thumb = pathThumb,
                conclusao = certificado.data_conclusao,
                cargaHoraria = certificado.carga_horaria,
                ativo = true,
                descricao = certificado.descricao,
                instituicao = certificado.instituicao,
                Principal = true,
                IdCertificado = certificado.id
            };
        }
    }
}