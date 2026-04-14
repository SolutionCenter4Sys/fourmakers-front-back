using ApiClient.Domain;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain.Usuario;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class UsuarioRepository : IUsuarioDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public UsuarioRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public UsuarioDTO GetUserByCpf(string cpf)
        {
            var registroDb = _colaboradorContext.tb_usuario
                .Where(x => x.codigo_interno_colaborador == cpf)
                .FirstOrDefault();

            return MapToDto(registroDb);
        }

        public UsuarioDTO GetUserByCpfAndOrg(string cpf, int orgId)
        {
            var registroDb = _colaboradorContext.tb_usuario
                .Where(x => x.codigo_interno_colaborador == cpf && x.tb_org_id == orgId)
                .FirstOrDefault();

            return MapToDto(registroDb);
        }

        public UsuarioDTO GetUserByLogin(string login, int orgId = 0)
        {
            tb_usuario registroDb;

            if (orgId == 0)
                registroDb = _colaboradorContext.tb_usuario
                    .Where(x => x.codigo_interno_colaborador == login || x.email == login)
                    .OrderBy(x => x.id)
                    .LastOrDefault();
            else
                registroDb = _colaboradorContext.tb_usuario
                    .Where(x => (x.codigo_interno_colaborador == login || x.email == login) && x.tb_org_id == orgId)
                    .OrderBy(x => x.id)
                    .LastOrDefault();

            return MapToDto(registroDb);
        }

        public UsuarioDTO GetUserByLoginAndOrg(string login, int orgId)
        {
            var registroDb = _colaboradorContext.tb_usuario
                .Where(x => (x.codigo_interno_colaborador == login || x.email == login) && x.tb_org_id == orgId)
                .OrderBy(x => x.id)
                .LastOrDefault();

            return MapToDto(registroDb);
        }

        public UsuarioDTO SaveUser(UsuarioDTO model)
        {
            if (_colaboradorContext.tb_usuario.Any(x => x.id == model.Id && model.Id != 0))
                throw new Exception("Este usuario já existe.");

            var row = new tb_usuario
            {
                ativo = 1,
                codigo_interno_colaborador = model.CodigoInternoColaborador,
                email = model.Email,
                fcm_token = model.FcmToken,
                password = model.Senha,
                primeiro_acesso_realizado = model.PrimeiroAcessoRealizado,
                tb_org_id = model.OrgId
            };

            _colaboradorContext.tb_usuario.Add(row);
            _colaboradorContext.SaveChanges();
            model.Id = row.id;

            return model;
        }

        public UsuarioDTO UpdateUser(UsuarioDTO model)
        {
            var userBanco = _colaboradorContext.tb_usuario
                .Where(x => x.id == model.Id)
                .FirstOrDefault();

            if (userBanco == null)
                throw new Exception("Usuário não encontrado para atualização.");

            userBanco.codigo_interno_colaborador = model.CodigoInternoColaborador;
            userBanco.email = model.Email;
            userBanco.fcm_token = model.FcmToken;
            userBanco.primeiro_acesso_realizado = model.PrimeiroAcessoRealizado;
            userBanco.dataAceiteTermo = model.DataAceiteTermo;
            userBanco.ativo = model.Ativo;
            userBanco.password = model.Senha;

            _colaboradorContext.tb_usuario.Update(userBanco);
            _colaboradorContext.SaveChanges();

            return model;
        }

        public void Logout(string login)
        {
            var registroDb = _colaboradorContext.tb_usuario
                .Where(x => x.codigo_interno_colaborador == login || x.email == login)
                .OrderBy(x => x.id)
                .LastOrDefault();

            if (registroDb != null)
            {
                registroDb.fcm_token = null;
                _colaboradorContext.tb_usuario.Update(registroDb);
                _colaboradorContext.SaveChanges();
            }
        }

        private static UsuarioDTO MapToDto(tb_usuario registroDb)
        {
            if (registroDb == null)
                return null;

            return new UsuarioDTO
            {
                Id = registroDb.id,
                CodigoInternoColaborador = registroDb.codigo_interno_colaborador,
                Email = registroDb.email,
                Senha = registroDb.password,
                FcmToken = registroDb.fcm_token,
                PrimeiroAcessoRealizado = registroDb.primeiro_acesso_realizado,
                Ativo = registroDb.ativo,
                Sistemico = registroDb.sistemico,
                DataExpiracao = registroDb.data_expiracao,
                OrgId = registroDb.tb_org_id
            };
        }
    }
}
