namespace DataTransferObject.Domain.Vaga
{
    /// <summary>Domínio tb_nivel_vaga (nível da vaga: Júnior … Especialista).</summary>
    public class NivelVagaDTO
    {
        public string Id { get; set; }
        public string Descricao { get; set; }
        public int Codigo { get; set; }
    }
}
