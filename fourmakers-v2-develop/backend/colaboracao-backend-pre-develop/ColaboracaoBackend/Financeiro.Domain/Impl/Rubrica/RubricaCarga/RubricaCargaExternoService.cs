using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Infra.Repositories.Financeiro.Financeiro.Rubrica;
using Core.Domain;
using Core.Domain.Financeiro.Rubrica;
using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using DataTransferObject.Domain.Util.Enum;
using Financeiro.Domain.Interfaces.Rubrica.RubricaCarga;
using Financeiro.Domain.Interfaces.Rubrica.RubricaColaborador;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;
using Core.Domain.Financeiro.NotaFiscal;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.Rubrica.RubricaCarga;

[LogDomainClass]
public class RubricaCargaExternoService : IRubricaCargaExternoService
{
    private readonly static string DESCRICAO_ENTIDADE = "Carga Rubrica";

    IRubricaColaboradorService _rubricaColaboradorService;
    IBuscaColaboradorOrgRepository _buscaColaboradorOrgRepository;
    IRubricaRepository _rubricaRepository;
    IRubricaCargaMassivaLogRepository _rubricaCargaMassivaLogRepository;
    ITokenSistemaService _tokenSistemaService;

    public RubricaCargaExternoService(IRubricaColaboradorService rubricaColaboradorService,
                                      IBuscaColaboradorOrgRepository buscaColaboradorOrgRepository,
                                      IRubricaRepository rubricaRepository,
                                      IRubricaCargaMassivaLogRepository rubricaCargaMassivaLogRepository,
                                      ITokenSistemaService tokenSistemaService)
    {
        _rubricaColaboradorService = rubricaColaboradorService;
        _buscaColaboradorOrgRepository = buscaColaboradorOrgRepository;
        _rubricaRepository = rubricaRepository;
        _rubricaCargaMassivaLogRepository = rubricaCargaMassivaLogRepository;
        _tokenSistemaService = tokenSistemaService;
    }

    public async Task<ApiGenericResult<RubricaCargaMassivaResult>> ProcessarCargaMassiva(string tokenSistema, Base64DTO base64DTO, PoliticaErroCarga politicaErro = PoliticaErroCarga.ContinuarComAviso, PoliticaConflitoCarga politicaConflito = PoliticaConflitoCarga.NaoAtualizar, string arquivoOrigem = null)
    {
        var apiGenericResult = new ApiGenericResult<RubricaCargaMassivaResult>();

        int orgId = _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(tokenSistema, EnumORG.ROYAL_9);

        var resultado = new RubricaCargaMassivaResult
        {
            CargaId = Guid.NewGuid().ToString(),
            Sucesso = true
        };

        // Criar log principal da carga
        var logCarga = new RubricaCargaMassivaLogDTO
        {
            CodigoCargaRubrica = resultado.CargaId,
            ArquivoOrigem = arquivoOrigem ?? "arquivo_carga_massiva.csv",
            CodDiretoria = "GERAL",
            TbRubricaId = "",
            TbRubricaTemplateId = 1, 
            StatusProcessamento = StatusProcessamentoCargaEnum.Iniciado,
            QtdRegistrosProcessadosSucesso = 0,
            QtdRegistrosRetornados = 0,
            TbOrgId = orgId,
            CodigoInternoColaboradorCriacao = "",
            Mes = 0,
            Ano = 0,
            DataCriacao = DateTime.Now
        };

        try
        {
            // Inserir log inicial
            await _rubricaCargaMassivaLogRepository.InserirLogCarga(logCarga);
            
            var arquivoBytes = base64DTO.Base64ToByteArray();

            if (arquivoBytes.Length == 0)
            {
                await _rubricaCargaMassivaLogRepository.AtualizarStatusCarga(resultado.CargaId, "erro", "Erro ao processar arquivo enviado");
                throw new ArgumentException("Erro ao processar arquivo enviado");
            }

            var linhasArquivo = ProcessarArquivoCSV(arquivoBytes);

            resultado.TotalLinhas = linhasArquivo.Count;

            // Atualizar log com informações do arquivo
            if (linhasArquivo.Count > 0)
            {
                var (mes, ano) = ExtrairMesAnoVigencia(linhasArquivo.First().Vigencia);
                // Atualizar no banco os dados que não estavam disponíveis na criação inicial
                await _rubricaCargaMassivaLogRepository.AtualizarMesAnoEQuantidade(resultado.CargaId, mes, ano, linhasArquivo.Count);
            }

            if (linhasArquivo.Count == 0)
            {
                await _rubricaCargaMassivaLogRepository.AtualizarStatusCarga(resultado.CargaId, "erro", "Arquivo não contém dados válidos");
                resultado.MensagensErro.Add("Arquivo não contém dados válidos");
                resultado.Sucesso = false;
                apiGenericResult.Retorno = resultado;
                return apiGenericResult;
            }

            // Atualizar status para processamento
            await _rubricaCargaMassivaLogRepository.AtualizarStatusCarga(resultado.CargaId, "processamento");

            var deveProcessar = await ValidarLinhasComRubricas(linhasArquivo, resultado, orgId, politicaErro, politicaConflito);

            if (deveProcessar)
            {
                await ProcessarRubricaColaborador(linhasArquivo, resultado, orgId);
            }

            resultado.LinhasProcessadas = resultado.TotalLinhas - resultado.LinhasComErro - resultado.LinhasDuplicadas;

            if ((resultado.LinhasComErro > 0 || resultado.LinhasDuplicadas > 0) && resultado.MensagensAlerta.Count == 0)
            {
                resultado.MensagensAlerta.Add($"Processamento concluído com alertas: {resultado.LinhasProcessadas} processadas, {resultado.LinhasComErro} com erro, {resultado.LinhasDuplicadas} duplicadas");
            }

            // Atualizar status final
            string statusFinal = resultado.Sucesso ? "sucesso" : "erro";
            string mensagemErro = resultado.Sucesso ? null : string.Join("; ", resultado.MensagensErro);
            await _rubricaCargaMassivaLogRepository.AtualizarStatusCarga(
                resultado.CargaId, 
                statusFinal, 
                mensagemErro, 
                resultado.LinhasProcessadas, 
                resultado.TotalLinhas);

            apiGenericResult.Retorno = resultado;
        }
        catch (Exception ex)
        {
            resultado.Sucesso = false;
            resultado.MensagensErro.Add($"Erro durante processamento: {ex.Message}");
            
            // Atualizar log com erro
            await _rubricaCargaMassivaLogRepository.AtualizarStatusCarga(resultado.CargaId, "erro", ex.Message);
            
            apiGenericResult.Retorno = resultado;
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, "Carga Massiva Rubrica");
        }

        return apiGenericResult;
    }

    private async Task<bool> ValidarLinhasComRubricas(List<RubricaColaboradorLinhaArquivoDTO> linhasArquivo, RubricaCargaMassivaResult resultado, int orgId, PoliticaErroCarga politicaErro, PoliticaConflitoCarga politicaConflito)
    {
        var linhasProcessadas = new HashSet<string>();
        var linhasParaRemover = new List<RubricaColaboradorLinhaArquivoDTO>();

        if (linhasArquivo.Where(x => string.IsNullOrEmpty(x.ProcessamentoInfo.ErroFormatacao)).GroupBy(x => x.Vigencia).Count() > 1)
        {
            resultado.Sucesso = false;
            resultado.MensagensErro.Add($"Erro ao processar arquivo: a coluna de vigência não pode ter mais de um valor.");
            return false;
        }

        var vigenciaExtraida = ExtrairMesAnoVigencia(linhasArquivo.First().Vigencia);

        var rubricasColaboradorExistentesDB = await _rubricaColaboradorService.ListarRubricasColaboradorDetalhado("#NAO_VALIDAR", vigenciaExtraida.mes, vigenciaExtraida.ano, orgId);
        var rubricaExistentesDB = await _rubricaRepository.ListarRubricasAsync(orgId);

        List<RubricaColaboradorLinhaArquivoDTO> rubricasExistentesDB = MapRubricaColaboradorDetalhadoToRubricaLinhaArquivoDTO(vigenciaExtraida, rubricasColaboradorExistentesDB);

        foreach (var linha in linhasArquivo)
        {

            linha.ProcessamentoInfo.MesVigencia = vigenciaExtraida.mes;
            linha.ProcessamentoInfo.AnoVigencia = vigenciaExtraida.ano;

            // 1. Verificar erro de formatação
            if (!string.IsNullOrEmpty(linha.ProcessamentoInfo.ErroFormatacao))
            {
                resultado.MensagensErro.Add($"Linha {linha.NumeroLinha}: {linha.ProcessamentoInfo.ErroFormatacao}");
                resultado.LinhasComErro++;
                AdicionarItemProcessado(resultado, linha, "Erro", linha.ProcessamentoInfo.ErroFormatacao);
                linhasParaRemover.Add(linha);
                continue;
            }

            // 2. Validação de preenchimento
            if (!ValidarPreenchimentoLinha(linha, out string mensagemErro))
            {
                resultado.MensagensErro.Add($"Linha {linha.NumeroLinha}: {mensagemErro}");
                resultado.LinhasComErro++;
                AdicionarItemProcessado(resultado, linha, "Erro", mensagemErro);
                linhasParaRemover.Add(linha);
                continue;
            }

            // 3. Verificar duplicatas no próprio arquivo
            if (linhasProcessadas.Contains(linha.ProcessamentoInfo.LinhaStringOriginal))
            {
                resultado.MensagensAlerta.Add($"Linha {linha.NumeroLinha}: Duplicata encontrada no arquivo");
                resultado.LinhasDuplicadas++;
                AdicionarItemProcessado(resultado, linha, "Duplicado", "Duplicata encontrada no arquivo");
                linhasParaRemover.Add(linha);
                continue;
            }
            linhasProcessadas.Add(linha.ProcessamentoInfo.LinhaStringOriginal);

            // 4. Validar se colaborador existe
            var colaborador = await _buscaColaboradorOrgRepository.GetColaboradorEOrgsAsync(linha.CPF, null, orgId);
            if (colaborador is null
                || colaborador.ColaboradorOrgs.Where(x => x.TbOrgId == orgId).Count() == 0)
            {
                resultado.MensagensErro.Add($"Linha {linha.NumeroLinha}: Colaborador {linha.NomeColaborador} não encontrado");
                resultado.LinhasComErro++;
                AdicionarItemProcessado(resultado, linha, "Erro", $"Colaborador {linha.NomeColaborador} não encontrado");
                linhasParaRemover.Add(linha);
                continue;
            }
            linha.ProcessamentoInfo.CodigoInternoColaborador = Guid.Parse(colaborador.CodigoInternoColaborador);

            // 5. Validar se rubrica existe e preencher RubricaId
            var rubrica = rubricaExistentesDB.Where(x => x.CodigoRubrica == linha.CodRubrica).SingleOrDefault();
            if (rubrica == null)
            {
                resultado.MensagensErro.Add($"Linha {linha.NumeroLinha}: Rubrica {linha.CodRubrica} não encontrada");
                resultado.LinhasComErro++;
                AdicionarItemProcessado(resultado, linha, "Erro", $"Rubrica {linha.CodRubrica} não encontrada");
                linhasParaRemover.Add(linha);
                continue;
            }
            linha.ProcessamentoInfo.RubricaId = rubrica.Id;

            // Processar o valor usando a validação existente
            if (decimal.TryParse(linha.ValorGerado, NumberStyles.Number, new CultureInfo("pt-BR"), out decimal valorTratado))
            {
                linha.ProcessamentoInfo.ValorTratado = valorTratado;
            }
            else
            {
                resultado.MensagensErro.Add($"Linha {linha.NumeroLinha}: Valor deve ser numérico");
                resultado.LinhasComErro++;
                AdicionarItemProcessado(resultado, linha, "Erro", "Valor deve ser numérico");
                linhasParaRemover.Add(linha);
                continue;
            }

            // 6. Verificar conflitos no banco de dados
            await VerificarConflitosLinhaEspecifica(linha, resultado, orgId, politicaConflito, linhasParaRemover, rubricasExistentesDB, vigenciaExtraida.mes, vigenciaExtraida.ano);
        }

        foreach (var linha in linhasParaRemover)
        {
            linhasArquivo.Remove(linha);
        }

        if (linhasArquivo.Count == 0)
        {
            resultado.Sucesso = false;
            resultado.MensagensAlerta.Add("Processamento interrompido: todas as linhas apresentaram erros ou duplicatas, não restando registros válidos para processar.");
            return false;
        }

        if (resultado.LinhasComErro > 0 && politicaErro == PoliticaErroCarga.RejeitarArquivo)
        {
            resultado.Sucesso = false;
            resultado.MensagensAlerta.Add("Processamento interrompido devido à política de rejeição de arquivo");
            return false;
        }

        return true;
    }

    private static List<RubricaColaboradorLinhaArquivoDTO> MapRubricaColaboradorDetalhadoToRubricaLinhaArquivoDTO((int mes, int ano) vig, ApiGenericResult<IEnumerable<RubricaColaboradorDetalhadoDTO>> rubricasExistentesResult)
    {
        var rubricasExistentesDB = rubricasExistentesResult.Retorno?.Select(r => new RubricaColaboradorLinhaArquivoDTO
        {
            Vigencia = $"{vig.mes:D2}/{vig.ano}",
            CPF = "",
            CodRubrica = r.Rubrica?.CodigoRubrica,
            DescricaoRubrica = r.Rubrica?.RubricaDescricao,
            ValorGerado = r.Valor?.ToString("F2").Replace(".", ",") ?? r.Percentual?.ToString("F2").Replace(".", ","),
            ProcessamentoInfo = new RubricaColaboradorProcessamentoInfo
            {
                IdRubricaColaboradorJaExistente = Guid.TryParse(r.Id, out var id) ? id : null,
                CodigoInternoColaborador = Guid.TryParse(r.CodigInternoColaborador, out var codigInternoColaborador) ? codigInternoColaborador : null,
                
            }
        }).ToList() ?? new List<RubricaColaboradorLinhaArquivoDTO>();
        return rubricasExistentesDB;
    }

    private async Task VerificarConflitosLinhaEspecifica(RubricaColaboradorLinhaArquivoDTO linha, RubricaCargaMassivaResult resultado, int orgId, PoliticaConflitoCarga politicaConflito, List<RubricaColaboradorLinhaArquivoDTO> linhasParaRemover, List<RubricaColaboradorLinhaArquivoDTO> rubricasExistentesDB, int mesVigencia, int anoVigencia)
    {
      
        var rubricaExistente = rubricasExistentesDB.FirstOrDefault(rubricaExistente => 
            CompararRubricaComLinha(rubricaExistente, linha, mesVigencia, anoVigencia));

        if (rubricaExistente != null)
        {
            var conflitos = IdentificarConflitos(rubricaExistente, linha);
            
            if (conflitos.Count == 0)
            {
                resultado.MensagensAlerta.Add($"Linha {linha.NumeroLinha}: Registro duplicado já existe no sistema");
                resultado.LinhasDuplicadas++;
                AdicionarItemProcessado(resultado, linha, "Duplicado", "Registro duplicado já existe no sistema", 
                    rubricaExistente.ProcessamentoInfo.IdRubricaColaboradorJaExistente?.ToString());
                linhasParaRemover.Add(linha);
            }
            else
            {
                if (politicaConflito == PoliticaConflitoCarga.NaoAtualizar)
                {
                    var conflitosTexto = string.Join(", ", conflitos);
                    resultado.MensagensAlerta.Add($"Linha {linha.NumeroLinha}: Registro já existe com diferenças ({conflitosTexto}) e não será atualizado (política: não atualizar)");
                    resultado.LinhasDuplicadas++;
                    AdicionarItemProcessado(resultado, linha, "Duplicado", $"Registro já existe com diferenças ({conflitosTexto}) e não será atualizado",
                        rubricaExistente.ProcessamentoInfo.IdRubricaColaboradorJaExistente?.ToString());
                    linhasParaRemover.Add(linha);
                }
                else if (politicaConflito == PoliticaConflitoCarga.AtualizarTodos)
                {
                    var conflitosTexto = string.Join(", ", conflitos);
                    resultado.MensagensAlerta.Add($"Linha {linha.NumeroLinha}: Registro existente será atualizado - diferenças: {conflitosTexto}");
                    linha.ProcessamentoInfo.IdRubricaColaboradorJaExistente = rubricaExistente.ProcessamentoInfo.IdRubricaColaboradorJaExistente;
                }
            }
        }
    }



    private bool CompararRubricaComLinha(RubricaColaboradorLinhaArquivoDTO rubricaExistente, RubricaColaboradorLinhaArquivoDTO linha, int mes, int ano)
    {
        var (mesExistente, anoExistente) = ExtrairMesAnoVigencia(rubricaExistente.Vigencia);
        
        var codigoInternoExistente = rubricaExistente.ProcessamentoInfo.CodigoInternoColaborador;
        var codigoInternoLinha = linha.ProcessamentoInfo.CodigoInternoColaborador?.ToString();

        if (codigoInternoExistente.ToStringOuVazio() == "c26d1dc7-a494-4fdf-8fad-d8a15f3a3e71")
        {
            int i = 0;
        }

        return codigoInternoExistente.ToStringOuVazio() == codigoInternoLinha &&
               rubricaExistente.CodRubrica == linha.CodRubrica &&
               mesExistente == mes &&
               anoExistente == ano;
    }

    private List<string> IdentificarConflitos(RubricaColaboradorLinhaArquivoDTO rubricaExistente, RubricaColaboradorLinhaArquivoDTO linhaNova)
    {
        var conflitos = new List<string>();

        if (rubricaExistente.ValorGerado.ToDecimalOuZero() != linhaNova.ValorGerado.ToDecimalOuZero())
            conflitos.Add($"Valor: {rubricaExistente.ValorGerado} -> {linhaNova.ValorGerado}");

        //if (rubricaExistente.CodRubrica != linhaNova.DescricaoRubrica)
        //    conflitos.Add($"Descrição: {rubricaExistente.DescricaoRubrica} -> {linhaNova.DescricaoRubrica}");
        //if (rubricaExistente.CPF != linhaNova.CPF)
        //    conflitos.Add($"CPF: {rubricaExistente.CPF} -> {linhaNova.CPF}");

        return conflitos;
    }

    private async Task ProcessarRubricaColaborador(List<RubricaColaboradorLinhaArquivoDTO> linhas, RubricaCargaMassivaResult resultado, int orgId)
    {
        foreach (var linha in linhas)
        {
            var logItem = new RubricaCargaMassivaItemLogDTO
            {
                CodigoCargaRubrica = resultado.CargaId,
                JsonItemTentativa = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    linha.NumeroLinha,
                    linha.Vigencia,
                    linha.NomeColaborador,
                    linha.CPF,
                    linha.CodRubrica,
                    linha.DescricaoRubrica,
                    linha.ValorGerado
                }),
                StatusProcessamento = StatusProcessamentoItemEnum.Sucesso, 
                TipoIdentificacao = TipoIdentificacaoEnum.CpfColaborador,
                CodigoInternoColaborador = linha.ProcessamentoInfo.CodigoInternoColaborador?.ToString(),
                DataProcessamento = DateTime.Now
            };

            try
            {
                var rubricaInput = new RubricaColaboradorInput();
                //rubricaInput.ConfigurarParaPersistencia(orgId, cpfRequest, Guid.NewGuid());

                rubricaInput.RubricaId = linha.ProcessamentoInfo.RubricaId.Value;
                rubricaInput.CodigoInternoColaborador = linha.ProcessamentoInfo.CodigoInternoColaborador.Value;
                rubricaInput.CodigoRubricaFrequencia = "UNICA";
                rubricaInput.MesInicial = linha.ProcessamentoInfo.MesVigencia;
                rubricaInput.AnoInicial = linha.ProcessamentoInfo.AnoVigencia;
                rubricaInput.MesFinal = null;
                rubricaInput.AnoFinal = null;
                rubricaInput.Hora = null;
                rubricaInput.CodigoCargaRubrica = resultado.CargaId;

                // Usar valor já tratado na validação
                rubricaInput.Valor = linha.ProcessamentoInfo.ValorTratado.Value;
                rubricaInput.Percentual = 0;

                ApiGenericResult<RubricaColaboradorResult> resultadoOperacao;

                // Verificar se é atualização ou inserção
                if (linha.ProcessamentoInfo.IdRubricaColaboradorJaExistente.HasValue)
                {
                    rubricaInput.Observacao = $"Atualizado via carga massiva - Carga: {resultado.CargaId}";

                    resultadoOperacao = await _rubricaColaboradorService.AtualizarRubricaColaborador(rubricaInput, linha.ProcessamentoInfo.IdRubricaColaboradorJaExistente.Value, null, orgId, false);
                    logItem.TbRubricaColaboradorId = linha.ProcessamentoInfo.IdRubricaColaboradorJaExistente.Value.ToString();
                    
                    if (resultadoOperacao?.Sucesso == true)
                    {
                        AdicionarItemProcessado(resultado, linha, "Atualizado", "Registro atualizado com sucesso",
                            linha.ProcessamentoInfo.IdRubricaColaboradorJaExistente.Value.ToString());
                    }
                }
                else
                {
                    rubricaInput.Observacao = $"Carregado via carga massiva - Carga: {resultado.CargaId}";
                    resultadoOperacao =
                        await _rubricaColaboradorService.InserirRubricaColaborador(rubricaInput, null, orgId, true, false);
                    logItem.TbRubricaColaboradorId = resultadoOperacao?.Retorno.Id.ToStringOuVazio();
                    
                    if (resultadoOperacao?.Sucesso == true)
                    {
                        AdicionarItemProcessado(resultado, linha, "Sucesso", "Registro inserido com sucesso",
                            resultadoOperacao?.Retorno.Id.ToStringOuVazio());
                    }
                }
            }
            catch (ArgumentException ex)
            {
                logItem.StatusProcessamento = StatusProcessamentoItemEnum.ErroSalvamento;
                logItem.MensagemErro = ex.Message;
                
                resultado.MensagensErro.Add($"Linha {linha.NumeroLinha}: Erro ao processar - {ex.Message}");
                resultado.LinhasComErro++;
            }
            catch (Exception ex)
            {
                // Atualizar log item com erro
                logItem.StatusProcessamento = StatusProcessamentoItemEnum.ErroSalvamento;
                logItem.MensagemErro = ex.Message;
                
                resultado.MensagensErro.Add($"Linha {linha.NumeroLinha}: Erro ao processar - {ex.Message}");
                resultado.LinhasComErro++;
                
                // Adicionar ao resultado como erro
                AdicionarItemProcessado(resultado, linha, "Erro", $"Erro ao processar - {ex.Message}");
            }
            finally
            {
                // Salvar log do item
                await _rubricaCargaMassivaLogRepository.InserirLogItem(logItem);
            }
        }
    }

    private List<RubricaColaboradorLinhaArquivoDTO> ProcessarArquivoCSV(byte[] arquivoBytes)
    {
        var linhas = new List<RubricaColaboradorLinhaArquivoDTO>();
        var conteudo = Encoding.UTF8.GetString(arquivoBytes);
        var linhasArquivo = conteudo.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        // Cabeçalho esperado
        string cabecalhoEsperado = "VIGENCIA;NOME;CPF;COD_RUBRICA;DESCRICAO;VALOR GERADO";

        // Validar se arquivo tem cabeçalho correto
        if (linhasArquivo.Length == 0)
        {
            throw new ArgumentException("Arquivo está vazio");
        }

        if (!linhasArquivo[0].Trim().Equals(cabecalhoEsperado, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"Cabeçalho do arquivo está incorreto. Esperado: '{cabecalhoEsperado}'. Encontrado: '{linhasArquivo[0].Trim()}'");
        }

        int startIndex = 1; // pular cabeçalho

        for (int i = startIndex; i < linhasArquivo.Length; i++)
        {
            var colunas = linhasArquivo[i].Split(';');

            var linha = new RubricaColaboradorLinhaArquivoDTO
            {
                NumeroLinha = i + 1,
                ProcessamentoInfo = new RubricaColaboradorProcessamentoInfo
                {
                    LinhaStringOriginal = linhasArquivo[i]
                }
            };

            if (colunas.Length != 6)
            {
                linha.ProcessamentoInfo.ErroFormatacao = $"Linha deve conter exatamente 6 campos separados por ponto e vírgula, encontrados {colunas.Length} campos";
            }
            else
            {
                linha.Vigencia = colunas[0].Trim().Trim('"');
                linha.NomeColaborador = colunas[1].Trim().Trim('"');
                linha.CPF = colunas[2].Trim().Trim('"');
                linha.CodRubrica = colunas[3].Trim().Trim('"');
                linha.DescricaoRubrica = colunas[4].Trim().Trim('"');
                linha.ValorGerado = colunas[5].Trim().Trim('"');
            }

            linhas.Add(linha);
        }

        return linhas;
    }

    public bool ValidarPreenchimentoLinha(RubricaColaboradorLinhaArquivoDTO linha, out string mensagemErro)
    {
        mensagemErro = "";

        if (string.IsNullOrWhiteSpace(linha.Vigencia))
        {
            mensagemErro = "Vigência é obrigatória";
            return false;
        }

        if (string.IsNullOrWhiteSpace(linha.CPF))
        {
            mensagemErro = "CPF é obrigatório";
            return false;
        }

        if (string.IsNullOrWhiteSpace(linha.CodRubrica))
        {
            mensagemErro = "Código da Rubrica é obrigatório";
            return false;
        }

        if (string.IsNullOrWhiteSpace(linha.ValorGerado))
        {
            mensagemErro = "Valor é obrigatório";
            return false;
        }

        // Validar formato da vigência (MM/YYYY)
        if (!ValidarFormatoVigencia(linha.Vigencia))
        {
            mensagemErro = "Formato da vigência deve ser MM/YYYY";
            return false;
        }

        // Validar se valor é numérico
        decimal valor;
        if (!decimal.TryParse(linha.ValorGerado, NumberStyles.Number, new CultureInfo("pt-BR"), out valor))
        {
            mensagemErro = "Valor deve ser numérico";
            return false;
        }

        return true;
    }

    private bool ValidarFormatoVigencia(string vigencia)
    {
        try
        {
            var partes = vigencia.Split('/');
            if (partes.Length != 2) return false;

            if (!int.TryParse(partes[0], out int mes) || mes < 1 || mes > 12) return false;
            if (!int.TryParse(partes[1], out int ano) || ano < 1000 || ano > 9999) return false;

            return true;
        }
        catch
        {
            return false;
        }
    }


    public static (int mes, int ano) ExtrairMesAnoVigencia(string vigencia)
    {
        var partes = vigencia.Split('/');
        return (int.Parse(partes[0]), int.Parse(partes[1]));
    }

    public async Task<string> ConverterJsonParaCSV(List<RubricaCargaJsonItemDTO> rubricasJson)
    {
        var csvContent = new StringBuilder();
        
        // Cabeçalho do CSV conforme formato esperado
        csvContent.AppendLine("VIGENCIA;NOME;CPF;COD_RUBRICA;DESCRICAO;VALOR GERADO");
        
        // Converter cada item JSON para linha CSV
        foreach (var item in rubricasJson)
        {
            var linha = $"{EscaparCampoCSV(item.Vigencia)};{EscaparCampoCSV(item.Nome)};{EscaparCampoCSV(item.Cpf)};{EscaparCampoCSV(item.CodRubrica)};{EscaparCampoCSV(item.Descricao)};{EscaparCampoCSV(item.ValorGerado)}";
            csvContent.AppendLine(linha);
        }
        
        // Converter para bytes e depois para Base64
        var csvBytes = Encoding.UTF8.GetBytes(csvContent.ToString());
        return Convert.ToBase64String(csvBytes);
    }

    private string EscaparCampoCSV(string campo)
    {
        if (string.IsNullOrEmpty(campo))
            return "";

        // Se o campo contém caracteres especiais (;, ", \n, \r), colocar entre aspas
        if (campo.Contains(';') || campo.Contains('"') || campo.Contains('\n') || campo.Contains('\r'))
        {
            // Escapar aspas duplicando-as
            campo = campo.Replace("\"", "\"\"");
            return $"\"{campo}\"";
        }

        return campo;
    }

    private void AdicionarItemProcessado(RubricaCargaMassivaResult resultado, RubricaColaboradorLinhaArquivoDTO linha, string status, string mensagemStatus, string idRubricaColaborador = null)
    {
        resultado.ItensProcessados.Add(new RubricaCargaItemProcessadoDTO
        {
            NumeroLinha = linha.NumeroLinha,
            Vigencia = linha.Vigencia,
            NomeColaborador = linha.NomeColaborador,
            CPF = linha.CPF,
            CodRubrica = linha.CodRubrica,
            DescricaoRubrica = linha.DescricaoRubrica,
            ValorGerado = linha.ValorGerado,
            Status = status,
            MensagemStatus = mensagemStatus,
            IdRubricaColaborador = idRubricaColaborador
        });
    }

}