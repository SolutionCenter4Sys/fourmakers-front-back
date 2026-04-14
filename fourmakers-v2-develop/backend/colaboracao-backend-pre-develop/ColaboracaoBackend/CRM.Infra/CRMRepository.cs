using Colaboracao.Helper;
using Core.Domain;
using Dapper;
using DataTransferObject.Domain.CRM;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Infra
{
    public class CRMRepository : ICRMRepository
    {
        private MySqlConnection CreateMySqlConnectionFromEnv() => new(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CRM_STRING_CONNECTION"));

        public CRMContatoResponsavelOutputDTO BuscarContatoOportunidade(string oportunidade)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@oportunidade", oportunidade);
                    var potentialQueryResult = db.QuerySingle<dynamic>("SELECT * FROM vtiger_potential where potential_no = @oportunidade",
                        param: parametro, commandType: System.Data.CommandType.Text);
                    if (potentialQueryResult == null)
                    {
                        throw new Exception("Oportunidade não encontrada");
                    }
                    var relatedTo = potentialQueryResult.related_to;
                    parametro = new DynamicParameters();
                    parametro.Add("@contactId", relatedTo);
                    var contatoQueryResult = db.QuerySingle<dynamic>("select * from vtiger_contactdetails where contactid = @contactId",
                        param: parametro, commandType: System.Data.CommandType.Text);
                    if (contatoQueryResult == null)
                    {
                        throw new Exception("Contato não encontrado");
                    }

                    var cRMContatoResponsavelDTO = new CRMContatoResponsavelOutputDTO()
                    {
                        Nome = contatoQueryResult.firstname,
                        SobreNome = contatoQueryResult.lastname,
                        Email = contatoQueryResult.email,
                        Celular = contatoQueryResult.mobile
                    };

                    return cRMContatoResponsavelDTO;
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public string BuscarOportunidade(string cotacao)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@cotacao", cotacao);
                    var quoteQueryResult = db.QuerySingle<dynamic>("SELECT * FROM vtiger_quotes where quote_no = @cotacao",
                        param: parametro, commandType: System.Data.CommandType.Text);
                    if (quoteQueryResult == null)
                    {
                        throw new Exception("cotação não encontrada");
                    }
                    parametro = new DynamicParameters();
                    parametro.Add("@potentialId", quoteQueryResult.potentialid);
                    var potentialQueryResult = db.QuerySingle<dynamic>("SELECT * FROM vtiger_potential where potentialid = @potentialId",
                        param: parametro, commandType: System.Data.CommandType.Text);
                    var potentialNo = potentialQueryResult.potential_no;
                    return potentialNo;
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public async Task<IEnumerable<VwClienteEnderecoDTO>> GetClientesCrm()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    return await db.QueryAsync<VwClienteEnderecoDTO>("SELECT * FROM vw_clientes");
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<VwClienteEnderecoDTO> GetClientePorAccountId(string accountId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                var query = @"SELECT
                                      accountid AS account_id,
                                      account_no AS account_no,
                                      accountname AS nome_conta,
                                      CONCAT(account_no,
                                              '-',
                                              industry) AS atividade
                                  FROM
                                      vtiger_account
                                  WHERE
                                      accountId = @AccountId";

                db.Open();
                return await db.QuerySingleOrDefaultAsync<VwClienteEnderecoDTO>(query, new { AccountId = accountId });
            }
        }

        public async Task<IEnumerable<VwClienteEnderecoDTO>> GetClientesFourmakersCrm()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();

                    return await db.QueryAsync<VwClienteEnderecoDTO>(@"select distinct 
	                                                                        vce.account_id_crm as account_id,
	                                                                        vce.account_no as account_no,
	                                                                        vce.nome_conta as nome_conta,
	                                                                        va.cf_726 as codigo_cch
                                                                        from
	                                                                        vw_clientes_enderecos vce
                                                                        left join
   	                                                                        vtiger_accountscf va ON	va.accountid = vce.account_id_crm;");
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<ContactDetailsDTO>> GetGestoresPorAccountNos(IEnumerable<string> accountNos)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();

                    var query = @"SELECT
                                    vcd.contactid,
                                	vcd.contact_no as contactno,
                                	vcd.accountid,
                                	vcd.salutation,
                                	vcd.firstname,
                                	vcd.lastname,
                                	vcd.email,
                                	vcd.phone,
                                	vcd.mobile,
                                	vcd.title,
                                	vcd.department,
                                    va.account_no as accountno,
                                    SUBSTRING_INDEX(vc.cf_620, '/', -1) as linkedin,
                                    vc.cf_703 as destituido
                                FROM
                                    vtiger_contactdetails vcd
                                JOIN
                                	vtiger_account va ON vcd.accountid = va.accountid
                                LEFT JOIN
                                    vtiger_contactscf vc ON vcd.contactid = vc.contactid
                                WHERE
                                    va.account_no IN @AccountNos AND vc.cf_703 = 0;";

                    return await db.QueryAsync<ContactDetailsDTO>(query, new { AccountNos = accountNos });
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<VwCotacoesDTO>> GetCotacoesByCrmId(string crmId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@crmId", crmId);
                    return await db.QueryAsync<VwCotacoesDTO>("SELECT * FROM vw_cotacoes WHERE CrmId = @crmId", param: parametro);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<VwCotacoesDTO>> GetCotacoesLastTwoYearsByAccountId(int accountId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var anoAtual = "P" + (DateTime.Now.Year);
                    var anoPassado = "P" + (DateTime.Now.Year - 1);

                    var parametro = new DynamicParameters();
                    parametro.Add("@accountId", accountId);
                    parametro.Add("@anoAtual", $"{anoAtual}%");
                    parametro.Add("@anoPassado", $"{anoPassado}%");

                    return await db.QueryAsync<VwCotacoesDTO>(
                        "SELECT * FROM vw_cotacoes WHERE Account_Id = @accountId AND (Quote_No LIKE @anoAtual OR Quote_No LIKE @anoPassado)",
                        param: parametro);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<VwCotacoesIncluindoAccountNoDTO>> GetCotacoesIncluindoAccountNoByQuoteNos(List<string> quoteNos)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();

                    var query = @"
                                    SELECT
                                        vc.quote_id AS Quote_Id,
                                        vc.quote_no AS Quote_No,
                                        vc.assunto AS Assunto,
                                        vc.estagio AS Estagio,
                                        vc.estagio_cotacao AS Estagio_Cotacao,
                                        vc.estagio_cotacao_pt AS Estagio_Cotacao_Pt,
                                        vc.account_id AS Account_Id,
                                        vc.nome_conta AS Nome_Conta,
                                        vc.contact_id AS Contact_Id,
                                        vc.nome_contato AS Nome_Contato,
                                        vc.nome_oportunidade AS Nome_Oportunidade,
                                        vc.potential_id AS Potential_Id,
                                        vc.unidade AS Unidade,
                                        va.account_no AS account_no,
                                        va.accountname AS nome_conta
                                    FROM
                                        vw_cotacoes vc
                                    JOIN
                                        vtiger_account va ON vc.account_id = va.accountid
                                    WHERE
                                        quote_no IN @QuoteNos";

                    return await db.QueryAsync<VwCotacoesIncluindoAccountNoDTO>(query, new { QuoteNos = quoteNos });
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar cotações por QuoteNos", ex);
                }
            }
        }
    }
}