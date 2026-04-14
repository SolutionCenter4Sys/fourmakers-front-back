using Colaboracao.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;

namespace Colaboracao.Core.Impl
{
    public class VerificaSeCpfESistemico : IVerificaSeCpfESistemico
    {
        private readonly IAspNetUser _aspNetUser;
        private readonly IConfiguration _configuration;

        public VerificaSeCpfESistemico(IAspNetUser aspNetUser, IConfiguration configuration)
        {
            _aspNetUser = aspNetUser;
            _configuration = configuration;
        }

        public string VerificaCpfSistemico(string cpfBody)
        {
            var cpfRequest = String.Empty;

            var codColaborador = _aspNetUser.GetUsuarioLogado().Cpf;

            if (codColaborador == _configuration["Clients:Colaborador:CpfAdmin"])
            {
                cpfRequest = cpfBody;
            }
            else
            {
                cpfRequest = codColaborador;
            }

            return cpfRequest;
        }
    }
}