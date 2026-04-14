using System;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Linha auxiliar para carregar destinatários do mural (repositório).
    /// </summary>
    public class Feedback360MuralDestinatarioRowDTO
    {
        public Guid Feedback360Id { get; set; }
        public Guid CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }
        public string ImagemPathRelativa { get; set; }
    }
}
