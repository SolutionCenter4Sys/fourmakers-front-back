using System.Text.Json;
using System.Text.RegularExpressions;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Financeiro.Banco.DadosBancariosColaborador;
using Financeiro.Domain.Interfaces.Banco;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.Banco;

[LogDomainClass]
public class DadosBancariosColaboradorValidadorService : IDadosBancariosColaboradorValidadorService
{
    public async Task<List<string>> ValidarEntrada(DadosBancariosColaboradorBase input, CRUDEnum crudEnum)
    {
        var retornoErros = new List<string>();
        AlterarEmptyParaNull(input);
        var (possuiPix, possuiTed) = ValidaSeExisteDadosBancarios(input);
        
        if (!possuiPix && !possuiTed)
        {
            retornoErros.Add("É obrigatório informar dados bancários (TED) ou uma chave PIX.");
            return retornoErros;
        }
        
        if (crudEnum == CRUDEnum.Create || crudEnum == CRUDEnum.Update)
        {
            if (possuiPix)
            {
                var errosPix = ValidarChavePix(input.ChavePix, input.TipoChavePix);
                retornoErros.AddRange(errosPix);
            }

            // Validação TED
            if (possuiTed)
            {
                var errosTed = await ValidarDadosTed(input);
                retornoErros.AddRange(errosTed);
            }

            var errosValidacaoMetodoPagamentoEscolhido = ValidaSeMetodoEscolhidoEValido(input, possuiTed, possuiPix);
            retornoErros.AddRange(errosValidacaoMetodoPagamentoEscolhido);
        }
        
        return retornoErros;
    }

    private List<string> ValidaSeMetodoEscolhidoEValido(
        DadosBancariosColaboradorBase input,
        bool possuiTed,
        bool possuiPix)
    {
        var erros = new List<string>();

        if (!Enum.IsDefined(typeof(FormaPagamentoEnum), input.FormaPagamento))
        {
            erros.Add($"A Forma de pagamento não é um método de pagamento válido.");
            return erros;
        }
        
        if (input.FormaPagamento == FormaPagamentoEnum.PIX && !possuiPix)
        {
            erros.Add("Forma de pagamento PIX escolhido, mas não há informações de pagamento para PIX.");
        }

        if (input.FormaPagamento == FormaPagamentoEnum.TED && !possuiTed)
        {
            erros.Add("Metodo de pagamento TED escolhido, mas não há informações de pagamento via TED.");
        }

        return erros;
    }

    private (bool possuiPix, bool possuiTed) ValidaSeExisteDadosBancarios(DadosBancariosColaboradorBase input)
    {
        bool possuiPix = !string.IsNullOrWhiteSpace(input.ChavePix) && 
                         !string.IsNullOrWhiteSpace(input.TipoChavePix);
           
        bool possuiTed = !string.IsNullOrWhiteSpace(input.CodigoBancoTed) ||
                         !string.IsNullOrWhiteSpace(input.AgenciaTed) ||
                         !string.IsNullOrWhiteSpace(input.ContaTed);
    
        return (possuiPix, possuiTed);
    }

    private void AlterarEmptyParaNull(DadosBancariosColaboradorBase input)
    {
        input.CodigoBancoTed = input.CodigoBancoTed.ToNullSeTextoNull();
        input.AgenciaTed = input.AgenciaTed.ToNullSeTextoNull();
        input.AgenciaDvTed = input.AgenciaDvTed.ToNullSeTextoNull();
        input.ContaTed = input.ContaTed.ToNullSeTextoNull();
        input.ContaDvTed = input.ContaDvTed.ToNullSeTextoNull();
        input.ChavePix = input.ChavePix.ToNullSeTextoNull();
        input.TipoChavePix = input.TipoChavePix.ToNullSeTextoNull();
    }

    private List<string> ValidarChavePix(string chave, string tipo)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(chave))
        {
            erros.Add("A chave PIX não pode ser nula ou vazia.");
            return erros;
        }

        tipo = tipo.Trim().ToUpperInvariant();

        bool validaCpf = ToolsUtil.ValidaCpf(chave);
        bool validaCnpj = ToolsUtil.ValidaCnpj(chave);
        bool validaEmail = ToolsUtil.ValidaEmail(chave);
        bool validaTelefone = ToolsUtil.ValidaTelefoneSemDDI(chave);

        switch (tipo)
        {
            case "C":
                if (!validaCpf)
                    erros.Add("Chave PIX inválida: CPF informado não é válido.");
                break;

            case "J":
                if (!validaCnpj)
                    erros.Add("Chave PIX inválida: CNPJ informado não é válido.");
                break;

            case "E":
                if (!validaEmail)
                    erros.Add("Chave PIX inválida: e-mail informado não é válido.");
                break;

            case "T":
                if (!validaTelefone)
                    erros.Add("Chave PIX inválida: telefone informado não é válido.");
                break;

            case "R":
                if (!Guid.TryParse(chave, out _))
                    erros.Add("Chave PIX inválida: EVP deve ser um GUID (UUID) válido.");
                break;

            default:
                erros.Add("Tipo de chave PIX inválido");
                break;
        }

        return erros;
    }

    private async Task<List<string>> ValidarDadosTed(DadosBancariosColaboradorBase input)
    {
        var erros = new List<string>();
    
        if (string.IsNullOrWhiteSpace(input.CodigoBancoTed))
            erros.Add("O código do banco (TED) é obrigatório quando houver dados bancários.");
        else if (input.CodigoBancoTed.Length != 3)
            erros.Add("O código do banco deve conter exatamente 3 dígitos.");
        else if (!Regex.IsMatch(input.CodigoBancoTed, @"^\d{3}$"))
            erros.Add("O código do banco deve conter apenas números.");
        else
        {
            var listaDeBancosCarregadas = await CarregarListasDeBancos();
            var buscaBanco = listaDeBancosCarregadas.FirstOrDefault(x => x.COMPE == input.CodigoBancoTed);
            if (buscaBanco == null)
            {
                erros.Add("O código do banco não existe em nossa base de conhecimento.");
            }
        }

        if (string.IsNullOrWhiteSpace(input.AgenciaTed))
            erros.Add("A agência bancária é obrigatória.");
        else if (input.AgenciaTed.Length > 5)
            erros.Add("A agência bancária deve ter no máximo 5 dígitos.");
        else if (!Regex.IsMatch(input.AgenciaTed, @"^\d+$"))
            erros.Add("A agência bancária deve conter apenas números.");

        if (string.IsNullOrEmpty(input.AgenciaDvTed) || input.AgenciaDvTed.Length != 1)
            erros.Add("O dígito verificador da agência deve ter 1 caractere.");

        if (string.IsNullOrWhiteSpace(input.ContaTed))
            erros.Add("A conta bancária é obrigatória.");
        else if (input.ContaTed.Length > 12)
            erros.Add("A conta bancária deve ter no máximo 12 dígitos.");
        else if (!Regex.IsMatch(input.ContaTed, @"^\d+$"))
            erros.Add("A conta bancária deve conter apenas números.");

        if (string.IsNullOrEmpty(input.ContaDvTed) || input.ContaDvTed.Length != 1)
            erros.Add("O dígito verificador da conta deve ter 1 caractere.");

        return erros;
    }

    private async Task<List<BancoJsonDTO>> CarregarListasDeBancos()
    {
        var caminho = Path.Combine(AppContext.BaseDirectory, "Resources", "lista_bancos.json");
        var json = await File.ReadAllTextAsync(caminho);
        var bancos = JsonSerializer.Deserialize<List<BancoJsonDTO>>(json);
        return bancos?? [];
    }
}