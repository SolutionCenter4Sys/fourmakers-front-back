using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace Colaboracao.Helper
{
    public static class ToolsUtil
    {
        public static long JsonStringToLong(string numero)
        {
            string aux = numero.Split(new char[2] { '.', ',' })[0];
            return Convert.ToInt64(aux);
        }

        public static bool VerificaArquivo(string arquivo)
        {
            try
            {
                if (File.Exists(arquivo))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool GravaImagem(string caminho, byte[] imagem, string nomeImagem)
        {
            try
            {
                Image result = null;
                ImageFormat format = ImageFormat.Png;
                nomeImagem = nomeImagem.Split('.')[0];
                result = new Bitmap(new MemoryStream(imagem));
                using (Image imageToExport = result)
                {
                    (new FileInfo(caminho)).Directory.Create();
                    if (File.Exists(caminho + nomeImagem + "." + format.ToString().ToLower()))
                    {
                        File.Delete(caminho + nomeImagem + "." + format.ToString().ToLower());
                    }
                    imageToExport.Save(caminho + nomeImagem + "." + format.ToString().ToLower(), format);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool GravaImagemThumb(string caminho, byte[] imagem, string nomeImagem)
        {
            try
            {
                Image result = null;
                ImageFormat format = ImageFormat.Png;
                nomeImagem = nomeImagem.Split('.')[0];
                result = new Bitmap(new MemoryStream(imagem));
                result = ResizeImage(result, new Size(200, 200));
                using (Image imageToExport = result)
                {
                    (new FileInfo(caminho)).Directory.Create();
                    if (File.Exists(caminho + nomeImagem + "." + format.ToString().ToLower()))
                    {
                        File.Delete(caminho + nomeImagem + "." + format.ToString().ToLower());
                    }
                    imageToExport.Save(caminho + nomeImagem + "." + format.ToString().ToLower(), format);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool GravaImagemThumbMini(string caminho, byte[] imagem, string nomeImagem)
        {
            try
            {
                Image result = null;
                ImageFormat format = ImageFormat.Png;
                nomeImagem = nomeImagem.Split('.')[0];
                result = new Bitmap(new MemoryStream(imagem));
                result = ResizeImage(result, new Size(25, 25));
                using (Image imageToExport = result)
                {
                    (new FileInfo(caminho)).Directory.Create();
                    if (File.Exists(caminho + nomeImagem + "." + format.ToString().ToLower()))
                    {
                        File.Delete(caminho + nomeImagem + "." + format.ToString().ToLower());
                    }
                    imageToExport.Save(caminho + nomeImagem + "." + format.ToString().ToLower(), format);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool GravaImagemThumbVeryMini(string caminho, byte[] imagem, string nomeImagem)
        {
            try
            {
                Image result = null;
                ImageFormat format = ImageFormat.Png;
                nomeImagem = nomeImagem.Split('.')[0];
                result = new Bitmap(new MemoryStream(imagem));
                result = ResizeImage(result, new Size(25, 25));
                using (Image imageToExport = result)
                {
                    (new FileInfo(caminho)).Directory.Create();
                    if (File.Exists(caminho + nomeImagem + "." + format.ToString().ToLower()))
                    {
                        File.Delete(caminho + nomeImagem + "." + format.ToString().ToLower());
                    }
                    imageToExport.Save(caminho + nomeImagem + "." + format.ToString().ToLower(), format);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Image ResizeImage(Image imgToResize, Size destinationSize)
        {
            var originalWidth = imgToResize.Width;
            var originalHeight = imgToResize.Height;

            //how many units are there to make the original length
            var hRatio = (float)originalHeight / destinationSize.Height;
            var wRatio = (float)originalWidth / destinationSize.Width;

            //get the shorter side
            var ratio = Math.Min(hRatio, wRatio);

            var hScale = Convert.ToInt32(destinationSize.Height * ratio);
            var wScale = Convert.ToInt32(destinationSize.Width * ratio);

            //start cropping from the center
            var startX = (originalWidth - wScale) / 2;
            var startY = (originalHeight - hScale) / 2;

            //crop the image from the specified location and size
            var sourceRectangle = new Rectangle(startX, startY, wScale, hScale);

            //the future size of the image
            var bitmap = new Bitmap(destinationSize.Width, destinationSize.Height);

            //fill-in the whole bitmap
            var destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            //generate the new image
            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        public static bool GravaArquivo(string caminho, byte[] arquivo, string nomeArquivo)
        {
            try
            {
                (new FileInfo(caminho)).Directory.Create();
                if (File.Exists(caminho + nomeArquivo))
                {
                    File.Delete(caminho + nomeArquivo);
                }
                File.WriteAllBytes(caminho + nomeArquivo, arquivo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DeletaImagem(string caminho, string nomeImagem)
        {
            try
            {
                if (File.Exists(caminho + nomeImagem))
                {
                    File.Delete(caminho + nomeImagem);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ValidaCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }

        public static bool ValidaCpf(string cpf)
        {
            string valor = cpf.RemoveMascaraCpf();

            if (valor.Length != 11)
                return false;

            bool igual = true;

            for (int i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;

            if (igual || valor == "12345678909")
                return false;

            int[] numeros = new int[11];

            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(valor[i].ToString());
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];
            int resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else if (numeros[9] != 11 - resultado)
                return false;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];
            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else
                if (numeros[10] != 11 - resultado)
                return false;
            return true;
        }

        public static bool ValidaEmail(string email)
        {
            Regex rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");

            if (rg.IsMatch(email))
                return true;
            else
                return false;
        }

        public static bool ValidaTelefone(string telefone)
        {
            return Regex.IsMatch(telefone, @"^\+55\d{10,11}$");
        }
        
        public static bool ValidaTelefoneSemDDI(string telefone)
        {
            return Regex.IsMatch(telefone, @"^\d{10,11}$");
        }
    }
}