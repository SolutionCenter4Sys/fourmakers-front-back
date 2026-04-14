using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Interfaces.Factorys
{
    public interface IStatusColaboradorDomainFactory
    {
        IStatusColaboradorModel buildStatusModel(int statusId, string descricao, string cpfUsuario);
        IStatusColaboradorModel buildStatusModel();
    }
}