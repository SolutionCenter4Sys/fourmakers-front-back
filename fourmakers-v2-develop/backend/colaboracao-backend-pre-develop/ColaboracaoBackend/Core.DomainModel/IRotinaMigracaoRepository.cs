using System.Collections.Generic;

namespace Core.Domain
{
    public interface IRotinaMigracaoRepository
    {
        List<string> ListaTodasImagensDoBanco();
        List<string> ListaTodosCertificadosDoBanco();
        List<string> ListaTodosCurriculosDeCandidatosDoBanco();
    }
}