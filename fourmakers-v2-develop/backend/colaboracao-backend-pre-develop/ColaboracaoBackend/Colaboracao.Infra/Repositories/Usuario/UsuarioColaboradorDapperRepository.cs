using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Extension;
using Colaboracao.Infra.Context;
using Core.Domain.Usuario;
using Dapper;
using DataTransferObject.Domain.Carga;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.Usuario;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObject.Domain.Foursys;

namespace Colaboracao.Infra.Repositories.Usuario
{
    public class UsuarioColaboradorDapperRepository : IUsuarioColaboradorRepository
    {
        private readonly IDBConnection _dapperConnection;

        private readonly ColaboradorContext _colaboradorContext;

        public UsuarioColaboradorDapperRepository(IDBConnection dapperConnection, ColaboradorContext colaboradorContext)
        {
            this._dapperConnection = dapperConnection;
            this._colaboradorContext = colaboradorContext;
        }

        public void AlterarPrestadorServicoPessoaJuridica(string cnpj, PrestadorServicoDTO prestadorServico, RegimeTributarioDTO regimeTributario, string cpf)
        {
            throw new NotImplementedException();
        }

        public void AlteraSenhaUsuario(string cpf, string senha, int orgId)
        {
            throw new NotImplementedException();
        }

        public void AtualizaEmailUsuario(UsuarioColaboradorDTO usuario)
        {
            var _connection = _dapperConnection.GetConnection();

            var result = _connection.Execute("update tb_usuario set email = @Email where codigo_interno_colaborador = @CPF and tb_org_id = @OrgId", new { CPF = usuario.Cpf, Email = usuario.Email, OrgId = usuario.OrgId });
            if (result == 0)
                throw new Exception("Falha de SQL ao atualizar o email do colaborador");
        }

        public void AtualizaDocumentoUsuario(String codInternoColaborador, String documentoColaborador)
        {
            var _connection = _dapperConnection.GetConnection();

            var result = _connection.Execute("update tb_colaborador set documento_colaborador = @DocumentoColaborador where codigo_interno_colaborador = @CodInternoColaborador", new { DocumentoColaborador = documentoColaborador, CodInternoColaborador = codInternoColaborador });
            if (result == 0)
                throw new Exception("Falha de SQL ao atualizar o documento do colaborador");
        }

        public void AtualizaNomelUsuario(String codInternoColaborador, String nomeCompleto)
        {
            var _connection = _dapperConnection.GetConnection();

            var result = _connection.Execute("update tb_colaborador set nome_completo = @NomeCompleto where codigo_interno_colaborador = @CodInternoColaborador", new { NomeCompleto = nomeCompleto, CodInternoColaborador = codInternoColaborador });
            if (result == 0)
                throw new Exception("Falha de SQL ao atualizar o nome do colaborador");
        }

        public bool CheckColaboradorOrg(string cpf, int orgId)
        {
            throw new NotImplementedException();
        }

        public bool CheckColaboradorOrgAtivo(string cpf, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 1
                FROM tb_colaborador_org
                WHERE codigo_interno_colaborador = @Cpf
                  AND tb_org_id = @OrgId
                  AND ativo = 1
                LIMIT 1";
            var result = connection.QueryFirstOrDefault<int?>(query, new { Cpf = cpf, OrgId = orgId });
            return result.HasValue;
        }

        public void DeletaColaboradorProjetoOrg(int orgId, string codColaborador)
        {
            throw new NotImplementedException();
        }

        public void EditarColaborador(CadastroColaboradorInput colaboradorInput, int orgId)
        {
            throw new NotImplementedException();
        }

        public bool ExisteColaborador(string cpf)
        {
            var _connection = _dapperConnection.GetConnection();

            return _connection.Query<dynamic>("SELECT * FROM tb_colaborador where codigo_interno_colaborador = @CPF", new { CPF = cpf }).ToList().Any();
        }

        public bool ExisteColaboradorComCodColaboradorExterno(string codColaboradorExt, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            string query = @"SELECT
                                1
                             FROM
                                tb_colaborador_org
                             WHERE
                                tb_org_id = @OrgId AND cod_colaborador_externo = @CodColaboradorExt
                             LIMIT 1";

            var result = connection.QueryFirstOrDefault<int?>(query, new { OrgId = orgId, CodColaboradorExt = codColaboradorExt });

            return result.HasValue;
        }
        
        public async Task<int> BuscarQuantidadeDeDependentesPorCodigoInternoColaborador(string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            string query = @"
                SELECT COUNT(*) 
                FROM tb_colaborador_dependente tcd
                WHERE tcd.codigo_interno_colaborador = @CodigoInternoColaborador";

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                query,
                new { CodigoInternoColaborador = codigoInternoColaborador }
            );

            return result;
        }

        public bool ExisteColaboradorComCPF(string cpf, int orgId)
        {
            throw new NotImplementedException();
        }

        public bool ExisteColaboradorComEmail(string email, int orgId)
        {
            throw new NotImplementedException();
        }

        public bool ExisteColaboradorComEmailAlternativo(string email, int orgId)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var connection = _dapperConnection.GetConnection();
            const string query = @"
                SELECT 1
                FROM tb_colaborador tc
                INNER JOIN tb_gestor_externo tge ON tc.codigo_interno_colaborador = tge.codigo_interno_colaborador
                WHERE tge.tb_org_id = @OrgId
                  AND tge.ativo = 1
                  AND tc.email_alternativo = @Email
                LIMIT 1";

            var result = connection.QueryFirstOrDefault<int?>(query, new { OrgId = orgId, Email = email.Trim() });
            return result.HasValue;
        }

        public UsuarioColaboradorDTO GetUserByCPFEOrgId(string cpf, int orgId, int? userId = null)
        {
            try
            {
                var row = _colaboradorContext.tb_usuario
                    .Where(usuario => ((usuario.codigo_interno_colaborador == cpf && usuario.tb_org_id == orgId) || usuario.id == userId) && usuario.ativo == 1)
                    .Join(_colaboradorContext.tb_colaborador,
                        usuario => usuario.codigo_interno_colaborador,
                        colaborador => colaborador.codigo_interno_colaborador,
                        (usuario, colaborador) => new { usuario, colaborador })
                    .Join(_colaboradorContext.tb_colaborador_org.Where(tco => tco.tb_org_id == orgId && tco.ativo == 1),
                        uc => uc.usuario.codigo_interno_colaborador,
                        tco => tco.codigo_interno_colaborador,
                        (uc, tco) => new
                        {
                            uc.usuario.id,
                            uc.usuario.email,
                            uc.colaborador.nome_completo,
                            uc.colaborador.contato_principal_ddi,
                            uc.colaborador.contato_principal
                        })
                    .FirstOrDefault();

                if (row == null)
                    throw new Exception("Usuário não encontrado");

                return new UsuarioColaboradorDTO
                {
                    Cpf = cpf,
                    UsuarioId = row.id,
                    Email = row.email,
                    NomeColaborador = row.nome_completo,
                    ContatoPrincipalDDI = row.contato_principal_ddi,
                    ContatoPrincipal = row.contato_principal
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void InsereColaboradorProjetoOrg(List<ColaboradorProjetoCargaDTO> colaboradoresProjetoCarga, int orgId)
        {
            throw new NotImplementedException();
        }

        public void InsereHierarquiaColaborador(List<ColaboradorHierarquiaCargaDTO> cargaColaboradorHierarquia, int orgId)
        {
            _colaboradorContext.tb_colaborador_hierarquia.AddRange(cargaColaboradorHierarquia.Select(
                colaboradorHierarquia => new tb_colaborador_hierarquia
                {
                    tb_org_id = orgId,
                    cod_colaborador_externo = colaboradorHierarquia.IdentificadorColaborador,
                    cod_colaborador_superior = colaboradorHierarquia.IdentificadorSuperior
                }).ToList()
            );
            _colaboradorContext.SaveChanges();
        }

        public void DeletaHierarquiaColaborador(int orgId)
        {
            _colaboradorContext.tb_colaborador_hierarquia.RemoveRange(_colaboradorContext.tb_colaborador_hierarquia.Where(x => x.tb_org_id == orgId));
            _colaboradorContext.SaveChanges();
        }

        public void DeletaHierarquiaColaboradorPorCodColaborador(string codColaborador, int orgId)
        {
            throw new NotImplementedException();
        }

        public void InsereHierarquiaColaborador(ColaboradorHierarquiaCargaDTO cargaColaboradorHierarquia, int orgId)
        {
            _colaboradorContext.tb_colaborador_hierarquia.Add(
               new tb_colaborador_hierarquia()
               {
                   tb_org_id = orgId,
                   cod_colaborador_externo = cargaColaboradorHierarquia.IdentificadorColaborador,
                   cod_colaborador_superior = cargaColaboradorHierarquia.IdentificadorSuperior
               });
            _colaboradorContext.SaveChanges();
        }

        public void InserePrestadorServicoPessoaJuridica(string cnpj, PrestadorServicoDTO prestadorServico, RegimeTributarioDTO regimeTributario, string cpf)
        {
            throw new NotImplementedException();
        }

        public void InsertUsuarioColaborador(UsuarioColaboradorDTO usuario, ColaboradorDTO colaborador, EnderecoDTO endereco)
        {
            throw new NotImplementedException();
        }

        public void InsertColaboradorSemUsuario(ColaboradorDTO colaborador)
        {
            InserirSomenteTbColaborador(colaborador, _dapperConnection.GetConnection());
        }

        public bool SeTokenSistema(string token)
        {
            throw new NotImplementedException();
        }

        public void UpsertColaboradorOrg(ColaboradorDTO colaborador, ColaboradorOrgDTO colaboradorOrg)
        {
            var _connection = _dapperConnection.GetConnection();

            if (_connection.Query<dynamic>("SELECT * FROM tb_colaborador_org where cod_colaborador_externo = @CodColaborador and tb_org_id = @OrgId",
                       new { CodColaborador = colaboradorOrg.CodColaborador, OrgId = colaboradorOrg.OrgId }).ToList().Any())
            {
                AtualizaColaboradorOrg(colaboradorOrg, _connection);
            }
            else
            {
                InsereColaboradorOrg(colaborador, colaboradorOrg, _connection);
            }
        }

        public void UpsertUsuarioColaborador(UsuarioColaboradorDTO usuario, ColaboradorDTO colaborador, EnderecoDTO endereco)
        {
            var _connection = _dapperConnection.GetConnection();

            if (_connection.Query<dynamic>("SELECT * FROM tb_colaborador where codigo_interno_colaborador = @CPF", new { CPF = colaborador.Cpf }).ToList().Any())
            {
                AtualizaColaborador(colaborador, endereco, _connection);
            }
            else
            {
                InsereNovoColaborador(usuario, colaborador, endereco, _connection);
            }
        }

        private void AtualizaColaboradorOrg(ColaboradorOrgDTO colaboradorOrg, MySqlConnection connection)
        {
            var sqlColab = @"UPDATE
                                tb_colaborador_org SET
                                cargo = @cargo,
                                codigo_interno_colaborador = @CPF,
                                cod_departamento = @cod_departamento,
                                cod_diretoria = @cod_diretoria,
                                codigo_cargo = @codigo_cargo,
                                data_alteracao = @data_alteracao,
                                departamento = @departamento,
                                diretoria = @diretoria,
                                data_admissao = @data_admissao,
                                ativo = @ativo,
                                modelo_contratacao = @modelo_contratacao,
                                empresa_relacionada = @empresa_relacionada,
                                modelo_trabalho = @modelo_trabalho,
                                dias_por_semana = @dias_por_semana,
                                valor_hora = @valor_hora,
                                custo_hora = @custo_hora,
                                base_hora_mes = @base_hora_mes,
                                codigo_modelo_contratacao = @codigo_modelo_contratacao,
                                data_inativacao = @DataInativacao
                             WHERE
                                cod_colaborador_externo = @cod_colaborador_externo
                                AND tb_org_id = @OrgId;";

            var resultColabOrg = connection.Execute(sqlColab, new
            {
                cargo = colaboradorOrg.Cargo,
                cod_colaborador_externo = colaboradorOrg.CodColaborador,
                CPF = colaboradorOrg.Cpf,
                cod_departamento = colaboradorOrg.CodDepartamento,
                cod_diretoria = colaboradorOrg.CodDiretoria,
                codigo_cargo = colaboradorOrg.CodCargo,
                data_alteracao = DateTime.Now,
                departamento = colaboradorOrg.Departamento,
                diretoria = colaboradorOrg.Diretoria,
                data_admissao = colaboradorOrg.DataAdmissao,
                ativo = (sbyte)(colaboradorOrg.Ativo ? 1 : 0),
                modelo_contratacao = colaboradorOrg.ModeloContratacao,
                codigo_modelo_contratacao = colaboradorOrg.CodigoModeloContratacao,
                empresa_relacionada = colaboradorOrg.EmpresaRelacionada?.ToUpperInvariant(),
                modelo_trabalho = colaboradorOrg.ModeloTrabalho,
                dias_por_semana = colaboradorOrg.DiasPorSemana,
                valor_hora = colaboradorOrg.ValorHora,
                custo_hora = colaboradorOrg.CustoHora,
                base_hora_mes = colaboradorOrg.BaseHoraMes,
                OrgId = colaboradorOrg.OrgId,
                DataInativacao = colaboradorOrg.DataInativacao
            });

            if (resultColabOrg == 0)
            {
                throw new Exception("Falha de SQL ao atualizar o colaborador org");
            }
        }

        private void InsereColaboradorOrg(ColaboradorDTO colaborador, ColaboradorOrgDTO colaboradorOrg, MySqlConnection connection)
        {
            var sqlColab = @"INSERT INTO tb_colaborador_org (
                                             cargo, cod_colaborador_externo, cod_departamento, cod_diretoria,
                                             codigo_cargo, data_alteracao, departamento, diretoria,
                                             codigo_interno_colaborador, tb_org_id, data_admissao, ativo,
                                             modelo_contratacao, empresa_relacionada, modelo_trabalho,
                                             dias_por_semana, valor_hora, custo_hora, base_hora_mes, codigo_modelo_contratacao, data_inativacao)
                                         VALUES (
                                             @cargo, @cod_colaborador_externo, @cod_departamento, @cod_diretoria,
                                             @codigo_cargo, @data_alteracao, @departamento, @diretoria,
                                             @tb_colaborador_cpf, @tb_org_id, @data_admissao, @ativo,
                                             @modelo_contratacao, @empresa_relacionada, @modelo_trabalho,
                                             @dias_por_semana, @valor_hora, @custo_hora, @base_hora_mes, @codigo_modelo_contratacao, @data_inativacao
                                         );";

            var resultColabOrg = connection.Execute(sqlColab, new
            {
                cargo = colaboradorOrg.Cargo,
                cod_colaborador_externo = colaboradorOrg.CodColaborador,
                cod_departamento = colaboradorOrg.CodDepartamento,
                cod_diretoria = colaboradorOrg.CodDiretoria,
                codigo_cargo = colaboradorOrg.CodCargo,
                data_alteracao = DateTime.Now,
                departamento = colaboradorOrg.Departamento,
                diretoria = colaboradorOrg.Diretoria,
                tb_colaborador_cpf = colaborador.Cpf,
                tb_org_id = colaboradorOrg.OrgId,
                data_admissao = colaboradorOrg.DataAdmissao,
                ativo = (sbyte)(colaboradorOrg.Ativo ? 1 : 0),
                modelo_contratacao = colaboradorOrg.ModeloContratacao?.ToUpperInvariant(),
                empresa_relacionada = colaboradorOrg.EmpresaRelacionada,
                modelo_trabalho = colaboradorOrg.ModeloTrabalho,
                dias_por_semana = colaboradorOrg.DiasPorSemana,
                valor_hora = colaboradorOrg.ValorHora,
                custo_hora = colaboradorOrg.CustoHora,
                base_hora_mes = colaboradorOrg.BaseHoraMes,
                codigo_modelo_contratacao = colaboradorOrg.CodigoModeloContratacao,
                data_inativacao = colaboradorOrg.DataInativacao
            });

            if (resultColabOrg == 0)
            {
                throw new Exception("Falha de SQL ao adicionar o colaborador org");
            }
        }

        private void InsereNovoColaborador(UsuarioColaboradorDTO usuario, ColaboradorDTO colaborador, EnderecoDTO endereco, MySqlConnection connection)
        {
            try
            {
                long? enderecoId = null;
                if (endereco != null)
                {
                    var sqlEndereco = @"insert into tb_endereco (cep, cidade, complemento, estado, endereco, numero, bairro, com_quem_mora)
                        values(
                            @Cep, @Cidade, @Complemento, @Estado, @Endereco, @Numero, @Bairro, @ComQuemMora
                        );";
                    var resultEndereco = connection.Execute(sqlEndereco, endereco);
                    if (resultEndereco == 0)
                    {
                        throw new Exception("Falha de SQL em endereço");
                    }
                    enderecoId = connection.QuerySingle<long?>("SELECT @@IDENTITY;");
                }

                InserirSomenteTbColaborador(colaborador, connection, enderecoId);

                if (connection.Query<dynamic>("SELECT * FROM tb_usuario WHERE codigo_interno_colaborador = @CPF", new { CPF = colaborador.Cpf }).ToList().Any())
                {
                    var sqlUsuario = @"update tb_usuario set primeiro_acesso_realizado = 1, email = @Email where codigo_interno_colaborador = @CPF;";
                    var resultUsuario = connection.Execute(sqlUsuario, new { usuario.Email, CPF = usuario.Cpf });
                    if (resultUsuario == 0)
                    {
                        throw new Exception("Falha de SQL ao atualizar usuario");
                    }
                }
                else
                {
                    var sqlUsuario = @"insert into tb_usuario (ativo, codigo_interno_colaborador, email, password, primeiro_acesso_realizado, tb_org_id)
                        values(
                            @ativo, @cpf, @email, @password, @primeiroAcessoRealizado, @orgId
                        );";
                    var resultUsuario = connection.Execute(sqlUsuario, new { ativo = 1, cpf = usuario.Cpf, email = usuario.Email, password = "", primeiroAcessoRealizado = 0, orgId = usuario.OrgId });
                    if (resultUsuario == 0)
                    {
                        throw new Exception("Falha de SQL ao adicionar usuario");
                    }
                }

                var sqlStatus = @"insert into tb_colaborador_status (ativo, data_alteracao, data_criacao, status_colaborador_id, codigo_interno_colaborador)
                        values(
                            @ativo, @data_alteracao, @data_criacao, @status_colaborador_id, @colaborador_cpf
                        );";
                var resultStatus = connection.Execute(sqlStatus, new
                {
                    ativo = 1,
                    data_alteracao = DateTime.Now,
                    data_criacao = DateTime.Now,
                    status_colaborador_id = 1,
                    colaborador_cpf = colaborador.Cpf
                });
                if (resultStatus == 0)
                {
                    throw new Exception("Falha de SQL ao adicionar o status do colaborador");
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void InserirSomenteTbColaborador(ColaboradorDTO colaborador, MySqlConnection connection, long? enderecoId = null)
        {
            var sqlColaborador = @"insert into tb_colaborador (codigo_interno_colaborador, nome_completo, data_nascimento, rg, matricula, endereco_id,
                        data_criacao, data_alteracao, contato_principal, contato_outro, candidato, ativo, passaporte, estado_civil, genero, etnia,
                        escolaridade, orientacao_sexual, refugiado, email_alternativo, documento_colaborador, url_linkedin)
                        values(
                            @cpf, @nome_completo, @data_nascimento, @rg, @matricula, @endereco_id, @data_criacao, @data_alteracao, @contato_principal,
                            @contato_outro, @candidato, @ativo, @passaporte, @estado_civil, @genero, @etnia, @escolaridade, @orientacao_sexual,
                            @refugiado, @email_alternativo, @documento_colaborador, @url_linkedin
                        );";

            var resultColaborador = connection.Execute(sqlColaborador, new
            {
                cpf = colaborador.Cpf,
                nome_completo = colaborador.NomeCompleto,
                data_nascimento = colaborador.DataNascimento != null && colaborador.DataNascimento != DateTime.MinValue ? colaborador.DataNascimento : (DateTime?)null,
                rg = colaborador.Rg,
                matricula = colaborador.Matricula,
                endereco_id = enderecoId,
                data_criacao = DateTime.Now,
                data_alteracao = DateTime.Now,
                contato_principal_ddi = colaborador.ContatoPrincipalDDI,
                contato_principal = colaborador.ContatoPrincipal,
                contato_outro = colaborador.ContatoOutros,
                candidato = 0,
                ativo = 1,
                passaporte = colaborador.Passaporte,
                estado_civil = colaborador.EstadoCivil,
                genero = colaborador.Genero,
                etnia = colaborador.Etnia,
                escolaridade = colaborador.Escolaridade,
                orientacao_sexual = colaborador.OrientacaoSexual,
                refugiado = (SByte)(colaborador.PessoaRefugiada ? 1 : 0),
                email_alternativo = colaborador.EmailAlternativo,
                documento_colaborador = colaborador.DocumentoColaborador,
                url_linkedin = colaborador.UrlLinkedin
            });
            if (resultColaborador == 0)
            {
                throw new Exception("Falha de SQL ao adicionar colaborador");
            }
        }

        private void AtualizaColaborador(ColaboradorDTO colaborador, EnderecoDTO endereco, MySqlConnection connection)
        {
            try
            {
                if (!connection.Query<dynamic>("SELECT * FROM tb_colaborador where codigo_interno_colaborador = @CPF", new { CPF = colaborador.Cpf }).ToList().Any())
                    throw new Exception("Este colaborador não existe.");

                var enderecoId = connection.QuerySingle<long?>("SELECT endereco_id FROM tb_colaborador where codigo_interno_colaborador = @CPF", new { CPF = colaborador.Cpf });
                if (enderecoId != null)
                {
                    var sqlEndereco = @"update tb_endereco set cep = @cep, cidade = @cidade, complemento = @complemento,
                        estado = @estado, endereco = @endereco, numero = @numero, bairro = @bairro where id = @endereco_id;";
                    var resultEndereco = connection.Execute(sqlEndereco, new
                    {
                        cep = endereco.Cep,
                        cidade = endereco.Cidade,
                        complemento = endereco.Complemento,
                        estado = endereco.Estado,
                        endereco = endereco.Endereco,
                        numero = endereco.Numero,
                        bairro = endereco.Bairro,
                        endereco_id = enderecoId
                    });
                    if (resultEndereco == 0)
                    {
                        throw new Exception("Falha de SQL ao atualizar o endereco do colaborador");
                    }
                }
                else
                {
                    var sqlEndereco = @"insert into tb_endereco (cep, cidade, complemento, estado, endereco, numero, bairro, com_quem_mora)
                        values(
                            @Cep, @Cidade, @Complemento, @Estado, @Endereco, @Numero, @Bairro, @ComQuemMora
                        );
                        update tb_colaborador set endereco_id = @@IDENTITY where codigo_interno_colaborador = @CPF";
                    var resultEndereco = connection.Execute(sqlEndereco, new
                    {
                        cep = endereco.Cep,
                        cidade = endereco.Cidade,
                        complemento = endereco.Complemento,
                        estado = endereco.Estado,
                        endereco = endereco.Endereco,
                        numero = endereco.Numero,
                        bairro = endereco.Bairro,
                        com_quem_mora = "",
                        CPF = colaborador.Cpf
                    });
                    if (resultEndereco == 0)
                    {
                        throw new Exception("Falha de SQL ao adicionar endereço do colaborador");
                    }
                }

                var sqlColaborador = @"update tb_colaborador set
                        nome_completo = @nome_completo, data_nascimento = @data_nascimento, rg = @rg, data_alteracao = @data_alteracao, contato_principal = @contato_principal,
                        contato_outro = @contato_outro, ativo = @ativo, estado_civil = @estado_civil, genero = @genero, etnia = @etnia, escolaridade = @escolaridade,
                        orientacao_sexual = @orientacao_sexual, refugiado = @refugiado, email_alternativo = @email_alternativo where codigo_interno_colaborador = @CPF;";
                var resultColaborador = connection.Execute(sqlColaborador, new
                {
                    nome_completo = colaborador.NomeCompleto,
                    data_nascimento = colaborador.DataNascimento ?? DateTime.MinValue,
                    rg = colaborador.Rg,
                    data_alteracao = DateTime.Now,
                    contato_principal_ddi = colaborador.ContatoPrincipalDDI,
                    contato_principal = colaborador.ContatoPrincipal,
                    contato_outro = colaborador.ContatoOutros,
                    ativo = colaborador.FlagAtivo ? (sbyte)1 : (sbyte)0,
                    estado_civil = colaborador.EstadoCivil,
                    genero = colaborador.Genero,
                    etnia = colaborador.Etnia,
                    escolaridade = colaborador.Escolaridade,
                    orientacao_sexual = colaborador.OrientacaoSexual,
                    refugiado = (SByte)(colaborador.PessoaRefugiada ? 1 : 0),
                    email_alternativo = colaborador.EmailAlternativo,
                    CPF = colaborador.Cpf
                });

                if (resultColaborador == 0)
                {
                    throw new Exception("Falha de SQL ao atualizar colaborador");
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void AtualizaListaColaboradorProjetoOrg(List<ColaboradorProjetoCargaDTO> colaboradoresProjetoCarga, string codColaborador, int orgId)
        {
            if (colaboradoresProjetoCarga.Count == 0 || codColaborador == null)
                return;
            var _connection = _dapperConnection.GetConnection();

            var tran = _connection.BeginTransaction();

            try
            {
                _connection.Execute("delete from tb_colaborador_projeto_org where tb_org_id = @OrgId and cod_colaborador = @CodColaborador;", new
                {
                    OrgId = orgId,
                    CodColaborador = codColaborador
                });

                var rowCount = _connection.Execute(@"insert into tb_colaborador_projeto_org (cod_colaborador, cod_projeto, nome_projeto, tb_org_id) values (
                                @CodColaborador, @cod_projeto, @nome_projeto, @OrgId
                            );", colaboradoresProjetoCarga.Select(x => new
                {
                    OrgId = orgId,
                    cod_projeto = x.CodigoProjeto,
                    nome_projeto = x.NomeProjeto,
                    CodColaborador = x.CodigoColaborador
                }));
                if (rowCount != colaboradoresProjetoCarga.Count)
                    throw new Exception("Número de projetos adicionados diferente do esperado.");

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw;
            }
        }

        public bool ExisteColaboradorComCodColaboradorExternoECpf(string codColaboradorExt, string cpf, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org
                    .Any(x => x.tb_org_id == orgId && x.cod_colaborador_externo == codColaboradorExt && x.codigo_interno_colaborador == cpf);
        }

        public string GetColaboradorCpf(string codColaborador, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org.Where(x => x.cod_colaborador_externo == codColaborador && x.tb_org_id == orgId).FirstOrDefault()?.codigo_interno_colaborador ?? null;
        }

        public string GetCodigoInternoByCodExterno(string codExterno, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org.Where(x => x.cod_colaborador_externo == codExterno && x.tb_org_id == orgId).FirstOrDefault()?.codigo_interno_colaborador ?? null;
        }

        public string GetCodigoInternoByDocumentoEOrg(string documentoColaborador, int orgId)
        {
             var _connection = _dapperConnection.GetConnection();
             var query = @"
                SELECT
                    tc.codigo_interno_colaborador
                FROM tb_colaborador tc
                    inner join tb_colaborador_org tco on tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                WHERE
                    tc.documento_colaborador = @DocumentoColaborador
                    AND tco.tb_org_id = @OrgId;";
            var parametros = new
            {
                DocumentoColaborador = documentoColaborador,
                OrgId = orgId
            };

            var result = _connection.QueryFirstOrDefault<string>(query, parametros);
            return result;
        }

        public async Task<bool> AlterarIdiomaPadrao(string idioma, string cpf, int orgId)
        {
            return await Task.Run(() => false);
        }

        public void AtualizaDadosBasicosUsuario(string codInternoColaborador, DateTime dataNascimento)
        {
            var _connection = _dapperConnection.GetConnection();

            var result = _connection.Execute("update tb_colaborador set data_nascimento = @DataNascimento where codigo_interno_colaborador = @CodInternoColaborador", new { DataNascimento = dataNascimento, CodInternoColaborador = codInternoColaborador });
            if (result == 0)
                throw new Exception("Falha de SQL ao atualizar a data de nascimento do colaborador");
        }
        public Task<List<UsuarioColaboradorDTO>> GetUsersByNome(string[] nomes)
        {
            throw new NotImplementedException();
        }

        public UsuarioColaboradorDTO BuscaColaboradorPorCodigo(string codColaborador, int orgId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetCodColaboradorByEmailEOrgId(string email, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    tu.codigo_interno_colaborador
                FROM tb_usuario tu
                WHERE
                    tu.email = @Email
                    AND tu.tb_org_id = @OrgId;";
            var parametros = new
            {
                Email = email,
                OrgId = orgId
            };

            var result = await connection.QueryFirstOrDefaultAsync<string>(query, parametros);

            return result;
        }

        public async Task<List<UsuarioDTO>> BuscarUsuariosOrgPorEmail(string email)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    tu.id AS Id,
                    tu.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tu.email AS Email,
                    tu.primeiro_acesso_realizado AS PrimeiroAcessoRealizado,
                    tu.ativo AS Ativo,
                    tu.sistemico AS Sistemico,
                    tu.data_expiracao AS DataExpiracao,
                    tu.dataAceiteTermo AS DataAceiteTermo,
                    tu.tb_org_id AS OrgId
                FROM tb_usuario tu
                WHERE
                    tu.email = @Email;";

            var parametros = new { Email = email };

            var result = await connection.QueryAsync<UsuarioDTO>(query, parametros);

            return result.ToList();
        }

        public void InserirUsuarioEColaboradorOrg(UsuarioColaboradorDTO usuario, ColaboradorOrgDTO colaboradorOrg)
        {
            var connection = _dapperConnection.GetConnection();
            var usuarioQuery = @"insert into tb_usuario (ativo, codigo_interno_colaborador, email, password, primeiro_acesso_realizado, tb_org_id)
                        values(
                            @ativo, @cpf, @email, @password, @primeiroAcessoRealizado, @orgId
                        );";
            connection.Execute(usuarioQuery, new { ativo = 1, cpf = usuario.Cpf, email = usuario.Email, password = "", primeiroAcessoRealizado = 0, orgId = usuario.OrgId });

            InsereColaboradorOrg(new ColaboradorDTO { Cpf = usuario.Cpf }, colaboradorOrg, connection);
        }
        
        public async Task<bool> VerificaSeEhGestorHierarquicoDeUmAprovador(int orgId, string codigoInternoGerente)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        SELECT 
                        CASE 
                            WHEN EXISTS (
                                SELECT 1
                                FROM vw_gestor_hierarquico_aprovadores vwgha 
                                WHERE 
                                    vwgha.tb_org_id = @OrgId
                                    AND vwgha.cod_interno_colaborador = @CodigoInternoGerente
                            ) THEN 1
                            ELSE 0
                        END AS Resultado
                ";

            var parametros = new
            {
                OrgId = orgId,
                CodigoInternoGerente = codigoInternoGerente,
            };

            var result = await connection.QueryFirstOrDefaultAsync<int>(query, parametros);
            return result == 1;
        }

        public bool SalvarDispositivoColaboradorApp(string codigoInternoColaborador, string deviceToken, int orgId)
        {
            var _connection = _dapperConnection.GetConnection();

            var result = _connection.Execute("UPDATE tb_usuario SET fcm_token = @deviceToken WHERE codigo_interno_colaborador = @codigoInternoColaborador AND tb_org_id = @orgId;", new { deviceToken, codigoInternoColaborador, orgId });
            if (result == 0)
                throw new Exception("Falha de SQL ao atualizar o email do colaborador");

            return true;
        }
        
        public async Task<List<ModeloContratacaoDTO>> ListarModelosDeContratacoesPorOrg(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    codigo_modelo_contratacao AS CodigoModeloContratacao,
                    descricao AS Descricao,
                    tb_org_id AS OrgId,
                    deve_criar_nf AS DeveCriarNF
                FROM tb_modelo_contratacao_org 
                WHERE tb_org_id = @OrgId;
            ";
            var parametros = new
            {
                OrgId = orgId
            };
            
            return (await connection.QueryAsync<ModeloContratacaoDTO>(query, parametros)).ToList();
        }

        public void AtualizarEnderecoColaborador(string codigoInternoColaborador, string cidade, string estado)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                // Verifica se o colaborador tem endereço
                var enderecoId = connection.QuerySingle<long?>("SELECT endereco_id FROM tb_colaborador WHERE codigo_interno_colaborador = @CPF", new { CPF = codigoInternoColaborador });
                
                if (enderecoId != null)
                {
                    // Atualiza o endereço existente
                    var sqlEndereco = @"UPDATE tb_endereco 
                                       SET cidade = @cidade, estado = @estado 
                                       WHERE id = @endereco_id;";
                    
                    var resultEndereco = connection.Execute(sqlEndereco, new
                    {
                        cidade = cidade,
                        estado = estado,
                        endereco_id = enderecoId
                    });
                    
                    if (resultEndereco == 0)
                    {
                        throw new Exception("Falha de SQL ao atualizar o endereço do colaborador");
                    }
                }
                else
                {
                    // Cria um novo endereço e associa ao colaborador
                    var sqlEndereco = @"INSERT INTO tb_endereco (cidade, estado, com_quem_mora)
                                       VALUES (@cidade, @estado, @com_quem_mora);
                                       UPDATE tb_colaborador SET endereco_id = @@IDENTITY WHERE codigo_interno_colaborador = @CPF";
                    
                    var resultEndereco = connection.Execute(sqlEndereco, new
                    {
                        cidade = cidade,
                        estado = estado,
                        com_quem_mora = "",
                        CPF = codigoInternoColaborador
                    });
                    
                    if (resultEndereco == 0)
                    {
                        throw new Exception("Falha de SQL ao adicionar endereço do colaborador");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar endereço do colaborador: {ex.Message}", ex);
            }
        }

        public void AtualizarContatoEEmailAlternativo(string codigoInternoColaborador, string contatoPrincipal, string emailAlternativo)
        {
            var connection = _dapperConnection.GetConnection();
            
            try
            {
                // Atualiza contato_principal e email_alternativo do colaborador
                var sql = @"UPDATE tb_colaborador 
                           SET contato_principal = @contatoPrincipal, 
                               email_alternativo = @emailAlternativo
                           WHERE codigo_interno_colaborador = @codigoInternoColaborador";
                
                var result = connection.Execute(sql, new
                {
                    contatoPrincipal = contatoPrincipal,
                    emailAlternativo = emailAlternativo,
                    codigoInternoColaborador = codigoInternoColaborador
                });
                
                if (result == 0)
                {
                    throw new Exception("Falha de SQL ao atualizar contato e email alternativo do colaborador");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar contato e email alternativo do colaborador: {ex.Message}", ex);
            }
        }

        public async Task<string> GetNomeColaboradorPorCodigoInterno(string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    nome_completo
                FROM tb_colaborador
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador
            ";

            var parametros = new
            {
                CodigoInternoColaborador = codigoInternoColaborador
            };
            
            return await  connection.QueryFirstOrDefaultAsync<string>(query, parametros);
        }

        public async Task<string> GetEmailColaboradorPorCodigoInterno(string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    email
                FROM tb_usuario
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador
                AND tb_org_id = @OrgId
            ";

            var parametros = new
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                OrgId = orgId
            };
            
            var result = await connection.QuerySingleOrDefaultAsync<string>(query, parametros);
            return result;
        }

        public async Task<List<CargoColaboradorOrgDTO>> ListarCargosPorOrgIdAsync(int orgId, List<string>? restricaoDiretorias = null)
        {
            var connection = _dapperConnection.GetConnection();
            var inClauseDiretorias = restricaoDiretorias.BuildInClauseOrNull();

            var query = $@"
                SELECT DISTINCT
                    tco.cargo AS Cargo,
                    tco.codigo_cargo AS CodigoCargo
                FROM tb_colaborador_org AS tco
                WHERE tco.tb_org_id = @OrgId
                  AND tco.ativo = 1
                  AND tco.cargo IS NOT NULL
                  AND tco.cargo <> ''
                  {(inClauseDiretorias == null ? "" : $"AND tco.cod_diretoria IN {inClauseDiretorias}")}
            ";

            var parametros = new
            {
                OrgId = orgId
            };
            
            var result = await connection.QueryAsync<CargoColaboradorOrgDTO>(query, parametros);
            return result.ToList();
        }

        public async Task EditarTelefoneColaborador(string telefone, string ddi, string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            
            var query = @"
                UPDATE tb_colaborador
                SET 
                    contato_principal_ddi = @Ddi,
                    contato_principal = @Telefone
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador
            ";

            var parametros = new
            {
                Ddi = ddi,
                Telefone = telefone,
                CodigoInternoColaborador = codigoInternoColaborador
            };
            
            await connection.ExecuteAsync(query, parametros);
        }

        public Task EditarColaboradorDapperAsync(CadastroColaboradorInput colaboradorInput, int orgId)
        {
            throw new NotImplementedException();
        }
    }
}