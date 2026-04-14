using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class MergeItemPerfilDTO
    {
        public MergeItemPerfilDTO()
        {
            itens = new List<AdicionarRemoverItemDTO>();
        }

        public string cpf { get; set; }
        public List<AdicionarRemoverItemDTO> itens { get; set; }
    }
}