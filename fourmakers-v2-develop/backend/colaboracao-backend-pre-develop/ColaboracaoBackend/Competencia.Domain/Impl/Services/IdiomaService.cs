using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using Core.DomainModel;
using Core.Domain.Colaborador;
using Core.Domain.IIdioma;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Competencia.Domain.Impl.Services
{
    [LogDomainClass]
    public class IdiomaService : IIdiomaService
    {
        private readonly IColaboradorClient _colaboradorClient;
        private readonly IFirebaseClient _firebaseClient;
        private readonly IAspNetUser _aspNetUser;
        private readonly IIdiomaRepository _repositoryIdioma;
        private readonly IIdiomaColaboradorRepository _repositoryColaboradorIdioma;
        private readonly IIdiomaNivelRepository _repositoryNivelIdioma;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;
        private IHistoricoCVRepository _historicoCvRepository;
        private readonly ICompetenciaDtoRepository _competenciaRepository;

        public IdiomaService(IColaboradorClient colaboradorClient, IFirebaseClient firebaseClient, IAspNetUser aspNetUser,
            IVerificaSeCpfESistemico verificaCpfSistemico, IHistoricoCVRepository historicoCvRepository, ICompetenciaDtoRepository competenciaRepository)
        {
            _colaboradorClient = colaboradorClient;
            _firebaseClient = firebaseClient;
            _aspNetUser = aspNetUser;
            _verificaCpfSistemico = verificaCpfSistemico;
            _historicoCvRepository = historicoCvRepository;
            _competenciaRepository = competenciaRepository;
        }

        public IdiomaService(IColaboradorClient colaboradorClient, IFirebaseClient firebaseClient,
            IAspNetUser aspNetUser, IIdiomaRepository repositoryIdioma,
            IIdiomaColaboradorRepository repositoryColaboradorIdioma,
            IIdiomaNivelRepository repositoryNivelIdioma,
            IVerificaSeCpfESistemico verificaCpfSistemico, IHistoricoCVRepository historicoCvRepository, ICompetenciaDtoRepository competenciaRepository)
        {
            _colaboradorClient = colaboradorClient;
            _firebaseClient = firebaseClient;
            _aspNetUser = aspNetUser;
            _repositoryIdioma = repositoryIdioma;
            _repositoryColaboradorIdioma = repositoryColaboradorIdioma;
            _repositoryNivelIdioma = repositoryNivelIdioma;
            _verificaCpfSistemico = verificaCpfSistemico;
            _historicoCvRepository = historicoCvRepository;
            _competenciaRepository = competenciaRepository;
        }

        public async Task<ApiGenericResult<List<CompetenciaGroupDTO>>> GetGroupIdiomaByIds(List<long> ids)
        {
            var resultado = new ApiGenericResult<List<CompetenciaGroupDTO>>();
            if (ids is null || ids.Count == 0)
                return resultado;

            // enviado tipo 9 - idioma do GCOLB
            var encontradas = await _competenciaRepository.GetCompetenciasByTypeAndGroupId((int)ItemPerfilEnum.IDIOMA, ids.Distinct().ToList());

            resultado.Retorno = encontradas;
            return resultado;
        }

        public IdiomaDTO AdicionarIdioma(string descricao)
        {
            try
            {
                var idioma = new IdiomaDTO();
                idioma.Descricao = descricao;
                _repositoryIdioma.AdicionarIdioma(idioma);
                return idioma;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<IdiomaDTO> ListarIdioma(string busca, int cursor, int limite)
        {
            return _repositoryIdioma.ListarIdiomas(busca, cursor, limite);
        }

        public IdiomaColaboradorDTO AlterarIdiomaColaborador(string cpf, int id, long? nivelId, string gestorExternoPerfil, bool minhaJornada, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var idiomaColaborador = new IdiomaColaboradorDTO()
                {
                    ColaboradorCpf = cpf,
                    Idioma = new IdiomaDTO()
                    {
                        Id = id,
                    },
                    Nivel = new NivelDTO()
                    {
                        Id = nivelId
                    }
                };
                _repositoryColaboradorIdioma.AlterarIdiomaColaborador(idiomaColaborador);

                var metodologia = _repositoryIdioma.GetIdiomaById(idiomaColaborador.Idioma.Id);
                idiomaColaborador.Idioma = new IdiomaDTO
                {
                    Id = metodologia.Id,
                    Descricao = metodologia.Descricao,
                    Pendente = metodologia.Pendente
                };

                _colaboradorClient.EnviaPushNotificationRedeColaborador(idiomaColaborador.ColaboradorCpf, "Idioma atualizado!", " atualizou o Idioma " + idiomaColaborador.Idioma.Descricao, _aspNetUser.GetUsuarioLogado().Token);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.UPDATE, id, nivelId, ItemCVEnum.IDIOMA);

                if (minhaJornada)
                {
                    SkillsLog logSkill = new()
                    {
                        CodigoInternoColaborador = cpf,
                        GestorExternoPerfil = gestorExternoPerfil,
                        SkillId = id,
                        ItemPerfil = ItemPerfilEnum.IDIOMA,
                        NivelId = nivelId,
                        SkillsMovimentacaoId = EnumSkillsMovimentacao.ATUALIZADO,
                        LogAutomatico = true,
                        CodigoInternoColaboradorLogado = cpf
                    };

                    _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
                }

                return idiomaColaborador;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IdiomaDTO GetIdiomaById(int id)
        {
            return _repositoryIdioma.GetIdiomaById(id);
        }
        public void RemoveIdiomaColaborador(int IdiomaId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var idiomaColaborador = new IdiomaColaboradorDTO();
                idiomaColaborador.ColaboradorCpf = cpf;
                idiomaColaborador.IdIdioma = IdiomaId;
                _repositoryColaboradorIdioma.RemoverIdiomaColaborador(idiomaColaborador);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.DELETE, IdiomaId, null, ItemCVEnum.IDIOMA);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IdiomaColaboradorDTO AdicionarIdiomaColaborador(int IdiomaId, long? nivelId, string cpf, string gestorExternoPerfil, bool minhaJornada, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            var idiomaColaborador = new IdiomaColaboradorDTO();

            idiomaColaborador.IdIdioma = IdiomaId;
            idiomaColaborador.IdNivel = nivelId;
            idiomaColaborador.ColaboradorCpf = cpf;
            _repositoryColaboradorIdioma.AdicionarIdiomaColaborador(idiomaColaborador);
            var Idioma = _repositoryIdioma.GetIdiomaById(IdiomaId);
            idiomaColaborador.Idioma = new IdiomaDTO
            {
                Id = Idioma.Id,
                Descricao = Idioma.Descricao,
            };

            var _tokenSistema = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO);

            _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.INSERT, IdiomaId, nivelId, ItemCVEnum.IDIOMA);

            if (minhaJornada)
            {
                SkillsLog logSkill = new()
                {
                    CodigoInternoColaborador = cpf,
                    GestorExternoPerfil = gestorExternoPerfil,
                    SkillId = IdiomaId,
                    ItemPerfil = ItemPerfilEnum.IDIOMA,
                    NivelId = nivelId,
                    SkillsMovimentacaoId = EnumSkillsMovimentacao.ADICIONADO_PERFIL,
                    LogAutomatico = true,
                    CodigoInternoColaboradorLogado = cpf
                };

                _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
            }

            return idiomaColaborador;
        }

        public List<IdiomaColaboradorDTO> ListarIdiomaColaborador(string cpfColaborador)
        {
            var idiomaColaborador = new IdiomaColaboradorDTO();
            try
            {
                idiomaColaborador.ColaboradorCpf = cpfColaborador;
                return _repositoryColaboradorIdioma.ListarIdiomaColaborador(idiomaColaborador);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<NivelDTO> ListaNivelIdioma()
        {
            return _repositoryNivelIdioma.ListaNivelIdioma();
        }

        public List<int> ListarIdiomasAtribuidos(string cpfColaborador)
        {
            try
            {
                return _repositoryIdioma.ListarIdiomasAtribuidos(cpfColaborador);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IdiomaColaboradorResult AdicionarIdiomaColaboradorEmLote(List<AddIdiomaColabParam> param, string cpf, bool minhaJornada)
        {
            var ret = new IdiomaColaboradorResult();
            ret.Respostas = new List<ItemPerfilResult>();
            try
            {
                foreach (var item in param)
                {
                    var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(cpf);

                    if (item.IdiomaId.ToIntOuZero() == 0)
                    {
                        var competenciaAdicionada = AdicionarIdioma(item.Descricao);
                        item.IdiomaId = competenciaAdicionada.Id;
                    }
                    else
                    {
                        var competenciaValidacao = GetIdiomaById(item.IdiomaId);
                    }

                    var retService = AdicionarIdiomaColaborador(item.IdiomaId, item.NivelId, cpfRequest, item.GestorExternoPerfil, minhaJornada);

                    ret.Respostas.Add(new ItemPerfilResult
                    {
                        Sucesso = true,
                        ItemId = retService.IdIdioma
                    });
                }
                ret.Sucesso = true;

                return ret;
            }
            catch
            {
                throw;
            }
        }

        public ListaIdiomaResult ListarIdiomasNaoAtribuidos(string busca, int cursor, int limite, string cpfColaborador)
        {
            var retService = ListarIdioma(busca, cursor, limite);
            List<int> ids = ListarIdiomasAtribuidos(cpfColaborador);
            var ret = new List<ItemPerfilDTO>();
            ret = retService
            .Where(x => !ids.Contains(x.Id))
            .Select(x => new ItemPerfilDTO
            {
                Descricao = x.Descricao,
                Id = x.Id,
                Pendente = x.Pendente
            }).ToList();

            var result = new ListaIdiomaResult();

            result.Idioma = ret;
            result.Nivel = ListaNivelIdioma();

            return result;
        }

        public List<KeyValuePair<string, long>> GetIdiomaInfoByDescricao(List<string> idiomas)
        {
            var ret = new List<KeyValuePair<string, long>>();
            try
            {
                foreach (var idioma in idiomas)
                {
                    long idiomaId;
                    var strValue = StringUtil.RemoveDiacritics(idioma);
                    var idiomaAux = ListarIdioma(strValue, 0, 999).Where(x => StringUtil.RemoveDiacritics(x.Descricao).ToUpper().Equals(strValue, StringComparison.InvariantCultureIgnoreCase) == true);
                    if (!idiomaAux.Any())
                        idiomaId = AdicionarIdioma(idioma).Id;
                    else
                        idiomaId = idiomaAux.First().Id;
                    ret.Add(new KeyValuePair<string, long>(strValue, idiomaId));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ret;
        }
    }
}