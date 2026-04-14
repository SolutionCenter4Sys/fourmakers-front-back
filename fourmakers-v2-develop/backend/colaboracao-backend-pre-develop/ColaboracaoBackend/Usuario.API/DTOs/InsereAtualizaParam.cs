using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.Usuario;

namespace Usuario.API.DTOs
{
    public class InsereAtualizaParam
    {
        public UsuarioColaboradorDTO usuario { get; set; }
        public ColaboradorDTO colaborador { get; set; }
        public EnderecoDTO endereco { get; set; }
        public string diretoria { get; set; }
    }
}