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
    public class SistemasLiberadosService : ISistemasLiberadosService
    {
        private readonly ISistemasLiberadosRepository _sistemasLiberadosRepository;
        private readonly ILogCore _log;

        public SistemasLiberadosService(ISistemasLiberadosRepository sistemasLiberadosRepository, ILogCore log)
        {
            _sistemasLiberadosRepository = sistemasLiberadosRepository;
            _log = log;
        }

        public async Task<ApiGenericResult<List<SistemaLiberadoDTO>>> ListarSistemasLiberadosAsync()
        {
            var result = new ApiGenericResult<List<SistemaLiberadoDTO>>();
            try
            {
                var sistemas = await _sistemasLiberadosRepository.ListarSistemasLiberadosAsync();
                
                result.Retorno = sistemas.ToList();
                result.Sucesso = true;
                result.Mensagem = "Sistemas liberados listados com sucesso";
                
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao listar sistemas liberados: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao listar sistemas liberados";
                return result;
            }
        }

        public async Task<ApiGenericResult<SistemaLiberadoDTO>> ObterSistemaLiberadoPorIdAsync(Guid id)
        {
            var result = new ApiGenericResult<SistemaLiberadoDTO>();
            try
            {
                if (id == Guid.Empty)
                {
                    result.Sucesso = false;
                    result.Mensagem = "ID do sistema liberado é obrigatório";
                    return result;
                }

                var sistema = await _sistemasLiberadosRepository.ObterSistemaLiberadoPorIdAsync(id);
                
                if (sistema == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Sistema liberado não encontrado";
                    return result;
                }

                result.Retorno = sistema;
                result.Sucesso = true;
                result.Mensagem = "Sistema liberado obtido com sucesso";
                
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao obter sistema liberado: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao obter sistema liberado";
                return result;
            }
        }
    }
}
