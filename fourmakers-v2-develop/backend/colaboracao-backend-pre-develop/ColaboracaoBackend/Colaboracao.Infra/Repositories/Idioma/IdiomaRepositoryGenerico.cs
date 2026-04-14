using Colaboracao.Infra.Context;
using Core.Domain.IIdioma;
using DataTransferObject.Domain.Idioma;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Idioma
{
    public class IdiomaRepositoryGenerico : IIdiomaRepositoryGenerico
    {
        private readonly ColaboradorContext _colaboradorContext;

        public IdiomaRepositoryGenerico(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }
        public IdiomaColaboradorDTO AlterarIdiomaColaborador(IdiomaColaboradorDTO idiomaColaborador)
        {
            try
            {
                var colabRow = _colaboradorContext.tb_colaborador.Find(idiomaColaborador.ColaboradorCpf);
                var IdiomaRow = _colaboradorContext.tb_idioma.Find(idiomaColaborador.Idioma.Id);

                if (IdiomaRow == null)
                    throw new Exception("Idioma não encontrado.");

                var IdiomaColaboradorRow = IdiomaRow.tb_colaborador_idioma
                    .Where(x => x.idioma_id == IdiomaRow.id && x.codigo_interno_colaborador == idiomaColaborador.ColaboradorCpf).FirstOrDefault();

                if (IdiomaColaboradorRow == null || IdiomaColaboradorRow.codigo_interno_colaborador != idiomaColaborador.ColaboradorCpf)
                {
                    throw new Exception("Colaborador não possuí este Idioma");
                }

                tb_nivel nivelRow = null;
                if (idiomaColaborador.Nivel.Id != null)
                {
                    nivelRow = _colaboradorContext.tb_nivel.Find(idiomaColaborador.Nivel.Id);
                    if (nivelRow == null)
                        throw new Exception("Nível do item não encontrado.");
                    else if (nivelRow.tb_item_perfil.descricao != "IDIOMA")
                        throw new Exception("Nível do item não pertence ao idioma.");
                }

                if (nivelRow != null)
                    IdiomaColaboradorRow.tb_nivel = nivelRow;

                _colaboradorContext.SaveChanges();

                idiomaColaborador.Id = IdiomaColaboradorRow.id;

                return idiomaColaborador;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}