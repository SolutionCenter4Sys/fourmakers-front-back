using DataTransferObject.Domain.Carga;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.CalculoMensal;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ProjetoProposta;
using DataTransferObject.Domain.MapaDeAlocacao.Projetos;
using DataTransferObject.Domain.Projeto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao
{
    public interface IProjetoMapaDeAlocacaoRepository
    {
        Task<int> GetQuantidadeDeAlocadosAsync(string cdProjeto, int orgId);
        Task<List<PeriodoDTO>> GetPeriodoAlocadoProjetoAsync(string cdProjeto, int orgId);
        List<BuscaProjetoHorasDTO> ListarProjetoHoras(string busca, int cursor, int limite, int orgId);
        List<StatusProjetosDTO> ListarStatusProjetos(string busca, int cursor, int limite, int orgId);
        ResumoConsultarColaboradorProjetoDTO ResumoHorasComerciais(string cdProjeto, int orgId);
        Task<List<ConsultaProjetoHorasDTO>> BuscarProjetosHoras(int cursor, int limite, string? idProjeto, string? nomeProjeto, int? idStatusProjeto, string cdCliente, string cliente, string gestorProjeto, int? codDiretoria, int orgId);
        List<ColaboradorAlocadoProjetoDTO> ColaboradorAlocadoProjeto(long cdProjeto, string cpf);
        List<TbColaboradorPeriodoAlocacaoDTO> BuscaColaboradoresAlocadosNoProjeto(string cdProjeto, int orgId);
        List<DataTransferObject.Domain.MapaDeAlocacao.ProjetoDTO> ColaboradorProjetoPorColaborador(long cdProjeto, string cpf, int orgId);
        List<PeriodoDTO> ColaboradorPeriodoPorMesAnoProjeto(string mesAno, string colaboradorCPF, int? codigoTBD, string projetoId, int orgId);
        void DeletaProjetosGerentes(string codProjeto, int orgId);
        void InsereProjetoGerente(ProjetoGerenteCargaDTO gerenteProjeto, int orgId);
        void UpsertProjetoOrg(ProjetoCargaDTO projetoCarga, TipoCadastroProjetoOrgEnum tipoCadastro, int orgId);
        GerenteProjetoDTO BuscaGerenteProjeto(int orgId, string codProjeto, string? gestorProjeto);
        Task<IEnumerable<FeriadoAlocacaoDTO>> GetFeriadosPorOrgId(int orgId);
        void InserePropostaProjeto(string proposta, string codProjeto, int orgId);
        void RemovePropostaProjeto(string proposta, string codProjeto, int orgId);
        List<ProjetoProposta> GetPropostaProjeto(string codProjeto, int orgId);
    }
}