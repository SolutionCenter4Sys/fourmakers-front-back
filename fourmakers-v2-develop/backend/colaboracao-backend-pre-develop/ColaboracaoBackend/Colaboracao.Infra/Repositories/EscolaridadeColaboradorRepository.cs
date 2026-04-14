using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain.Escolaridade;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class EscolaridadeColaboradorRepository : IEscolaridadeColaboradorDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        public EscolaridadeColaboradorRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public EscolaridadeDTO Save(EscolaridadeDTO dto)
        {
            try
            {
                var colaborador = _colaboradorContext.tb_colaborador.Where(x => x.codigo_interno_colaborador == dto.ColaboradorCpf).FirstOrDefault();
                if (colaborador != null)
                {
                    var row = new tb_escolaridade();
                    row.ativo = 1;

                    if (_colaboradorContext.tb_formacao.Where(x => x.id == dto.FormacaoId && x.ativo == 1).FirstOrDefault() != null)
                    {
                        row.tb_formacao_id = dto.FormacaoId;
                    }
                    else
                    {
                        throw new Exception("formação não encontrada!");
                    }
                    dto.Ativo = Convert.ToBoolean(row.ativo);
                    row.instituicao = dto.Instituicao;
                    row.data_inicio = Convert.ToDateTime(dto.DataInicio);
                    row.data_termino = Convert.ToDateTime(dto.DataTermino);
                    row.descricao = dto.Descricao;
                    row.codigo_interno_colaborador = dto.ColaboradorCpf;
                    row.tipo_diploma_id = dto.TipoDiplomaId;
                    row.path_diploma = dto.FilePathInternal;
                    row.ativo = 1;
                    dto.Ativo = Convert.ToBoolean(row.ativo);
                    _colaboradorContext.tb_escolaridade.Add(row);
                    _colaboradorContext.SaveChanges();
                    dto.Id = row.id;
                }

                return dto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EscolaridadeDTO> Listar(string cpf)
        {
            try
            {
                var ret = new List<EscolaridadeDTO>();

                var aux = _colaboradorContext.tb_escolaridade.Where(x => x.ativo == 1 && x.codigo_interno_colaborador == cpf).ToList();

                foreach (var row in aux)
                {
                    var dto = new EscolaridadeDTO
                    {
                        Id = row.id,
                        FormacaoId = row.tb_formacao_id,
                        Instituicao = row.instituicao,
                        DataInicio = row.data_inicio,
                        DataTermino = row.data_termino,
                        Descricao = row.descricao,
                        ColaboradorCpf = row.codigo_interno_colaborador,
                        TipoDiplomaId = row.tipo_diploma_id,
                        FilePathInternal = row.path_diploma,
                        Ativo = true
                    };
                    ret.Add(dto);
                }
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EscolaridadeDTO> Listar(string busca, int cursor, int limite, string cpf)
        {
            try
            {
                var ret = new List<EscolaridadeDTO>();

                var aux = _colaboradorContext.tb_escolaridade
                            .Join(
                                _colaboradorContext.tb_formacao,
                                escolaridade => escolaridade.tb_formacao_id,
                                formacao => formacao.id,
                                (escolaridade, formacao) => new { Escolaridade = escolaridade, DescricaoFormacao = formacao.descricao }
                            )
                            .Where(x => x.Escolaridade.ativo == 1 && (string.IsNullOrEmpty(busca) ? x.Escolaridade.codigo_interno_colaborador == cpf : EF.Functions.Like(x.Escolaridade.codigo_interno_colaborador, "%" + busca + "%")))
                            .Skip(cursor)
                            .Take(limite)
                            .Select(x => new
                            {
                                x.Escolaridade,
                                x.DescricaoFormacao
                            })
                            .ToList();

                foreach (var row in aux)
                {
                    var dto = new EscolaridadeDTO
                    {
                        Id = row.Escolaridade.id,
                        FormacaoId = row.Escolaridade.tb_formacao_id,
                        FormacaoDescricao = row.DescricaoFormacao,
                        Instituicao = row.Escolaridade.instituicao,
                        DataInicio = row.Escolaridade.data_inicio,
                        DataTermino = row.Escolaridade.data_termino,
                        Descricao = row.Escolaridade.descricao,
                        ColaboradorCpf = row.Escolaridade.codigo_interno_colaborador,
                        TipoDiplomaId = row.Escolaridade.tipo_diploma_id,
                        FilePathInternal = row.Escolaridade.path_diploma,
                        Ativo = true
                    };
                    ret.Add(dto);
                }
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EscolaridadeDTO GetModel(EscolaridadeDTO dto)
        {
            var EscolaridadeBanco = _colaboradorContext.tb_escolaridade.Where(x => x.id == dto.Id).FirstOrDefault();
            if (EscolaridadeBanco != null)
            {
                dto.FormacaoId = (long)EscolaridadeBanco.tb_formacao_id;
                dto.Instituicao = EscolaridadeBanco.instituicao;
                dto.DataInicio = EscolaridadeBanco.data_inicio;
                dto.DataTermino = EscolaridadeBanco.data_termino;
                dto.Descricao = EscolaridadeBanco.descricao;
                dto.Ativo = Convert.ToBoolean(EscolaridadeBanco.ativo);
                dto.ColaboradorCpf = EscolaridadeBanco.codigo_interno_colaborador;
                dto.FilePathInternal = EscolaridadeBanco.path_diploma;
                dto.TipoDiplomaId = EscolaridadeBanco.tipo_diploma_id;

                return dto;
            }
            else
            {
                return null;
            }
        }

        public void DeleteEscolaridadeColaboradorModel(EscolaridadeDTO dto)
        {
            try
            {
                var itemColabRow = _colaboradorContext.tb_escolaridade
                    .Where(x => x.codigo_interno_colaborador == dto.ColaboradorCpf
                    && x.id == dto.Id).FirstOrDefault();

                if (itemColabRow == null)
                    throw new KeyNotFoundException("Colaborador não possui essa escolaridade.");

                _colaboradorContext.tb_escolaridade.Remove(itemColabRow);

                _colaboradorContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EscolaridadeDTO Update(EscolaridadeDTO dto)
        {
            try
            {
                var colaborador = _colaboradorContext.tb_colaborador.Where(x => x.codigo_interno_colaborador == dto.ColaboradorCpf).FirstOrDefault();
                if (colaborador != null)
                {
                    var row = _colaboradorContext.tb_escolaridade.Where(x => x.id == dto.Id).FirstOrDefault();
                    if (row != null && row.codigo_interno_colaborador != dto.ColaboradorCpf)
                    {
                        throw new Exception("escolaridade não está atribuida a seu cpf");
                    }
                    if (_colaboradorContext.tb_formacao.Where(x => x.id == dto.FormacaoId && x.ativo == 1).FirstOrDefault() != null)
                    {
                        row.tb_formacao_id = dto.FormacaoId;
                    }
                    else
                    {
                        throw new Exception("formação não encontrada!");
                    }
                    dto.Ativo = Convert.ToBoolean(row.ativo);
                    row.instituicao = dto.Instituicao;
                    row.data_inicio = Convert.ToDateTime(dto.DataInicio);
                    row.data_termino = Convert.ToDateTime(dto.DataTermino);
                    row.descricao = dto.Descricao;
                    row.codigo_interno_colaborador = dto.ColaboradorCpf;
                    row.path_diploma = dto.FilePathInternal;
                    row.tipo_diploma_id = dto.TipoDiplomaId;

                    dto.Ativo = Convert.ToBoolean(row.ativo);
                    _colaboradorContext.tb_escolaridade.Update(row);
                    _colaboradorContext.SaveChanges();
                }

                return dto;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
