using DataTransferObject.Domain;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain
{
    public interface IPerfilRepository
    {
        Task<int> CountPerfis(int orgId);
        Task<IEnumerable<ListarPerfisResult>> ListarPerfis(DateTime dataInicio, DateTime dataFim, string cliente, string cpf, int limite, int cursor, string busca, int orgId);
    }
}