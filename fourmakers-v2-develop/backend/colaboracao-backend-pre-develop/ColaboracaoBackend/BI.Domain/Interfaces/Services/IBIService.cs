using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BI;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;
using DataTransferObject.Domain.SRS;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BI.Domain.Interfaces.Services
{
    public interface IBIService
    {
        Task<IEnumerable<RelatorioApontamentoDTO>> GeraRelatorioApontamento(string tokenSistema);
        Task<List<RelatorioApontamentoDTO>> GeraRelatorioApontamentoRecentes(string tokenSistema);
        Task<List<dynamic>> GeraRelatorioAlocacao(string tokenSistema);
        Task<ColaboradorCompletoResult> GetListaColaboradoresCompleto(string tokenSistema);
        Task<List<AlocacaoColaboradorOrgDTO>> GeraAlocacaoTodos(string tokenSistema);
        Task<List<CandidateRelatorioBI>> GetRelatorioCandidate(string tokenSistema);
        Task<List<RelatorioAprovadoresDTO>> RelatorioProjetosEAprovadores(string tokenSistema);
    }
}