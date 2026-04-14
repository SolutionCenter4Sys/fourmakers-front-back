using DataTransferObject.Domain.Certificado;

namespace Core.Domain.Formacao
{
    public interface IFormacaoCertificadoRepository
    {
        CertificadoDTO GetCertificado(CertificadoDTO certificado);
    }
}