using System.Collections.Generic;

namespace DataTransferObject.Domain.Usuario.Permissao
{
    public class EditarGrupoAcessoInput
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public List<GrupoAcessoClienteInput> Clientes { get; set; }
    }
}

