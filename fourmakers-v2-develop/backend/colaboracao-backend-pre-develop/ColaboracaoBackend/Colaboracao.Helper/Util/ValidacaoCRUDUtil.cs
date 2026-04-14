using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Colaboracao.Helper
{
    public static class ValidadorCamposUtil
    {
        public static Task ValidaCampos(List<CampoValidacao> campos)
        {
            var erroObrigatoriedade = ValidaCamposObrigatorios(campos);

            var errorMessages = new List<string>();

            if (!string.IsNullOrEmpty(erroObrigatoriedade))
            {
                errorMessages.Add(erroObrigatoriedade);
            }

            foreach (var campo in campos.Where(o => o.TipoValidacao != TipoValidacaoEnum.Obrigatoriedade))
            {
                try
                {
                    if (campo.Valor.IsEmpty()) // valida somente se tem valor, obrigatoriedade deve ser tratada com o TipoValidacaoEnum.Obrigatoriedade
                        continue;

                    switch (campo.TipoValidacao)
                    {
                        case TipoValidacaoEnum.ValorMaiorQueZero:
                            ValidarMaiorQueZero(campo.Valor, campo.DescricaoCampo);
                            break;

                        case TipoValidacaoEnum.ValidarFormatoCodigo:
                            ValidaFormatoCodigo(campo.Valor.ToStringOuVazio(), campo.DescricaoCampo);
                            break;

                        case TipoValidacaoEnum.ValidarEmail:
                            ValidarEmail(campo.Valor.ToStringOuVazio(), campo.DescricaoCampo);
                            break;

                        case TipoValidacaoEnum.ValidarTelefoneFixoOuCelular:
                            ValidarTelefoneFixoOuCelular(campo.Valor.ToStringOuVazio(), campo.DescricaoCampo);
                            break;

                        case TipoValidacaoEnum.TamanhoMinimo:
                            ValidarTamanhoMinimo(campo.Valor.ToStringOuVazio(), campo.DescricaoCampo, campo.TamanhoMinimo);
                            break;

                        case TipoValidacaoEnum.TamanhoMaximo:
                            ValidarTamanhoMaximo(campo.Valor.ToStringOuVazio(), campo.DescricaoCampo, campo.TamanhoMaximo);
                            break;

                        case TipoValidacaoEnum.TamanhoExato:
                            ValidarTamanhoExato(campo.Valor.ToStringOuVazio(), campo.DescricaoCampo, campo.TamanhoExato);
                            break;

                        case TipoValidacaoEnum.TamanhoEntre:
                            ValidarTamanhoEntre(campo.Valor.ToStringOuVazio(), campo.DescricaoCampo, campo.TamanhoMinimo, campo.TamanhoMaximo);
                            break;

                        case TipoValidacaoEnum.ValidarData:
                            ValidarData(campo.Valor.ToStringOuVazio(), campo.DescricaoCampo);
                            break;

                        case TipoValidacaoEnum.ValidarFormatoHora:
                            ValidarFormatoHora(campo.Valor.ToStringOuVazio(), campo.DescricaoCampo);
                            break;

                        case TipoValidacaoEnum.ContemAoMenosUmNaLista:
                            ValidarExistenciaDeItensNalista(campo, campo.DescricaoCampo);
                            break;

                        case TipoValidacaoEnum.ContemAoMenosDoisNaLista:
                            ValidarExistenciaDeAoMenosDoisItensNalista(campo, campo.DescricaoCampo);
                            break;

                        default:
                            throw new NotSupportedException($"Validação não suportada: {campo.TipoValidacao}");
                    }
                }
                catch (Exception ex)
                {
                    errorMessages.Add($"{ex.Message}");
                }
            }

            if (errorMessages.Any())
            {
                throw new ApplicationException($"{string.Join("; ", errorMessages)}");
            }

            return Task.CompletedTask;
        }

        private static void ValidarExistenciaDeAoMenosDoisItensNalista(CampoValidacao campo, string descricaoCampo)
        {
            var lista = campo.Valor as System.Collections.IEnumerable;

            int contador = 0;
            if (lista is not null)
                foreach (var item in lista)
                {
                    contador = contador + 1;
                    if(contador >=2)
                        return;
                }

            throw new ArgumentException($"Campo {descricaoCampo} deve ter dois itens na lista.");
        }

        private static void ValidarExistenciaDeItensNalista(CampoValidacao campo, string descricaoCampo)
        {
            var lista = campo.Valor as System.Collections.IEnumerable;
            
            if(lista is not null)
                foreach (var item in lista)
                    return;

            throw new ArgumentException($"Campo {descricaoCampo} deve ser preenchido com um valor.");
        }

        private static string ValidaCamposObrigatorios(List<CampoValidacao> campos)
        {
            var errorMessages = new List<string>();

            foreach (var campo in campos)
            {
                if (campo.TipoValidacao == TipoValidacaoEnum.Obrigatoriedade)
                {
                    if (campo.Valor.IsEmpty())
                    {
                        errorMessages.Add($"{campo.DescricaoCampo}");
                    }
                }
            }

            if (errorMessages.Any())
            {
                var plural = errorMessages.Count > 1;
                var pluralLetraS = (plural ? "s" : "");

                string mensagemCampos = errorMessages.Count == 1
                    ? errorMessages.First()
                    : string.Join(", ", errorMessages.Take(errorMessages.Count - 1)) +
                      " e " +
                      errorMessages.Last();

                return $"Campo{pluralLetraS} {mensagemCampos} {(plural ? "são" : "é")} obrigatório{pluralLetraS}.";
            }
            return string.Empty;
        }

        private static void ValidarMaiorQueZero(object valor, string descricaoCampo)
        {
            var numero = valor.ToDecimalOuZero();
            if (numero <= 0)
            {
                throw new ArgumentException($"Campo {descricaoCampo} deve ser preenchido com um valor maior que zero.");
            }
        }

        private static void ValidaFormatoCodigo(string codigo, string descricaoCampo)
        {
            var regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_-]+$");
            if (!regex.IsMatch(codigo))
            {
                throw new ApplicationException($"Campo {descricaoCampo} deve conter apenas letras, números, underscore (_) e traço (-).");
            }
        }

        private static void ValidarEmail(string email, string descricaoCampo)
        {
            if (!Colaboracao.Helper.ToolsUtil.ValidaEmail(email))
            {
                throw new ArgumentException($"Campo {descricaoCampo} deve conter um e-mail válido.");
            }
        }

        private static void ValidarTelefoneFixoOuCelular(string telefone, string descricaoCampo)
        {
            var digitos = 9;
            if (telefone.Length < digitos)
            {
                throw new ArgumentException($"Campo {descricaoCampo} inválido. O telefone deve conter pelo menos {digitos} dígitos, incluindo o DDD.");
            }
        }

        private static void ValidarTelefoneCelular(string telefone, string descricaoCampo)
        {
            var digitos = 10;
            if (telefone.Length < digitos)
            {
                throw new ArgumentException($"Campo {descricaoCampo} inválido. O telefone deve conter pelo menos {digitos} dígitos, incluindo o DDD.");
            }
        }

        private static void ValidarTamanhoMinimo(string valor, string descricaoCampo, int tamanhoMinimo)
        {
            if (valor.Length < tamanhoMinimo)
            {
                throw new ArgumentException($"Campo {descricaoCampo} deve ter no mínimo {tamanhoMinimo} caracteres.");
            }
        }

        private static void ValidarTamanhoMaximo(string valor, string descricaoCampo, int tamanhoMaximo)
        {
            if (valor.Length > tamanhoMaximo)
            {
                throw new ArgumentException($"Campo {descricaoCampo} deve ter no máximo {tamanhoMaximo} caracteres.");
            }
        }

        private static void ValidarTamanhoExato(string valor, string descricaoCampo, int tamanhoExato)
        {
            if (valor.Length != tamanhoExato)
            {
                throw new ArgumentException($"Campo {descricaoCampo} deve ter exatamente {tamanhoExato} caracteres.");
            }
        }

        private static void ValidarTamanhoEntre(string valor, string descricaoCampo, int tamanhoMinimo, int tamanhoMaximo)
        {
            if (valor.Length < tamanhoMinimo || valor.Length > tamanhoMaximo)
            {
                throw new ArgumentException($"Campo {descricaoCampo} deve ter entre {tamanhoMinimo} e {tamanhoMaximo} caracteres.");
            }
        }

        private static void ValidarData(string data, string descricaoCampo)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return; // Se a data está vazia, não valida (a obrigatoriedade é validada separadamente)
            }

            // Tenta parsear a data usando a cultura brasileira para aceitar formato dd/MM/yyyy
            var culturaBR = new System.Globalization.CultureInfo("pt-BR");
            if (!DateTime.TryParse(data, culturaBR, System.Globalization.DateTimeStyles.None, out _))
            {
                throw new ArgumentException($"Campo {descricaoCampo} deve conter uma data válida no formato dd/MM/yyyy.");
            }
        }

        private static void ValidarFormatoHora(string hora, string descricaoCampo)
        {
            // Regex para validar formatos: XXX:XX, XX:XX, X:XX
            var regex = new Regex(@"^\d{1,3}:[0-5]\d$");
            
            if (!regex.IsMatch(hora))
                throw new ArgumentException($"Campo {descricaoCampo} deve estar no formato H:MM, HH:MM ou HHH:MM (ex: 8:30, 40:15, 180:00).");

            // Validar se os minutos não passam de 59
            var partes = hora.Split(':');
            if (partes.Length == 2 && int.TryParse(partes[1], out int minutos))
            {
                if (minutos > 59)
                    throw new ArgumentException($"Campo {descricaoCampo}: minutos não podem ser superiores a 59.");
            }
        }
    }

    public class CampoValidacao
    {
        public CampoValidacao(string descricaoCampo, object valor, TipoValidacaoEnum tipoValidacao)
        {
            DescricaoCampo = descricaoCampo;
            Valor = valor;
            TipoValidacao = tipoValidacao;
        }

        public string DescricaoCampo { get; set; } = "";
        public object Valor { get; set; } = "";
        public TipoValidacaoEnum TipoValidacao { get; set; }
        public int TamanhoMinimo { get; set; }
        public int TamanhoMaximo { get; set; }
        public int TamanhoExato { get; set; }
    }

    public enum TipoValidacaoEnum
    {
        Obrigatoriedade,
        ValorMaiorQueZero,
        ValidarFormatoCodigo,
        ValidarEmail,
        ValidarTelefoneCelular,
        ValidarTelefoneFixoOuCelular,
        TamanhoMinimo,
        TamanhoMaximo,
        TamanhoExato,
        TamanhoEntre,
        ValidarData,
        ContemAoMenosUmNaLista,
        ContemAoMenosDoisNaLista,
        ValidarFormatoHora
    }
}