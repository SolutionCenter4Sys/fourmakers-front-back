using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.Projetos;
using DataTransferObject.Domain.Projeto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces
{
    public interface IProjetoMapaDeAlocacaoService
    {
        List<BuscaProjetoHorasDTO> ListarProjetoHoras(string busca, int cursor, int limite, string cpfSolicitante, int orgId);
        List<StatusProjetosDTO> ListarStatusProjetos(string busca, int cursor, int limite, string cpfSolicitante, int orgId);
        Task<List<ConsultaProjetoHorasDTO>> ConsultaProjetoHoras(int cursor, int limite, string cdProjeto, string? nomeProjeto, DisponibilidadeHorarioEnum? disponibilidadeHorario, int? cdStatusProjeto, int? codDiretoria, string cdCliente, string cliente, string gestorProjeto, string cpfSolicitante, int orgId);
        ConsultarColaboradorProjetoResult ConsultarColaboradoresProjeto(string cdProjeto, string cpf, int? tbdBusca, DateTime? de, DateTime? ate, string cpfSolicitante, int orgId, bool exibirColabComAlocacoesAtuaisEFuturas);
    }
}