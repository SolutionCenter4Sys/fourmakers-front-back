using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using Competencia.Domain.Utils;
using Core.DomainModel.Competencia;
using System;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Competencia.Domain.Impl.Services
{
    [LogDomainClass]
    public class CompetenciaHistoricoService : ICompetenciaHistoricoService
    {
        private readonly ICompetenciaHistoricoRepository _competenciaHistoricoRepository;

        public CompetenciaHistoricoService(ICompetenciaHistoricoRepository competenciaHistoricoRepository)
        {
            _competenciaHistoricoRepository = competenciaHistoricoRepository;
        }

        private StatusHistoricoCompetenciaEnum ReturnStatusHistoricoCompetenciaPorAcao(AcaoHistoricoCompetenciaEnum acao)
        {
            if (acao == AcaoHistoricoCompetenciaEnum.Aprovar)
            {
                return StatusHistoricoCompetenciaEnum.Aprovada;
            }
            else if (acao == AcaoHistoricoCompetenciaEnum.Unificar)
            {
                return StatusHistoricoCompetenciaEnum.Unificada;
            }
            else if (acao == AcaoHistoricoCompetenciaEnum.Adicionar)
            {
                return StatusHistoricoCompetenciaEnum.Adicionada;
            }
            else if (acao == AcaoHistoricoCompetenciaEnum.Reprovar)
            {
                return StatusHistoricoCompetenciaEnum.Reprovada;
            }
            else if (acao == AcaoHistoricoCompetenciaEnum.Editar)
            {
                return StatusHistoricoCompetenciaEnum.Editada;
            }
            throw new ArgumentException("Ação do histórico de competências não encontrada.");
        }

        public bool InserirHistoricoCompetenciaAcaoUnificar(AcaoHistoricoCompetenciaEnum acao, TipoCompetenciaSRSEnum tipoCompetencia, string descricaoCompetencia1, string descricaoCompetencia2, string cpfUsuarioCriacao)
        {
            if (String.IsNullOrEmpty(descricaoCompetencia1) || String.IsNullOrEmpty(descricaoCompetencia2))
            {
                throw new ArgumentException("Ambas as descrições são obrigatórias.");
            }
            var situacao = ReturnStatusHistoricoCompetenciaPorAcao(acao);
            _competenciaHistoricoRepository.InserirHistoricoCompetencia(
                tipoCompetencia.ToString(),
                descricaoCompetencia1,
                situacao.ToString(),
                ValidacaoCompetenciaUtil.RetornarLabelObservacaoHistoricoCompetencia(acao, descricaoCompetencia1, descricaoCompetencia2),
                cpfUsuarioCriacao
            );
            return true;
        }

        public bool InserirHistoricoCompetencia(AcaoHistoricoCompetenciaEnum acao, TipoCompetenciaSRSEnum tipoCompetencia, string descricaoCompetencia, string cpfUsuarioCriacao)
        {
            if (String.IsNullOrEmpty(descricaoCompetencia))
            {
                throw new ArgumentException("A descrição da skill é obrigatória.");
            }
            var situacao = ReturnStatusHistoricoCompetenciaPorAcao(acao);
            _competenciaHistoricoRepository.InserirHistoricoCompetencia(
                tipoCompetencia.ToString(),
                descricaoCompetencia,
                situacao.ToString(),
                ValidacaoCompetenciaUtil.RetornarLabelObservacaoHistoricoCompetencia(acao, descricaoCompetencia, null),
                cpfUsuarioCriacao
            );
            return true;
        }
        public async Task<bool> EditarHistoricoCompetencia(AcaoHistoricoCompetenciaEnum acao, TipoCompetenciaSRSEnum tipoCompetencia, string descricaoCompetencia1, string descricaoCompetencia2, string cpfUsuarioCriacao)
        {
            if (String.IsNullOrEmpty(descricaoCompetencia1) || String.IsNullOrEmpty(descricaoCompetencia2))
            {
                throw new ArgumentException("Ambas as descrições são obrigatórias.");
            }
            if (descricaoCompetencia1.ToUpper().Equals(descricaoCompetencia2.ToUpper()))
            {
                descricaoCompetencia2 = null;
            }
            var situacao = ReturnStatusHistoricoCompetenciaPorAcao(acao);
            return await _competenciaHistoricoRepository.InserirHistoricoCompetencia(
                tipoCompetencia.ToString(),
                descricaoCompetencia1,
                situacao.ToString(),
                ValidacaoCompetenciaUtil.RetornarLabelObservacaoHistoricoCompetencia(acao, descricaoCompetencia1, descricaoCompetencia2),
                cpfUsuarioCriacao
            );
        }
    }
}