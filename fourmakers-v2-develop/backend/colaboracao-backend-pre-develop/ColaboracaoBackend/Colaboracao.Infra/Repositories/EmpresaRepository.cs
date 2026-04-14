using Colaboracao.Infra.Context;
using Core.Domain;
using DataTransferObject.Domain.Empresa;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        public EmpresaRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public void AddUser(string cnpj, long usuarioId, bool pendente, bool responsavelAcesso)
        {
            try
            {
                var row = new tb_empresa_usuario();
                if (pendente && !responsavelAcesso)
                    row.confirmado = 0;
                else
                    row.confirmado = 1;
                row.responsavel_acesso = (sbyte)(responsavelAcesso == true ? 1 : 0);
                row.pendente = (sbyte)(pendente == true ? 1 : 0);
                row.data_convite = DateTime.Now;
                row.tb_empresa_cnpjNavigation = _colaboradorContext.tb_empresa.Find(cnpj) ?? throw new Exception("Empresa não encontrada");
                row.tb_usuario = _colaboradorContext.tb_usuario.Find(usuarioId) ?? throw new Exception("Usuario não encontrado");
                _colaboradorContext.tb_empresa_usuario.Add(row);
                _colaboradorContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void ConfirmaConvite(string cnpj, long usuarioId, bool confirmado)
        {
            try
            {
                var row = _colaboradorContext.tb_empresa_usuario.Where(x => x.tb_empresa_cnpj == cnpj && x.tb_usuario_id == usuarioId).FirstOrDefault();
                if (row == null)
                {
                    throw new Exception("Usuario ou Empresa não encontrados");
                }
                row.confirmado = (sbyte)(confirmado ? 1 : 0);
                _colaboradorContext.tb_empresa_usuario.Update(row);
                _colaboradorContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public EmpresaDTO GetById(string cnpj)
        {
            try
            {
                var row = _colaboradorContext.tb_empresa.Find(cnpj);
                if (row == null)
                    return null;

                return new EmpresaDTO
                {
                    Cnpj = row.cnpj,
                    Descricao = row.descricao,
                    Linkedin = row.linkedin,
                    NomeFantasia = row.nome_fantasia,
                    RazaoSocial = row.razao_social,
                    Site = row.site
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public long GetUsuarioAcesso(string cnpj)
        {
            try
            {
                var row = _colaboradorContext.tb_empresa.Find(cnpj);
                if (row == null)
                    throw new Exception("Empresa não encontrada");
                return row.tb_empresa_usuario.Where(x => x.responsavel_acesso == 1).Select(x => x.tb_usuario_id).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<UsuarioEmpresaDTO> ListaUsuariosEmpresa(string cnpj)
        {
            try
            {
                var ret = new List<UsuarioEmpresaDTO>();
                var rows = _colaboradorContext.tb_empresa_usuario.Where(x => x.tb_empresa_cnpj == cnpj && x.ativo == 1).ToList();
                if (rows != null && rows.Count() > 0)
                {
                    foreach (var usuarioEmpresa in rows)
                    {
                        var nome = usuarioEmpresa.tb_usuario.codigo_interno_colaboradorNavigation.nome_completo;
                        ret.Add(new UsuarioEmpresaDTO
                        {
                            AcessoPermitido = usuarioEmpresa.confirmado == (sbyte)1,
                            Cpf = usuarioEmpresa.tb_usuario.codigo_interno_colaborador,
                            Email = usuarioEmpresa.tb_usuario.email,
                            DataConvite = usuarioEmpresa.data_convite,
                            NomeCompleto = nome
                        }); ;
                    }
                }

                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void RemoveUser(string cnpj, long usuarioId)
        {
            try
            {
                var row = _colaboradorContext.tb_empresa_usuario.Where(x => x.tb_empresa_cnpj == cnpj && x.tb_usuario_id == usuarioId).FirstOrDefault();
                if (row == null)
                {
                    throw new Exception("Usuario ou Empresa não encontrados");
                }
                _colaboradorContext.tb_empresa_usuario.Remove(row);
                _colaboradorContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public EmpresaDTO Save(EmpresaDTO empresa, long usuarioId)
        {
            try
            {
                var row = new tb_empresa();
                row.cnpj = empresa.Cnpj;
                row.descricao = empresa.Descricao;
                row.linkedin = empresa.Linkedin;
                row.nome_fantasia = empresa.NomeFantasia;
                row.razao_social = empresa.RazaoSocial;
                row.site = empresa.RazaoSocial;
                _colaboradorContext.tb_empresa.Add(row);
                _colaboradorContext.SaveChanges();
                return empresa;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void Update(EmpresaDTO empresa)
        {
            try
            {
                var row = _colaboradorContext.tb_empresa.Find(empresa.Cnpj);
                if (row == null)
                    throw new Exception("Empresa não encontrada");
                row.descricao = empresa.Descricao;
                row.linkedin = empresa.Linkedin;
                row.nome_fantasia = empresa.NomeFantasia;
                row.razao_social = empresa.RazaoSocial;
                row.site = empresa.RazaoSocial;
                _colaboradorContext.tb_empresa.Update(row);
                _colaboradorContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void UpdateUsuarioAcesso(string cnpj, long usuarioId)
        {
            try
            {
                var row = _colaboradorContext.tb_empresa_usuario.Where(x => x.tb_empresa_cnpj == cnpj && x.tb_usuario_id == usuarioId).FirstOrDefault();
                if (row == null)
                    throw new Exception("Usuario não pertence à empresa não encontrada");
                var rowEmpresa = _colaboradorContext.tb_empresa.Find(cnpj);
                if (rowEmpresa == null)
                    throw new Exception("Empresa não encontrada");
                var rowOldResponsavel = rowEmpresa.tb_empresa_usuario.Where(x => x.responsavel_acesso == 1).FirstOrDefault();
                if (row != null)
                {
                    rowOldResponsavel.responsavel_acesso = (sbyte)0;
                    _colaboradorContext.tb_empresa_usuario.Update(rowOldResponsavel);
                }
                row.responsavel_acesso = (sbyte)1;
                _colaboradorContext.tb_empresa_usuario.Update(row);
                _colaboradorContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public bool VerificaAcessoDados(long idUsuario, string cnpj)
        {
            try
            {
                bool ret = false;

                var aux = _colaboradorContext.tb_empresa_usuario.Where(x => x.tb_usuario_id == idUsuario && x.tb_empresa_cnpj.Contains(cnpj)).FirstOrDefault();

                if (aux != null)
                {
                    if (aux.confirmado == 1)
                    {
                        ret = true;
                    }
                }

                return ret;
            }
            catch
            {
                throw;
            }
        }

        public List<UsuarioCpfIdDTO> ListarUsuarioEmpresaPorId(List<long> idUsuarios)
        {
            var ret = new List<UsuarioCpfIdDTO>();
            var usuarios = new List<tb_usuario>();
            try
            {
                if (idUsuarios != null)
                {
                    foreach (long id in idUsuarios)
                    {
                        usuarios.Add(_colaboradorContext.tb_usuario.Where(x => x.id == id).FirstOrDefault());
                    }
                }

                if (usuarios != null)
                {
                    foreach (tb_usuario usuario in usuarios)
                    {
                        ret.Add(new UsuarioCpfIdDTO { Cpf = usuario.codigo_interno_colaborador, UsuarioId = usuario.id });
                    }
                }

                return ret;
            }
            catch
            {
                throw;
            }
        }
    }
}