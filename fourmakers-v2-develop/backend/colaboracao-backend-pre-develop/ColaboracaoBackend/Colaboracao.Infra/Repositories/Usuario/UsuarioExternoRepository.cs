using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.Domain.Usuario;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.Usuario;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Usuario
{
    public class UsuarioExternoRepository : IUsuarioExternoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public UsuarioExternoRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public UsuarioColaboradorDTO GetUserByEmailSenha(string email, string senhaMD5, int orgId)
        {
            var row = _colaboradorContext.tb_usuario.Where(x => x.email == email && x.password == senhaMD5 && x.ativo == 1 && x.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(tco => tco.tb_org_id == orgId && tco.ativo == 1)).FirstOrDefault();
            if (row == null)
                throw new Exception("Usuário não econtrado");
            return new UsuarioColaboradorDTO
            {
                Cpf = row.codigo_interno_colaborador,
                UsuarioId = row.id,
                Email = row.email
            };
        }
        public UsuarioColaboradorDTO GetUserByCPFSenha(string cpf, string senhaMD5, int orgId)
        {
            var cpfSemMascara = cpf.RemoveMascaraCpf();
            var row = _colaboradorContext.tb_usuario.Where(x =>
                x.codigo_interno_colaboradorNavigation.documento_colaborador == cpf
                && x.password == senhaMD5
                && x.ativo == 1
                && x.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(tco => tco.tb_org_id == orgId && tco.ativo == 1))
            .FirstOrDefault();

            if (row == null) return null;

            var colabOrg = row.codigo_interno_colaboradorNavigation.tb_colaborador_org.FirstOrDefault(tco => tco.tb_org_id == orgId);
            return new UsuarioColaboradorDTO
            {
                Cpf = row.codigo_interno_colaborador,
                UsuarioId = row.id,
                Email = row.email,
                ColaboradorOrg = new ColaboradorOrgDTO()
                {
                    Ativo = colabOrg != null && colabOrg.ativo == 1
                }
            };
        }

        public UsuarioColaboradorDTO GetUserByEmail(string email, int orgId)
        {
            var row = _colaboradorContext.tb_usuario.Where(x => x.email == email && x.ativo == 1 && x.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(tco => tco.tb_org_id == orgId && tco.ativo == 1)).FirstOrDefault();
            if (row == null)
                throw new Exception("Usuário não econtrado");
            return new UsuarioColaboradorDTO
            {
                Cpf = row.codigo_interno_colaborador,
                UsuarioId = row.id,
                Email = row.email,
                NomeColaborador = row.codigo_interno_colaboradorNavigation.nome_completo
            };
        }

        public bool IfPrimeiroAcessoUsuario(long usuarioId)
        {
            var user = _colaboradorContext.tb_usuario.Find(usuarioId);
            if (user != null)
            {
                return user.primeiro_acesso_realizado == (sbyte)1;
            }
            else
            {
                throw new Exception("Usuário não encontrado");
            }
        }

        public DateTime? GetDataAceiteTermo(long usuarioId)
        {
            var user = _colaboradorContext.tb_usuario.Find(usuarioId);
            if (user != null)
            {
                return user.dataAceiteTermo;
            }
            else
            {
                throw new Exception("Usuário não encontrado");
            }
        }

        public void AlteraSenhaUsuario(long usuarioId, string password)
        {
            var user = _colaboradorContext.tb_usuario.Find(usuarioId);
            user.password = password;
            _colaboradorContext.tb_usuario.Update(user);
            _colaboradorContext.SaveChanges();
        }

        public void SavePedidoReseteSenha(string token, long usuarioId, DateTime validade)
        {
            try
            {
                var oldRow = _colaboradorContext.tb_token_resete_senha.Where(x => x.tb_usuario_id == usuarioId).FirstOrDefault();
                if (oldRow != null)
                {
                    _colaboradorContext.tb_token_resete_senha.Remove(oldRow);
                    _colaboradorContext.SaveChanges();
                }
                var row = new tb_token_resete_senha();
                row.tb_usuario_id = usuarioId;
                row.validade = validade;
                row.token = token;
                _colaboradorContext.tb_token_resete_senha.Add(row);
                _colaboradorContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long GetUsuarioFromTokenReseteSenha(string token)
        {
            try
            {
                var row = _colaboradorContext.tb_token_resete_senha.Where(x => x.token == token).FirstOrDefault();
                if (row == null)
                    throw new Exception("Token não encontrado");
                if (row.validade < DateTime.Now)
                    throw new Exception("Token vencido");
                return row.tb_usuario_id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeletePedidoReseteSenha(string token)
        {
            try
            {
                var oldRow = _colaboradorContext.tb_token_resete_senha.Where(x => x.token == token).FirstOrDefault();
                if (oldRow != null)
                {
                    _colaboradorContext.tb_token_resete_senha.Remove(oldRow);
                    _colaboradorContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SubmeterConvite(string cpf, string cnpj, bool confirmado)
        {
            try
            {
                var usuarioRow = _colaboradorContext.tb_usuario.Where(x => x.codigo_interno_colaborador == cpf).FirstOrDefault();
                if (usuarioRow == null)
                    throw new Exception("Usuario não encontrado");
                var invite = _colaboradorContext.tb_empresa_usuario.Where(x => x.tb_usuario_id == usuarioRow.id && x.tb_empresa_cnpj == cnpj).FirstOrDefault();
                if (invite != null)
                {
                    invite.confirmado = confirmado ? (sbyte)1 : (sbyte)0;
                    _colaboradorContext.tb_empresa_usuario.Update(invite);
                    _colaboradorContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public long AlterarSenhaUsuarioLogado(string cpf)
        {
            var userid = _colaboradorContext.tb_usuario.Where(x => x.codigo_interno_colaborador == cpf).FirstOrDefault().id;

            return userid;
        }

        public ConfirmacaoEmailDTO RetornaEmailByCpf(string cpf)
        {
            try
            {
                var confirmacaoEmailDTO = _colaboradorContext.tb_usuario
                                                            .Where(u => u.codigo_interno_colaborador.Equals(cpf))
                                                            .Join(_colaboradorContext.tb_colaborador,
                                                                  u => u.codigo_interno_colaborador,
                                                                  c => c.codigo_interno_colaborador,
                                                                  (u, c) => new ConfirmacaoEmailDTO
                                                                  {
                                                                      UsuarioId = u.id,
                                                                      NomeColaborador = c.nome_completo,
                                                                      Cpf = u.codigo_interno_colaborador,
                                                                      Email = u.email,
                                                                      CodigoEnviado = 0,
                                                                      CodigoConfirmado = false
                                                                  })
                                                            .FirstOrDefault();

                return confirmacaoEmailDTO ?? new ConfirmacaoEmailDTO();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public bool SalvarAtualizarConfirmacaoEmail(int codigo, ConfirmacaoEmailDTO confirmacaoEmailDTO, bool reenviar)
        {
            bool enviarEmail = false;
            try
            {
                var existe_tb_confirmacao_email = GetConfirmacaoEmailByCpf(confirmacaoEmailDTO.Cpf);

                var confirmacaoEmail = new tb_confirmacao_email
                {
                    nome_colaborador = confirmacaoEmailDTO.NomeColaborador,
                    usuario_id = confirmacaoEmailDTO.UsuarioId,
                    codigo_interno_colaborador = confirmacaoEmailDTO.Cpf,
                    email = confirmacaoEmailDTO.Email,
                    codigo_enviado = codigo,
                    codigo_confirmado = confirmacaoEmailDTO.CodigoConfirmado,
                    hora_expiracao_codigo = confirmacaoEmailDTO.HoraExpiracaoCodigo
                };

                if (!String.IsNullOrEmpty(confirmacaoEmailDTO.Cpf) && String.IsNullOrEmpty(existe_tb_confirmacao_email.Cpf))
                {
                    AdicionarConfirmacaoEmail(confirmacaoEmail, codigo);
                    return true;
                }
                else if (reenviar == true)
                {
                    ReenviarCodigoConfirmacao(confirmacaoEmail, codigo);
                    return true;
                }
                else if (existe_tb_confirmacao_email.CodigoEnviado == codigo && VerificarExpiracaoCodigo(existe_tb_confirmacao_email.HoraExpiracaoCodigo))
                {
                    AtualizarConfirmacaoEmail(confirmacaoEmail);
                }
                else
                {
                    throw new("Código inválido.");
                }
                return enviarEmail;
            }
            catch (Exception e)
            {
                if (String.IsNullOrEmpty(confirmacaoEmailDTO.Cpf))
                {
                    throw new("EXCEPTION: Ocorreu um erro ao buscar CPF na base");
                }
                else
                {
                    throw;
                }
                return false;
            }
        }

        private void AdicionarConfirmacaoEmail(tb_confirmacao_email confirmacaoEmail, int codigo)
        {
            confirmacaoEmail.codigo_confirmado = false;
            confirmacaoEmail.hora_expiracao_codigo = DateTime.Now;
            confirmacaoEmail.codigo_enviado = codigo;
            _colaboradorContext.tb_confirmacao_email.Add(confirmacaoEmail);
            _colaboradorContext.SaveChanges();
        }

        private void AtualizarConfirmacaoEmail(tb_confirmacao_email confirmacaoEmail)
        {
            var tb_confirmacao_email = _colaboradorContext.tb_confirmacao_email.Where(x => x.codigo_interno_colaborador == confirmacaoEmail.codigo_interno_colaborador).FirstOrDefault();

            if (tb_confirmacao_email != null)
            {
                tb_confirmacao_email.codigo_confirmado = true;
                _colaboradorContext.tb_confirmacao_email.Update(tb_confirmacao_email);
                _colaboradorContext.SaveChanges();
            }
        }

        private bool ReenviarCodigoConfirmacao(tb_confirmacao_email confirmacaoEmail, int codigo)
        {
            var tb_confirmacao_email = _colaboradorContext.tb_confirmacao_email.Where(x => x.codigo_interno_colaborador == confirmacaoEmail.codigo_interno_colaborador).FirstOrDefault();

            if (tb_confirmacao_email != null)
            {
                tb_confirmacao_email.codigo_confirmado = false;
                tb_confirmacao_email.hora_expiracao_codigo = DateTime.Now;
                tb_confirmacao_email.codigo_enviado = codigo;

                _colaboradorContext.tb_confirmacao_email.Update(tb_confirmacao_email);
                _colaboradorContext.SaveChanges();
            }

            return true;
        }

        private static bool VerificarExpiracaoCodigo(DateTime tempoGerado)
        {
            // Verificar se o código ainda é válido após 2 minutos
            DateTime tempoExpiracao = tempoGerado.AddMinutes(2);

            if (DateTime.Now <= tempoExpiracao)
            {
                return true;
            }
            else
            {
                throw new("Código expirado.");
            }
        }

        public UsuarioColaboradorDTO GetUserByCpfEOrgId(string cpf, int orgId)
        {
            var row = _colaboradorContext.tb_usuario.AsNoTracking().Where(x => x.codigo_interno_colaborador == cpf && x.tb_org_id == orgId).FirstOrDefault();
            if (row == null)
                return null;
            return new UsuarioColaboradorDTO
            {
                Cpf = row.codigo_interno_colaborador,
                UsuarioId = row.id,
                Email = row.email
            };
        }

        public ConfirmacaoEmailDTO GetConfirmacaoEmailByCpf(string cpf)
        {
            DateTime minValue = DateTime.MinValue;
            var confirmacao_Email = _colaboradorContext.tb_confirmacao_email
                    .Where(u => u.codigo_interno_colaborador.Equals(cpf))
                    .Select(c => new ConfirmacaoEmailDTO
                    {
                        Id = c.id,
                        NomeColaborador = c.nome_colaborador,
                        UsuarioId = c.usuario_id ?? 0,
                        Cpf = c.codigo_interno_colaborador,
                        Email = c.email,
                        CodigoEnviado = c.codigo_enviado ?? -1,
                        CodigoConfirmado = c.codigo_confirmado ?? false,
                        HoraExpiracaoCodigo = c.hora_expiracao_codigo ?? minValue
                    }).FirstOrDefault();

            return confirmacao_Email ?? new ConfirmacaoEmailDTO();
        }

        public bool RetornaExisteEmail(string email, int orgId)
        {
            try
            {
                if (_colaboradorContext.tb_usuario.Any(u => u.email.Equals(email) && u.tb_org_id.Equals(orgId)))
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}