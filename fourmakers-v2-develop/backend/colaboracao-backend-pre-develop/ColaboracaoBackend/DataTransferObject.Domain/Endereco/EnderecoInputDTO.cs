using DataTransferObject.Domain.Arquivo;

namespace DataTransferObject.Domain.Endereco;

public class EnderecoInputDTO : EnderecoDTO
{
    public Base64DTO? ComprovanteResidenciaBase64 { get; set; }
}