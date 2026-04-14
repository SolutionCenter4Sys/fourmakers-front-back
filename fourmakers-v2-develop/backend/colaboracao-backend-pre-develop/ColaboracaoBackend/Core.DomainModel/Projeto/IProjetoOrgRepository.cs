using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Projeto;
using DataTransferObject.Domain.Projeto.ProjetoOrg;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel.Projeto
{
    public interface IProjetoOrgRepository
    {
        List<String> GetProjetosColaboradorOrg(string codColaboradorExterno, int orgId);
        Task<List<ProjetosColaboradorDTO>> GetProjetosCodColaboradorOrg(string codColaboradorExterno, int orgId, bool associacaoAutomaticaColaboradorAoProjeto, bool tbd, string codigoGerenteProjeto, List<string> listaCodigoCliente, string status, FiltroProjetosPrioritariosEnum prioritarioFiltroEnum);
        Task<List<GestorDTO>> GetGestoresOrg(int orgId, List<string>? codDiretoria, string codDepartamento);
        List<NomeRecursoDTO> GetColaboradoresGestor(string codGestor, int orgId, string cpf = null);
        List<ProjetosColaboradorDTO> GetProjetosGestor(string codGestor, int orgId);
        Task<bool> RemoverAssociacaoProjetoColaborador(string codColaborador, int orgId, string codProjeto);
        Task<IEnumerable<ProjetosOrgResult>> ListarProjetos(int orgId, int cursor, int limite, int codStatus, string nomeProjeto);
        bool ProjetoExiste(string codigoProjeto, int orgId);
        Task<bool> CadastrarProjeto(ProjetoOrgDTO param, int orgId, string codDiretoria, string diretoria, StatusProjetosDTO status, TipoCadastroProjetoOrgEnum tipoCadastroProjeto, bool clienteExiste, bool ocultoNaGestaoAlocados);
        Task<ProjetoOrgDetalhesDTO> ObterProjetoPorCodigo(string codProjeto, int orgId);
        Task<List<AtividadeProjetoDTO>> CadastraAtividade(int orgId, List<string> atividades, string codProjeto);
        Task<List<ProjetoOrgColaboradorGerenteDetalhesDTO>> ListarGestoresProjeto(List<string>? diretorias, string codigoDepartamento, string codigoGestorAdm, int orgId);
        Task<IEnumerable<ProjetoComPropostaDTO>> ListarProjetosComPropostasAsync(int orgId);
        Task<int> AtualizarCodigoClienteNosProjetosPorCodClienteAntigo(string codClienteNovo, string codClienteAntigo, int orgId);
        Task<int> AtualizarCodigoClienteNosGestoresExternosPorCodClienteAntigo(string codClienteNovo, string codClienteAntigo, int orgId);
        Task<IEnumerable<ProjetoSimplesDTO>> ListarProjetosDoCliente(string codigoCliente, int orgId);
        Task<bool> VerificarSeCodigoClienteAntigoExiste(string codClienteAntigo, int orgId);
        Task<bool> VerificarSeCodClienteAtualEhDiferenteDoCodigoClienteCRM(string codClienteCRM, string codClienteCCH, int orgId);
        Task<ProjetoOrgDTO> GetProjetoPorCodigoClienteRegistroCargaAsync(string codigoClienteCCH, int orgId);
        Task<(string codDiretoria, string diretoria)> BuscaDadosDiretoriaAsync(string codColaboradorGerente, int orgId);
        Task<StatusProjetosDTO> BuscaDadosStatusAsync(int codStatus, int orgId);
        Task<bool> EditarProjetoAsync(ProjetoOrgDTO param, int orgId, string codDiretoria, string diretoria, StatusProjetosDTO status, string codClienteRegistroCarga, string nomeClienteRegistroCarga, string tipoCadastroProjetoOrg, bool clienteExiste, bool deveOcultarNaGestaoDeAlocados);
        Task<string> ObterDescricaoProjeto(string codProjeto, int orgId);

    }
}