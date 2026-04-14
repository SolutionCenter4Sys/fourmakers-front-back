using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Colaborador;
using Core.Domain.ParametroOrg;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Org;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Fourmakers.ParametroConfiguracao;
using Foursys.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logs.Infra.Attributes;

namespace Foursys.Domain.Impl.Services
{
    [LogDomainClass]
    public class ParametroConfiguracaoValidatorService : IParametroConfiguracaoValidatorService
    {
        private readonly IParametroConfiguracaoRepository _parametroConfiguracaoRepository;
        private readonly IParametroRepository _parametroRepository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly IGrupoAcessoRepository _grupoAcessoRepository;
        private readonly IOrgRepository _orgRepository;

        public ParametroConfiguracaoValidatorService(IParametroConfiguracaoRepository parametroConfiguracaoRepository,
                                                     IParametroRepository parametroRepository,
                                                     IBuscaColaboradorRepository buscaColaboradorRepository,
                                                     IGrupoAcessoRepository grupoAcessoRepository,
                                                     IOrgRepository orgRepository)
        {
            _parametroConfiguracaoRepository = parametroConfiguracaoRepository;
            _parametroRepository = parametroRepository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _grupoAcessoRepository = grupoAcessoRepository;
            _orgRepository = orgRepository;
        }

        public async Task ValidaParametroConfiguracao(ParametroConfiguracaoRepositoryInput input, CRUDEnum crudOperation)
        {
            await ValidaCamposObrigatorios(input);
            await ValidaSeParametroValido(input.CodigoParametro);
            await ValidaSeExisteOrgId(input);

            if (crudOperation == CRUDEnum.Create)
            {
                await ValidaSeParametroConfiguracaoValido(input);
            }

            if (crudOperation == CRUDEnum.Update)
            {
                var parametroConfiguracaoAtual = await _parametroConfiguracaoRepository.ObterParametroConfiguracaoPorId(input.Id.ToString(), TipoParametroEnum.FRONTEND, input.OrgId);

                await ValidaSeExisteParametroConfiguracao(parametroConfiguracaoAtual);

                if (input.CodigoParametro != parametroConfiguracaoAtual.CodigoParametro
                    || input.GrupoAcessoId != parametroConfiguracaoAtual.GrupoAcessoId
                    || input.OrgId != parametroConfiguracaoAtual.OrgId
                    || input.ColaboradorOrgCpf != parametroConfiguracaoAtual.ColaboradorOrgCpf)
                {
                    await ValidaSeParametroConfiguracaoValido(input);
                }
            }
        }

        private async Task ValidaSeExisteOrgId(ParametroConfiguracaoRepositoryInput input)
        {
            var orgResult = _orgRepository.BuscarOrg(input.OrgId);

            if (orgResult.IsNull() || orgResult.Id != input.OrgId)
            {
                throw new ApplicationException($"OrgId: '{input.OrgId}' não encontrada.");
            }
        }

        private async Task ValidaSeExisteParametroConfiguracao(ParametroConfiguracaoResult parametroConfiguracaoAtual)
        {
            if (parametroConfiguracaoAtual.IsNull())
            {
                throw new ApplicationException("Parâmetro de configuração não encontrado.");
            }
        }

        private async Task ValidaSeParametroValido(string codigoParametro)
        {
            var parametro = await _parametroRepository.ObterParametroPorCodigo(codigoParametro, TipoParametroEnum.FRONTEND);

            if (parametro.IsNull())
            {
                throw new Exception($"Código: '{codigoParametro}' não encontrado na tabela de Parâemtros.");
            }
        }

        private async Task ValidaSeParametroConfiguracaoValido(ParametroConfiguracaoRepositoryInput input)
        {
            var listaParametroConfiguracao = await _parametroConfiguracaoRepository.ListarParametroConfiguracaoPorCodigoParametro(input.CodigoParametro, input.OrgId, TipoParametroEnum.FRONTEND);

            if (input.ParametroNivelId == (int)ParametroNivelEnum.org)
            {
                if (listaParametroConfiguracao.Any(x => x.ParametroNivelId == (int)ParametroNivelEnum.org))
                {
                    throw new ApplicationException($"Configuração: '{input.CodigoParametro}' no Nível: '{(int)ParametroNivelEnum.org} - {ParametroNivelEnum.org}' já existente para Org: '{input.OrgId}', favor atualizar a configuração atual.");
                }
            }

            if (input.ParametroNivelId == (int)ParametroNivelEnum.grupo_acesso)
            {
                if (listaParametroConfiguracao.Any(x => x.ParametroNivelId == (int)ParametroNivelEnum.grupo_acesso && x.GrupoAcessoId == input.GrupoAcessoId))
                {
                    throw new ApplicationException($"Configuração: '{input.CodigoParametro}' no Nível: '{(int)ParametroNivelEnum.grupo_acesso} - {ParametroNivelEnum.grupo_acesso}' já existente para Grupo de Acesso: '{input.GrupoAcessoId}', favor atualizar a configuração atual.");
                }

                var listaGrupoAcesso = await _grupoAcessoRepository.ListarGruposAcesso(input.OrgId);

                if (listaGrupoAcesso.IsNull() || !listaGrupoAcesso.Any(x => x.Id == input.GrupoAcessoId))
                {
                    throw new ApplicationException($"Grupo de Acesso: '{input.GrupoAcessoId}' não encontrado para Org: '{input.OrgId}'.");
                }
            }

            if (input.ParametroNivelId == (int)ParametroNivelEnum.colaborador_org)
            {
                if (listaParametroConfiguracao.Any(x => x.ParametroNivelId == (int)ParametroNivelEnum.colaborador_org && x.ColaboradorOrgCpf == input.ColaboradorOrgCpf))
                {
                    throw new ApplicationException($"Configuração: '{input.CodigoParametro}' no Nível: '{(int)ParametroNivelEnum.colaborador_org} - {ParametroNivelEnum.colaborador_org}' já existente para Colaborador: '{input.ColaboradorOrgCpf}', favor atualizar a configuração atual.");
                }

                var colaboradorOrg = _buscaColaboradorRepository.GetColaboradorOrg(input.ColaboradorOrgCpf, input.OrgId);

                if (colaboradorOrg.IsNull())
                {
                    throw new ApplicationException($"ColaboradorOrgCpf: '{input.ColaboradorOrgCpf}' não pertencente a Org: {input.OrgId}.");
                }
            }
        }

        private async Task ValidaCamposObrigatorios(ParametroConfiguracaoRepositoryInput input)
        {
            var errorMessages = new List<string>();

            if (input.ParametroNivelId == 0)
            {
                errorMessages.Add("Campo ParametroNivelId obrigatório");
            }

            if (string.IsNullOrEmpty(input.ValorParametro))
            {
                errorMessages.Add("Campo ValorParametro obrigatório");
            }

            if (string.IsNullOrEmpty(input.CodigoParametro))
            {
                errorMessages.Add("Campo CodigoParametro obrigatório");
            }

            if (input.OrgId == 0)
            {
                errorMessages.Add("Campo OrgId obrigatório");
            }

            if (!Enum.IsDefined(typeof(ParametroNivelEnum), input.ParametroNivelId))
            {
                errorMessages.Add("ParametroNivelId inválido");
            }

            if (input.ParametroNivelId == (int)ParametroNivelEnum.grupo_acesso && input.GrupoAcessoId == 0)
            {
                errorMessages.Add("GrupoAcessoId é obrigatório quando ParametroNivelId = 2 (grupo_acesso)");
            }

            if (input.ParametroNivelId == (int)ParametroNivelEnum.colaborador_org && string.IsNullOrEmpty(input.ColaboradorOrgCpf))
            {
                errorMessages.Add("Campo ColaboradorOrgCpf é obrigatório quando ParametroNivelId = 3 (colaborador_org)");
            }

            if (errorMessages.Any())
            {
                throw new ApplicationException(string.Join("; ", errorMessages));
            }
        }
    }
}