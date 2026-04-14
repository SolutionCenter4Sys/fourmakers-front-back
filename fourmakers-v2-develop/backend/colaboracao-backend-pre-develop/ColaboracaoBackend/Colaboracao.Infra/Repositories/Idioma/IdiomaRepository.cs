using Colaboracao.Infra.Context;
using Core.Domain.IIdioma;
using DataTransferObject.Domain.Idioma;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Idioma
{
    public class IdiomaRepository : IIdiomaRepository
    {
        private const int ATIVO = 1;
        private readonly ColaboradorContext _colaboradorContext;
        private readonly string MSG_IDIOMA_NAO_ENCONTRADO = "Idioma não encontrado.";
        private readonly string MSG_NENHUM_IDIOMA_ENCONTRADO = "Nenhum idioma encontrado.";
        private readonly string MSG_IDIOMA_JA_EXISTE = "Esse idioma já existe.";

        public IdiomaRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public IdiomaColaboradorDTO AdicionarIdiomaColaborador(IdiomaColaboradorDTO idioma)
        {
            throw new NotImplementedException();
        }

        public void DeleteIdioma(IdiomaDTO idioma)
        {
            throw new NotImplementedException();
        }

        public IdiomaDTO GetIdioma(IdiomaDTO idioma)
        {
            var IdiomaBanco = _colaboradorContext.tb_idioma.Where(x => x.id == idioma.Id).FirstOrDefault()
                ?? throw new Exception(MSG_IDIOMA_NAO_ENCONTRADO);

            idioma.Descricao = IdiomaBanco.descricao;
            return idioma;
        }

        public List<IdiomaDTO> ListarIdiomas(string busca, int cursor, int limite)
        {
            var ret = new List<IdiomaDTO>();

            if (String.IsNullOrEmpty(busca))
            {
                ret = _colaboradorContext.tb_idioma.Where(x => x.ativo == 1)
                    .OrderBy(x => x.descricao)
                    .Skip(cursor)
                    .Take(limite)
                    .Select(x => new IdiomaDTO
                    {
                        Id = x.id,
                        Descricao = x.descricao,
                        Pendente = !Convert.ToBoolean(x.confirmada)
                    })
                    .ToList()
                    ?? new List<IdiomaDTO>();
            }
            else
            {
                ret = _colaboradorContext.tb_idioma
                    .Where(x => x.ativo == 1 && EF.Functions.Like(x.descricao.ToUpper(), "%" + busca.ToUpper() + "%"))
                    .OrderBy(x => x.descricao)
                    .Skip(cursor)
                    .Take(limite)
                    .Select(x => new IdiomaDTO
                    {
                        Id = x.id,
                        Descricao = x.descricao,
                        Pendente = !Convert.ToBoolean(x.confirmada)
                    })
                    .ToList()
                    ?? new List<IdiomaDTO>();
            }

            return ret;
        }

        public IdiomaDTO AdicionarIdioma(IdiomaDTO idioma)
        {
            if (_colaboradorContext.tb_idioma.Any(x => x.descricao.ToUpper() == idioma.Descricao.ToUpper() && x.ativo == 1))
                throw new ArgumentException(MSG_IDIOMA_JA_EXISTE);

            var oldRow = _colaboradorContext.tb_idioma.Where(x => x.descricao.ToUpper() == idioma.Descricao.ToUpper()).FirstOrDefault();
            if (oldRow == null)
            {
                var row = new tb_idioma
                {
                    ativo = ATIVO,
                    descricao = idioma.Descricao,
                    confirmada = 0
                };

                _colaboradorContext.tb_idioma.Add(row);
                _colaboradorContext.SaveChanges();
                idioma.Id = row.id;
            }
            else
            {
                oldRow.ativo = 1;
                _colaboradorContext.tb_idioma.Update(oldRow);
                _colaboradorContext.SaveChanges();
                idioma.Id = oldRow.id;
            }

            return idioma;
        }

        public IdiomaDTO GetIdiomaById(int id)
        {
            var idioma = new IdiomaDTO();
            var IdiomaBanco = _colaboradorContext.tb_idioma.Where(x => x.id == id && x.ativo == 1).FirstOrDefault()
                ?? throw new ArgumentException(MSG_IDIOMA_NAO_ENCONTRADO);
            idioma.Descricao = IdiomaBanco.descricao;
            idioma.Id = id;
            idioma.Pendente = !Convert.ToBoolean(IdiomaBanco.confirmada);
            return idioma;
        }

        public List<int> ListarIdiomasAtribuidos(string cpfColaborador)
        {
            try
            {
                var listaSoftskillsExistentes = new List<int>();

                listaSoftskillsExistentes = _colaboradorContext.tb_colaborador_idioma
                                            .Join(_colaboradorContext.tb_idioma, cc => cc.idioma_id, c => c.id, (cc, c) => new { cc, c })
                                            .Where(x => x.cc.codigo_interno_colaborador == cpfColaborador && x.c.ativo == 1 && x.cc.ativo == 1)
                                            .Select(x => x.cc.idioma_id)
                                            .ToList();

                return listaSoftskillsExistentes;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}