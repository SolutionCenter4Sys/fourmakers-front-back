namespace DataTransferObject.Domain.Marketing.Comunicacao
{
    public class ColaboradorResumoDTO
    {
        public string CodigoColaboradorInterno { get; set; }
        public string Email { get; set; }
        public string NomeCompleto { get; set; }
        /// <summary>Quando autoria_tipo=alternativo e a org tem nome configurado, retorna o nome para exibição (ex.: Foursys). Caso contrário, null.</summary>
        public string NomeAutorAlternativo { get; set; }
        public string CodDepartamento { get; set; }
        public string Departamento { get; set; }
        /// <summary>Código do modelo de contratação na org (tb_colaborador_org.codigo_modelo_contratacao).</summary>
        public string CodigoModeloContratacao { get; set; }
        /// <summary>Descrição do modelo (tb_modelo_contratacao_org ou fallback em tb_colaborador_org).</summary>
        public string ModeloContratacao { get; set; }
        public string CodDiretoria { get; set; }
        public string Diretoria { get; set; }
        public string UrlFoto { get; set; }
        /// <summary>Quando autoria_tipo=alternativo e a org tem url configurada, retorna a URL da foto para exibição. Caso contrário, null.</summary>
        public string UrlFotoAlternativa { get; set; }
    }
}
