using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaborador.Domain.Interfaces.Colaborador.DepartamentoOrg;
using Core.Domain.Colaborador.Colaborador;
using DataTransferObject.Domain.Colaborador.DepartamentoOrg;
using Logs.Infra.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Impl.Services.Colaborador.DepartamentoOrg
{
    [LogDomainClass]
    public class DepartamentoOrgValidatorService : IDepartamentoOrgValidatorService
    {
        private readonly IDepartamentoOrgRepository _departamentoOrgRepository;

        public DepartamentoOrgValidatorService(IDepartamentoOrgRepository departamentoOrgRepository)
        {
            _departamentoOrgRepository = departamentoOrgRepository;
        }

        public async Task ValidaDepartamentoOrg(DepartamentoOrgInput input, CRUDEnum cRUDEnum)
        {
            // primeiro passo: verificar se existe o objeto no banco
            if (cRUDEnum == CRUDEnum.Update || cRUDEnum == CRUDEnum.Delete)
            {
                await ValidaSeExisteDepartamentoOrg(input.Id);
            }

            if (cRUDEnum == CRUDEnum.Create || cRUDEnum == CRUDEnum.Update)
            {
                await ValidarCamposDeEntrada(input, cRUDEnum); // faz as validações padrões
            }
        }

        private async Task ValidarCamposDeEntrada(DepartamentoOrgInput input, CRUDEnum cRUDEnum)
        {
            var campos = new List<CampoValidacao>();

            if (cRUDEnum == CRUDEnum.Update)
            {
                campos.Add(new("Id", input.Id, TipoValidacaoEnum.Obrigatoriedade));
            }
            ;

            //<<adicionar aqui validacoes pertinentes>>

            // Obrigatoriedade (campos obrigatórios)
            campos.Add(new("OrgId", input.OrgId, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Ativo", input.Ativo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Departamento", input.Departamento, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("CodDepartamento", input.CodDepartamento, TipoValidacaoEnum.Obrigatoriedade));

            campos.Add(new("Departamento", input.Departamento, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 255 });
            campos.Add(new("Código Departamento", input.CodDepartamento, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 255 });

            // Tamanho exato para o campo CodigoInternoColaboradorCriacao
            campos.Add(new("CodigoInternoColaboradorCriacao", input.CodigoInternoColaboradorAlteracao, TipoValidacaoEnum.TamanhoExato) { TamanhoExato = 36 });

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        private async Task ValidaSeExisteDepartamentoOrg(Guid id)
        {
            var departamentoOrg = await _departamentoOrgRepository.ObterDepartamentoOrgPorIdAsync(id);

            if (departamentoOrg.IsNull())
            {
                throw new ApplicationException($"Parâmetro com id: '{id.ToString()}' não encontrado.");
            }
        }
    }
}