using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using Core.DomainModel;
using Core.Domain.Colaborador;
using Core.DomainModel.Softskill;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Softskill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Competencia.Domain.Impl.Services
{
    [LogDomainClass]
    public class SoftskillService : ISoftskillService
    {
        private const string DESCRICAO_SOFTSKILL = "SOFTSKILL";
        private readonly ILogCore _logCore;
        private readonly IColaboradorClient _colaboradorClient;
        private readonly IFirebaseClient _firebaseClient;
        private readonly ISoftskillRepository _softskillRepository;
        private readonly ISoftskillColaboradorRepository _softskillColaboradorRepository;
        private readonly ISoftskillNivelRepository _softskillNivelRepository;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;
        private IHistoricoCVRepository _historicoCvRepository;
        private readonly IAspNetUser _aspNetUser;
        private readonly ICompetenciaDtoRepository _competenciaRepository;
        private readonly ISkillGenericService _skillGenericService;

        public SoftskillService(IColaboradorClient colaboradorClient,
                                IFirebaseClient firebaseClient,
                               ISoftskillRepository softskillRepository,
                                IVerificaSeCpfESistemico verificaCpfSistemico,
                                ISoftskillColaboradorRepository softskillColaboradorRepository,
                                ISoftskillNivelRepository softskillNivelRepository,
                                IHistoricoCVRepository historicoCvRepository,
                                IAspNetUser aspNetUser, ICompetenciaDtoRepository competenciaRepository, ISkillGenericService skillGenericService)
        {
            _colaboradorClient = colaboradorClient;
            _firebaseClient = firebaseClient;
            _softskillRepository = softskillRepository;
            _verificaCpfSistemico = verificaCpfSistemico;
            _softskillColaboradorRepository = softskillColaboradorRepository;
            _softskillNivelRepository = softskillNivelRepository;
            _historicoCvRepository = historicoCvRepository;
            _aspNetUser = aspNetUser;
            _competenciaRepository = competenciaRepository;
            _skillGenericService = skillGenericService;
        }
        public async Task<ApiGenericResult<List<CompetenciaGroupDTO>>> GetGroupSoftSkillByIds(List<long> ids)
        {
            var resultado = new ApiGenericResult<List<CompetenciaGroupDTO>>();
            if (ids is null || ids.Count == 0)
                return resultado;

            // enviado tipo 8 - softskill do GCOLB
            var encontradas = await _competenciaRepository.GetCompetenciasByTypeAndGroupId((int)ItemPerfilEnum.SOFTSKILL, ids.Distinct().ToList());

            resultado.Retorno = encontradas;
            return resultado;
        }
        public SoftskillDTO AdicionarSoftSkill(string descricao, string cpfRequest, bool processamentoLote = false)
        {
            try
            {
                var cpf = "";
                if(processamentoLote)
                {
                    cpf = cpfRequest;
                }
                else
                {
                    cpf = _verificaCpfSistemico.VerificaCpfSistemico(cpfRequest);
                }
                var idUser = _softskillRepository.BuscarIdUsuarioPorCpf(cpf);
                var softskill = new SoftskillDTO
                {
                    Descricao = descricao,
                    UsuarioCriacaoId = idUser.UsuarioCriacaoId
                };
                softskill = _softskillRepository.AdicionarSoftSkill(softskill);
                return softskill;
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

        public List<SoftskillDTO> ListarSoftSkill(string busca, int cursor, int limite)
        {
            try
            {
                return _softskillRepository.ListarSoftSkill(busca, cursor, limite);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ListaSoftskillResult ListarSoftSkillsNaoAtribuidas(string busca, int cursor, int limite, string cpfRequest)
        {
            try
            {
                var competenciaResult = new ListaSoftskillResult();

                List<long> ids = ListarSoftSkillsAtribuidas(cpfRequest);

                var listaSoftSkill = _softskillRepository.ListarSoftSkill(busca, cursor, limite);

                var listaItemPerfil = listaSoftSkill.Where(x => !ids.Contains(x.Id))
                .Select(x => new ItemPerfilDTO
                {
                    Descricao = x.Descricao,
                    Id = x.Id,
                    Pendente = x.Pendente,
                }).ToList();

                competenciaResult.SoftSkill = listaItemPerfil;
                competenciaResult.Nivel = ListarNivelSoftskill();

                return competenciaResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SoftskillDTO GetSoftskillById(long id)
        {
            try
            {
                return _softskillRepository.GetSoftskillById(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SoftskillColaboradorDTO> ListarSoftskillColaborador(string cpfColaborador)
        {
            try
            {
                return _softskillColaboradorRepository.ListarSoftskillColaborador(cpfColaborador);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SoftskillColaboradorDTO AddSoftskillColaborador(long softskillId, long? nivelId, string cpf, string token, bool minhaJornada, string gestorExternoPerfil, string usuarioLogado, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var softSkillColaborador = new SoftskillColaboradorDTO();
                softSkillColaborador.IdSoftSkill = softskillId;
                softSkillColaborador.IdNivel = nivelId;
                softSkillColaborador.ColaboradorCpf = cpf;
                softSkillColaborador = _softskillColaboradorRepository.AddSoftskillColaborador(softSkillColaborador);

                var softskill = _softskillRepository.GetSoftskillById(softskillId);

                softSkillColaborador.SoftSkill = new ItemPerfilDTO
                {
                    Id = softskill.Id,
                    Descricao = softskill.Descricao,
                };

                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.INSERT, softskillId, nivelId, ItemCVEnum.SOFTSKILL);

                if (minhaJornada)
                {
                    SkillsLog logSkill = new()
                    {
                        CodigoInternoColaborador = cpf,
                        GestorExternoPerfil = gestorExternoPerfil,
                        SkillId = softskillId,
                        ItemPerfil = ItemPerfilEnum.SOFTSKILL,
                        NivelId = nivelId,
                        SkillsMovimentacaoId = EnumSkillsMovimentacao.ADICIONADO_PERFIL,
                        LogAutomatico = true,
                        CodigoInternoColaboradorLogado = usuarioLogado
                    };

                    _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
                }

                return softSkillColaborador;
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

        public List<ItemPerfilResult> AdicionarSoftskillColaboradorEmLote(List<AddSoftskillColabParam> param, string token, bool minhaJornada, string usuarioLogado)
        {
            var ret = new List<ItemPerfilResult>();
            try
            {
                foreach (var item in param)
                {
                    try
                    {
                        var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(item.Cpf);

                        if (item.SoftskillId.ToIntOuZero() == 0)
                        {
                            var competenciaAdicionada = AdicionarSoftSkill(item.Descricao, cpfRequest);
                            item.SoftskillId = competenciaAdicionada.Id;
                        }
                        else
                        {
                            var competenciaValidacao = GetSoftskillById(item.SoftskillId);
                        }
                        var retService = AddSoftskillColaborador(item.SoftskillId, item.NivelId, cpfRequest, token, minhaJornada, item.GestorExternoPerfil, usuarioLogado);
                        ret.Add(new ItemPerfilResult
                        {
                            Sucesso = true,
                            ItemId = retService.IdSoftSkill
                        });
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SoftskillColaboradorDTO RemoveSoftskillColaborador(long softskillId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var softSkillColaborador = new SoftskillColaboradorDTO();
                softSkillColaborador.ColaboradorCpf = cpf;
                softSkillColaborador.IdSoftSkill = softskillId;
                _softskillColaboradorRepository.RemoveSoftskillColaborador(softSkillColaborador);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.DELETE, softskillId, null, ItemCVEnum.SOFTSKILL);
                return softSkillColaborador;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<NivelDTO> ListarNivelSoftskill()
        {
            try
            {
                var itemPerfilId = _softskillNivelRepository.BuscarIdItemPerfilPorDescricao(DESCRICAO_SOFTSKILL);
                var ret = _softskillNivelRepository.ListarNivelSoftskill(itemPerfilId);

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SoftskillColaboradorDTO AlterarSoftskillColaborador(string cpf, long id, long? nivelId, string gestorExternoPerfil, bool minhaJornada, string usuarioLogado, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var softSkillColaborador = new SoftskillColaboradorDTO();
                softSkillColaborador.ColaboradorCpf = cpf;
                softSkillColaborador.SoftSkill.Id = id;
                softSkillColaborador.Nivel.Id = nivelId;
                _softskillColaboradorRepository.AlterarSoftskillColaborador(softSkillColaborador);

                var softskill = _softskillRepository.GetSoftskillById(id);
                softSkillColaborador.SoftSkill = new ItemPerfilDTO
                {
                    Id = softskill.Id,
                    Descricao = softskill.Descricao,
                };

                if (minhaJornada)
                {
                    SkillsLog logSkill = new()
                    {
                        CodigoInternoColaborador = cpf,
                        GestorExternoPerfil = gestorExternoPerfil,
                        SkillId = id,
                        ItemPerfil = ItemPerfilEnum.SOFTSKILL,
                        NivelId = nivelId,
                        SkillsMovimentacaoId = EnumSkillsMovimentacao.ATUALIZADO,
                        LogAutomatico = true,
                        CodigoInternoColaboradorLogado = usuarioLogado
                    };

                    _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
                }

                //_colaboradorClient.EnviaPushNotificationRedeColaborador(softSkillColaborador.SoftskillColaboradorDTO.ColaboradorCpf, "Soft Skill atualizada!", " atualizou a Soft Skill " + softSkillColaborador.SoftskillColaboradorDTO.SoftSkill.Descricao, _aspNetUser.GetUsuarioLogado().Token);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.UPDATE, id, nivelId, ItemCVEnum.SOFTSKILL);
                return softSkillColaborador;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<long> ListarSoftSkillsAtribuidas(string cpfColaborador)
        {
            try
            {
                return _softskillRepository.ListarSoftskillsAtribuidas(cpfColaborador);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<KeyValuePair<string, long>>> GetSoftSkillInfoByDescricao(List<string> softSkills)
        {
            return await _skillGenericService.GetSkillInfoByDescricaoAsync(softSkills, TipoSkillEnum.SOFTSKILL, _aspNetUser.GetUsuarioLogado().Cpf);
        }
    }
}