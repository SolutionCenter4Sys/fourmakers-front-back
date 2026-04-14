using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ConsultarColaboradorProjetoResult : StatusResult
    {
        //por Colaborador que possui o projeto se não for passado o cpf
        public List<ConsultarColaboradorProjetoDTO> ConsultarColaboradorProjetoDTO { get; set; }
    }
}