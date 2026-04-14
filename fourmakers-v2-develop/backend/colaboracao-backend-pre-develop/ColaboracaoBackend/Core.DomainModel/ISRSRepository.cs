using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.VagasSRS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain
{
    public interface ISRSRepository
    {
        List<SRSDTO> BuscarVagas(string busca, int cursor, int limite, FiltroStatusPublicacaoEnum? filtroStatusPublicacao, int orgId);

        List<DetalheDTO> BuscarVagaDetalhada(long id_vaga);
        List<SkillsVagasDTO> BuscarSkills(List<long> id);
        List<FavoritarVagasDTO> FavoritarVagas(long id_vaga, long tb_usuario_id);
        List<FavoritarVagasDTO> ListarVagasFavoritadasPorUsuario(long tb_usuario_id);
        DesfavoritarResult DesfavoritarVagas(long id_vaga, long tb_usuario_id);
        List<RecomendarVagasDTO> ListarVagasRecomendadas(List<SkillsDTO> skills);
        List<SkillsDTO> GetSkillsUsuario(string cpfSolicitante);
        long BuscaQuatidadeVagas();
        List<VagaIndicadaDTO> ListarVagasIndicadas(string cpf, int orgId);
        void InserirVagaIndicada(long idVaga, DateTime dataCriacao, string urlLinkedin, string nomeCandidato, string emailUsuarioIndicou, string link, int orgId);
        bool CandidatoIndicadoVaga(long idVaga, string urlLinkedin);
        void RemoverVagaIndicada(long joborder_id, string cpf);
        long IndicarVaga(long usuarioid, long vagaId, string nome, string email, string telefone, string deOndeConhece, bool autorizou, bool estaDisponivel, string linkedin);
        void ValidarIndicacaoParcial(long idIndicacaoParc);
        void AddConviteIndicacaoParcial(long idTb, string convite, string pathCurriculo);
        //Task CriarOuAtualizarOrdemDeTrabalho(List<VagaDTO> vagas);
        bool ValidaAcessoAdministrador(string token);
        Task<IEnumerable<VagaOrquestracaoDTO>> ListarVagaOrquestracao(string email);
        Task<ActionResult<IEnumerable<CanditatoVagaSrsDTO>>> ListarCandidatosPorVaga(long? idVaga);
        Task<IEnumerable<TotalizadoresVagaDTO>> TotalizadoresVaga(long? idVaga);
        Task<bool> VerificarVagaExistente(long? idVaga);
        Task<List<IndicacaoPremiadaParcialDTO>> ListarIndicacoesPremiadas(ListarIndicacaoPremiadaParcialParam param, int orgId);
        Task<IndicacaoPremiadaParcialDTO> EditarIndicacaoPremiadaParcial(EditarIndicacaoPremiadaParcialParam param, int orgId);
        Task<bool> VerificaSeExisteIndicacaoPremiadaParcial(int id);
        Task<IndicacaoPremiadaParcialDTO> BuscarIndicacaoPremiadaPorId(int id, int orgId);
        Task InserirVagaCandidato(string CodInternoColaborador, long vagaId, int orgId, int candidatoId, int status);
        Task RemoverVagaCandidato(int candidateId, long vagaId);
    }
}