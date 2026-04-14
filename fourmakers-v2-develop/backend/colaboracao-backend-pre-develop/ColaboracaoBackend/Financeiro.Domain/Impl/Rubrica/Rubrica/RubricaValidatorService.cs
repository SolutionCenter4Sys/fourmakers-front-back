using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.Rubrica;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using Financeiro.Domain.Impl.Rubrica.Rubrica.Constants;
using Financeiro.Domain.Interfaces.Rubrica.Rubrica;
using Sprache;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.Rubrica.Rubrica
{
    [LogDomainClass]
    public class RubricaValidatorService : IRubricaValidatorService
    {
        private readonly IRubricaRepository _rubricaRepository;

        public RubricaValidatorService(IRubricaRepository rubricaRepository)
        {
            _rubricaRepository = rubricaRepository;
        }

        public async Task ValidaRubrica(RubricaInput input, CRUDEnum cRUDEnum)
        {
            // primeiro passo: verificar se existe o objeto no banco
            if (cRUDEnum == CRUDEnum.Update || cRUDEnum == CRUDEnum.Delete)
            {
                await ValidaSeExisteRubrica(input.Id);
            }

            if (cRUDEnum == CRUDEnum.Create || cRUDEnum == CRUDEnum.Update)
            {
                await ValidarCamposDeEntrada(input, cRUDEnum); // faz as validações padrões

                await ValidarRubricaTipo(input.RubricaTipo);
                await ValidarCalculoTipo(input.CalculoTipo);

                if (cRUDEnum == CRUDEnum.Create)
                {
                    await ValidaSeJaExisteRubrica(input.CodigoRubrica, input.OrgId); //tentando alterar o codigo, mas já existe o objeto no banco 
                    await ValidaSeExisteRubricaPorDescricao(input.Descricao, input.OrgId); //tentando alterar o codigo, mas já existe o objeto no banco 
                    
                }

                if (cRUDEnum == CRUDEnum.Update)
                {
                    var rubricaAtual = await _rubricaRepository.ObterRubricaPorIdAsync(input.Id);

                    if (input.CodigoRubrica != rubricaAtual.CodigoRubrica)
                    {
                        await ValidaSeJaExisteRubrica(input.CodigoRubrica, input.OrgId); //tentando alterar o codigo, mas já existe o objeto no banco 
                    }

                    if (input.Descricao != rubricaAtual.Descricao)
                    {
                        await ValidaSeExisteRubricaPorDescricao(input.Descricao, input.OrgId); //tentando alterar o Descricao, mas já existe no banco 
                    }

                }
            }

            if (cRUDEnum == CRUDEnum.Delete)
            {
                //caso tem algum relacionamento crucial que seja bloqueante e não permita a exclusão
            }
        }

        private async Task ValidaSeExisteRubrica(Guid id)
        {
            var existente = await _rubricaRepository.ObterRubricaPorIdAsync(id);
            if (existente == null)
                throw new ApplicationException($"Rubrica com id {id} não encontrada.");
        }


        private async Task ValidaSeExisteRubricaPorDescricao(string descricao, int orgId)
        {
            var rubrica = await _rubricaRepository.ObterRubricaPorDescricaoAsync(descricao, orgId);
            if (rubrica != null)
                throw new ApplicationException($"Rubrica com descricao {descricao} já existente.");
        }

        private Task ValidarRubricaTipo(string rubricaTipo)
        {
            var tiposValidos = new[] { RubricaTipoConstant.DESCONTO, RubricaTipoConstant.PROVENTO };

            if (!tiposValidos.Contains(rubricaTipo))
            {
                throw new ApplicationException($"Tipo de rubrica inválido. Valores aceitos: {string.Join(", ", tiposValidos)}.");
            }

            return Task.CompletedTask;
        }

        private Task ValidarCalculoTipo(string calculoTipo)
        {
            var tiposValidos = new[] { CalculoTipoConstant.VALOR, CalculoTipoConstant.PORCENTAGEM, CalculoTipoConstant.HORA };

            if (!tiposValidos.Contains(calculoTipo))
            {
                throw new ApplicationException($"Tipo de cálculo inválido. Valores aceitos: {string.Join(", ", tiposValidos)}.");
            }

            return Task.CompletedTask;
        }


        private async Task ValidarCamposDeEntrada(RubricaInput input, CRUDEnum cRUDEnum)
        {
            var campos = new List<CampoValidacao>();

            if (cRUDEnum == CRUDEnum.Update)
            {
                campos.Add(new("Id", input.Id, TipoValidacaoEnum.Obrigatoriedade));
            }

            campos.Add(new("Id", input.Id, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Ativo", input.Ativo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("CodigoInternoColaboradorAlteracao", input.CodigoInternoColaboradorAlteracao, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Descricao", input.Descricao, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("RubricaTipo", input.RubricaTipo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("CalculoTipo", input.CalculoTipo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("CodigoRubrica", input.CodigoRubrica, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("OrgId", input.OrgId, TipoValidacaoEnum.Obrigatoriedade));

            campos.Add(new("Descricao", input.Descricao, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 100 });
            campos.Add(new("CodigoRubrica", input.RubricaTipo, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 50 });


            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        private async Task ValidaSeJaExisteRubrica(string codigoRubrica, int orgId)
        {
            var rubrica = await _rubricaRepository.ObterRubricaPorCodigoAsync(codigoRubrica, orgId);

            if (rubrica.IsNotNull())
            {
                throw new ApplicationException($"CodigoRubrica: {codigoRubrica} já existente na tabela {RubricaService.DESCRICAO_ENTIDADE}.");
            }
        }
    }
}