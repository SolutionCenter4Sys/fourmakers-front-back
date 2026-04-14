using Colaboracao.Infra.Context;
using Core.Domain.Formacao;
using DataTransferObject.Domain.Endosso;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Formacao
{
    public class FormacaoEndossoColaboradorRepository : IFormacaoEndossoColaboradorRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public FormacaoEndossoColaboradorRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }
        public List<EndossoColaboradorDTO> ListEndossoColaborador(EndossoColaboradorDTO model)
        {
            try
            {
                var ret = new List<EndossoColaboradorDTO>();

                var rows = _colaboradorContext.tb_endosso_formacao
                    .Where(x => x.colaborador_formacao_id == model.TipoEndossoId
                    && x.ativo == FormacaoConstants.ATIVO
                    && x.tb_status_endosso_id == FormacaoConstants.STATUS_ENDOSSO_ID_CONFIRMADO)
                    .ToList();
                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var tipoEndosso = _colaboradorContext.tb_tipo_endosso
                    .Where(x => x.id == row.tb_tipo_endosso_id)
                    .Select(x => new TipoEndossoDTO()
                    {
                        Descricao = x.descricao,
                        Id = x.id,
                        NivelEndosso = x.nivel
                    }).FirstOrDefault();

                        var formacaoEndosso = new EndossoColaboradorDTO()
                        {
                            ColaboradorCpf = row.codigo_interno_colaborador,
                            DataEndosso = row.data_alteracao,
                            TipoEndossoId = row.tb_tipo_endosso_id,
                            TipoEndosso = tipoEndosso
                        };
                        ret.Add(formacaoEndosso);
                    }
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