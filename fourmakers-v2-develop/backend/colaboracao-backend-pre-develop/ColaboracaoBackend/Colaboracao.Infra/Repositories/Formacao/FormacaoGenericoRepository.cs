using Colaboracao.Infra.Context;
using Core.Domain.Formacao;
using DataTransferObject.Domain.Formacao;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Formacao
{
    public class FormacaoGenericoRepository : IFormacaoGenericoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public FormacaoGenericoRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public FormacaoColaboradorDTO AtualizaFormacaoColaborador(FormacaoColaboradorDTO formacaoColab)
        {
            {
                try
                {
                    var colabRow = _colaboradorContext.tb_colaborador
                        .Find(formacaoColab.ColaboradorCpf);
                    var formacaoRow = _colaboradorContext.tb_formacao
                        .Find(formacaoColab.Formacao.Id)
                        ?? throw new Exception("Formação não encontrada.");

                    var formacaoColaboradorRow = formacaoRow.tb_colaborador_formacao
                        .Where(x => x.formacao_id == formacaoRow.id && x.codigo_interno_colaborador == formacaoColab.ColaboradorCpf)
                        .FirstOrDefault();
                    if (formacaoColaboradorRow == null || formacaoColaboradorRow.codigo_interno_colaborador != formacaoColab.ColaboradorCpf)
                    {
                        throw new Exception("Colaborador não possui esta formação");
                    }

                    tb_nivel nivelRow = null;
                    if (formacaoColab.IdNivel != null)
                    {
                        nivelRow = _colaboradorContext.tb_nivel
                            .Find(formacaoColab.IdNivel)
                            ?? throw new Exception("Nível do item não encontrado.");
                        if (nivelRow.tb_item_perfil.descricao != "FORMACAO")
                            throw new Exception("Nível do item não pertence a formação.");
                    }
                    formacaoColaboradorRow.tb_nivel = nivelRow;

                    if (formacaoColaboradorRow.emissor != formacaoColab.Emissor)
                    {
                        formacaoColaboradorRow.emissor = formacaoColab.Emissor;
                    }

                    if (formacaoColaboradorRow.data_conclusao != formacaoColab.DataConclusao)
                    {
                        formacaoColaboradorRow.data_conclusao = formacaoColab.DataConclusao;
                    }

                    _colaboradorContext.SaveChanges();

                    formacaoColab.Id = formacaoColaboradorRow.id;

                    return formacaoColab;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}