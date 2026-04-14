using DataTransferObject.Domain.Colaborador;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Verba;

public class VerbaPersonalizadaColaboradorDTO : VerbaPersonalizadaDTO
{
    public SimpleColaboradorDTO Colaborador { get; set; }
}