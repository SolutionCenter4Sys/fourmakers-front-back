using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ClienteOrg;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ColaboradoresAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.Permanencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ModeloTrabalho;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ProfissionalLocalidade;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.RateCardsDosPerfis;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SincronizarCRM;
using DataTransferObject.Domain.Pricing.Equipe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados
{
    public interface IGestaoAlocadosService
    {
        Task<ApiGenericResult<IEnumerable<ClienteOrgDaGestaoAlocadosResult>>> ListarClienteOrgDaGestaoDeAlocados(string cpfRequest, int limite, int cursor, bool semPerfil, string buscaCodigoOuNome, int orgId);
        Task<ApiGenericResult<IEnumerable<GestoresEPerfisDaGestaoDeAlocadosResult>>> ListarGestoresEPerfisDaGestaoDeAlocados(string cpf, int limite, int cursor, string busca, string codigoCliente, bool semPerfil, bool semAreaAtuacao, int orgId);
        Task<ApiGenericResult<IEnumerable<RateCardsDosPerfisResult>>> ListarRateCardsDosPerfisPorCliente(string cpfRequest, int limite, int cursor, string busca, string codigoCliente, int orgId);
        Task<ApiGenericResult<IEnumerable<RateCardsDosPerfisHistoricoResult>>> ListarRateCardsDosPerfisHistoricoPorPerfilId(string cpfRequest, int limite, int cursor, Guid gestorExternoPerfilId, int orgId);
        Task<ApiGenericResult<IEnumerable<PermanenciaResult>>> ListarPermanencias(string cpfRequest, int orgId);
        Task<ApiGenericResult<IEnumerable<ModeloTrabalhoResult>>> ListarModelosTrabalho(string cpfRequest, int orgId);
        Task<ApiGenericResult<IEnumerable<ModeloTrabalhoResult>>> ListarModelosTrabalhoPublico();
        Task<ApiGenericResult<IEnumerable<ProfissionalLocalidadeResult>>> ListarProfissionaisLocalidades(string cpfRequest, int orgId);
        Task<ApiGenericResult<IEnumerable<RatecardClienteResult>>> ObterRatecardPorCodigoCliente(string cpfRequest, string codCliente, int orgId);
        Task<ApiGenericResult<SincronizarCRMResult>> SincronizarClientesEGestoresCRM(string cpfRequest, int orgId);
        Task<ApiGenericResult<SincronizarCRMResult>> GetDataUltimaSincronizacaoClientesEGestoresCRM(string cpfRequest, int orgId);
        Task<ApiGenericResult<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>>> ListarColaboradoresAlocadosPorCliente(string cpfRequest, int limite, int cursor, string busca, string codCliente, int orgId);
        Task<ApiGenericResult<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>>> ListarColaboradoresAlocadosPorClienteSemAderencia(string cpfRequest, int limite, int cursor, string busca, string codCliente, int orgId);
        Task<ApiGenericResult<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>>> ListarColaboradoresAlocadosPorClienteCompleto(string cpfRequest, int limite, int cursor, string codCliente, int orgId, string codGestorAdm, string codGestorOper);
    }
}