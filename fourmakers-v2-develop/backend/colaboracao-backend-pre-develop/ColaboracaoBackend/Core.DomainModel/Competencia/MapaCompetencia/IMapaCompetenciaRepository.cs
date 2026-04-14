using DataTransferObject.Domain.Competencia.MapaCompetencia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Competencia
{
    public interface IMapaCompetenciaRepository
    {
        Task<List<ColaboradorSkillDetalheDTO>> ListaColaboradoresSkillsDetalhes(int orgId, string separador = ",");
        Task<List<ColaboradorSkillTotalizadorDTO>> ListaColaboradoresSkillsTotalizador(int orgId);
    }
}