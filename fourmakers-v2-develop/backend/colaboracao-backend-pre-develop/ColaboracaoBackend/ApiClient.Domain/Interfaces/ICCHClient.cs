using DataTransferObject.Domain.CCH;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface ICCHClient
    {
        public Task<TokenCCH> Autenticacao();

        public Task<List<ColaboradorCCH>> Validacao(string emailColaborador, string tokenUsuario);

        public Task<RecursoCCH> Recurso(string codigoProfissional, string tokenUsuario);

        public Task<List<ProjetoRecursoCCH>> ProjetoRecurso(string codigoProfissional, string tokenUsuario);

        public Task<List<ColaboradorCCH>> RecursoProjeto(string tokenUsusario);

        public Task<IEnumerable<ValidacaoProjetoResult>> ValidacaoProjeto(string codigoProjeto, string tokenUsuario);

        public Task<List<HierarquiaResult>> Hierarquia(string tokenUsuario);
        public Task<List<ColaboradorFourMakersCCH>> ColaboradoresFourMakers(string tokenUsuario);
        public Task<List<ColaboradorFourMakersCCH>> ColaboradorNomeFourMakers(string nmProfissional, string tokenUsuario);
        public Task<List<ComboProjetosFourMakersCCH>> ComboProjetosFourMakers(string tokenUsuario);
        public Task<List<ProjetoHorasFourMakersCCH>> ProjetoHorasFourMakers(string tokenUsuario);
        public Task<TokenCCH> AutenticacaoMapaAlocacao();
    }
}