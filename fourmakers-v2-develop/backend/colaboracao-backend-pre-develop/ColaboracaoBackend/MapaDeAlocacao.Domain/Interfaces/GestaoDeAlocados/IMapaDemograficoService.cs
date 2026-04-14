using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.MapaDemografico;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados
{
    public interface IMapaDemograficoService
    {
        Task<ApiGenericResult<List<MapaDemograficoDTO>>> BuscarBancoTalentosOrgId(string cpfRequest, int? orgID = null, string talento = null);

        Task<ApiGenericResult<List<MapaDemograficoDTO>>> BuscarBancoTalentosCodColaborador(string codColaborador);
        Task<ApiGenericResult<List<MapaDemograficoPcdSumarioDTO>>> ListarSumarioPcdsAsync(int orgId);
    }    
};
