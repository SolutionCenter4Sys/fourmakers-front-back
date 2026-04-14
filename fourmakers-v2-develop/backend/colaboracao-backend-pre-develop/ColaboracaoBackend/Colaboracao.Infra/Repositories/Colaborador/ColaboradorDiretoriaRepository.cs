using Colaboracao.Infra.Context;
using Core.Domain.Colaborador;
using DataTransferObject.Domain.Diretoria;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class ColaboradorDiretoriaRepository : IColaboradorDiretoriaRepository
    {
        private ColaboradorContext _colaboradorContext;

        public ColaboradorDiretoriaRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public List<DiretoriaColaboradorDTO> ListarDiretoriaDosColaboradores(int orgId, List<string>? restricaoDiretorias = null)
        {
            try
            {
                var query = _colaboradorContext.tb_colaborador_org
                    .Where(c => c.tb_org_id == orgId &&
                               c.cod_diretoria != null &&
                               c.diretoria != null &&
                               c.cod_diretoria.Trim() != "" &&
                               c.diretoria.Trim() != "");

                if (restricaoDiretorias != null && restricaoDiretorias.Count > 0)
                    query = query.Where(c => restricaoDiretorias.Contains(c.cod_diretoria));

                var lista = query
                    .GroupBy(c => new { c.cod_diretoria, c.diretoria })
                    .Select(g => new DiretoriaColaboradorDTO
                    {
                        Cod = g.Key.cod_diretoria,
                        Diretoria = g.Key.diretoria
                    })
                    .ToList();
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao listar diretorias dos colaboradores: " + ex.Message, ex);
            }
        }
    }
}