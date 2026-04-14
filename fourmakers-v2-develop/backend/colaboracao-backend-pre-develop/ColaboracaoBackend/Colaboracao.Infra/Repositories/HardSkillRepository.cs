using ApiClient.Domain;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.Domain;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Softskill;
using Microsoft.EntityFrameworkCore;
using SRS.Infra.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class HardSkillRepository : IHardSkillRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        private const int ITEM_PERFIL_HARDSKILL = 1;
        private const int ITEM_PERFIL_SOFTSKILL = 8;
        private const int ITEM_PERFIL_IDIOMA = 9;
        private const int TODOS_NIVEIS = 0;
        private const double MEIO_PESO = 2.0;
        private const string NIVEL_HARDSKILL = "NivelHardskill";
        private const string NIVEL_SOFTSKILL = "NivelSoftskill";
        private const string NIVEL_IDIOMA = "NivelIdioma";
        private const string SOFTSKILL = "SOFTSKILL";
        private const string HARDSKILL = "HARDSKILL";
        private const string IDIOMA = "IDIOMA";
        private const int CANDIDATO_ORG_ID = 1;

        public HardSkillRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public List<CompetenciaDTO> ListarHardSkillPorDescricao(string descricao)
        {
            return _colaboradorContext.tb_competencia
                                        .Where(x => x.descricao.ToUpper() == descricao.ToUpper())
                                        .Select(x => new CompetenciaDTO()
                                        {
                                            Id = x.id,
                                            Descricao = x.descricao,
                                            UsuarioCriacaoId = x.usuario_criacao_id,
                                            Pendente = !Convert.ToBoolean(x.confirmada),
                                            Ativo = Convert.ToBoolean(x.ativo)
                                        })
                                        .ToList();
        }

        public CompetenciaDTO InserirHardSkill(string descricao, long? usuarioCriacaoId)
        {
            long hardSkillId = 0;

            var hardSkillExiste = _colaboradorContext.tb_competencia.Where(x => x.descricao == descricao).FirstOrDefault();

            if (hardSkillExiste == null || hardSkillExiste.id == default)
            {
                var novaHardSkill = new tb_competencia()
                {
                    descricao = descricao,
                    ativo = 1,
                    confirmada = 0,
                    usuario_criacao_id = UsuarioSistemicoConstants.SYSTEMIC_USER_ID
                };
                _colaboradorContext.Add(novaHardSkill);
                _colaboradorContext.SaveChanges();

                var hardSkillCriada = _colaboradorContext.tb_competencia.Where(x => x.descricao == descricao).FirstOrDefault();
                hardSkillId = hardSkillCriada.id;
            }
            else
                hardSkillId = hardSkillExiste.id;

            return ObterHardSkillPorId(hardSkillId);
        }
        public CompetenciaDTO AtualizarHardSkill(CompetenciaDTO competencia)
        {
            long hardSkillId = 0;

            var hardSkill = _colaboradorContext.tb_competencia.Where(x => x.descricao == competencia.Descricao || x.id == competencia.Id).SingleOrDefault();

            hardSkill.ativo = Convert.ToSByte(competencia.Ativo);
            hardSkill.confirmada = Convert.ToSByte(!competencia.Pendente);
            hardSkill.descricao = competencia.Descricao;

            return ObterHardSkillPorId(hardSkillId);
        }

        public List<long> ListarCodigoCompetenciasColaborador(string cpf)
        {
            return _colaboradorContext.tb_colaborador_competencia.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1).Select(x => x.competencia_id).ToList();
        }

        public CompetenciaDTO InserirHardSkillColaborador(long competenciaId, long? nivelId, string cpf)
        {
            var skill = ObterHardSkillPorId(competenciaId);
            var associacaoColaboradorHardSkill = new tb_colaborador_competencia()
            {
                codigo_interno_colaborador = cpf,
                competencia_id = competenciaId,
                ativo = 1,
                tb_nivel_id = nivelId
            };
            _colaboradorContext.Add(associacaoColaboradorHardSkill);
            _colaboradorContext.SaveChanges();
            return skill;
        }

        public CompetenciaDTO ObterHardSkillPorId(long competenciaId)
        {
            var hardSkill = _colaboradorContext.tb_competencia.Where(x => x.id == competenciaId && x.ativo == 1).Select(x => new CompetenciaDTO()
            {
                Id = x.id,
                Descricao = x.descricao,
                UsuarioCriacaoId = x.usuario_criacao_id,
                Pendente = !Convert.ToBoolean(x.confirmada),
                Ativo = true
            }).SingleOrDefault();
            return hardSkill;
        }

        public long? ObterIdUsuarioPorCpf(string cpf)
        {
            return _colaboradorContext.tb_usuario.Where(x => x.codigo_interno_colaborador == cpf).FirstOrDefault()?.id;
        }

        public List<CompetenciaDTO> ListarHardSkill(string busca, int cursor, int limite)
        {
            if (!String.IsNullOrEmpty(busca))
            {
                return _colaboradorContext.tb_competencia
                                    .Where(x => x.ativo == 1
                                                && EF.Functions.Like(x.descricao, "%" + busca + "%"))
                                    .OrderBy(x => x.descricao)
                                    .Skip(cursor)
                                    .Take(limite)
                                    .Select(x => new CompetenciaDTO()
                                    {
                                        Id = x.id,
                                        Descricao = x.descricao,
                                        UsuarioCriacaoId = x.usuario_criacao_id,
                                        Pendente = !Convert.ToBoolean(x.confirmada)
                                    })
                                    .ToList();
            }
            else
            {
                return _colaboradorContext.tb_competencia
                                    .Where(x => x.ativo == 1)
                                    .OrderBy(x => x.descricao)
                                    .Skip(cursor)
                                    .Take(limite)
                                    .Select(x => new CompetenciaDTO()
                                    {
                                        Id = x.id,
                                        Descricao = x.descricao,
                                        UsuarioCriacaoId = x.usuario_criacao_id,
                                        Pendente = !Convert.ToBoolean(x.confirmada)
                                    })
                                    .ToList();
            }
        }

        public CompetenciaColaboradorDTO ObterCompetenciaColaborador(long competenciaId, string cpf)
        {
            var competenciaColaboradorDTO = _colaboradorContext.tb_colaborador_competencia
                    .Where(x => x.competencia_id == competenciaId && x.codigo_interno_colaborador == cpf && x.ativo == 1)
                    .Select(ret => new CompetenciaColaboradorDTO()
                    {
                        Id = ret.id,
                        IdCompetencia = ret.competencia_id,
                        IdNivel = ret.tb_nivel_id,
                        Data = ret.data_alteracao
                    }).FirstOrDefault();

            return competenciaColaboradorDTO;
        }

        public CompetenciaColaboradorDTO ObterCompetenciaColaborador(long competenciaColaboradorId)
        {
            var competenciaColaboradorDTO = _colaboradorContext.tb_colaborador_competencia
                    .Where(x => x.id == competenciaColaboradorId && x.ativo == 1)
                    .Select(ret => new CompetenciaColaboradorDTO()
                    {
                        Id = ret.id,
                        IdCompetencia = ret.competencia_id,
                        IdNivel = ret.tb_nivel_id,
                        Data = ret.data_alteracao
                    })
                    .FirstOrDefault();

            return competenciaColaboradorDTO;
        }

        public List<NivelDTO> ListarNivelHardSkill()
        {
            return _colaboradorContext.tb_nivel
            .Join(_colaboradorContext.tb_item_perfil, tn => tn.tb_item_perfil_id, tip => tip.id, (tn, tip) => new { tn, tip })
            .Where(x => x.tip.descricao == "COMPETENCIA")
            .Select(x => new NivelDTO()
            {
                Id = x.tn.id,
                Descricao = x.tn.descricao,
                PrioridadeUnificacao = x.tn.prioridade_unificacao,
                OrdemExibicao = x.tn.ordem_exibicao
            })
            .OrderBy(x => x.OrdemExibicao)
            .ToList();
        }

        public List<CertificadoDTO> ListarCertificadosHardSkillColaborador(long competenciaColaboradorId)
        {
            return _colaboradorContext
            .tb_colaborador_competencia_certificado
            .Where(x => x.tb_colaborador_competencia_id == competenciaColaboradorId && x.ativo == 1 && x.tb_certificado.ativo == 1)
            .Join(_colaboradorContext.tb_certificado, tccc => tccc.tb_certificado_id, tc => tc.id, (tccc, tc) => new { tccc, tc })
            .Select(x => new CertificadoDTO()
            {
                Path = x.tc.path,
                Thumb = null,
                IdCertificado = x.tc.id,
                Principal = Convert.ToBoolean(x.tccc.principal),
                ativo = Convert.ToBoolean(x.tccc.ativo),
                conclusao = x.tc.data_conclusao,
                descricao = x.tc.descricao,
                instituicao = x.tc.instituicao,
                cargaHoraria = x.tc.carga_horaria
            }).ToList();
        }

        public bool RemoverHardSkillColaborador(long competenciaColaboradorId)
        {
            var row = _colaboradorContext.tb_colaborador_competencia.Where(x => x.id == competenciaColaboradorId).FirstOrDefault();
            _colaboradorContext.Remove(row);
            _colaboradorContext.SaveChanges();
            return true;
        }

        public long InserirCertificadoHardSkillColaborador(CertificadoDTO certificado, string codigoInternoColaborador, long? competenciaColaboradorId)
        {
            using (var transaction = _colaboradorContext.Database.BeginTransaction())
            {
                try
                {
                    var certificadoRow = new tb_certificado
                    {
                        path = certificado.Path,
                        ativo = (sbyte)(certificado.ativo ? 1 : 0),
                        data_conclusao = certificado.conclusao,
                        instituicao = certificado.instituicao,
                        descricao = certificado.descricao,
                        carga_horaria = certificado.cargaHoraria,
                        codigo_interno_colaborador = codigoInternoColaborador
                    };
                    _colaboradorContext.Add(certificadoRow);
                    _colaboradorContext.SaveChanges();
                    long certificadoId = certificadoRow.id;

                    if (competenciaColaboradorId != null)
                    {
                        var certificadoCompetenciaColaboradorRow = new tb_colaborador_competencia_certificado
                        {
                            tb_certificado_id = certificadoId,
                            tb_colaborador_competencia_id = competenciaColaboradorId,
                            ativo = 1
                        };
                        if (!_colaboradorContext.tb_colaborador_competencia_certificado.Where(x => x.tb_colaborador_competencia_id == competenciaColaboradorId && x.ativo == 1).Any())
                        {
                            certificadoCompetenciaColaboradorRow.principal = 1;
                        }
                        _colaboradorContext.Add(certificadoCompetenciaColaboradorRow);
                        _colaboradorContext.SaveChanges();
                    }

                    transaction.Commit();
                    return certificadoId;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool RemoverCertificadoHardSkillColaboradorRollback(long certificadoId, long? competenciaColaboradorId)
        {
            using (var transaction = _colaboradorContext.Database.BeginTransaction())
            {
                try
                {
                    if (competenciaColaboradorId != null)
                    {
                        var certificadoCompetenciaColaboradorRow = _colaboradorContext.tb_colaborador_competencia_certificado.Where(x => x.tb_certificado_id == certificadoId && x.tb_colaborador_competencia_id == competenciaColaboradorId).FirstOrDefault();
                        _colaboradorContext.Remove(certificadoCompetenciaColaboradorRow);
                    }
                    var certificadoRow = _colaboradorContext.tb_certificado.Where(x => x.id == certificadoId).FirstOrDefault();
                    _colaboradorContext.Remove(certificadoRow);
                    _colaboradorContext.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool InativarCertificadoHardSkillColaborador(long certificadoCompetenciaId, string cpf)
        {
            using (var transaction = _colaboradorContext.Database.BeginTransaction())
            {
                try
                {
                    var certificadoCompetenciaColaboradorRow = _colaboradorContext.tb_colaborador_competencia_certificado.Where(x => x.id == certificadoCompetenciaId).FirstOrDefault();
                    var certificadoRow = _colaboradorContext.tb_certificado.Where(x => x.id == certificadoCompetenciaColaboradorRow.tb_certificado_id).FirstOrDefault();
                    certificadoCompetenciaColaboradorRow.ativo = 0;
                    certificadoCompetenciaColaboradorRow.principal = 0;
                    certificadoRow.ativo = 0;
                    if (certificadoCompetenciaColaboradorRow == null || certificadoRow == null)
                    {
                        throw new Exception("Certificado não encontrado.");
                    }
                    if (certificadoCompetenciaColaboradorRow.ativo == 0)
                    {
                        throw new Exception("Certificado já foi removido.");
                    }
                    var novoCertificadoPrincipal = BuscarUltimoCertificadoParaACompetenciaDesteCertificadoQueEstaSendoExcluido(certificadoCompetenciaId);
                    if (novoCertificadoPrincipal != null)
                    {
                        novoCertificadoPrincipal.Principal = 1;
                        var novoCertificadoPrincipalRow = new tb_colaborador_competencia_certificado();
                        novoCertificadoPrincipalRow.ativo = novoCertificadoPrincipal.Ativo;
                        novoCertificadoPrincipalRow.tb_colaborador_competencia.codigo_interno_colaborador = novoCertificadoPrincipal.CpfTbColaboradroCompetencia;
                        novoCertificadoPrincipalRow.principal = novoCertificadoPrincipal.Principal;
                        novoCertificadoPrincipalRow.id = novoCertificadoPrincipal.Id;
                        _colaboradorContext.Update(novoCertificadoPrincipalRow);
                    }
                    _colaboradorContext.Update(certificadoCompetenciaColaboradorRow);
                    _colaboradorContext.Update(certificadoRow);
                    _colaboradorContext.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public ColaboradorCompetenciaCertificadoDTO BuscarUltimoCertificadoParaACompetenciaDesteCertificadoQueEstaSendoExcluido(long idCertificadoCompetencia)
        {
            var certificadoCompetenciaRow = _colaboradorContext.tb_colaborador_competencia_certificado.Find(idCertificadoCompetencia);

            var penultimoCertificadoCompetenciaRow = _colaboradorContext.tb_colaborador_competencia_certificado
                .Where(x => x.id != certificadoCompetenciaRow.id
                        && x.tb_colaborador_competencia_id == certificadoCompetenciaRow.tb_colaborador_competencia_id
                        && x.ativo == 1).FirstOrDefault();

            var model = new ColaboradorCompetenciaCertificadoDTO();

            if (penultimoCertificadoCompetenciaRow != null)
            {
                model.Ativo = penultimoCertificadoCompetenciaRow.ativo;
                model.CpfTbColaboradroCompetencia = penultimoCertificadoCompetenciaRow.tb_colaborador_competencia.codigo_interno_colaborador;
                model.Principal = penultimoCertificadoCompetenciaRow.principal;
                model.Id = penultimoCertificadoCompetenciaRow.id;

                return model;
            }
            else
            {
                return model;
            }
        }

        public ColaboradorCompetenciaCertificadoDTO ObterColaboradorCompetenciaCertificado(long idCertificadoCompetencia)
        {
            var certificadoColabRow = _colaboradorContext.tb_colaborador_competencia_certificado.Find(idCertificadoCompetencia);

            var colaboradorCompetenciaCertificadoDTO = new ColaboradorCompetenciaCertificadoDTO();

            colaboradorCompetenciaCertificadoDTO.Ativo = certificadoColabRow.ativo;
            colaboradorCompetenciaCertificadoDTO.CpfTbColaboradroCompetencia = certificadoColabRow.tb_colaborador_competencia.codigo_interno_colaborador;
            colaboradorCompetenciaCertificadoDTO.Principal = certificadoColabRow.principal;
            colaboradorCompetenciaCertificadoDTO.Id = certificadoColabRow.id;

            return colaboradorCompetenciaCertificadoDTO;
        }

        public ColaboradorCompetenciaCertificadoDTO ObterColaboradorCompetenciaCertificadoPrincipal(long idCertificadoCompetencia)
        {
            var certificadoColabRow = _colaboradorContext.tb_colaborador_competencia_certificado.Find(idCertificadoCompetencia);

            var antigoPrincipal = _colaboradorContext
                    .tb_colaborador_competencia_certificado.Where(
                        x => x.id != certificadoColabRow.id
                        && x.tb_colaborador_competencia_id == certificadoColabRow.tb_colaborador_competencia_id
                        && x.ativo == 1
                        && x.principal == 1)
                    .FirstOrDefault();

            if (antigoPrincipal != null)
            {
                var colaboradorCompetenciaCertificadoDTO = new ColaboradorCompetenciaCertificadoDTO();

                colaboradorCompetenciaCertificadoDTO.Ativo = antigoPrincipal.ativo;
                colaboradorCompetenciaCertificadoDTO.CpfTbColaboradroCompetencia = antigoPrincipal.tb_colaborador_competencia.codigo_interno_colaborador;
                colaboradorCompetenciaCertificadoDTO.Principal = antigoPrincipal.principal;
                colaboradorCompetenciaCertificadoDTO.Id = antigoPrincipal.id;

                return colaboradorCompetenciaCertificadoDTO;
            }
            else
            {
                throw new Exception("Colaborador não possui ainda não possui um certificado principal para alterá-lo.");
            }
        }

        public ColaboradorCompetenciaCertificadoDTO AtualizarColaboradorCompetenciaCertificado(ColaboradorCompetenciaCertificadoDTO colaboradorCompetenciaCertificadoDTO)
        {
            var colaboradorCompetenciaCertificado = _colaboradorContext.tb_colaborador_competencia_certificado
                .Where(x => x.id == colaboradorCompetenciaCertificadoDTO.Id)
                .FirstOrDefault();

            colaboradorCompetenciaCertificado.ativo = colaboradorCompetenciaCertificadoDTO.Ativo;
            colaboradorCompetenciaCertificado.principal = colaboradorCompetenciaCertificadoDTO.Principal;

            _colaboradorContext.tb_colaborador_competencia_certificado.Update(colaboradorCompetenciaCertificado);
            _colaboradorContext.SaveChanges();

            return colaboradorCompetenciaCertificadoDTO;
        }

        public CertificadoDTO ObterCertificadoPorId(long certificadoId)
        {
            var certificadoRow = _colaboradorContext.tb_certificado.Where(x => x.id == certificadoId).FirstOrDefault();
            if (certificadoRow == null)
                return null;
            var certificado = new CertificadoDTO
            {
                IdCertificado = certificadoRow.id,
                descricao = certificadoRow.descricao,
                conclusao = certificadoRow.data_conclusao,
                instituicao = certificadoRow.instituicao,
                ativo = Convert.ToBoolean(certificadoRow.ativo),
                Path = certificadoRow.path,
                CodigoInternoColaborador = certificadoRow.codigo_interno_colaborador
            };

            return certificado;
        }

        public CertificadoDTO AtualizarCertificado(CertificadoDTO certificadoDTO)
        {
            var CertificadoRow = _colaboradorContext.tb_certificado
                .Where(x => x.id == certificadoDTO.IdCertificado)
                .FirstOrDefault();

            CertificadoRow.path = certificadoDTO.Path;
            CertificadoRow.descricao = certificadoDTO.descricao;
            CertificadoRow.instituicao = certificadoDTO.instituicao;
            CertificadoRow.data_conclusao = certificadoDTO.conclusao;
            CertificadoRow.carga_horaria = certificadoDTO.cargaHoraria;
            CertificadoRow.ativo = Convert.ToSByte(certificadoDTO.ativo);
            _colaboradorContext.tb_certificado.Update(CertificadoRow);
            _colaboradorContext.SaveChanges();

            return certificadoDTO;
        }

        public CompetenciaColaboradorDTO AtualizarCompetenciaColaborador(CompetenciaColaboradorDTO competenciaColaboradorDTO)
        {
            try
            {
                var competenciaColaboradorRow = _colaboradorContext.tb_colaborador_competencia.Where(x => x.id == competenciaColaboradorDTO.Id).FirstOrDefault();
                competenciaColaboradorRow.id = competenciaColaboradorDTO.Id;
                competenciaColaboradorRow.competencia_id = competenciaColaboradorDTO.Competencia.Id;
                competenciaColaboradorRow.tb_nivel_id = competenciaColaboradorDTO.Nivel.Id;
                competenciaColaboradorRow.codigo_interno_colaborador = competenciaColaboradorDTO.ColaboradorCpf;
                competenciaColaboradorRow.ativo = 1;
                _colaboradorContext.Update(competenciaColaboradorRow);
                _colaboradorContext.SaveChanges();
                return competenciaColaboradorDTO;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<long> ListarIdsPorHardSkillId(long id)
        {
            var rows = _colaboradorContext.tb_colaborador_competencia.Where(x => x.competencia_id == id && x.ativo == 1).Select(x => x.id).ToList();
            return rows;
        }

        public ColaboradorCompetenciaCertificadoDTO ObterColaboradorCompetenciaCertificadoPorCertificadoId(long idCertificado)
        {
            var certificadoColabRow = _colaboradorContext.tb_colaborador_competencia_certificado.Where(x => x.tb_certificado_id == idCertificado).FirstOrDefault();

            var model = new ColaboradorCompetenciaCertificadoDTO();

            model.Ativo = certificadoColabRow.ativo;
            model.CpfTbColaboradroCompetencia = certificadoColabRow.tb_colaborador_competencia.codigo_interno_colaborador;
            model.Principal = certificadoColabRow.principal;
            model.Id = certificadoColabRow.id;

            return model;
        }

        private void AtualizarSkillsComNivelTodos(List<FiltroCompetenciaDTO> param)
        {
            var mapeamentoCompetencia = new Dictionary<string, int>
                {{ HARDSKILL, ITEM_PERFIL_HARDSKILL },
                { SOFTSKILL, ITEM_PERFIL_SOFTSKILL },
                { IDIOMA, ITEM_PERFIL_IDIOMA}};

            var competencias = param
                .Where(competencia => competencia.NivelId == TODOS_NIVEIS)
                .ToList();

            foreach (var item in competencias)
            {
                if (mapeamentoCompetencia.TryGetValue(item.Tipo.ToUpper(), out var nivel))
                {
                    var niveis = _colaboradorContext.tb_nivel.Where(c => c.tb_item_perfil_id == nivel).Select(c => c.id).ToList();
                    foreach (var n in niveis)
                    {
                        param.Add(new FiltroCompetenciaDTO()
                        {
                            Id = item.Id,
                            Tipo = item.Tipo,
                            NivelId = n
                        });
                    }

                    param.Remove(item);
                }
            }
        }

        private NivelDTO ObterNivel(int nivelId)
        {
            if (nivelId == 0)
            {
                return new NivelDTO();
            }

            var nivel = _colaboradorContext.tb_nivel
                .Where(n => n.id == nivelId)
                .Select(n => new NivelDTO
                {
                    Id = n.id,
                    Descricao = n.descricao
                })
                .FirstOrDefault();

            return nivel;
        }

        private List<CompetenciaNivelDTO> ObterCompetencias(IEnumerable<tb_colaborador_competencia> competencias)
        {
            if (competencias.Count() == 0)
            {
                return new List<CompetenciaNivelDTO>();
            }
            var comp = competencias
       .Where(c => c.ativo == 1 && (c.tb_nivel_id != null || c.tb_nivel_id == null)) // Modificação aqui
       .Select(h => new CompetenciaNivelDTO
       {
           Id = h.competencia_id,
           Descricao = h.competencia.descricao,
           Nivel = h.tb_nivel_id != null ? ObterNivel((int)h.tb_nivel_id) : null // Modificação aqui
       })
       .ToList();

            return comp;
        }

        private List<IdiomaNivelDTO> ObterIdiomas(IEnumerable<tb_colaborador_idioma> idiomas)
        {
            if (idiomas.Count() == 0)
            {
                return new List<IdiomaNivelDTO>();
            }

            return idiomas
                .Where(c => c.ativo == 1 && c.tb_nivel_id != null)
                .Select(h => new IdiomaNivelDTO
                {
                    Id = h.idioma_id,
                    Descricao = h.idioma.descricao,
                    Nivel = ObterNivel((int)h.tb_nivel_id)
                })
                .ToList();
        }

        private string ObterNomeGestor(tb_colaborador x)
        {
            var ret = _colaboradorContext.vw_gestores_colaboradores_org.Where(c => c.codigo_interno_colaborador_subordinado == x.codigo_interno_colaborador).Select(c => c.nome_completo_gestor).FirstOrDefault();
            if (ret == null)
            {
                ret = "";
            }
            return ret;
        }

        private static double CalcularAderencia(tb_colaborador colaborador, List<FiltroCompetenciaDTO> competencias)
        {
            int quantidadeTotalCompetencias = competencias.Count;

            double totalAderencia = competencias
        .Select(competencia =>
        {
            var hardskill = colaborador.tb_colaborador_competencia.FirstOrDefault(h => h.competencia_id == competencia.Id && h.ativo == 1);
            var sofskill = colaborador.tb_colaborador_softskill.FirstOrDefault(h => h.softskill_id == competencia.Id && h.ativo == 1);
            var idioma = colaborador.tb_colaborador_idioma.FirstOrDefault(h => h.idioma_id == competencia.Id && h.ativo == 1);

            if ((competencia.Tipo.ToUpper() == HARDSKILL && hardskill != null) ||
                (competencia.Tipo.ToUpper() == SOFTSKILL && sofskill != null) ||
                (competencia.Tipo.ToUpper() == IDIOMA && idioma != null))
            {
                double peso = 1.0 / quantidadeTotalCompetencias;

                if ((competencia.NivelId == TODOS_NIVEIS) ||
                    (hardskill != null && hardskill.tb_nivel_id == competencia.NivelId) ||
                    (sofskill != null && sofskill.tb_nivel_id == competencia.NivelId) ||
                    (idioma != null && idioma.tb_nivel_id == competencia.NivelId))
                {
                    // Peso sem alteração
                }
                else
                {
                    peso /= MEIO_PESO;
                }

                return peso;
            }

            return 0.0;
        })
        .Sum();
            double aderenciaPercentual = Math.Round(totalAderencia * 100.0, 2);
            return aderenciaPercentual;
        }

        private static void mockNivelSkillsTodos(ItensSumarioResult ret)
        {
            ret.niveis.Add(new SkillSumarioDTO()
            {
                Descricao = "Todos",
                Id = 0,
                Tipo = NIVEL_HARDSKILL
            });
            ret.niveis.Add(new SkillSumarioDTO()
            {
                Descricao = "Todos",
                Id = 0,
                Tipo = NIVEL_SOFTSKILL
            });
            ret.niveis.Add(new SkillSumarioDTO()
            {
                Descricao = "Todos",
                Id = 0,
                Tipo = NIVEL_IDIOMA
            });
        }

        private void AddRangeSkills<T>(ItensSumarioResult ret, string skillTipo, DbSet<T> tabela)
        where T : class
        {
            var ativoProperty = typeof(T).GetProperty("ativo");

            if (ativoProperty != null)
            {
                var registros = tabela.ToList();

                var registrosAtivos = registros.Where(x => Convert.ToBoolean(ativoProperty.GetValue(x))).ToList();

                ret.skills.AddRange(registrosAtivos.Select(x => new SkillSumarioDTO()
                {
                    Tipo = skillTipo,
                    Descricao = x.GetType().GetProperty("descricao")?.GetValue(x)?.ToString(),
                    Id = GetIdValue(x)
                }));
            }
        }

        private long GetIdValue<T>(T item)
        {
            object idValue = item.GetType().GetProperty("id")?.GetValue(item);

            if (idValue != null && long.TryParse(idValue.ToString(), out long parsedId))
            {
                return parsedId;
            }
            return 0;
        }

        private void AddNiveisRange(ItensSumarioResult ret, string nivelTipo, int itemPerfilId)
        {
            ret.niveis.AddRange(_colaboradorContext.tb_nivel.Where(x => x.tb_item_perfil_id == itemPerfilId).Select(x => new SkillSumarioDTO()
            {
                Tipo = nivelTipo,
                Descricao = x.descricao,
                Id = x.id
            }).ToList());
        }

        public ItensSumarioResult ListarSkillsSumario()
        {
            var ret = new ItensSumarioResult();
            ret.skills = new List<SkillSumarioDTO>();
            ret.niveis = new List<SkillSumarioDTO>();

            mockNivelSkillsTodos(ret);

            AddRangeSkills(ret, SOFTSKILL, _colaboradorContext.tb_softskill);
            AddRangeSkills(ret, HARDSKILL, _colaboradorContext.tb_competencia);
            AddRangeSkills(ret, IDIOMA, _colaboradorContext.tb_idioma);

            AddNiveisRange(ret, NIVEL_HARDSKILL, ITEM_PERFIL_HARDSKILL);
            AddNiveisRange(ret, NIVEL_SOFTSKILL, ITEM_PERFIL_SOFTSKILL);
            AddNiveisRange(ret, NIVEL_IDIOMA, ITEM_PERFIL_IDIOMA);

            return ret;
        }

        private List<SoftskillNivelDTO> ObterSoftskills(IEnumerable<tb_colaborador_softskill> softskills)
        {
            if (softskills.Count() == 0)
            {
                return new List<SoftskillNivelDTO>();
            }

            return softskills
                .Where(c => c.ativo == 1 && c.tb_nivel_id != null)
                .Select(h => new SoftskillNivelDTO
                {
                    Id = h.softskill_id,
                    Descricao = h.softskill.descricao,
                    Nivel = ObterNivel((int)h.tb_nivel_id)
                })
                .ToList();
        }

        public CompetenciasSumarioResult BuscarColaboradorSumario(FiltroSumarioCompetenciasParam param, string token, int orgId)
        {
            var paramCompetencias = new List<FiltroCompetenciaDTO>(param.competencia);
            string serviceMidia = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
            AtualizarSkillsComNivelTodos(param.competencia);

            var unidadesIdsList = param.UnidadeId.Select(id => id.ToString()).ToList();

            var colaboradoresUnidade = _colaboradorContext.tb_colaborador
                .Where(colaborador =>
                    _colaboradorContext.tb_colaborador_org
                        .Any(colaboradorOrg =>
                            (colaboradorOrg.codigo_interno_colaborador == colaborador.codigo_interno_colaborador &&
                            unidadesIdsList.Contains(colaboradorOrg.cod_diretoria) &&
                            colaboradorOrg.tb_org_id == orgId) ||
                            (param.UnidadeId.Contains(0) && colaboradorOrg.tb_org_id == orgId)
                        )
                );

            if (param.competencia != null && param.competencia.Any())
            {
                var competenciaIds = param.competencia
                                        .Where(comp => comp.Tipo == "HARDSKILL")
                                        .Select(comp => comp.Id)
                                        .ToList();

                var idiomaIds = param.competencia
                                    .Where(comp => comp.Tipo == "IDIOMA")
                                    .Select(comp => comp.Id)
                                    .ToList();

                var softskillIds = param.competencia
                                        .Where(comp => comp.Tipo == "SOFTSKILL")
                                        .Select(comp => comp.Id)
                                        .ToList();

                colaboradoresUnidade = colaboradoresUnidade
                    .Where(u =>
                        u.tb_colaborador_competencia
                            .Any(c => competenciaIds.Contains(c.competencia_id) && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        u.tb_colaborador_idioma
                            .Any(c => idiomaIds.Contains(c.idioma_id) && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        u.tb_colaborador_softskill
                            .Any(c => softskillIds.Contains(c.softskill_id) && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId)));
            }

            var resultColabsUnidade = colaboradoresUnidade.ToList();
            var colaboradoresCpf = _colaboradorContext.tb_colaborador
                .Where(u =>
                    param.Cpf == null || param.Cpf.Contains(u.codigo_interno_colaborador)
                )
                .ToList();

            if (param.Cpf.Count >= 1 && param.competencia.Count < 1)
            {
                resultColabsUnidade.RemoveAll(x => x.codigo_interno_colaborador != null);
            }

            var resultado = colaboradoresCpf
            .Union(resultColabsUnidade)
            .Skip(param.Cursor)
            .Take(param.Limite);

            var ret = resultado
                    .Select(x =>
                    {
                        var competencias = ObterCompetencias(x.tb_colaborador_competencia);
                        var idiomas = ObterIdiomas(x.tb_colaborador_idioma);
                        var softskills = ObterSoftskills(x.tb_colaborador_softskill);
                        var imagePath = "";
                        var nomeGestor = ObterNomeGestor(x);

                        if (x.imagem != null && x.imagem.path != null)
                        {
                            imagePath = serviceMidia.Replace("$1", token) + x.imagem.path;
                        }
                        return new ColaboradorSumarioDTO()
                        {
                            Id = x.tb_usuario.Where(a => a.tb_org_id == orgId).FirstOrDefault()?.id,
                            NomeCompleto = x.nome_completo,
                            Aderencia = CalcularAderencia(x, paramCompetencias),
                            ImagePath = imagePath,
                            Cpf = x.codigo_interno_colaborador,
                            Gestor = nomeGestor,
                            Unidade = x.tb_colaborador_org.Where(x => x.tb_org_id == orgId).FirstOrDefault()?.diretoria ?? "",
                            Hardskills = competencias,
                            Idiomas = idiomas,
                            Softskills = softskills
                        };
                    });

            var retorno = new CompetenciasSumarioResult
            {
                ListaColaboradoresSumario = ret.OrderByDescending(x => x.Aderencia).ToList()
            };

            return retorno;
        }
    }
}