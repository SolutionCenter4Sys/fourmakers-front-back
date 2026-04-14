using System.Collections.Generic;

namespace DataTransferObject.Domain.Usuario.Permissao
{
    public class CriarGrupoAcessoInput
    {
        public string Descricao { get; set; }
        public List<GrupoAcessoClienteInput> Clientes { get; set; }
    }
}

