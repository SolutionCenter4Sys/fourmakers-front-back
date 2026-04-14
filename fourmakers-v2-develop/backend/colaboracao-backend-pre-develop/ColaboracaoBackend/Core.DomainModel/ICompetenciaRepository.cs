using Competencia.API.DTOs;
using Competencia.Domain.Enums;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Competencia.MapaCompetencia;

namespace Core.Domain
{
    public interface ICompetenciaRepository<TModel, TFactory>
    {
        TModel GetModel(TModel model);
        TModel GetModelByKey(string key, TFactory factory);
        TModel SaveModel(TModel model);
        List<TModel> ListModel(string busca, int cursor, int limite, TFactory factory);
        long SaveCertificado(TModel competenciaModel, string codigoInternoColaborador, long? competenciaColaboradorId);
        long? GetCompetenciaColabRowId(long competenciaColaboradorId);
        TModel GetColaboradorCompetenciaCertificado(long idCertificadoCompetencia, TFactory factory);
        TModel BuscarUltimoCertificadoParaACompetenciaDesteCertificadoQueEstaSendoExcluido(long idCertificadoCompetencia, TFactory factory);
        TModel UpdateColaboradorCompetenciaCertificado(TModel competenciaModel);
        TModel BuscarColaboradorCompetenciaCertificadoPrincipal(long idCertificadoCompetencia, TFactory competenciaFactory);
        TModel UpdateCertificado(TModel Certificado);
        TModel getCertificado(long certificadoId, TFactory factory);
        TModel GetColaboradorCertificadoByCertificadoId(long idCertificadoCompetencia, TFactory factory);
        CompetenciasSumarioResult BuscarColaboradorSumario(FiltroSumarioCompetenciasParam param, string token, int orgId);
        Task<ItensSumarioResult> ListarSkillsSumario(int cursor, int limite, string? descricao);
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
        Task AtualizarCompetenciasColaboradorCuradoria(int idCompetenciaSugerida, int idCompetenciaConsolidada, TipoCompetenciaSRSEnum tipo);
        Task<List<CompetenciaGroupDTO>> GetCompetenciasByTypeAndGroupId(int v, List<long> list);
        Task ReprovarAssociacaoCompetenciarGestorExternoPerfil(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarAlocado(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarPerfil(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarVaga(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarVagaSRS(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarVagaCandidato(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);
        Task ReprovarAssociacaoCompetenciarVagaFourmakers(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum);

        Task<List<VwSkillColaboradorDTO>> BuscarSkillsPorCodigoInternoColaborador(string codigoInternoColaborador);
        Task<SkillsLog> GravarLogsSkillsMinhaJornada(SkillsLog logSkills);
    }
}