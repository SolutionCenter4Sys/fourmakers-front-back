using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.VagasSRS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service
{
    public interface ISRSService
    {
        SRSResult Buscarvagas(string busca, int cursor, int limite, FiltroStatusPublicacaoEnum filtroStatusPublicacao);

        SkillsVagasResult BuscarSkillsVagas(List<long> id);

        Task<SRSResult> BuscarRazao();

        DetalheResult BuscarVagaDetalhada(long id_vaga);

        Task<InscricoesCandidatoResult> CandidatoInscritoNasVagas();

        Task<CandidatarResult> CandidatarSe(long joborder_id, string convite, string origem = "4Makers", int? candidateId = 0, int? userId = null);

        //Task<DescandidatarResult> DescandidatarSe(long joborder_id, int status_reason_cancellation, int reason_cancellation);
        Task<DescandidatarResult> DescandidatarSe(long joborder_id, int status_reason_cancellation, int reason_cancellation);

        FavoritarVagasResult FavoritarVagas(long id_vaga, long tb_usuario_id);

        DesfavoritarResult DesfavoritarVagas(long id_vaga, long tb_usuario_id);
        FavoritarVagasResult ListarVagasFavoritadas(long tb_usuario_id);
        List<RecomendarVagasDTO> ListarVagasRecomendadas(string cpfSolicitante);
        long BuscaQuantidadeVaga();
        VagasIndicadasResult ListarVagasIndicadas();
        Task<string> GerarConvite(long vagaId, long idTbIndicacaoParc, string telefone, string nome, string linkedin, UsuarioColaboradorDTO usuario);
        string[] DesembrulharConvite(string convite);
        Task<IdIndicacaoUsuario> IndicarVaga(long idVaga, string nome, string email, string telefone, string deOndeConhece, bool autorizou, bool estaDisponivel, string linkedin);
        Task AddConviteIndicacaoParcial(long idTb, string convite, byte[] curriculo, TipoCertificadoEnum tipo);
        Task<List<VagaDTO>> BuscarVagaSRS(string token);
        Task<CriarVagasSRSParam> EditarCriarVagasFourmakersSRS(CriarVagasSRSParam param, string token);
        Task<SkillVagaParam> EditarSkillVagaFourmakersSRS(SkillVagaParam param);
        Task<SkillVagaParam> InserirSkillVagaFourmakersSRS(SkillVagaParam param);
        Task<SkillVagaParam> RemoverSkillVagaFourmakersSRS(int skillId);
        Task<List<SkillVagaParam>> BuscaSkillsVagaFourmakersSRS(int vagaId);
        Task<bool> GetColaboradorAtivoOuInativoPorCPF(string colaboradorCPF, string token);
        Task<bool> AlterarCategoriaHabilidade(SRSAlterarCategoriaHabilidadeParam param);
        Task<ValidarUsuarioLinkedinResponse> ValidarUsuarioLinkedinService(int? userId, string? userName);
        Task<List<IndicacaoPremiadaParcialDTO>> ListarIndicacoesPremiadas(ListarIndicacaoPremiadaParcialParam param);
        Task<IndicacaoPremiadaParcialDTO> EditarIndicacaoPremiadaParcial(EditarIndicacaoPremiadaParcialParam param);
        Task<IndicacaoPremiadaParcialDTO> BuscarIndicacaoPremiadaPorId(int id);
        Task<CriarVagasSRSParam> ObterVagaPorId(string token, int vagaId);
        Task<List<JobOrderDTO>> BuscarVagasDoUltimoAno(string token);
    }
}