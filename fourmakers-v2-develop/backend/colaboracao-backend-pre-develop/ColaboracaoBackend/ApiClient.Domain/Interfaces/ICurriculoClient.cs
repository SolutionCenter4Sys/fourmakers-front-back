using DataTransferObject.Domain;
using DataTransferObject.Domain.Apontamento;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using DataTransferObject.Domain.Financeiro.Holerite;
using DataTransferObject.Domain.Linkedin;
using DataTransferObject.Domain.Marketing.Comunicacao.IA;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga.AnaliseDocumentosResult;

namespace ApiClient.Domain.Interfaces
{
    public interface ICurriculoClient
    {
        Task<Root> GetPerfilLinkedin(string urlPerfil, string tokenAcesso);
        Task<Root> GetPerfilLinkedinRapidAPI(string urlPerfil, string tokenAcesso);
        Task<ListaSkillDTO> SkillClassify(List<string> skills, string tokenAcesso);
        Task<RootIA> GetProfileByDocumentContent(byte[] documento, string tokenAcesso);
        Task<RootIA> AnalizarDocumentoIA(string documentTipe, string base64Image, string tokenAcesso);
        Task<SumarioFolhaPontoDTO> SumarioFolhaPonto(string documentType, string base64Pdf, string tokenAcesso);
        Task<RelatorioPontoRootDTO> AnaliseColaboradorFolhaPonto(string documentType, string base64Pdf, string tokenAcesso);
        Task<SumarioHoleriteIaResult> SumarioHolerite(string documentType, string base64Pdf, string tokenAcesso);
        Task<HoleriteAnaliseResultDTO> AnaliseHoleriteColaborador(string documentType, string base64Pdf, string tokenAcesso);
        Task<RubricaCargaResult> InserirRubricaCarga(string rubricaUrl, string rubricaPath, string rubricaId, string codigoRubricaFrequencia, int MesInicial, int AnoInicial, string cpfRequest, string codDiretoria, string tokenAcesso, int orgId);
        Task<RubricaCargaConsultaResult> BuscarStatusCargaPorId(string cargaId, string tokenAcesso);
        Task<AnaliseRubricaCargaResult<AnaliseUnimedResultWrapper>> AnaliseUnimed(string base64Pdf, string tokenAcesso);
        Task<AnaliseRubricaCargaResult<AnalizeAmilResultWrapper>> AnaliseAmil(string base64Pdf, string tokenAcesso);
        Task<AnaliseRubricaCargaResult<AnalisePortoSeguroOdontoResultWrapper>> AnalisePortoSeguroOdonto(string base64Pdf, string tokenAcesso);
        Task<AnaliseRubricaCargaResult<AnalizarGenericoProfarmaWrapper>> AnaliseProfarma(string base64Pdf, string tokenAcesso);
        Task<AnaliseRubricaCargaResult<AnaliseGenericoResultWrapper>> AnaliseGenerico(string base64Pdf, string tokenAcesso, string campos, string informacoesAdicionais);
        Task<T> AnaliseXlsxGenerico<T>(string base64Xlsx, dynamic fields, List<string> requiredFields, string tokenAcesso, int headerRow = 0);
        Task<AnaliseIANotaFiscalValorDTO> AnalisarValorNotaFiscal(Base64DTO base64Dto, string tokenAcesso);
        Task<string> RefatorarTextoAsync(string texto, ModoRefatoracaoTextoEnum modo, bool negrito, string tokenAcesso);
    }
}