using Colaboracao.Core.Interfaces;
using Core.Domain.TemplateEmail;
using Dapper;
using DataTransferObject.Domain.TemplateEmail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TemplateOrg.Constantes;

namespace Colaboracao.Infra.Repositories.TemplateEmail
{
    public class TemplateRepository : ITemplateRepository
    {
        private IConnectionStringCore _connectionString;

        public TemplateRepository(IConnectionStringCore connectionString)
        {
            this._connectionString = connectionString;
        }
        public TemplateEmailDTO BuscaTemplateEmail(TemplateOrgParametroEnum templateEnum, string idioma = null)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = "SELECT id, template FROM tb_template_email WHERE ativo = 1 AND codigo = @Codigo AND (@Idioma is null OR idioma = @Idioma)";
                    var result = _connection.QueryFirstOrDefault<EmailTemplateDTO>(query, new { Codigo = templateEnum.ToString(), Idioma = idioma });

                    if (result == null)
                        return null;

                    return new TemplateEmailDTO
                    {
                        Id = result.id,
                        Template = result.template
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("Template not found", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public void RegistraTemplateEmail(int orgId, string emails, string assunto, string templateParametrizado)
        {
            RegistraTemplateEmailAsync(orgId, emails, assunto, templateParametrizado).GetAwaiter().GetResult();
        }

        public async Task RegistraTemplateEmailAsync(int orgId, string emails, string assunto, string templateParametrizado)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string query = @"
                        INSERT INTO tb_template_email_rotina (id, assunto, corpo_email_parametrizado, emails, tb_org_id) VALUES (@Id, @Assunto, @CorpoEmailParametrizado, @Emails, @OrgId);
                    ";
                    int result = await _connection.ExecuteAsync(query,
                        new
                        {
                            Id = Guid.NewGuid(),
                            Assunto = assunto,
                            CorpoEmailParametrizado = templateParametrizado,
                            Emails = emails,
                            OrgId = orgId
                        }
                    );
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public List<EmailDTO> GetEmailPendente(int numeroMaximoTentativas)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string query = @"
                        SELECT id, emails, tb_org_id, assunto, corpo_email_parametrizado, numero_tentativas, mensagem_erro, data_criacao, data_disparo, data_alteracao
                        FROM tb_template_email_rotina WHERE data_disparo is null and numero_tentativas < @NumeroMaximoTentativas;
                    ";
                    List<dynamic> result = _connection.Query<dynamic>(query, new { NumeroMaximoTentativas = numeroMaximoTentativas }).ToList();
                    return result.Select(x => new EmailDTO
                    {
                        Id = x.id,
                        Assunto = x.assunto,
                        CorpoEmail = x.corpo_email_parametrizado,
                        Destinatarios = x.emails,
                        DataAlteracao = x.data_alteracao,
                        DataDisparo = x.data_disparo,
                        MensagemErro = x.mensagem_erro,
                        NumeroTentativas = x.numero_tentativas
                    }).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public void UpdateEmailPendente(EmailDTO emailInfo)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string query = @"
                        UPDATE tb_template_email_rotina set
                            numero_tentativas = @NumeroTentativas, mensagem_erro = @MensagemErro,
                            data_disparo = @DataDisparo, data_alteracao = @DataAlteracao
                        WHERE id = @Id;
                    ";
                    _connection.Execute(query, emailInfo);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}