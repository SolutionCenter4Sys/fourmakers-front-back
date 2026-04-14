using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain.Experiencia;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class ExperienciaProfissionalRepository : IExperienciaProfissionalDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        public ExperienciaProfissionalRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public void UpsertSobre(string cpf, string sobre)
        {
            var row = _colaboradorContext.tb_colaborador_sobre.Where(c => c.codigo_interno_colaborador == cpf).FirstOrDefault();
            if (row == null)
            {
                row = new tb_colaborador_sobre();
                row.descricao = sobre;
                row.codigo_interno_colaborador = cpf;
                _colaboradorContext.tb_colaborador_sobre.Add(row);
                _colaboradorContext.SaveChanges();
            }
            else
            {
                row.descricao = sobre;
                row.data_criacao = DateTime.Now;
                _colaboradorContext.tb_colaborador_sobre.Update(row);
                _colaboradorContext.SaveChanges();
            }
        }
        public void RemoveSobre(string cpf)
        {
            var row = _colaboradorContext.tb_colaborador_sobre.Where(c => c.codigo_interno_colaborador == cpf).FirstOrDefault();
            if (row != null)
            {
                _colaboradorContext.tb_colaborador_sobre.Remove(row);
                _colaboradorContext.SaveChanges();
            }
            else
                throw new Exception("Sobre não encontrado para este colaborador");
        }

        public ColaboradorSobreDTO GetSobre(string cpf)
        {
            ColaboradorSobreDTO ret = null;
            var row = _colaboradorContext.tb_colaborador_sobre.AsNoTracking().FirstOrDefault(c => c.codigo_interno_colaborador == cpf);

            if (row != null)
            {
                ret = new ColaboradorSobreDTO();
                ret.Descricao = row.descricao;
                ret.Id = row.id;
                ret.DataCriacao = row.data_criacao;
            }

            return ret;
        }

        public List<string> GetAutoCompleteEmpresa(string nomeEmpresa, int limite, int cursor)
        {
            return _colaboradorContext.tb_experiencia_empresa_sugestao
                .Where(x => x.nome.Contains(nomeEmpresa))
                .Select(x => x.nome)
                .OrderBy(x => x)
                .Skip(cursor)
                .Take(limite)
                .ToList();
        }

        public List<string> GetAutoCompleteProjeto(string nomeProjeto, int limite, int cursor)
        {
            return _colaboradorContext.tb_experiencia_projeto_sugestao
                .Where(x => x.nome.Contains(nomeProjeto))
                .Select(x => x.nome)
                .OrderBy(x => x)
                .Skip(cursor)
                .Take(limite)
                .ToList();
        }

        private List<string> getProjetos(long Id)
        {
            return _colaboradorContext.tb_experiencia_projeto
                            .Where(ep => ep.experiencia_id == Id)
                            .Select(ep => ep.nome_projeto).ToList();
        }

        public void AddProjetosAusentes(List<string> projetos)
        {
            foreach (var projeto in projetos)
            {
                if (!_colaboradorContext.tb_experiencia_projeto_sugestao.Any(x => x.nome.Contains(projeto)))
                {
                    _colaboradorContext.tb_experiencia_projeto_sugestao.Add(new tb_experiencia_projeto_sugestao()
                    {
                        nome = projeto
                    });
                }
            }
        }

        public ExperienciaDTO Save(ExperienciaDTO dto)
        {
            try
            {
                var colaborador = _colaboradorContext.tb_colaborador.Where(x => x.codigo_interno_colaborador == dto.ColaboradorCpf).FirstOrDefault();
                if (colaborador != null)
                {
                    var row = new tb_experiencia();
                    row.ativo = 1;
                    dto.Ativo = Convert.ToBoolean(row.ativo);
                    row.descricao = dto.Atividades;
                    row.titulo = dto.Funcao;
                    row.empresa = dto.Empresa;
                    row.codigo_interno_colaborador = dto.ColaboradorCpf;
                    row.data_inicio = Convert.ToDateTime(dto.DataInicio);
                    row.data_saida = dto.DataSaida;

                    try
                    {
                        AddSujestaoEmpresaByName(dto.Empresa);
                        _colaboradorContext.SaveChanges();
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("Falha ao adicionar Sujestão de Empresa. " + err.Message);
                    }

                    AddProjetosAusentes(dto.Projetos);

                    if (dto.DataSaida != null)
                    {
                        row.atual = 0;
                    }
                    else
                    {
                        row.atual = 1;
                    }
                    dto.Atual = Convert.ToBoolean(row.atual);
                    _colaboradorContext.tb_experiencia.Add(row);
                    _colaboradorContext.SaveChanges();
                    dto.Id = row.id;
                    AddExperienciaProjetosByList(dto.Projetos, row);
                }

                return dto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ExperienciaDTO> Listar(string busca, int cursor, int limite, string cpf)
        {
            try
            {
                var ret = new List<ExperienciaDTO>();

                var query = _colaboradorContext.tb_experiencia
                        .Where(x => x.ativo == 1 && (string.IsNullOrEmpty(busca) ? x.codigo_interno_colaborador == cpf : EF.Functions
                        .Like(x.codigo_interno_colaborador, "%" + busca + "%")))
                        .OrderByDescending(x => x.data_inicio)
                        .Skip(cursor)
                        .Take(limite)
                        .ToList();

                foreach (var row in query)
                {
                    var listaProjetos = getProjetos(row.id);
                    var dto = new ExperienciaDTO
                    {
                        Id = row.id,
                        Atividades = row.descricao,
                        ColaboradorCpf = row.codigo_interno_colaborador,
                        Empresa = row.empresa,
                        Funcao = row.titulo,
                        DataInicio = row.data_inicio,
                        DataSaida = row.data_saida,
                        Projetos = listaProjetos,
                        Ativo = true,
                        Atual = row.data_saida == null
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

        public ExperienciaDTO GetModel(ExperienciaDTO dto)
        {
            var ExperienciaBanco = _colaboradorContext.tb_experiencia.Where(x => x.id == dto.Id).FirstOrDefault();
            if (ExperienciaBanco != null)
            {
                dto.Atividades = ExperienciaBanco.descricao;
                dto.Empresa = ExperienciaBanco.empresa;
                dto.Funcao = ExperienciaBanco.titulo;
                dto.Ativo = Convert.ToBoolean(ExperienciaBanco.ativo);
                dto.DataSaida = ExperienciaBanco.data_saida;
                dto.DataInicio = ExperienciaBanco.data_inicio;
                dto.ColaboradorCpf = ExperienciaBanco.codigo_interno_colaborador;
                dto.Atual = Convert.ToBoolean(ExperienciaBanco.atual);
                dto.Projetos = getProjetos(dto.Id);
                return dto;
            }
            else
            {
                return null;
            }
        }

        public void DeleteExperienciaProfissionalModel(ExperienciaDTO dto)
        {
            try
            {
                var experiencia = _colaboradorContext.tb_experiencia
                                    .Where(x => x.id == dto.Id)
                                    .FirstOrDefault();

                if (experiencia == null)
                    throw new KeyNotFoundException("A experiência com o ID fornecido não existe.");

                var itemColabRow = _colaboradorContext.tb_experiencia
                                    .Where(x => x.codigo_interno_colaborador == dto.ColaboradorCpf
                                            && x.id == experiencia.id)
                                    .FirstOrDefault();

                if (itemColabRow == null)
                    throw new KeyNotFoundException("Colaborador não possui essa experiência.");

                var projetos = _colaboradorContext.tb_experiencia_projeto
                                .Where(p => p.experiencia_id == itemColabRow.id);

                _colaboradorContext.tb_experiencia_projeto.RemoveRange(projetos);
                _colaboradorContext.tb_experiencia.Remove(itemColabRow);

                _colaboradorContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ExperienciaDTO Update(ExperienciaDTO dto)
        {
            try
            {
                var colaborador = _colaboradorContext.tb_colaborador.Where(x => x.codigo_interno_colaborador == dto.ColaboradorCpf).FirstOrDefault();
                if (colaborador != null)
                {
                    var row = _colaboradorContext.tb_experiencia.Where(x => x.id == dto.Id).FirstOrDefault();
                    row.descricao = dto.Atividades;
                    row.titulo = dto.Funcao;
                    row.empresa = dto.Empresa;
                    row.codigo_interno_colaborador = dto.ColaboradorCpf;
                    row.data_inicio = Convert.ToDateTime(dto.DataInicio);
                    row.data_saida = dto.DataSaida;

                    UpdateProjetosByList(dto.Projetos, row);

                    _colaboradorContext.tb_experiencia_projeto.Where(p => p.experiencia_id == row.id);
                    AddSujestaoEmpresaByName(dto.Empresa);
                    AddProjetosAusentes(dto.Projetos);

                    if (dto.DataSaida != null)
                    {
                        row.atual = 0;
                    }
                    else
                    {
                        row.atual = 1;
                    }
                    dto.Atual = Convert.ToBoolean(row.atual);
                    _colaboradorContext.tb_experiencia.Update(row);
                    _colaboradorContext.SaveChanges();
                }

                return dto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AddSujestaoEmpresaByName(string empresa)
        {
            var empresaExistente = _colaboradorContext.tb_experiencia_empresa_sugestao.ToList()
                                                            .Any(x => StringUtil.RemoveDiacritics(x.nome).ToUpper().Contains(StringUtil.RemoveDiacritics(empresa).ToUpper()));

            if (!empresaExistente)
            {
                _colaboradorContext.tb_experiencia_empresa_sugestao.Add(new tb_experiencia_empresa_sugestao
                {
                    nome = empresa
                });
            }
        }

        private void AddExperienciaProjetosByList(List<string> projetos, tb_experiencia row)
        {
            foreach (var item in projetos)
            {
                _colaboradorContext.tb_experiencia_projeto.Add(new tb_experiencia_projeto
                {
                    nome_projeto = item,
                    experiencia_id = row.id,
                });
            }
            _colaboradorContext.SaveChanges();
        }

        private void UpdateProjetosByList(List<string> projetosList, tb_experiencia row)
        {
            var projetos = _colaboradorContext.tb_experiencia_projeto.Where(p => p.experiencia_id == row.id);
            _colaboradorContext.tb_experiencia_projeto.RemoveRange(projetos);
            foreach (var item in projetosList)
            {
                _colaboradorContext.tb_experiencia_projeto.Add(new tb_experiencia_projeto
                {
                    nome_projeto = item,
                    experiencia_id = row.id,
                });
            }
        }
    }
}
