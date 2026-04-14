using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Financeiro.Reembolso.Parametro;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Financeiro.Domain.Interfaces.Reembolso.Validadores;

public interface IParametroReembolsoValidadorService
{
    List<VerbaLogDTO> GerenciarLogs(ParametroReembolsoDTO prev, ParametroReembolsoDTO next, CRUDEnum tipo);
    void ValidaParametroReembolso(ParametroReembolsoDTO input, CRUDEnum operation);
}