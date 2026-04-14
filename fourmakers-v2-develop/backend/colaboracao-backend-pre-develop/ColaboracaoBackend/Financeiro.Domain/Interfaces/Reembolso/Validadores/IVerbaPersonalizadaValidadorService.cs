using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Financeiro.Domain.Interfaces.Reembolso.Validadores;

public interface IVerbaPersonalizadaValidadorService
{
    Task ValidaVerbaPersonalizada(VerbaPersonalizadaInput input, CRUDEnum operacao, int orgId);
}