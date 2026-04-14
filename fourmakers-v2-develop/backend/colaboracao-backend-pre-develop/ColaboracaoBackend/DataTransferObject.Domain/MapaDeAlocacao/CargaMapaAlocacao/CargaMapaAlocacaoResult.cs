using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao
{
    public class CargaMapaAlocacaoResult : StatusResult
    {
        public int MapaAlocacaoInseridos { get; set; }
        public int ColaboradoresInseridos { get; set; }
        public int ColaboradoresExistentes { get; set; }

        public List<Result> CargaInserida { get; set; }
        public List<ConflitoAlocacao> ConflitoAlocacao { get; set; }
        public Response Response { get; set; }
    }
}