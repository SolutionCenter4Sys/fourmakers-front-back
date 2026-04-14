using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Competencia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel.Competencia
{
    public interface ICompetenciaColaboradorRepository
    {
        List<CompetenciaColaboradorDTO> GetCompetenciaColaborador(string cpf);
        List<CertificadoDTO> GetCertificadoColaborador(string cpf);
        List<long> ListarCompetenciasAtribuidas(string cpfColaborador);
        Task<List<CompetenciaColaboradorDTO>> BuscarCompetenciaColaboradorPorCompetenciaIds(long[] competenciaIds);
        Task ExcluirColaboradorCompetenciaPorId(long id);
    }
}