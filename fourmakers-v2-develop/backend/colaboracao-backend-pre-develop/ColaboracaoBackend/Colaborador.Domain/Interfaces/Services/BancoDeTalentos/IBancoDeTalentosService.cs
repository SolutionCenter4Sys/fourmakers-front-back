using DataTransferObject.Domain.BancoDeTalentos;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.Vaga;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Services.BancoDeTalentos
{
    public interface IBancoDeTalentosService
    {
        Task<string> InserirBancoDeTalentos(string codInternoColaborador, int orgId, string origem);
        Task<string> InserirBancoDeTalentosFourmakers(string codInternoColaborador, int orgId, string origem, string codigoInternoColaboradorCadastrante, System.DateTime utcNow, int formaCadastro);
        Task DeletarBancoDeTalentos(string codInternoColaborador);
        Task<BancoDeTalentoDTO> BuscarBancoDeTalentosPorColaborador(string codInternoColaborador);
        Task<string> BuscarNomeCadastrante(string cpf, int orgId);
        Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarPessoasQueEuCadastrei(string cpf, int orgId, string busca, int cursor, int limite);
        Task<IEnumerable<ProcessamentoCurriculoLoteDTO>> BuscarMeusLotes(string cpf, int orgId);
        Task<ApiGenericResult<BuscarInformacoesLoteResult>> BuscarInformacoesLote(string idLote);
        Task<IEnumerable<RecrutadoresQuantidadeCadastroBancoTalentos>> BuscarRecrutadoresQuantidadeCadastrada();
        Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarBancoTalentos(int orgId, string busca, int cursor, int limite);
        Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarBancoTalentosComMatch(GestorExternoPerfilInput gestorExternoPerfilInput, int orgId, int limite, int cursor);
        Task<PromptMatchResult> BuscarBancoTalentosComPromptMatch(ExtrairPerfilDeUmPromptRequest request, int orgId, int limite, int cursor, string? codigoInternoColaborador = null, string? idVaga = null);
        Task<PromptMatchResultSnakeCase> BuscarBancoTalentosComPromptMatchSnakeCase(ExtrairPerfilDeUmPromptRequest request, int orgId, int limite, int cursor, string? codigoInternoColaborador = null, string? idVaga = null);
        Task<IEnumerable<BuscarPessoasCadastradasPorOrgResult>> BuscarPessoasCadastradasPorOrg(string cpf, int orgId, BuscarPessoasCadastradasPorOrgInput input);
    }
}