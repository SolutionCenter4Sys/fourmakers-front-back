using DataTransferObject.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Organograma
{
    public interface IOrganogramaRepository
    {
        Task<OrganogramaDeptoListarPorIdResponseDTO> InserirDepartamento(OrganogramaDeptoInserirParamDTO param, int orgId);
        Task<OrganogramaDeptoListarPorIdResponseDTO> AtualizarDepartamento(OrganogramaDeptoAtualizarParamDTO param, int orgId);
        Task<bool> DeletarDepartamento(string id);
        Task<OrganogramaDeptoListarPorIdResponseDTO> ListaDepartamentoPorId(string buscaId);
        Task<List<OrganogramaDeptoListarPorIdResponseDTO>> ListarDepartamentoPorCliente(string codCliente);

        Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoInserir(OrganogramaPosicaoInserirParamDTO param, int orgId);
        Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoAtualizar(OrganogramaPosicaoAtualizarParamDTO param, int orgId);
        Task<bool> PosicaoDeletar(string id);
        Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoListaPorId(string buscaId);
        Task<List<OrganogramaPosicaoListarPorIdResponseDTO>> PosicaoListarPorCliente(string codCliente);

        Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoInserir(OrganogramaAlocacaoInserirParamDTO param, int orgId);
        Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoAtualizar(OrganogramaAlocacaoAtualizarParamDTO param, int orgId);
        Task<bool> AlocacaoDeletar(string id);
        Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoListaPorId(string buscaId);

        Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> InserirPerfilCorporativoAlocacao(OrganogramaPerfilCorporativoAlocacaoInserirParamDTO param, int orgId);
        Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> AtualizarPerfilCorporativoAlocacao(OrganogramaPerfilCorporativoAlocacaoAtualizarParamDTO param, int orgId);
        Task<bool> DeletarPerfilCorporativoAlocacao(string id);
        Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> BuscarPerfilCorporativoAlocacaoPorId(string buscaId);
        Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternosAlocados(int orgId, int limit, int cursor, string nome);
        Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternos(int orgId, int limit, int cursor, string nome);
        Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternosPorCliente(int orgId, string codigoCliente, int limit, int cursor, string nome);

        Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoInserir(OrganogramaPerfilCorporativoInserirParamDTO param, string codColaborador, int orgId);
        Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoAtualizar(OrganogramaPerfilCorporativoAtualizarParamDTO param, string codColaborador, int orgId);
        Task<bool> PerfilCorporativoDeletar(string id);
        Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoListaPorId(string buscaId);
        Task<IEnumerable<OrganogramaPerfilCorporativoListarPorIdResponseDTO>> BuscarPerfisPorOrgId(int orgId);

        Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillInserir(OrganogramaPerfilCorporativoSkillInserirParamDTO param, string codColaborador, int orgId);
        Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillAtualizar(OrganogramaPerfilCorporativoSkillAtualizarParamDTO param, string codColaborador, int orgId);
        Task<bool> PerfilCorporativoSkillDeletar(string id);
        Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillListaPorId(string buscaId);

        Task<IEnumerable<OrganogramaClienteOrgDTO>> RetornarClientesPorOrgId(int orgId, int limite, int cursor, string busca);
        Task<OrganogramaCompletoResponseDTO> RetornarOrganogramaCompletoPorCliente(int orgId, string codigoCliente);
        Task<OrganogramaCompletoResponseDTO> RetornarOrganogramaCompletoPorClienteCLevel(int orgId, string codigoCliente);
    }
}
