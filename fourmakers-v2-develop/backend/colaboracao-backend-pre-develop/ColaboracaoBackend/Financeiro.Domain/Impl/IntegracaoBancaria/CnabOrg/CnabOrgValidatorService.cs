using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.IntegracaoBancaria;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.CnabOrg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Financeiro.Domain.Impl.IntegracaoBancaria.CnabOrg
{
    public class CnabOrgValidatorService : ICnabOrgValidatorService
    {

        private readonly ICnabOrgRepository _cnabOrgRepository;

        public CnabOrgValidatorService(ICnabOrgRepository cnabOrgRepository)
        {
            _cnabOrgRepository = cnabOrgRepository;
        }

        public async Task ValidaCnabOrg(CnabOrgInput input, CRUDEnum cRUDEnum)
        {

            // primeiro passo: verificar se existe o objeto no banco
            if (cRUDEnum == CRUDEnum.Update || cRUDEnum == CRUDEnum.Delete)
            {
                await ValidaSeExisteCnabOrg(input.Id);
            }

            if (cRUDEnum == CRUDEnum.Create || cRUDEnum == CRUDEnum.Update)
            {
                await ValidarCamposDeEntrada(input, cRUDEnum); // faz as validações padrões

                // Valida se já existe uma configuração com a mesma combinação de org_id, diretoria e forma_pagamento
                if (cRUDEnum == CRUDEnum.Create)
                {
                    await ValidaSeJaExisteCnabOrgComMesmaChaveUnica(input.TbOrgId, input.CodDiretoria, input.FormaPagamento);
                }

                if (cRUDEnum == CRUDEnum.Update)
                {
                    var cnabOrgAtual = await _cnabOrgRepository.ObterCnabOrgPorIdAsync(input.Id);

                    // Se alterou algum campo da chave única, valida se a nova combinação já existe
                    if (cnabOrgAtual.TbOrgId != input.TbOrgId ||
                        cnabOrgAtual.CodDiretoria != input.CodDiretoria ||
                        cnabOrgAtual.FormaPagamento != input.FormaPagamento)
                    {
                        await ValidaSeJaExisteCnabOrgComMesmaChaveUnica(input.TbOrgId, input.CodDiretoria, input.FormaPagamento);
                    }
                }
            }

            if (cRUDEnum == CRUDEnum.Delete)
            {
                // await ValidaSeExisteCnabOrgConfiguracaoUtilizando(input.CodigoCnabOrg, cRUDEnum); //caso tem algum relacionamento que seja bloqueante e não permita a exclusão
            }
        
        }

        private async Task ValidarCamposDeEntrada(CnabOrgInput input, CRUDEnum cRUDEnum)
        {
            var campos = new List<CampoValidacao>();

            if (cRUDEnum == CRUDEnum.Update)
            {
                campos.Add(new("Id", input.Id, TipoValidacaoEnum.Obrigatoriedade));
            };

            //<<adicionar aqui validacoes pertinentes>>

            //Obrigatoriedade (campos obrigatórios)
            campos.Add(new("Id", input.Id, TipoValidacaoEnum.Obrigatoriedade));

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        private async Task ValidaSeExisteCnabOrg(Guid id)
        {
            var cnabOrg = await _cnabOrgRepository.ObterCnabOrgPorIdAsync(id);

            if (cnabOrg == null)
            {
                throw new ApplicationException($"Configuração CNAB com id: '{id.ToString()}' não encontrada.");
            }
        }

        private async Task ValidaSeExisteCnabOrgEmOutraTabelaUtilizando(string codigoCnabOrg, CRUDEnum cRUDEnum)
        {
            string acao = "";

            if (cRUDEnum == CRUDEnum.Delete) acao = "deletado";
            if (cRUDEnum == CRUDEnum.Update) acao = "editado";

            //if ((await _cnabOrgOutraTabelaRepository.ListarCnabOrgOutraTabelaPorCodigoCnabOrg(codigoCnabOrg).Any()).Any())
            //{
            //    throw new ApplicationException($"CodigoCnabOrg: {codigoCnabOrg} não pode ser {acao} pois está em uso em outra tabela no momento.");
            //}
        }

        private async Task ValidaSeJaExisteCnabOrg(string codigoCnabOrg, int orgId)
        {
            //var cnabOrg = await _cnabOrgRepository.ObterCnabOrgPorCodigoAsync(codigoCnabOrg, orgId);

            //if (cnabOrg.IsNotNull())
            //{
            //    throw new ApplicationException($"CodigoCnabOrg: {codigoCnabOrg} já existente na tabela {CnabOrgService.DESCRICAO_ENTIDADE}.");
            //}
        }

        private async Task ValidaSeJaExisteCnabOrgComMesmaChaveUnica(int orgId, string diretoria, string formaPagamento)
        {
            var cnabOrg = await _cnabOrgRepository.ObterCnabOrgPorChaveUnicaAsync(orgId, diretoria, formaPagamento);

            if (cnabOrg != null)
            {
                throw new ApplicationException($"Já existe uma configuração CNAB para a Organização '{orgId}', Diretoria '{diretoria}' e Forma de Pagamento '{formaPagamento}'.");
            }
        }
    }
}