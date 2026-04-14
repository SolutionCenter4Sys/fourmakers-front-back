using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento.FolhaPonto
{
    public class FolhaPontoColaboradorDTO
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
        /// Objeto da folha de ponto
        /// </summary>
        public RelatorioPontoRootDTO ObjetoFolhaPonto { get; set; }

        /// <summary>
        /// Objeto da folha de ponto em formato JSON
        /// </summary>
        [JsonIgnore]
        public string ObjetoFolhaPontoString { get; set; }

        /// <summary>
        /// Competência no formato MM/YYYY
        /// </summary>
        public string Competencia { get; set; }

        /// <summary>
        /// CNPJ da empresa
        /// </summary>
        public string Cnpj { get; set; }

        /// <summary>
        /// Caminho do arquivo PDF da folha de ponto
        /// </summary>
        public string FolhaPdf { get; set; }
    }
} 