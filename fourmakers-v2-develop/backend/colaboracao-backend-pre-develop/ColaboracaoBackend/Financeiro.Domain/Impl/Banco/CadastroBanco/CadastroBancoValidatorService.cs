using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.Banco;
using DataTransferObject.Domain.Financeiro.Banco.CadastroBanco;
using Financeiro.Domain.Interfaces.Banco.CadastroBanco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Financeiro.Domain.Impl.Banco.CadastroBanco
{
    public class CadastroBancoValidatorService : ICadastroBancoValidatorService
    {

        private readonly ICadastroBancoRepository _cadastroBancoRepository;

        public CadastroBancoValidatorService(ICadastroBancoRepository cadastroBancoRepository)
        {
            _cadastroBancoRepository = cadastroBancoRepository;
        }

        public async Task ValidaCadastroBanco(CadastroBancoInput input, CRUDEnum cRUDEnum)
        {

            if (cRUDEnum == CRUDEnum.Update || cRUDEnum == CRUDEnum.Delete)
            {
                await ValidaSeExisteCadastroBanco(input.CodigoBanco);
            }

            if (cRUDEnum == CRUDEnum.Create || cRUDEnum == CRUDEnum.Update)
            {
                await ValidarCamposDeEntrada(input, cRUDEnum);
            }
        
        }

        private async Task ValidarCamposDeEntrada(CadastroBancoInput input, CRUDEnum cRUDEnum)
        {
            var campos = new List<CampoValidacao>();


            //Obrigatoriedade (campos obrigatórios)
            campos.Add(new("Descricao", input.Descricao, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("CodigoBanco", input.CodigoBanco, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("CodigoBanco", input.CodigoBanco, TipoValidacaoEnum.TamanhoExato) { TamanhoExato = 3});
            campos.Add(new("Nome", input.Nome, TipoValidacaoEnum.Obrigatoriedade));

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        private async Task ValidaSeExisteCadastroBanco(string codigo)
        {
            var cadastroBanco = await _cadastroBancoRepository.ObterCadastroBancoPorCodigoAsync(codigo);

            if (cadastroBanco.IsNull())
            {
                throw new ApplicationException($"Parâmetro com id: '{codigo.ToString()}' não encontrado.");
            }
        }
    }
}