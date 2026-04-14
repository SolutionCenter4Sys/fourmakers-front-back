using Colaboracao.Infra.Context;
using Core.Domain.Formacao;
using DataTransferObject.Domain.Formacao;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Formacao
{
    public class FormacaoNivelRepository : IFormacaoNivelRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public FormacaoNivelRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public NivelDTO GetNivel(NivelDTO model)
        {
            try
            {
                model.Descricao = _colaboradorContext.tb_nivel.Where(x => x.id == model.Id)
                    .Select(x => x.descricao)
                    .FirstOrDefault()
                   ?? throw new Exception("Nivel não encontrado.");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public NivelDTO GetNivelByKey(string key)
        {
            return _colaboradorContext.tb_item_perfil
                   .Where(x => x.descricao.Equals(key)).Select(X => new NivelDTO()
                   {
                       Id = X.id
                   })
                   .FirstOrDefault()
                   ?? throw new Exception("Nivel não encontrado.");
        }

        public List<NivelDTO> ListNivel(NivelDTO model)
        {
            try
            {
                return _colaboradorContext.tb_nivel
                        .Where(x => x.tb_item_perfil_id == model.Id)
                    .Select(x => new NivelDTO()
                    {
                        Id = x.id,
                        Descricao = x.descricao
                    })
                    .ToList()
                ?? throw new Exception("Erro ao buscar niveis!");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<FormacaoNivel> ListNivelFormacao(NivelDTO model)
        {
            return _colaboradorContext.tb_nivel
                .Where(x => x.tb_item_perfil_id == model.Id)
                .Select(x => new FormacaoNivel()
                {
                    Formacao_id = x.id,
                    FormacaoDescricao = x.descricao
                })
                .ToList()
            ?? throw new Exception("Erro ao buscar niveis!");
        }
    }
}