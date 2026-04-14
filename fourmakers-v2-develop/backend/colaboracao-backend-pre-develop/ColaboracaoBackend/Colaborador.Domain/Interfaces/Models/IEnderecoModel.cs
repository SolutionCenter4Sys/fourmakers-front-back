using Colaborador.Domain.Interfaces.Factorys;

namespace Colaborador.Domain.Interfaces.Models
{
    public interface IEnderecoModel
    {
        long Id { get; set; }
        string Cep { get; set; }
        string Endereco { get; set; }
        string Complemento { get; set; }
        int? Numero { get; set; }
        string Bairro { get; set; }
        string Cidade { get; set; }
        string Estado { get; set; }
        string ComQuemMora { get; set; }
        string InternacionalLinhaUm { get; set; }
        string InternacionalLinhaDois { get; set; }

        IEnderecoModel GetByCpfColaborador(string cpf, IEnderecoDomainFactory enderecoDomainFactory);
        IEnderecoModel SaveModel();
        IEnderecoModel UpdateModel();
    }
}