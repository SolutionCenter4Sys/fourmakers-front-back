using DataTransferObject.Domain.Usuario;
using System;

namespace Core.Domain.Usuario
{
    public interface IUsuarioFourmakerRepository
    {
        bool IsFourmakerUser(string cpf);
        void AddConvite(ConviteUsuarioExternoDTO convite, DateTime validade);
        void DeleteConvite(string cpf, string cnpj);
        ConviteUsuarioExternoDTO Valida(string token);
    }
}