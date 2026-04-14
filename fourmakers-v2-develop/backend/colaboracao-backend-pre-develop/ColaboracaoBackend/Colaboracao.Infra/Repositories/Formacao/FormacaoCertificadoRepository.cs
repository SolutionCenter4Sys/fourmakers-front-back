using Colaboracao.Infra.Context;
using Core.Domain.Formacao;
using DataTransferObject.Domain.Certificado;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Formacao
{
    public class FormacaoCertificadoRepository : IFormacaoCertificadoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public FormacaoCertificadoRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }
        public CertificadoDTO GetCertificado(CertificadoDTO certificado)
        {
            try
            {
                var certificadoPath = _colaboradorContext.tb_certificado
                    .Where(x => x.id == certificado.IdCertificado).FirstOrDefault().path;
                string pathThumb = null;
                if (certificadoPath.IndexOf("pdf") != -1)
                    pathThumb = certificadoPath.Replace(".pdf", "_thumb.jpg");

                certificado.Path = certificadoPath;
                certificado.Thumb = pathThumb;
                certificado.Principal = true;

                return certificado;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}