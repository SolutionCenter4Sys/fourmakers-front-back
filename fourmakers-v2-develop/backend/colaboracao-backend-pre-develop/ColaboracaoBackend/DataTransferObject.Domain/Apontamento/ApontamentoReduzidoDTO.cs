using System;

namespace DataTransferObject.Domain.Apontamento
{
    public class ApontamentoReduzidoDTO
    {
        public string Id { get; set; }
        public DateTime Data { get; set; }
        public string Cpf { get; set; }
        public string CodProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public string NomeCompletoColaborador { get; set; }
        public string EmailColaborador { get; set; }
        public string AtividadeDescricao { get; set; }
    }
}