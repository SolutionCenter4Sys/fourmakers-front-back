using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colaboracao.Helper;

namespace Financeiro.Domain.Impl.IntegracaoBancaria.RemessaBancaria
{
    public class GerarCnab240Service
    {
        // Método principal que orquestra a geração do arquivo CNAB240 completo
        public List<string> GerarArquivoCnab240(CnabOrgResult cnabOrg, List<ColaboradorPagamentoAgrupadoDTO> pagamentos, DateTime dataGeracao)
        {
            var linhasArquivo = new List<string>();
            int numeroSequencialRegistro = 1;
            int numeroLote = 1;

            // Header de Arquivo
            linhasArquivo.Add(GerarHeaderArquivo(cnabOrg, 1, dataGeracao));
            numeroSequencialRegistro++;

            // Header de Lote
            linhasArquivo.Add(GerarHeaderLote(cnabOrg, numeroLote, dataGeracao));
            numeroSequencialRegistro++;

            int numeroRegistroLote = 0;
            decimal valorTotalLote = 0;

            // Detalhes (Segmentos A e B) para cada pagamento
            foreach (var pagamento in pagamentos)
            {
                bool isPix = pagamento.DadosBancarios?.FormaPagamento?.ToUpper() == "PIX";

                // Segmento A (obrigatório para todos)
                linhasArquivo.Add(GerarDetalheSegmentoA(cnabOrg, pagamento, numeroLote, numeroRegistroLote, dataGeracao));
                numeroSequencialRegistro++;
                numeroRegistroLote++;

                // Segmento B (obrigatório apenas para PIX)
                if (isPix)
                {
                    linhasArquivo.Add(GerarDetalheSegmentoB(cnabOrg, pagamento, numeroLote, numeroRegistroLote));
                }

                valorTotalLote += pagamento.ValorTotal;
            }

            // Trailer de Lote (Header Lote + Detalhes + Trailer Lote)
            // numeroRegistroLote = próximo número sequencial = 1 + qtd de Segmentos
            // Total do lote = 1 (Header) + qtd Segmentos + 1 (Trailer) = numeroRegistroLote + 1
            int quantidadeRegistrosLote = numeroRegistroLote;
            linhasArquivo.Add(GerarTrailerLote(cnabOrg, numeroLote, quantidadeRegistrosLote, valorTotalLote));
            numeroSequencialRegistro++;

            // Trailer de Arquivo (Header Arquivo + Lotes + Trailer Arquivo)
            // numeroSequencialRegistro já representa o número do próximo registro (incluindo o trailer)
            int quantidadeRegistrosArquivo = numeroSequencialRegistro;
            linhasArquivo.Add(GerarTrailerArquivo(cnabOrg, 1, quantidadeRegistrosArquivo));
            linhasArquivo.Add("");

            return linhasArquivo;
        }

        // Métodos auxiliares para formatação
        private string FormatarTexto(string texto, int tamanho, char preenchimento = ' ', bool alinharEsquerda = true)
        {
            if (string.IsNullOrEmpty(texto))
                texto = "";

            if (texto.Length > tamanho)
                return texto.Substring(0, tamanho);

            if (alinharEsquerda)
                return texto.PadRight(tamanho, preenchimento);
            else
                return texto.PadLeft(tamanho, preenchimento);
        }

        private string FormatarNumero(long numero, int tamanho, char preenchimento = '0')
        {
            return numero.ToString().PadLeft(tamanho, preenchimento);
        }

        private string FormatarDecimal(decimal valor, int tamanhoTotal, int decimais)
        {
            long valorSemDecimal = (long)(valor * (decimal)Math.Pow(10, decimais));
            return FormatarNumero(valorSemDecimal, tamanhoTotal, '0');
        }

        private string FormatarData(DateTime data)
        {
            return data.ToString("ddMMyyyy");
        }

        private string FormatarHora(DateTime data)
        {
            return data.ToString("HHmmss");
        }

        // Mapeia o tipo de chave PIX do banco para o código CNAB
        private string MapearTipoChavePixParaCnab(string tipoChavePix)
        {
            if (string.IsNullOrWhiteSpace(tipoChavePix))
                return "  ";

            return tipoChavePix.ToUpper() switch
            {
                "T" => "01",  // Telefone
                "E" => "02",  // E-mail
                "C" => "03",  // CPF
                "J" => "03",  // CNPJ (mesmo código que CPF)
                "R" => "04",  // Chave Aleatória (EVP)
                _ => "  "
            };
        }

        // Header de Arquivo (Tipo 0)
        public string GerarHeaderArquivo(CnabOrgResult cnabOrg, int numeroSequencialArquivo, DateTime dataGeracao)
        {
            var linha = new StringBuilder();

            linha.Append(FormatarTexto(cnabOrg.CodigoBanco, 3, '0', false));  // 001-003: Código do Banco na Compensação
            linha.Append("0000");                                              // 004-007: Lote de Serviço
            linha.Append("0");                                                 // 008-008: Registro Header de Arquivo
            linha.Append(FormatarTexto("", 6));                               // 009-014: Brancos - Complemento de Registro
            linha.Append("085");                                               // 015-017: Nº da Versão do Layout do Arquivo
            linha.Append("2");                                                 // 018-018: Tipo de Inscrição da Empresa (1=CPF, 2=CNPJ)
            linha.Append(FormatarTexto(cnabOrg.CnpjEmpresa, 14, '0', false)); // 019-032: Número CNPJ Empresa Debitada
            linha.Append(FormatarTexto("", 20));                              // 033-052: Brancos - Complemento de Registro
            linha.Append(FormatarTexto(cnabOrg.Agencia, 5, '0', false));      // 053-057: Número Agência Debitada
            linha.Append(" ");                                                 // 058-058: Brancos - Complemento de Registro
            linha.Append(FormatarTexto(cnabOrg.Conta, 12, '0', false));       // 059-070: Número de C/C Debitada
            linha.Append(" ");                                                 // 071-071: Brancos - Complemento de Registro
            linha.Append(FormatarTexto(cnabOrg.ContaDV, 1));                  // 072-072: DAC da Agência/Conta Debitada
            linha.Append(FormatarTexto(StringUtil.RemoveDiacritics(cnabOrg.NomeEmpresa.ToUpper()), 30));             // 073-102: Nome da Empresa
            linha.Append(FormatarTexto(cnabOrg.NomeBanco, 30));               // 103-132: Nome do Banco
            linha.Append(FormatarTexto("", 10));                              // 133-142: Brancos - Complemento de Registro
            linha.Append("1");                                                 // 143-143: Código Remessa/Retorno (1=Remessa)
            linha.Append(FormatarData(dataGeracao));                          // 144-151: Data de Geração do Arquivo
            linha.Append(FormatarHora(dataGeracao));                          // 152-157: Hora de Geração do Arquivo
            linha.Append("000000000");                                         // 158-166: Número Sequencial do Arquivo (fixo em zeros)
            linha.Append("00000");                                             // 167-171: Densidade de Gravação do Arquivo
            linha.Append(FormatarTexto("", 69));                              // 172-240: Brancos - Complemento de Registro

            return linha.ToString();
        }

        // Header de Lote (Tipo 1)
        public string GerarHeaderLote(CnabOrgResult cnabOrg, int numeroLote, DateTime dataGeracao)
        {
            var linha = new StringBuilder();

            linha.Append(FormatarTexto(cnabOrg.CodigoBanco, 3, '0', false));  // 001-003: Código Banco na Compensação
            linha.Append(FormatarNumero(numeroLote, 4));                      // 004-007: Lote Identificação de Pagtos
            linha.Append("1");                                                 // 008-008: Registro Header de Lote
            linha.Append("C");                                                 // 009-009: Tipo da Operação (C=Crédito)
            linha.Append("30");                                                // 010-011: Tipo de Pagto
            linha.Append("01");                                                // 012-013: Forma de Pagamento
            linha.Append("040");                                               // 014-016: Nº da Versão do Layout do Lote
            linha.Append(" ");                                                 // 017-017: Brancos - Complemento de Registro
            linha.Append("2");                                                 // 018-018: Tipo Inscrição Empresa Debitada (1=CPF, 2=CNPJ)
            linha.Append(FormatarTexto(cnabOrg.CnpjEmpresa, 14, '0', false)); // 019-032: Número CNPJ Empresa Debitada
            linha.Append(FormatarTexto("", 4));                               // 033-036: Identificação do Lançamento no Extrato
            linha.Append(FormatarTexto("", 16));                              // 037-052: Brancos - Complemento de Registro
            linha.Append(FormatarTexto(cnabOrg.Agencia, 5, '0', false));      // 053-057: Número Agência Debitada
            linha.Append(" ");                                                 // 058-058: Brancos - Complemento de Registro
            linha.Append(FormatarTexto(cnabOrg.Conta, 12, '0', false));       // 059-070: Número de C/C Debitada
            linha.Append(" ");                                                 // 071-071: Brancos - Complemento de Registro
            linha.Append(FormatarTexto(cnabOrg.ContaDV, 1));                  // 072-072: DAC da Agência/Conta Debitada
            linha.Append(FormatarTexto(StringUtil.RemoveDiacritics(cnabOrg.NomeEmpresa.ToUpper()), 30));             // 073-102: Nome da Empresa Debitada
            linha.Append(FormatarTexto("01", 30));                            // 103-132: Finalidade dos Pagtos do Lote
            linha.Append(FormatarTexto("", 10));                              // 133-142: Complemento Histórico C/C Debitada
            linha.Append(FormatarTexto(StringUtil.RemoveDiacritics(cnabOrg.EnderecoEmpresa.ToUpper()), 30));         // 143-172: Nome da Rua, Av, Pça, etc...
            linha.Append(FormatarTexto(cnabOrg.NumeroLocal, 5, '0', false));  // 173-177: Número do Local
            linha.Append(FormatarTexto(StringUtil.RemoveDiacritics(cnabOrg.ComplementoEndereco?? "").ToUpper() ?? "", 15)); // 178-192: Casa, Apto, Sala, etc...
            linha.Append(FormatarTexto(StringUtil.RemoveDiacritics(cnabOrg.Cidade), 20));                  // 193-212: Nome da Cidade
            linha.Append(FormatarTexto(cnabOrg.Cep, 8, '0', false));          // 213-220: CEP
            linha.Append(FormatarTexto(cnabOrg.Estado, 2));                   // 221-222: Sigla do Estado
            linha.Append(FormatarTexto("", 8));                               // 223-230: Brancos - Complemento de Registro
            linha.Append(FormatarTexto("", 10));                              // 231-240: Código Ocorrências p/Retorno

            return linha.ToString();
        }

        // Detalhe Segmento A (Tipo 3 - Segmento A)
        public string GerarDetalheSegmentoA(CnabOrgResult cnabOrg, ColaboradorPagamentoAgrupadoDTO pagamento, int numeroLote, int numeroRegistro, DateTime dataGeracao)
        {
            var linha = new StringBuilder();
            bool isPix = pagamento.DadosBancarios.FormaPagamento?.ToUpper() == "PIX";

            linha.Append(FormatarTexto(cnabOrg.CodigoBanco, 3, '0', false));                        // 001-003: Código Banco na Compensação
            linha.Append(FormatarNumero(numeroLote, 4));                                            // 004-007: Lote de Serviço
            linha.Append("3");                                                                       // 008-008: Registro Detalhe de Lote
            linha.Append(FormatarNumero(numeroRegistro, 5));                                        // 009-013: Nº Sequencial Registro no Lote
            linha.Append("A");                                                                       // 014-014: Segmento
            linha.Append("000");                                                                     // 015-017: Tipo de Movimento (000=Inclusão)
            linha.Append(isPix ? "009" : "000");                                                    // 018-020: Câmara (009=PIX, 000=TED)
            linha.Append(FormatarTexto(pagamento.DadosBancarios.CodigoBanco ?? "", 3, '0', false));// 021-023: Código Banco Favorecido

            // 024-043: Agência Conta Favorecido (formato diferente para PIX e TED)
            if (isPix)
            {
                // PIX via chave: campo pode ser zerado (chave vai no Segmento B)
                linha.Append(FormatarTexto("", 20, '0'));                                           // 024-043: Zeros para PIX via chave
            }
            else
            {
                // TED: formato específico para outros bancos (NOTA 11)
                linha.Append(FormatarTexto(pagamento.DadosBancarios.Agencia ?? "", 5, '0', false));// 024-028: Agência
                linha.Append(" ");                                                                   // 029-029: Branco
                linha.Append(FormatarTexto(pagamento.DadosBancarios.Conta ?? "", 12, '0', false)); // 030-041: Conta
                linha.Append(" ");                                                                   // 042-042: Branco
                linha.Append(FormatarTexto(pagamento.DadosBancarios.ContaDv ?? "", 1));            // 043-043: DAC
            }

            linha.Append(FormatarTexto(StringUtil.RemoveDiacritics(pagamento.DadosBancarios.NomeColaborador) ?? "", 30));       // 044-073: Nome do Favorecido
            linha.Append(FormatarTexto("", 20));                                                    // 074-093: Nº Docto Atribuído pela Empresa
            linha.Append(FormatarData(dataGeracao));                                                // 094-101: Data Prevista para Pagto
            linha.Append("REA");                                                                     // 102-104: Tipo da Moeda (REA ou 009)
            linha.Append(FormatarTexto("", 8));                                                     // 105-112: Código ISPB (brancos)
            linha.Append(isPix ? "04" : "01");                                                      // 113-114: Identificação Transferência (04=Chave Pix, 01=Conta Corrente)
            linha.Append("00000");                                                                   // 115-119: Zeros
            linha.Append(FormatarDecimal(pagamento.ValorTotal, 15, 2));                            // 120-134: Valor Previsto do Pagto
            linha.Append(FormatarTexto("", 15));                                                    // 135-149: Nosso Número (brancos)
            linha.Append(FormatarTexto("", 5));                                                     // 150-154: Brancos
            linha.Append("00000000");                                                                // 155-162: Data Efetiva (zeros)
            linha.Append(FormatarDecimal(0, 15, 2));                                               // 163-177: Valor Efetivo (zeros)
            linha.Append(FormatarTexto("", 20));                                                    // 178-197: Finalidade Detalhe
            linha.Append("000000");                                                                  // 198-203: Nº do Documento (zeros)
            linha.Append(FormatarTexto(pagamento.DadosBancarios.CpfColaborador ?? "", 14, '0', false)); // 204-217: CPF/CNPJ Favorecido
            linha.Append("  ");                                                                      // 218-219: Finalidade DOC e Status Funcionário
            linha.Append(isPix ? "     " : "00005");                                                // 220-224: Finalidade TED (00005=Pagto Fornecedores, brancos para PIX)
            linha.Append(FormatarTexto("", 5));                                                     // 225-229: Brancos
            linha.Append("0");                                                                       // 230-230: Aviso ao Favorecido (0=não emite)
            linha.Append(FormatarTexto("", 10));                                                    // 231-240: Ocorrências (brancos)

            return linha.ToString();
        }

        // Detalhe Segmento B (Tipo 3 - Segmento B) - Obrigatório para PIX
        public string GerarDetalheSegmentoB(CnabOrgResult cnabOrg, ColaboradorPagamentoAgrupadoDTO pagamento, int numeroLote, int numeroRegistro)
        {
            var linha = new StringBuilder();

            linha.Append(FormatarTexto(cnabOrg.CodigoBanco, 3, '0', false));                    // 001-003: Código Banco na Compensação
            linha.Append(FormatarNumero(numeroLote, 4));                                        // 004-007: Lote de Serviço
            linha.Append("3");                                                                   // 008-008: Registro Detalhe do Lote
            linha.Append(FormatarNumero(numeroRegistro, 5));                                    // 009-013: Nº Sequencial Registro no Lote
            linha.Append("B");                                                                   // 014-014: Código Segmento Reg. Detalhe
            linha.Append(MapearTipoChavePixParaCnab(pagamento.DadosBancarios.TipoChavePix));  // 015-016: Tipo Identificação de Chave PIX
            linha.Append(" ");                                                                   // 017-017: Brancos - Complemento de Registro

            // Tipo de inscrição: 1=CPF, 2=CNPJ
            var cpfCnpj = pagamento.DadosBancarios.CpfColaborador ?? "";
            var tipoInscricao = cpfCnpj.Length == 11 ? "1" : "2";
            linha.Append(tipoInscricao);                                                        // 018-018: Tipo Inscrição do Favorecido (1=CPF, 2=CNPJ)
            linha.Append(FormatarTexto(cpfCnpj, 14, '0', false));                              // 019-032: Nº de Inscrição do Favorecido (CPF/CNPJ)

            linha.Append(FormatarTexto("", 30));                                                // 033-062: Brancos - Complemento de Registro
            linha.Append(FormatarTexto("", 65));                                                // 063-127: Informações Entre Usuários (opcional - NOTA 39)
            linha.Append(FormatarTexto(pagamento.DadosBancarios.ChavePix ?? "", 100));         // 128-227: Chave de Endereçamento (Chave PIX - NOTA 40)
            linha.Append(FormatarTexto("", 3));                                                 // 228-230: Brancos - Complemento de Registro
            linha.Append(FormatarTexto("", 10));                                                // 231-240: Ocorrências (brancos na remessa)

            return linha.ToString();
        }

        // Trailer de Lote (Tipo 5)
        public string GerarTrailerLote(CnabOrgResult cnabOrg, int numeroLote, int quantidadeRegistros, decimal valorTotal)
        {
            var linha = new StringBuilder();

            linha.Append(FormatarTexto(cnabOrg.CodigoBanco, 3, '0', false));  // 001-003: Código do Banco
            linha.Append(FormatarNumero(numeroLote, 4));                      // 004-007: Lote de Serviço
            linha.Append("5");                                                 // 008-008: Tipo de Registro (5 = Trailer de Lote)
            linha.Append(FormatarTexto("", 9));                               // 009-017: Uso exclusivo FEBRABAN
            linha.Append(FormatarNumero(quantidadeRegistros, 6));             // 018-023: Quantidade de Registros do Lote
            linha.Append(FormatarDecimal(valorTotal, 18, 2));                 // 024-041: Somatória dos Valores
            linha.Append(FormatarNumero(0, 18));                              // 042-059: Somatória de Quantidade de Moedas
            linha.Append(FormatarTexto("", 6, '0', false));                   // 060-065: Número Aviso Débito
            linha.Append(FormatarTexto("", 165));                             // 066-230: Uso exclusivo FEBRABAN
            linha.Append(FormatarTexto("", 10));                              // 231-240: Códigos de Ocorrências

            return linha.ToString();
        }

        // Trailer de Arquivo (Tipo 9)
        // Trailer de Arquivo (Tipo 9)
        public string GerarTrailerArquivo(CnabOrgResult cnabOrg, int quantidadeLotes, int quantidadeRegistros)
        {
            var linha = new StringBuilder();

            linha.Append(FormatarTexto(cnabOrg.CodigoBanco, 3, '0', false));  // 001-003: Código Banco na Compensação
            linha.Append("9999");                                              // 004-007: Lote de Serviço (9999 para trailer)
            linha.Append("9");                                                 // 008-008: Registro Trailer de Arquivo
            linha.Append(FormatarTexto("", 9));                               // 009-017: Brancos - Complemento de Registro
            linha.Append(FormatarNumero(quantidadeLotes, 6));                 // 018-023: Qtde Lotes do Arquivo
            linha.Append(FormatarNumero(quantidadeRegistros, 6));             // 024-029: Qtde Registros do Arquivo
            linha.Append(FormatarTexto("", 211));                             // 030-240: Brancos - Complemento de Registro

            return linha.ToString();
        }
    }
}
