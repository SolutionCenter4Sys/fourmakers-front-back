using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Banco.DadosBancariosColaborador;

public class BancoJsonDTO
{
    public string COMPE  { get; set; } // Codigo Banco EX: 001 Banco do brasil
    public string ShortName { get; set; }
}