using System.Collections.Generic;

namespace DataTransferObject.Domain.Usuario.Permissao
{
    public class UsuarioGrupoAcessoFuncionalidadeSistemaDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public IEnumerable<GrupoAcessoFuncionalidadeSistemaDTO> GrupoAcesso { get; set; }
    }
}