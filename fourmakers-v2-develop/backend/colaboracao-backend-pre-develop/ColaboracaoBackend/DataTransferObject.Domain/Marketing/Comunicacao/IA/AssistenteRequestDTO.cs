namespace DataTransferObject.Domain.Marketing.Comunicacao.IA
{
    public class AssistenteRequestDTO
    {
        /// <summary>Texto a ser refatorado.</summary>
        public string Texto { get; set; }

        /// <summary>Modo de refatoração (melhorar_texto, mais_profissional, resumo, sumario_executivo, adicionar_topicos, expandir_conteudo).</summary>
        public ModoRefatoracaoTextoEnum Modo { get; set; }

        /// <summary>Se deve aplicar negrito no resultado.</summary>
        public bool Negrito { get; set; }
    }
}
