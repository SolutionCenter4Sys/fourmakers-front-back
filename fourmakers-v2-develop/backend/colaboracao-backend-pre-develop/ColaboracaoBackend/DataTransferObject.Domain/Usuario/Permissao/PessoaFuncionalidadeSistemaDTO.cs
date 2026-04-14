using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Usuario.Permissao
{
    public class PessoaFuncionalidadeSistemaDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public long UsuarioId { get; set; }
        public DateTime? DataInsercaoGrupo { get; set; }
        public List<FuncionalidadeSistemaDTO> FuncionalidadesSistema { get; set; }
    }
}

