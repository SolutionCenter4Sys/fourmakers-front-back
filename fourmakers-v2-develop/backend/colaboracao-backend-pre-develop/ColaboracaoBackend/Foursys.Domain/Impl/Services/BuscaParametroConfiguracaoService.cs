using Core.Domain.ParametroOrg;
using DataTransferObject.Domain.Fourmakers;
using Foursys.Domain.Interfaces.Services;
using System;

using Logs.Infra.Attributes;

namespace Foursys.Domain.Impl.Services
{
    [LogDomainClass]
    public class BuscaParametroConfiguracaoService : IBuscaParametroConfiguracaoService
    {
        private readonly IParametroConfiguracaoRepository _parametroConfiguracaoRepository;

        public BuscaParametroConfiguracaoService(IParametroConfiguracaoRepository parametroConfiguracaoRepository)
        {
            _parametroConfiguracaoRepository = parametroConfiguracaoRepository;
        }

        private string GetParametroConfiguracao(ParametroOrgCodigoEnum parametro, int orgId, string codigoInternoColaborador)
        {
            try
            {
                return _parametroConfiguracaoRepository.GetParametroConfiguracao(parametro.ToString(), codigoInternoColaborador, orgId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetParametroConfiguracao(ParametroOrgCodigoFrontEndEnum parametro, int orgId, string codigoInternoColaborador)
        {
            try
            {
                return _parametroConfiguracaoRepository.GetParametroConfiguracao(parametro.ToString(), codigoInternoColaborador, orgId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T GetParametroConfiguracao<T>(ParametroOrgCodigoEnum parametro, int orgId, string codigoInternoColaborador)
        {
            string valorParametro = GetParametroConfiguracao(parametro, orgId, codigoInternoColaborador);
            return ConvertParametro<T>(valorParametro);
        }

        public T GetParametroConfiguracao<T>(ParametroOrgCodigoFrontEndEnum parametro, int orgId, string codigoInternoColaborador)
        {
            string valorParametro = GetParametroConfiguracao(parametro, orgId, codigoInternoColaborador);
            return ConvertParametro<T>(valorParametro);
        }

        private T ConvertParametro<T>(string valorParametro)
        {
            if (string.IsNullOrEmpty(valorParametro))
            {
                if (typeof(T) == typeof(int) || typeof(T) == typeof(double))
                    return (T)Convert.ChangeType(0, typeof(T));
                else if (typeof(T) == typeof(bool))
                    return (T)Convert.ChangeType(false, typeof(T));
                else
                    return (T)Convert.ChangeType("", typeof(T));
            }

            return (T)Convert.ChangeType(valorParametro, typeof(T));
        }
    }
}
