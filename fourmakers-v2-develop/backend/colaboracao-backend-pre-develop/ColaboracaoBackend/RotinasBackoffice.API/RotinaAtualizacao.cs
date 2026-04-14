using ApiClient.Domain.Interfaces;
using Dapper;
using DataTransferObject.Domain.CCH;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice.API
{
    public class RotinaAtualizacao : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        
        public RotinaAtualizacao(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SincronizarDadosSrsCchGcolb();
        }

        public async Task SincronizarDadosSrsCchGcolb()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var cchClient = scope.ServiceProvider.GetRequiredService<ICCHClient>();
                var tokenCCH = (await cchClient.Autenticacao()).token;
                var connectionString = "server=10.10.12.58;port=3306;database=cats;uid=root;pwd=F0ursysPa$$2021;AllowZeroDateTime=True;";

                using (var con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            var colabsRows = con.Query("select * from candidate where is_active = 1", commandType: System.Data.CommandType.Text, transaction: transaction).ToList();
                            foreach (var colab in colabsRows)
                            {
                                if (!String.IsNullOrEmpty(colab.emailFoursys))
                                {
                                    List<ColaboradorCCH> retCch = await cchClient.Validacao(colab.emailFoursys, tokenCCH);
                                    if (retCch != null && retCch.Count() > 0 && retCch.Where(x => x.flFuncionarioAtivo == 1).Any())
                                    {
                                        var cpfCCh = retCch.Where(x => x.flFuncionarioAtivo == 1).LastOrDefault().cdCpf;
                                        if (String.IsNullOrEmpty(colab.cand_cpf))
                                        {
                                            continue;
                                        }
                                        cpfCCh = new string(cpfCCh.Where(c => char.IsDigit(c)).ToArray());
                                        var cpfSrs = new string((colab.cand_cpf as string).Where(c => char.IsDigit(c)).ToArray());
                                        if (cpfCCh != cpfSrs)
                                        {
                                            var obj = new
                                            {
                                                Cpf = cpfCCh,
                                                Email = colab.emailFoursys
                                            };
                                            var sqlStatement = @"
                                            UPDATE candidate
                                            SET
                                                cand_cpf = @Cpf
                                            WHERE emailFoursys = @Email";
                                            con.Execute(sqlStatement, obj, transaction: transaction);
                                            Console.WriteLine(colab.first_name + ";" + cpfSrs + ";" + cpfCCh);
                                        }
                                    }
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
            }
        }
    }
}