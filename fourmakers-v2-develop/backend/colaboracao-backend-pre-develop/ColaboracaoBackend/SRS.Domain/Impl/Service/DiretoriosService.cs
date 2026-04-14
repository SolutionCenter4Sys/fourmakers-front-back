using Colaboracao.Core;
using Colaboracao.Helper.Enum;
using Core.Domain.Contratacao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Contratacao;
using DataTransferObject.Domain.Log;
using SRS.Domain.Interfaces.Service;
using SRS.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace SRS.Domain.Impl.Service
{
    [LogDomainClass]
    public class DiretoriosService : IDiretoriosService
    {
        private readonly IDiretoriosRepository _diretoriosRepository;
        private readonly ILogCore _log;

        public DiretoriosService(IDiretoriosRepository diretoriosRepository, ILogCore log)
        {
            _diretoriosRepository = diretoriosRepository;
            _log = log;
        }

        public async Task<ApiGenericResult<List<DiretorioDTO>>> ListarDiretoriosAsync()
        {
            var result = new ApiGenericResult<List<DiretorioDTO>>();
            try
            {
                var diretorios = await _diretoriosRepository.ListarDiretoriosAsync();
                
                result.Retorno = diretorios.ToList();
                result.Sucesso = true;
                result.Mensagem = "Diretórios listados com sucesso";
                
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao listar diretórios: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao listar diretórios";
                return result;
            }
        }

        public async Task<ApiGenericResult<DiretorioDTO>> ObterDiretorioPorIdAsync(Guid id)
        {
            var result = new ApiGenericResult<DiretorioDTO>();
            try
            {
                if (id == Guid.Empty)
                {
                    result.Sucesso = false;
                    result.Mensagem = "ID do diretório é obrigatório";
                    return result;
                }

                var diretorio = await _diretoriosRepository.ObterDiretorioPorIdAsync(id);
                
                if (diretorio == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Diretório não encontrado";
                    return result;
                }

                result.Retorno = diretorio;
                result.Sucesso = true;
                result.Mensagem = "Diretório obtido com sucesso";
                
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao obter diretório: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao obter diretório";
                return result;
            }
        }
    }
}
