using Colaboracao.Core.Exceptions;
using Colaboracao.Infra.Context;
using Core.Domain.Formacao;
using DataTransferObject.Domain.Formacao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Formacao
{
    public class FormacaoColaboradorRepository : IFormacaoColaboradorRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public FormacaoColaboradorRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public void RemoverFormacaoColaborador(FormacaoColaboradorDTO formacaoColab)
        {
            try
            {
                var itemColabRow = _colaboradorContext.tb_colaborador_formacao
                    .Where(x => x.codigo_interno_colaborador == formacaoColab.ColaboradorCpf
                    && x.formacao_id == formacaoColab.IdFormacao && x.ativo == FormacaoConstants.ATIVO)
                    .FirstOrDefault()
                    ?? throw new KeyNotFoundException("Colaborador não possui esta formação.");

                itemColabRow.ativo = FormacaoConstants.NAO_ATIVO;
                _colaboradorContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<FormacaoColaboradorDTO> ListFormacaoColaborador(FormacaoColaboradorDTO formacaoColab)
        {
            var ret = new List<FormacaoColaboradorDTO>();

            var rows = _colaboradorContext.tb_colaborador_formacao
                .Where(x => x.codigo_interno_colaborador == formacaoColab.ColaboradorCpf && x.ativo == FormacaoConstants.ATIVO)
                .OrderBy(x => x.id)
                .ToList()
                ?? throw new Exception("Erro ao consultar a formação do colaborador!");

            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var retorno = new FormacaoColaboradorDTO()
                    {
                        Id = row.id,
                        Emissor = row.emissor,
                        DataConclusao = row.data_conclusao,
                        ColaboradorCpf = row.codigo_interno_colaborador,
                        IdNivel = row.tb_nivel_id ?? 0,
                        Data = row.data_alteracao,
                        IdCertificado = row.tb_certificado_id ?? 0,
                        Formacao =
                            {
                                Id = row.formacao.id,
                                Descricao = row.formacao.descricao
                            },
                    };
                    if (row.tb_certificado_id != null)
                    {
                        retorno.IdCertificado = row.tb_certificado_id;
                    }
                    else
                    {
                        retorno.Certificado = null;
                    }
                    ret.Add(retorno);
                }
            }
            return ret;
        }
        public FormacaoColaboradorDTO CadastrarFormacaoColaborador(FormacaoColaboradorDTO formacaoColab)
        {
            try
            {
                var colabRow = ValidaColaborador(formacaoColab);
                var itemRow = ValidarFormacao(formacaoColab);
                var nivelRow = ValidaNivel(formacaoColab);
                var formacaoColabIdRow = GetFormacaoColaborador(formacaoColab);
                if (formacaoColabIdRow != null)
                {
                    formacaoColab.Id = formacaoColabIdRow.Id;
                }

                var row = new tb_colaborador_formacao();
                row.ativo = FormacaoConstants.ATIVO;
                row.codigo_interno_colaborador = colabRow.codigo_interno_colaborador;
                row.formacao_id = itemRow.id;
                row.tb_nivel_id = nivelRow.id;
                row.data_conclusao = formacaoColab.DataConclusao;
                row.emissor = formacaoColab.Emissor;

                _colaboradorContext.tb_colaborador_formacao.Add(row);
                _colaboradorContext.SaveChanges();

                formacaoColab.Id = row.id;
                return formacaoColab;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private FormacaoColaboradorDTO GetFormacaoColaborador(FormacaoColaboradorDTO formacaoColab)
        {
            var item = _colaboradorContext.tb_colaborador_formacao
                .Where(x => x.codigo_interno_colaborador == formacaoColab.ColaboradorCpf
                && x.formacao_id == formacaoColab.IdFormacao).FirstOrDefault();
            if (item != null)
            {
                if (item.ativo == FormacaoConstants.ATIVO)
                {
                    item.emissor = formacaoColab.Emissor;
                    item.tb_nivel_id = formacaoColab.IdNivel;
                    item.data_conclusao = formacaoColab.DataConclusao;
                    _colaboradorContext.SaveChanges();
                    throw new ValidationException("Colaborador já possui esta formação");
                }
                else
                {
                    item.ativo = FormacaoConstants.ATIVO;
                    item.tb_nivel_id = formacaoColab.IdNivel;
                    _colaboradorContext.SaveChanges();
                    formacaoColab.Id = item.id;
                    return formacaoColab;
                }
            }
            return null;
        }

        public tb_nivel ValidaNivel(FormacaoColaboradorDTO formacaoColab)
        {
            var item = _colaboradorContext.tb_nivel.Where(x => x.id == formacaoColab.IdNivel).FirstOrDefault()
                ?? throw new Exception("Nível não encontrado.");
            if (item.tb_item_perfil_id != FormacaoConstants.ITEM_PERFIL_ID_FORMACAO)
                throw new Exception("Nível não pertence a formação.");
            return item;
        }

        private tb_formacao ValidarFormacao(FormacaoColaboradorDTO formacaoColab)
        {
            return _colaboradorContext.tb_formacao.Find(formacaoColab.IdFormacao)
                ?? throw new ValidationException("Esta formação não existe.");
        }

        private tb_colaborador ValidaColaborador(FormacaoColaboradorDTO formacaoColab)
        {
            return (_colaboradorContext.tb_colaborador.Find(formacaoColab.ColaboradorCpf)
                ?? throw new ValidationException("Colaborador não existe."));
        }
    }
}