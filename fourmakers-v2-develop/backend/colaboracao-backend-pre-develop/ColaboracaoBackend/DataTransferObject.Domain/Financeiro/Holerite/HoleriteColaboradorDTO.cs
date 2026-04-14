using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Holerite
{
    public class HoleriteColaboradorDTO
    {   
        /// <summary>
        /// ID do item de lote (chave primária composta)
        /// </summary>
        public string TbItemLoteId { get; set; }

        /// <summary>
        /// Código interno do colaborador (chave primária composta)
        /// </summary>
        public string CodigoInternoColaborador { get; set; }

        /// <summary>
        /// Objeto do holerite
        /// </summary>
        public HoleriteWrapperDTO ObjetoHolerite { get; set; }

        /// <summary>
        /// Objeto do holerite em formato JSON
        /// </summary>
        [JsonIgnore]
        public string ObjetoHoleriteString { get; set; }

        /// <summary>
        /// Competência no formato MM/YYYY
        /// </summary>
        public string Competencia { get; set; }

        /// <summary>
        /// CNPJ da empresa
        /// </summary>
        public string Cnpj { get; set; }
        
        /// <summary>
        /// Horas Adicionais
        /// </summary>
        public string TotalHorasExtras { get; set; }
        
        /// <summary>
        /// Dias Trabalhados
        /// </summary>
        public int DiasTrabalhados { get; set; }
        
        /// <summary>
        /// Se o Documento foi Assinado
        /// </summary>
        public bool Assinado { get; set; }
        
        /// <summary>
        /// Quando o Documento foi Assinado
        /// </summary>
        public DateTime? AssinadoEm { get; set; }

        /// <summary>
        /// Caminho do arquivo PDF do holerite
        /// </summary>
        public string HoleritePdf { get; set; }

        /// <summary>
        /// Se o holerite é de adiantamento
        /// </summary>
        public bool? Adiantamento { get; set; }
        /// <summary>
        /// Se o holerite é de Férias
        /// </summary>
        public bool? Ferias { get; set; }
        /// <summary>
        /// Se o holerite é de Décimo terceiro
        /// </summary>
        public bool? DecimoTerceiro { get; set; }
        /// <summary>
        /// Se o holerite é de Adiantamento do Décimo terceiro
        /// </summary>
        public bool? DecimoTerceiroAdiantamento { get; set; }
        /// <summary>
        /// Se o holerite é de Adiantamento do Décimo terceiro
        /// </summary>
        public bool? InformeDeRendimentos { get; set; }
    }
   
}


