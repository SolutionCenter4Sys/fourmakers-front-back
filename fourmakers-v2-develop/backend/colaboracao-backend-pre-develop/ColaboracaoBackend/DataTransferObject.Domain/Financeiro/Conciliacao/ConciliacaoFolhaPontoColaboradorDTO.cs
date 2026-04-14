using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Conciliacao
{
    public class ConciliacaoFolhaPontoColaboradorDTO
    {
        /// <summary>
        /// ID único da conciliação de folha ponto do colaborador
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Indica se houve divergência (0 = Não, 1 = Sim)
        /// </summary>
        public bool HouveDivergencia { get; set; }

        /// <summary>
        /// Número de divergências encontradas
        /// </summary>
        public int NumeroDivergencias { get; set; }

        /// <summary>
        /// ID do item de lote relacionado
        /// </summary>
        [JsonIgnore]
        public string TbItemLoteId { get; set; }

        /// <summary>
        /// ID da conciliação de folha ponto relacionada
        /// </summary>
        [JsonIgnore]
        public string TbConciliacaoFolhaPontoId { get; set; }

        /// <summary>
        /// Código interno do colaborador
        /// </summary>
        [JsonIgnore]
        public string CodigoInternoColaborador { get; set; }

        /// <summary>
        /// Divergências encontradas
        /// </summary>
        public List<ConciliacaoFolhaPontoDivergenciaDTO> Divergencias { get; set; }
    }
} 