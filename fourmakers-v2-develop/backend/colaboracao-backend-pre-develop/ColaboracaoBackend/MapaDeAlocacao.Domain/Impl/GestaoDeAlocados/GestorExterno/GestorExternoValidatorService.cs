using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Projeto.GestorExterno;
using Projeto.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logs.Infra.Attributes;

namespace Projeto.Domain.Impl.Services
{
    [LogDomainClass]
    public class GestorExternoValidatorService : IGestorExternoValidatorService
    {
        private readonly IGestorExternoRepository _gestorExternoRepository;
        private readonly IGestaoAlocadosRepository _gestaoAlocadosRepository;

        public GestorExternoValidatorService(IGestorExternoRepository gestorExternoRepository,
                                             IGestaoAlocadosRepository gestaoAlocadosRepository)
        {
            _gestorExternoRepository = gestorExternoRepository;
            _gestaoAlocadosRepository = gestaoAlocadosRepository;
        }

        public async Task ValidaGestorExterno(GestorExternoInput input, CRUDEnum crudOperation, string codGestorExternoChave = null)
        {
            if (crudOperation == CRUDEnum.Update || crudOperation == CRUDEnum.Delete)
            {
                await ValidaSeExisteGestorExterno(codGestorExternoChave, input.OrgId);
                await ValidaSeJaExisteGestorExternoComCodigoInternoColaboradorInformado(input.CodGestorExterno, input.CodigoInternoColaborador, input.OrgId);
            }

            if (crudOperation == CRUDEnum.Create || crudOperation == CRUDEnum.Update)
            {
                await ValidarCamposDeEntrada(input, crudOperation);

                await ValidaSeJaExisteGestorExternoComCodigoInformado(input.CodGestorExterno, codGestorExternoChave, input.OrgId);
                await ValidaSeJaExisteGestorExternoComEmailInformado(input.Email, input.OrgId, codGestorExternoChave);

                if (crudOperation == CRUDEnum.Create)
                {
                    await ValidarGestorExternoComNomeIgual(input.CodGestorExterno, input.Nome, input.CodigoCliente, input.OrgId);
                }

                this.ValidaSeExistePermanenciaInfornada(input);
            }
        }

        private async Task ValidarCamposDeEntrada(GestorExternoInput input, CRUDEnum cRUDEnum)
        {
            var campos = new List<CampoValidacao>();

            campos.Add(new("Nome", input.Nome, TipoValidacaoEnum.Obrigatoriedade));

            campos.Add(new("Código do Gestor", input.CodGestorExterno, TipoValidacaoEnum.ValidarFormatoCodigo));
            campos.Add(new("Email", input.Email, TipoValidacaoEnum.ValidarEmail));
            campos.Add(new("Telefone", input.Telefone, TipoValidacaoEnum.ValidarTelefoneFixoOuCelular));

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        private async Task ValidaSeJaExisteGestorExternoComEmailInformado(string email, int orgId, string codGestorExternoEmEdicao = null)
        {
            if (email.IsNotEmpty())
            {
                var gestorComMesmoEmail = await _gestorExternoRepository.ObterGestorExternoPorEmailAsync(email, orgId);

                if (gestorComMesmoEmail.IsNotNull())
                {
                    if (codGestorExternoEmEdicao.IsNotEmpty()
                        && string.Equals(gestorComMesmoEmail.CodGestorExterno, codGestorExternoEmEdicao, StringComparison.Ordinal))
                    {
                        return;
                    }

                    throw new ApplicationException($"Gestor com email: '{email}' já existente.");
                }
            }
        }

        private async Task ValidaSeJaExisteGestorExternoComCodigoInformado(string codGestorExternoNovo, string codGestorExternoAntigo, int orgId)
        {
            if (codGestorExternoNovo != codGestorExternoAntigo)
            {
                var gestorExternoNovo = await _gestorExternoRepository.ObterGestorExternoPorCodigoAsync(codGestorExternoNovo.ToString(), orgId);

                if (gestorExternoNovo.IsNotNull())
                {
                    throw new ApplicationException($"Gestor com codigo: '{codGestorExternoNovo.ToString()}' já existente.");
                }
            }
        }

        private async Task ValidaSeJaExisteGestorExternoComCodigoInternoColaboradorInformado(string codGestorExterno, string codigoInternoColaborador, int orgId)
        {
            var gestorExterno = await _gestorExternoRepository.ObterGestorExternoPorCodigoAsync(codGestorExterno.ToString(), orgId);

            if (gestorExterno.IsNotNull() && gestorExterno.CodigoInternoColaborador.IsNotEmpty())
            {
                if (gestorExterno.CodigoInternoColaborador != codigoInternoColaborador)
                {
                    throw new ApplicationException($"A alteração do código interno do colaborador não é permitida: o valor '{codigoInternoColaborador}' informado difere do registrado na base de dados.");
                }
            }
        }

        private void ValidaSeExistePermanenciaInfornada(GestorExternoInput input)
        {
            var permanencias = _gestaoAlocadosRepository.ListarPermanenciasAsync().Result;

            foreach (var areaAtuacao in input.AreasDeAtuacao)
            {
                if (areaAtuacao.Permanencia?.Id is not null)
                {
                    if (!permanencias.Any(x => x.Id == areaAtuacao.Permanencia.Id))
                    {
                        throw new ApplicationException($"A permanência informada não existe na tabela de referência. ID não encontrado: {areaAtuacao.Permanencia.Id}.");
                    }
                }
            }
        }

        private async Task ValidaSeExisteGestorExterno(string codGestorExterno, int orgId)
        {
            var gestorExterno = await _gestorExternoRepository.ObterGestorExternoPorCodigoAsync(codGestorExterno.ToString(), orgId);

            if (gestorExterno.IsNull())
            {
                throw new ApplicationException($"Gestor com codigo: '{codGestorExterno.ToString()}' não encontrado.");
            }
        }

        private async Task ValidarGestorExternoComNomeIgual(string codigoGestorExterno, string nome, string codigoCliente, int orgId)
        {
            var gestorExterno = await _gestorExternoRepository.ObterGestorExternoPorNomeAsync(nome, orgId);

            if (gestorExterno.IsNotNull())
            {
                if (gestorExterno.CodigoCliente == codigoCliente)
                {
                    throw new ApplicationException($"Nome já está em uso para outro gestor no cliente informado. Gestor existente: {gestorExterno.CodGestorExterno} - {gestorExterno.Nome}");
                }
            }
        }

        private async Task ValidaCamposObrigatorios(GestorExternoInput input, CRUDEnum cRUDEnum)
        {
            var errorMessages = new List<string>();

            if (cRUDEnum == CRUDEnum.Update)
            {
                if (string.IsNullOrEmpty(input.CodGestorExterno.ToString()))
                {
                    errorMessages.Add("'CodGestorExterno'");
                }
            }

            if (string.IsNullOrEmpty(input.Nome))
            {
                errorMessages.Add("'NomeGestor'");
            }

            if (string.IsNullOrEmpty(input.CodigoCliente))
            {
                errorMessages.Add("'CodigoCliente'");
            }

            if (errorMessages.Any())
            {
                var plural = (errorMessages.Count > 1 ? "s" : "");
                throw new ApplicationException($"Campo{plural} {string.Join("; ", errorMessages)} obrigatório{plural}");
            }
        }
    }
}