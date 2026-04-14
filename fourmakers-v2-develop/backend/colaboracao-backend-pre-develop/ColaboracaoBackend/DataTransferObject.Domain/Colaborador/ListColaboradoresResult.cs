using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class ListColaboradoresResult : StatusResult
    {
        public ListColaboradoresResult()
        {
            Colaboradores = new List<SimpleColaboradorDTO>();
        }
        public List<SimpleColaboradorDTO> Colaboradores { get; set; }
    }
}