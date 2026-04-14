using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.MapaDemografico;

namespace Core.Domain.MapaAlocacao.GestaoDeAlocados
{    
    public interface IMapaDemograficoRepository
    {
        Task<List<MapaDemograficoDTO>> BuscarBancoTalentosOrgId(List<string>? diretorias, int? orgID = null, string talento = null);
        Task<List<MapaDemograficoDTO>> BuscarBancoTalentosCodColaborador(string codColaborador);
        Task<List<MapaDemograficoPcdSumarioDTO>> ListarSumarioPcdsAsync(int orgId);
    }
}
