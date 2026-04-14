using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Social
{
    public class ColaboradorGestorFeedbackDTO
    {
        public Guid CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }
    }

    public class ListagemColaboradoresGestorFeedbackDTO
    {
        public List<ColaboradorGestorFeedbackDTO> Colaboradores { get; set; }
    }
}
