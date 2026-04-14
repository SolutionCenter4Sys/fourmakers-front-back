using Colaboracao.Infra.Context;
using Core.Domain.IIdioma;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Idioma
{
    public class IdiomaNivelRepository : IIdiomaNivelRepository
    {
        private const int ID_ITEMPERFIL_IDIOMA = 9;
        private readonly ColaboradorContext _colaboradorContext;

        public IdiomaNivelRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }
        public NivelDTO GetModel(NivelDTO nivel)
        {
            try
            {
                var nivelBanco = _colaboradorContext.tb_nivel.FirstOrDefault(x => x.id == nivel.Id);

                nivel.Descricao = nivelBanco.descricao;

                return nivel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public NivelDTO GetByDescricao(string descricao)
        {
            try
            {
                var IdiomaNegocioId = _colaboradorContext.tb_item_perfil
                    .Where(x => x.descricao.Equals(descricao)).Select(x => new NivelDTO()
                    {
                        Id = x.id,
                        Descricao = descricao
                    }).FirstOrDefault();
                return IdiomaNegocioId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<NivelDTO> ListaNivelIdioma()
        {
            try
            {
                var ret = _colaboradorContext.tb_nivel.Where(x => x.tb_item_perfil_id == ID_ITEMPERFIL_IDIOMA)
                    .Select(x => new NivelDTO()
                    {
                        Id = x.id,
                        Descricao = x.descricao,
                        PrioridadeUnificacao = x.prioridade_unificacao,
                        OrdemExibicao = x.ordem_exibicao
                    })
                    .OrderBy(x => x.OrdemExibicao)
                    .ToList();

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}