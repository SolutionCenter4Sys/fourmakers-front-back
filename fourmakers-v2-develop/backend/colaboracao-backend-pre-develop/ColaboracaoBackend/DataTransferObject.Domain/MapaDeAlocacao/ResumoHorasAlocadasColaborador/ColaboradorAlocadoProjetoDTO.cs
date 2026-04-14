using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ColaboradorAlocadoProjetoDTO
    {
        public long CodigoProjeto { get; set; }
        public string Cpf { get; set; }
        public int? CodigoTBD { get; set; }
        public string NomeColaborador { get; set; }
        public string CodigoColaboradorExterno { get; set; }
        public List<ForcaMensalColaboradorProjetoDTO> forcaMensalColaboradorProjetoDTOs { get; set; }
    }
}