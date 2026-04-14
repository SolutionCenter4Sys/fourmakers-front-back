using System.Threading.Tasks;
using Competencia.Domain.Enums;

namespace Competencia.Domain.Interfaces.Services
{
    public interface ICompetenciaHistoricoService
    {
        bool InserirHistoricoCompetenciaAcaoUnificar(AcaoHistoricoCompetenciaEnum acao, TipoCompetenciaSRSEnum tipoCompetencia, string descricaoCompetencia1, string descricaoCompetencia2, string cpfUsuarioSolicitante);
        bool InserirHistoricoCompetencia(AcaoHistoricoCompetenciaEnum acao, TipoCompetenciaSRSEnum tipoCompetencia, string descricaoCompetencia, string cpfUsuarioSolicitante);
        Task<bool> EditarHistoricoCompetencia(AcaoHistoricoCompetenciaEnum editar, TipoCompetenciaSRSEnum tipoCompetencia, string descricaoCompetencia1, string descricaoCompetencia2, string cpfUsuarioCriacao);
    }
}