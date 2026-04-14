using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using Competencia.Domain.Interfaces.Services.Metodologia;
using Core.DomainModel;
using Core.Domain.Colaborador;
using Core.Domain.Competencia.Metodologia;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Vaga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Competencia.Domain.Impl.Services.Metodologia
{
    [LogDomainClass]
    public class MetodologiasService : IMetodologiasService
    {
        private IMetodologiasRepository _metodologiaRepository;
        private readonly IColaboradorClient _colaboradorClient;
        private IHistoricoCVRepository _historicoCvRepository;
        private readonly IAspNetUser _aspNetUser;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;
        private readonly ICompetenciaDtoRepository _competenciaRepository;
        private readonly ISkillGenericService _skillGenericService;

        public MetodologiasService(IMetodologiasRepository metodologiaRepository, IColaboradorClient colaboradorClient, IHistoricoCVRepository historicoCvRepository, IAspNetUser aspNetUser, IVerificaSeCpfESistemico verificaCpfSistemico
            , ICompetenciaDtoRepository competenciaRepository, ISkillGenericService skillGenericService)
        {
            _colaboradorClient = colaboradorClient;
            _metodologiaRepository = metodologiaRepository;
            _historicoCvRepository = historicoCvRepository;
            _aspNetUser = aspNetUser;
            _verificaCpfSistemico = verificaCpfSistemico;
            _competenciaRepository = competenciaRepository;
            _skillGenericService = skillGenericService;
        }

        public async Task<ApiGenericResult<List<CompetenciaGroupDTO>>> GetGroupMetodologiaByIds(List<long> ids)
        {
            var resultado = new ApiGenericResult<List<CompetenciaGroupDTO>>();
            if (ids is null || ids.Count == 0)
                return resultado;

            // enviado tipo 3 - metodologia do GCOLB
            var encontradas = await _competenciaRepository.GetCompetenciasByTypeAndGroupId((int)ItemPerfilEnum.METODOLOGIA, ids.Distinct().ToList());

            resultado.Retorno = encontradas;
            return resultado;
        }

        public List<long> ListarMetodologiasAtribuidas(string cpf)
        {
            try
            {
                return _metodologiaRepository.ListarMetodologiasAtribuidas(cpf);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ListaMetodologiaResult ListarMetodologiasNaoAtribuidas(string busca, int cursor, int limite, string cpf)
        {
            try
            {
                var listaResult = new ListaMetodologiaResult();

                List<long> idsMetodologiasAtribuidas = _metodologiaRepository.ListarMetodologiasAtribuidas(cpf);

                var metodologias = _metodologiaRepository.ListarMetodologias(busca, cursor, limite);

                if (idsMetodologiasAtribuidas.Count > 0)
                {
                    metodologias = metodologias.Where(x => !idsMetodologiasAtribuidas.Contains(x.Id)).ToList();
                }

                listaResult.Metodologia = metodologias.Select(x => new ItemPerfilDTO
                {
                    Descricao = x.Descricao,
                    Id = x.Id,
                    Pendente = x.Pendente
                }).ToList();

                listaResult.Nivel = ListarNivelMetodologia();

                return listaResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<MetodologiaDTO> ListarMetodologias(string busca, int cursor, int limite)
        {
            try
            {
                return _metodologiaRepository.ListarMetodologias(busca, cursor, limite);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public MetodologiaDTO ListarMetodologiasById(int codMetodologia)
        {
            try
            {
                return _metodologiaRepository.ListarMetodologiasById(codMetodologia);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<NivelDTO> ListarNivelMetodologia()
        {
            try
            {
                return _metodologiaRepository.ListarNivelMetodologia();
            }
            catch (Exception)
            {
                throw new Exception("Erro ao listar os níveis para metodologia.");
            }
        }
        public List<ListaMetodologiaColaboradorResult> ListarMetodologiasColaborador(string cpf)
        {
            try
            {
                return _metodologiaRepository.ListarMetodologiasColaborador(cpf);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MetodologiaColaboradorResult InserirMetodologiaColaborador(List<MetodologiaDTO> listaMetodologias, string cpf, string token, bool minhaJornada, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            var ret = new MetodologiaColaboradorResult();
            ret.Respostas = new List<ItemPerfilResult>();

            long usuarioId = _metodologiaRepository.ObterIdUsuarioPorCpf(cpf);
            InserirMetodologiaColaborador(listaMetodologias, cpf, token, origem, ret, usuarioId, minhaJornada);

            return ret;
        }
        public MetodologiaColaboradorResult InserirMetodologiaColaboradorLote(List<MetodologiaDTO> listaMetodologias, string cpf, string token, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            var ret = new MetodologiaColaboradorResult();
            ret.Respostas = new List<ItemPerfilResult>();

            long usuarioId = _metodologiaRepository.ObterIdUsuarioPorCpf(ApiClient.Domain.ClientConfig.Clients.Colaborador.CpfAdmin);
            InserirMetodologiaColaborador(listaMetodologias, cpf, token, origem, ret, usuarioId, false);

            return ret;
        }

        private void InserirMetodologiaColaborador(List<MetodologiaDTO> listaMetodologias, string cpf, string token, OrigemAlteracaoCVEnum origem, MetodologiaColaboradorResult ret, long usuarioId, bool minhaJornada)
        {
            foreach (var metodologia in listaMetodologias)
            {
                if (metodologia.Id == 0)
                {
                    if (!MetodologiaExiste(metodologia.Descricao) && metodologia.NivelId > 0)
                    {
                        metodologia.UsuarioCriacaoId = usuarioId != 0 ? usuarioId : -1;
                        metodologia.Pendente = false; // 0 = está pendente aprovação/confirmação
                        metodologia.Ativo = true;
                        var novoId = _metodologiaRepository.InserirMetodologiaColaborador(metodologia, cpf);

                        ret.Respostas.Add(new ItemPerfilResult()
                        {
                            ItemId = novoId,
                            Sucesso = true
                        });
                    }
                    else
                    {
                        if (metodologia.NivelId > 0)
                        {
                            throw new ArgumentException("Metodologia já existe.");
                        }
                        throw new ArgumentException("Selecione um nível.");
                    }
                }
                else if (!MetodologiaAssociadaComColaborador(metodologia.Id, cpf))
                {
                    metodologia.UsuarioCriacaoId = usuarioId != 0 ? usuarioId : -1;
                    metodologia.Ativo = true;
                    _metodologiaRepository.AssociaMetodologiaComColaborador(metodologia, cpf);
                    ret.Respostas.Add(new ItemPerfilResult()
                    {
                        ItemId = metodologia.Id,
                        Sucesso = true
                    });
                }
                else
                {
                    throw new ArgumentException("Metodologia já vinculado ao colaborador.");
                }

                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.INSERT, metodologia.Id, metodologia.NivelId, ItemCVEnum.METODOLOGIA);

                if (minhaJornada)
                {
                    SkillsLog logSkill = new()
                    {
                        CodigoInternoColaborador = cpf,
                        GestorExternoPerfil = metodologia.GestorExternoPerfil,
                        SkillId = metodologia.Id,
                        ItemPerfil = ItemPerfilEnum.METODOLOGIA,
                        NivelId = metodologia.NivelId,
                        SkillsMovimentacaoId = EnumSkillsMovimentacao.ADICIONADO_PERFIL,
                        LogAutomatico = true,
                        CodigoInternoColaboradorLogado = cpf
                    };

                    _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
                }
            }
        }

        public bool AdicionarMetodologia(List<MetodologiaDTO> listaMetodologias, string cpf, string token, bool importacaoLote = false)
        {
            try
            {
                long usuarioId = _metodologiaRepository.ObterIdUsuarioPorCpf(cpf);

                foreach (var metodologia in listaMetodologias)
                {
                    if (!MetodologiaExiste(metodologia.Descricao))
                    {
                        metodologia.UsuarioCriacaoId = usuarioId != 0 ? usuarioId : -1;
                        metodologia.Pendente = false; // 0 = está pendente aprovação/confirmação
                        metodologia.Ativo = true;
                        _metodologiaRepository.AdicionarMetodologia(metodologia, cpf);

                        _colaboradorClient.EnviaPushNotificationRedeColaborador(cpf, "Nova Metodologia", " adicionou a metodologia " + metodologia.Descricao, token);
                        return true;
                    }
                    else
                    {
                        throw new InvalidOperationException("Metodologia já existe.");
                    }
                }

                throw new InvalidOperationException("Metodologia já existe.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long AdicionarMetodologia(string descricao, string cpf, bool importacaoLote = false)
        {
            try
            {
                var cpfRequest = String.Empty;
                if (importacaoLote)
                    cpfRequest = ApiClient.Domain.ClientConfig.Clients.Colaborador.CpfAdmin;
                else
                    cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(cpf);

                var usuarioId = _metodologiaRepository.ObterIdUsuarioPorCpf(cpfRequest);

                if (!MetodologiaExiste(descricao))
                {
                    var metodologia = new MetodologiaDTO();
                    metodologia.Descricao = descricao;
                    metodologia.UsuarioCriacaoId = usuarioId != 0 ? usuarioId : -1;
                    metodologia.Pendente = false; // 0 = está pendente aprovação/confirmação
                    metodologia.Ativo = true;
                    return _metodologiaRepository.AdicionarMetodologia(metodologia, cpf);
                }
                else
                {
                    throw new InvalidOperationException("Metodologia já existe.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void AtualizarNivelMetodologiaColaborador(MetodologiaDTO metodologia, string cpf, bool minhaJornada, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                if (metodologia.Id != 0 && metodologia.NivelId > 0)
                {
                    _metodologiaRepository.AtualizarNivelMetodologiaColaborador(metodologia, cpf);
                    _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.UPDATE, metodologia.Id, metodologia.NivelId, ItemCVEnum.METODOLOGIA);

                    if (minhaJornada)
                    {
                        SkillsLog logSkill = new()
                        {
                            CodigoInternoColaborador = cpf,
                            GestorExternoPerfil = metodologia.GestorExternoPerfil,
                            SkillId = metodologia.Id,
                            ItemPerfil = ItemPerfilEnum.METODOLOGIA,
                            NivelId = metodologia.NivelId,
                            SkillsMovimentacaoId = EnumSkillsMovimentacao.ATUALIZADO,
                            LogAutomatico = true,
                            CodigoInternoColaboradorLogado = cpf
                        };

                        _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
                    }
                }
                else
                {
                    if (metodologia.NivelId > 0)
                    {
                        throw new InvalidOperationException("Informe um Id válido");
                    }
                    throw new InvalidOperationException("Selecione um nível.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoverMetodologiaColaborador(int codMetodologia, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                if (codMetodologia != 0)
                {
                    _metodologiaRepository.RemoverMetodologiaColaborador(codMetodologia, cpf);
                    _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.DELETE, codMetodologia, null, ItemCVEnum.METODOLOGIA);
                }
                else
                {
                    throw new InvalidOperationException("Informe um Id válido");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool MetodologiaExiste(string descricao)
        {
            if (!string.IsNullOrEmpty(descricao) && descricao != "string")
            {
                return _metodologiaRepository.MetodologiaExiste(descricao);
            }
            throw new InvalidOperationException("Descrição inválida ou vazia.");
        }

        public bool MetodologiaAssociadaComColaborador(long idMetodologia, string cpf)
        {
            return _metodologiaRepository.IsAssociadaComColaborador(idMetodologia, cpf);
        }

        public async Task<List<KeyValuePair<string, long>>> GetMetodologiaInfoByDescricao(List<string> metodologias)
        {
            return await _skillGenericService.GetSkillInfoByDescricaoAsync(metodologias, TipoSkillEnum.METODOLOGIA, _aspNetUser.GetUsuarioLogado().Cpf);
        }
    }
}