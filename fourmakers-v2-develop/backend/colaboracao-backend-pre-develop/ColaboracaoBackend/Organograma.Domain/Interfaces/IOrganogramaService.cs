using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace Organograma.Domain.Interfaces
{
    public interface IOrganogramaService
    {
        Task<OrganogramaDeptoListarPorIdResponseDTO> InserirDepartamento(OrganogramaDeptoInserirParamDTO param);
        Task<OrganogramaDeptoListarPorIdResponseDTO> AtualizarDepartamento(OrganogramaDeptoAtualizarParamDTO param);
        Task<bool> DeletarDepartamento(string id);
        Task<OrganogramaDeptoListarPorIdResponseDTO> ListaDepartamentoPorId(string buscaId);
        Task<List<OrganogramaDeptoListarPorIdResponseDTO>> ListarDepartamentoPorCliente(string codCliente);

        Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoInserir(OrganogramaPosicaoInserirParamDTO param);
        Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoAtualizar(OrganogramaPosicaoAtualizarParamDTO param);
        Task<bool> PosicaoDeletar(string id);
        Task<OrganogramaPosicaoListarPorIdResponseDTO> PosicaoListaPorId(string buscaId);
        Task<List<OrganogramaPosicaoListarPorIdResponseDTO>> PosicaoListarPorCliente(string buscaId);

        Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoInserir(OrganogramaAlocacaoInserirParamDTO param);
        Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoAtualizar(OrganogramaAlocacaoAtualizarParamDTO param);
        Task<bool> AlocacaoDeletar(string id);
        Task<OrganogramaAlocacaoListarPorIdResponseDTO> AlocacaoListaPorId(string buscaId);

        Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> InserirPerfilCorporativoAlocacao(OrganogramaPerfilCorporativoAlocacaoInserirParamDTO param);
        Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> AtualizarPerfilCorporativoAlocacao(OrganogramaPerfilCorporativoAlocacaoAtualizarParamDTO param);
        Task<bool> DeletarPerfilCorporativoAlocacao(string id);
        Task<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO> BuscarPerfilCorporativoAlocacaoPorId(string buscaId);
        Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternosAlocados(int limit, int cursor, string nome);
        Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternos(int limit, int cursor, string nome);
        Task<IEnumerable<OrganogramaColaboradorAlocadoDTO>> ListarColaboradoresExternosPorCliente(string codigoCliente, int limit, int cursor, string nome);

        Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoInserir(OrganogramaPerfilCorporativoInserirParamDTO param);
        Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoAtualizar(OrganogramaPerfilCorporativoAtualizarParamDTO param);
        Task<bool> PerfilCorporativoDeletar(string id);
        Task<OrganogramaPerfilCorporativoListarPorIdResponseDTO> PerfilCorporativoListaPorId(string buscaId);
        Task<IEnumerable<OrganogramaPerfilCorporativoListarPorIdResponseDTO>> BuscarPerfisPorOrgId(int orgId);

        Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillInserir(OrganogramaPerfilCorporativoSkillInserirParamDTO param);
        Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillAtualizar(OrganogramaPerfilCorporativoSkillAtualizarParamDTO param);
        Task<bool> PerfilCorporativoSkillDeletar(string id);
        Task<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO> PerfilCorporativoSkillListaPorId(string buscaId);

        Task<IEnumerable<OrganogramaClienteOrgDTO>> RetornarClientesPorOrgId(int orgId, int limite, int cursor, string busca);
        Task<OrganogramaCompletoResponseDTO> RetornarOrganogramaCompletoPorCliente(string codigoCliente);
        Task<OrganogramaCompletoResponseDTO> RetornarOrganogramaCompletoPorClienteCLevel(string codigoCliente);

    }
}