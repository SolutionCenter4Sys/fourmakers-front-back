using Colaboracao.Infra.Context;
using Core.Domain.Formacao;
using DataTransferObject.Domain.Endosso;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Formacao
{
    public class FormacaoEndossoRepository : IFormacaoEndossoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public FormacaoEndossoRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public StatusEndossoDTO GetTipoEndosso(EndossoColaboradorDTO model)
        {
            try
            {
                var ret = new StatusEndossoDTO();
                var rowsEndosso = _colaboradorContext.tb_endosso_formacao
                    .Where(x => x.ativo == FormacaoConstants.ATIVO && x.colaborador_formacao_id == model.TipoEndossoId)
                    .ToList();

                if (rowsEndosso == null || rowsEndosso.Count() == 0) return new StatusEndossoDTO();

                var statusFinalizado = rowsEndosso.Where(x => x.tb_status_endosso_id == FormacaoConstants.STATUS_ENDOSSO_ID_CONFIRMADO)
                                                  .OrderByDescending(x => x.data_alteracao)
                                                  .FirstOrDefault();
                if (statusFinalizado != null)
                {
                    ret.Endossado = true;
                    ret.DescricaoStatus = statusFinalizado.tb_status_endosso_id == FormacaoConstants.STATUS_ENDOSSO_ID_CONFIRMADO ? FormacaoConstants.CONFIRMADO : FormacaoConstants.PENDENTE;
                }
                else
                {
                    ret.DescricaoStatus = rowsEndosso
                        .OrderByDescending(x => x.data_alteracao).First().tb_status_endosso_id == FormacaoConstants.STATUS_ENDOSSO_ID_CONFIRMADO ? FormacaoConstants.CONFIRMADO : FormacaoConstants.PENDENTE;
                    ret.Endossado = false;
                }

                var finalizados = rowsEndosso
                    .Where(x => x.tb_status_endosso_id == FormacaoConstants.STATUS_ENDOSSO_ID_CONFIRMADO)
                    .ToList();
                finalizados = finalizados
                    .Select(x =>
                    {
                        x.tb_tipo_endosso = _colaboradorContext.tb_tipo_endosso.Find(x.tb_tipo_endosso_id);
                        x.tb_status_endosso = _colaboradorContext.tb_status_endosso.Find(x.tb_status_endosso_id);
                        return x;
                    })
                    .ToList();

                if (finalizados != null && finalizados.Count() > 0)
                {
                    ret.NivelEndosso = finalizados
                        .Average(x => x.tb_tipo_endosso.nivel);
                    ret.QuantidadeSolicitacaoEndosso = rowsEndosso.Count();
                    ret.QuantidadeEndosso = rowsEndosso.Where(x => x.tb_status_endosso_id == FormacaoConstants.STATUS_ENDOSSO_ID_CONFIRMADO).Count();
                }
                else
                {
                    ret.NivelEndosso = 0;
                    ret.QuantidadeSolicitacaoEndosso = rowsEndosso.Count();
                    ret.QuantidadeEndosso = rowsEndosso.Where(x => x.tb_status_endosso_id == FormacaoConstants.STATUS_ENDOSSO_ID_CONFIRMADO).Count();
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