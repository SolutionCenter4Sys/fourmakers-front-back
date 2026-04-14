using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.Usuario;
using Dapper;
using DataTransferObject.Domain.Carga;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.Usuario;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sprache;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Helper;
using Colaboracao.Helper.Extension;
using DataTransferObject.Domain.Foursys;
using Microsoft.IdentityModel.Tokens;

namespace Colaboracao.Infra.Repositories.Usuario
{
    public class UsuarioColaboradorRepository : IUsuarioColaboradorRepository
    {
        private const int ORG_ID_FOURMARKERS = 1;
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;
        private readonly IConfiguration _configuration;

        public UsuarioColaboradorRepository(ColaboradorContext colaboradorContext, IConfiguration configuration, IDBConnection dapperConnection)
        {
            this._colaboradorContext = colaboradorContext;
            this._configuration = configuration;
            _dapperConnection = dapperConnection;
        }

        public void InsertUsuarioColaborador(UsuarioColaboradorDTO usuario, ColaboradorDTO colaborador, EnderecoDTO endereco)
        {
            InsereNovoColaborador(usuario, colaborador, endereco);
        }

        public void InsertColaboradorSemUsuario(ColaboradorDTO colaborador)
        {
            InserirOuAtualizarSomenteTbColaborador(colaborador);
        }

        public void UpsertUsuarioColaborador(UsuarioColaboradorDTO usuario, ColaboradorDTO colaborador, EnderecoDTO endereco)
        {
            if (_colaboradorContext.tb_colaborador.Where(x => x.codigo_interno_colaborador == colaborador.Cpf).Count() > 0)
                AtualizaColaborador(colaborador, endereco);
            else
                InsereNovoColaborador(usuario, colaborador, endereco);
        }

        public bool CheckColaboradorOrg(string cpf, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org.Where(x => x.codigo_interno_colaborador == cpf && x.tb_org_id == orgId).Any();
        }

        public bool CheckColaboradorOrgAtivo(string cpf, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org.Where(x => x.codigo_interno_colaborador == cpf && x.tb_org_id == orgId && x.ativo == 1).Any();
        }

        public void UpsertColaboradorOrg(ColaboradorDTO colaborador, ColaboradorOrgDTO colaboradorOrg)
        {
            var colabOrgRow = _colaboradorContext.tb_colaborador_org
                .Where(x => x.cod_colaborador_externo == colaboradorOrg.CodColaborador && colaboradorOrg.OrgId == x.tb_org_id).FirstOrDefault();
            if (colabOrgRow != null)
            {
                AtualizaColaboradorOrg(colabOrgRow, colaboradorOrg);
            }
            else
            {
                InsereColaboradorOrg(colaborador, colaboradorOrg);
            }
        }

        private void AtualizaColaboradorOrg(tb_colaborador_org colabOrgRow, ColaboradorOrgDTO colaboradorOrg)
        {
            colabOrgRow.cargo = colaboradorOrg.Cargo;
            colabOrgRow.cod_colaborador_externo = colaboradorOrg.CodColaborador;
            colabOrgRow.codigo_interno_colaboradorNavigation = _colaboradorContext.tb_colaborador.Find(colaboradorOrg.Cpf);
            colabOrgRow.cod_departamento = colaboradorOrg.CodDepartamento;
            colabOrgRow.cod_diretoria = colaboradorOrg.CodDiretoria;
            colabOrgRow.codigo_cargo = colaboradorOrg.CodCargo;
            colabOrgRow.data_alteracao = DateTime.Now;
            colabOrgRow.departamento = colaboradorOrg.Departamento;
            colabOrgRow.diretoria = colaboradorOrg.Diretoria;
            colabOrgRow.data_admissao = colaboradorOrg.DataAdmissao;
            colabOrgRow.modelo_contratacao = colaboradorOrg.ModeloContratacao;
            colabOrgRow.empresa_relacionada = colaboradorOrg.EmpresaRelacionada?.ToUpperInvariant();
            colabOrgRow.modelo_trabalho = colaboradorOrg.ModeloTrabalho;
            colabOrgRow.dias_por_semana = colaboradorOrg.DiasPorSemana;
            colabOrgRow.valor_hora = colaboradorOrg.ValorHora;
            colabOrgRow.custo_hora = colaboradorOrg.CustoHora;
            colabOrgRow.base_hora_mes = colaboradorOrg.BaseHoraMes;
            colabOrgRow.ativo = (sbyte)(colaboradorOrg.Ativo ? 1 : 0);
            colabOrgRow.codigo_modelo_contratacao = colaboradorOrg.CodigoModeloContratacao?.ToNullSeTextoNull();
            _colaboradorContext.tb_colaborador_org.Update(colabOrgRow);
            _colaboradorContext.SaveChanges();
        }

        private void InsereColaboradorOrg(ColaboradorDTO colaborador, ColaboradorOrgDTO colaboradorOrg)
        {
            var colaboradorOrgRow = new tb_colaborador_org
            {
                cargo = colaboradorOrg.Cargo,
                cod_colaborador_externo = colaboradorOrg.CodColaborador,
                cod_departamento = colaboradorOrg.CodDepartamento,
                cod_diretoria = colaboradorOrg.CodDiretoria,
                codigo_cargo = colaboradorOrg.CodCargo,
                data_alteracao = DateTime.Now,
                departamento = colaboradorOrg.Departamento,
                diretoria = colaboradorOrg.Diretoria,
                codigo_interno_colaborador = colaborador.Cpf,
                tb_org_id = colaboradorOrg.OrgId,
                data_admissao = colaboradorOrg.DataAdmissao,
                modelo_contratacao = colaboradorOrg.ModeloContratacao,
                empresa_relacionada = colaboradorOrg.EmpresaRelacionada?.ToUpperInvariant(),
                modelo_trabalho = colaboradorOrg.ModeloTrabalho,
                dias_por_semana = colaboradorOrg.DiasPorSemana,
                valor_hora = colaboradorOrg.ValorHora,
                custo_hora = colaboradorOrg.CustoHora,
                base_hora_mes = colaboradorOrg.BaseHoraMes,
                ativo = (sbyte)(colaboradorOrg.Ativo ? 1 : 0),
                codigo_modelo_contratacao = colaboradorOrg.CodigoModeloContratacao?.ToNullSeTextoNull()
            };
            _colaboradorContext.tb_colaborador_org.Add(colaboradorOrgRow);
            _colaboradorContext.SaveChanges();

            _colaboradorContext.ChangeTracker.Clear();
        }

        private void InsereNovoColaborador(UsuarioColaboradorDTO usuario, ColaboradorDTO colaborador, EnderecoDTO endereco)
        {
            try
            {
                tb_endereco rowEndereco = null;
                if (endereco != null)
                {
                    rowEndereco = new tb_endereco();

                    rowEndereco.cep = endereco.Cep;
                    rowEndereco.cidade = endereco.Cidade;
                    rowEndereco.complemento = endereco.Complemento;
                    rowEndereco.estado = endereco.Estado;
                    rowEndereco.endereco = endereco.Endereco;
                    rowEndereco.numero = endereco.Numero;
                    rowEndereco.bairro = endereco.Bairro;
                    rowEndereco.com_quem_mora = endereco.ComQuemMora;

                    _colaboradorContext.tb_endereco.Add(rowEndereco);
                    _colaboradorContext.SaveChanges();
                }

                InserirOuAtualizarSomenteTbColaborador(colaborador, rowEndereco?.id);

                if (usuario != null)
                {
                    if (_colaboradorContext.tb_usuario.Where(x => x.codigo_interno_colaborador == usuario.Cpf).Count() > 0)
                    {
                        var userRow = _colaboradorContext.tb_usuario.Where(x => x.codigo_interno_colaborador == usuario.Cpf).First();
                        userRow.primeiro_acesso_realizado = 1;
                        userRow.email = usuario.Email;
                        _colaboradorContext.tb_usuario.Update(userRow);
                    }
                    else
                    {
                        var usuarioRow = new tb_usuario();

                        usuarioRow.ativo = 1;
                        usuarioRow.codigo_interno_colaborador = usuario.Cpf;
                        usuarioRow.email = usuario.Email;
                        usuarioRow.fcm_token = null;
                        usuarioRow.password = "";
                        usuarioRow.primeiro_acesso_realizado = 0;
                        usuarioRow.tb_org_id = usuario.OrgId;

                        _colaboradorContext.tb_usuario.Add(usuarioRow);
                    }
                }

                _colaboradorContext.SaveChanges();

                var colaboradorStatus = new tb_colaborador_status();
                colaboradorStatus.ativo = 1;
                colaboradorStatus.data_alteracao = DateTime.Now;
                colaboradorStatus.data_criacao = DateTime.Now;
                colaboradorStatus.status_colaborador_id = 1;
                colaboradorStatus.codigo_interno_colaborador = colaborador.Cpf;

                _colaboradorContext.tb_colaborador_status.Add(colaboradorStatus);
                _colaboradorContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void InserirOuAtualizarSomenteTbColaborador(ColaboradorDTO colaborador, long? enderecoId = null)
        {
            var colaboradorExistente = _colaboradorContext.tb_colaborador.FirstOrDefault(c => c.codigo_interno_colaborador == colaborador.Cpf);

            var row = colaboradorExistente != null ? colaboradorExistente : new tb_colaborador();

            if (colaboradorExistente is not null)
            {
                row.ativo = 1;
                _colaboradorContext.tb_colaborador.Update(row);
            }
            else
            {
                row.codigo_interno_colaborador = colaborador.Cpf;
                row.documento_colaborador = colaborador.DocumentoColaborador;
                row.nome_completo = colaborador.NomeCompleto;
                row.data_nascimento = colaborador.DataNascimento != null && colaborador.DataNascimento != DateTime.MinValue ? colaborador.DataNascimento : (DateTime?)null;
                row.rg = colaborador.Rg;
                row.matricula = colaborador.Matricula;
                row.endereco_id = enderecoId;
                row.data_criacao = DateTime.Now;
                row.data_alteracao = DateTime.Now;
                row.contato_principal_ddi = colaborador.ContatoPrincipalDDI;
                row.contato_principal = colaborador.ContatoPrincipal;
                row.contato_outro = colaborador.ContatoOutros;
                row.imagem_id = null;
                row.candidato = 0;
                row.ativo = 1;
                row.passaporte = colaborador.Passaporte;
                row.estado_civil = colaborador.EstadoCivil;
                row.genero = colaborador.Genero;
                row.etnia = colaborador.Etnia;
                row.escolaridade = colaborador.Escolaridade;
                row.orientacao_sexual = colaborador.OrientacaoSexual;
                row.refugiado = (SByte)(colaborador.PessoaRefugiada ? 1 : 0);
                row.email_alternativo = colaborador.EmailAlternativo;
                row.url_linkedin = colaborador.UrlLinkedin;
                row.visualizar_busca_aderencia = (SByte)(colaborador.VisualizarBuscaAderencia ? 1 : 0);

                _colaboradorContext.tb_colaborador.Add(row);
                _colaboradorContext.SaveChanges();
            }
        }

        public void EditarColaborador(CadastroColaboradorInput colaboradorInput, int orgId)
        {
            try
            {
                
                var colaborador = _colaboradorContext.tb_colaborador.FirstOrDefault(c => c.codigo_interno_colaborador == colaboradorInput.Cpf);
                if (colaborador != null)
                {
                    colaborador.documento_colaborador = StringUtil.SomenteNumeros(colaboradorInput.DocumentoColaborador);
                    colaborador.nome_completo = colaboradorInput.NomeColaborador;
                    colaborador.data_alteracao = DateTime.Now;
                    colaborador.contato_principal_ddi = colaboradorInput.ContatoPrincipalDDI;
                    colaborador.contato_principal = colaboradorInput.ContatoPrincipal;
                    colaborador.visualizar_busca_aderencia = (SByte)(colaboradorInput.ConsiderarVisualizacaoAderentes ? 1 : 0);
                    _colaboradorContext.tb_colaborador.Update(colaborador);
                }

                var usuario = _colaboradorContext.tb_usuario.FirstOrDefault(u => u.codigo_interno_colaborador == colaboradorInput.Cpf);
                if (usuario != null)
                {
                    usuario.email = colaboradorInput.Email;

                    _colaboradorContext.tb_usuario.Update(usuario);
                }
                else 
                {
                    if (colaboradorInput.Email.EhStringValidaEDiferenteDeZero())
                    {
                        var usuario_model = new tb_usuario
                        {
                            email = colaboradorInput.Email,
                            codigo_interno_colaborador = colaboradorInput.Cpf,
                            sistemico = 0,
                            ativo = 1,
                            tb_org_id = orgId,
                            data_alteracao = DateTime.UtcNow,
                            data_criacao = DateTime.UtcNow,
                            password = String.Empty
                        };
                        _colaboradorContext.tb_usuario.Add(usuario_model);
                    }
                }

                var colaboradorOrg = _colaboradorContext.tb_colaborador_org.FirstOrDefault(o => o.codigo_interno_colaborador == colaboradorInput.Cpf && o.tb_org_id == orgId);
                if (colaboradorOrg != null)
                {
                    var codigoModeloContratacao = colaboradorInput.ModeloContratacao.EhStringValidaEDiferenteDeZero() ? colaboradorInput.ModeloContratacao : null;
                    
                    string cargoInput = colaboradorInput.Cargo.ToStringOuVazio();
                    string codigoCargoInput = colaboradorInput.CodigoCargo.ToStringOuVazio();

                    // Sempre garantem string (nunca null)
                    string cargo = cargoInput;  
                    string codigoCargo = string.IsNullOrWhiteSpace(codigoCargoInput)
                        ? cargoInput
                        : codigoCargoInput;
                    
                    colaboradorOrg.cod_colaborador_externo = colaboradorInput.CodColaborador;
                    colaboradorOrg.cod_diretoria = colaboradorInput.CodDiretoria;
                    colaboradorOrg.diretoria = colaboradorInput.Diretoria;
                    colaboradorOrg.cod_departamento = colaboradorInput.CodDepartamento;
                    colaboradorOrg.departamento = colaboradorInput.Departamento;
                    colaboradorOrg.data_admissao = colaboradorInput.DataAdmissao;
                    colaboradorOrg.modelo_contratacao = colaboradorInput.ModeloContratacao;
                    colaboradorOrg.empresa_relacionada = colaboradorInput.EmpresaRelacionada?.ToUpperInvariant();
                    colaboradorOrg.modelo_trabalho = colaboradorInput.ModeloTrabalho;
                    colaboradorOrg.dias_por_semana = colaboradorInput.DiasPorSemana;
                    colaboradorOrg.valor_hora = colaboradorInput.ValorHora;
                    colaboradorOrg.custo_hora = colaboradorInput.CustoHora;
                    colaboradorOrg.base_hora_mes = colaboradorInput.BaseHoraMes;
                    colaboradorOrg.ativo = (sbyte)(colaboradorInput.Ativo ? 1 : 0);
                    colaboradorOrg.codigo_modelo_contratacao = codigoModeloContratacao;
                    colaboradorOrg.cargo = cargo;
                    colaboradorOrg.codigo_cargo = codigoCargo;

                    if (colaboradorInput.Ativo)
                    {
                        colaboradorOrg.data_inativacao = null;
                    }
                    else
                    {
                        colaboradorOrg.data_inativacao = colaboradorInput.DataInativacao ?? DateTime.Now;
                    }

                    _colaboradorContext.tb_colaborador_org.Update(colaboradorOrg);
                }

                var colaboradorHierarquia = _colaboradorContext.tb_colaborador_hierarquia
                                            .Include(h => h.tb_org)
                                            .FirstOrDefault(h => h.cod_colaborador_externo == colaboradorInput.CodColaborador);

                if (colaboradorHierarquia != null)
                {
                    _colaboradorContext.tb_colaborador_hierarquia.Remove(colaboradorHierarquia);
                    _colaboradorContext.SaveChanges();
                }

                var novoColaboradorHierarquia = new tb_colaborador_hierarquia
                {
                    cod_colaborador_externo = colaboradorInput.CodColaborador,
                    cod_colaborador_superior = colaboradorInput.CodGestor,
                    tb_org_id = orgId
                };
                _colaboradorContext.tb_colaborador_hierarquia.Add(novoColaboradorHierarquia);

                _colaboradorContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao editar colaborador.", ex);
            }
        }

        public async Task EditarColaboradorDapperAsync(CadastroColaboradorInput colaboradorInput, int orgId)
        {
            try
            {
                var existeColaborador = await ExisteColaboradorComCodigoInternoDapperAsync(colaboradorInput.Cpf);
                if (existeColaborador)
                {
                    var colaborador = new tb_colaborador();
                    colaborador.documento_colaborador = StringUtil.SomenteNumeros(colaboradorInput.DocumentoColaborador);
                    colaborador.nome_completo = colaboradorInput.NomeColaborador;
                    colaborador.data_alteracao = DateTime.Now;
                    colaborador.contato_principal_ddi = colaboradorInput.ContatoPrincipalDDI;
                    colaborador.contato_principal = colaboradorInput.ContatoPrincipal;
                    colaborador.visualizar_busca_aderencia = (SByte)(colaboradorInput.ConsiderarVisualizacaoAderentes ? 1 : 0);
                    colaborador.codigo_interno_colaborador = colaboradorInput.Cpf;
                    
                    await EditarColaboradorInternoDapperAsync(colaborador);
                }
                
                var existeUsuario = await ExisteUsuarioComCodigoInternoEOrgIdDapperAsync(colaboradorInput.Cpf, orgId);
                
                if (existeUsuario)
                {
                    await EditarEmailUsuarioDapperAsync(colaboradorInput.Email.ToLower(), colaboradorInput.Cpf, orgId);
                }
                else
                {
                    if (colaboradorInput.Email.EhStringValidaEDiferenteDeZero())
                    {
                        var usuario = new tb_usuario
                        {
                            email = colaboradorInput.Email,
                            codigo_interno_colaborador = colaboradorInput.Cpf,
                            sistemico = 0,
                            ativo = 1,
                            tb_org_id = orgId,
                            data_alteracao = DateTime.UtcNow,
                            data_criacao = DateTime.UtcNow,
                            password = String.Empty
                        };

                        await InserirUsuarioDapperAsync(usuario);
                    }
                }
                
                var existeColaboradorOrg = await ExisteColaboradorOrgComCodigoInternoEOrgIdDapperAsync(colaboradorInput.Cpf, orgId);
                
                if (existeColaboradorOrg)
                {
                    var colaboradorOrg = new tb_colaborador_org();
                    var codigoModeloContratacao = colaboradorInput.ModeloContratacao.EhStringValidaEDiferenteDeZero() ? colaboradorInput.ModeloContratacao : null;
                    
                    string cargoInput = colaboradorInput.Cargo.ToStringOuVazio();
                    string codigoCargoInput = colaboradorInput.CodigoCargo.ToStringOuVazio();

                    // Sempre garantem string (nunca null)
                    string cargo = cargoInput;  
                    string codigoCargo = string.IsNullOrWhiteSpace(codigoCargoInput)
                        ? cargoInput
                        : codigoCargoInput;
                    
                    colaboradorOrg.cod_colaborador_externo = colaboradorInput.CodColaborador;
                    colaboradorOrg.cod_diretoria = colaboradorInput.CodDiretoria;
                    colaboradorOrg.diretoria = colaboradorInput.Diretoria;
                    colaboradorOrg.cod_departamento = colaboradorInput.CodDepartamento;
                    colaboradorOrg.departamento = colaboradorInput.Departamento;
                    colaboradorOrg.data_admissao = colaboradorInput.DataAdmissao;
                    colaboradorOrg.modelo_contratacao = colaboradorInput.ModeloContratacao;
                    colaboradorOrg.empresa_relacionada = colaboradorInput.EmpresaRelacionada?.ToUpperInvariant();
                    colaboradorOrg.modelo_trabalho = colaboradorInput.ModeloTrabalho;
                    colaboradorOrg.dias_por_semana = colaboradorInput.DiasPorSemana;
                    colaboradorOrg.valor_hora = colaboradorInput.ValorHora;
                    colaboradorOrg.custo_hora = colaboradorInput.CustoHora;
                    colaboradorOrg.base_hora_mes = colaboradorInput.BaseHoraMes;
                    colaboradorOrg.ativo = (sbyte)(colaboradorInput.Ativo ? 1 : 0);
                    colaboradorOrg.codigo_modelo_contratacao = codigoModeloContratacao;
                    colaboradorOrg.cargo = cargo;
                    colaboradorOrg.codigo_cargo = codigoCargo;

                    if (colaboradorInput.Ativo)
                    {
                        colaboradorOrg.data_inativacao = null;
                    }
                    else
                    {
                        colaboradorOrg.data_inativacao = colaboradorInput.DataInativacao ?? DateTime.Now;
                    }

                    colaboradorOrg.codigo_interno_colaborador = colaboradorInput.Cpf;
                    colaboradorOrg.tb_org_id = orgId;
                    
                    await EditarColaboradorOrgDapperAsync(colaboradorOrg);
                }
                
                var existeColaboradorHierarquia = await ExisteColaboradorHirarquiaDapperAsync(colaboradorInput.CodColaborador, orgId);

                if (existeColaboradorHierarquia)
                {
                    await RemoverColaboradorHierarquia(colaboradorInput.CodColaborador, orgId);
                }

                if (!colaboradorInput.CodGestor.IsNullOrEmpty())
                {
                    var novoColaboradorHierarquia = new tb_colaborador_hierarquia
                    {
                        cod_colaborador_externo = colaboradorInput.CodColaborador,
                        cod_colaborador_superior = colaboradorInput.CodGestor,
                        tb_org_id = orgId
                    };
                
                    await InserirColaboradorHierarquia(novoColaboradorHierarquia);
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao editar colaborador.", ex);
            }
        }

        private async Task<bool> ExisteColaboradorComCodigoInternoDapperAsync(string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 1
                FROM tb_colaborador
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador
            ";
            var parametros = new
            {
                CodigoInternoColaborador = codigoInternoColaborador
            };

            var result = await connection.QueryFirstOrDefaultAsync<bool>(query, parametros);
            return result;
        }

        private async Task EditarColaboradorInternoDapperAsync(tb_colaborador colaborador)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                UPDATE tb_colaborador
                SET documento_colaborador = @documento_colaborador,
                    nome_completo = @nome_completo,
                    data_alteracao = @data_alteracao,
                    contato_principal_ddi = @contato_principal_ddi,
                    contato_principal = @contato_principal,
                    visualizar_busca_aderencia = @visualizar_busca_aderencia
                WHERE codigo_interno_colaborador = @codigo_interno_colaborador
            ";

            await connection.ExecuteAsync(query, colaborador);
        }
        
        private async Task<bool> ExisteUsuarioComCodigoInternoEOrgIdDapperAsync(string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 1
                FROM tb_usuario
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador
                AND tb_org_id = @OrgId
            ";
            var parametros = new
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                OrgId = orgId
            };

            var result = await connection.QueryFirstOrDefaultAsync<bool>(query, parametros);
            return result;
        }

        private async Task EditarEmailUsuarioDapperAsync(string email, string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                UPDATE tb_usuario
                SET email = @Email
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador
                AND tb_org_id = @OrgId
            ";

            var parametros = new
            {
                Email = email,
                CodigoInternoColaborador = codigoInternoColaborador,
                OrgId = orgId
            };

            await connection.ExecuteAsync(query, parametros);
        }

        private async Task InserirUsuarioDapperAsync(tb_usuario usuario)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                INSERT INTO tb_usuario
                (email, codigo_interno_colaborador, sistemico, ativo, tb_org_id, data_alteracao, data_criacao, password)
                VALUES 
                (@email, @codigo_interno_colaborador, @sistemico, @ativo, @tb_org_id, @data_alteracao, @data_criacao, @password)
            ";

            await connection.ExecuteAsync(query, usuario);
        }
        
        private async Task<bool> ExisteColaboradorOrgComCodigoInternoEOrgIdDapperAsync(string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 1
                FROM tb_colaborador_org
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador
                AND tb_org_id = @OrgId
            ";
            var parametros = new
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                OrgId = orgId
            };

            var result = await connection.QueryFirstOrDefaultAsync<bool>(query, parametros);
            return result;
        }

        private async Task EditarColaboradorOrgDapperAsync(tb_colaborador_org colaboradorOrg)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                UPDATE tb_colaborador_org
                SET 
                    cod_colaborador_externo = @cod_colaborador_externo,
                    cod_diretoria = @cod_diretoria,
                    diretoria = @diretoria,
                    cod_departamento = @cod_departamento,
                    departamento = @departamento,
                    data_admissao = @data_admissao,
                    modelo_contratacao = @modelo_contratacao,
                    empresa_relacionada = @empresa_relacionada,
                    modelo_trabalho = @modelo_trabalho,
                    dias_por_semana = @dias_por_semana,
                    valor_hora = @valor_hora,
                    custo_hora = @custo_hora,
                    base_hora_mes = @base_hora_mes,
                    ativo = @ativo,
                    codigo_modelo_contratacao = @codigo_modelo_contratacao,
                    cargo = @cargo,
                    codigo_cargo = @codigo_cargo,
                    data_inativacao = @data_inativacao
                WHERE
                    codigo_interno_colaborador = @codigo_interno_colaborador
                    AND tb_org_id = @tb_org_id
            ";

            await connection.ExecuteAsync(query, colaboradorOrg);
        }
        
        private async Task<bool> ExisteColaboradorHirarquiaDapperAsync(string codColaboradorExterno, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 1
                FROM tb_colaborador_hierarquia 
                WHERE cod_colaborador_externo = @CodColaboradorExterno
                AND tb_org_id = @OrgId
            ";
            var parametros = new
            {
                CodColaboradorExterno = codColaboradorExterno,
                OrgId = orgId
            };

            var result = await connection.QueryFirstOrDefaultAsync<bool>(query, parametros);
            return result;
        }

        public async Task InserirColaboradorHierarquia(tb_colaborador_hierarquia colaboradorHierarquia)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                INSERT INTO tb_colaborador_hierarquia
                (cod_colaborador_externo, cod_colaborador_superior, tb_org_id)
                VALUES 
                (@cod_colaborador_externo, @cod_colaborador_superior, @tb_org_id)
            ";

            await connection.ExecuteAsync(query, colaboradorHierarquia);
        }

        public async Task RemoverColaboradorHierarquia(string codColaboradorExteno, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                DELETE FROM tb_colaborador_hierarquia
                WHERE cod_colaborador_externo = @CodColaboradorExteno
                AND tb_org_id = @OrgId
            ";

            var parametros = new
            {
                CodColaboradorExteno = codColaboradorExteno,
                OrgId = orgId
            };

            await connection.ExecuteAsync(query, parametros);
        }
        
        private void AtualizaColaborador(ColaboradorDTO colaborador, EnderecoDTO endereco)
        {
            try
            {
                if (!_colaboradorContext.tb_colaborador
                    .Where(x => x.codigo_interno_colaborador == colaborador.Cpf).Any())
                    throw new Exception("Este colaborador não existe.");

                var colabRow = _colaboradorContext.tb_colaborador
                    .Where(x => x.codigo_interno_colaborador == colaborador.Cpf).FirstOrDefault();

                var rowEndereco = colabRow.endereco;
                if (rowEndereco != null)
                {
                    rowEndereco.cep = endereco.Cep;
                    rowEndereco.cidade = endereco.Cidade;
                    rowEndereco.complemento = endereco.Complemento;
                    rowEndereco.estado = endereco.Estado;
                    rowEndereco.endereco = endereco.Endereco;
                    rowEndereco.numero = endereco.Numero;
                    rowEndereco.bairro = endereco.Bairro;

                    _colaboradorContext.tb_endereco.Update(rowEndereco);
                }
                else
                {
                    rowEndereco = new tb_endereco();

                    rowEndereco.cep = endereco.Cep;
                    rowEndereco.cidade = endereco.Cidade;
                    rowEndereco.complemento = endereco.Complemento;
                    rowEndereco.estado = endereco.Estado;
                    rowEndereco.endereco = endereco.Endereco;
                    rowEndereco.numero = endereco.Numero;
                    rowEndereco.bairro = endereco.Bairro;
                    rowEndereco.com_quem_mora = "";

                    _colaboradorContext.tb_endereco.Add(rowEndereco);
                }

                _colaboradorContext.SaveChanges();

                colabRow.nome_completo = colaborador.NomeCompleto;
                colabRow.data_nascimento = colaborador.DataNascimento ?? DateTime.MinValue;
                colabRow.rg = colaborador.Rg;
                colabRow.endereco_id = rowEndereco.id;
                colabRow.data_alteracao = DateTime.Now;
                colabRow.contato_principal_ddi = colaborador.ContatoPrincipalDDI;
                colabRow.contato_principal = colaborador.ContatoPrincipal;
                colabRow.contato_outro = colaborador.ContatoOutros;
                colabRow.ativo = colaborador.FlagAtivo ? (sbyte)1 : (sbyte)0;
                colabRow.estado_civil = colaborador.EstadoCivil;
                colabRow.genero = colaborador.Genero;
                colabRow.etnia = colaborador.Etnia;
                colabRow.escolaridade = colaborador.Escolaridade;
                colabRow.orientacao_sexual = colaborador.OrientacaoSexual;
                colabRow.refugiado = (SByte)(colaborador.PessoaRefugiada ? 1 : 0);
                colabRow.email_alternativo = colaborador.EmailAlternativo;

                _colaboradorContext.tb_colaborador.Update(colabRow);
                _colaboradorContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void AtualizaEmailUsuario(UsuarioColaboradorDTO usuario)
        {
            var usuarioRow = _colaboradorContext.tb_usuario.FirstOrDefault(u => u.codigo_interno_colaborador == usuario.Cpf && u.email != usuario.Email && u.tb_org_id == usuario.OrgId);
            if (usuarioRow != null)
            {
                usuarioRow.email = usuario.Email;
                _colaboradorContext.tb_usuario.Update(usuarioRow);
                _colaboradorContext.SaveChanges();
            }
        }

        public void AtualizaDocumentoUsuario(String codInternoColaborador, String documentoColaborador)
        {
            throw new NotImplementedException();
        }

        public void AtualizaNomelUsuario(String codInternoColaborador, String nomeCompleto)
        {
            throw new NotImplementedException();
        }

        public void InserePrestadorServicoPessoaJuridica(string cnpj, PrestadorServicoDTO prestadorServico, RegimeTributarioDTO regimeTributario, string cpf)
        {
            try
            {
                if (_colaboradorContext.tb_pessoa_juridica
              .Where(x => x.cnpj == prestadorServico.Cnpj).Count() > 0)
                {
                    throw new Exception("Este prestador já existe.");
                }

                // Verifica se o código do banco é válido
                var bancoExistente = _colaboradorContext.tb_bancos.FirstOrDefault(b => b.codigo == prestadorServico.CodigoBanco);
                if (bancoExistente == null)
                {
                    throw new Exception("O código do banco é inválido");
                }
                var idRegimeTributario = _colaboradorContext.tb_regime_tributario.Where(x => x.descricao == regimeTributario.Descricao.ToString()).FirstOrDefault();

                var row = new tb_pessoa_juridica();

                row.cnpj = prestadorServico.Cnpj.ToString();
                row.tb_regime_tributario_id = idRegimeTributario.id;
                row.nome_fantasia = prestadorServico.NomeFantasia.ToString();
                row.razao_social = prestadorServico.RazaoSocial.ToString();
                row.agencia = prestadorServico.Agencia.ToString();
                row.conta_digito = prestadorServico.ContaDigito.ToString();
                row.tb_bancos_codigo = prestadorServico.CodigoBanco.ToString();
                row.codigo_interno_colaborador = cpf;
                _colaboradorContext.tb_pessoa_juridica.Add(row);
                _colaboradorContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void AlterarPrestadorServicoPessoaJuridica(string cnpj, PrestadorServicoDTO prestadorServico, RegimeTributarioDTO regimeTributario, string cpf)
        {
            try
            {
                if (!_colaboradorContext.tb_pessoa_juridica
              .Where(x => x.cnpj == prestadorServico.Cnpj).Any())
                {
                    throw new Exception("Este prestador Não existe.");
                }

                // Verifica se o código do banco é válido
                var bancoExistente = _colaboradorContext.tb_bancos.FirstOrDefault(b => b.codigo == prestadorServico.CodigoBanco);
                if (bancoExistente == null)
                {
                    throw new Exception("O código do banco é inválido");
                }
                var idRegimeTributario = _colaboradorContext.tb_regime_tributario.Where(x => x.descricao == regimeTributario.Descricao.ToString()).FirstOrDefault();

                var row = new tb_pessoa_juridica();

                row.cnpj = prestadorServico.Cnpj.ToString();
                row.tb_regime_tributario_id = idRegimeTributario.id;
                row.nome_fantasia = prestadorServico.NomeFantasia.ToString();
                row.razao_social = prestadorServico.RazaoSocial.ToString();
                row.agencia = prestadorServico.Agencia.ToString();
                row.conta_digito = prestadorServico.ContaDigito.ToString();
                row.tb_bancos_codigo = prestadorServico.CodigoBanco.ToString();
                row.codigo_interno_colaborador = cpf;
                _colaboradorContext.tb_pessoa_juridica.Update(row);
                _colaboradorContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public bool ExisteColaborador(string cpf)
        {
            return _colaboradorContext.tb_colaborador
                    .Where(x => x.codigo_interno_colaborador == cpf).Any();
        }

        public bool ExisteColaboradorComCPF(string cpf, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org
                    .Any(x => x.tb_org_id == orgId && x.codigo_interno_colaboradorNavigation.documento_colaborador == cpf);
        }

        public bool ExisteColaboradorComEmail(string email, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org
                    .Any(x => x.tb_org_id == orgId
                              && x.codigo_interno_colaboradorNavigation.tb_usuario.Any(u => u.email == email && u.tb_org_id == orgId));
        }

        public bool ExisteColaboradorComEmailAlternativo(string email, int orgId)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            email = email.Trim();

            var connection = _dapperConnection.GetConnection();
            const string query = @"
                SELECT 1
                FROM tb_colaborador tc
                INNER JOIN tb_gestor_externo tge ON tc.codigo_interno_colaborador = tge.codigo_interno_colaborador
                WHERE tge.tb_org_id = @OrgId
                  AND tge.ativo = 1
                  AND tc.email_alternativo = @Email
                LIMIT 1";

            var result = connection.QueryFirstOrDefault<int?>(query, new { OrgId = orgId, Email = email });
            return result.HasValue;
        }

        public bool ExisteColaboradorComCodColaboradorExterno(string codColaboradorExt, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org
                    .Any(x => x.tb_org_id == orgId && x.cod_colaborador_externo == codColaboradorExt);
        }

        public bool ExisteColaboradorComCodColaboradorExternoECpf(string codColaboradorExt, string cpf, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org
                    .Any(x => x.tb_org_id == orgId && x.cod_colaborador_externo == codColaboradorExt && x.codigo_interno_colaborador == cpf);
        }

        public bool SeTokenSistema(string token)
        {
            return _colaboradorContext.tb_token_sistema
                    .Where(x => x.token == token).Any();
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

        public void DeletaHierarquiaColaborador(int orgId)
        {
            _colaboradorContext.tb_colaborador_hierarquia.RemoveRange(_colaboradorContext.tb_colaborador_hierarquia.Where(x => x.tb_org_id == orgId));
            _colaboradorContext.SaveChanges();
        }

        public void DeletaHierarquiaColaboradorPorCodColaborador(string codColaborador, int orgId)
        {
            _colaboradorContext.tb_colaborador_hierarquia.RemoveRange(
                _colaboradorContext.tb_colaborador_hierarquia.Where(x => x.cod_colaborador_externo == codColaborador
                                                                         && x.tb_org_id == orgId)
                                                                     );
            _colaboradorContext.SaveChanges();
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

        public void DeletaColaboradorProjetoOrg(int orgId, string codColaborador)
        {
            _colaboradorContext.tb_colaborador_projeto_org.RemoveRange(_colaboradorContext.tb_colaborador_projeto_org.Where(x => x.tb_org_id == orgId && x.cod_colaborador == codColaborador));
            _colaboradorContext.SaveChanges();
        }

        public void InsereColaboradorProjetoOrg(List<ColaboradorProjetoCargaDTO> colaboradoresProjetoCarga, int orgId)
        {
            _colaboradorContext.tb_colaborador_projeto_org.AddRange(colaboradoresProjetoCarga.Select(colaboradorProjeto => new tb_colaborador_projeto_org
            {
                tb_org_id = orgId,
                cod_colaborador = colaboradorProjeto.CodigoColaborador,
                cod_projeto = colaboradorProjeto.CodigoProjeto,
                nome_projeto = colaboradorProjeto.NomeProjeto
            }).ToList()
            );
            _colaboradorContext.SaveChanges();
        }

        public void AlteraSenhaUsuario(string cpf, string senha, int orgId)
        {
            var senhaHash = Criptografia.MD5Hash(senha);
            var usuarioRow = _colaboradorContext.tb_usuario.Where(x => x.codigo_interno_colaborador == cpf && x.tb_org_id == orgId).FirstOrDefault();
            if (usuarioRow == null)
                throw new Exception("Usuário não existe");
            usuarioRow.password = senhaHash;
            _colaboradorContext.tb_usuario.Update(usuarioRow);
            _colaboradorContext.SaveChanges();
        }

        public string GetColaboradorCpf(string codColaborador, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org.Where(x => x.cod_colaborador_externo == codColaborador && x.tb_org_id == orgId).FirstOrDefault()?.codigo_interno_colaborador ?? null;
        }
        public void AtualizaListaColaboradorProjetoOrg(List<ColaboradorProjetoCargaDTO> colaboradoresProjetoCarga, string codColaborador, int orgId)
        {
            throw new NotImplementedException();
        }

        public string GetCodigoInternoByDocumentoEOrg(string documentoColaborador, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org.Where(x => x.codigo_interno_colaboradorNavigation.documento_colaborador == documentoColaborador && x.tb_org_id == orgId).FirstOrDefault()?.codigo_interno_colaborador ?? null;
        }

        public string GetCodigoInternoByCodExterno(string codExterno, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org.Where(x => x.cod_colaborador_externo == codExterno && x.tb_org_id == orgId).FirstOrDefault()?.codigo_interno_colaborador ?? null;
        }

        public async Task<bool> AlterarIdiomaPadrao(string idioma, string cpf, int orgId)
        {
            var colaborador = await _colaboradorContext.tb_colaborador_org.Where(x => x.codigo_interno_colaborador == cpf && x.tb_org_id == orgId).FirstOrDefaultAsync();
            if (colaborador == null)
                return false;

            try
            {
                colaborador.idioma = idioma;
                _colaboradorContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

            return true;
        }

        public void AtualizaDadosBasicosUsuario(string codInternoColaborador, DateTime dataNascimento)
        {
            throw new NotImplementedException();
        }

        public UsuarioColaboradorDTO BuscaColaboradorPorCodigo(string codColaborador, int orgId)
        {
            var colaborador = _colaboradorContext.tb_colaborador_org
                .Where(x => x.codigo_interno_colaborador == codColaborador && x.tb_org_id == orgId)
                .ToList()
                .Select(x =>
                {
                    var usuario = x.codigo_interno_colaboradorNavigation.tb_usuario.Where(u => u.tb_org_id == orgId).FirstOrDefault();
                    return new UsuarioColaboradorDTO
                    {
                        NomeColaborador = x.codigo_interno_colaboradorNavigation.nome_completo,
                        Email = usuario?.email
                    };
                })
                .FirstOrDefault();

            return colaborador;
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

            InsereColaboradorOrg(new ColaboradorDTO { Cpf = usuario.Cpf }, colaboradorOrg);
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
                                    AND vwgha.cod_interno_gestor = @CodigoInternoGerente
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
            try
            {
                // Busca o colaborador
                var colaborador = _colaboradorContext.tb_colaborador
                    .FirstOrDefault(c => c.codigo_interno_colaborador == codigoInternoColaborador);

                if (colaborador == null)
                {
                    throw new Exception("Colaborador não encontrado");
                }

                // Verifica se o colaborador já tem endereço
                if (colaborador.endereco_id.HasValue)
                {
                    // Atualiza o endereço existente
                    var endereco = _colaboradorContext.tb_endereco
                        .FirstOrDefault(e => e.id == colaborador.endereco_id.Value);

                    if (endereco != null)
                    {
                        endereco.cidade = cidade;
                        endereco.estado = estado;
                        _colaboradorContext.tb_endereco.Update(endereco);
                    }
                }
                else
                {
                    // Cria um novo endereço
                    var novoEndereco = new tb_endereco
                    {
                        cidade = cidade,
                        estado = estado,
                        com_quem_mora = ""
                    };

                    _colaboradorContext.tb_endereco.Add(novoEndereco);
                    _colaboradorContext.SaveChanges();

                    // Associa o endereço ao colaborador
                    colaborador.endereco_id = novoEndereco.id;
                    _colaboradorContext.tb_colaborador.Update(colaborador);
                }

                _colaboradorContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar endereço do colaborador: {ex.Message}", ex);
            }
        }

        public void AtualizarContatoEEmailAlternativo(string codigoInternoColaborador, string contatoPrincipal, string emailAlternativo)
        {
            try
            {
                // Busca o colaborador
                var colaborador = _colaboradorContext.tb_colaborador
                    .FirstOrDefault(c => c.codigo_interno_colaborador == codigoInternoColaborador);

                if (colaborador == null)
                {
                    throw new Exception("Colaborador não encontrado");
                }

                // Atualiza contato_principal e email_alternativo
                colaborador.contato_principal = contatoPrincipal;
                colaborador.email_alternativo = emailAlternativo;

                _colaboradorContext.tb_colaborador.Update(colaborador);
                _colaboradorContext.SaveChanges();
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
                FROM tb_colaborador_org tco
                WHERE tco.tb_org_id =  @OrgId
                AND tco.ativo = 1
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
    }
}