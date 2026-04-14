using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao
{
    public class Response
    {
        public int cursor { get; set; }
        public List<Result> results { get; set; }
        public int count { get; set; }
        public int remaining { get; set; }
    }
}