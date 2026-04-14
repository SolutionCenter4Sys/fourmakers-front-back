using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Grupo
{
    public class AtualizarGrupoRequestDTO : InserirGrupoRequestDTO
    {
        public Guid GrupoId { get; set; }
    }
}
