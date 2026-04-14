using Competencia.API.DTOs;
using Competencia.Domain.Enums;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Foursys;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia.MapaCompetencia;

namespace Competencia.Domain.Interfaces.Services
{
    public interface ICompetenciaService
    {
        CompetenciaDTO AddCompetencia(string descricao);
        List<CompetenciaDTO> ListCompetencia(string busca, int cursor, int limite);
        CompetenciaDTO GetCompetenciaById(long id);
        Task<List<CompetenciaColaboradorDTO>> ListCompetenciaColaborador(string cpfColaborador);
        CompetenciaColaboradorDTO RemoveCompetenciaColaborador(long CompetenciaId, string cpf);
        CompetenciaColaboradorDTO AddCompetenciaColaborador(long CompetenciaId, long? nivelId, string cpf);
        List<NivelDTO> ListaNivelCompetencia();
        void RemoveCertificadoCompetenciaColaborador(long id, string cpf);
        void RemoverCertificado(long id, string cpf);
        void AlteraCertificadoPrincipalColaborador(long id, string cpf);
        CompetenciaColaboradorDTO AlterarCompetenciaColaborador(string cpf, long id, long? nivelId);
        List<long> ListarIdsPorCompetenciaId(long id);
        Task<CertificadoDTO> InserirCertificadoCompetenciaColaborador(string cpf, long? competenciaColaboradorId, byte[] imagem, TipoCertificadoEnum tipo, DateTime conclusao, string descricao, string instituicao, int cargaHoraria);
        Task<CertificadoDTO> AlteraCertificado(string cpf, long competenciaColaboradorId, byte[] imagem, TipoCertificadoEnum tipo, DateTime conclusao, string descricao, string instituicao, int cargaHoraria, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        CompetenciasSumarioResult SumarioCompetencias(FiltroSumarioCompetenciasParam param);
        Task<ItensSumarioResult> SumarioCompetenciasListarSkills(int cursor, int limite, string? descricao);
        List<SkillSumarioDTO> SumarioCompetenciasListarNiveis();
        List<long> ListarCompetenciasAtribuidas(string cpfColaborador);
        List<UnidadesDTO> ListUnidadesComDefaultPorOrg(string token, int orgId);
        List<UnidadesDTO> ListUnidadesPorOrg(string token, int orgId);
        CompetenciaColaboradorResult AddCompetenciaColaboradorEmLote(List<AddCompetenciaColabParam> param, string cpf);
        List<CompetenciaSugeridaDTO> ListarCompetenciasSugeridas(TipoCompetenciaSRSEnum competencia);
        Task AprovarCompetencia(int idCompetenciaASerAprovada, TipoCompetenciaSRSEnum enumTipoCompetencia);
        Task ReprovarCompetencia(int idCompetenciaASerReprovada, TipoCompetenciaSRSEnum tipoCompetencia);
        Task UnificarCompetencia(int idCompetenciaSugerida, int idCompetenciaConsolidada, TipoCompetenciaSRSEnum tipoCompetencia, string cpfRequest, string tokenUsuarioLogado);
        Task<List<CompetenciaConsolidadaDTO>> ListarCompetenciasConsolidadas(TipoCompetenciaSRSEnum competencia);
        Task<List<LogCompetenciaDTO>> ListarLogCompetencias(TipoCompetenciaSRSEnum enumTipoCompetencia);
        Task<EditarCompetenciaDTO> EditarCompetencia(EditarCompetenciaParam param, bool pulaValidacaoAcesso = false, bool pulaValidacaoDuplicidade = false);
        Task<AdicionarCompetenciaDTO> AdicionarCompetencia(string descricao, TipoCompetenciaSRSEnum competencia, string cpf);
        Task<AdicionarCompetenciaDTO> AdicionarCompetenciaSemTransaction(string descricao, TipoCompetenciaSRSEnum competencia, string cpf, int orgId);
        List<PerfilAlocacaoDTO> ListarPerfilAlocacao(int orgId);
        Task<ApiGenericResult<AlterarNomeCompetenciaResultDTO>> AlterarNomeCompetencia(string nomeAtual, string novoNome, TipoCompetenciaSRSEnum tipo, string cpfReuest, string tokenUsuarioLogado);
        Task<ApiGenericResult<List<CompetenciaNomeEIdDTO>>> ListarNomeDeSkillsPorTipo(TipoCompetenciaSRSEnum tipo);
        Task SincronizarSkillsNaoUnificadasNaCuradoriaAntiga();
        Task<ApiGenericResult<List<VwSkillColaboradorDTO>>> ListarSkillsColaborador(string codigoInternoColaborador);
        Task<ApiGenericResult<SkillsLog>> GravarLogsSkillsMinhaJornada(SkillsLog logSkill);
    }
}