using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using UglyToad.PdfPig;

namespace Colaboracao.Helper.Util
{
    /// <summary>
    /// Utilitário para manipulação de arquivos PDF
    /// </summary>
    public static class PdfManipulationUtil
    {
        /// <summary>
        /// Obtém a quantidade de páginas de um arquivo PDF
        /// </summary>
        /// <param name="arquivoPdf">Array de bytes do arquivo PDF</param>
        /// <returns>Número de páginas do PDF</returns>
        public static int ObterQuantidadePaginas(byte[] arquivoPdf)
        {
            try
            {
                using (var memoryStream = new MemoryStream(arquivoPdf))
                using (var pdf = UglyToad.PdfPig.PdfDocument.Open(memoryStream))
                {
                    return pdf.NumberOfPages;
                }
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// Extrai a primeira página de um arquivo PDF
        /// </summary>
        /// <param name="arquivoPdf">Array de bytes do arquivo PDF</param>
        /// <returns>Array de bytes contendo apenas a primeira página</returns>
        public static byte[] ExtrairPrimeiraPagina(byte[] arquivoPdf)
        {
            try
            {
                using (var inputStream = new MemoryStream(arquivoPdf))
                using (var outputStream = new MemoryStream())
                {
                    var pdfReader = new PdfReader(inputStream.ToArray());
                    var document = new Document();
                    var pdfCopy = new PdfCopy(document, outputStream);
                    document.Open();
                    pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, 1));
                    document.Close();
                    pdfReader.Close();
                    return outputStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair primeira página: {ex.Message}");
                return arquivoPdf;
            }
        }

        /// <summary>
        /// Divide um arquivo PDF em páginas individuais
        /// </summary>
        /// <param name="pdfBytes">Array de bytes do arquivo PDF</param>
        /// <returns>Lista de arrays de bytes, cada um representando uma página</returns>
        public static List<byte[]> DividirPdfEmPaginas(byte[] pdfBytes)
        {
            var paginas = new List<byte[]>();
            
            try
            {
                using (var inputStream = new MemoryStream(pdfBytes))
                {
                    var pdfReader = new PdfReader(inputStream.ToArray());
                    
                    for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        using (var outputStream = new MemoryStream())
                        {
                            var document = new Document();
                            var pdfCopy = new PdfCopy(document, outputStream);
                            document.Open();
                            pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, i));
                            document.Close();
                            
                            paginas.Add(outputStream.ToArray());
                        }
                    }
                    
                    pdfReader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao dividir PDF em páginas: {ex.Message}", ex);
            }
            
            return paginas;
        }
    }
} 