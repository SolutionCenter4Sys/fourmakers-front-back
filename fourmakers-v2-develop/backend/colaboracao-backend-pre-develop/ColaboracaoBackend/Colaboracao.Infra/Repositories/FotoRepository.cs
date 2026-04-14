using Colaboracao.Infra.Context;
using Core.DomainModel;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class FotoRepository : IFotoDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public FotoRepository(ColaboradorContext FotoContext)
        {
            this._colaboradorContext = FotoContext;
        }

        public long SaveFoto(string path)
        {
            try
            {
                var row = new tb_imagem();
                row.path = path;
                row.data_criacao = DateTime.Now;
                row.data_alteracao = DateTime.Now;
                row.ativo = 1;

                _colaboradorContext.tb_imagem.Add(row);
                _colaboradorContext.SaveChanges();

                return row.id;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
