using System.Collections.Generic;

namespace Core.Domain
{
    public interface IRotinaIntegracaoRepository
    {
        List<string> ListaEmailColaboradoresFoursys();
        void AtualizaStatusColaboradorPorEmail(int ativo, string email);
    }
}