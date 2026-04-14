using Colaboracao.Infra.Context;
using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class RotinaMigracaoRepository : IRotinaMigracaoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public RotinaMigracaoRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public List<string> ListaTodasImagensDoBanco()
        {
            try
            {
                var ret = new List<string>();
                var registros = _colaboradorContext.tb_imagem.Where(x => x.ativo == 1).ToList();

                foreach (var imagem in registros)
                {
                    ret.Add(imagem.path);
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> ListaTodosCertificadosDoBanco()
        {
            try
            {
                var ret = new List<string>();
                var registros = _colaboradorContext.tb_certificado.Where(x => x.ativo == 1).ToList();

                foreach (var certificado in registros)
                {
                    ret.Add(certificado.path);
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> ListaTodosCurriculosDeCandidatosDoBanco()
        {
            try
            {
                var ret = new List<string>();
                var registros = _colaboradorContext.tb_candidato.Where(x => x.ativo == 1).ToList();

                foreach (var curriculoCandidato in registros)
                {
                    ret.Add(curriculoCandidato.path_curriculo);
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}