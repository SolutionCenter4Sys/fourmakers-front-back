using System.Threading.Tasks;

namespace Core.DomainModel.Competencia
{
    public interface ICompetenciaHistoricoRepository
    {
        Task<bool> InserirHistoricoCompetencia(string tipoCompetencia, string descricaoCompetencia, string situacao, string observacao, string cpfUsuarioCriacao);
    }
}