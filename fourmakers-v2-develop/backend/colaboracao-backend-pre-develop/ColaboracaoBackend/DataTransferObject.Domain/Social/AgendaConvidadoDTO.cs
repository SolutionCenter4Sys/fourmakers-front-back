using System;

namespace DataTransferObject.Domain.Social
{
    public class ConvidarParaAgendaRequestDTO
    {
        public int TbAgendasComerciaisId { get; set; }

        /// <summary>
        /// Um ou mais códigos (CPF/CNPJ ou identificador do gestor externo), separados por vírgula.
        /// </summary>
        public string CodigoColaboradorInternoExterno { get; set; }
    }

    public class AgendaConvidadoDTO
    {
        public int Id { get; set; }
        public int TbAgendasComerciaisId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string CodigoColaboradorInternoExterno { get; set; }
        public int? TipoCodigo { get; set; }
        public int TbStatusAppId { get; set; }
        public DateTime? DataConvite { get; set; }
        public DateTime? DataResposta { get; set; }
    }

    public class AgendaConvidadoAtualizacaoDTO
    {
        public int TbAgendasComerciaisId { get; set; }
        public string CodigoColaboradorExterno { get; set; }
        /// <summary>1 = aceito, 2 = recusado (únicos valores permitidos neste endpoint).</summary>
        public int TbStatusAppId { get; set; }
    }
}
