
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.IntegracaoContabil
{
    public class RemessaContabilRegistroDTO
    {
        public string Origem { get; set; }
        public string Cnpj { get; set; }
        public string Unidade { get; set; }
        public string Cpf { get; set; }
        public string Matricula { get; set; }
        public string NomeColaborador { get; set; }
        public string Tipo { get; set; }
        public string Valor { get; set; }
        public string Categoria { get; set; }
        public string Documento { get; set; }
        [JsonIgnore]
        public string IdRegistro__ND { get; set; } //não apagar, retorno para controle ids

    }

}
