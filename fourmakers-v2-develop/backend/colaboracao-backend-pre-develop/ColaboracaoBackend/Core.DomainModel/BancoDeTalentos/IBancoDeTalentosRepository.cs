using DataTransferObject.Domain.BancoDeTalentos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel.BancoDeTalentos
{
    public interface IBancoDeTalentosRepository
    {
        Task<string> InserirBancoDeTalentos(string codInternoColaborador, int orgId, string tipoCadastro);
        Task RemoverBancoDeTalentos(string codInternoColaborador);
        Task<BancoDeTalentoDTO> BuscarBancoDeTalentosPorColaborador(string codInternoColaborador);
        Task<string> InserirBancoDeTalentosIdExterno(string codInternoColaborador, int orgId, string origem, string idExterno, string codigoInternoColaboradorCadastrante, System.DateTime utcNow, int formaCadastro);
        Task<string> BuscarNomeCadastrante(string cpf, int orgId);
        Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarPessoasQueEuCadastrei(string codInternoColaborador, int orgId, string busca, int cursor, int limite);
        Task<IEnumerable<RecrutadoresQuantidadeCadastroBancoTalentos>> BuscarRecrutadoresQuantidadeCadastrada();
        Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarBancoTalentosPorOrg(int orgId, string busca, int cursor, int limite);
        Task<IEnumerable<BuscarPessoasCadastradasPorOrgResult>> BuscarPessoasCadastradasPorOrg(int orgId, string busca, List<int> statusVaga, List<int> statusCandidatura, string dataInicio, string dataFim, int cursor, int limite);
    }
}