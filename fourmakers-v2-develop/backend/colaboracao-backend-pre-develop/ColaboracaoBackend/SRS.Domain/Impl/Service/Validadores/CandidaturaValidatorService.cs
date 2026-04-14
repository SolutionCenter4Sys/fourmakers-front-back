using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.DomainModel;
using DataTransferObject.Domain.Vaga;
using SRS.Domain.Interfaces.Service.Validadores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logs.Infra.Attributes;

namespace SRS.Domain.Impl.Service.Validadores
{
    [LogDomainClass]
    public class CandidaturaValidatorService : ICandidaturaValidatorService
    {
        private readonly ICandidaturaRepository _candidaturaRepository;

        public CandidaturaValidatorService(ICandidaturaRepository candidaturaRepository)
        {
            _candidaturaRepository = candidaturaRepository;
        }

        public async Task ValidaCandidaturaStatus(int vagaId, int candidatoId, int statusId, string descricao, OrigemVagaEnum origem, CRUDEnum cRUDEnum)
        {
            if (cRUDEnum == CRUDEnum.Create || cRUDEnum == CRUDEnum.Update)
            {
                await ValidaSeExisteCandidatura(vagaId, candidatoId);
                await ValidarCamposDeEntrada(vagaId, candidatoId, statusId, descricao, origem, cRUDEnum);
            }
        }
        private async Task ValidarCamposDeEntrada(int vagaId, int candidatoId, int statusId, string descricao, OrigemVagaEnum origem, CRUDEnum cRUDEnum)
        {
            var campos = new List<CampoValidacao>();

            if (cRUDEnum == CRUDEnum.Update)
            {
                campos.Add(new("VagaId", vagaId, TipoValidacaoEnum.Obrigatoriedade));
            }
            ;

            campos.Add(new("CandidatoId", candidatoId, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("StatusId", statusId, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Descricao", descricao, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Origem", origem, TipoValidacaoEnum.Obrigatoriedade));

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        private async Task ValidaSeExisteCandidatura(int vagaId, int candidatoId)
        {
            var candidatura = await _candidaturaRepository.ExisteStatusCandidatura(vagaId, candidatoId);

            if (candidatura == 0)
            {
                throw new ApplicationException($"Candidatura não encontrada.");
            }
        }
    }
}