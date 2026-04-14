using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.Domain.Dominio;
using Dapper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.Nivel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Competencia.Dominio
{
    public class DominioRepository : IDominioRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;

        public DominioRepository(ColaboradorContext colaboradorContext, IDBConnection dapperConnection)
        {
            _colaboradorContext = colaboradorContext;
            _dapperConnection = dapperConnection;
        }
        public List<long> ListarDominiosAtribuidos(string cpfColaborador)
        {
            try
            {
                var listaCompetenciasExistentes = _colaboradorContext.tb_colaborador_dominionegocio
                                                         .Join(_colaboradorContext.tb_dominionegocio,
                                                               cd => cd.dominionegocio_id,
                                                               d => d.id,
                                                               (cd, d) => new { cd, d })
                                                         .Where(x => x.cd.codigo_interno_colaborador == cpfColaborador
                                                                     && x.cd.ativo == 1
                                                                     && x.d.ativo == 1)
                                                         .Select(x => x.cd.dominionegocio_id)
                                                         .ToList();
                return listaCompetenciasExistentes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DominioDTO> ListarDominio(string busca, int cursor, int limite)
        {
            return _colaboradorContext.tb_dominionegocio
                                .Where(x => x.ativo == 1
                                            && (busca.IsEmpty() || EF.Functions.Like(x.descricao, "%" + busca + "%")))
                                .OrderBy(x => x.descricao)
                                .Skip(cursor)
                                .Take(limite)
                                .Select(x => new DominioDTO()
                                {
                                    Id = x.id,
                                    Descricao = x.descricao,
                                    UsuarioCriacaoId = x.usuario_criacao_id,
                                    Pendente = !Convert.ToBoolean(x.confirmada)
                                })
                                .ToList();
        }

        public DominioDTO ObterDominioPorId(long id)
        {
            return _colaboradorContext.tb_dominionegocio
                                    .Where(x => x.id == id && x.ativo == 1)
                                    .Select(x => new DominioDTO()
                                    {
                                        Id = x.id,
                                        Descricao = x.descricao,
                                        UsuarioCriacaoId = x.usuario_criacao_id,
                                        Pendente = !Convert.ToBoolean(x.confirmada)
                                    })
                                    .FirstOrDefault();
        }

        public DominioDTO InserirDominio(string descricao, long usuarioId)
        {
            try
            {
                var row = new tb_dominionegocio()
                {
                    descricao = descricao,
                    ativo = 1,
                    usuario_criacao_id = usuarioId,
                    confirmada = 0
                };
                _colaboradorContext.Add(row);
                _colaboradorContext.SaveChanges();

                var dominio = _colaboradorContext.tb_dominionegocio.Where(x => x.descricao == descricao).FirstOrDefault().id;
                return ObterDominioPorId(dominio);
            }
            catch
            {
                throw new Exception("Erro ao inserir domínio de negócio.");
            }
        }

        public long? GetUsuarioCriacaoId(string cpf)
        {
            return _colaboradorContext.tb_usuario.Where(x => x.codigo_interno_colaborador == cpf).FirstOrDefault()?.id;
        }

        public List<DominioDTO> ListarDominioPorDescricao(string descricao)
        {
            return _colaboradorContext.tb_dominionegocio
                                        .Where(x => x.descricao.ToUpper() == descricao.ToUpper())
                                        .Select(x => new DominioDTO()
                                        {
                                            Id = x.id,
                                            Descricao = x.descricao,
                                            UsuarioCriacaoId = x.usuario_criacao_id,
                                            Pendente = !Convert.ToBoolean(x.confirmada)
                                        })
                                        .ToList();
        }

        public List<DominioColaboradorDTO> ListarDominioColaborador(string cpf)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();
                var query = @"
                    SELECT
                        cd.id AS Id,
                        cd.data_criacao AS Data,
                        cd.codigo_interno_colaborador AS ColaboradorCpf,
                        cd.dominionegocio_id AS IdDominio,
                        cd.tb_nivel_id AS IdNivel,
                        d.id AS Id,
                        d.descricao AS Descricao,
                        d.confirmada AS Pendente,
                        n.id AS Id,
                        n.descricao AS Descricao
                    FROM
                        tb_colaborador_dominionegocio cd
                    INNER JOIN
                        tb_dominionegocio d ON cd.dominionegocio_id = d.id
                    LEFT JOIN
                        tb_nivel n ON cd.tb_nivel_id = n.id
                    WHERE
                        cd.codigo_interno_colaborador = @ColaboradorCpf
                        AND cd.ativo = 1
                        AND d.ativo = 1
                    ORDER BY
                        d.descricao";

                var parametros = new { ColaboradorCpf = cpf };

                var resultado = connection.Query<DominioColaboradorDTO, ItemPerfilDTO, NivelDTO, DominioColaboradorDTO>(
                    query,
                    (dto, dominio, nivel) =>
                    {
                        dto.Dominio = dominio;
                        dto.Nivel = nivel ?? new NivelDTO { Id = 32, Descricao = "A definir" };
                        return dto;
                    },
                    parametros,
                    splitOn: "Id" //separar objetos pelo id para o dapper conseguir transformar em dtos (tb_colaborador_dominionegocio/ tb_dominio_negocio/ tb_nivel)
                ).ToList();

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DominioColaboradorDTO ObterDominioColaborador(string cpf, long dominioId)
        {
            var dominioColaborador = _colaboradorContext.tb_colaborador_dominionegocio
                    .Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1 && x.dominionegocio_id == dominioId)
                    .Select(ret => new DominioColaboradorDTO()
                    {
                        Id = ret.id,
                        Dominio = _colaboradorContext.tb_dominionegocio.Where(x => x.id == ret.dominionegocio_id).Select(dominio => new ItemPerfilDTO()
                        {
                            Id = dominio.id,
                            Descricao = dominio.descricao,
                            Pendente = !Convert.ToBoolean(dominio.confirmada),
                        }).FirstOrDefault(),
                        IdNivel = ret.tb_nivel_id,
                        Data = ret.data_alteracao,
                        Nivel = _colaboradorContext.tb_nivel.Where(x => x.id == ret.tb_nivel_id).Select(nivel => new NivelDTO()
                        {
                            Id = nivel.id,
                            Descricao = nivel.descricao
                        }).FirstOrDefault()
                    }).FirstOrDefault();
            return dominioColaborador;
        }

        public DominioColaboradorDTO AssociarDominioColaborador(long dominioId, long? nivelId, string cpf)
        {
            var skill = ObterDominioPorId(dominioId);
            var associacaoColaboradorHardSkill = new tb_colaborador_dominionegocio()
            {
                codigo_interno_colaborador = cpf,
                dominionegocio_id = dominioId,
                ativo = 1,
                tb_nivel_id = nivelId
            };
            _colaboradorContext.Add(associacaoColaboradorHardSkill);
            _colaboradorContext.SaveChanges();
            return new DominioColaboradorDTO()
            {
                Dominio = new ItemPerfilDTO()
                {
                    Id = skill.Id,
                    Descricao = skill.Descricao,
                    Pendente = skill.Pendente
                },
            };
        }

        public bool RemoverDominioColaborador(long dominioId, string cpf)
        {
            var row = _colaboradorContext.tb_colaborador_dominionegocio.Where(x => x.dominionegocio_id == dominioId && x.codigo_interno_colaborador == cpf && x.ativo == 1).FirstOrDefault();
            if (row == null)
            {
                throw new ArgumentException($"O colaborador não possui a competência {dominioId}");
            }
            _colaboradorContext.Remove(row);
            _colaboradorContext.SaveChanges();
            return true;
        }

        public List<NivelDTO> ListaNivelDominio()
        {
            return _colaboradorContext.tb_nivel
            .Join(_colaboradorContext.tb_item_perfil, tn => tn.tb_item_perfil_id, tip => tip.id, (tn, tip) => new { tn, tip })
            .Where(x => x.tip.descricao == "DOMINIONEGOCIO")
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

        public DominioColaboradorDTO AlterarDominioColaborador(long dominioId, long? nivelId, string cpf)
        {
            var competenciaColaboradorRow = _colaboradorContext.tb_colaborador_dominionegocio.Where(x => x.dominionegocio_id == dominioId && x.codigo_interno_colaborador == cpf && x.ativo == 1).FirstOrDefault();
            competenciaColaboradorRow.dominionegocio_id = dominioId;
            competenciaColaboradorRow.tb_nivel_id = nivelId;
            competenciaColaboradorRow.codigo_interno_colaborador = cpf;
            competenciaColaboradorRow.ativo = 1;
            _colaboradorContext.Update(competenciaColaboradorRow);
            _colaboradorContext.SaveChanges();
            return ObterDominioColaborador(cpf, dominioId);
        }

        public StatusEndossoDTO GetStatusEndosso(long idColaboradorDominio)
        {
            var rows = _colaboradorContext.tb_endosso_dominionegocio
                .Where(x => x.colaborador_dominionegocio_id == idColaboradorDominio)
                .ToList();

            if (rows == null || rows.Count == 0)
                return new StatusEndossoDTO();

            var quantidadeSolicitacao = rows.Count;
            var quantidadeEndosso = rows.Count(x => x.ativo == 1);

            return new StatusEndossoDTO
            {
                QuantidadeSolicitacaoEndosso = quantidadeSolicitacao,
                QuantidadeEndosso = quantidadeEndosso,
                Endossado = quantidadeEndosso >= 3
            };
        }

        public List<EndossoRawDTO> GetEndossoConcedido(long idColaboradorDominio)
        {
            return _colaboradorContext.tb_endosso_dominionegocio
                .Where(x => x.colaborador_dominionegocio_id == idColaboradorDominio && x.ativo == 1)
                .Select(x => new EndossoRawDTO
                {
                    cpfColaborador = x.codigo_interno_colaborador,
                    dataEndosso = x.data_alteracao,
                    TipoEndossoId = x.tb_tipo_endosso_id ?? 0
                })
                .ToList();
        }

        public TipoEndossoDTO GetTipoEndosso(long tipoEndossoId)
        {
            return _colaboradorContext.tb_tipo_endosso
                .Where(x => x.id == tipoEndossoId)
                .Select(x => new TipoEndossoDTO
                {
                    Id = x.id,
                    Descricao = x.descricao,
                    NivelEndosso = x.nivel
                })
                .FirstOrDefault();
        }

        public NivelDTO GetNivelById(long nivelId)
        {
            return _colaboradorContext.tb_nivel
                .Where(x => x.id == nivelId)
                .Select(x => new NivelDTO
                {
                    Id = x.id,
                    Descricao = x.descricao
                })
                .FirstOrDefault();
        }
    }
}