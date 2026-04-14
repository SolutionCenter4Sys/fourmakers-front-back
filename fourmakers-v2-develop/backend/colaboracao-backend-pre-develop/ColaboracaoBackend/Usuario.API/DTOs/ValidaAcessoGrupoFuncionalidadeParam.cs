using DataTransferObject.Domain.Usuario;

namespace Usuario.API.DTOs
{
    public class ValidaAcessoGrupoFuncionalidadeParam
    {
        public string cpf { get; set; }
        public FuncionalidadeSistemaEnum funcionalidadeSistemaEnum { get; set; }
    }
}