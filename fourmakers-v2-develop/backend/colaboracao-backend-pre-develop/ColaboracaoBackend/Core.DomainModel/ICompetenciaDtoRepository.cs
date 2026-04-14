using Competencia.API.DTOs;
using Competencia.Domain.Enums;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Competencia.MapaCompetencia;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public interface ICompetenciaDtoRepository
    {
        CompetenciasSumarioResult BuscarColaboradorSumario(FiltroSumarioCompetenciasParam param, string token, int orgId);
        Task<ItensSumarioResult> ListarSkillsSumario(int cursor, int limite, string descricao);
        List<SkillSumarioDTO> ListarNiveisSumario();
        List<CompetenciaSugeridaDTO> ListarCompetenciasSugeridas(TipoCompetenciaSRSEnum competencia);
        Task AprovarCompetencia(int idCompetenciaASerAprovada, TipoCompetenciaSRSEnum tipoCompetencia);
        Task ReprovarCompetencia(int idCompetenciaASerReprovada, TipoCompetenciaSRSEnum competenciaEnum);
        Task ReprovarAssociacaoCompetenciaColaborador(int idCompetenciaASerReprovada, TipoCompetenciaSRSEnum competenciaEnum);
        Task<int> ObterQuantidadeDeSkillsAtivas(List<int> id, TipoCompetenciaSRSEnum competenciaEnum);
        Task<int> ObterQuantidadeDeSkillSugerida(int id, TipoCompetenciaSRSEnum competenciaEnum);
        Task<int> ObterQuantidadeDeSkillConsolidada(int id, TipoCompetenciaSRSEnum competenciaEnum);
        Task InsereLogReprovacaoCompetencia(int idCompetenciaASerReprovada, TipoCompetenciaSRSEnum competenciaEnum, string cpfUsrLogado);
        Task GravaLogUnificacao(int idCompetencia, int idCompetenciaConsolidada, TipoCompetenciaSRSEnum competenciaEnum, string cpfUsrLogado);
        Task<List<CompetenciaConsolidadaDTO>> ListarCompetenciasConsolidadas(TipoCompetenciaSRSEnum competencia);
        Task<List<CompetenciaSumarioDTO>> ListarCompetenciaSumario(TipoCompetenciaSRSEnum competencia, int orgId);
        Task<CompetenciaDTO> ObterCompetenciaAtivaPorTipoEId(int idCompetencia, TipoCompetenciaSRSEnum competencia);
        Task DesativarCompetencia(int idCompetencia, TipoCompetenciaSRSEnum competenciaEnum);
        Task<int> AtualizarCompetenciaColaboradorDeSugeridaParaConsolidada(int idCompetencia, int idCompetenciaConsolidada, TipoCompetenciaSRSEnum competenciaEnum);
        Task<List<LogCompetenciaDTO>> ListarLogCompetencias(TipoCompetenciaSRSEnum enumTipoCompetencia);
        Task<EditarCompetenciaDTO> EditarCompetencia(EditarCompetenciaParam param, bool pulaValidacaoDuplicidade = false);
        Task<AdicionarCompetenciaDTO> AdicionarCompetencia(string descricao, TipoCompetenciaSRSEnum competencia, string cpf);
        Task<AdicionarCompetenciaDTO> AdicionarCompetenciaCvGestaoDeSkills(string descricao, TipoCompetenciaSRSEnum competencia, string cpf, int orgid);
        Task<CompetenciaDTO> ObterCompetenciaAtivaPorTipoENome(string nomeCompetencia, TipoCompetenciaSRSEnum competencia);
        Task<List<CompetenciaNomeEIdDTO>> ObterNomeSkillsPorTipo(TipoCompetenciaSRSEnum tipo);
        Task<List<CompetenciaNomeEIdDTO>> ObterNomeSkillsPorTipoEIds(TipoCompetenciaSRSEnum tipo, IReadOnlyList<int> ids);
        Task AtualizarCompetenciasColaboradorCuradoria(int idCompetenciaSugerida, int idCompetenciaConsolidada, TipoCompetenciaSRSEnum tipo);
        Task<List<CompetenciaGroupDTO>> GetCompetenciasByTypeAndGroupId(int v, List<long> list);
        Task<List<VwSkillColaboradorDTO>> BuscarSkillsPorCodigoInternoColaborador(string codigoInternoColaborador);
        Task<SkillsLog> GravarLogsSkillsMinhaJornada(SkillsLog logSkills);
        long? GetCompetenciaColabRowId(long competenciaColaboradorId);

        CompetenciaDTO GetCompetenciaById(long id);
        long GetUsuarioCriacaoIdByCpf(string cpf);
        long SaveCompetencia(string descricao, long usuarioCriacaoId);
        List<CompetenciaDTO> ListCompetencias(string busca, int cursor, int limite);
        CertificadoDTO GetCertificadoById(long certificadoId);
        void UpdateCertificado(CertificadoDTO dto);
        ColaboradorCompetenciaCertificadoDTO GetColaboradorCompetenciaCertificado(long idCertificadoCompetencia);
        ColaboradorCompetenciaCertificadoDTO GetColaboradorCertificadoByCertificadoId(long idCertificadoCompetencia);
        void UpdateColaboradorCompetenciaCertificado(long id, sbyte ativo, sbyte principal);
        ColaboradorCompetenciaCertificadoDTO BuscarColaboradorCompetenciaCertificadoPrincipal(long idCertificadoCompetencia);
        ColaboradorCompetenciaCertificadoDTO BuscarUltimoCertificadoParaACompetenciaDesteCertificadoQueEstaSendoExcluido(long idCertificadoCompetencia);
        long SaveCertificado(CertificadoDTO certificado, string codigoInternoColaborador, long? competenciaColaboradorId);
        long SaveCompetenciaColaborador(long competenciaId, long? nivelId, string cpf);
        void RemoveCompetenciaColaborador(string cpf, long competenciaId);
        void AtualizaCompetenciaColaborador(string cpf, long competenciaId, long? nivelId);
        List<long> ListarIdsPorCompetenciaId(long id);
        NivelDTO GetNivelById(long? id);
        List<NivelDTO> ListNiveisCompetencia();
        StatusEndossoDTO GetEndossoByCompetenciaColaboradorId(long idCompetenciaColaborador);
        CertificadoDTO GetCertificadoByIdRelacao(long? idCertificado);

        Task ReprovarAssociacaoCompetenciarGestorExternoPerfil(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarAlocado(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarPerfil(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarVaga(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarVagaSRS(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarVagaCandidato(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarVagaFourmakers(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
    }
}
