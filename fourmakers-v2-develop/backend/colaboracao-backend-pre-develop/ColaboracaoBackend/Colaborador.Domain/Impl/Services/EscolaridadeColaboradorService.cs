using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Helper;
using Colaborador.Domain.Interfaces.Services;
using Core.Domain.Colaborador;
using Core.DomainModel;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Escolaridade;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class EscolaridadeColaboradorService : IEscolaridadeColaboradorService
    {
        private readonly ILogCore _logCore;
        private readonly IEscolaridadeColaboradorDtoRepository _escolaridadeRepository;
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly IConfiguration _configuration;
        private IHistoricoCVRepository _historicoCvRepository;

        public EscolaridadeColaboradorService(
            ILogCore logCore,
            IEscolaridadeColaboradorDtoRepository escolaridadeRepository,
            IUploadFilesClient uploadFilesClient,
            IConfiguration configuration,
            IHistoricoCVRepository historicoCvRepository)
        {
            _logCore = logCore;
            _escolaridadeRepository = escolaridadeRepository;
            _uploadFilesClient = uploadFilesClient;
            _configuration = configuration;
            _historicoCvRepository = historicoCvRepository;
        }

        public async Task<EscolaridadeDTO> AdicionarNovaEscolaridade(long formacaoId, string instituicao, DateTime dataInicio, DateTime? dataTermino, string descricao, string colaboradorCpf, int? tipoDiplomaId, byte[] certificado, string fileType, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            string retFilePath = null;
            if (certificado != null)
            {
                retFilePath = await uploadCertificado(colaboradorCpf, certificado, fileType);
            }

            var dto = new EscolaridadeDTO
            {
                FormacaoId = formacaoId,
                Instituicao = instituicao,
                DataInicio = dataInicio,
                DataTermino = dataTermino,
                Descricao = descricao,
                ColaboradorCpf = colaboradorCpf,
                TipoDiplomaId = tipoDiplomaId,
                FilePathInternal = retFilePath,
                Ativo = true
            };

            var ret = _escolaridadeRepository.Save(dto);
            if (certificado != null)
            {
                ret.FilePath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + retFilePath;
            }

            _historicoCvRepository.InserirHistoricoCV(colaboradorCpf, origem, TipoItemCVEnum.INSERT, null, null, ItemCVEnum.ESCOLARIDADE);
            return ret;
        }

        private async Task<string> uploadCertificado(string cpf, byte[] file, string fileType)
        {
            var filePath = _configuration["PathCertificadoEscolaridade"] + cpf + DateTime.Now.ToString("yyyyMMddHHmmss") + fileType;
            var ret = await _uploadFilesClient.UploadFile(filePath, file);
            if (ret.Sucesso)
            {
                return filePath;
            }

            throw new Exception("Não foi possível realizar o upload do certificado.");
        }

        public async Task<EscolaridadeDTO> AtualizarEscolaridade(long id, long? formacaoId, string instituicao, DateTime dataInicio, DateTime? dataTermino, string descricao, int tipoDiplomaId, string colaboradorCpf, byte[] certificado, string fileType, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            string retFilePath = null;
            if (certificado != null)
            {
                retFilePath = await uploadCertificado(colaboradorCpf, certificado, fileType);
            }

            var dto = new EscolaridadeDTO
            {
                Id = id,
                FormacaoId = formacaoId,
                Instituicao = instituicao,
                DataInicio = dataInicio,
                DataTermino = dataTermino,
                Descricao = descricao,
                ColaboradorCpf = colaboradorCpf,
                TipoDiplomaId = tipoDiplomaId,
                FilePathInternal = retFilePath,
                Ativo = true
            };

            var ret = _escolaridadeRepository.Update(dto);
            if (certificado != null)
            {
                ret.FilePath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + retFilePath;
            }

            _historicoCvRepository.InserirHistoricoCV(colaboradorCpf, origem, TipoItemCVEnum.UPDATE, null, null, ItemCVEnum.ESCOLARIDADE);
            return ret;
        }

        public EscolaridadeDTO GetEscolaridadeColaboradorById(long id)
        {
            try
            {
                var dto = new EscolaridadeDTO { Id = id };
                var ret = _escolaridadeRepository.GetModel(dto);
                if (ret == null)
                    throw new Exception();

                if (ret.FilePathInternal != null)
                {
                    ret.FilePath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + ret.FilePathInternal;
                }

                return ret;
            }
            catch (Exception)
            {
                throw new Exception("Escolaridade não encontrada");
            }
        }

        public List<EscolaridadeDTO> ListarEscolaridadeColaborador(string busca, int cursor, int limite, string cpf)
        {
            return _escolaridadeRepository
                .Listar(busca, cursor, limite, cpf)
                .Select(ret =>
                {
                    if (ret.FilePath != null)
                    {
                        ret.FilePath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + ret.FilePath;
                    }

                    return ret;
                })
                .OrderByDescending(x => x.DataInicio)
                .ToList();
        }

        public void RemoverEscolaridadeColaborador(long escolaridadeId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            var dto = new EscolaridadeDTO
            {
                Id = escolaridadeId,
                ColaboradorCpf = cpf
            };
            _escolaridadeRepository.DeleteEscolaridadeColaboradorModel(dto);
            _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.DELETE, null, null, ItemCVEnum.ESCOLARIDADE);
        }
    }
}
