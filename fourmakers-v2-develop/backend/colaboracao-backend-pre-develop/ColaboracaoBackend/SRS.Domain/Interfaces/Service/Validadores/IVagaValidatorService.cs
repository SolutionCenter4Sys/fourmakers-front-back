using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Vaga;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service.Validadores
{
    public interface IVagaValidatorService
    {
        Task ValidaVaga(int vagaId, VagaFourmakersDTO vaga, int orgId, CRUDEnum cRUDEnum);
        Task ValidarCamposEnvioCandidatosAderencia(RecomendarCandidatosParam param);
        Task ValidarDatas(string dataInicio, string dataFim);
        Task ValidarCadastroVagaRecrutamento(InserirVagaRecrutamentoDTO inserirVagaDTO);
        Task ValidarAtualizacaoVagaRecrutamento(VagaVindaDeGestorExternoPerfil atualizarVagaDTO);
        Task ValidaMudancaVagaParaEmRefinamento(VagaRecrutamentoDTO vaga);
        Task<List<string>> ValidarAcessoEListarClientesPermitidos(string cpf, int orgId);
        Task ValidarCandidatarSeRecrutamento(CandidatarSeRecrutamentoParam param);
    }
}