using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Diretoria;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class ListarDiretoriaDosColaboradoresResult : StatusResult
    {
        public ListarDiretoriaDosColaboradoresResult()
        {
            DiretoriaColaborador = new List<DiretoriaColaboradorDTO>();
        }

        public List<DiretoriaColaboradorDTO> DiretoriaColaborador { get; set; }
    }
}