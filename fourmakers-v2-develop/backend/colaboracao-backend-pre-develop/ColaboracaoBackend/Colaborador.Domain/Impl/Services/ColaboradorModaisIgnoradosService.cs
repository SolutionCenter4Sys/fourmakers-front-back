using Colaboracao.Helper.Enum;
using Colaborador.Domain.Interfaces.Services;
using Core.Domain.Colaborador;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using Logs.Infra.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class ColaboradorModaisIgnoradosService : IColaboradorModaisIgnoradosService
    {
        private readonly IColaboradorModaisIgnoradosRepository _repository;

        public ColaboradorModaisIgnoradosService(IColaboradorModaisIgnoradosRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiGenericResult<List<string>>> ListarMeusIgnoradosAsync(string cpf, int orgId)
        {
            var result = new ApiGenericResult<List<string>>();
            try
            {
                var lista = await _repository.ListarPorColaboradorOrgAsync(cpf, orgId);
                result.Retorno = lista.Select(x => x.Tag).ToList();
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "ColaboradorModaisIgnorados");
            }
            return result;
        }

        public async Task<ApiGenericResult<List<string>>> IgnoreDialogAsync(string cpf, int orgId, string tag)
        {
            var result = new ApiGenericResult<List<string>>();
            try
            {
                if (string.IsNullOrWhiteSpace(tag))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Tag é obrigatória.";
                    return result;
                }

                await _repository.InserirAsync(cpf, orgId, tag.Trim());
                var lista = await _repository.ListarPorColaboradorOrgAsync(cpf, orgId);
                result.Retorno = lista.Select(x => x.Tag).ToList();
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, "ColaboradorModaisIgnorados");
            }
            return result;
        }
    }
}
