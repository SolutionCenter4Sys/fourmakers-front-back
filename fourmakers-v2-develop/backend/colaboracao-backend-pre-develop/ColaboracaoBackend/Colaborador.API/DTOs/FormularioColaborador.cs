using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Endereco;
using System;

namespace Colaborador.API.DTOs
{
    public class FormularioColaborador
    {
        public string nome { get; set; }
        public DateTime? data_nascimento { get; set; }
        public string cpf { get; set; }
        public string rg { get; set; }
        public string estado_civil { get; set; }
        public string escolaridade { get; set; }
        public string etnia { get; set; }
        public string genero { get; set; }
        public string orientacao_sexual { get; set; }
        public bool? pessoa_refugiada { get; set; }
        public string email { get; set; }
        public string emailAlternativo { get; set; }
        public string celular { get; set; }
        public string passaporte { get; set; }
        public EnderecoDTO endereco { get; set; }
        public ColaboradorSaudeDTO saude { get; set; }
        public string documentoColaborador { get; set; }
    }
}