using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.Formacao;
using Dapper;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Formacao;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Formacao
{
    public class FormacaoRepository : IFormacaoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IFormacaoGenericoRepository _repositoryFormacaoGenerico;
        private readonly IConnectionStringCore _connectionStringCore;

        public FormacaoRepository(ColaboradorContext colaboradorContext, IFormacaoGenericoRepository repositoryFormacaoGenerico, IConnectionStringCore connectionStringCore)
        {
            _colaboradorContext = colaboradorContext;
            _repositoryFormacaoGenerico = repositoryFormacaoGenerico;
            _connectionStringCore = connectionStringCore;
        }

        public long? GetFormacaoColabRowId(long formacaoColaboradorId)
        {
            return _colaboradorContext.tb_colaborador_formacao.Find(formacaoColaboradorId)?.id;
        }

        public FormacaoDTO GetModel(FormacaoDTO model)
        {
            var formacaoBanco = _colaboradorContext.tb_formacao.Where(x => x.id == model.Id).FirstOrDefault()
                ?? throw new Exception("Formacao não encontrada.");
            model.Descricao = formacaoBanco.descricao;
            return model;
        }

        public FormacaoDTO GetFormacaoByCpf(string key)
        {
            try
            {
                var userRow = _colaboradorContext.tb_usuario.FirstOrDefault(x => x.codigo_interno_colaborador.Equals(key));
                var model = new FormacaoDTO();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<FormacaoDTO> ListarFormacao(string busca, int cursor, int limite)
        {
            try
            {
                IQueryable<tb_formacao> query = _colaboradorContext.tb_formacao
               .Where(x => x.ativo == FormacaoConstants.ATIVO);

                if (!string.IsNullOrEmpty(busca))
                    query = query.Where(x => EF.Functions.Like(x.descricao.ToUpper(), $"%{busca.ToUpper()}%"));

                return query.OrderBy(x => x.descricao)
                    .Skip(cursor)
                    .Take(limite)
                    .Select(x => new FormacaoDTO()
                    {
                        Id = x.id,
                        Descricao = x.descricao
                    })
                    .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long SaveCertificado(CertificadoDTO certificado, long formacaoColaboradorId)
        {
            var certificadoRow = new tb_certificado();
            certificadoRow.ativo = FormacaoConstants.ATIVO;
            certificadoRow.path = certificado.Path;
            _colaboradorContext.tb_certificado.Add(certificadoRow);
            _colaboradorContext.SaveChanges();

            var listaRegistro = _colaboradorContext.tb_colaborador_formacao
                .Where(x => x.id == formacaoColaboradorId && x.ativo == 1);

            if (listaRegistro.Any())
            {
                var registro = listaRegistro.Single();

                registro.tb_certificado = certificadoRow;
            }
            else
            {
                var formacaoColabRow = new tb_colaborador_formacao();
                formacaoColabRow.ativo = 1;
                formacaoColabRow.tb_certificado = certificadoRow;
                _colaboradorContext.tb_colaborador_formacao.Add(formacaoColabRow);
            }

            var pathFile = certificado.Path;

            if (pathFile.Contains(".png"))
            {
                certificadoRow.path = pathFile.Replace(".png", "/" + certificadoRow.id.ToString() + ".png");
            }
            else
            {
                certificadoRow.path = pathFile.Replace(".pdf", "/" + certificadoRow.id.ToString() + ".pdf");
            }
            _colaboradorContext.SaveChanges();
            certificado.Path = certificadoRow.path;

            return certificadoRow.id;
        }

        public FormacaoDTO AddFormacao(FormacaoDTO formacao, string cpfUsuarioCriacao)
        {
            try
            {
                tb_usuario userRow = ValidarUsuario(cpfUsuarioCriacao);
                var formacaoRow = _colaboradorContext.tb_formacao
                    .FirstOrDefault(x => x.descricao.ToUpper() == formacao.Descricao
                    .ToUpper());
                if (formacaoRow != null && formacaoRow.ativo != FormacaoConstants.ATIVO)
                {
                    formacaoRow.ativo = FormacaoConstants.ATIVO;
                    _colaboradorContext.SaveChanges();
                    formacao.Id = formacaoRow.id;
                    return formacao;
                }
                formacaoRow = new tb_formacao
                {
                    ativo = FormacaoConstants.ATIVO,
                    descricao = formacao.Descricao,
                    usuario_criacao = userRow
                };
                _colaboradorContext.tb_formacao.Add(formacaoRow);
                _colaboradorContext.SaveChanges();
                formacao.Id = formacaoRow.id;
                return formacao;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private tb_usuario ValidarUsuario(string cpf)
        {
            return _colaboradorContext.tb_usuario.Where(x => x.codigo_interno_colaborador == cpf)
                .FirstOrDefault()
                ?? throw new Exception("Usuário não encontrado");
        }

        public FormacaoDTO GetFormacaoById(int formacaoId)
        {
            try
            {
                return _colaboradorContext.tb_formacao.Where(x => x.id.Equals(formacaoId))
                    .Select(x => new FormacaoDTO()
                    {
                        Id = x.id,
                        Descricao = x.descricao
                    }).FirstOrDefault()
                    ?? throw new ArgumentNullException("Formacao nao encontrada!");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FormacaoColaboradorDTO AlterarFormacaoColaborador(FormacaoColaboradorDTO formacaoColab)
        {
            return _repositoryFormacaoGenerico.AtualizaFormacaoColaborador(formacaoColab);
        }

        public async Task<List<FormacaoSumarioDTO>> ListarFormacaoSumario(int orgId)
        {
            using (var _connection = _connectionStringCore.CreateMySqlConnection())
            {
                try
                {
                    string sql = @$"
                                    SELECT
                                        COUNT(te.codigo_interno_colaborador) AS qtd,
                                        tf.descricao,
                                        te.tb_formacao_id as formacao_id,
                                        CASE WHEN te.descricao IS NULL THEN ""Não definido"" ELSE te.descricao END AS nivel
                                    FROM
                                        tb_escolaridade te
                                    JOIN
                                        tb_colaborador_org tco ON te.codigo_interno_colaborador = tco.codigo_interno_colaborador
									JOIN
										tb_formacao tf ON tf.id = te.tb_formacao_id
                                    WHERE
                                        te.ativo = 1 AND tco.tb_org_id = {orgId} AND tco.ativo = 1
                                    GROUP BY
                                        tf.descricao, te.descricao;
                                ;";

                    _connection.Open();

                    var resultdb = await _connection.QueryAsync<dynamic>(sql);

                    var result = resultdb
                                .GroupBy(x => x.nivel)
                                .Select(group => new FormacaoSumarioDTO
                                {
                                    NivelEscolaridade = group.Key,
                                    QtdUsuarios = group.Sum(x => (int)x.qtd),
                                    Cursos = resultdb.Where(x => x.nivel == group.First().nivel).Select(curso => new EscolaridadeSumarioDTO()
                                    {
                                        NivelEscolaridade = group.Key,
                                        CdFormacao = curso.formacao_id,
                                        DescricaoFormacao = curso.descricao,
                                        QtdUsuarios = resultdb.Where(x => x.descricao == curso.descricao && x.nivel == group.First().nivel).Sum(x => (int)x.qtd)
                                    }).ToList()
                                })
                                .OrderByDescending(x => x.QtdUsuarios)
                                .ToList();

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                }
            }
        }
    }
}