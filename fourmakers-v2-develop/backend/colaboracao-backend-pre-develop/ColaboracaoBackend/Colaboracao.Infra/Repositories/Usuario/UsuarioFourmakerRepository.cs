using Colaboracao.Infra.Context;
using Core.Domain.Usuario;
using DataTransferObject.Domain.Usuario;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Usuario
{
    public class UsuarioFourmakerRepository : IUsuarioFourmakerRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        public UsuarioFourmakerRepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        public void AddConvite(ConviteUsuarioExternoDTO convite, DateTime validade)
        {
            try
            {
                var oldInvite = _colaboradorContext.tb_convite_empresa.Where(x => x.cpf == convite.Cpf || x.email == convite.Email).FirstOrDefault();
                if (oldInvite != null)
                {
                    _colaboradorContext.tb_convite_empresa.Remove(oldInvite);
                    _colaboradorContext.SaveChanges();
                }
                var row = new tb_convite_empresa();
                row.cpf = convite.Cpf;
                row.tb_empresa_cnpj = convite.Cnpj;
                row.email = convite.Email;
                row.nome_completo = convite.NomeCompleto;
                row.token = convite.Token;
                row.validade = validade;
                _colaboradorContext.tb_convite_empresa.Add(row);
                _colaboradorContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void DeleteConvite(string cpf, string cnpj)
        {
            try
            {
                var oldInvite = _colaboradorContext.tb_convite_empresa.Where(x => x.cpf == cpf && x.tb_empresa_cnpj == cnpj).FirstOrDefault();
                if (oldInvite != null)
                {
                    _colaboradorContext.tb_convite_empresa.Remove(oldInvite);
                    _colaboradorContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public bool IsFourmakerUser(string cpf)
        {
            return _colaboradorContext.tb_colaborador.Find(cpf) != null;
        }

        public ConviteUsuarioExternoDTO Valida(string token)
        {
            var row = _colaboradorContext.tb_convite_empresa.Where(x => x.token == token).FirstOrDefault();
            if (row == null)
                return null;

            return new ConviteUsuarioExternoDTO
            {
                Cpf = row.cpf,
                Email = row.email,
                NomeCompleto = row.nome_completo,
                Token = token,
                Cnpj = row.tb_empresa_cnpj
            };
        }
    }
}