using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.SRS;
using Core.Domain.Usuario.Permissao;
using Core.Domain.Vaga;
using Core.DomainModel;
using DataTransferObject.Domain.Calculos;
using DataTransferObject.Domain.Usuario;
using SRS.Domain.Interfaces.Service.Validadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logs.Infra.Attributes;

namespace SRS.Domain.Impl.Service.Validadores
{
    [LogDomainClass]
    public class CalculosValidatorService : ICalculosValidatorService
    {
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IAspNetUser _aspNetUser;
        private readonly IAdmissaoCargoRepository _admissaoCargoRepository;
        private readonly IVagaFourmakersRepository _vagaFourmakersRepository;

        public CalculosValidatorService(
            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
            IAspNetUser aspNetUser,
            IAdmissaoCargoRepository admissaoCargoRepository,
            IVagaFourmakersRepository vagaFourmakersRepository)
        {
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _aspNetUser = aspNetUser;
            _admissaoCargoRepository = admissaoCargoRepository;
            _vagaFourmakersRepository = vagaFourmakersRepository;
        }

        public async Task ValidarCalcularSalarioLiquido(CalcularSalarioLiquidoInputDTO input)
        {
            ValidaAcessoSimuladorRemuneracao();

            var campos = new List<CampoValidacao>();

            // Campos obrigatórios
            campos.Add(new("Salário Bruto", input.SalarioBruto, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Número de Dependentes", input.NumeroDependentes, TipoValidacaoEnum.Obrigatoriedade));

            // Validações específicas de valor
            if (input.SalarioBruto <= 0)
            {
                throw new ApplicationException("Salário bruto deve ser maior que zero.");
            }

            if (input.NumeroDependentes < 0)
            {
                throw new ApplicationException("Número de dependentes não pode ser negativo.");
            }

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        public async Task ValidarSimularRemuneracaoTotal(SimularRemuneracaoTotalInputDTO input)
        {
            ValidaAcessoSimuladorRemuneracao();

            //if (!input.AdmissaoCargoId.HasValue)
            //    throw new ApplicationException("Cargo (AdmissaoCargoId) eh obrigatorio.");

            //if (!input.NivelVagaCod.HasValue)
            //    throw new ApplicationException("Nível da vaga (nivelVagaCod)  eh obrigatorio.");

            //if (input.NivelVagaCod.HasValue && (input.NivelVagaCod.Value < 1 || input.NivelVagaCod.Value > 4))
            //    throw new ApplicationException("Nível da vaga (nivelVagaCod) deve estar entre 1 e 4.");

            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            if (usuarioLogado == null)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            //if (input.AdmissaoCargoId.HasValue && input.AdmissaoCargoId.Value != Guid.Empty)
            //{
            //    var cargoExiste = await _admissaoCargoRepository.ExisteAtivoPorIdEOrgAsync(input.AdmissaoCargoId.Value, usuarioLogado.OrgId);
            //    if (!cargoExiste)
            //        throw new ApplicationException("Cargo de admissão (admissaoCargoId) não encontrado ou inativo para a organização.");
            //}

            //if (input.NivelVagaCod.HasValue)
            //{
            //    var niveis = await _vagaFourmakersRepository.ListarNiveisVaga();
            //    if (niveis == null || !niveis.Any(n => n.Codigo == input.NivelVagaCod.Value))
            //        throw new ApplicationException("Nível da vaga (nivelVagaCod) não existe na base de dados.");
            //}

            await Task.CompletedTask;
        }

        private void ValidaAcessoSimuladorRemuneracao()
        {
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            if (usuarioLogado == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                usuarioLogado.Cpf,
                usuarioLogado.OrgId,
                FuncionalidadeSistemaEnum.SIMULADOR_REMUNERACAO
            ).Result;

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Acesso negado. Você não possui permissão para acessar o Simulador de Remuneração.");
            }
        }
    }
}
