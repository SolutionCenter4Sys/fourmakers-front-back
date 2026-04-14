using DataTransferObject.Domain.Colaborador;

namespace Usuario.Domain.Interfaces.Models
{
    public interface IStatusColaboradorModel
    {
        StatusColaboradorDTO Status { get; set; }
        string CpfUsuario { get; set; }

        IStatusColaboradorModel GetStatusByCpfColaborador(string cpf);
    }
}