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
    public class GruposEmailsService : IGruposEmailsService
    {
        private readonly IGruposEmailsRepository _gruposEmailsRepository;
        private readonly ILogCore _log;

        public GruposEmailsService(IGruposEmailsRepository gruposEmailsRepository, ILogCore log)
        {
            _gruposEmailsRepository = gruposEmailsRepository;
            _log = log;
        }

        public async Task<ApiGenericResult<List<GrupoEmailDTO>>> ListarGruposEmailsAsync()
        {
            var result = new ApiGenericResult<List<GrupoEmailDTO>>();
            try
            {
                var gruposEmails = await _gruposEmailsRepository.ListarGruposEmailsAsync();
                
                result.Retorno = gruposEmails.ToList();
                result.Sucesso = true;
                result.Mensagem = "Grupos de emails listados com sucesso";
                
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao listar grupos de emails: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao listar grupos de emails";
                return result;
            }
        }

        public async Task<ApiGenericResult<GrupoEmailDTO>> ObterGrupoEmailPorIdAsync(Guid id)
        {
            var result = new ApiGenericResult<GrupoEmailDTO>();
            try
            {
                if (id == Guid.Empty)
                {
                    result.Sucesso = false;
                    result.Mensagem = "ID do grupo de email é obrigatório";
                    return result;
                }

                var grupoEmail = await _gruposEmailsRepository.ObterGrupoEmailPorIdAsync(id);
                
                if (grupoEmail == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Grupo de email não encontrado";
                    return result;
                }

                result.Retorno = grupoEmail;
                result.Sucesso = true;
                result.Mensagem = "Grupo de email obtido com sucesso";
                
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao obter grupo de email: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao obter grupo de email";
                return result;
            }
        }
    }
}
