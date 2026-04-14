using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using Core.DomainModel;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Interesse;
using System;
using System.Collections.Generic;

using Logs.Infra.Attributes;

namespace Competencia.Domain.Impl.Services
{
    [LogDomainClass]
    public class InteresseService : IInteresseService
    {
        private readonly IInteresseDtoRepository _interesseRepository;
        private readonly IInteresseColaboradorDtoRepository _interesseColaboradorRepository;
        private readonly IAspNetUser _aspNetUser;
        private readonly ICompetenciaDtoRepository _competenciaRepository;

        public InteresseService(IInteresseDtoRepository interesseRepository, IInteresseColaboradorDtoRepository interesseColaboradorRepository,
            IAspNetUser aspNetUser, ICompetenciaDtoRepository competenciaRepository)
        {
            _interesseRepository = interesseRepository;
            _interesseColaboradorRepository = interesseColaboradorRepository;
            _aspNetUser = aspNetUser;
            _competenciaRepository = competenciaRepository;
        }

        public InteresseDTO InserirInteresse(string descricao)
        {
            var idUsuario = _interesseRepository.GetUsuarioCriacaoIdByCpf(_aspNetUser.GetUsuarioLogado().Cpf);
            return _interesseRepository.SaveInteresse(descricao, idUsuario);
        }

        public List<InteresseDTO> ListarInteresses(string busca, int cursor, int limite)
        {
            return _interesseRepository.ListInteresses(busca, cursor, limite);
        }

        public InteresseDTO GetInteressesById(long id)
        {
            return _interesseRepository.GetInteresseById(id);
        }

        public InteresseColaboradorDTO InserirInteresseColaborador(long id, string cpf, bool interesseAtivo, int tipoId, int skillId, bool minhaJornada, string gestorExternoPerfil, long nivelId, string usuarioLogado)
        {
            var ret = _interesseColaboradorRepository.SaveInteresseColaboradorDapper(id, cpf, tipoId, skillId, interesseAtivo);

            if (minhaJornada)
            {
                SkillsLog logSkill = new()
                {
                    CodigoInternoColaborador = cpf,
                    GestorExternoPerfil = gestorExternoPerfil,
                    SkillId = skillId,
                    ItemPerfil = Enum.IsDefined(typeof(ItemPerfilEnum), tipoId) ? (ItemPerfilEnum)tipoId : ItemPerfilEnum.DESCONHECIDO,
                    NivelId = nivelId,
                    SkillsMovimentacaoId = interesseAtivo == true ? EnumSkillsMovimentacao.INTERESSADO : EnumSkillsMovimentacao.NAO_INTERESSADO,
                    LogAutomatico = true,
                    CodigoInternoColaboradorLogado = usuarioLogado
                };

                _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
            }

            return ret;
        }

        public List<InteresseColaboradorDTO> ListarInteressesColaborador(string cpfColaborador)
        {
            return _interesseColaboradorRepository.ListInteressesColaborador(cpfColaborador);
        }

        public void RemoverInteresseColaborador(long id, string cpf)
        {
            _interesseColaboradorRepository.RemoveInteresseColaborador(id, cpf);
        }
    }
}
