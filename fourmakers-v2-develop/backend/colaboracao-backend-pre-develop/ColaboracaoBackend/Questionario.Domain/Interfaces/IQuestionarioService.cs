using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador.Campanha._2025_01_COLETA_PERFIL_COLABORADOR;

namespace Questionario.Domain.Interfaces;

public interface IQuestionarioService
{
    Task<ApiGenericResult<string>> ColetarPerfilColaboradorAsync(ColetaPerfilColaboradorCampanhaRootDTO param, string codigoInternoColaborador, int orgId);
}