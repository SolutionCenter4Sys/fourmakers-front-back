using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.SRS.Candidate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Competencia.API.DTOs;
using Competencia.Domain.Enums;
using DataTransferObject.Domain.Competencia.MapaCompetencia;

namespace ApiClient.Domain.Interfaces
{
    public interface ICompetenciaClient
    {
        Task<ItemPerfilDTO> GetCompetenciaById(long id, string tokenUsuario);
        Task<List<CompetenciaColaboradorDTO>> ListarCompetenciasColaborador(string cpf, string tokenUsuario);
        Task<List<ItemPerfilResult>> AdicionarCompetenciaColaborador(List<AdicionarRemoverItemDTO> dtos, string token);
        Task<StatusResult> RemoverCompetenciaColaborador(string token, string cpf, long id);
        Task<CertificadoDTO> InserirCertificadoCompetenciaColaborador(string cpfRequest, long competenciaColaboradorId, byte[] file, TipoCertificadoEnum tipo, string tokenUsuario, DateTime dataConclusao, string instituicao, string descricao, int cargaHoraria);
        Task RemoveCertificadoCompetenciaColaborador(string cpf, long id, string tokenUsuario);
        Task AlteraCertificadoPrincipalColaborador(string cpf, long id, string tokenUsuario);
        Task<ItemPerfilResult> AlterarCompetenciaColaborador(AdicionarRemoverItemDTO dtos, string token);
        Task<StatusResult> MergeCompetenciaColaborador(MergeItemPerfilDTO dtos, string token);
        Task<List<long>> ListarIdsPorCompetenciaId(long id, string tokenUsuario);
        Task<List<SkillValuePair>> GetHardSkillInfoByDescricao(List<string> skills, string token);
        Task<CertificadoColaboradorResult> ListarCertificadoColaboradorPorCodigoInterno(string codInternoColaborador, string tokenUsuario);
        Task<CompetenciaResult> AdicionarCompetencia(string descricao, string token);
        Task<List<CompetenciaNomeEIdDTO>> ListarNomeDeSkillsPorTipo(TipoCompetenciaSRSEnum tipo, string tokenUsuario);
        Task<ApiGenericResult<AlterarNomeCompetenciaResultDTO>> AlterarNomeCompetencia(EditarNomeCompetenciaParam param, string token);
    }
}