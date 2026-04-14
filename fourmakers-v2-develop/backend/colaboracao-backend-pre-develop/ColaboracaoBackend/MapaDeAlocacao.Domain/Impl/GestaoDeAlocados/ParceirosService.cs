using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Util;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados
{
    [LogDomainClass]
    public class ParceirosService : IParceirosService
    {

        private readonly IParceirosRepository _parceirosRepository;
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly string _pastaKey = "arquivos/parceiros/";


        public ParceirosService(IParceirosRepository parceirosRepository, IUploadFilesClient uploadFilesClient)
        {
            _parceirosRepository = parceirosRepository;
            _uploadFilesClient = uploadFilesClient;
        }

        // ───────────────────────────────────────────────
        // Parceiro CRUD
        // ───────────────────────────────────────────────
        public async Task<ApiGenericResult<ParceiroDTO>> BuscarParceiroPorId(string parceiroID)
        {
            try
            {
                var partner = await _parceirosRepository.BuscarParceiroPorId(parceiroID);
                return partner == null ? new ApiGenericResult<ParceiroDTO>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Parceiro {parceiroID} não encontrado."

                } : new ApiGenericResult<ParceiroDTO>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Parceiro encontrado. id: {parceiroID}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar parceiro: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<List<ParceiroDTO>>> BuscarTodosParceiros(int? orgId, string? filtro, string? bucket)
        {
            try
            {
                var list = await _parceirosRepository.BuscarTodosParceiros(orgId, filtro, bucket);
                return list == null ? new ApiGenericResult<List<ParceiroDTO>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Parceiros não encontrados."

                } : new ApiGenericResult<List<ParceiroDTO>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Parceiros encontrados. total: {list.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar parceiros: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<ParceiroParamDTO>> InserirParceiro(ParceiroInserirParam param, string cpfUsuarioLogado)
        {
            try
            {
                var partner = await _parceirosRepository.InserirParceiro(param, cpfUsuarioLogado);
                return partner == null ? new ApiGenericResult<ParceiroParamDTO>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Não foi possível inserir o parceiro."

                } : new ApiGenericResult<ParceiroParamDTO>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Parceiro inserido. id: {partner.ID}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao inserir parceiro: {e.Message}");
            }
        }
        public async Task<ApiGenericResult<ParceiroParamDTO>> AtualizarParceiro(string parceiroID, ParceiroIDParam param, string cpfUsuarioLogado)
        {
            try
            {
                var partner = await _parceirosRepository.AtualizarParceiro(parceiroID, param, cpfUsuarioLogado);
                return partner == null ? new ApiGenericResult<ParceiroParamDTO>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Não foi possível atualizar o parceiro. Id: {parceiroID}"

                } : new ApiGenericResult<ParceiroParamDTO>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Parceiro atualizado. id: {partner.ID}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao atualizar parceiro: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<bool>> DeletarParceiro(string parceiroID)
        {
            try
            {
                var ok = await _parceirosRepository.DeletarParceiro(parceiroID);
                return ok == false ? new ApiGenericResult<bool>
                {
                    Retorno = false,
                    Sucesso = false,
                    Mensagem = $"Não foi possível excluir o Parceiro. Id: {parceiroID}"

                } : new ApiGenericResult<bool>
                {
                    Retorno = ok,
                    Sucesso = true,
                    Mensagem = $"Parceiro excluido. id: {parceiroID}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao excluir parceiro: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<ParceiroArchiveDTO>> BuscarArquivoPorId(string arquivoId)
        {

            try
            {
                var result = await _parceirosRepository.BuscarArquivoPorId(arquivoId);

                if (result == null)
                {
                    return new ApiGenericResult<ParceiroArchiveDTO>
                    {
                        Retorno = null,
                        Sucesso = false,
                        Mensagem = $"Não foi possível encontrar o arquivo: {arquivoId}"
                    };
                }

                return new ApiGenericResult<ParceiroArchiveDTO>
                {
                    Retorno = result,
                    Sucesso = true,
                    Mensagem = $"Arquivo encontrado com sucesso",
                    Erros = null

                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"Não foi possível encontrar o arquivo: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<List<ParceiroArchiveDTO>>> BuscarArquivosPorParceiroId(string parceiroID)
        {
            try
            {

                var result = await _parceirosRepository.BuscarArquivosPorParceiroId(parceiroID);

                if (result == null)
                {
                    return new ApiGenericResult<List<ParceiroArchiveDTO>>
                    {
                        Retorno = null,
                        Sucesso = false,
                        Mensagem = $"Não foi possível encontrar os arquivos: {parceiroID}"
                    };
                }

                return new ApiGenericResult<List<ParceiroArchiveDTO>>
                {
                    Retorno = result,
                    Sucesso = true,
                    Mensagem = $"Arquivos encontrados",
                    Erros = null

                };

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"Não foi possível encontrar os arquivos: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<ParceiroArchiveDTO>> InserirArquivo(ParceiroArchiveParam param)
        {
            var data = DateTime.Now.ToString("yyyyMMddHHmm");
            var prefixo = GetArchiveNameByOrigin(param.ArquivoOriginID);
            var nomeBase = $"{prefixo}{data}";
            var extensao = GetArchiveType(param.ArquivoTipoID);
            var objectKey = $"{_pastaKey}{nomeBase}{extensao}";
            var fileName = $"{nomeBase}{extensao}";
            var bytes = param.bytes; // já é byte[]

            try
            {
                await _uploadFilesClient.UploadFile(objectKey, bytes);

                var linkUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(
                    EnvironmentVariables.SERVICE_MIDIA) + objectKey;

                var res = await _parceirosRepository.InserirArquivo(param, linkUrl);

                if (res == null)
                {
                    return new ApiGenericResult<ParceiroArchiveDTO>
                    {
                        Sucesso = false,
                        Mensagem = "Não foi possível salvar o arquivo no banco.",
                        Retorno = null
                    };
                }

                return new ApiGenericResult<ParceiroArchiveDTO>
                {
                    Erros = null,
                    Sucesso = true,
                    Mensagem = "Arquivo salvo com sucesso",
                    Retorno = res,
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"Não foi possível salvar o arquivo: {e.Message}", e);
            }
        }

        public async Task<ApiGenericResult<ParceiroGestaoContratoDTO>> InserirContrato(ParceiroGestaoContratoParceiroIDParam param)
        {
            try
            {
                var partner = await _parceirosRepository.InserirContrato(param);
                return partner == null ? new ApiGenericResult<ParceiroGestaoContratoDTO>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Não foi possível inserir o Contrato(Documento)."

                } : new ApiGenericResult<ParceiroGestaoContratoDTO>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Contrato(Documento) inserido. id: {partner.ID}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao inserir Contrato(Documento): {e.Message}");
            }
        }
        public async Task<ApiGenericResult<ParceiroGestaoContratoDTO>> AtualizarContrato(ParceiroGestaoContratoParam param)
        {
            try
            {
                var partner = await _parceirosRepository.AtualizarContrato(param);
                return partner == null ? new ApiGenericResult<ParceiroGestaoContratoDTO>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Não foi possível atualizar o Contrato. Id: {param.ID}"

                } : new ApiGenericResult<ParceiroGestaoContratoDTO>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Contrato atualizado. id: {partner.ID}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao atualizar o Contrato: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<bool>> DeletarContrato(string ID)
        {
            try
            {
                var ok = await _parceirosRepository.DeletarContrato(ID);
                return ok == false ? new ApiGenericResult<bool>
                {
                    Retorno = false,
                    Sucesso = false,
                    Mensagem = $"Não foi possível excluir o Contrato. Id: {ID}"

                } : new ApiGenericResult<bool>
                {
                    Retorno = ok,
                    Sucesso = true,
                    Mensagem = $"Contrato excluido. id: {ID}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao excluir Contrato: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<bool>> DeletarArquivo(string ID)
        {
            try
            {
                var ok = await _parceirosRepository.DeletarArquivo(ID);
                return ok == false ? new ApiGenericResult<bool>
                {
                    Retorno = false,
                    Sucesso = false,
                    Mensagem = $"Não foi possível excluir o Arquivo. Id: {ID}"

                } : new ApiGenericResult<bool>
                {
                    Retorno = ok,
                    Sucesso = true,
                    Mensagem = $"Arquivo excluido. id: {ID}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao excluir o Arquivo: {e.Message}");
            }
        }
        public async Task<ApiGenericResult<ParceiroGestaoContratoDTO>> BuscarGestaoContratoPorId(string gestaoContratoID)
        {
            try
            {
                var list = await _parceirosRepository.BuscarGestaoContratoPorId(gestaoContratoID);
                return list == null ? new ApiGenericResult<ParceiroGestaoContratoDTO>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Contrato não encontrado."

                } : new ApiGenericResult<ParceiroGestaoContratoDTO>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Contrato encontrado: {list.ID}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Contrato: {e.Message}");
            }
        }

        private string GetArchiveType(ParceirosArchiveType type)
        {
            switch (type)
            {
                case ParceirosArchiveType.DOC:
                    return "_doc.pdf";

                case ParceirosArchiveType.SHEET:
                    return "_sheet.xlsx";

                case ParceirosArchiveType.IMAGEM:
                    return "_imagem.jpg";

                case ParceirosArchiveType.OUTROS:
                    return "_outros.pdf";

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Tipo de arquivo desconhecido: {type}");
            }
        }

        private string GetArchiveNameByOrigin(ParceirosArchiveOrigin type)
        {
            switch (type)
            {
                case ParceirosArchiveOrigin.ADTIVO:
                    return "adtivo_";

                case ParceirosArchiveOrigin.CONTRATO:
                    return "contrato_";

                case ParceirosArchiveOrigin.LOGO:
                    return "logo_";

                case ParceirosArchiveOrigin.NDA:
                    return "nda_";

                case ParceirosArchiveOrigin.OUTROS:
                    return "outros_";

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Tipo de arquivo desconhecido: {type}");
            }
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioParceriaAliancas(int orgId)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {

                var relatorioVagasResult = await _parceirosRepository.RelatorioParceriaAliancas(orgId);

                if (!relatorioVagasResult.Any())
                {
                    ret.Mensagem = "Não existem dados para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(relatorioVagasResult);
                var fileName = "Relatorio_Parceria_Aliancas_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ret.Retorno.FileDownloadName = fileName;
                ret.Sucesso = true;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        public async Task<ApiGenericResult<(int Sucesso, int Erros, List<string> Mensagens)>> ImportacaoPlanilhaContrato(
                string caminhoArquivo,
                string cpfUsuarioLogado,
                int orgId)
        {
            try
            {
                var resultado = await _parceirosRepository.ImportacaoPlanilhaContrato(caminhoArquivo, cpfUsuarioLogado, orgId);

                return new ApiGenericResult<(int Sucesso, int Erros, List<string> Mensagens)>
                {
                    Retorno = resultado,
                    Sucesso = true,
                    Mensagem = $"Importação concluída: {resultado.Sucesso} sucesso(s), {resultado.Erros} erro(s)."
                };
            }
            catch (Exception e)
            {
                return new ApiGenericResult<(int Sucesso, int Erros, List<string> Mensagens)>
                {
                    Retorno = (0, 1, new List<string> { e.Message }),
                    Sucesso = false,
                    Mensagem = $"Erro ao importar planilha: {e.Message}"
                };
            }
        }

    }

}
