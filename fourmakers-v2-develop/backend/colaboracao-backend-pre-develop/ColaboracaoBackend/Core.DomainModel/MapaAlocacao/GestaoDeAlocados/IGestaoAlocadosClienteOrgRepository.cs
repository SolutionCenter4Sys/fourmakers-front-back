using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ClienteOrg;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ColaboradoresAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.Permanencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ModeloTrabalho;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ProfissionalLocalidade;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.RateCardsDosPerfis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao.GestaoDeAlocados
{
    public interface IGestaoAlocadosRepository
    {
        Task<IEnumerable<ClienteOrgDaGestaoAlocadosResult>> ListarClienteOrgDaGestaoDeAlocados(string buscaCodigoOuNome, int limite, int cursor, bool semPerfil, int orgId);
        Task<List<GestoresEPerfisDaGestaoDeAlocadosResult>> ListarGestoresEPerfisDaGestaoDeAlocados(int limite, int cursor, string busca, string codigoCliente, bool semPerfil, bool semAreaAtuacao, int orgId);
        Task<List<RateCardsDosPerfisResult>> ListarRateCardsDosPerfisPorCliente(int limite, int cursor, string busca, string codigoCliente, int orgId);
        Task<List<RateCardsDosPerfisHistoricoResult>> ListarRateCardsDosPerfisHistoricoPorPerfilId(int limite, int cursor, Guid gestorExternoPerfilId, int orgId);
        Task<IEnumerable<PermanenciaResult>> ListarPermanenciasAsync();
        Task<IEnumerable<ModeloTrabalhoResult>> ListarModelosTrabalhoAsync();
        Task<IEnumerable<ProfissionalLocalidadeResult>> ListarProfissionaisLocalidadesAsync();
        Task<IEnumerable<string>> ListarPropostasPorCliente(string codCliente, int orgId);
        Task<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>> ListarColaboradoresAlocadosAtualmentePorCliente(int limite, int cursor, string busca, string codCliente, int orgId);
        Task<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>> ListarColaboradoresAlocadosPorClienteCompletoDetalhe(int limite, int cursor, string codCliente, int orgId, string codGestorAdm, string codGestorOper);
    }
}