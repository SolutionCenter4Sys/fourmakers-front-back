using Dapper;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice
{
    public class FakerDadosBackoffice : BackgroundService
    {
        public FakerDadosBackoffice()
        {
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Começando a unanimização dos dados de colaboradores no SRS");
            //UnanimizarDadosBackoffice();
            //UnanimizarDadosGcolb();
            //SincronizarDadosBackoffice();
        }

        public void SincronizarDadosBackoffice()
        {
            var gcolbConnectionString = "server=10.10.12.58;port=3306;database=gcolb_hml;uid=usuariohml;pwd=F0rsys*@@2021;SslMode=none;AllowZeroDateTime=True;";
            var srsConnectionString = "server=10.10.10.223;port=3306;database=cats;uid=cats;pwd=cats;SslMode=none;AllowZeroDateTime=True;";

            using (var con = new MySqlConnection(gcolbConnectionString))
            {
                try
                {
                    con.Open();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            using (var conSRS = new MySqlConnection(srsConnectionString))
                            {
                                try
                                {
                                    conSRS.Open();
                                    var colabsRows = con.Query("select * from tb_colaborador", commandType: System.Data.CommandType.Text, transaction: transaction).ToList();
                                    foreach (var colab in colabsRows)
                                    {
                                        var userRow = con.Query(@"select * from tb_usuario where
                                            cpf = @Cpf", new { Cpf = colab.cpf },
                                            commandType: System.Data.CommandType.Text, transaction: transaction).ToList();
                                        if (userRow == null || userRow.Count == 0)
                                            continue;
                                        var srsColabs = conSRS.Query(@"select
                                            first_name,
                                            phone_home,
                                            phone_cell,
                                            address,
                                            address_number,
	                                        address_complement,
                                            district,
                                            city,
                                            state,
                                            zip,
                                            email1,
                                            cand_rg,
                                            cand_estCivil,
                                            dataNascimento,
                                            uniresp,
                                            genero,
                                            orientacao_sexual,
                                            etnia,
                                            pessoa_refugiada,
                                            nivel_escolaridade
                                            from candidate where
                                            is_active = 1 AND emailFoursys = @Email AND cand_cpf = @Cpf", new { Cpf = colab.cpf, Email = userRow.ElementAt(0).email },
                                            commandType: System.Data.CommandType.Text).ToList();
                                        if (srsColabs == null || srsColabs.Count == 0)
                                            continue;

                                        var srsRow = srsColabs.ElementAt(0);
                                        if (colab.endereco_id != null)
                                        {
                                            var objEnd = new
                                            {
                                                Id = colab.endereco_id,
                                                Endereco = srsRow.address,
                                                Cidade = srsRow.city,
                                                Estado = srsRow.state,
                                                Numero = srsRow.address_number,
                                                Complemento = srsRow.address_complement,
                                                Bairro = srsRow.district,
                                                Cep = srsRow.zip
                                            };
                                            var sqlStatementEnd = @"
                                            UPDATE tb_endereco
                                            SET  cep = @Cep
                                            ,endereco = @Endereco
                                            ,numero = @Numero
                                            ,complemento = @Complemento
                                            ,bairro = @Bairro
                                            ,cidade = @Cidade
                                            ,estado = @Estado
                                            WHERE id = @Id";
                                            con.Execute(sqlStatementEnd, objEnd, transaction: transaction);
                                        }

                                        var obj = new
                                        {
                                            Cpf = colab.cpf,
                                            NomeCompleto = srsRow.first_name,
                                            DataNascimento = srsRow.dataNascimento,
                                            Rg = srsRow.cand_rg,
                                            ContatoPrincipal = srsRow.phone_cell,
                                            ContatoOutro = srsRow.phone_home,
                                            EstadoCivil = srsRow.cand_estCivil,
                                            Genero = srsRow.genero,
                                            Etnia = srsRow.etnia,
                                            OrientacaoSexual = srsRow.orientacao_sexual,
                                            Escolaridade = srsRow.nivel_escolaridade,
                                            Refugiado = srsRow.pessoa_refugiada,
                                            EmailAlternativo = srsRow.email1
                                        };
                                        var sqlStatement = @"
                                            UPDATE tb_colaborador
                                            SET
                                            nome_completo = @NomeCompleto
                                            ,data_nascimento = @DataNascimento
                                            ,rg = @Rg
                                            ,contato_principal = @ContatoPrincipal
                                            ,contato_outro = @ContatoOutro
                                            ,estado_civil = @EstadoCivil
                                            ,genero = @Genero
                                            ,etnia = @Etnia
                                            ,orientacao_sexual = @OrientacaoSexual
                                            ,escolaridade = @Escolaridade
                                            ,refugiado = @Refugiado
                                            ,email_alternativo = @EmailAlternativo
                                            WHERE cpf = @Cpf";
                                        con.Execute(sqlStatement, obj, transaction: transaction);
                                    }
                                }
                                catch (Exception e)
                                {
                                }
                                finally
                                {
                                    conSRS.Close();
                                }
                            }

                            transaction.Commit();
                            //transaction.Rollback();
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

        public void UnanimizarDadosBackoffice()
        {
            var connectionString = "server=10.10.10.223;port=3306;database=cats;uid=cats;pwd=cats;SslMode=none;AllowZeroDateTime=True;";

            using (var con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            var companyRows = con.Query("select * from company", commandType: System.Data.CommandType.Text, transaction: transaction).ToList();
                            foreach (var company in companyRows)
                            {
                                var obj = new
                                {
                                    Id = company.company_id,
                                    Name = Faker.Company.Name(),
                                    Address = Faker.Address.StreetAddress(),
                                    Zip = String.Join("", Enumerable.Range(0, 8).Select(x => Faker.RandomNumber.Next(9).ToString())),
                                    Phone1 = "11" + String.Join("", Enumerable.Range(0, 8).Select(x => Faker.RandomNumber.Next(9).ToString())),
                                    Phone2 = "11" + String.Join("", Enumerable.Range(0, 8).Select(x => Faker.RandomNumber.Next(9).ToString())),
                                    Url = Faker.Internet.Url(),
                                    Notes = Faker.Lorem.Paragraph(20)
                                };
                                var sqlStatement = @"
                                    UPDATE company
                                    SET  name = @Name
                                    ,address = @Address
                                    ,zip = @Zip
                                    ,phone1 = @Phone1
                                    ,phone2 = @Phone2
                                    ,url = @Url
                                    ,notes = @Notes
                                    WHERE company_id = @Id";
                                con.Execute(sqlStatement, obj, transaction: transaction);
                            }

                            var colabsRows = con.Query("select * from candidate", commandType: System.Data.CommandType.Text, transaction: transaction).ToList();
                            foreach (var colab in colabsRows)
                            {
                                var first = Faker.Name.First();
                                var middle = Faker.Name.Middle();
                                var last = Faker.Name.Last();
                                var fullName = first + " " + middle + " " + last;
                                var obj = new
                                {
                                    Id = colab.candidate_id,
                                    last_name = last,
                                    first_name = first,
                                    middle_name = middle,
                                    phone_home = "11" + String.Join("", Enumerable.Range(0, 8).Select(x => Faker.RandomNumber.Next(9))),
                                    phone_cel = "11" + String.Join("", Enumerable.Range(0, 9).Select(x => Faker.RandomNumber.Next(9))),
                                    address = Faker.Address.StreetAddress(),
                                    address_number = Faker.RandomNumber.Next(99).ToString(),
                                    address_complement = Faker.Address.SecondaryAddress(),
                                    district = "",
                                    city = Faker.Address.City(),
                                    state = Faker.Address.UsStateAbbr(),
                                    zip = String.Join("", Enumerable.Range(0, 8).Select(x => Faker.RandomNumber.Next(9).ToString())),
                                    notes = Faker.Lorem.Paragraph(20),
                                    email1 = Faker.Internet.Email(),
                                    email2 = Faker.Internet.Email(),
                                    cand_rg = Faker.RandomNumber.Next(1, 9).ToString() + String.Join("", Enumerable.Range(0, 6).Select(x => Faker.RandomNumber.Next(9).ToString())),
                                    cand_cpf_old = Faker.RandomNumber.Next(1, 9).ToString() + String.Join("", Enumerable.Range(0, 10).Select(x => Faker.RandomNumber.Next(9).ToString())),
                                    //cand_cpf = Faker.Lorem.Paragraph(20),
                                    cand_Lkdin = Faker.Internet.Url(),
                                    cand_skype = Faker.Internet.FreeEmail(),
                                    instagram = "@" + Faker.Name.First(),
                                    facebook = Faker.Internet.FreeEmail(),
                                    twitter = "@" + Faker.Name.First(),
                                    notesPP = Faker.Lorem.Paragraph(20),
                                    notesPT = Faker.Lorem.Paragraph(20),
                                    notesFC = Faker.Lorem.Paragraph(20),
                                    notesORH = Faker.Lorem.Paragraph(20),
                                    notesPF = Faker.Lorem.Paragraph(20),
                                    notesPD = Faker.Lorem.Paragraph(20),
                                    notesDPG = Faker.Lorem.Paragraph(20),
                                    notesDPA = Faker.Lorem.Paragraph(20),
                                    nomeCompleto = Faker.Name.FullName(),
                                    buscaCombNcomp = fullName
                                };
                                var sqlStatement = @"
                                    UPDATE candidate
                                    SET  last_name = @last_name
                                    ,first_name = @first_name
                                    ,middle_name = @middle_name
                                    ,phone_home = @phone_home
                                    ,phone_cell = @phone_cel
                                    ,address = @address
                                    ,address_number = @address_number
                                    ,address_complement = @address_complement
                                    ,district = @district
                                    ,city = @city
                                    ,state = @state
                                    ,zip = @zip
                                    ,notes = @notes
                                    ,email1 = @email1
                                    ,email2 = @email2
                                    ,cand_rg = @cand_rg
                                    ,cand_cpf_old = @cand_cpf_old
                                    ,cand_Lkdin = @cand_Lkdin
                                    ,cand_skype = @cand_skype
                                    ,instagram = @instagram
                                    ,facebook = @facebook
                                    ,twitter = @twitter
                                    ,notesPP = @notesPP
                                    ,notesPT = @notesPT
                                    ,notesFC = @notesFC
                                    ,notesORH = @notesORH
                                    ,notesPF = @notesPF
                                    ,notesPD = @notesPD
                                    ,notesDPG = @notesDPG
                                    ,notesDPA = @notesDPA
                                    ,nomeCompleto = @nomeCompleto
                                    ,buscaCombNcomp = @buscaCombNcomp
                                    WHERE candidate_id = @Id";
                                con.Execute(sqlStatement, obj, transaction: transaction);
                            }
                            transaction.Commit();
                            //transaction.Rollback();
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

        private void UnanimizarDadosSrs()
        {
            var connectionString = "server=127.0.0.1:3306;database=gcolb_hml;uid=root;pwd=Wnfa0n3h@";

            using (var con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    //return con.Query<string>(text, commandType: System.Data.CommandType.Text).ToList();
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