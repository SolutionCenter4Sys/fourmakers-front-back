using System.Collections.Generic;
using System.Linq;

namespace Financeiro.Domain.Impl.IntegracaoBancaria.RetornoBancaria
{
    public class ParseCnab240RetornoService
    {
        public class SegmentoRetorno
        {
            public int NumeroLote { get; set; }
            public int NumeroSequencial { get; set; }
            public string TipoRegistro { get; set; }
            public string TipoRegistroDescritivo { get; set; }
            public string Segmento { get; set; }
            public string CodigoOcorrencia { get; set; }
            public List<string> ListaOcorrencias { get; set; }
            public string LinhaCompleta { get; set; }
        }

        
        public class OcorrenciaInfo
        {
            public string Codigo { get; set; } = "";
            public string Descricao { get; set; } = "";
            public TipoOcorrencia Tipo { get; set; }
        }

        public enum TipoOcorrencia
        {
            SUCESSO,
            ERRO,
            AVISO,
            AGENDADO
        }

        // Mapeamento completo dos códigos de ocorrência CNAB 240 Itaú V085
        private static readonly Dictionary<string, OcorrenciaInfo> _ocorrenciasItauV085 = new Dictionary<string, OcorrenciaInfo>
        {
            { "00", new OcorrenciaInfo { Codigo = "00", Descricao = "PAGAMENTO EFETUADO", Tipo = TipoOcorrencia.SUCESSO } },
            { "AE", new OcorrenciaInfo { Codigo = "AE", Descricao = "DATA DE PAGAMENTO ALTERADA", Tipo = TipoOcorrencia.AVISO } },
            { "AG", new OcorrenciaInfo { Codigo = "AG", Descricao = "NÚMERO DO LOTE INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "AH", new OcorrenciaInfo { Codigo = "AH", Descricao = "NÚMERO SEQUENCIAL DO REGISTRO NO LOTE INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "AI", new OcorrenciaInfo { Codigo = "AI", Descricao = "PRODUTO DEMONSTRATIVO DE PAGAMENTO NÃO CONTRATADO", Tipo = TipoOcorrencia.ERRO } },
            { "AJ", new OcorrenciaInfo { Codigo = "AJ", Descricao = "TIPO DE MOVIMENTO INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "AL", new OcorrenciaInfo { Codigo = "AL", Descricao = "CÓDIGO DO BANCO FAVORECIDO INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "AM", new OcorrenciaInfo { Codigo = "AM", Descricao = "AGÊNCIA DO FAVORECIDO INVÁLIDA", Tipo = TipoOcorrencia.ERRO } },
            { "AN", new OcorrenciaInfo { Codigo = "AN", Descricao = "CONTA CORRENTE DO FAVORECIDO INVÁLIDA", Tipo = TipoOcorrencia.ERRO } },
            { "AO", new OcorrenciaInfo { Codigo = "AO", Descricao = "NOME DO FAVORECIDO INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "AP", new OcorrenciaInfo { Codigo = "AP", Descricao = "DATA DE PAGAMENTO / DATA DE VALIDADE / HORA DE LANÇAMENTO INVÁLIDA", Tipo = TipoOcorrencia.ERRO } },
            { "AQ", new OcorrenciaInfo { Codigo = "AQ", Descricao = "QUANTIDADE DE REGISTROS MAIOR QUE 999999", Tipo = TipoOcorrencia.ERRO } },
            { "AR", new OcorrenciaInfo { Codigo = "AR", Descricao = "VALOR ARRECADADO / LANÇAMENTO INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "BC", new OcorrenciaInfo { Codigo = "BC", Descricao = "NOSSO NÚMERO INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "BD", new OcorrenciaInfo { Codigo = "BD", Descricao = "PAGAMENTO AGENDADO", Tipo = TipoOcorrencia.AGENDADO } },
            { "BE", new OcorrenciaInfo { Codigo = "BE", Descricao = "PAGAMENTO AGENDADO COM FORMA ALTERADA PARA OP", Tipo = TipoOcorrencia.AGENDADO } },
            { "BI", new OcorrenciaInfo { Codigo = "BI", Descricao = "CNPJ/CPF DO FAVORECIDO INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "BL", new OcorrenciaInfo { Codigo = "BL", Descricao = "VALOR DA PARCELA INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "CD", new OcorrenciaInfo { Codigo = "CD", Descricao = "CNPJ/CPF INFORMADO DIVERGENTE DO CADASTRADO", Tipo = TipoOcorrencia.ERRO } },
            { "CE", new OcorrenciaInfo { Codigo = "CE", Descricao = "PAGAMENTO CANCELADO", Tipo = TipoOcorrencia.ERRO } },
            { "CF", new OcorrenciaInfo { Codigo = "CF", Descricao = "VALOR DO DOCUMENTO INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "CG", new OcorrenciaInfo { Codigo = "CG", Descricao = "VALOR DO ABATIMENTO INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "CH", new OcorrenciaInfo { Codigo = "CH", Descricao = "VALOR DO DESCONTO INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "CI", new OcorrenciaInfo { Codigo = "CI", Descricao = "CNPJ/CPF/IDENTIFICADOR INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "CJ", new OcorrenciaInfo { Codigo = "CJ", Descricao = "VALOR DA MULTA INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "CK", new OcorrenciaInfo { Codigo = "CK", Descricao = "TIPO DE INSCRIÇÃO INVÁLIDA", Tipo = TipoOcorrencia.ERRO } },
            { "CN", new OcorrenciaInfo { Codigo = "CN", Descricao = "CONTA NÃO CADASTRADA", Tipo = TipoOcorrencia.ERRO } },
            { "CP", new OcorrenciaInfo { Codigo = "CP", Descricao = "CONFIRMAÇÃO DE OP CUMPRIDA", Tipo = TipoOcorrencia.SUCESSO } },
            { "DM", new OcorrenciaInfo { Codigo = "DM", Descricao = "E-MAIL DO FAVORECIDO INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "DV", new OcorrenciaInfo { Codigo = "DV", Descricao = "DOC/TED DEVOLVIDO PELO BANCO FAVORECIDO", Tipo = TipoOcorrencia.ERRO } },
            { "EM", new OcorrenciaInfo { Codigo = "EM", Descricao = "CONFIRMAÇÃO DE OP EMITIDA", Tipo = TipoOcorrencia.SUCESSO } },
            { "EX", new OcorrenciaInfo { Codigo = "EX", Descricao = "DEVOLUÇÃO DE OP NÃO SACADA PELO FAVORECIDO", Tipo = TipoOcorrencia.ERRO } },
            { "FC", new OcorrenciaInfo { Codigo = "FC", Descricao = "PAGAMENTO EFETUADO ATRAVÉS DE FINANCIAMENTO COMPROR", Tipo = TipoOcorrencia.SUCESSO } },
            { "FD", new OcorrenciaInfo { Codigo = "FD", Descricao = "PAGAMENTO EFETUADO ATRAVÉS DE FINANCIAMENTO DESCOMPROR", Tipo = TipoOcorrencia.SUCESSO } },
            { "HÁ", new OcorrenciaInfo { Codigo = "HÁ", Descricao = "ERRO NO LOTE", Tipo = TipoOcorrencia.ERRO } },
            { "HM", new OcorrenciaInfo { Codigo = "HM", Descricao = "ERRO NO REGISTRO HEADER DE ARQUIVO", Tipo = TipoOcorrencia.ERRO } },
            { "II", new OcorrenciaInfo { Codigo = "II", Descricao = "DATA DE VENCIMENTO INVÁLIDA / QR CODE EXPIRADO", Tipo = TipoOcorrencia.ERRO } },
            { "IJ", new OcorrenciaInfo { Codigo = "IJ", Descricao = "COMPETÊNCIA / PERÍODO REFERÊNCIA / PARCELA INVÁLIDA", Tipo = TipoOcorrencia.ERRO } },
            { "IK", new OcorrenciaInfo { Codigo = "IK", Descricao = "TRIBUTO NÃO LIQUIDÁVEL VIA SISPAG", Tipo = TipoOcorrencia.ERRO } },
            { "IM", new OcorrenciaInfo { Codigo = "IM", Descricao = "TIPO X FORMA NÃO COMPATÍVEL", Tipo = TipoOcorrencia.ERRO } },
            { "IN", new OcorrenciaInfo { Codigo = "IN", Descricao = "BANCO/AGÊNCIA NÃO CADASTRADOS", Tipo = TipoOcorrencia.ERRO } },
            { "IO", new OcorrenciaInfo { Codigo = "IO", Descricao = "DAC/VALOR/COMPETÊNCIA INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "IP", new OcorrenciaInfo { Codigo = "IP", Descricao = "DAC DO CÓDIGO DE BARRAS INVÁLIDO", Tipo = TipoOcorrencia.ERRO } },
            { "IR", new OcorrenciaInfo { Codigo = "IR", Descricao = "PAGAMENTO ALTERADO", Tipo = TipoOcorrencia.AVISO } },
            { "IS", new OcorrenciaInfo { Codigo = "IS", Descricao = "CONCESSIONÁRIA NÃO CONVENIADA COM ITAÚ", Tipo = TipoOcorrencia.ERRO } },
            { "LA", new OcorrenciaInfo { Codigo = "LA", Descricao = "DATA DE PAGAMENTO DE UM LOTE ALTERADA", Tipo = TipoOcorrencia.AVISO } },
            { "LC", new OcorrenciaInfo { Codigo = "LC", Descricao = "LOTE DE PAGAMENTOS CANCELADO", Tipo = TipoOcorrencia.ERRO } },
            { "NA", new OcorrenciaInfo { Codigo = "NA", Descricao = "PAGAMENTO CANCELADO POR FALTA DE AUTORIZAÇÃO", Tipo = TipoOcorrencia.ERRO } },
            { "NR", new OcorrenciaInfo { Codigo = "NR", Descricao = "OPERAÇÃO NÃO REALIZADA", Tipo = TipoOcorrencia.ERRO } },
            { "PD", new OcorrenciaInfo { Codigo = "PD", Descricao = "AQUISIÇÃO CONFIRMADA", Tipo = TipoOcorrencia.SUCESSO } },
            { "RJ", new OcorrenciaInfo { Codigo = "RJ", Descricao = "REGISTRO REJEITADO", Tipo = TipoOcorrencia.ERRO } },
            { "RS", new OcorrenciaInfo { Codigo = "RS", Descricao = "PAGAMENTO DISPONÍVEL PARA ANTECIPAÇÃO NO RISCO SACADO", Tipo = TipoOcorrencia.AVISO } }
        };

        /// <summary>
        /// Valida se o documento CNAB 240 está em formato válido
        /// </summary>
        /// <param name="conteudoArquivo">Conteúdo do arquivo CNAB 240</param>
        /// <exception cref="ApplicationException">Lançada quando o documento é inválido</exception>
        public static void ValidarDocumento(string conteudoArquivo)
        {
            if (string.IsNullOrWhiteSpace(conteudoArquivo))
            {
                throw new ApplicationException("Arquivo CNAB está vazio ou nulo.");
            }

            var linhas = conteudoArquivo.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            
            if (linhas.Length == 0)
            {
                throw new ApplicationException("Arquivo CNAB não contém linhas válidas.");
            }

            // Validar que todas as linhas têm pelo menos 240 caracteres
            var linhasInvalidas = new List<int>();
            for (int i = 0; i < linhas.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(linhas[i]) && linhas[i].Length < 240)
                {
                    linhasInvalidas.Add(i + 1);
                }
            }

            if (linhasInvalidas.Any())
            {
                throw new ApplicationException(
                    $"Arquivo CNAB contém linhas com tamanho inválido (menor que 240 caracteres). " +
                    $"Linhas inválidas: {string.Join(", ", linhasInvalidas)}");
            }

            // Validar estrutura básica do arquivo CNAB 240
            var linhasValidas = linhas.Where(l => !string.IsNullOrWhiteSpace(l) && l.Length >= 240).ToList();
            
            if (linhasValidas.Count == 0)
            {
                throw new ApplicationException("Arquivo CNAB não contém linhas válidas com 240 caracteres.");
            }

            // Validar Header de Arquivo (primeira linha deve ser tipo 0)
            var primeiraLinha = linhasValidas.First();
            var tipoRegistroPrimeira = primeiraLinha.Length >= 8 ? primeiraLinha.Substring(7, 1) : "";
            
            if (tipoRegistroPrimeira != "0")
            {
                throw new ApplicationException(
                    $"Arquivo CNAB inválido: primeira linha deve ser Header de Arquivo (tipo 0), mas encontrado tipo {tipoRegistroPrimeira}.");
            }

            // Validar Trailer de Arquivo (última linha deve ser tipo 9)
            var ultimaLinha = linhasValidas.Last();
            var tipoRegistroUltima = ultimaLinha.Length >= 8 ? ultimaLinha.Substring(7, 1) : "";
            
            if (tipoRegistroUltima != "9")
            {
                throw new ApplicationException(
                    $"Arquivo CNAB inválido: última linha deve ser Trailer de Arquivo (tipo 9), mas encontrado tipo {tipoRegistroUltima}.");
            }

            // Validar que existe pelo menos um Header de Lote (tipo 1)
            var temHeaderLote = linhasValidas.Any(l => l.Length >= 8 && l.Substring(7, 1) == "1");
            if (!temHeaderLote)
            {
                throw new ApplicationException("Arquivo CNAB inválido: não foi encontrado nenhum Header de Lote (tipo 1).");
            }

            // Validar que existe pelo menos um Trailer de Lote (tipo 5)
            var temTrailerLote = linhasValidas.Any(l => l.Length >= 8 && l.Substring(7, 1) == "5");
            if (!temTrailerLote)
            {
                throw new ApplicationException("Arquivo CNAB inválido: não foi encontrado nenhum Trailer de Lote (tipo 5).");
            }

            // Validar que existe pelo menos um registro de detalhe (tipo 3)
            var temDetalhe = linhasValidas.Any(l => l.Length >= 8 && l.Substring(7, 1) == "3");
            if (!temDetalhe)
            {
                throw new ApplicationException("Arquivo CNAB inválido: não foi encontrado nenhum registro de detalhe (tipo 3).");
            }
        }

        public List<SegmentoRetorno> ParsearArquivoRetorno(string conteudoArquivo)
        {
            var segmentos = new List<SegmentoRetorno>();
            var linhas = conteudoArquivo.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (var linha in linhas)
            {
                if (string.IsNullOrWhiteSpace(linha) || linha.Length < 240)
                    continue;

                var tipoRegistro = linha.Substring(7, 1);
                var tipoRegistroDescritivo = ObterTipoRegistroDescritivo(linha, tipoRegistro);

                int numeroLote = 0;
                int numeroSequencial = 0;
                string segmento = "";
                string codigoOcorrencia = "";
                var listaOcorrencias = new List<string>();

                // Tipo 0 = Header de Arquivo
                // Tipo 1 = Header de Lote
                // Tipo 3 = Detalhe (Segmento A, B, etc.)
                // Tipo 5 = Trailer de Lote
                // Tipo 9 = Trailer de Arquivo

                if (tipoRegistro == "3")
                {
                    // Registros de detalhe
                    segmento = linha.Substring(13, 1);
                    numeroLote = int.TryParse(linha.Substring(3, 4).Trim(), out var lote) ? lote : 0;
                    numeroSequencial = int.TryParse(linha.Substring(8, 5).Trim(), out var seq) ? seq : 0;

                    // Posição 230-239: campo de ocorrências (até 5 ocorrências de 2 dígitos cada)
                    // Extrair diretamente das posições 230-231 (primeira ocorrência) - índice 229-230
                    if (linha.Length >= 231)
                    {
                        var codigoOcorrenciaRaw = linha.Substring(229, 2);
                        // Normalizar: remover espaços e converter para maiúsculo
                        codigoOcorrencia = codigoOcorrenciaRaw.Trim().ToUpper();
                        
                        // Se houver código válido (exatamente 2 caracteres alfanuméricos), adicionar à lista
                        if (!string.IsNullOrWhiteSpace(codigoOcorrencia) && codigoOcorrencia.Length == 2)
                        {
                            listaOcorrencias.Add(codigoOcorrencia);
                        }
                        else
                        {
                            // Se não encontrou código válido, usar vazio
                            codigoOcorrencia = "";
                        }
                        
                        // Extrair outras ocorrências (posições 232-239) se necessário
                        if (linha.Length >= 240)
                        {
                            var campoOcorrenciasRestantes = linha.Substring(231, 8);
                            for (int i = 0; i < campoOcorrenciasRestantes.Length; i += 2)
                            {
                                if (i + 1 < campoOcorrenciasRestantes.Length)
                                {
                                    var ocorrenciaRaw = campoOcorrenciasRestantes.Substring(i, 2);
                                    var ocorrencia = ocorrenciaRaw.Trim().ToUpper();
                                    if (!string.IsNullOrWhiteSpace(ocorrencia) && ocorrencia.Length == 2)
                                    {
                                        listaOcorrencias.Add(ocorrencia);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        codigoOcorrencia = "";
                    }
                }
                else if (tipoRegistro == "1")
                {
                    // Header de lote
                    numeroLote = int.TryParse(linha.Substring(3, 4).Trim(), out var lote) ? lote : 0;
                }

                segmentos.Add(new SegmentoRetorno
                {
                    NumeroLote = numeroLote,
                    NumeroSequencial = numeroSequencial,
                    TipoRegistro = tipoRegistro,
                    TipoRegistroDescritivo = tipoRegistroDescritivo,
                    Segmento = segmento,
                    CodigoOcorrencia = codigoOcorrencia,
                    ListaOcorrencias = listaOcorrencias,
                    LinhaCompleta = linha
                });
            }

            return segmentos;
        }

        private string ObterTipoRegistroDescritivo(string linha, string tipoRegistro)
        {
            return tipoRegistro switch
            {
                "0" => "HEADER_ARQUIVO",
                "1" => "HEADER_LOTE",
                "3" => $"DETALHE_SEGMENTO_{linha.Substring(13, 1)}",
                "5" => "TRAILER_LOTE",
                "9" => "TRAILER_ARQUIVO",
                _ => "DESCONHECIDO"
            };
        }

        public List<SegmentoRetorno> ObterSegmentosA(List<SegmentoRetorno> segmentos)
        {
            return segmentos.Where(s => s.Segmento == "A").OrderBy(s => s.NumeroSequencial).ToList();
        }

        /// <summary>
        /// Obtém informações sobre uma ocorrência CNAB pelo código
        /// </summary>
        /// <param name="codigo">Código da ocorrência</param>
        /// <param name="modeloCnab">Modelo CNAB (ex: CNAB240_ITAU_V085). Se null, usa padrão Itaú</param>
        public static OcorrenciaInfo ObterOcorrencia(string codigo, string modeloCnab = null)
        {
            // Normalizar código: remover espaços e converter para maiúsculo
            var codigoNormalizado = codigo?.Trim().ToUpper() ?? "";
            
            if (string.IsNullOrWhiteSpace(codigoNormalizado))
                return new OcorrenciaInfo 
                { 
                    Codigo = "", 
                    Descricao = "CÓDIGO DE OCORRÊNCIA NÃO INFORMADO", 
                    Tipo = TipoOcorrencia.ERRO 
                };

            // Por enquanto, usa apenas Itaú V085. Futuramente pode ser expandido para outros bancos
            if (_ocorrenciasItauV085.TryGetValue(codigoNormalizado, out var ocorrencia))
                return ocorrencia;

            return new OcorrenciaInfo 
            { 
                Codigo = codigoNormalizado, 
                Descricao = $"CÓDIGO DE OCORRÊNCIA DESCONHECIDO: {codigoNormalizado}", 
                Tipo = TipoOcorrencia.ERRO 
            };
        }

        /// <summary>
        /// Verifica se o código de ocorrência indica sucesso no pagamento
        /// </summary>
        public static bool EhSucesso(string codigo, string modeloCnab = null)
        {
            var ocorrencia = ObterOcorrencia(codigo, modeloCnab);
            return ocorrencia.Tipo == TipoOcorrencia.SUCESSO;
        }

        /// <summary>
        /// Verifica se o código de ocorrência indica erro no pagamento
        /// </summary>
        public static bool EhErro(string codigo, string modeloCnab = null)
        {
            var ocorrencia = ObterOcorrencia(codigo, modeloCnab);
            return ocorrencia.Tipo == TipoOcorrencia.ERRO;
        }

        /// <summary>
        /// Verifica se o código de ocorrência indica aviso
        /// </summary>
        public static bool EhAviso(string codigo, string modeloCnab = null)
        {
            var ocorrencia = ObterOcorrencia(codigo, modeloCnab);
            return ocorrencia.Tipo == TipoOcorrencia.AVISO;
        }

        /// <summary>
        /// Verifica se o código de ocorrência indica pagamento agendado
        /// </summary>
        public static bool EhAgendado(string codigo, string modeloCnab = null)
        {
            var ocorrencia = ObterOcorrencia(codigo, modeloCnab);
            return ocorrencia.Tipo == TipoOcorrencia.AGENDADO;
        }
    }
    
}
