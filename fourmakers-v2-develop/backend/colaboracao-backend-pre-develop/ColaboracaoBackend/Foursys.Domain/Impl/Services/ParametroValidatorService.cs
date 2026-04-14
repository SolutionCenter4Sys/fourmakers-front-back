using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.ParametroOrg;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Fourmakers.Parametro;
using Foursys.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Logs.Infra.Attributes;

namespace Foursys.Domain.Impl.Services
{
    [LogDomainClass]
    public class ParametroValidatorService : IParametroValidatorService
    {
        private readonly IParametroConfiguracaoRepository _parametroConfiguracaoRepository;
        private readonly IParametroRepository _parametroRepository;

        public ParametroValidatorService(IParametroConfiguracaoRepository parametroConfiguracaoRepository,
                                         IParametroRepository parametroRepository)
        {
            _parametroConfiguracaoRepository = parametroConfiguracaoRepository;
            _parametroRepository = parametroRepository;
        }

        public async Task ValidaParametro(ParametroRepositoryInput input, CRUDEnum crudOperation)
        {
            if (crudOperation == CRUDEnum.Create || crudOperation == CRUDEnum.Update)
            {
                await ValidaCamposObrigatorios(input, crudOperation);
                await ValidaSeEhSomenteLetraNumeroEUnderscore(input.CodigoParametro, nameof(input.CodigoParametro));
                await ValidaSeEhSomenteLetraNumeroEUnderscore(input.CodigoModuloSistema, nameof(input.CodigoModuloSistema));

                if (crudOperation == CRUDEnum.Create)
                {
                    await ValidaSeJaExisteParametro(input.CodigoParametro);
                }

                if (crudOperation == CRUDEnum.Update)
                {
                    var parametroAtual = await _parametroRepository.ObterParametroPorId(input.Id.ToString(), TipoParametroEnum.FRONTEND);

                    if (input.CodigoParametro != parametroAtual.CodigoParametro)
                    {
                        await ValidaSeJaExisteParametro(input.CodigoParametro);
                        await ValidaSeExisteParametroConfiguracaoUtilizando(parametroAtual.CodigoParametro, crudOperation);
                    }
                }
            }

            if (crudOperation == CRUDEnum.Delete || crudOperation == CRUDEnum.Update)
            {
                await this.ValidaSeExisteParametro(input.Id);

                if (crudOperation == CRUDEnum.Delete)
                {
                    await ValidaSeExisteParametroConfiguracaoUtilizando(input.CodigoParametro, crudOperation);
                }
            }
        }

        private async Task ValidaSeExisteParametro(Guid id)
        {
            var parametro = await _parametroRepository.ObterParametroPorId(id.ToString(), TipoParametroEnum.FRONTEND);

            if (parametro.IsNull())
            {
                throw new ApplicationException($"Parâmetro com id: '{id.ToString()}' não encontrado.");
            }
        }

        private async Task ValidaSeExisteParametroConfiguracaoUtilizando(string codigoParametro, CRUDEnum cRUDEnum)
        {
            string acao = "";

            if (cRUDEnum == CRUDEnum.Delete) acao = "deletado";
            if (cRUDEnum == CRUDEnum.Update) acao = "editado";

            if ((await _parametroConfiguracaoRepository.ListarParametroConfiguracaoPorCodigoParametro(codigoParametro, 0, TipoParametroEnum.FRONTEND)).Any()
                || (await _parametroConfiguracaoRepository.ListarParametroConfiguracaoPorCodigoParametro(codigoParametro, 0, TipoParametroEnum.BACKEND)).Any())
            {
                throw new ApplicationException($"CodigoParametro: {codigoParametro} não pode ser {acao} pois possui configurações em uso no momento.");
            }
        }

        private async Task ValidaSeJaExisteParametro(string codigoParametro)
        {
            var parametro = await _parametroRepository.ObterParametroPorCodigo(codigoParametro, TipoParametroEnum.FRONTEND);

            if (parametro.IsNotNull())
            {
                throw new ApplicationException($"CodigoParametro: {codigoParametro} já existente na tabela Parâmetro.");
            }

            var parametroBackend = await _parametroRepository.ObterParametroPorCodigo(codigoParametro, TipoParametroEnum.BACKEND);

            if (parametroBackend.IsNotNull())
            {
                throw new ApplicationException($"CodigoParametro: '{codigoParametro}' já existe na tabela Parâmetro (uso exclusivo do backend).");
            }
        }

        private async Task ValidaSeEhSomenteLetraNumeroEUnderscore(string codigoParametro, string nomeCampo)
        {
            var regex = new Regex("^[a-zA-Z0-9_]+$");
            if (!regex.IsMatch(codigoParametro))
            {
                throw new ApplicationException($"'{nomeCampo}' deve conter apenas letras, números e underscore (_).");
            }
        }

        private async Task ValidaCamposObrigatorios(ParametroRepositoryInput input, CRUDEnum cRUDEnum)
        {
            var errorMessages = new List<string>();

            if (cRUDEnum == CRUDEnum.Update)
            {
                if (string.IsNullOrEmpty(input.Id.ToString()))
                {
                    errorMessages.Add("'Id'");
                }
            }

            if (string.IsNullOrEmpty(input.NomeParametro))
            {
                errorMessages.Add("'NomeParametro'");
            }

            if (string.IsNullOrEmpty(input.DescricaoParametro))
            {
                errorMessages.Add("'DescricaoParametro'");
            }

            if (string.IsNullOrEmpty(input.CodigoParametro))
            {
                errorMessages.Add("'CodigoParametro'");
            }

            if (string.IsNullOrEmpty(input.CodigoModuloSistema))
            {
                errorMessages.Add("'CodigoModuloSistema'");
            }

            if (errorMessages.Any())
            {
                var plural = (errorMessages.Count > 1 ? "s" : "");
                throw new ApplicationException($"Campo{plural} {string.Join("; ", errorMessages)} obrigatório{plural}");
            }
        }
    }
}