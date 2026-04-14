using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using DataTransferObject.Domain.Util;
using UglyToad.PdfPig.Content;
using PdfDocument = UglyToad.PdfPig.PdfDocument;

// Imports do iText7 (corretos para o método AddSignature)
using iText.Kernel.Pdf;

namespace Colaboracao.Helper.Util
{
    public static class PdfUtil
    {
        public static string ToString(byte[] file)
        {
            if (file == null || file.Length == 0)
                throw new ValidationException("Arquivo inválido.");

            if (!IsPdfFile(file))
                throw new ValidationException("O arquivo enviado não é um PDF válido.");

            using (var memoryStream = new MemoryStream(file))
            using (var pdf = PdfDocument.Open(memoryStream))
            {
                var fullText = new StringBuilder();

                Console.WriteLine($"Total de páginas: {pdf.NumberOfPages}");

                for (int i = 1; i <= pdf.NumberOfPages; i++) // PdfPig usa 1-based indexing
                {
                    Page page = pdf.GetPage(i);
                    string pageText = page.Text;

                    Console.WriteLine($"Página {i}: {(string.IsNullOrWhiteSpace(pageText) ? "[vazia]" : $"texto extraído")}");

                    fullText.AppendLine(pageText);
                }

                return fullText.ToString();
            }
        }

        private static bool IsPdfFile(byte[] file)
        {
            if (file == null || file.Length < 4)
                return false;

            return file[0] == 0x25 && file[1] == 0x50 &&
                   file[2] == 0x44 && file[3] == 0x46; // %PDF
        }

        public static byte[] AddSignature(byte[] pdfBytes, List<PdfSignatureLine> lines, int? pageNumber = null)
        {
            using var inputPdfStream = new MemoryStream(pdfBytes);
            using var outputPdfStream = new MemoryStream();

            var pdfReader = new PdfReader(inputPdfStream);
            var pdfWriter = new PdfWriter(outputPdfStream);
            var pdfDoc = new iText.Kernel.Pdf.PdfDocument(pdfReader, pdfWriter);
            var document = new iText.Layout.Document(pdfDoc);

            int targetPage = pageNumber ?? pdfDoc.GetNumberOfPages();

            foreach (var line in lines)
            {
                var textParagraph = new iText.Layout.Element.Paragraph(line.Text)
                    .SetFontSize(line.FontSize);
            
                document.ShowTextAligned(
                    textParagraph,
                    line.PosX, line.PosY, targetPage,
                    iText.Layout.Properties.TextAlignment.LEFT,
                    iText.Layout.Properties.VerticalAlignment.BOTTOM,
                    0
                );
            }

            document.Close();

            return outputPdfStream.ToArray();
        }
    }
}