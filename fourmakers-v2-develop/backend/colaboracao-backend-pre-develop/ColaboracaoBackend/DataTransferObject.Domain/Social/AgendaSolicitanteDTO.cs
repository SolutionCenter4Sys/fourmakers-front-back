using System;

namespace DataTransferObject.Domain.Social
{
    public class AgendaSolicitanteDTO
    {
        public int Id { get; set; }
        public int TbAgendasComerciaisId { get; set; }
        /// <summary>Preenchido na leitura via join com tb_colaborador ou tb_gestor_externo.</summary>
        public string Nome { get; set; }
        public string Email { get; set; }
        public string CodigoColaboradorInternoExterno { get; set; }
        public string CodColaboradorInternoCriador { get; set; }
        public int? TipoCodigo { get; set; }
        public int TbStatusAppId { get; set; }
        public DateTime? DataSolicitacao { get; set; }
        public DateTime? DataResposta { get; set; }
    }

    /// <summary>
    /// Atualização de status da solicitação (sem nome/e-mail persistidos). Chave: agenda + código do participante.
    /// </summary>
    public class AgendaSolicitanteAtualizacaoDTO
    {
        public int TbAgendasComerciaisId { get; set; }
        public string CodigoColaboradorExterno { get; set; }
        /// <summary>1 = aceito, 2 = recusado (únicos valores permitidos neste endpoint).</summary>
        public int TbStatusAppId { get; set; }
    }
}
