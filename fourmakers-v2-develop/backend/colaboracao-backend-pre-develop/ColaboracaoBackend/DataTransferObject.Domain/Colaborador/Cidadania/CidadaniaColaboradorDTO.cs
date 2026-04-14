using System;

namespace DataTransferObject.Domain.Colaborador.Cidadania
{
    public class CidadaniaColaboradorDTO
    {
        public int Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public int CidadaniaId { get; set; }
        public int CidadaniaStatusId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string CidadaniaDescricao { get; set; }
        public string StatusDescricao { get; set; }
    }
}