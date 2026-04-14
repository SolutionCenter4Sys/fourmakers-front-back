using BitMiracle.Docotic.Pdf;
using SkiaSharp;
using System;
using System.IO;

namespace Colaboracao.Helper
{
    public static class GeradorThumbUtil
    {
        public static byte[] Converter(byte[] imagem, int tamanho)
        {
            return Resize(imagem, tamanho, tamanho).FileContents;
        }

        public static byte[] ConverterPDF(byte[] document, int tamanho)
        {
            using (var pdf = new PdfDocument(document))
            {
                PdfDrawOptions options = PdfDrawOptions.CreateFitHeight(tamanho);
                byte[] retorno = null;

                using (var stream = new MemoryStream())
                {
                    pdf.Pages[0].Save(stream, options);
                    retorno = stream.ToArray();
                }

                return retorno;
            }
        }
        public static (byte[] FileContents, int Height, int Width) Resize(byte[] fileContents,
            int maxWidth, int maxHeight,
            SKFilterQuality quality = SKFilterQuality.Medium)
        {
            using MemoryStream ms = new MemoryStream(fileContents);
            using SKBitmap sourceBitmap = SKBitmap.Decode(ms);

            int height = Math.Min(maxHeight, sourceBitmap.Height);
            int width = Math.Min(maxWidth, sourceBitmap.Width);

            using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), quality);
            using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
            using SKData data = scaledImage.Encode();

            return (data.ToArray(), height, width);
        }
    }
}