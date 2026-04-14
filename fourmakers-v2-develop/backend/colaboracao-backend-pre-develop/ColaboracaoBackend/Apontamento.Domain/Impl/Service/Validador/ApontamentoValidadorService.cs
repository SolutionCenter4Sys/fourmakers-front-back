using Apontamento.Domain.Enums;
using Apontamento.Domain.Interfaces;
using Apontamento.Domain.Interfaces.Service;
using Colaboracao.Core;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Apontamento;
using Core.Domain.Projeto;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Apontamento;
using DataTransferObject.Domain.Apontamento.ApontamentoValidador.ValidaApontamentoInput;
using DataTransferObject.Domain.Apontamento.ApontamentoValidador.ValidaApontamentoResult;
using DataTransferObject.Domain.Usuario;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Util.Enum;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Microsoft.IdentityModel.Tokens;

namespace Apontamento.Domain.Impl.Service.Validador
{
    [LogDomainClass]
    public class ApontamentoValidadorService : IApontamentoValidadorService
    {
        private IStringLocalizer<ApontamentoMessage> _stringLocalizer;
        private IPeriodoFechadoService _periodoFechadoService;
        private IApontamentoRepository _apontamentoRepository;
        private IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private IAtividadeProjetoRepository _atividadeProjetoRepository;

        public ApontamentoValidadorService(IStringLocalizer<ApontamentoMessage> stringLocalizer,
                                           IPeriodoFechadoService periodoFechadoService,
                                           IApontamentoRepository apontamentoRepository,
                                           IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                           IAtividadeProjetoRepository atividadeProjetoRepository)
        {
            _stringLocalizer = stringLocalizer;
            _periodoFechadoService = periodoFechadoService;
            _apontamentoRepository = apontamentoRepository;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _atividadeProjetoRepository = atividadeProjetoRepository;
        }

        public async Task<ValidaApontamentoResult> ValidaApontamento(ValidaApontamentoInput input, CRUDEnum crudEnum, int orgId)
        {
            ColaboradorApontamentoDTO apontamentoAtual = null;

            if (crudEnum == CRUDEnum.Update || crudEnum == CRUDEnum.Delete)
            {
                VerificaSeIdDoApontamentoFoiPreenchido(input.ColaboradorApontamentoId);

                apontamentoAtual = await this.ValidaSeExisteApontamento(input.ColaboradorApontamentoId, crudEnum);
                
                // preenche CPF Colaborador que est� sendo editado/deletado
                // fica vazio pra "indicar" que n�o estou tentando apagar pra outro colaborador
                input.CpfColaborador = apontamentoAtual.ColaboradorCpf == input.CpfRequest ? string.Empty : apontamentoAtual.ColaboradorCpf;
            }

            var validaResult = ValidarApontamentoParaOutraPessoa(input.CpfRequest, input.CpfColaborador, orgId);
            

            if (crudEnum == CRUDEnum.Create || crudEnum == CRUDEnum.Update || crudEnum == CRUDEnum.Delete)
            {
                var dataRegistro = crudEnum == CRUDEnum.Delete
                    ? apontamentoAtual?.Data.ToString()
                    : input.DataRegistro;
                var horas = crudEnum == CRUDEnum.Delete
                    ? apontamentoAtual?.Horas?? 0
                    : input.Horas;
                
                this.ValidaDataEHoras(horas, dataRegistro, input.HoraZerada);
                await _periodoFechadoService.ValidaSeEstaNoPeriodoFechado(dataRegistro, orgId);
            }

            if (crudEnum == CRUDEnum.Create)
            {
                await ValidaSeDataRegistroEhMenorQueDeCadastro(input.DataRegistro, validaResult.CpfUtilizado, orgId);
                await ValidarSeAtividadeEstaAssociadaAoProjeto(input.AtividadeId, input.ProjetoId, orgId);
            }

            if (crudEnum == CRUDEnum.Update)
            {
                this.ValidaSeEPossivelFazerEdicao(input.DataColetaDeDados, apontamentoAtual);
                await ValidaSeDataRegistroEhMenorQueDeCadastro(input.DataRegistro, validaResult.CpfUtilizado, orgId);
                ValidaSeTemAtividade(input.AtividadeId);
                await ValidarSeAtividadeEstaAssociadaAoProjeto(input.AtividadeId, input.ProjetoId, orgId);
            }

            if (crudEnum == CRUDEnum.Delete)
            {
                ValidacaoParaDeletarApontamentoProprio(input, validaResult.CpfUtilizado, apontamentoAtual.StatusGrupoCodigo, apontamentoAtual.OrgId,orgId, validaResult);
            }
            
            validaResult.Apontamento = apontamentoAtual;
            return validaResult;
        }

        private void ValidaSeTemAtividade(string atividade)
        {
            if (atividade.IsNullOrEmpty())
            {
                throw new ArgumentException("Não é possivel editar um apontamento sem fornecer uma atividade");
            }
        }
        private static void VerificaSeIdDoApontamentoFoiPreenchido(string colaboradorApontamentoId)
        {
            if (string.IsNullOrEmpty(colaboradorApontamentoId))
            {
                throw new ArgumentException("O ID do colaborador n�o foi fornecido.");
            }
        }

        private async Task ValidaSeDataRegistroEhMenorQueDeCadastro(string dataRegistro, string codigoInternoColaborador, int orgId)
        {
            var registro = await _apontamentoRepository.VerificaSeDataDeRegistroEhMaiorQueDataDoCadastro(dataRegistro, codigoInternoColaborador, orgId);
            if (!registro)
            {
                throw new ArgumentException("Não é possivel lançar apontamentos para uma data anterior ao inicio do cadastro.");
            }
        }

        private void ValidacaoParaDeletarApontamentoProprio(ValidaApontamentoInput input, string colaboradorCPF, int statusGrupoCodigo, int orgIdApontamento,int orgId, ValidaApontamentoResult validaResult)
        {
            //aqui valida apenas lan�amento pessoal
            if (!validaResult.EhLancamentoParaOutroColaborador)
            {
                ValidaSeEstouDeletandoParaOutroColaborador(input, colaboradorCPF, orgIdApontamento, orgId);

                //estou deletando meu pr�prio lan�amento Aprovado, mas tenho acesso lan�amento para outro colaborador
                if (statusGrupoCodigo == (int)EnumStatusApontamentoGrupo.Aprovado
                    && !_funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(colaboradorCPF, orgId, FuncionalidadeSistemaEnum.APONTAMENTO_HORAS_COLABORADOR).Result)
                {
                    //throw new ArgumentException("Exclus�o de apontamento com status Aprovado n�o permitida.");
                    throw new ArgumentException(_stringLocalizer.GetStringOuVazio("DELETION_TRACKING_WITH_APRROVED_STATUS_NOT_ALLOWED"));
                }
            }
        }

        private void ValidaSeEstouDeletandoParaOutroColaborador(ValidaApontamentoInput input,string colaboradorCpf, int orgIdApontamento,int orgId)
        {
            if (colaboradorCpf != input.CpfRequest && orgIdApontamento == orgId)
            {
                //throw new ArgumentException("Exclus�o de apontamento permitida apenas pelo pr�prio colaborador.");
                throw new ArgumentException(_stringLocalizer.GetStringOuVazio("DELETION_TRACKING_ALLOWED_ONLY_BY_YOURSELF"));
            }
        }

        public async Task<ValidaApontamentoResult> ValidaApontamentoEmLote(string projetoId, string atividadeId, long horas, string dataInicio, string dataFim, string cpfRequest, string cpfColaborador, int orgId)
        {
            var validaApontamentoResult = ValidarApontamentoParaOutraPessoa(cpfRequest, cpfColaborador, orgId);

            await _periodoFechadoService.ValidaSeEstaNoPeriodoFechado(dataInicio, orgId);

            if (horas < 0)
            {
                // throw new ArgumentException("Para efetuar um apontamento, � necess�rio atribuir um valor igual ou superior a zero horas.");
                throw new ArgumentException(_stringLocalizer.GetStringOuVazio("REQUIRED_EQUAL_OR_HIGHER_AMOUNT"));
            }
            if (string.IsNullOrEmpty(dataInicio))
            {
                //throw new ArgumentException("A data de in�cio n�o pode ser vazia.");
                throw new ArgumentException(_stringLocalizer.GetStringOuVazio("REQUIRED_START_DATE"));
            }

            if (string.IsNullOrEmpty(dataFim))
            {
                //throw new ArgumentException("A data fim n�o pode ser vazia.");
                throw new ArgumentException(_stringLocalizer.GetStringOuVazio("REQUIRED_END_DATE"));
            }

            DateTime dataInicioDt = DateTime.Parse(dataInicio);
            DateTime dataFimDt = DateTime.Parse(dataFim);

            if (dataInicioDt > dataFimDt)
            {
                //throw new ArgumentException("A data fim deve ser maior ou igual � data in�cio.");
                throw new ArgumentException(_stringLocalizer.GetStringOuVazio("END_DATE_MUST_BE_GREATER_THAN_OR EQUAL_TO_START_DATE"));
            }


            await ValidarSeColaboradorPodeApontarNoProjeto(validaApontamentoResult.CpfUtilizado, orgId, projetoId, validaApontamentoResult.EhLancamentoParaOutroColaborador, dataFimDt);
            await ValidarSeColaboradorEstaApontandoAntesDaDataInativacao(validaApontamentoResult.CpfUtilizado, orgId, dataFimDt);
            await ValidaSeDataRegistroEhMenorQueDeCadastro(dataInicio,  validaApontamentoResult.CpfUtilizado, orgId);
            await ValidarSeAtividadeEstaAssociadaAoProjeto(atividadeId, projetoId, orgId);

            return validaApontamentoResult;
        }


        private async Task<ColaboradorApontamentoDTO> ValidaSeExisteApontamento(string colaboradorApontamentoId, CRUDEnum crudEnum)
        {
            var idioma = CultureUtil.GetCurrentCulture();
            var apontamento = await _apontamentoRepository.GetApontamentoById(colaboradorApontamentoId, idioma);

            if (apontamento == null)
            {
                if (crudEnum == CRUDEnum.Delete)
                {
                        //throw new Exception("Apontamento n�o encontrado para exclus�o!");
                        throw new Exception(_stringLocalizer.GetStringOuVazio("TRACKING_NOT_FOUND_FOR_DELETION"));
                }

                if (crudEnum == CRUDEnum.Update)
                {
                    throw new ArgumentException(_stringLocalizer.GetStringOuVazio("NOT_TIMESHEET"));
                }
            }

            return apontamento;
        }

        private void ValidaSeEPossivelFazerEdicao(DateTime? dataColetaDeDados, ColaboradorApontamentoDTO apontamento)
        {
            if(dataColetaDeDados != null && apontamento.DataAlteracao > dataColetaDeDados 
                || apontamento.StatusGrupoCodigo == (int)EnumStatusApontamentoGrupo.Aprovado) 
            {
                throw new ArgumentException(_stringLocalizer.GetStringOuVazio("MODIFIED_DURING_CHANGES"));
            }
        }

        private void ValidaDataEHoras(long horas, string dataRegistro, bool horaZerada)
        {
            if (horas == 0 && !horaZerada)
            {
                throw new ArgumentException(_stringLocalizer.GetStringOuVazio("CANNOT_BE_TRACKING_ZERO_HOURS"));
            }

            if (string.IsNullOrEmpty(dataRegistro))
            {
                throw new ArgumentException(_stringLocalizer.GetStringOuVazio("REQUIRED_START_DATE"));
            }
        }

        public async Task ValidaSeEhMeuLancamento(int orgId, string apontamentoId, string cpfRequest)
        {
           
                var apomntamento = await _apontamentoRepository.BuscarCpfPorApontamentoId(apontamentoId);
                if (apomntamento == cpfRequest)
                {
                    throw new ArgumentException(_stringLocalizer.GetStringOuVazio("ACCESS_DENIED_TO_SELF_APPROVAL"));
                }
            
        }

        private ValidaApontamentoResult ValidarApontamentoParaOutraPessoa(string cpfRequest, string cpfColaborador, int orgId)
        {
            bool ehLancamentoParaOutroColaborador = !string.IsNullOrEmpty(cpfColaborador);
            if (ehLancamentoParaOutroColaborador)
            {
                var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpfRequest, orgId, FuncionalidadeSistemaEnum.APONTAMENTO_HORAS_COLABORADOR).Result;
                
                if (!isValid)
                {
                    //throw new UnauthorizedAccessException("Acesso negado: Apontamento de Horas para outro Colaborador.");
                    throw new UnauthorizedAccessException(_stringLocalizer.GetStringOuVazio("ACCESS_DENIED_TO_ANOTHER_COLLABORATOR"));
                }
            }
            string cpfUtilizado = ehLancamentoParaOutroColaborador ? cpfColaborador : cpfRequest;

            return new ValidaApontamentoResult
            {
                EhLancamentoParaOutroColaborador = ehLancamentoParaOutroColaborador,
                CpfUtilizado = cpfUtilizado
            };
        }

        private async Task<bool> ValidarSeColaboradorPodeApontarNoProjeto(string cpf, int orgId, string codProjeto, bool ehLancamentoParaOutroColaborador, DateTime dataFimApontamento)
        {
            var listaProjetos = await _apontamentoRepository.ListarProjetosAtivosPorColaborador(cpf, orgId);

            if (listaProjetos.Any(x => x.Cod_projeto == codProjeto))
            {
                return true;
            }

            var projetoDTO = await _apontamentoRepository.GetProjetoPorCodProjetoOrgId(codProjeto, orgId);

            if (projetoDTO.DataFim != null || projetoDTO.DataFim != new DateTime(1, 1, 1))
            {
                //if (dataFimApontamento > projetoDTO.DataFim) throw new ArgumentException("Voc� n�o pode apontar horas depois que um projeto est� finalizado.");
                if (dataFimApontamento > projetoDTO.DataFim) throw new ArgumentException(_stringLocalizer.GetStringOuVazio("CANNOT_TRACKING_HOURS_AFTER_PROJECT_FINISHED"));
            }

            if (projetoDTO.PermiteApontamentoSemAlocacao
                || (ehLancamentoParaOutroColaborador && projetoDTO.PermiteApontamentoSemAlocacaoParaOutroColaborador))
            {
                return true;
            }

            //throw new ArgumentException("Voc� precisa estar alocado no projeto para apontar horas.");
            throw new ArgumentException(_stringLocalizer.GetStringOuVazio("MUST_BE_PROJECT_ALLOCATED_FOR_TRACKING_HOUR"));
        }

        private async Task<bool> ValidarSeColaboradorEstaApontandoAntesDaDataInativacao(string cpf, int orgId, DateTime dtFinalApontamento)
        {
            var colaboradorDTO = await _apontamentoRepository.GetColaboradorPorCpfEOrgId(cpf, orgId);

            if (colaboradorDTO.DataInativacao is not null && !colaboradorDTO.Ativo)
            {
                if (dtFinalApontamento > colaboradorDTO.DataInativacao.ToDateTime())
                {
                    //throw new ArgumentException($"O colaborador n�o pode apontar ap�s {colaboradorDTO.DataInativacao.ToDateTime().ToString("dd/MM/yyyy")}, porque est� inativo nessa data.");
                    var errorMessage = _stringLocalizer.GetStringOuVazio("COLLABORATOR_CANNOT_TRACKING_BY_INACTIVE_DATE");
                    errorMessage = errorMessage.Replace("{{INACTIVE_DATE}}", colaboradorDTO.DataInativacao.ToDateTime().ToShortDateString());

                    throw new ArgumentException(errorMessage);
                }
            }
            return true;
        }

        private async Task ValidarSeAtividadeEstaAssociadaAoProjeto(string atividadeId, string projetoId, int orgId)
        {
            if (string.IsNullOrEmpty(atividadeId) || string.IsNullOrEmpty(projetoId))
            {
                return;
            }

            var atividadesAssociadas = await _atividadeProjetoRepository.ListarAtividadesAssociadasComProjeto(projetoId, orgId);

            var atividadeEstaAssociada = atividadesAssociadas != null && atividadesAssociadas.Any(a => a.Id.ToString() == atividadeId);

            if (!atividadeEstaAssociada)
            {
                throw new ArgumentException("A atividade selecionada não está associada ao projeto. Por favor, selecione uma atividade válida para este projeto.");
            }
        }

    }
}