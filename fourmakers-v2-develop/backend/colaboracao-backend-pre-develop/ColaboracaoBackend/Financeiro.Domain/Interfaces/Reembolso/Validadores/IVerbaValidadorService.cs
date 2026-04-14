using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Financeiro.Domain.Interfaces.Reembolso.Validadores;

public interface IVerbaValidadorService
{
    Task<List<VerbaLogDTO>> GerenciarLogs(VerbaDTO prev, VerbaDTO next, CRUDEnum tipo);
    Task ValidaVerba(VerbaDTO verba, CRUDEnum crud);
}