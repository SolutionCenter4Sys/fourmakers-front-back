using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.Competencia.Metodologia;
using Dapper;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Nivel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Competencia
{
    public class MetodologiasRepository : IMetodologiasRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;

        public MetodologiasRepository(ColaboradorContext colaboradorContext, IDBConnection dapperConnection)
        {
            this._colaboradorContext = colaboradorContext;
            _dapperConnection = dapperConnection;
        }

        public long ObterIdUsuarioPorCpf(string cpf)
        {
            return _colaboradorContext.tb_usuario.Where(x => x.codigo_interno_colaborador == cpf).FirstOrDefault().id;
        }

        public List<long> ListarMetodologiasAtribuidas(string cpf)
        {
            try
            {
                var listaCompetenciasExistentes = _colaboradorContext.tb_colaborador_metodologia
                                                        .Join(_colaboradorContext.tb_metodologia,
                                                              cm => cm.metodologia_id,
                                                              m => m.id,
                                                              (cm, m) => new { cm, m })
                                                        .Where(x => x.cm.codigo_interno_colaborador == cpf
                                                                    && x.cm.ativo == 1
                                                                    && x.m.ativo == 1)
                                                        .Select(x => x.cm.metodologia_id)
                                                        .ToList();
                return listaCompetenciasExistentes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<MetodologiaDTO> ListarMetodologias(string busca, int cursor, int limite)
        {
            return _colaboradorContext.tb_metodologia
                        .Where(m => m.ativo == 1
                                    && (string.IsNullOrEmpty(busca) || EF.Functions.Like(m.descricao, "%" + busca + "%")))
                        .OrderBy(m => m.descricao)
                        .Skip(cursor)
                        .Take(limite)
                        .Select(m => new MetodologiaDTO
                        {
                            Id = m.id,
                            Descricao = m.descricao,
                            Pendente = m.confirmada != 0,
                        })
                        .ToList();
        }

        public MetodologiaDTO ListarMetodologiasById(int codMetodologia)
        {
            return _colaboradorContext.tb_metodologia.Where(m => m.ativo == 1 && m.id == codMetodologia).Select(metodologia => new MetodologiaDTO
            {
                Id = metodologia.id,
                Descricao = metodologia.descricao,
                Pendente = metodologia.confirmada != 0,
            }).SingleOrDefault();
        }

        public List<NivelDTO> ListarNivelMetodologia()
        {
            return _colaboradorContext.tb_nivel
                            .Join(_colaboradorContext.tb_item_perfil, tn => tn.tb_item_perfil_id, tip => tip.id, (tn, tip) => new { tn, tip })
                            .Where(x => x.tip.descricao == "METODOLOGIA" && x.tip.ativo == 1)
                            .Select(x => new NivelDTO
                            {
                                Id = x.tn.id,
                                Descricao = x.tn.descricao,
                                PrioridadeUnificacao = x.tn.prioridade_unificacao,
                                OrdemExibicao = x.tn.ordem_exibicao
                            })
                            .OrderBy(x => x.OrdemExibicao)
                            .ToList();
        }

        public List<ListaMetodologiaColaboradorResult> ListarMetodologiasColaborador(string cpf)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();
                var query = @"
                    SELECT
                        cm.id AS Id,
                        cm.codigo_interno_colaborador as ColaboradorCpf,
                        m.id AS Id,
                        m.descricao AS Descricao,
                        m.confirmada AS Pendente,
                        cm.tb_nivel_id AS NivelId,
                        n.id AS Id,
                        n.descricao AS Descricao
                    FROM
                        tb_colaborador_metodologia cm
                    INNER JOIN
                        tb_metodologia m ON cm.metodologia_id = m.id
                    LEFT JOIN
                        tb_nivel n ON cm.tb_nivel_id = n.id
                    WHERE
                        cm.codigo_interno_colaborador = @ColaboradorCpf
                        AND cm.ativo = 1
                        AND m.ativo = 1
                    ORDER BY
                        m.descricao";

                var parametros = new { ColaboradorCpf = cpf };

                var resultado = connection.Query<ListaMetodologiaColaboradorResult, MetodologiaDTO, NivelDTO, ListaMetodologiaColaboradorResult>(
                    query,
                    (dto, metodologia, nivel) =>
                    {
                        dto.Metodologia = metodologia;
                        dto.Nivel = nivel ?? new NivelDTO { Id = 32, Descricao = "A definir" };
                        return dto;
                    },
                    parametros,
                    splitOn: "Id"  //separar objetos pelo id para o dapper conseguir transformar em dtos (tb_colaborador_metodologia/ tb_metodologia/ tb_nivel)
                ).ToList();

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long AdicionarMetodologia(MetodologiaDTO metodologia, string cpf)
        {
            try
            {
                var metodologiaEntity = new tb_metodologia
                {
                    descricao = metodologia.Descricao,
                    ativo = metodologia.Ativo ? (sbyte)1 : (sbyte)0,
                    usuario_criacao_id = metodologia.UsuarioCriacaoId,
                    confirmada = metodologia.Pendente ? (sbyte)1 : (sbyte)0
                };
                _colaboradorContext.tb_metodologia.Add(metodologiaEntity);

                _colaboradorContext.SaveChanges();
                return metodologiaEntity.id;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Ocorreu um erro ao adicionar metodologia.", e);
            }
        }

        public long InserirMetodologiaColaborador(MetodologiaDTO metodologia, string cpf)
        {
            using (var transaction = _colaboradorContext.Database.BeginTransaction())
            {
                try
                {
                    _colaboradorContext.tb_metodologia.Add(new tb_metodologia
                    {
                        descricao = metodologia.Descricao,
                        ativo = metodologia.Ativo ? (sbyte)1 : (sbyte)0,
                        usuario_criacao_id = metodologia.UsuarioCriacaoId,
                        confirmada = metodologia.Pendente ? (sbyte)1 : (sbyte)0
                    });

                    _colaboradorContext.SaveChanges();

                    var novaMetodologia = _colaboradorContext.tb_metodologia
                                                    .Where(m => m.descricao.ToUpper().Equals(metodologia.Descricao.ToUpper())).SingleOrDefault();

                    if (novaMetodologia != null)
                    {
                        AssociaMetodologiaComColaborador(new MetodologiaDTO
                        {
                            Id = novaMetodologia.id,
                            Ativo = novaMetodologia.ativo != 0,
                            NivelId = metodologia.NivelId
                        }, cpf);
                    }

                    _colaboradorContext.SaveChanges();
                    transaction.Commit();
                    return novaMetodologia.id;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException("Ocorreu um erro ao inserir metodologia.", e);
                }
            }
        }

        public void AssociaMetodologiaComColaborador(MetodologiaDTO metodologia, string cpf)
        {
            try
            {
                var metodologiaAssociada = _colaboradorContext.tb_colaborador_metodologia
                             .SingleOrDefault(m => m.metodologia_id == metodologia.Id && m.codigo_interno_colaborador == cpf);

                if (metodologiaAssociada != null)
                {
                    metodologiaAssociada.ativo = 1;
                    _colaboradorContext.SaveChanges();
                }
                else
                {
                    _colaboradorContext.tb_colaborador_metodologia.Add(new tb_colaborador_metodologia
                    {
                        codigo_interno_colaborador = cpf,
                        metodologia_id = metodologia.Id,
                        ativo = metodologia.Ativo ? (sbyte)1 : (sbyte)0,
                        tb_nivel_id = metodologia.NivelId
                    });
                    _colaboradorContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Ocorreu um erro ao associar metodologia.", e);
            }
        }

        public bool MetodologiaExiste(string descricao)
        {
            return _colaboradorContext.tb_metodologia.Any(m => m.descricao.ToUpper().Equals(descricao.ToUpper()) && m.ativo == 1);
        }

        public bool IsAssociadaComColaborador(long idMetodologia, string cpf)
        {
            return _colaboradorContext.tb_colaborador_metodologia.Any(m => m.metodologia_id == idMetodologia && m.codigo_interno_colaborador.Equals(cpf) && m.ativo == 1);
        }

        public void AtualizarNivelMetodologiaColaborador(MetodologiaDTO metodologia, string cpf)
        {
            var registroParaAtualizar = _colaboradorContext.tb_colaborador_metodologia
                                     .SingleOrDefault(cm => cm.metodologia_id == metodologia.Id
                                     && cm.codigo_interno_colaborador == cpf);

            if (registroParaAtualizar != null)
            {
                registroParaAtualizar.tb_nivel_id = metodologia.NivelId;

                _colaboradorContext.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Não há nivel para atualizar.");
            }
        }

        public void RemoverMetodologiaColaborador(int codMetodologia, string cpf)
        {
            try
            {
                var registroParaAtualizar = _colaboradorContext.tb_colaborador_metodologia
                                                    .SingleOrDefault(cm => cm.metodologia_id == codMetodologia
                                                    && cm.codigo_interno_colaborador == cpf);

                if (registroParaAtualizar != null)
                {
                    registroParaAtualizar.ativo = 0;

                    _colaboradorContext.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException("Não há registro de metodologia para remover.");
                }
                _colaboradorContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Ocorreu um erro ao remover metodologia.", e);
            }
        }
    }
}