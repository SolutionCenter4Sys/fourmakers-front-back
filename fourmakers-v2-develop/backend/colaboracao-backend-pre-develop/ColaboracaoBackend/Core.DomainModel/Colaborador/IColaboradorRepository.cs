using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Colaborador
{
    public interface IColaboradorDapperRepository
    {
        Task<EditarColaboradorDTO> ObterColaboradorPorCodigoAsync(string codigoInternoColaborador);
        Task<EditarColaboradorDTO> AtualizarColaboradorComLogAsync(EditarColaboradorDTO colaborador, EditarColaboradorDTO colaboradorAnterior, string codigoInternoColaboradorAlterador);
        Task<bool> ExisteColaboradorAsync(string codigoInternoColaborador);
        Task<List<ColaboradorLogDTO>> ListarLogsPorColaboradorAsync(string codigoInternoColaborador, int limite, int cursor);
        Task<List<ColaboradorLogDTO>> ListarLogsPorAlteradorAsync(string codigoInternoColaboradorAlterador, int limite, int cursor);
        Task<List<ColaboradorLogDTO>> ListarLogsPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, int limite, int cursor);
        Task InserirDadosDemograficosAsync(DadosDemograficosColaboradorDTO dadosDemograficos, string codigoInternoColaborador);
        Task AlterarDadosDemograficosAsync(DadosDemograficosColaboradorDTO dadosDemograficos, string codigoInternoColaborador);
        Task<DadosDemograficosColaboradorDTO> ObterDadosDemograficosAsync(string codigoInternoColaborador);
    }
}
