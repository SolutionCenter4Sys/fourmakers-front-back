using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using Core.DomainModel;
using Core.Domain.Colaborador;
using Core.Domain.Dominio;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Competencia.Domain.Impl.Services
{
    [LogDomainClass]
    public class DominioService : IDominioService
    {
        private readonly IColaboradorClient _colaboradorClient;
        private readonly IFirebaseClient _firebaseClient;
        private readonly IAspNetUser _aspNetUser;
        private readonly IDominioRepository _repository;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;
        private IHistoricoCVRepository _historicoCvRepository;
        private readonly ICompetenciaDtoRepository _competenciaRepository;
        private readonly ISkillGenericService _skillGenericService;

        public DominioService(IColaboradorClient colaboradorClient, IFirebaseClient firebaseClient,
            IAspNetUser aspNetUser, IDominioRepository repository, IVerificaSeCpfESistemico verificaCpfSistemico,
            IHistoricoCVRepository historicoCvRepository, ICompetenciaDtoRepository competenciaRepository, ISkillGenericService skillGenericService)
        {
            _colaboradorClient = colaboradorClient;
            _firebaseClient = firebaseClient;
            _aspNetUser = aspNetUser;
            _repository = repository;
            _verificaCpfSistemico = verificaCpfSistemico;
            _historicoCvRepository = historicoCvRepository;
            _competenciaRepository = competenciaRepository;
            _skillGenericService = skillGenericService;
        }

        public async Task<ApiGenericResult<List<CompetenciaGroupDTO>>> GetGroupDominioByIds(List<long> ids)
        {
            var resultado = new ApiGenericResult<List<CompetenciaGroupDTO>>();
            if (ids is null || ids.Count == 0)
                return resultado;

            // enviado tipo 4 - dominio do GCOLB
            var encontradas = await _competenciaRepository.GetCompetenciasByTypeAndGroupId((int)ItemPerfilEnum.DOMINIONEGOCIO, ids.Distinct().ToList());

            resultado.Retorno = encontradas;
            return resultado;
        }

        public DominioDTO AddDominio(string descricao)
        {
            try
            {
                var listaDesc = _repository.ListarDominioPorDescricao(descricao);
                if (listaDesc.Any())
                {
                    throw new ArgumentException("Esse domínio já existe.");
                }
                var idUser = _repository.GetUsuarioCriacaoId(_aspNetUser.GetUsuarioLogado().Cpf);
                if (idUser == null)
                {
                    throw new ArgumentException("Usuário não encontrado.");
                }
                var dominio = _repository.InserirDominio(descricao, idUser ?? -1);
                return dominio;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DominioDTO AddDominio(string descricao, string codInternoColaborador, bool importacaoLote = false)
        {
            try
            {
                var listaDesc = _repository.ListarDominioPorDescricao(descricao);
                if (listaDesc.Any())
                {
                    throw new ArgumentException("Esse domínio já existe.");
                }

                var cpfRequest = String.Empty;
                if (importacaoLote)
                    cpfRequest = ApiClient.Domain.ClientConfig.Clients.Colaborador.CpfAdmin;
                else
                    cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(codInternoColaborador);

                var idUser = _repository.GetUsuarioCriacaoId(cpfRequest);
                if (idUser == null)
                {
                    throw new ArgumentException("Usuário não encontrado.");
                }
                var dominio = _repository.InserirDominio(descricao, idUser ?? -1);
                return dominio;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ItemPerfilDTO> ListDominio(string busca, int cursor, int limite)
        {
            try
            {
                var dominios = _repository.ListarDominio(busca, cursor, limite);
                return dominios.Select(x => new ItemPerfilDTO()
                {
                    Id = x.Id,
                    Nome = x.Descricao,
                    Descricao = x.Descricao,
                    Pendente = x.Pendente
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ItemPerfilDTO GetDominioById(long id)
        {
            try
            {
                var dominio = _repository.ObterDominioPorId(id);
                if (dominio == null)
                {
                    throw new ArgumentException($"Não foi possível encontrar o domínio com id {id}");
                }
                return new ItemPerfilDTO
                {
                    Descricao = dominio.Descricao,
                    Id = dominio.Id,
                    Pendente = dominio.Pendente
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DominioColaboradorDTO> ListDominioColaborador(string cpfColaborador)
        {
            try
            {
                var ret = _repository.ListarDominioColaborador(cpfColaborador);
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DominioColaboradorDTO AddDominioColaborador(long dominioId, long? nivelId, string cpf, bool minhaJornada, string gestorExternoPerfil, string usuarioLogado, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var listaIds = _repository.ListarDominiosAtribuidos(cpf);
                if (listaIds.Any(x => x == dominioId))
                {
                    throw new ArgumentException("Colaborador já possui esse domínio de negócio.");
                }
                var ret = _repository.AssociarDominioColaborador(dominioId, nivelId, cpf);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.INSERT, dominioId, nivelId, ItemCVEnum.DOMINIO_NEGOCIO);

                if (minhaJornada)
                {
                    SkillsLog logSkill = new()
                    {
                        CodigoInternoColaborador = cpf,
                        GestorExternoPerfil = gestorExternoPerfil,
                        SkillId = dominioId,
                        ItemPerfil = ItemPerfilEnum.DOMINIONEGOCIO,
                        NivelId = nivelId,
                        SkillsMovimentacaoId = EnumSkillsMovimentacao.ADICIONADO_PERFIL,
                        LogAutomatico = true,
                        CodigoInternoColaboradorLogado = usuarioLogado
                    };

                    _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool RemoveDominioColaborador(long dominioId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var ret = _repository.RemoverDominioColaborador(dominioId, cpf);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.DELETE, dominioId, null, ItemCVEnum.DOMINIO_NEGOCIO);
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<NivelDTO> ListaNivelDominio()
        {
            try
            {
                return _repository.ListaNivelDominio();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DominioColaboradorDTO AlterarDominioColaborador(string cpf, long id, long? nivelId, string gestorExternoPerfil, bool minhaJornada, string usuarioLogado, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var listaIds = _repository.ListarDominiosAtribuidos(cpf);
                if (!listaIds.Any(x => x == id))
                {
                    throw new ArgumentException("Colaborador não possui esse domínio de negócio.");
                }
                if (nivelId != null)
                {
                    var listaNiveis = ListaNivelDominio();
                    if (!listaNiveis.Any(x => x.Id == nivelId))
                    {
                        throw new ArgumentException("Esse nível não existe para domínio.");
                    }
                }
                var dominioColaborador = _repository.AlterarDominioColaborador(id, nivelId, cpf);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.UPDATE, id, nivelId, ItemCVEnum.DOMINIO_NEGOCIO);

                if (minhaJornada)
                {
                    SkillsLog logSkill = new()
                    {
                        CodigoInternoColaborador = cpf,
                        GestorExternoPerfil = gestorExternoPerfil,
                        SkillId = id,
                        ItemPerfil = ItemPerfilEnum.DOMINIONEGOCIO,
                        NivelId = nivelId,
                        SkillsMovimentacaoId = EnumSkillsMovimentacao.ATUALIZADO,
                        LogAutomatico = true,
                        CodigoInternoColaboradorLogado = usuarioLogado
                    };

                    _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
                }

                return dominioColaborador;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<long> ListarDominiosAtribuidos()
        {
            return _repository.ListarDominiosAtribuidos(_aspNetUser.GetUsuarioLogado().Cpf);
        }

        public List<long> ListarDominiosAtribuidos(string cpf)
        {
            return _repository.ListarDominiosAtribuidos(cpf);
        }

        public DominioColaboradorResult AdicionarDominioColaboradorEmLote(List<AddDominioColabParam> param, bool minhaJornada, string usuarioLogado)
        {
            var ret = new DominioColaboradorResult();
            ret.Respostas = new List<ItemPerfilResult>();
            foreach (var item in param)
            {
                try
                {
                    var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(item.Cpf);
                    if (item.DominioId.ToIntOuZero() == 0)
                    {
                        var competenciaAdicionada = AddDominio(item.Descricao);
                        item.DominioId = competenciaAdicionada.Id;
                    }
                    else
                    {
                        var competenciaValidacao = GetDominioById(item.DominioId);
                    }
                    var retService = AddDominioColaborador(item.DominioId, item.NivelId, cpfRequest, minhaJornada, item.GestorExternoPerfil, usuarioLogado);
                    ret.Respostas.Add(new ItemPerfilResult
                    {
                        Sucesso = true,
                        ItemId = retService.Dominio.Id
                    });
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            ret.Sucesso = true;

            return ret;
        }

        public ListaDominioResult ListarDominiosNaoAtribuidos(string busca, int cursor, int limite)
        {
            var listaResult = new ListaDominioResult();

            var listaIds = ListarDominiosAtribuidos(_aspNetUser.GetUsuarioLogado().Cpf);
            var listaDominios = ListDominio(busca, cursor, limite);

            listaResult.Dominio = listaDominios.Where(x => !listaIds.Contains(x.Id)).ToList();
            listaResult.Nivel = ListaNivelDominio();

            return listaResult;
        }

        public async Task<List<KeyValuePair<string, long>>> GetDominioInfoByDescricao(List<string> dominios)
        {
            return await _skillGenericService.GetSkillInfoByDescricaoAsync(dominios, TipoSkillEnum.DOMINIO, _aspNetUser.GetUsuarioLogado().Cpf);
        }
    }
}