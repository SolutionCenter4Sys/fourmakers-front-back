using Colaboracao.Core;
using Colaboracao.Helper;
using Colaborador.Domain.Interfaces.Services;
using Core.Domain.Colaborador;
using Core.Domain;
using Core.DomainModel;
using Core.DomainModel.Org;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Experiencia;
using System;
using System.Collections.Generic;
using System.Linq;
using Colaboracao.Core.Interfaces;

using Logs.Infra.Attributes;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class ExperienciaProfissionalService : IExperienciaProfissionalService
    {
        private readonly ILogCore _logCore;
        private readonly IExperienciaProfissionalDtoRepository _experienciaProfissionalRepository;
        private IHistoricoCVRepository _historicoCvRepository;

        private readonly IRealizacaoColaboradorRepository _realizacaoColaboradorRepository;
        private readonly IOrgRepository _orgRepository;
        private readonly IClassificacaoService _classificacaoService;

        public ExperienciaProfissionalService(ILogCore logCore,
            IExperienciaProfissionalDtoRepository experienciaProfissionalRepository, IHistoricoCVRepository historicoCvRepository,
            IRealizacaoColaboradorRepository realizacaoColaboradorRepository, IOrgRepository orgRepository, IClassificacaoService classificacaoService)
        {
            _experienciaProfissionalRepository = experienciaProfissionalRepository;
            _logCore = logCore;
            _historicoCvRepository = historicoCvRepository;
            _realizacaoColaboradorRepository = realizacaoColaboradorRepository;
            _orgRepository = orgRepository;
            _classificacaoService = classificacaoService;
        }

        public ExperienciaDTO AdicionarNovaExperiencia(string descricao, string cpf, string titulo, string empresa, DateTime dataInicio, DateTime? dataSaida, List<string> projetos, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL, bool useClassificacao = true)
        {
            VerificarSeDataFimEhMaiorOuIgualQueDataInicio(dataInicio, dataSaida);

            var dto = new ExperienciaDTO
            {
                Atividades = descricao,
                ColaboradorCpf = cpf,
                Funcao = titulo,
                Empresa = empresa,
                DataInicio = dataInicio,
                DataSaida = dataSaida,
                Projetos = projetos
            };
            if (dto.DataSaida != null)
            {
                dto.Atual = false;
            }
            var ret = _experienciaProfissionalRepository.Save(dto);
            _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.UPDATE, null, null, ItemCVEnum.EXPERIENCIA);
            if (useClassificacao)
            {
                _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaborador(cpf);
            }
            return ret;
        }

        public UpdateExperienciaDTO AtualizarExperiencia(long id, string descricao, string colaboradorCpf, string titulo, string empresa, DateTime dataInicio, DateTime? dataSaida, List<string> projetos, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL, bool useClassificacao = true)
        {
            VerificarSeDataFimEhMaiorOuIgualQueDataInicio(dataInicio, dataSaida);

            var dto = new ExperienciaDTO
            {
                Id = id,
                Atividades = descricao,
                ColaboradorCpf = colaboradorCpf,
                Funcao = titulo,
                Empresa = empresa,
                DataInicio = dataInicio,
                DataSaida = dataSaida,
                Projetos = projetos
            };
            if (dto.DataSaida != null)
            {
                dto.Atual = false;
            }
            var updated = _experienciaProfissionalRepository.Update(dto);
            var ret = new UpdateExperienciaDTO
            {
                Id = updated.Id,
                Funcao = updated.Funcao,
                ColaboradorCpf = updated.ColaboradorCpf,
                DataInicio = updated.DataInicio,
                DataSaida = updated.DataSaida,
                Atividades = updated.Atividades,
                Empresa = updated.Empresa,
                Projetos = updated.Projetos
            };
            _historicoCvRepository.InserirHistoricoCV(colaboradorCpf, origem, TipoItemCVEnum.UPDATE, null, null, ItemCVEnum.EXPERIENCIA);
            if (useClassificacao)
            {
                _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaborador(colaboradorCpf);
            }
            return ret;
        }

        public bool VerificarSeDataFimEhMaiorOuIgualQueDataInicio(DateTime dataInicio, DateTime? dataSaida)
        {
            if (dataSaida.HasValue)
            {
                DateTime dataEfetivaSaida = dataSaida.Value;

                if (!DateTimeUtil.VerificarSeDataFimEhMaiorOuIgualQueDataInicio(dataInicio, dataEfetivaSaida))
                {
                    throw new ArgumentException("A data de início de um período não pode ser superior à data fim");
                }
            }

            return true;
        }

        public ExperienciaDTO GetExperinciaProfissionalById(long id)
        {
            try
            {
                var dto = new ExperienciaDTO { Id = id };
                var result = _experienciaProfissionalRepository.GetModel(dto);
                if (result == null)
                    throw new Exception("Experiência Profissional não encontrada");
                return result;
            }
            catch (Exception)
            {
                throw new Exception("Experiência Profissional não encontrada");
            }
        }

        public List<ExperienciaEmpresaDTO> ListarExperienciaProfissionalAgrupada(string busca, int cursor, int limite, string cpf, int orgId)
        {
            var experienciasRaw = ListarExperienciaProfissional(busca, cursor, limite, cpf);
            var experienciaEmpresasGrp = experienciasRaw.Select(x => new KeyValuePair<string, ListaExperienciaDTO>(StringUtil.RemoveDiacritics(x.Empresa).ToUpper().Trim(), x)).GroupBy(x => x.Key).ToList();

            //Montando os objetos agrupados por empresa
            var experienciaEmpresasAux = new List<ExperienciaEmpresaDTO>();
            foreach (var grpExperienciaEmpresa in experienciaEmpresasGrp)
            {
                var nomeEmpresa = grpExperienciaEmpresa.ElementAt(0).Value.Empresa;
                var dataInicio = grpExperienciaEmpresa.Min(x => x.Value.DataInicio);
                var atual = grpExperienciaEmpresa.Where(x => x.Value.DataSaida == null).Any();
                DateTime? dataSaida = !atual ? grpExperienciaEmpresa.Max(x => x.Value.DataSaida) : null;
                var experienciaEmpresa = new ExperienciaEmpresaDTO
                {
                    Atual = atual,
                    Empresa = nomeEmpresa,
                    DataInicio = dataInicio,
                    DataSaida = dataSaida,
                    Experiencias = new List<ItemExperienciaEmpresaDTO>()
                };
                foreach (var experiencia in grpExperienciaEmpresa)
                {
                    var cargo = experiencia.Value.Funcao;
                    var atividades = experiencia.Value.Atividades;
                    var projetos = experiencia.Value.Projetos;
                    var dataInicioExperiencia = experiencia.Value.DataInicio;
                    var experienciaAtual = experiencia.Value.DataSaida == null;
                    var dataSaidaExperiencia = experiencia.Value.DataSaida;
                    experienciaEmpresa.Experiencias.Add(new ItemExperienciaEmpresaDTO
                    {
                        Atividades = atividades,
                        Atual = experienciaAtual,
                        ColaboradorCpf = cpf,
                        Id = experiencia.Value.Id,
                        Funcao = cargo,
                        DataInicio = dataInicioExperiencia,
                        DataSaida = dataSaidaExperiencia,
                        Projetos = projetos
                    });
                }
                experienciaEmpresa.Experiencias = experienciaEmpresa.Experiencias.OrderByDescending(x => x.DataInicio).ToList();
                experienciaEmpresasAux.Add(experienciaEmpresa);
            }

            //Ordenando as experiências pela data de inicio
            var experienciasOrdenadas = new List<KeyValuePair<string, ItemExperienciaEmpresaDTO>>();
            experienciaEmpresasAux.ForEach(x =>
                experienciasOrdenadas.AddRange(x.Experiencias.Select(y => new KeyValuePair<string, ItemExperienciaEmpresaDTO>(x.Empresa, y)).ToList())
            );
            experienciasOrdenadas = experienciasOrdenadas.OrderBy(x => x.Value.DataInicio).ToList();

            if (experienciasOrdenadas.Count() == 0)
                return new List<ExperienciaEmpresaDTO>() { };

            //Montando a resposta com os blocos de empresa, separados por intervalo de tempo da experiência
            var experienciaEmpresas = new List<ExperienciaEmpresaDTO>(){
                MontaExperienciaEmpresaListaAgrupada(experienciaEmpresasAux, experienciasOrdenadas.ElementAt(0))
            };
            foreach (var experiencia in experienciasOrdenadas.Skip(1))
            {
                if (experiencia.Key == experienciaEmpresas.Last().Empresa)
                {
                    experienciaEmpresas.Last().DataSaida = experiencia.Value.DataSaida;
                    experienciaEmpresas.Last().Experiencias.Add(experiencia.Value);
                }
                else
                {
                    experienciaEmpresas.Add(MontaExperienciaEmpresaListaAgrupada(experienciaEmpresasAux, experiencia));
                }
            }
            experienciaEmpresas.ForEach(x => x.Experiencias = x.Experiencias.OrderByDescending(y => y.DataInicio).ToList());
            experienciaEmpresas.Reverse();

            var realizacoesRaw = _realizacaoColaboradorRepository.ListaRealizacoesColaborador(cpf, orgId);
            var realizacoesGrp = realizacoesRaw.GroupBy(x => x.Cliente + "|" + x.Projeto + "|" + x.Perfil);
            var realizacoes = realizacoesGrp.Select(x => new RealizacaoColaboradorDTO
            {
                DataInicio = x.Min(y => y.DataInicio),
                DataFim = x.Max(y => y.DataFim),
                Atual = x.Max(y => y.DataFim) > DateTime.Now,
                Cliente = x.First().Cliente,
                Projeto = x.First().Projeto,
                Perfil = x.First().Perfil,
                Horas = x.Sum(y => y.Horas),
                Skills = new List<DataTransferObject.Domain.Competencia.SkillNivelDTO>()
            }).ToList();

            foreach (var realizacao in realizacoes)
            {
                foreach (var grpRealizacao in realizacoesGrp.Where(x => x.Key == realizacao.Cliente + "|" + realizacao.Projeto + "|" + realizacao.Perfil).First())
                {
                    foreach (var skill in grpRealizacao.Skills)
                    {
                        if (!realizacao.Skills.Where(x => x.Id == skill.Id).Any())
                            realizacao.Skills.Add(skill);
                    }
                }
            }

            realizacoes = realizacoes.OrderByDescending(x => x.DataInicio).ToList();

            var orgInfo = _orgRepository.BuscarOrg(orgId);
            if (StringUtil.RemoveDiacritics(experienciaEmpresas.First().Empresa).ToUpper().Trim() == StringUtil.RemoveDiacritics(orgInfo.Descricao).ToUpper().Trim())
            {
                experienciaEmpresas.First().Realizacoes = realizacoes ?? new List<RealizacaoColaboradorDTO>();
            }
            else if (realizacoes.Count() > 0)
            {
                experienciaEmpresas = experienciaEmpresas.Prepend(new ExperienciaEmpresaDTO
                {
                    Atual = realizacoes.First().Atual,
                    DataInicio = realizacoes.Last().DataInicio,
                    DataSaida = realizacoes.First().DataFim,
                    Empresa = orgInfo.Descricao,
                    Experiencias = new List<ItemExperienciaEmpresaDTO>(),
                    Realizacoes = realizacoes ?? new List<RealizacaoColaboradorDTO>()
                }).ToList();
            }
            experienciaEmpresas.ForEach(x =>
            {
                if (x.Realizacoes == null)
                    x.Realizacoes = new List<RealizacaoColaboradorDTO>();
            });
            return experienciaEmpresas;
        }

        private static ExperienciaEmpresaDTO MontaExperienciaEmpresaListaAgrupada(List<ExperienciaEmpresaDTO> experienciaEmpresasAux, KeyValuePair<string, ItemExperienciaEmpresaDTO> experiencia)
        {
            var experienciaEmpresaAux = experienciaEmpresasAux.Where(x => x.Empresa == experiencia.Key).First();
            var experienciaEmpresa = new ExperienciaEmpresaDTO
            {
                Empresa = experienciaEmpresaAux.Empresa,
                Atual = experienciaEmpresaAux.Atual,
                DataInicio = experiencia.Value.DataInicio,
                DataSaida = experiencia.Value.DataSaida,
                Experiencias = new List<ItemExperienciaEmpresaDTO> { experiencia.Value }
            };
            return experienciaEmpresa;
        }

        public List<ListaExperienciaDTO> ListarExperienciaProfissional(string busca, int cursor, int limite, string cpf)
        {
            var experiencias = _experienciaProfissionalRepository.Listar(busca, cursor, limite, cpf);
            return experiencias.Select(item => new ListaExperienciaDTO
            {
                Id = item.Id,
                Atividades = item.Atividades,
                ColaboradorCpf = item.ColaboradorCpf,
                Empresa = item.Empresa,
                Funcao = item.Funcao,
                DataInicio = item.DataInicio,
                DataSaida = item.DataSaida,
                Projetos = item.Projetos,
                Atual = item.DataSaida == null
            }).ToList();
        }

        public void RemoverExperienciaProfissional(long experienciaId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL, bool useClassificacao = true)
        {
            try
            {
                var dto = new ExperienciaDTO
                {
                    Id = experienciaId,
                    ColaboradorCpf = cpf
                };
                _experienciaProfissionalRepository.DeleteExperienciaProfissionalModel(dto);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.DELETE, null, null, ItemCVEnum.EXPERIENCIA);
                if (useClassificacao)
                {
                    _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaborador(cpf);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> AutoCompleteEmpresa(string nomeEmpresa, int limite, int cursor)
        {
            var ret = _experienciaProfissionalRepository.GetAutoCompleteEmpresa(nomeEmpresa, limite, cursor);
            return ret;
        }
        public List<string> AutoCompleteProjeto(string nomeProjeto, int limite, int cursor)
        {
            var ret = _experienciaProfissionalRepository.GetAutoCompleteProjeto(nomeProjeto, limite, cursor);
            return ret;
        }

        public void AdicionarSobre(string cpf, string sobre, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL, bool useClassificacao = true)
        {
            try
            {
                _experienciaProfissionalRepository.UpsertSobre(cpf, sobre);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.UPDATE, null, null, ItemCVEnum.SOBRE);
                if (useClassificacao)
                {
                    _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaborador(cpf);
                }
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }

        public ColaboradorSobreDTO BuscarSobre(string cpf)
        {
            var colaboradorSobre = _experienciaProfissionalRepository.GetSobre(cpf);
            return colaboradorSobre;
        }

        public void RemoverSobre(string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL,  bool useClassificacao = true)
        {
            try
            {
                _experienciaProfissionalRepository.RemoveSobre(cpf);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.DELETE, null, null, ItemCVEnum.SOBRE);
                if (useClassificacao)
                {
                    _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaborador(cpf);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
